
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

