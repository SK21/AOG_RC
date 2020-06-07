#if (CommType == 2)
void SendUDPWifi()
{
	// PGN 32765
	int temp;

	//actual steer angle
	temp = (100 * steerAngleActual);
	OutBuffer[2] = (byte)(temp >> 8);
	OutBuffer[3] = (byte)(temp);

	// heading
	temp = IMUheading * 16;
	OutBuffer[4] = (byte)(temp >> 8);
	OutBuffer[5] = (byte)(temp);

	// Roll
	temp = FilteredRoll * 16;
	OutBuffer[6] = (byte)(temp >> 8);
	OutBuffer[7] = (byte)(temp);

	//switch byte
	OutBuffer[8] = switchByte;

	//pwm value
	OutBuffer[9] = abs(pwmDrive);

	//off to AOG
	UDPout.beginPacket(DestinationIP, DestinationPort);
	UDPout.write(OutBuffer, sizeof(OutBuffer));
	UDPout.endPacket();
}

void ReceiveUDPWifi()
{
	int PacketSize = UDPin.parsePacket();	// get packet
	if (PacketSize)
	{
		int Len = UDPin.read(InBuffer, 150);

		// autosteer data, PGN32766
		if (Len > 7)
		{
			if (InBuffer[0] == 0x7F && InBuffer[1] == 0xFE)
			{
				relay = InBuffer[2];
				CurrentSpeed = InBuffer[3] / 4.0;

				//distance from the guidance line in mm
				distanceFromLine = (float)(InBuffer[4] << 8 | InBuffer[5]);

				//set point steer angle * 10 is sent
				steerAngleSetPoint = ((float)(InBuffer[6] << 8 | InBuffer[7])); //high low bytes 
				steerAngleSetPoint *= 0.01;

				watchdogTimer = 0;

				uTurn = 0;

				Len = 0;	// to skip autosteer settings
			}
		}

		// autosteer settings, PGN32764
		if (Len > 9)
		{
			if (InBuffer[0] == 0x7F && InBuffer[1] == 0xFC)
			{
				Kp = (float)InBuffer[2] * 1.0;
				LowMaxPWM = InBuffer[3];
				AOGzeroAdjustment = (InBuffer[6] - 127) * 20;	// 20 times the setting displayed in AOG
				MinPWMvalue = InBuffer[7]; //read the minimum amount of PWM for instant on
				HighMaxPWM = InBuffer[8];
				SteerCPD = InBuffer[9] * 2; // 2 times the setting displayed in AOG
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


