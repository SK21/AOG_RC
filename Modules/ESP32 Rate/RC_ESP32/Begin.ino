// valid pins for each processor
uint8_t ValidPins0[] = { 0,2,4,13,14,15,16,17,21,22,25,26,27,32,33 };	// SPI pins 5,18,19,23 excluded for ethernet module

void DoSetup()
{
	uint8_t ErrorCount = 0;
	bool WheelMatch = false;

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");

	// eeprom
	EEPROM.begin(EEPROM_SIZE);
	LoadData();
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
	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

	// I2C
	Wire.begin();			// I2C on pins SCL 22, SDA 21
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

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
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDLnetwork.IP3 };

	Ethernet.init(W5500_SS);   // SS pin
	Ethernet.begin(LocalMac, 0);
	Ethernet.setLocalIP(LocalIP);
	IPAddress Mask(255, 255, 255, 0);
	Ethernet.setSubnetMask(Mask);
	IPAddress Gateway(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 1);
	Ethernet.setGatewayIP(Gateway);

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
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		pinMode(Sensor[i].IN1, OUTPUT);
		pinMode(Sensor[i].IN2, OUTPUT);

		switch (i)
		{
		case 0:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR0, RISING);
			break;
		case 1:
			attachInterrupt(digitalPinToInterrupt(Sensor[i].FlowPin), ISR1, RISING);
			break;
		}

		// pwm frequency change from default 5000 Hz to 490 Hz, required for some valves to work
		ledcAttach(Sensor[i].IN1, PWM_FREQ, PWM_BITS);
		ledcWrite(Sensor[i].IN1, 0);

		ledcAttach(Sensor[i].IN2, PWM_FREQ, PWM_BITS);
		ledcWrite(Sensor[i].IN2, 0);

		if (Sensor[i].FlowPin == MDL.WheelSpeedPin) WheelMatch = true;
	}

	// wheel speed sensor
	if (MDL.WheelSpeedPin != NC && !WheelMatch)
	{
		pinMode(MDL.WheelSpeedPin, INPUT_PULLUP);
		attachInterrupt(digitalPinToInterrupt(MDL.WheelSpeedPin), ISR_Speed, FALLING);
	}

	// Relays
	switch (MDL.RelayControl)
	{
	case 1:
		// Relay GPIO Pins
		Serial.println("");
		Serial.println("Using GPIO pins for relays.");
		for (int i = 0; i < 16; i++)
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
		Serial.println("Starting PCA9555 I/O Expander ...");
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
			Serial.println("PCA9555 found.");

			PCA.attach(Wire);
			PCA.polarity(PCA95x5::Polarity::ORIGINAL_ALL);
			PCA.direction(PCA95x5::Direction::OUT_ALL);
			PCA.write(PCA95x5::Level::H_ALL);
		}
		else
		{
			Serial.println("PCA9555 not found.");
		}
		break;

	case 4:
		// MCP23017 I/O expander on 0x20, 0x21

		Serial.println("");
		Serial.println("Starting MCP23017 ...");

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
		Serial.println("Starting PCA9685 I/O Expander ...");
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
		Serial.println("Starting PCF8574 I/O Expander ...");
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

	WiFi.softAPConfig(AP_LocalIP, AP_GateWay, AP_Subnet);
	if (strlen(MDL.APpassword) >= 8) 
	{
		// WPA2-PSK
		WiFi.softAP(AP.c_str(), MDL.APpassword, 6, false, 4);
	}
	else
	{
		// Fallback: invalid WPA passphrase length -> force open
		WiFi.softAP(AP.c_str(), nullptr, 6, false, 4);
	}

	dnsServer.start(DNS_PORT, "*", AP_LocalIP);

	UDP_Wifi.begin(ListeningPort);

	Serial.println("");
	Serial.print("Access Point name: ");
	Serial.println(AP);
	Serial.print("Settings Page IP: ");
	Serial.println(AP_LocalIP);

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

	// OTA
	server.on("/myurl", HTTP_GET, []() {
		server.sendHeader("Connection", "close");
		server.send(200, "text/plain", "Hello there!");
	});

	server.begin();

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);

	Serial.println("OTA started.");

	// wifi client mode
	if (MDLnetwork.WifiModeUseStation)
	{
		// connect to network
		delay(1000);
		WiFi.onEvent(WiFiStationConnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_CONNECTED);
		WiFi.onEvent(WiFiGotIP, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_GOT_IP);
		WiFi.onEvent(WiFiStationDisconnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_DISCONNECTED);
		WiFi.begin(MDLnetwork.SSID, MDLnetwork.Password);
		Serial.println();
		Serial.println("Connecting to wifi network ...");
	}

	delay(1500);

	Serial.println("");
	Serial.print("Sensors enabled: ");
	Serial.println(MDL.SensorCount);
	Serial.println("");
	Serial.println("Sensor 1: ");
	Serial.print("Flow Pin: ");
	Serial.println(Sensor[0].FlowPin);
	Serial.print("IN1 Pin: ");
	Serial.println(Sensor[0].IN1);
	Serial.print("IN2 Pin: ");
	Serial.println(Sensor[0].IN2);

	Serial.println("");
	Serial.println("Sensor 2: ");
	Serial.print("Flow Pin: ");
	Serial.println(Sensor[1].FlowPin);
	Serial.print("IN1 Pin: ");
	Serial.println(Sensor[1].IN1);
	Serial.print("IN2 Pin: ");
	Serial.println(Sensor[1].IN2);

	Serial.println("");
	Serial.print("Work Switch Pin: ");
	Serial.println(MDL.WorkPin);
	Serial.print("Pressure Pin: ");
	Serial.println(MDL.PressurePin);

	Serial.println("");
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

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

