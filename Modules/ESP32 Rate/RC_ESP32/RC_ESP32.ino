
#include <Adafruit_SPIDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_GenericDevice.h>
#include <Adafruit_BusIO_Register.h>
#include "PCA95x5_RC.h"					// modified from https://github.com/hideakitai/PCA95x5
#include <PCF8574.h>					// https://github.com/RobTillaart/PCF8574
#include "ESP2SOTA_RC.h"				// modified from https://github.com/pangodream/ESP2SOTA

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
const uint16_t InoID = 5116;	// firmware build, ddmmy, no leading 0. Version label only: it no longer
								// resets stored settings — bump a record's version below for that
const uint8_t InoType = 4;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate
const uint8_t Processor = 0;	// 0 - ESP32-Wroom-32U
const uint8_t PCB_Type = 0;		// 0 - RC15
const uint8_t NewModuleID = 25;	// unassigned board: above every real ID (0-15), and 25+200 / 25+50 still make valid addresses

#define NC 0xFF		// Pins not connected
#define SsidLength 33			// 32 bytes + null, the 802.11 maximum
#define PasswordLength 64		// 63 bytes + null, the WPA2 passphrase maximum
const char APname[] = "RateModule";	// hotspot name prefix; "_" + ID + "_" + MAC suffix is added (Begin.ino)

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

const uint16_t EEPROM_SIZE = 1024;	// ESP32 EEPROM bounds-checks and no-ops writes past the size

// EEPROM records. Each slot holds a 5-byte header, then the data:
//   0 magic (which record)  1 InoType (which firmware wrote it)  2 layout version  3-4 CRC16
// A record loads only if all of these match, else that record alone gets defaults. InoType
// catches data left by other firmware on the same chip (Nano SwitchBox, Teensy AutoSteer).
// Change a record's layout or defaults -> bump its version; the other records are kept.
const uint8_t RecordHeaderSize = 5;

const uint16_t EE_Identity = 0;			// 0-255: ID, board label, network, AP password. Kept across updates
const uint16_t EE_IdentitySize = 256;
const uint8_t IdentityMagic = 0xA1;
const uint8_t IdentityVersion = 1;

const uint16_t EE_Config = 256;			// 256-319: ModuleConfig
const uint16_t EE_ConfigSize = 64;
const uint8_t ConfigMagic = 0xA2;
const uint8_t ConfigVersion = 1;

const uint16_t EE_Sensor = 320;			// 320 + i*112: stored part of SensorConfig, sensor 6 ends at 991
const uint16_t EE_SensorStep = 112;
const uint8_t SensorMagic = 0xA3;
const uint8_t SensorVersion = 1;
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

struct ModuleConfig	// about 38 bytes
{
	// RC15
	uint8_t ID = NewModuleID;		// stored in the identity record, which is loaded last and sets this
	uint8_t SensorCount = 1;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;			// sets on value for flow valve or sets motor direction
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t OnboardRelayControl = 5;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t RemoteRelayControl = 0;			// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = NC;
	bool WorkPinIsMomentary = false;
	bool InvertWork = false;		// true for NO work switch sensor
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output1 only, Output2 is off
	uint8_t PressurePin = NC;		// NC - no pressure pin
	bool ADS1115Enabled = true;
	uint8_t WheelSpeedPin = NC;
	float WheelCal = 0;
	uint16_t MaxPressureReading = 0xFFFF;	// raw analog reading for pressure. 0xFFFF is off
};

ModuleConfig MDL;

struct ModuleNetwork	// stored in the identity record
{
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
	bool WifiModeUseStation = false;				// false - AP mode, true - AP + Station
	char SSID[SsidLength] = "Tractor";				// name of network ESP32 connects to
	char Password[PasswordLength] = "111222333";
	// Channel the network was last joined on. Lets a reconnect skip the full
	// scan, and lets the softAP start where the join will land — see Wifi.ino.
	uint8_t StaChannelCache = 0;					// 0 = none cached yet
	char APpassword[PasswordLength] = "";			// module hotspot; under 8 characters = open
};

