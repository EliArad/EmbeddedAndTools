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

#include "fsl_debug_console.h"
#include "fsl_smc.h"
#include "fsl_pmc.h"
#include "fsl_adc16.h"
#include "board.h"
#include "fsl_lptmr.h"

#include "pin_mux.h"
#include "clock_config.h"
#include "adc.h"

/*******************************************************************************
 * Definitions
 ******************************************************************************/
#define DEMO_ADC16_BASEADDR ADC0
#define DEMO_ADC16_CHANNEL_GROUP 0U

#define DEMO_ADC16_IRQ_ID ADC0_IRQn
#define DEMO_ADC16_IRQ_HANDLER_FUNC ADC0_IRQHandler

#define DEMO_LPTMR_BASE LPTMR0



#define LED1_INIT() LED_RED_INIT(LOGIC_LED_OFF)
#define LED1_ON() LED_RED_ON()
#define LED1_OFF() LED_RED_OFF()

#define LED2_INIT() LED_GREEN_INIT(LOGIC_LED_OFF)
#define LED2_ON() LED_GREEN_ON()
#define LED2_OFF() LED_GREEN_OFF()


#define DEMO_ADC16_BASE ADC0
#define DEMO_ADC16_CHANNEL_GROUP 0U
//#define DEMO_ADC16_USER_CHANNEL 12U
#define DEMO_ADC16_USER_CHANNEL 11U




/*
 * These values are used to get the temperature. DO NOT MODIFY
 * The method used in this demo to calculate temperature of chip is mapped to
 * Temperature Sensor for the HCS08 Microcontroller Family document (Document Number: AN3031)
 */
#define ADCR_VDD (65535U) /* Maximum value when use 16b resolution */
#define V_BG (1000U)      /* BANDGAP voltage in mV (trim to 1.0V) */
#define V_TEMP25 (716U)   /* Typical VTEMP25 in mV */
#define M (1620U)         /* Typical slope: (mV x 1000)/oC */
#define STANDARD_TEMP (25U)

#define LED1_INIT() LED_RED_INIT(LOGIC_LED_OFF)
#define LED1_ON() LED_RED_ON()
#define LED1_OFF() LED_RED_OFF()

#define LED2_INIT() LED_GREEN_INIT(LOGIC_LED_OFF)
#define LED2_ON() LED_GREEN_ON()
#define LED2_OFF() LED_GREEN_OFF()
#define kAdcChannelTemperature (26U) /*! ADC channel of temperature sensor */
#define kAdcChannelBandgap (27U)     /*! ADC channel of BANDGAP */

#define UPPER_VALUE_LIMIT (1U) /*! This value/10 is going to be added to current Temp to set the upper boundary*/
#define LOWER_VALUE_LIMIT                                                               \
    (1U) /*! This Value/10 is going to be subtracted from current Temp to set the lower \
            boundary*/
#define UPDATE_BOUNDARIES_TIME                                                       \
    (20U) /*! This value indicates the number of cycles needed to update boundaries. \
              To know the Time it will take, multiply this value times LPTMR_COMPARE_VALUE*/

#define LPTMR_COMPARE_VALUE (500U) /* Low Power Timer interrupt time in miliseconds */



static void ADC16_PauseConversion(ADC_Type *base);
static void LPTMR_InitTriggerSourceOfAdc(LPTMR_Type *base);
static bool ADC16_InitHardwareTrigger(ADC_Type *base);

/*******************************************************************************
 * Variables
 ******************************************************************************/
volatile static uint32_t adcValue = 0; /*! ADC value */
static uint32_t adcrTemp25 = 0;        /*! Calibrated ADCR_TEMP25 */
static uint32_t adcr100m = 0;
volatile bool conversionCompleted = false; /*! Conversion is completed Flag */

/*******************************************************************************
 * Code
 ******************************************************************************/

