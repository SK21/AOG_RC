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
	String OldAPPassword = String(MDL.APpassword);

	String newSSID = server.arg("prop1");
	newSSID.trim();
	String newPassword = server.arg("prop2");
	newPassword.trim();

	// Hotspot/AP password (prop3). May be empty to make AP open.
	String newAPPassword = OldAPPassword;
	if (server.hasArg("prop3"))
	{
		newAPPassword = server.arg("prop3");
		newAPPassword.trim();

		// Enforce max length 
		const size_t kMaxApLen = 10;
		if (newAPPassword.length() > kMaxApLen)
		{
			newAPPassword.remove(kMaxApLen); 
		}
	}

	newSSID.toCharArray(MDLnetwork.SSID, sizeof(MDLnetwork.SSID));
	newPassword.toCharArray(MDLnetwork.Password, sizeof(MDLnetwork.Password));
	MDLnetwork.WifiModeUseStation = server.hasArg("connect");

	// Apply AP password if provided (including empty -> open network)
	if (server.hasArg("prop3"))
	{
		newAPPassword.toCharArray(MDL.APpassword, sizeof(MDL.APpassword));
	}

	server.send(200, "text/html", GetPage0());

	bool stationChanged =
		(MDLnetwork.WifiModeUseStation != OldMode) ||
		(String(MDLnetwork.SSID) != OldSSID) ||
		(String(MDLnetwork.Password) != OldPassword);

	bool apChanged = (String(MDL.APpassword) != OldAPPassword);

	if (stationChanged) SaveNetworks();
	if (apChanged) SaveData();

	if (stationChanged || apChanged)
	{
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




