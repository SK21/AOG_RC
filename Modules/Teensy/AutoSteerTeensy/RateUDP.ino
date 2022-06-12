IPAddress RateSendIP(192, 168, PCB.IPpart3, 255);

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
	//11 crc

	// PGN 32613, 12 bytes
	Packet[0] = 101;
	Packet[1] = 127;
	Packet[2] = BuildModSenID(PCB.ModuleID, 0);

	// rate applied, 10 X actual
	uint8_t Temp = (int)(UPM[0] * 10);
	Packet[3] = Temp;
	Temp = (int)(UPM[0] * 10) >> 8;
	Packet[4] = Temp;
	Temp = (int)(UPM[0] * 10) >> 16;
	Packet[5] = Temp;

	// accumulated quantity, 10 X actual
	uint16_t Units = TotalPulses[0] * 10.0 / MeterCal[0];
	Temp = Units;
	Packet[6] = Temp;
	Temp = Units >> 8;
	Packet[7] = Temp;
	Temp = Units >> 16;
	Packet[8] = Temp;

	//pwmSetting
	Temp = (byte)(RatePWM[0] * 10);
	Packet[9] = Temp;
	Temp = (byte)(RatePWM[0] * 10 >> 8);
	Packet[10] = Temp;

	// crc
	Packet[11] = CRC(11, 0);

	// send to RateController
	UDPrate.beginPacket(RateSendIP, DestinationPortRate);
	UDPrate.write(Packet, 12);
	UDPrate.endPacket();
}

void ReceiveRateUDP()
{
	uint16_t len = UDPrate.parsePacket();
	if (len > 1)
	{
		UDPrate.read(Packet, UDP_TX_PACKET_MAX_SIZE);
		PGN = Packet[1] << 8 | Packet[0];

		if (PGN == 32614 && len > 13)
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
			//- bit 3		    MasterOn
			//- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
			//- bit 5           AutoOn
			//11    power relay Lo      list of power type relays 0-7
			//12    power relay Hi      list of power type relays 8-15
			//13	crc

			if (GoodCRC(14))
			{
				uint8_t tmp = Packet[2];
				if (ParseModID(tmp) == PCB.ModuleID)
				{
					byte SensorID = ParseSenID(tmp);
					if (SensorID == 0)
					{
						RelayLo = Packet[3];
						RelayHi = Packet[4];

						// rate setting, 10 times actual
						uint16_t tmp = Packet[5] | Packet[6] << 8 | Packet[7] << 16;
						float TmpSet = (float)tmp * 0.1;

						// Meter Cal, 100 times actual
						tmp = Packet[8] | Packet[9] << 8;
						MeterCal[SensorID] = (float)tmp * 0.01;

						// command byte
						InCommand = Packet[10];
						if ((InCommand & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

						ControlType[SensorID] = 0;
						if ((InCommand & 2) == 2) ControlType[SensorID] += 1;
						if ((InCommand & 4) == 4) ControlType[SensorID] += 2;

						MasterOn = ((InCommand & 8) == 8);
						UseMultiPulses[SensorID] = ((InCommand & 16) == 16);

						AutoOn = ((InCommand & 32) == 32);
						if (AutoOn)
						{
							RateSetting[SensorID] = TmpSet;
						}
						else
						{
							ManualAdjust[SensorID] = TmpSet;
						}

						// power relays
						PowerRelayLo = Packet[11];
						PowerRelayHi = Packet[12];

						RateCommTime[SensorID] = millis();
					}
				}
			}
		}
		else if (PGN == 32616 && len > 10)
		{
			// PID to Arduino from RateController

			if (GoodCRC(11))
			{
				uint8_t tmp = Packet[2];
				if (ParseModID(tmp) == PCB.ModuleID)
				{
					byte SensorID = ParseSenID(tmp);
					if (SensorID == 0)
					{
						PIDkp[SensorID] = Packet[3];
						PIDminPWM[SensorID] = Packet[4];
						PIDLowMax[SensorID] = Packet[5];
						PIDHighMax[SensorID] = Packet[6];
						PIDdeadband[SensorID] = Packet[7];
						PIDbrakePoint[SensorID] = Packet[8];
						AdjustTime[SensorID] = Packet[9];
					}
				}
			}
		}
	}
}
