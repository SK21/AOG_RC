void SendData()
{
	//PGN32618, switch data
	// 0   106
	// 1   127
	// 2   status
	//      - bit 0 -
	//      - bit 1 MasterOn
	//      - bit 2 MasterOff
	//      - bit 3 RateUp
	//      - bit 4 RateDown
	//      - bit 5 AutoSection
	//      - bit 6 AutoRate
	//		- bit 7 Work switch
	// 3    sw0 to sw7
	// 4    sw8 to sw15
	// 5    InoID Lo
	// 6    InoID Hi
	// 7    -
	// 8    crc

	byte PGNlength = 9;
	byte Data[PGNlength];

	byte Pins[] = { 0,0,0,0,0,0,0,0 };
	Data[0] = 106;
	Data[1] = 127;

	// read switches
	Data[2] = 0;

	if (MDL.MasterOn < NC) Pins[1] = !ReadPin(MDL.MasterOn);
	if (MDL.MasterOff < NC) Pins[2] = !ReadPin(MDL.MasterOff);
	if (MDL.RateUp < NC) Pins[3] = !ReadPin(MDL.RateUp);
	if (MDL.RateDown < NC) Pins[4] = !ReadPin(MDL.RateDown);
	if (MDL.AutoSection < NC) Pins[5] = !ReadPin(MDL.AutoSection);
	if (MDL.AutoRate < NC) Pins[6] = !ReadPin(MDL.AutoRate);
	if (MDL.WorkPin < NC) Pins[7] = ReadPin(MDL.WorkPin);	// high is off, low is on - pin connected to ground

	for (int i = 0; i < 8; i++)
	{
		if (Pins[i]) Data[2] = Data[2] | (1 << i);
	}

	// read section switches
	Data[3] = 0;
	Data[4] = 0;

	for (int i = 0; i < 16; i++)
	{
		if (MDL.SectionPins[i]< NC)
		{
			if (i < 8)
			{
				if (!ReadPin(MDL.SectionPins[i])) Data[3] = Data[3] | (1 << i);
			}
			else
			{
				if (!ReadPin(MDL.SectionPins[i])) Data[4] = Data[4] | (1 << (i - 8));
			}
		}
	}

	Data[5] = InoID;
	Data[6] = InoID >> 8;

	Data[PGNlength-1] = CRC(Data, PGNlength-1, 0);

	if (EthernetConnected())
	{
		// send ethernet
		ether.sendUdp(Data, PGNlength, SourcePort, DestinationIP, DestinationPort);
	}
}

bool ReadPin(int PinID)
{
	bool Result = false;

	if (PinID == 20 || PinID == 21)
	{
		// A6 or A7 analog read
		Result = analogRead(PinID) > 500;
	}
	else
	{
		Result = digitalRead(PinID);
	}
	return Result;
}