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
#include "EM7180.h"

uint8_t rawADC[6];


EM7180::EM7180(I2Cdev* i2c, uint8_t sensornum)
{
  I2C = i2c;
  SensorNum = sensornum;
}

/**
* @fn: initSensors();
*
* @brief: Initializes EM7180 and associated sensors
* @params: void
* @returns: void
*/
void EM7180::initSensors()
{
  uint8_t STAT;
  int count = 0;

  #ifdef SERIAL_DEBUG
    Serial.print("Sentral_"); 
    Serial.print(SensorNum);
    Serial.print(":");
    Serial.println("");
    I2C->I2Cscan();
  #endif
  
  // Check SENtral status, make sure EEPROM upload of firmware was accomplished
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_SentralStatus);
  while(!(STAT & 0x01))
  {
    I2C->writeByte(EM7180_ADDRESS, EM7180_ResetRequest, 0x01);
    delay(500);  
    count++;  
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_SentralStatus);
    if(count > 100) break;
  }
  #ifdef SERIAL_DEBUG
    Serial.print("Sentral Status: "); 
    Serial.print(STAT);
    Serial.print(" (Should be 11)");
    Serial.println("");
    Serial.println("");
  #endif
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_SentralStatus);
  if(!(STAT & 0x04))
  {
    Alarms::blink_IndLED(12,100,1);
  }
  delay(100);
  #ifdef SERIAL_DEBUG
    Serial.print("Sentral firmware loaded. Fetching Accel Cal and WS parameters...");
    Serial.println("");
  #endif

  // Place SENtral in pass-through mode
  I2C->writeByte(EM7180_ADDRESS, EM7180_PassThruControl, 0x01);
  delay(5);
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
  delay(5);
  while(!(STAT & 0x01))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
    delay(5);
  }

  // Fetch Accel Caldata from I2C EEPROM
  EM7180::readAccelCal();
  
  // Fetch Warm Start data from I2C EEPROM
  EM7180::readSenParams();

  // Cancel pass-through mode
  I2C->writeByte(EM7180_ADDRESS, EM7180_PassThruControl, 0x00);
  delay(5);
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
  while((STAT & 0x01))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
    delay(5);
  }
  #ifdef SERIAL_DEBUG
    Serial.print("Done...");
    Serial.println("");
    Serial.println("");
  #endif
  
  // Print Accel Cal raw data for inspection
  #ifdef SERIAL_DEBUG
    Serial.println("Acceleromater Calibration Data:");
    Serial.print("X-acc max: "); Serial.println(global_conf.accZero_max[SensorNum][0]);
    Serial.print("Y-acc max: "); Serial.println(global_conf.accZero_max[SensorNum][1]);
    Serial.print("Z-acc max: "); Serial.println(global_conf.accZero_max[SensorNum][2]);
    Serial.print("X-acc min: "); Serial.println(global_conf.accZero_min[SensorNum][0]);
    Serial.print("Y-acc min: "); Serial.println(global_conf.accZero_min[SensorNum][1]);
    Serial.print("Z-acc min: "); Serial.println(global_conf.accZero_min[SensorNum][2]);
    Serial.println("");
    Serial.print("Checking/loading Acc Cal data...");
    Serial.println("");
  #endif

  // Be sure Sentral is in "Idle" state
  I2C->writeByte(EM7180_ADDRESS, EM7180_HostControl, 0x00);

  // Load Accel Cal
  // Check that the Acc Cal values are valid and will not crash the Sentral
  Accel_Cal_valid[SensorNum] = 1;
  for (uint8_t i = 0; i < 3; i++)
  {
    if ((global_conf.accZero_min[SensorNum][i] < -2240) || (global_conf.accZero_min[SensorNum][i] > -1800)) Accel_Cal_valid[SensorNum] = 0;
    if ((global_conf.accZero_max[SensorNum][i] < 1800) || (global_conf.accZero_max[SensorNum][i] > 2240)) Accel_Cal_valid[SensorNum] = 0;
  }
  
  // If Accel Cal data bogus, null data loaded instead
  EM7180::EM7180_acc_cal_upload();
  if(ACCEL_CAL && Accel_Cal_valid[SensorNum])
  {
    Serial.print("Acc Cal data valid...");
    Serial.println("");
    Alarms::blink_IndLED(2,100,1);                                                                                              // Blink if accel cal'd true
  } else
  {
    Serial.print("Acc Cal data invalid! Defaults loaded...");
    Serial.println("");
  }

  // Force initialize; reads Accel Cal data into static variable
  I2C->writeByte(EM7180_ADDRESS, EM7180_HostControl, 0x01);
  #ifdef SERIAL_DEBUG
    Serial.print("Done. Loading Warm Start Parameters, modifying sensor ranges and data rates...");
    Serial.println("");
  #endif
  delay(20);

  // Apply Warm Start Parameters
  if(SENTRAL_WARM_START && Sentral_WS_valid[SensorNum] == 1)
  {
    EM7180::EM7180_set_WS_params();
    Serial.print("Warm Start data loaded...");
    Serial.println("");
    Alarms::blink_IndLED(6,100,1);
  } else
  {
    Serial.print("Warm Start data NOT loaded!");
    Serial.println("");
  }

  // Set Sensor LPF bandwidth. MUST BE DONE BEFORE SETTING ODR's
  I2C->writeByte(EM7180_ADDRESS, EM7180_ACC_LPF_BW, LSM6DSM_ACC_DLPF_CFG);                                                      // Accelerometer
  I2C->writeByte(EM7180_ADDRESS, EM7180_GYRO_LPF_BW, LSM6DSM_GYRO_DLPF_CFG);                                                    // Gyroscope
  I2C->writeByte(EM7180_ADDRESS, EM7180_MAG_LPF_BW, LIS2MDL_MAG_LPF);                                                           // Magnetometer
  I2C->writeByte(EM7180_ADDRESS, EM7180_BARO_LPF_BW, LPS22HB_BARO_LPF);                                                         // Baro

  // Set accel/gyro/mage desired ODR rates
  I2C->writeByte(EM7180_ADDRESS, EM7180_AccelRate, ACC_ODR);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GyroRate, GYRO_ODR);
  I2C->writeByte(EM7180_ADDRESS, EM7180_MagRate, MAG_ODR);
  I2C->writeByte(EM7180_ADDRESS, EM7180_QRateDivisor, QUAT_DIV);
  
  // ODR + 10000000b to activate the eventStatus bit for the barometer...
  I2C->writeByte(EM7180_ADDRESS, EM7180_BaroRate, (0x80 + BARO_ODR));

  // Configure operating mode
  // Output scaled sensor data (Quaternion convention NED)
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
  
  // Enable interrupt to host upon certain events
  // Choose interrupts when: gyros updated (0x20), Sentral error (0x02) or Sentral reset (0x01)
  I2C->writeByte(EM7180_ADDRESS, EM7180_EnableEvents, 0x23);

  #ifdef SERIAL_DEBUG
    Serial.print("Done. Starting the Sentral...");
    Serial.println("");
  #endif

  // Start the Sentral
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
  #ifdef SERIAL_DEBUG
    Serial.print("Done. Loading algorithm tuning parameters...");
    Serial.println("");
  #endif

  // Perform final Sentral alogorithm parameter modifications
  EM7180::EM7180_set_integer_param (0x49, 0x00);                                                                                // Disable "Stillness" mode
  EM7180::EM7180_set_integer_param (0x48, 0x01);                                                                                // Set Gbias_mode to 1
  EM7180::EM7180_set_mag_acc_FS (MAG_SCALE, ACC_SCALE);                                                                         // Set magnetometer/accelerometer full-scale ranges
  EM7180::EM7180_set_gyro_FS (GYRO_SCALE);                                                                                      // Set gyroscope full-scale range
  EM7180::EM7180_set_float_param (0x3B, 0.0f);                                                                                  // Param 59 Mag Transient Protect off (0.0)
  //EM7180::EM7180_set_float_param (0x34, 4.0f);                                                                                  // Param 52 Mag merging rate (0.7 default)
  //EM7180::EM7180_set_float_param (0x35, 0.3f);                                                                                  // Param 53 Accel merging rate (0.6 default)

  //I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x02);                                                                // Diagnostic; reports unscaled sensor data

  // Choose interrupt events: Gyros updated (0x20), Sentral error (0x02) or Sentral reset (0x01)
  I2C->writeByte(EM7180_ADDRESS, EM7180_EnableEvents, 0x23);

  // Read event status register
  eventStatus[SensorNum] = I2C->readByte(EM7180_ADDRESS, EM7180_EventStatus);

  #ifdef SERIAL_DEBUG
    Serial.print("Sentral initialization complete!");
    Serial.println("");
    Serial.println("");
  #endif
}

