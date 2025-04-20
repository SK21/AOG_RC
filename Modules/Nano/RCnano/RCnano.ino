
#include <Wire.h>
#include <EEPROM.h>
#include <SPI.h>
#include <EtherCard.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

// rate control with nano
# define InoDescription "RCnano :  19-Apr-2025"
const uint16_t InoID = 19045;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 2;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define MaxProductCount 2
#define NC 0xFF		// Pins are not connected
uint8_t MCP23017address;


// MCP23017 control pins, RC5, RC8	{ 8,9,10,11,12,13,14,15,7,6,5,4,3,2,1,0 }
// MCP23017 control pins, RC12-3	{ 0,15,1,14,2,13,3,12,4,11,5,10,6,9,7,8 }
struct ModuleConfig
{
	// RC12-3
	uint8_t ID = 0;
	uint8_t SensorCount = 2;        // up to 2 sensors, if 0 rate control will be disabled
	bool InvertRelay = true;	    // value that turns on relays
	bool InvertFlow = true;		// sets on value for flow valve or sets motor direction
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
	uint8_t RelayControlPins[16] = { 0,15,1,14,2,13,3,12,4,11,5,10,6,9,7,8 };	// MCP23017, RC12-3
	uint8_t RelayControl = 4;		// 0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - PCA9685, 6 - PCF8574
	uint8_t WorkPin = 14;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - powered on/off, True - powered on only
	uint8_t PressurePin = 15;		
	bool ADS1115Enabled = false;
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin;
	uint8_t DirPin;
	uint8_t PWMPin;
	bool FlowEnabled;
	double UPM;				// sent as upm X 1000
	double PWM;
	uint32_t CommTime;
	byte ControlType;		// flow control, 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan, 5 timed combo
	uint32_t TotalPulses;
	double TargetUPM;
	double MeterCal;
	double ManualAdjust;
	double HighAdjust;
	double LowAdjust;
	double AdjustThreshold;
	double MaxPower;
	double MinPower;
	double Scaling;
	double Hz;
};

SensorConfig Sensor[2];

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

// ethernet
byte Ethernet::buffer[100];			// udp send and receive buffer
static byte selectPin = 10;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
byte DestinationIP[] = { MDL.IP0, MDL.IP1, MDL.IP2, 255 };	// broadcast 255
unsigned int SourcePort = 5123;		// to send from
bool ENCfound;

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

bool MasterSwitchOn = false;
bool AutoOn = true;

PCA9555 PCA;
bool PCA9555PW_found = false;

//reset function
void(*resetFunc) (void) = 0;

bool GoodPins;	// pin configuration correct
bool WrkOn;
bool WrkLast;
bool WrkCurrent;

int TimedCombo(byte, bool);	// function prototype
int16_t PressureReading = 0;
bool WorkSwitchOn = false;
bool MCP23017_found = false;

bool EthernetConnected()
{
	bool Result = false;
	if (ENCfound)
	{
		Result = ether.isLinkUp();

	}
	return Result;
}

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

	SetPWM();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000)
				&& ((Sensor[i].TargetUPM > 0 && MasterSwitchOn)
					|| ((Sensor[i].ControlType == 4) && (Sensor[i].TargetUPM > 0))
					|| (!AutoOn && MasterSwitchOn));
		}

		CheckRelays();
		GetUPM();
		AdjustFlow();
		CheckWorkSwitch();
		CheckPressure();
	}

	SendData();
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
	PressureReading = 0;
	if (MDL.PressurePin < NC)
	{
		PressureReading = analogRead(MDL.PressurePin);
	}
}

//uint32_t DebugTime;
//uint32_t MaxLoopTime;
//uint32_t LoopTmr;
//byte ReadReset;
//int MinMem = 2000;
//double FlowHz;
//double debug1;
//double debug2;
//double debug3;
//
//void DebugTheIno()
//{
//	if (millis() - DebugTime > 1000)
//	{
//		DebugTime = millis();
//		Serial.println("");
//
//		Serial.print(MaxLoopTime);
//
//		Serial.print(", ");
//		Serial.print(MinMem);
//
//		Serial.print(", ");
//		Serial.print(Sensor[0].Hz);
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
