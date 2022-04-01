void ReceiveConfig()
{
	uint16_t len = UDPconfig.parsePacket();
	if (len > 1)
	{
		UDPconfig.read(ConfigData, UDP_TX_PACKET_MAX_SIZE);
		PGN = ConfigData[1] << 8 | ConfigData[0];

		if (PGN == 32622 && len > 12)
		{
			// pcb config
			PCB.Receiver = ConfigData[2];
			PCB.IMU = ConfigData[3];
			PCB.IMUdelay = ConfigData[4];
			PCB.IMU_Interval = ConfigData[5];
			PCB.ZeroOffset = ConfigData[6] | ConfigData[7] << 8;
			PCB.MinSpeed = ConfigData[8];
			PCB.MaxSpeed = ConfigData[9];
			PCB.PulseCal = ConfigData[10] | ConfigData[11] << 8;

			EEPROM.put(110, PCB);

			if (ConfigData[12])  SCB_AIRCR = 0x05FA0004; //reset the Teensy 	
		}
		else if (PGN == 32623 && len > 9)
		{
			// pcb config 2
			PCB.RTCMport = ConfigData[2] | ConfigData[3] << 8;
			PCB.AdsWASpin = ConfigData[4] - 1;
			PCB.ModuleID = ConfigData[5];
			PCB.PowerRelay = ConfigData[6];
			PCB.RS485PortNumber = ConfigData[7];

			uint8_t Commands = ConfigData[8];
			if (bitRead(Commands, 0)) PCB.GyroOn = 1; else PCB.GyroOn = 0;
			if (bitRead(Commands, 1)) PCB.GGAlast = 1; else PCB.GGAlast = 0;
			if (bitRead(Commands, 2)) PCB.UseRate = 1; else PCB.UseRate = 0;
			if (bitRead(Commands, 3)) PCB.UseAds = 1; else PCB.UseAds = 0;
			if (bitRead(Commands, 4)) PCB.RelayOnSignal = 1; else PCB.RelayOnSignal = 0;
			if (bitRead(Commands, 5)) PCB.FlowOnDirection = 1; else PCB.FlowOnDirection = 0;
			if (bitRead(Commands, 6)) PCB.SwapRollPitch = 1; else PCB.SwapRollPitch = 0;
			if (bitRead(Commands, 7)) PCB.InvertRoll = 1; else PCB.InvertRoll = 0;

			EEPROM.put(110, PCB);

			if (ConfigData[9])  SCB_AIRCR = 0x05FA0004; //reset the Teensy   
		}
		else if (PGN == 32624 && len > 15)
		{
			// pcb pins
			PINS.SteerDir = ConfigData[2];
			PINS.SteerPWM = ConfigData[3];
			PINS.STEERSW = ConfigData[4];
			PINS.WAS = ConfigData[5];
			PINS.SteerSW_Relay = ConfigData[6];
			PINS.WORKSW = ConfigData[7];
			PINS.CurrentSensor = ConfigData[8];
			PINS.PressureSensor = ConfigData[9];
			PINS.Encoder = ConfigData[10];
			PINS.FlowDir = ConfigData[11];
			PINS.FlowPWM = ConfigData[12];
			PINS.SpeedPulse = ConfigData[13];
			PINS.RS485SendEnable = ConfigData[14];

			EEPROM.put(150, PCB);

			if (ConfigData[15])  SCB_AIRCR = 0x05FA0004; //reset the Teensy   
		}

	}
}
