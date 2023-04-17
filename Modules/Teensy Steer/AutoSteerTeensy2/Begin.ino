int16_t StoredID = 0;

void DoSetup()
{
	// watchdog timer
	WDT_timings_t config;
	config.timeout = 60;	// seconds
	wdt.begin(config);

	pinMode(LED_BUILTIN, OUTPUT);

	Serial.begin(38400);
	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	// eeprom data
	EEPROM.get(0, StoredID);              // read identifier
	if (StoredID == InoID)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(10, steerSettings);
		EEPROM.get(40, steerConfig);
		EEPROM.get(110, MDL);
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(0, InoID);
		EEPROM.put(10, steerSettings);
		EEPROM.put(40, steerConfig);
		EEPROM.put(110, MDL);
	}

	// on-board motor controller
	digitalWrite(10, MDL.UseTB6612);	// enable motor controller

	// receive data from gps receiver
	switch (MDL.SerialReceiveGPS)
	{
	case 1:
		SerialNMEA = &Serial1;
		break;
	case 2:
		SerialNMEA = &Serial2;
		break;
	case 3:
		SerialNMEA = &Serial3;
		break;
	case 4:
		SerialNMEA = &Serial4;
		break;
	case 5:
		SerialNMEA = &Serial5;
		break;
	case 6:
		SerialNMEA = &Serial6;
		break;
	case 7:
		SerialNMEA = &Serial7;
		break;
	default:
		SerialNMEA = &Serial8;
		break;
	}

	// send gps corrections to receiver
	switch (MDL.SerialSendNtrip)
	{
	case 1:
		SerialNtrip = &Serial1;
		break;
	case 2:
		SerialNtrip = &Serial2;
		break;
	case 3:
		SerialNtrip = &Serial3;
		break;
	case 4:
		SerialNtrip = &Serial4;
		break;
	case 5:
		SerialNtrip = &Serial5;
		break;
	case 6:
		SerialNtrip = &Serial6;
		break;
	case 7:
		SerialNtrip = &Serial7;
		break;
	default:
		SerialNtrip = &Serial8;
		break;
	}

	// Wemos D1 Mini serial port
	switch (MDL.WemosSerialPort)
	{
	case 1:
		SerialWemos = &Serial1;
		break;
	case 2:
		SerialWemos = &Serial2;
		break;
	case 3:
		SerialWemos = &Serial3;
		break;
	case 4:
		SerialWemos = &Serial4;
		break;
	case 5:
		SerialWemos = &Serial5;
		break;
	case 6:
		SerialWemos = &Serial6;
		break;
	case 7:
		SerialWemos = &Serial7;
		break;
	default:
		SerialWemos = &Serial8;
		break;
	}

	SerialWemos->begin(115200);

	static char SerialWemosSendBuffer[512];
	static char SerialWemosReadBuffer[512];
	SerialWemos->addMemoryForWrite(SerialWemosSendBuffer, 512);
	SerialWemos->addMemoryForRead(SerialWemosReadBuffer, 512);

	if (MDL.Receiver != 0)
	{
		SerialNtrip->begin(115200);	// RTCM
		SerialNMEA->begin(115200);

		parser.setErrorHandler(errorHandler);
		parser.addHandler("G-GGA", GGA_Handler);
		parser.addHandler("G-VTG", VTG_Handler);

		static char NMEAreceiveBuffer[512];
		static char NMEAsendBuffer[512];
		static char RTCMreceiveBuffer[512];
		static char RTCMsendBuffer[512];

		SerialNMEA->addMemoryForRead(NMEAreceiveBuffer, 512);
		SerialNMEA->addMemoryForWrite(NMEAsendBuffer, 512);
		SerialNtrip->addMemoryForRead(RTCMreceiveBuffer, 512);
		SerialNtrip->addMemoryForWrite(RTCMsendBuffer, 512);
	}

	// pins
	pinMode(MDL.Encoder, INPUT_PULLUP);
	pinMode(MDL.WorkSw, INPUT_PULLUP);
	pinMode(MDL.SteerSw, INPUT_PULLUP);
	pinMode(MDL.SteerSw_Relay, OUTPUT);
	pinMode(MDL.Dir1, OUTPUT);
	pinMode(MDL.PWM1, OUTPUT);
	pinMode(MDL.Dir2, OUTPUT);
	pinMode(MDL.PWM2, OUTPUT);
	pinMode(MDL.SpeedPulse, OUTPUT);

	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000);	//Increase I2C data rate to 400kHz

	// ADS1115
	Serial.println("Starting ADS1115 ...");
	ErrorCount = 0;
	while (!ADSfound)
	{
		Wire.beginTransmission(ADS1115_Address);
		Wire.write(0b00000000);	//Point to Conversion register
		Wire.endTransmission();
		Wire.requestFrom(ADS1115_Address, 2);
		ADSfound = Wire.available();
		Serial.print(".");
		delay(500);
		if (ErrorCount++ > 10) break;
	}
	Serial.println("");
	if (ADSfound)
	{
		Serial.println("ADS1115 connected.");
		Serial.println("");
		MDL.AnalogMethod = 0;
	}
	else
	{
		Serial.println("ADS1115 not found.");
		Serial.println("");

		switch (MDL.AnalogMethod)
		{

		case 1:
			Serial.println("Using Teensy pins for analog data.");
			Serial.println("");
			break;

		case 2:
			Serial.println("Using Wemos D1 Mini for analog data.");
			Serial.println("");
			break;
		}
	}

	// ethernet 
	Serial.println("Starting Ethernet ...");
	IPAddress LocalIP(MDL.IP0, MDL.IP1, MDL.IP2, MDL.IP3);
	static uint8_t LocalMac[] = { 0x00,0x00,0x56,0x00,0x00,MDL.IP3 };

	Ethernet.begin(LocalMac, 0);
	Ethernet.setLocalIP(LocalIP);
	DestinationIP = IPAddress(MDL.IP0, MDL.IP1, MDL.IP2, 255);	// update from saved data

	Serial.print("IP Address: ");
	Serial.println(Ethernet.localIP());
	delay(1000);
	if (Ethernet.linkStatus() == LinkON)
	{
		Serial.println("Ethernet Connected.");
	}
	else
	{
		Serial.println("Ethernet Not Connected.");
	}
	Serial.println("");

	UDPsteering.begin(ListeningPort);
	UDPntrip.begin(MDL.NtripPort);
	UDPswitches.begin(ListeningPortSwitches);

	StartIMU();

	noTone(MDL.SpeedPulse);
	SteerSwitch = HIGH;

	// usb host
	myusb.begin();

	Serial.println("");
	Serial.println("Finished setup.");
	Serial.println("");
}

