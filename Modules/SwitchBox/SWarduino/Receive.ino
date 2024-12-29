
byte SerialMSB;
byte SerialLSB;
unsigned int SerialPGN;
byte SerialPGNlength;
byte SerialReceive[35];
bool PGNfound;

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* Data, uint16_t len)
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
		//2         -
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
				MDL.MasterOn = Data[3];
				MDL.MasterOff = Data[4];
				MDL.RateUp = Data[5];
				MDL.RateDown = Data[6];

				for (int i = 7; i < 23; i++)
				{
					MDL.SectionPins[i - 7] = Data[i];
					if (MDL.SectionPins[i - 7] < 3) MDL.SectionPins[i - 7] = NC;
				}
				MDL.AutoSection = NC;
				MDL.AutoRate = NC;
				MDL.WorkPin = Data[23];
				if (MDL.WorkPin < 3) MDL.WorkPin = NC;

				SaveData();

				// restart
				resetFunc();
			}
		}
		break;
	}
}


