void SendData()
{
	//PGN32618, switch data
	//0     HeaderLo    106
	//1     HeaderHi    127
	// 2    - bit 0 -
	//      - bit 1 MasterOn
	//      - bit 2 MasterOff
	//      - bit 3 RateUp
	//      - bit 4 RateDown
	//      - bit 5 AutoSection
	//      - bit 6 AutoRate
	//		- bit 7 Work switch
	//3	    sw0 to sw7
	//4     sw8 to sw15
	//5	    CRC

	byte Pins[] = { 0,0,0,0,0,0,0,0 };
	byte Data[6];
	Data[0] = 106;
	Data[1] = 127;

	// read switches
	Data[2] = 0;

	if (MDL.MasterOn < NC) Pins[1] = !digitalRead(MDL.MasterOn);
	if (MDL.MasterOff < NC) Pins[2] = !digitalRead(MDL.MasterOff);
	if (MDL.RateUp < NC) Pins[3] = !digitalRead(MDL.RateUp);
	if (MDL.RateDown < NC) Pins[4] = !digitalRead(MDL.RateDown);
	if (MDL.AutoSection < NC) Pins[5] = !digitalRead(MDL.AutoSection);
	if (MDL.AutoRate < NC) Pins[6] = !digitalRead(MDL.AutoRate);
	if (MDL.WorkPin < NC) Pins[7] = digitalRead(MDL.WorkPin);	// high is off, low is on - pin connected to ground

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
				if (!digitalRead(MDL.SectionPins[i])) Data[3] = Data[3] | (1 << i);
			}
			else
			{
				if (!digitalRead(MDL.SectionPins[i])) Data[4] = Data[4] | (1 << (i - 8));
			}
		}
	}

	Data[5] = CRC(Data, 5, 0);

	if (EthernetConnected())
	{
		// send ethernet
		ether.sendUdp(Data, 6, SourcePort, DestinationIP, DestinationPort);
	}
}