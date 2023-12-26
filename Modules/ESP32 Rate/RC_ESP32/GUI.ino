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
	int Val;

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


	Val = WebInputValue(server.arg("prop4"));
	if (Val >= 0 && Val <= 50) MDL.AdsAddress = Val;

	Val = WebInputValue(server.arg("prop5"));
	if (Val >= 0 && Val <= 50) Sensor[0].FlowPin = Val;

	Val = WebInputValue(server.arg("prop6"));
	if (Val >= 0 && Val <= 50) Sensor[0].DirPin = Val;

	Val = WebInputValue(server.arg("prop7"));
	if (Val >= 0 && Val <= 50) Sensor[0].PWMPin = Val;

	Val = WebInputValue(server.arg("prop8"));
	if (Val >= 0 && Val <= 50) Sensor[1].FlowPin = Val;

	Val = WebInputValue(server.arg("prop9"));
	if (Val >= 0 && Val <= 50) Sensor[1].DirPin = Val;

	Val = WebInputValue(server.arg("prop10"));
	if (Val >= 0 && Val <= 50) Sensor[1].PWMPin = Val;

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

int WebInputValue(String tmp)
{
	if (tmp.length() > (ModStringLengths - 1))
	{
		tmp = tmp.substring(0, (ModStringLengths - 1));
	}
	tmp.trim();
	char tmp2[ModStringLengths];
	tmp.toCharArray(tmp2, tmp.length() + 1);
	return atoi(tmp2);
}

