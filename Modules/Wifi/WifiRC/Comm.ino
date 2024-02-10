byte ErrorCount;
unsigned int DataPGN;
byte Data[100];
byte PGNlength;
byte MSB;
byte LSB;

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
		case 32400:
			//PGN32400, Rate info from module to RC
			PGNlength = 13;
			if (Serial.available() > PGNlength - 3)
			{
				Data[0] = 144;
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

		case 32401:
			//PGN32401, module, analog info from module to RC
			PGNlength = 15;
			if (Serial.available() > PGNlength - 3)
			{
				Data[0] = 145;
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
	//PGN32600, section switches from ESP to module
	// 0    88
	// 1    127
	// 2    MasterOn
	// 3	switches 0-7
	// 4	switches 8-15
	// 5	crc

	byte Data[6];
	Data[0] = 88;
	Data[1] = 127;
	Data[2] = MasterOn;
	Data[3] = 0;
	Data[4] = 0;

	// convert section switches to bits
	for (int i = 0; i < 16; i++)
	{
		SendByte = i / 8;
		SendBit = i - SendByte * 8;
		if (Button[i]) bitSet(Data[SendByte + 3], SendBit);
	}

	// crc
	Data[5] = CRC(Data, 5, 0);

	// send
	for (int i = 0; i < 6; i++)
	{
		Serial.write(Data[i]);
	}
	Serial.println("");
}


