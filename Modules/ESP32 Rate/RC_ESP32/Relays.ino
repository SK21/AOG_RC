
// If both onboard relays and remote relays are enabled, onboard relays will do 0-7, remote will do 8-15.
// If only one or the other are enabled it will do 0-15.

uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 }; // 8 relay module and a PCA9535PW on RelayDriver PCB
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 }; // 16 relay module and a PCA9535PW on RelayDriver PCB

uint8_t NewLo = 0;
uint8_t NewHi = 0;

void CheckRelays()
{
	uint8_t Rlys;
	bool BitState;
	uint8_t IOpin;

	bool Connected = false;
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (SensorConnected[i])
		{
			Connected = true;
			break;
		}
	}

	if (WifiMasterOn)
	{
		// wifi relay control
		// controls by relay # not section #
		if (millis() - WifiSwitchesTimer > 30000)   // 30 second timer
		{
			// wifi switches have timed out
			WifiMasterOn = false;
		}
		else
		{
			// set relays
			for (int i = 0; i < 8; i++)
			{
				if (Button[i]) bitSet(NewLo, i);
				if (Button[i + 8]) bitSet(NewHi, i);
			}
		}
	}
	else if (Connected)
	{
		NewLo = RelayLo;
		NewHi = RelayHi;
	}
	else
	{
		// connection lost, enable power and inverted relays
		// for valves that require power to close
		NewLo = PowerRelayLo | InvertedLo;
		NewHi = PowerRelayHi | InvertedHi;
	}

	// onboard relays
	byte End = 15;
	if (MDL.RemoteRelayControl > 0) End = 7; // remote does last 8
	ControlSwitch(0, End, MDL.OnboardRelayControl);


	// remote relays
	byte Start = 0;
	if (MDL.OnboardRelayControl > 0) Start = 8; // onboard does first 8
	ControlSwitch(Start, 15, MDL.RemoteRelayControl);
}

