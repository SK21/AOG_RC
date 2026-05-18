
// rate control with Teensy 4.1

#include <Wire.h>
#include <EEPROM.h>
#include <NativeEthernet.h>
#include <fnet.h>  // hint for VM�s library resolver. Speed up compile with Deep Search off.
#include <NativeEthernetUdp.h>
#include <FlexCAN_T4.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5
#include <PCF8574.h>		// https://github.com/RobTillaart/PCF8574

#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}

#include <Adafruit_SPIDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_GenericDevice.h>
#include <Adafruit_BusIO_Register.h>
#include <Adafruit_PWMServoDriver.h>	// Adafruit PCA9685 PWM Servo Driver Library

#include "src/AgIsoStack/FlexCAN_T4.hpp"
#include "src/AgIsoStack/AgIsoStack.hpp"
#include "ISOBUS_VT_ObjectPool.cpp"
#include <cstring>
#include <memory>
#include <string>
#include <vector>

std::shared_ptr<isobus::CANHardwarePlugin> canPlugin = nullptr;
std::shared_ptr<isobus::InternalControlFunction> ISOBUSControlFunction = nullptr;
std::shared_ptr<isobus::DiagnosticProtocol> ISOBUSDiagnostics = nullptr;
std::shared_ptr<isobus::VirtualTerminalClient> ISOBUSVirtualTerminal = nullptr;
std::shared_ptr<isobus::TaskControllerClient> ISOBUSTaskController = nullptr;
std::shared_ptr<isobus::SpeedMessagesInterface> ISOBUSSpeedMessages = nullptr;

#define MACHINE_SETTINGS_IDENTIFIER 0x2026

# define InoDescription "RCteensy-Isobus"
const uint16_t InoID = 17056;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 5;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate, 5 - Teensy Isobus

#define MaxProductCount 2
#define NC 0xFF		// Pins not connected
const int MaxSampleSize = 25;
const uint32_t FlowTimeout = 4000;

const int16_t ADS1115_Address = 0x48;
uint8_t MCP23017address;
const uint8_t PCA9685Address = 0x40;
const uint8_t PCF8574address = 0x20;
uint8_t DefaultRelayPins[] = { 8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC };		// pin numbers when GPIOs are used for relay control (1), default RC11

const int PWM_BITS = 12;
const int PWM_FREQ = 490;

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
};

ModuleConfig MDL;

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
	byte PulseSampleSize;
};

SensorConfig Sensor[MaxProductCount];
bool SensorConnected[MaxProductCount];
bool PIDenabled[MaxProductCount];
bool Applying[MaxProductCount];

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
	uint8_t UnitMode = 0; // 0=l, 1=kg, 2 gallons, 3 lbs
	float TripAreaHa = 0.0f;
	float TripAppliedUnits = 0.0f;
	float LifetimeAreaHa = 0.0f;
	float LifetimeAppliedUnits = 0.0f;
	float LifetimeHours = 0;
};

DMAMEM MachineSettings Machine;


// Relays
volatile byte RelayLo = 0;	// sections 0-7
volatile byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;
byte InvertedLo;
byte InvertedHi;
byte FlowMasterValveIndex = 255;

DMAMEM PCA9555 PCA;
bool PCA9555PW_found = false;
bool MCP23017_found = false;
bool PCA9685_found = false;
bool PCF8574_found = false;

PCF8574 PCF;

// PCA9685
Adafruit_PWMServoDriver PWMServoDriver = Adafruit_PWMServoDriver(PCA9685Address);
const uint8_t OutputEnablePin = 27;
const uint8_t PCA9685address = 0x55;	

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;

bool MasterOn = false;
bool AutoOn = true;

// analog
uint PressureReading = 0;
bool ADSfound = false;

bool GoodPins = false;	// configuration pins correct
float TimedCombo(byte, bool);	// function prototype

DMAMEM float WheelSpeed = 0;
DMAMEM uint32_t WheelCounts = 0;

// Isobus
DMAMEM uint32_t TC_LastSectionCommand = 0;
bool TC_SectionControlActive = false;
bool TC_DDOPNeedsReupload = false;
bool RelayTestForce = false;
DMAMEM float ISOBUSSpeedKmh = 0;
DMAMEM uint32_t ISOBUSSpeedLastMs = 0;
uint8_t ISOBUSSpeedSource = 0;

void setup()
{
	DoSetup();
}

void loop()
{
	CANBus_Update();
	Server_Update();
	SetPWM();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			SensorConnected[i] = (millis() - Sensor[i].CommTime < 4000) || (TC_SectionControlActive && millis() - TC_LastSectionCommand < 4000);
			PIDenabled[i] = SensorConnected[i] && AutoOn && (Sensor[i].TargetUPM > 0);
			Applying[i] = MasterOn && (Sensor[i].TargetUPM > 0 || !AutoOn);
		}

		CheckRelays();
		GetUPM();
		if (MDL.WheelSpeedPin != NC) GetSpeed();
		RateControl_UpdateTargets();
		RateControl_UpdateAreaCounter();
		AdjustFlow();
		ReadAnalog();
		UpdateHourMeter();
	}

	Blink();
}

void UpdateHourMeter()
{
	static uint32_t lastSave = 0;
	static uint32_t lastTick = 0;
	static bool initialized = false;
	uint32_t now = millis();

	if (!initialized)
	{
		lastTick = now;
		lastSave = now;
		initialized = true;
	}

	if (now - lastTick >= 1000)
	{
		Machine.LifetimeHours += (now - lastTick) / 3600000.0f;
		lastTick = now;
	}

	if (now - lastSave >= 360000UL)
	{
		lastSave = now;
		SaveMachineSettings();
	}
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
//		Serial.println("");
//		Serial.print(debug1);
//
//		Serial.print(", ");
//		Serial.print(debug2);
//
//		Serial.print(", ");
//		Serial.print(debug3);
//
//		Serial.print(", ");
//		Serial.print(debug4);
//
//		Serial.print(", ");
//		Serial.print(debug5);
//
//		Serial.print(", ");
//		Serial.print(debug6);
//	}
//}

