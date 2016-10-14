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
#include "diag.h"
#include "flash.h"
#include "fsl_wdog.h"
#include "fsl_rcm.h"



#define WDOG_WCT_INSTRUCITON_COUNT (256U)

/*******************************************************************************
 * Prototypes
 ******************************************************************************/

/*******************************************************************************
* Variables
******************************************************************************/
static WDOG_Type *wdog_base = WDOG;
static RCM_Type *rcm_base = RCM;

uint16_t wdog_reset_count = 0;
wdog_test_config_t test_config;
wdog_config_t config;


/*******************************************************************************
 * Code
 ******************************************************************************/

static void WaitWctClose(WDOG_Type *base)
{
    /* Accessing register by bus clock */
    for (uint32_t i = 0; i < WDOG_WCT_INSTRUCITON_COUNT; i++)
    {
        (void)base->RSTCNT;
    }
}

/*!
 * @brief Gets the Watchdog timer output.
 *
 * @param base WDOG peripheral base address
 * @return Current value of watchdog timer counter.
 */
static inline uint32_t GetTimerOutputValue(WDOG_Type *base)
{
    return (uint32_t)((((uint32_t)base->TMROUTH) << 16U) | (base->TMROUTL));
}


void wdog_init()
{

	/*If not wdog reset*/
	if (!(RCM_GetPreviousResetSources(rcm_base) & kRCM_SourceWdog))
	{
		WDOG_ClearResetCount(wdog_base);
	}
	wdog_reset_count = WDOG_GetResetCount(wdog_base);

	WDOG_GetDefaultConfig(&config);
	config.timeoutValue = 0x7ffU * 30; // 30 seconds of wdog
	/* wdog refresh test in none-window mode */
	PRINTF("\r\n--- None-window mode refresh test start---\r\n");
	WDOG_Init(wdog_base, &config);
	WaitWctClose(wdog_base);

}

void wdog_close()
{

	WDOG_Deinit(wdog_base);
}
