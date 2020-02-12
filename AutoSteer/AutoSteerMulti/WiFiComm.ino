#if (UseNano33 && UseWifi)
void CommToAOGwifi()
{
	int temp;

	//actual steer angle
	temp = (100 * steerAngleActual);
	OutBuffer[2] = (byte)(temp >> 8);
	OutBuffer[3] = (byte)(temp);

	//heading  
	temp = 9999;

#if UseSerialIMU
	temp = IMUheading;
#endif

#if UseOnBoardIMU
	if (OnBoardIMUenabled) temp = IMUheading;
#endif

	OutBuffer[4] = (byte)(temp >> 8);
	OutBuffer[5] = (byte)(temp);

    //Vehicle roll --- * 16 in degrees
	temp = 9999;

#if (UseDog2 | (UseIMUroll && UseSerialIMU))
	temp = (int)XeRoll;
#endif

#if (UseIMUroll && UseOnBoardIMU)
	if (OnBoardIMUenabled) temp = (int)XeRoll;
#endif

	OutBuffer[6] = (byte)(temp >> 8);
	OutBuffer[7] = (byte)(temp);

	//switch byte
	OutBuffer[8] = switchByte;

	//pwm value
	OutBuffer[9] = abs(pwmDrive);

	//off to AOG
	UDPout.beginPacket(ipDestination, portDestination);
	UDPout.write(OutBuffer, sizeof(OutBuffer));
	UDPout.endPacket();
}

void CommFromAOGwifi()
{
	delay(50);	// prevent wifi lockup
	int PacketSize = UDPin.parsePacket();	// get packet
	if (PacketSize)
	{
		int Len = UDPin.read(InBuffer, 150);

		// autosteer data
		if (Len > 7)
		{
			if (InBuffer[0] == 0x7F && InBuffer[1] == 0xFE)
			{
				relay = InBuffer[2];
				CurrentSpeed = InBuffer[3] / 4;

				//distance from the guidance line in mm
				distanceFromLine = (float)(InBuffer[4] << 8 | InBuffer[5]);

				//set point steer angle * 10 is sent
				steerAngleSetPoint = ((float)(InBuffer[6] << 8 | InBuffer[7])); //high low bytes 
				steerAngleSetPoint *= 0.01;

				watchdogTimer = 0;

				uTurn = 0;

				Len = 0;	// to skip autosteer settings

				UDPin.flush();	// clear buffer
			}
		}

		// autosteer settings
		if (Len > 9)
		{
			if (InBuffer[0] == 0x7F && InBuffer[1] == 0xFC)
			{
				Kp = (float)InBuffer[2] * 1.0;
				Ki = (float)InBuffer[3] * .001;
				Kd = (float)InBuffer[4] * 1.0;
				Ko = (float)InBuffer[5] * .1;

				AOGzeroAdjustment = (InBuffer[6] - 127) * 20;	// 20 times the setting displayed in AOG
				SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;
				MinPWMvalue = InBuffer[7]; //read the minimum amount of PWM for instant on
				maxIntegralValue = InBuffer[8] * 0.1;
				SteerCPD = InBuffer[9] * 2; // 2 times the setting displayed in AOG
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
