#include <Wire.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

#include <Adafruit_MCP23008.h>
#include <Adafruit_MCP23X08.h>
#include <Adafruit_MCP23X17.h>
#include <Adafruit_MCP23XXX.h>

#include <Adafruit_BusIO_Register.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_SPIDevice.h>

#include <WiFi.h>
#include <WiFiUdp.h>
#include <WiFiClient.h>
#include <WiFiAP.h>

#include <WebServer.h>
#include <EEPROM.h> 

#include <SPI.h>
#include <Ethernet.h>
#include <EthernetUdp.h>
#include <Adafruit_PWMServoDriver.h>

#include <ESP2SOTA.h>		// https://github.com/pangodream/ESP2SOTA

// rate control with ESP32	board: DOIT ESP32 DEVKIT V1
# define InoDescription "RC_ESP32 :  05-Feb-2024"
const uint16_t InoID = 5124;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 4;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate
const uint8_t Processor = 0;	// 0 - ESP32-Wroom-32U

#define MaxReadBuffer 100	// bytes
#define MaxProductCount 2
#define EEPROM_SIZE 512
#define ModStringLengths 20

// servo driver
#define OutputEnablePin 27
#define DriverAddress 0x55

#define NC 0xFF		// Pin not connected

struct ModuleConfig
{
	uint8_t ID = 0;
	uint8_t SensorCount = 1;        // up to 2 sensors, if 0 rate control will be disabled
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 5;
	uint8_t IP3 = 60;
	uint8_t RelayControl = 6;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685 single , 6 - PCA9685 paired 
	uint8_t RelayPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	char Name[ModStringLengths] = "RateModule";
	char Password[ModStringLengths] = "111222333";
	uint8_t AdsAddress = 0x48;			// enter 0 to search all
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
	bool UseMultiPulses;	// 0 - time for one pulse, 1 - average time for multiple pulses
	uint8_t Debounce;
};

SensorConfig Sensor[2];

// network
const uint16_t ListeningPort = 28888;
const uint16_t DestinationPort = 29999;

// ethernet
EthernetUDP UDP_Ethernet;
IPAddress DestinationIP(MDL.IP0, MDL.IP1, MDL.IP2, 255);
bool ChipFound;

// AGIO
EthernetUDP UDP_AGIO;
uint16_t ListeningPortAGIO = 8888;		// to listen on
uint16_t DestinationPortAGIO = 9999;	// to send to

// wifi
WiFiUDP UDP_Wifi;
IPAddress AP_LocalIP(192, 168, 30, 1);
IPAddress AP_Subnet(255, 255, 255, 0);
IPAddress AP_DestinationIP(192, 168, 30, 255);
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

PCA9555 PCA;
bool PCA9555PW_found = false;

Adafruit_PWMServoDriver PWMServoDriver = Adafruit_PWMServoDriver(DriverAddress);
bool PCA9685_found = false;

Adafruit_MCP23X17 MCP;
bool MCP23017_found = false;

// analog
struct AnalogConfig
{
	int16_t AIN0;	// Pressure 0
	int16_t AIN1;	// Pressure 1
	int16_t AIN2;
	int16_t AIN3;
};
AnalogConfig AINs;

int ADS1115_Address;
bool ADSfound = false;

int TimedCombo(byte, bool);	// function prototype
void IRAM_ATTR ISR0();		// function prototype
void IRAM_ATTR ISR1();

void setup()
{
	DoSetup();
}

void loop()
{
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
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		SendUDP();
	}

	SetPWM();
	ReceiveUDP();
	ReceiveAGIO();

	server.handleClient();

	//Blink();
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

bool State = false;
uint32_t LastBlink;
uint32_t LastLoop;
byte ReadReset;
uint32_t MaxLoopTime;
double debug1;
double debug2;
double debug3;

void Blink()
{
	if (millis() - LastBlink > 1000)
	{
		LastBlink = millis();
		State = !State;
		//digitalWrite(LED_BUILTIN, State);

		Serial.print(" Micros: ");
		Serial.print(MaxLoopTime);
		Serial.print(", ");
		Serial.print(debug1);
		Serial.print(", ");
		Serial.print(debug2);
		Serial.print(", ");
		Serial.print(debug3);

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
