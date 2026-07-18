
// rate control with Teensy 4.1

#include <Wire.h>
#include <EEPROM.h>
#include <NativeEthernet.h>
#include <fnet.h>  // hint for VM�s library resolver. Speed up compile with Deep Search off.
#include <NativeEthernetUdp.h>
#include <FlexCAN_T4.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}

# define InoDescription "RCteensy"
const uint16_t InoID = 17076;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 1;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

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
const uint8_t MaxProductCount = 6;
const int MaxSampleSize = 25;
#else // Nano & similar AVR
const int PWM_BITS = 8;
const int PWM_FREQ = 490;  // Default
uint8_t ditherCounter = 0; // for Nano dithering
const uint8_t MaxProductCount = 2;
const int MaxSampleSize = 11;
#endif

enum ControlType
{
	StandardValve_ct = 0,
	ComboClose_ct = 1,
	Motor_ct = 2,
	Fan_ct = 4,
	TimedCombo_ct = 5
};

struct ModuleConfig
{
	// RC11-2
	uint8_t ID = 0;
	uint8_t SensorCount = 1;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;			// sets on value for flow valve or sets motor direction
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t OnboardRelayControl = 1;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t RemoteRelayControl = 0;			// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = 30;
	bool WorkPinIsMomentary = false;
	bool InvertWork = false;		// true for NO work switch sensor
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output2 only, Output1 is off
	uint8_t PressurePin = 40;
	bool ADS1115Enabled = false;
	uint8_t WheelSpeedPin = NC;
	float WheelCal = 0;
	uint8_t CommMode = 1;			// 0 - UDP only, 1 - CAN Proprietary, 2 - UDP + CAN Proprietary
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
	uint8_t DirPin;
	uint8_t PWMPin;
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
	uint8_t BinPin;		// bin level sensor (digital), NC = no bin alarm
	bool BinInvert;		// invert bin sensor reading
};

SensorConfig Sensor[MaxProductCount];
bool SensorConnected[MaxProductCount];
bool PIDenabled[MaxProductCount];
bool Applying[MaxProductCount];

// ethernet
EthernetUDP UDPcomm;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
IPAddress DestinationIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255);

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
bool AutoOn[MaxProductCount];	// per-sensor: each sensor's own PGN32500 bit 6 (allows mixed auto/manual, e.g. multi-sensor calibration phases); set true in DoSetup

PCA9555 PCA;
bool PCA9555PW_found = false;

bool MCP23017_found = false;

// analog
uint PressureReading = 0;
bool ADSfound = false;

// Pressure max gate (Layer 1 over-pressure cutout). Module-wide: one pressure sensor per module.
bool PressureGateActive = false;	// currently driving actuators to relieve
bool PressureGateLatched = false;	// persistent fault - holds relief until operator reset (master off)
uint32_t PressureGateStart = 0;		// millis() when current relief began (min-hold timer)
uint8_t PressureTripCount = 0;		// consecutive trips (re-trips within PressureTripResetMs of the last)
uint32_t PressureTripLast = 0;		// millis() of the most recent trip
const uint16_t PressureMinHold = 3000;			// ms: minimum relief hold after a trip (rate-limits cycling)
const uint16_t PressureTripResetMs = 20000;		// ms: a quiet spell this long since the last trip forgives the count
const uint8_t PressureMaxTrips = 3;				// consecutive trips -> escalate to hard latch

bool GoodPins = false;	// configuration pins correct

float TimedCombo(byte, bool);	// function prototype

// firmware update
EthernetUDP UpdateComm;
uint16_t UpdateReceivePort = 29100;
uint16_t UpdateSendPort = 29000;
uint32_t buffer_addr, buffer_size;
bool FirmwareUpdateMode = false;

bool CalibrationOn[MaxProductCount];	// zero-initialized; sized by constant, not initializer (an
										// aggregate initializer silently under-fills at 6 products)
float WheelSpeed = 0;
uint32_t WheelCounts = 0;

// PID damper
bool LastAboveTarget[MaxProductCount];
float OscDamp[MaxProductCount];		// set to 1.0 in DoSetup (no aggregate initializer - see CalibrationOn)

// Bin level sensors: debounced per-sensor empty state, reported in PGN 32400 status bit 1
bool BinEmpty[MaxProductCount];
uint32_t BinChangeTime[MaxProductCount];		// millis() when the raw reading last matched the debounced state
const uint16_t BinDebounce = 2500;				// ms: raw state must persist this long before the reported state flips

// Deferred restart for config that arrives as multiple packets (PGN 32507 / 0xFF17):
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
	// Communication - UDP and/or CAN based on CommMode
	switch (MDL.CommMode)
	{
	case 0:
		// UDP only — receive CAN so PGN 32700 can switch CommMode from CAN
		CANBus_Receive();
		break;
	case 1:
		// CAN Proprietary only
		CANBus_Update();
		break;
	case 2:
		// UDP + CAN Proprietary
		CANBus_Update();
		break;
	}

	ReceiveUDP();
	ReceiveUpdate();
	DoPID();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			SensorConnected[i] = (millis() - Sensor[i].CommTime < 4000);
			PIDenabled[i] = SensorConnected[i] && AutoOn[i] && MasterOn && (RelayLo || RelayHi) && (Sensor[i].TargetUPM > 0);
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
			SCB_AIRCR = 0x05FA0004;
		}
	}

	// Send data back based on CommMode
	switch (MDL.CommMode)
	{
	case 0:
		SendComm();
		SendPIDlog();
		break;
	case 2:
		SendComm();
		SendPIDlog();
		break;
		// CommMode 1 doesn't need SendComm() - data sent via CANBus_Update()
	}

	Blink();
	//DebugTheIno();
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
	// so a HIGH read means empty.
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

void Blink()
{
	static bool State = false;
	static elapsedMillis BlinkTmr;
	static elapsedMicros LoopTmr;

	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
	}
}

//int16_t debug1;
//int16_t debug2;
//int16_t debug3;
//int16_t debug4;
//int16_t debug5;
//int16_t debug6;
//void DebugTheIno()
//{
//	static uint32_t DebugTime;
//	if (millis() - DebugTime > 1000)
//	{
//		DebugTime = millis();
//
//		debug1=WorkPinOn();
//		debug2=digitalRead(MDL.WorkPin);
//
//		Serial.println("");
//		Serial.print(debug1);
//
//		Serial.print(", ");
//		Serial.print(debug2);
//
//		//Serial.print(", ");
//		//Serial.print(debug3);
//
//		//Serial.print(", ");
//		//Serial.print(debug4);
//
//		//Serial.print(", ");
//		//Serial.print(debug5);
//
//		//Serial.print(", ");
//		//Serial.print(debug6);
//	}
//}

