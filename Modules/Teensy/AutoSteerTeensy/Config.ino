
bool PGN32622Found;
bool PGN32623Found;
bool PGN32624Found;

//void ReceiveConfig()
//{
//	uint16_t len = UDPconfig.parsePacket();
//	if (len > 1)
//	{
//		Serial.println("Config");
//		UDPconfig.read(ConfigData, UDP_TX_PACKET_MAX_SIZE);
//		PGN = ConfigData[1] << 8 | ConfigData[0];
//
//		if (PGN == 32622 && len > 13)
//		{
//			// pcb config
//			PCB.Receiver = ConfigData[2];
//			PCB.NMEAserialPort = ConfigData[3];
//			PCB.RTCMserialPort = ConfigData[4];
//			PCB.RTCMport = ConfigData[5] | ConfigData[6] << 8;
//			PCB.IMU = ConfigData[7];
//			PCB.IMUdelay = ConfigData[8];
//			PCB.IMU_Interval = ConfigData[9];
//			PCB.ZeroOffset = ConfigData[10] | ConfigData[11] << 8;
//			PCB.RelayControl = ConfigData[12];
//			//PCB.IPpart3 = ConfigData[13];
//
//			EEPROM.put(110, PCB);
//		}
//		else if (PGN == 32623 && len > 9)
//		{
//			// pcb config 2
//			PCB.MinSpeed = ConfigData[2];
//			PCB.MaxSpeed = ConfigData[3];
//			PCB.PulseCal = ConfigData[4] | ConfigData[5] << 8;
//			PCB.AdsWASpin = ConfigData[6];
//			PCB.RS485PortNumber = ConfigData[7];
//			PCB.ModuleID = ConfigData[8];
//
//			uint8_t Commands = ConfigData[9];
//			if (bitRead(Commands, 0)) PCB.GyroOn = 1; else PCB.GyroOn = 0;
//			if (bitRead(Commands, 1)) PCB.GGAlast = 1; else PCB.GGAlast = 0;
//			if (bitRead(Commands, 2)) PCB.UseRate = 1; else PCB.UseRate = 0;
//			if (bitRead(Commands, 3)) PCB.UseAds = 1; else PCB.UseAds = 0;
//			if (bitRead(Commands, 4)) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
//			if (bitRead(Commands, 5)) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;
//			if (bitRead(Commands, 6)) PCB.SwapRollPitch = 1; else PCB.SwapRollPitch = 0;
//			if (bitRead(Commands, 7)) PCB.InvertRoll = 1; else PCB.InvertRoll = 0;
//
//			EEPROM.put(110, PCB);
//		}
//		else if (PGN == 32624 && len > 14)
//		{
//			// pcb pins
//			PINS.SteerDir = ConfigData[2];
//			PINS.SteerPWM = ConfigData[3];
//			PINS.STEERSW = ConfigData[4];
//			PINS.WAS = ConfigData[5];
//			PINS.SteerSW_Relay = ConfigData[6];
//			PINS.WORKSW = ConfigData[7];
//			PINS.CurrentSensor = ConfigData[8];
//			PINS.PressureSensor = ConfigData[9];
//			PINS.Encoder = ConfigData[10];
//			PINS.FlowDir = ConfigData[11];
//			PINS.FlowPWM = ConfigData[12];
//			PINS.SpeedPulse = ConfigData[13];
//			PINS.RS485SendEnable = ConfigData[14];
//
//			EEPROM.put(150, PINS);
//
//			SCB_AIRCR = 0x05FA0004; //reset the Teensy   
//		}
//	}
//}

void ReceiveConfig()
{
	if (Serial.available())
	{
		if (!PGN32622Found && !PGN32623Found && !PGN32624Found)
		{
			MSB = Serial.read();
			PGN = MSB << 8 | LSB;               //high,low bytes to make int
			LSB = MSB;                          //save for next time

			PGN32622Found = (PGN == 32622);
			PGN32623Found = (PGN == 32623);
			PGN32624Found = (PGN == 32624);
		}

		if (Serial.available() > 11 && PGN32622Found)
		{
			// pcb config
			PGN32622Found = false;

			PCB.Receiver = Serial.read();
			PCB.NMEAserialPort = Serial.read();
			PCB.RTCMserialPort = Serial.read();
			PCB.RTCMport = Serial.read() | Serial.read() << 8;
			PCB.IMU = Serial.read();
			PCB.IMUdelay = Serial.read();
			PCB.IMU_Interval = Serial.read();
			PCB.ZeroOffset = Serial.read() | Serial.read() << 8;
			PCB.RelayControl = Serial.read();
			PCB.IPpart3 = Serial.read();

			EEPROM.put(110, PCB);
		}

		if (Serial.available() > 7 && PGN32623Found)
		{
			// pcb config 2
			PGN32623Found = false;

			PCB.MinSpeed = Serial.read();
			PCB.MaxSpeed = Serial.read();
			PCB.PulseCal = Serial.read() | Serial.read() << 8;
			PCB.AdsWASpin = Serial.read();
			PCB.RS485PortNumber = Serial.read();
			PCB.ModuleID = Serial.read();

			uint8_t Commands = Serial.read();
			if (bitRead(Commands, 0)) PCB.UseRate = 1; else PCB.UseRate = 0;
			if (bitRead(Commands, 1)) PCB.UseAds = 1; else PCB.UseAds = 0;
			if (bitRead(Commands, 2)) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
			if (bitRead(Commands, 3)) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;
			if (bitRead(Commands, 4)) PCB.SwapRollPitch = 1; else PCB.SwapRollPitch = 0;
			if (bitRead(Commands, 5)) PCB.InvertRoll = 1; else PCB.InvertRoll = 0;
			if (bitRead(Commands, 6)) PCB.GyroOn = 1; else PCB.GyroOn = 0;

			EEPROM.put(110, PCB);
		}

		if (Serial.available() > 12 && PGN32624Found)
		{
			// pcb pins
			PGN32624Found = false;

			PINS.SteerDir = Serial.read();
			PINS.SteerPWM = Serial.read();
			PINS.STEERSW = Serial.read();
			PINS.WAS = Serial.read();
			PINS.SteerSW_Relay = Serial.read();
			PINS.WORKSW = Serial.read();
			PINS.CurrentSensor = Serial.read();
			PINS.PressureSensor = Serial.read();
			PINS.Encoder = Serial.read();
			PINS.FlowDir = Serial.read();
			PINS.FlowPWM = Serial.read();
			PINS.SpeedPulse = Serial.read();
			PINS.RS485SendEnable = Serial.read();

			EEPROM.put(150, PINS);

			SCB_AIRCR = 0x05FA0004; //reset the Teensy   
		}
	}
}
