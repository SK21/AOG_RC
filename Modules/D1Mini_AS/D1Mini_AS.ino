# define InoDescription "D1Mini_AS  :  30-Aug-2022"

// used for remote section on/off to test if functional
// for Wemos D1 mini Pro,  board: LOLIN(Wemos) D1 R2 & mini
// OTA update from access point using Arduino IDE

#include <ArduinoOTA.h>
#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ESP8266WebServer.h>
#include <WiFiClient.h>
#include <Wire.h>

ESP8266WebServer server(80);

const char* ssid = "D1Mini_AS";
const char* password = "tractor99"; // needs to be at least 8 characters

unsigned long BlinkTime;
bool BlinkState;
bool MasterOn = false;
bool Button[16];

byte SendByte;
byte SendBit;

unsigned long LoopTime;
String tmp;
IPAddress apIP(192, 168, 4, 1);
byte Packet[30];

struct PCBanalog
{
	int16_t AIN0;	// WAS
	int16_t AIN1;	// linear actuator position or pressure sensor
	int16_t AIN2;	// current
	int16_t AIN3;
};

PCBanalog AINs;

uint8_t ErrorCount;
const int16_t AdsI2Caddress = 0x48;
bool ADSfound = false;
unsigned long LastRead;

void setup()
{
	Serial.begin(38400);
	delay(2000);
	Serial.println("");
	Serial.println(InoDescription);

	pinMode(LED_BUILTIN, OUTPUT);

	WiFi.disconnect();
	WiFi.mode(WIFI_AP);
	WiFi.softAPConfig(apIP, apIP, IPAddress(255, 255, 255, 0));
	WiFi.softAP(ssid, password);

	MDNS.begin("esp8266", WiFi.softAPIP());
	Serial.print("IP address: ");
	Serial.println(WiFi.softAPIP());

	StartOTA();

	server.on("/", handleRoot);
	server.on("/ButtonPressed", ButtonPressed);
	server.begin();
	Serial.println("HTTP server started");

	Wire.begin();			// I2C on pins SCL D1, SDA D2
	// ADS1115
	Serial.println("Starting ADS ...");
	ErrorCount = 0;
	while (!ADSfound)
	{
		Wire.beginTransmission(AdsI2Caddress);
		Wire.write(0b00000000);	//Point to Conversion register
		Wire.endTransmission();
		Wire.requestFrom(AdsI2Caddress, 2);
		ADSfound = Wire.available();
		Serial.print(".");
		delay(500);
		if (ErrorCount++ > 10) break;
	}
	Serial.println("");
	if (ADSfound)
	{
		Serial.println("ADS connected.");
		Serial.println("");
	}
	else
	{
		Serial.println("ADS not found.");
		Serial.println("");
	}

	Serial.println("Finished Setup");
}

void loop()
{
	ArduinoOTA.handle();
	server.handleClient();
	if (ADSfound)
	{
		if (millis() - LastRead > 5)
		{
			LastRead = millis();
			ReadAnalog();
			SendAnalog();
		}
	}
	Blink();
}

void Blink()
{
	if (millis() - BlinkTime > 1000)
	{
		BlinkTime = millis();
		BlinkState = !BlinkState;
		digitalWrite(LED_BUILTIN, BlinkState);
	}
}

bool GoodCRC(uint16_t Length)
{
	byte ck = CRC(Length - 1, 0);
	bool Result = (ck == Packet[Length - 1]);
	return Result;
}

byte CRC(int Length, byte Start)
{
	byte Result = 0;
	if (Length <= sizeof(Packet))
	{
		int CK = 0;
		for (int i = Start; i < Length; i++)
		{
			CK += Packet[i];
		}
		Result = (byte)CK;
	}
	return Result;
}

