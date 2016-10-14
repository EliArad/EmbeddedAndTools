#ifndef _COMMON_HEADER_FILE_
#define _COMMON_HEADER_FILE_

#include "errors.h"

#define I2C_IN_BUF_SIZE  1000
#define I2C_OUT_BUF_SIZE 1000
#define MAX_HEAT_LIST    200
#define MAX_FREQUENCIES  101

#define START_FREQUENCY     2400
#define STOP_FREQUENCY 		2500

#define false 0
#define true  1


extern uint32_t g_errors;
typedef enum OPCODES
{
        OPC_SET_FREQUENCY,
        OPC_SET_MAGNITUDE,
        OPC_READ_DETECTOR,
        OPC_GET_VERSION,

		OPC_ADDALGO,
		OPC_APPENDALGO,
		OPC_CLEARALGOS,
		OPC_START_COOKING,
		OPC_STOP_COOKING,
		OPC_PAUSE_COOKING,
		OPC_START_DISH_COOKING,


        OPC_WRITE_TOPAZ_INFO,
		OPC_WRITE_CAL_AB_LINEAR,
		OPC_WRITE_DISHES,

		OPC_READ_TOPAZ_INFO,
		OPC_READ_CAL_AB_LINEAR,
		OPC_READ_DISHES,

		OPC_READ_STATUS,
		OPC_KEEP_ALIVE,
		OPC_USE_INTERNAL_CLOCK,
		OPC_WRITE_FLASH_CHUNK,
		OPC_READ_FLASH_CHUNK,


		OPC_GET_DR,
		OPC_GET_COOKING_DATA,



        OPC_LAST_OPCODE
} OPCODES;

typedef struct _MsgHeader
{
	unsigned short prehumble;
	unsigned short opcode;
	unsigned short size;
} msgHeader;


typedef struct AB
{
	double A;
	double B;
}AB;


typedef struct _StartCookingParams
{
	uint16_t time;
	uint8_t oneTime;
} StartCookingParams;


#define HEADER_SIZE 6


typedef struct _Response
{
	unsigned char res;
	unsigned char reserved;
	unsigned short opcode;
} Response;

typedef struct ADCResponse
{
	unsigned short forward;
	unsigned short reflected;
	unsigned short opcode;
} ADCResponse;


typedef struct PowerInfo
{
	uint16_t timestamp;
	float fwpower;
	float rwpower;
	uint8_t valid;
	uint8_t freqIndex;

}PowerInfo;

typedef struct CookingData1
{
	uint16_t forward;

} CookingData1;


typedef struct CookingData2
{
	uint16_t reflected;
} CookingData2;


typedef struct CookingResponse
{
	float    kj;
	uint8_t  clockSeconds;
	uint8_t  clockMinutes;
	uint8_t  clockHours;
	uint8_t  cycleElapsedTime;
	uint16_t AlgoClockSeconds;
	uint8_t  running;
	uint8_t  algoIndex;
	uint16_t opcode;
	uint8_t  rowIndex;
	uint8_t  drready;

	uint8_t  cycleEndded;
	uint8_t  drCount;

	uint32_t error1;
	uint32_t error2;
	uint32_t error3;

} CookingResponse;




typedef struct _FrequencyData
{
	unsigned short frequency;
} FrequencyData;

typedef struct _FrequencyGetData
{
	unsigned short frequency;
} FrequencyGetData;



void halt(int code);
int GetInFifoSize();
void IncOutFifo(int size);
int GetOutFifoSize();
int isOutFifoFull();
int isInFifoFull();
void MsgHandler();
void sendResponse(unsigned short opcode, unsigned char res);
void configTimer(int timeInMili);
uint16_t calreader_GetCode(float f, float power);
uint16_t calreader_GetCodeByFreqIndex(int f, float power);
void GetDR(float drPower, float *accK);
int WriteFlashChunk(uint8_t *buffer, int size);
void SleepMilliseconds(uint32_t ms);
uint32_t gfxSystemTicks(void);
void RTOS_Tick_TickStart();
uint8_t RTOS_Tick_IsElapsedTime(uint32_t time, uint32_t *diff);
uint32_t RTOS_Tick_GetLast();
uint32_t RTOS_Tick_GetStopDiff();
void calreader_Init();
int GetCookingData(uint8_t *buffer, int size);
void _StopCooking();



















#define DSPI_MASTER_INSTANCE        BOARD_DSPI_INSTANCE /*! User change define to choose DSPI instance */
#define TRANSFER_SIZE               (32)                /*! Transfer size */
#define TRANSFER_BAUDRATE           (500000U)           /*! Transfer baudrate - 500k */
#define MASTER_TRANSFER_TIMEOUT     (5000U)

#endif
