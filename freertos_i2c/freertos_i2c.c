/*
* Copyright (c) 2015, Freescale Semiconductor, Inc.
* All rights reserved.
*
* Redistribution and use in source and binary forms, with or without modification,
* are permitted provided that the following conditions are met:
*
* o Redistributions of source code must retain the above copyright notice, this list
*   of conditions and the following disclaimer.
*
* o Redistributions in binary form must reproduce the above copyright notice, this
*   list of conditions and the following disclaimer in the documentation and/or
*   other materials provided with the distribution.
*
* o Neither the name of Freescale Semiconductor, Inc. nor the names of its
*   contributors may be used to endorse or promote products derived from this
*   software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
* ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR
* ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
* ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/*  Standard C Included Files */
#include <string.h>

/* FreeRTOS kernel includes. */
#include "FreeRTOS.h"
#include "task.h"
#include "queue.h"
#include "timers.h"
#include "semphr.h"

/*  SDK Included Files */
#include "board.h"
#include "fsl_debug_console.h"
#include "fsl_i2c.h"
#include "fsl_i2c_freertos.h"

#include "pin_mux.h"
#include "clock_config.h"
#include "common.h"
#include "adc.h"
#include "diag.h"
#include "flash.h"
#include "fsl_wdog.h"
#include "fsl_rcm.h"
#include "wdog.h"
#include "algo.h"
#include "event.h"
#include "timer.h"


uint32_t g_errors = 0;

int(*systemFunctions[100])(uint8_t *buffer, int size) = { 0 };
TaskHandle_t kjTaskHandle;
TaskHandle_t mainTaskHandle;

uint8_t InputDataBuffer[256];

extern uint8_t inBuff[I2C_IN_BUF_SIZE];
extern uint8_t inBuffer_writeIndex;
extern uint8_t inBuffer_readIndex;

extern uint8_t outBuff[I2C_OUT_BUF_SIZE];
extern uint8_t outBuffer_writeIndex;
extern uint8_t outBuffer_readIndex;
extern int g_startCooking;
extern uint32_t g_overallTimer;
extern int(*algoFunctions[4])();
extern uint16_t cookingHeatList[MAX_HEAT_LIST];
extern CookingResponse  g_cookingResponse;
extern volatile uint8_t g_pause;
extern volatile uint32_t g_timeMilliseconds;
extern TimerHandle_t SwOverallTimerHandle;
volatile uint8_t g_algoIndex = 0;

volatile int 	g_cookingIndex = 0;
uint8_t receiveBuffer[4] = {0};
uint8_t sendBuffer[4] = {0};



/* The software timer period. */
#define SW_TIMER_PERIOD_MS (1 / portTICK_PERIOD_MS)
/*******************************************************************************
 * Prototypes
 ******************************************************************************/

#define LED1_INIT() LED_RED_INIT(LOGIC_LED_OFF)
#define LED1_ON() LED_RED_ON()
#define LED1_OFF() LED_RED_OFF()

#define LED2_INIT() LED_GREEN_INIT(LOGIC_LED_OFF)
#define LED2_ON() LED_GREEN_ON()
#define LED2_OFF() LED_GREEN_OFF()


/*******************************************************************************
 * Definitions
 ******************************************************************************/
#define EXAMPLE_I2C_MASTER_BASE I2C0_BASE
#define EXAMPLE_I2C_SLAVE_BASE I2C1_BASE

#define EXAMPLE_I2C_MASTER ((I2C_Type *)EXAMPLE_I2C_MASTER_BASE)
#define EXAMPLE_I2C_SLAVE ((I2C_Type *)EXAMPLE_I2C_SLAVE_BASE)

#if (EXAMPLE_I2C_MASTER_BASE == I2C0_BASE)
#define I2C_MASTER_CLK_SRC (I2C0_CLK_SRC)
#elif(EXAMPLE_I2C_MASTER_BASE == I2C1_BASE)
#define I2C_MASTER_CLK_SRC (I2C1_CLK_SRC)
#elif(EXAMPLE_I2C_MASTER_BASE == I2C2_BASE)
#define I2C_MASTER_CLK_SRC (I2C2_CLK_SRC)
#else
#error Should define the I2C_MASTER_CLK_SRC!
#endif

#define I2C_MASTER_SLAVE_ADDR_7BIT (0x7FU)
#define I2C_DATA_LENGTH (32)  /* MAX is 256 */

/*******************************************************************************
 * Prototypes
 ******************************************************************************/

/*******************************************************************************
 * Variables
 ******************************************************************************/

uint8_t g_slave_buff[I2C_DATA_LENGTH];



i2c_slave_handle_t g_s_handle;

/*******************************************************************************
 * Definitions
 ******************************************************************************/
/* Task priorities. */
#define main_task_PRIORITY (configMAX_PRIORITIES - 1)
#define kjmonitor_task_PRIORITY (configMAX_PRIORITIES - 1)

/*******************************************************************************
 * Prototypes
 ******************************************************************************/

static void main_task(void *pvParameters);
static void kjmonitor_task(void *pvParameters);


