
byte CRC[] = { 0xD9, 0x6A, 0x29, 0x6A, 0x78, 0xAA, 0xC9, 0x6B, 0x98, 0xAB, 0x68, 0xAB, 0x39, 0x6B, 0x09, 0x68,
0xD9, 0x9A, 0x29, 0x9A, 0x78, 0x5A, 0xC9, 0x9B, 0x98, 0x5B, 0x68, 0x5B, 0x39, 0x9B, 0x09, 0x98 };

uint32_t LastRelaySend;
bool RelayStatus[16];
bool BitState;
uint8_t IOpin;
PCA9535 ioex;
uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 }; // 8 relay module and a PCA9535PW
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 }; // 16 relay module and a PCA9535PW

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
        //if (FlowEnabled[0])
        //{
        //    SetRelays(RelayLo, RelayHi);
        //}
        //else
        //{
        //    SetRelays(0, 0);
        //}

        SetRelays(RelayLo, RelayHi);
    }
}

void SetRelays(byte LoByte, byte HiByte)
{
    switch (PCB.RelayControl)
    {
    case 1:
        // RS485
        // https://discourse.agopengps.com/t/smd-pcb-project-for-an-all-in-one-compact-pcb-for-aog-qog/3640/51

        if (millis() - LastRelaySend > 30)
        {
            LastRelaySend = millis();
            for (int i = 0; i < 16; i++)
            {
                if (i < 8)
                {
                    BitState = bitRead(LoByte, i);
                }
                else
                {
                    BitState = bitRead(HiByte, i - 8);
                }

                if (RelayStatus[i] != BitState)
                {
                    byte ID = i + 1;
                    byte SendArray[] = { 1,6,0,ID,(byte)(2 - BitState),0,CRC[2 * ID + 16 * BitState - 2],CRC[2 * ID + 16 * BitState - 1] };
                    SerialRS485->write(SendArray, sizeof(SendArray));
                    RelayStatus[i] = BitState;
                    break;  // do one relay at a time
                }
            }
        }
        break;

    case 2:
        // 8 relay module
        for (int i = 0; i < 8; i++)
        {
            BitState = bitRead(LoByte, i);

            if (RelayStatus[i] != BitState)
            {
                IOpin = Relays8[i];

                if (BitState)
                {
                    // on
                    ioex.write(IOpin, PCA95x5::Level::L);
                }
                else
                {
                    // off
                    ioex.write(IOpin, PCA95x5::Level::H);
                }
                RelayStatus[i] = BitState;
            }
        }
        break;

    case 3:
        // 16 relay module
        for (int i = 0; i < 16; i++)
        {
                if (i < 8)
                {
                    BitState = bitRead(LoByte, i);
                }
                else
                {
                    BitState = bitRead(HiByte, i - 8);
                }

            if (RelayStatus[i] != BitState)
            {
                IOpin = Relays16[i];

                if (BitState)
                {
                    // on
                    ioex.write(IOpin, PCA95x5::Level::L);
                }
                else
                {
                    // off
                    ioex.write(IOpin, PCA95x5::Level::H);
                }
                RelayStatus[i] = BitState;
            }
        }
        break;

    default:
        // no relays
        break;
    }
}
