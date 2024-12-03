void HandleRoot()
{
	if (server.hasArg("prop1"))
	{
		handleCredentials();
	}
	else
	{
		server.send(200, "text/html", GetPage0());
	}
}

void HandlePage1()
{
	// switches
	server.send(200, "text/html", GetPage1());
}

void HandlePage2()
{
	// network
	server.send(200, "text/html", GetPage2());
}

void handleCredentials()
{
	int NewID;
	int Interval;

	server.arg("prop1").toCharArray(MDL.SSID, sizeof(MDL.SSID) - 1);
	server.arg("prop2").toCharArray(MDL.Password, sizeof(MDL.Password) - 1);
	MDL.WifiModeUseStation = true;

	server.send(200, "text/html", GetPage0());

	SaveData();

	delay(3000);

	ESP.restart();
}

void ButtonPressed()
{
	if (server.arg("Btn") == "Master")
	{
		WifiMasterOn = !WifiMasterOn;
		WifiSwitchesTimer = millis();
		HandlePage1();
	}
	else
	{
		int ID = server.arg("Btn").toInt() - 1;
		if (ID >= 0 && ID < 16)
		{
			Button[ID] = !Button[ID];
			WifiSwitchesTimer = millis();
			HandlePage1();
		}
	}
}


