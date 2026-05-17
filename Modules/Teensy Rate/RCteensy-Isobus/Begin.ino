
void DoSetup()
{
	uint8_t ErrorCount = 0;
	bool WheelMatch = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	if (CrashReport)
	{
		Serial.print(CrashReport);
		CrashReport.clear();
		Serial.flush();
	}

	// eeprom
	LoadData();
	LoadMachineSettings();

	Serial.println("");
	Serial.println(InoDescription);

	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	String fwVer;
	if (mn <= 12 && dy <= 31)
	{
		fwVer = "Firmware Version: v";
		fwVer += String(yr);
		fwVer += ".";
		if (mn < 10) fwVer += "0";
		fwVer += String(mn);
		fwVer += ".";
		if (dy < 10) fwVer += "0";
		fwVer += String(dy);
	}
	else
	{
		fwVer = "Firmware Version: invalid";
	}
	Serial.println(fwVer);

	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.println("");

	if (MDL.WorkPin < NC) pinMode(MDL.WorkPin, INPUT_PULLUP);

	bool SettingsChanged = false;
	if (MDL.SensorCount < 1)
	{
		MDL.SensorCount = 1;
		SettingsChanged = true;
	}
	if (MDL.SensorCount > MaxProductCount)
	{
		MDL.SensorCount = MaxProductCount;
		SettingsChanged = true;
	}
	if (SettingsChanged) SaveData();

	// I2C
	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ADS1115
	ADSfound = false;
	if (MDL.ADS1115Enabled)
	{
		Serial.print("Starting ADS1115 at address ");
		Serial.println(ADS1115_Address);
		while (!ADSfound)
		{
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0b00000000);	//Point to Conversion register
			Wire.endTransmission();
			ADSfound = (Wire.requestFrom(ADS1115_Address, 2) == 2);
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");
		if (ADSfound)
		{
			Serial.println("ADS1115 found.");
			Serial.println("");
		}
		else
		{
			Serial.println("ADS1115 not found.");
			Serial.println("ADS1115 disabled.");
			Serial.println("");
		}
	}

	// analog pins
	analogReadResolution(12);

	// ethernet 
	Server_Begin();

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		pinMode(Sensor[i].DirPin, OUTPUT);
		pinMode(Sensor[i].PWMPin, OUTPUT);

		switch (i)
		{
		case 0:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR0, FALLING);
			break;
		case 1:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, FALLING);
			break;
		}

		// pwm frequency change from default 4482 Hz to 490 Hz, required for some valves to work
		analogWriteFrequency(Sensor[i].PWMPin, 490);

		if (Sensor[i].FlowPin == MDL.WheelSpeedPin) WheelMatch = true;
	}

	// wheel speed sensor
	if (MDL.WheelSpeedPin != NC && !WheelMatch)
	{
		pinMode(MDL.WheelSpeedPin, INPUT_PULLUP);
		attachInterrupt(digitalPinToInterrupt(MDL.WheelSpeedPin), ISR_Speed, FALLING);
	}

	analogWriteResolution(PWM_BITS);

	// Relays
	if (MDL.RemoteRelayControl > 0)
	{
		InitializeRelays(MDL.OnboardRelayControl, 7);
		InitializeRelays(MDL.RemoteRelayControl, -1);
	}
	else
	{
		InitializeRelays(MDL.OnboardRelayControl, 15);
	}

	pinMode(LED_BUILTIN, OUTPUT);

	// CAN/ISOBUS initialization — always started so PGN 32700 can be received
	// in any CommMode (allows switching from Ethernet to CAN without Ethernet)
	Serial.println("");
	CANBus_Begin();

	Serial.println("");
	Serial.print("Sensors enabled: ");
	Serial.println(MDL.SensorCount);
	Serial.println("");
	Serial.println("Sensor 1: ");
	Serial.print("Enabled: ");
	Serial.print("Flow Pin: ");
	Serial.println(Sensor[0].FlowPin);
	Serial.print("DIR Pin: ");
	Serial.println(Sensor[0].DirPin);
	Serial.print("PWM Pin: ");
	Serial.println(Sensor[0].PWMPin);

	Serial.println("");
	Serial.println("Sensor 2: ");
	Serial.print("Flow Pin: ");
	Serial.println(Sensor[1].FlowPin);
	Serial.print("DIR Pin: ");
	Serial.println(Sensor[1].DirPin);
	Serial.print("PWM Pin: ");
	Serial.println(Sensor[1].PWMPin);

	Serial.println("");

	Serial.print("Work Switch Pin: ");
	if (MDL.WorkPin == NC)
	{
		Serial.println(F("Disabled"));
	}
	else
	{
		Serial.println(MDL.WorkPin);
	}

	Serial.print("Pressure Pin: ");
	if (MDL.PressurePin == NC)
	{
		Serial.println(F("Disabled"));
	}
	else
	{
		Serial.println(MDL.PressurePin);
	}

	Serial.print(F("Wheel Speed Pin: "));
	if (WheelMatch)
	{
		Serial.println(F("error, duplicate flow pin"));
	}
	else if (MDL.WheelSpeedPin == 255)
	{
		Serial.println(F("Disabled"));
	}
	else
	{
		Serial.println(MDL.WheelSpeedPin);
	}

	if (ADSfound)
	{
		Serial.println(F("ADS1115: Enabled "));
	}
	else
	{
		Serial.println(F("ADS1115: Disabled "));
	}

	Serial.print(F("Comm Mode: "));
	switch (MDL.CommMode)
	{
	case 0:
		Serial.println(F("UDP only"));
		break;
	case 1:
		Serial.println(F("CAN/ISOBUS Proprietary"));
		break;
	case 2:
		Serial.println(F("UDP + CAN/ISOBUS Proprietary"));
		break;
	default:
		Serial.println(F("Unknown"));
		break;
	}

	if (GoodPins)
	{
		Serial.println(F("Pin configuration correct."));
	}
	else
	{
		Serial.println(F("Pin configuration not correct."));
	}

	if (MDL.Is3Wire)
	{
		Serial.println(F("Valves are 3 wire."));
	}
	else
	{
		Serial.println(F("Valves are 2 wire."));
	}

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void InitializeRelays(uint8_t Control, int8_t End)
{
	uint8_t ErrorCount;
	switch (Control)
	{
	case 1:
		// Relay GPIO Pins
		Serial.println("");
		Serial.println("Using GPIO pins for relays.");
		for (int i = 0; i <= End; i++)
		{
			if (MDL.RelayControlPins[i] < NC)
			{
				pinMode(MDL.RelayControlPins[i], OUTPUT);
			}
		}
		break;

	case 2:
	case 3:
		// PCA9555 I/O expander on default address 0x20
		Serial.println("");
		Serial.println("Starting PCA9555 I/O Expander for relays ...");
		ErrorCount = 0;
		while (!PCA9555PW_found)
		{
			Serial.print(".");
			Wire.beginTransmission(0x20);
			PCA9555PW_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (PCA9555PW_found)
		{
			Serial.println("PCA9555 expander found.");

			PCA.attach(Wire);
			PCA.polarity(PCA95x5::Polarity::ORIGINAL_ALL);
			PCA.direction(PCA95x5::Direction::OUT_ALL);
			PCA.write(PCA95x5::Level::H_ALL);
		}
		else
		{
			Serial.println("PCA9555 expander not found.");
		}
		Serial.println("");
		break;

	case 4:
		// MCP23017 I/O expander on 0x20, 0x21

		Serial.println("");
		Serial.println("Starting MCP23017 for relays ...");

		ErrorCount = 0;
		MCP23017address = 0x21;
		while (!MCP23017_found)
		{
			// RC12-3
			Serial.print(".");
			Wire.beginTransmission(0x21);
			MCP23017_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		if (!MCP23017_found)
		{
			ErrorCount = 0;
			MCP23017address = 0x20;
			while (!MCP23017_found)
			{
				Serial.print(".");
				Wire.beginTransmission(MCP23017address);
				MCP23017_found = (Wire.endTransmission() == 0);
				ErrorCount++;
				delay(500);
				if (ErrorCount > 5) break;
			}
		}

		Serial.println("");
		if (MCP23017_found)
		{
			Wire.beginTransmission(MCP23017address);
			Wire.write(0x00); // IODIRA register
			Wire.write(0x00); // set all of port A to outputs
			Wire.endTransmission();

			Wire.beginTransmission(MCP23017address);
			Wire.write(0x01); // IODIRB register
			Wire.write(0x00); // set all of port B to outputs
			Wire.endTransmission();

			Serial.println("MCP23017 found.");
		}
		else
		{
			Serial.println("MCP23017 not found.");
		}
		break;
	}
}

// eeprom map:
// ID					0-1
// module type			2
// module data			23-147
// network				168-232
// sensor 1				253-356
// sensor 2				377-480

void LoadData()
{
	bool IsValid = false;
	int16_t StoredID;
	int8_t StoredType;
	EEPROM.get(0, StoredID);
	EEPROM.get(2, StoredType);
	if (StoredID == InoID && StoredType == InoType)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(23, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(253 + i * 124, Sensor[i]);
		}
		IsValid = ValidData();
	}

	if (!IsValid)
	{
		Serial.println("Stored settings not valid.");
		LoadDefaults();
		SaveData();
		GoodPins = true;
	}
}

void SaveData()
{
	Serial.println("Updating stored settings.");
	EEPROM.put(0, InoID);
	EEPROM.put(2, InoType);
	EEPROM.put(23, MDL);

	for (int i = 0; i < MaxProductCount; i++)
	{
		EEPROM.put(253 + i * 124, Sensor[i]);
	}
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	// RC11-2
	Sensor[0].FlowPin = 28;
	Sensor[0].DirPin = 37;
	Sensor[0].PWMPin = 36;

	Sensor[1].FlowPin = 29;
	Sensor[1].DirPin = 14;
	Sensor[1].PWMPin = 15;

	// default control settings
	for (int i = 0; i < 2; i++)
	{
		Sensor[i].MaxPWM = 255;
		Sensor[i].MinPWM = 5;
		Sensor[i].Kp = pow(1.1, 65 - 120);	// Kp = 65
		Sensor[i].Ki = pow(1.1, 65 - 120);	// Ki = 65
		Sensor[i].Deadband = 0.015;
		Sensor[i].BrakePoint = 35;
		Sensor[i].PIDslowAdjust = 80;
		Sensor[i].SlewRate = 25;
		Sensor[i].MaxIntegral = 25;
		Sensor[i].TimedMinStart = 0.5;
		Sensor[i].TimedAdjust = 80;
		Sensor[i].TimedPause = 400;
		Sensor[i].PIDtime = 100;
		Sensor[i].PulseMin = 250;		// 4000 Hz
		Sensor[i].PulseMax = 1000000;	// 1 Hz
		Sensor[i].PulseSampleSize = 12;
	}

	// relay pins
	for (int i = 0; i < 16; i++)
	{
		MDL.RelayControlPins[i] = DefaultRelayPins[i];
	}

	// module settings
	MDL.ID = 0;
	MDL.SensorCount = 1;
	MDL.InvertRelay = true;
	MDL.InvertFlow = true;
	MDL.OnboardRelayControl = 1;
	MDL.RemoteRelayControl = 0;
	MDL.WorkPin = 30;
	MDL.WorkPinIsMomentary = false;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.PressurePin = 40;
	MDL.WheelCal = 0;
	MDL.WheelSpeedPin = NC;
	MDL.CommMode = 0;
}

bool ValidData()
{
	bool Result = true;

	if (MDL.WorkPin > 41 && MDL.WorkPin != NC) Result = false;
	if (MDL.PressurePin > 41 && MDL.PressurePin != NC) Result = false;
	if (MDL.WheelSpeedPin > 41 && MDL.WheelSpeedPin != NC) Result = false;

	if (Result)
	{
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			if ((Sensor[i].FlowPin > 41) || (Sensor[i].DirPin > 41) || (Sensor[i].PWMPin > 41))
			{
				Result = false;
				break;
			}
		}
	}

	if (Result && MDL.OnboardRelayControl == 1)
	{
		// check GPIOs for relays
		for (int i = 0; i < 16; i++)
		{
			if (MDL.RelayControlPins[i] > 41 && MDL.RelayControlPins[i] != NC)
			{
				Result = false;
				break;
			}
		}
	}

	GoodPins = Result;
	return Result;
}

