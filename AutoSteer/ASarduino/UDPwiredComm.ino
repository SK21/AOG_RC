#if(CommType == 1)
void SendUDPwired()
{
	// PGN32765
	// header
	toSend[0] = 127;
	toSend[1] = 253;

	//actual steer angle
	temp = (100 * steerAngleActual);
	toSend[2] = (byte)(temp >> 8);
	toSend[3] = (byte)(temp);

	// heading
	toSend[4] = (byte)((int)(IMUheading * 16) >> 8);
	toSend[5] = (byte)(IMUheading * 16);

	// Roll
	toSend[6] = (byte)((int)(FilteredRoll * 16) >> 8);
	toSend[7] = (byte)(FilteredRoll * 16);

	//switch byte
	toSend[8] = switchByte;

	//pwm value
	toSend[9] = abs(pwmDrive);

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);
}

//void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, byte* data, uint16_t len)
{
	if ((len > 7) && (data[0] == 0x7F) && (data[1] == 0xFE))
	{
		// autosteer data
		relay = 0;
		CurrentSpeed = data[3] / 4;
		distanceFromLine = (float)(data[4] << 8 | data[5]);
		steerAngleSetPoint = (((float)(data[6] << 8 | data[7]))) * 0.01;
		uTurn = 0;
		watchdogTimer = 0;
	}

	if ((len > 9) && (data[0] == 0x7F) && (data[1] == 0xFC))
	{
		// autosteer settings
		Kp = (float)data[2] * 1.0;
		LowMaxPWM = data[3];
		AOGzeroAdjustment = (data[6] - 127) * 20;	// 20 X the setting displayed in AOG
		MinPWMvalue = data[7];	// read the minimum amount of PWM for instant on
		HighMaxPWM = data[8];
		SteerCPD = data[9] * 2;	// 2 X the setting displayed in AOG
	}
}
#endif


