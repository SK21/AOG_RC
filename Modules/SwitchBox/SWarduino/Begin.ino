 
uint8_t ValidPins0[] = { 3,4,5,6,9,14,15,16,17,18,19,20,21 };	// using ethernet shield, 20 and 21 are analog only
//uint8_t ValidPins0[] = { 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21 };	// no ethernet shield


void DoSetup()
{
	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	LoadData();

	// pins
	if (MDL.MasterOn < NC) pinMode(MDL.MasterOn, INPUT_PULLUP);
	if (MDL.MasterOff < NC) pinMode(MDL.MasterOff, INPUT_PULLUP);
	if (MDL.RateUp < NC) pinMode(MDL.RateUp, INPUT_PULLUP);
	if (MDL.RateDown < NC) pinMode(MDL.RateDown, INPUT_PULLUP);
	if (MDL.AutoSection < NC) pinMode(MDL.AutoSection, INPUT_PULLUP);
	if (MDL.AutoRate < NC) pinMode(MDL.AutoRate, INPUT_PULLUP);
	if (MDL.WorkPin < NC) pinMode(MDL.WorkPin, INPUT_PULLUP);

	for (int i = 0; i < 16; i++)
	{
		if (MDL.SectionPins[i] < NC) pinMode(MDL.SectionPins[i], INPUT_PULLUP);
	}

	// ethernet
	Serial.println("");
	Serial.println("Starting Ethernet ...");
	MDL.IP3 = MDL.ID + 188;
	byte ArduinoIP[] = { MDL.IP0, MDL.IP1,MDL.IP2, MDL.IP3 };
	static uint8_t LocalMac[] = { 0x70,0x62,0x21,0x2D,0x31,MDL.IP3 };
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
			Serial.println("");
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

	Serial.println("");
	Serial.println("Finished Setup.");
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
		EEPROM.get(110, MDL);
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
	// update stored data
	Serial.println("Updating stored settings.");
	EEPROM.put(0, InoID);
	EEPROM.put(4, InoType);
	EEPROM.put(110, MDL);
}

void LoadDefaults()
{
	Serial.println("Loading default settings.");

	// SW2 pcb
	MDL.MasterOn = 3;
	MDL.MasterOff = 5;
	MDL.RateUp = A3;
	MDL.RateDown = A2;
	MDL.AutoSection = A5;
	MDL.AutoRate = A5;
	MDL.WorkPin = NC;
	MDL.IP0 = 192;
	MDL.IP1 = 168;
	MDL.IP2 = 1;
	MDL.IP3 = 50;

	// section switch pins
	MDL.SectionPins[0] = A4;
	MDL.SectionPins[1] = 9;
	MDL.SectionPins[2] = 6;
	MDL.SectionPins[3] = 4;

	for (int i = 4; i < 16; i++)
	{
		MDL.SectionPins[i] = NC;
	}
}

bool ValidData()
{
	bool Found = false;
	for (int i = 0; i < sizeof(ValidPins0); i++)
	{
		Found = ((MDL.MasterOn == ValidPins0[i]) || (MDL.MasterOn == NC));
		if (Found) break;
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.MasterOff == ValidPins0[i]) || (MDL.MasterOff == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.RateUp == ValidPins0[i]) || (MDL.RateUp == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.RateDown == ValidPins0[i]) || (MDL.RateDown == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.AutoSection == ValidPins0[i]) || (MDL.AutoSection == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.AutoRate == ValidPins0[i]) || (MDL.AutoRate == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < sizeof(ValidPins0); i++)
		{
			Found = ((MDL.WorkPin == ValidPins0[i]) || (MDL.WorkPin == NC));
			if (Found) break;
		}
	}

	if (Found)
	{
		for (int i = 0; i < 16; i++)
		{
			for (int j = 0; j < sizeof(ValidPins0); j++)
			{
				Found = ((MDL.SectionPins[i] == ValidPins0[j]) || (MDL.SectionPins[i] == NC));
				if (Found) break;
			}
			if (!Found) break;
		}
	}
	return Found;
}
