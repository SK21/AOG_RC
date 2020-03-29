// #includes - need to be before first line of actual code so that all necessary prototypes are added
#include <EtherCard.h>

// works with Nano on USB or wired UDP,  PCB 3.1 

// LED on pin 13 may need to be disconnected / burnt with soldering iron

// user settings ****************************
#define CommType 0				// 0 Serial/USB, 1 UDP wired
int SecID[] = { 1, 2, 3, 4, 0, 0, 0, 0 }; // id of switch controlling relay, 1,2,3,4 or 0 for none
byte FlowOn = LOW;	// flowmeter pin on direction
bool UseSwitches = true;	// manual switches
float MeterCal = 188;		// pulses per Unit 
byte ValveType = 1;			// 0 standard, 1 Fast Close
bool SimulateFlow = true;	
byte MinPWMvalue = 145;
byte MaxPWMvalue = 255;
#define UseSwitchedPowerPin 0	// 0 use Relay8 as a normal relay, 1 use Relay8 as a switched power pin - turns on when sketch starts
// ******************************************

#define Relay1 3
#define Relay2 4
#define Relay3 5
#define Relay4 6
#define Relay5 7
#define Relay6 8
#define Relay7 11
#define Relay8 12

#define FlowPin 2	// interrupt on this pin
#define FlowPWM 9
#define FlowDIR 10

#define RateDownPin 13	// disconnect LED for some Nano's to work
#define RateUpPin A0
#define AutoPin A1
#define SW1pin A5
#define SW2pin A4
#define SW3pin A3
#define SW4pin A2
#define MasterOffPin A6
#define MasterOnPin A7

//loop time variables in microseconds
const unsigned long LOOP_TIME = 200; //in msec = 5hz 
unsigned long lastTime = LOOP_TIME;
unsigned long currentTime = LOOP_TIME;
byte watchdogTimer = 0;

//Kalman variables
float Pc = 0.0;
float G = 0.0;
float P = 1.0;
float Xp = 0.0;
float Zp = 0.0;
float RateAppliedUPM = 0.0; //the filtered flowrate
const float varRate = 100; // variance, smaller, more filtering
const float varProcess = 10;

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
float pulseAverage = 0;
unsigned long accumulatedCounts = 0;
float countsThisLoop = 0;
unsigned long DurationThisLoop = 0;

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
bool RateDownMan = false;
bool RateUpMan = false;
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
int pwmManualRatio = 0;
unsigned int unsignedTemp = 0;
boolean AOGconnected = false;

byte InCommand = 0;	// command byte from AOG
byte Temp = 0;

// PID
float KP;
float KI;
float KD;
float DeadBand;

bool PGN31300Found;
bool PGN31400Found;
int tempHeader;
int header;

#if (CommType == 1)
// wired UDP comm
// ethernet mac address - must be unique on your network
static byte LocalMac[] = { 0x70,0x69,0x69,0x2D,0x30,0x41 };

unsigned int ListeningPort = 8888;	// local port to listen on

// ethernet source - Arduino
static byte SourceIP[] = { 192,168,1,88 };
unsigned int SourcePort = 6100;		// local port to send from 

// ethernet destination - AOG
static byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 9999; //AOG port that listens 

// gateway ip address
static byte gwip[] = { 192,168,1,1 };
//DNS- you just need one anyway
static byte myDNS[] = { 8,8,8,8 };
//mask
static byte mask[] = { 255,255,255,0 };

byte Ethernet::buffer[200]; // udp send and receive buffer

//Array to send data back to AgOpenGPS
byte toSend[] = { 0,0,0,0,0,0,0,0,0,0 };
#endif

float PercentError = 0.0;
byte HiByte;
byte LoByte;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("Rate Control Nano 400 :  08/Mar/2020");
	Serial.println();

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

	attachInterrupt(digitalPinToInterrupt(FlowPin), FlowPinISR, CHANGE);

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

#if(UseSwitchedPowerPin == 1)
	// turn on
	digitalWrite(Relay8, HIGH);
#endif
}

void loop()
{
#if (CommType == 0)
	ReceiveSerial();
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

		if (SimulateFlow) DoSimulate();

		if (RelaysOn)
		{
			rateError = CalRateError();
			pwmSetting = DoPID(rateError, rateSetPoint, LOOP_TIME, MinPWMvalue, MaxPWMvalue, KP, KI, KD, DeadBand);

			PercentError = 0.0;
			if (rateSetPoint > 0) PercentError = (rateError / rateSetPoint) * 100;
		}

		motorDrive();

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
}

void ConvertToSignedBytes(int Num)
{
	// converts a negative integer to positive and sets bit 8 on the HiByte
	// max # size is 32767
	bool Neg = (Num < 0);
	Num = abs(Num);
	if (Num > 32767) Num = 32767;
	HiByte = Num >> 8;
	LoByte = Num;
	if (Neg) HiByte = HiByte | B10000000;
}