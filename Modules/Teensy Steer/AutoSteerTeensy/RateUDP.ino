
IPAddress RateSendIP(192, 168, PCB.IPpart3, 255);
byte RatePacket[30];
unsigned int RatePGN;

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
	//11 Status
	//12 crc

	// PGN 32613, 13 bytes
	RatePacket[0] = 101;
	RatePacket[1] = 127;
	RatePacket[2] = BuildModSenID(PCB.ModuleID, 0);

	// rate applied, 10 X actual
	uint8_t Temp = (int)(UPM[0] * 10);
	RatePacket[3] = Temp;
	Temp = (int)(UPM[0] * 10) >> 8;
	RatePacket[4] = Temp;
	Temp = (int)(UPM[0] * 10) >> 16;
	RatePacket[5] = Temp;

	// accumulated quantity, 10 X actual
	uint16_t Units = TotalPulses[0] * 10.0 / MeterCal[0];
	Temp = Units;
	RatePacket[6] = Temp;
	Temp = Units >> 8;
	RatePacket[7] = Temp;
	Temp = Units >> 16;
	RatePacket[8] = Temp;

	//pwmSetting
	Temp = (byte)(RatePWM[0] * 10);
	RatePacket[9] = Temp;
	Temp = (byte)(RatePWM[0] * 10 >> 8);
	RatePacket[10] = Temp;

	// status
	// bit 0    - sensor 0 receiving rate controller data
	// bit 1    - sensor 1 receiving rate controller data
	RatePacket[11] = 0;
	if (millis() - RateCommTime[0] < 4000) RatePacket[11] |= 0b00000001;
	if (millis() - RateCommTime[1] < 4000) RatePacket[11] |= 0b00000010;

	// crc
	RatePacket[12] = CRC(RatePacket, 12, 0);

	// send to RateController
	UDPrate.beginPacket(RateSendIP, DestinationPortRate);
	UDPrate.write(RatePacket, 13);
	UDPrate.endPacket();
}

void ReceiveRateUDP()
{
	uint16_t len = UDPrate.parsePacket();
	if (len > 1)
	{
		UDPrate.read(RatePacket, MaxReadBuffer);
		RatePGN = RatePacket[1] << 8 | RatePacket[0];
		switch (RatePGN)
		{
		case 32614:
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
			PGNlength = 14;

			if (len > PGNlength - 1)
			{
				if (GoodCRC(RatePacket, PGNlength))
				{
					uint8_t tmp = RatePacket[2];
					if (ParseModID(tmp) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(tmp);
						if (SensorID == 0)
						{
							RelayLo = RatePacket[3];
							RelayHi = RatePacket[4];

							// rate setting, 10 times actual
							uint16_t tmp = RatePacket[5] | RatePacket[6] << 8 | RatePacket[7] << 16;
							float TmpSet = (float)tmp * 0.1;

							// Meter Cal, 100 times actual
							tmp = RatePacket[8] | RatePacket[9] << 8;
							MeterCal[SensorID] = (float)tmp * 0.01;

							// command byte
							InCommand[SensorID] = RatePacket[10];
							if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

							ControlType[SensorID] = 0;
							if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
							if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;

							MasterOn[SensorID] = ((InCommand[SensorID] & 8) == 8);
							UseMultiPulses[SensorID] = ((InCommand[SensorID] & 16) == 16);

							AutoOn = ((InCommand[SensorID] & 32) == 32);
							if (AutoOn)
							{
								RateSetting[SensorID] = TmpSet;
							}
							else
							{
								ManualAdjust[SensorID] = TmpSet;
							}

							DebugOn = ((InCommand[SensorID] & 64) == 64);

							// power relays
							PowerRelayLo = RatePacket[11];
							PowerRelayHi = RatePacket[12];

							RateCommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32616:
			// PID to Arduino from RateController
			PGNlength = 12;

			if (len > PGNlength - 1)
			{
				if (GoodCRC(RatePacket, PGNlength))
				{
					uint8_t tmp = RatePacket[2];
					if (ParseModID(tmp) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(tmp);
						if (SensorID == 0)
						{
							PIDkp[SensorID] = RatePacket[3];
							PIDminPWM[SensorID] = RatePacket[4];
							PIDLowMax[SensorID] = RatePacket[5];
							PIDHighMax[SensorID] = RatePacket[6];
							PIDdeadband[SensorID] = RatePacket[7];
							PIDbrakePoint[SensorID] = RatePacket[8];
							AdjustTime[SensorID] = RatePacket[9];
							PIDki[SensorID] = RatePacket[10];

							RateCommTime[SensorID] = millis();
						}
					}
				}
			}
			break;
		}
	}
}

void DebugTheINO()
{
	// send debug info to RateController
	if (millis() - DebugTime > 1000)
	{
		DebugTime = millis();
		//DebugVal1 = PINS.WAS;
		//DebugVal2 = PINS.PressureSensor;

		DebugVal1 = PCB.RS485PortNumber;
		DebugVal2 = PCB.WemosSerialPort;
		DebugVal3 = PCB.AnalogMethod;

		// Serial
		// PGNudp 2748 - 0xABC
		Serial.print(0xBC);
		Serial.print(",");
		Serial.print(0xA);
		Serial.print(",");
		Serial.print(DebugVal1);
		Serial.print(",");
		Serial.print(DebugVal2);
		Serial.print(",");
		Serial.print(DebugVal3);
		Serial.print(", ");
		Serial.print(DebugVal4);
		Serial.print(",");
		Serial.print(DebugVal5);
		Serial.print(",");
		Serial.print(DebugVal6);

		Serial.println("");

		//// UDP, will lock up if not connected
		//RatePacket[0] = 0xBC;
		//RatePacket[1] = 0xA;
		//RatePacket[2] = DebugVal1;
		//RatePacket[3] = DebugVal2;
		//RatePacket[4] = DebugVal5;
		//RatePacket[5] = DebugVal6;

		//// send to RateController
		//UDPrate.beginPacket(RateSendIP, DestinationPortRate);
		//UDPrate.write(RatePacket, 6);
		//UDPrate.endPacket();
	}
}
