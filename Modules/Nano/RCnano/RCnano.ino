#include <SPI.h>
#include <Adafruit_MCP23008.h>
#include <Adafruit_MCP23X08.h>
#include <Adafruit_MCP23X17.h>
#include <Adafruit_MCP23XXX.h>
#include <EtherCard.h>
#include <Wire.h>
#include <EEPROM.h>

#include <Adafruit_BusIO_Register.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_SPIDevice.h>

# define InoDescription "RCnano  :  04-Feb-2023"
const int16_t InoID = 4900;	// change to send defaults to eeprom
int16_t StoredID;			// Defaults ID stored in eeprom	

# define UseEthernet 0

float debug1;
float debug2;
float debug3;

struct ModuleConfig    // 5 bytes
{
	uint8_t ModuleID = 0;
	uint8_t UseMCP23017 = 1;        // 0 use Nano pins for relays, 1 use MCP23017 for relays
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t SensorCount = 2;        // up to 2 sensors
	uint8_t	IPpart3 = 3;			// IP address, 3rd octet
	uint8_t Flow1 = 2;
	uint8_t Flow2 = 3;
	uint8_t Dir1 = 4;
	uint8_t Dir2 = 6;
	uint8_t PWM1 = 5;
	uint8_t PWM2 = 9;
	uint8_t Relays[16];
	uint8_t Debounce = 3;			// minimum ms pin change
};

ModuleConfig MDL;

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

#if UseEthernet
// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
unsigned int SourcePort = 6100;		// to send from

// ethernet destination - Rate Controller
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[500]; // udp send and receive buffer
#endif

Adafruit_MCP23X17 mcp;

// Pin number is an integer in the range 0-15,
// where pins numbered from 0 to 7 are on Port A, GPA0 = 0,
// and pins numbered from 8 to 15 are on Port B, GPB0 = 8.

// MCP23017 pins RC5, RC8
#define Relay1 8
#define Relay2 9
#define Relay3 10
#define Relay4 11
#define Relay5 12
#define Relay6 13
#define Relay7 14
#define Relay8 15

#define Relay9 7
#define Relay10 6
#define Relay11 5
#define Relay12 4
#define Relay13 3
#define Relay14 2
#define Relay15 1
#define Relay16 0

// flow
byte FlowPin[2];
byte FlowDir[2];
byte FlowPWM[2];

bool FlowEnabled[] = { false, false };
float rateError[] = { 0, 0 };

const uint16_t LOOP_TIME = 50;      //in msec = 20hz
uint32_t LoopLast = LOOP_TIME;

const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

float UPM[2];   // UPM rate
int pwmSetting[2];

// PID
float PIDkp[2];
float PIDki[2];
float PIDkd[2];
byte MinPWM[2];
byte MaxPWM[2];

byte InCommand[] = { 0, 0 };		// command byte from RateController
byte ControlType[] = { 0, 0 };		// 0 standard, 1 Fast Close, 2 Motor, 3 Motor/weight, 4 fan

unsigned long TotalPulses[2];
unsigned long CommTime[2];

float RateSetting[] = { 0.0, 0.0 };	// auto UPM setting
float MeterCal[] = { 1.0, 1.0 };	// pulses per Unit

//bit 0 is section 0
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

byte Temp = 0;
bool AutoOn = true;

float ManualAdjust[2];
unsigned long ManualLast[2];

// WifiSwitches connection to Wemos D1 Mini
// Use Serial RX, remove RX wire before uploading
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
bool UseMultiPulses[2] = { 0, 0 };   //  0 - average time for multiple pulses, 1 - time for one pulse

 //reset function
void(*resetFunc) (void) = 0;

bool MasterOn[2];
bool IOexpanderFound;
uint8_t ErrorCount;
byte PGNlength;

byte BrakePoint = 30;	// %
byte Deadband = 3;		// %

