
uint16_t SerialPGN;
byte SerialMSB;
byte SerialLSB;
byte SerialPacket[30];

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
		case 32622:
			// Teensy config, 15 bytes
			if (Serial.available() > 12)
			{
				SerialPGN = 0;
				SerialPacket[0] = 110;
				SerialPacket[1] = 127;

				for (int i = 2; i < 15; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, 15))
				{
					PCB.Receiver = SerialPacket[2];
					PCB.NMEAserialPort = SerialPacket[3];
					PCB.RTCMserialPort = SerialPacket[4];
					PCB.RTCMport = SerialPacket[5] | SerialPacket[6] << 8;
					PCB.IMU = SerialPacket[7];
					PCB.IMUdelay = SerialPacket[8];
					PCB.IMU_Interval = SerialPacket[9];
					PCB.ZeroOffset = SerialPacket[10] | SerialPacket[11] << 8;
					PCB.RelayControl = SerialPacket[12];
					PCB.IPpart3 = SerialPacket[13];

					EEPROM.put(110, PCB);
				}
			}
			break;

		case 32623:
			// Teensy config 2, 10 bytes
			if (Serial.available() > 7)
			{
				SerialPGN = 0;
				SerialPacket[0] = 111;
				SerialPacket[1] = 127;

				for (int i = 2; i < 10; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, 10))
				{
					PCB.MinSpeed = SerialPacket[2];
					PCB.MaxSpeed = SerialPacket[3];
					PCB.PulseCal = SerialPacket[4] | SerialPacket[5] << 8;
					PCB.AdsWASpin = SerialPacket[6];
					PCB.RS485PortNumber = SerialPacket[7];
					PCB.ModuleID = SerialPacket[8];

					uint8_t Commands = SerialPacket[9];
					if (bitRead(Commands, 0)) PCB.UseRate = 1; else PCB.UseRate = 0;
					if (bitRead(Commands, 1)) PCB.UseAds = 1; else PCB.UseAds = 0;
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
			if (Serial.available() > 13)
			{
				SerialPGN = 0;
				SerialPacket[0] = 112;
				SerialPacket[1] = 127;

				for (int i = 2; i < 16; i++)
				{
					SerialPacket[i] = Serial.read();
				}

				if (GoodCRC(SerialPacket, 16))
				{
					PINS.SteerDir = SerialPacket[2];
					PINS.SteerPWM = SerialPacket[3];
					PINS.STEERSW = SerialPacket[4];
					PINS.WAS = SerialPacket[5];
					PINS.SteerSW_Relay = SerialPacket[6];
					PINS.WORKSW = SerialPacket[7];
					PINS.CurrentSensor = SerialPacket[8];
					PINS.PressureSensor = SerialPacket[9];
					PINS.Encoder = SerialPacket[10];
					PINS.FlowDir = SerialPacket[11];
					PINS.FlowPWM = SerialPacket[12];
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


