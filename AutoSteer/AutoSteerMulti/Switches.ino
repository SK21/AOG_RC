
void ReadSwitches()
{
	workSwitch = digitalRead(WORKSW_PIN);  // read work switch
	ReadSteerSwitch();
	switchByte = 0;
	switchByte = steerSwitch << 1; //put steerswitch status in bit 1 position
	switchByte = workSwitch | switchByte;
}

void ReadSteerSwitch()
{
	// check momemtary push button steer switch
	if (digitalRead(STEERSW_PIN) == LOW)
		// button pushed
	{
		switch (SteerSwitchState)
		{
		case 0:
			// off, turn on
			steerSwitch = 0;
			OldSteerSwitchValue = 0;
			SteerSwitchState = 1;
			break;
		case 1:
			// button still pushed, maintain value
			steerSwitch = OldSteerSwitchValue;
			break;
		case 2:
			// on, turn off
			steerSwitch = 1;
			OldSteerSwitchValue = 1;
			SteerSwitchState = 1;
			break;
		}
	}
	else
		// button not pushed
	{
		steerSwitch = OldSteerSwitchValue;
		if (SteerSwitchState == 1)
		{
			// button was released
			if (steerSwitch == 0)
			{
				SteerSwitchState = 2;
			}
			else
			{
				SteerSwitchState = 0;
			}
		}
	}
}




