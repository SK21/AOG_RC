
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5
#include <pcf8574.h>		// https://github.com/RobTillaart/PCF8574
#include <ESP2SOTA.h>		// https://github.com/pangodream/ESP2SOTA

#include <WiFi.h>
#include <WiFiUdp.h>
#include <WiFiClient.h>
#include <WiFiAP.h>

#include <WebServer.h>
#include <EEPROM.h> 
#include <Wire.h>

#include <SPI.h>
#include <Ethernet.h>
#include <EthernetUdp.h>

#include <Adafruit_PWMServoDriver.h>

//#include <Adafruit_BusIO_Register.h>
//#include <Adafruit_I2CDevice.h>
//#include <Adafruit_I2CRegister.h>
//#include <Adafruit_SPIDevice.h>

// rate control with ESP32	board: DOIT ESP32 DEVKIT V1
# define InoDescription "RC_ESP32 :  01-Apr-2025"
const uint16_t InoID = 1045;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 4;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate
const uint8_t Processor = 0;	// 0 - ESP32-Wroom-32U

const uint8_t MaxReadBuffer = 100;	// bytes
const uint8_t MaxProductCount = 2;
const uint16_t EEPROM_SIZE = 512;
const uint8_t ModStringLengths = 20;

// servo driver
const uint8_t OutputEnablePin = 27;
const uint8_t PCA9685address = 0x55;	// RC15 1010101, 1 + A5-A0

const int16_t ADS1115_Address = 0x48;
uint8_t MCP23017address;
const uint8_t PCF8574address = 0x20;
const uint8_t W5500_SS = 5;		// W5500 SPI SS
const uint8_t NC = 0xFF;		// Pin not connected

struct ModuleConfig
{
	// RC15
	uint8_t ID = 0;
	uint8_t SensorCount = 1;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = false;	    // value that turns on relays
	bool InvertFlow = false;		// sets on value for flow valve or sets motor direction
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t RelayControl = 5;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	char APname[ModStringLengths] = "RateModule";
	char APpassword[ModStringLengths] = "111222333";
	bool WifiModeUseStation = false;				// false - AP mode, true - AP + Station 
	char SSID[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
	char Password[ModStringLengths] = "111222333";
	uint8_t WorkPin = NC;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output1 only, Output2 is off
	uint8_t PressurePin = NC;		// NC - no pressure pin
	bool ADS1115Enabled = true;
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin;
	uint8_t IN1;
	uint8_t IN2;
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
};

SensorConfig Sensor[2];

// network
const uint16_t ListeningPort = 28888;
const uint16_t DestinationPort = 29999;

// ethernet
EthernetUDP UDP_Ethernet;
IPAddress Ethernet_DestinationIP(MDL.IP0, MDL.IP1, MDL.IP2, 255);
bool ChipFound;

// wifi
WiFiUDP UDP_Wifi;
IPAddress Wifi_DestinationIP(192, 168, 100, 255);
IPAddress AP_Subnet(255, 255, 255, 0);
WiFiClient client;
WebServer server(80);

// control page
bool WifiMasterOn = false;
bool Button[16];
uint32_t WifiSwitchesTimer;

// Relays
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;
byte InvertedLo;
byte InvertedHi;

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

bool MasterOn = false;
bool AutoOn = true;

PCF8574 PCF;
bool PCF_found = false;

PCA9555 PCA;
bool PCA9555PW_found = false;

bool MCP23017_found = false;

// PCA9685
bool PCA9685_found = false;
#define PCA9685Address 0x55
Adafruit_PWMServoDriver PWMServoDriver = Adafruit_PWMServoDriver(PCA9685Address);

// analog
struct AnalogConfig
{
	int16_t AIN0;	// Pressure 0
	int16_t AIN1;	
	int16_t AIN2;
	int16_t AIN3;
};
AnalogConfig AINs;

bool ADSfound = false;

bool GoodPins;	// pin configuration correct
bool WorkSwitchOn = false;
bool WrkLast;
bool WrkCurrent;
uint8_t DisconnectCount = 0;
int16_t CurrentPressure = 0;

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
		MDL.WifiModeUseStation = false;
		SaveData();
		ESP.restart();
	}
}

int TimedCombo(byte, bool);	// function prototype
void IRAM_ATTR ISR0();		// function prototype
void IRAM_ATTR ISR1();

void setup()
{
	DoSetup();
}

void loop()
{
	ReceiveUDP();
	SetPWM();
	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000)
				&& ((Sensor[i].TargetUPM > 0 && MasterOn)
					|| ((Sensor[i].ControlType == 4) && (Sensor[i].TargetUPM > 0))
					|| (!AutoOn && MasterOn));
		}

		CheckRelays();
		GetUPM();
		AdjustFlow();
		ReadAnalog();
		CheckWorkSwitch();
		CheckPressure();
	}
	SendComm();
	server.handleClient();
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

void CheckWorkSwitch()
{
	if (MDL.WorkPin < NC)
	{
		WrkCurrent = digitalRead(MDL.WorkPin);
		if (MDL.WorkPinIsMomentary)
		{
			if (WrkCurrent != WrkLast)
			{
				if (WrkCurrent) WorkSwitchOn = !WorkSwitchOn;	// only cycle when going from low to high
				WrkLast = WrkCurrent;
			}
		}
		else
		{
			WorkSwitchOn = WrkCurrent;
		}
	}
	else
	{
		WorkSwitchOn = false;
	}
}

void CheckPressure()
{
	CurrentPressure = 0;
	if (MDL.PressurePin < NC)
	{
		CurrentPressure = analogRead(MDL.PressurePin) * 10.0;
	}
	else if (ADSfound)
	{
		CurrentPressure = AINs.AIN0 / 10.0;
	}
}

bool State = false;
uint32_t LastBlink;
uint32_t LastLoop;
byte ReadReset;
uint32_t MaxLoopTime;
//double debug1;
//double debug2;
//double debug3;
//double debug4;

void Blink()
{
	if (millis() - LastBlink > 1000)
	{
		LastBlink = millis();
		State = !State;
		//digitalWrite(LED_BUILTIN, State);

		Serial.print(" Micros: ");
		Serial.print(MaxLoopTime);

		//Serial.print(", ");
		//Serial.print(debug1);
		//
		//Serial.print(", ");
		//Serial.print(debug2);

		//Serial.print(", ");
		//Serial.print(debug3);

		//Serial.print(", ");
		//Serial.print(debug4);

		Serial.println("");

		if (ReadReset++ > 5)
		{
			ReadReset = 0;
			MaxLoopTime = 0;
		}
	}
	if (micros() - LastLoop > MaxLoopTime) MaxLoopTime = micros() - LastLoop;
	LastLoop = micros();
}