void i2c_slave_callback(I2C_Type *base, i2c_slave_transfer_t *xfer, void *userData)
{
    //callback_message_t *cb_msg = (callback_message_t *)userData;
    //BaseType_t reschedule;

    switch (xfer->event)
    {
        /*  Transmit request */
        case kI2C_SlaveTransmitEvent:
            /*  Update information for transmit process */
            xfer->data = g_slave_buff;
            xfer->dataSize = I2C_DATA_LENGTH;
            break;

        /*  Receive request */
        case kI2C_SlaveReceiveEvent:
            /*  Update information for received process */
            xfer->data = g_slave_buff;
            xfer->dataSize = I2C_DATA_LENGTH;
            break;

        /*  Transfer done */
        case kI2C_SlaveCompletionEvent:
            //cb_msg->async_status = xfer->completionStatus;
            //xSemaphoreGiveFromISR(cb_msg->sem, &reschedule);
            //portYIELD_FROM_ISR(reschedule);
        	{
             	MsgHandler();
            }
        break;
        default:
            break;
    }
}


int main(void)
{
    /* Init board hardware. */
    BOARD_InitPins();
    BOARD_BootClockRUN();
    BOARD_InitDebugConsole();
    i2c_slave_config_t slaveConfig;

    InitializeSystem();
    InitializeAlgoFunctions();

    /* Init using Led in Demo app */
    LED1_INIT();
    LED2_INIT();

    PRINTF("portTICK_PERIOD_MS = %d\r\n" , portTICK_PERIOD_MS);

    /* Set up I2C slave */
    /*
	* slaveConfig.addressingMode = kI2C_Address7bit;
	* slaveConfig.enableGeneralCall = false;
	* slaveConfig.enableWakeUp = false;
	* slaveConfig.enableHighDrive = false;
	* slaveConfig.enableBaudRateCtl = false;
	* slaveConfig.enableSlave = true;
	*/
   I2C_SlaveGetDefaultConfig(&slaveConfig);

   slaveConfig.addressingMode = kI2C_Address7bit;
   slaveConfig.slaveAddress = I2C_MASTER_SLAVE_ADDR_7BIT;
   slaveConfig.upperAddress = 0; /*  not used for this example */

   I2C_SlaveInit(EXAMPLE_I2C_SLAVE, &slaveConfig);


   memset(&g_s_handle, 0, sizeof(g_s_handle));

   I2C_SlaveTransferCreateHandle(EXAMPLE_I2C_SLAVE, &g_s_handle, i2c_slave_callback, NULL);
   I2C_SlaveTransferNonBlocking(EXAMPLE_I2C_SLAVE, &g_s_handle, kI2C_SlaveCompletionEvent);

   #if 0
       if (xTaskCreate(master_task, "Master_task", configMINIMAL_STACK_SIZE + 124, NULL, master_task_PRIORITY, NULL) !=
           pdPASS)
       {
           PRINTF("Failed to create master task");
           vTaskSuspend(NULL);
       }
   #endif

   /*  Set i2c slave interrupt priority higher. */
   #if (EXAMPLE_I2C_SLAVE_BASE == LPI2C0_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(LPI2C0_IRQn, 5);
	   else
		   NVIC_SetPriority(LPI2C0_IRQn, 2);
   #elif(EXAMPLE_I2C_SLAVE_BASE == LPI2C1_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(LPI2C1_IRQn, 5);
	   else
		   NVIC_SetPriority(LPI2C1_IRQn, 2);
   #elif(EXAMPLE_I2C_SLAVE_BASE == LPI2C2_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(LPI2C2_IRQn, 5);
	   else
		   NVIC_SetPriority(LPI2C2_IRQn, 2);
   #elif(EXAMPLE_I2C_SLAVE_BASE == I2C0_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(I2C0_IRQn, 5);
	   else
		   NVIC_SetPriority(I2C0_IRQn, 2);
   #elif(EXAMPLE_I2C_SLAVE_BASE == I2C1_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(I2C1_IRQn, 5);
	   else
		   NVIC_SetPriority(I2C1_IRQn, 2);
   #elif(EXAMPLE_I2C_SLAVE_BASE == I2C2_BASE)
	   if (__CORTEX_M >= 0x03)
		   NVIC_SetPriority(I2C2_IRQn, 5);
	   else
		   NVIC_SetPriority(I2C2_IRQn, 2);
   #endif

    InitADCInOneShotMode();
    flash_init();


    calreader_Init();
    //wdog_init();
    PRINTF("Ready\r\n");

    if (xTaskCreate(main_task, "main_task", configMINIMAL_STACK_SIZE + 60, NULL, main_task_PRIORITY, &mainTaskHandle) != pdPASS)
    {
        PRINTF("Failed to create slave task");
    }

    if (xTaskCreate(kjmonitor_task, "kjmonitor_task", configMINIMAL_STACK_SIZE + 60, NULL, kjmonitor_task_PRIORITY, &kjTaskHandle) != pdPASS)
	{
		PRINTF("Failed to create slave task");
	}
    vTaskSuspend(kjTaskHandle);

    vTaskStartScheduler();
    while(true){}

}


