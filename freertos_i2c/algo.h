#ifndef _ALGO_HEADER_FILE
#define _ALGO_HEADER_FILE


typedef struct _DR
{
	uint16_t valid;
	float value;
	uint16_t freq;
} DR;

int DRThresholdAlgo(int algoIndex);
int EqualEnergyAlgo(int algoIndex);
int TopPercentageAlgo(int algoIndex);
int RFOff(int algoIndex);
void InitializeAlgoFunctions();
void CheckPause();
void AlgoDispose();
void CreateAlgoTimers(uint32_t drTime, uint32_t algoTime );
void CreateAlgoCycleTimer(uint32_t algoTime);
void DestroyOverallCookingTimer();
void CreateOverallCookingTimer(uint32_t Time);

extern volatile int g_algoRunning;
extern uint8_t g_frequenciesTable[MAX_FREQUENCIES];
extern DR g_dr[MAX_FREQUENCIES];
extern uint8_t g_fixMag[MAX_FREQUENCIES];
extern volatile uint32_t g_agcTimeDone;
extern CookingData1 g_cookingData1[MAX_FREQUENCIES];
extern CookingData2 g_cookingData2[MAX_FREQUENCIES];

#endif
