
int16_t ReadWAS(uint8_t NextPinNumber = 0)
{
	int16_t Result = 0;
	if (PCB.UseAds)
	{
		// based on https://github.com/RalphBacon/ADS1115-ADC/blob/master/ADS1115_ADC_16_bit_SingleEnded.ino

		// use ADS1115
		// read current value
		Wire.beginTransmission(AdsI2Caddress);
		Wire.write(0b00000000); //Point to Conversion register 
		Wire.endTransmission();
		Wire.requestFrom(AdsI2Caddress, 2);
		Result = (Wire.read() << 8 | Wire.read());
		Result = Result >> 1;


		// do next conversion
		Wire.beginTransmission(AdsI2Caddress);
		Wire.write(0b00000001); // Point to Config Register

		// Write the MSB + LSB of Config Register
		// MSB: Bits 15:8
		// Bit  15    0=No effect, 1=Begin Single Conversion (in power down mode)
		// Bits 14:12   How to configure A0 to A3 (comparator or single ended)
		// Bits 11:9  Programmable Gain 000=6.144v 001=4.096v 010=2.048v .... 111=0.256v
		// Bits 8     0=Continuous conversion mode, 1=Power down single shot
		switch (NextPinNumber)
		{
			// single ended
		case 1:
			Wire.write(0b01010000);	// AIN1
			break;
		case 2:
			Wire.write(0b01100000);	// AIN2
			break;
		case 3:
			Wire.write(0b01110000);	// AIN3
			break;
		default:
			Wire.write(0b01000000);	// AIN0
			break;
		}

		// LSB: Bits 7:0
		// Bits 7:5 Data Rate (Samples per second) 000=8, 001=16, 010=32, 011=64,
		//      100=128, 101=250, 110=475, 111=860
		// Bit  4   Comparator Mode 0=Traditional, 1=Window
		// Bit  3   Comparator Polarity 0=low, 1=high
		// Bit  2   Latching 0=No, 1=Yes
		// Bits 1:0 Comparator # before Alert pin goes high
		//      00=1, 01=2, 10=4, 11=Disable this feature
		Wire.write(0b11100011);	//860 samples/sec
		Wire.endTransmission();
	}
	else
	{
		// use Teensy analog pin
		Result = adc->adc0->analogRead(PINS.WAS);
	}
	return Result;
}

void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = ReadWAS(PCB.AdsWASpin);
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
	digitalWrite(PINS.SteerDir, (pwmDrive >= 0));
	analogWrite(PINS.SteerPWM, abs(pwmDrive));
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

void PositionMotor()
{
	float Reading = (float)adc->adc0->analogRead(PINS.PressureSensor);

	if (digitalRead(PINS.SteerSW_Relay))
	{
		// steering engaged, extend linear actuator
		if (Reading < 3)
		{
			digitalWrite(PINS.FlowDir, HIGH);
			analogWrite(PINS.FlowPWM, 255);
		}
	}
	else
	{
		// steering disengaged, retract linear actuator
		if (Reading > 0.1)
		{
			digitalWrite(PINS.FlowDir, LOW);
			analogWrite(PINS.FlowPWM, 255);
		}
	}
}
