
void DoSetup()
{
	uint8_t ErrorCount = 0;
	bool WheelMatch = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");

	// eeprom
	LoadData();
	LoadNetworks();
	LoadBoardID();

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
	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

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
	Serial.println("Starting Ethernet ...");
	MDLnetwork.IP3 = MDL.ID + 50;
	IPAddress LocalIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, MDLnetwork.IP3);

	// the chip's burned-in unique MAC, not an ID-derived one - two boards that
	// happen to share a module ID must still be distinct on the wire
	static uint8_t LocalMac[6];
	TeensyMAC(LocalMac);

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

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (Sensor[i].FlowPin < NC)
		{
			pinMode(Sensor[i].FlowPin, INPUT_PULLUP);

			switch (i)
			{
			case 0:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR0, FALLING);
				break;
			case 1:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, FALLING);
				break;
			case 2:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR2, FALLING);
				break;
			case 3:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR3, FALLING);
				break;
			case 4:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR4, FALLING);
				break;
			case 5:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR5, FALLING);
				break;
			}
		}

		if (Sensor[i].DirPin < NC) pinMode(Sensor[i].DirPin, OUTPUT);
		if (Sensor[i].PWMPin < NC)
		{
			pinMode(Sensor[i].PWMPin, OUTPUT);

			// pwm frequency change from default 4482 Hz to 490 Hz, required for some valves to work
			analogWriteFrequency(Sensor[i].PWMPin, 490);
		}

		if (Sensor[i].BinPin < NC) pinMode(Sensor[i].BinPin, INPUT_PULLUP);

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

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Serial.println("");
		Serial.print("Sensor ");
		Serial.print(i + 1);
		Serial.println(": ");
		Serial.print("Flow Pin: ");
		Serial.println(Sensor[i].FlowPin);
		Serial.print("DIR Pin: ");
		Serial.println(Sensor[i].DirPin);
		Serial.print("PWM Pin: ");
		Serial.println(Sensor[i].PWMPin);
		Serial.print("Bin Pin: ");
		if (Sensor[i].BinPin == NC)
		{
			Serial.println(F("Disabled"));
		}
		else
		{
			Serial.println(Sensor[i].BinPin);
		}
	}

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

	// PID damper + per-sensor auto mode + bin state
	for (int i = 0; i < MaxProductCount; i++)
	{
		OscDamp[i] = 1.0f;
		LastAboveTarget[i] = false;
		AutoOn[i] = true;
		BinEmpty[i] = false;
		BinChangeTime[i] = millis();
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
// board label			3-22
// module data			23-147
// network				168-232
// sensors 1-6			253 + i*124, ~106 bytes each (6th ends at 977, EEPROM is 4284)

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

	// sensors beyond the board's two driver channels have no default pins;
	// explicit NC - zero-initialized globals would otherwise leave pin 0 (a real pin)
	for (int i = 2; i < MaxProductCount; i++)
	{
		Sensor[i].FlowPin = NC;
		Sensor[i].DirPin = NC;
		Sensor[i].PWMPin = NC;
	}

	// default control settings
	for (int i = 0; i < MaxProductCount; i++)
	{
		Sensor[i].BinPin = NC;
		Sensor[i].BinInvert = false;
		Sensor[i].MaxPWM = 255;
		Sensor[i].MinPWM = 5;
		Sensor[i].Kp = 45 / 100.0;	// Kp = 45 (KPdefault, app Props.cs) - matches uniform /100 decode (Receive.ino)
		Sensor[i].Ki = 70 / 100.0;	// Ki = 70 (KIdefault) - matches uniform /100 decode (Receive.ino)
		Sensor[i].Deadband = 0.015;			// DeadbandDefault 15 / 1000
		Sensor[i].BrakePoint = 35;			// BrakePointDefault
		Sensor[i].PIDslowAdjust = 60;		// PIDslowAdjustDefault
		Sensor[i].SlewRate = 25;
		Sensor[i].MaxIntegral = 25;
		Sensor[i].TimedMinStart = 0.5;
		Sensor[i].TimedAdjust = 80;
		Sensor[i].TimedPause = 400;
		Sensor[i].PIDtime = 150;		// PIDtimeDefault
		Sensor[i].PulseMin = 250;		// 4000 Hz
		Sensor[i].PulseMax = 1000000;	// 1 Hz
		Sensor[i].SampleWindow = 40;	// flow window: 40 centiseconds = 400 ms
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
	MDL.InvertWork = false;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.PressurePin = 40;
	MDL.WheelCal = 0;
	MDL.WheelSpeedPin = NC;
	MDL.CommMode = 0;
}

// NC is a valid setting (input/output not used); Teensy 4.1 GPIOs are 0-41
bool PinAllowed(byte pin)
{
	return (pin <= 41 || pin == NC);
}

bool ValidData()
{
	bool Result = true;

	if (MDL.WorkPin > 41 && MDL.WorkPin != NC) Result = false;
	if (MDL.PressurePin > 41 && MDL.PressurePin != NC) Result = false;
	if (MDL.WheelSpeedPin > 41 && MDL.WheelSpeedPin != NC) Result = false;

	if (Result)
	{
		// NC is a valid setting (sensor input/output not used) - a 6-sensor module
		// may legitimately have sensors without pins assigned yet
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			if ((Sensor[i].FlowPin > 41 && Sensor[i].FlowPin != NC)
				|| (Sensor[i].DirPin > 41 && Sensor[i].DirPin != NC)
				|| (Sensor[i].PWMPin > 41 && Sensor[i].PWMPin != NC)
				|| (Sensor[i].BinPin > 41 && Sensor[i].BinPin != NC))
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

void LoadBoardID()
{
	// Independent EEPROM slot, NOT guarded by InoID, so the board label survives a firmware
	// reflash / LoadDefaults (it identifies the hardware, not the settings). SaveData()
	// only writes offsets 0/2/23/253+, never the 3-22 region, so this is left untouched there.
	BoardLabel tmp;
	EEPROM.get(EE_BoardID, tmp);
	if (tmp.Identifier == BoardIDMagic)
	{
		MDLboard = tmp;
	}
	else
	{
		// uninitialized EEPROM -> start with an empty label
		MDLboard.Identifier = BoardIDMagic;
		memset(MDLboard.Text, 0, sizeof(MDLboard.Text));
		SaveBoardID();
	}
}

void SaveBoardID()
{
	MDLboard.Identifier = BoardIDMagic;
	EEPROM.put(EE_BoardID, MDLboard);
}

void TeensyMAC(uint8_t* mac)
{
	// the unique factory MAC burned into the i.MX RT1062 fuses
	uint32_t m1 = HW_OCOTP_MAC1;
	uint32_t m2 = HW_OCOTP_MAC0;
	mac[0] = m1 >> 8;
	mac[1] = m1 >> 0;
	mac[2] = m2 >> 24;
	mac[3] = m2 >> 16;
	mac[4] = m2 >> 8;
	mac[5] = m2 >> 0;
}

void EffectiveBoardLabel(uint8_t* out)
{
	// The stored label, or "MAC xxxxxxxxxxxx" from the chip's unique MAC when
	// none is stored (runtime fallback, never written to EEPROM). Gives a fresh
	// board a unique identity so RC can tell two boards with the same module ID
	// apart. out must hold 16 bytes, 0-padded like the stored label.
	bool empty = true;
	for (byte i = 0; i < 16; i++)
	{
		if (MDLboard.Text[i] > 32 && MDLboard.Text[i] < 127)
		{
			empty = false;
			break;
		}
	}

	if (empty)
	{
		const char hex[] = "0123456789ABCDEF";
		uint8_t mac[6];
		TeensyMAC(mac);
		out[0] = 'M';
		out[1] = 'A';
		out[2] = 'C';
		out[3] = ' ';
		for (byte i = 0; i < 6; i++)
		{
			out[4 + i * 2] = hex[mac[i] >> 4];
			out[5 + i * 2] = hex[mac[i] & 0x0F];
		}
	}
	else
	{
		memcpy(out, MDLboard.Text, 16);
	}
}

