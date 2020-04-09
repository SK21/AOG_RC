#include <SoftwareSerial.h>
#include <Wire.h>
#include <Adafruit_ADS1015.h>
#include <EtherCard.h>
#include <EEPROM.h> 


// user settings ****************************

// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)

float SteerCPD = 237;		// AOG value sent * 2
int SteeringZeroOffset = 15000; 
int AOGzeroAdjustment = 0;	// AOG value sent * 20 to give range of +-10 degrees
int SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;

byte MinPWMvalue = 10;

#define CommType 0		// 0 Serial/USB, 1 UDP wired

#define AdsWAS 3	// ADS1115 wheel angle sensor pin
#define AdsRoll 2	// ADS1115 roll pin
#define AdsPitch 1	// ADS1115 pitch pin

int SteerDeadband = 3;	// % error allowed

// ******************************************

// PCB 3.1
#define EncoderPin  2
#define WORKSW_PIN  4
#define STEERSW_PIN  7
#define DIR_PIN  8
#define PWM_PIN  9
#define SSRX 5
#define SSTX 6
// SDA			A4
// SCL			A5

// PCB 7
//#define EncoderPin 3
//#define WORKSW_PIN 4
//#define STEERSW_PIN A0
//#define DIR_PIN A1
//#define PWM_PIN 9
//#define SSRX 5
//#define SSTX 6
// SDA			A4
// SCL			A5


SoftwareSerial IMUserial(SSRX, SSTX);

Adafruit_ADS1115 ads(0x48);

//loop time variables in microseconds
const unsigned int LOOP_TIME = 100; // 10 hz
unsigned int lastTime = LOOP_TIME;
unsigned int currentTime = LOOP_TIME;
byte watchdogTimer = 12;
byte serialResetTimer = 0; //if serial buffer is getting full, empty it

//Kalman variables
float XeRoll = 0;
float RawRoll = 0;
float FilteredRoll = 0;
float Pc = 0.0;
float G = 0.0;
float P = 1.0;
float Xp = 0.0;
float Zp = 0.0;
const float varRoll = 0.1; // variance, smaller, more filtering
const float varProcess = 0.0001; //smaller is more filtering

 //program flow
bool PGN32762Found = false; // machine data
bool PGN32763Found = false; // AogSettings
bool PGN32764Found = false;	// autosteer settings
bool PGN32766Found = false;	// autosteer data

unsigned int header;
unsigned int tempHeader;
unsigned int temp;

byte relay = 0, uTurn = 0;
float distanceFromLine = 0;

//steering variables
float steerAngleActual = 0;
float steerAngleSetPoint = 0; //the desired angle from AgOpen
int steeringPosition = 0;
float steerAngleError = 0; //setpoint - actual

//inclinometer variables
int roll = 0;

//pwm variables
int pwmDrive = 0;

float CurrentSpeed = 0.0;

//PID variables
float Ko = 0.0f;  //overall gain
float Kp = 0.0f;  //proportional gain
float Ki = 0.0f;//integral gain
float Kd = 0.0f;  //derivative gain

//integral values - **** change as required *****
float maxIntegralValue = 20; //max PWM value for integral PID component

//IMU
float IMUheading = 9999;	// *******  if there is no gyro installed send 9999
float IMUroll = 9999;		//*******  if no roll is installed, send 9999
float IMUpitch = 9999;
boolean PGN32750 = false;
int IMUheader;
int IMUtempHeader;

// steering wheel encoder
volatile int pulseCount = 0; // Steering Wheel Encoder

// steer switch
byte SteerSwitch = HIGH;	// Low on, High off
byte SWreading = HIGH;
byte SWPrevious = LOW;
unsigned int SWtime = 0;
unsigned int SWdebounce = 50;

byte switchByte = 0;
byte workSwitch = 0;

#if (CommType == 1)
// wired UDP comm

// ethernet mac address - must be unique on your network
static byte LocalMac[] = { 0x70,0x69,0x69,0x2D,0x30,0x31 };

unsigned int ListeningPort = 8888;	// local port to listen on

// ethernet source - Arduino
static byte SourceIP[] = { 192,168,1,77 };
unsigned int SourcePort = 5577;		// local port to send from 

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

//Variables for settings - 0 is false  
struct Setup {
	byte InvertWAS = 0;
	byte InvertRoll = 0;
	byte MotorDriveDirection = 0;
	byte SingleInputWAS = 1;
	byte CytronDriver = 1;
	byte SteerSwitch = 1;
	byte UseMMA_X_Axis = 0;
	byte ShaftEncoder = 0;

	byte BNOInstalled = 0;
	byte InclinometerInstalled = 0;   // set to 0 for none
									  // set to 1 if DOGS2 Inclinometer is installed
	byte MaxSpeed = 20;
	byte MinSpeed = 15;
	byte PulseCountMax = 5;
	byte AckermanFix = 100;     //sent as percent
};  Setup aogSettings;          //11 bytes

//reset function
void(*resetFunc) (void) = 0;

#define EEP_Ident 0xEDFB  
int EEread = 0;

void setup()
{
	//set up communication
	Wire.begin();
	Serial.begin(38400);

	// software serial
	IMUserial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("AutoSteer Nano   :  08/Apr/2020");
	Serial.println();

	EEPROM.get(0, EEread);              // read identifier

	if (EEread != EEP_Ident)   // check on first start and write EEPROM
	{
		EEPROM.put(0, EEP_Ident);
		//EEPROM.put(2, 1660);
		//EEPROM.put(10, steerSettings);
		EEPROM.put(40, aogSettings);
	}
	else
	{
		//EEPROM.get(2, EEread);            // read SteerPosZero
		//EEPROM.get(10, steerSettings);     // read the Settings
		EEPROM.get(40, aogSettings);
	}

	//keep pulled high and drag low to activate, noise free safe
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
	pinMode(DIR_PIN, OUTPUT);
	pinMode(EncoderPin, INPUT_PULLUP);

	ads.begin();

	//Setup Interrupt -Steering Wheel encoder 
	attachInterrupt(digitalPinToInterrupt(EncoderPin), EncoderISR, FALLING);

#if (CommType == 1)
	if (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) == 0)
		Serial.println(F("Failed to access Ethernet controller"));

	//set up connection
	ether.staticSetup(SourceIP, gwip, myDNS, mask);
	ether.printIp("IP:  ", ether.myip);
	ether.printIp("GW:  ", ether.gwip);
	ether.printIp("DNS: ", ether.dnsip);

	//register sub to receive data
	ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif
}

void loop()
{
#if (CommType == 0)
	ReceiveSerial();
#endif

	UpdateHeadingRoll();

	currentTime = millis();
	if (currentTime - lastTime >= LOOP_TIME)
	{
		lastTime = currentTime;

		//clean out serial buffer to prevent buffer overflow
		if (serialResetTimer++ > 20)
		{
			while (Serial.available() > 0) char t = Serial.read();
			serialResetTimer = 0;
		}

		ReadSwitches();
		DoSteering();

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


//ISR Steering Wheel Encoder
void EncoderISR()
{
#if (UseEncoder)      
	if (digitalRead(EncoderPin) == 0) // decide if external triggered  
	{
		pulseCount++;
	}
#endif     
}
