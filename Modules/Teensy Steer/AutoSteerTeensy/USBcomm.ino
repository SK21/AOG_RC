
uint16_t SerialPGN;
byte SerialMSB;
byte SerialLSB;
byte SerialPacket[30];

void ReceiveSerialUSB()
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
		case 32622:
			// Teensy config
			PGNlength = 16;
			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;
				SerialPacket[0] = 110;
				SerialPacket[1] = 127;

				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}
				if (GoodCRC(SerialPacket, PGNlength))
				{
					PCB.Receiver = SerialPacket[2];
					PCB.NMEAserialPort = SerialPacket[3];
					PCB.RTCMserialPort = SerialPacket[4];
					PCB.RTCMport = SerialPacket[5] | SerialPacket[6] << 8;
					PCB.IMU = SerialPacket[7];
					PCB.IMUdelay = SerialPacket[8];
					PCB.IMU_Interval = SerialPacket[9];
					PCB.ZeroOffset = SerialPacket[10] | SerialPacket[11] << 8;
					PCB.RS485PortNumber = SerialPacket[12];
					PCB.IPpart3 = SerialPacket[13];
					PCB.WemosSerialPort = SerialPacket[14];

					EEPROM.put(110, PCB);
				}
			}
			break;

		case 32623:
			// Teensy config 2
			PGNlength = 11;
			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;
				SerialPacket[0] = 111;
				SerialPacket[1] = 127;

				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					PCB.MinSpeed = SerialPacket[2];
					PCB.MaxSpeed = SerialPacket[3];
					PCB.PulseCal = SerialPacket[4] | SerialPacket[5] << 8;
					PCB.AnalogMethod = SerialPacket[6];
					PCB.RelayControl = SerialPacket[7];
					PCB.ModuleID = SerialPacket[8];

					uint8_t Commands = SerialPacket[9];
					if (bitRead(Commands, 0)) PCB.UseRate = 1; else PCB.UseRate = 0;
					if (bitRead(Commands, 1)) PCB.UseTB6612 = 1; else PCB.UseTB6612 = 0;
					if (bitRead(Commands, 2)) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
					if (bitRead(Commands, 3)) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;
					if (bitRead(Commands, 4)) PCB.SwapRollPitch = 1; else PCB.SwapRollPitch = 0;
					if (bitRead(Commands, 5)) PCB.InvertRoll = 1; else PCB.InvertRoll = 0;
					if (bitRead(Commands, 6)) PCB.GyroOn = 1; else PCB.GyroOn = 0;
					if (bitRead(Commands, 7)) PCB.UseLinearActuator = 1; else PCB.UseLinearActuator = 0;

					EEPROM.put(110, PCB);
				}
			}
			break;

		case 32624:
			// Teensy Pins, 16 bytes
			PGNlength = 16;
			if (Serial.available() > PGNlength - 3)
			{
				SerialPGN = 0;
				SerialPacket[0] = 112;
				SerialPacket[1] = 127;

				for (int i = 2; i < PGNlength; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, PGNlength))
				{
					PINS.Motor1Dir = SerialPacket[2];
					PINS.Motor1PWM = SerialPacket[3];
					PINS.STEERSW = SerialPacket[4];
					PINS.WAS = SerialPacket[5];
					PINS.SteerSW_Relay = SerialPacket[6];
					PINS.WORKSW = SerialPacket[7];
					PINS.CurrentSensor = SerialPacket[8];
					PINS.PressureSensor = SerialPacket[9];
					PINS.Encoder = SerialPacket[10];
					PINS.Motor2Dir = SerialPacket[11];
					PINS.Motor2PWM = SerialPacket[12];
					PINS.SpeedPulse = SerialPacket[13];
					PINS.RS485SendEnable = SerialPacket[14];

					EEPROM.put(150, PINS);

					SCB_AIRCR = 0x05FA0004; //reset the Teensy   
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


