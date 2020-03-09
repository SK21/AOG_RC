#if(CommType == 1)
void SendUDPwired()
{
	// PGN 31100
	// header
	toSend[0] = 121;
	toSend[1] = 124;

	// rate applied, 100 X actual
	Temp = ((int)RateAppliedUPM * 100) >> 8;
	toSend[2] = Temp;
	Temp = (RateAppliedUPM * 100);
	toSend[3] = Temp;

	// accumulated quantity
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal)) >> 8;
	toSend[4] = Temp;
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal));
	toSend[5] = Temp;

	// rate error %
	ConvertToSignedBytes(PercentError * 100);
	toSend[6] = HiByte;
	toSend[7] = LoByte;

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);


	// PGN 31200
	// header
	toSend[0] = 121;
	toSend[1] = 224;

	// relay Hi
	toSend[2] = 0;

	// relay Lo
	toSend[3] = RelayToAOG;

	// SecSwOff Hi
	toSend[4] = SecSwOff[1];

	// SecSwOff Lo
	toSend[5] = SecSwOff[0];

	// command byte
	toSend[6] = OutCommand;

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);
}

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
{
	// PGN31300
	if ((len > 8) && (data[0] == 0x7A) && (data[1] = 0x44))
	{
		RelayHi = data[2];
		RelayFromAOG = data[3];

		// rate setting, 100 times actual
		unsignedTemp = data[4] << 8 | data[5];
		rateSetPoint = (float)unsignedTemp * 0.01;

		// Meter Cal, 100 times actual
		unsignedTemp = data[6] << 8 | data[7];
		MeterCal = (float)unsignedTemp * 0.01;

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

	//PGN31400
	if ((len > 7) && (data[0] == 0x7A) && (data[1] = 0xA8))
	{
		KP = (float)data[2] * 0.1;
		KI = (float)data[3] * 0.0001;
		KD = (float)data[4] * 0.1;
		DeadBand = (float)data[5];
		MinPWMvalue = data[6];
		MaxPWMvalue = data[7];

		//reset watchdog as we just heard from AgOpenGPS
		watchdogTimer = 0;
		AOGconnected = true;
	}
}
#endif
