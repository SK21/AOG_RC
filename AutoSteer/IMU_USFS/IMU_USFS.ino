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
 /*
   This utility is written specifically to work with the "Dragonfly" and "Butterfly" STM32L4 microcontroller development boards
 */

// ******************************************************************************

 // 22/Dec/2019
 // to enable the calibration mode uncomment //#define SERIAL_DEBUG in the config.h file

 // 5 pins need to be connected:
 // GND
 // 3.3V
 // enter these values in the config.h file
 // SCL		pin 5, D1
 // SDA		pin 4, D2
 // INT		pin 12, D6

// ******************************************************************************

#include <Wire.h>
#include "I2Cdev.h"
#include "EM7180.h"
#include "IMU.h"
#include "Alarms.h"
#include "Globals.h"
#include "Types.h"
#include "def.h"

// Define function pointers for class instances
I2Cdev      *i2c_0;
EM7180      *Sentral_0;
IMU         *imu_0;

// Declare main loop helper functions
void DRDY_Handler();
void FetchEventStatus(I2Cdev* i2c_BUS, uint8_t sensorNUM);
void FetchSentralData(EM7180* em7180, IMU* IMu, uint8_t sensorNUM);

void setup()
{
  // Open serial port
  Serial.begin(38400);

  // Power up on-board USFS
  pinMode(USFS_GND, OUTPUT);
  digitalWrite(USFS_GND, LOW);
  pinMode(USFS_VCC, OUTPUT);
  digitalWrite(USFS_VCC, HIGH);
  delay(2000);
  // Assign Indicator LED
  LEDPIN_PINMODE;
  Alarms::IndLEDon();

  // Assign interrupt pin as input
  pinMode(INT_PIN, INPUT);
   
  // Instantiate Wire class for I2C
  SENSOR_0_WIRE_INSTANCE.begin(SDA_PIN, SCL_PIN);
  delay(100);
  SENSOR_0_WIRE_INSTANCE.setClock(400000);
  delay(500);

  // Instantiate Sentral_0 classes and create function pointers
  i2c_0     = new I2Cdev(&SENSOR_0_WIRE_INSTANCE);                                                                   // Use class instance/function pointer to specify I2C bus (may be more than one)
  Sentral_0 = new EM7180(i2c_0, 0);                                                                                  // Use class instance/function pointer to specify Sentral board (may be more than one)
  imu_0     = new IMU(Sentral_0, 0);                                                                                 // Use class instance/function pointer to specify the Sentral and I2C instances for the IMU calcs

  // Initialize Sentral_0
  #ifdef SERIAL_DEBUG
    Serial.print("Initializing Sentral_0");
    Serial.println("");
  #endif

  // Main function to set up Sentral and sensors 
  Sentral_0->initSensors();

  // Give a little time to see startup results
  delay(2000);

  // Boot Complete, blink 5 times
  Alarms::blink_IndLED(2, 40, 5);
  
  // Spreadsheet output column headings when "SERIAL_DEBUG" is not defined in config.h
  #ifndef SERIAL_DEBUG
    Serial.print("Timestamp, q0, q1, q2, q3, Heading, Pitch, Roll"); Serial.println("");
  #endif

  // Attach adat ready interrupt
  attachInterrupt(INT_PIN, DRDY_Handler, RISING);

  // Set sketch start time
  Start_time = micros();
}

