
void DoSetup()
{
	uint8_t ErrorCount = 0;

	//watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	LoadData();

	if (MDL.WorkPin < NC) pinMode(MDL.WorkPin, INPUT_PULLUP);

	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

	Serial.println("");
	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.print("Module Version: ");
	Serial.println(InoID);
	Serial.println("");

	// I2C
	Wire.begin();			// I2C on pins SCL 19, SDA 18
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
			Wire.requestFrom(ADS1115_Address, 2);
			ADSfound = Wire.available();
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
	MDL.IP3 = MDL.ID + 50;
	IPAddress LocalIP(MDL.IP0, MDL.IP1, MDL.IP2, MDL.IP3);
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDL.IP3 };

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
	DestinationIP = IPAddress(MDL.IP0, MDL.IP1, MDL.IP2, 255);	// update from saved data
	Serial.println("");

	// UDP
	UDPcomm.begin(ListeningPort);

	// update
	UpdateComm.begin(UpdateReceivePort);

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
	}

	//ESP8266 serial port
	switch (MDL.ESPserialPort)
	{
		case 1:
			SerialESP = &Serial1;
			break;
		case 2:
			SerialESP = &Serial2;
			break;
		case 3:
			SerialESP = &Serial3;
			break;
		case 4:
			SerialESP = &Serial4;
			break;
		case 5:
			SerialESP = &Serial5;
			break;
		case 6:
			SerialESP = &Serial6;
			break;
		case 7:
			SerialESP = &Serial7;
			break;
		case 8:
			SerialESP = &Serial8;
		default:
			MDL.ESPserialPort = NC;
			break;
	}
	if(MDL.ESPserialPort!=NC) SerialESP->begin(38400);

	// Relays
	switch (MDL.RelayControl)
	{
	case 1:
		// Relay GPIO Pins
		for (int i = 0; i < 16; i++)
		{
			if (MDL.RelayPins[i] < NC)
			{
				pinMode(MDL.RelayPins[i], OUTPUT);
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
		// MCP23017 I/O expander on default address 0x20
		Serial.println("");
		Serial.println("Starting MCP23017 ...");
		ErrorCount = 0;
		while (!MCP23017_found)
		{
			Serial.print(".");
			Wire.beginTransmission(MCP23017address);
			MCP23017_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (MCP23017_found)
		{
			Serial.println("MCP23017 found.");
			MCP.begin_I2C();

			for (int i = 0; i < 16; i++)
			{
				MCP.pinMode(MDL.RelayPins[i], OUTPUT);
			}
		}
		else
		{
			Serial.println("MCP23017 not found.");
		}
		break;
	}

	pinMode(LED_BUILTIN, OUTPUT);

	Serial.println("");
	Serial.println("Sensor 1: ");
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
	Serial.println(MDL.WorkPin);
	Serial.print("Pressure Pin: ");
	Serial.println(MDL.PressurePin);

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void LoadData()
{
	bool IsValid = false;
	int16_t StoredID;
	int8_t StoredType;
	EEPROM.get(0, StoredID);
	EEPROM.get(4, StoredType);

	if (StoredID == InoID && StoredType == InoType)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(92, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(200 + i * 80, Sensor[i]);
		}
		IsValid = ValidData();
		if (!IsValid)
		{
			Serial.println("Stored settings not valid.");
		}
	}

	if (!IsValid)
	{
		LoadDefaults();
		SaveData();
	}
}

void SaveData()
{
	Serial.println("Updating stored settings.");
	EEPROM.put(0, InoID);
	EEPROM.put(4, InoType);
	EEPROM.put(92, MDL);

	for (int i = 0; i < MaxProductCount; i++)
	{
		EEPROM.put(200 + i * 80, Sensor[i]);
	}
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	MDL.WorkPin = NC;

	// RC11-2
	Sensor[0].FlowPin = 28;
	Sensor[0].DirPin = 37;
	Sensor[0].PWMPin = 36;

	Sensor[1].FlowPin = 29;
	Sensor[1].DirPin = 14;
	Sensor[1].PWMPin = 15;

	// default control settings
	Sensor[0].HighAdjust = 50;
	Sensor[0].LowAdjust = 20;
	Sensor[0].AdjustThreshold = 0.25;
	Sensor[0].MaxPower = 255;
	Sensor[0].MinPower = 10;
	Sensor[1].HighAdjust = 50;
	Sensor[1].LowAdjust = 20;
	Sensor[1].AdjustThreshold = 0.25;
	Sensor[1].MaxPower = 255;
	Sensor[1].MinPower = 10;

	// relay pins
	for (int i = 0; i < 16; i++)
	{
		MDL.RelayPins[i] = DefaultRelayPins[i];
	}

	// module settings
	MDL.ID = 0;
	MDL.SensorCount = 2;
	MDL.InvertRelay = true;
	MDL.InvertFlow = true;
	MDL.IP0 = 192;
	MDL.IP1 = 168;
	MDL.IP2 = 1;
	MDL.IP3 = 50;
	MDL.RelayControl = 1;
	MDL.ESPserialPort = NC;
	MDL.WifiModeUseStation = false;
	MDL.WorkPin = 30;
	MDL.WorkPinIsMomentary = false;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.PressurePin = 40;

	// network name
	memset(MDL.SSID, '\0', sizeof(MDL.SSID)); // erase old name
	memcpy(MDL.SSID, &DefaultNetName, 14);

	// network password
	memset(MDL.Password, '\0', sizeof(MDL.Password)); // erase old name
	memcpy(MDL.Password, &DefaultNetPassword, 14);
}

bool ValidData()
{
	bool Result = true;

	if (MDL.WorkPin > 41 && MDL.WorkPin != NC) Result = false;

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

	if (Result && MDL.RelayControl == 1)
	{
		// check GPIOs for relays
		for (int i = 0; i < 16; i++)
		{
			if (MDL.RelayPins[i] > 41 && MDL.RelayPins[i] != NC)
			{
				Result = false;
				break;
			}
		}
	}

	GoodPins = Result;
	return Result;
}

