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
	MDL.WifiMode = 1;

	server.send(200, "text/html", GetPage0());

	SaveData();

	delay(3000);

	ESP.restart();
}




