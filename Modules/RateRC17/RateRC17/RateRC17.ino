#include <ESP2SOTA.h>		// https://github.com/pangodream/ESP2SOTA

#include <WiFi.h>
#include <WiFiUdp.h>
#include <WiFiClient.h>
#include <WiFiAP.h>

#include <WebServer.h>
#include <EEPROM.h> 
#include <Wire.h>

// rate control with ESP32	board: DOIT ESP32 DEVKIT V1  PCB: RC17
# define InoDescription "RateRC17 :  12-Nov-2024"
const uint16_t InoID = 12114;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 5;		// RateRC17

const uint8_t MCPaddress = 0x20;
#define ModStringLengths 20
#define EEPROM_SIZE 512

struct ModuleConfig
{
	uint8_t ID = 0;
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	char APname[ModStringLengths] = "RateModule";
	char APpassword[ModStringLengths] = "111222333";
	uint8_t WifiMode = 1;			// 0 AP mode, 1 Station + AP
	char SSID[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
	char Password[ModStringLengths] = "111222333";
	uint8_t WorkPin = 2;
	bool WorkPinIsMomentary = false;
	uint8_t PressurePin = 15;
	uint8_t RelaysSingle = 1;	// 0 pair of relays to control a valve, 1 Single relay to control a valve
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin = 13;
	uint8_t Motor1 = 17;
	uint8_t Motor2 = 5;
	bool FlowEnabled;
	double UPM;				// sent as upm X 1000
	double PWM;
	uint32_t CommTime;
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan, 5 timed combo
	uint32_t TotalPulses;
	double TargetUPM;
	double MeterCal;
	double ManualAdjust;
	double KP;
	double KI;
	double KD;
	byte MinPWM;
	byte MaxPWM;
	bool UseMultiPulses;	// 0 - time for one pulse, 1 - average time for multiple pulses
};

SensorConfig FlowSensor;

// network
const uint16_t ListeningPort = 28888;
const uint16_t DestinationPort = 29999;

// wifi
WiFiUDP UDP_Wifi;
IPAddress Wifi_DestinationIP(192, 168, 100, 255);
IPAddress AP_Subnet(255, 255, 255, 0);
WiFiClient client;
WebServer server(80);
#define MaxReadBuffer 100	// bytes

// Relays
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;
byte InvertedLo;	// relays that require power to close
byte InvertedHi;

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

bool MasterOn = false;
bool AutoOn = true;

bool WrkOn;
bool WrkLast;
bool WrkCurrent;
uint8_t DisconnectCount;

void WiFiStationConnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Connected to '");
	Serial.print(MDL.SSID);
	Serial.println("'");
}

void WiFiGotIP(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Network IP: ");
	Serial.println(WiFi.localIP());
	IPAddress Wifi_LocalIP = WiFi.localIP();
	Wifi_DestinationIP = IPAddress(Wifi_LocalIP[0], Wifi_LocalIP[1], Wifi_LocalIP[2], 255);
}

void WiFiStationDisconnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.println("Disconnected from WiFi access point");
	Serial.print("WiFi lost connection. Reason: ");
	Serial.println(info.wifi_sta_disconnected.reason);
	Serial.print("Trying to Reconnect: ");
	DisconnectCount++;
	Serial.println(DisconnectCount);
	WiFi.begin(MDL.SSID, MDL.Password);

	if (DisconnectCount > 10)
	{
		// use AP mode only
		MDL.WifiMode = 0;
		SaveData();
		ESP.restart();
	}
}

int TimedCombo(byte, bool);	// function prototype
void IRAM_ATTR ISR0();		// function prototype

void setup()
{
	DoSetup();
}

void loop()
{
	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		FlowSensor.FlowEnabled = (millis() - FlowSensor.CommTime < 4000)
			&& ((FlowSensor.TargetUPM > 0 && MasterOn)
				|| ((FlowSensor.ControlType == 4) && (FlowSensor.TargetUPM > 0))
				|| (!AutoOn && MasterOn));

		CheckRelays();
		GetUPM();
		AdjustFlow();
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		CheckWorkPin();
		SendUDP();
	}

	SetPWM();
	ReceiveUDP();

	server.handleClient();
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

void CheckWorkPin()
{
	WrkCurrent = digitalRead(MDL.WorkPin);
	if (MDL.WorkPinIsMomentary)
	{
		if (WrkCurrent != WrkLast)
		{
			if (WrkCurrent) WrkOn = !WrkOn;	// only cycle when going from low to high
			WrkLast = WrkCurrent;
		}
	}
	else
	{
		WrkOn = WrkCurrent;
	}
}