#ifndef _DIAG_HEADER_FILE
#define _DIAG_HEADER_FILE


void InitializeSystem();
int setFrequency(uint8_t *buffer, int size);
int setMagnitude(uint8_t *buffer, int size);
int readADC(uint8_t *buffer, int size);
int AddAlgo(uint8_t *buffer, int size);
int AppendAlgo(uint8_t *buffer, int size);
int StartCooking(uint8_t *buffer, int size);
int StopCooking(uint8_t *buffer, int size);
int PauseCooking(uint8_t *buffer, int size);
int ClearAlgos(uint8_t *buffer, int size);
int WriteFlashCalAbLinear(uint8_t *buffer, int size);
int ReadFlashCalAbLinear(uint8_t *buffer, int size);
int ReadFlashDishes(uint8_t *buffer, int size);
int WriteFlashDishes(uint8_t *buffer, int size);
int WriteFlashTopazInfo(uint8_t *buffer, int size);
int ReadFlashTopazInfo(uint8_t *buffer, int size);
int ReadStatus(uint8_t *buffer, int size);
int KeepAlive(uint8_t *buffer, int size);
void WriteI2COutputData(int size, uint8_t *p);
int ReadFlashChunk(uint8_t *buffer, int size);
int SetInternalClockForDisplay(uint8_t *buffer, int size);
int UserGetDR(uint8_t *buffer, int size);
#endif
