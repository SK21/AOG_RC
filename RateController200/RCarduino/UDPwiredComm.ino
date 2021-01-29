#if(CommType == 1)
void SendUDPwired()
{
	// PGN 32613
	// header
	toSend[0] = 127;
	toSend[1] = 101;

	toSend[2] = ControllerID;

	// rate applied, 100 X actual
	Temp = (int)(FlowRate * 100) >> 8;
	toSend[3] = Temp;
	Temp = (FlowRate * 100);
	toSend[4] = Temp;

	// accumulated quantity, 3 bytes
	long Units = accumulatedCounts * 100.0 / MeterCal;
	Temp = Units >> 16;
	toSend[5] = Temp;
	Temp = Units >> 8;
	toSend[6] = Temp;
	Temp = Units;
	toSend[7] = Temp;

	//pwmSetting
	Temp = (byte)((pwmSetting * 10) >> 8);
	toSend[8] = Temp;

	Temp = (byte)(pwmSetting * 10);
	toSend[9] = Temp;

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);


	// PGN 32761
	// header
	toSend[0] = 127;
	toSend[1] = 249;

	toSend[2] = 0;
	toSend[3] = 0;
	toSend[4] = 0;

	// relay Hi
	toSend[5] = 0;

	// relay Lo
	toSend[6] = RelayToAOG;

	// SecSwOff Hi
	toSend[7] = SecSwOff[1];

	// SecSwOff Lo
	toSend[8] = SecSwOff[0];

	// command byte
	toSend[9] = OutCommand;

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);
}
void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, byte* data, uint16_t len)

//void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
{
	PGN = data[0] << 8 | data[1];

	if (len > 9)
	{
		if (PGN == 32614)
		{
			byte ConID = data[2];
			if (ConID == ControllerID)
			{
				RelayHi = data[3];
				RelayFromAOG = data[4];

				// rate setting, 100 times actual
				UnSignedTemp = data[5] << 8 | data[6];
				rateSetPoint = (float)UnSignedTemp * 0.01;

				// Meter Cal, 100 times actual
				UnSignedTemp = data[7] << 8 | data[8];
				MeterCal = (float)UnSignedTemp * 0.01;

				// command byte
				InCommand = data[9];
				if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

				ValveType = 0;
				if ((InCommand & 2) == 2) ValveType += 1;
				if ((InCommand & 4) == 4) ValveType += 2;

				SimulateFlow = ((InCommand & 8) == 8);

				UseVCN = ((InCommand & 16) == 16);

				//reset watchdog as we just heard from AgOpenGPS
				watchdogTimer = 0;
				AOGconnected = true;
			}
		}

		if (PGN == 32615)
		{
			byte ConID = data[2];
			if (ConID == ControllerID)
			{
				VCN = data[3] << 8 | data[4];
				SendTime = data[5] << 8 | data[6];
				WaitTime = data[7] << 8 | data[8];
				MinPWMvalue = data[9];

				watchdogTimer = 0;
				AOGconnected = true;
			}
		}

		if (PGN == 32616)
		{
			byte ConID = data[2];
			if (ConID == ControllerID)
			{
				PIDkp = data[3];
				PIDminPWM = data[4];
				PIDLowMax = data[5];
				PIDHighMax = data[6];
				PIDdeadband = data[7];
				PIDbrakePoint = data[8];

				watchdogTimer = 0;
				AOGconnected = true;
			}
		}
	}
}
#endif


