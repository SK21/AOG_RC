
void SendSerial()
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
	//12	crc

	for (int i = 0; i < PCB.SensorCount; i++)
	{
		// PGNudp 32613
		Packet[0] = 101;
		Packet[1] = 127;
		Packet[2] = BuildModSenID(PCB.ModuleID, i);

		// rate applied, 10 X actual
		Packet[3] = UPM[i] * 10;
		Packet[4] = (int)(UPM[i] * 10) >> 8;
		Packet[5] = (int)(UPM[i] * 10) >> 16;

		// accumulated quantity, 10 X actual
		long Units = TotalPulses[i] * 10.0 / MeterCal[i];
		Packet[6] = Units;
		Packet[7] = Units >> 8;
		Packet[8] = Units >> 16;

		// pwmSetting
		Packet[9] = pwmSetting[i] * 10;
		Packet[10] = (pwmSetting[i] * 10) >> 8;

		// status
		// bit 0    - sensor 0 receiving rate controller data
		// bit 1    - sensor 1 receiving rate controller data
		Packet[11] = 0;
		if (millis() - CommTime[0] < 4000) Packet[11] |= 0b00000001;
		if (millis() - CommTime[1] < 4000) Packet[11] |= 0b00000010;
		
		// crc
		Packet[12] = CRC(12, 0);

		Serial.print(Packet[0]);
		for (int i = 1; i < 13; i++)
		{
			Serial.print(",");
			Serial.print(Packet[i]);
		}
		Serial.println("");
	}
}

void ReceiveSerial()
{
	if (Serial.available())
	{
		if (Serial.available() > 30)
		{
			// clear buffer
			while (Serial.available())
			{
				Serial.read();
			}
		}

		switch (PGNserial)
		{
		case 32614:
			if (Serial.available() > 11)
			{
				//PGN32614 to Arduino from Rate Controller, 14 bytes
				//0	HeaderLo		102
				//1	HeaderHi		127
				//2 Controller ID
				//3	relay Lo		0 - 7
				//4	relay Hi		8 - 15
				//5	rate set Lo		10 X actual
				//6 rate set Mid
				//7	rate set Hi		10 X actual
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
				//13	CRC

				PGNserial = 0;	// reset pgn
				Packet[0] = 102;
				Packet[1] = 127;
				for (int i = 2; i < 14; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(14))
				{
					if (ParseModID(Packet[2]) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(Packet[2]);
						if (SensorID < PCB.SensorCount)
						{
							RelayLo = Packet[3];
							RelayHi = Packet[4];

							// rate setting, 10 times actual
							int RateSet = Packet[5] | Packet[6] << 8 | Packet[7] << 16;
							float TmpSet = (float)(RateSet * 0.1);

							// Meter Cal, 100 times actual
							UnSignedTemp = Packet[8] | Packet[9] << 8;
							MeterCal[SensorID] = (float)(UnSignedTemp * 0.01);

							// command byte
							InCommand[SensorID] = Packet[10];
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
							PowerRelayLo = Packet[11];
							PowerRelayHi = Packet[12];

							CommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32616:
			if (Serial.available() > 8)
			{
				// PID to Arduino from RateController, 11 bytes

				PGNserial = 0;	// reset pgn
				Packet[0] = 104;
				Packet[1] = 127;
				for (int i = 2; i < 11; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(11))
				{
					if (ParseModID(Packet[2]) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(Packet[2]);
						if (SensorID < PCB.SensorCount)
						{
							PIDkp[SensorID] = Packet[3];
							PIDminPWM[SensorID] = Packet[4];
							PIDLowMax[SensorID] = Packet[5];
							PIDHighMax[SensorID] = Packet[6];
							PIDdeadband[SensorID] = Packet[7];
							PIDbrakePoint[SensorID] = Packet[8];
							AdjustTime[SensorID] = Packet[9];

							CommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32619:
			if (Serial.available() > 3)
			{
				// from Wemos D1 mini, 6 bytes
				// section buttons
				PGNserial = 0;	// reset pgn
				Packet[0] = 107;
				Packet[1] = 127;
				for (int i = 2; i < 6; i++)
				{
					Packet[i] = Serial.read();
					WifiSwitches[i] = Packet[i];
				}

				if (GoodCRC(6))
				{
					WifiSwitchesEnabled = true;
					WifiSwitchesTimer = millis();
				}
			}
			break;

		case 32620:
			if (Serial.available() > 8)
			{
				// from rate controller, 11 bytes
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
				// 10	crc

				PGNserial = 0;	// reset pgn
				Packet[0] = 108;
				Packet[1] = 127;
				for (int i = 2; i < 11; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(11))
				{
					for (int i = 0; i < 8; i++)
					{
						SwitchBytes[i] = Packet[i + 2];
					}
					TranslateSwitchBytes();
				}
			}
			break;

		case 32625:
			if (Serial.available() > 4)
			{
				// from rate controller, 7 bytes
				// Nano config
				// 0    113
				// 1    127
				// 2    ModuleID
				// 3    SensorCount
				// 4	IP address
				// 5    Commands
				//      - UseMCP23017
				//      - RelyOnSignal
				//      - FlowOnSignal
				// 6	crc

				PGNserial = 0;	// reset pgn
				Packet[0] = 113;
				Packet[1] = 127;
				for (int i = 2; i < 7; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(7))
				{
					PCB.ModuleID = Packet[2];
					PCB.SensorCount = Packet[3];
					PCB.IPpart3 = Packet[4];

					byte tmp = Packet[5];
					if ((tmp & 1) == 1) PCB.UseMCP23017 = 1; else PCB.UseMCP23017 = 0;
					if ((tmp & 2) == 2) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
					if ((tmp & 4) == 4) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;

					EEPROM.put(10, PCB);
				}
			}
			break;

		case 32626:
			if (Serial.available() > 22)
			{
				// Nano pins from rate controller, 25 bytes
				// 0        114
				// 1        127
				// 2        Flow 1
				// 3        Flow 2
				// 4        Dir 1
				// 5        Dir 2
				// 6        PWM 1
				// 7        PWM 2
				// 8 - 23   Relays 1-16
				// 24		crc

				PGNserial = 0;	// reset pgn
				Packet[0] = 114;
				Packet[1] = 127;
				for (int i = 2; i < 25; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(25))
				{
					PINS.Flow1 = Packet[2];
					PINS.Flow2 = Packet[3];
					PINS.Dir1 = Packet[4];
					PINS.Dir2 = Packet[5];
					PINS.PWM1 = Packet[6];
					PINS.PWM2 = Packet[7];

					for (int i = 0; i < 16; i++)
					{
						PINS.Relays[i] = Packet[i + 8];
					}

					EEPROM.put(40, PINS);

					//reset the arduino
					resetFunc();
				}
			}
			break;

		default:
			// find pgn
			MSB = Serial.read();
			PGNserial = MSB << 8 | LSB;
			LSB = MSB;
			break;
		}
	}
}

