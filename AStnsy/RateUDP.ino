#if(UseRateControl)
void SendRateUDP()
{
	//PGN32613 to Rate Controller from Arduino
	//0	HeaderLo		101
	//1	HeaderHi		127
	//2 Controller ID
	//3	rate applied Lo 	10 X actual
	//4 rate applied Mid
	//5	rate applied Hi
	//6	acc.Quantity Lo		10 X actual
	//7	acc.Quantity Mid
	//8	acc.Quantity Hi
	//9 PWM Lo
	//10 PWM Hi

	// rate applied, 10 X actual
	uint8_t Temp = (int)(UPM[0] * 10);
	RateSend[3] = Temp;
	Temp = (int)(UPM[0] * 10) >> 8;
	RateSend[4] = Temp;
	Temp = (int)(UPM[0] * 10) >> 16;
	RateSend[5] = Temp;

	// accumulated quantity, 10 X actual
	uint16_t Units = TotalPulses[0] * 10.0 / MeterCal[0];
	Temp = Units;
	RateSend[6] = Temp;
	Temp = Units >> 8;
	RateSend[7] = Temp;
	Temp = Units >> 16;
	RateSend[8] = Temp;

	//pwmSetting
	Temp = (byte)(RatePWM[0] * 10);
	RateSend[9] = Temp;
	Temp = (byte)(RatePWM[0] * 10 >> 8);
	RateSend[10] = Temp;

	// send to RateController
	UDPrate.beginPacket(AGIOip, DestinationPortRate);
	UDPrate.write(RateSend, sizeof(RateSend));
	UDPrate.endPacket();
}

void ReceiveRateUDP()
{
	//PGN32614 to Arduino from Rate Controller
	//0	HeaderLo		102
	//1	HeaderHi		127
	//2 Controller ID
	//3	relay Lo		0 - 7
	//4	relay Hi		8 - 15
	//5	rate set Lo		10 X actual
	//6 rate set Mid
	//7	rate set Hi		
	//8	Flow Cal Lo		100 X actual
	//9	Flow Cal Hi		
	//10	Command
	//- bit 0		    reset acc.Quantity
	//- bit 1, 2		valve type 0 - 3
	//- bit 3		    -
	//- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
	//- bit 5           AutoOn

	uint16_t len = UDPrate.parsePacket();
	if (len > 1)
	{
		UDPrate.read(RateData, UDP_TX_PACKET_MAX_SIZE);
		PGN = RateData[1] << 8 | RateData[0];

		if (PGN == 32614 && len > 10)
		{
			// settings
			uint8_t tmp = RateData[2];

			if (ParseModID(tmp) == ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID == 0)
				{
					RelayLo = RateData[3];
					RelayHi = RateData[4];

					// rate setting, 10 times actual
					uint16_t tmp = RateData[5] | RateData[6] << 8 | RateData[7] << 16;
					float TmpSet = (float)tmp * 0.1;

					// Meter Cal, 100 times actual
					tmp = RateData[8] | RateData[9] << 8;
					MeterCal[SensorID] = (float)tmp * 0.01;

					// command byte
					InCommand = RateData[10];
					if ((InCommand & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

					ControlType[SensorID] = 0;
					if ((InCommand & 2) == 2) ControlType[SensorID] += 1;
					if ((InCommand & 4) == 4) ControlType[SensorID] += 2;

					UseMultiPulses[SensorID] = ((InCommand & 16) == 16);

					AutoOn = ((InCommand & 32) == 32);
					if (AutoOn)
					{
						RateSetting[SensorID] = TmpSet;
					}
					else
					{
						NewRateFactor[SensorID] = TmpSet;
					}
					RateCommTime[SensorID] = millis();
				}
			}
		}
		else if (PGN == 32616 && len > 9)
		{
			// PID to Arduino from RateController
			uint8_t tmp = RateData[2];
			if (ParseModID(tmp) == ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID == 0)
				{
					PIDkp[SensorID] = RateData[3];
					PIDminPWM[SensorID] = RateData[4];
					PIDLowMax[SensorID] = RateData[5];
					PIDHighMax[SensorID] = RateData[6];
					PIDdeadband[SensorID] = RateData[7];
					PIDbrakePoint[SensorID] = RateData[8];
					AdjustTime[SensorID] = RateData[9];
				}
			}
		}
		else if (PGN == 32620 && len > 9)
		{
			// section switch IDs to arduino
			// 0    108
			// 1    127
			// 2    sec 0-1
			// 3    sec 2-3
			// 4    sec 4-5
			// 5    sec 6-7
			// 6    sec 8-9
			// 7    sec 10-11
			// 8    sec 12-13
			// 9    sec 14-15
			for (int i = 0; i < 8; i++)
			{
				SwitchBytes[i] = RateData[i + 2];
			}

			// Translate Switch IDs from Rate Controller
			// ex: byte 2: bits 0-3 identify switch # (0-15) for sec 0
			// ex: byte 2: bits 4-7 identify switch # (0-15) for sec 1

			for (int i = 0; i < 16; i++)
			{
				byte ByteID = i / 2;
				byte Mask = 15 << (4 * (i - 2 * ByteID));    // move mask to correct bits
				SectionSwitchID[i] = SwitchBytes[ByteID] & Mask;    // mask out bits
				SectionSwitchID[i] = SectionSwitchID[i] >> (4 * (i - 2 * ByteID)); // move bits for number
			}
		}
	}
}
#endif

