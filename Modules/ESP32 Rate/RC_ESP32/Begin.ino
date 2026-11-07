// valid pins for each processor
uint8_t ValidPins0[] = { 0,2,4,13,14,15,16,17,21,22,25,26,27,32,33,34,35,36,39 };	// SPI pins 5,18,19,23 excluded for ethernet module
uint8_t OutputPins0[] = { 0,2,4,13,14,15,16,17,21,22,25,26,27,32,33 };	// GPIO 34-39 are input-only, cannot drive IN1/IN2

// NC is always allowed (input/output not used)
bool PinAllowed(byte pin)
{
	bool Result = (pin == NC);
	for (int i = 0; !Result && i < (int)sizeof(ValidPins0); i++)
	{
		Result = (pin == ValidPins0[i]);
	}
	return Result;
}

bool OutputPinAllowed(byte pin)
{
	bool Result = (pin == NC);
	for (int i = 0; !Result && i < (int)sizeof(OutputPins0); i++)
	{
		Result = (pin == OutputPins0[i]);
	}
	return Result;
}

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
	EEPROM.begin(EEPROM_SIZE);
	LoadData();

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
	Wire.begin();			// I2C on pins SCL 22, SDA 21
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz
	// 25ms timeout. setWireTimeout(us, reset) is AVR only - the ESP32 Wire takes
	// milliseconds and its driver recovers the bus itself, so there is no reset flag
	Wire.setTimeOut(25);

	// ADS1115
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

	// ethernet 
	Serial.println("Starting Ethernet ...");
	MDLnetwork.IP3 = MDL.ID + 50;
	IPAddress LocalIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, MDLnetwork.IP3);

	// the chip's unique efuse MAC (+3, the ESP-IDF ethernet offset, so it can't
	// collide with the WiFi STA MAC), not an ID-derived one - two boards that
	// happen to share a module ID must still be distinct on the wire
	static uint8_t LocalMac[6];
	ChipMAC(LocalMac);
	LocalMac[5] += 3;

	Ethernet.init(W5500_SS);   // SS pin
	IPAddress Gateway(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 1);
	IPAddress Mask(255, 255, 255, 0);
	Ethernet.begin(LocalMac, LocalIP, Gateway, Gateway, Mask);

	delay(1500);
	ChipFound = (Ethernet.hardwareStatus() != EthernetNoHardware);
	if (ChipFound)
	{
		if (Ethernet.linkStatus() == LinkON)
		{
			Serial.println("Ethernet connected.");
		}
		else
		{
			Serial.println("Ethernet not connected.");
		}
		Serial.print("IP Address: ");
		Serial.println(Ethernet.localIP());
	}
	else
	{
		Serial.println("Ethernet hardware not found.");
	}

	Ethernet_DestinationIP = IPAddress(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255);	// update from saved data

	// UDP
	UDP_Ethernet.begin(ListeningPort);

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (Sensor[i].FlowPin < NC)
		{
			pinMode(Sensor[i].FlowPin, INPUT_PULLUP);

			switch (i)
			{
			case 0:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR0, RISING);
				break;
			case 1:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, RISING);
				break;
			case 2:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR2, RISING);
				break;
			case 3:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR3, RISING);
				break;
			case 4:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR4, RISING);
				break;
			case 5:
				attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR5, RISING);
				break;
			}
		}

		// pwm frequency change from default 5000 Hz to 490 Hz, required for some valves to work
		if (Sensor[i].IN1 < NC)
		{
			pinMode(Sensor[i].IN1, OUTPUT);
			ledcAttach(Sensor[i].IN1, PWM_FREQ, PWM_BITS);
			ledcWrite(Sensor[i].IN1, 0);
		}

		if (Sensor[i].IN2 < NC)
		{
			pinMode(Sensor[i].IN2, OUTPUT);
			ledcAttach(Sensor[i].IN2, PWM_FREQ, PWM_BITS);
			ledcWrite(Sensor[i].IN2, 0);
		}

		if (Sensor[i].BinPin < NC) pinMode(Sensor[i].BinPin, INPUT_PULLUP);

		if (Sensor[i].FlowPin == MDL.WheelSpeedPin) WheelMatch = true;
	}

	// wheel speed sensor
	if (MDL.WheelSpeedPin != NC && !WheelMatch)
	{
		pinMode(MDL.WheelSpeedPin, INPUT_PULLUP);
		attachInterrupt(digitalPinToInterrupt(MDL.WheelSpeedPin), ISR_Speed, FALLING);
	}

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

	// Wifi
	WiFi.mode(WIFI_MODE_APSTA);
	WiFi.disconnect(true);

	// Access Point
	Wifi_DestinationIP = IPAddress(192, 168, MDL.ID + 200, 255);
	IPAddress AP_LocalIP = IPAddress(192, 168, MDL.ID + 200, 1);
	IPAddress AP_GateWay = AP_LocalIP;
	IPAddress AP_Subnet(255, 255, 255, 0);

	// hotspot name: APname + "_" + ID + "_" + the chip MAC's last 3 bytes, e.g. RateModule_25_123456.
	// getEfuseMac() holds the MAC's first byte lowest, so bytes 3-5 (the board's own part,
	// after the 3-byte maker code) are bits 24-47. The old low 32 bits were the maker code
	// plus a single board byte.
	uint64_t mac = ESP.getEfuseMac();
	char suffix[7]; // 6 hex + null
	sprintf(suffix, "%02X%02X%02X", (uint8_t)(mac >> 24), (uint8_t)(mac >> 32), (uint8_t)(mac >> 40));

	String AP = APname;
	AP += "_";
	AP += String(MDL.ID);
	AP += "_";
	AP += suffix;

	// Bring the hotspot up on the channel the station network was last found on.
	// One radio serves both interfaces and they cannot sit on different channels,
	// so joining a network on any other channel DRAGS the softAP across and
	// disconnects everyone on it. Starting where the join will land makes that a
	// non-event. Falls back to 6, which is what this always used to be.
	uint8_t APchannel = 6;
	if (MDLnetwork.StaChannelCache >= 1 && MDLnetwork.StaChannelCache <= 13)
	{
		APchannel = MDLnetwork.StaChannelCache;
	}

	WiFi.softAPConfig(AP_LocalIP, AP_GateWay, AP_Subnet);
	if (strlen(MDLnetwork.APpassword) >= 8)
	{
		// WPA2-PSK
		WiFi.softAP(AP.c_str(), MDLnetwork.APpassword, APchannel, false, 4);
	}
	else
	{
		// Fallback: invalid WPA passphrase length -> force open
		WiFi.softAP(AP.c_str(), nullptr, APchannel, false, 4);
	}

	dnsServer.start(AP_DNS_PORT, "*", AP_LocalIP);

	UDP_Wifi.begin(ListeningPort);

	Serial.println("");
	Serial.print("Access Point name: ");
	Serial.println(AP);
	Serial.print("Settings Page IP: ");
	Serial.println(AP_LocalIP);
	Serial.print("Access Point channel: ");
	Serial.println(APchannel);

	// web server
	Serial.println();
	Serial.println("Starting Web Server");

	server.on("/", HandleRoot);
	server.on("/page1", HandlePage1);
	server.on("/page2", HandlePage2);
	server.on("/ButtonPressed", ButtonPressed);
	server.onNotFound(HandleRoot);

	server.on("/generate_204", []() {server.send(204, "text/plain", "");	});
	server.on("/fwlink", []() { server.send(200, "text/plain", "OK"); });
	server.on("/hotspot-detect.html", HTTP_GET, []() { server.send(200, "text/html", "<html><body>Portal</body></html>"); });
	server.on("/ncsi.txt", HTTP_GET, []() { server.send(200, "text/plain", "Microsoft NCSI"); });
	// Windows 10's probe — /ncsi.txt above is the Windows 7/8 one. Without this
	// the poll falls through to HandleRoot, Windows gets a page of HTML where it
	// expects this exact string, decides the hotspot is a captive portal and
	// launches the browser. Windows polls it for as long as a PC sits on the
	// hotspot, so answering here also stops the page being rebuilt every time —
	// that page build is one of the longer things loop() ever blocks on.
	server.on("/connecttest.txt", HTTP_GET, []() { server.send(200, "text/plain", "Microsoft Connect Test"); });

	// Register custom update page BEFORE ESP2SOTA so it takes priority (first registration wins)
	server.on("/update", HTTP_GET, []() {
		server.sendHeader("Connection", "close");
		server.send(200, "text/html", GetPageUpdate());
	});

	server.begin();

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);

	Serial.println("OTA started.");

	// wifi client mode — see Wifi.ino
	StartWifiStation();

	delay(1500);

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
		Serial.print("IN1 Pin: ");
		Serial.println(Sensor[i].IN1);
		Serial.print("IN2 Pin: ");
		Serial.println(Sensor[i].IN2);
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
	else if (MDL.WheelSpeedPin == NC)
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

	case 5:
		// PCA9685
		Serial.println("");
		Serial.println("Starting PCA9685 I/O Expander for relays ...");
		ErrorCount = 0;
		while (!PCA9685_found)
		{
			Serial.print(".");
			Wire.beginTransmission(PCA9685Address);
			PCA9685_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5)break;
		}

		Serial.println("");
		if (PCA9685_found)
		{
			Serial.println("PCA9685 expander found.");
			PWMServoDriver.begin();
			PWMServoDriver.setPWMFreq(200);

			pinMode(OutputEnablePin, OUTPUT);
			digitalWrite(OutputEnablePin, LOW);	//enable
		}
		else
		{
			Serial.println("PCA9685 expander not found.");
		}
		break;

	case 6:
		// PCF8574
		Serial.println("");
		Serial.println("Starting PCF8574 I/O Expander for relays ...");
		ErrorCount = 0;
		while (!PCF_found)
		{
			Serial.print(".");
			Wire.beginTransmission(PCF8574address);
			PCF_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (PCF_found)
		{
			Serial.println("PCF8574 found.");
			PCF.begin();
		}
		else
		{
			Serial.println("PCF8574 not found.");
		}
		break;
	}
}

