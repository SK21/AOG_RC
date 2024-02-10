
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
	str.toCharArray(WC.SSID, str.length() + 1);
	str = "111222333";
	str.toCharArray(WC.Password, str.length() + 1);

    EEPROM.begin(512);
    int16_t StoredID;
    EEPROM.get(0, StoredID);
    if (StoredID == InoID)
    {
        Serial.println("Loading stored settings.");
        EEPROM.get(10, WC);
    }
    else
    {
        Serial.println("Updating stored settings.");
        EEPROM.put(0, InoID);
        EEPROM.put(10, WC);
        EEPROM.commit();
    }

    ConnectWifi();
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

    Serial.println("");
    Serial.println("Finished Setup");
    Serial.println("");
}
