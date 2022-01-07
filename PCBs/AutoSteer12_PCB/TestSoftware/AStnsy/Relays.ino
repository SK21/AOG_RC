#if(UseRateControl)
void CheckRelays()
{
    if (WifiSwitchesEnabled)
    {
        if (millis() - WifiSwitchesTimer > 3000) // 30 second timer
        {
            // wifi switches have timed out
            WifiSwitchesEnabled = false;
        }
        else
        {
            if (WifiSwitches[2])
            {
                // master on
                SetRelays(WifiSwitches[3], WifiSwitches[4]);
            }
            else
            {
                // master off
                SetRelays(0, 0);
                WifiSwitchesEnabled = false;
            }
        }
    }
    else
    {
        if (FlowEnabled[0])
        {
            SetRelays(RelayLo, RelayHi);
        }
        else
        {
            SetRelays(0, 0);
        }
    }
}

// https://discourse.agopengps.com/t/smd-pcb-project-for-an-all-in-one-compact-pcb-for-aog-qog/3640/51

byte CRC[] = { 0xD9, 0x6A, 0x29, 0x6A, 0x78, 0xAA, 0xC9, 0x6B, 0x98, 0xAB, 0x68, 0xAB, 0x39, 0x6B, 0x09, 0x68,
0xD9, 0x9A, 0x29, 0x9A, 0x78, 0x5A, 0xC9, 0x9B, 0x98, 0x5B, 0x68, 0x5B, 0x39, 0x9B, 0x09, 0x98 };

unsigned long LastRelaySend;
bool RelayStatus[16];
bool BitState;

bool RelaysRemaining;   // relay messages left to send
byte RelaysLoByte;
byte RelaysHiByte;

void SetRelays(byte LoByte, byte HiByte)
{
    if (!RelaysRemaining)
    {
        RelaysLoByte = LoByte;
        RelaysHiByte = HiByte;
        RelaysRemaining = true;
    }

    if (millis() - LastRelaySend > 30)  // do one relay at a time, 30 ms apart
    {
        LastRelaySend = millis();
        for (int i = 0; i < 16; i++)
        {
            if (i == SwitchedPowerRelay)
            {
                BitState = HIGH;
            }
            else if (i < 8)
            {
                BitState = bitRead(RelaysLoByte, i);
            }
            else
            {
                BitState = bitRead(RelaysHiByte, i - 8);
            }

            if (RelayStatus[i] != BitState)
            {
                WriteRelay(i + 1, BitState);
                RelayStatus[i] = BitState;
                break;
            }

            if (i == 15) RelaysRemaining = false;
        }
    }
}

void WriteRelay(byte ID, byte State)
{
    byte SendArray[] = { 1,6,0,ID,(byte)(2 - State),0,CRC[2 * ID + 16 * State - 2],CRC[2 * ID + 16 * State - 1] };
    Serial1.write(SendArray, sizeof(SendArray));
}
#endif
