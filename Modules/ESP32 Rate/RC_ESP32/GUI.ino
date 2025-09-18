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
	bool OldMode = ClientNetwork.WifiModeUseStation;
	String OldSSID = String(ClientNetwork.SSID);
	String OldPassword = String(ClientNetwork.Password);

	String newSSID = server.arg("prop1");
	newSSID.trim();  
	String newPassword = server.arg("prop2");
	newPassword.trim();

	newSSID.toCharArray(ClientNetwork.SSID, sizeof(ClientNetwork.SSID));
	newPassword.toCharArray(ClientNetwork.Password, sizeof(ClientNetwork.Password));
	ClientNetwork.WifiModeUseStation = server.hasArg("connect");

	server.send(200, "text/html", GetPage0());

	if (ClientNetwork.WifiModeUseStation != OldMode ||
		String(ClientNetwork.SSID) != OldSSID ||
		String(ClientNetwork.Password) != OldPassword)
	{
		SaveNetwork();
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


