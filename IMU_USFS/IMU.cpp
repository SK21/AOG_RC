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

#include "Arduino.h"
#include "IMU.h"

IMU::IMU(EM7180* sentral, uint8_t sensornum)
{
  Sentral = sentral;
  SensorNum = sensornum;
}

/**
* @fn: computeIMU ();
*
* @brief: Calculates state estimate
* 
* @params: 
* @returns: 
*/
void IMU::computeIMU ()
{
  float a11[2], a21[2], a31[2], a32[2], a33[2];
  float yaw[2];
  static float buff_roll[2] = {0.0f, 0.0f}, buff_pitch[2] = {0.0f, 0.0f}, buff_heading[2] = {0.0f, 0.0f};

  // Pass-thru for future filter experimentation
  accSmooth[SensorNum][0] = accADC[SensorNum][0];
  accSmooth[SensorNum][1] = accADC[SensorNum][1];
  accSmooth[SensorNum][2] = accADC[SensorNum][2];

  Sentral->getQUAT();
 
  // Only five elements of the rotation matrix are necessary to calculate the three Euler angles
  a11[SensorNum] = qt[SensorNum][0]*qt[SensorNum][0]+qt[SensorNum][1]*qt[SensorNum][1]
                   -qt[SensorNum][2]*qt[SensorNum][2]-qt[SensorNum][3]*qt[SensorNum][3];
  a21[SensorNum] = 2.0f*(qt[SensorNum][0]*qt[SensorNum][3]+qt[SensorNum][1]*qt[SensorNum][2]);
  a31[SensorNum] = 2.0f*(qt[SensorNum][1]*qt[SensorNum][3]-qt[SensorNum][0]*qt[SensorNum][2]);
  a32[SensorNum] = 2.0f*(qt[SensorNum][0]*qt[SensorNum][1]+qt[SensorNum][2]*qt[SensorNum][3]);
  a33[SensorNum] = qt[SensorNum][0]*qt[SensorNum][0]-qt[SensorNum][1]*qt[SensorNum][1]
                   -qt[SensorNum][2]*qt[SensorNum][2]+qt[SensorNum][3]*qt[SensorNum][3];

  // Pass-thru for future filter experimentation
  buff_roll[SensorNum]    = (atan2(a32[SensorNum], a33[SensorNum]))*(57.2957795f);                                                          // Roll Right +ve
  buff_pitch[SensorNum]   = -(asin(a31[SensorNum]))*(57.2957795f);                                                                          // Pitch Up +ve
  buff_heading[SensorNum] = (atan2(a21[SensorNum], a11[SensorNum]))*(57.2957795f);                                                          // Yaw CW +ve
  
  angle[SensorNum][0] = buff_roll[SensorNum];
  angle[SensorNum][1] = buff_pitch[SensorNum];
  yaw[SensorNum]      = buff_heading[SensorNum];
  heading[SensorNum]  = yaw[SensorNum] + MAG_DECLINIATION;
  if(heading[SensorNum] < 0.0f) heading[SensorNum] += 360.0f;                                                                               // Convert heading to 0 - 360deg range
  TimeStamp           = ((float)micros() - (float)Start_time)/1000000.0f;
}
