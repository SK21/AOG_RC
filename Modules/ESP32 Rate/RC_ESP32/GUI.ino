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
	bool OldMode = MDLnetwork.WifiModeUseStation;
	String OldSSID = String(MDLnetwork.SSID);
	String OldPassword = String(MDLnetwork.Password);

	String newSSID = server.arg("prop1");
	newSSID.trim();  
	String newPassword = server.arg("prop2");
	newPassword.trim();

	newSSID.toCharArray(MDLnetwork.SSID, sizeof(MDLnetwork.SSID));
	newPassword.toCharArray(MDLnetwork.Password, sizeof(MDLnetwork.Password));
	MDLnetwork.WifiModeUseStation = server.hasArg("connect");

	server.send(200, "text/html", GetPage0());

	if (MDLnetwork.WifiModeUseStation != OldMode ||
		String(MDLnetwork.SSID) != OldSSID ||
		String(MDLnetwork.Password) != OldPassword)
	{
		SaveNetworks();
		delay(3000);
		ESP.restart();
	}
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