void LoadMachineSettings()
{
	MachineSettings stored;
	EEPROM.get(640, stored);

	if (stored.Identifier == MACHINE_SETTINGS_IDENTIFIER)
	{
		Machine = stored;
	}
	else
	{
		// Set fresh defaults
		Machine.SectionCount = 8;
		for (int i = 0; i < 16; i++) Machine.SectionWidthCm[i] = 50;
		for (int i = 0; i < MaxProductCount; i++)
		{
			Machine.TargetUPM[i] = Sensor[i].TargetUPM;
			Machine.MeterCal[i] = (Sensor[i].MeterCal > 0.0f) ? Sensor[i].MeterCal : 600.0f;
			Machine.TargetRateLHa[i] = 100.0f;
			Machine.ManualPWM[i] = 0;
		}
		Machine.AutoOn = true;
		Machine.TankCapacityUnits = 0.0f;
		Machine.TankRemainingUnits = 0.0f;
		Machine.UnitMode = 0;
		Machine.TripAreaHa = 0.0f;
		Machine.TripAppliedUnits = 0.0f;
		Machine.LifetimeAreaHa = 0.0f;
		Machine.LifetimeAppliedUnits = 0.0f;
		Machine.LifetimeHours = 0.0f;

		// Migrate from version 0x2026
		//struct PreviousMachineSettings
		//{
		//    uint16_t Identifier;
		//    uint8_t  SectionCount;
		//    uint16_t SectionWidthCm[16];
		//    float    TargetUPM[MaxProductCount];
		//    float    MeterCal[MaxProductCount];
		//    float    TargetRateLHa[MaxProductCount];
		//    int16_t  ManualPWM[MaxProductCount];
		//    bool     AutoOn;
		//    float    TankCapacityUnits;
		//    float    TankRemainingUnits;
		//    uint8_t  UnitMode;
		//    float    TripAreaHa;
		//    float    TripAppliedUnits;
		//    float    LifetimeAreaHa;
		//    float    LifetimeAppliedUnits;
		//    float    LifetimeHours;
		//};
		//PreviousMachineSettings previous;
		//EEPROM.get(640, previous);
		//if (previous.Identifier == 0x2026)
		//{
		//    // copy preserved fields, default any new ones added above
		//}

		SaveMachineSettings();
	}

	ApplyMachineSettings();
}

