################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/adc.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/agc.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/algo.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/amp.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/calcdr.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/calreader.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/diag.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/dr_threshold_algo.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/equal_energy_algo.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/flash.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/freertos_i2c.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/kjmonitor.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/rfoff.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/rtostimertick.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/sortalgo.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/spi_master.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/timer.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/top_percentage_algo.c \
C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/wdog.c 

OBJS += \
./source/adc.o \
./source/agc.o \
./source/algo.o \
./source/amp.o \
./source/calcdr.o \
./source/calreader.o \
./source/diag.o \
./source/dr_threshold_algo.o \
./source/equal_energy_algo.o \
./source/flash.o \
./source/freertos_i2c.o \
./source/kjmonitor.o \
./source/rfoff.o \
./source/rtostimertick.o \
./source/sortalgo.o \
./source/spi_master.o \
./source/timer.o \
./source/top_percentage_algo.o \
./source/wdog.o 

C_DEPS += \
./source/adc.d \
./source/agc.d \
./source/algo.d \
./source/amp.d \
./source/calcdr.d \
./source/calreader.d \
./source/diag.d \
./source/dr_threshold_algo.d \
./source/equal_energy_algo.d \
./source/flash.d \
./source/freertos_i2c.d \
./source/kjmonitor.d \
./source/rfoff.d \
./source/rtostimertick.d \
./source/sortalgo.d \
./source/spi_master.d \
./source/timer.d \
./source/top_percentage_algo.d \
./source/wdog.d 


# Each subdirectory must supply rules for building sources it contributes
source/adc.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/adc.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/agc.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/agc.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/algo.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/algo.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/amp.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/amp.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/calcdr.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/calcdr.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/calreader.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/calreader.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/diag.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/diag.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/dr_threshold_algo.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/dr_threshold_algo.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/equal_energy_algo.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/equal_energy_algo.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/flash.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/flash.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/freertos_i2c.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/freertos_i2c.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/kjmonitor.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/kjmonitor.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/rfoff.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/rfoff.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/rtostimertick.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/rtostimertick.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/sortalgo.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/sortalgo.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/spi_master.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/spi_master.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/timer.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/timer.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/top_percentage_algo.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/top_percentage_algo.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '

source/wdog.o: C:/Topaz/SDK_2.0_FRDM-K64F/boards/frdmk64f/rtos_examples/freertos_i2c/wdog.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


