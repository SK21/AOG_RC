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
#include "I2Cdev.h"

I2Cdev::I2Cdev(TwoWire* i2c_bus)
{
  I2C_Bus = i2c_bus;
}


/**
* @fn: readByte(uint8_t address, uint8_t subAddress)
*
* @brief: Read one byte from an I2C device
* 
* @params: I2C slave device address, Register subAddress
* @returns: unsigned short read
*/
uint8_t I2Cdev::readByte(uint8_t address, uint8_t subAddress)
{
  uint8_t data;                                                                                                                            // `data` will store the register data
  
  I2C_Bus->beginTransmission(address);                                                                                                     // Initialize the Tx buffer
  I2C_Bus->write(subAddress);                                                                                                              // Put slave register address in Tx buffer
  I2C_Bus->endTransmission(false);                                                                                                         // Send the Tx buffer, but send a restart to keep connection alive
  I2C_Bus->requestFrom(address, (size_t) 1);                                                                                               // Read one byte from slave register address 
  data = I2C_Bus->read();                                                                                                                  // Fill Rx buffer with result
  return data;                                                                                                                             // Return data read from slave register
}

/**
* @fn: M24512DFMreadByte(uint8_t device_address, uint8_t data_address1, uint8_t data_address2)
*
* @brief: Read one byte from an M24512DFM EEPROM
* 
* @params: I2C slave device address, data_address1 memory address MSB, data_address2 memory address LSB
* @returns: unsigned short read
*/
uint8_t I2Cdev::M24512DFMreadByte(uint8_t device_address, uint8_t data_address1, uint8_t data_address2)
{
  uint8_t data;                                                                                                                            // `data` will store the register data
  
  I2C_Bus->beginTransmission(device_address);                                                                                              // Initialize the Tx buffer
  I2C_Bus->write(data_address1);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->write(data_address2);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->endTransmission(false);                                                                                                         // Send the Tx buffer, but send a restart to keep connection alive
  I2C_Bus->requestFrom(device_address, (size_t)1);                                                                                         // Read one byte from slave register address 
  data = I2C_Bus->read();                                                                                                                  // Fill Rx buffer with result
  return data;                                                                                                                             // Return data read from slave register
}

/**
* @fn: readBytes(uint8_t address, uint8_t subAddress, uint8_t count, uint8_t * dest)
*
* @brief: Read multiple bytes from an I2C device
* 
* @params: I2C slave device address, Register subAddress, number of btes to be read, aray to store the read data
* @returns: void
*/
void I2Cdev::readBytes(uint8_t address, uint8_t subAddress, uint8_t count, uint8_t * dest)
{  
  uint8_t i = 0;

  I2C_Bus->beginTransmission(address);                                                                                                     // Initialize the Tx buffer
  I2C_Bus->write(subAddress);                                                                                                              // Put slave register address in Tx buffer
  I2C_Bus->endTransmission(false);                                                                                                         // Send the Tx buffer, but send a restart to keep connection alive
  I2C_Bus->requestFrom(address, (size_t) count);                                                                                           // Read bytes from slave register address 
  while (I2C_Bus->available())
  {
    dest[i++] = I2C_Bus->read();
  }                                                                                                                                        // Put read results in the Rx buffer
}

/**
* @fn: M24512DFMreadBytes(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t count, uint8_t *dest)
*
* @brief: Read multiple bytes from an M24512DFM EEPROM
* 
* @params: I2C slave device address, data_address1 memory address MSB, data_address2 memory address LSB
* @params: number of btes to be read, aray to store the read data
* @returns: void
*/
void I2Cdev::M24512DFMreadBytes(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t count, uint8_t *dest)
{  
  I2C_Bus->beginTransmission(device_address);                                                                                              // Initialize the Tx buffer
  I2C_Bus->write(data_address1);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->write(data_address2);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->endTransmission(false);                                                                                                         // Send the Tx buffer, but send a restart to keep connection alive
  uint8_t i = 0;
  I2C_Bus->requestFrom(device_address, (size_t)count);                                                                                     // Read bytes from slave register address 
  while (I2C_Bus->available())
  {
    dest[i++] = I2C_Bus->read();
  }                                                                                                                                        // Put read results in the Rx buffer
}

