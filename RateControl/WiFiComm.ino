#if UseWifi
void CommToAOGwifi()
{
	int temp;

	// rate applied, 100 X actual
	temp = rateAppliedUPM;
	OutBuffer[2] = (byte)(temp >> 8);
	OutBuffer[3] = (byte)(temp);

	// accumulated quantiy
	temp = (int)((float)accumulatedCounts / (float)MeterCal);
	OutBuffer[4] = (byte)(temp >> 8);
	OutBuffer[5] = (byte)(temp);

	// relayLo
	OutBuffer[6] = RelayToAOG;

	// SecSwOff
	OutBuffer[7] = SecSwOff[1];
	OutBuffer[8] = SecSwOff[0];

	// OutCommand
	OutBuffer[9] = OutCommand;

	//off to AOG
	UDPout.beginPacket(ipDestination, portDestination);
	UDPout.write(OutBuffer, sizeof(OutBuffer));
	UDPout.endPacket();
}

void CommFromAOGwifi()
{
	delay(50);
	int PacketSize = UDPin.parsePacket();	// get packet
	if (PacketSize)
	{
		int Len = UDPin.read(InBuffer, 150);
		if (Len > 13)
		{
			int temp = InBuffer[0] << 8 | InBuffer[1];
			Serial.println("Header: " + String(temp));
			if (temp == 31000)
			{
				relayLo = InBuffer[2];	// read relay control from AgOpenGPS  1 -> 8

				// rate setting, 100 times actual
				unsignedTemp = InBuffer[3] << 8 | InBuffer[4];
				rateSetPoint = (float)unsignedTemp * 0.01;

				//Meter Cal, 100 times actual
				unsignedTemp = InBuffer[5] << 8 | InBuffer[6];
				MeterCal = (float)unsignedTemp * 0.01;

				// command byte
				InCommand = InBuffer[7];
				if ((InCommand & 1) == 1) accumulatedCounts = 0;	// reset accumulated count

				ValveType = 0;
				if ((InCommand & 2) == 2) ValveType += 1;
				if ((InCommand & 4) == 4) ValveType += 2;

				SimulateFlow = ((InCommand & 8) == 8);

				// PID
				KP = (float)InBuffer[8] * 0.1;
				KI = (float)InBuffer[9] * 0.0001;
				KD = (float)InBuffer[10] * 0.1;
				DeadBand = (float)InBuffer[11];
				MinPWMvalue = InBuffer[12];
				MaxPWMvalue = InBuffer[13];

				//reset watchdog as we just heard from AgOpenGPS
				watchdogTimer = 0;
				AOGconnected = true;
				UDPin.flush();	// clear buffer 
			}
		}
	}
}

void CheckWifi()
{
	if (millis() - CommTime > 5000)
	{
		Serial.println();
		ConnectionStatus = WiFi.status();
		Serial.println("Wifi status: " + String(ConnectionStatus));

		if ((ConnectionStatus != WL_CONNECTED) || (WiFi.RSSI() <= -90) || (WiFi.RSSI() == 0))
		{
			Serial.print("Connecting to ");
			Serial.println(ssid);

			ConnectionStatus = WiFi.begin(ssid, pass);
			delay(5000);
			ReconnectCount++;
			ConnectedCount = 0;
			Serial.print("RSSI: ");
			Serial.println(WiFi.RSSI());
		}
		else
		{
			ConnectedCount++;
		}
		Serial.println("Reconnect count: " + String(ReconnectCount));
		Serial.println("Connected count: " + String(ConnectedCount));
		Serial.println("Minutes connected: " + String(ConnectedCount * 5 / 60));
		CommTime = millis();
	}
}
#endif
