
// rate control with Teensy 4.1

#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}

# define InoDescription "RCteensy"
const uint16_t InoID = 14095;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 1;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define MaxProductCount 2
#define NC 0xFF		// Pins not connected
#define ModStringLengths 15

const int16_t ADS1115_Address = 0x48;
uint8_t MCP23017address;
const uint8_t PCF8574address = 0x20;
uint8_t DefaultRelayPins[] = {8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC};		// pin numbers when GPIOs are used for relay control (1), default RC11

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

struct ModuleConfig
{
	// RC11-2
	uint8_t ID = 0;
	uint8_t SensorCount = 2;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;			// sets on value for flow valve or sets motor direction
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t RelayControl = 1;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	char APname[ModStringLengths] = "RateModule";
	char APpassword[ModStringLengths] = "111222333";
	uint8_t WorkPin = 30;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output2 only, Output1 is off
	uint8_t PressurePin = 40;		
	bool ADS1115Enabled = false;
};

ModuleConfig MDL;

struct SensorConfig	// about 104 bytes
{
	uint8_t FlowPin;
	uint8_t DirPin;
	uint8_t PWMPin;
	bool FlowEnabled;
	float UPM;				// sent as upm X 1000
	float PWM;
	uint32_t CommTime;
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 -, 4 fan, 5 timed combo
	uint32_t TotalPulses;
	float TargetUPM;
	float MeterCal;
	float ManualAdjust;
	float Hz;
	float MaxPWM;
	float MinPWM;
	float Kp;
	float Ki;
	float Deadband;
	float BrakePoint;
	float PIDslowAdjust;
	float SlewRate;
	float MaxMotorIntegral;
	float MaxValveIntegral;
	float TimedMinStart;
	uint32_t TimedAdjust;
	uint32_t TimedPause;
	uint32_t PIDtime;
	uint32_t PulseMin;
	uint32_t PulseMax;
	byte PulseSampleSize;
};

SensorConfig Sensor[2];

// ethernet
EthernetUDP UDPcomm;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
IPAddress DestinationIP(MDL.IP0, MDL.IP1, MDL.IP2, 255);

// Relays
volatile byte RelayLo = 0;	// sections 0-7
volatile byte RelayHi = 0;	// sections 8-15
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

bool MCP23017_found = false;

// analog
uint PressureReading = 0;
bool ADSfound = false;

bool GoodPins = false;	// configuration pins correct

float TimedCombo(byte, bool);	// function prototype

// firmware update
EthernetUDP UpdateComm;
uint16_t UpdateReceivePort = 29100;
uint16_t UpdateSendPort = 29000;
uint32_t buffer_addr, buffer_size;
bool FirmwareUpdateMode = false;

void setup()
{
	DoSetup();
}

void loop()
{
	ReceiveUDP();
	ReceiveUpdate();
	SetPWM();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();
		SetSensorsEnabled();
		CheckRelays();
		GetUPM();
		AdjustFlow();
		ReadAnalog();
	}

	SendComm();
	Blink();
}

void SetSensorsEnabled()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		bool Result = false;
		if (millis() - Sensor[i].CommTime < 5000)
		{
			if (Sensor[i].TargetUPM > 0 && MasterOn)
			{
				Result = true;
			}
			else if (MasterOn && !AutoOn)
			{
				Result = true;
			}
			else if ((Sensor[i].ControlType == 4) && (Sensor[i].TargetUPM > 0))
			{
				// fan
				Result = true;
			}
		}
		Sensor[i].FlowEnabled = Result;
	}
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

void Blink()
{
	static bool State = false;
	static elapsedMillis BlinkTmr;
	static elapsedMicros LoopTmr;
	static byte Count = 0;
	static uint32_t MaxLoopTime = 0;

	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);

		if (!FirmwareUpdateMode)
		{
			Serial.print(" Micros: ");
			Serial.print(MaxLoopTime);

			Serial.print(", ");
			Serial.print(Ethernet.localIP());

			Serial.println("");
		}

		if (Count++ > 10)
		{
			Count = 0;
			MaxLoopTime = 0;
		}
	}
	if (LoopTmr > MaxLoopTime) MaxLoopTime = LoopTmr;
	LoopTmr = 0;
}
