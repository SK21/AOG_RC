void HandleRoot()
{
	// Only a genuine page load means the user is standing at the portal, and only
	// that should suspend the station retry (see Wifi.ino). This is also the
	// onNotFound handler, so it sees traffic nobody asked for — connectivity
	// probes a phone or PC repeats for as long as it sits on the hotspot, and
	// counting those would suspend retries forever. The credentials form posts to
	// '/' as well, so one check covers both the page and the submit.
	// /page0 is the Back link from the other pages — it has no handler of its
	// own and arrives here, but it is still a person pressing something.
	if (server.uri() == "/" || server.uri() == "/page0") NotePortalRequest();

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
	NotePortalRequest();
	server.send(200, "text/html", GetPage1());
}

void HandlePage2()
{
	// network
	NotePortalRequest();

	if (server.hasArg("scan"))
	{
		// Async. A blocking scan stalls loop() for ~2 s — no web server, no DNS,
		// no UDP, no PID — while trying to serve this very response.
		if (WiFi.scanComplete() != WIFI_SCAN_RUNNING) WiFi.scanNetworks(true);
		server.send(200, "text/html", GetPageScanning());
		return;
	}

	// "Retry Connection Now" — the one place a full scan is asked for rather
	// than rationed, because the user is standing here expecting a hiccup.
	if (server.hasArg("retry")) RequestNetworkRetry();

	server.send(200, "text/html", GetPage2());
}

void handleCredentials()
{
	bool OldMode = MDLnetwork.WifiModeUseStation;
	uint8_t OldChannel = MDLnetwork.StaChannelCache;
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

	// Channel cache. The scan already knows where the chosen network lives, so
	// priming it here makes the first join after the restart a directed attempt
	// rather than a full scan. Only honoured when the submitted name still
	// matches the one that was tapped — otherwise the user typed something else
	// and the channel is not theirs. A changed network with no pick clears the
	// cache rather than pointing the next attempt at the old network's channel.
	if (server.hasArg("pickch") && server.arg("pickssid") == newSSID && newSSID.length())
	{
		int ch = server.arg("pickch").toInt();
		if (ch >= 1 && ch <= 13) MDLnetwork.StaChannelCache = (uint8_t)ch;
	}
	else if (newSSID != OldSSID)
	{
		MDLnetwork.StaChannelCache = 0;
	}

	server.send(200, "text/html", GetPage0());

	bool stationChanged =
		(MDLnetwork.WifiModeUseStation != OldMode) ||
		(String(MDLnetwork.SSID) != OldSSID) ||
		(String(MDLnetwork.Password) != OldPassword) ||
		(MDLnetwork.StaChannelCache != OldChannel);

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




