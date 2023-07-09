
// Wemos D1 mini Pro, ESP 12F    board: LOLIN(Wemos) D1 R2 & mini  or NodeMCU 1.0 (ESP-12E Module)
# define InoDescription "WifiRC   27-Jun-2023"

#define InoID 27063  // change to load default values

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <EEPROM.h>

//bool ShowWebPage = true;
bool ShowWebPage = false;
bool ResetIno = false;

struct WifiConnection
{
    char SSID[32];
    char Password[32];
};

WifiConnection WC;

// Rate
WiFiUDP UDPrate;
uint16_t ListeningPortRate = 28888;
uint16_t DestinationPortRate = 29999;

IPAddress DestinationIP(192, 168, 1, 255);

char WifiBuffer[512];

// web page
ESP8266WebServer server(80);

byte Packet[30];

// control page
bool MasterOn = false;
bool Button[16];
byte SendByte;
byte SendBit;

byte ModuleID;
uint32_t TeensyTime;
bool TeensyConnected;

byte DebugVal1;
byte DebugVal2;

void setup()
{
    pinMode(LED_BUILTIN, OUTPUT);

    Serial.begin(115200);
    delay(5000);
    Serial.println();
    Serial.println(InoDescription);
    Serial.println();

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

void loop()
{
    ArduinoOTA.handle();
    server.handleClient();
    ReceiveSerial();
    ReceiveWifi();
    Blink();
    CheckReset();
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

        SendStatus();

        //Serial.print(" MaxLoopTime micros: ");
        //Serial.print(MaxLoopTime);

        //if (ResetRead++ > 10)
        //{
        //    MaxLoopTime = 0;
        //    ResetRead = 0;
        //}
    }
    //if (micros() - LoopTmr > MaxLoopTime) MaxLoopTime = micros() - LoopTmr;
    //LoopTmr = micros();
}

void CheckReset()
{
    if (ResetIno)
    {
        EEPROM.put(0, InoID + 1);
        EEPROM.commit();
        delay(100);
        ResetIno = 0;
        ESP.reset();
    }
}



