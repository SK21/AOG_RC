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

	uint64_t mac = ESP.getEfuseMac();
	uint32_t low32 = (uint32_t)(mac & 0xFFFFFFFF);

	char suffix[9]; // 8 hex + null
	sprintf(suffix, "%08X", low32);

	String AP = MDL.APname;
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
	if (strlen(MDL.APpassword) >= 8)
	{
		// WPA2-PSK
		WiFi.softAP(AP.c_str(), MDL.APpassword, APchannel, false, 4);
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

// eeprom map:
// ID			0-1
// module type	2
// board label	3-22
// module data	23-147
// network		168-232
// sensors 1-6	253 + i*124, ~106 bytes each (6th ends at ~979; EEPROM_SIZE is 1024)

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
	MDL.ID = 0;
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

void LoadNetworks()
{
	ModuleNetwork tmp;
	EEPROM.get(168, tmp);
	if (tmp.Identifier == 9876)
	{
		MDLnetwork = tmp;

		// StaChannelCache was added to the end of this struct, so a record
		// written before it existed leaves whatever EEPROM already held in that
		// byte. Anything outside the 2.4 GHz channels means "nothing cached" —
		// the first join then learns it for real.
		if (MDLnetwork.StaChannelCache > 13) MDLnetwork.StaChannelCache = 0;
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
		MDLnetwork.StaChannelCache = 0;
		strcpy(MDLnetwork.SSID, "Tractor");
		strcpy(MDLnetwork.Password, "111222333");

		SaveNetworks();
	}
}

void SaveNetworks()
{
	EEPROM.put(168, MDLnetwork);
	EEPROM.commit();
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
	EEPROM.commit();
}

void ChipMAC(uint8_t* mac)
{
	// the unique factory base MAC from the ESP32's efuse
	uint64_t chip = ESP.getEfuseMac();
	for (byte i = 0; i < 6; i++)
	{
		mac[i] = (chip >> (8 * i)) & 0xFF;
	}
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
		ChipMAC(mac);
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



