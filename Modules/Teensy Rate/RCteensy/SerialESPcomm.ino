
byte MSB;
byte LSB;
byte SD[MaxReadBuffer];
uint16_t ESPpgn;
byte ESPpgnLength;

void ReceiveESP()
{
	if (SerialESP->available())
	{
		if (SerialESP->available() > MaxReadBuffer)
		{
			// clear buffer
			while (SerialESP->available())
			{
				SerialESP->read();
			}
			ESPpgn = 0;
			LSB = 0;
		}

		switch (ESPpgn)
		{
		case 32600:
			//PGN32600, section switches from ESP to module
			// 0    88
			// 1    127
			// 2    MasterOn
			// 3	switches 0-7
			// 4	switches 8-15
			// 5	crc

			ESPpgnLength = 6;

			if (Serial.available() > ESPpgnLength - 3)
			{
				ESPpgn = 0;	// reset pgn
				SD[0] = 88;
				SD[1] = 127;
				for (int i = 2; i < ESPpgnLength; i++)
				{
					SD[i] = SerialESP->read();
					WifiSwitches[i] = SD[i];
				}

				if (GoodCRC(SD, ESPpgnLength))
				{
					WifiSwitchesEnabled = true;
					WifiSwitchesTimer = millis();
				}
			}
			break;

		default:
			// find pgn
			MSB = SerialESP->read();
			ESPpgn = MSB << 8 | LSB;
			LSB = MSB;
			break;
		}
	}
}

