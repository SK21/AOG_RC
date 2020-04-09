#include <SparkFunMPU9250-DMP.h>
#include <MPU9250_RegisterMap.h>
#include <Wire.h>

#define SerialPort SerialUSB

MPU9250_DMP imu;

int SendHz = 10;

float Roll = 0.0;
float Pitch = 0.0;

unsigned long LastTime = 0;
int temp = 0;

void setup() 
{
	SerialPort.begin(38400);

	delay(5000);
	SerialPort.println();
	SerialPort.println("IMU_Razor  :   08/Apr/2020");
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
	if (imu.fifoAvailable())
	{
		// Use dmpUpdateFifo to update the ax, gx, mx, etc. values
		if (imu.dmpUpdateFifo() == INV_SUCCESS)
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

	Pitch = imu.pitch;
	if (Pitch > 180)
	{
		Pitch = Pitch - 360.0;
	}

	if ((millis() - LastTime) > (1000 / SendHz))
	{
		LastTime = millis();

		// PGN32750 
		// 0 HeaderHi       127
		// 1 HeaderLo       238
		// 2 -
		// 3 -
		// 4 HeadingHi      actual X 16
		// 5 HeadingLo
		// 6 RollHi         actual X 16
		// 7 RollLo
		// 8 PitchHi        actual X 16
		// 9 PitchLo

		SendSerialUSB();
		SendSerial1();
	}
}

void SendSerialUSB()
{
	SerialPort.print("127,238,0,0,");

	// heading
	temp = imu.yaw * 16;
	SerialPort.print((byte)(temp >> 8));
	SerialPort.print(",");
	SerialPort.print((byte)temp);
	SerialPort.print(",");

	// roll
	temp = Roll * 16;
	SerialPort.print((byte)(temp >> 8));
	SerialPort.print(",");
	SerialPort.print((byte)temp);
	SerialPort.print(",");

	// pitch
	temp = Pitch * 16;
	SerialPort.print((byte)(temp >> 8));
	SerialPort.print(",");
	SerialPort.print((byte)temp);

	SerialPort.println();
	SerialPort.flush();

	//SerialPort.print("  Heading " + String(imu.yaw));
	//SerialPort.print("  Roll " + String(imu.roll) + "  " + String(Roll));
	//SerialPort.println("  Pitch " + String(imu.pitch) + "  " + String(Pitch));
}

void SendSerial1()
{
	Serial1.write(127);
	Serial1.write(238);

	temp = 0;
	Serial1.write(temp);
	Serial1.write(temp);

	// heading
	temp = imu.yaw * 16;
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);

	// roll
	temp = Roll * 16;
	Serial1.write((byte)(temp >> 8));

	Serial1.write((byte)temp);

	// pitch
	temp = Pitch * 16;
	Serial1.write((byte)(temp >> 8));
	Serial1.write((byte)temp);
}


