#if(CommType == 2)
void SendUDPWifi()
{
	// PGN 31100
	// header
	toSend[0] = 121;
	toSend[1] = 124;

	// rate applied, 100 X actual
	Temp = ((int)RateAppliedUPM * 100) >> 8;
	toSend[2] = Temp;
	Temp = (RateAppliedUPM * 100);
	toSend[3] = Temp;

	// accumulated quantity
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal)) >> 8;
	toSend[4] = Temp;
	Temp = ((int)((float)accumulatedCounts / (float)MeterCal));
	toSend[5] = Temp;

	// rate error %
	ConvertToSignedBytes(PercentError * 100);
	toSend[6] = HiByte;
	toSend[7] = LoByte;

	//off to AOG
	UDPout.beginPacket(DestinationIP, DestinationPort);
	UDPout.write(toSend, sizeof(toSend));
	UDPout.endPacket();


	// PGN 31200
	// header
	toSend[0] = 121;
	toSend[1] = 224;

	// relay Hi
	toSend[2] = 0;

	// relay Lo
	toSend[3] = RelayToAOG;

	// SecSwOff Hi
	toSend[4] = SecSwOff[1];

	// SecSwOff Lo
	toSend[5] = SecSwOff[0];

	// command byte
	toSend[6] = OutCommand;

	//off to AOG
	UDPout.beginPacket(DestinationIP, DestinationPort);
	UDPout.write(toSend, sizeof(toSend));
	UDPout.endPacket();
}

void ReceiveUDPWifi()
{
	delay(50);	// prevent wifi lockup
	int PacketSize = UDPin.parsePacket();	// get packet
	if (PacketSize)
	{
		int len = UDPin.read(InBuffer, 150);

		// PGN31300
		if ((len > 8) && (InBuffer[0] == 0x7A) && (InBuffer[1] = 0x44))
		{
			RelayHi = InBuffer[2];
			RelayFromAOG = InBuffer[3];

			// rate setting, 100 times actual
			unsignedTemp = InBuffer[4] << 8 | InBuffer[5];
			rateSetPoint = (float)unsignedTemp * 0.01;

			// Meter Cal, 100 times actual
			unsignedTemp = InBuffer[6] << 8 | InBuffer[7];
			MeterCal = (float)unsignedTemp * 0.01;

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
			UDPin.flush();	// clear buffer 
			len = 0;
		}

		//PGN31400
		if ((len > 7) && (InBuffer[0] == 0x7A) && (InBuffer[1] = 0xA8))
		{
			KP = (float)InBuffer[2] * 0.1;
			KI = (float)InBuffer[3] * 0.0001;
			KD = (float)InBuffer[4] * 0.1;
			DeadBand = (float)InBuffer[5];
			MinPWMvalue = InBuffer[6];
			MaxPWMvalue = InBuffer[7];

			//reset watchdog as we just heard from AgOpenGPS
			watchdogTimer = 0;
			AOGconnected = true;
			UDPin.flush();	// clear buffer 
		}
	}
	Serial.println("AOGconnected " + String(AOGconnected));
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
