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
	LoadData();
	LoadNetworks();

	Serial.println("");
	Serial.println(InoDescription);

	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	if (mn <= 12 && dy <= 31)
	{
		Serial.print(F("Firmware Version: v"));
		Serial.print(yr);
		Serial.print('.');
		if (mn < 10) Serial.print('0');
		Serial.print(mn);
		Serial.print('.');
		if (dy < 10) Serial.print('0');
		Serial.println(dy);
	}
	else
	{
		Serial.println(F("Firmware Version: invalid"));
	}

	Serial.print(F("Module ID: "));
	Serial.println(MDL.ID);
	Serial.println("");

	if (MDL.WorkPin < NC) pinMode(MDL.WorkPin, INPUT_PULLUP);
	if (MDL.SensorCount > MaxProductCount) MDL.SensorCount = MaxProductCount;

	// I2C
	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ethernet 
	Serial.println(F("Starting Ethernet ..."));
	MDLnetwork.IP3 = MDL.ID + 50;
	byte ArduinoIP[] = { MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, MDLnetwork.IP3 };
	static uint8_t LocalMac[] = { 0x0A,0x0B,0x42,0x0C,0x0D,MDLnetwork.IP3 };
	static byte gwip[] = { MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 1 };
	static byte myDNS[] = { 8, 8, 8, 8 };
	static byte mask[] = { 255, 255, 255, 0 };

	// update from saved data
	DestinationIP[0] = MDLnetwork.IP0;
	DestinationIP[1] = MDLnetwork.IP1;
	DestinationIP[2] = MDLnetwork.IP2;

	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println(F("Ethernet controller found."));
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

		//register sub for received data
		ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);

		delay(500);
		if (EthernetConnected())
		{
			ether.printIp(F("IP Address:     "), ether.myip);
		}
		else
		{
			Serial.println(F("Ethernet cable not connected."));
		}
	}
	else
	{
		Serial.println("");
		Serial.println(F("Ethernet controller not found."));
	}

	// sensors
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		//pinMode(Sensor[i].FlowPin, INPUT); 	// for direct connection to inductive sensor, no opto
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

#if defined(ARDUINO_TEENSY41)
		// pwm frequency change from default 4482 Hz to 490 Hz, required for some valves to work
		analogWriteFrequency(Sensor[i].PWMPin, PWM_FREQ);
#endif
	}

#if defined(ESP32) || defined(ARDUINO_TEENSY41)
	analogWriteResolution(PWM_BITS);
#endif

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
		Serial.println(F("Starting PCA9555 I/O Expander ..."));
		ErrorCount = 0;
		while (!PCA9555PW_found)
		{
			Serial.print(F("."));
			Wire.beginTransmission(0x20);
			PCA9555PW_found = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (PCA9555PW_found)
		{
			Serial.println(F("PCA9555 expander found."));

			PCA.attach(Wire);
			PCA.polarity(PCA95x5::Polarity::ORIGINAL_ALL);
			PCA.direction(PCA95x5::Direction::OUT_ALL);
			PCA.write(PCA95x5::Level::H_ALL);
		}
		else
		{
			Serial.println(F("PCA9555 expander not found."));
		}
		Serial.println("");
		break;

	case 4:
		// MCP23017 I/O expander on 0x20, 0x21

		Serial.println("");
		Serial.println(F("Starting MCP23017 ..."));

		ErrorCount = 0;
		MCP23017address = 0x21;
		while (!MCP23017_found)
		{
			// RC12-3
			Serial.print(F("."));
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
				Serial.print(F("."));
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

			Serial.println(F("MCP23017 found."));
		}
		else
		{
			Serial.println(F("MCP23017 not found."));
		}
		break;
	}

	Serial.println("");
	Serial.print(F("Sensors enabled: "));
	Serial.println(MDL.SensorCount);
	Serial.println("");
	Serial.println(F("Sensor 1: "));
	Serial.print(F("Flow Pin: "));
	Serial.println(Sensor[0].FlowPin);
	Serial.print(F("DIR Pin: "));
	Serial.println(Sensor[0].DirPin);
	Serial.print(F("PWM Pin: "));
	Serial.println(Sensor[0].PWMPin);

	Serial.println("");
	Serial.println(F("Sensor 2: "));
	Serial.print(F("Flow Pin: "));
	Serial.println(Sensor[1].FlowPin);
	Serial.print(F("DIR Pin: "));
	Serial.println(Sensor[1].DirPin);
	Serial.print(F("PWM Pin: "));
	Serial.println(Sensor[1].PWMPin);

	Serial.println("");

	Serial.print(F("Work Switch Pin: "));
	if (MDL.WorkPin == NC)
	{
		Serial.println(F("Disabled"));
	}
	else
	{
		Serial.println(MDL.WorkPin);
	}

	Serial.print(F("Pressure Pin: "));
	if (MDL.PressurePin == NC)
	{
		Serial.println(F("Disabled"));
	}
	else
	{
		Serial.println(MDL.PressurePin);
	}

	Serial.println(F("ADS1115: Disabled "));

	Serial.println("");
	Serial.println(F("Finished setup."));
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
		Serial.println(F("Loading stored settings."));
		EEPROM.get(23, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(253 + i * 124, Sensor[i]);
		}
		IsValid = ValidData();
	}

	if (!IsValid)
	{
		Serial.println(F("Stored settings not valid."));
		LoadDefaults();
		SaveData();
	}
}

void SaveData()
{
	Serial.println(F("Updating stored settings."));
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
	Serial.println(F("Loading default settings."));

	// default flow pins
	Sensor[0].FlowPin = 3;
	Sensor[0].DirPin = 4;
	Sensor[0].PWMPin = 5;

	Sensor[1].FlowPin = 2;
	Sensor[1].DirPin = 6;
	Sensor[1].PWMPin = 9;

	// default control settings
	for (int i = 0; i < 2; i++)
	{
		Sensor[i].MaxPWM = 255;
		Sensor[i].MinPWM = 5;
		Sensor[i].Kp = pow(1.1, 65 - 120);	// Kp = 65
		Sensor[i].Ki = pow(1.1, 65 - 120);	// Ki = 65
		Sensor[i].Deadband = 0.015;
		Sensor[i].BrakePoint = 35;
		Sensor[i].PIDslowAdjust = 30;
		Sensor[i].SlewRate = 25;
		Sensor[i].MaxIntegral = 25;
		Sensor[i].TimedMinStart = 0.5;
		Sensor[i].TimedAdjust = 80;
		Sensor[i].TimedPause = 400;
		Sensor[i].PIDtime = 100;
		Sensor[i].PulseMin = 250;      // 4000 Hz
		Sensor[i].PulseMax = 1000000;  // 1 Hz
		Sensor[i].PulseSampleSize = 12;
		Sensor[i].AutoOn = true;
	}

	// relay pins
	for (int i = 0; i < 16; i++)
	{
		MDL.RelayControlPins[i] = NC;
	}
	MDL.SensorCount = 1;
	MDL.RelayControl = 4;
	MDL.Is3Wire = true;
	MDL.ADS1115Enabled = false;
	MDL.InvertFlow = true;
	MDL.InvertRelay = true;
	MDL.WorkPin = 15;
	MDL.PressurePin = 14;
}

bool ValidData()
{
	bool Result = true;

	if (MDL.WorkPin > 21 && MDL.WorkPin != NC) Result = false;
	if (MDL.PressurePin > 21 && MDL.PressurePin != NC) Result = false;

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

		SaveNetworks();
	}
}

void SaveNetworks()
{
	EEPROM.put(168, MDLnetwork);
}
