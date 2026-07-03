
// If both onboard relays and remote relays are enabled, onboard relays will do 0-7, remote will do 8-15.
// If only one or the other are enabled it will do 0-15.

uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 }; // 8 relay module and a PCA9535PW
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 }; // 16 relay module and a PCA9535PW

uint8_t NewLo = 0;
uint8_t NewHi = 0;

const char *RelayControlName(uint8_t control)
{
	switch (control)
	{
	case 0:
		return "None";
	case 1:
		return "GPIO";
	case 2:
		return "PCA9555-8";
	case 3:
		return "PCA9555-16";
	case 4:
		return "MCP23017";
	case 5:
		return "PCA9685";
	case 6:
		return "PCF8574";
	default:
		return "Unknown";
	}
}

uint8_t RelayControlStep(uint8_t current, int8_t direction)
{
	int8_t next = static_cast<int8_t>(current) + ((direction >= 0) ? 1 : -1);
	if (next < 0) next = 6;
	if (next > 6) next = 0;
	return static_cast<uint8_t>(next);
}

uint8_t RemoteRelayControlStep(uint8_t current, int8_t direction)
{
	const uint8_t options[] = { 0, 2, 3, 4, 5, 6 };
	const uint8_t optionCount = sizeof(options) / sizeof(options[0]);
	uint8_t index = 0;

	for (uint8_t i = 0; i < optionCount; i++)
	{
		if (options[i] == current)
		{
			index = i;
			break;
		}
	}

	if (direction >= 0)
	{
		index = (index + 1) % optionCount;
	}
	else
	{
		index = (index == 0) ? (optionCount - 1) : (index - 1);
	}
	return options[index];
}

uint8_t RelayPinStep(uint8_t current, int8_t direction)
{
	int16_t next = (current == NC) ? 42 : current;
	next += (direction >= 0) ? 1 : -1;
	if (next < 0) next = 42;
	if (next > 42) next = 0;
	return (next == 42) ? NC : static_cast<uint8_t>(next);
}

uint8_t RemoteRelayPinStep(uint8_t current, int8_t direction)
{
	int16_t next = (current == NC) ? 16 : current;
	next += (direction >= 0) ? 1 : -1;
	if (next < 0) next = 16;
	if (next > 16) next = 0;
	return (next == 16) ? NC : static_cast<uint8_t>(next);
}

bool RelayGPIOPinIsUsable(uint8_t pin)
{
	return (pin <= 41);
}

bool RelayGPIOPinSettingIsValid(uint8_t pin)
{
	return RelayGPIOPinIsUsable(pin) || (pin == NC);
}

uint8_t RelayRequiredGPIOCount()
{
	uint8_t required = Machine.SectionCount;
	if (required < 8) required = 8;
	if (required > 16) required = 16;
	return required;
}

bool RelayIndexRequiresGPIOPin(uint8_t relay)
{
	if (relay >= RelayRequiredGPIOCount()) return false;
	if ((MDL.OnboardRelayControl == 1) && (relay < 8)) return true;
	return false;
}

void RelayLoadDefaultPins()
{
	for (uint8_t i = 0; i < 16; i++)
	{
		MDL.RelayControlPins[i] = DefaultRelayPins[i];
	}
}

void RelayLoadDefaultOnboardPins()
{
	for (uint8_t i = 0; i < 8; i++)
	{
		MDL.RelayControlPins[i] = DefaultRelayPins[i];
	}
}

void RelayLoadDefaultRemotePins()
{
	uint8_t startRelay = (MDL.OnboardRelayControl > 0) ? 8 : 0;
	for (uint8_t relay = startRelay; relay < 16; relay++)
	{
		MDL.RelayControlPins[relay] = relay - startRelay;
	}
}

