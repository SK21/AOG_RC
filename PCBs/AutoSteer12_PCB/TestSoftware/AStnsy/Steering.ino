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

		// release steer motor 
		digitalWrite(SteerSW_Relay, HIGH);
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

		// engage steer motor
		digitalWrite(SteerSW_Relay, LOW);
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

