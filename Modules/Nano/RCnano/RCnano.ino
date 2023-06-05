#include <Adafruit_MCP23008.h>
#include <Adafruit_MCP23X08.h>
#include <Adafruit_MCP23X17.h>
#include <Adafruit_MCP23XXX.h>
#include <Wire.h>
#include <EEPROM.h>

#include <Adafruit_BusIO_Register.h>
#include <Adafruit_I2CDevice.h>
#include <Adafruit_I2CRegister.h>
#include <Adafruit_SPIDevice.h>

#include <SPI.h>
#include <EtherCard.h>

# define InoDescription "SlowPulse : 04-Jun-2023"
const uint16_t InoID = 4063;	// change to send defaults to eeprom, ddmmy, no leading 0
int16_t StoredID;				// Defaults ID stored in eeprom	

#define MaxProductCount 2

struct ModuleConfig
{
	uint8_t ID = 0;
	uint8_t SensorCount = 2;        // up to 2 sensors
	uint8_t	IPpart3 = 1;			// IP address, 3rd octet
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t UseMCP23017 = 1;        // 0 use Nano pins for relays, 1 use MCP23017 for relays
	uint8_t Relays[16];
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin;
	uint8_t DirPin;
	uint8_t PWMPin;
	bool FlowEnabled;
	float RateError;		// rate error X 1000
	float UPM;				// upm X 1000
	int pwmSetting;
	uint32_t CommTime;
	byte InCommand;			// command byte from RateController
	byte ControlType;		// 0 standard, 1 combo close, 2 motor, 3 motor/weight, 4 fan
	uint32_t TotalPulses;
	float RateSetting;
	float MeterCal;
	float ManualAdjust;
	float KP;
	float KI;
	float KD;
	byte MinPWM;
	byte MaxPWM;
	byte Deadband;
	byte BrakePoint;
	bool UseMultiPulses;	// 0 - time for one pulse, 1 - average time for multiple pulses
	uint8_t Debounce;
};

SensorConfig Sensor[2];

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
unsigned int SourcePort = 5123;		// to send from

// AGIO
uint16_t ListeningPortAGIO = 8888;		// to listen on
uint16_t DestinationPortAGIO = 9999;	// to send to

// ethernet destination - Rate Controller
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[300]; // udp send and receive buffer
bool ENCfound;
static byte selectPin = 10;

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

const uint16_t LOOP_TIME = 50;      //in msec = 20hz
uint32_t LoopLast = LOOP_TIME;

const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

//bit 0 is section 0
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

bool AutoOn = true;
bool MasterOn = true;

// WifiSwitches connection to Wemos D1 Mini
// Use Serial RX, remove RX wire before uploading
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

 //reset function
void(*resetFunc) (void) = 0;

bool IOexpanderFound;
uint8_t ErrorCount;
byte PGNlength;

volatile unsigned long debug1;
volatile int debug2;
volatile unsigned long debug3;
volatile unsigned long debug4;
int debug5;

bool SendStatusPGN;

