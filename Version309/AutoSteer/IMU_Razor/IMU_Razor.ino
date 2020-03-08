#include <SparkFunMPU9250-DMP.h>
#include <MPU9250_RegisterMap.h>
#include <Wire.h>

#define SerialPort SerialUSB

MPU9250_DMP imu;

float Roll = 0;
int SendHz = 10;
unsigned long LastTime = 0;

void setup() 
{
	SerialPort.begin(38400);

	delay(5000);
	SerialPort.println();
	SerialPort.println("IMU_Razor  :  Version Date: 11/Feb/2020");
	SerialPort.println();

	Serial1.begin(38400);

  // Call imu.begin() to verify communication and initialize
  if (imu.begin() != INV_SUCCESS)
  {
    while (1)
    {
      SerialPort.println("Unable to communicate with MPU-9250");
      SerialPort.println("Check connections, and try again.");
      SerialPort.println();
      delay(5000);
    }
  }
  
  imu.dmpBegin(DMP_FEATURE_6X_LP_QUAT | // Enable 6-axis quat
               DMP_FEATURE_GYRO_CAL, // Use gyro calibration
              10); // Set DMP FIFO rate to 10 Hz
}

void loop() 
{
	// Check for new data in the FIFO
	if ( imu.fifoAvailable() )
	{
		// Use dmpUpdateFifo to update the ax, gx, mx, etc. values
		if ( imu.dmpUpdateFifo() == INV_SUCCESS)
		{
			// computeEulerAngles can be used -- after updating the
			// quaternion values -- to estimate roll, pitch, and yaw
			imu.computeEulerAngles();
		}
	}

	Roll = imu.roll;
	if (Roll > 180)
	{
		Roll = Roll - 360.0;
	}

	if ((millis() - LastTime) > (1000 / SendHz))
	{
		LastTime = millis();
		CommToAutoSteer();
	}

	SerialPort.println();
	SerialPort.println("Heading " + String(imu.yaw));
	SerialPort.println("Roll " + String(Roll));
}

void CommToAutoSteer()
{
	//header bytes for 32500
	Serial1.write(126);
	Serial1.write(244);

	// heading
	int temp = imu.yaw * 16;	// 16 * actual
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);

	// roll
	temp = Roll * 16;	// 16 * actual
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);
}


