void SendComm()
{
	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();

		//PGN32400, Rate info from module to RC
		//0     HeaderLo    144
		//1     HeaderHi    126
		//2     Mod/Sen ID          0-15/0-15
		//3	    rate applied Lo 	1000 X actual
		//4     rate applied Mid
		//5	    rate applied Hi
		//6	    acc.Quantity Lo		10 X actual
		//7	    acc.Quantity Mid
		//8     acc.Quantity Hi
		//9     PWM Lo
		//10    PWM Hi
		//11    Status
		//      bit 0   sensor connected
		//      bit 1   bin empty
		//12    Hz Lo
		//13    Hz Hi
		//14    CRC

		byte Data[20];

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Data[0] = 144;
			Data[1] = 126;
			Data[2] = BuildModSenID(MDL.ID, i);

			// rate applied, 1000 X actual
			uint32_t Applied = Sensor[i].UPM * 1000;
			Data[3] = Applied;
			Data[4] = Applied >> 8;
			Data[5] = Applied >> 16;

			// accumulated quantity, 10 X actual
			if (Sensor[i].MeterCal > 0)
			{
				uint32_t Units = Sensor[i].TotalPulses * 10.0 / Sensor[i].MeterCal;
				Data[6] = Units;
				Data[7] = Units >> 8;
				Data[8] = Units >> 16;
			}
			else
			{
				Data[6] = 0;
				Data[7] = 0;
				Data[8] = 0;
			}

			//PWM
			Data[9] = (int)Sensor[i].PWM;
			Data[10] = (int)Sensor[i].PWM >> 8;


			// status
			Data[11] = 0;
			if (SensorConnected[i]) Data[11] |= 0b00000001;
			if (BinEmpty[i]) Data[11] |= 0b00000010;

			// Hz
			uint16_t Hz = Sensor[i].Hz * 10;
			Data[12] = Hz;
			Data[13] = Hz >> 8;

			// crc
			Data[14] = CRC(Data, 14, 0);
			bool Sent = false;

			// ethernet
			if (ChipFound)
			{
				if (Ethernet.linkStatus() == LinkON)
				{
					UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
					UDP_Ethernet.write(Data, 15);
					UDP_Ethernet.endPacket();
					Sent = true;
				}
			}

			// wifi
			if (!Sent)
			{
				UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
				UDP_Wifi.write(Data, 15);
				UDP_Wifi.endPacket();
			}
		}

		//PGN32401, module info from module to RC
		//0     145
		//1     126
		//2     module ID
		//3     Pressure Lo 
		//4     Pressure Hi
		//5     wheel speed Lo  actual * 10
		//6     wheel speed Hi
		//7     wheel count Lo
		//8     wheel count mid
		//9     wheel count Hi
		//10    InoType
		//11    InoID lo
		//12    InoID hi
		//13    status
		//      bit 0   work switch
		//      bit 1   wifi rssi < -80
		//      bit 2	wifi rssi < -70
		//      bit 3	wifi rssi < -65
		//      bit 4   ethernet connected
		//      bit 5   good pin configuration
		//      bit 6   0 - 2 wire relays, 1 - 3 wire relays
		//14    CRC

		Data[0] = 145;
		Data[1] = 126;
		Data[2] = MDL.ID;

		if (MDL.PressurePin == NC) PressureReading = 0;
		Data[3] = (byte)PressureReading;
		Data[4] = (byte)(PressureReading >> 8);

		// wheel speed, 10 X actual
		if (MDL.WheelSpeedPin == NC) WheelSpeed = 0;
		uint32_t Speed = WheelSpeed * 10.0;
		Data[5] = Speed;
		Data[6] = Speed >> 8;

		if (MDL.WheelSpeedPin == NC) WheelCounts = 0;
		Data[7] = WheelCounts;
		Data[8] = WheelCounts >> 8;
		Data[9] = WheelCounts >> 16;

		Data[10] = InoType;
		Data[11] = (byte)InoID;
		Data[12] = (byte)(InoID >> 8);

		// status
		Data[13] = 0;
		if ((MDL.WorkPin != NC) && WorkPinOn()) Data[13] |= 0b00000001;

		if (WiFi.isConnected())
		{
			int8_t WifiStrength = WiFi.RSSI();
			if (WifiStrength < -80)
			{
				Data[13] |= 0b00000010;
			}
			else if (WifiStrength < -70)
			{
				Data[13] |= 0b00000100;
			}
			else
			{
				Data[13] |= 0b00001000;
			}
		}

		if (ChipFound)
		{
			if (Ethernet.linkStatus() == LinkON) Data[13] |= 0b00010000;
		}

		if (GoodPins) Data[13] |= 0b00100000;
		if (MDL.Is3Wire) Data[13] |= 0b01000000;

		Data[14] = CRC(Data, 14, 0);

		bool Sent = false;
		// ethernet
		if (ChipFound)
		{
			if (Ethernet.linkStatus() == LinkON)
			{

				UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
				UDP_Ethernet.write(Data, 15);
				UDP_Ethernet.endPacket();
				Sent = true;
			}
		}

		// wifi
		if (!Sent)
		{
			UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
			UDP_Wifi.write(Data, 15);
			UDP_Wifi.endPacket();
		}

		// PGN32403, board ID label report from module to RC (slow cyclic - static label)
		//0     HeaderLo    147
		//1     HeaderHi    126
		//2     Module ID   0-7
		//3-18  16 chars    board label, 0-padded
		//19    CRC
		static uint32_t BoardIDLast = 0;
		if (millis() - BoardIDLast > 2000)
		{
			BoardIDLast = millis();
			byte B[20];
			B[0] = 147;
			B[1] = 126;
			B[2] = MDL.ID;	// raw module ID, matching the module-level status report (PGN 32401)
			for (byte k = 0; k < 16; k++) B[3 + k] = MDLboard.Text[k];
			B[19] = CRC(B, 19, 0);

			bool Sent = false;
			// ethernet
			if (ChipFound)
			{
				if (Ethernet.linkStatus() == LinkON)
				{

					UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
					UDP_Ethernet.write(B, 20);
					UDP_Ethernet.endPacket();
					Sent = true;
				}
			}

			// wifi
			if (!Sent)
			{
				UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
				UDP_Wifi.write(B, 20);
				UDP_Wifi.endPacket();
			}
		}
	}
}

