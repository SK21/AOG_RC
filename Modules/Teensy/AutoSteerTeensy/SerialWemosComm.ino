
byte SerialWemosPacket[30];
byte SerialWemosMSB;
byte SerialWemosLSB;
unsigned int SerialWemosPGN;

void ReceiveSerialWemos()
{
	if (SerialWemos->available())
	{
		if (SerialWemos->available() > 50)
		{
			// clear buffer
			while (SerialWemos->available())
			{
				SerialWemos->read();
			}
			SerialWemosPGN = 0;
		}

		switch (SerialWemosPGN)
		{
		case 32619:
			// switches from Wemos D1 mini
			PGNlength = 6;
			if (SerialWemos->available() > PGNlength - 3)
			{
				SerialWemosPGN = 0;	// reset pgn
				SerialWemosPacket[0] = 107;
				SerialWemosPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialWemosPacket[i] = SerialWemos->read();
				}

				if (GoodCRC(SerialWemosPacket, PGNlength))
				{
					WifiSwitches.Enabled = true;
					WifiSwitches.StartTime = millis();
					WifiSwitches.MasterOn = SerialWemosPacket[2];
					WifiSwitches.RelaysLo = SerialWemosPacket[3];
					WifiSwitches.RelaysHi = SerialWemosPacket[4];
				}
			}
			break;

		case 32628:
			// analog from Wemos D1 mini

			PGNlength = 11;
			if (SerialWemos->available() > PGNlength - 3)
			{
				SerialWemosPGN = 0;	// reset pgn
				SerialWemosPacket[0] = 116;
				SerialWemosPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialWemosPacket[i] = SerialWemos->read();
				}

				if (GoodCRC(SerialWemosPacket, PGNlength))
				{
					if (PCB.AnalogMethod == 2)
					{
						AINs.AIN0 = (int16_t)(SerialWemosPacket[2] | SerialWemosPacket[3] << 8);
						AINs.AIN1 = (int16_t)(SerialWemosPacket[4] | SerialWemosPacket[5] << 8);
						AINs.AIN2 = (int16_t)(SerialWemosPacket[6] | SerialWemosPacket[7] << 8);
						AINs.AIN3 = (int16_t)(SerialWemosPacket[8] | SerialWemosPacket[9] << 8);
					}
				}
			}
			break;

		default:
			// find pgn, little endian
			SerialWemosMSB = SerialWemos->read();
			SerialWemosPGN = SerialWemosMSB << 8 | SerialWemosLSB;
			SerialWemosLSB = SerialWemosMSB;
			break;
		}
	}
}
