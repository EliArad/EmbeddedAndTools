
#include <string.h>

/* FreeRTOS kernel includes. */
#include "FreeRTOS.h"
#include "task.h"
#include "queue.h"
#include "timers.h"
#include "semphr.h"
#include "fsl_flash.h"
#include "fsl_debug_console.h"

/*  SDK Included Files */
#include "board.h"

#include "common.h"
#include "flash.h"



/*******************************************************************************
 * Prototypes
 ******************************************************************************/


#define BUFFER_LEN 1024



flash_config_t flashDriver;                                            /* Flash driver Structure */
flash_security_state_t securityStatus = kFLASH_securityStateNotSecure; /* Return protection status */
status_t result;    /* Return code from each flash driver function */
uint32_t destAdrss; /* Address of the target location */
uint32_t i, failAddr, failDat;

uint32_t pflashBlockBase = 0;
uint32_t pflashTotalSize = 0;
uint32_t pflashSectorSize = 0;
uint32_t dflashTotalSize = 0;
uint32_t dflashBlockBase = 0;
uint32_t eepromTotalSize = 0;



#define TEST_ERASE_SECTOR_START_ADDRESSS 0x80000u
#define FLASH_ERASE_SIZE 0x400




int flash_init(void)
{

    /* Clean up Flash driver Structure*/
    memset(&flashDriver, 0, sizeof(flash_config_t));

    /* Setup flash driver structure for device and initialize variables. */
    result = FLASH_Init(&flashDriver);
    if (kStatus_FLASH_Success != result)
    {
    	return 405;
    }
    /* Get flash properties*/
    FLASH_GetProperty(&flashDriver, kFLASH_propertyPflashBlockBaseAddr, &pflashBlockBase);
    FLASH_GetProperty(&flashDriver, kFLASH_propertyPflashTotalSize, &pflashTotalSize);
    FLASH_GetProperty(&flashDriver, kFLASH_propertyPflashSectorSize, &pflashSectorSize);
    FLASH_GetProperty(&flashDriver, kFLASH_propertyDflashTotalSize, &dflashTotalSize);
    FLASH_GetProperty(&flashDriver, kFLASH_propertyDflashBlockBaseAddr, &dflashBlockBase);
    FLASH_GetProperty(&flashDriver, kFLASH_propertyEepromTotalSize, &eepromTotalSize);


    PRINTF("\r\n Flash Information: ");
    PRINTF("\r\n pflashBlockBase:\t%d", pflashBlockBase);
	PRINTF("\r\n Total Program Flash Size:\t%d KB, Hex: (0x%x)", (pflashTotalSize / 1024), pflashTotalSize);
	PRINTF("\r\n Program Flash Sector Size:\t%d KB, Hex: (0x%x) ", (pflashSectorSize / 1024), pflashSectorSize);


    /* Check if DFlash exist on this device. */
	if (dflashTotalSize)
	{
		PRINTF("\r\n Data Flash Size:\t%d KB, Hex: (0x%x)", (dflashTotalSize / 1024), dflashTotalSize);
		PRINTF("\r\n Data Flash Base Address:\t0x%x", dflashBlockBase);
	}
	else
	{
		PRINTF("\r\n There is no D-Flash (FlexNVM) on this Device.");
	}
	/* Check if FlexMemory exist on this device. */
	if (eepromTotalSize)
	{
		PRINTF("\r\n Enhanced EEPROM (EEE) Block Size:\t%d KB, Hex: (0x%x)", (eepromTotalSize / 1024), eepromTotalSize);
	}
	else
	{
		PRINTF("\r\n There is no Enhanced EEPROM (EEE) on this Device.");
	}


	/* Check security status. */
	result = FLASH_GetSecurityState(&flashDriver, &securityStatus);
	if (kStatus_FLASH_Success != result)
	{
		return 402;
	}
	/* Print security status. */
	switch (securityStatus)
	{
		case kFLASH_securityStateNotSecure:
			PRINTF("\r\n Flash is UNSECURE!");
			break;
		case kFLASH_securityStateBackdoorEnabled:
			PRINTF("\r\n Flash is SECURE, BACKDOOR is ENABLED!");
			break;
		case kFLASH_securityStateBackdoorDisabled:
			PRINTF("\r\n Flash is SECURE, BACKDOOR is DISABLED!");
			break;
		default:
			break;
	}
	PRINTF("\r\n");
	return 0;
}

