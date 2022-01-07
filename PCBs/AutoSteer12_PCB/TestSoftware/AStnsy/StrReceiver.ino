#if(ReceiverType !=0 && !UsePanda)
byte Count = 0;
bool GPSheaderFound = false;
char CheckSum[2];
char CH;
char GPSdata[100];

void RelayCorrectionData()
{
    int packetSize = UDPgps.parsePacket();
    if (packetSize)
    {
        for (int i = 0; i < packetSize; i++)
        {
            SerialRTCM.write(UDPgps.read());   
        }
    }
}

void RelayGPSData()
{
    RelayCorrectionData();
    UpdateHeadingRoll();

    // GPS data from receiver over serial to AGIO over UDP
    if (SerialNMEA.available())
    {
        CH = SerialNMEA.read();

        if (GPSheaderFound)
        {
            if (CH == '$')
            {
                // start of next message
                if (CheckSumMatch() && HeaderValid())
                {
                    // send current message
                    UDPgps.beginPacket(AGIOip, AGIOport);
                    UDPgps.write(GPSdata, Count);
                    UDPgps.endPacket();
                }

                // begin next message
                GPSdata[0] = CH;
                Count = 1;
            }
            else
            {
                // build current message
                GPSdata[Count] = CH;
                Count++;
                if (Count > 149)
                {
                    // reset
                    Count = 149;
                    GPSheaderFound = false;
                }
            }
        }
        else
        {
            // find new message
            GPSheaderFound = (CH == '$');
            if (GPSheaderFound)
            {
                GPSdata[0] = CH;
                Count = 1;
            }
        }
    }
}

bool CheckSumMatch()
{
    bool Result = false;
    if (Count > 4)
    {
        if (GPSdata[0] == '$')
        {
            byte CS = 0;
            for (int i = 1; i < Count; i++)
            {
                if (GPSdata[i] == '*')
                {
                    if (Count - i > 2)
                    {
                        CheckSum[0] = toHex(CS / 16);
                        CheckSum[1] = toHex(CS % 16);
                        Result = (CheckSum[0] == GPSdata[i + 1]) && (CheckSum[1] = GPSdata[i + 2]);
                        break;
                    }
                    else
                    {
                        // wrong length message
                        break;
                    }
                }
                else
                {
                    CS ^= GPSdata[i];   // xor
                }
            }
        }
    }
    return Result;
}

String Sentence;
bool HeaderValid()
{
    bool Result = false;
    if (Count > 5)
    {
        Sentence = "";
        for (int i = 0; i < 6; i++)
        {
            Sentence += GPSdata[i];
        }
        if (Sentence.indexOf("$GPGGA") != -1)
        {
            Result = true;
        }
        else if (Sentence.indexOf("$GNGGA") != -1)
        {
            Result = true;
        }
        else if (Sentence.indexOf("$GPVTG") != -1)
        {
            Result = true;
        }
        else if (Sentence.indexOf("$GNVTG") != -1)
        {
            Result = true;
        }
    }
    return Result;
}

static char toHex(uint8_t nibble)
{
    if (nibble >= 10)
    {
        return nibble + 'A' - 10;
    }
    else
    {
        return nibble + '0';
    }
}
#endif



