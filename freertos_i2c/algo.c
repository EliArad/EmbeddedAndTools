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
#include "algo.h"
#include "timer.h"
#include "event.h"




int(*algoFunctions[4])(int algoIndex);
volatile int g_algoRunning = 0;

CookingResponse  g_cookingResponse;
uint8_t g_frequenciesTable[MAX_FREQUENCIES];
extern volatile uint8_t g_pause;


DR g_dr[MAX_FREQUENCIES];
uint8_t g_fixMag[MAX_FREQUENCIES];

TimerHandle_t SwDrCycleTimerHandle = NULL;
TimerHandle_t SwAGCTimerHandle  = NULL;
TimerHandle_t SwOverallTimerHandle  = NULL;

extern TimerHandle_t SwDrCycleTimerHandle;
extern TimerHandle_t SwAlgoTimerHandle;
extern uint8_t g_startCooking;
volatile uint32_t g_agcTimeDone = 0;

void SwAlgoTimerCallback(TimerHandle_t xTimer);
void SwDrCycleTimerCallback(TimerHandle_t xTimer);


CookingData1 g_cookingData1[MAX_FREQUENCIES];
CookingData2 g_cookingData2[MAX_FREQUENCIES];

static void SwAGCTimerCallback(TimerHandle_t xTimer)
{
	g_agcTimeDone = 1;
}

void SwOverallCookingTimer(TimerHandle_t xTimer)
{
	_StopCooking();
	PRINTF("Stop overall cooking timer\r\n");
}

void InitializeAlgoFunctions()
{
	algoFunctions[0] = &EqualEnergyAlgo;
	algoFunctions[1] = &DRThresholdAlgo;
	algoFunctions[2] = &TopPercentageAlgo;
	algoFunctions[3] = &RFOff;

	for (int i = 0 ; i < MAX_FREQUENCIES; i++)
	{
		g_frequenciesTable[i] = true;
	}
}

void CreateAlgoTimers(uint32_t drTime, uint32_t algoTime )
{

	if (SwDrCycleTimerHandle == NULL)
	{

		SwDrCycleTimerHandle = xTimerCreate("SwDrCycleTimer",       /* Text name. */
											drTime * 1000, /* Timer period. */
											pdTRUE,                /* Enable auto reload. */
											0,                     /* ID is not used. */
											SwDrCycleTimerCallback);      /* The callback function. */

	}

	if (SwAlgoTimerHandle == NULL && algoTime > 0)
	{
		SwAlgoTimerHandle = xTimerCreate("SwAlgoCycleTimer",       /* Text name. */
										 algoTime * 1000, /* Timer period. */
										 pdTRUE,                /* Enable auto reload. */
										 0,                     /* ID is not used. */
										 SwAlgoTimerCallback);      /* The callback function. */
	}
}

void CreateAlgoCycleTimer(uint32_t algoTime)
{
	if (SwAlgoTimerHandle == NULL)
	{
		SwAlgoTimerHandle = xTimerCreate("SwAlgoCycleTimer",       /* Text name. */
										 algoTime * 1000, /* Timer period. */
										 pdTRUE,                /* Enable auto reload. */
										 0,                     /* ID is not used. */
										 SwAlgoTimerCallback);      /* The callback function. */
	}
}


void CreateOverallCookingTimer(uint32_t Time)
{
	if (SwOverallTimerHandle == NULL)
	{
		SwOverallTimerHandle = xTimerCreate("SwOverallCookingTimer",       /* Text name. */
										 Time * 1000, /* Timer period. */
										 pdTRUE,                /* Enable auto reload. */
										 0,                     /* ID is not used. */
										 SwOverallCookingTimer);      /* The callback function. */
	}
}
void DestroyOverallCookingTimer()
{
	if (SwOverallTimerHandle != NULL)
		xTimerDelete(SwOverallTimerHandle,0);
	SwOverallTimerHandle = NULL;
}

void DestroyAlgoTimers()
{

	if (SwDrCycleTimerHandle != NULL)
		xTimerDelete(SwDrCycleTimerHandle,0);
	SwDrCycleTimerHandle = NULL;
	if (SwAlgoTimerHandle != NULL)
		xTimerDelete(SwAlgoTimerHandle,0);
	SwAlgoTimerHandle = NULL;

}

void CheckPause()
{
	uint8_t stopped = 0;
	if (g_pause == 1)
	{
		timer_stop();
		stopped = 1;
	}
	while (g_pause == 1)
	{
		vTaskDelay(10);
	}
	if (stopped == 1)
		timer_start();
}

void AlgoDispose()
{

	if (SwDrCycleTimerHandle != NULL)
		xTimerStop(SwDrCycleTimerHandle, 0);
	if (SwAlgoTimerHandle != NULL)
		xTimerStop(SwAlgoTimerHandle, 0);
	DestroyAlgoTimers();

}