uint32_t getDestAddress(uint32_t section)
{
	return  pflashBlockBase + (pflashTotalSize - pflashSectorSize * section);
}
void EraseSection(int section)
{
	/* Erase a sector from destAdrss. */
	destAdrss =  getDestAddress(section);
	PRINTF("destAdrss = 0x%x\r\n" , destAdrss);

	result = FLASH_Erase(&flashDriver, destAdrss, pflashSectorSize, kFLASH_apiEraseKey);
	if (kStatus_FLASH_Success != result)
	{
		halt(401);
	}

	/* Verify sector if it's been erased. */
	result = FLASH_VerifyErase(&flashDriver, destAdrss, pflashSectorSize, kFLASH_marginValueUser);
	if (kStatus_FLASH_Success != result)
	{
		 halt(400);
	}
}
void Write8BytesToSection(int section, uint32_t *data, uint32_t index)
{
	destAdrss =  getDestAddress(section);
	//PRINTF("destAdrss = 0x%x\r\n" , destAdrss);

	result = FLASH_Program(&flashDriver, destAdrss + index, data, 8);
	if (kStatus_FLASH_Success != result)
	{
		halt(402);
	}

	/* Program Check user margin levels */
	result = FLASH_VerifyProgram(&flashDriver, destAdrss + index, 8, data, kFLASH_marginValueUser, &failAddr,
								 &failDat);
	if (kStatus_FLASH_Success != result)
	{
		halt(404);
	}
}
int WriteSerialAndPartNumber(uint8_t *buffer, int size)
{

	/* Erase a sector from destAdrss. */
	destAdrss =  getDestAddress(GENERAL_SECTION_AND_SERIAL);
	PRINTF("destAdrss = 0x%x\r\n" , destAdrss);

	uint32_t data[1024];
	memset(data , 0 , sizeof(data));
	data[0] = size;
	int j = 1;
	for (int i = 0; i < size; i+=4)
	{
		data[j] = (*(buffer + i + 3) << 24) |  (*(buffer + i + 2) << 16) | (*(buffer + i + 1) << 8) | *(buffer + i);
		PRINTF("data[%d] = 0x%x\r\n" , j, data[j]);
		j++;
	}

	result = FLASH_Erase(&flashDriver, destAdrss, pflashSectorSize, kFLASH_apiEraseKey);
	if (kStatus_FLASH_Success != result)
	{
		halt(401);
	}

	/* Verify sector if it's been erased. */
	result = FLASH_VerifyErase(&flashDriver, destAdrss, pflashSectorSize, kFLASH_marginValueUser);
	if (kStatus_FLASH_Success != result)
	{
		 halt(400);
	}


	result = FLASH_Program(&flashDriver, destAdrss, data, sizeof(data));
	if (kStatus_FLASH_Success != result)
	{
		halt(402);
	}

	/* Program Check user margin levels */
	result = FLASH_VerifyProgram(&flashDriver, destAdrss, sizeof(data), data, kFLASH_marginValueUser, &failAddr,
								 &failDat);
	if (kStatus_FLASH_Success != result)
	{
		halt(404);
	}

	destAdrss =  getDestAddress(GENERAL_SECTION_AND_SERIAL);
	uint32_t *ptr = (uint32_t *)destAdrss;

	memcpy((uint8_t *)data , ptr, 8);

	for (int i = 0 ; i < 2; i++)
	{
		PRINTF("data[%d] = %x\r\n" , i, data[i]);
	}

	return 1;
}

void showError(int error)
{

	switch (error)
	{


		case kStatus_FLASH_InvalidArgument:
			PRINTF("kStatus_FLASH_InvalidArgument\r\n");
		break;
		case kStatus_FLASH_SizeError:
			PRINTF("kStatus_FLASH_SizeError\r\n");
		break;
		case kStatus_FLASH_AlignmentError:
			PRINTF("kStatus_FLASH_AlignmentError\r\n");
		break;
		case kStatus_FLASH_AddressError:
			PRINTF("kStatus_FLASH_AddressError\r\n");
		break;
		case kStatus_FLASH_AccessError:
			PRINTF("kStatus_FLASH_AccessError\r\n");
		break;
		case kStatus_FLASH_ProtectionViolation:
			PRINTF("kStatus_FLASH_ProtectionViolation\r\n");
		break;
		case kStatus_FLASH_CommandFailure:
			PRINTF("kStatus_FLASH_CommandFailure\r\n");
		break;
		case kStatus_FLASH_UnknownProperty:
			PRINTF("kStatus_FLASH_UnknownProperty\r\n");
		break;
		case kStatus_FLASH_RegionExecuteOnly:
			PRINTF("kStatus_FLASH_RegionExecuteOnly\r\n");
		break;
		case kStatus_FLASH_PartitionStatusUpdateFailure:
			PRINTF("kStatus_FLASH_PartitionStatusUpdateFailure\r\n");
		break;
		case kStatus_FLASH_SetFlexramAsEepromError:
			PRINTF("kStatus_FLASH_SetFlexramAsEepromError\r\n");
		break;
		case kStatus_FLASH_RecoverFlexramAsRamError:
			PRINTF("kStatus_FLASH_RecoverFlexramAsRamError\r\n");
		break;
		case kStatus_FLASH_SetFlexramAsRamError:
			PRINTF("kStatus_FLASH_RecoverFlexramAsRamError\r\n");
		break;
		case kStatus_FLASH_RecoverFlexramAsEepromError:
			PRINTF("kStatus_FLASH_RecoverFlexramAsEepromError\r\n");
		break;
		case kStatus_FLASH_CommandNotSupported:
			PRINTF("kStatus_FLASH_CommandNotSupported\r\n");
		break;
		case kStatus_FLASH_SwapSystemNotInUninitialized:
			PRINTF("kStatus_FLASH_SwapSystemNotInUninitialized\r\n");
		break;
		case kStatus_FLASH_SwapIndicatorAddressError:
			PRINTF("kStatus_FLASH_SwapIndicatorAddressError\r\n");
		break;

	}
}