/**
* @fn: i2c_getSixRawADC(uint8_t add, uint8_t reg);
*
* @brief: Sequential burst reads six bytes from an I2C slave device
* 
* @params: I2C device address, register address
* @returns: void
*/
void EM7180::i2c_getSixRawADC(uint8_t add, uint8_t reg)
{
  I2C->readBytes(add, reg, 6, rawADC);
}

/**
* @fn: uint32_reg_to_float (uint8_t *buf);
*
* @brief: Converts 4 1byte integers to a little endian float
* 
* @params: pointer to 4 byte array
* @returns: void
*/
float EM7180::uint32_reg_to_float (uint8_t *buf)
{
  union
  {
    uint32_t ui32;
    float f;
  } u;

  u.ui32 =     (((uint32_t)buf[0]) +
               (((uint32_t)buf[1]) <<  8) +
               (((uint32_t)buf[2]) << 16) +
               (((uint32_t)buf[3]) << 24));
  return u.f;
}

/**
* @fn: float_to_bytes (float param_val, uint8_t *buf);
*
* @brief: Converts float to bytes (little endian)
* 
* @params: Floating point argument, pointer to byte array
* @returns: void
*/
void EM7180::float_to_bytes (float param_val, uint8_t *buf)
{
  union
  {
    float f;
    uint8_t comp[sizeof(float)];
  } u;
  
  u.f = param_val;
  for (uint8_t i=0; i < sizeof(float); i++)
  {
    buf[i] = u.comp[i];
  }
  
  // Convert to LITTLE ENDIAN
  for (uint8_t i=0; i < sizeof(float); i++)
  {
    buf[i] = buf[(sizeof(float)-1) - i];
  }
}