ModuleNetwork MDLnetwork;

// Board label: a user-entered text description stored on the module to identify the
// physical board/PCB ("RC12-3", etc). Stored in the identity record, so it survives
// a firmware update - it describes the hardware, not the settings.
struct BoardLabel
{
	char Text[16];			// up to 16 chars; app 0-pads short strings
};
BoardLabel MDLboard;

// What the identity record stores: kept across firmware updates
struct ModuleIdentity
{
	uint8_t ID;
	char Label[16];
	ModuleNetwork Network;
};
static_assert(RecordHeaderSize + sizeof(ModuleIdentity) <= EE_IdentitySize, "ModuleIdentity overflows its EEPROM slot");

struct SensorConfig
{
	// stored: everything before UPM (SensorStoredBytes)
	uint8_t FlowPin;
	uint8_t IN1;
	uint8_t IN2;
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 -, 4 fan, 5 timed combo
	float MeterCal;
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
	uint8_t BinPin;		// bin level sensor (digital), NC = no bin alarm
	bool BinInvert;		// invert bin sensor reading

	// live values, not stored
	float UPM;				// sent as upm X 1000
	float PWM;
	uint32_t CommTime;
	uint32_t TotalPulses;
	float TargetUPM;
	int16_t ManualAdjust;
	float Hz;
};

const uint16_t SensorStoredBytes = offsetof(SensorConfig, UPM);

static_assert(RecordHeaderSize + sizeof(ModuleConfig) <= EE_ConfigSize, "ModuleConfig overflows its EEPROM slot");
static_assert(RecordHeaderSize + SensorStoredBytes <= EE_SensorStep, "SensorConfig overflows its EEPROM slot");
static_assert(EE_Sensor + MaxProductCount * EE_SensorStep <= EEPROM_SIZE, "sensor slots overflow EEPROM_SIZE");

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
// Power type relays - always energized, so they must not count as a flow path.
volatile byte PowerRelayLo;
volatile byte PowerRelayHi;
// relays whose energized state is the complement of their logical state
// (Invert_Section/Master/FlowMaster, Bypass). Volatile - read in PulseISR.
volatile byte InvertedLo;
volatile byte InvertedHi;
byte FlowMasterValveIndex = 255;

// "is any section on", the flow-path test. The relay bits are OUTPUT states, so two
// corrections are needed before they mean anything logical: XOR the inverted list
// (Invert_Section and friends read backwards - a module with every section inverted
// otherwise looks idle exactly when all sections are on), and mask off Power relays
// (always energized, so they would make the test permanently true).
// A macro, not a function: PulseISR uses it, and an ESP32 ISR cannot call into flash.
// STILL IMPERFECT - Master/Switch/Hyd/Tram/GeoStop relays are not sections either but
// have no mask on the wire. See the Option B note (explicit section state in PGN 32500
// byte 12) for the real fix.
#define AnySectionOn() ((((RelayLo ^ InvertedLo) & ~PowerRelayLo) | ((RelayHi ^ InvertedHi) & ~PowerRelayHi)) != 0)

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

bool MasterOn = false;
bool AutoOn[MaxProductCount];	// per-sensor: each sensor's own PGN32500 bit 6 (allows mixed auto/manual, e.g. multi-sensor calibration phases); set true in DoSetup

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
uint32_t LastADSReadMs = 0;	// set on every successful conversion-register read

// Pressure max gate (Layer 1 over-pressure cutout). Module-wide: one pressure sensor per module.
bool PressureGateActive = false;	// currently driving actuators to relieve
bool PressureGateLatched = false;	// persistent fault - holds relief until operator reset (master off)
uint32_t PressureGateStart = 0;		// millis() when current relief began (min-hold timer)
uint8_t PressureTripCount = 0;		// consecutive trips (re-trips within PressureTripResetMs of the last)
uint32_t PressureTripLast = 0;		// millis() of the most recent trip
const uint16_t PressureMinHold = 3000;			// ms: minimum relief hold after a trip (rate-limits cycling)
const uint16_t PressureTripResetMs = 20000;		// ms: a quiet spell this long since the last trip forgives the count
const uint8_t PressureMaxTrips = 3;				// consecutive trips -> escalate to hard latch

