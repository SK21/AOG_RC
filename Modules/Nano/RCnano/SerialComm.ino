byte MSB;
byte LSB;
byte LoLast;
uint32_t LoChangeTime;

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
	//11	crc

	for (int i = 0; i < PCB.SensorCount; i++)
	{
		// PGN 32613
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

		// crc
		Packet[11] = CRC(11, 0);

		Serial.print(Packet[0]);
		for (int i = 1; i < 12; i++)
		{
			Serial.print(",");
			Serial.print(Packet[i]);
		}
		Serial.println("");
	}
}

void ReceiveSerial()
{
	if (Serial.available() > 0 && !PGN32614Found && !PGN32616Found
		&& !PGN32619Found && !PGN32620Found
		&& !PGN32625Found && !PGN32626Found) //find the header
	{
		MSB = Serial.read();
		PGN = MSB << 8 | LSB;               //high,low bytes to make int
		LSB = MSB;                          //save for next time

		PGN32614Found = (PGN == 32614);
		PGN32616Found = (PGN == 32616);
		PGN32619Found = (PGN == 32619);
		PGN32620Found = (PGN == 32620);
		PGN32625Found = (PGN == 32625);
		PGN32626Found = (PGN == 32626);

		if (Serial.available() > 40)
		{
			// clear buffer
			while (Serial.available() > 0) char t = Serial.read();
		}
	}

	if (Serial.available() > 11 && PGN32614Found)
	{
		//PGN32614 to Arduino from Rate Controller
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

		PGN32614Found = false;
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
					UnSignedTemp = Packet[5] | Packet[6] << 8 | Packet[7] << 16;
					float TmpSet = (float)(UnSignedTemp * 0.1);

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
						NewRateFactor[SensorID] = TmpSet;
					}

					// power relays
					PowerRelayLo = Packet[11];
					PowerRelayHi = Packet[12];

					CommTime[SensorID] = millis();
				}
			}
		}
	}

	else if (Serial.available() > 8 && PGN32616Found)
	{
		// PID to Arduino from RateController
		PGN32616Found = false;
		Packet[0] = 104;
		Packet[1] = 127;
		for (int i = 2; i < 11; i++)
		{
			Packet[i] = Serial.read();
		}

		if (GoodCRC(11))
		{
			if (ParseModID(Packet[2] == PCB.ModuleID))
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

	else if (Serial.available() > 3 && PGN32619Found)
	{
		// from Wemos D1 mini
		// section buttons
		// 6 bytes

		PGN32619Found = false;
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

	else if (Serial.available() > 8 && PGN32620Found)
	{
		// from rate controller
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

		// 11 bytes
		PGN32620Found = false;
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

	else if (Serial.available() > 2 && PGN32625Found)
	{
		// from rate controller
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

		// 7 bytes

		PGN32625Found = false;
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

	else if (Serial.available() > 22 && PGN32626Found)
	{
		// from rate controller
		// Nano pins
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

		// 25 bytes
		PGN32626Found = false;
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
}