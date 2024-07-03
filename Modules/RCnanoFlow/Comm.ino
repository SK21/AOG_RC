
void SendData()
{
	//PGN32400, Rate info from module to RC
	//0     HeaderLo    144
	//1     HeaderHi    126
	//2     Mod/Sen ID          0-15/0-15
	//3		flow meter Hz low
	//4		Hz mid
	//5		Hz high
	//6	    -
	//7	    -
	//8     -
	//9     -
	//10    -
	//11    Status
	//      bit 0 - 
	//      bit 1 - 
	//      bit 2   - 
	//      bit 3	- 
	//      bit 4	- 
	//      bit 5   Sending Hz only
	//      bit 6   
	//      bit 7  
	//12    CRC

	byte Data[13];

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Data[0] = 144;
		Data[1] = 126;
		Data[2] = BuildModSenID(MDL.ID, i);
		Serial.println(Sensor[0].Hz);

		uint32_t Hz = Sensor[i].Hz * 1000.0;
		Data[3] = Hz;
		Data[4] = Hz >> 8;
		Data[5] = Hz >> 16;

		Data[6] = 0;
		Data[7] = 0;
		Data[8] = 0;
		Data[9] = 0;
		Data[10] = 0;

		Data[11] = 0b11100011;	// sensor 0 receiving, sensor 1 receiving, Hz only mode, ethernet connected, good pins
		Data[12] = CRC(Data, 12, 0);

		if (EthernetConnected())
		{
			// send ethernet
			ether.sendUdp(Data, 13, SourcePort, DestinationIP, DestinationPort);
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
	}
}

