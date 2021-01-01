#if(CommType == 1)
void SendUDPwired()
{
	// PGN 32741
	// header
	toSend[0] = 127;
	toSend[1] = 229;

	// rate applied, 100 X actual
	Temp = (int)(FlowRate * 100) >> 8;
	toSend[2] = Temp;
	Temp = (FlowRate * 100);
	toSend[3] = Temp;

	// accumulated quantity, 3 bytes
	long Units = accumulatedCounts * 100.0 / MeterCal;
	Temp = Units >> 16;
	toSend[4] = Temp;
	Temp = Units >> 8;
	toSend[5] = Temp;
	Temp = Units;
	toSend[6] = Temp;

	//pwmSetting
	Temp = (int)((300 - pwmSetting) * 10) >> 8;	// account for negative values
	toSend[7] = Temp;
	Temp = (300 - pwmSetting) * 10;
	toSend[8] = Temp;

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

	if ((len > 8) && (PGN == 32742))
	{
		RelayHi = data[2];
		RelayFromAOG = data[3];

		// rate setting, 100 times actual
		UnSignedTemp = data[4] << 8 | data[5];
		rateSetPoint = (float)UnSignedTemp * 0.01;

		// Meter Cal, 100 times actual
		UnSignedTemp = data[6] << 8 | data[7];
		MeterCal = (float)UnSignedTemp * 0.01;

		// command byte
		InCommand = data[8];
		if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

		ValveType = 0;
		if ((InCommand & 2) == 2) ValveType += 1;
		if ((InCommand & 4) == 4) ValveType += 2;

		SimulateFlow = ((InCommand & 8) == 8);

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}

	//PGN32744
	if ((len > 7) && (PGN == 32744))
	{
		VCN = data[2] << 8 | data[3];
		SendTime = data[4] << 8 | data[5];
		WaitTime = data[6] << 8 | data[7];
		MaxPWMvalue = data[8];
		MinPWMvalue = data[9];

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}
}
#endif


