
// rate control with Teensy 4.1

#define ETHERNET_COMM_ENABLED 0

#include <Wire.h>
#include <EEPROM.h>
#if ETHERNET_COMM_ENABLED
#include <NativeEthernet.h>
#include <fnet.h>  // hint for VM�s library resolver. Speed up compile with Deep Search off.
#include <NativeEthernetUdp.h>
#endif
#include "src/AgIsoStack/FlexCAN_T4.hpp"
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5
// #include <AgioStack.h>
#include "src/AgIsoStack/AgIsoStack.hpp"
#include <cstring>
#include <memory>
#include <string>
#include <vector>
// using namespace isobus;

#include "ISOBUS_VT_ObjectPool.cpp"
#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}
std::shared_ptr<isobus::CANHardwarePlugin> canPlugin = nullptr;
std::shared_ptr<isobus::InternalControlFunction> ISOBUSControlFunction = nullptr;
std::shared_ptr<isobus::DiagnosticProtocol> ISOBUSDiagnostics = nullptr;
std::shared_ptr<isobus::VirtualTerminalClient> ISOBUSVirtualTerminal = nullptr;
std::shared_ptr<isobus::TaskControllerClient> ISOBUSTaskController = nullptr;
std::shared_ptr<isobus::SpeedMessagesInterface> ISOBUSSpeedMessages = nullptr;

# define InoDescription "RCteensy"
const uint16_t InoID = 3076;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 1;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define ISOBUS_TC_MODE 1
#define MACHINE_SETTINGS_IDENTIFIER 0x5446

#define MaxProductCount 2
#define NC 0xFF		// Pins not connected
#define ModStringLengths 15
const int MaxSampleSize = 25;
const uint32_t FlowTimeout = 4000;
const bool EthernetCommEnabled = (ETHERNET_COMM_ENABLED != 0);
const bool CANProprietaryTelemetryEnabled = false;

const int16_t ADS1115_Address = 0x48;
DMAMEM uint8_t MCP23017address;
const uint8_t PCA9685address = 0x40;
const uint8_t PCF8574address = 0x20;
const uint8_t DefaultRelayPins[] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11

#if defined(ESP32)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
#elif defined(ARDUINO_TEENSY41)
const int PWM_BITS = 12;
const int PWM_FREQ = 490;
#else // Nano & similar AVR
const int PWM_BITS = 8;
const int PWM_FREQ = 490;  // Default
DMAMEM uint8_t ditherCounter = 0; // for Nano dithering
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
	uint8_t SensorCount = 2;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;			// sets on value for flow valve or sets motor direction
	uint8_t RelayControlPins[16] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11
	uint8_t OnboardRelayControl = 1;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t RemoteRelayControl = 0;			// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = 30;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output2 only, Output1 is off
	uint8_t PressurePin = 40;
	bool ADS1115Enabled = false;
	uint8_t WheelSpeedPin = NC;
	float WheelCal = 0;
	uint8_t CommMode = 1;			// 0 - UDP only, 1 - CAN Proprietary, 2 - UDP + CAN Proprietary
};

DMAMEM ModuleConfig MDL;

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

DMAMEM ModuleNetwork MDLnetwork;

struct SensorConfig	// about 104 bytes
{
	uint8_t FlowPin;
	uint8_t DirPin;
	uint8_t PWMPin;
	bool AdjustmentEnabled;
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
};

DMAMEM SensorConfig Sensor[2];

struct MachineSettings
{
	uint16_t Identifier = MACHINE_SETTINGS_IDENTIFIER; // TC + VT settings
	uint8_t SectionCount = 8;
	uint16_t SectionWidthCm[16] = { 50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50 };
	float TargetUPM[MaxProductCount] = { 0.0f,0.0f };
	float MeterCal[MaxProductCount] = { 0.0f,0.0f };
	float TargetRateLHa[MaxProductCount] = { 100.0f,100.0f };
	int16_t ManualPWM[MaxProductCount] = { 0,0 };
	bool AutoOn = true;
	float TankCapacityUnits = 0.0f;
	float TankRemainingUnits = 0.0f;
	uint8_t UnitMode = 0; // 0=l, 1=kg
	float TripAreaHa = 0.0f;
	float TripAppliedUnits = 0.0f;
	float LifetimeAreaHa = 0.0f;
	float LifetimeAppliedUnits = 0.0f;
};

