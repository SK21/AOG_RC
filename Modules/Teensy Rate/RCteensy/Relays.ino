
bool RelayStatus[16];
uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 }; // 8 relay module and a PCA9535PW
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 }; // 16 relay module and a PCA9535PW

void CheckRelays()
{
    uint8_t Rlys;
    bool BitState;
    uint8_t IOpin;

    uint8_t NewLo = 0;
    uint8_t NewHi = 0;

    if (WifiSwitchesEnabled)
    {
        // wifi relay control
        // controls by relay # not section #
        if (millis() - WifiSwitchesTimer > 30000)   // 30 second timer
        {
            // wifi switches have timed out
            WifiSwitchesEnabled = false;
        }
        else
        {
            if (WifiSwitches[2])
            {
                // wifi master on
                NewLo = WifiSwitches[3];
                NewHi = WifiSwitches[4];
            }
            else
            {
                // wifi master off
                WifiSwitchesEnabled = false;
            }
        }
    }
    else if (Sensor[0].FlowEnabled || Sensor[1].FlowEnabled)
    {
        // normal relay control
        NewLo |= RelayLo;
        NewHi |= RelayHi;
    }
    else
    {
        // inverted relays, 1 is off
        NewLo |= InvertedLo;
        NewHi |= InvertedHi;
    }

    // power relays, always on
    NewLo |= PowerRelayLo;
    NewHi |= PowerRelayHi;

    switch (MDL.RelayControl)
    {
    case 1:
        // GPIOs
        for (int j = 0; j < 2; j++)
        {
            if (j < 1) Rlys = NewLo; else Rlys = NewHi;
            for (int i = 0; i < 8; i++)
            {
                if (MDL.RelayPins[i + j * 8] < NC) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(MDL.RelayPins[i + j * 8], MDL.RelayOnSignal); else digitalWrite(MDL.RelayPins[i + j * 8], !MDL.RelayOnSignal);
                }
            }
        }
        break;

    case 2:
        // PCA9555 8 relays
        if (PCA9555PW_found)
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
                        PCA.write(IOpin, PCA95x5::Level::L);
                    }
                    else
                    {
                        // off
                        PCA.write(IOpin, PCA95x5::Level::H);
                    }
                    RelayStatus[i] = BitState;
                }
            }
        }
        break;

    case 3:
        // PCA9555 16 relays
        if (PCA9555PW_found)
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
                        PCA.write(IOpin, PCA95x5::Level::L);
                    }
                    else
                    {
                        // off
                        PCA.write(IOpin, PCA95x5::Level::H);
                    }
                    RelayStatus[i] = BitState;
                }
            }
        }
        break;

    case 4:
        // MCP23017
        if (MCP23017_found)
        {
            for (int j = 0; j < 2; j++)
            {
                if (j < 1) Rlys = NewLo; else Rlys = NewHi;
                for (int i = 0; i < 8; i++)
                {
                    if (bitRead(Rlys, i))
                    {
                        MCP.digitalWrite(MDL.RelayPins[i + j * 8], MDL.RelayOnSignal);
                    }
                    else
                    {
                        MCP.digitalWrite(MDL.RelayPins[i + j * 8], !MDL.RelayOnSignal);
                    }
                }
            }
        }
        break;
    }
}


