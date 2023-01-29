
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
	//3	rate applied Lo 	1000 X actual
	//4 rate applied Mid
	//5	rate applied Hi
	//6	acc.Quantity Lo		10 X actual
	//7	acc.Quantity Mid
	//8	acc.Quantity Hi
	//9 PWM Lo
	//10 PWM Hi
	//11 Status
	//12	crc

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		// UDPpgn 32613
		SerialPacket[0] = 101;
		SerialPacket[1] = 127;
		SerialPacket[2] = BuildModSenID(MDL.ModuleID, i);

		// rate applied, 1000 X actual
		uint32_t Applied = UPM[i] * 1000;
		SerialPacket[3] = Applied;
		SerialPacket[4] = Applied >> 8;
		SerialPacket[5] = Applied >> 16;

		// accumulated quantity, 10 X actual
		if (MeterCal[i] > 0)
		{
			uint32_t Units = TotalPulses[i] * 10.0 / MeterCal[i];
			SerialPacket[6] = Units;
			SerialPacket[7] = Units >> 8;
			SerialPacket[8] = Units >> 16;
		}
		else
		{
			SerialPacket[6] = 0;
			SerialPacket[7] = 0;
			SerialPacket[8] = 0;
		}

		// pwmSetting
		SerialPacket[9] = pwmSetting[i];
		SerialPacket[10] = pwmSetting[i] >> 8;

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
		if (Serial.available() > 50)
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
			//5	rate set Lo		1000 X actual
			//6 rate set Mid
			//7	rate set Hi		10 X actual
			//8	Flow Cal Lo		100 X actual
			//9 Flow Cal Mid
			//10 Flow Cal Hi
			//11 Command
			//	        - bit 0		    reset acc.Quantity
			//	        - bit 1,2,3		control type 0-4
			//	        - bit 4		    MasterOn
			//          - bit 5         0 - average time for multiple pulses, 1 - time for one pulse
			//          - bit 6         AutoOn
			//          - bit 7         Calibration On
			//12    power relay Lo      list of power type relays 0-7
			//13    power relay Hi      list of power type relays 8-15
			//14	manual pwm
			//15	CRC
			PGNlength = 16;

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
					if (ParseModID(SerialPacket[2]) == MDL.ModuleID)
					{
						byte SensorID = ParseSenID(SerialPacket[2]);
						if (SensorID < MDL.SensorCount)
						{
							RelayLo = SerialPacket[3];
							RelayHi = SerialPacket[4];

							// rate setting, 1000 times actual
							uint32_t RateSet = SerialPacket[5] | (uint32_t)SerialPacket[6] << 8 | (uint32_t)SerialPacket[7] << 16;
							RateSetting[SensorID] = (float)(RateSet * 0.001);

							// Meter Cal, 1000 times actual
							uint32_t Temp = SerialPacket[8] | (uint32_t)SerialPacket[9] << 8 | (uint32_t)SerialPacket[10] << 16;
							MeterCal[SensorID] = (float)(Temp * 0.001);

							// command byte
							InCommand[SensorID] = SerialPacket[11];
							if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

							ControlType[SensorID] = 0;
							if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
							if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;
							if ((InCommand[SensorID] & 8) == 8) ControlType[SensorID] += 4;

							MasterOn[SensorID] = ((InCommand[SensorID] & 16) == 16);
							UseMultiPulses[SensorID] = ((InCommand[SensorID] & 32) == 32);
							AutoOn = ((InCommand[SensorID] & 64) == 64);

							// power relays
							PowerRelayLo = SerialPacket[12];
							PowerRelayHi = SerialPacket[13];

							ManualAdjust[SensorID] = SerialPacket[14];

							CommTime[SensorID] = millis();
						}
					}
				}
			}
			break;

		case 32616:
			// PID to Arduino from RateController
			// 0    104
			// 1    127
			// 2    Mod/Sen ID     0-15/0-15
			// 3    KP 0
			// 4    KP 1
			// 5    KP 2
			// 6    KP 3
			// 7    KI 0
			// 8    KI 1
			// 9    KI 2
			// 10   KI 3
			// 11   KD 0
			// 12   KD 1
			// 13   KD 2
			// 14   KD 3
			// 15   MinPWM
			// 16   MaxPWM
			// 17   CRC

			PGNlength = 18;

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
					if (ParseModID(SerialPacket[2]) == MDL.ModuleID)
					{
						byte SensorID = ParseSenID(SerialPacket[2]);
						if (SensorID < MDL.SensorCount)
						{
							uint32_t tmp = SerialPacket[3] | (uint32_t)SerialPacket[4] << 8 | (uint32_t)SerialPacket[5] << 16 | (uint32_t)SerialPacket[6] << 24;
							PIDkp[SensorID] = (float)(tmp * 0.0001);

							tmp = SerialPacket[7] | (uint32_t)SerialPacket[8] << 8 | (uint32_t)SerialPacket[9] << 16 | (uint32_t)SerialPacket[10] << 24;
							PIDki[SensorID] = (float)(tmp * 0.0001);

							tmp = SerialPacket[11] | (uint32_t)SerialPacket[12] << 8 | (uint32_t)SerialPacket[13] << 16 | (uint32_t)SerialPacket[14] << 24;
							PIDkd[SensorID] = (float)(tmp * 0.0001);

							MinPWM[SensorID] = SerialPacket[15];
							MaxPWM[SensorID] = SerialPacket[16];

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
			// from rate controller, 8 bytes
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
			// 6    minimum ms debounce
			// 7    crc
			PGNlength = 8;

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
					MDL.ModuleID = SerialPacket[2];
					MDL.SensorCount = SerialPacket[3];
					MDL.IPpart3 = SerialPacket[4];

					byte tmp = SerialPacket[5];
					if ((tmp & 1) == 1) MDL.UseMCP23017 = 1; else MDL.UseMCP23017 = 0;
					if ((tmp & 2) == 2) MDL.RelayOnSignal = 1; else MDL.RelayOnSignal = 0;
					if ((tmp & 4) == 4) MDL.FlowOnDirection = 1; else MDL.FlowOnDirection = 0;

					MDL.Debounce = SerialPacket[6];

					EEPROM.put(10, MDL);
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
					MDL.Flow1 = SerialPacket[2];
					MDL.Flow2 = SerialPacket[3];
					MDL.Dir1 = SerialPacket[4];
					MDL.Dir2 = SerialPacket[5];
					MDL.PWM1 = SerialPacket[6];
					MDL.PWM2 = SerialPacket[7];

					for (int i = 0; i < 16; i++)
					{
						MDL.Relays[i] = SerialPacket[i + 8];
					}

					EEPROM.put(10, MDL);

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

