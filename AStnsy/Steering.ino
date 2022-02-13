
// based on https://github.com/RalphBacon/ADS1115-ADC/blob/master/ADS1115_ADC_16_bit_SingleEnded.ino

uint16_t ReadAds1115(byte NextPinNumber = 0)
{
	// read current value
	Wire.beginTransmission(AdsI2Caddress);
	Wire.write(0b00000000); //Point to Conversion register 
	Wire.endTransmission();
	Wire.requestFrom(AdsI2Caddress, 2);
	uint16_t convertedValue = (Wire.read() << 8 | Wire.read());


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
	Wire.write(0b00000011);
	Wire.endTransmission();

	return convertedValue;
}

void DoSteering()
{
	//************** Steering Angle ******************
	steeringPosition = ReadAds1115();
	steeringPosition = (steeringPosition >> 1);			//bit shift by 2  0 to 13610 is 0 to 5v

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

		// engage steer motor
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

float tmpIMU;
float HeadingLast;

void ReadIMU()
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