/**
* @fn: writeByte(uint8_t devAddr, uint8_t regAddr, uint8_t data)
*
* @brief: Write one byte to an I2C device
* 
* @params: I2C slave device address, Register subAddress, data to be written
* @returns: void
*/
void I2Cdev::writeByte(uint8_t devAddr, uint8_t regAddr, uint8_t data)
{
  writeBytes(devAddr, regAddr, 1, &data);
}

/**
* @fn: M24512DFMwriteByte(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t  data)
*
* @brief: Write one byte to an M24512DFM EEPROM
* 
* @params: I2C slave device address, data_address1 memory address MSB, data_address2 memory address LSB
* @params: data byte to be written
* @returns: void
*/
void I2Cdev::M24512DFMwriteByte(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t  data)
{
  I2C_Bus->beginTransmission(device_address);                                                                                              // Initialize the Tx buffer
  I2C_Bus->write(data_address1);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->write(data_address2);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->write(data);                                                                                                                    // Put data in Tx buffer
  I2C_Bus->endTransmission();                                                                                                              // Send the Tx buffer
  delay(5);
}

/**
* @fn: writeBytes(uint8_t devAddr, uint8_t regAddr, uint8_t length, uint8_t *data)
*
* @brief: Write multiple bytes to an to an I2C device
* 
* @params: I2C slave device address, Register subAddress, data to be written
* @params: number of bytes to be written, data array to be written
* @returns: void
*/
void I2Cdev::writeBytes(uint8_t devAddr, uint8_t regAddr, uint8_t length, uint8_t *data)
{
  uint8_t status = 0;
    
  I2C_Bus->beginTransmission(devAddr);
  I2C_Bus->write((uint8_t) regAddr);
  for (uint8_t i=0; i < length; i++)
  {
    I2C_Bus->write((uint8_t)data[i]);
  }
  status = I2C_Bus->endTransmission();
}

/**
* @fn: M24512DFMwriteBytes(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t count, uint8_t *dest)
*
* @brief: Write multiple bytes to an M24512DFM EEPROM
* 
* @params: I2C slave device address, data_address1 memory address MSB, data_address2 memory address LSB
* @params: number of btes to be written, data array to be written
* @returns: void
*/
void I2Cdev::M24512DFMwriteBytes(uint8_t device_address, uint8_t data_address1, uint8_t data_address2, uint8_t count, uint8_t *dest)
{
  I2C_Bus->beginTransmission(device_address);                                                                                              // Initialize the Tx buffer
  I2C_Bus->write(data_address1);                                                                                                           // Put slave register address in Tx buffer
  I2C_Bus->write(data_address2);                                                                                                           // Put slave register address in Tx buffer
  for(uint8_t i=0; i < count; i++)
  {
    I2C_Bus->write(dest[i]);                                                                                                               // Put data in Tx buffer
  }
  I2C_Bus->endTransmission();                   // Send the Tx buffer
  delay(5);
}

/**
* @fn:I2Cscan()
* @brief: Scan the I2C bus for active I2C slave devices
* 
* @params: void
* @returns: void
*/
void I2Cdev::I2Cscan() 
{
  // Scan for i2c devices
  byte error, address;
  int nDevices;

  Serial.println("Scanning...");

  nDevices = 0;
  for(address = 1; address < 127; address++ ) 
  {
    // The i2c_scanner uses the return value of the Wire.endTransmisstion to see if a device did acknowledge to the address.
    I2C_Bus->beginTransmission(address);
    error = I2C_Bus->endTransmission();

    if (error == 0)
    {
      Serial.print("I2C device found at address 0x");
      if (address<16) 
      Serial.print("0");
      Serial.print(address,HEX);
      Serial.println("  !");
      nDevices++;
    }
    else if (error==4) 
    {
      Serial.print("Unknow error at address 0x");
      if (address<16) 
        Serial.print("0");
      Serial.println(address,HEX);
    }    
  }
  if (nDevices == 0)
    Serial.println("No I2C devices found\n");
  else
    Serial.println("I2C scan complete\n");
}