// eeprom map (slot constants and header layout in RC_ESP32.ino):
//   identity      0-255         ID, board label, network, hotspot password. Kept across firmware updates
//   module data   256-319       ModuleConfig
//   sensors 1-6   320 + i*112   stored part of SensorConfig (before UPM); sensor 6 ends at 991
//   free          992-1023
// Each slot: 5-byte header (magic, InoType, version, CRC16), then the data.

// ── EEPROM records ── header layout and slots in RC_ESP32.ino
uint16_t Crc16(uint16_t crc, const uint8_t* data, uint16_t len)
{
	// CRC-16/CCITT-FALSE, start with 0xFFFF
	for (uint16_t i = 0; i < len; i++)
	{
		crc ^= (uint16_t)data[i] << 8;
		for (uint8_t b = 0; b < 8; b++) crc = (crc & 0x8000) ? (crc << 1) ^ 0x1021 : (crc << 1);
	}
	return crc;
}

void WriteRecord(uint16_t Addr, uint8_t Magic, uint8_t Version, const void* Data, uint16_t Len)
{
	// no commit here: the caller commits once after all its records
	uint8_t Hdr[3] = { Magic, InoType, Version };
	uint16_t Crc = Crc16(Crc16(0xFFFF, Hdr, 3), (const uint8_t*)Data, Len);
	for (uint8_t i = 0; i < 3; i++) EEPROM.write(Addr + i, Hdr[i]);
	EEPROM.write(Addr + 3, Crc & 0xFF);
	EEPROM.write(Addr + 4, Crc >> 8);
	for (uint16_t i = 0; i < Len; i++) EEPROM.write(Addr + RecordHeaderSize + i, ((const uint8_t*)Data)[i]);
}