/**
* @fn: getQUAT();
*
* @brief: Fetches quaternion data from EM7180 (NED orientation)
* 
* @params: void
* @returns: void
*/
void EM7180::getQUAT()
{
  uint8_t rawData[18];
  
  I2C->readBytes(EM7180_ADDRESS, EM7180_QX, 18, &rawData[0]);
  qt[SensorNum][1] = uint32_reg_to_float (&rawData[0]);
  qt[SensorNum][2] = uint32_reg_to_float (&rawData[4]);
  qt[SensorNum][3] = uint32_reg_to_float (&rawData[8]);
  qt[SensorNum][0] = uint32_reg_to_float (&rawData[12]);
  QT_Timestamp[SensorNum] = ((int16_t)(rawData[17]<<8) | rawData[16]);
}

/**
* @fn: Baro_getPress();
*
* @brief: Fetches baro pressure data from EM7180
* 
* @params: void
* @returns: int16_t pressure
*/
int16_t EM7180::Baro_getPress()
{
  uint8_t rawData[2];
  
  I2C->readBytes(EM7180_ADDRESS, EM7180_Baro, 2, &rawData[0]);
  return (int16_t) (((int16_t)rawData[1] << 8) | rawData[0]);
}

/**
* @fn: Baro_getTemp();
*
* @brief: Fetches baro temperature data from EM7180
* 
* @params: void
* @returns: int16_t pressure
*/
int16_t EM7180::Baro_getTemp()
{
  uint8_t rawData[2];
  
  I2C->readBytes(EM7180_ADDRESS, EM7180_Temp, 2, &rawData[0]);
  return (int16_t) (((int16_t)rawData[1] << 8) | rawData[0]);
}

/**
* @fn: Gyro_getADC();
*
* @brief: Fetches gro data from EM7180, applies orientation matrix (def.h)
* 
* @params: void
* @returns: void
*/
void EM7180::Gyro_getADC()
{
  EM7180::i2c_getSixRawADC(EM7180_ADDRESS, EM7180_GX);
  GYRO_ORIENTATION( ((int16_t)(rawADC[1]<<8) | rawADC[0]),                                                                      // Range: +/- 32768; +/- 2000 deg/sec
                    ((int16_t)(rawADC[3]<<8) | rawADC[2]),
                    ((int16_t)(rawADC[5]<<8) | rawADC[4]));
}

