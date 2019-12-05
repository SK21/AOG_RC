void DoSteering()
{
	//********** Roll **************
#if (UseNano33 && UseIMUroll && UseSerialIMU)
	rollK = IMUroll;
#endif

#if (UseNano33 && UseIMUroll && UseOnBoardIMU)
	if (OnBoardIMUenabled) rollK = IMUroll;
#endif

#if UseDog2
	//inclinometer
	//ADS1115 address 0x48, X is AIN1, Y is AIN2
	//ADS max volts is 6.144 at 32767
	//Dog2 model is G-NSDOG2-001
	//Dog2 range is 0.5 to 4.5 V, +-25 degrees
	//ADS reading of the Dog2 ranges from 2700 to 24000 (21300)
	// counts per degree for this sensor is 426 (21300/50)
	//rollK = (((ads.readADC_SingleEnded(1) - 2700) / 426) - 25) * 16;
	rollK = (ads.readADC_SingleEnded(1) - 13350) / 26.6;
#endif

	//Kalman filter
	Pc = P + varProcess;
	G = Pc / (Pc + varRoll);
	P = (1 - G) * Pc;
	Xp = XeRoll;
	Zp = Xp;
	XeRoll = G * (rollK - Zp) + Xp;


	//************** Steering Angle ******************
	steeringPosition = ads.readADC_SingleEnded(0);	//read the steering position sensor
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

	// testing
	//steerSwitch = 0;
	//Serial.println("WD " + String(watchdogTimer));
	//Serial.println("Distance " + String(distanceFromLine));
	//Serial.println("Speed " + String(CurrentSpeed));
	//Serial.println("pulse " + String(pulseCount));
	//steerSwitch = 0;

	if (watchdogTimer > 10 || distanceFromLine == 32020 || CurrentSpeed < 1 || steerSwitch == 1 || pulseCount >= pulseCountMax)
	{
		watchdogTimer = 12;
		pulseCount = 0;
		steerSwitch = 1;
		OldSteerSwitchValue = 1;
		SteerSwitchState = 1;
		return false;
	}
	else
	{
		return true;
	}
}

