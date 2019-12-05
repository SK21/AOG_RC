void ReadSectionSwitches()
{
	SWreadTime = millis();

	SW1on = !digitalRead(SW1pin);
	SW2on = !digitalRead(SW2pin);
	SW3on = !digitalRead(SW3pin);
	SW4on = !digitalRead(SW4pin);

	// master state
	if (analogRead(MasterOffPin) < 500)
	{
		MasterOn = false;
	}
	else if (analogRead(MasterOnPin) < 500)
	{
		MasterOn = true;
	}

	if ((MasterOn != MasterLast) && !MasterChanged)
	{
		// create AOG master notification
		MasterTime = SWreadTime;
		MasterChanged = true;
	}

	if (SWreadTime > MasterTime + SWdelay)
	{
		// delay over, cancel AOG master notification
		MasterChanged = false;
		MasterLast = MasterOn;
	}

	// auto state
	AutoOn = !digitalRead(AutoPin);

	if ((AutoOn != AutoLast) && !AutoChanged)
	{
		// create AOG auto notification
		AutoTime = SWreadTime;
		AutoChanged = true;
	}

	if (SWreadTime > AutoTime + SWdelay)
	{
		// cancel AOG auto notification
		AutoChanged = false;
		AutoLast = AutoOn;
	}

	// relays
	SecSwOff[0] = 0;

	if (MasterOn)
	{
		if (AutoOn)
		{
			// master on, auto on
			for (int i = 0; i < 7; i++)	// 7 sections
			{
				switch (SecID[i])
				{
				case 1:
					PinState = SW1on;
					break;
				case 2:
					PinState = SW2on;
					break;
				case 3:
					PinState = SW3on;
					break;
				case 4:
					PinState = SW4on;
					break;
				default:
					PinState = false;
					break;
				}
				if (PinState)
				{
					// AOG in control
					if (bitRead(relayLo, i) == HIGH) bitSet(RelayControl, i); else bitClear(RelayControl, i);
				}
				else
				{
					// switch off
					bitClear(RelayControl, i);
					bitSet(SecSwOff[0], i);
				}
			}
			// bytes to AOG, no change
			RelayToAOG = 0;

			if (AutoChanged)
			{
				// change section buttons to auto state by resending master on
				bitSet(OutCommand, 0);

				// continue application for delay time of autochanged
				RelayControl = RelayControlLast;
			}
		}
		else
		{
			// master on, auto off
			for (int i = 0; i < 7; i++)	// 7 sections
			{
				switch (SecID[i])
				{
				case 1:
					PinState = SW1on;
					break;
				case 2:
					PinState = SW2on;
					break;
				case 3:
					PinState = SW3on;
					break;
				case 4:
					PinState = SW4on;
					break;
				default:
					PinState = false;
					break;
				}
				if (PinState)
				{
					// manual on
					bitSet(RelayControl, i);
				}
				else
				{
					// switch off
					bitClear(RelayControl, i);
					bitSet(SecSwOff[0], i);
				}
			}
			// bytes to AOG, update
			RelayToAOG = RelayControl;

			if (AutoChanged)
			{
				// the relay byte will change the section button
				// to the appropriate state

				// continue application for delay time of autochanged
				RelayControl = RelayControlLast;
			}
		}
	}
	else
	{
		// master off
		// turn relays off
		RelayControl = 0;
		SecSwOff[0] = 255;

		if (AutoChanged && !AutoOn)
		{
			// update section buttons state by repressing master off
			bitClear(OutCommand, 0);
			bitSet(OutCommand, 1);
		}
	}

	// record relay byte before auto state change
	if (!AutoChanged) RelayControlLast = RelayControl;

	// AOG notification of master state
	bitClear(OutCommand, 0);
	bitClear(OutCommand, 1);
	if (MasterChanged)
	{
		if (MasterOn)
		{
			bitSet(OutCommand, 0);
		}
		else
		{
			bitSet(OutCommand, 1);
		}
	}
}