DMAMEM MachineSettings Machine;

// ethernet
#if ETHERNET_COMM_ENABLED
DMAMEM EthernetUDP UDPcomm;
DMAMEM uint16_t ListeningPort = 28888;
DMAMEM uint16_t DestinationPort = 29999;
DMAMEM IPAddress DestinationIP(MDLnetwork.IP0, MDLnetwork.IP1, MDLnetwork.IP2, 255);
#endif

// Relays
volatile byte RelayLo = 0;	// sections 0-7
volatile byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;
byte InvertedLo;
byte InvertedHi;
byte FlowMasterValveIndex = 255;
DMAMEM uint32_t TC_LastSectionCommand = 0;
bool TC_SectionControlActive = false;
bool TC_DDOPNeedsReupload = false;

const uint16_t LoopTime = 50;      //in msec = 20hz
DMAMEM uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
DMAMEM uint32_t SendLast = SendTime;

bool MasterOn = false;
bool RelayTestForce = false;

DMAMEM PCA9555 PCA;
bool PCA9555PW_found = false;

bool MCP23017_found = false;
bool PCA9685_found = false;
bool PCF8574_found = false;

// analog
DMAMEM uint PressureReading = 0;
bool ADSfound = false;

bool GoodPins = false;	// configuration pins correct

float TimedCombo(byte, bool);	// function prototype

// firmware update
#if ETHERNET_COMM_ENABLED
DMAMEM EthernetUDP UpdateComm;
DMAMEM uint16_t UpdateReceivePort = 29100;
DMAMEM uint16_t UpdateSendPort = 29000;
#endif
DMAMEM uint32_t buffer_addr, buffer_size;
bool FirmwareUpdateMode = false;

bool CalibrationOn[] = { false,false };
DMAMEM float WheelSpeed = 0;
DMAMEM uint32_t WheelCounts = 0;
DMAMEM float ISOBUSSpeedKmh = 0;
DMAMEM uint32_t ISOBUSSpeedLastMs = 0;
uint8_t ISOBUSSpeedSource = 0;

void setup()
{
	DoSetup();
}

void loop()
{
#if ISOBUS_TC_MODE
	CANBus_Update();
#else
	// Communication - UDP and/or CAN based on CommMode
	switch (MDL.CommMode)
	{
	case 0:
		// UDP only — receive CAN so PGN 32700 can switch CommMode from CAN
		CANBus_Receive();
		if (ISOBUSDiagnostics != nullptr) ISOBUSDiagnostics->update();
		VT_Update();
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
#endif

#if ETHERNET_COMM_ENABLED
	ReceiveUDP();
	ReceiveUpdate();
#endif
	RateControl_UpdateTargets();
	SetPWM();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();
		SetSensorsEnabled();
		CheckRelays();
		GetUPM();
		if (MDL.WheelSpeedPin != NC) GetSpeed();
		RateControl_UpdateAreaCounter();
		RateControl_UpdateTargets();
		AdjustFlow();
		ReadAnalog();
	}

#if ETHERNET_COMM_ENABLED
	// Send data back based on CommMode
	switch (MDL.CommMode)
	{
	case 0:
		SendComm();
		break;
	case 2:
		SendComm();
		break;
		// CommMode 1 doesn't need SendComm() - data sent via CANBus_Update()
	}
#endif

	Blink();
}

void SetSensorsEnabled()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		bool Result = false;
		bool commandFresh = (millis() - Sensor[i].CommTime < 5000);
	#if ISOBUS_TC_MODE
		commandFresh = commandFresh || TC_SectionControlActive || MasterOn;
	#endif
		if (commandFresh)
		{
			if (!MasterOn)
			{
				Result = true;
			}
			else if (Sensor[i].TargetUPM > 0)
			{
				Result = true;
			}
			else if (!Machine.AutoOn)
			{
				Result = true;
			}
		}
		Sensor[i].AdjustmentEnabled = Result;
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

uint32_t MedianFromArray(uint32_t buf[], int count)
{
	uint32_t Result = 0;
	if (count > 0)
	{
		static DMAMEM uint32_t sorted[MaxSampleSize];
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