typedef struct _callback_message_t
{
    status_t async_status;
    SemaphoreHandle_t sem;
} callback_message_t;


static void main_task(void *pvParameters)
{
	//TimerHandle_t SwTimerHandle = NULL;

	g_cookingResponse.opcode = OPC_READ_STATUS;
	g_cookingResponse.running = g_startCooking;
	g_cookingResponse.kj = 0;
	g_pause = 0;



    while (true)
    {
    	if (g_startCooking == 1)
    	{
    		if (g_overallTimer > 0)
    		{
    			PRINTF("g_overallTimer = %d\r\n", g_overallTimer);
    			CreateOverallCookingTimer(g_overallTimer);
    			xTimerStart(SwOverallTimerHandle, 0);
    			g_overallTimer = 0;
    		}

    		g_cookingResponse.running = g_startCooking;
    		//PRINTF("Main Task g_cookingIndex = %d    g_algoIndex = %d \r\n" , g_cookingIndex, g_algoIndex);
    		uint16_t algoType = cookingHeatList[g_cookingIndex];
    		if (algoType == 0xF1F2)
    		{
    			PRINTF("Cooking script list finished\r\n");
    			g_startCooking  = 0;
    			continue;
    		}
    		//PRINTF("algoType = %d\r\n" , algoType);
    		g_cookingResponse.rowIndex = g_algoIndex;
    		g_cookingResponse.cycleEndded = 0;
    		g_cookingResponse.drCount = 0;
    		algoFunctions[algoType](g_algoIndex);
    		//PRINTF("ALGO FUNCTION ENDDED\r\n");
    		g_algoIndex++;
    	} else {
    		g_cookingResponse.running = 0;
    		timer_stop();
    	}
    	vTaskDelay(10);

    	//xTimerStop(SwTimerHandle, 0);
    	//PRINTF("g_timeMilliseconds %d\r\n" , g_timeMilliseconds);
    }


#if 0

    /* Wait for transfer to finish */
    xSemaphoreTake(cb_msg.sem, portMAX_DELAY);

    if (cb_msg.async_status == kStatus_Success)
    {
        PRINTF("I2C slave transfer completed successfully. \r\n\r\n");
    }
    else
    {
        PRINTF("I2C slave transfer completed with error. \r\n\r\n");
    }
    /* Wait for transfer to finish */
    xSemaphoreTake(cb_msg.sem, portMAX_DELAY);
#endif

    vTaskSuspend(NULL);
}

static void kjmonitor_task(void *pvParameters)
{


	PRINTF("kjmonitor_task\r\n");
	while (true)
	{
		vTaskDelay(1000);
		PRINTF("kjmonitor_task\n\r");
	}

}

void sendResponse(unsigned short opcode, unsigned char res)
{
	Response _response;
	_response.opcode = opcode;
	_response.res = res;
	_response.reserved = 0;


	uint8_t *p = (uint8_t *)&_response;
	WriteI2COutputData(sizeof(Response), p);
}

int isOutFifoFull()
{

	if (GetOutFifoSize() == (I2C_OUT_BUF_SIZE - 1))
	{
		return 1;
	} else
	return 0;
}
int isInFifoFull()
{

	if (GetInFifoSize() == (I2C_IN_BUF_SIZE - 1))
	{
		return 1;
	} else
	return 0;
}


void halt(int code)
{
	PRINTF("halt code: %d\r\n" , code);
	while (1){}
}



int GetInFifoSize()
{
	int size = 0;
	if (inBuffer_writeIndex >= inBuffer_readIndex)
	{
		size = inBuffer_writeIndex - inBuffer_readIndex;
	} else {
		size = (I2C_IN_BUF_SIZE - inBuffer_readIndex + inBuffer_writeIndex);
	}
	return size;
}


void IncInFifo(int size)
{
	inBuffer_readIndex = (inBuffer_readIndex + size) % I2C_IN_BUF_SIZE;
}


void MsgHandler()
{

	uint8_t headerBuffer[6];

	int size = GetInFifoSize();

	int res;
	if (size >= 6)
	{

		for (int i = 0 ; i < 6 ;i++)
		{
			headerBuffer[i] = inBuff[inBuffer_readIndex];
			inBuffer_readIndex = (inBuffer_readIndex + 1) % I2C_IN_BUF_SIZE;
		}
		//PRINTF("size %d\r\n" , size);
		msgHeader *p = (msgHeader *)&headerBuffer;
		if (p->size > 0)
		{
			 while (GetInFifoSize() < p->size)
			 {

			 }
			 for (int i = 0 ; i < p->size; i++)
			 {
				 InputDataBuffer[i] = inBuff[inBuffer_readIndex];
				 inBuffer_readIndex = (inBuffer_readIndex + 1) % I2C_IN_BUF_SIZE;
			 }
			 res = systemFunctions[p->opcode](InputDataBuffer,p->size);
			 if (res != 0)
				 sendResponse(p->opcode, res);

		} else {
			 res = systemFunctions[p->opcode](NULL,p->size);
			 if (res != 0)
				 sendResponse(p->opcode, res);
		}
	}
}

