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
#include "algo.h"
#include "sortalgo.h"

extern DR g_dr[MAX_FREQUENCIES];


void display()
{
   int i;


   // navigate through all items
   for(i = 0; i < MAX_FREQUENCIES; i++){
      PRINTF("%f   freq  %f\r\n ",g_dr[i].value  , g_dr[i].freq);
   }

}

void bubbleSort(int topPercentage)
{
   DR temp;
   int i,j;

   bool swapped = false;

   // loop through all numbers
   for(i = 0; i < MAX_FREQUENCIES - 1; i++)
   {
      swapped = false;

      // loop through numbers falling ahead
      for(j = 0; j < MAX_FREQUENCIES-1-i; j++)
      {
         if(g_dr[j].value > g_dr[j+1].value)
         {
            temp.value = g_dr[j].value;
            temp.freq = g_dr[j].freq;
            temp.valid = g_dr[j].valid;

            g_dr[j].value = g_dr[j+1].value;
            g_dr[j].freq = g_dr[j+1].freq;
			g_dr[j].valid = g_dr[j+1].valid;

            g_dr[j+1].value = temp.value;
            g_dr[j+1].freq = temp.freq;
            g_dr[j+1].valid = temp.valid;

            swapped = true;
         }
      }

      // if no number was swapped that means
      //   array is sorted now, break the loop.
      if(!swapped) {
         break;
      }
   }
}
