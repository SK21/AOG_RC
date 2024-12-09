
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
                if (MDL.RelayPins[i + j * 8] < NC) // check if relay is enabled
                {
                    if (bitRead(Rlys, i)) digitalWrite(MDL.RelayPins[i + j * 8], MDL.InvertRelay); else digitalWrite(MDL.RelayPins[i + j * 8], !MDL.InvertRelay);
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
            uint8_t mcpOutA = 0; // Output for port A
            uint8_t mcpOutB = 0; // Output for port B

            // Calculate output for port A, sections 1-8
            mcpOutA = (bitRead(NewLo, 0) ? 1 : 0) |
                (bitRead(NewLo, 1) ? 2 : 0) |
                (bitRead(NewLo, 2) ? 4 : 0) |
                (bitRead(NewLo, 3) ? 8 : 0) |
                (bitRead(NewLo, 4) ? 16 : 0) |
                (bitRead(NewLo, 5) ? 32 : 0) |
                (bitRead(NewLo, 6) ? 64 : 0) |
                (bitRead(NewLo, 7) ? 128 : 0);

            // Calculate output for port B, sections 9-16
            mcpOutB = (bitRead(NewHi, 0) ? 1 : 0) |
                (bitRead(NewHi, 1) ? 2 : 0) |
                (bitRead(NewHi, 2) ? 4 : 0) |
                (bitRead(NewHi, 3) ? 8 : 0) |
                (bitRead(NewHi, 4) ? 16 : 0) |
                (bitRead(NewHi, 5) ? 32 : 0) |
                (bitRead(NewHi, 6) ? 64 : 0) |
                (bitRead(NewHi, 7) ? 128 : 0);

            if (MDL.InvertRelay)
            {
                mcpOutA = ~mcpOutA;
                mcpOutB = ~mcpOutB;
            }

            // Send both outputs in a single transmission
            Wire.beginTransmission(MCP23017address);
            Wire.write(0x12); // address port A of MCP
            Wire.write(mcpOutA); // value for port A
            Wire.write(mcpOutB); // value for port B
            Wire.endTransmission();
        }
        break;
    }
}

