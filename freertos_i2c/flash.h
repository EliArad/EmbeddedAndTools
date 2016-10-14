#ifndef _FLASH_HEADER_FILE
#define _FLASH_HEADER_FILE

int flash_init(void);
int WriteSerialAndPartNumber(uint8_t *buffer, int size);
void showError(int error);
uint32_t getDestAddress(uint32_t section);
void Write8BytesToSection(int section, uint32_t *data, uint32_t index);
void EraseSection(int section);

#define  GENERAL_SECTION_AND_SERIAL   4

#define  GENERAL_SECTION_AB_REFLECTED 8
#define  GENERAL_SECTION_AB_FORWARD   10

#endif
