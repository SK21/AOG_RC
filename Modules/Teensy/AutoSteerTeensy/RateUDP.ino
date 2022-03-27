
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

	// rate applied, 10 X actual
	uint8_t Temp = (int)(UPM[0] * 10);
	RateSend[3] = Temp;
	Temp = (int)(UPM[0] * 10) >> 8;
	RateSend[4] = Temp;
	Temp = (int)(UPM[0] * 10) >> 16;
	RateSend[5] = Temp;

	// accumulated quantity, 10 X actual
	uint16_t Units = TotalPulses[0] * 10.0 / MeterCal[0];
	Temp = Units;
	RateSend[6] = Temp;
	Temp = Units >> 8;
	RateSend[7] = Temp;
	Temp = Units >> 16;
	RateSend[8] = Temp;

	//pwmSetting
	Temp = (byte)(RatePWM[0] * 10);
	RateSend[9] = Temp;
	Temp = (byte)(RatePWM[0] * 10 >> 8);
	RateSend[10] = Temp;

	// send to RateController
	UDPrate.beginPacket(AGIOip, DestinationPortRate);
	UDPrate.write(RateSend, sizeof(RateSend));
	UDPrate.endPacket();
}

void ReceiveRateUDP()
{
	uint16_t len = UDPrate.parsePacket();
	if (len > 1)
	{
		UDPrate.read(RateData, UDP_TX_PACKET_MAX_SIZE);
		PGN = RateData[1] << 8 | RateData[0];

		if (PGN == 32614 && len > 10)
		{
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
			//- bit 3		    -
			//- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
			//- bit 5           AutoOn

			uint8_t tmp = RateData[2];

			if (ParseModID(tmp) == PCB.ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID == 0)
				{
					RelayLo = RateData[3];
					RelayHi = RateData[4];

					// rate setting, 10 times actual
					uint16_t tmp = RateData[5] | RateData[6] << 8 | RateData[7] << 16;
					float TmpSet = (float)tmp * 0.1;

					// Meter Cal, 100 times actual
					tmp = RateData[8] | RateData[9] << 8;
					MeterCal[SensorID] = (float)tmp * 0.01;

					// command byte
					InCommand = RateData[10];
					if ((InCommand & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

					ControlType[SensorID] = 0;
					if ((InCommand & 2) == 2) ControlType[SensorID] += 1;
					if ((InCommand & 4) == 4) ControlType[SensorID] += 2;

					UseMultiPulses[SensorID] = ((InCommand & 16) == 16);

					AutoOn = ((InCommand & 32) == 32);
					if (AutoOn)
					{
						RateSetting[SensorID] = TmpSet;
					}
					else
					{
						NewRateFactor[SensorID] = TmpSet;
					}
					RateCommTime[SensorID] = millis();
				}
			}
		}
		else if (PGN == 32616 && len > 9)
		{
			// PID to Arduino from RateController
			uint8_t tmp = RateData[2];
			if (ParseModID(tmp) == PCB.ModuleID)
			{
				byte SensorID = ParseSenID(tmp);
				if (SensorID == 0)
				{
					PIDkp[SensorID] = RateData[3];
					PIDminPWM[SensorID] = RateData[4];
					PIDLowMax[SensorID] = RateData[5];
					PIDHighMax[SensorID] = RateData[6];
					PIDdeadband[SensorID] = RateData[7];
					PIDbrakePoint[SensorID] = RateData[8];
					AdjustTime[SensorID] = RateData[9];
				}
			}
		}
		else if (PGN == 32620 && len > 9)
		{
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
			for (int i = 0; i < 8; i++)
			{
				SwitchBytes[i] = RateData[i + 2];
			}

			// Translate Switch IDs from Rate Controller
			// ex: byte 2: bits 0-3 identify switch # (0-15) for sec 0
			// ex: byte 2: bits 4-7 identify switch # (0-15) for sec 1

			for (int i = 0; i < 16; i++)
			{
				byte ByteID = i / 2;
				byte Mask = 15 << (4 * (i - 2 * ByteID));    // move mask to correct bits
				SectionSwitchID[i] = SwitchBytes[ByteID] & Mask;    // mask out bits
				SectionSwitchID[i] = SectionSwitchID[i] >> (4 * (i - 2 * ByteID)); // move bits for number
			}
		}
		else if (PGN == 32622 && len > 12)
		{
			// pcb config
			PCB.Receiver = RateData[2];
			PCB.IMU = RateData[3];
			PCB.IMUdelay = RateData[4];
			PCB.IMU_Interval = RateData[5];
			PCB.ZeroOffset = RateData[6] | RateData[7] << 8;
			PCB.MinSpeed = RateData[8];
			PCB.MaxSpeed = RateData[9];
			PCB.PulseCal = RateData[10] | RateData[11] << 8;

			EEPROM.put(80, PCB);

			if (RateData[12])  SCB_AIRCR = 0x05FA0004; //reset the Teensy 	
		}
		else if (PGN == 32623 && len > 9)
		{
			// pcb config 2
			PCB.RTCMport = RateData[2] | RateData[3] << 8;
			PCB.AdsWASpin = RateData[4];
			PCB.ModuleID = RateData[5];
			PCB.PowerRelay = RateData[6];
			PCB.RS485PortNumber = RateData[7];

			uint8_t Commands = RateData[8];
			if (bitRead(Commands, 0)) PCB.GyroOn = 1; else PCB.GyroOn = 0;
			if (bitRead(Commands, 1)) PCB.GGAlast = 1; else PCB.GGAlast = 0;
			if (bitRead(Commands, 2)) PCB.UseRate = 1; else PCB.UseRate = 0;
			if (bitRead(Commands, 3)) PCB.UseAds = 1; else PCB.UseAds = 0;
			if (bitRead(Commands, 4)) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
			if (bitRead(Commands, 5)) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;
			if (bitRead(Commands, 6)) PCB.SwapRollPitch = 1; else PCB.SwapRollPitch = 0;
			if (bitRead(Commands, 7)) PCB.InvertRoll = 1; else PCB.InvertRoll = 0;

			EEPROM.put(80, PCB);

			if (RateData[9])  SCB_AIRCR = 0x05FA0004; //reset the Teensy   
		}
		else if (PGN == 32624 && len > 15)
		{
			// pcb pins
			PINS.SteerDir = RateData[2];
			PINS.SteerPWM = RateData[3];
			PINS.STEERSW = RateData[4];
			PINS.WAS = RateData[5];
			PINS.SteerSW_Relay = RateData[6];
			PINS.WORKSW = RateData[7];
			PINS.CurrentSensor = RateData[8];
			PINS.PressureSensor = RateData[9];
			PINS.Encoder = RateData[10];
			PINS.FlowDir = RateData[11];
			PINS.FlowPWM = RateData[12];
			PINS.SpeedPulse = RateData[13];
			PINS.RS485SendEnable = RateData[14];

			EEPROM.put(120, PCB);

			if (RateData[15])  SCB_AIRCR = 0x05FA0004; //reset the Teensy   
		}
	}
}
