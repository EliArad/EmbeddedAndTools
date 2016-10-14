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
#include "amp.h"
#include "fsl_smc.h"
#include "fsl_pmc.h"
#include "fsl_adc16.h"
#include "common.h"


extern AB g_ABForward[MAX_FREQUENCIES];
extern AB g_ABReflected[MAX_FREQUENCIES];


void amp_SetOutputPower(uint16_t mag)
{


}
void amp_SetRFOff()
{

}

void amp_SetFrequency(float freq)
{

}

float amp_getForwardPower(float freq)
{

	uint32_t x = xTaskGetTickCount();
	float r = 40 + (x & 0x3);
	if (r > 53)
		r = 53;
	return r;
}

float amp_getReflectedPower(float freq)
{
	return 31;
}