void BOARD_ConfigTriggerSource(void)
{
    /* Configure SIM for ADC hw trigger source selection */
    SIM->SOPT7 |= 0x0000008EU;
}
/* Enable the trigger source of LPTimer */
static void LPTMR_InitTriggerSourceOfAdc(LPTMR_Type *base)
{
    lptmr_config_t lptmrUserConfig;

    /*
     * lptmrUserConfig.timerMode = kLPTMR_TimerModeTimeCounter;
     * lptmrUserConfig.pinSelect = kLPTMR_PinSelectInput_0;
     * lptmrUserConfig.pinPolarity = kLPTMR_PinPolarityActiveHigh;
     * lptmrUserConfig.enableFreeRunning = false;
     * lptmrUserConfig.bypassPrescaler = true;
     * lptmrUserConfig.prescalerClockSource = kLPTMR_PrescalerClock_1;
     * lptmrUserConfig.value = kLPTMR_Prescale_Glitch_0;
     */
    LPTMR_GetDefaultConfig(&lptmrUserConfig);
    /* Init LPTimer driver */
    LPTMR_Init(base, &lptmrUserConfig);

    /* Set the LPTimer period */
    LPTMR_SetTimerPeriod(base, LPTMR_COMPARE_VALUE);

    /* Start the LPTimer */
    LPTMR_StartTimer(base);

    /* Configure SIM for ADC hw trigger source selection */
    BOARD_ConfigTriggerSource();
}

/*!
 * @brief ADC stop conversion
 */
static void ADC16_PauseConversion(ADC_Type *base)
{
    adc16_channel_config_t adcChnConfig;

    adcChnConfig.channelNumber = 31U; /*!< AD31 channel */
    adcChnConfig.enableInterruptOnConversionCompleted = false;
#if defined(FSL_FEATURE_ADC16_HAS_DIFF_MODE) && FSL_FEATURE_ADC16_HAS_DIFF_MODE
    adcChnConfig.enableDifferentialConversion = false;
#endif
    ADC16_SetChannelConfig(base, DEMO_ADC16_CHANNEL_GROUP, &adcChnConfig);
}

/*!
 * @brief calibrate parameters: VDD and ADCR_TEMP25
 */
static void ADC16_CalibrateParams(ADC_Type *base)
{
    uint32_t bandgapValue = 0; /*! ADC value of BANDGAP */
    uint32_t vdd = 0;          /*! VDD in mV */

    adc16_config_t adcUserConfig;
    adc16_channel_config_t adcChnConfig;
    pmc_bandgap_buffer_config_t pmcBandgapConfig;

    pmcBandgapConfig.enable = true;

#if (defined(FSL_FEATURE_PMC_HAS_BGEN) && FSL_FEATURE_PMC_HAS_BGEN)
    pmcBandgapConfig.enableInLowPowerMode = false;
#endif
#if (defined(FSL_FEATURE_PMC_HAS_BGBDS) && FSL_FEATURE_PMC_HAS_BGBDS)
    pmcBandgapConfig.drive = kPmcBandgapBufferDriveLow;
#endif
    /* Enable BANDGAP reference voltage */
    PMC_ConfigureBandgapBuffer(PMC, &pmcBandgapConfig);

    /*
    * Initialization ADC for
    * 16bit resolution, interrupt mode, hw trigger disabled.
    * normal convert speed, VREFH/L as reference,
    * disable continuous convert mode
    */
    /*
     * adcUserConfig.referenceVoltageSource = kADC16_ReferenceVoltageSourceVref;
     * adcUserConfig.clockSource = kADC16_ClockSourceAsynchronousClock;
     * adcUserConfig.enableAsynchronousClock = true;
     * adcUserConfig.clockDivider = kADC16_ClockDivider8;
     * adcUserConfig.resolution = kADC16_ResolutionSE12Bit;
     * adcUserConfig.longSampleMode = kADC16_LongSampleDisabled;
     * adcUserConfig.enableHighSpeed = false;
     * adcUserConfig.enableLowPower = false;
     * adcUserConfig.enableContinuousConversion = false;
     */
    ADC16_GetDefaultConfig(&adcUserConfig);
    adcUserConfig.resolution = kADC16_Resolution16Bit;
    adcUserConfig.enableContinuousConversion = false;
    adcUserConfig.clockSource = kADC16_ClockSourceAsynchronousClock;
    adcUserConfig.enableLowPower = 1;
    adcUserConfig.longSampleMode = kADC16_LongSampleCycle24;
#ifdef BOARD_ADC_USE_ALT_VREF
    adcUserConfig.referenceVoltageSource = kADC16_ReferenceVoltageSourceValt;
#endif
    ADC16_Init(base, &adcUserConfig);

#if defined(FSL_FEATURE_ADC16_HAS_CALIBRATION) && FSL_FEATURE_ADC16_HAS_CALIBRATION
    /* Auto calibration */
    ADC16_DoAutoCalibration(base);
#endif

#if defined(FSL_FEATURE_ADC16_HAS_HW_AVERAGE) && FSL_FEATURE_ADC16_HAS_HW_AVERAGE
    /* Use hardware average to increase stability of the measurement  */
    ADC16_SetHardwareAverage(base, kADC16_HardwareAverageCount32);
#endif /* FSL_FEATURE_ADC16_HAS_HW_AVERAGE */

    adcChnConfig.channelNumber = kAdcChannelBandgap;
#if defined(FSL_FEATURE_ADC16_HAS_DIFF_MODE) && FSL_FEATURE_ADC16_HAS_DIFF_MODE
    adcChnConfig.enableDifferentialConversion = false;
#endif
    adcChnConfig.enableInterruptOnConversionCompleted = false;
    ADC16_SetChannelConfig(base, DEMO_ADC16_CHANNEL_GROUP, &adcChnConfig);

    /* Wait for the conversion to be done */
    while (!ADC16_GetChannelStatusFlags(base, DEMO_ADC16_CHANNEL_GROUP))
    {
    }

    /* Get current ADC BANDGAP value */
    bandgapValue = ADC16_GetChannelConversionValue(base, DEMO_ADC16_CHANNEL_GROUP);

    ADC16_PauseConversion(base);

    /* Get VDD value measured in mV: VDD = (ADCR_VDD x V_BG) / ADCR_BG */
    vdd = ADCR_VDD * V_BG / bandgapValue;
    /* Calibrate ADCR_TEMP25: ADCR_TEMP25 = ADCR_VDD x V_TEMP25 / VDD */
    adcrTemp25 = ADCR_VDD * V_TEMP25 / vdd;
    /* ADCR_100M = ADCR_VDD x M x 100 / VDD */
    adcr100m = (ADCR_VDD * M) / (vdd * 10);

    /* Disable BANDGAP reference voltage */
    pmcBandgapConfig.enable = false;
    PMC_ConfigureBandgapBuffer(PMC, &pmcBandgapConfig);
}



