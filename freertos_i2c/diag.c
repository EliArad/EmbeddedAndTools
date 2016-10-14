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
#include "fsl_wdog.h"

#include "pin_mux.h"
#include "clock_config.h"
#include "common.h"
#include "adc.h"
#include "diag.h"
#include "flash.h"
#include "algo.h"


extern uint8_t inBuff[I2C_IN_BUF_SIZE];
extern uint8_t inBuffer_writeIndex;
extern uint8_t inBuffer_readIndex;
extern uint32_t destAdrss;
extern uint8_t outBuff[I2C_OUT_BUF_SIZE];
extern uint8_t outBuffer_writeIndex;
extern uint8_t outBuffer_readIndex;
uint32_t  g_location;
volatile uint8_t g_pause = 0;
extern int(*systemFunctions[100])(uint8_t *buffer, int size);
int32_t g_FlashWriteChunkSize = 0;
uint32_t g_flashWriteIndex = 0;
uint32_t g_flashReadIndex = 0;
int32_t g_FlashReadChunkSize = 0;
int m_algoBufIndex = 0;
uint16_t cookingHeatList[MAX_HEAT_LIST];
extern TimerHandle_t SwOverallTimerHandle;
uint8_t g_startCooking = 0;
extern TaskHandle_t mainTaskHandle;
extern CookingResponse  g_cookingResponse;
extern volatile uint8_t g_algoIndex;
extern volatile int 	g_cookingIndex;
uint32_t g_stayAliveCounter = 0;
uint8_t g_useInternalClockForDisplay = 1;
uint32_t g_overallTimer = 0;

void InitializeSystem()
{
	systemFunctions[OPC_SET_FREQUENCY] = &setFrequency;
	systemFunctions[OPC_SET_MAGNITUDE] = &setMagnitude;
	systemFunctions[OPC_READ_DETECTOR] = &readADC;


	systemFunctions[OPC_ADDALGO] = &AddAlgo;
	systemFunctions[OPC_APPENDALGO] = &AppendAlgo;
	systemFunctions[OPC_CLEARALGOS] = &ClearAlgos;
	systemFunctions[OPC_START_COOKING] = &StartCooking;
	systemFunctions[OPC_STOP_COOKING] = &StopCooking;
	systemFunctions[OPC_PAUSE_COOKING] = &PauseCooking;


	systemFunctions[OPC_WRITE_TOPAZ_INFO] = &WriteFlashTopazInfo;
	systemFunctions[OPC_READ_TOPAZ_INFO] = &ReadFlashTopazInfo;
	systemFunctions[OPC_WRITE_CAL_AB_LINEAR] = &WriteFlashCalAbLinear;
	systemFunctions[OPC_WRITE_DISHES] = &WriteFlashCalAbLinear;
	systemFunctions[OPC_READ_CAL_AB_LINEAR] = &ReadFlashCalAbLinear;
	systemFunctions[OPC_READ_DISHES] = &ReadFlashDishes;



	systemFunctions[OPC_READ_STATUS] = &ReadStatus;

	systemFunctions[OPC_KEEP_ALIVE] = &KeepAlive;

	systemFunctions[OPC_WRITE_FLASH_CHUNK] = &WriteFlashChunk;
	systemFunctions[OPC_READ_FLASH_CHUNK] = &ReadFlashChunk;


	systemFunctions[OPC_GET_DR] = &UserGetDR;

	systemFunctions[OPC_USE_INTERNAL_CLOCK] = &SetInternalClockForDisplay;

	systemFunctions[OPC_GET_COOKING_DATA] = &GetCookingData;





}


int setFrequency(uint8_t *buffer, int size)
{

	FrequencyData *p = (FrequencyData *)buffer;

	//PRINTF("setFrequency:  %d\r\n" , p->frequency);
	return 1;
}
int setMagnitude(uint8_t *buffer, int size)
{
	//PRINTF("setMagnitude: \r\n");
	return 1;
}

int readADC(uint8_t *buffer, int size)
{

	uint8_t *p;
	// read real adc values here
	ADCResponse _response;
	_response.opcode = 0;


	uint32_t f = GetADCOnsShotValue();

	_response.forward = f;

	uint32_t r = GetADCOnsShotValue();
	_response.reflected = r;

	_response.opcode = OPC_READ_DETECTOR;

	p = (uint8_t *)&_response;

	WriteI2COutputData(sizeof(ADCResponse), p);

	return 0;
}

void WriteI2COutputData(int size, uint8_t *p)
{

	if (isOutFifoFull())
	{
		halt(1001);
	}

	for (int i = 0; i < size ; i++)
	{
		if (isOutFifoFull())
		{
			halt(1002);
		}
		outBuff[outBuffer_writeIndex] = *p;
		outBuffer_writeIndex  = (outBuffer_writeIndex + 1) % I2C_OUT_BUF_SIZE;
		p++;
	}
}