/**
* @fn: ACC_getADC();
*
* @brief: Fetches acc data from EM7180, applies orientation matrix (def.h); GUI indication only
* 
* @params: void
* @returns: void
*/
void EM7180::ACC_getADC()
{
  EM7180::i2c_getSixRawADC(EM7180_ADDRESS, EM7180_AX);
  acc[SensorNum][0] = ((int16_t)(rawADC[1]<<8) | rawADC[0]);                                                                    // Scale: 2048cts = 1g
  acc[SensorNum][1] = ((int16_t)(rawADC[3]<<8) | rawADC[2]);
  acc[SensorNum][2] = ((int16_t)(rawADC[5]<<8) | rawADC[4]);
  ACC_ORIENTATION(acc[SensorNum][0],
                  acc[SensorNum][1],
                  acc[SensorNum][2]);

  // If Accel Cal active, assign ACC_CAL_ORIENTATION definition to generate calibration values
  if(calibratingA[SensorNum] > 0)
  {
    ACC_CAL_ORIENTATION(acc[SensorNum][0],
                        acc[SensorNum][1],
                        acc[SensorNum][2]);
  }
}

/**
* @fn: LIN_ACC_getADC();
*
* @brief: Fetches linear Acc data from EM7180; world coordinate WITHOUT Mag declination applied
* 
* @params: void
* @returns: void
*/
void EM7180::LIN_ACC_getADC()
{
  EM7180::i2c_getSixRawADC(EM7180_ADDRESS, EM7180_GP8);
  accLIN[SensorNum][0] = ((int16_t)(rawADC[1]<<8) | rawADC[0]);                                                                 // Scale: 2048cts = 1g
  accLIN[SensorNum][1] = ((int16_t)(rawADC[3]<<8) | rawADC[2]);
  accLIN[SensorNum][2] = ((int16_t)(rawADC[5]<<8) | rawADC[4]);
}

/**
* @fn: ACC_Common();
*
* @brief: Executes accelerometer calibration functions
* 
* @params: void
* @returns: void
*/
void EM7180::ACC_Common()
{
  if (calibratingA[SensorNum] == 512)
  {
    // Tell the Sentral to send unscaled sensor data
    I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x02);
    delay(20);

    // Re-read Acc data in raw data mode
    EM7180::ACC_getADC();
    
  }
  if (calibratingA[SensorNum] > 0)
  {
    for (uint8_t axis = 0; axis < 3; axis++)
    {
      // Raw Acc data from Sentral is scaled according to sensor range selected; divide by appropriate factor of two to get 1g=2048cts
      if ((acc_calADC[SensorNum][axis]/(0x10/ACC_SCALE) > 1024))
      {
        // Sum up 512 readings
        a_acc[SensorNum][axis] += acc_calADC[SensorNum][axis]/(0x10/ACC_SCALE);
      }
      if ((acc_calADC[SensorNum][axis]/(0x10/ACC_SCALE)) < -1024)
      {
        b_acc[SensorNum][axis] += acc_calADC[SensorNum][axis]/(0x10/ACC_SCALE);
      }
      // Clear global variables for next reading
      acc_calADC[SensorNum][axis] = 0;
    }
    
    // Calculate averages, and store values in EEPROM at end of calibration
    if (calibratingA[SensorNum] == 1)
    {
      for (uint8_t axis = 0; axis < 3; axis++)
      {
        if (a_acc[SensorNum][axis]>>9 > 1024)
        {
          global_conf.accZero_max[SensorNum][axis] = a_acc[SensorNum][axis]>>9;
        }
        if (b_acc[SensorNum][axis]>>9 < -1024)
        {
          global_conf.accZero_min[SensorNum][axis] = b_acc[SensorNum][axis]>>9;
        }
        // Clear global variables for next time
        a_acc[SensorNum][axis] = 0;
        b_acc[SensorNum][axis] = 0;
      }

      // Save accZero to I2C EEPROM
      // Put the Sentral in pass-thru mode
      EM7180::WS_PassThroughMode();

      // Store accelerometer calibration data to the M24512DFM I2C EEPROM
      EM7180::writeAccCal();

      // Take Sentral out of pass-thru mode and re-start algorithm
      // Also resumes sending calibrated sensor data to the output registers
      EM7180::WS_Resume();
      Alarms::IndLEDon();
      Alarms::blink_IndLED(10,50,1);
    }
    calibratingA[SensorNum]--;
  }
}

