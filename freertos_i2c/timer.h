#ifndef _TIMER_HEADER_FILE
#define _TIMER_HEADER_FILE

/* Get source clock for LPTMR driver */
#define LPTMR_SOURCE_CLOCK CLOCK_GetFreq(kCLOCK_LpoClk)




void timer_start();
void timer_stop();
void configTimer(int timeInMili);




#endif
