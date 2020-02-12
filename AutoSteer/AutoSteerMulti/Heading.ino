#if (UseNano33 && UseOnBoardIMU)
void UpdateHeading()
{
	if (OnBoardIMUenabled)
	{
		// on-board IMU
		float xAcc, yAcc, zAcc;
		float xGyro, yGyro, zGyro;
		float roll, pitch, heading;
		if (IMU.accelerationAvailable() && IMU.gyroscopeAvailable())
		{
			IMU.readAcceleration(xAcc, yAcc, zAcc);
			IMU.readGyroscope(xGyro, yGyro, zGyro);
			MKfilter.updateIMU(xGyro, yGyro, zGyro, xAcc, yAcc, zAcc);
			roll = MKfilter.getRoll();
			pitch = MKfilter.getPitch();
			heading = MKfilter.getYaw();

			IMUheading = heading * 16;
			IMUroll = roll * 16;
		}
	}
} 
#endif

#if (UseSerialIMU)
void UpdateHeading()
{
	// serial-attached IMU
	if (IMUserial.available() > 0 && !IMUdataFound)
	{
		int temp = IMUserial.read();
		IMUheader = IMUtempHeader << 8 | temp;
		IMUtempHeader = temp;
		if (IMUheader == 32500) IMUdataFound = true;
	}

	if (IMUserial.available() > 3 && IMUdataFound)
	{
		IMUheading = Serial.read() << 8 | Serial.read();
		IMUroll = Serial.read() << 8 | Serial.read();
		IMUdataFound = false;
	}

	if (IMUserial.available() > 60)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}
}
#endif