void RelayPrintGPIOPins(const __FlashStringHelper *prefix)
{
	Serial.print(prefix);
	for (uint8_t i = 0; i < 16; i++)
	{
		if (RelayGPIOPinIsUsable(MDL.RelayControlPins[i]))
		{
			Serial.print(F(" R"));
			Serial.print(i + 1);
			Serial.print(F("="));
			Serial.print(MDL.RelayControlPins[i]);
		}
	}
	Serial.println();
}

void RelayApplyGPIOPinModes(uint8_t startRelay, uint8_t endRelay)
{
	for (uint8_t relay = startRelay; relay <= endRelay && relay < 16; relay++)
	{
		if (RelayGPIOPinIsUsable(MDL.RelayControlPins[relay]))
		{
			pinMode(MDL.RelayControlPins[relay], OUTPUT);
			digitalWrite(MDL.RelayControlPins[relay], !MDL.InvertRelay);
		}
	}
}

bool RelayEnsureGPIOPins()
{
	bool invalid = false;
	bool missing = false;
	bool duplicate = false;
	bool onboardChanged = false;

	if (MDL.OnboardRelayControl == 1)
	{
		for (uint8_t i = 0; i < 8; i++)
		{
			if (MDL.RelayControlPins[i] != DefaultRelayPins[i])
			{
				onboardChanged = true;
				break;
			}
		}
	}

	if (onboardChanged)
	{
		RelayLoadDefaultOnboardPins();
		Serial.println(F("Onboard GPIO relay pins are fixed; default onboard pin map restored."));
		RelayPrintGPIOPins(F("Relay GPIO onboard pin map:"));
	}

	for (uint8_t i = 0; i < 16; i++)
	{
		if (!RelayGPIOPinSettingIsValid(MDL.RelayControlPins[i]))
		{
			invalid = true;
			break;
		}
	}

	for (uint8_t i = 0; i < 16; i++)
	{
		if (RelayIndexRequiresGPIOPin(i) && !RelayGPIOPinIsUsable(MDL.RelayControlPins[i]))
		{
			missing = true;
			break;
		}
	}

	for (uint8_t i = 0; i < 16 && !duplicate; i++)
	{
		if (!RelayIndexRequiresGPIOPin(i)) continue;
		for (uint8_t j = i + 1; j < 16; j++)
		{
			if (RelayIndexRequiresGPIOPin(j) && (MDL.RelayControlPins[i] == MDL.RelayControlPins[j]))
			{
				duplicate = true;
				break;
			}
		}
	}

	if (invalid || missing || duplicate)
	{
		if (invalid) Serial.println(F("Relay GPIO pin settings contain invalid pin numbers."));
		if (missing) Serial.println(F("Relay GPIO pin settings are missing required section pins."));
		if (duplicate) Serial.println(F("Relay GPIO pin settings contain duplicated section pins."));
		RelayLoadDefaultPins();
		Serial.println(F("Relay GPIO pin settings were invalid, missing, or duplicated; default GPIO relay pins loaded."));
		RelayPrintGPIOPins(F("Relay GPIO default pin map:"));
	}

	return onboardChanged || invalid || missing || duplicate;
}

bool RelayRemotePinSettingIsValid(uint8_t pin)
{
	return (pin <= 15) || (pin == NC);
}

bool RelayEnsureRemotePins()
{
	if (MDL.RemoteRelayControl != 4) return false;

	uint8_t startRelay = (MDL.OnboardRelayControl > 0) ? 8 : 0;
	bool invalid = false;
	bool duplicate = false;

	for (uint8_t relay = startRelay; relay < 16; relay++)
	{
		if (!RelayRemotePinSettingIsValid(MDL.RelayControlPins[relay]))
		{
			invalid = true;
			break;
		}
	}

	for (uint8_t relay = startRelay; relay < 16 && !duplicate; relay++)
	{
		if (MDL.RelayControlPins[relay] == NC) continue;
		for (uint8_t other = relay + 1; other < 16; other++)
		{
			if (MDL.RelayControlPins[relay] == MDL.RelayControlPins[other])
			{
				duplicate = true;
				break;
			}
		}
	}

	if (invalid || duplicate)
	{
		RelayLoadDefaultRemotePins();
		Serial.println(F("Remote MCP23017 relay channels were invalid or duplicated; default remote channel map loaded."));
		return true;
	}

	return false;
}

