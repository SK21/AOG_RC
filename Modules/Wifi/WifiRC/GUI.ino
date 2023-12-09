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

	server.arg("prop1").toCharArray(WC.SSID, sizeof(WC.SSID) - 1);
	server.arg("prop2").toCharArray(WC.Password, sizeof(WC.Password) - 1);

	server.send(200, "text/html", GetPage0());

	EEPROM.begin(512);
	EEPROM.put(0, InoID);
	EEPROM.put(10, WC);
	EEPROM.commit();

	delay(3000);

	ESP.restart();
}

void ButtonPressed()
{
	if (server.arg("Btn") == "Master")
	{
		MasterOn = !MasterOn;
		SendSwitches();
		HandlePage1();
	}
	else
	{
		int ID = server.arg("Btn").toInt() - 1;
		if (ID >= 0 && ID < 16)
		{
			Button[ID] = !Button[ID];
			SendSwitches();
			HandlePage1();
		}
	}
}