static bool ADC16_InitHardwareTrigger(ADC_Type *base)
{
#if defined(FSL_FEATURE_ADC16_HAS_CALIBRATION) && FSL_FEATURE_ADC16_HAS_CALIBRATION
    uint16_t offsetValue = 0; /*!< Offset error from correction value. */
#endif
    adc16_config_t adcUserConfig;
    adc16_channel_config_t adcChnConfig;

#if defined(FSL_FEATURE_ADC16_HAS_CALIBRATION) && FSL_FEATURE_ADC16_HAS_CALIBRATION
    /* Auto calibration */
    ADC16_DoAutoCalibration(base);
    offsetValue = base->OFS;
    ADC16_SetOffsetValue(base, offsetValue);
#endif
    /*
    * Initialization ADC for
    * 16bit resolution, interrupt mode, hw trigger enabled.
    * normal convert speed, VREFH/L as reference,
    * disable continuous convert mode.
    */
    /*
     * adcUserConfig.referenceVoltageSource = kADC16_ReferenceVoltageSourceVref;
     * adcUserConfig.clockSource = kADC16_ClockSourceAsynchronousClock;
     * adcUserConfig.enableAsynchronousClock = true;
     * adcUserConfig.clockDivider = kADC16_ClockDivider8;
     * adcUserConfig.resolution = kADC16_ResolutionSE12Bit;
     * adcUserConfig.longSampleMode = kADC16_LongSampleDisabled;
     * adcUserConfig.enableHighSpeed = false;
     * adcUserConfig.enableLowPower = false;
     * adcUserConfig.enableContinuousConversion = false;
     */
    ADC16_GetDefaultConfig(&adcUserConfig);
    adcUserConfig.resolution = kADC16_Resolution16Bit;
    /* enabled hardware trigger  */
    ADC16_EnableHardwareTrigger(base, true);
    adcUserConfig.enableContinuousConversion = false;
    adcUserConfig.clockSource = kADC16_ClockSourceAsynchronousClock;

    adcUserConfig.longSampleMode = kADC16_LongSampleCycle24;
    adcUserConfig.enableLowPower = 1;
#if ((defined BOARD_ADC_USE_ALT_VREF) && BOARD_ADC_USE_ALT_VREF)
    adcUserConfig.referenceVoltageSource = kADC16_ReferenceVoltageSourceValt;
#endif
    ADC16_Init(base, &adcUserConfig);

    adcChnConfig.channelNumber = kAdcChannelTemperature;
#if defined(FSL_FEATURE_ADC16_HAS_DIFF_MODE) && FSL_FEATURE_ADC16_HAS_DIFF_MODE
    adcChnConfig.enableDifferentialConversion = false;
#endif
    adcChnConfig.enableInterruptOnConversionCompleted = true;
    /* Configure channel 0 */
    ADC16_SetChannelConfig(base, DEMO_ADC16_CHANNEL_GROUP, &adcChnConfig);
    return true;
}

