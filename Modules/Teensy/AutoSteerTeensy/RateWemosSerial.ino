
void ReceiveWemos()
{
	if (Serial1.available() > 0 && !PGN32619Found)
	{
		MSB = Serial1.read();
		PGN = MSB << 8 | LSB;
		LSB = MSB;
		PGN32619Found = (PGN == 32619);
	}
	if (Serial1.available() > 2 && PGN32619Found)
	{
		// from Wemos D1 mini
		// section buttons
		PGN32619Found = false;
		for (int16_t i = 2; i < 5; i++)
		{
			WifiSwitches[i] = Serial1.read();
		}
		WifiSwitchesEnabled = true;
		WifiSwitchesTimer = millis();
	}
}

