// LED on pin 13 may need to be disconnected / burnt with soldering iron

// user settings ****************************

bool UseSwitches = true;	// manual switches
int SecID[7] = { 1, 2, 3, 4, 0, 0, 0 }; // id of switch controlling section, 1,2,3,4 or 0 for none

float MeterCal = 188;	// pulses per Unit 
byte ValveType = 1;	// 0 standard, 1 Fast Close
bool SimulateFlow = true;	
byte OnDirection = LOW;	// flowmeter pin
byte OffDirection = HIGH;	// flowmeter pin
byte MinPWMvalue = 145;
byte MaxPWMvalue = 255;

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
float XeRate = 0.0; //the filtered flowrate
const float varRate = 100; // variance, smaller, more filtering
const float varProcess = 10;

//bit 0 is section 0
byte RelayFromAOG = 0; // bytes from AOG
byte RelayControl = 0;
byte RelayToAOG = 0;
int RelayTemp = 0;
bool RelaysOn = false;

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

bool isDataFound;
int tempHeader;
int header;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("Rate Control Nano :  12/Feb/2020");
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
}

void loop()
{
	CommFromAOG();

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
		rateError = CalRateError();
		if (RelayControl > 0) pwmSetting = DoPID(rateError, rateSetPoint, LOOP_TIME, MinPWMvalue, MaxPWMvalue, KP, KI, KD, DeadBand);
		motorDrive();

		CommToAOG();
	}
}
