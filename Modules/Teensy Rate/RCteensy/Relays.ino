byte Rlys;
uint8_t NewLo;
uint8_t NewHi;

void CheckRelays()
{
    NewLo = 0;
    NewHi = 0;

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
        {
            // normal relay control
            NewLo = RelayLo;
            NewHi = RelayHi;
        }
    }

    // power relays, always on
    NewLo |= PowerRelayLo;
    NewHi |= PowerRelayHi;

    switch (MDL.RelayControl)
    {
    case 1:
        // rs485
        break;

    case 2:
        // PCA9555 8 relays
        break;

    case 3:
        // PCA9555 16 relays
        break;

    case 4:
        // MCP23017
        break;

    case 5:
        // GPIOs
        for (int j = 0; j < 2; j++)
        {
            if (j < 1) Rlys = NewLo; else Rlys = NewHi;
            for (int i = 0; i < 8; i++)
            {
                if (MDL.RelayPins[i] > 1) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(MDL.RelayPins[i], MDL.RelayOnSignal); else digitalWrite(MDL.RelayPins[i], !MDL.RelayOnSignal);
                }
            }
        }
        break;
    }
}

