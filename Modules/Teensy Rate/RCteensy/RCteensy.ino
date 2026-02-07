
// rate control with Teensy 4.1

#include <Wire.h>
#include <EEPROM.h>
#include <NativeEthernet.h>
#include <fnet.h>  // hint for VM�s library resolver. Speed up compile with Deep Search off.
#include <NativeEthernetUdp.h>
#include <FlexCAN_T4.h>
#include "TCDefs.h"  // TC Client shared definitions (must be after FlexCAN_T4.h)
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}

# define InoDescription "RCteensy"
const uint16_t InoID = 31016;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 1;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define MaxProductCount 2
#define NC 0xFF		// Pins not connected
#define ModStringLengths 15
const int MaxSampleSize = 25;
const uint32_t FlowTimeout = 4000;

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
	uint8_t RelayControl = 1;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = 30;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output2 only, Output1 is off
	uint8_t PressurePin = 40;
	bool ADS1115Enabled = false;
	uint8_t WheelSpeedPin = NC;
	float WheelCal = 0;
	uint8_t CommMode = 3;			// 0 - UDP only, 1 - CAN Proprietary, 2 - UDP + CAN Proprietary, 3 - TC Client, 4 - UDP + TC Client
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
	bool AutoOn;
};

SensorConfig Sensor[2];

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

const uint16_t LoopTime = 50;      //in msec = 20hz
uint32_t LoopLast = LoopTime;
const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

bool MasterOn = false;

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

bool CalibrationOn[] = { false,false };
float WheelSpeed = 0;
uint32_t WheelCounts = 0;

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
		// UDP only
		ReceiveUDP();
		break;
	case 1:
		// CAN Proprietary only
		CANBus_Update();
		break;
	case 2:
		// UDP + CAN Proprietary
		ReceiveUDP();
		CANBus_Update();
		break;
	case 3:
		// TC Client only - STANDARD ISOBUS
		CANBus_MaintainAddress();  // Handle address claiming
		CANBus_Receive();          // Handle incoming CAN (address claim, TP, etc.)
		TP_Update();               // Transport Protocol state machine
		TCClient_Update();         // TC Client state machine
		// NO proprietary status - standard ISOBUS only
		break;
	case 4:
		// UDP + TC Client
		ReceiveUDP();
		CANBus_MaintainAddress();  // Handle address claiming
		CANBus_Receive();          // Handle incoming CAN
		TP_Update();
		TCClient_Update();
		CANBus_SendProprietaryStatus();  // Send PWM/Hz and module ident for RC display
		break;
	}

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
		if (MDL.WheelSpeedPin != NC) GetSpeed();
	}

	// Send data back based on CommMode
	switch (MDL.CommMode)
	{
	case 0:
		SendComm();
		break;
	case 2:
		SendComm();
		break;
	case 4:
		SendComm();
		break;
	// CommMode 1, 3 don't need SendComm() - data sent via CANBus_Update() or TCClient_Update()
	}

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
			else if (MasterOn && !Sensor[i].AutoOn)
			{
				Result = true;
			}
			else if ((Sensor[i].ControlType == Fan_ct) && (Sensor[i].TargetUPM > 0))
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
	static uint32_t lastRxCount = 0;

	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);

		// Debug output for TC Client mode
		if (MDL.CommMode == 3 || MDL.CommMode == 4)
		{
			Serial.print("CAN RX: ");
			Serial.print(canStats.rxCount - lastRxCount);
			Serial.print("/s, TC State: ");
			Serial.print(TCClient_GetState());
			Serial.print(", TC Addr: 0x");
			Serial.println(TCClient_GetTCAddress(), HEX);
			lastRxCount = canStats.rxCount;
		}
	}
}
