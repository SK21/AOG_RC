
void CheckRelays()
{
    if ((millis() - FlowSensor.CommTime > 4000) )
    {
        // connection lost, enable power and inverted relays
        // for valves that require power to close
        RelayLo = PowerRelayLo | InvertedLo;
        RelayHi = PowerRelayHi | InvertedHi;
    }

    uint8_t mcpOutA = 0; // Output for port A
    uint8_t mcpOutB = 0; // Output for port B

    if (MDL.RelaysSingle)
    {
        mcpOutA = RelayLo;
        mcpOutB = RelayHi;
    }
    else
    {
        // Calculate output for port A
        mcpOutA = (bitRead(RelayLo, 0) ? 2 : 1) |
            (bitRead(RelayLo, 1) ? 8 : 4) |
            (bitRead(RelayLo, 2) ? 32 : 16) |
            (bitRead(RelayLo, 3) ? 128 : 64);

        // Calculate output for port B
        mcpOutB = (bitRead(RelayLo, 4) ? 2 : 1) |
            (bitRead(RelayLo, 5) ? 8 : 4) |
            (bitRead(RelayLo, 6) ? 32 : 16) |
            (bitRead(RelayLo, 7) ? 128 : 64);
    }

    // Send both outputs in a single transmission
    Wire.beginTransmission(MCPaddress);
    Wire.write(0x12); // address port A of MCP
    Wire.write(mcpOutA); // value for port A
    Wire.write(mcpOutB); // value for port B
    Wire.endTransmission();
}