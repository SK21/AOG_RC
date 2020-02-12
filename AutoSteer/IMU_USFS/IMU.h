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

#ifndef IMU_h
#define IMU_h

#include "def.h"
#include "EM7180.h"

extern uint32_t                         Start_time;
extern float                            TimeStamp;
extern float                            BaroAlt[2];
extern float                            EstAlt[2];
extern int16_t                          accLIN[2][3];
extern float                            RMS_accLIN[2];
extern float                            heading[2];
extern float                            angle[2][2];
extern int16_t                          accSmooth[2][3];

class IMU
{
  public:
                                        IMU(EM7180*, uint8_t);
	  void                                computeIMU();
    EM7180*                             Sentral;                                                 // Class constructor variable
    uint8_t                             SensorNum;                                               // Class constructor variable
  private:
};

#endif // IMU_h
