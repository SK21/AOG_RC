#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include "PCA95x5_RC.h"		// modified from https://github.com/hideakitai/PCA95x5

#include "FXUtil.h"		// read_ascii_line(), hex file support
extern "C" {
#include "FlashTxx.h"		// TLC/T3x/T4x/TMM flash primitives
}

// rate control with Teensy 4.1
# define InoDescription "RCteensy :  08-Apr-2025"
const uint16_t InoID = 8045;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 1;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define MaxReadBuffer 100	// bytes
#define MaxProductCount 2
#define NC 0xFF		// Pins not connected
#define ModStringLengths 15

const int16_t ADS1115_Address = 0x48;
uint8_t MCP23017address;
const uint8_t PCF8574address = 0x20;

uint8_t DefaultRelayPins[] = {8,9,10,11,12,25,26,27,NC,NC,NC,NC,NC,NC,NC,NC};		// pin numbers when GPIOs are used for relay control (1), default RC11
char DefaultNetName[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
char DefaultNetPassword[ModStringLengths] = "111222333";

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
	bool WifiModeUseStation = false;				// false - AP mode, true - AP + Station 
	char SSID[ModStringLengths] = "Tractor";		// name of network ESP32 connects to
	char Password[ModStringLengths] = "111222333";
	uint8_t WorkPin = 30;
	bool WorkPinIsMomentary = false;
	bool Is3Wire = true;			// False - DRV8870 provides powered on/off with Output1/Output2, True - DRV8870 provides on/off with Output2 only, Output1 is off
	uint8_t PressurePin = 40;		
	bool ADS1115Enabled = false;
	uint8_t ESPserialPort = NC;		// serial port to connect to wifi module
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
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan, 5 timed combo
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
};

SensorConfig Sensor[2];

// ethernet
EthernetUDP UDPcomm;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
IPAddress DestinationIP(MDL.IP0, MDL.IP1, MDL.IP2, 255);

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

bool MCP23017_found = false;

uint32_t ESPtime;
int8_t WifiStrength;

// analog
struct AnalogConfig
{
	int16_t AIN0;	// Pressure 0
	int16_t AIN1;	// Pressure 1
	int16_t AIN2;
	int16_t AIN3;
};
AnalogConfig AINs;
int PressureReading = 0;

bool ADSfound = false;

// WifiSwitches connection to ESP8266
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];
HardwareSerial* SerialESP;

bool GoodPins = false;	// configuration pins correct
bool WrkOn;
bool WrkLast;
bool WrkCurrent;

int TimedCombo(byte, bool);	// function prototype

// firmware update
EthernetUDP UpdateComm;
uint16_t UpdateReceivePort = 29100;
uint16_t UpdateSendPort = 29000;
uint32_t buffer_addr, buffer_size;
bool UpdateMode = false;

//******************************************************************************
// hex_info_t struct for hex record and hex file info
//******************************************************************************
typedef struct {  //
	char* data;   // pointer to array allocated elsewhere
	unsigned int addr;  // address in intel hex record
	unsigned int code;  // intel hex record type (0=data, etc.)
	unsigned int num; // number of data bytes in intel hex record

	uint32_t base;  // base address to be added to intel hex 16-bit addr
	uint32_t min;   // min address in hex file
	uint32_t max;   // max address in hex file

	int eof;    // set true on intel hex EOF (code = 1)
	int lines;    // number of hex records received
} hex_info_t;

static char data[16];// buffer for hex data

hex_info_t hex =
{ // intel hex info struct
  data, 0, 0, 0,        //   data,addr,num,code
  0, 0xFFFFFFFF, 0,     //   base,min,max,
  0, 0					//   eof,lines
};


void setup()
{
	DoSetup();
}

void loop()
{
	ReceiveSerial();
	ReceiveUDPwired();
	ReceiveESP();
	ReceiveUpdate();
	SetPWM();

	if (millis() - LoopLast >= LoopTime)
	{
		LoopLast = millis();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 5000)
				&& ((Sensor[i].TargetUPM > 0 && MasterOn)
					|| ((Sensor[i].ControlType == 4) && (Sensor[i].TargetUPM > 0))
					|| (!AutoOn && MasterOn));
		}

		CheckRelays();
		GetUPM();
		AdjustFlow();
		ReadAnalog();
	}

	SendComm();
	Blink();
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

bool WorkPinOn()
{
	if (MDL.WorkPin < NC)
	{
		WrkCurrent = digitalRead(MDL.WorkPin);
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

bool State = false;
elapsedMillis BlinkTmr;
elapsedMicros LoopTmr;
byte ReadReset;
uint32_t MaxLoopTime;
double FlowHz;

//double debug1;
//double debug2;
//double debug3;
//double debug4;

void Blink()
{
	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
		if (!UpdateMode)
		{
			Serial.print(" Micros: ");
			Serial.print(MaxLoopTime);

			Serial.print(", ");
			Serial.print(Ethernet.localIP());
			Serial.print(", ");
			Serial.print(FlowHz);

			//Serial.print(", ");
			//Serial.print(debug1);

			//debug2 = PressureReading;
			//Serial.print(", ");
			//Serial.print(debug2);

			//Serial.print(", ");
			//Serial.print(debug3);

			//Serial.print(", ");
			//Serial.print(debug4);

			Serial.println("");

			if (ReadReset++ > 10)
			{
				ReadReset = 0;
				MaxLoopTime = 0;
			}
		}
	}
	if (LoopTmr > MaxLoopTime) MaxLoopTime = LoopTmr;
	LoopTmr = 0;
}
