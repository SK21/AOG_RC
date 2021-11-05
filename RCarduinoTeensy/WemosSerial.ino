void ReceiveWemos()
{
    if (Serial8.available() > 0 && !PGN32619Found)
    {
        LSB = Serial8.read();
        PGN = MSB << 8 | LSB;
        MSB = LSB;
        PGN32619Found = (PGN == 32619);
    }
    if (Serial8.available() > 4 && PGN32619Found)
    {
        PGN32619Found = false;
        for (int16_t i = 0; i < 5; i++)
        {
            WifiSwitches[i] = Serial8.read();
        }
        WifiSwitchesEnabled = true;
        WifiSwitchesTimer = millis();
        SetRelaysWifi();
    }
}

void SetRelaysWifi()
{
    if (WifiSwitches[4])
    {
        // master on
        byte HiByte = 0;
        byte LoByte = 0;
        bool PinState;

        for (int i = 0; i < 16; i++)
        {
            switch (SwitchID[i])
            {
            case 0:
                PinState = WifiSwitches[0];
                break;
            case 1:
                PinState = WifiSwitches[1];
                break;
            case 2:
                PinState = WifiSwitches[2];
                break;
            case 3:
                PinState = WifiSwitches[3];
                break;
            default:
                PinState = false;
                break;
            }
            if (i < 8)
            {
                // LoByte
                if (PinState) bitSet(LoByte, i);
            }
            else
            {
                // HiByte
                if (PinState) bitSet(HiByte, i - 8);
            }
        }

        SetRelays(HiByte, LoByte);
    }
    else
    {
        // master off
        SetRelays(0, 0);
        WifiSwitchesEnabled = false;
    }
}


