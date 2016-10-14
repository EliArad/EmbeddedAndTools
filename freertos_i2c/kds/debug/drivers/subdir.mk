################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_adc16.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_clock.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_common.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_dspi.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_flash.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_gpio.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_i2c.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_lptmr.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_rcm.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_smc.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_uart.c \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_wdog.c 

OBJS += \
./drivers/fsl_adc16.o \
./drivers/fsl_clock.o \
./drivers/fsl_common.o \
./drivers/fsl_dspi.o \
./drivers/fsl_flash.o \
./drivers/fsl_gpio.o \
./drivers/fsl_i2c.o \
./drivers/fsl_lptmr.o \
./drivers/fsl_rcm.o \
./drivers/fsl_smc.o \
./drivers/fsl_uart.o \
./drivers/fsl_wdog.o 

C_DEPS += \
./drivers/fsl_adc16.d \
./drivers/fsl_clock.d \
./drivers/fsl_common.d \
./drivers/fsl_dspi.d \
./drivers/fsl_flash.d \
./drivers/fsl_gpio.d \
./drivers/fsl_i2c.d \
./drivers/fsl_lptmr.d \
./drivers/fsl_rcm.d \
./drivers/fsl_smc.d \
./drivers/fsl_uart.d \
./drivers/fsl_wdog.d 


# Each subdirectory must supply rules for building sources it contributes
drivers/fsl_adc16.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_adc16.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_clock.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_clock.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_common.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_common.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_dspi.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_dspi.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_flash.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_flash.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_gpio.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_gpio.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_i2c.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_i2c.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_lptmr.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_lptmr.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_rcm.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_rcm.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_smc.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_smc.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_uart.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_uart.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

drivers/fsl_wdog.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_wdog.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


