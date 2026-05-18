const char* RelayControlName(uint8_t control)
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

void RelayPrintGPIOPins(const __FlashStringHelper* prefix)
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
	if (TC_SectionControlActive && (millis() - TC_LastSectionCommand < 4000)) return true;

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