bool ReadRecord(uint16_t Addr, uint8_t Magic, uint8_t Version, void* Data, uint16_t Len)
{
	// true and Data filled only if magic, InoType, version and CRC all match.
	// On false Data is untouched, so it keeps the defaults already in it
	bool Result = false;
	uint8_t Hdr[3];
	for (uint8_t i = 0; i < 3; i++) Hdr[i] = EEPROM.read(Addr + i);
	if (Hdr[0] == Magic && Hdr[1] == InoType && Hdr[2] == Version && Len <= EE_IdentitySize)
	{
		uint8_t Buf[EE_IdentitySize];		// the largest slot
		for (uint16_t i = 0; i < Len; i++) Buf[i] = EEPROM.read(Addr + RecordHeaderSize + i);
		uint16_t Stored = EEPROM.read(Addr + 3) | (EEPROM.read(Addr + 4) << 8);
		if (Crc16(Crc16(0xFFFF, Hdr, 3), Buf, Len) == Stored)
		{
			memcpy(Data, Buf, Len);
			Result = true;
		}
	}
	return Result;
}

void LoadData()
{
	// Each record loads on its own: a missing or bad one keeps its defaults, the others are
	// kept. Identity goes last because it sets MDL.ID, which LoadDefaults() resets
	LoadDefaults();

	bool ConfigOK = ReadRecord(EE_Config, ConfigMagic, ConfigVersion, &MDL, sizeof(MDL));
	if (!ConfigOK) Serial.println("Module settings not stored, using defaults.");

	bool SensorsOK = true;
	for (int i = 0; i < MaxProductCount; i++)
	{
		if (!ReadRecord(EE_Sensor + i * EE_SensorStep, SensorMagic, SensorVersion, &Sensor[i], SensorStoredBytes))
		{
			SensorsOK = false;
			Serial.print("Sensor ");
			Serial.print(i + 1);
			Serial.println(" settings not stored, using defaults.");
		}
	}

	if (!ValidData())
	{
		Serial.println("Stored pin settings not valid, using defaults.");
		LoadDefaults();
		ConfigOK = false;
		GoodPins = true;
	}

	bool IdentityOK = LoadIdentity();

	if (!ConfigOK || !SensorsOK || !IdentityOK) SaveData();
}