void loop()
{
  // Calculate loop cycle time
  currentTime = micros();
  cycleTime = currentTime - previousTime;
  previousTime = currentTime;
  
  // Check for user input. Allows the user to execute calibration functions from a serial monitor
  if(Serial.available()) serial_input = Serial.read();
  if (serial_input == 49 && calibratingA[0] < 1) {calibratingA[0] = 512;;}                                           // Type "1" to initiate Sentral_0 Accel Cal
  if (serial_input == 50 && calibratingA[0] < 1) {Sentral_0->Save_Sentral_WS_params();}                              // Type "2" to initiate Sentral_0 "Warm Start" parameter save
  serial_input = 0;

  // See what data are available from the Sentral by polling the Status register
  if(drdy == 1)
  {
    drdy = 0;
    FetchEventStatus(i2c_0, 0);                                                                                      // I2C instance 0, Sensor instance 0 (and implicitly Sentral instance 0)
  }

  // Acquire data the Sentral
  FetchSentralData(Sentral_0, imu_0, 0);                                                                             // Sentral instance 0, IMU calculation instance 0 and Sensor instance 0
  
  // Update serial
  delt_t = millis() - last_refresh;
  if (delt_t > UPDATE_PERIOD)                                                                                        // Update the serial monitor every "UPDATE_PERIOD" ms
  {
    last_refresh = millis();
    #ifdef SERIAL_DEBUG

      // Algorithm status
      Serial.print("Algorithm Status = "); Serial.print(algostatus[0]); Serial.println(""); Serial.println("");

       // Sentral_0 sensor and raw quaternion outout
      Serial.print("ax_0 = "); Serial.print((int)(1000.0f*accData[0][0])); Serial.print(" ay_0 = "); Serial.print((int)(1000.0f*accData[0][1]));
      Serial.print(" az_0 = "); Serial.print((int)(1000.0f*accData[0][2])); Serial.println(" mg");
      Serial.print("gx_0 = "); Serial.print(gyroData[0][0], 2); Serial.print(" gy_0 = "); Serial.print(gyroData[0][1], 2); 
      Serial.print(" gz_0 = "); Serial.print(gyroData[0][2], 2); Serial.println(" deg/s"); Serial.println("");
      Serial.print("mx_0 = "); Serial.print((int)magData[0][0]); Serial.print(" my_0 = "); Serial.print((int)magData[0][1]);
      Serial.print(" mz_0 = "); Serial.print((int)magData[0][2]); Serial.println(" uT"); Serial.println("");
      Serial.println("Sentral_0 Quaternion (NED):"); Serial.print("Q0_0 = "); Serial.print(qt[0][0]);
      Serial.print(" Qx_0 = "); Serial.print(qt[0][1]); Serial.print(" Qy_0 = "); Serial.print(qt[0][2]); 
      Serial.print(" Qz_0 = "); Serial.print(qt[0][3]); Serial.println(""); Serial.println("");
      
      // Euler angles
      Serial.print("Sentral_0 Yaw, Pitch, Roll: ");
      Serial.print(heading[0], 2); Serial.print(", "); Serial.print(angle[0][1], 2); Serial.print(", "); Serial.println(angle[0][0], 2);
      Serial.println("");

      // Temperature and pressure
      Serial.print("Baro Pressure: "); Serial.print(pressure[0], 2); Serial.print(" mbar"); Serial.println("");
      Serial.print("Baro Temperature: "); Serial.print(temperature[0], 2); Serial.print(" deg C"); Serial.println("");
      Serial.println("");

      // Loop cycle time
      Serial.print("Loop Cycletime:"); Serial.print(cycleTime); Serial.println(" us"); Serial.println("");

      // Hotkey messaging
      if(calibratingA[0] < 1)
      {
        Serial.println("Send '1' for Sentral_0 Accel Cal");
        Serial.println("Make sure the desired accelerometer axis is properly aligned with gravity and remains still");
        Serial.println("All three accelerometers must to be calibrated in the +/-1g condition for accurate results");
      }
      Serial.println("Send '2' to save Sentral_0 Warm Start params");
      Serial.println("");
    #endif

    // Spreadsheet output when "SERIAL_DEBUG" is not defined in config.h
    #ifndef SERIAL_DEBUG
      //Serial.print(TimeStamp, 2);              Serial.print(","); Serial.print(qt[0][0], 2);         Serial.print(","); Serial.print(qt[0][1], 2);         
      //Serial.print(",");                       Serial.print(qt[0][2], 2);  Serial.print(",");        Serial.print(qt[0][3], 2); Serial.print(",");
      //Serial.print(heading[0], 2);             Serial.print(","); Serial.print(angle[0][1], 2);      Serial.print(","); Serial.print(angle[0][0], 2);
      //Serial.println("");
       
      CommToAutoSteer();
    #endif

    // Toggle LED unless calibrating accelerometers
    if(calibratingA[0] < 1)
    {
      Alarms::toggle_IndLED();
    } else
    {
      Alarms::IndLEDoff();
    }
  }
}

void DRDY_Handler()
{
  drdy = 1;
}

void FetchEventStatus(I2Cdev* i2c_BUS, uint8_t sensorNUM)
{
    eventStatus[sensorNUM] = i2c_BUS->readByte(EM7180_ADDRESS, EM7180_EventStatus);
    if(eventStatus[sensorNUM] & 0x04) Quat_flag[sensorNUM] = 1;
    if(eventStatus[sensorNUM] & 0x20) Gyro_flag[sensorNUM] = 1;
    if(eventStatus[sensorNUM] & 0x10) Acc_flag[sensorNUM]  = 1;
    if(eventStatus[sensorNUM] & 0x08) Mag_flag[sensorNUM]  = 1;
    if(eventStatus[sensorNUM] & 0x40) Baro_flag[sensorNUM] = 1;
    algostatus[sensorNUM] = i2c_BUS->readByte(EM7180_ADDRESS, EM7180_AlgorithmStatus);
}

void FetchSentralData(EM7180* em7180, IMU* IMu, uint8_t sensorNUM)
{
  if(Gyro_flag[sensorNUM] == 1)
  {
    em7180->Gyro_getADC();
    for(uint8_t i=0; i<3; i++)
    {
      gyroData[sensorNUM][i] = (float)gyroADC[sensorNUM][i]*DPS_PER_COUNT;
    }
    Gyro_flag[sensorNUM] = 0;
  }
  if(Quat_flag[sensorNUM] == 1)
  {
    IMu->computeIMU();
    Quat_flag[sensorNUM] = 0;
  }
  if(Acc_flag[sensorNUM])
  {
    em7180->ACC_getADC();
    em7180->ACC_Common();
    for(uint8_t i=0; i<3; i++)
    {
      accData[sensorNUM][i] = (float)accADC[sensorNUM][i]*G_PER_COUNT;
      LINaccData[sensorNUM][i] = (float)accLIN[sensorNUM][i]*G_PER_COUNT;
    }
    Acc_flag[sensorNUM] = 0;
  }
  if(Mag_flag[sensorNUM])
  {
    em7180->Mag_getADC();
    for(uint8_t i=0; i<3; i++)
    {
      magData[sensorNUM][i] = (float)magADC[sensorNUM][i]*SENTRAL_UT_PER_COUNT;
    }
    Mag_flag[sensorNUM] = 0;
  }
  if(Baro_flag[sensorNUM])
  {
    rawPressure[sensorNUM]    = em7180->Baro_getPress();
    pressure[sensorNUM]       = (float)rawPressure[sensorNUM]*0.01f +1013.25f;                                       // Pressure in mBar
    rawTemperature[sensorNUM] = em7180->Baro_getTemp();
    temperature[sensorNUM]    = (float)rawTemperature[sensorNUM]*0.01;                                               // Temperature in degrees C
    Baro_flag[sensorNUM]      = 0;
  }
}
