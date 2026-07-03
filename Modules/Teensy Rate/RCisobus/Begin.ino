
void DoSetup()
{
	uint8_t ErrorCount = 0;
	bool WheelMatch = false;

	Sensor[0].AdjustmentEnabled = false;
	Sensor[1].AdjustmentEnabled = false;

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
	LoadNetworks();

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
	bool moduleSettingsChanged = false;
	#if ISOBUS_TC_MODE
	if (MDL.SensorCount < 1)
	{
		MDL.SensorCount = 1;
		moduleSettingsChanged = true;
	}
	#endif
	if (MDL.SensorCount > MaxProductCount)
	{
		MDL.SensorCount = MaxProductCount;
		moduleSettingsChanged = true;
	}
	if (moduleSettingsChanged) SaveData();

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

#if ETHERNET_COMM_ENABLED
	// ethernet
	Serial.println("Starting Ethernet ...");
	MDLnetwork.IP3 = MDL.ID + 50;
	IPAddress LocalIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, MDLnetwork.IP3);
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDLnetwork.IP3 };

	Ethernet.begin(LocalMac, 0);
	Ethernet.setLocalIP(LocalIP);

	delay(1500);
	if (Ethernet.linkStatus() == LinkON)
	{
		Serial.println("Ethernet Connected.");
	}
	else
	{
		Serial.println("Ethernet Not Connected.");
	}
	Serial.print("IP Address: ");
	Serial.println(Ethernet.localIP());
	DestinationIP = IPAddress(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255);	// update from saved data

	// UDP
	UDPcomm.begin(ListeningPort);

	// update
	UpdateComm.begin(UpdateReceivePort);
#else
	Serial.println("Ethernet communication disabled; machine setup is controlled from ISOBUS VT.");
#endif

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
	if (MDL.WheelSpeedPin != NC && !WheelMatch )
	{
		pinMode(MDL.WheelSpeedPin, INPUT_PULLUP);
		attachInterrupt(digitalPinToInterrupt(MDL.WheelSpeedPin), ISR_Speed, FALLING);
	}

	analogWriteResolution(PWM_BITS);

	// Relays
	InitializeRelayOutputs();

	pinMode(LED_BUILTIN, OUTPUT);

	// CAN/ISOBUS initialization — always started so PGN 32700 can be received
	// in any CommMode (allows switching from Ethernet to CAN without Ethernet)
	Serial.println("");
	Serial.println("Starting CAN/ISOBUS ...");
	Serial.flush();
	CANBus_Begin();
	Serial.println("CAN/ISOBUS started.");
	Serial.flush();

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

