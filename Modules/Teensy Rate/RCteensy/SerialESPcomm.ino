
byte MSB;
byte LSB;
byte SD[MaxReadBuffer];
uint16_t ESPpgn;
byte ESPpgnLength;
bool ESPpgnFound;

void ReceiveESP()
{
	if (MDL.ESPserialPort != NC)
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
				ESPpgnFound = false;
			}

			if (ESPpgnFound)
			{
				if (SerialESP->available() > ESPpgnLength - 3)
				{
					for (int i = 2; i < ESPpgnLength; i++)
					{
						SD[i] = SerialESP->read();
					}
					ReadPGNs(SD, ESPpgnLength);

					// reset pgn
					ESPpgn = 0;
					ESPpgnFound = false;
				}
			}
			else
			{
				switch (ESPpgn)
				{
				case 32500:
					ESPpgnLength = 14;
					ESPpgnFound = true;
					ESPtime = millis();
					break;

				case 32501:
					ESPpgnLength = 10;
					ESPpgnFound = true;
					break;

				case 32502:
					ESPpgnLength = 10;
					ESPpgnFound = true;
					break;

				case 32503:
					ESPpgnLength = 6;
					ESPpgnFound = true;
					break;

				case 32600:
					ESPpgnLength = 8;
					ESPpgnFound = true;
					break;

				case 32700:
					ESPpgnLength = 31;
					ESPpgnFound = true;
					break;

				case 32702:
					ESPpgnLength = 33;
					ESPpgnFound = true;
					break;

				default:
					// find pgn
					MSB = SerialESP->read();
					ESPpgn = MSB << 8 | LSB;

					SD[0] = LSB;
					SD[1] = MSB;

					LSB = MSB;
					break;
				}
			}
		}
	}
}

