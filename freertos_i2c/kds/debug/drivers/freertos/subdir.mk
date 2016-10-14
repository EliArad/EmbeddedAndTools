################################################################################
# Automatically-generated file. Do not edit!
################################################################################

# Add inputs and outputs from these tool invocations to the build variables 
C_SRCS += \
C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_i2c_freertos.c 

OBJS += \
./drivers/freertos/fsl_i2c_freertos.o 

C_DEPS += \
./drivers/freertos/fsl_i2c_freertos.d 


# Each subdirectory must supply rules for building sources it contributes
drivers/freertos/fsl_i2c_freertos.o: C:/Topaz/SDK_2.0_FRDM-K64F/devices/MK64F12/drivers/fsl_i2c_freertos.c
	@echo 'Building file: $<'
	@echo 'Invoking: Cross ARM C Compiler'
	arm-none-eabi-gcc -mcpu=cortex-m4 -mthumb -mfloat-abi=hard -mfpu=fpv4-sp-d16 -O0 -fmessage-length=0 -fsigned-char -ffunction-sections -fdata-sections -Wall  -g -DDEBUG -DCPU_MK64FN1M0VMD12 -DFSL_RTOS_FREE_RTOS -DFRDM_K64F -DFREEDOM -I../../../../../../rtos/freertos_8.2.3/Source/portable/GCC/ARM_CM4F -I../../../../../../rtos/freertos_8.2.3/Source/include -I../../../../../../CMSIS/Include -I../../../../../../devices -I../../../../../../devices/MK64F12/drivers -I../.. -I../../../../../../devices/MK64F12/utilities -I../../../../../../rtos/freertos_8.2.3/Source -I../../../.. -I../../../../../../devices/MK64F12 -std=gnu99 -fno-common  -ffreestanding  -fno-builtin  -mapcs -MMD -MP -MF"$(@:%.o=%.d)" -MT"$@" -c -o "$@" "$<"
	@echo 'Finished building: $<'
	@echo ' '


