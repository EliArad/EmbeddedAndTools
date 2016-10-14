#include "fsl_debug_console.h"
#include "common.h"
#include "amp.h"
#include "algo.h"
#include <math.h>
/* FreeRTOS kernel includes. */
#include "FreeRTOS.h"
#include "task.h"
#include "queue.h"
#include "timers.h"
#include "semphr.h"

extern CookingResponse  g_cookingResponse;


extern DR g_dr[MAX_FREQUENCIES];


void GetDR(float drPower, float *accK)
{
	float forward_watts;
	float reflected_watts;
	int i;

	 double forward = 0;
	 double reflected = 0;
	 uint16_t code = calreader_GetCode(2450, drPower);
	 *accK = 0;
	 i = 0;

	 for (float f = 2400; f <= 2500; f++)
	 {

		amp_SetOutputPower(code);

		if (g_frequenciesTable[i] == 0)
		{
			g_dr[i].valid = false;
			i++;
			continue;
		}

		amp_SetFrequency(f);

		forward = amp_getForwardPower(f);
		forward_watts = pow(10, forward / 10) / 1000;

		reflected = amp_getReflectedPower(f);
		reflected_watts = pow(10, reflected / 10) / 1000;


		g_dr[i].freq = f;
		g_dr[i].value = (forward_watts - reflected_watts) / forward_watts;
		if (g_dr[i].value < 0)
			g_dr[i].value = 0;
		g_dr[i].valid = true;

		//TimeSpan diff = DateTime.Now - start;
		//TickType_t stop = xTaskGetTickCount();
		//TickType_t diff = stop - start;

		//uint32_t diffInMicroSeconds = COUNT_TO_USEC(diff , CLOCK_GetFreq(kCLOCK_CoreSysClk));
		//PRINTF("diff = %d   ,   diffInMicroSeconds  = %d\r\n" , diff, diffInMicroSeconds);
		//*accKj += ((forward_watts - reflected_watts) * diff.TotalMilliseconds) / 1000000;

		i++;
	 }

	 g_cookingResponse.drready = 1;
}
