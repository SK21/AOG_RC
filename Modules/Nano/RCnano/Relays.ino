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

    if (PCB.UseMCP23017 == 1)
    {
        if (IOexpanderFound)
        {
            if (bitRead(NewLo, 0)) mcp.digitalWrite(Relay1, PCB.RelayOnSignal); else mcp.digitalWrite(Relay1, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 1)) mcp.digitalWrite(Relay2, PCB.RelayOnSignal); else mcp.digitalWrite(Relay2, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 2)) mcp.digitalWrite(Relay3, PCB.RelayOnSignal); else mcp.digitalWrite(Relay3, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 3)) mcp.digitalWrite(Relay4, PCB.RelayOnSignal); else mcp.digitalWrite(Relay4, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 4)) mcp.digitalWrite(Relay5, PCB.RelayOnSignal); else mcp.digitalWrite(Relay5, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 5)) mcp.digitalWrite(Relay6, PCB.RelayOnSignal); else mcp.digitalWrite(Relay6, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 6)) mcp.digitalWrite(Relay7, PCB.RelayOnSignal); else mcp.digitalWrite(Relay7, !PCB.RelayOnSignal);
            if (bitRead(NewLo, 7)) mcp.digitalWrite(Relay8, PCB.RelayOnSignal); else mcp.digitalWrite(Relay8, !PCB.RelayOnSignal);

            if (bitRead(NewHi, 0)) mcp.digitalWrite(Relay9, PCB.RelayOnSignal); else mcp.digitalWrite(Relay9, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 1)) mcp.digitalWrite(Relay10, PCB.RelayOnSignal); else mcp.digitalWrite(Relay10, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 2)) mcp.digitalWrite(Relay11, PCB.RelayOnSignal); else mcp.digitalWrite(Relay11, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 3)) mcp.digitalWrite(Relay12, PCB.RelayOnSignal); else mcp.digitalWrite(Relay12, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 4)) mcp.digitalWrite(Relay13, PCB.RelayOnSignal); else mcp.digitalWrite(Relay13, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 5)) mcp.digitalWrite(Relay14, PCB.RelayOnSignal); else mcp.digitalWrite(Relay14, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 6)) mcp.digitalWrite(Relay15, PCB.RelayOnSignal); else mcp.digitalWrite(Relay15, !PCB.RelayOnSignal);
            if (bitRead(NewHi, 7)) mcp.digitalWrite(Relay16, PCB.RelayOnSignal); else mcp.digitalWrite(Relay16, !PCB.RelayOnSignal);
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
                if (PINS.Relays[i] > 1) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(PINS.Relays[i], PCB.RelayOnSignal); else digitalWrite(PINS.Relays[i], !PCB.RelayOnSignal);
                }
            }
        }
    }
}
