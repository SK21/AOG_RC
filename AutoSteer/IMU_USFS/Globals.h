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

#ifndef Globals_h
#define Globals_h

#include "def.h"
#include "Types.h"

uint8_t                                 drdy       = 1;                                                                                                   // Set to "1" to force Sentral status register read entering the "loop()" function
uint32_t                                Start_time = 0;
float                                   TimeStamp = 0.0f;
uint32_t                                currentTime = 0;
uint32_t                                previousTime = 0;
int                                     serial_input = 0;
uint32_t                                last_refresh = 0;
uint32_t                                delt_t = 0;
uint32_t                                cycleTime = 0;                                                                                                    // Main loop time (us)
uint16_t                                state_data_Index = 0;
uint16_t                                calibratingA[2] = {0, 0};                                                                                         // Acc calibration
uint8_t                                 eventStatus[2], Sentral_WS_valid[2], Accel_Cal_valid[2] = {0, 0}, algostatus[2];                                  // EM7180 specific
uint8_t                                 Quat_flag[2] = {0, 0}, Gyro_flag[2] = {0, 0}, Acc_flag[2] = {0, 0}, Mag_flag[2] = {0, 0}, Baro_flag[2] = {0, 0};
int16_t                                 gyroADC[2][3], acc[2][3], accADC[2][3], acc_calADC[2][3], magADC[2][3], rawPressure[2], rawTemperature[2];
float                                   gyroData[2][3] = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
float                                   AngAcc[2][3]   = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
float                                   accData[2][3]  = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
float                                   magData[2][3]  = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
float                                   pressure[2]    = {0.0f, 0.0f};
float                                   temperature[2] = {0.0f, 0.0f};
int16_t                                 accLIN[2][3], accSmooth[2][3];
float                                   LINaccData[2][3] = {0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f};
int64_t                                 a_acc[2][3] = {0, 0, 0, 0, 0, 0}, b_acc[2][3] = {0, 0, 0, 0, 0, 0};
float                                   qt[2][4] = {1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f};                                                      // Quaternions
int16_t                                 QT_Timestamp[2];                                                                                                  // Quaternion timestamps
float                                   angle[2][2] = {0, 0, 0, 0};                                                                                       // P,R Euler angles
float                                   heading[2] = {0.0f, 0.0f};                                                                                        // Heading Euler angles
global_conf_t                           global_conf;
Sentral_WS_params                       WS_params;

#endif // Globals_h
