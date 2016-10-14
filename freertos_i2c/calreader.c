#include "fsl_debug_console.h"
#include "common.h"
#include "diag.h"
#include "flash.h"


AB g_ABForward[MAX_FREQUENCIES];
AB g_ABReflected[MAX_FREQUENCIES];

uint16_t calreader_GetCode(float f, float power)
{

	return 1;

}

uint16_t calreader_GetCodeByFreqIndex(int f, float power)
{

	return 1;

}

void calreader_Init()
{
	uint32_t destAdrss;
	destAdrss =  getDestAddress(GENERAL_SECTION_AB_FORWARD);
	uint32_t *ptr = (uint32_t *)destAdrss;

	uint32_t size = *(uint32_t *)(ptr + 1);
	if (size != 1616)
	{
		for (int i = 0, k = 0 ; k < MAX_FREQUENCIES; i+=4, k++)
		{
			double a =  *(double *)(ptr + 2 + i);
			double b =  *(double *)(ptr + 2 + 2 + i);
			a = a  / 1000000000.0;
			a = a / 100.0;
			g_ABForward[k].A = a;
			b = b / 100000.0;
			g_ABForward[k].B = b;
			//PRINTF("a = %f  b = %f\r\n" , g_ABForward[k].A, g_ABForward[k].B);
		}
	} else {
		g_errors |= e1_FORWARD_CAL_NOT_FOUND;
	}

	destAdrss =  getDestAddress(GENERAL_SECTION_AB_REFLECTED);
	//PRINTF("destAdrss = %x\r\n", destAdrss);
	ptr = (uint32_t *)destAdrss;
	//PRINTF("location reflected: %x\r\n" , *ptr);
	//PRINTF("size reflected: %x\r\n" , *(ptr + 1));
	size = *(uint32_t *)(ptr + 1);
	if (size != 1616)
	{
		for (int i = 0, k = 0 ; k < MAX_FREQUENCIES; i+=4, k++)
		{
			double a =  *(double *)(ptr + 2 + i);
			double b =  *(double *)(ptr + 2 + 2 + i);
			a = a  / 1000000.0;
			a = a / 100000;
			g_ABReflected[k].A = a;
			b = b / 100000;
			g_ABReflected[k].B = b;
			//PRINTF("a = %f  b = %f\r\n" , g_ABReflected[k].A, g_ABReflected[k].B);
		}
	} else {
		g_errors |= e1_REFLECTED_CAL_NOT_FOUND;
	}

}
