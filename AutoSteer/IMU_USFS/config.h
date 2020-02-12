/*
 * Copyright (c) 2019 Gregory Tomasch.  All rights reserved.
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal with the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 *  1. Redistributions of source code must retain the above copyright notice,
 *     this list of conditions and the following disclaimers.
 *  2. Redistributions in binary form must reproduce the above copyright
 *     notice, this list of conditions and the following disclaimers in the
 *     documentation and/or other materials provided with the distribution.
 *  3. The names of Gregory Tomasch and his successors
 *     may not be used to endorse or promote products derived from this Software
 *     without specific prior written permission.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
 * CONTRIBUTORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
 * WITH THE SOFTWARE.
 */

#ifndef config_h
#define config_h

/*************************************************************************************************/
/*************                                                                     ***************/
/*************                     SECTION  1 - BASIC SETUP                        ***************/
/*************                                                                     ***************/
/*************************************************************************************************/
    //#define SERIAL_DEBUG
    
    // I2C Wire Library instance (Uncomment one only)
    #define SENSOR_0_WIRE_INSTANCE             Wire
    //#define SENSOR_0_WIRE_INSTANCE             Wire1

    #define SDA_PIN                            4
    #define SCL_PIN                            5

    #define LED_PIN                            2
    #define INT_PIN                           12

    // GPIO pins used to power the USFS if "Piggy-backed" onboard
    #define USFS_GND                           12
    #define USFS_VCC                           13

    #define UPDATE_PERIOD                      100                                                      // Screen/serial update period (ms)

/********************                   EM7180 IMU Board                   **********************/
    /* Add boards as they become available */
    #define USFS                                                                                        // Pesky Products USFS or other EM7180 + MPU9250 board
    
/**************************            Sensor Data Rates              ****************************/
    /* LSM6DSM Acc Output data rate. Uncomment only one option */
    //#define ACC_ODR                            0xA6                 // 1660Hz
    #define ACC_ODR                            0x53                 // 834Hz
    //#define ACC_ODR                            0x29                 // 416Hz
    //#define ACC_ODR                            0x14                 // 208Hz
    //#define ACC_ODR                            0x0A                 // 104Hz
    //#define ACC_ODR                            0x05                 // 52Hz
    //#define ACC_ODR                            0x02                 // 26Hz
    //#define ACC_ODR                            0x01                 // 12Hz

    /* LSM6DSM Gyro Output data rate. Uncomment only one option */
    //#define GYRO_ODR                           0xA6                 // 1660Hz
    #define GYRO_ODR                           0x53                 // 834Hz
    //#define GYRO_ODR                           0x29                 // 416Hz
    //#define GYRO_ODR                           0x14                 // 208Hz
    //#define GYRO_ODR                           0x0A                 // 104Hz
    //#define GYRO_ODR                           0x05                 // 52Hz
    //#define GYRO_ODR                           0x02                 // 26Hz
    //#define GYRO_ODR                           0x01                 // 12Hz

    /* LIS2MDL Mag Output data rate. Uncomment only one option */
    #define MAG_ODR                            0x64                 // 100Hz
    //#define MAG_ODR                            0x32                 // 50Hz
    //#define MAG_ODR                            0x14                 // 20Hz
    //#define MAG_ODR                            0x0A                 // 10Hz

    /* BMP280 Baro Output data rate. Uncomment only one option */
    //#define BARO_ODR                           0x4B                 // 75Hz
    //#define BARO_ODR                            0x32                 // 50Hz
    #define BARO_ODR                            0x19                 // 25Hz
    //define BARO_ODR                            0x0A                 // 10Hz
    //define BARO_ODR                            0x01                 // 1Hz

    /* EM7180 Quaternion rate divisor; quaternion rate is the gyro ODR (in Hz) divided by the quaternion rate divisor. Uncomment only one option */
    //#define QUAT_DIV                           0x0F                 // 16
    //#define QUAT_DIV                           0x09                 // 10
    #define QUAT_DIV                           0x07                 // 8
    //#define QUAT_DIV                           0x04                 // 5
    //#define QUAT_DIV                           0x03                 // 4
    //#define QUAT_DIV                           0x01                 // 2
    //#define QUAT_DIV                           0x00                 // 1

