
void DoSetup()
{
	uint8_t ErrorCount;
	int ADS[] = { 0x48,0x49,0x4A,0x4B };	// ADS1115 addresses

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	// default flow pins
	Sensor[0].FlowPin = 12;
	Sensor[0].DirPin = 25;
	Sensor[0].PWMPin = 26;

	// default pid
	Sensor[0].KP = 5;
	Sensor[0].KI = 0;
	Sensor[0].KD = 0;
	Sensor[0].MinPWM = 5;
	Sensor[0].MaxPWM = 50;
	Sensor[0].Debounce = 3;

	Sensor[1].KP = 5;
	Sensor[1].KI = 0;
	Sensor[1].KD = 0;
	Sensor[1].MinPWM = 5;
	Sensor[1].MaxPWM = 50;
	Sensor[1].Debounce = 3;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	EEPROM.begin(EEPROM_SIZE);

	int16_t StoredID;
	EEPROM.get(100, StoredID);
	if (StoredID == InoID)
	{
		LoadData();
	}
	else
	{
		SaveData();
	}

	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

	Serial.println("");
	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.print("Module Version: ");
	Serial.println(InoID);
	Serial.println("");

	// I2C
	Wire.begin();			// I2C on pins SCL 22, SDA 21
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ADS1115
	if (MDL.AdsAddress == 0)
	{
		for (int i = 0; i < 4; i++)
		{
			ADS1115_Address = ADS[i];
			Serial.print("Starting ADS1115 at address ");
			Serial.println(ADS1115_Address);
			ErrorCount = 0;
			while (!ADSfound)
			{
				Wire.beginTransmission(ADS1115_Address);
				Wire.write(0b00000000);	//Point to Conversion register
				Wire.endTransmission();
				Wire.requestFrom(ADS1115_Address, 2);
				ADSfound = Wire.available();
				Serial.print(".");
				delay(500);
				if (ErrorCount++ > 5) break;
			}
			Serial.println("");
			if (ADSfound)
			{
				Serial.print("ADS1115 connected at address ");
				Serial.println(ADS1115_Address);
				Serial.println("");
				break;
			}
			else
			{
				Serial.print("ADS1115 not found.");
				Serial.println("");
			}
		}
	}
	else
	{
		ADS1115_Address = MDL.AdsAddress;
		Serial.print("Starting ADS1115 at address ");
		Serial.println(ADS1115_Address);
		ErrorCount = 0;
		while (!ADSfound)
		{
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0b00000000);	//Point to Conversion register
			Wire.endTransmission();
			Wire.requestFrom(ADS1115_Address, 2);
			ADSfound = Wire.available();
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 5) break;
		}
		Serial.println("");
		if (ADSfound)
		{
			Serial.print("ADS1115 connected at address ");
			Serial.println(ADS1115_Address);
			Serial.println("");
		}
		else
		{
			Serial.print("ADS1115 not found.");
			Serial.println("");
		}
	}

	if (!ADSfound)
	{
		Serial.println("ADS1115 disabled.");
		Serial.println("");
	}

	// ethernet 
	Serial.println("Starting Ethernet ...");
	MDL.IP3 = MDL.ID + 50;
	IPAddress LocalIP(MDL.IP0, MDL.IP1, MDL.IP2, MDL.IP3);
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDL.IP3 };

	Ethernet.init(5);   // SS pin
	Ethernet.begin(LocalMac, 0);
	Ethernet.setLocalIP(LocalIP);
	IPAddress Mask(255, 255, 255, 0);
	Ethernet.setSubnetMask(Mask);
	IPAddress Gateway(MDL.IP0, MDL.IP1, MDL.IP2, 1);
	Ethernet.setGatewayIP(Gateway);

	delay(1500);
	ChipFound = (Ethernet.hardwareStatus() != EthernetNoHardware);
	if (ChipFound)
	{
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
	}
	else
	{
		Serial.println("No ethernet hardware found.");
	}

	DestinationIP = IPAddress(MDL.IP0, MDL.IP1, MDL.IP2, 255);	// update from saved data
	Serial.println("");

	// UDP
	UDPcomm.begin(ListeningPort);

	// AGIO
	AGIOcomm.begin(ListeningPortAGIO);

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

		// pwm
		ledcSetup(i, 500, 8);
		ledcAttachPin(Sensor[i].PWMPin, i);
	}

	// Relays
	switch (MDL.RelayControl)
	{
	case 1:
		// PCA9685
		Serial.println("");
		Serial.println("Starting PCA9685 I/O Expander ...");
		ErrorCount = 0;
		while (!PCA9685_found)
		{
			Serial.print(".");
			Wire.beginTransmission(DriverAddress);
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
		Serial.println("");
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
			Wire.beginTransmission(0x20);
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

	case 5:
		// Relay GPIO Pins
		for (int i = 0; i < 16; i++)
		{
			if (MDL.RelayPins[i] > 0)
			{
				pinMode(MDL.RelayPins[i], OUTPUT);
			}
		}
		break;
	}

	// Access Point
	AP_LocalIP = IPAddress(192, 168, MDL.ID + 100, 1);
	AP_DestinationIP = IPAddress(192, 168, MDL.ID + 100, 255);
	
	String AP = MDL.Name;
	AP += "  (";
	AP += WiFi.macAddress();
	AP += ")";

	WiFi.softAP(AP,MDL.Password);
	WiFi.softAPConfig(AP_LocalIP, AP_LocalIP, AP_Subnet);
	WifiComm.begin(ListeningPort);

	Serial.println("");
	Serial.print("Access Point name: ");
	Serial.println(AP);
	Serial.print("Access Point IP: ");
	Serial.println(AP_LocalIP);

	// web server
	Serial.println();
	Serial.println("Starting Web Server");
	server.on("/", HandlePage1);
	server.on("/ButtonPressed", ButtonPressed);
	server.onNotFound(HandlePage1);

	// OTA
	server.on("/myurl", HTTP_GET, []() {
		server.sendHeader("Connection", "close");
		server.send(200, "text/plain", "Hello there!");
	});

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);
	
	server.begin();

	Serial.println("OTA started.");

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void LoadData()
{
	// load stored data
	Serial.println("Loading stored settings.");
	EEPROM.get(110, MDL);

	for (int i = 0; i < MaxProductCount; i++)
	{
		EEPROM.get(300 + i * 80, Sensor[i]);
	}
}

void SaveData()
{
	// update stored data
	Serial.println("Updating stored data.");
	EEPROM.put(100, InoID);
	EEPROM.put(110, MDL);

	for (int i = 0; i < MaxProductCount; i++)
	{
		EEPROM.put(300 + i * 80, Sensor[i]);
	}
	EEPROM.commit();
}

