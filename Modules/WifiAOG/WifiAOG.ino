# define InoDescription "WifiAOG   27-Nov-2022"
// Wemos D1 mini Pro,  board: LOLIN(Wemos) D1 R2 & mini

#include <ESP8266WiFi.h>
#include <ESP8266WebServer.h>
#include <ESP8266mDNS.h>
#include <WiFiUdp.h>
#include <ArduinoOTA.h>
#include <EEPROM.h>
#include <Wire.h>


//bool ShowWebPage = true;
bool ShowWebPage = false;

struct WifiConnection
{
    char SSID[32];
    char Password[32];
};

WifiConnection WC;

struct AnalogData
{
    int16_t AIN0;	// WAS
    int16_t AIN1;	// linear actuator position or pressure sensor
    int16_t AIN2;	// current
    int16_t AIN3;
};

AnalogData AINs;

// wifi
IPAddress DestinationIP(192, 168, 1, 255);
uint16_t ListeningPortRate = 28888;
uint16_t DestinationPort = 29999;
WiFiUDP UDPwifi;

// Config port
WiFiUDP UDPconfig;
uint16_t ListeningPortConfig = 28800;

char WifiBuffer[512];

// web page
ESP8266WebServer server(80);

unsigned int dcount;
byte dcount2;

#define CheckValue 8230
int16_t DataCheck;

uint8_t ErrorCount;
const int16_t AdsI2Caddress = 0x48;
bool ADSfound = false;
unsigned long LastRead;

byte Packet[30];
unsigned long LoopTime;

// control page
bool MasterOn = false;
bool Button[16];
byte SendByte;
byte SendBit;
String tmp;
byte count;

bool TeensyConnected;   // check if teensy is connected through ethernet

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
    EEPROM.get(0, DataCheck);
    if (DataCheck == CheckValue)
    {
        EEPROM.get(10, WC);
    }
    else
    {
        EEPROM.put(0, CheckValue);
        EEPROM.put(10, WC);
        EEPROM.commit();
    }

    StartOTA();

    CheckWifi();
    String AP = "WifiRate " + WiFi.macAddress();
    WiFi.softAP(AP);

    UDPwifi.begin(ListeningPortRate);
    UDPconfig.begin(ListeningPortConfig);

    Wire.begin();			// I2C on pins SCL D1, SDA D2
    // ADS1115
    Serial.println("");
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
    }
    else
    {
        Serial.println("ADS not found.");
    }

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

    if (millis() - LoopTime > 2000)
    {
        LoopTime = millis();
        if (!TeensyConnected && (WiFi.status() != WL_CONNECTED)) CheckWifi();
    }

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
uint32_t LastBlink;
void Blink()
{
    if (millis() - BlinkTime > 1000)
    {
        State = !State;
        if (State) digitalWrite(LED_BUILTIN, HIGH);
        else digitalWrite(LED_BUILTIN, LOW);
        BlinkTime = millis();
        //Serial.print(" Loop interval (ms): ");
        //Serial.println((float)(micros() - LastBlink) / 1000.0, 3);
    }
    LastBlink = micros();
}


