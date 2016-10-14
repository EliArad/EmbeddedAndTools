#ifndef _ADC_HEADER_FILE
#define _ADC_HEADER_FILE


typedef struct lowPowerAdcBoundaries
{
    int32_t upperBoundary; /*! upper boundary in degree */
    int32_t lowerBoundary; /*! lower boundary in degree */
} lowPowerAdcBoundaries_t;



/*******************************************************************************
 * Prototypes
 ******************************************************************************/
void BOARD_ConfigTriggerSource(void);



int InitADCInOneShotMode();
int InitADCinContinuesDMAMode();
uint32_t GetADCContinuesDMAValue();
uint32_t GetADCOnsShotValue();

#endif
