byte Rlys;

void CheckRelays()
{
    if (WifiSwitchesEnabled)
    {
        if (millis() - WifiSwitchesTimer > 30000)   // 30 second timer
        {
            // wifi switches have timed out
            WifiSwitchesEnabled = false;
            SetRelays(0, 0);
        }
    }
    else
    {
        SetRelays(RelayLo, RelayHi);
    }
}

void SetRelaysWifi()
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

void SetRelays(byte LoByte, byte HiByte)
{
    if (PCB.UseMCP23017 == 1)
    {
        if (bitRead(LoByte, 0)) mcp.digitalWrite(Relay1, PCB.RelayOnSignal); else mcp.digitalWrite(Relay1, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 1)) mcp.digitalWrite(Relay2, PCB.RelayOnSignal); else mcp.digitalWrite(Relay2, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 2)) mcp.digitalWrite(Relay3, PCB.RelayOnSignal); else mcp.digitalWrite(Relay3, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 3)) mcp.digitalWrite(Relay4, PCB.RelayOnSignal); else mcp.digitalWrite(Relay4, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 4)) mcp.digitalWrite(Relay5, PCB.RelayOnSignal); else mcp.digitalWrite(Relay5, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 5)) mcp.digitalWrite(Relay6, PCB.RelayOnSignal); else mcp.digitalWrite(Relay6, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 6)) mcp.digitalWrite(Relay7, PCB.RelayOnSignal); else mcp.digitalWrite(Relay7, !PCB.RelayOnSignal);
        if (bitRead(LoByte, 7)) mcp.digitalWrite(Relay8, PCB.RelayOnSignal); else mcp.digitalWrite(Relay8, !PCB.RelayOnSignal);

        if (bitRead(HiByte, 0)) mcp.digitalWrite(Relay9, PCB.RelayOnSignal); else mcp.digitalWrite(Relay9, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 1)) mcp.digitalWrite(Relay10, PCB.RelayOnSignal); else mcp.digitalWrite(Relay10, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 2)) mcp.digitalWrite(Relay11, PCB.RelayOnSignal); else mcp.digitalWrite(Relay11, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 3)) mcp.digitalWrite(Relay12, PCB.RelayOnSignal); else mcp.digitalWrite(Relay12, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 4)) mcp.digitalWrite(Relay13, PCB.RelayOnSignal); else mcp.digitalWrite(Relay13, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 5)) mcp.digitalWrite(Relay14, PCB.RelayOnSignal); else mcp.digitalWrite(Relay14, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 6)) mcp.digitalWrite(Relay15, PCB.RelayOnSignal); else mcp.digitalWrite(Relay15, !PCB.RelayOnSignal);
        if (bitRead(HiByte, 7)) mcp.digitalWrite(Relay16, PCB.RelayOnSignal); else mcp.digitalWrite(Relay16, !PCB.RelayOnSignal);
    }
    else
    {
        // use Nano pins
        for (int i = 0; i < 16; i++)
        {
            if (PINS.Relays[i] > 1) // check if relay is enabled
            {
                if (i < 8) Rlys = LoByte; else Rlys = HiByte;
            }
            if (bitRead(Rlys, i)) digitalWrite(PINS.Relays[i], PCB.RelayOnSignal); else digitalWrite(PINS.Relays[i], !PCB.RelayOnSignal);
        }
    }
}