void setup()
{
	// default flow pins
	Sensor[0].FlowPin = 2;
	Sensor[0].DirPin = 4;
	Sensor[0].PWMPin = 5;

	Sensor[1].FlowPin = 3;
	Sensor[1].DirPin = 6;
	Sensor[1].PWMPin = 9;

	// default pid
	Sensor[0].MinPWM = 5;
	Sensor[0].MaxPWM = 50;
	Sensor[0].Deadband = 4;
	Sensor[0].BrakePoint = 20;

	Sensor[1].MinPWM = 5;
	Sensor[1].MaxPWM = 50;
	Sensor[1].Deadband = 4;
	Sensor[1].BrakePoint = 20;

	Sensor[0].Debounce = 3;
	Sensor[1].Debounce = 3;

	Serial.begin(38400);
	delay(2000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	// eeprom
	EEPROM.get(0, StoredID);

	if (StoredID == InoID)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(10, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.get(100 + i * 80, Sensor[i]);
		}
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(0, InoID);
		EEPROM.put(10, MDL);

		for (int i = 0; i < MaxProductCount; i++)
		{
			EEPROM.put(100 + i * 80, Sensor[i]);
		}
	}

	Serial.println("");
	Serial.print("Module ID: ");
	Serial.println(MDL.ID);
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

	for (int i = 0; i < MDL.SensorCount; i++)
	{
		pinMode(Sensor[i].FlowPin, INPUT_PULLUP);
		//pinMode(Sensor[i].FlowPin, INPUT);	// for direct connection to inductive sensor
		pinMode(Sensor[i].DirPin, OUTPUT);
		pinMode(Sensor[i].PWMPin, OUTPUT);
	}

	attachInterrupt(digitalPinToInterrupt(Sensor[0].FlowPin), ISR0, FALLING);
	//attachInterrupt(digitalPinToInterrupt(Sensor[0].FlowPin), ISR0Alt, FALLING);

	attachInterrupt(digitalPinToInterrupt(Sensor[1].FlowPin), ISR1, FALLING);

	Serial.println("");
	Serial.println("Starting Ethernet ...");

	// ethernet interface ip address
	byte ArduinoIP[] = { 192, 168,MDL.IPpart3, 50 + MDL.ID };

	// ethernet interface Mac address
	byte LocalMac[] = { 0x70, 0x31, 0x21, 0x2D, 0x62, MDL.ID };

	// gateway ip address
	static byte gwip[] = { 192, 168,MDL.IPpart3, 1 };

	//DNS- you just need one anyway
	static byte myDNS[] = { 8, 8, 8, 8 };

	//mask
	static byte mask[] = { 255, 255, 255, 0 };

	DestinationIP[2] = MDL.IPpart3;

	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println("Ethernet controller found.");
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
		ether.printIp("IP Address:     ", ether.myip);

		//register sub for received data
		ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
		ether.udpServerListenOnPort(&ReceiveAGIO, ListeningPortAGIO);
	}
	else
	{
		Serial.println("");
		Serial.println("Ethernet controller not found.");
	}

	Serial.println("");
	Serial.println("Finished Setup.");
}

void loop()
{
	//DebugTheIno();

	if (millis() - LoopLast >= LOOP_TIME)
	{
		LoopLast = millis();
		GetUPM();

		for (int i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].FlowEnabled = (millis() - Sensor[i].CommTime < 4000)
				&& ((Sensor[i].RateSetting > 0 && MasterOn)
					|| ((Sensor[i].ControlType == 4) && (Sensor[i].RateSetting > 0))
						|| (!AutoOn && MasterOn));
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

		if(SendStatusPGN) SendStatus(0);
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		//SendSerial();
		SendUDPwired();
	}

	if (ENCfound)
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}

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
		Sensor[i].RateError = Sensor[i].RateSetting - Sensor[i].UPM;

		switch (Sensor[i].ControlType)
		{
		case 2:
		case 3:
		case 4:
			// motor control
			Sensor[i].pwmSetting = PIDmotor(i);
			break;

		default:
			// valve control
			Sensor[i].pwmSetting = PIDvalve(i);
			break;
		}
	}
}

void ManualControl()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].pwmSetting = Sensor[i].ManualAdjust;
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
uint32_t MaxLoopTime;
uint32_t LoopTmr;
byte ReadReset;

void DebugTheIno()
{
	if (millis() - DebugTime > 1000)
	{
		DebugTime = millis();
		Serial.println("");

		Serial.print(" Micros: ");
		Serial.print(MaxLoopTime);

		Serial.print(", ");
		Serial.print(debug1);

		Serial.print(", ");
		Serial.print(debug2);

		Serial.print(", ");
		Serial.print(debug3);

		Serial.print(", ");
		Serial.print(debug5);

		Serial.print(", Debounce ");
		Serial.print(Sensor[0].Debounce);

		Serial.println("");

		if (ReadReset++ > 10)
		{
			ReadReset = 0;
			MaxLoopTime = 0;
		}
	}
	if (micros() - LoopTmr > MaxLoopTime) MaxLoopTime = micros() - LoopTmr;
	LoopTmr = micros();
}