void ControlSwitch(byte Start, byte End, byte Control)
{
	uint8_t Rlys;
	bool BitState;
	uint8_t IOpin;

	switch (Control)
	{
	case 1:
		// GPIOs
		for (int j = 0; j < 2; j++)
		{
			if (j < 1) Rlys = NewLo; else Rlys = NewHi;
			for (int i = 0; i < 8; i++)
			{
				int Pin = i + j * 8;
				if (MDL.RelayControlPins[Pin] < NC && Pin >= Start && Pin <= End) // check if relay is enabled
				{
					if (bitRead(Rlys, i)) digitalWrite(MDL.RelayControlPins[i + j * 8], MDL.InvertRelay); else digitalWrite(MDL.RelayControlPins[i + j * 8], !MDL.InvertRelay);
				}
			}
		}
		break;

	case 2:
		// PCA9555 8 relays
		if (PCA9555PW_found)
		{
			uint8_t RelayByte = (Start == 0) ? NewLo : NewHi;
			for (int i = 0; i < 8; i++)
			{
				IOpin = Relays8[i];
				if (bitRead(RelayByte, i))
				{
					PCA.write(IOpin, PCA95x5::Level::L);
				}
				else
				{
					PCA.write(IOpin, PCA95x5::Level::H);
				}
			}
		}
		break;

	case 3:
		// PCA9555 16 relays
		if (PCA9555PW_found)
		{
			for (int i = 0; i < 16; i++)
			{
				if (i >= Start && i <= End)
				{
					if (i < 8)
					{
						BitState = bitRead(NewLo, i);
					}
					else
					{
						BitState = bitRead(NewHi, i - 8);
					}
					IOpin = Relays16[i];
					if (BitState)
					{
						PCA.write(IOpin, PCA95x5::Level::L);
					}
					else
					{
						PCA.write(IOpin, PCA95x5::Level::H);
					}
				}
			}
		}
		break;

	case 4:
		// MCP23017 control pins, example { 8,9,10,11,12,13,14,15,7,6,5,4,3,2,1,0 }

		if (MCP23017_found)
		{
			uint8_t mcpOutA = 0;
			uint8_t mcpOutB = 0;
			uint8_t Relay;
			uint8_t RelayBanks[] = { NewLo, NewHi };

			for (int bit = 0; bit < 8; bit++)
			{
				for (int bank = 0; bank < 2; bank++)
				{
					Relay = bit + bank * 8;
					if (Relay >= Start && Relay <= End)
					{
						if ((RelayBanks[bank] & (1 << bit)) == (1 << bit))
						{
							if (MDL.RelayControlPins[Relay] < 8)
							{
								mcpOutA |= (1 << MDL.RelayControlPins[Relay]);
							}
							else
							{
								mcpOutB |= (1 << (MDL.RelayControlPins[Relay] - 8));
							}
						}
					}
				}
			}

			if (MDL.InvertRelay)
			{
				mcpOutA = (uint8_t)~mcpOutA;
				mcpOutB = (uint8_t)~mcpOutB;
			}

			// Now send the output bytes.
			Wire.beginTransmission(MCP23017address);
			Wire.write(0x12);         // Starting register address (GPIOA)
			Wire.write(mcpOutA);      // GPA value
			Wire.write(mcpOutB);      // GPB value
			Wire.endTransmission();
		}
		break;

	case 5:
		// PCA9685
		if (PCA9685_found)
		{
			bool UseSpareDRV = (MDL.SensorCount == 1 && PCB_Type == 0);	// use spare DRV for relay 8
			uint8_t RelayByte = (Start == 0) ? NewLo : NewHi;
			for (int i = 0; i < 8; i++)
			{
				uint8_t RelayIndex = i + Start;
				if (RelayIndex > End) continue;

				BitState = bitRead(RelayByte, i);
				bool Use2Wire = (!MDL.Is3Wire || FlowMasterValveIndex == RelayIndex);

				// One DRV8870 per relay, its two inputs on PCA9685 channels i*2 and i*2+1.
				// Invert Relays swaps the pair so the bridge drives the other way and the
				// motor runs in reverse - flipping the level instead would power the valve
				// while the section is off, which is not what inverting means on a bridge.
				uint8_t DrivePin = Use2Wire ? (i * 2) : (i * 2 + 1);
				uint8_t OtherPin = Use2Wire ? (i * 2 + 1) : (i * 2);
				if (MDL.InvertRelay)
				{
					uint8_t Swap = DrivePin;
					DrivePin = OtherPin;
					OtherPin = Swap;
				}

				if (Use2Wire)
				{
					// 2 pins used for each valve, powered on and off
					if (BitState)
					{
						// on
						PWMServoDriver.setPWM(DrivePin, 4096, 0);
						PWMServoDriver.setPWM(OtherPin, 0, 4096);
					}
					else
					{
						// off
						PWMServoDriver.setPWM(DrivePin, 0, 4096);
						PWMServoDriver.setPWM(OtherPin, 4096, 0);
					}
				}
				else
				{
					// 1 pin for each valve, powered on only, 8 sections, 1 drv for each section
					if (BitState)
					{
						// on
						PWMServoDriver.setPWM(DrivePin, 4096, 0);
					}
					else
					{
						// off
						PWMServoDriver.setPWM(DrivePin, 0, 4096);
					}

					// hold the unused input of the pair off so changing Invert Relays cannot
					// leave the channel it used to drive latched on
					PWMServoDriver.setPWM(OtherPin, 0, 4096);
				}

				if (UseSpareDRV && i == 7)
				{
					// spare DRV8870 on GPIO 25/26 stands in for relay 8, swapped the same way
					uint8_t Level = BitState ? 255 : 0;
					if (MDL.InvertRelay)
					{
						analogWrite(25, 255 - Level);
						analogWrite(26, Level);
					}
					else
					{
						analogWrite(25, Level);
						analogWrite(26, 255 - Level);
					}
				}
			}
		}
		break;

	case 6:
		// PCF8574
		if (PCF_found)
		{
			for (int i = 0; i < 8; i++)
			{
				if (bitRead(NewLo, i)) PCF.write(i, MDL.InvertRelay); else PCF.write(i, !MDL.InvertRelay);
			}
		}
		break;
	}
}