/**
* @fn: EM7180_acc_cal_upload ();
*
* @brief: Utility function for EM7180 to set accelerometer scaling and offset
* 
* @params: void
* @returns: void
*/
void EM7180::EM7180_acc_cal_upload()
{
  int64_t big_cal_num;
  union
  {
    int16_t cal_num;
    unsigned char cal_num_byte[2];
  };
  
  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
    //NORTH SCALE
    big_cal_num = (4096000000/(global_conf.accZero_max[SensorNum][ACC_NORTH] - global_conf.accZero_min[SensorNum][ACC_NORTH])) - 1000000;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP36, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP37, cal_num_byte[1]);
  
  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
   // EAST SCALE
   big_cal_num = (4096000000/(global_conf.accZero_max[SensorNum][ACC_EAST] - global_conf.accZero_min[SensorNum][ACC_EAST])) - 1000000;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP38, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP39, cal_num_byte[1]);  

  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
   // DOWN SCALE
   big_cal_num = (4096000000/(global_conf.accZero_max[SensorNum][ACC_DOWN] - global_conf.accZero_min[SensorNum][ACC_DOWN])) - 1000000;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP40, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP50, cal_num_byte[1]);

  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
    // NORTH OFFSET
    big_cal_num = (((global_conf.accZero_max[SensorNum][ACC_NORTH] - 2048) + (global_conf.accZero_min[SensorNum][ACC_NORTH] + 2048))*100000)/4096;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP51, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP52, cal_num_byte[1]);

  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
    // EAST OFFSET
    big_cal_num = (((global_conf.accZero_max[SensorNum][ACC_EAST] - 2048) + (global_conf.accZero_min[SensorNum][ACC_EAST] + 2048))*100000)/4096;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP53, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP54, cal_num_byte[1]);

  if(!ACCEL_CAL || !Accel_Cal_valid[SensorNum])
  {
    cal_num_byte[0] = 0;
    cal_num_byte[1] = 0;
  } else
  {
    // DOWN OFFSET
    big_cal_num = (((global_conf.accZero_max[SensorNum][ACC_DOWN] - 2048) + (global_conf.accZero_min[SensorNum][ACC_DOWN] + 2048))*100000)/4096;
    cal_num = (int16_t)big_cal_num;
  }
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP55, cal_num_byte[0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_GP56, cal_num_byte[1]);
}

/**
* @fn: Mag_getADC();
*
* @brief: Fetches mag data from EM7180, applies orientation matrix (def,h); GUI indication only
* 
* @params: void
* @returns: void
*/
void EM7180::Mag_getADC()
{
  EM7180::i2c_getSixRawADC(EM7180_ADDRESS, EM7180_MX);
  MAG_ORIENTATION( (int16_t)((rawADC[1]<<8) | rawADC[0]),
                   (int16_t)((rawADC[3]<<8) | rawADC[2]),
                   (int16_t)((rawADC[5]<<8) | rawADC[4]));
}

/**
* @fn: EM7180_set_gyro_FS (uint16_t gyro_fs);
*
* @brief: Utility function for EM7180 to set gyro dynamic range
* 
* @params: Gyro full scale (+/-) range (dps)
* @returns: void
*/
void EM7180::EM7180_set_gyro_FS (uint16_t gyro_fs)
{
  uint8_t bytes[4], STAT;
  
  bytes[0] = gyro_fs & (0xFF);
  bytes[1] = (gyro_fs >> 8) & (0xFF);
  bytes[2] = 0x00;
  bytes[3] = 0x00;
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, bytes[0]);                                                                               // Gyro LSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, bytes[1]);                                                                               // Gyro MSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, bytes[2]);                                                                               // Unused
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, bytes[3]);                                                                               // Unused

  // Parameter 75; 0xCB is 75 decimal with the MSB set high to indicate a paramter write processs
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0xCB);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);

  // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==0xCB))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
}

