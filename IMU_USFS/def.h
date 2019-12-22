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

#ifndef def_h
#define def_h

#include "config.h"

/*************************************************************************************************/
/*************                                                                     ***************/
/*************              IMU Orientations and Sensor Definitions                ***************/
/*************                                                                     ***************/
/*************************************************************************************************/
#if defined(USFS)
  #define SENTRAL

  // Conventional Orientation
  // Should match the EM7180 Accel calibration matrix
  #define ACC_NORTH 0        // MPU9250 X-axis
  #define ACC_EAST  1        // MPU9250 Y-axis
  #define ACC_DOWN  2        // MPU9250 Z-axis
  #define MAG_NORTH 0        // AK8963 X-axis
  #define MAG_EAST  1        // AK8963 Y-axis
  #define MAG_DOWN  2        // AK8963 Z-axis

  // This block of sensor rotation definitions can be used to rotate individual sensor axes w.r.t Sentral NED conventions
  // No transformation for EM7180 scaled data output is (X=+X, Y=+Y, Z=+Z)
  #define ACC_ORIENTATION(X, Y, Z)      {accADC[SensorNum][NORTH] = +X;       accADC[SensorNum][EAST]  = +Y;      accADC[SensorNum][DOWN]  = +Z;}

  // No transformation for EM7180 scaled data output is (X=+X, Y=+Y, Z=+Z)
  #define ACC_CAL_ORIENTATION(X, Y, Z)  {acc_calADC[SensorNum][0] = +X;       acc_calADC[SensorNum][1] = +Y;      acc_calADC[SensorNum][2] = +Z;}

  // No transformation for EM7180 scaled data output (X=+X, Y=+Y, Z=+Z)
  #define MAG_ORIENTATION(X, Y, Z)      {magADC[SensorNum][NORTH] = +X;       magADC[SensorNum][EAST]  = +Y;      magADC[SensorNum][DOWN]  = +Z;}

  // No transformation for EM7180 scaled data output (X=+X, Y=+Y, Z=+Z)
  #define GYRO_ORIENTATION(X, Y, Z)     {gyroADC[SensorNum][0]    = +X;       gyroADC[SensorNum][1]    = +Y;      gyroADC[SensorNum][2]    = +Z;}
#endif
#if defined(SENTRAL)
  #define GYRO                                  1
  #define MAG                                   1
  #define ACC                                   1
  #define BARO                                  1

  // Sentral I2C EEPROM
  #define M24512DFM_DATA_ADDRESS                0x50
  #define M24512DFM_IDPAGE_ADDRESS              0x58

  // MPU LPF's
  #if defined(LSM6DSM_GYRO_LPF_167) || defined(LSM6DSM_GYRO_LPF_223) || defined(LSM6DSM_GYRO_LPF_314) \
    || defined(LSM6DSM_GYRO_LPF_655)
  
    #if defined(LSM6DSM_GYRO_LPF_167)
      #define LSM6DSM_GYRO_DLPF_CFG             0x00
    #endif
    #if defined(LSM6DSM_GYRO_LPF_223)
      #define LSM6DSM_GYRO_DLPF_CFG             0x01
    #endif
    #if defined(LSM6DSM_GYRO_LPF_314)
      #define LSM6DSM_GYRO_DLPF_CFG             0x02
    #endif
    #if defined(LSM6DSM_GYRO_LPF_655)
      #define LSM6DSM_GYRO_DLPF_CFG             0x03
    #endif
  #else
      //Default settings LPF 314Hz
      #define LSM6DSM_GYRO_DLPF_CFG             0x02
  #endif
  
  #if defined(LSM6DSM_ACC_LPF_ODR_DIV2) || defined(LSM6DSM_ACC_LPF_ODR_DIV4) || defined(LSM6DSM_ACC_LPF_ODR_DIV9) \
    || defined(LSM6DSM_ACC_LPF_ODR_DIV50) || defined(LSM6DSM_ACC_LPF_ODR_DIV100) || defined(LSM6DSM_ACC_LPF_ODR_DIV400)
  
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV2)
      #define LSM6DSM_ACC_DLPF_CFG              0x00
    #endif
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV4)
      #define LSM6DSM_ACC_DLPF_CFG              0x01
    #endif
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV9)
      #define LSM6DSM_ACC_DLPF_CFG              0x02
    #endif
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV50)
      #define LSM6DSM_ACC_DLPF_CFG              0x03
    #endif
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV100)
      #define LSM6DSM_ACC_DLPF_CFG              0x04
    #endif
    #if defined(LSM6DSM_ACC_LPF_ODR_DIV400)
      #define LSM6DSM_ACC_DLPF_CFG              0x05
    #endif
  #else
      //Default settings LPF ODR/9
      #define LSM6DSM_ACC_DLPF_CFG              0x02
  #endif
#endif // Sentral

#define SENTRAL_UT_PER_COUNT                    0.0305176f;
#define DPS_PER_COUNT                           0.1525878906f;
#define G_PER_COUNT                             0.0004882813f;

/*************************************************************************************************/
/*************                                                                     ***************/
/*************                       Error Checking Section                        ***************/
/*************                                                                     ***************/
/*************************************************************************************************/
#ifndef SENTRAL
  #error "Does this board have an EM7180?"
#endif

#endif // def_h
