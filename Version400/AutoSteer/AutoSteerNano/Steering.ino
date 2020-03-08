void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = ads.readADC_SingleEnded(AdsWAS);	//read the steering position sensor

	steeringPosition = (steeringPosition - SteeringPositionZero);

	//convert position to steer angle. 6 counts per degree of steer pot position in my case
	//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
	// remove or add the minus for steerSensorCounts to do that.
	steerAngleActual = (float)(steeringPosition) / -SteerCPD;


	//*************** Drive Steering Motor ***********
	pwmDrive = 0; //turn off steering motor
	if (SteeringEnabled())
	{
		steerAngleError = steerAngleActual - steerAngleSetPoint;   //calculate the steering error

		// PID
		pwmDrive = Kp * steerAngleError * Ko;
		pwmDrive = (constrain(pwmDrive, -255, 255));

		//add min throttle factor so no delay from motor resistance.
		if (pwmDrive < 0) pwmDrive -= MinPWMvalue;
		else if (pwmDrive > 0) pwmDrive += MinPWMvalue;

		if (pwmDrive > 255) pwmDrive = 255;
		if (pwmDrive < -255) pwmDrive = -255;
	}

	// pwm value out to motor
	if (pwmDrive >= 0)
	{
		digitalWrite(DIR_PIN, HIGH);
	}
	else
	{
		digitalWrite(DIR_PIN, LOW);
		pwmDrive = -1 * pwmDrive;
	}
	analogWrite(PWM_PIN, pwmDrive);
}

bool SteeringEnabled()
{
	// if connection lost to AgOpenGPS, the watchdog will count up and turn off steering
	// auto Steer is off if 32020,Speed is too slow, motor pos or footswitch open
	// check steering wheel encoder
	watchdogTimer++;

	if (pulseCount >= pulseCountMax)
	{
		SteerSwitch = HIGH;
		pulseCount = 0;
	}

	if (watchdogTimer > 10 || distanceFromLine == 32020 || CurrentSpeed < 1 || SteerSwitch == HIGH)
	{
		watchdogTimer = 12;
		return false;
	}
	else
	{
		return true;
	}
}

