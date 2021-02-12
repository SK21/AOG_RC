
// user settings ****************************

#define CommType 0			// 0 Serial/USB , 1 UDP wired Nano, 2 UDP wifi Nano33

bool UseSwitches = false;	// manual switches

#define WifiSSID "tractor"
#define WifiPassword ""

#define IPpart3 1			// ex: 192.168.IPpart3.255, 0-255
#define IPMac 100			// unique number for Arduino IP address and Mac part 6, 0-255

byte FlowOn = HIGH;			// flowmeter pin on value

int SecID[] = { 0, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // id of switch controlling relay, 0-3

byte SlowSpeed = 9;		// low pwm rate, 0 fast, 9 slow

const unsigned long LOOP_TIME = 200; //in msec = 5hz 

#define UseSwitchedPowerPin 1	// 0 use Relay8 as a normal relay
								// 1 use Relay8 as a switched power pin - turns on when sketch starts, required for Raven valve

#define PCBversion	4		// 3 - ver3.1, 4 - ver4 (Nano only)
#define ControllerID 0		// unique ID 0-4
#define LowMsPulseTrigger 50 	// ms/pulse below which is low ms/pulse

// ******************************************

#if (CommType == 1)
#include <EtherCard.h>
// ethernet interface ip address
static byte ArduinoIP[] = { 192,168,IPpart3,IPMac };

// ethernet interface Mac address
static byte LocalMac[] = { 0x70,0x2D,0x31,0x21,0x62,IPMac };

// gateway ip address
static byte gwip[] = { 192,168,IPpart3,1 };
//DNS- you just need one anyway
static byte myDNS[] = { 8,8,8,8 };
//mask
static byte mask[] = { 255,255,255,0 };

// local ports on Arduino
unsigned int ListeningPort = 9999;	// to listen on
unsigned int SourcePort = 6100;		// to send from 

// ethernet destination - Rate Controller
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 8000; // Rate Controller listening port 

byte Ethernet::buffer[500]; // udp send and receive buffer

//Array to send data back to AgOpenGPS
byte toSend[] = { 0,0,0,0,0,0,0,0,0,0 };
#endif

#if (CommType == 2)
#include <SPI.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>

int ConnectionStatus = WL_IDLE_STATUS;
char ssid[] = WifiSSID;        // your network SSID (name)
char pass[] = WifiPassword;    // your network password (use for WPA, or use as key for WEP)

char InBuffer[150];	 //buffer to hold incoming packet
byte toSend[] = { 0,0,0,0,0,0,0,0,0,0 }; //Array to send data back to AgOpenGPS

WiFiUDP UDPin;
WiFiUDP UDPout;

unsigned int ListeningPort = 9999;	// local port to listen on
unsigned int SourcePort = 6100;		// local port to send from 

// ethernet destination - AOG
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 8000; // RateControl port that listens 

unsigned long CheckTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;
#endif

#if (PCBversion == 3)
#define Relay1 3
#define Relay2 4
#define Relay3 5
#define Relay4 6
#define Relay5 7
#define Relay6 8
#define Relay7 11
#define Relay8 12

#define MasterOnPin A7
#define MasterOffPin A6
#define SW0pin A5
#define SW1pin A4
#define SW2pin A3
#define SW3pin A2
#define AutoPin A1

#define FlowPin 2	// interrupt on this pin
#define FlowDIR 10
#define FlowPWM 9
#define RateUpPin A0
#define RateDownPin 13	// disconnect LED for some Nano's to work
#endif

#if (PCBversion == 4)
#include "Adafruit_MCP23017.h"

Adafruit_MCP23017 mcp;

// MCP23017 pins
#define Relay1 0
#define Relay2 1
#define Relay3 2
#define Relay4 3
#define Relay5 4
#define Relay6 5
#define Relay7 6
#define Relay8 7

#define MasterOnPin 8
#define MasterOffPin 9
#define SW0pin 10
#define SW1pin 11
#define SW2pin 12
#define SW3pin 13
#define AutoPin 14

// Nano pins
#define FlowPin 2	// interrupt on this pin
#define FlowDIR 4
#define FlowPWM 9
#define RateUpPin A0
#define RateDownPin A1	
#endif

//loop time variables in microseconds
unsigned long lastTime = LOOP_TIME;
byte watchdogTimer = 0;

byte RelayControl = 0;
byte RelayToAOG = 0;
int RelayTemp = 0;
bool ApplicationOn = false;

float rateError = 0; //for PID

// UPM rate
float UPM;
float TotalPulses;

// AOG section buttons
byte SecSwOff[2] = { 0, 0 }; // AOG section button off if set
//			- byte 0 is sections 0 to 7
//			- byte 1 is sections 8 to 15

byte OutCommand = 0;	// command byte out to AOG
//			- bit 0		- AOG section buttons auto (xxxxxxx1)
//			- bit 1		- AOG section buttons auto off (xxxxxx1x)
//			- bits 2,3	- change rate steps 0 (xxxx00xx) no change, 1 (xxxx01xx), 2 (xxxx10xx), 3 (xxxx11xx)
//			- bit 4		- 0 change rate left (xxx0xxxx), 1 change rate right (xxx1xxxx)
//			- bit 5		- 0 rate down (xx0xxxxx), 1 rate up (xx1xxxxx)

// rate switches
unsigned long RateDelayTime = 1500;  // time between adjustments to rate
unsigned long RateLastTime = 0;
bool RateDownPressed = false;
bool RateUpPressed = false;

// section switches
unsigned long SWreadTime = 0;
bool SW0on = false;
bool SW1on = false;
bool SW2on = false;
bool SW3on = false;

bool PinState = false;
const unsigned long SWdelay = 1400; //time the arduino waits after manual Switch is used
byte RelayControlLast = 0;

bool AutoOn = false;
bool AutoChanged = false;
unsigned long AutoTime = 0;
bool AutoLast = false;

bool MasterOn = true;
bool MasterChanged = false;
unsigned long MasterTime = 0;
bool MasterLast = true;

int pwmSetting = 0;
int pwmSettingManual = 0;
boolean ControllerConnected = false;

byte Temp = 0;
unsigned int UnSignedTemp = 0;

bool PGN32614Found;
bool PGN32615Found;
bool PGN32616Found;
bool PGN32617Found;

//bit 0 is section 0
byte RelayHi = 0;			// 8-15
byte RelayFromAOG = 0;		// bytes from AOG, 0-7
float rateSetPoint = 0.0;
float MeterCal = 17;		// pulses per Unit 
byte InCommand = 0;			// command byte from AOG
byte ControlType = 2;			// 0 standard, 1 Fast Close, 2 Motor
bool SimulateFlow = true;

// VCN
long VCN = 343;
long SendTime = 400;	// ms pwm is sent to valve
long WaitTime = 500;	// ms to wait before adjusting valve again
byte MinPWMvalue = 200;
byte MaxPWMvalue = 255;
bool UseVCN = 1;		// 0 PID, 1 VCN

// PID
byte PIDkp = 20;
byte PIDminPWM = 50;
byte PIDLowMax = 100;
byte PIDHighMax = 255;
byte PIDdeadband = 3;
byte PIDbrakePoint = 20;

byte PWMhi;
byte PWMlo;

byte MSB;
byte LSB;
unsigned int PGN;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("RCarduino  :  12-Feb-2021");
	Serial.println();

#if (CommType == 1)
	if (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) == 0) Serial.println(F("Failed to access Ethernet controller"));
	ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

	ether.printIp("IP Address:     ", ether.myip);
	Serial.println("Destination IP: " + IPadd(DestinationIP));

	//register sub for received data
	ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif

#if (CommType == 2)
	// check for the WiFi module:
	if (WiFi.status() == WL_NO_MODULE)
	{
		Serial.println("Communication with WiFi module failed!");
		// don't continue
		while (true);
	}

	String fv = WiFi.firmwareVersion();
	Serial.println("Wifi firmware version: " + fv);

	UDPin.begin(ListeningPort);
	UDPout.begin(SourcePort);
	delay(1000);
#endif

#if (PCBversion == 3)
	pinMode(Relay1, OUTPUT);
	pinMode(Relay2, OUTPUT);
	pinMode(Relay3, OUTPUT);
	pinMode(Relay4, OUTPUT);
	pinMode(Relay5, OUTPUT);
	pinMode(Relay6, OUTPUT);
	pinMode(Relay7, OUTPUT);
	pinMode(Relay8, OUTPUT);

	pinMode(FlowPin, INPUT_PULLUP);
	pinMode(FlowPWM, OUTPUT);
	pinMode(FlowDIR, OUTPUT);

	pinMode(RateDownPin, INPUT_PULLUP);
	pinMode(RateUpPin, INPUT_PULLUP);
	pinMode(AutoPin, INPUT_PULLUP);
	pinMode(SW0pin, INPUT_PULLUP);
	pinMode(SW1pin, INPUT_PULLUP);
	pinMode(SW2pin, INPUT_PULLUP);
	pinMode(SW3pin, INPUT_PULLUP);

	// analog pins with external pullup, needed for Nano A6 and A7
	// A6	MasterOffPin
	// A7	MasterOnPin

#if(UseSwitchedPowerPin == 1)
	// turn on
	digitalWrite(Relay8, HIGH);
#endif

#endif

