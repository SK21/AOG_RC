
void ReceiveWemos()
{
	if (Serial1.available() > 0 && !PGN32619Found)
	{
		MSB = Serial1.read();
		PGN = MSB << 8 | LSB;
		LSB = MSB;
		PGN32619Found = (PGN == 32619);
	}
	if (Serial1.available() > 3 && PGN32619Found)
	{
		// from Wemos D1 mini
		// section buttons
		PGN32619Found = false;
		Packet[0] = 107;
		Packet[1] = 127;
		for (int i = 2; i < 6; i++)
		{
			Packet[i] = Serial1.read();
			WifiSwitches[i] = Packet[i];
		}

		if (GoodCRC(6))
		{
			WifiSwitchesEnabled = true;
			WifiSwitchesTimer = millis();
		}
	}
}

