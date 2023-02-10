
unsigned int DataPGN;
byte Data[100];
byte PGNlength;
byte MSB;
byte LSB;

void StartUDP()
{
	UDPrate.stop();
	delay(500);
	UDPrate.begin(ListeningPortRate);

	// clear buffer
	uint16_t PacketSize = UDPrate.parsePacket();
	if(PacketSize) UDPrate.read(WifiBuffer, PacketSize);

	RestartCount++;
	DebugVal1 = RestartCount;

	// if Teensy is sending but not receiving, restart esp8266
	if (RestartCount > 20 && TeensyConnected) ESP.restart();
}

void CheckConnection()
{
	if (UDPreceive)
	{
		ReStartingUDP = false;
	}
	else
	{
		// not receiving
		if (ReStartingUDP)
		{
			if (millis() - WifiTime > 4000)
			{
				StartUDP();
				ReStartingUDP = false;
			}
		}
		else
		{
			ReStartingUDP = true;
			WifiTime = millis();
		}
	}

	if (millis() - LoopTime > 30000)
	{
		LoopTime = millis();
		if (!RCteensyConnected && (WiFi.status() != WL_CONNECTED)) CheckWifi();
	}

	TeensyConnected = (millis() - TeensyTime < 4000);
	if (!TeensyConnected) RCteensyConnected = false;
}

bool CheckWifi()
{
	if (WiFi.status() != WL_CONNECTED)
	{
		WiFi.disconnect();
		delay(500);
		WiFi.mode(WIFI_AP_STA);

		WiFi.begin(WC.SSID, WC.Password);
		Serial.println();
		Serial.println("Connecting to Wifi");
		unsigned long WifiConnectStart = millis();

		while ((WiFi.status() != WL_CONNECTED) && ((millis() - WifiConnectStart) < 15000))
		{
			delay(500);
			Serial.print(".");
		}
		if (WiFi.status() == WL_CONNECTED)
		{
			UDPrate.begin(ListeningPortRate);
			DestinationIP = WiFi.localIP();
			DestinationIP[3] = 255;		// change to broadcast
			Serial.println();
			Serial.println("Connected.");
			Serial.print("IP: ");
			Serial.println(WiFi.localIP());
		}
		else
		{
			Serial.println();
			Serial.println("Not connected");
		}
		LoopTime = millis();	// reset to give time to check if teensy connected
	}
	return (WiFi.status() == WL_CONNECTED);
}

void ReceiveWifi()
{
	uint16_t PacketSize = UDPrate.parsePacket();
	UDPreceive = (PacketSize);

	if (PacketSize)
	{
		int Count = UDPrate.read(WifiBuffer, PacketSize);

		DebugVal2 = Count;

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
					Data[i] = Serial.read();
				}

				// send data over wifi
				UDPrate.beginPacket(DestinationIP, DestinationPortRate);
				UDPrate.write(Data, PGNlength);
				UDPrate.endPacket();

				DataPGN = 0;
				LSB = 0;

				TeensyTime = millis();

				RCteensyConnected = ((Data[11] & 0b00000001) == 0b00000001) || ((Data[11] & 0b00000010) == 0b00000010);
				ModuleID = Data[2] >> 4;
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
	// 3	RSSI
	// 4	Status
	//		- bit 0 Wifi connected
	//		- bit 1 Restarting UDP
	// 5	DebugVal1
	// 6	DebugVal2
	// 7	CRC

	byte Length = 8;
	Packet[0] = 247;
	Packet[1] = 126;
	Packet[2] = ModuleID;
	Packet[3] = WiFi.RSSI();

	// status
	Packet[4] = 0;
	if (WiFi.status() == WL_CONNECTED)
	{
		Packet[4] |= 0b00000001;
	}

	if (ReStartingUDP)
	{
		Packet[4] |= 0b00000010;
	}

	if (RCteensyConnected)
	{
		Packet[4] |= 0b00000100;
	}

	if (TeensyConnected)
	{
		Packet[4] |= 0b00001000;
	}

	Packet[5] = DebugVal1;
	Packet[6] = DebugVal2;
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