void SendPIDlog()
{
	// PGN32402, PID diagnostics from module to RC (one packet per sensor, at PID-loop cadence)
	//0     HeaderLo    146
	//1     HeaderHi    126
	//2     Mod/Sen ID          0-15/0-15
	//3     millis Lo           PID loop timestamp, uint32
	//4     millis
	//5     millis
	//6     millis Hi
	//7     TargetUPM Lo        1000 X actual
	//8     TargetUPM Mid
	//9     TargetUPM Hi
	//10    UPM Lo              1000 X actual
	//11    UPM Mid
	//12    UPM Hi
	//13    Error Lo            1000 X actual, signed 24-bit
	//14    Error Mid
	//15    Error Hi
	//16    Integral Lo         10 X actual, signed 16-bit
	//17    Integral Hi
	//18    Change Lo           signed 16-bit
	//19    Change Hi
	//20    PWM Lo              signed 16-bit
	//21    PWM Hi
	//22    Samples             pulse count used in the median
	//23    InoID Lo            firmware build id, uint16 (identifies exact firmware that produced the log)
	//24    InoID Hi
	//25    CRC

	// gate on the flag only - the per-packet code below picks ethernet or wifi
	// (the Teensy version gates on ethernet link because it has no other transport)
	if (PidLogEnabled)
	{
		byte Data[26];

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			if (PidSampleReady[i])
			{
				PidSampleReady[i] = false;

				Data[0] = 146;
				Data[1] = 126;
				Data[2] = BuildModSenID(MDL.ID, i);

				uint32_t Stamp = DiagMillis[i];
				Data[3] = Stamp;
				Data[4] = Stamp >> 8;
				Data[5] = Stamp >> 16;
				Data[6] = Stamp >> 24;

				uint32_t Target = DiagTarget[i] * 1000;
				Data[7] = Target;
				Data[8] = Target >> 8;
				Data[9] = Target >> 16;

				uint32_t Applied = DiagApplied[i] * 1000;
				Data[10] = Applied;
				Data[11] = Applied >> 8;
				Data[12] = Applied >> 16;

				int32_t Error = DiagError[i] * 1000;	// signed 24-bit
				Data[13] = Error;
				Data[14] = Error >> 8;
				Data[15] = Error >> 16;

				int16_t Integral = DiagIntegral[i] * 10;
				Data[16] = Integral;
				Data[17] = Integral >> 8;

				int16_t Change = DiagChange[i];
				Data[18] = Change;
				Data[19] = Change >> 8;

				int16_t PWM = (int16_t)DiagPWM[i];
				Data[20] = PWM;
				Data[21] = PWM >> 8;

				Data[22] = DiagSamples[i];	// pulse samples used in the median

				Data[23] = (byte)InoID;		// firmware build id, lets the analyzer name the exact firmware
				Data[24] = InoID >> 8;

				Data[25] = CRC(Data, 25, 0);

				bool Sent = false;
				// ethernet
				if (ChipFound)
				{
					if (Ethernet.linkStatus() == LinkON)
					{

						UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
						UDP_Ethernet.write(Data, 26);
						UDP_Ethernet.endPacket();
						Sent = true;
					}
				}

				// wifi
				if (!Sent)
				{
					UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
					UDP_Wifi.write(Data, 26);
					UDP_Wifi.endPacket();
				}
			}
		}
	}
}
