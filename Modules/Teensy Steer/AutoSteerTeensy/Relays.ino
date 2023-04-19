
byte CRC_RS485[] = { 0xD9, 0x6A, 0x29, 0x6A, 0x78, 0xAA, 0xC9, 0x6B, 0x98, 0xAB, 0x68, 0xAB, 0x39, 0x6B, 0x09, 0x68,
0xD9, 0x9A, 0x29, 0x9A, 0x78, 0x5A, 0xC9, 0x9B, 0x98, 0x5B, 0x68, 0x5B, 0x39, 0x9B, 0x09, 0x98 };

uint32_t LastRelaySend;
bool RelayStatus[16];
bool BitState;
uint8_t IOpin;
uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 }; // 8 relay module and a PCA9535PW
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 }; // 16 relay module and a PCA9535PW
uint8_t NewLo;
uint8_t NewHi;

void CheckRelays()
{
    NewLo = 0;
    NewHi = 0;

    if (WifiSwitches.Enabled)
    {
        // wifi relay control
        // controls by relay # not section #
        if (millis() - WifiSwitches.StartTime > 30000) // 30 second timer
        {
            // wifi switches have timed out
            WifiSwitches.Enabled = false;
        }
        else
        {
            if (WifiSwitches.MasterOn)
            {
                // wifi master on
                NewLo = WifiSwitches.RelaysLo;
                NewHi = WifiSwitches.RelaysHi;
            }
            else
            {
                // wifi master off
                WifiSwitches.MasterOn = false;
            }
        }
    }
    else if (FlowEnabled[0] || FlowEnabled[1])
    {
        // normal relay control
        NewLo = RelayLo;
        NewHi = RelayHi;
    }

    // power relays, always on
    NewLo |= PowerRelayLo;
    NewHi |= PowerRelayHi;

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
                    BitState = bitRead(NewLo, i);
                }
                else
                {
                    BitState = bitRead(NewHi, i - 8);
                }

                if (RelayStatus[i] != BitState)
                {
                    byte ID = i + 1;
                    byte SendArray[] = { 1,6,0,ID,(byte)(2 - BitState),0,CRC_RS485[2 * ID + 16 * BitState - 2],CRC_RS485[2 * ID + 16 * BitState - 1] };
                    SerialRS485->write(SendArray, sizeof(SendArray));
                    RelayStatus[i] = BitState;
                    break;  // do one relay at a time
                }
            }
        }
        break;

    case 2:
        // 8 relay module
        if (IOexpanderFound)
        {
            for (int i = 0; i < 8; i++)
            {
                BitState = bitRead(NewLo, i);

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
        }
        break;

    case 3:
        // 16 relay module
        if (IOexpanderFound)
        {
            for (int i = 0; i < 16; i++)
            {
                if (i < 8)
                {
                    BitState = bitRead(NewLo, i);
                }
                else
                {
                    BitState = bitRead(NewHi, i - 8);
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
        }
        break;

    default:
        // no relays
        break;
    }
}