void setup()
{
	Serial.begin(38400);

	// module data
	EEPROM.get(0, StoredID);
	if (StoredID == InoID)
	{
		// load stored data
		EEPROM.get(10, MDL);
	}
	else
	{
		Serial.println("Updating stored data.");
		// update stored data
		EEPROM.put(0, InoID);
		EEPROM.put(10, MDL);
	}

	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.print("Module ID: ");
	Serial.println(MDL.ModuleID);
	Serial.println();

	if (MDL.SensorCount < 1) MDL.SensorCount = 1;
	if (MDL.SensorCount > 2) MDL.SensorCount = 2;

	if (MDL.UseMCP23017)
	{
		Serial.println("Using MCP23017 for relays.");
	}
	else
	{
		Serial.println("Using Nano pins for relays.");
	}

	Wire.begin();
	if (MDL.UseMCP23017)
	{
		// I/O expander on default address 0x20
		Serial.println("");
		Serial.println("Starting I/O Expander ...");
		ErrorCount = 0;
		while (!IOexpanderFound)
		{
			Serial.print(".");
			Wire.beginTransmission(0x20);
			IOexpanderFound = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (IOexpanderFound)
		{
			Serial.println("I/O Expander found.");
			mcp.begin_I2C();

			// MCP20317 pins
			mcp.pinMode(Relay1, OUTPUT);
			mcp.pinMode(Relay2, OUTPUT);
			mcp.pinMode(Relay3, OUTPUT);
			mcp.pinMode(Relay4, OUTPUT);
			mcp.pinMode(Relay5, OUTPUT);
			mcp.pinMode(Relay6, OUTPUT);
			mcp.pinMode(Relay7, OUTPUT);
			mcp.pinMode(Relay8, OUTPUT);

			mcp.pinMode(Relay9, OUTPUT);
			mcp.pinMode(Relay10, OUTPUT);
			mcp.pinMode(Relay11, OUTPUT);
			mcp.pinMode(Relay12, OUTPUT);
			mcp.pinMode(Relay13, OUTPUT);
			mcp.pinMode(Relay14, OUTPUT);
			mcp.pinMode(Relay15, OUTPUT);
			mcp.pinMode(Relay16, OUTPUT);

		}
		else
		{
			Serial.println("I/O Expander not found.");
		}
	}
	else
	{
		// Nano pins
		for (int i = 0; i < 16; i++)
		{
			// check if relay is enabled (pins 0 and 1 are for comm) and set pin mode
			if (MDL.Relays[i] > 1) pinMode(MDL.Relays[i], OUTPUT);
		}
	}

	// flow
	FlowPin[0] = MDL.Flow1;
	FlowPin[1] = MDL.Flow2;
	FlowDir[0] = MDL.Dir1;
	FlowDir[1] = MDL.Dir2;
	FlowPWM[0] = MDL.PWM1;
	FlowPWM[1] = MDL.PWM2;

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(FlowPin[i], INPUT_PULLUP);
		pinMode(FlowDir[i], OUTPUT);
		pinMode(FlowPWM[i], OUTPUT);
	}

	attachInterrupt(digitalPinToInterrupt(FlowPin[0]), ISR0, FALLING);
	attachInterrupt(digitalPinToInterrupt(FlowPin[1]), ISR1, FALLING);

#if UseEthernet
	Serial.println("");
	Serial.println("Starting Ethernet ...");
	// ethernet interface ip address
	byte ArduinoIP[] = { 192, 168,MDL.IPpart3, 50 + MDL.ModuleID };

	// ethernet interface Mac address
	byte LocalMac[] = { 0x70, 0x31, 0x21, 0x2D, 0x62, MDL.ModuleID };

	// gateway ip address
	static byte gwip[] = { 192, 168,MDL.IPpart3, 1 };

	//DNS- you just need one anyway
	static byte myDNS[] = { 8, 8, 8, 8 };

	//mask
	static byte mask[] = { 255, 255, 255, 0 };

	DestinationIP[2] = MDL.IPpart3;

	ether.begin(sizeof Ethernet::buffer, LocalMac, 10);

	Serial.println("");
	Serial.println("Ethernet controller found.");
	ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
	ether.printIp("IP Address:     ", ether.myip);

	//register sub for received data
	ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif

	Serial.println("");
	Serial.println("Finished Setup.");
}

void loop()
{
	DebugTheIno();

	if (millis() - LoopLast >= LOOP_TIME)
	{
		LoopLast = millis();
		GetUPM();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			FlowEnabled[i] = (millis() - CommTime[i] < 4000)
				&& ((RateSetting[i] > 0 && MasterOn[i])
					|| ((ControlType[i] == 4) && (RateSetting[i] > 0))
						|| (!AutoOn && MasterOn[i]));
		}

		CheckRelays();
		AdjustFlow();

		if (AutoOn)
		{
			AutoControl();
		}
		else
		{
			ManualControl();
		}
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		SendSerial();
#if UseEthernet
		SendUDPwired();
#endif
	}

#if UseEthernet
	//this must be called for ethercard functions to work.
	ether.packetLoop(ether.packetReceive());
#endif

	ReceiveSerial();
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

void AutoControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		rateError[i] = RateSetting[i] - UPM[i];

		switch (ControlType[i])
		{
		case 2:
		case 3:
		case 4:
			// motor control
			pwmSetting[i] = PIDmotor(PIDkp[i], PIDki[i], PIDkd[i], rateError[i], RateSetting[i], MinPWM[i], MaxPWM[i], i);
			break;

		default:
			// valve control
			pwmSetting[i] = PIDvalve(PIDkp[i], PIDki[i], PIDkd[i], rateError[i], RateSetting[i], MinPWM[i], MaxPWM[i], i);
			break;
		}
	}
}

void ManualControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pwmSetting[i] = ManualAdjust[i];
	}
}

void TranslateSwitchBytes()
{
	// Switch IDs from Rate Controller
	// ex: byte 2: bits 0-3 identify switch # (0-15) for sec 0
	// ex: byte 2: bits 4-7 identify switch # (0-15) for sec 1

	for (int i = 0; i < 16; i++)
	{
		byte ByteID = i / 2;
		byte Mask = 15 << (4 * (i - 2 * ByteID));    // move mask to correct bits
		SectionSwitchID[i] = SwitchBytes[ByteID] & Mask;    // mask out bits
		SectionSwitchID[i] = SectionSwitchID[i] >> (4 * (i - 2 * ByteID)); // move bits for number
	}
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


uint32_t DebugTime;
void DebugTheIno()
{
	if (millis() - DebugTime > 1000)
	{
		DebugTime = millis();
		Serial.println("");
		Serial.print(FlowEnabled[0]);

		Serial.print(", ");
		Serial.print(pwmSetting[0]);

		Serial.print(", ");
		Serial.print(debug1);

		//Serial.print(", ");
		//Serial.print(debug2);

		//Serial.print(", ");
		//Serial.print(debug3);

		Serial.print(", ");
		Serial.print(AutoOn);

		Serial.println("");
	}
}