void DEMO_ADC16_IRQ_HANDLER_FUNC(void)
{
    /* Get current ADC value */
    adcValue = ADC16_GetChannelConversionValue(DEMO_ADC16_BASEADDR, DEMO_ADC16_CHANNEL_GROUP);
    /* Set conversionCompleted flag. This prevents an wrong conversion in main function */
    conversionCompleted = true;
}



int InitADCinContinuesDMAMode()
{



	return 1;

}

int InitADCInOneShotMode()
{

  adc16_config_t adc16ConfigStruct;

  /*
  * adc16ConfigStruct.referenceVoltageSource = kADC16_ReferenceVoltageSourceVref;
  * adc16ConfigStruct.clockSource = kADC16_ClockSourceAsynchronousClock;
  * adc16ConfigStruct.enableAsynchronousClock = true;
  * adc16ConfigStruct.clockDivider = kADC16_ClockDivider8;
  * adc16ConfigStruct.resolution = kADC16_ResolutionSE12Bit;
  * adc16ConfigStruct.longSampleMode = kADC16_LongSampleDisabled;
  * adc16ConfigStruct.enableHighSpeed = false;
  * adc16ConfigStruct.enableLowPower = false;
  * adc16ConfigStruct.enableContinuousConversion = false;
  */
 ADC16_GetDefaultConfig(&adc16ConfigStruct);
 ADC16_Init(DEMO_ADC16_BASE, &adc16ConfigStruct);
 ADC16_EnableHardwareTrigger(DEMO_ADC16_BASE, false); /* Make sure the software trigger is used. */

 ADC16_EnableDMA(DEMO_ADC16_BASEADDR, true);

#if defined(FSL_FEATURE_ADC16_HAS_CALIBRATION) && FSL_FEATURE_ADC16_HAS_CALIBRATION
	if (kStatus_Success == ADC16_DoAutoCalibration(DEMO_ADC16_BASE))
	{
		PRINTF("ADC16_DoAutoCalibration() Done.\r\n");
	}
	else
	{
		PRINTF("ADC16_DoAutoCalibration() Failed.\r\n");
	}
#endif /* FSL_FEATURE_ADC16_HAS_CALIBRATION */

	return 1;
}

uint32_t GetADCContinuesDMAValue()
{
	while (!conversionCompleted)
	{
	}
	/* Get current Temperature Value */
	PRINTF("GetADCContinuesDMAValue adcValue = %d\r\n" , adcValue);

	/* Clear conversionCompleted flag */
	conversionCompleted = false;
	return adcValue;
}

uint32_t GetADCOnsShotValue()
{
	adc16_channel_config_t adc16ChannelConfigStruct;

	adc16ChannelConfigStruct.channelNumber = DEMO_ADC16_USER_CHANNEL;
	adc16ChannelConfigStruct.enableInterruptOnConversionCompleted = false;
	#if defined(FSL_FEATURE_ADC16_HAS_DIFF_MODE) && FSL_FEATURE_ADC16_HAS_DIFF_MODE
		adc16ChannelConfigStruct.enableDifferentialConversion = false;
	#endif /* FSL_FEATURE_ADC16_HAS_DIFF_MODE */


	ADC16_SetChannelConfig(DEMO_ADC16_BASE, DEMO_ADC16_CHANNEL_GROUP, &adc16ChannelConfigStruct);
	while (0U == (kADC16_ChannelConversionDoneFlag &
						  ADC16_GetChannelStatusFlags(DEMO_ADC16_BASE, DEMO_ADC16_CHANNEL_GROUP)))
	{
	}
	uint32_t value = ADC16_GetChannelConversionValue(DEMO_ADC16_BASE, DEMO_ADC16_CHANNEL_GROUP);

	return value;
}
