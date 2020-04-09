void UpdateHeadingRoll()
{
#if (UseOnBoardIMU)
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
			Serial.println("H " + String(heading));

			IMUheading = heading * 16.0;
			rollK = roll * 16.0;
		}
	}
#endif

#if (UseSerialIMU)
	// serial-attached IMU
	if (IMUserial.available() > 0 && !IMUdataFound)
	{
		int temp = IMUserial.read();
		IMUheader = IMUtempHeader << 8 | temp;
		IMUtempHeader = temp;
		if (IMUheader == 32750) IMUdataFound = true;
	}

	if (IMUserial.available() > 3 && IMUdataFound)
	{
		IMUserial.read();
		IMUserial.read();

		IMUheading = IMUserial.read() << 8 | IMUserial.read();
		rollK = IMUserial.read() << 8 | IMUserial.read();
		IMUdataFound = false;
	}

	if (IMUserial.available() > 20)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}
#endif

#if UseDog2
	//inclinometer
	//ADS1115 address 0x48, X is AIN1, Y is AIN2
	//ADS max volts is 6.144 at 32767
	//Dog2 model is G-NSDOG2-001
	//Dog2 range is 0.5 to 4.5 V, +-25 degrees
	//ADS reading of the Dog2 ranges from 2700 to 24000 (21300)
	// counts per degree for this sensor is 426 (21300/50)
	//rollK = (((ads.readADC_SingleEnded(AdsRoll) - 2700) / 426) - 25) * 16;

	rollK = ads.readADC_SingleEnded(AdsRoll);
	rollK = (rollK - 13350) / 26.6;
#endif

	if (rollK == 9999)
	{
		CurrentRoll = 9999;
	}
	else
	{
		//Kalman filter
		Pc = P + varProcess;
		G = Pc / (Pc + varRoll);
		P = (1 - G) * Pc;
		Xp = XeRoll;
		Zp = Xp;
		XeRoll = G * (rollK - Zp) + Xp;
		CurrentRoll = (int)XeRoll;
	}
}
