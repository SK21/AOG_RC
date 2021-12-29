#if(ReceiverType !=0)
char GPSdata[150];
char NewByte;
byte Count = 0;
bool GPSheaderFound = false;
char CheckSum[2];

// Blank RTCM3 type 1008 message
const char packet1008[12] = { 0xd3,0x00,0x06,0x3f,0x00,0x00,0x00,0x00,0x00,0x99,0x25,0xca };

byte RTCMbyte = 0;
int RTCMlength = 0;
unsigned int RTCMtype = 0;
int RTCMcount = 0;
bool RTCMheaderFound;
bool RTCMlengthFound;
bool RTCMtypeFound;

void RelayCorrectionData()
{
    int packetSize = UDPgps.parsePacket();
    if (packetSize)
    {
        for (int i = 0; i < packetSize; i++)
        {
            RTCMbyte = UDPgps.read();
            RcvrSerial2.write(RTCMbyte);

#if(Use1008)
            Inject1008();
#endif
        }
    }
}

void RelayGPSData()
{
    // GPS data from receiver over serial to AGIO over UDP
    while (RcvrSerial1.available())
    {
        NewByte = RcvrSerial1.read();

        if (GPSheaderFound)
        {
            if (NewByte == '$')
            {
                // start of next message
                if (CheckSumMatch())
                {
                    // send current message
                    UDPgps.beginPacket(AGIOip, AGIOport);
                    UDPgps.write(GPSdata, Count);
                    UDPgps.endPacket();
                }

                // begin next message
                GPSdata[0] = NewByte;
                Count = 1;
            }
            else
            {
                // build current message
                GPSdata[Count] = NewByte;
                Count++;
                if (Count > 150)
                {
                    // reset
                    Count = 150;
                    GPSheaderFound = false;
                }
            }
        }
        else
        {
            // find new message
            GPSheaderFound = (NewByte == '$');
            if (GPSheaderFound)
            {
                GPSdata[0] = NewByte;
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

//adapted from https://github.com/torriem/rtcm3add1008
void Inject1008()
{
    if (RTCMheaderFound)
    {
        if (RTCMlengthFound)
        {
            if (RTCMtypeFound)
            {
                RTCMcount++;
                if (RTCMcount == RTCMlength + 3)    // message length + 3 CRC bytes
                {
                    //inject a 1008 message
                    RcvrSerial1.write(packet1008, 12);
                    RTCMheaderFound = false;    // start again
                }
            }
            else
            {
                RTCMtype = (RTCMtype << 8) + RTCMbyte;
                RTCMcount++;
                if (RTCMcount == 2)
                {
                    RTCMtypeFound = true;
                    RTCMtype = RTCMtype >> 4;   //isolate type to the most significant 12 bits
                    if ((RTCMtype != 1005) && (RTCMtype != 1006)) RTCMheaderFound = false;  // wrong type, start over
                }
            }
        }
        else
        {
            RTCMlength = (RTCMlength << 8) + RTCMbyte;
            RTCMcount++;
            if (RTCMcount == 2)
            {
                RTCMlengthFound = true;
                RTCMlength = RTCMlength & 0x03FF;   //isolate only the least significant 10 bits
                RTCMcount = 0;
            }
        }
    }
    else
    {
        if (RTCMbyte == 0xD3)
        {
            RTCMheaderFound = true;
            RTCMcount = 0;
            RTCMlength = 0;
            RTCMlengthFound = false;
            RTCMtype = 0;
            RTCMtypeFound = false;
        }
    }
}
#endif

