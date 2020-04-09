void UpdateHeadingRoll()
{
	// serial-attached IMU
	if (IMUserial.available() > 0 && !PGN32750)
	{
		int temp = IMUserial.read();
		IMUheader = IMUtempHeader << 8 | temp;
		IMUtempHeader = temp;
		PGN32750 = (IMUheader == 32750);
	}

	if (IMUserial.available() > 7 && PGN32750)
	{
		IMUserial.read();
		IMUserial.read();

		IMUheading = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUroll = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;
		IMUpitch = (IMUserial.read() << 8 | IMUserial.read()) / 16.0;

		PGN32750 = false;

		if (aogSettings.UseMMA_X_Axis)
		{
			RawRoll = IMUroll;
		}
		else
		{
			RawRoll = IMUpitch;
		}

		//Serial.println();
		//Serial.println("Heading " + String(IMUheading));
		//Serial.println("Roll " + String(IMUroll));
		//Serial.println("Pitch " + String(IMUpitch));
	}

	if (IMUserial.available() > 20)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}

	if (aogSettings.InclinometerInstalled == 1)
	{
		//Dog2 inclinometer
		//ADS1115 address 0x48
		//ADS max volts is 6.144 at 32767
		//Dog2 model is G-NSDOG2-001
		//Dog2 range is 0.5 to 4.5 V, +-25 degrees
		//ADS reading of the Dog2 ranges from 2700 to 24000 (21300)
		// counts per degree for this sensor is 426 (21300/50)
		// zero = 2700 + (21300/2)

		if (aogSettings.UseMMA_X_Axis)
		{
			RawRoll = (ads.readADC_SingleEnded(AdsRoll) - 13350) / 426;
		}
		else
		{
			RawRoll = (ads.readADC_SingleEnded(AdsPitch) - 13350) / 426;
		}
	}

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

		if (aogSettings.InvertRoll) FilteredRoll *= -1.0;
	}
}