// eeprom map:
// ID			0-1
// module type	2
// module data	23-147
// network		168-232
// sensor 1		253-356
// sensor 2		377-480

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

	// default control settings
	for (int i = 0; i < 2; i++)
	{
		Sensor[i].MaxPWM = 255;
		Sensor[i].MinPWM = 10;
		Sensor[i].Kp = 0.0003;	// gain 35
		Sensor[i].Ki = 0.00123;	// integral 5
		Sensor[i].Deadband = 0.015;
		Sensor[i].BrakePoint = 0.35;
		Sensor[i].PIDslowAdjust = 0.3;
		Sensor[i].SlewRate = 15;
		Sensor[i].MaxIntegral = 0.1;
		Sensor[i].TimedMinStart = 0.03;
		Sensor[i].TimedAdjust = 80;
		Sensor[i].TimedPause = 400;
		Sensor[i].PIDtime = 100;
		Sensor[i].PulseMin = 250;		// 4000 Hz
		Sensor[i].PulseMax = 1000000;	// 1 Hz
		Sensor[i].PulseSampleSize = 12;
		Sensor[i].AutoOn = true;
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
	MDL.RelayControl = 5;
	MDL.WorkPin = NC;
	MDL.WorkPinIsMomentary = false;
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
			for (int i = 0; i < MDL.SensorCount; i++)
			{

				// flow pin
				Result = false;
				for (int j = 0; j < sizeof(ValidPins0); j++)
				{
					if (Sensor[i].FlowPin == ValidPins0[j])
					{
						Result = true;
						break;
					}
				}
				if (!Result) break;

				// IN1
				Result = false;
				for (int j = 0; j < sizeof(ValidPins0); j++)
				{
					if (Sensor[i].IN1 == ValidPins0[j])
					{
						Result = true;
						break;
					}
				}
				if (!Result) break;

				// IN2
				Result = false;
				for (int j = 0; j < sizeof(ValidPins0); j++)
				{
					if (Sensor[i].IN2 == ValidPins0[j])
					{
						Result = true;
						break;
					}
				}
				if (!Result) break;
			}
		}

		if (Result && MDL.RelayControl == 1)
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
	EEPROM.commit();
}


