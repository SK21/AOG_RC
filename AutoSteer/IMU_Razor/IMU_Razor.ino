#include <SparkFunMPU9250-DMP_2.h>
#include <MPU9250_RegisterMap_2.h>
#include <Wire.h>

#define SerialPort SerialUSB

MPU9250_DMP imu;

void setup() 
{
	SerialPort.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("IMU_Razor  :  Version Date: 22/Dec/2019");
	Serial.println();

	Serial1.begin(38400);
	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, LOW);

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

	CommToAutoSteer();
}