#if (PCBversion == 4)
	mcp.begin();      // use default address 0

// MCP20317 pins
	mcp.pinMode(Relay1, OUTPUT);
	mcp.pinMode(Relay2, OUTPUT);
	mcp.pinMode(Relay3, OUTPUT);
	mcp.pinMode(Relay4, OUTPUT);
	mcp.pinMode(Relay5, OUTPUT);
	mcp.pinMode(Relay6, OUTPUT);
	mcp.pinMode(Relay7, OUTPUT);
	mcp.pinMode(Relay8, OUTPUT);

	mcp.pinMode(MasterOnPin, INPUT);
	mcp.pullUp(MasterOnPin, HIGH);

	mcp.pinMode(MasterOffPin, INPUT);
	mcp.pullUp(MasterOffPin, HIGH);

	mcp.pinMode(SW0pin, INPUT);
	mcp.pullUp(SW0pin, HIGH);

	mcp.pinMode(SW1pin, INPUT);
	mcp.pullUp(SW1pin, HIGH);

	mcp.pinMode(SW2pin, INPUT);
	mcp.pullUp(SW2pin, HIGH);

	mcp.pinMode(SW3pin, INPUT);
	mcp.pullUp(SW3pin, HIGH);

	mcp.pinMode(AutoPin, INPUT);
	mcp.pullUp(AutoPin, HIGH);

	// Nano pins
	pinMode(FlowPin, INPUT_PULLUP);
	pinMode(FlowDIR, OUTPUT);
	pinMode(FlowPWM, OUTPUT);
	pinMode(RateUpPin, INPUT_PULLUP);
	pinMode(RateDownPin, INPUT_PULLUP);

#if(UseSwitchedPowerPin == 1)
	// turn on
	mcp.digitalWrite(Relay8, HIGH);
#endif

#endif

	attachInterrupt(digitalPinToInterrupt(FlowPin), PPMisr, RISING);
}

void loop()
{
#if (CommType == 0)
	ReceiveSerial();
#endif

#if (CommType == 2)
	CheckWifi();
	ReceiveUDPWifi();
#endif

	if (UseVCN)
	{
		PWMhi = MaxPWMvalue;
		PWMlo = MinPWMvalue;
	}
	else
	{
		PWMhi = PIDHighMax;
		PWMlo = PIDminPWM;
	}

	if (UseSwitches)
	{
		ReadSectionSwitches();
		ReadRateSwitch(PWMlo, PWMhi);
	}
	else
	{
		// use control byte from AOG
		RelayControl = RelayFromAOG;
		RelayToAOG = 0;
		AutoOn = true;
	}

	ApplicationOn = ControllerConnected && (rateSetPoint > 0);

	SetRelays();

	motorDrive();

	if (millis() - lastTime >= LOOP_TIME)
	{
		lastTime = millis();

		switch (ControlType)
		{
		case 2:
			// motor control
			if (SimulateFlow) SimulateMotor(PIDminPWM, PIDHighMax);
			rateError = rateSetPoint - GetUPM();

			if (AutoOn)
			{
				pwmSetting = ControlMotor(PIDkp, rateError, rateSetPoint, PIDminPWM,
					PIDHighMax, PIDdeadband);
			}
			break;
		default:
			// valve control
			if (SimulateFlow) SimulateValve(PWMlo, PWMhi);
			rateError = rateSetPoint - GetUPM();

			if (AutoOn)
			{
				if (UseVCN)
				{
					pwmSetting = VCNpwm(rateError, rateSetPoint, MinPWMvalue, MaxPWMvalue,
						VCN, UPM, SendTime, WaitTime, SlowSpeed, ControlType);
				}
				else
				{
					pwmSetting = DoPID(PIDkp, rateError, rateSetPoint, PIDminPWM, PIDLowMax,
						PIDHighMax, PIDbrakePoint, PIDdeadband);
				}
			}
			break;
		}

		// check connection to AOG
		watchdogTimer++;
		if (watchdogTimer > 30)
		{
			//clean out serial buffer
			while (Serial.available() > 0) char t = Serial.read();

			watchdogTimer = 0;
			ControllerConnected = false;
		}

#if(CommType == 0)
		SendSerial();
	}
#endif

#if(CommType == 1)
	SendUDPwired();
}
delay(10);

//this must be called for ethercard functions to work. 
ether.packetLoop(ether.packetReceive());
#endif

#if(CommType == 2)
SendUDPWifi();
}
#endif
}

String IPadd(byte Address[])
{
	return String(Address[0]) + "." + String(Address[1]) + "." + String(Address[2]) + "." + String(Address[3]);
}

bool IsBitSet(byte b, int pos)
{
	return ((b >> pos) & 1) != 0;
}

