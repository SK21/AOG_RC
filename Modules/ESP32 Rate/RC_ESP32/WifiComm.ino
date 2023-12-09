
const uint32_t ReconnectTime = 120000;	// 2 minutes, allow time for web page access
uint32_t WifiLastTime = millis() - ReconnectTime;

void CheckWifi()
{
	if (!ESPconnected && (millis() - WifiLastTime > ReconnectTime))
	{
		WifiLastTime = millis();
		ConnectWifi();
	}
}

void ConnectWifi()
{
		Serial.println("");
		Serial.print("Connecting to ");
		Serial.println(MDL.SSID);

		WiFi.mode(WIFI_MODE_APSTA);
		WiFi.onEvent(WiFiEvent);
		WiFi.begin(MDL.SSID, MDL.Password);

		Serial.println("Waiting for Wifi connection ...");
}

void WiFiEvent(WiFiEvent_t event)
{
	switch (event)
	{
	case ARDUINO_EVENT_WIFI_STA_GOT_IP:
		Serial.println("WiFi connected.");
		Serial.print("IP address: ");
		Serial.println(WiFi.localIP());
		Serial.println("");
		WifiComm.begin(WiFi.localIP(), ListeningPort);
		ESPconnected = true;

		WifiDestinationIP = WiFi.localIP();
		WifiDestinationIP[3] = 255;
		break;

	case ARDUINO_EVENT_WIFI_STA_DISCONNECTED:
		Serial.println("WiFi lost connection");
		Serial.println("");
		ESPconnected = false;
		WiFi.disconnect();		// prevent auto reconnect
		break;

	default:
		break;
	}
}
