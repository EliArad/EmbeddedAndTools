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


static TickType_t  g_tickStartValue;
void RTOS_Tick_TickStart()
{
	g_tickStartValue = xTaskGetTickCount();
}

uint8_t RTOS_Tick_IsElapsedTime(uint32_t time, uint32_t *diff)
{
	//time = time * portTICK_RATE_MS; // but it is 1
	TickType_t curTick = xTaskGetTickCount();
	if (curTick >= g_tickStartValue)
	{
		//PRINTF("curTick = %d   g_tickStartValue = %d  time = %d\r\n" , curTick, g_tickStartValue, time);
		if ((curTick - g_tickStartValue) >= time)
		{
			*diff = curTick - g_tickStartValue;
			return 1;
		} else {
			*diff = 0;
			return 0;
		}
	} else {
		halt(109);
		return 0;
	}
}
uint32_t RTOS_Tick_GetLast()
{
	TickType_t curTick = xTaskGetTickCount();
	return curTick;
}

uint32_t RTOS_Tick_GetStopDiff()
{

	TickType_t curTick = xTaskGetTickCount();
	if (curTick >= g_tickStartValue)
	{
		return  (curTick - g_tickStartValue);
	} else {
		halt(109);
		return 0;
	}
}


