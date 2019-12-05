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

#if (UseNano33 && UseSerialIMU)
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

	if (IMUserial.available() && IMUdataFound)
	{
		CommData += char(IMUserial.read());
		if (CommData.length() > MaxCommData)
		{
			IMUdataFound = false;
			CommData = "";
		}
		if (CommData.indexOf('\n') > 0)
		{
			WordCount = StringSplit(CommData, ',', Word, MaxWords);

			// heading
			IMUheading = Word[0].toFloat() * 16;  // Heading is 16 X actual degrees

			// roll
			IMUroll = Word[1].toFloat() * 16;	// Roll is 16 X actual degrees

			IMUdataFound = false;
			CommData = "";
		}
	}
	if (IMUserial.available() > 60)
	{
		// empty buffer
		while (IMUserial.available() > 0) char t = IMUserial.read();
	}
}

int StringSplit(String SourceStr, char Delim, String SubStr[], int MaxSubStr)
{
	int Count = 0;
	int Start = 0;
	if (SourceStr.length() > 0)
	{
		 //check for final delimiter
		if (SourceStr.charAt(SourceStr.length() - 1) != Delim) SourceStr += Delim;

		for (int i = 0; i < SourceStr.length(); i++)
		{
			if (SourceStr.charAt(i) == Delim)
			{
				SubStr[Count] = SourceStr.substring(Start, i);
				Start = (i + 1);
				if (Count == MaxSubStr) break;
				Count++;
			}
		}
	}
	return Count;
}
#endif
