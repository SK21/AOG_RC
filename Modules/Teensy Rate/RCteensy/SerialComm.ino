
byte MSB;
byte LSB;
byte SD[MaxReadBuffer];
uint16_t SerialPGN;
byte SerialPGNlength;

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
            SerialPGN = 0;
			LSB = 0;
		}

		switch (SerialPGN)
		{
        case 32600:
			//PGN32600, section switches from ESP to module
			// 0    88
			// 1    127
			// 2    MasterOn
			// 3	switches 0-7
			// 4	switches 8-15
			// 5	crc

            SerialPGNlength = 6;

            if (Serial.available() > SerialPGNlength - 3)
            {
                SerialPGN = 0;	// reset pgn
                SD[0] = 88;
                SD[1] = 127;
                for (int i = 2; i < SerialPGNlength; i++)
                {
                    SD[i] = SerialESP->read();
                    WifiSwitches[i] = SD[i];
                }

                if (GoodCRC(SD, SerialPGNlength))
                {
                    WifiSwitchesEnabled = true;
                    WifiSwitchesTimer = millis();
                }
            }
            break;

        default:
			// find pgn
			MSB = SerialESP->read();
            SerialPGN = MSB << 8 | LSB;
			LSB = MSB;
			break;
		}
	}
}
