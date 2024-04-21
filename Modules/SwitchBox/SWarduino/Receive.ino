
byte SerialMSB;
byte SerialLSB;
unsigned int SerialPGN;
byte SerialPGNlength;
byte SerialReceive[35];
bool PGNfound;

void ReceiveSerial()
{
	if (Serial.available())
	{
		if (Serial.available() > 50)
		{
			// clear buffer and reset pgn
			while (Serial.available())
			{
				Serial.read();
			}
			SerialPGN = 0;
			PGNfound = false;
		}

		if (PGNfound)
		{
			if (Serial.available() > SerialPGNlength - 3)
			{
				for (int i = 2; i < SerialPGNlength; i++)
				{
					SerialReceive[i] = Serial.read();
				}
				ReadPGNs(SerialReceive, SerialPGNlength);

				// reset pgn
				SerialPGN = 0;
				PGNfound = false;
			}
		}
		else
		{
			switch (SerialPGN)
			{
			case 32503:
				SerialPGNlength = 6;
				PGNfound = true;
				break;

			case 32701:
				SerialPGNlength = 25;
				PGNfound = true;
				break;

			default:
				// find pgn
				SerialMSB = Serial.read();
				SerialPGN = SerialMSB << 8 | SerialLSB;

				SerialReceive[0] = SerialLSB;
				SerialReceive[1] = SerialMSB;

				SerialLSB = SerialMSB;
				break;
			}
		}
	}
}

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* Data, uint16_t len)
{
	ReadPGNs(Data, len);
}

void ReadPGNs(byte Data[], uint16_t len)
{
	byte PGNlength;
	uint16_t PGN = Data[1] << 8 | Data[0];

	switch (PGN)
	{
	case 32503:
		//PGN32503, Subnet change
		//0     HeaderLo    247
		//1     HeaderHI    126
		//2     IP 0
		//3     IP 1
		//4     IP 2
		//5     CRC

		PGNlength = 6;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				MDL.IP0 = Data[2];
				MDL.IP1 = Data[3];
				MDL.IP2 = Data[4];

				SaveData();

				// restart
				resetFunc();
			}
		}
		break;

	case 32701:
		// Switchbox pins
		//0         HeaderLo    189
		//1         HeaderHi    127
		//2         Auto
		//3         Master On
		//4         Master Off
		//5         Rate Up
		//6         Rate Down
		//7-22      switches 1-16
		//23		Work pin
		//24        crc

		PGNlength = 25;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				MDL.Auto = Data[2];
				MDL.MasterOn = Data[3];
				MDL.MasterOff = Data[4];
				MDL.RateUp = Data[5];
				MDL.RateDown = Data[6];

				for (int i = 7; i < 23; i++)
				{
					MDL.SectionPins[i - 7] = Data[i];
				}

				MDL.WorkPin = Data[23];

				SaveData();

				// restart
				resetFunc();
			}
		}
		break;
	}
}


