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
    else if (FlowEnabled[0] || FlowEnabled[1])
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

    if (MDL.UseMCP23017 == 1)
    {
        if (IOexpanderFound)
        {
            if (bitRead(NewLo, 0)) mcp.digitalWrite(Relay1, MDL.RelayOnSignal); else mcp.digitalWrite(Relay1, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 1)) mcp.digitalWrite(Relay2, MDL.RelayOnSignal); else mcp.digitalWrite(Relay2, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 2)) mcp.digitalWrite(Relay3, MDL.RelayOnSignal); else mcp.digitalWrite(Relay3, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 3)) mcp.digitalWrite(Relay4, MDL.RelayOnSignal); else mcp.digitalWrite(Relay4, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 4)) mcp.digitalWrite(Relay5, MDL.RelayOnSignal); else mcp.digitalWrite(Relay5, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 5)) mcp.digitalWrite(Relay6, MDL.RelayOnSignal); else mcp.digitalWrite(Relay6, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 6)) mcp.digitalWrite(Relay7, MDL.RelayOnSignal); else mcp.digitalWrite(Relay7, !MDL.RelayOnSignal);
            if (bitRead(NewLo, 7)) mcp.digitalWrite(Relay8, MDL.RelayOnSignal); else mcp.digitalWrite(Relay8, !MDL.RelayOnSignal);

            if (bitRead(NewHi, 0)) mcp.digitalWrite(Relay9, MDL.RelayOnSignal); else mcp.digitalWrite(Relay9, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 1)) mcp.digitalWrite(Relay10, MDL.RelayOnSignal); else mcp.digitalWrite(Relay10, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 2)) mcp.digitalWrite(Relay11, MDL.RelayOnSignal); else mcp.digitalWrite(Relay11, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 3)) mcp.digitalWrite(Relay12, MDL.RelayOnSignal); else mcp.digitalWrite(Relay12, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 4)) mcp.digitalWrite(Relay13, MDL.RelayOnSignal); else mcp.digitalWrite(Relay13, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 5)) mcp.digitalWrite(Relay14, MDL.RelayOnSignal); else mcp.digitalWrite(Relay14, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 6)) mcp.digitalWrite(Relay15, MDL.RelayOnSignal); else mcp.digitalWrite(Relay15, !MDL.RelayOnSignal);
            if (bitRead(NewHi, 7)) mcp.digitalWrite(Relay16, MDL.RelayOnSignal); else mcp.digitalWrite(Relay16, !MDL.RelayOnSignal);
        }
    }
    else
    {
        // use Nano pins
        for (int j = 0; j < 2; j++)
        {
            if (j < 1) Rlys = NewLo; else Rlys = NewHi;
            for (int i = 0; i < 8; i++)
            {
                if (MDL.Relays[i] > 1) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(MDL.Relays[i], MDL.RelayOnSignal); else digitalWrite(MDL.Relays[i], !MDL.RelayOnSignal);
                }
            }
        }
    }
}