int ClearAlgos(uint8_t *buffer, int size)
{
	m_algoBufIndex = 0;
	memset(cookingHeatList , -1, MAX_HEAT_LIST);
	return 1;
}
int AddAlgo(uint8_t *buffer, int size)
{

	m_algoBufIndex = 0;
	return AppendAlgo(buffer, size);

}

int AppendAlgo(uint8_t *buffer, int size)
{

	memcpy((uint16_t *)(cookingHeatList + m_algoBufIndex), (uint16_t *)buffer, size);

	m_algoBufIndex+= (size >> 1);

#if 0
	for(int i = 0; i < m_algoBufIndex; i++)
	{
		PRINTF("cookingHeatList[%d] = 0x%x\r\n" ,i,  cookingHeatList[i]);
	}
#endif

	return 1;
}

int StartCooking(uint8_t *buffer, int size)
{

	if (g_startCooking == 0)
	{
		//PRINTF("StartCooking\r\n");
		g_cookingResponse.clockSeconds = 0;
		g_cookingResponse.clockMinutes = 0;
		g_cookingResponse.clockHours = 0;
		g_cookingIndex = 1;
		g_cookingResponse.kj = 0;

		g_cookingResponse.AlgoClockSeconds = 0;
		g_cookingResponse.drready = 0;

		g_cookingResponse.cycleEndded = 0;
		g_cookingResponse.drCount = 0;

		g_cookingResponse.error1 = 0;
		g_cookingResponse.error2 = 0;
		g_cookingResponse.error3 = 0;

		g_algoIndex = 0;
		g_startCooking = 1;

		StartCookingParams *p = (StartCookingParams *)buffer;

		g_overallTimer = p->time;

	}

	return 1;
}
int StopCooking(uint8_t *buffer, int size)
{
	_StopCooking();
	return 1;
}
void _StopCooking()
{

	PRINTF("STOP Cooking\r\n");
	g_startCooking = 0;
	g_algoRunning = 0;
	if (SwOverallTimerHandle != NULL)
	{
		xTimerStop(SwOverallTimerHandle, 0);
		DestroyOverallCookingTimer();
	}

}
int PauseCooking(uint8_t *buffer, int size)
{

   g_pause = buffer[0];
   PRINTF("Pause: %d\r\n" , g_pause);

   return 1;
}


int WriteFlashCalAbLinear(uint8_t *buffer, int size)
{
	return 1;
}
int ReadFlashCalAbLinear(uint8_t *buffer, int size)
{
	return 1;
}
int WriteFlashDishes(uint8_t *buffer, int size)
{

	PRINTF("WriteFlashDishes\r\n");

	return 1;

}
int ReadFlashDishes(uint8_t *buffer, int size)
{

	PRINTF("ReadFlashDishes\r\n");
	return 1;

}

int WriteFlashTopazInfo(uint8_t *buffer, int size)
{
	return WriteSerialAndPartNumber(buffer, size);
}
int ReadFlashTopazInfo(uint8_t *buffer, int size)
{
	uint32_t data[1024];
	uint32_t destAdrss = getDestAddress(GENERAL_SECTION_AND_SERIAL);
	PRINTF("destAdrss = 0x%x\r\n" , destAdrss);

	uint32_t *ptr = (uint32_t *)destAdrss;

	memcpy((uint8_t *)data, ptr, 4);

	uint32_t dataSize = data[0];
	PRINTF("Data size: %d\r\n", dataSize);

	ptr = (uint32_t *)(destAdrss + 4);
	memcpy((uint8_t *)data, ptr, dataSize);

	for (int i = 0; i < (dataSize >> 2); i++)
	{
		PRINTF("0x%x\r\n" , data[i]);
	}

	WriteI2COutputData(8, (uint8_t *)data);

	return 1;

}

int ReadStatus(uint8_t *buffer, int size)
{


	WDOG_Type *wdog_base = WDOG;
	WDOG_Refresh(wdog_base);

	uint8_t *p = (uint8_t *)&g_cookingResponse;
	WriteI2COutputData(sizeof(CookingResponse), p);

	g_cookingResponse.cycleEndded = 0;

	return 0;
}


int SetInternalClockForDisplay(uint8_t *buffer, int size)
{
	g_useInternalClockForDisplay = buffer[0];
	return 1;

}

