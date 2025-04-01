
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

    if ((millis() - Sensor[0].CommTime < 4000) || (millis() - Sensor[1].CommTime < 4000))
    {
        NewLo = RelayLo;
        NewHi = RelayHi;
    }
    else
    {
        // connection lost, enable power and inverted relays
        // for valves that require power to close
        NewLo = PowerRelayLo | InvertedLo;
        NewHi = PowerRelayHi | InvertedHi;
    }

    switch (MDL.RelayControl)
    {
    case 1:
        // GPIOs
        for (int j = 0; j < 2; j++)
        {
            if (j < 1) Rlys = NewLo; else Rlys = NewHi;
            for (int i = 0; i < 8; i++)
            {
                if (MDL.RelayControlPins[i + j * 8] < NC) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(MDL.RelayControlPins[i + j * 8], MDL.InvertRelay); else digitalWrite(MDL.RelayControlPins[i + j * 8], !MDL.InvertRelay);
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
                        // IsOn
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
                        // IsOn
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
        // MCP23017 control pins, RC5, RC8	{ 8,9,10,11,12,13,14,15,7,6,5,4,3,2,1,0 }
        // MCP23017 control pins, RC12-3	{ 0,15,1,14,2,13,3,12,4,11,5,10,6,9,7,8 }

        if (MCP23017_found)
        {
            uint8_t mcpOutA = 0; 
            uint8_t mcpOutB = 0;
            uint8_t Relay;
			uint8_t RelayBanks[] = { NewLo, NewHi };

            for (int bit = 0; bit < 8; bit++)
            {
                for (int bank = 0; bank < 2; bank++)
                {
                    Relay = bit + bank * 8;
                    if ((RelayBanks[bank] & (1 << bit)) == (1 << bit))
                    {
                        if (MDL.RelayControlPins[Relay] < 8)
                        {
                            mcpOutA |= (1 << MDL.RelayControlPins[Relay]);
                        }
                        else
                        {
                            mcpOutB |= (1 << (MDL.RelayControlPins[Relay] - 8));
                        }
                    }
                }
            }

            if (MDL.InvertRelay)
            {
                mcpOutA = (uint8_t)~mcpOutA;
                mcpOutB = (uint8_t)~mcpOutB;
            }

            // Now send the output bytes.
            Wire.beginTransmission(MCP23017address);
            Wire.write(0x12);         // Starting register address (GPIOA)
            Wire.write(mcpOutA);      // GPA value
            Wire.write(mcpOutB);      // GPB value
            Wire.endTransmission();
        }
        break;
    }
}

