byte MSB_ESP8266;
byte LSB_ESP8266;
byte MSBusb;
byte LSBusb;

uint16_t PGN_ESP8266;
uint16_t PGNusb;
uint16_t PGNethernet;

byte DataESP8266[MaxReadBuffer];
byte DataUSB[MaxReadBuffer];
byte DataEthernet[MaxReadBuffer];

byte DataOut[50];

const byte PGN32500Length = 32;
const byte PGN32501Length = 8;
const byte PGN32502Length = 7;
const byte PGN32503Length = 9;
const byte PGN32613Length = 13;
const byte PGN32614Length = 17;
const byte PGN32616Length = 18;
const byte PGN32619Length = 6;
const byte PGN32621Length = 12;

uint32_t TestWeight = 430000;

void SendData()
{
	//PGN32613 to Rate Controller from Arduino
	//0	HeaderLo		101
	//1	HeaderHi		127
	//2 Mod/Sen ID      0-15/0-15
	//3	rate applied Lo 	1000 X actual
	//4 rate applied Mid
	//5	rate applied Hi
	//6	acc.Quantity Lo		10 X actual
	//7	acc.Quantity Mid
	//8	acc.Quantity Hi
	//9 PWM Lo
	//10 PWM Hi
	//11 Status
	//12 crc

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		DataOut[0] = 101;
		DataOut[1] = 127;
		DataOut[2] = BuildModSenID(MDL.ID, i);

		// rate applied, 10 X actual
		uint32_t Applied = Sensor[i].UPM * 1000;
		DataOut[3] = Applied;
		DataOut[4] = Applied >> 8;
		DataOut[5] = Applied >> 16;

		// accumulated quantity, 10 X actual
		if (Sensor[i].MeterCal > 0)
		{
			uint32_t Units = Sensor[i].TotalPulses * 10.0 / Sensor[i].MeterCal;
			DataOut[6] = Units;
			DataOut[7] = Units >> 8;
			DataOut[8] = Units >> 16;
		}
		else
		{
			DataOut[6] = 0;
			DataOut[7] = 0;
			DataOut[8] = 0;
		}

		// pwmSetting
		DataOut[9] = Sensor[i].pwmSetting;
		DataOut[10] = Sensor[i].pwmSetting >> 8;

		// status
		// bit 0    - sensor 0 receiving rate controller data
		// bit 1    - sensor 1 receiving rate controller data
		// bit 2    - wifi rssi < -80
		// bit 3	- wifi rssi < -70
		// bit 4	- wifi rssi < -65
		// bit 5	- Ethernet connected
		// bit 6	- reset esp8266
		DataOut[11] = 0;
		if (millis()-Sensor[0].CommTime < 4000) DataOut[11] |= 0b00000001;
		if (millis()-Sensor[1].CommTime < 4000) DataOut[11] |= 0b00000010;

		// wifi
		if (ESPconnected)
		{
			if (Wifi_dBm < -80)
			{
				DataOut[11] |= 0b00000100;
			}
			else if (Wifi_dBm < -70)
			{
				DataOut[11] |= 0b00001000;
			}
			else 
			{
				DataOut[11] |= 0b00010000;
			}
		}

		if (Ethernet.linkStatus() == LinkON) DataOut[11] |= 0b00100000;

		if (ResetESP8266) DataOut[11] |= 0b01000000;

		// crc
		DataOut[12] = CRC(DataOut, PGN32613Length - 1, 0);

		// to wifi
		SerialESP8266->write(DataOut, PGN32613Length);

		// to ethernet
		if (Ethernet.linkStatus() == LinkON)
		{
			UDPcomm.beginPacket(DestinationIP, DestinationPort);
			UDPcomm.write(DataOut, PGN32613Length);
			UDPcomm.endPacket();
		}
	}

	//PGN 32621, pressures to RC
	//0    109
	//1    127
	//2    module ID
	//3    sensor 0, Lo
	//4    sensor 0, Hi
	//5    sensor 1, Lo
	//6    sensor 1, Hi
	//7    sensor 2, Lo
	//8    sensor 2, Hi
	//9    sensor 3, Lo
	//10   sensor 3, Hi
	//11   CRC

	DataOut[0] = 109;
	DataOut[1] = 127;
	DataOut[2] = MDL.ID;
	DataOut[3] = (byte)AINs.AIN0;
	DataOut[4] = (byte)(AINs.AIN0 >> 8);
	DataOut[5] = (byte)AINs.AIN1;
	DataOut[6] = (byte)(AINs.AIN1 >> 8);
	DataOut[7] = (byte)AINs.AIN2;
	DataOut[8] = (byte)(AINs.AIN2 >> 8);
	DataOut[9] = (byte)AINs.AIN3;
	DataOut[10] = (byte)(AINs.AIN3 >> 8);

	DataOut[11] = CRC(DataOut, PGN32621Length - 1, 0);

	// to wifi
	SerialESP8266->write(DataOut, PGN32621Length);

	// to ethernet
	if (Ethernet.linkStatus() == LinkON)
	{
		UDPcomm.beginPacket(DestinationIP, DestinationPort);
		UDPcomm.write(DataOut, PGN32621Length);
		UDPcomm.endPacket();
	}

	// weights
	for (int i = 0; i < MaxProductCount; i++)
	{
		if (ScaleFound[i] && scale[i].is_ready())
		{
			// PGN32501
			// 0    245
			// 1    126
			// 2    Mod/Sen ID, 0-15/0-15
			// 3    weight byte 0
			// 4    weight byte 1
			// 5    weight byte 2
			// 6    weight byte 3
			// 7    CRC

			DataOut[0] = 245;
			DataOut[1] = 126;
			DataOut[2] = BuildModSenID(MDL.ID, i);

			long tmp = scale[i].read();
			if (tmp > 0)
			{
				DataOut[3] = (byte)tmp;
				DataOut[4] = (byte)(tmp >> 8);
				DataOut[5] = (byte)(tmp >> 16);
				DataOut[6] = (byte)(tmp >> 24);

				DataOut[7] = CRC(DataOut, PGN32501Length - 1, 0);

				// to wifi
				SerialESP8266->write(DataOut, PGN32501Length);

				// to ethernet
				if (Ethernet.linkStatus() == LinkON)
				{
					UDPcomm.beginPacket(DestinationIP, DestinationPort);
					UDPcomm.write(DataOut, PGN32501Length);
					UDPcomm.endPacket();
				}
			}
		}
	}

	////test pgn
	////    PGN32501
	////0    245
	////1    126
	////2    Mod/Sen ID, 0-15/0-15
	////3    weight byte 0
	////4    weight byte 1
	////5    weight byte 2
	////6    weight byte 3
	////7    CRC

	//DataOut[0] = 245;
	//DataOut[1] = 126;
	//DataOut[2] = BuildModSenID(MDL.ID, 0);

	//DataOut[3] = (byte)TestWeight;
	//DataOut[4] = (byte)(TestWeight >> 8);
	//DataOut[5] = (byte)(TestWeight >> 16);
	//DataOut[6] = (byte)(TestWeight >> 24);

	//DataOut[7] = CRC(DataOut, PGN32501Length - 1, 0);

	//// to wifi
	//SerialESP8266->write(DataOut, PGN32501Length);
	//Debug2++;

	//// to ethernet
	//if (Ethernet.linkStatus() == LinkON)
	//{
	//	UDPcomm.beginPacket(DestinationIP, DestinationPort);
	//	UDPcomm.write(DataOut, PGN32501Length);
	//	UDPcomm.endPacket();
	//}

	//TestWeight -= 25;
	//if (TestWeight < 10)TestWeight = 450000;
}

