uint32_t PulseStart;
int SwitchPulseCount;
bool PulseRead;
bool PulseLast;
bool LatchedOff;
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
		SWpin = digitalRead(PINS.STEERSW);

		if (SWpin)
		{
			// pin high, turn off
			LatchedOff = false;
			SteerSwitch = SWpin;
		}
		else
		{
			// pin low, turn on
			if (!LatchedOff) SteerSwitch = SWpin;
		}
		switchByte = SteerSwitch << 1;
	}
	else if (steerConfig.SteerButton == 1)
	{
		// push button
		// pin is pulled high and goes low when button is pushed

		SWreading = digitalRead(PINS.STEERSW);

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
		PulseRead = digitalRead(PINS.Encoder);
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
				LatchedOff = true;
			}
		}
	}

	// current sensor
	if (steerConfig.CurrentSensor)
	{
		float SensorSample = (float)adc->adc0->analogRead(PINS.CurrentSensor);
		SensorSample = (abs(512 - SensorSample)) * 0.5;
		SensorReading = SensorReading * 0.7 + SensorSample * 0.3;
		if (SensorReading >= steerConfig.PulseCountMax)
		{
			SteerSwitch = HIGH;
			switchByte = SteerSwitch << 1;
			SWprevious = LOW;
			LatchedOff = true;
		}
	}

	// pressure sensor
	if (steerConfig.PressureSensor)
	{
		float SensorSample = (float)adc->adc0->analogRead(PINS.PressureSensor);
		SensorSample *= 0.25;
		SensorReading = SensorReading * 0.6 + SensorSample * 0.4;
		if (SensorReading >= steerConfig.PulseCountMax)
		{
			SteerSwitch = HIGH;
			switchByte = SteerSwitch << 1;
			SWprevious = LOW;
			LatchedOff = true;
		}
	}

	workSwitch = digitalRead(PINS.WORKSW);  // read work switch, Low on, High off
	switchByte |= workSwitch;
}




