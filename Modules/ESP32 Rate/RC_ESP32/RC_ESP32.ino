
#include <Adafruit_SPIDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_GenericDevice.h>
#include <Adafruit_BusIO_Register.h>
#include <index_html.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5
#include <PCF8574.h>		// https://github.com/RobTillaart/PCF8574
#include "ESP2SOTA_RC/ESP2SOTA_RC.h"	// modified from https://github.com/pangodream/ESP2SOTA

#include <WiFi.h>
#include <WebServer.h>
#include <DNSServer.h>

#include <WiFiUdp.h>
#include <WiFiClient.h>
#include <EEPROM.h> 
#include <Wire.h>

#include <SPI.h>
#include <Ethernet_Generic.h>
#include <EthernetUdp.h>

#include <Adafruit_PWMServoDriver.h>	// Adafruit PCA9685 PWM Servo Driver Library

//rate control with ESP32, board: DOIT ESP32 DEVKIT V1
# define InoDescription "RC_ESP32"
const uint16_t InoID = 8076;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 4;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate
const uint8_t Processor = 0;	// 0 - ESP32-Wroom-32U
const uint8_t PCB_Type = 0;		// 0 - RC15

#define NC 0xFF		// Pins not connected
#define ModStringLengths 15

const uint32_t FlowTimeout = 4000;
const uint8_t MinMedianSamples = 7;		// floor: take at least this many pulses for the
										// median even if older than the flow window, so a
										// 2-sample window can't be corrupted by one bad period.
										// 7 (was 5) rejects more bad periods at low flow (field
										// test ~4-8 UPM sat at the floor) at the cost of ~2 pulse
										// periods more lag; only affects low flow (high flow fills
										// the time window before reaching the floor)

const int16_t ADS1115_Address = 0x48;
uint8_t MCP23017address;
const uint8_t PCF8574address = 0x20;
uint8_t DefaultRelayPins[] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11

#if defined(ESP32)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
const uint8_t MaxProductCount = 6;
const int MaxSampleSize = 25;
#elif defined(ARDUINO_TEENSY41)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
const uint8_t MaxProductCount = 2;
const int MaxSampleSize = 25;
#else // Nano & similar AVR
const int PWM_BITS = 8;
const int PWM_FREQ = 490;  // Default
uint8_t ditherCounter = 0; // for Nano dithering
const uint8_t MaxProductCount = 2;
const int MaxSampleSize = 11;
#endif

const uint16_t EEPROM_SIZE = 512;
const uint8_t W5500_SS = 5;		// W5500 SPI SS

// servo driver
const uint8_t OutputEnablePin = 27;
const uint8_t PCA9685address = 0x55;	// RC15 1010101, 1 + A5-A0

enum ControlType
{
	StandardValve_ct = 0,
	ComboClose_ct = 1,
	Motor_ct = 2,
	Fan_ct = 4,
	TimedCombo_ct = 5
};

struct ModuleConfig	// about 130 bytes
{
	// RC15
	uint8_t ID = 0;
	uint8_t SensorCount = 1;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;			// sets on value for flow valve or sets motor direction
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t OnboardRelayControl = 5;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t RemoteRelayControl = 0;			// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	char APname[ModStringLengths] = "RateModule";
	char APpassword[ModStringLengths] = "";
	uint8_t WorkPin = NC;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output1 only, Output2 is off
	uint8_t PressurePin = NC;		// NC - no pressure pin
	bool ADS1115Enabled = true;
	uint8_t WheelSpeedPin = NC;
	float WheelCal = 0;
	uint16_t MaxPressureReading = 0xFFFF;	// raw analog reading for pressure. 0xFFFF is off
};

ModuleConfig MDL;

