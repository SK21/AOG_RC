
unsigned int DataPGN;
byte Data[100];
byte PGNlength;
byte MSB;
byte LSB;
bool EthernetConnected;
byte ErrorCount;

void ConnectWifi()
{
	Serial.println("");
	Serial.print("Connecting to ");
	Serial.println(WC.SSID);
	WiFi.mode(WIFI_AP_STA);
	WiFi.begin(WC.SSID, WC.Password);
	ErrorCount = 0;
	while (WiFi.status() != WL_CONNECTED)
	{
		delay(1000);
		Serial.print(".");
		if (ErrorCount++ > 60) break;
	}

	if (WiFi.status() == WL_CONNECTED)
	{
		Serial.println("");
		Serial.println("WiFi connected");
		Serial.println("IP address: ");
		Serial.println(WiFi.localIP());
		Serial.println("");

		WiFi.setAutoReconnect(true);
		WiFi.persistent(true);

		DestinationIP = WiFi.localIP();
		DestinationIP[3] = 255;		// change to broadcast
	}
	else
	{
		Serial.println("");
		Serial.println("WiFi not connected");
		Serial.println("");
	}
}

void ReceiveWifi()
{
	uint16_t PacketSize = UDPrate.parsePacket();
	if (PacketSize)
	{
		int Count = UDPrate.read(WifiBuffer, PacketSize);
		yield();
		Serial.write(WifiBuffer, Count);
	}
}

void ReceiveSerial()
{
	if (Serial.available())
	{
		if (Serial.available() > 100)
		{
			// clear buffer
			while (Serial.available())
			{
				yield();
				Serial.read();
			}
			DataPGN = 0;
			LSB = 0;
		}

		switch (DataPGN)
		{
		case 32501:
			// weights
			PGNlength = 8;
			if (Serial.available() > PGNlength - 3)
			{
				Data[0] = 245;
				Data[1] = 126;
				for (int i = 2; i < PGNlength; i++)
				{
					Data[i] = Serial.read();
				}

				// send data over wifi
				UDPrate.beginPacket(DestinationIP, DestinationPortRate);
				UDPrate.write(Data, PGNlength);
				UDPrate.endPacket();

				DataPGN = 0;
				LSB = 0;
			}
			break;

		case 32613:
			// rate data to RC
			PGNlength = 13;
			if (Serial.available() > PGNlength - 3)
			{
				Data[0] = 101;
				Data[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					yield();
					Data[i] = Serial.read();
				}

				// send data over wifi
				UDPrate.beginPacket(DestinationIP, DestinationPortRate);
				UDPrate.write(Data, PGNlength);
				UDPrate.endPacket();

				DataPGN = 0;
				LSB = 0;

				TeensyTime = millis();

				ModuleID = Data[2] >> 4;
				EthernetConnected = ((Data[11] & 0b00100000) == 0b00100000);
				ResetIno = ((Data[11] & 0b01000000) == 0b01000000);
			}
			break;

		case 32621:
			// pressure data to RC
			PGNlength = 12;
			if (Serial.available() > PGNlength - 3)
			{
				Data[0] = 109;
				Data[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					yield();
					Data[i] = Serial.read();
				}

				// send data over wifi
				UDPrate.beginPacket(DestinationIP, DestinationPortRate);
				UDPrate.write(Data, PGNlength);
				UDPrate.endPacket();

				DataPGN = 0;
				LSB = 0;
			}
			break;

		default:
			// find pgn
			MSB = Serial.read();
			DataPGN = MSB << 8 | LSB;
			LSB = MSB;
			break;
		}
	}
}

void SendSwitches()
{
	// PGN32619
	// 0    107
	// 1    127
	// 2    MasterOn
	// 3	switches 0-7
	// 4	switches 8-15
	// 5	crc

	Packet[0] = 107;
	Packet[1] = 127;
	Packet[2] = MasterOn;
	Packet[3] = 0;
	Packet[4] = 0;

	// convert section switches to bits
	for (int i = 0; i < 16; i++)
	{
		SendByte = i / 8;
		SendBit = i - SendByte * 8;
		if (Button[i]) bitSet(Packet[SendByte + 3], SendBit);
	}

	// crc
	Packet[5] = CRC(5, 0);

	// send
	for (int i = 0; i < 6; i++)
	{
		Serial.write(Packet[i]);
	}
	Serial.println("");
}

void SendStatus()
{
	// PGN32503
	// 0	247
	// 1	126
	// 2	Module ID
	// 3	dBm lo
	// 4	dBm Hi
	// 5	Status
	//		- bit 0 Wifi connected
	//		- bit 1 Teensy connected
	//		- bit 2 Teensy ethernet connected
	// 6	DebugVal1
	// 7	DebugVal2
	// 8	CRC

	byte Length = 9;
	Packet[0] = 247;
	Packet[1] = 126;
	Packet[2] = ModuleID;
	Packet[3] = WiFi.RSSI();
	Packet[4] = WiFi.RSSI() >> 8;

	// status
	Packet[5] = 0;
	if (WiFi.status() == WL_CONNECTED) Packet[5] |= 0b00000001;

	TeensyConnected = (millis() - TeensyTime < 4000);
	if (TeensyConnected) Packet[5] |= 0b00000010;

	if (EthernetConnected && TeensyConnected) Packet[5] |= 0b00000100;

	Packet[6] = DebugVal1;
	Packet[7] = DebugVal2;

	Packet[Length - 1] = CRC(Length - 1, 0);

	// send to serial
	for (int i = 0; i < Length; i++)
	{
		Serial.write(Packet[i]);
	}
	Serial.println("");

	// send to wifi
	UDPrate.beginPacket(DestinationIP, DestinationPortRate);
	UDPrate.write(Packet, Length);
	UDPrate.endPacket();
}
