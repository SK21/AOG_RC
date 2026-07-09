#include <Wire.h>
#include <EEPROM.h>
#include <SPI.h>
#include <EtherCard.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

// rate control with arduino nano
# define InoDescription "RCnano"
const uint16_t InoID = 9076;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 2;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define MaxProductCount 2
#define NC 0xFF		// Pins are not connected
uint8_t MCP23017address;
const int MaxSampleSize = 11;
const uint32_t FlowTimeout = 4000UL;

#if defined(ESP32)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
#elif defined(ARDUINO_TEENSY41)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
#else // Nano & similar AVR
const int PWM_BITS = 8;
const int PWM_FREQ = 490;  // Default
uint8_t ditherCounter = 0; // for Nano dithering
#endif

enum ControlType
{
	StandardValve_ct = 0,
	ComboClose_ct = 1,
	Motor_ct = 2,
	Fan_ct = 4,
	TimedCombo_ct = 5
};

// MCP23017 control pins, RC5, RC8	{ 8,9,10,11,12,13,14,15,7,6,5,4,3,2,1,0 }
// MCP23017 control pins, RC12-3	{ 0,15,1,14,2,13,3,12,4,11,5,10,6,9,7,8 }

struct ModuleConfig
{
	// RC12-3
	uint8_t ID = 0;
	uint8_t SensorCount = 2;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;		// sets on value for flow valve or sets motor direction
	uint8_t RelayControlPins[16] = { 0,15,1,14,2,13,3,12,4,11,5,10,6,9,7,8 };	// MCP23017, RC12-3
	uint8_t RelayControl = 4;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = 15;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - powered on/off, True - powered on only
	uint8_t PressurePin = 14;
	bool ADS1115Enabled = false;
	uint16_t MaxPressureReading = 0xFFFF;	// raw analog reading for pressure gate. 0xFFFF is off
};

ModuleConfig MDL;

struct ModuleNetwork
{
	uint16_t Identifier = 9876;
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
};

ModuleNetwork MDLnetwork;

struct SensorConfig
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
	byte PulseSampleSize;
	uint8_t BinPin;		// bin level sensor (digital), NC = no bin alarm
	bool BinInvert;		// invert bin sensor reading
};

SensorConfig Sensor[MaxProductCount];
bool SensorConnected[MaxProductCount];
bool PIDenabled[MaxProductCount];
bool Applying[MaxProductCount];
bool LastAboveTarget[MaxProductCount];
float OscDamp[MaxProductCount] = { 1.0f, 1.0f };

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

// ethernet
byte Ethernet::buffer[400];			// udp send and receive buffer (changed to 400 25Nov2025)
static byte selectPin = 10;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
byte DestinationIP[] = { MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255 };	// broadcast 255
unsigned int SourcePort = 5123;		// to send from
bool ENCfound;

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
int16_t PressureReading = 0;

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

//reset function
void(*resetFunc) (void) = 0;

bool EthernetConnected()
{
	bool Result = false;
	if (ENCfound)
	{
		Result = ether.isLinkUp();

	}
	return Result;
}

bool CalibrationOn[] = { false,false };

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

// declared here (not PID.ino) so Motor.ino's pressure gate can see it - .ino files
// concatenate alphabetically and Motor comes before PID
float IntegralSum[MaxProductCount];

void setup()
{
	DoSetup();
}

void loop()
{
	if (EthernetConnected())
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}

	DoPID();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			SensorConnected[i] = (millis() - Sensor[i].CommTime < 4000);
			// gate PID off when master or all sections are off - no flow path means UPM=0
			// with a nonzero target, so the loop would wind the valve open (huge overshoot
			// when flow starts). PWM=0 makes a standard valve HOLD position, not close.
			PIDenabled[i] = SensorConnected[i] && AutoOn[i] && MasterOn && (RelayLo || RelayHi) && (Sensor[i].TargetUPM > 0);
			Applying[i] = MasterOn && (Sensor[i].TargetUPM > 0 || !AutoOn[i]);
		}

		CheckRelays();
		GetUPM();
		CheckPressure();	// read the sensor BEFORE AdjustFlow so the gate acts on fresh pressure
		AdjustFlow();
		CheckBinSensors();

		// deferred restart: the sensor-pins config (PGN 32507) arrives as one packet
		// per sensor; restart once the stream has been quiet for RestartDelay
		if (RestartPending && (millis() - RestartLastConfig >= RestartDelay))
		{
			resetFunc();
		}
	}

	SendComm();
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

void CheckPressure()
{
	PressureReading = 0;
	if (MDL.PressurePin < NC)
	{
		PressureReading = analogRead(MDL.PressurePin);	// 10 bit, 0-1023
	}
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

//uint32_t DebugTime;
//uint32_t MaxLoopTime;
//uint32_t LoopTmr;
//byte ReadReset;
//int MinMem = 2000;
//double debug1;
//double debug2;
//double debug3;
//double debug4;
//double debug5;

//void DebugTheIno()
//{
//	if (millis() - DebugTime > 1000)
//	{
//		DebugTime = millis();
//		Serial.println("");
//
//		Serial.print(F(" Micros: "));
//		Serial.print(MaxLoopTime);
//
//		Serial.print(F(",  SRAM: "));
//		Serial.print(MinMem);
//		//Serial.print(", ");
//
//		//Serial.print(debug1);
//
//		//Serial.print(", ");
//		//Serial.print(debug2);
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
//		if (ReadReset++ > 10)
//		{
//			ReadReset = 0;
//			MaxLoopTime = 0;
//			MinMem = 2000;
//		}
//	}
//	if (micros() - LoopTmr > MaxLoopTime) MaxLoopTime = micros() - LoopTmr;
//	LoopTmr = micros();
//	if (freeRam() < MinMem) MinMem = freeRam();
//}
//
//int freeRam() {
//	extern int __heap_start, * __brkval;
//	int v;
//	return (int)&v - (__brkval == 0
//		? (int)&__heap_start : (int)__brkval);
//}
//
