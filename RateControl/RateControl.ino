
// user settings ****************************
float MeterCal = 188;	// pulses per Unit 
byte ValveType = 1;	// 0 standard, 1 Fast Close
int SecID[7] = { 1, 2, 3, 4, 0, 0, 0 }; // id of switch controlling section, 1,2,3,4 or 0 for none
byte OnDirection = LOW;	// flowmeter pin
byte OffDirection = HIGH;	// flowmeter pin
byte MinPWMvalue = 145;
byte MaxPWMvalue = 255;
bool SimulateFlow = false;	
bool UseSwitches = false;	// manual switches

#define UseWifi 1		// 0 serial - Nano/Nano33, 1 wifi - Nano33

#define WifiSSID "AOG"
#define WifiPassword "tractor145"
// ******************************************

// for pcb 3.1
#define Section1 3
#define Section2 4
#define Section3 5
#define Section4 6
#define Section5 7
#define Section6 8
#define Section7 11
#define SwitchedPowerPin 12

#define FlowPin 2	// interrupt on this pin
#define FlowPWM 9
#define FlowDIR 10

#define RateDownPin 13
#define RateUpPin A0
#define AutoPin A1
//#define SW1pin A5
//#define SW2pin A4
#define SW1pin A0	// testing
#define SW2pin A0	// testing
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
float rateK = 0.0;
float Pc = 0.0;
float G = 0.0;
float P = 1.0;
float Xp = 0.0;
float Zp = 0.0;
float XeRate = 0.0; //the filtered flowrate
const float varRate = 100; // variance, smaller, more filtering
const float varProcess = 10;

//program flow
bool isDataFound = false, isSettingFound = false;
int header = 0, tempHeader = 0;

//bit 0 is section 0
byte relayHi = 0, relayLo = 0; // bytes from AOG
byte RelayControl = 0;
byte RelayToAOG = 0;
byte RelayTemp = 0;

float rateSetPoint = 0.0;
float rateError = 0; //for PID
int rateAppliedUPM = 0;	// units per minute

// flow rate
volatile unsigned long pulseDuration;
volatile unsigned long pulseCount = 0;
float FlowRate = 0;
float pulseAverage = 0;
unsigned long accumulatedCounts = 0;
float countsThisLoop = 0;


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
unsigned long RateDelayTime = 1000;  // time between adjustments to rate
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

bool MasterOn = false;
bool MasterChanged = false;
unsigned long MasterTime = 0;
bool MasterLast = false;

// debounce
unsigned long DebounceDelay = 50;

float pwmSetting = 0.0;
unsigned int unsignedTemp = 0;
boolean AOGconnected = false;

byte InCommand = 0;	// command byte from AOG
byte Temp = 0;

bool RelaysOn = false;

// PID
float KP;
float KI;
float KD;
float DeadBand;

#if UseWifi
	// wifi
	#include <SPI.h>
	#include <WiFiNINA.h>
	#include <WiFiUdp.h>

	int ConnectionStatus = WL_IDLE_STATUS;
	char ssid[] = WifiSSID;        // your network SSID (name)
	char pass[] = WifiPassword;    // your network password (use for WPA, or use as key for WEP)

	char InBuffer[150];	 //buffer to hold incoming packet
	char OutBuffer[] = { 0,0,0,0,0,0,0,0,0,0 };	 // Array to send data back to AgOpenGPS, has to be 10 items

	WiFiUDP UDPin;
	WiFiUDP UDPout;

	// UDP
	unsigned int localPort = 8888;      // local port to listen on
	unsigned int RatePort = 5555;

	//sending back to where and which port
	static byte ipDestination[] = { 192, 168, 2, 255 };
	unsigned int portDestination = 9999; //AOG port that listens

	unsigned long CommTime;
	unsigned long ConnectedCount = 0;
	unsigned long ReconnectCount = 0;
#endif

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("Rate Control  :  Version Date: 04/Dec/2019");
	Serial.println();

	pinMode(SwitchedPowerPin, OUTPUT);
	digitalWrite(SwitchedPowerPin, HIGH);	// turn on switched power

	pinMode(Section1, OUTPUT);
	pinMode(Section2, OUTPUT);
	pinMode(Section3, OUTPUT);
	pinMode(Section4, OUTPUT);
	pinMode(Section5, OUTPUT);
	pinMode(Section6, OUTPUT);
	pinMode(Section7, OUTPUT);

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

#if UseWifi
	// check for the WiFi module:
	if (WiFi.status() == WL_NO_MODULE) 
	{
		Serial.println("Communication with WiFi module failed!");
		// don't continue
		while (true);
	}

	String fv = WiFi.firmwareVersion();
	Serial.println("Wifi firmware version: " + fv);

	UDPin.begin(localPort);
	UDPout.begin(RatePort);
	delay(1000);

	//set up the pgn for returning data for AOG
	OutBuffer[0] = 121;
	OutBuffer[1] = 124;
#endif
}

void loop()
{
#if UseWifi
	CheckWifi();
	CommFromAOGwifi();
#else
	CommFromAOG();
#endif

	if (UseSwitches)
	{
		ReadSectionSwitches();
		ReadRateSwitch();
	}
	else
	{
		// use control byte from AOG
		RelayControl = relayLo;
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
		CalRateError();
		if (RelayControl > 0) pwmSetting = DoPID(rateError, rateSetPoint, LOOP_TIME, MinPWMvalue, MaxPWMvalue, KP, KI, KD, DeadBand);
		motorDrive();

#if UseWifi
		CommToAOGwifi();
#else
		CommToAOG();
#endif
	}
}
