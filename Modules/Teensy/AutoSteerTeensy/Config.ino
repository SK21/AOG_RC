
bool PGN32622Found;
bool PGN32623Found;
bool PGN32624Found;

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
