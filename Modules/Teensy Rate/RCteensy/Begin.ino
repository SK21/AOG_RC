
void DoSetup()
{
	uint8_t ErrorCount;
	int ADS[] = { 0x48,0x49,0x4A,0x4B };	// ADS1115 addresses

	//watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	// default flow pins
	Sensor[0].FlowPin = 28;
	Sensor[0].DirPin = 37;
	Sensor[0].PWMPin = 36;

	Sensor[1].FlowPin = 29;
	Sensor[1].DirPin = 14;
	Sensor[1].PWMPin = 15;

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
	delay(5000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	int16_t StoredID;
	EEPROM.get(100, StoredID);
	if (StoredID == InoID)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(200 + i * 80, Sensor[i]);
		}
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(100, InoID);
		EEPROM.put(110, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(200 + i * 80, Sensor[i]);
		}
	}

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
	for (int i = 0; i < 4; i++)
	{
		ADS1115_Address = ADS[i];
		Serial.print("Starting ADS1115 at address ");
		Serial.print(ADS1115_Address);
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
			if (ErrorCount++ > 10) break;
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
	default:
		SerialESP = &Serial8;
		break;
	}
	SerialESP->begin(38400);

	// Relays
	switch (MDL.RelayControl)
	{
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

	pinMode(LED_BUILTIN, OUTPUT);

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}