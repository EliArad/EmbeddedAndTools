
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
#include "amp.h"
#include "algo.h"
#include "timer.h"


extern volatile int g_cookingIndex;
extern uint16_t cookingHeatList[MAX_HEAT_LIST];
extern uint8_t g_useInternalClockForDisplay;
extern int g_totalTimeInTimerTikcs;
extern TimerHandle_t SwAlgoTimerHandle;
extern CookingResponse  g_cookingResponse;




int RFOff(int algoIndex)
{
	 g_algoRunning = 1;

	 PRINTF("RF OFF\r\n");

 	 g_cookingIndex++;
	 uint16_t TotalSeconds = cookingHeatList[g_cookingIndex++];

	 CreateAlgoCycleTimer(TotalSeconds);
	 amp_SetRFOff();


	 if (g_useInternalClockForDisplay)
 		configTimer(1000);

 	 g_totalTimeInTimerTikcs = TotalSeconds;

	 if (g_useInternalClockForDisplay)
		timer_start();


	 if (g_useInternalClockForDisplay == 0)
		xTimerStart(SwAlgoTimerHandle, 0);

	 g_cookingResponse.AlgoClockSeconds = g_totalTimeInTimerTikcs;


	 while (g_algoRunning)
	 {
		 // to replace to event wait
		 vTaskDelay(10);
	 }
	 AlgoDispose();
	 return 1;
}