bool RelayIndexIsGPIOControlled(uint8_t relay)
{
	if (relay >= 16) return false;
	if ((MDL.OnboardRelayControl == 1) && (relay < 8)) return true;
	return false;
}

FLASHMEM void PCA9685_SetChannelHigh(uint8_t channel, bool outputHigh)
{
	if (channel >= 16) return;

	const uint8_t baseRegister = 0x06 + (channel * 4);
	Wire.beginTransmission(PCA9685address);
	Wire.write(baseRegister);
	if (outputHigh)
	{
		Wire.write(0x00); // LEDn_ON_L
		Wire.write(0x10); // LEDn_ON_H full-on bit
		Wire.write(0x00); // LEDn_OFF_L
		Wire.write(0x00); // LEDn_OFF_H
	}
	else
	{
		Wire.write(0x00); // LEDn_ON_L
		Wire.write(0x00); // LEDn_ON_H
		Wire.write(0x00); // LEDn_OFF_L
		Wire.write(0x10); // LEDn_OFF_H full-off bit
	}
	Wire.endTransmission();
}

FLASHMEM void PCF8574_WriteOutputs(uint8_t relayByte)
{
	uint8_t outputByte = 0;
	for (uint8_t i = 0; i < 8; i++)
	{
		bool relayOn = bitRead(relayByte, i);
		bool outputHigh = relayOn ? MDL.InvertRelay : !MDL.InvertRelay;
		if (outputHigh) bitSet(outputByte, i);
	}

	Wire.beginTransmission(PCF8574address);
	Wire.write(outputByte);
	Wire.endTransmission();
}

FLASHMEM void RelayLogOutputState()
{
	static uint8_t lastLo = 0xFF;
	static uint8_t lastHi = 0xFF;
	static bool lastInvertRelay = false;
	static uint8_t lastOnboardControl = 0xFF;
	static uint8_t lastRemoteControl = 0xFF;

	if ((NewLo == lastLo) &&
	    (NewHi == lastHi) &&
	    (MDL.InvertRelay == lastInvertRelay) &&
	    (MDL.OnboardRelayControl == lastOnboardControl) &&
	    (MDL.RemoteRelayControl == lastRemoteControl))
	{
		return;
	}

	lastLo = NewLo;
	lastHi = NewHi;
	lastInvertRelay = MDL.InvertRelay;
	lastOnboardControl = MDL.OnboardRelayControl;
	lastRemoteControl = MDL.RemoteRelayControl;

	Serial.print(F("Relay output: newLo=0x"));
	Serial.print(NewLo, HEX);
	Serial.print(F(" newHi=0x"));
	Serial.print(NewHi, HEX);
	Serial.print(F(" onboard="));
	Serial.print(MDL.OnboardRelayControl);
	Serial.print(F("("));
	Serial.print(RelayControlName(MDL.OnboardRelayControl));
	Serial.print(F(")"));
	Serial.print(F(" remote="));
	Serial.print(MDL.RemoteRelayControl);
	Serial.print(F("("));
	Serial.print(RelayControlName(MDL.RemoteRelayControl));
	Serial.print(F(")"));
	Serial.print(F(" relay on level="));
	Serial.println(MDL.InvertRelay ? F("HIGH") : F("LOW"));

	if (MDL.OnboardRelayControl == 1)
	{
		Serial.print(F("Relay GPIO pins:"));
		for (uint8_t i = 0; i < 16; i++)
		{
			if (RelayGPIOPinIsUsable(MDL.RelayControlPins[i]))
			{
				Serial.print(F(" "));
				Serial.print(F("R"));
				Serial.print(i + 1);
				Serial.print(F("="));
				Serial.print(MDL.RelayControlPins[i]);
			}
		}
		Serial.println();
	}

	if ((MDL.OnboardRelayControl == 1) || (MDL.RemoteRelayControl == 1))
	{
		Serial.print(F("Relay GPIO writes:"));
		for (uint8_t relay = 0; relay < 16; relay++)
		{
			if (RelayIndexIsGPIOControlled(relay) && RelayGPIOPinIsUsable(MDL.RelayControlPins[relay]))
			{
				const bool relayOn = (relay < 8) ? bitRead(NewLo, relay) : bitRead(NewHi, relay - 8);
				const bool outputHigh = relayOn ? MDL.InvertRelay : !MDL.InvertRelay;
				Serial.print(F(" R"));
				Serial.print(relay + 1);
				Serial.print(F(" pin"));
				Serial.print(MDL.RelayControlPins[relay]);
				Serial.print(F("="));
				Serial.print(outputHigh ? F("HIGH") : F("LOW"));
			}
		}
		Serial.println();
	}
}