/**
* @fn: EM7180_set_mag_acc_FS (uint16_t mag_fs, uint16_t acc_fs);
*
* @brief: Utility function for EM7180 to set mag and acc dynamic ranges
* 
* @params: Mag full scale (+/-) range (uT), acc full scale (+/-) range (g)
* @returns: void
*/
void EM7180::EM7180_set_mag_acc_FS (uint16_t mag_fs, uint16_t acc_fs)
{
  uint8_t bytes[4], STAT;
  
  bytes[0] = mag_fs & (0xFF);
  bytes[1] = (mag_fs >> 8) & (0xFF);
  bytes[2] = acc_fs & (0xFF);
  bytes[3] = (acc_fs >> 8) & (0xFF);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, bytes[0]);                                                                               // Mag LSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, bytes[1]);                                                                               // Mag MSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, bytes[2]);                                                                               // Acc LSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, bytes[3]);                                                                               // Acc MSB
  
  //Parameter 74; 0xCA is 74 decimal with the MSB set high to indicate a paramter write processs
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0xCA);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);

  // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==0xCA)) 
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
}

/**
* @fn: EM7180_set_integer_param (uint8_t param, uint32_t param_val);
*
* @brief: Utility function for EM7180 to set any integer parameter
* 
* @params: Param number, parameter value
* @returns: void
*/
void EM7180::EM7180_set_integer_param (uint8_t param, uint32_t param_val)
{
  uint8_t bytes[4], STAT;
  
  bytes[0] = param_val & (0xFF);
  bytes[1] = (param_val >> 8) & (0xFF);
  bytes[2] = (param_val >> 16) & (0xFF);
  bytes[3] = (param_val >> 24) & (0xFF);

  // Parameter is the decimal value with the MSB set high to indicate a paramter write processs
  param = param | 0x80;
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, bytes[0]);                                                                               // Param LSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, bytes[1]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, bytes[2]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, bytes[3]);                                                                               // Param MSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);

  // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==param))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
}

/**
* @fn: EM7180_set_float_param (uint8_t param, float param_val);
*
* @brief: Utility function for EM7180 to set any floating point parameter
* 
* @params: Param number, parameter value
* @returns: void
*/
void EM7180::EM7180_set_float_param (uint8_t param, float param_val)
{
  uint8_t bytes[4], STAT;
  
  EM7180::float_to_bytes (param_val, &bytes[0]);

  // Parameter is the decimal value with the MSB set high to indicate a paramter write processs
  param = param | 0x80;
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, bytes[0]);                                                                               // Param LSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, bytes[1]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, bytes[2]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, bytes[3]);                                                                               // Param MSB
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);

  // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==param))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
}

/**
* @fn: EM7180_set_WS_params();
*
* @brief: Loads Warm Start params from EEPROM into the Sentral
* 
* @params: Param number, parameter value
* @returns:
*/
void EM7180::EM7180_set_WS_params()
{
  uint8_t param = 1;
  uint8_t STAT;
  
  // Parameter is the decimal value with the MSB set high to indicate a paramter write processs
  param = param | 0x80;
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, WS_params.Sen_param[SensorNum][0][0]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, WS_params.Sen_param[SensorNum][0][1]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, WS_params.Sen_param[SensorNum][0][2]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, WS_params.Sen_param[SensorNum][0][3]);
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);

  // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==param))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  for(uint8_t i=1; i<35; i++)
  {
    param = (i+1) | 0x80;
    I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte0, WS_params.Sen_param[SensorNum][i][0]);
    I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte1, WS_params.Sen_param[SensorNum][i][1]);
    I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte2, WS_params.Sen_param[SensorNum][i][2]);
    I2C->writeByte(EM7180_ADDRESS, EM7180_LoadParamByte3, WS_params.Sen_param[SensorNum][i][3]);
    I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);
    
    // Check the parameter acknowledge register and loop until the result matches parameter request byte
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
    while(!(STAT==param))
    {
      STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
    }
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
}

