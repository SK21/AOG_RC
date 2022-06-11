# define InoDescription "RCwifi  :  11-May-2022"

// used for remote section on/off to test if functional
// for Wemos D1 mini Pro,  board: LOLIN(Wemos) D1 R2 & mini
// OTA update from access point using Arduino IDE

#include <Arduino.h>
#include <ESP8266WiFi.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <ESP8266WebServer.h>
#include <WiFiClient.h>

ESP8266WebServer server(80);

const char* ssid = "Switches";
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

	Packet[0] = 107;
	Packet[1] = 127;

	Serial.println("Finished Setup");
}

void loop()
{
	ArduinoOTA.handle();
	server.handleClient();
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

void Send()
{
	// PGN32619
	// 0    107
	// 1    127
	// 2    MasterOn
	// 3	switches 0-7
	// 4	switches 8-15
	// 5	crc

	Packet[2] = MasterOn;
	Packet[3] = 0;
	Packet[4] = 0;

	// convert section switches to bits
	for (int i = 0; i < 16; i++)
	{
		SendByte = i / 8;
		SendBit = i - SendByte * 8;
		if (Button[i]) bitSet(Packet[SendByte + 3], SendBit);
	}

	// crc
	Packet[5] = CRC(5, 0);

	// send
	for (int i = 0; i < 6; i++)
	{
		Serial.write(Packet[i]);
		//Serial.print(Packet[i]);
		//if (i < 5) Serial.print(",");
		yield();
	}
	Serial.println("");
}

void handleRoot()
{
	server.send(200, "text/html", GetPage1());
}

void ButtonPressed()
{
	if (server.arg("Btn") == "Master")
	{
		MasterOn = !MasterOn;
		Send();
		handleRoot();
	}
	else
	{
		int ID = server.arg("Btn").toInt() - 1;
		if (ID >= 0 && ID < 16)
		{
			Button[ID] = !Button[ID];
			Send();
			handleRoot();
		}
	}
}

String GetPage1()
{
	String st = "<HTML>";
	st += "";
	st += "  <head>";
	st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
	st += "    <meta name=vs_targetSchema content='HTML 4.0'>";
	st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
	st += "    <title>Temp Monitor</title>";
	st += "    <style>";
	st += "      html {";
	st += "        font-family: Helvetica;";
	st += "        display: inline-block;";
	st += "        margin: 0px auto;";
	st += "        text-align: center;";
	st += "";
	st += "      }";
	st += "";
	st += "      h1 {";
	st += "        color: #444444;";
	st += "        margin: 50px auto 30px;";
	st += "      }";
	st += "";
	st += "      table.center {";
	st += "        margin-left: auto;";
	st += "        margin-right: auto;";
	st += "      }";
	st += "";
	st += "      .buttonOn {";
	st += "        background-color: #00ff00;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "      .buttonOff {";
	st += "        background-color: #ff0000;";
	st += "        border: none;";
	st += "        color: white;";
	st += "        padding: 15px 32px;";
	st += "        text-align: center;";
	st += "        text-decoration: none;";
	st += "        display: inline-block;";
	st += "        margin: 4px 2px;";
	st += "        cursor: pointer;";
	st += "        font-size: 15px;";
	st += "        width: 30%;";
	st += "      }";
	st += "";
	st += "    </style>";
	st += "  </head>";
	st += "";
	st += "  <BODY>";
	st += "    <style>";
	st += "      body {";
	st += "        margin-top: 50px;";
	st += "        background-color: DeepSkyBlue";
	st += "      }";
	st += "";
	st += "      font-family: Arial,";
	st += "      Helvetica,";
	st += "      Sans-Serif;";
	st += "";
	st += "    </style>";
	st += "";
	st += "    <h1 align=center>RateController Switches</h1>";
	st += "    <form id=FORM1 method=post action='/'>&nbsp;";
	st += "";
	st += "";

	if (MasterOn) tmp = "buttonOn"; else tmp = "buttonOff";
	st += "      <p> <input class='" + tmp + "' name='Btn' type=submit formaction='/ButtonPressed' value='Master'> </p>";

	for (int i = 0; i < 16; i++)
	{
		if(Button[i]) tmp = "buttonOn"; else tmp = "buttonOff";
		st += "      <p> <input class='" + tmp + "' name='Btn' type=submit formaction='/ButtonPressed' value='"+ String(i+1) +"'> </p>";
	}

	st += "    </form>";
	st += "";
	st += "</HTML>";

	return st;
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

