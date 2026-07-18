
void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
{
	uint8_t PGNlength;
	uint16_t PGN = data[1] << 8 | data[0];

	switch (PGN)
	{
	case 32500:
		//PGN32500, Rate settings from RC to module
		//0	    HeaderLo		    244
		//1	    HeaderHi		    126
		//2     Mod/Sen ID          0-15/0-15
		//3	    rate set Lo		    1000 X actual
		//4     rate set Mid
		//5	    rate set Hi
		//6	    Flow Cal Lo	        1000 X actual
		//7     Flow Cal Mid
		//8     Flow Cal Hi
		//9	    Command
		//	        - bit 0		    reset acc.Quantity
		//	        - bit 1,2,3		control type 0-4
		//	        - bit 4		    MasterOn
		//          - bit 5         -
		//          - bit 6         AutoOn
		//          - bit 7         calibration on
		//10    manual pwm Lo
		//11    manual pwm Hi
		//12    -
		//13    CRC

		PGNlength = 14;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				if (ParseModID(data[2]) == MDL.ID)
				{
					byte SensorID = ParseSenID(data[2]);
					if (SensorID < MDL.SensorCount)
					{
						// rate setting, 1000 times actual
						uint32_t RateSet = data[3] | (uint32_t)data[4] << 8 | (uint32_t)data[5] << 16;
						Sensor[SensorID].TargetUPM = (float)(RateSet * 0.001);

						// Meter Cal, 1000 times actual
						uint32_t Temp = data[6] | (uint32_t)data[7] << 8 | (uint32_t)data[8] << 16;
						Sensor[SensorID].MeterCal = Temp * 0.001;

						// command byte
						byte InCommand = data[9];
						if ((InCommand & 1) == 1) Sensor[SensorID].TotalPulses = 0;	// reset accumulated count

						Sensor[SensorID].ControlType = 0;
						if ((InCommand & 2) == 2) Sensor[SensorID].ControlType += 1;
						if ((InCommand & 4) == 4) Sensor[SensorID].ControlType += 2;
						if ((InCommand & 8) == 8) Sensor[SensorID].ControlType += 4;

						MasterOn = ((InCommand & 16) == 16);

						AutoOn[SensorID] = ((InCommand & 64) == 64);	// per-sensor: this packet's bit only affects this sensor

						CalibrationOn[SensorID] = ((InCommand & 128) == 128);

						int16_t tmp = data[10] | data[11] << 8;
						Sensor[SensorID].ManualAdjust = tmp;

						Sensor[SensorID].CommTime = millis();
					}
				}
			}
		}
		break;

	case 32501:
		//PGN32501, Relay settings from RC to module
		//0	    HeaderLo		    245
		//1	    HeaderHi		    126
		//2     Module ID
		//3	    relay Lo		    0-7
		//4 	relay Hi		    8-15
		//5     power relay Lo      list of power type relays 0-7
		//6     power relay Hi      list of power type relays 8-15
		//7     Inverted Lo         
		//8     Inverted Hi
		//9     FlowMasterValveIndex    0-15, 255 disabled
		//10    CRC

		PGNlength = 11;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				if (ParseModID(data[2]) == MDL.ID)
				{
					RelayLo = data[3];
					RelayHi = data[4];
					PowerRelayLo = data[5];
					PowerRelayHi = data[6];
					InvertedLo = data[7];
					InvertedHi = data[8];
					FlowMasterValveIndex = data[9];
				}
			}
		}
		break;

	case 32502:
		// PGN32502, Control settings from RC to module
		// 0    246
		// 1    126
		// 2    Mod/Sen ID     0-15/0-15
		// 3    MaxPWM
		// 4    MinPWM
		// 5    Kp
		// 6    Ki
		// 7    Deadband        %       actual X 10
		// 8    Brakepoint      %
		// 9    PIDslowAdjust   %
		// 10   Slew Rate
		// 11   Max Integral      actual X 10
		// 12   -
		// 13   TimedMinStart
		// 14   TimedAdjust Lo
		// 15   TimedAdjust Hi
		// 16   TimedPause Lo
		// 17   TimedPause Hi
		// 18   PIDtime
		// 19   PulseMinHz              actual X 10
		// 20   PulseMaxHz Lo
		// 21   PulseMaxHz Hi
		// 22   PulseSampleSize
		// 23   CRC

		PGNlength = 24;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				if (ParseModID(data[2]) == MDL.ID)
				{
					byte SensorID = ParseSenID(data[2]);
					if (SensorID < MDL.SensorCount)
					{
						Sensor[SensorID].MaxPWM = (255.0 * data[3] / 100.0);
						Sensor[SensorID].MinPWM = (255.0 * data[4] / 100.0);

						// Normalized PID: Kp/Ki are dimensionless (fraction of ref flow -> fraction of PWM
						// authority). Uniform /100 decode: slider 0-100 -> 0.00-1.00. Per-actuator
						// scaling (valve vs motor) is applied in PID.ino, not here.
						if (data[5] > 0)
						{
							Sensor[SensorID].Kp = data[5] / 100.0;
						}
						else
						{
							Sensor[SensorID].Kp = 0;
						}

						if (data[6] > 0)
						{
							Sensor[SensorID].Ki = data[6] / 100.0;
						}
						else
						{
							Sensor[SensorID].Ki = 0;
						}

						Sensor[SensorID].Deadband = data[7] / 1000.0;
						Sensor[SensorID].BrakePoint = data[8];
						Sensor[SensorID].PIDslowAdjust = data[9];
						Sensor[SensorID].SlewRate = data[10];
						Sensor[SensorID].MaxIntegral = data[11] / 10.0;
						Sensor[SensorID].TimedMinStart = data[13] / 100.0;
						Sensor[SensorID].TimedAdjust = data[14] | data[15] << 8;
						Sensor[SensorID].TimedPause = data[16] | data[17] << 8;
						Sensor[SensorID].PIDtime = data[18];

						byte MinHz = data[19];
						if (MinHz > 0) Sensor[SensorID].PulseMax = 10000000 / MinHz;	//Hz * 10 to micros, minimum Hz - maximum pulse time

						uint16_t MaxHz = data[20] | data[21] << 8;
						if (MaxHz > 0) Sensor[SensorID].PulseMin = 1000000 / MaxHz;

						// byte 22 is the flow window in centiseconds on Teensy/ESP32; the Nano has no
						// room for pulse timestamps, so it uses the value as a pulse-count cap for the
						// median instead (the app default 40 clamps to all 11 slots = max smoothing).
						// Clamped >= 1 so the value can never zero the median.
						Sensor[SensorID].PulseSampleSize = constrain(data[22], 1, MaxSampleSize);

						SaveData();
					}
				}
			}
		}
		break;

	case 32505:
		// PGN32505, max pressure gate threshold (raw ADC counts, 0xFFFF = disabled)
		//0		HeaderLo	249
		//1		HeaderHi	126
		//2		ModuleID	0-7
		//3		MaxLo
		//4		MaxHi
		//5		CRC

		PGNlength = 6;

		if (len > PGNlength - 1)
		{
			// module-level PGN: data[2] is the raw module ID (like 32700/32401), not Mod/Sen packed
			if (GoodCRC(data, PGNlength) && data[2] == MDL.ID)
			{
				MDL.MaxPressureReading = data[3] | (uint16_t)data[4] << 8;
				SaveData();
			}
		}
		break;

	case 32503:
		//PGN32503, Subnet change
		//0     HeaderLo    247
		//1     HeaderHI    126
		//2     IP 0
		//3     IP 1
		//4     IP 2
		//5     CRC

		PGNlength = 6;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength))
			{
				MDLnetwork.IP0 = data[2];
				MDLnetwork.IP1 = data[3];
				MDLnetwork.IP2 = data[4];

				SaveNetworks();

				// restart
				resetFunc();
			}
		}
		break;

	case 32507:
		// PGN32507, sensor pins from RC, one packet per sensor
		//0		HeaderLo	251
		//1		HeaderHi	126
		//2		Mod/Sen ID
		//3		flow pin
		//4		dir pin
		//5		pwm pin
		//6		bin sensor pin		255 = no bin alarm
		//7		flags				bit 0 - invert bin sensor
		//8		spare
		//9		spare
		//10	CRC

		PGNlength = 11;

		if (len > PGNlength - 1)
		{
			if (GoodCRC(data, PGNlength) && ParseModID(data[2]) == MDL.ID)
			{
				byte SenID = ParseSenID(data[2]);
				if (SenID < MaxProductCount)
				{
					if (PinAllowed(data[3]) && PinAllowed(data[4])
						&& PinAllowed(data[5]) && PinAllowed(data[6]))
					{
						bool BinInv = ((data[7] & 1) == 1);
						bool Changed = (Sensor[SenID].FlowPin != data[3])
							|| (Sensor[SenID].DirPin != data[4])
							|| (Sensor[SenID].PWMPin != data[5])
							|| (Sensor[SenID].BinPin != data[6])
							|| (Sensor[SenID].BinInvert != BinInv);

						if (Changed)
						{
							Sensor[SenID].FlowPin = data[3];
							Sensor[SenID].DirPin = data[4];
							Sensor[SenID].PWMPin = data[5];
							Sensor[SenID].BinPin = data[6];
							Sensor[SenID].BinInvert = BinInv;

							SaveData();
							RestartPending = true;	// deferred - more sensor packets may follow
						}
						RestartLastConfig = millis();
					}
					else
					{
						// pins not usable on this board - discard the packet,
						// report via PGN 32401 byte 13 bit 7 for the app to alert
						ConfigRejected = true;
						ConfigRejectedTime = millis();
					}
				}
			}
		}
		break;

	case 32700:
		// module config
		//0     HeaderLo    188
		//1     HeaderHi    127
		//2     Module ID   0-15
		//3	    sensor count
		//4     commands
		//      bit 0 - Relay on high
		//      bit 1 - Flow on high
		//      bit 2 - client mode
		//      bit 3 - work pin is momentary
		//      bit 4 - Is3Wire valve
		//      bit 6 - assign module ID: adopt data[2] as the new ID (only one
		//              board connected); clear = data[2] is a filter, config is
		//              for that module only
		//5	    relay control type   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
		//                           , 5 - PCA9685, 6 - PCF8574
		//6	    wifi module serial port
		//7	    Sensor 0, Flow pin
		//8     Sensor 0, Dir pin
		//9     Sensor 0, PWM pin
		//10    Sensor 1, Flow pin
		//11    Sensor 1, Dir pin
		//12    Sensor 1, PWM pin
		//13    Relay pins 0-15, bytes 13-28
		//29    work pin
		//30    pressure pin
		//31    -
		//32    CRC

		PGNlength = 33;

		if (len > PGNlength - 1)
		{
			// bit 6 set = ID assignment, adopt data[2] unconditionally (commissioning,
			// one board connected); clear = data[2] must match our ID (normal update,
			// multi-board safe). MDL.ID = data[2] below is a no-op in filtered mode.
			bool AssignID = ((data[4] & 64) == 64);
			if (GoodCRC(data, PGNlength) && (AssignID || data[2] == MDL.ID))
			{
				bool PinsOK = PinAllowed(data[7]) && PinAllowed(data[8]) && PinAllowed(data[9])
					&& PinAllowed(data[10]) && PinAllowed(data[11]) && PinAllowed(data[12])
					&& PinAllowed(data[29]) && PinAllowed(data[30]);

				if (PinsOK && data[5] == 1)
				{
					// relay control by GPIOs
					for (int i = 0; i < 16; i++)
					{
						if (!PinAllowed(data[13 + i]))
						{
							PinsOK = false;
							break;
						}
					}
				}

				if (PinsOK)
				{
					MDL.ID = data[2];
					MDL.SensorCount = data[3];

					byte tmp = data[4];
					MDL.InvertRelay = ((tmp & 1) == 1);
					MDL.InvertFlow = ((tmp & 2) == 2);
					MDL.WorkPinIsMomentary = ((tmp & 8) == 8);
					MDL.Is3Wire = ((tmp & 16) == 16);
					MDL.ADS1115Enabled = ((tmp & 32) == 32);
					MDL.InvertWork = ((tmp & 128) == 128);

					MDL.RelayControl = data[5];
					Sensor[0].FlowPin = data[7];
					Sensor[0].DirPin = data[8];
					Sensor[0].PWMPin = data[9];
					Sensor[1].FlowPin = data[10];
					Sensor[1].DirPin = data[11];
					Sensor[1].PWMPin = data[12];

					for (int i = 0; i < 16; i++)
					{
						MDL.RelayControlPins[i] = data[13 + i];
					}

					MDL.WorkPin = data[29];
					MDL.PressurePin = data[30];

					SaveData();

					// restart
					resetFunc();
				}
				else
				{
					// pins not usable on this board - discard the config,
					// report via PGN 32401 byte 13 bit 7 for the app to alert
					ConfigRejected = true;
					ConfigRejectedTime = millis();
				}
			}
		}
		break;
	}
}

