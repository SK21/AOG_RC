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

	String tmp = server.arg("prop1");
	if (tmp.length() > (ModStringLengths - 1))
	{
		tmp = tmp.substring(0, (ModStringLengths - 1));
	}
	tmp.trim();
	tmp.toCharArray(MDL.SSID, tmp.length() + 1);

	tmp = server.arg("prop2");
	if (tmp.length() > (ModStringLengths - 1))
	{
		tmp = tmp.substring(0, (ModStringLengths - 1));
	}
	tmp.trim();
	tmp.toCharArray(MDL.Password, tmp.length() + 1);

	tmp = server.arg("prop3");
	if (tmp.length() > (ModStringLengths - 1))
	{
		tmp = tmp.substring(0, (ModStringLengths - 1));
	}
	tmp.trim();
	tmp.toCharArray(MDL.Name, tmp.length() + 1);

	tmp = server.arg("prop4");
	if (tmp.length() > (ModStringLengths - 1))
	{
		tmp = tmp.substring(0, (ModStringLengths - 1));
	}
	tmp.trim();
	char tmp2[ModStringLengths];
	tmp.toCharArray(tmp2, tmp.length() + 1);
	MDL.AdsAddress = atoi(tmp2);

	server.send(200, "text/html", GetPage0());

	SaveData();
	esp_restart();
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

