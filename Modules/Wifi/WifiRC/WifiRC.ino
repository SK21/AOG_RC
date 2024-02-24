
#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <EEPROM.h>

// Wemos D1 mini Pro, ESP 12F    board: LOLIN(Wemos) D1 R2 & mini  or NodeMCU 1.0 (ESP-12E Module)
# define InoDescription "WifiRC   24-Feb-2024"
# define InoID 24024  // change to load default values

struct ModuleConfig
{
    char SSID[15];
    char Password[15];
};

ModuleConfig MDL;

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

uint32_t SendTime;
bool SwitchesChanged = false;

WiFiEventHandler gotIpEventHandler, disconnectedEventHandler;

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
	if (millis() - SendTime > 1000)
	{
		SendStatus();
	}
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
//uint32_t debug1;
//uint32_t debug2;
//uint32_t debug3;

void Blink()
{
	if (millis() - BlinkTime > 1000)
	{
		BlinkTime = millis();
		State = !State;
		if (State) digitalWrite(LED_BUILTIN, HIGH);
		else digitalWrite(LED_BUILTIN, LOW);

		//Serial.print("Micros: ");
		//Serial.print(MaxLoopTime);

		//Serial.print(", ");
		//Serial.print(WiFi.RSSI());

		//Serial.print(", ");
		//Serial.print(WiFi.status() == WL_CONNECTED);

		//Serial.print(", ");
		//Serial.print(MDL.SSID);

		//Serial.print(", ");
		//Serial.print(MDL.Password);

		//Serial.println("");

		//if (ResetRead++ > 10)
		//{
		//    MaxLoopTime = 0;
		//    ResetRead = 0;
		//}
	}
	//if (micros() - LoopTmr > MaxLoopTime) MaxLoopTime = micros() - LoopTmr;
	//LoopTmr = micros();
}
