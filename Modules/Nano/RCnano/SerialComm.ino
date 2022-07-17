
byte SerialMSB;
byte SerialLSB;
unsigned int SerialPGN;
byte SerialPacket[30];

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
		// UDPpgn 32613
		SerialPacket[0] = 101;
		SerialPacket[1] = 127;
		SerialPacket[2] = BuildModSenID(PCB.ModuleID, i);

		// rate applied, 10 X actual
		SerialPacket[3] = UPM[i] * 10;
		SerialPacket[4] = (int)(UPM[i] * 10) >> 8;
		SerialPacket[5] = (int)(UPM[i] * 10) >> 16;

		// accumulated quantity, 10 X actual
		long Units = TotalPulses[i] * 10.0 / MeterCal[i];
		SerialPacket[6] = Units;
		SerialPacket[7] = Units >> 8;
		SerialPacket[8] = Units >> 16;

		// pwmSetting
		SerialPacket[9] = pwmSetting[i] * 10;
		SerialPacket[10] = (pwmSetting[i] * 10) >> 8;

		// status
		// bit 0    - sensor 0 receiving rate controller data
		// bit 1    - sensor 1 receiving rate controller data
		SerialPacket[11] = 0;
		if (millis() - CommTime[0] < 4000) SerialPacket[11] |= 0b00000001;
		if (millis() - CommTime[1] < 4000) SerialPacket[11] |= 0b00000010;
		
		// crc
		SerialPacket[12] = CRC(SerialPacket, 12, 0);

		Serial.print(SerialPacket[0]);
		for (int i = 1; i < 13; i++)
		{
			Serial.print(",");
			Serial.print(SerialPacket[i]);
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
			SerialPGN = 0;
		}

		switch (SerialPGN)
		{
		case 32614:
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
			PGNlength = 14;

			if (Serial.available() > PGNlength - 3)
			{

				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 102;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					if (ParseModID(SerialPacket[2]) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(SerialPacket[2]);
						if (SensorID < PCB.SensorCount)
						{
							RelayLo = SerialPacket[3];
							RelayHi = SerialPacket[4];

							// rate setting, 10 times actual
							int RateSet = SerialPacket[5] | SerialPacket[6] << 8 | SerialPacket[7] << 16;
							float TmpSet = (float)(RateSet * 0.1);

							// Meter Cal, 100 times actual
							UnSignedTemp = SerialPacket[8] | SerialPacket[9] << 8;
							MeterCal[SensorID] = (float)(UnSignedTemp * 0.01);

							// command byte
							InCommand[SensorID] = SerialPacket[10];
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
							PowerRelayLo = SerialPacket[11];
							PowerRelayHi = SerialPacket[12];

							CommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32616:
			// PID to Arduino from RateController, 12 bytes
			PGNlength = 12;

			if (Serial.available() > PGNlength - 3)
			{

				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 104;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					if (ParseModID(SerialPacket[2]) == PCB.ModuleID)
					{
						byte SensorID = ParseSenID(SerialPacket[2]);
						if (SensorID < PCB.SensorCount)
						{
							PIDkp[SensorID] = SerialPacket[3];
							PIDminPWM[SensorID] = SerialPacket[4];
							PIDLowMax[SensorID] = SerialPacket[5];
							PIDHighMax[SensorID] = SerialPacket[6];
							PIDdeadband[SensorID] = SerialPacket[7];
							PIDbrakePoint[SensorID] = SerialPacket[8];
							AdjustTime[SensorID] = SerialPacket[9];
							PIDki[SensorID] = SerialPacket[10];

							CommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32619:
			// from Wemos D1 mini, 6 bytes
			// section buttons
			PGNlength = 6;

			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 107;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
					WifiSwitches[i] = SerialPacket[i];
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					WifiSwitchesEnabled = true;
					WifiSwitchesTimer = millis();
				}
			}
			break;

		case 32620:
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
			PGNlength = 11;

			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 108;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					for (int i = 0; i < 8; i++)
					{
						SwitchBytes[i] = SerialPacket[i + 2];
					}
					TranslateSwitchBytes();
				}
			}
			break;

		case 32625:
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
			PGNlength = 7;

			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 113;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					PCB.ModuleID = SerialPacket[2];
					PCB.SensorCount = SerialPacket[3];
					PCB.IPpart3 = SerialPacket[4];

					byte tmp = SerialPacket[5];
					if ((tmp & 1) == 1) PCB.UseMCP23017 = 1; else PCB.UseMCP23017 = 0;
					if ((tmp & 2) == 2) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
					if ((tmp & 4) == 4) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;

					EEPROM.put(10, PCB);
				}
			}
			break;

		case 32626:
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
			PGNlength = 25;

			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;	// reset pgn
				SerialPacket[0] = 114;
				SerialPacket[1] = 127;
				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					PINS.Flow1 = SerialPacket[2];
					PINS.Flow2 = SerialPacket[3];
					PINS.Dir1 = SerialPacket[4];
					PINS.Dir2 = SerialPacket[5];
					PINS.PWM1 = SerialPacket[6];
					PINS.PWM2 = SerialPacket[7];

					for (int i = 0; i < 16; i++)
					{
						PINS.Relays[i] = SerialPacket[i + 8];
					}

					EEPROM.put(40, PINS);

					//reset the arduino
					resetFunc();
				}
			}
			break;

		default:
			// find pgn
			SerialMSB = Serial.read();
			SerialPGN = SerialMSB << 8 | SerialLSB;
			SerialLSB = SerialMSB;
			break;
		}
	}
}