void SaveData()
{
	Serial.println("Updating stored settings.");
	WriteRecord(EE_Config, ConfigMagic, ConfigVersion, &MDL, sizeof(MDL));
	for (int i = 0; i < MaxProductCount; i++)
	{
		WriteRecord(EE_Sensor + i * EE_SensorStep, SensorMagic, SensorVersion, &Sensor[i], SensorStoredBytes);
	}
	WriteIdentity();
	EEPROM.commit();
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	// RC15
	// default flow pins
	Sensor[0].FlowPin = 17;
	Sensor[0].IN1 = 32;
	Sensor[0].IN2 = 33;

	Sensor[1].FlowPin = 16;
	Sensor[1].IN1 = 25;
	Sensor[1].IN2 = 26;

	// sensors beyond the board's two driver channels have no default pins;
	// explicit NC - zero-initialized globals would otherwise leave pin 0 (a real pin)
	for (int i = 2; i < MaxProductCount; i++)
	{
		Sensor[i].FlowPin = NC;
		Sensor[i].IN1 = NC;
		Sensor[i].IN2 = NC;
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
		Sensor[i].Deadband = 0.015;
		Sensor[i].BrakePoint = 35;
		Sensor[i].PIDslowAdjust = 60;	// PIDslowAdjustDefault, matches app (was 30, pre-existing divergence)
		Sensor[i].SlewRate = 25;
		Sensor[i].MaxIntegral = 25;
		Sensor[i].TimedMinStart = 0.5;
		Sensor[i].TimedAdjust = 80;
		Sensor[i].TimedPause = 400;
		Sensor[i].PIDtime = 150;
		Sensor[i].PulseMin = 250;		// 4000 Hz
		Sensor[i].PulseMax = 1000000;	// 1 Hz
		Sensor[i].SampleWindow = 40;	// flow window: 40 centiseconds = 400 ms
	}

	// relay pins
	for (int i = 0; i < 16; i++)
	{
		MDL.RelayControlPins[i] = NC;
	}

	// module settings
	MDL.ID = NewModuleID;
	MDL.SensorCount = 1;
	MDL.InvertRelay = true;
	MDL.InvertFlow = true;
	MDL.OnboardRelayControl = 5;
	MDL.RemoteRelayControl = 0;
	MDL.WorkPin = NC;
	MDL.WorkPinIsMomentary = false;
	MDL.InvertWork = false;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = true;
	MDL.PressurePin = NC;
	MDL.WheelCal = 0;
	MDL.WheelSpeedPin = NC;
}