/**
* @fn: EM7180_get_WS_params();
*
* @brief: Loads Warm Start params into the Sentral
* 
* @params: Param number, parameter value
* @returns: void
*/
void EM7180::EM7180_get_WS_params()
{
  uint8_t param = 1;
  uint8_t STAT;

  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);
  delay(20);

  // Request parameter transfer procedure
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x80);
  delay(20);

   // Check the parameter acknowledge register and loop until the result matches parameter request byte
  STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  while(!(STAT==param))
  {
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
  }
  
  // Parameter is the decimal value with the MSB set low (default) to indicate a paramter read processs
  WS_params.Sen_param[SensorNum][0][0] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte0);
  WS_params.Sen_param[SensorNum][0][1] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte1);
  WS_params.Sen_param[SensorNum][0][2] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte2);
  WS_params.Sen_param[SensorNum][0][3] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte3);

  for(uint8_t i=1; i<35; i++)
  {
    param = (i+1);
    I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, param);
    delay(20);
    
    // Check the parameter acknowledge register and loop until the result matches parameter request byte
    STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
    while(!(STAT==param))
    {
      STAT = I2C->readByte(EM7180_ADDRESS, EM7180_ParamAcknowledge);
    }
    WS_params.Sen_param[SensorNum][i][0] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte0);
    WS_params.Sen_param[SensorNum][i][1] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte1);
    WS_params.Sen_param[SensorNum][i][2] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte2);
    WS_params.Sen_param[SensorNum][i][3] = I2C->readByte(EM7180_ADDRESS, EM7180_SavedParamByte3);
  }
  // Parameter request = 0 to end parameter transfer process
  I2C->writeByte(EM7180_ADDRESS, EM7180_ParamRequest, 0x00);
  delay(20);
  
  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
  delay(20);
}

/**
* @fn: WS_PassThroughMode();
*
* @brief: Utility function to put Sentral into passthrough mode for I2C EEPROM read/write
* 
* @params: void
* @returns: void
*/
void EM7180::WS_PassThroughMode()
{
  uint8_t stat = 0;
  
  // Put SENtral in standby mode
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x01);
  delay(5);
  
  // Place SENtral in pass-through mode
  I2C->writeByte(EM7180_ADDRESS, EM7180_PassThruControl, 0x01);
  delay(5);
  stat = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
  delay(5);
  while(!(stat & 0x01))
  {
    stat = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
    delay(5);
  }
}

/**
* @fn: WS_Resume();
*
* @brief: Utility function to put Sentral back into normal operation after I2C EEPROM read/write
* 
* @params: void
* @returns: void
*/
void EM7180::WS_Resume()
{
  uint8_t stat = 0;
  
  // Cancel pass-through mode
  I2C->writeByte(EM7180_ADDRESS, EM7180_PassThruControl, 0x00);
  delay(5);
  stat = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
  while((stat & 0x01))
  {
    stat = I2C->readByte(EM7180_ADDRESS, EM7180_PassThruStatus);
    delay(5);
  }

  // Re-start algorithm
  I2C->writeByte(EM7180_ADDRESS, EM7180_AlgorithmControl, 0x00);
  delay(5);
  stat = I2C->readByte(EM7180_ADDRESS, EM7180_AlgorithmStatus);
  while((stat & 0x01))
  {
    stat = I2C->readByte(EM7180_ADDRESS, EM7180_AlgorithmStatus);
    delay(5);
  }
  
  // Read event status register to clear interrupt
  eventStatus[SensorNum] = I2C->readByte(EM7180_ADDRESS, EM7180_EventStatus);
}

/**
* @fn: readSenParams();
*
* @brief: Utility function to read Sentral Warm Start parameters from I2C EEPROM
* 
* @params: void
* @returns: void
*/
void EM7180::readSenParams()
{
  uint8_t data[140];
  uint8_t paramnum;
  
  // Write in 16 byte chunks because the ESP8266 Wire buffer is only 32 bytes
  // 140 bytes total; first 128:
  for(uint8_t i = 0; i < 8; i++)
  {
    I2C->M24512DFMreadBytes(M24512DFM_DATA_ADDRESS, 0x80, (0x00 + 16*i), 16, &data[(16*i)]);                                                     // Page 256
  }

  // Last 12 bytes
  I2C->M24512DFMreadBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x80, 12, &data[128]);                                                                   // Page 257
  for (paramnum = 0; paramnum < 35; paramnum++) // 35 parameters
  {
    for (uint8_t i= 0; i < 4; i++)
    {
      WS_params.Sen_param[SensorNum][paramnum][i] = data[(paramnum*4 + i)];
    }
  }
  I2C->M24512DFMreadBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x98, 1, &Sentral_WS_valid[SensorNum]);                                                  // Page 257

  // If byte 0x98 is 10101010b (0xaa) that means valid WS data is in the EEPROM
  if(Sentral_WS_valid[SensorNum] == 0xaa)
  {
    Sentral_WS_valid[SensorNum] = 1;
  }else
  {
    Sentral_WS_valid[SensorNum] = 0;
  }
}