void ReceiveData()
{
	// wifi from ESP8266
	if (SerialESP8266->available())
	{
		if (SerialESP8266->available() > MaxReadBuffer)
		{
			// clear buffer
			while (SerialESP8266->available())
			{
				SerialESP8266->read();
			}
			PGN_ESP8266 = 0;
			LSB_ESP8266 = 0;
		}

		switch (PGN_ESP8266)
		{
		case 32500:
			ProcessWifi(PGN32500Length);
			break;

		case 32502:
			ProcessWifi(PGN32502Length);
			break;

		case 32503:
			ProcessWifi(PGN32503Length);
		break;

		case 32614:
			ProcessWifi(PGN32614Length);
			break;

		case 32616:
			ProcessWifi(PGN32616Length);
			break;

		case 32619:
			ProcessWifi(PGN32619Length);
			break;

		default:
			// find pgn
			MSB_ESP8266 = SerialESP8266->read();
			PGN_ESP8266 = MSB_ESP8266 << 8 | LSB_ESP8266;
			LSB_ESP8266 = MSB_ESP8266;
			break;
		}
	}

	//ethernet
	if (Ethernet.linkStatus() == LinkON)
	{
		uint16_t len = UDPcomm.parsePacket();
		if (len)
		{
			UDPcomm.read(DataEthernet, MaxReadBuffer);
			PGNethernet = DataEthernet[1] << 8 | DataEthernet[0];
			ReadPGN(len, DataEthernet, PGNethernet);
		}
	}

	// USB
	if (Serial.available())
	{
		if (Serial.available() > MaxReadBuffer)
		{
			// clear buffer
			while (Serial.available())
			{
				Serial.available();
			}
			PGNusb = 0;
			LSBusb = 0;
		}

		switch (PGNusb)
		{
		case 32500:
			ProcessUSB(PGN32500Length);
			break;

		case 32502:
			ProcessUSB(PGN32502Length);
			break;

		case 32614:
			ProcessUSB(PGN32614Length);
			break;

		case 32616:
			ProcessUSB(PGN32616Length);
			break;

		case 32619:
			ProcessUSB(PGN32619Length);
			break;

		default:
			// find pgn
			MSBusb = Serial.read();
			PGNusb = MSBusb << 8 | LSBusb;
			LSBusb = MSBusb;
			break;
		}
	}
}

