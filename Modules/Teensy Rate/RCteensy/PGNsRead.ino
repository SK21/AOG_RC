
void ReadMessages(uint8_t PGN, uint8_t SensorID, uint8_t* Data)
{
	switch (PGN)
	{
	case 4:
		// subnet change
		// 0	IP 0
		// 1	IP 1
		// 2	IP 2

		MDL.IP0 = Data[2];
		MDL.IP1 = Data[3];
		MDL.IP2 = Data[4];

		SaveData();

		// restart the Teensy
		SCB_AIRCR = 0x05FA0004;
		break;

	case 6:
		// Rate settings 
		// 0	rate set lo		1000 X actual
		// 1	rate set mid
		// 2	rate set hi
		// 3	flow cal lo		100 X actual
		// 4	flow cal mid
		// 5	flow cal hi
		// 6	manual PWM
		// 7	commands
		//			- bit 0		reset accumulated quantity
		//			- bit 1,2,3	control type 0-4
		//			- bit 4		master is on
		//			- bit 5		-
		//			- bit 6		auto is on

		if (SensorID < MDL.SensorCount)
		{
			// rate setting, 1000 times actual
			uint32_t RateSet = Data[0] | (uint32_t)Data[1] << 8 | (uint32_t)Data[2] << 16;
			Sensor[SensorID].TargetUPM = (float)(RateSet * 0.001);

			// Meter Cal, 100 times actual
			uint32_t Temp = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16;
			Sensor[SensorID].MeterCal = Temp * 0.01;

			// manual pwm
			Sensor[SensorID].ManualAdjust = Data[6];

			// command byte
			byte InCommand = Data[7];
			if ((InCommand & 1) == 1) Sensor[SensorID].TotalPulses = 0;	// reset accumulated count

			Sensor[SensorID].ControlType = 0;
			if ((InCommand & 2) == 2) Sensor[SensorID].ControlType += 1;
			if ((InCommand & 4) == 4) Sensor[SensorID].ControlType += 2;
			if ((InCommand & 8) == 8) Sensor[SensorID].ControlType += 4;

			MasterOn = ((InCommand & 16) == 16);

			AutoOn = ((InCommand & 64) == 64);

			Sensor[SensorID].CommTime = millis();
		}
		break;

	case 7:
		// Relay settings
		// 0	relay lo		0-7
		// 1	relay hi		8-15
		// 2	power relay lo	power type relay 0-7
		// 3	power relay hi  power type relay 8-15
		// 4	inverted lo		inverted type relay 0-7
		// 5	inverted hi		inverted type relay 8-15

		RelayLo = Data[0];
		RelayHi = Data[1];
		PowerRelayLo = Data[2];
		PowerRelayHi = Data[3];
		InvertedLo = Data[4];
		InvertedHi = Data[5];
		break;

	case 8:
		// Control settings
		// 0	High adjust
		// 1	Low adjust
		// 2	Threshold
		// 3	Minimum
		// 4	Maximum
		// 5	Scale Factor

		if (SensorID < MDL.SensorCount)
		{
			Sensor[SensorID].HighAdjust = (double)(255.0 * Data[0] / 100.0);
			Sensor[SensorID].LowAdjust = (double)(255.0 * Data[1] / 100.0);
			Sensor[SensorID].AdjustThreshold = (double)(255.0 * Data[2] / 100.0);
			Sensor[SensorID].MinPower = (double)(255.0 * Data[3] / 100.0);
			Sensor[SensorID].MaxPower = (double)(255.0 * Data[4] / 100.0);

			// 1.15 ^ ((100 - Scaling scroll bar value)* -1 + 3). 3 changes the max range of the scaling.
			// 1.17 ^ -100 is approx equivalent to 1/1,000,000
			Sensor[SensorID].Scaling = pow(1.15, (100 - Data[5]) * -1 + 3);
		}
		break;

	case 15:
		// module config 1
		// 0	new module ID
		// 1	sensor count
		// 2	relay control type 0-6
		// 3	wifi serial port
		// 4	work pin
		// 5	pressure pin
		// 6	commands

		MDL.ID = Data[0];
		MDL.SensorCount = Data[1];
		MDL.RelayControl = Data[2];
		MDL.WorkPin = Data[4];
		MDL.PressurePin = Data[5];

		MDL.InvertRelay = ((Data[6] & 1) == 1);
		MDL.InvertFlow = ((Data[6] & 2) == 2);
		MDL.WifiModeUseStation = ((Data[6] & 4) == 4);
		MDL.WorkPinIsMomentary = ((Data[6] & 8) == 8);
		MDL.Is3Wire = ((Data[6] & 16) == 16);
		MDL.ADS1115Enabled = ((Data[6] & 32) == 32);
		break;

	case 16:
		// module config 2
		// 0	sensor 0, flow pin
		// 1	sensor 0, dir pin
		// 2	sensor 0, pwm pin
		// 3	sensor 1, flow pin
		// 4	sensor 1, dir pin
		// 5	sensor 1, pwm pin

		Sensor[0].FlowPin = Data[0];
		Sensor[0].DirPin = Data[1];
		Sensor[0].PWMPin = Data[2];
		Sensor[1].FlowPin = Data[3];
		Sensor[1].DirPin = Data[4];
		Sensor[1].PWMPin = Data[5];
		break;

	case 17:
		// module config 3
		// 0	relay 0 pin
		// 1	relay 1 pin
		// 2	relay 2 pin
		// 3	relay 3 pin
		// 4	relay 4 pin
		// 5	relay 5 pin
		// 6	relay 6 pin
		// 7	relay 7 pin

		for (int i = 0; i < 8; i++)
		{
			MDL.RelayPins[i] = Data[i];
		}
		break;

	case 18:
		// module config 4
		// 0	relay 8 pin
		// 1	relay 9 pin
		// 2	relay 10 pin
		// 3	relay 11 pin
		// 4	relay 12 pin
		// 5	relay 13 pin
		// 6	relay 14 pin
		// 7	relay 15 pin

		for (int i = 0; i < 8; i++)
		{
			MDL.RelayPins[i + 8] = Data[i];
		}

		SaveData();	// saves changes from PGNs 15 to 18

		//restart the Teensy
		SCB_AIRCR = 0x05FA0004;
		break;

	}
}
