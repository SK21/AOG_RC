
const double KiMultiplier = 10;

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* Data, uint16_t len)
{
	byte PGNlength;
	uint16_t PGN = Data[1] << 8 | Data[0];

	switch (PGN)
	{
	case 32500:
		//PGN32500, Rate settings from RC to module
		//0	    HeaderLo		    244
		//1	    HeaderHi		    126
		//2     Mod/Sen ID          0-15/0-15
		//3	    rate set Lo		    1000 X actual
		//4     rate set Mid
		//5	    rate set Hi
		//6	    Flow Cal Lo	        1000 X actual
		//7     Flow Cal Mid
		//8     Flow Cal Hi
		//9	    Command
		//	        - bit 0		    reset acc.Quantity
		//	        - bit 1,2,3		control type 0-4
		//	        - bit 4		    MasterSwitchOn
		//          - bit 5         -
		//          - bit 6         AutoOn
		//          - bit 7         -
		//10    manual pwm Lo
		//11    manual pwm Hi
		//12    -
		//13    CRC

		PGNlength = 14;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				if (ParseModID(Data[2]) == MDL.ID)
				{
					byte SensorID = ParseSenID(Data[2]);
					if (SensorID < MDL.SensorCount)
					{
						// rate setting, 1000 times actual
						uint32_t RateSet = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16;
						Sensor[SensorID].TargetUPM = (float)(RateSet * 0.001);

						// Meter Cal, 1000 times actual
						uint32_t Temp = Data[6] | (uint32_t)Data[7] << 8 | (uint32_t)Data[8] << 16;
						Sensor[SensorID].MeterCal = Temp * 0.001;

						// command byte
						byte InCommand = Data[9];
						if ((InCommand & 1) == 1) Sensor[SensorID].TotalPulses = 0;	// reset accumulated count

						Sensor[SensorID].ControlType = 0;
						if ((InCommand & 2) == 2) Sensor[SensorID].ControlType += 1;
						if ((InCommand & 4) == 4) Sensor[SensorID].ControlType += 2;
						if ((InCommand & 8) == 8) Sensor[SensorID].ControlType += 4;

						MasterSwitchOn = ((InCommand & 16) == 16);

						AutoOn = ((InCommand & 64) == 64);

						int16_t tmp = Data[10] | Data[11] << 8;
						Sensor[SensorID].ManualAdjust = tmp;

						Sensor[SensorID].CommTime = millis();
					}
				}
			}
		}
		break;

	case 32501:
		//PGN32501, Relay settings from RC to module
		//0	    HeaderLo		    245
		//1	    HeaderHi		    126
		//2     Module ID
		//3	    relay Lo		    0-7
		//4 	relay Hi		    8-15
		//5     power relay Lo      list of power type relays 0-7
		//6     power relay Hi      list of power type relays 8-15
		//7     Inverted Lo         
		//8     Inverted Hi
		//9     CRC

		PGNlength = 10;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				if (ParseModID(Data[2]) == MDL.ID)
				{
					RelayLo = Data[3];
					RelayHi = Data[4];
					PowerRelayLo = Data[5];
					PowerRelayHi = Data[6];
					InvertedLo = Data[7];
					InvertedHi = Data[8];
				}
			}
		}
		break;

	case 32502:
		// PGN32502, Control settings from RC to module
		// 0   246
		// 1   126
		// 2   Mod/Sen ID     0-15/0-15
		// 3   Ki
		// 4   -
		// 5   -
		// 6   MinAdjust
		// 7   MaxAdjust
		// 8   Kp
		// 9   CRC

		PGNlength = 10;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				if (ParseModID(Data[2]) == MDL.ID)
				{
					byte SensorID = ParseSenID(Data[2]);
					if (SensorID < MDL.SensorCount)
					{
						if (Data[3] > 0)
						{
							Sensor[SensorID].Ki = pow(1.06, Data[3] - 120);
						}
						else
						{
							Sensor[SensorID].Ki = 0;
						}

						Sensor[SensorID].MinPower = (double)(255.0 * Data[6] / 100.0);
						Sensor[SensorID].MaxPower = (double)(255.0 * Data[7] / 100.0);

						// 1.1 ^ (gain scroll bar value - 120) gives a scale range of 0.00001 to 0.1486
						Sensor[SensorID].Kp = pow(1.1, Data[8] - 120);
					}
				}
			}
		}
		break;

	case 32503:
		//PGN32503, Subnet change
		//0     HeaderLo    247
		//1     HeaderHI    126
		//2     IP 0
		//3     IP 1
		//4     IP 2
		//5     CRC

		PGNlength = 6;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				MDL.IP0 = Data[2];
				MDL.IP1 = Data[3];
				MDL.IP2 = Data[4];

				SaveData();

				// restart
				resetFunc();
			}
		}
		break;

	case 32700:
		// module config
		//0     HeaderLo    188
		//1     HeaderHi    127
		//2     Module ID   0-15
		//3	    sensor count
		//4     commands
		//      bit 0 - Relay on high
		//      bit 1 - Flow on high
		//      bit 2 - client mode
		//      bit 3 - work pin is momentary
		//      bit 4 - Is3Wire valve
		//5	    relay control type   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
		//                           , 5 - PCA9685, 6 - PCF8574
		//6	    wifi module serial port
		//7	    Sensor 0, Flow pin
		//8     Sensor 0, Dir pin
		//9     Sensor 0, PWM pin
		//10    Sensor 1, Flow pin
		//11    Sensor 1, Dir pin
		//12    Sensor 1, PWM pin
		//13    Relay pins 0-15, bytes 13-28
		//29    work pin
		//30    pressure pin
		//31    -
		//32    CRC

		PGNlength = 33;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(Data, PGNlength))
			{
				MDL.ID = Data[2];
				MDL.SensorCount = Data[3];

				byte tmp = Data[4];
				MDL.InvertRelay = ((tmp & 1) == 1);
				MDL.InvertFlow = ((tmp & 2) == 2);
				MDL.WorkPinIsMomentary = ((tmp & 8) == 8);
				MDL.Is3Wire = ((tmp & 16) == 16);
				MDL.ADS1115Enabled = ((tmp & 32) == 32);

				MDL.RelayControl = Data[5];
				Sensor[0].FlowPin = Data[7];
				Sensor[0].DirPin = Data[8];
				Sensor[0].PWMPin = Data[9];
				Sensor[1].FlowPin = Data[10];
				Sensor[1].DirPin = Data[11];
				Sensor[1].PWMPin = Data[12];

				for (int i = 0; i < 16; i++)
				{
					MDL.RelayControlPins[i] = Data[13 + i];
				}

				MDL.WorkPin = Data[29];
				MDL.PressurePin = Data[30];

				SaveData();

				// restart
				resetFunc();
			}
		}
		break;
	}
}