bool RelayCommandFresh()
{
	for (uint8_t i = 0; i < MDL.SensorCount && i < MaxProductCount; i++)
	{
		if (millis() - Sensor[i].CommTime < 4000) return true;
	}

#if ISOBUS_TC_MODE
	if (TC_SectionControlActive && (millis() - TC_LastSectionCommand < 4000)) return true;
#endif

	return false;
}

uint8_t RelayTestMaskLo()
{
	uint8_t mask = 0;
	for (uint8_t i = 0; i < 8; i++)
	{
		if (RelayGPIOPinIsUsable(MDL.RelayControlPins[i])) bitSet(mask, i);
	}
	return mask;
}

uint8_t RelayTestMaskHi()
{
	uint8_t mask = 0;
	for (uint8_t i = 8; i < 16; i++)
	{
		if (RelayGPIOPinIsUsable(MDL.RelayControlPins[i])) bitSet(mask, i - 8);
	}
	return mask;
}

uint8_t RelayEffectiveLo()
{
	if (RelayTestForce) return RelayTestMaskLo();
	if (RelayCommandFresh()) return RelayLo;
	return PowerRelayLo | InvertedLo;
}

uint8_t RelayEffectiveHi()
{
	if (RelayTestForce) return RelayTestMaskHi();
	if (RelayCommandFresh()) return RelayHi;
	return PowerRelayHi | InvertedHi;
}

FLASHMEM void CheckRelays()
{
	if ((MDL.OnboardRelayControl == 1) || (MDL.RemoteRelayControl == 1))
	{
		if (RelayEnsureGPIOPins())
		{
			SaveData();
			RelayApplyGPIOPinModes(0, 15);
		}
	}

	if (RelayTestForce)
	{
		NewLo = RelayTestMaskLo();
		NewHi = RelayTestMaskHi();
	}
	else if (RelayCommandFresh())
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

	RelayLogOutputState();

	// onboard relays
	ControlSwitch(0, 7, MDL.OnboardRelayControl);


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
					if (RelayGPIOPinIsUsable(MDL.RelayControlPins[Pin]) && Pin >= Start && Pin <= End) // check if relay is enabled
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
			if (PCA9685_found)
			{
				for (uint8_t relay = Start; relay <= End && relay < 16; relay++)
				{
					if (relay < 8)
					{
						BitState = bitRead(NewLo, relay);
					}
					else
					{
						BitState = bitRead(NewHi, relay - 8);
					}

					uint8_t channel = relay - Start;
					bool outputHigh = BitState ? MDL.InvertRelay : !MDL.InvertRelay;
					PCA9685_SetChannelHigh(channel, outputHigh);
				}
			}
			break;

		case 6:
			if (PCF8574_found)
			{
				uint8_t RelayByte = (Start == 0) ? NewLo : NewHi;
				PCF8574_WriteOutputs(RelayByte);
			}
			break;
		}
	}
