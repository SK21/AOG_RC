
unsigned int DataPGN;
byte Data[100];
byte PGNlength;
byte MSB;
byte LSB;

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
	if (PacketSize)
	{
		UDPrate.read(WifiBuffer, PacketSize);
		Serial.write(WifiBuffer, PacketSize);
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

				RCteensyConnected = Data[11];

				// send data over wifi
				UDPrate.beginPacket(DestinationIP, DestinationPortRate);
				UDPrate.write(Data, PGNlength);
				UDPrate.endPacket();

				DataPGN = 0;
				LSB = 0;
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
		yield();
	}
	Serial.println("");
}
