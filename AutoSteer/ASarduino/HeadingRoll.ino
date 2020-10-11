float KalResult = 0;
float KalPc = 0.0;
float KalG = 0.0;
float KalP = 1.0;
float KalVariance = 0.1;	// larger is more filtering
float KalProcess = 0.0001;	// smaller is more filtering

void UpdateHeadingRoll()
{
#if (IMUSource == 1 | IMUSource == 2)
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
		PGN32750Found = (IMUheader == 32750);
	}

	if (IMUserial.available() > 7 && PGN32750Found)
	{
		IMUserial.read();
		IMUserial.read();

		IMUheading = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUroll = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUpitch = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;

		PGN32750Found = false;

#if (RollSource == 1)
		if (SwapPitchRoll)
		{
			RawRoll = IMUpitch;
		}
		else
		{
			RawRoll = IMUroll;
		}
#endif
	}

	if (IMUserial.available() > 20)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}

#endif

#if (IMUSource == 3)
	if (OnBoardIMUenabled)
	{
		// on-board IMU
		float xAcc, yAcc, zAcc;
		float xGyro, yGyro, zGyro;
		if (IMU.accelerationAvailable() && IMU.gyroscopeAvailable())
		{
			IMU.readAcceleration(xAcc, yAcc, zAcc);
			IMU.readGyroscope(xGyro, yGyro, zGyro);
			MKfilter.updateIMU(xGyro, yGyro, zGyro, xAcc, yAcc, zAcc);

			IMUroll = MKfilter.getRoll();
			IMUpitch = MKfilter.getPitch();
			IMUheading = MKfilter.getYaw();

#if (RollSource == 1)
			if (SwapPitchRoll)
			{
				RawRoll = IMUpitch;
			}
			else
			{
				RawRoll = IMUroll;
			}
#endif
		}
	}

#endif

#if (RollSource == 2)
	//Dog2 inclinometer
	//ADS1115 address 0x48
	//ADS max volts is 6.144 at 32767
	//Dog2 model is G-NSDOG2-001
	//Dog2 range is 0.5 to 4.5 V, +-25 degrees
	//ADS reading of the Dog2 ranges from 2700 to 24000 (21300)
	// counts per degree for this sensor is 426 (21300/50)
	// zero = 2700 + (21300/2)

	if (SwapPitchRoll)
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
		// Kalmen filter
		KalPc = KalP + KalProcess;
		KalG = KalPc / (KalPc + KalVariance);
		KalP = (1 - KalG) * KalPc;
		KalResult = KalG * (RawRoll - KalResult) + KalResult;
		FilteredRoll = KalResult;

		if (InvertRoll) FilteredRoll *= -1.0;
	}
}
