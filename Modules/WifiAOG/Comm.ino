
byte SerialMSB;
byte SerialLSB;
unsigned int DataPGN;
byte Data[50];
byte PGNlength;

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
			UDPwifi.begin(ListeningPortRate);
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
	uint16_t PacketSize = UDPwifi.parsePacket();
	if (PacketSize)
	{
		UDPwifi.read(WifiBuffer, PacketSize);
		Serial.write(WifiBuffer, PacketSize);
	}

	PacketSize = UDPconfig.parsePacket();
	if (PacketSize)
	{
		UDPconfig.read(WifiBuffer, PacketSize);
		Serial.write(WifiBuffer, PacketSize);
	}
}

void ReceiveSerial()
{
	if (Serial.available())
	{
		if (Serial.available() > 50)
		{
			// clear buffer
			while (Serial.available())
			{
				Serial.read();
			}
			DataPGN = 0;
		}

		switch (DataPGN)
		{
		case 32613:
			PGNlength = 13;
			if (Serial.available() > PGNlength - 3)
			{
				DataPGN = 0;	// reset pgn
				Data[0] = 101;
				Data[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					Data[i] = Serial.read();
				}

				TeensyConnected = Data[11];

				// send data over wifi
				UDPwifi.beginPacket(DestinationIP, DestinationPort);
				UDPwifi.write(Data, PGNlength);
				UDPwifi.endPacket();
			}
			break;

		case 32621:
			PGNlength = 12;
			if (Serial.available() > PGNlength - 3)
			{
				DataPGN = 0;
				Data[0] = 109;
				Data[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					Data[i] = Serial.read();
				}

				// send data over wifi
				UDPwifi.beginPacket(DestinationIP, DestinationPort);
				UDPwifi.write(Data, PGNlength);
				UDPwifi.endPacket();
			}
			break;

		default:
			// find pgn
			SerialMSB = Serial.read();
			DataPGN = SerialMSB << 8 | SerialLSB;
			SerialLSB = SerialMSB;
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
		yield();
	}
	Serial.println("");
}