void SaveMachineSettings()
{
	Machine.Identifier = MACHINE_SETTINGS_IDENTIFIER;
	EEPROM.put(640, Machine);
}

void ApplyMachineSettings()
{
	Machine.SectionCount = constrain(Machine.SectionCount, 1, 16);
	for (int i = 0; i < 16; i++) Machine.SectionWidthCm[i] = constrain(Machine.SectionWidthCm[i], 1, 2000);

	for (int i = 0; i < MaxProductCount; i++)
	{
		if (Machine.TargetUPM[i] < 0.0f) Machine.TargetUPM[i] = 0.0f;
		Machine.TargetRateLHa[i] = constrain(Machine.TargetRateLHa[i], 0.0f, 10000.0f);
		Machine.ManualPWM[i] = constrain(Machine.ManualPWM[i], -4095, 4095);
		if (Machine.MeterCal[i] > 0.0f) Sensor[i].MeterCal = Machine.MeterCal[i];
		Sensor[i].ManualAdjust = Machine.ManualPWM[i];
	}

	AutoOn = Machine.AutoOn;
	if (!Machine.AutoOn)
	{
		for (int i = 0; i < MDL.SensorCount; i++) Sensor[i].TargetUPM = 0.0f;
	}

	Machine.TankCapacityUnits = constrain(Machine.TankCapacityUnits, 0.0f, 100000.0f);
	Machine.TankRemainingUnits = constrain(Machine.TankRemainingUnits, 0.0f, 100000.0f);
	if (Machine.TankRemainingUnits > Machine.TankCapacityUnits && Machine.TankCapacityUnits > 0.0f)	Machine.TankRemainingUnits = Machine.TankCapacityUnits;

	if (Machine.UnitMode > 3) Machine.UnitMode = 0;

	if (!(Machine.TripAreaHa >= 0.0f && Machine.TripAreaHa <= 1000000.0f)) Machine.TripAreaHa = 0.0f;
	if (!(Machine.TripAppliedUnits >= 0.0f && Machine.TripAppliedUnits <= 100000000.0f)) Machine.TripAppliedUnits = 0.0f;
	if (!(Machine.LifetimeAreaHa >= 0.0f && Machine.LifetimeAreaHa <= 10000000.0f))	Machine.LifetimeAreaHa = 0.0f;
	if (!(Machine.LifetimeAppliedUnits >= 0.0f && Machine.LifetimeAppliedUnits <= 1000000000.0f)) Machine.LifetimeAppliedUnits = 0.0f;
	if (!(Machine.LifetimeHours >= 0.0f && Machine.LifetimeHours <= 1000000.0f)) Machine.LifetimeHours = 0.0f;
}