int ReadFlashChunk(uint8_t *buffer, int size)
{


	//PRINTF("ReadFlashChunk = %d\r\n", g_FlashReadChunkSize);
	uint32_t data[2];
	if (g_FlashReadChunkSize == 0)
	{
		g_flashReadIndex = 0;
		g_location = (uint32_t)( *(uint16_t *)buffer);
		uint32_t destAdrss =  getDestAddress(g_location);

		uint32_t *ptr = (uint32_t *)destAdrss;
		memcpy((uint8_t *)data , ptr, 8);

		g_location =  *(uint32_t *)(data);
		g_FlashReadChunkSize = *(uint32_t *)(data + 1);

		//PRINTF("location = %d\r\n" , g_location);
		//PRINTF("g_FlashReadChunkSize = %d\r\n" , g_FlashReadChunkSize);
		WriteI2COutputData(8, (uint8_t *)data);
		g_flashReadIndex+=8;
	} else {
		int sizeToRead = *(uint16_t *)buffer;
		//PRINTF("sizeToRead = %d\r\n" , sizeToRead);
		//PRINTF("location = %d\r\n" , g_location);
		uint32_t destAdrss =  getDestAddress(g_location);
		//PRINTF("g_flashIndex = %d\r\n" , g_flashIndex);
		uint32_t *ptr = (uint32_t *)(destAdrss + g_flashReadIndex);
		memcpy((uint8_t *)data , ptr, sizeToRead);
		WriteI2COutputData(sizeToRead, (uint8_t *)data);

		g_FlashReadChunkSize-=sizeToRead;
		g_flashReadIndex+=sizeToRead;
		//PRINTF("g_FlashReadChunkSize = %d\r\n" , g_FlashReadChunkSize);
		if (g_FlashReadChunkSize <= 0)
		{
			g_flashReadIndex = 0;
			g_FlashReadChunkSize = 0;
			//PRINTF("g_FlashReadChunkSize = %d\r\n" , g_FlashReadChunkSize);
		}
	}

	return 0;
}

int WriteFlashChunk(uint8_t *buffer, int size)
{


	if (g_FlashWriteChunkSize == 0)
	{
		//PRINTF("g_FlashWriteChunkSize = %d\r\n", size);
		g_FlashWriteChunkSize = *(uint32_t *)(buffer + 4);
		g_flashWriteIndex = 0;
		//PRINTF("g_FlashWriteChunkSize = %d\r\n" , g_FlashWriteChunkSize);
		g_location =  *(uint32_t *)(buffer);
		//PRINTF("location = %d\r\n" , g_location);
		EraseSection(g_location);
		Write8BytesToSection(g_location, (uint32_t *)buffer, 0);
		g_flashWriteIndex+=8;
	} else {
		int i = 0;
		while (size > 0)
		{
			g_FlashWriteChunkSize-=8;
			size-=8;
			//PRINTF("g_FlashWriteChunkSize = %d  , g_flashIndex = %d\r\n" , g_FlashWriteChunkSize, g_flashWriteIndex);
			//for (int j = 0; j < 8; j++)
				//PRINTF("%x", buffer[j]);
			//PRINTF("\r\n");
			Write8BytesToSection(g_location, (uint32_t *)(buffer + i), g_flashWriteIndex);
			g_flashWriteIndex+=8;
			i+=8;
			if (g_FlashWriteChunkSize <= 0)
			{
				g_FlashWriteChunkSize = 0;
				g_flashWriteIndex = 0;
				//PRINTF("g_FlashWriteChunkSize = %d\r\n" , g_FlashWriteChunkSize);
			}
		}
	}

	return 1;
}
int KeepAlive(uint8_t *buffer, int size)
{

	WDOG_Type *wdog_base = WDOG;
	WDOG_Refresh(wdog_base);

	uint8_t *p = (uint8_t *)&g_stayAliveCounter;
	WriteI2COutputData(sizeof(CookingResponse), p);


	g_stayAliveCounter++;
	return 0;
}

int UserGetDR(uint8_t *buffer, int size)
{
	uint8_t drvalue[MAX_FREQUENCIES + 13];
	for (int i = 0 ; i < MAX_FREQUENCIES; i++)
	{
		g_dr[i].valid = 1;
		g_dr[i].value = 0.5;
		drvalue[i] = g_dr[i].value * 10;
	}

	int x = 0;
	for (int i = 0 ; i < 13; i++)
	{
		 for (int j = 0; j < 8; j++)
		 {
			 drvalue[MAX_FREQUENCIES + j] = g_dr[x].valid;
			 drvalue[MAX_FREQUENCIES + j] = drvalue[MAX_FREQUENCIES + j] << 1;
			 x++;
		 }
	}

	uint8_t *p = (uint8_t *)drvalue;
	WriteI2COutputData(sizeof(drvalue), p);

	g_cookingResponse.drready = 0;

	return 0;
}

int GetCookingData(uint8_t *buffer, int size)
{


	uint8_t type = buffer[0];
	if (type == 0)
	{
		uint8_t *p = (uint8_t *)g_cookingData1;
		WriteI2COutputData(sizeof(g_cookingData1), p);
	} else {
		uint8_t *p = (uint8_t *)g_cookingData2;
		WriteI2COutputData(sizeof(g_cookingData2), p);
	}

	return 0;


}


