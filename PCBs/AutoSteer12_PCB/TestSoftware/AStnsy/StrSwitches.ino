unsigned long PulseStart;
int SwitchPulseCount;
bool PulseRead;
bool PulseLast;
bool Latched;
int8_t SWpin;

void ReadSwitches()
{
	// steer switch		- low, steering on 
	//					- high, steering off

	// guidanceStatus	- low, steering off
	//					- high, steering on

	if (steerConfig.SteerSwitch == 1)
	{
		// on off switch
		SWpin = digitalRead(STEERSW_PIN);

		if (SWpin == LOW && !Latched) SteerSwitch = SWpin;
		if (SWpin)
		{
			Latched = false;
			SteerSwitch = SWpin;
		}
		switchByte = SteerSwitch << 1;
	}
	else if (steerConfig.SteerButton == 1)
	{
		// push button
		// pin is pulled high and goes low when button is pushed

		SWreading = digitalRead(STEERSW_PIN);
		if (SWreading == LOW && SWprevious == HIGH && millis() - SWtime > SWdebounce)
		{
			if (SteerSwitch == HIGH)
			{
				SteerSwitch = LOW;
			}
			else
			{
				SteerSwitch = HIGH;
			}
			SWtime = millis();
		}
		switchByte = SteerSwitch << 1;
		SWprevious = SWreading;
	}
	else
	{
		// no switch, match status
		SteerSwitch = !guidanceStatus;
		switchByte = SteerSwitch << 1;
	}

	// encoder
	if (steerConfig.ShaftEncoder)
	{
		PulseRead = digitalRead(Encoder_Pin);
		if ((PulseRead != PulseLast) && (millis() - PulseStart > SWdebounce))
		{
			PulseStart = millis();
			PulseLast = PulseRead;
			SwitchPulseCount++;

			if (SwitchPulseCount >= steerConfig.PulseCountMax)
			{
				SteerSwitch = HIGH;
				switchByte = SteerSwitch << 1;
				SWprevious = HIGH;
				SwitchPulseCount = 0;
				Latched = true;
			}
		}
	}

	// current sensor
	if (steerConfig.CurrentSensor)
	{
		int16_t analogValue = analogRead(CurrentSensorPin);

		// When the current sensor is reading current high enough, shut off
		if (abs(((analogValue - 512)) / 10.24) >= steerConfig.PulseCountMax) //amp current limit switch off
		{
			SteerSwitch = HIGH;
			switchByte = SteerSwitch << 1;
			SWprevious = LOW;
			Latched = true;
		}
	}

	// pressure sensor, 3.3V pin
	if (steerConfig.PressureSensor)
	{
		int16_t analogValue = analogRead(PressureSensorPin);

		// Calculations below do some assumptions, but we should be close?
		// 0-250bar sensor 4-20ma with 150ohm 1V - 5V -> 62,5 bar/V
		// 5v  / 1024 values -> 0,0048828125 V/bit
		// 62,5 * 0,0048828125 = 0,30517578125 bar/count
		// 1v = 0 bar = 204,8 counts
		int16_t steeringWheelPressureReading = (analogValue - 204) * 0.30517578125;

		// When the pressure sensor is reading pressure high enough, shut off
		if (steeringWheelPressureReading >= steerConfig.PulseCountMax)
		{
			SteerSwitch = HIGH;
			switchByte = SteerSwitch << 1;
			SWprevious = LOW;
			Latched = true;
		}
	}

	workSwitch = digitalRead(WORKSW_PIN);  // read work switch, Low on, High off
	switchByte |= workSwitch;
}




