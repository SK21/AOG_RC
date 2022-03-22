
void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = adc->adc0->analogRead(WAS_Pin);
	steeringPosition = steeringPosition >> 1;

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
	steerAngleError = steerAngleActual - steerAngleSetPoint;   

	if ((millis() - CommTime > 4000) || (bitRead(guidanceStatus, 0) == 0) || SteerSwitch == HIGH || (Speed_KMH < MinSpeed) || Speed_KMH > MaxSpeed)
	{
		// steering disabled

		pwmDrive = 0;

		// release steer motor 
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

		if (steerConfig.IsDanfoss)
		{
			// Danfoss: PWM 25% On = Left Position max  (below Valve=Center)
			// Danfoss: PWM 50% On = Center Position
			// Danfoss: PWM 75% On = Right Position max (above Valve=Center)
			pwmDrive = (constrain(pwmDrive, -250, 250));

			// Calculations below make sure pwmDrive values are between 65 and 190
			// This means they are always positive, so in motorDrive, no need to check for
			// steerConfig.isDanfoss anymore
			pwmDrive = pwmDrive >> 2; // Devide by 4
			pwmDrive += 128;          // add Center Pos.
		}

		// engage steer motor
		digitalWrite(SteerSW_Relay, HIGH);
	}

	// pwm value out to motor
	digitalWrite(DIR1_PIN, (pwmDrive >= 0));
	analogWrite(PWM1_PIN, abs(pwmDrive));
}


float tmpIMU;
float HeadingLast;

void ReadIMU()
{
	if (IMUstarted)
	{
#if(IMUtype == 1)
		// BNO080x
		if (myIMU.dataAvailable())
		{
			IMU_Heading = (myIMU.getYaw()) * 180.0 / PI; // Convert yaw / heading to degrees
			IMU_Heading = -IMU_Heading; //BNO085 counter clockwise data to clockwise data
			if (IMU_Heading < 0 && IMU_Heading >= -180) //Scale BNO085 yaw from [-180�;180�] to [0;360�]
			{
				IMU_Heading = IMU_Heading + 360;
			}
			IMU_Heading *= 10.0;

			if (SwapPitchRoll) //Adafruit library: roll is rotation around X axis
			{
				tmpIMU = 10 * (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
				if (InvertRoll) tmpIMU *= -1.0;
				IMU_Roll = IMU_Roll * 0.8 + tmpIMU * 0.2;

				tmpIMU = 10 * (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
				IMU_Pitch = IMU_Pitch * 0.8 + 0.2 * tmpIMU;
			}
			else //Adafruit library: pitch is rotation around Y axis
			{
				tmpIMU = 10 * (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
				if (InvertRoll) tmpIMU *= -1.0;
				IMU_Roll = IMU_Roll * 0.8 + tmpIMU * 0.2;

				tmpIMU = 10 * (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
				IMU_Pitch = IMU_Pitch * 0.8 + 0.2 * tmpIMU;
			}

#if(EnableGyro)
			tmpIMU = -10 * (myIMU.getGyroZ()) * 180.0 / PI;
			IMU_YawRate = IMU_YawRate * 0.8 + tmpIMU * 0.2;
#endif
		}
#endif

#if(IMUtype == 2)
		// CMPS14
		int16_t temp = 0;

		//the heading x10
		Wire.beginTransmission(CMPS14_ADDRESS);
		Wire.write(0x02);
		Wire.endTransmission();

		Wire.requestFrom(CMPS14_ADDRESS, 3);
		while (Wire.available() < 3);

		IMU_Heading = Wire.read() << 8 | Wire.read();

		//3rd byte pitch
		IMU_Pitch = Wire.read();

		//roll
		Wire.beginTransmission(CMPS14_ADDRESS);
		Wire.write(0x1C);
		Wire.endTransmission();

		Wire.requestFrom(CMPS14_ADDRESS, 2);
		while (Wire.available() < 2);

		tmpIMU = int16_t(Wire.read() << 8 | Wire.read());

		//Complementary filter
		IMU_Roll = 0.9 * IMU_Roll + 0.1 * tmpIMU;

		//Get the Z gyro
		Wire.beginTransmission(CMPS14_ADDRESS);
		Wire.write(0x16);
		Wire.endTransmission();

		Wire.requestFrom(CMPS14_ADDRESS, 2);
		while (Wire.available() < 2);

		tmpIMU = int16_t(Wire.read() << 8 | Wire.read());

		//Complementary filter
		IMU_YawRate = 0.93 * IMU_YawRate + 0.07 * tmpIMU;

#endif
	}
}
