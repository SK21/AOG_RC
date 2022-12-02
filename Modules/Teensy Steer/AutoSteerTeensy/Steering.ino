
void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = AINs.AIN0;
	helloSteerPosition = steeringPosition - PCB.ZeroOffset;

	//  ***** make sure that negative steer angle makes a left turn and positive value is a right turn *****
	if (steerConfig.InvertWAS)
	{
		steeringPosition = (steeringPosition - PCB.ZeroOffset - steerSettings.wasOffset);   // 1/2 of full scale
		steerAngleActual = (float)(steeringPosition) / -steerSettings.steerSensorCounts;
	}
	else
	{
		steeringPosition = (steeringPosition - PCB.ZeroOffset + steerSettings.wasOffset);   // 1/2 of full scale
		steerAngleActual = (float)(steeringPosition) / steerSettings.steerSensorCounts;
	}

	if (steerAngleActual < 0) steerAngleActual = (steerAngleActual * steerSettings.AckermanFix);
	steerAngleError = steerAngleActual - steerAngleSetPoint;

	if ((millis() - CommTime > 4000) || (bitRead(guidanceStatus, 0) == 0) || SteerSwitch == HIGH || (Speed_KMH < PCB.MinSpeed) || Speed_KMH > PCB.MaxSpeed)
	{
		// steering disabled

		pwmDrive = 0;

		// release steer relay
		digitalWrite(PINS.SteerSW_Relay, LOW);
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

		// engage steer relay
		digitalWrite(PINS.SteerSW_Relay, HIGH);
	}

	// pwm value out to motor
	digitalWrite(PINS.Motor1Dir, (pwmDrive >= 0));
	analogWrite(PINS.Motor1PWM, abs(pwmDrive));
}

float tmpIMU;
float HeadingLast;

void ReadIMU()
{
	if (IMUstarted)
	{
		switch (PCB.IMU)
		{
		case 1:	// Sparkfun
		case 3:	// Adafruit
			if (myIMU.dataAvailable())
			{
				IMU_Heading = (myIMU.getYaw()) * 180.0 / PI; // Convert yaw / heading to degrees
				IMU_Heading = -IMU_Heading; //BNO085 counter clockwise data to clockwise data
				if (IMU_Heading < 0 && IMU_Heading >= -180) //Scale BNO085 yaw from [-180�;180�] to [0;360�]
				{
					IMU_Heading = IMU_Heading + 360;
				}
				IMU_Heading *= 10.0;

				if (PCB.SwapRollPitch)
				{
					//Adafruit library: pitch is rotation around Y axis
					tmpIMU = 10 * (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
					if (PCB.InvertRoll) tmpIMU *= -1.0;
					IMU_Roll = IMU_Roll * 0.8 + tmpIMU * 0.2;

					tmpIMU = 10 * (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
					IMU_Pitch = IMU_Pitch * 0.8 + 0.2 * tmpIMU;
				}
				else
				{
					//Adafruit library: roll is rotation around X axis
					tmpIMU = 10 * (myIMU.getRoll()) * 180.0 / PI; //Convert roll to degrees
					if (PCB.InvertRoll) tmpIMU *= -1.0;
					IMU_Roll = IMU_Roll * 0.8 + tmpIMU * 0.2;

					tmpIMU = 10 * (myIMU.getPitch()) * 180.0 / PI; // Convert pitch to degrees
					IMU_Pitch = IMU_Pitch * 0.8 + 0.2 * tmpIMU;
				}

				if (PCB.GyroOn)
				{
					tmpIMU = -10 * (myIMU.getGyroZ()) * 180.0 / PI;
					IMU_YawRate = IMU_YawRate * 0.8 + tmpIMU * 0.2;
				}
			}
			break;

		case 2:	// CMPS14
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
			break;
		}
	}
}

uint32_t AdjustStartTime;
byte AdjLast = 100;
byte AdjDirection;
bool IsMoving;
int ReadingLast;
int MinPosition = 2000;
int MaxPosition = 0;
int MinStop = 100;
int MaxStop = 850;
byte MinMovement = 50;
uint16_t MinAdjustTime = 500;

void PositionActuator()
{
	// position steering motor next to steering wheel, range 0-1023, position from Razor IMU
	AdjDirection = bitRead(guidanceStatus, 0);
	//AdjDirection = digitalRead(PINS.SteerSW_Relay);

	// check if changed direction
	if (AdjLast != AdjDirection)
	{
		// reset
		AdjLast = AdjDirection;
		AdjustStartTime = millis();
		ReadingLast = AINs.AIN1;
		IsMoving = true;
		MinPosition = 2000;
		MaxPosition = 0;
	}

	// check for actuator movement
	if (abs(AINs.AIN1 - ReadingLast) > MinMovement)
	{
		// moving, reset start
		AdjustStartTime = millis();
		ReadingLast = AINs.AIN1;
	}
	else
	{
		// not moving, check elapsed time
		if (millis() - AdjustStartTime > MinAdjustTime) IsMoving = false;
	}

	if (AdjDirection)
	{
		// steering engaged, retract linear actuator
		if (AINs.AIN1 > MinStop && IsMoving)
		{
			ControlMotor2(255, LOW);
		}
		else
		{
			ControlMotor2(0, LOW);
		}

		// check for slippage
		if (AINs.AIN1 < MinPosition) MinPosition = AINs.AIN1;
		if (AINs.AIN1 - MinPosition > 50) AdjLast = 100;	// reset
	}
	else
	{
		// steering disengaged, extend linear actuator
		if (AINs.AIN1 < MaxStop && IsMoving)
		{
			ControlMotor2(255, HIGH);
		}
		else
		{
			ControlMotor2(0, HIGH);
		}

		// check for slippage
		if (AINs.AIN1 > MaxPosition) MaxPosition = AINs.AIN1;
		if (MaxPosition - AINs.AIN1 > 50) AdjLast = 100;  // reset
	}
}