bool GoodPins;	// pin configuration correct

float TimedCombo(byte, bool);	// function prototype
void  ISR0();		// function prototype
void  ISR1();

// Station mode — the connect/disconnect handlers and the paced retry policy
// that replaced the disconnect counter that used to live here — is in Wifi.ino.

bool CalibrationOn[MaxProductCount] = { false };
float WheelSpeed = 0;
uint32_t WheelCounts = 0;

// PID damper
bool LastAboveTarget[MaxProductCount];
float OscDamp[MaxProductCount];		// set to 1.0 in DoSetup (an aggregate initializer
									// silently under-fills at 6 products)

// Bin level sensors: debounced per-sensor empty state, reported in PGN 32400 status bit 1
bool BinEmpty[MaxProductCount];
uint32_t BinChangeTime[MaxProductCount];		// millis() when the raw reading last matched the debounced state
const uint16_t BinDebounce = 2500;				// ms: raw state must persist this long before the reported state flips

// Deferred restart for config that arrives as multiple packets (PGN 32507):
// restarting on the first packet would drop the rest, so apply+save each one and
// restart after the config stream has been quiet for RestartDelay
bool RestartPending = false;
uint32_t RestartLastConfig = 0;					// millis() of the last 32507 packet
const uint16_t RestartDelay = 1500;

// Config rejection: a received 32507/32700 with pins invalid for this board is
// discarded; reported in PGN 32401 byte 13 bit 7 for 2 s so the app can alert
bool ConfigRejected = false;
uint32_t ConfigRejectedTime = 0;			// millis() of the rejected packet
const uint16_t ConfigRejectedReport = 2000;	// ms: how long the status bit stays set

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
	ServiceWifiStation();	// paced station retry — see Wifi.ino; returns on a bool when connected
	ReceiveUDP();
	DoPID();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			SensorConnected[i] = (millis() - Sensor[i].CommTime < 4000);
			PIDenabled[i] = SensorConnected[i] && AutoOn[i] && MasterOn && AnySectionOn() && (Sensor[i].TargetUPM > 0);
			Applying[i] = MasterOn && (Sensor[i].TargetUPM > 0 || !AutoOn[i]);
		}

		CheckRelays();
		GetUPM();
		ReadAnalog();
		AdjustFlow();
		CheckBinSensors();
		if (MDL.WheelSpeedPin != NC) GetSpeed();

		// deferred restart: the sensor-pins config (PGN 32507) arrives as one packet
		// per sensor; restart once the stream has been quiet for RestartDelay
		if (RestartPending && (millis() - RestartLastConfig >= RestartDelay))
		{
			ESP.restart();
		}
	}
	SendComm();
	SendPIDlog();
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
		if (MDL.InvertWork) WrkCurrent = !WrkCurrent;
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

void CheckBinSensors()
{
	// Debounced per-sensor bin level. The raw reading must disagree with the
	// reported state for BinDebounce ms straight before the state flips -
	// paddle/capacitive sensors flicker as product shifts across them.
	// Default sense (no invert): pin pulled up, sensor pulls low while covered,
	// so a HIGH read means empty. Note GPIO 34-39 have no internal pull-up -
	// those pins need an external resistor.
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (Sensor[i].BinPin < NC)
		{
			bool RawEmpty = digitalRead(Sensor[i].BinPin);
			if (Sensor[i].BinInvert) RawEmpty = !RawEmpty;

			if (RawEmpty == BinEmpty[i])
			{
				BinChangeTime[i] = millis();
			}
			else if (millis() - BinChangeTime[i] >= BinDebounce)
			{
				BinEmpty[i] = RawEmpty;
			}
		}
		else
		{
			BinEmpty[i] = false;
		}
	}
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