void ProcessWifi(uint16_t len)
{
	if (SerialESP8266->available() > len - 3)
	{
		DataESP8266[0] = (byte)PGN_ESP8266;
		DataESP8266[1] = (byte)(PGN_ESP8266 >> 8);
		for (int i = 2; i < len; i++)
		{
			DataESP8266[i] = SerialESP8266->read();
		}
		ReadPGN(len, DataESP8266, PGN_ESP8266);
		PGN_ESP8266 = 0;
		LSB_ESP8266 = 0;
	}
}

void ProcessUSB(uint16_t len)
{
	if (Serial.available() > len - 3)
	{
		DataUSB[0] = (byte)PGNusb;
		DataUSB[1] = (byte)(PGNusb >> 8);
		for (int i = 2; i < len; i++)
		{
			DataUSB[i] = Serial.read();
		}
		ReadPGN(len, DataUSB, PGNusb);
		PGNusb = 0;
		LSBusb = 0;
	}
}

void ReadPGN(uint16_t len, byte Data[], uint16_t PGN)
{
	switch (PGN)
	{
	case 32500:
		// module config
		// 0        244
		// 1        126
		// 2        ID
		// 3        Sensor count
		// 4        IPpart3
		// 5        Commands
		//          - Relay on high
		//          - Flow on high
		// 6        Relay control type  0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
		// 7        ESP8266 serial port
		// 8        Sensor 0, flow pin
		// 9        Sensor 0, dir pin
		// 10       Sensor 0, pwm pin
		// 11       Sensor 1, flow pin
		// 12       Sensor 1, dir pin
		// 13       Sensor 1, pwm pin
		// 14-29    Relay pins 0-15\
		// 30		debounce minimum ms
		// 31       CRC

		if (len > PGN32500Length - 1)
		{
			if (GoodCRC(Data, PGN32500Length))
			{
				MDL.ID = Data[2];
				MDL.SensorCount = Data[3];
				MDL.IPpart3 = Data[4];

				byte tmp = Data[5];
				if ((tmp & 1) == 1) MDL.RelayOnSignal = 1; else MDL.RelayOnSignal = 0;
				if ((tmp & 2) == 2) MDL.FlowOnDirection = 1; else MDL.FlowOnDirection = 0;

				MDL.RelayControl = Data[6];
				MDL.ESP8266SerialPort = Data[7];
				Sensor[0].FlowPin = Data[8];
				Sensor[0].DirPin = Data[9];
				Sensor[0].PWMPin = Data[10];
				Sensor[1].FlowPin = Data[11];
				Sensor[1].DirPin = Data[12];
				Sensor[1].PWMPin = Data[13];

				for (int i = 0; i < 16; i++)
				{
					MDL.RelayPins[i] = Data[14 + i];
				}

				// save
				EEPROM.put(100, InoID);
				EEPROM.put(110, MDL);

				for (int i = 0; i < MaxProductCount; i++)
				{
					EEPROM.put(200 + i * 80, Sensor[i]);
				}

				MDL.Debounce = Data[30];

				// restart the Teensy
				SCB_AIRCR = 0x05FA0004;
			}
		}
		break;

	case 32502:
		// PGN32502 IP addresses
		// 0    HeaderLo    246
		// 1    HeaderHi    126
		// 2    Ethernet IP Part2
		// 3    Ethernet IP Part3
		// 4    Wifi IP Part2
		// 5    Wifi IP Part3
		// 6    CRC

		if (len > PGN32502Length - 1)
		{
			if (GoodCRC(Data, PGN32502Length))
			{
				MDL.IPpart2 = Data[2];
				MDL.IPpart3 = Data[3];

				// save
				EEPROM.put(100, InoID);
				EEPROM.put(110, MDL);

				for (int i = 0; i < MaxProductCount; i++)
				{
					EEPROM.put(200 + i * 80, Sensor[i]);
				}

				// restart the Teensy
				SCB_AIRCR = 0x05FA0004;
			}
		}
		break;

	case 32503:
		// WifiAOG status
		// 0	247
		// 1	126
		// 2	Module ID
		// 3	dBm lo
		// 4	dBm Hi
		// 5	Status
		//		- bit 0 Wifi connected
		//		- bit 1 Teensy connected
		// 6	DebugVal1
		// 7	DebugVal2
		// 8	CRC

		if (len > PGN32503Length - 1)
		{
			if (GoodCRC(Data, PGN32503Length))
			{
				Wifi_dBm = Data[3] | Data[4] << 8;

				// Status
				ESPconnected = ((Data[5] & 1) == 1);

				ESPdebug1 = Data[6];

				WifiTime = millis() - WifiLastTime;
				WifiLastTime = millis();
			}
		}
		break;

	case 32614:
		//PGN32614 to Arduino from Rate Controller, 16 bytes
		//0	HeaderLo		102
		//1	HeaderHi		127
		//2 Controller ID
		//3	relay Lo		0 - 7
		//4	relay Hi		8 - 15
		//5	rate set Lo		1000 X actual
		//6 rate set Mid
		//7	rate set Hi		
		//8	Flow Cal Lo		1000 X actual
		//9	Flow Cal Mid
		//10 Flow Cal Hi
		//11	Command
		//	        - bit 0		    reset acc.Quantity
		//	        - bit 1,2,3		control type 0-4
		//	        - bit 4		    MasterOn, or true if no switchbox
		//          - bit 5         0 - average time for multiple pulses, 1 - time for one pulse
		//          - bit 6         AutoOn
		//          - bit 7         Calibration On
		//12    power relay Lo      list of power type relays 0-7
		//13    power relay Hi      list of power type relays 8-15
		//14	manual pwm Lo
		//15    manual pwm Hi
		//16	CRC

		if (len > PGN32614Length - 1)
		{
			if (GoodCRC(Data, PGN32614Length))
			{
				byte tmp = Data[2];

				if (ParseModID(tmp) == MDL.ID)
				{
					byte ID = ParseSenID(tmp);  // sensor ID
					if (ID < MDL.SensorCount)
					{
						RelayLo = Data[3];
						RelayHi = Data[4];

						// rate setting, 1000 times actual
						uint32_t RateSet = Data[5] | (uint32_t)Data[6] << 8 | (uint32_t)Data[7] << 16;
						Sensor[ID].RateSetting = (float)(RateSet * 0.001);

						// Meter Cal, 1000 X actual
						uint32_t Temp = Data[8] | (uint32_t)Data[9] << 8 | (uint32_t)Data[10] << 16;
						Sensor[ID].MeterCal = (float)Temp * 0.001;

						// command byte
						Sensor[ID].InCommand = Data[11];
						if ((Sensor[ID].InCommand & 1) == 1) Sensor[ID].TotalPulses = 0; // reset accumulated count

						Sensor[ID].ControlType = 0;
						if ((Sensor[ID].InCommand & 2) == 2) Sensor[ID].ControlType += 1;
						if ((Sensor[ID].InCommand & 4) == 4) Sensor[ID].ControlType += 2;
						if ((Sensor[ID].InCommand & 8) == 8) Sensor[ID].ControlType += 4;

						MasterOn = ((Sensor[ID].InCommand & 16) == 16);
						Sensor[ID].UseMultiPulses = ((Sensor[ID].InCommand & 32) == 32);
						AutoOn = ((Sensor[ID].InCommand & 64) == 64);

						// power relays
						PowerRelayLo = Data[12];
						PowerRelayHi = Data[13];

						// manual control
						int16_t tmp = Data[14] | Data[15] << 8;
						Sensor[ID].ManualAdjust = tmp;

						Sensor[ID].CommTime = millis();
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

		if (len > PGN32616Length - 1)
		{
			if (GoodCRC(Data, PGN32616Length))
			{
				byte tmp = Data[2];
				if (ParseModID(tmp) == MDL.ID)
				{
					byte ID = ParseSenID(tmp);
					if (ID < MDL.SensorCount)
					{
						uint32_t tmp = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16 | (uint32_t)Data[6] << 24;
						Sensor[ID].KP = (float)(tmp * 0.0001);

						tmp = Data[7] | (uint32_t)Data[8] << 8 | (uint32_t)Data[9] << 16 | (uint32_t)Data[10] << 24;
						Sensor[ID].KI = (float)(tmp * 0.0001);

						tmp = Data[11] | (uint32_t)Data[12] << 8 | (uint32_t)Data[13] << 16 | (uint32_t)Data[14] << 24;
						Sensor[ID].KD = (float)(tmp * 0.0001);

						Sensor[ID].MinPWM = Data[15];
						Sensor[ID].MaxPWM = Data[16];
					}
				}
			}
		}
		break;

	case 32619:
		// from ESP8266, 6 bytes
		// section buttons

		if (len > PGN32619Length - 1)
		{
			if (GoodCRC(Data, PGN32619Length))
			{
				for (int i = 2; i < 6; i++)
				{
					WifiSwitches[i] = Data[i];
				}
				WifiSwitchesEnabled = true;
				WifiSwitchesTimer = millis();
			}
		}
		break;
	}
}