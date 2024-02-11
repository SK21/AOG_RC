
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <EEPROM.h>

// Wemos D1 mini Pro, ESP 12F    board: LOLIN(Wemos) D1 R2 & mini  or NodeMCU 1.0 (ESP-12E Module)
# define InoDescription "WifiRC   10-Feb-2024"
# define InoID 10024  // change to load default values

struct WifiConnection
{
    char SSID[32];
    char Password[32];
};

WifiConnection WC;

// web page
ESP8266WebServer server(80);

// control page
bool MasterOn = false;
bool Button[16];
byte SendByte;
byte SendBit;

// Rate
WiFiUDP UDPrate;
uint16_t ListeningPortRate = 28888;
uint16_t DestinationPortRate = 29999;
IPAddress DestinationIP(192, 168, 1, 255);
char WifiBuffer[512];

void setup()
{
	DoSetup();
}

void loop()
{
	ArduinoOTA.handle();
	server.handleClient();
	ReceiveWifi();
	ReceiveSerial();
	Blink();
}

byte ParseModID(byte ID)
{
	// top 4 bits
	return ID >> 4;
}

byte ParseSenID(byte ID)
{
	// bottom 4 bits
	return (ID & 0b00001111);
}

byte BuildModSenID(byte Mod_ID, byte Sen_ID)
{
	return ((Mod_ID << 4) | (Sen_ID & 0b00001111));
}

bool GoodCRC(byte Data[], byte Length)
{
	byte ck = CRC(Data, Length - 1, 0);
	bool Result = (ck == Data[Length - 1]);
	return Result;
}

byte CRC(byte Chk[], byte Length, byte Start)
{
	byte Result = 0;
	int CK = 0;
	for (int i = Start; i < Length; i++)
	{
		CK += Chk[i];
	}
	Result = (byte)CK;
	return Result;
}

bool State = 0;
uint32_t BlinkTime;
uint32_t LoopTmr;
uint32_t MaxLoopTime;
byte ResetRead;

void Blink()
{
	if (millis() - BlinkTime > 1000)
	{
		BlinkTime = millis();
		State = !State;
		if (State) digitalWrite(LED_BUILTIN, HIGH);
		else digitalWrite(LED_BUILTIN, LOW);

		Serial.print("Micros: ");
		Serial.print(MaxLoopTime);

		Serial.println("");

		if (ResetRead++ > 10)
		{
		    MaxLoopTime = 0;
		    ResetRead = 0;
		}
	}
	if (micros() - LoopTmr > MaxLoopTime) MaxLoopTime = micros() - LoopTmr;
	LoopTmr = micros();
}
