
void ReadSwitches()
{
	workSwitch = digitalRead(WORKSW_PIN);  // read work switch, Low on, High off
	switchByte = 0;

	if (UseSteerSwitch)
	{
		// on off switch
		switchByte = (digitalRead(STEERSW_PIN)) << 1;
	}
	else
	{
		switchByte = ReadPushButton() << 1; //put steerswitch status in bit 1 position
	}

	switchByte = workSwitch | switchByte;
}

byte ReadPushButton()
{
	// pin is pulled high and goes low when button is pushed
	// Low on, High off

	SWreading = digitalRead(STEERSW_PIN);
	if (SWreading == LOW && SWPrevious == HIGH && millis() - SWtime > SWdebounce)
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
	SWPrevious = SWreading;
	return SteerSwitch;
}


