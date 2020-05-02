void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = ads.readADC_SingleEnded(AdsWAS);	//read the steering position sensor

	steeringPosition = (steeringPosition - SteeringPositionZero);

	//convert position to steer angle. 6 counts per degree of steer pot position in my case
	//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
	// remove or add the minus for steerSensorCounts to do that.
	steerAngleActual = (float)(steeringPosition) / (SteerCPD);

	if (InvertWAS) steerAngleActual *= -1.0;

	if (steerAngleActual < 0) steerAngleActual = (steerAngleActual * AckermanFix) / 100.0;

	steerAngleError = steerAngleActual - steerAngleSetPoint;   //calculate the steering error

	if (SteeringEnabled())
	{
		pwmDrive = GetPWM();
		//pwmDrive = DoPID(steerAngleError, steerAngleSetPoint, LOOP_TIME, MinPWMvalue, 255, Kp * Ko, Ki, Kd, SteerDeadband);
	
		if (InvertMotorDrive) pwmDrive *= -1;
	}
	else
	{
		pwmDrive = 0;
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

	if (watchdogTimer > 10 || distanceFromLine == 32020 || SteerSwitch == HIGH 
		|| pulseCount >= PulseCountMax 
		|| (CurrentSpeed < MinSpeed) || CurrentSpeed > MaxSpeed)
	{
		//SteerSwitch = HIGH;
		pulseCount = 0;
		watchdogTimer = 12;
		digitalWrite(SteerSW_Relay, LOW);

		return false;
	}
	else
	{
		digitalWrite(SteerSW_Relay, HIGH);

		return true;
	}
}

int GetPWM()
{
	// PID
	pwmTmp = Kp * steerAngleError * Ko;
	pwmTmp = (constrain(pwmTmp, -255, 255));

	//add min throttle factor so no delay from motor resistance.
	if (pwmTmp < 0) pwmTmp -= MinPWMvalue;
	else if (pwmTmp > 0) pwmTmp += MinPWMvalue;

	if (pwmTmp > 255) pwmTmp = 255;
	if (pwmTmp < -255) pwmTmp = -255;

	return pwmTmp;
}


