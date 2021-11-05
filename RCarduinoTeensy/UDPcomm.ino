void SendUDP()
{
	// PGN 32613
	for (int i = 0; i < SensorCount; i++)
	{
        // header
        RateSend[i][0] = 127;
        RateSend[i][1] = 101;

        RateSend[i][2] = BuildModSenID(ModuleID, i);

        // rate applied, 10 X actual
        int Temp = (int)(UPM[i] * 10) >> 16;
        RateSend[i][3] = Temp;
		Temp = (int)(UPM[i] * 10) >> 8;
        RateSend[i][4] = Temp;
		Temp = (UPM[i] * 10);
		RateSend[i][5] = Temp;

        // accumulated quantity, 3 bytes
        long Units = TotalPulses[i] * 100.0 / MeterCal[i];
        Temp = Units >> 16;
        RateSend[i][6] = Temp;
        Temp = Units >> 8;
        RateSend[i][7] = Temp;
        Temp = Units;
        RateSend[i][8] = Temp;

        //pwmSetting
        Temp = (byte)((pwmSetting[i] * 10) >> 8);
        RateSend[i][9] = Temp;

        Temp = (byte)(pwmSetting[i] * 10);
        RateSend[i][10] = Temp;

        // send to RateController
        UDPmain.beginPacket(DestinationIP, DestinationPort);
        UDPmain.write(RateSend[i], sizeof(RateSend[i]));
        UDPmain.endPacket();
    }

	// PGN 32621, pressures
	for (int i = 0; i < 4; i++)
	{
		PressureSend[i * 2 + 3] = adc[i] >> 8;
		PressureSend[i * 2 + 4] = adc[i];
	}

	for (int i = 0; i < 10; i++)
	{
		// send to RateController
		UDPmain.beginPacket(DestinationIP, DestinationPort);
		UDPmain.write(PressureSend, sizeof(PressureSend));
		UDPmain.endPacket();
	}

}

void ReceiveUDP()
{
	uint16_t len = UDPmain.parsePacket();
	if (len > 1)
	{
		UDPmain.read(RateData, UDP_TX_PACKET_MAX_SIZE);
		PGN = RateData[0] << 8 | RateData[1];

		if (PGN == 32614 && len > 9)
		{
			// settings
			uint8_t tmp = RateData[2];

			if (ParseModID(tmp) == ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID < SensorCount)
				{
					RelayHi = RateData[3];
					RelayLo = RateData[4];

					// rate setting, 10 times actual
					uint16_t tmp = RateData[5] << 8 | RateData[6];
					float TmpSet = (float)tmp * 0.1;

					// Meter Cal, 100 times actual
					tmp = RateData[7] << 8 | RateData[8];
					MeterCal[SensorID] = (float)tmp * 0.01;

					// command byte
					InCommand[SensorID] = RateData[9];
					if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

					ControlType[SensorID] = 0;
					if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
					if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;

					SimulateFlow[SensorID] = ((InCommand[SensorID] & 8) == 8);

					UseMultiPulses[SensorID] = ((InCommand[SensorID] & 16) == 16);

					AutoOn = ((InCommand[SensorID] & 32) == 32);
					if (AutoOn)
					{
						RateSetting[SensorID] = TmpSet;
					}
					else
					{
						NewRateFactor[SensorID] = TmpSet;
					}
					CommTime[SensorID] = millis();
				}
			}
		}
		else if (PGN == 32616 && len > 8)
		{
			// PID
			uint8_t tmp = RateData[2];
			if (ParseModID(tmp) == ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID < SensorCount)
				{
					PIDkp[SensorID] = RateData[3];
					PIDminPWM[SensorID] = RateData[4];
					PIDLowMax[SensorID] = RateData[5];
					PIDHighMax[SensorID] = RateData[6];
					PIDdeadband[SensorID] = RateData[7];
					PIDbrakePoint[SensorID] = RateData[8];
					AdjustTime[SensorID] = RateData[9];

					CommTime[SensorID] = millis();
				}
			}
		}
		else if (PGN == 32620 && len > 3)
		{
			// section switch IDs to arduino
			// 0    127
			// 1    108
			// 2    sec 0-3
			// 3    sec 4-7
			// 4    sec 8-11
			// 5    sec 12-15
			for (int i = 0; i < 4; i++)
			{
				SwitchBytes[i] = RateData[i + 2];
			}
			TranslateSwitchBytes();
		}
	}
}

void TranslateSwitchBytes()
{
	// Switch IDs from Rate Controller
	// byte 0, bits 0,1 = Switch 0 ID, bits 2,3 = Switch 1 ID
	// byte 0, bits 4,5 = Switch 2 ID, bits 6,7 = Switch 3 ID
	// bytes 1-3 for switches 4-15

	for (int i = 0; i < 16; i++)
	{
		byte Count = i / 4;
		byte Mask = 3 << (2 * (i - 4 * Count));
		SwitchID[i] = SwitchBytes[Count] & Mask;
		SwitchID[i] = SwitchID[i] >> (2 * (i - 4 * Count));
	}
}

byte ParseModID(byte ID)
{
	// top 4 bits
	return ID >> 4;
}

byte ParseSenID(byte ID)
{
	// bottom 4 bits
	return (ID & 0b00001111);
}

byte BuildModSenID(byte Mod_ID, byte Sen_ID)
{
	return ((Mod_ID << 4) | (Sen_ID & 0b00001111));
}

