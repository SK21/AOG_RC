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

void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
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
		Ki = (float)data[3] * 0.0001;
		Kd = (float)data[4] * 0.1;
		Ko = (float)data[5] * 0.1;

		AOGzeroAdjustment = (data[6] - 127) * 20;	// 20 X the setting displayed in AOG
		SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;
		MinPWMvalue = data[7];	// read the minimum amount of PWM for instant on
		maxIntegralValue = data[8] * 0.1;
		SteerCPD = data[9] * 2;	// 2 X the setting displayed in AOG
	}

	if ((len > 7) && (data[0] == 0x7F) && (data[1] == 0xFB))
	{
		// aogSettings

		//set0
		byte sett = data[2];  

		if (bitRead(sett, 0)) aogSettings.InvertWAS = 1; else aogSettings.InvertWAS = 0;
		if (bitRead(sett, 1)) aogSettings.InvertRoll = 1; else aogSettings.InvertRoll = 0;
		if (bitRead(sett, 2)) aogSettings.MotorDriveDirection = 1; else aogSettings.MotorDriveDirection = 0;
		if (bitRead(sett, 3)) aogSettings.SingleInputWAS = 1; else aogSettings.SingleInputWAS = 0;
		if (bitRead(sett, 4)) aogSettings.CytronDriver = 1; else aogSettings.CytronDriver = 0;
		if (bitRead(sett, 5)) aogSettings.SteerSwitch = 1; else aogSettings.SteerSwitch = 0;
		if (bitRead(sett, 6)) aogSettings.UseMMA_X_Axis = 1; else aogSettings.UseMMA_X_Axis = 0;
		if (bitRead(sett, 7)) aogSettings.ShaftEncoder = 1; else aogSettings.ShaftEncoder = 0;

		//set1
		sett = data[3];
		if (bitRead(sett, 0)) aogSettings.BNOInstalled = 1; else aogSettings.BNOInstalled = 0;

		aogSettings.MaxSpeed = data[4]; 
		aogSettings.MinSpeed = data[5];

		sett = data[6];
		aogSettings.InclinometerInstalled = sett & 192;
		aogSettings.InclinometerInstalled = aogSettings.InclinometerInstalled >> 6;
		aogSettings.PulseCountMax = sett & 63;

		aogSettings.AckermanFix = data[7];

		EEPROM.put(40, aogSettings);

		resetFunc();
	}
}
#endif

