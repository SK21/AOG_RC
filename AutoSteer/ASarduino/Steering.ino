
void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = ads.readADC_SingleEnded(AdsWAS);	//read the steering position sensor

	steeringPosition = (steeringPosition - SteeringZeroOffset - AOGzeroAdjustment);

	//convert position to steer angle. 6 counts per degree of steer pot position in my case
	//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
	// remove or add the minus for steerSensorCounts to do that.
	steerAngleActual = (float)(steeringPosition) / (SteerCPD);

	if (InvertWAS) steerAngleActual *= -1.0;

	if (steerAngleActual < 0) steerAngleActual = (steerAngleActual * AckermanFix) / 100.0;

	steerAngleError = steerAngleActual - steerAngleSetPoint;   //calculate the steering error

	if (SteeringEnabled())
	{
		// limit PWM when steer angle error is low
		MaxPWMvalue = HighMaxPWM;
		if (abs(steerAngleError) < LOW_HIGH_DEGREES)
		{
			MaxPWMvalue = (abs(steerAngleError) * ((HighMaxPWM - LowMaxPWM) / LOW_HIGH_DEGREES)) + LowMaxPWM;
		}

		// PID
		pwmDrive = Kp * steerAngleError;

		//add min throttle factor so no delay from motor resistance.
		if (pwmDrive < 0) pwmDrive -= MinPWMvalue;
		else if (pwmDrive > 0) pwmDrive += MinPWMvalue;

		if (pwmDrive > MaxPWMvalue) pwmDrive = MaxPWMvalue;
		if (pwmDrive < -MaxPWMvalue) pwmDrive = -MaxPWMvalue;

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
		pwmDir = 1;
	}
	else
	{
		digitalWrite(DIR_PIN, LOW);
		pwmDir = -1;
	}

	analogWrite(PWM_PIN, pwmDrive * pwmDir);
}

bool SteeringEnabled()
{
	// if connection lost to AgOpenGPS, the watchdog will count up and turn off steering
	// auto Steer is off if 32020, Speed is too slow, motor pos or footswitch open
	// check steering wheel encoder
	watchdogTimer++;

	if (watchdogTimer > 10 || distanceFromLine == 32020 || distanceFromLine == 32000
		|| SteerSwitch == HIGH || pulseCount >= PulseCountMax
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
