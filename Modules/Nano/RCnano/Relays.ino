
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
		if (FlowEnabled[0] || FlowEnabled[1])
		{
			SetRelays(RelayLo, RelayHi);
		}
		else
		{
			SetRelays(0, 0);
		}
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
#if(UseMCP23017 == 1)
	if (bitRead(LoByte, 0)) mcp.digitalWrite(Relay1, RelayOn); else mcp.digitalWrite(Relay1, !RelayOn);
	if (bitRead(LoByte, 1)) mcp.digitalWrite(Relay2, RelayOn); else mcp.digitalWrite(Relay2, !RelayOn);
	if (bitRead(LoByte, 2)) mcp.digitalWrite(Relay3, RelayOn); else mcp.digitalWrite(Relay3, !RelayOn);
	if (bitRead(LoByte, 3)) mcp.digitalWrite(Relay4, RelayOn); else mcp.digitalWrite(Relay4, !RelayOn);
	if (bitRead(LoByte, 4)) mcp.digitalWrite(Relay5, RelayOn); else mcp.digitalWrite(Relay5, !RelayOn);
	if (bitRead(LoByte, 5)) mcp.digitalWrite(Relay6, RelayOn); else mcp.digitalWrite(Relay6, !RelayOn);
	if (bitRead(LoByte, 6)) mcp.digitalWrite(Relay7, RelayOn); else mcp.digitalWrite(Relay7, !RelayOn);

#if(UseSwitchedPowerPin == 0)
	if (bitRead(LoByte, 7)) mcp.digitalWrite(Relay8, RelayOn); else mcp.digitalWrite(Relay8, !RelayOn);
#endif

	if (bitRead(HiByte, 0)) mcp.digitalWrite(Relay9, RelayOn); else mcp.digitalWrite(Relay9, !RelayOn);
	if (bitRead(HiByte, 1)) mcp.digitalWrite(Relay10, RelayOn); else mcp.digitalWrite(Relay10, !RelayOn);
	if (bitRead(HiByte, 2)) mcp.digitalWrite(Relay11, RelayOn); else mcp.digitalWrite(Relay11, !RelayOn);
	if (bitRead(HiByte, 3)) mcp.digitalWrite(Relay12, RelayOn); else mcp.digitalWrite(Relay12, !RelayOn);
	if (bitRead(HiByte, 4)) mcp.digitalWrite(Relay13, RelayOn); else mcp.digitalWrite(Relay13, !RelayOn);
	if (bitRead(HiByte, 5)) mcp.digitalWrite(Relay14, RelayOn); else mcp.digitalWrite(Relay14, !RelayOn);
	if (bitRead(HiByte, 6)) mcp.digitalWrite(Relay15, RelayOn); else mcp.digitalWrite(Relay15, !RelayOn);
	if (bitRead(HiByte, 7)) mcp.digitalWrite(Relay16, RelayOn); else mcp.digitalWrite(Relay16, !RelayOn);

#else
	// use Nano pins
	if (bitRead(LoByte, 0)) digitalWrite(Relay1, RelayOn); else digitalWrite(Relay1, !RelayOn);
	if (bitRead(LoByte, 1)) digitalWrite(Relay2, RelayOn); else digitalWrite(Relay2, !RelayOn);
	if (bitRead(LoByte, 2)) digitalWrite(Relay3, RelayOn); else digitalWrite(Relay3, !RelayOn);

#if(UseSwitchedPowerPin == 0)
	if (bitRead(LoByte, 3)) digitalWrite(Relay4, RelayOn); else digitalWrite(Relay4, !RelayOn);
#endif

	//if (bitRead(LoByte, 4)) digitalWrite(Relay5, RelayOn); else digitalWrite(Relay5, !RelayOn);
	//if (bitRead(LoByte, 5)) digitalWrite(Relay6, RelayOn); else digitalWrite(Relay6, !RelayOn);
	//if (bitRead(LoByte, 6)) digitalWrite(Relay7, RelayOn); else digitalWrite(Relay7, !RelayOn);
	//if (bitRead(LoByte, 7)) digitalWrite(Relay8, RelayOn); else digitalWrite(Relay8, !RelayOn);
#endif
}