void StartIMU()
{
	uint8_t IMUaddress;
	// Sparkfun BNO
	Serial.println("Starting Sparkfun IMU ...");
	IMUaddress = 0x4B;
	ErrorCount = 0;
	MDL.IMU = 0;
	bool IMUstarted = false;
	while (!IMUstarted)
	{
		IMUstarted = myIMU.begin(IMUaddress, Wire);
		Serial.print(".");
		delay(500);
		if (ErrorCount++ > 10) break;
	}
	Serial.println("");

	if (IMUstarted)
	{
		MDL.IMU = 1;
	}
	else
	{
		Serial.println("Sparkfun IMU failed to start.");
		Serial.println("Starting Adafruit IMU ...");
		IMUaddress = 0x4A;
		ErrorCount = 0;
		while (!IMUstarted)
		{
			IMUstarted = myIMU.begin(IMUaddress, Wire);
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");

		if (IMUstarted)
		{
			MDL.IMU = 2;
		}
		else
		{
			Serial.println("Adafruit IMU failed to start.");
			Serial.println("Starting  CMPS14 IMU  ...");
			ErrorCount = 0;
			while (!IMUstarted)
			{
				Wire.beginTransmission(CMPS14_ADDRESS);
				IMUstarted = !Wire.endTransmission();
				Serial.print(".");
				delay(500);
				if (ErrorCount++ > 10) break;
			}

			if (IMUstarted) MDL.IMU = 3;
		}
	}

	if (MDL.IMU == 1 || MDL.IMU == 2)
	{
		if (MDL.GyroOn) myIMU.enableGyro(MDL.IMU_Interval - 1);

		myIMU.enableGameRotationVector(MDL.IMU_Interval); //Send data update every REPORT_INTERVAL in ms for BNO085

		//Retrieve the getFeatureResponse report to check if Rotation vector report is corectly enable
		if (myIMU.getFeatureResponseAvailable() == true)
		{
			if (myIMU.checkReportEnable(SENSOR_REPORTID_GAME_ROTATION_VECTOR, MDL.IMU_Interval) == false) myIMU.printGetFeatureResponse();
			Serial.println("BNO08x init succeeded.");
		}
		else
		{
			Serial.println(F("BNO08x init fails!!"));
		}
	}

	Serial.println("");
	switch (MDL.IMU)
	{
	case 1:
		Serial.println("Sparkfun IMU started.");
		break;
	case 2:
		Serial.println("Adafruit IMU started.");
		break;
	case 3:
		Serial.println("CMPS14 IMU started.");
		break;
	default:
		Serial.println("No IMU found.");
		break;
	}
}