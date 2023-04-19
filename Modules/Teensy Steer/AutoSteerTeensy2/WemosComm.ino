
byte DataWemos[MaxReadBuffer];
uint16_t PGNwemos;
byte MSBw;
byte LSBw;
elapsedMicros WemosTime;

void ReceiveWemos()
{

    // Wemos D1 Mini
    if (SerialWemos->available())
    {
        if (SerialWemos->available() > MaxReadBuffer)
        {
            // clear buffer
            while (SerialWemos->available())
            {
                SerialWemos->read();
            }
            PGNwemos = 0;
            LSBw = 0;
        }

        switch (PGNwemos)
        {
        case 32628:
            // analog data from ADS1115 through Wemos D1 Mini
            PGNlength = 11;
            if (SerialWemos->available() > PGNlength - 3)
            {
                DataWemos[0] = 116;
                DataWemos[1] = 127;
                for (int i = 2; i < PGNlength; i++)
                {
                    DataWemos[i] = SerialWemos->read();
                }

                if (GoodCRC(DataWemos, PGNlength))
                {
                    if (MDL.AnalogMethod == 2)
                    {
                        AINs.AIN0 = (int16_t)(DataWemos[2] | DataWemos[3] << 8);
                        AINs.AIN1 = (int16_t)(DataWemos[4] | DataWemos[5] << 8);
                        AINs.AIN2 = (int16_t)(DataWemos[6] | DataWemos[7] << 8);
                        AINs.AIN3 = (int16_t)(DataWemos[8] | DataWemos[9] << 8);
                    }
                }
                PGNwemos = 0;
                LSBw = 0;
            }
            break;

        default:
            // find pgn
            MSBw = SerialWemos->read();
            PGNwemos = MSBw << 8 | LSBw;
            LSBw = MSBw;
            break;
        }
    }
}