bool ValidData()
{
	bool Result = false;

	switch (Processor)
	{
	case 0:
		// work switch
		Result = (MDL.WorkPin == NC);
		if (!Result)
		{
			for (int j = 0; j < sizeof(ValidPins0); j++)
			{
				if (MDL.WorkPin == ValidPins0[j])
				{
					Result = true;
					break;
				}
			}
			if (!Result) break;
		}

		// pressure pin
		if (Result && MDL.PressurePin < NC)
		{
			for (int j = 0; j < sizeof(ValidPins0); j++)
			{
				if (MDL.PressurePin == ValidPins0[j])
				{
					Result = true;
					break;
				}
			}
			if (!Result) break;
		}

		// wheel speed pin
		if (Result && MDL.WheelSpeedPin < NC)
		{
			for (int j = 0; j < sizeof(ValidPins0); j++)
			{
				if (MDL.WheelSpeedPin == ValidPins0[j])
				{
					Result = true;
					break;
				}
			}
			if (!Result) break;
		}

		if (Result)
		{
			// NC is a valid setting (sensor input/output not used) - a 6-sensor module
			// may legitimately have sensors without pins assigned yet
			for (int i = 0; i < MDL.SensorCount; i++)
			{
				// flow pin
				Result = (Sensor[i].FlowPin == NC);
				for (int j = 0; !Result && j < sizeof(ValidPins0); j++)
				{
					Result = (Sensor[i].FlowPin == ValidPins0[j]);
				}
				if (!Result) break;

				// IN1
				Result = (Sensor[i].IN1 == NC);
				for (int j = 0; !Result && j < sizeof(ValidPins0); j++)
				{
					Result = (Sensor[i].IN1 == ValidPins0[j]);
				}
				if (!Result) break;

				// IN2
				Result = (Sensor[i].IN2 == NC);
				for (int j = 0; !Result && j < sizeof(ValidPins0); j++)
				{
					Result = (Sensor[i].IN2 == ValidPins0[j]);
				}
				if (!Result) break;

				// bin sensor pin
				Result = (Sensor[i].BinPin == NC);
				for (int j = 0; !Result && j < sizeof(ValidPins0); j++)
				{
					Result = (Sensor[i].BinPin == ValidPins0[j]);
				}
				if (!Result) break;
			}
		}

		if (Result && MDL.OnboardRelayControl == 1)
		{
			// check GPIOs for relays
			for (int k = 0; k < 16; k++)
			{
				Result = false;
				for (int j = 0; j < sizeof(ValidPins0); j++)
				{
					if ((MDL.RelayControlPins[k] == ValidPins0[j])
						|| (MDL.RelayControlPins[k] == NC))
					{
						Result = true;
						break;
					}
				}
				if (!Result) break;
			}
		}
		break;
	}
	GoodPins = Result;
	return Result;
}

bool LoadIdentity()
{
	ModuleIdentity Id;
	bool Result = ReadRecord(EE_Identity, IdentityMagic, IdentityVersion, &Id, sizeof(Id));
	if (Result)
	{
		MDL.ID = Id.ID;
		if (MDL.ID > MaxModuleID && MDL.ID != NewModuleID)
		{
			// an ID the app can't list or reassign starts as a new board
			Serial.println("Module ID out of range, starting as a new board.");
			MDL.ID = NewModuleID;
			Result = false;		// the caller saves again (Begin)
		}
		memcpy(MDLboard.Text, Id.Label, sizeof(MDLboard.Text));
		MDLnetwork = Id.Network;
		if (MDLnetwork.StaChannelCache > 13) MDLnetwork.StaChannelCache = 0;
	}
	else
	{
		Serial.println("Module identity not stored, using defaults.");
		MDL.ID = NewModuleID;
		memset(MDLboard.Text, 0, sizeof(MDLboard.Text));
		MDLnetwork = ModuleNetwork();		// the struct's own defaults
	}
	return Result;
}

void WriteIdentity()
{
	// no commit: SaveData() and SaveIdentity() commit
	ModuleIdentity Id;
	Id.ID = MDL.ID;
	memcpy(Id.Label, MDLboard.Text, sizeof(Id.Label));
	Id.Network = MDLnetwork;
	WriteRecord(EE_Identity, IdentityMagic, IdentityVersion, &Id, sizeof(Id));
}

void SaveIdentity()
{
	WriteIdentity();
	EEPROM.commit();
}

// callers kept: the network and the board label are both in the identity record
void SaveNetworks() { SaveIdentity(); }
void SaveBoardID() { SaveIdentity(); }

void ChipMAC(uint8_t* mac)
{
	// the unique factory base MAC from the ESP32's efuse
	uint64_t chip = ESP.getEfuseMac();
	for (byte i = 0; i < 6; i++)
	{
		mac[i] = (chip >> (8 * i)) & 0xFF;
	}
}



