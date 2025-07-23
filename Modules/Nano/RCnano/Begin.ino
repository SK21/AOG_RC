
void DoSetup()
{
	uint8_t ErrorCount;

	Sensor[0].FlowEnabled = false;
	Sensor[1].FlowEnabled = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");

	// eeprom
	LoadData();

	Serial.println("");
	Serial.println(InoDescription);

	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	Serial.print("Module Version: v");
	Serial.print(yr);
	Serial.print(".");
	Serial.print(mn);
	Serial.print(".");
	Serial.println(dy);

	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.println("");

	if (MDL.WorkPin < NC) pinMode(MDL.WorkPin, INPUT_PULLUP);
	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

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

		delay(500);
		if (EthernetConnected())
		{
			ether.printIp("IP Address:     ", ether.myip);
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
	case 1:
		// Relay GPIO Pins
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
	}

	Serial.println("");
	Serial.print("Flow Pin: ");
	Serial.println(Sensor[0].FlowPin);
	Serial.print("DIR Pin: ");
	Serial.println(Sensor[0].DirPin);
	Serial.print("PWM Pin: ");
	Serial.print(Sensor[0].PWMPin);
	Serial.println("");

	Serial.print("Work Pin: ");
	Serial.println(MDL.WorkPin);
	Serial.print("Pressure Pin: ");
	Serial.print(MDL.PressurePin);
	Serial.println("");

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
		EEPROM.get(10, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(100 + i * 80, Sensor[i]);
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
	EEPROM.put(10, MDL);

	for (int i = 0; i < MaxProductCount; i++)
	{
		EEPROM.put(100 + i * 80, Sensor[i]);
	}
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	MDL.WorkPin = 14;
	MDL.PressurePin = 15;

	// default flow pins
	Sensor[0].FlowPin = 3;
	Sensor[0].DirPin = 4;
	Sensor[0].PWMPin = 5;

	Sensor[1].FlowPin = 2;
	Sensor[1].DirPin = 6;
	Sensor[1].PWMPin = 9;

	// default control settings
	Sensor[0].MaxPower = 255;
	Sensor[0].MinPower = 10;
	Sensor[1].MaxPower = 255;
	Sensor[1].MinPower = 10;

	// relay pins
	for (int i = 0; i < 16; i++)
	{
		MDL.RelayControlPins[i] = NC;
	}
	MDL.SensorCount = 2;
	MDL.RelayControl = 4;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.InvertFlow = true;
	MDL.InvertRelay = true;
}

bool ValidData()
{
	bool Result = true;

	if (MDL.WorkPin > 21 && MDL.WorkPin != NC) Result = false;

	if (Result)
	{
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			if ((Sensor[i].FlowPin > 21) || (Sensor[i].DirPin > 21) || (Sensor[i].PWMPin > 21))
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
			if (MDL.RelayControlPins[i] > 21 && MDL.RelayControlPins[i] != NC)
			{
				Result = false;
				break;
			}
		}
	}
	GoodPins = Result;
	return Result;
}