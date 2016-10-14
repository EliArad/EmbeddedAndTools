#ifndef _AMP_HEADER_FILE
#define _AMP_HEADER_FILE




void amp_SetFrequency(float freq);
void amp_SetOutputPower(uint16_t mag);
float amp_getForwardPower(float freq);
float amp_getReflectedPower(float freq);
void amp_SetRFOff();


#endif