struct ModuleNetwork
{
	uint16_t Identifier = 9876;
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
	bool WifiModeUseStation = false;				// false - AP mode, true - AP + Station 
	char SSID[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
	char Password[ModStringLengths] = "111222333";
};

ModuleNetwork MDLnetwork;

// Board label: a user-entered text description stored on the module to identify the
// physical board/PCB ("RC12-3", etc). Kept in its own EEPROM slot (like
// ModuleNetwork) so it SURVIVES a firmware reflash / default reload - it describes the
// hardware, not the settings. See PID_Normalized_Control_Rationale.md neighbours in docs.
struct BoardLabel
{
	uint16_t Identifier;	// magic to detect an initialized slot
	char Text[16];			// up to 16 chars; app 0-pads short strings
};
BoardLabel MDLboard;
const int EE_BoardID = 3;			// free EEPROM region 3-22 (between InoType@2 and MDL@23)
const uint16_t BoardIDMagic = 4321;	// marks an initialized board label slot

struct SensorConfig	// about 104 bytes
{
	uint8_t FlowPin;
	uint8_t IN1;
	uint8_t IN2;
	float UPM;				// sent as upm X 1000
	float PWM;
	uint32_t CommTime;
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 -, 4 fan, 5 timed combo
	uint32_t TotalPulses;
	float TargetUPM;
	float MeterCal;
	int16_t ManualAdjust;
	float Hz;
	uint8_t MaxPWM;
	uint8_t MinPWM;
	float Kp;
	float Ki;
	float Deadband;
	uint8_t BrakePoint;
	uint8_t PIDslowAdjust;
	uint8_t SlewRate;
	float MaxIntegral;
	float TimedMinStart;
	uint16_t TimedAdjust;
	uint16_t TimedPause;
	uint8_t PIDtime;
	uint32_t PulseMin;
	uint32_t PulseMax;
	byte SampleWindow;	// flow window in centiseconds (x10 ms)
};

SensorConfig Sensor[MaxProductCount];
bool SensorConnected[MaxProductCount];
bool PIDenabled[MaxProductCount];
bool Applying[MaxProductCount];

// ethernet
EthernetUDP UDP_Ethernet;
const uint16_t ListeningPort = 28888;
const uint16_t DestinationPort = 29999;
IPAddress Ethernet_DestinationIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255);
bool ChipFound;

// wifi
WiFiUDP UDP_Wifi;
IPAddress Wifi_DestinationIP(192, 168, 100, 255);
WiFiClient client;
WebServer server(80);
DNSServer dnsServer;
const byte AP_DNS_PORT = 53;

// control page
bool WifiMasterOn = false;
bool Button[16];
uint32_t WifiSwitchesTimer;

// Relays
volatile byte RelayLo = 0;	// sections 0-7
volatile byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;
byte InvertedLo;
byte InvertedHi;
byte FlowMasterValveIndex = 255;

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
int16_t PressureReading = 0;
bool ADSfound = false;

// Pressure max gate (Layer 1 over-pressure cutout). Module-wide: one pressure sensor per module.
bool PressureGateActive = false;	// currently driving actuators to relieve
bool PressureGateLatched = false;	// persistent fault - holds relief until operator reset (master off)
uint32_t PressureGateStart = 0;		// millis() when current relief began (min-hold timer)
uint8_t PressureTripCount = 0;		// trips counted in the current window
uint32_t PressureTripWindow = 0;	// millis() at the start of the trip-count window
const uint16_t PressureMinHold = 3000;			// ms: minimum relief hold after a trip (rate-limits cycling)
const uint16_t PressureTripWindowMs = 10000;	// ms: window for counting repeated trips
const uint8_t PressureMaxTrips = 3;				// trips within the window -> escalate to hard latch

bool GoodPins;	// pin configuration correct

float TimedCombo(byte, bool);	// function prototype
void  ISR0();		// function prototype
void  ISR1();

uint8_t DisconnectCount = 0;

void WiFiStationConnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Connected to '");
	Serial.print(MDLnetwork.SSID);
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
	WiFi.begin(MDLnetwork.SSID, MDLnetwork.Password);

	if (DisconnectCount > 5)
	{
		// use AP mode only
		MDLnetwork.WifiModeUseStation = false;
		SaveNetworks();
		ESP.restart();
	}
}

