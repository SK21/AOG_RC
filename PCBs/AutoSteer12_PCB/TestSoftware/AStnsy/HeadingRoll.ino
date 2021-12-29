
void UpdateHeadingRoll()
{
#if(IMUtype == 1)
	// BNO080x
	if (myIMU.dataAvailable())
	{
		IMUheading = (myIMU.getYaw()) * 180.0 / PI; // Convert yaw / heading to degrees
		IMUheading = -IMUheading; //BNO085 counter clockwise data to clockwise data
		if (IMUheading < 0 && IMUheading >= -180) //Scale BNO085 yaw from [-180°;180°] to [0;360°]
		{
			IMUheading = IMUheading + 360;
		}

		if (SwapPitchRoll) //Adafruit library: roll is rotation around X axis
		{
			IMUroll = (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
		}
		else //Adafruit library: pitch is rotation around Y axis
		{
			IMUroll = (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
		}

		if (InvertRoll)
		{
			IMUroll *= -1.0; //Invert roll sign if needed
		}
	}
#endif
}
