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

			IMUheading = heading;

			if (UsePitch)
			{
				RawRoll = pitch;
			}
			else
			{
				RawRoll = roll;
			}
		}
	}
#endif

#if (UseSerialIMU)
	// serial-attached IMU
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

	if (IMUserial.available() > 0 && !PGN32750Found)
	{
		int temp = IMUserial.read();
		IMUheader = IMUtempHeader << 8 | temp;
		IMUtempHeader = temp;
		if (IMUheader == 32750) PGN32750Found = true;
	}

	if (IMUserial.available() > 7 && PGN32750Found)
	{
		IMUserial.read();
		IMUserial.read();

		IMUheading = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUroll = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUpitch = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;

		PGN32750Found = false;

		if (UsePitch)
		{
			RawRoll = IMUpitch;
		}
		else
		{
			RawRoll = IMUroll;
		}
	}

	if (IMUserial.available() > 20)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}
#endif

#if UseDog2
	//Dog2 inclinometer
	//ADS1115 address 0x48
	//ADS max volts is 6.144 at 32767
	//Dog2 model is G-NSDOG2-001
	//Dog2 range is 0.5 to 4.5 V, +-25 degrees
	//ADS reading of the Dog2 ranges from 2700 to 24000 (21300)
	// counts per degree for this sensor is 426 (21300/50)
	// zero = 2700 + (21300/2)

	if (UsePitch)
	{
		RawRoll = (ads.readADC_SingleEnded(AdsPitch) - 13350) / 426;
	}
	else
	{
		RawRoll = (ads.readADC_SingleEnded(AdsRoll) - 13350) / 426;
	}
#endif

	if (RawRoll == 9999)
	{
		FilteredRoll = 9999;
	}
	else
	{
		//Kalman filter
		Pc = P + varProcess;
		G = Pc / (Pc + varRoll);
		P = (1 - G) * Pc;
		Xp = XeRoll;
		Zp = Xp;
		XeRoll = G * (RawRoll - Zp) + Xp;
		FilteredRoll = XeRoll;

		if (InvertRoll) FilteredRoll *= -1.0;
	}
}
