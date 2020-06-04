#if(CommType == 2)
void SendUDPWifi()
{
	// PGN 32741
	// header
	toSend[0] = 127;
	toSend[1] = 229;

	// rate applied, 100 X actual
	Temp = ((int)FlowRate * 100) >> 8;
	toSend[2] = Temp;
	Temp = (FlowRate * 100);
	toSend[3] = Temp;

	// accumulated quantity, 3 bytes
	long Units = accumulatedCounts * 100.0 / MeterCal;
	Temp = Units >> 16;
	toSend[4] = Temp;
	Temp = Units >> 8;
	toSend[5] = Temp;
	Temp = Units;
	toSend[6] = Temp;

	// pwmSetting
	Temp = (int)((300 - pwmSetting) * 10) >> 8;	// account for negative values
	toSend[7] = Temp;
	Temp = (300 - pwmSetting) * 10;
	toSend[8] = Temp;

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
		tempHeader = (InBuffer[0] << 8) | InBuffer[1];

		// PGN 32742
		if ((len > 8) && (tempHeader == 32742))
		{
			RelayHi = InBuffer[2];
			RelayFromAOG = InBuffer[3];

			// rate setting, 100 times actual
			UnSignedTemp = InBuffer[4] << 8 | InBuffer[5];
			rateSetPoint = (float)UnSignedTemp * 0.01;

			// Meter Cal, 100 times actual
			UnSignedTemp = InBuffer[6] << 8 | InBuffer[7];
			MeterCal = (float)UnSignedTemp * 0.01;

			// command byte
			InCommand = InBuffer[8];
			if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

			ValveType = 0;
			if ((InCommand & 2) == 2) ValveType += 1;
			if ((InCommand & 4) == 4) ValveType += 2;

			SimulateFlow = ((InCommand & 8) == 8);

			//reset watchdog as we just heard from AgOpenGPS
			watchdogTimer = 0;
			AOGconnected = true;
			len = 0;
		}

		//PGN 32744
		if ((len > 9) && (tempHeader == 32744))
		{
			VCN = InBuffer[2] << 8 | InBuffer[3];
			SendTime = InBuffer[4] << 8 | InBuffer[5];
			WaitTime = InBuffer[6] << 8 | InBuffer[7];
			MaxPWMvalue = InBuffer[8];
			MinPWMvalue = InBuffer[9];

			//reset watchdog as we just heard from AgOpenGPS
			watchdogTimer = 0;
			AOGconnected = true;
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