void InitializeRelayOutputs()
{
	PCA9555PW_found = false;
	MCP23017_found = false;
	PCA9685_found = false;
	PCF8574_found = false;

	bool relayConfigChanged = false;
	if (MDL.OnboardRelayControl > 1)
	{
		MDL.OnboardRelayControl = 1;
		relayConfigChanged = true;
	}
	if ((MDL.RemoteRelayControl == 1) || (MDL.RemoteRelayControl > 6))
	{
		MDL.RemoteRelayControl = 0;
		relayConfigChanged = true;
	}
	if ((MDL.OnboardRelayControl == 1) || (MDL.RemoteRelayControl == 1))
	{
		relayConfigChanged = RelayEnsureGPIOPins() || relayConfigChanged;
	}
	if (MDL.RemoteRelayControl == 4)
	{
		relayConfigChanged = RelayEnsureRemotePins() || relayConfigChanged;
	}
	if (relayConfigChanged) SaveData();

	if (MDL.RemoteRelayControl > 0)
	{
		InitializeRelays(MDL.OnboardRelayControl, 7);
		InitializeRelays(MDL.RemoteRelayControl, 15);
	}
	else
	{
		InitializeRelays(MDL.OnboardRelayControl, 15);
	}
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
		RelayPrintGPIOPins(F("Relay GPIO active pin map:"));
		RelayApplyGPIOPinModes(0, End);
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

		case 5:
			Serial.println("");
			Serial.println("Starting PCA9685 for relays ...");
			ErrorCount = 0;
			while (!PCA9685_found)
			{
				Serial.print(".");
				Wire.beginTransmission(PCA9685address);
				PCA9685_found = (Wire.endTransmission() == 0);
				ErrorCount++;
				delay(500);
				if (ErrorCount > 5) break;
			}

			Serial.println("");
			if (PCA9685_found)
			{
				Wire.beginTransmission(PCA9685address);
				Wire.write(0x00); // MODE1
				Wire.write(0x00); // Normal mode
				Wire.endTransmission();

				Wire.beginTransmission(PCA9685address);
				Wire.write(0x01); // MODE2
				Wire.write(0x04); // Totem pole outputs
				Wire.endTransmission();

				for (uint8_t channel = 0; channel < 16; channel++)
				{
					PCA9685_SetChannelHigh(channel, !MDL.InvertRelay);
				}
				Serial.println("PCA9685 found.");
			}
			else
			{
				Serial.println("PCA9685 not found.");
			}
			break;

		case 6:
			Serial.println("");
			Serial.println("Starting PCF8574 for relays ...");
			ErrorCount = 0;
			while (!PCF8574_found)
			{
				Serial.print(".");
				Wire.beginTransmission(PCF8574address);
				PCF8574_found = (Wire.endTransmission() == 0);
				ErrorCount++;
				delay(500);
				if (ErrorCount > 5) break;
			}

			Serial.println("");
			if (PCF8574_found)
			{
				PCF8574_WriteOutputs(0);
				Serial.println("PCF8574 found.");
			}
			else
			{
				Serial.println("PCF8574 not found.");
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
		GoodPins=true;
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
		Sensor[i].ControlType = StandardValve_ct;
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
	MDL.RemoteRelayControl=0;
	MDL.WorkPin = 30;
	MDL.WorkPinIsMomentary = false;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.PressurePin = 40;
	MDL.WheelCal = 0;
	MDL.WheelSpeedPin = NC;
	MDL.CommMode = 1;
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
		struct LegacyMachineSettings
		{
			uint16_t Identifier;
			uint8_t SectionCount;
			uint16_t SectionWidthCm[16];
			float TargetUPM[MaxProductCount];
			float MeterCal[MaxProductCount];
		};

		struct PreviousMachineSettings
		{
			uint16_t Identifier;
			uint8_t SectionCount;
			uint16_t SectionWidthCm[16];
			float TargetUPM[MaxProductCount];
			float MeterCal[MaxProductCount];
			float TargetRateLHa[MaxProductCount];
			int16_t ManualPWM[MaxProductCount];
			bool AutoRate[MaxProductCount];
		};

		struct Previous5445MachineSettings	// 0x5445 layout: per-product AutoRate[], since replaced by global AutoOn
		{
			uint16_t Identifier;
			uint8_t SectionCount;
			uint16_t SectionWidthCm[16];
			float TargetUPM[MaxProductCount];
			float MeterCal[MaxProductCount];
			float TargetRateLHa[MaxProductCount];
			int16_t ManualPWM[MaxProductCount];
			bool AutoRate[MaxProductCount];
			float TankCapacityUnits;
			float TankRemainingUnits;
			uint8_t UnitMode;
			float TripAreaHa;
			float TripAppliedUnits;
			float LifetimeAreaHa;
			float LifetimeAppliedUnits;
		};

		LegacyMachineSettings legacy;
		PreviousMachineSettings previous;
		Previous5445MachineSettings previous5445;
		EEPROM.get(640, legacy);
		EEPROM.get(640, previous);
		EEPROM.get(640, previous5445);

		Machine.Identifier = MACHINE_SETTINGS_IDENTIFIER;
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

		if (previous5445.Identifier == 0x5445)
		{
			Machine.SectionCount = previous5445.SectionCount;
			for (int i = 0; i < 16; i++) Machine.SectionWidthCm[i] = previous5445.SectionWidthCm[i];
			for (int i = 0; i < MaxProductCount; i++)
			{
				Machine.TargetUPM[i] = previous5445.TargetUPM[i];
				Machine.MeterCal[i] = previous5445.MeterCal[i];
				Machine.TargetRateLHa[i] = previous5445.TargetRateLHa[i];
				Machine.ManualPWM[i] = previous5445.ManualPWM[i];
			}
			Machine.AutoOn = previous5445.AutoRate[0];	// product 0 governed TC mode in the old layout
			Machine.TankCapacityUnits = previous5445.TankCapacityUnits;
			Machine.TankRemainingUnits = previous5445.TankRemainingUnits;
			Machine.UnitMode = previous5445.UnitMode;
			Machine.TripAreaHa = previous5445.TripAreaHa;
			Machine.TripAppliedUnits = previous5445.TripAppliedUnits;
			Machine.LifetimeAreaHa = previous5445.LifetimeAreaHa;
			Machine.LifetimeAppliedUnits = previous5445.LifetimeAppliedUnits;
		}
		else if (previous.Identifier == 0x5444)
		{
			Machine.SectionCount = previous.SectionCount;
			for (int i = 0; i < 16; i++) Machine.SectionWidthCm[i] = previous.SectionWidthCm[i];
			for (int i = 0; i < MaxProductCount; i++)
			{
				Machine.TargetUPM[i] = previous.TargetUPM[i];
				Machine.MeterCal[i] = previous.MeterCal[i];
				Machine.TargetRateLHa[i] = previous.TargetRateLHa[i];
				Machine.ManualPWM[i] = previous.ManualPWM[i];
			}
			Machine.AutoOn = previous.AutoRate[0];
		}
		else if (legacy.Identifier == 0x5443)
		{
			Machine.SectionCount = legacy.SectionCount;
			for (int i = 0; i < 16; i++) Machine.SectionWidthCm[i] = legacy.SectionWidthCm[i];
			for (int i = 0; i < MaxProductCount; i++)
			{
				Machine.TargetUPM[i] = legacy.TargetUPM[i];
				Machine.MeterCal[i] = legacy.MeterCal[i];
			}
		}
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
	for (int i = 0; i < 16; i++)
	{
		Machine.SectionWidthCm[i] = constrain(Machine.SectionWidthCm[i], 1, 2000);
	}

	for (int i = 0; i < MaxProductCount; i++)
	{
		if (Machine.TargetUPM[i] < 0.0f) Machine.TargetUPM[i] = 0.0f;
		Machine.TargetRateLHa[i] = constrain(Machine.TargetRateLHa[i], 0.0f, 10000.0f);
		Machine.ManualPWM[i] = constrain(Machine.ManualPWM[i], -4095, 4095);

		if (Machine.MeterCal[i] > 0.0f) Sensor[i].MeterCal = Machine.MeterCal[i];
		Sensor[i].ManualAdjust = Machine.ManualPWM[i];
		if (!Machine.AutoOn) Sensor[i].TargetUPM = 0.0f;
	}

	Machine.TankCapacityUnits = constrain(Machine.TankCapacityUnits, 0.0f, 100000.0f);
	Machine.TankRemainingUnits = constrain(Machine.TankRemainingUnits, 0.0f, 100000.0f);
	if (Machine.UnitMode > 1) Machine.UnitMode = 0;
	if (!(Machine.TripAreaHa >= 0.0f && Machine.TripAreaHa <= 1000000.0f)) Machine.TripAreaHa = 0.0f;
	if (!(Machine.TripAppliedUnits >= 0.0f && Machine.TripAppliedUnits <= 100000000.0f)) Machine.TripAppliedUnits = 0.0f;
	if (!(Machine.LifetimeAreaHa >= 0.0f && Machine.LifetimeAreaHa <= 10000000.0f)) Machine.LifetimeAreaHa = 0.0f;
	if (!(Machine.LifetimeAppliedUnits >= 0.0f && Machine.LifetimeAppliedUnits <= 1000000000.0f)) Machine.LifetimeAppliedUnits = 0.0f;
	if ((Machine.TankCapacityUnits > 0.0f) && (Machine.TankRemainingUnits > Machine.TankCapacityUnits))
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
	}

	MDL.CommMode = 1;
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

	if (Result && ((MDL.OnboardRelayControl == 1) || (MDL.RemoteRelayControl == 1)))
	{
		// check GPIOs for relays
		for (int i = 0; i < 16; i++)
		{
				if (!RelayGPIOPinSettingIsValid(MDL.RelayControlPins[i]))
			{
				Result = false;
				break;
			}
		}
	}

	GoodPins = Result;
	return Result;
}

void LoadNetworks()
{
	ModuleNetwork tmp;
	EEPROM.get(168, tmp);
	if (tmp.Identifier == 9876)
	{
		MDLnetwork = tmp;
	}
	else
	{
		// load network defaults
		MDLnetwork.Identifier = 9876;
		MDLnetwork.IP0 = 192;
		MDLnetwork.IP1 = 168;
		MDLnetwork.IP2 = 1;
		MDLnetwork.IP3 = 50;
		MDLnetwork.WifiModeUseStation = false;
		strcpy(MDLnetwork.SSID, "Tractor");
		strcpy(MDLnetwork.Password, "111222333");

		SaveNetworks();
	}
}

void SaveNetworks()
{
	EEPROM.put(168, MDLnetwork);
}
