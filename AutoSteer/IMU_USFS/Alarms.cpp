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
#include "Alarms.h"

Alarms::Alarms()
{
}

/**
* @fn: blink_IndLED(uint8_t num, uint8_t ontime,uint8_t repeat);
*
* @brief: Flashes red LED indicator; blocking function, do not use in main loop
* 
* @params: Blinks per ccle, on time, number of cycles
* @returns:
*/
void Alarms::blink_IndLED(uint8_t num, uint8_t ontime,uint8_t repeat) 
{
  uint8_t i, r;
  for(r=0; r<repeat; r++)
  {
    for(i=0; i<num; i++)
    {
      Alarms::toggle_IndLED();
      delay(ontime);
    }
    //wait 60 ms
    delay(60);
  }
}

/**
* @fn: LED On, off, toggle functions
*
* @brief: Fast direct manipulation functions for green/yellow LED
* 
* @params:
* @returns:
*/
void Alarms::IndLEDon()
{
  LEDPIN_ON;
}

void Alarms::IndLEDoff()
{
  LEDPIN_OFF;
}

void Alarms::toggle_IndLED()
{
  LEDPIN_TOGGLE;
}
