byte ErrorCount;
unsigned int DataPGN;
byte Data[100];
byte PGNlength;
byte MSB;
byte LSB;

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
				DataPGN = 0;
				LSB = 0;

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
			}
			break;

		case 32401:
			//PGN32401, module, analog info from module to RC
			PGNlength = 15;
			if (Serial.available() > PGNlength - 3)
			{
				DataPGN = 0;
				LSB = 0;

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
			}
			break;

		case 32702:
			// PGN32702, network config
			// 0        190
			// 1        127
			// 2-16     Network Name
			// 17-31    Newtwork password
			// 32       CRC

			PGNlength = 33;
			if (Serial.available() > PGNlength - 3)
			{
				DataPGN = 0;
				LSB = 0;

				Data[0] = 190;
				Data[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					Data[i] = Serial.read();
				}

				if (GoodCRC(Data, PGNlength))
				{
					// network name
					memset(MDL.SSID, '\0', sizeof(MDL.SSID)); // erase old name
					memcpy(MDL.SSID, &Data[2], 14);

					// network password
					memset(MDL.Password, '\0', sizeof(MDL.Password)); // erase old name
					memcpy(MDL.Password, &Data[17], 14);

					SaveData();

					ESP.restart();
				}
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

void SendStatus()
{
	//PGN32600, ESP status to rate module
	// 0    88
	// 1    127
	// 2    MasterOn
	// 3	switches 0-7
	// 4	switches 8-15
	// 5	switches changed
	// 6	signal strength
	// 7	crc

	PGNlength = 8;

	byte Data[PGNlength];
	Data[0] = 88;
	Data[1] = 127;
	Data[2] = MasterOn;
	Data[3] = 0;
	Data[4] = 0;
	Data[5] = SwitchesChanged;
	Data[6] = WiFi.RSSI();

	// convert section switches to bits
	for (int i = 0; i < 16; i++)
	{
		SendByte = i / 8;
		SendBit = i - SendByte * 8;
		if (Button[i]) bitSet(Data[SendByte + 3], SendBit);
	}

	// crc
	Data[PGNlength - 1] = CRC(Data, PGNlength - 1, 0);

	// send
	for (int i = 0; i < PGNlength; i++)
	{
		Serial.write(Data[i]);
	}
	Serial.println("");
	SwitchesChanged = false;
}