/**************************              Sensor Scales                ****************************/
    /* LSM6DSM Acc Output Scale. Uncomment only one option */
    //#define ACC_SCALE                          0x02                 // +/-2g
    //#define ACC_SCALE                          0x04                 // +/-4g
    #define ACC_SCALE                          0x08                 // +/-8g
    //#define ACC_SCALE                          0x10                 // +/-16g

    /* LSM6DSM Gyro Output Scale. Uncomment only one option */
    //#define GYRO_SCALE                          0x7D                // +/-125DPS
    //#define GYRO_SCALE                          0xFA                // +/-250DPS
    //#define GYRO_SCALE                          0x1F4               // +/-500DPS
    //#define GYRO_SCALE                          0x3E8               // +/-1000DPS
    #define GYRO_SCALE                          0x7D0               // +/-2000DPS

    /* LSM6DSM Acc Output Scale. Uncomment only one option */
    #define MAG_SCALE                          0x133                // +/-4915uT (0x133)

/**************************                LP Filters                 ****************************/
    /* LSM6DSM Gyro low pass filter setting. Uncomment only one option */
    #define LSM6DSM_GYRO_LPF_167
    //#define LSM6DSM_GYRO_LPF_223
    //#define LSM6DSM_GYRO_LPF_314
    //#define LSM6DSM_GYRO_LPF_655

    /* LSM6DSM Acc low pass filter setting. Uncomment only one option */
    //#define LSM6DSM_ACC_LPF_ODR_DIV2
    //#define LSM6DSM_ACC_LPF_ODR_DIV4
    //#define LSM6DSM_ACC_LPF_ODR_DIV9
    //#define LSM6DSM_ACC_LPF_ODR_DIV50
    //#define LSM6DSM_ACC_LPF_ODR_DIV100
    #define LSM6DSM_ACC_LPF_ODR_DIV400

    /* LIS2MDL Mag low pass filter setting. Uncomment only one option */
    //#define LIS2MDL_MAG_LPF                    0x00                 // ODR/2
    #define LIS2MDL_MAG_LPF                    0x01                 // ODR/4

    /* LPS22HB Mag low pass filter setting. Uncomment only one option */
    #define LPS22HB_BARO_LPF                   0x00                 // ODR/2
    //#define LPS22HB_BARO_LPF                   0x01                 // ODR/9
    //#define LPS22HB_BARO_LPF                   0x02                 // ODR/20

/**************************          Sentral Calibrations             ****************************/
      /* These configuration definitions allow the user to select whether or not to use
         the stored accel cal and WS parameters. Defined as "1", the accel/WS data will
         be loaded if valid. Defined as "0", the accel/WS data will NOT be loaded, even
         if it is valid... */

      /* Define as 0 to suppress Acc scale/offset corrections. Uncomment only one option */
      #define ACCEL_CAL                   1
      //#define ACCEL_CAL                   0

      /* Define as 0 to suppress Sentral "Warm Start" heading correction. Uncomment only one option */
      #define SENTRAL_WARM_START          1
      //#define SENTRAL_WARM_START          0

/********************                 Magnetic Declination                 ***********************/

      /* Magnetic decliniation data: http://magnetic-declination.com/
         Units are decimal degrees (Not DMS)*/
         
      //#define MAG_DECLINIATION            13.80f // For Sunnyvale, CA USA.
      //#define MAG_DECLINIATION            14.30f // For Kelseyville, CA USA.
      //#define MAG_DECLINIATION            14.05f // Composite between Sunnyvale and Kelseyville
	  #define MAG_DECLINIATION            8.06f // for Carrot River, SK Canada

/**************************            End Configuration              ****************************/

#endif
