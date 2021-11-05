uint16_t RelayState;
bool BitState;

void SetRelays(byte HiByte, byte LoByte)
{
	for (int i = 0; i < 16; i++)
	{
		if (i == SwitchedPowerRelay)
		{
			BitState = HIGH;
		}
		else if (i < 8)
		{
			BitState = bitRead(LoByte, i);
		}
		else
		{
			BitState = bitRead(HiByte, i - 8);
		}

		if (BitState)
		{
			RelayState = 0x0100;	// on
		}
		else
		{
			RelayState = 0x0200;	// off
		}

		// relay ID's are 1-16
		ModbusRTUClient.holdingRegisterWrite(SlaveID, i + 1, RelayState);
		//delay(100);
	}
}
