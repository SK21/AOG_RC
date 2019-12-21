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

	// set state of AOG master switch (btnSectionOffAutoOn)
	bitClear(OutCommand, 0);
	bitClear(OutCommand, 1);
	if (MasterChanged)
	{
		if (MasterOn) bitSet(OutCommand, 0);	// request AOG master switch on, section buttons to auto 
		if (!MasterOn) bitSet(OutCommand, 1);	// request AOG master switch off, section buttons to off
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
					// switch on, AOG in control
					if (bitRead(RelayFromAOG, i) == HIGH) bitSet(RelayControl, i); else bitClear(RelayControl, i);
					// aog section button is auto
				}
				else
				{
					// switch off
					bitClear(RelayControl, i);	// turn off section
					bitSet(SecSwOff[0], i);		// set aog section button to off
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
					bitSet(RelayControl, i);	// turn section on
					// aog section button will also be set to on
				}
				else
				{
					// switch off
					bitClear(RelayControl, i);	// turn off section
					bitSet(SecSwOff[0], i);		// set aog section button to off
				}
			}
			// bytes to AOG, update
			RelayToAOG = RelayControl;

			if (AutoChanged)
			{
				// continue application for delay time of autochanged
				RelayControl = RelayControlLast;
			}
		}
	}
	else
	{
		// master off
		// turn relays off
		RelayControl = 0;	// turn all sections off
		RelayToAOG = 0;
		SecSwOff[0] = 255;	// turn all aog section buttons off
	}

	// record relay byte before auto state change
	if (!AutoChanged) RelayControlLast = RelayControl;
}