bool CalibrationOn[] = { false,false };
float WheelSpeed = 0;
uint32_t WheelCounts = 0;

// PID damper
bool LastAboveTarget[MaxProductCount];
float OscDamp[MaxProductCount] = { 1.0f, 1.0f };

// PID diagnostics logging (PGN 32402). Kept out of SensorConfig so EEPROM layout is unchanged.
// All fields are snapshotted together when the PID computes so the logged packet is
// internally consistent (Target/Applied/Error/PWM all from the same instant) regardless
// of when SendPIDlog() later runs.
float DiagError[MaxProductCount];
float DiagIntegral[MaxProductCount];
float DiagChange[MaxProductCount];
float DiagTarget[MaxProductCount];
float DiagApplied[MaxProductCount];
float DiagPWM[MaxProductCount];
uint8_t DiagSamples[MaxProductCount];	// pulse samples used in the median (snapshot for logging)
uint8_t MedianCount[MaxProductCount];	// live: samples GetUPM used in the latest median
uint32_t DiagMillis[MaxProductCount];
bool PidSampleReady[MaxProductCount];
bool PidLogEnabled = false;

float IntegralSum[MaxProductCount];

void setup()
{
	DoSetup();
}

void loop()
{
	dnsServer.processNextRequest();
	server.handleClient();
	ReceiveUDP();
	DoPID();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			SensorConnected[i] = (millis() - Sensor[i].CommTime < 4000);
			PIDenabled[i] = SensorConnected[i] && AutoOn && MasterOn && (RelayLo || RelayHi) && (Sensor[i].TargetUPM > 0);
			Applying[i] = MasterOn && (Sensor[i].TargetUPM > 0 || !AutoOn);
		}

		CheckRelays();
		GetUPM();
		ReadAnalog();
		AdjustFlow();
		if (MDL.WheelSpeedPin != NC) GetSpeed();
	}
	SendComm();
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
	for (int i = Start; i < Length; i++)
	{
		Result += Chk[i];
	}
	return Result;
}

bool WorkPinOn()
{
	static bool WrkOn = false;
	static bool WrkLast = false;

	if (MDL.WorkPin < NC)
	{
		bool WrkCurrent = digitalRead(MDL.WorkPin);
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
	else
	{
		WrkOn = false;
	}
	return WrkOn;
}

uint32_t MedianFromArray(uint32_t buf[], int count)
{
	uint32_t Result = 0;
	if (count > 0)
	{
		uint32_t sorted[MaxSampleSize];
		for (int i = 0; i < count; i++) sorted[i] = buf[i];

		// insertion sort
		for (int i = 1; i < count; i++)
		{
			uint32_t key = sorted[i];
			int j = i - 1;
			while (j >= 0 && sorted[j] > key)
			{
				sorted[j + 1] = sorted[j];
				j--;
			}
			sorted[j + 1] = key;
		}

		if (count % 2 == 1)
		{
			Result = sorted[count / 2];
		}
		else
		{
			int mid = count / 2;
			// average of middle two
			Result = (sorted[mid - 1] + sorted[mid]) / 2;
		}
	}
	return Result;
}

//bool State = false;
//uint32_t LastBlink;
//uint32_t LastLoop;
//byte ReadReset;
//uint32_t MaxLoopTime;
//int debug1;
//double debug2;
//double debug3;
//
//// max loop about 2500, 18/Sep/2025
//void Blink()
//{
//	if (millis() - LastBlink > 1000)
//	{
//		LastBlink = millis();
//		State = !State;
//
//		Serial.print(MaxLoopTime);
//
//		Serial.print(", ");
//		Serial.print(debug1);
//
//		Serial.print(", ");
//		Serial.print(debug2);
//
//		Serial.print(", ");
//		Serial.print(debug3);
//
//		Serial.println("");
//
//		if (ReadReset++ > 5)
//		{
//			ReadReset = 0;
//			MaxLoopTime = 0;
//		}
//	}
//	if (micros() - LastLoop > MaxLoopTime) MaxLoopTime = micros() - LastLoop;
//	LastLoop = micros();
//}
