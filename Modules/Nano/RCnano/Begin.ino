
void DoSetup()
{
	uint8_t ErrorCount;

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	// default flow pins
	Sensor[0].FlowPin = 3;
	Sensor[0].DirPin = 6;
	Sensor[0].PWMPin = 9;

	Sensor[1].FlowPin = 2;
	Sensor[1].DirPin = 4;
	Sensor[1].PWMPin = 5;

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
	int8_t StoredType;
	EEPROM.get(0, StoredID);
	EEPROM.get(4, StoredType);
	if (StoredID == InoID && StoredType==InoType)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(10, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(100 + i * 80, Sensor[i]);
		}
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(0, InoID);
		EEPROM.put(4, InoType);
		EEPROM.put(10, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(100 + i * 80, Sensor[i]);
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

	// ethernet 
	Serial.println("Starting Ethernet ...");
	MDL.IP3 = MDL.ID + 50;
	byte ArduinoIP[] = { MDL.IP0, MDL.IP1,MDL.IP2, MDL.IP3 };
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDL.IP3 };
	static byte gwip[] = { MDL.IP0, MDL.IP1,MDL.IP2, 1 };
	static byte myDNS[] = { 8, 8, 8, 8 };
	static byte mask[] = { 255, 255, 255, 0 };

	// update from saved data
	DestinationIP[0] = MDL.IP0;
	DestinationIP[1] = MDL.IP1;
	DestinationIP[2] = MDL.IP2;

	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println("Ethernet controller found.");
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

		//register sub for received data
		ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
		ether.udpServerListenOnPort(&ReceiveAGIO, ListeningPortAGIO);

		delay(500);
		if (EthernetConnected())
		{
			Serial.println("");
			ether.printIp("IP Address:     ", ether.myip);
			Serial.println("Serial data disabled.");
		}
		else
		{
			Serial.println("Ethernet cable not connected.");
		}
	}
	else
	{
		Serial.println("");
		Serial.println("Ethernet controller not found.");
		Serial.println("");
		Serial.println("Serial data enabled.");
	}

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		//pinMode(Sensor[i].FlowPin, INPUT);	// for direct connection to inductive sensor, no opto
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

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}