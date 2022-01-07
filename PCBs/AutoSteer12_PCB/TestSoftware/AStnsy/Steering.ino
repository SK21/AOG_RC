void DoSteering()
{
	//************** Steering Angle ******************
	adc.setMux(AdsWAS);
	steeringPosition = adc.getConversion();
	steeringPosition = (steeringPosition >> 1);			//bit shift by 2  0 to 13610 is 0 to 5v
	adc.triggerConversion();		//ADS1115 Single Mode 

	//convert position to steer angle. 32 counts per degree of steer pot position in my case
	//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
	if (steerConfig.InvertWAS)
	{
		steeringPosition = (steeringPosition - SteeringZeroOffset - steerSettings.wasOffset);   // 1/2 of full scale
		steerAngleActual = (float)(steeringPosition) / -steerSettings.steerSensorCounts;
	}
	else
	{
		steeringPosition = (steeringPosition - SteeringZeroOffset + steerSettings.wasOffset);   // 1/2 of full scale
		steerAngleActual = (float)(steeringPosition) / steerSettings.steerSensorCounts;
	}

	if (steerAngleActual < 0) steerAngleActual = (steerAngleActual * steerSettings.AckermanFix);

	steerAngleError = steerAngleActual - steerAngleSetPoint;   //calculate the steering error

	if ((millis() - CommTime > 4000) || (bitRead(guidanceStatus, 0) == 0)
		|| SteerSwitch == HIGH || (Speed_KMH < MinSpeed) || Speed_KMH > MaxSpeed)
	{
		// steering disabled

		pwmDrive = 0;

		// release steer motor relay
		digitalWrite(SteerSW_Relay, LOW);
	}
	else
	{
		// steering enabled

		// limit PWM when steer angle error is low
		MaxPWMvalue = steerSettings.highPWM;
		if (abs(steerAngleError) < LOW_HIGH_DEGREES)
		{
			MaxPWMvalue = (abs(steerAngleError) * ((steerSettings.highPWM - steerSettings.lowPWM) / LOW_HIGH_DEGREES)) + steerSettings.lowPWM;
		}

		// PID
		pwmDrive = steerSettings.Kp * steerAngleError;

		//add min throttle factor so no delay from motor resistance.
		if (pwmDrive < 0) pwmDrive -= steerSettings.minPWM;
		else if (pwmDrive > 0) pwmDrive += steerSettings.minPWM;

		if (pwmDrive > MaxPWMvalue) pwmDrive = MaxPWMvalue;
		if (pwmDrive < -MaxPWMvalue) pwmDrive = -MaxPWMvalue;

		if (steerConfig.MotorDriveDirection) pwmDrive *= -1;

		// engage steer motor relay
		digitalWrite(SteerSW_Relay, HIGH);
	}

	// pwm value out to motor
	if (pwmDrive >= 0)
	{
		digitalWrite(DIR1_PIN, HIGH);
		pwmDir = 1;
	}
	else
	{
		digitalWrite(DIR1_PIN, LOW);
		pwmDir = -1;
	}

	analogWrite(PWM1_PIN, pwmDrive * pwmDir);
}

void UpdateHeadingRoll()
{
#if(IMUtype == 1)
	// BNO080x
	if (myIMU.dataAvailable())
	{
		IMU_Heading = (myIMU.getYaw()) * 180.0 / PI; // Convert yaw / heading to degrees
		IMU_Heading = -IMU_Heading; //BNO085 counter clockwise data to clockwise data
		if (IMU_Heading < 0 && IMU_Heading >= -180) //Scale BNO085 yaw from [-180°;180°] to [0;360°]
		{
			IMU_Heading = IMU_Heading + 360;
		}

		if (SwapPitchRoll) //Adafruit library: roll is rotation around X axis
		{
			IMU_Roll = (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
			IMU_Pitch = (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
		}
		else //Adafruit library: pitch is rotation around Y axis
		{
			IMU_Roll = (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
			IMU_Pitch = (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
		}

		if (InvertRoll)
		{
			IMU_Roll *= -1.0; //Invert roll sign if needed
		}
	}
#endif
}