/**
* @fn: writeSenParams();
*
* @brief: Utility function to write Sentral Warm Start parameters to I2C EEPROM
* 
* @params: void
* @returns: void
*/
void EM7180::writeSenParams()
{
  uint8_t data[140];
  uint8_t paramnum;
  
  for (paramnum = 0; paramnum < 35; paramnum++) // 35 parameters
  {
    for (uint8_t i= 0; i < 4; i++)
    {
      data[(paramnum*4 + i)] = WS_params.Sen_param[SensorNum][paramnum][i];
    }
  }
  
  // Write in 16 byte chunks because the ESP8266 Wire buffer is only 32 bytes
  // 140 bytes total; first 128:
  for(uint8_t i = 0; i < 8; i++)
  {
    I2C->M24512DFMwriteBytes(M24512DFM_DATA_ADDRESS, 0x80, (0x00 + 16*i), 16, &data[(16*i)]);                                                    // Page 256
  }

  // Last 12 bytes
  I2C->M24512DFMwriteBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x80, 12, &data[128]);                                                                  // Page 257

  // Valid Warm Start byte; if save of WS parameters is successful write 10101010b (0xaa) to byte 0x98 (Free from 0x99 to 0xFF)
  Sentral_WS_valid[SensorNum] = 0xaa;
  I2C->M24512DFMwriteBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x98, 1, &Sentral_WS_valid[SensorNum]);                                                 // Page 257
  Sentral_WS_valid[SensorNum] = 0x00;
}

/**
* @fn: readAccelCal();
*
* @brief: Utility function to read Sentral Accelerometer alibration parameters from I2C EEPROM
* 
* @params: void
* @returns: void
*/
void EM7180::readAccelCal()
{
  uint8_t data[12];
  uint8_t axis;

  I2C->M24512DFMreadBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x8c, 12, data);                                                                         // Page 257
  for (axis = 0; axis < 3; axis++)
  {
    global_conf.accZero_max[SensorNum][axis] = ((int16_t)(data[(2*axis + 1)]<<8) | data[2*axis]);
    global_conf.accZero_min[SensorNum][axis] = ((int16_t)(data[(2*axis + 7)]<<8) | data[(2*axis + 6)]);
  }
}

/**
* @fn: writeAccCal();
*
* @brief: Utility function to write Sentral Accelerometer calibration parameters to I2C EEPROM
* 
* @params: void
* @returns: void
*/
void EM7180::writeAccCal()
{
  uint8_t data[12];
  uint8_t axis;
  for (axis = 0; axis < 3; axis++)
  {
    data[2*axis] = (global_conf.accZero_max[SensorNum][axis] & 0xff);
    data[(2*axis + 1)] = (global_conf.accZero_max[SensorNum][axis] >> 8);
    data[(2*axis + 6)] = (global_conf.accZero_min[SensorNum][axis] & 0xff);
    data[(2*axis + 7)] = (global_conf.accZero_min[SensorNum][axis] >> 8);
  }
  I2C->M24512DFMwriteBytes(M24512DFM_DATA_ADDRESS, 0x80, 0x8c, 12, data);                                                                        // Page 257
}

/**
* @fn: Save_Sentral_WS_params();
*
* @brief: Utility function to save Swntral WS parameters in a single function call
* 
* @params: void
* @returns: void
*/
void EM7180::Save_Sentral_WS_params()
{
      EM7180::EM7180_get_WS_params();
      
      // Put the Sentral in pass-thru mode
      EM7180::WS_PassThroughMode();

      // Store WarmStart data to the M24512DFM I2C EEPROM
      EM7180::writeSenParams();

      // Take Sentral out of pass-thru mode and re-start algorithm
      EM7180::WS_Resume();
}
