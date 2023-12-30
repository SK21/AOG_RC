void SendData()
{
	//PGN32618, switch data
	//0     HeaderLo    106
	//1     HeaderHi    127
	//2     command
	//      bit 0 - auto
	//      bit 1 - MasterOn
	//      bit 2 - MasterOff
	//      bit 3 - Rate Up
	//      bit 4 - Rate Down
	//3	    sw0 to sw7
	//4     sw8 to sw15
	//5	    CRC

	byte Pins[5];
	byte Data[6];
	Data[0] = 106;
	Data[1] = 127;

	// read switches
	Pins[0] = !digitalRead(MDL.Auto);
	Pins[1] = !digitalRead(MDL.MasterOn);
	Pins[2] = !digitalRead(MDL.MasterOff);
	Pins[3] = !digitalRead(MDL.RateUp);
	Pins[4] = !digitalRead(MDL.RateDown);

	Data[2] = 0;
	for (int i = 0; i < 5; i++)
	{
		if (Pins[i]) Data[2] = Data[2] | (1 << i);
	}

	Data[3] = 0;
	Data[4] = 0;
	for (int i = 0; i < 16; i++)
	{
		if (MDL.PinIDs[i] > 0)
		{
			if (i < 8)
			{
				if (!digitalRead(MDL.PinIDs[i])) Data[3] = Data[3] | (1 << i);
			}
			else
			{
				if (!digitalRead(MDL.PinIDs[i])) Data[4] = Data[4] | (1 << (i - 8));
			}
		}
	}

	Data[5] = CRC(Data, 12, 0);

	if (ENCfound)
	{
		// send ethernet
		ether.sendUdp(Data, 6, SourcePort, DestinationIP, DestinationPort);
	}
	else
	{
		// send serial
		for (int i = 0; i < 6; i++)
		{
			Serial.print(Data[i]);
			if (i < 5) Serial.print(",");
		}
		Serial.println();
	}
}