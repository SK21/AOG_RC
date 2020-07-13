
// user settings ****************************
#define CommType 0			// 0 Serial/USB , 1 UDP wired Nano, 2 UDP wifi Nano33

#define PCBversion	4		// 3 - ver3.1, 4 - ver4 (Nano only)

byte FlowOn = HIGH;			// flowmeter pin on value
bool UseSwitches = true;	// manual switches

int SecID[] = { 1, 2, 3, 4, 0, 0, 0, 0 }; // id of switch controlling relay, 1,2,3,4 or 0 for none

long SendTime = 200;	// ms pwm is sent to valve
long WaitTime = 750;	// ms to wait before adjusting valve again
byte SlowSpeed = 9;		// low pwm rate, 0 fast, 9 slow

unsigned long RateCheckInterval = 500;	// ms interval when checking rate error

#define UseSwitchedPowerPin 1	// 0 use Relay8 as a normal relay
								// 1 use Relay8 as a switched power pin - turns on when sketch starts, required for Raven valve

#define WifiSSID "tractor"
#define WifiPassword ""
#define IPpart3 1	// ex: 192.168.IPpart3.255

// ******************************************

#if (CommType == 1)
#include <EtherCard.h>

// ethernet mac address - must be unique on your network
static byte LocalMac[] = { 0x70,0x69,0x69,0x2D,0x30,0x41 };

unsigned int ListeningPort = 9999;	// local port to listen on

// ethernet source - Arduino
static byte SourceIP[] = { 192,168,IPpart3,88 };
unsigned int SourcePort = 6100;		// local port to send from 

// ethernet destination - Rate Controller
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 8000; // Rate Controller listening port 

// gateway ip address
static byte gwip[] = { 192,168,IPpart3,1 };
//DNS- you just need one anyway
static byte myDNS[] = { 8,8,8,8 };
//mask
static byte mask[] = { 255,255,255,0 };

byte Ethernet::buffer[200]; // udp send and receive buffer

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
#define SW1pin A5
#define SW2pin A4
#define SW3pin A3
#define SW4pin A2
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
#define SW1pin 10
#define SW2pin 11
#define SW3pin 12
#define SW4pin 13
#define AutoPin 14

// Nano pins
#define FlowPin 3	// interrupt on this pin
#define FlowDIR 4
#define FlowPWM 9
#define RateUpPin A0
#define RateDownPin A1	
#endif

//loop time variables in microseconds
const unsigned long LOOP_TIME = 200; //in msec = 5hz 
unsigned long lastTime = LOOP_TIME;
unsigned long currentTime = LOOP_TIME;
byte watchdogTimer = 0;

//bit 0 is section 0
byte RelayHi = 0;	// 8-15
byte RelayFromAOG = 0; // bytes from AOG, 0-7
byte RelayControl = 0;
byte RelayToAOG = 0;
int RelayTemp = 0;
bool RelaysOn = false;

float rateSetPoint = 0.0;
float rateError = 0; //for PID

// flow rate
volatile unsigned long pulseDuration;
volatile unsigned long pulseCount = 0;
float FlowRate = 0;
float accumulatedCounts = 0;

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
bool SW1on = false;
bool SW2on = false;
bool SW3on = false;
bool SW4on = false;

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

float pwmSetting = 0.0;
float pwmSettingManual = 0.0;
float HalfWay = 0.0;
boolean AOGconnected = false;

byte InCommand = 0;	// command byte from AOG
byte Temp = 0;

float DeadBand;

bool PGN32742Found;
bool PGN32744Found;
unsigned int tempHeader;	// must be unsigned
unsigned int header;		// must be unsigned

unsigned long RateCheckLast = 0;

float MeterCal = 17;	// pulses per Unit 
byte ValveType = 1;		// 0 standard, 1 Fast Close

bool SimulateFlow = true;
byte MinPWMvalue = 150;
byte MaxPWMvalue = 255;

long VCN = 743;

unsigned int UnSignedTemp = 0;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("RCarduino  :  12/Jul/2020");
	Serial.println();

#if (CommType == 1)
	if (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) == 0)
		Serial.println(F("Failed to access Ethernet controller"));

	//set up connection
	ether.staticSetup(SourceIP, gwip, myDNS, mask);
	ether.printIp("IP:  ", ether.myip);
	ether.printIp("GW:  ", ether.gwip);
	ether.printIp("DNS: ", ether.dnsip);

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
	pinMode(SW1pin, INPUT_PULLUP);
	pinMode(SW2pin, INPUT_PULLUP);
	pinMode(SW3pin, INPUT_PULLUP);
	pinMode(SW4pin, INPUT_PULLUP);

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

	mcp.pinMode(SW1pin, INPUT);
	mcp.pullUp(SW1pin, HIGH);

	mcp.pinMode(SW2pin, INPUT);
	mcp.pullUp(SW2pin, HIGH);

	mcp.pinMode(SW3pin, INPUT);
	mcp.pullUp(SW3pin, HIGH);

	mcp.pinMode(SW4pin, INPUT);
	mcp.pullUp(SW4pin, HIGH);

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

	attachInterrupt(digitalPinToInterrupt(FlowPin), FlowISR, RISING);
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

	if (UseSwitches)
	{
		ReadSectionSwitches();
		ReadRateSwitch();
	}
	else
	{
		// use control byte from AOG
		RelayControl = RelayFromAOG;
		RelayToAOG = 0;
		AutoOn = true;
	}

	SetRelays();

	if (millis() - RateCheckLast > RateCheckInterval)
	{
		RateCheckLast = millis();
		if (RelaysOn)
		{
			if (SimulateFlow) DoSimulate();
			rateError = CalRateError();
		}
	}

	if (RelaysOn && AutoOn)
	{
		pwmSetting = VCNpwm(rateError, rateSetPoint, MinPWMvalue, MaxPWMvalue,
			VCN, FlowRate, SendTime, WaitTime, SlowSpeed, ValveType);
	}

	motorDrive();

	currentTime = millis();
	if (currentTime - lastTime >= LOOP_TIME)
	{
		lastTime = currentTime;

		// check connection to AOG
		watchdogTimer++;
		if (watchdogTimer > 30)
		{
			//clean out serial buffer
			while (Serial.available() > 0) char t = Serial.read();

			watchdogTimer = 0;
			AOGconnected = false;
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
