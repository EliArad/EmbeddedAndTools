
#include "fsl_debug_console.h"
#include "fsl_lptmr.h"
#include "fsl_gpio.h"

#include "pin_mux.h"
#include "clock_config.h"
#include "common.h"
#include "algo.h"
#include "timer.h"

#define LPTMR_LED_HANDLER LPTMR0_IRQHandler

#define LED_INIT() LED_RED_INIT(LOGIC_LED_ON)
#define LED_TOGGLE() LED_RED_TOGGLE()

uint8_t g_configTimer = 0;
volatile uint32_t g_drTimer = 0U;
volatile uint8_t  g_lptmrFirst = 0U;
extern int g_totalTimeInTimerTikcs;
extern CookingResponse  g_cookingResponse;


volatile uint32_t  g_cookingFreeRunning = 0U;

uint8_t g_timerStatus = 0;


void LPTMR_LED_HANDLER(void)
{

    LPTMR_ClearStatusFlags(LPTMR0, kLPTMR_TimerCompareFlag);
    if (g_lptmrFirst > 0)
    {
    	g_cookingFreeRunning++;

    	g_cookingResponse.clockSeconds++;
    	if (g_cookingResponse.clockSeconds == 60)
    	{
    		g_cookingResponse.clockSeconds = 0;
    		g_cookingResponse.clockMinutes++;

    		if (g_cookingResponse.clockMinutes == 60)
    		{
    			g_cookingResponse.clockMinutes = 0;

    			g_cookingResponse.clockHours++;
    		    g_cookingResponse.clockMinutes = 0;

    		}
    	}
    	PRINTF("%d:%d\r\n" ,g_cookingResponse.clockMinutes,  g_cookingResponse.clockSeconds);
        if (g_totalTimeInTimerTikcs > 0)
        {
        	g_totalTimeInTimerTikcs--;
        	g_cookingResponse.AlgoClockSeconds = g_totalTimeInTimerTikcs;
        	if (g_totalTimeInTimerTikcs == 0)
        	{
        	    timer_stop();
        	    g_algoRunning = 0;
        		PRINTF("STOP ALGO\r\n");
        	}
        }
    }
    g_lptmrFirst = 1;
}


void configTimer(int timeInMili)
{
	if (g_configTimer == 1)
		return;


    lptmr_config_t lptmrConfig;
	/* Configure LPTMR */
	/*
	 * lptmrConfig.timerMode = kLPTMR_TimerModeTimeCounter;
	 * lptmrConfig.pinSelect = kLPTMR_PinSelectInput_0;
	 * lptmrConfig.pinPolarity = kLPTMR_PinPolarityActiveHigh;
	 * lptmrConfig.enableFreeRunning = false;
	 * lptmrConfig.bypassPrescaler = true;
	 * lptmrConfig.prescalerClockSource = kLPTMR_PrescalerClock_1;
	 * lptmrConfig.value = kLPTMR_Prescale_Glitch_0;
	 */
	LPTMR_GetDefaultConfig(&lptmrConfig);

	/* Initialize the LPTMR */
	LPTMR_Init(LPTMR0, &lptmrConfig);

	/* Set timer period */
	LPTMR_SetTimerPeriod(LPTMR0, MSEC_TO_COUNT(timeInMili, LPTMR_SOURCE_CLOCK));

	/* Enable timer interrupt */
	LPTMR_EnableInterrupts(LPTMR0, kLPTMR_TimerInterruptEnable);

	/* Enable at the NVIC */
	EnableIRQ(LPTMR0_IRQn);

	g_configTimer = 1;

}
void timer_start()
{

	if (g_timerStatus == 0)
	{
		LPTMR_StartTimer(LPTMR0);
		g_timerStatus = 1;
	}


}
void timer_stop()
{
	if (g_timerStatus == 1)
	{
		LPTMR_StopTimer(LPTMR0);
		g_timerStatus = 0;
	}

}
