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
		float SensorSample = (float)analogRead(CurrentSensorPin);
		SensorSample = (abs(512 - SensorSample)) * 0.5;
		SensorReading = SensorReading * 0.7 + SensorSample * 0.3;
		if (SensorReading >= steerConfig.PulseCountMax)
		{
			SteerSwitch = HIGH;
			switchByte = SteerSwitch << 1;
			SWprevious = LOW;
			Latched = true;
		}
	}

	// pressure sensor
	if (steerConfig.PressureSensor)
	{
		float SensorSample = (float)analogRead(PressureSensorPin);
		SensorSample *= 0.25;
		SensorReading = SensorReading * 0.6 + SensorSample * 0.4;
		if (SensorReading >= steerConfig.PulseCountMax)
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




