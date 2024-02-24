
void DoSetup()
{
	Serial.begin(38400);
	delay(5000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// defaults
	String str = "tractor";
	str.toCharArray(MDL.SSID, str.length() + 1);
	str = "111222333";
	str.toCharArray(MDL.Password, str.length() + 1);

    EEPROM.begin(512);
    int16_t StoredID;
    EEPROM.get(0, StoredID);
    if (StoredID == InoID)
    {
        Serial.println("Loading stored settings.");
        EEPROM.get(10, MDL);
    }
    else
    {
        SaveData();
    }

    // Wifi
    gotIpEventHandler = WiFi.onStationModeGotIP([](const WiFiEventStationModeGotIP& event)
    {
        Serial.println("Wifi client connected");
        Serial.print("IP address: ");
        Serial.println(WiFi.localIP());

        DestinationIP = WiFi.localIP();
        DestinationIP[3] = 255;		// change to broadcast
    });

    disconnectedEventHandler = WiFi.onStationModeDisconnected([](const WiFiEventStationModeDisconnected& event)
    {
        Serial.println("Wifi client disconnected");
    });

    Serial.println();
    Serial.printf("Connecting to %s ...\n", MDL.SSID);
    WiFi.disconnect(true);
    delay(500);
    WiFi.mode(WIFI_AP_STA);
    delay(500);
    WiFi.begin(MDL.SSID, MDL.Password);
    delay(500);

    StartOTA();

    String AP = "WifiRC " + WiFi.macAddress();
    WiFi.softAP(AP);

    UDPrate.begin(ListeningPortRate);

    // web server
    Serial.println();
    Serial.println("Starting Web Server");
    server.on("/", HandleRoot);
    server.on("/page1", HandlePage1);
    server.on("/page2", HandlePage2);
    server.on("/ButtonPressed", ButtonPressed);
    server.onNotFound(HandleRoot);
    server.begin();

    pinMode(LED_BUILTIN, OUTPUT);

    Serial.println("");
    Serial.println("Finished Setup");
    Serial.println("");
}

void SaveData()
{
    Serial.println("Updating stored settings.");
    MDL.SSID[14] = '\0';
    MDL.Password[14] = '\0';

    EEPROM.begin(512);
    EEPROM.put(0, InoID);
    EEPROM.put(10, MDL);
    EEPROM.commit();
}

