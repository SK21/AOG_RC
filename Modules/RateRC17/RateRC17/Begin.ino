
void DoSetup()
{
	uint8_t ErrorCount;
	FlowSensor.FlowEnabled = false;

	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println("");
	Serial.println("");
	Serial.println(InoDescription);
	Serial.println("");

	// eeprom
	EEPROM.begin(EEPROM_SIZE);
	LoadData();

	Serial.println("");
	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
	Serial.print("Module Version: ");
	Serial.println(InoID);
	Serial.println("");

	pinMode(MDL.WorkPin, INPUT_PULLUP);

	// FlowSensor
	pinMode(FlowSensor.FlowPin, INPUT_PULLUP);
	pinMode(FlowSensor.Motor1, OUTPUT);
	pinMode(FlowSensor.Motor2, OUTPUT);
	attachInterrupt(digitalPinToInterrupt(FlowSensor.FlowPin), ISR0, RISING);
	
	// flow pwm
	// DRV8870 IN1
	ledcSetup(0, 500, 8);
	ledcAttachPin(FlowSensor.Motor1, 0);

	// DRV8870 IN2
	ledcSetup(1, 500, 8);
	ledcAttachPin(FlowSensor.Motor2, 1);

	// I2C
	Wire.begin();			// I2C on pins SCL 22, SDA 21
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// MCP23017 I/O expander on default address
	Serial.println("");
	Serial.println("Starting MCP23017 ...");
	ErrorCount = 0;
	bool MCP23017_found = false;
	while (!MCP23017_found)
	{
		Serial.print(".");
		Wire.beginTransmission(MCPaddress);
		MCP23017_found = (Wire.endTransmission() == 0);
		ErrorCount++;
		delay(500);
		if (ErrorCount > 5) break;
	}

	Serial.println("");
	if (MCP23017_found)
	{
		Wire.beginTransmission(MCPaddress);
		Wire.write(0x00); // IODIRA register
		Wire.write(0x00); // set all of port A to outputs
		Wire.endTransmission();

		Wire.beginTransmission(MCPaddress);
		Wire.write(0x01); // IODIRB register
		Wire.write(0x00); // set all of port B to outputs
		Wire.endTransmission();

		Serial.println("MCP23017 found.");
	}
	else
	{
		Serial.println("MCP23017 not found.");
	}

	// Wifi
	WiFi.mode(WIFI_MODE_APSTA);
	WiFi.disconnect(true);

	// Access Point
	IPAddress AP_LocalIP = IPAddress(192, 168, MDL.ID + 200, 1);
	Wifi_DestinationIP = IPAddress(192, 168, MDL.ID + 200, 255);

	String AP = MDL.APname;
	AP += "  (";
	AP += WiFi.macAddress();
	AP += ")";

	WiFi.softAP(AP, MDL.APpassword, 1, 0, 2);
	delay(500);
	WiFi.softAPConfig(AP_LocalIP, AP_LocalIP, AP_Subnet);
	UDP_Wifi.begin(ListeningPort);

	Serial.println("");
	Serial.print("Access Point name: ");
	Serial.println(AP);
	Serial.print("Access Point IP: ");
	Serial.println(AP_LocalIP);

	// web server
	Serial.println();
	Serial.println("Starting Web Server");
	server.on("/", HandleRoot);
	server.on("/page2", HandlePage2);
	server.onNotFound(HandleRoot);

	// OTA
	server.on("/myurl", HTTP_GET, []() {
		server.sendHeader("Connection", "close");
		server.send(200, "text/plain", "Hello there!");
	});

	server.begin();

	/* INITIALIZE ESP2SOTA LIBRARY */
	ESP2SOTA.begin(&server);

	Serial.println("OTA started.");

	// wifi client mode
	if (MDL.WifiMode == 1)
	{
		// connect to network
		delay(1000);
		WiFi.onEvent(WiFiStationConnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_CONNECTED);
		WiFi.onEvent(WiFiGotIP, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_GOT_IP);
		WiFi.onEvent(WiFiStationDisconnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_DISCONNECTED);
		WiFi.begin(MDL.SSID, MDL.Password);
		Serial.println();
		Serial.println("Connecting to wifi network ...");
	}

	delay(1500);
	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void LoadData()
{
	int16_t StoredID;
	int8_t StoredType;
	EEPROM.get(0, StoredID);
	EEPROM.get(4, StoredType);
	if (StoredID == InoID && StoredType == InoType)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(110, MDL);
		EEPROM.get(300, FlowSensor);
	}
}

void SaveData()
{
	// update stored data
	Serial.println("Updating stored settings.");
	EEPROM.put(0, InoID);
	EEPROM.put(4, InoType);
	EEPROM.put(110, MDL);
	EEPROM.put(300, FlowSensor);
	EEPROM.commit();
}
