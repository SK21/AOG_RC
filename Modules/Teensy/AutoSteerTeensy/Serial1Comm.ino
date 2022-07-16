
bool IDfound = false;
byte Serial1Packet[30];
byte Serial1MSB;
byte Serial1LSB;
unsigned int Serial1PGN;

void ReceiveSerial1()
{
	if (Serial1.available())
	{
		if (Serial1.available() > 30)
		{
			// clear buffer
			while (Serial1.available())
			{
				Serial1.read();
			}
			Serial1PGN = 0;
			IDfound = false;
		}

		switch (Serial1PGN)
		{
		case 32619:
			// from Wemos D1 mini
			PGNlength = 6;
			if (Serial1.available() > PGNlength - 3)
			{
				// section buttons, pgn 6 bytes
				Serial1PGN = 0;	// reset pgn
				Serial1Packet[0] = 107;
				Serial1Packet[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					Serial1Packet[i] = Serial1.read();
					WifiSwitches[i] = Serial1Packet[i];
				}

				if (GoodCRC(Serial1Packet, PGNlength))
				{
					WifiSwitchesEnabled = true;
					WifiSwitchesTimer = millis();
				}
			}
			break;

		case 33152:	// little endian conversion of 0x8081
			// AOG pgn
			if (IDfound)
			{
				if (Serial1Packet[2] == 121 && Serial1Packet[3] == 211)
				{
					// imu, pgn 211, 13 bytes
					if (Serial1.available() > 8)
					{
						Serial1PGN = 0;	// reset pgn
						IDfound = false;
						for (int i = 4; i < 13; i++)
						{
							Serial1Packet[i] = Serial1.read();
						}

						if (GoodCRC(Serial1Packet, 13))
						{
							int16_t tmp = (int16_t)(Serial1Packet[4] | Serial1Packet[5] << 8);
							Heading_Serial = tmp;

							tmp = (int16_t)(Serial1Packet[6] | Serial1Packet[7] << 8);
							Roll_Serial = tmp;

							tmp = (int16_t)(Serial1Packet[10] | Serial1Packet[11] << 8);
							Pitch_Serial = tmp;
						}
					}
				}
				else
				{
					// nothing matches, reset
					Serial1PGN = 0;
					IDfound = false;
				}
			}
			else if (Serial1.available() > 1)
			{
				// find ID
				Serial1Packet[0] = 0x80;			// header 1
				Serial1Packet[1] = 0x81;			// header 2
				Serial1Packet[2] = Serial1.read();	// source
				Serial1Packet[3] = Serial1.read();	// AOG PGN
				IDfound = true;
			}
			break;

		default:
			// find pgn, little endian
			Serial1MSB = Serial1.read();
			Serial1PGN = Serial1MSB << 8 | Serial1LSB;
			Serial1LSB = Serial1MSB;
			break;
		}
	}
}
