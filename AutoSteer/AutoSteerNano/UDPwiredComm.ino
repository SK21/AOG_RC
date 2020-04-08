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
	toSend[4] = (byte)(IMUheading >> 8);
	toSend[5] = (byte)(IMUheading);

	// Roll
	toSend[6] = (byte)(CurrentRoll >> 8);
	toSend[7] = (byte)(CurrentRoll);

	//switch byte
	toSend[8] = switchByte;

	//pwm value
	toSend[9] = abs(pwmDrive);

	//off to AOG
	ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);
}

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
{
	// PGN32766 - autosteer data
	if ((len > 7) && (data[0] == 0x7F) && (data[1] == 0xFE))
	{
		relay = 0;
		CurrentSpeed = data[3] / 4;
		distanceFromLine = (float)(data[4] << 8 | data[5]);
		steerAngleSetPoint = (((float)(data[6] << 8 | data[7]))) * 0.01;
		uTurn = 0;
		watchdogTimer = 0;
	}

	// PGN32764 - autosteer settings
	if ((len > 9) && (data[0] == 0x7F) && (data[1] == 0xFC))
	{
		Kp = (float)data[2] * 1.0;
		Ki = (float)data[3] * 0.001;
		Kd = (float)data[4] * 1.0;
		Ko = (float)data[5] * 0.1;

		AOGzeroAdjustment = (data[6] - 127) * 20;	// 20 X the setting displayed in AOG
		SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;
		MinPWMvalue = data[7];	// read the minimum amount of PWM for instant on
		maxIntegralValue = data[8] * 0.1;
		SteerCPD = data[9] * 2;	// 2 X the setting displayed in AOG
	}
}
#endif

