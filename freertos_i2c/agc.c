/*  Standard C Included Files */
#include <string.h>
#include "fsl_debug_console.h"
#include "fsl_smc.h"
#include "fsl_pmc.h"
#include "fsl_adc16.h"
#include "board.h"
#include "fsl_lptmr.h"

#include "pin_mux.h"
#include "clock_config.h"
#include "adc.h"
#include "agc.h"
#include "common.h"
#include "algo.h"
#include "amp.h"
#include <math.h>


extern uint8_t g_fixMag[MAX_FREQUENCIES];


PowerInfo g_powerInfo[MAX_FREQUENCIES];

extern volatile uint32_t  g_cookingFreeRunning;
uint16_t g_powerAvgCount = 0;

void AGCThread(bool agc,
			   int idx,
			   float maxpower)
{

	float forward = 0;
	float reflected = 0;
	int count = 0;
	uint16_t freq = g_dr[idx].freq;



	forward = amp_getForwardPower(freq);
	g_cookingData1[idx].forward = forward * 100;

	float diff = 0;
	diff = maxpower - forward;


	if (agc == true)
	{
		if (g_fixMag[idx] > 10)
			g_fixMag[idx] = 10;
		amp_SetOutputPower(g_fixMag[idx]);
	}
	else
	{
		if (g_fixMag[idx] > 10)
			g_fixMag[idx] = 10;
		amp_SetOutputPower(g_fixMag[idx]);
	}

	forward = amp_getForwardPower(freq);
	reflected = amp_getReflectedPower(freq);
	g_cookingData2[idx].reflected = reflected * 100;

	count++;

	g_powerInfo[idx].valid = true;
	g_powerInfo[idx].freqIndex = freq - 2400;
	g_powerInfo[idx].rwpower += reflected;
	g_powerInfo[idx].fwpower += forward;
	g_powerInfo[idx].timestamp = g_cookingFreeRunning;
	g_powerAvgCount++;

}
