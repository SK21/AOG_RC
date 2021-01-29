#if(CommType == 2)
void SendUDPWifi()
{
	// PGN 32613
	// header
	toSend[0] = 127;
	toSend[1] = 101;

	toSend[2] = ControllerID;

	// rate applied, 100 X actual
	Temp = (int)(FlowRate * 100) >> 8;
	toSend[3] = Temp;
	Temp = (FlowRate * 100);
	toSend[4] = Temp;

	// accumulated quantity, 3 bytes
	long Units = accumulatedCounts * 100.0 / MeterCal;
	Temp = Units >> 16;
	toSend[5] = Temp;
	Temp = Units >> 8;
	toSend[6] = Temp;
	Temp = Units;
	toSend[7] = Temp;

	// pwmSetting
	Temp = (byte)((pwmSetting * 10) >> 8);
	toSend[8] = Temp;

	Temp = (byte)(pwmSetting * 10);
	toSend[9] = Temp;

	//off to AOG
	UDPout.beginPacket(DestinationIP, DestinationPort);
	UDPout.write(toSend, sizeof(toSend));
	UDPout.endPacket();


	// PGN 32761
	// header
	toSend[0] = 127;
	toSend[1] = 249;

	// relay Hi
	toSend[5] = 0;

	// relay Lo
	toSend[6] = RelayToAOG;

	// SecSwOff Hi
	toSend[7] = SecSwOff[1];

	// SecSwOff Lo
	toSend[8] = SecSwOff[0];

	// command byte
	toSend[9] = OutCommand;

	//off to AOG
	UDPout.beginPacket(DestinationIP, DestinationPort);
	UDPout.write(toSend, sizeof(toSend));
	UDPout.endPacket();

	UDPout.flush();
}

void ReceiveUDPWifi()
{
	//delay(50);	// prevent wifi lockup
	int PacketSize = UDPin.parsePacket();	// get packet
	if (PacketSize)
	{
		int len = UDPin.read(InBuffer, 150);
		PGN = (InBuffer[0] << 8) | InBuffer[1];

		if (len > 9)
		{
			if (PGN == 32614)
			{
				byte ConID = InBuffer[2];
				if (ConID == ControllerID)
				{
					RelayHi = InBuffer[3];
					RelayFromAOG = InBuffer[4];

					// rate setting, 100 times actual
					UnSignedTemp = InBuffer[5] << 8 | InBuffer[6];
					rateSetPoint = (float)UnSignedTemp * 0.01;

					// Meter Cal, 100 times actual
					UnSignedTemp = InBuffer[7] << 8 | InBuffer[8];
					MeterCal = (float)UnSignedTemp * 0.01;

					// command byte
					InCommand = InBuffer[9];
					if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

					ValveType = 0;
					if ((InCommand & 2) == 2) ValveType += 1;
					if ((InCommand & 4) == 4) ValveType += 2;

					SimulateFlow = ((InCommand & 8) == 8);

					UseVCN = ((InCommand & 16) == 16);

					//reset watchdog as we just heard from AgOpenGPS
					watchdogTimer = 0;
					AOGconnected = true;
				}
			}

			if (PGN == 32615)
			{
				byte ConID = InBuffer[2];
				if (ConID == ControllerID)
				{
					VCN = InBuffer[3] << 8 | InBuffer[4];
					SendTime = InBuffer[5] << 8 | InBuffer[6];
					WaitTime = InBuffer[7] << 8 | InBuffer[8];
					MinPWMvalue = InBuffer[9];

					watchdogTimer = 0;
					AOGconnected = true;
				}
			}

			if (PGN == 32616)
			{
				byte ConID = InBuffer[2];
				if (ConID == ControllerID)
				{
					PIDkp = InBuffer[3];
					PIDminPWM = InBuffer[4];
					PIDLowMax = InBuffer[5];
					PIDHighMax = InBuffer[6];
					PIDdeadband = InBuffer[7];
					PIDbrakePoint = InBuffer[8];

					watchdogTimer = 0;
					AOGconnected = true;
				}
			}
		}
	}
}

void CheckWifi()
{
	if (millis() - CheckTime > 5000)
	{
		Serial.println();
		ConnectionStatus = WiFi.status();
		Serial.println("Wifi status: " + String(ConnectionStatus));
		Serial.println("RSSI: " + String(WiFi.RSSI()));

		if ((ConnectionStatus != WL_CONNECTED) || (WiFi.RSSI() <= -100) || (WiFi.RSSI() == 0))
		{
			Serial.println("Connecting to " + String(ssid));

			ConnectionStatus = WiFi.begin(ssid, pass);
			delay(5000);
			ReconnectCount++;
			ConnectedCount = 0;
		}
		else
		{
			ConnectedCount++;
			CheckTime = millis();
		}
		Serial.println("Reconnect count: " + String(ReconnectCount));
		Serial.println("Connected count: " + String(ConnectedCount));
		Serial.println("Minutes connected: " + String(ConnectedCount * 5 / 60));

		IPAddress ip = WiFi.localIP();
		Serial.print("IP Address: ");
		Serial.println(ip);
	}
}
#endif

