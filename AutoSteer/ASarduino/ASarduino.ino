
// user settings ****************************
#define PCBversion	7		// 6 - ver6, 7 - ver7 (Nano only)

#define CommType 0			// 0 Serial/USB , 1 UDP wired Nano, 2 UDP wifi Nano33

#define IMUSource	1		// 0 none, 1 serial Nano, 2 serial Nano33, 3 onboard Nano33
#define RollSource	1		// 0 none, 1 IMU, 2 Dog2

#define SteerSwitchType 0	// 0 steer momentary button, 1 on/off switch
#define UseEncoder 0		// 0 none, 1 Steering Wheel ENCODER Installed
#define PulseCountMax  3

#define InvertWAS  1
#define InvertRoll  0
#define InvertMotorDrive 1

#define AckermanFix  100     //sent as percent
#define MinSpeed  3
#define MaxSpeed  20

#define AdsWAS 0		// ADS1115 wheel angle sensor pin
#define AdsPitch 1		// ADS1115 pitch pin
#define AdsRoll 2		// ADS1115 roll pin
#define SwapPitchRoll 0	// 0 use roll value for roll, 1 use pitch value for roll

#define WifiSSID "tractor"
#define WifiPassword ""
#define IPpart3 1	// ex: 192.168.IPpart3.255

//How many degrees before decreasing Max PWM
#define LOW_HIGH_DEGREES 5.0

// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)
//
// Pivot arm ratio, (length of steering arm) / (length of sensor arm)
// 
// Adjust counts per degree for the pivot arm ratio.
// ex: steering arm 9.5", sensor arm 6.5", ratio is 1.46
//	   237 * 1.46 = 346

float SteerCPD = 237;		// AOG value sent * 2
int SteeringZeroOffset = 16400;

// ******************************************

#include <Wire.h>
#include <Adafruit_ADS1015.h>

Adafruit_ADS1115 ads(0x48);

#if (CommType == 1)
// UDP wired Nano

#include <EtherCard.h>

// ethernet mac address - must be unique on your network
static byte LocalMac[] = { 0x70,0x69,0x69,0x2D,0x30,0x31 };

unsigned int ListeningPort = 8888;	// local port to listen on

// ethernet source - Arduino
static byte SourceIP[] = { 192,168,IPpart3,77 };
unsigned int SourcePort = 5577;		// local port to send from 

// ethernet destination - AOG
static byte DestinationIP[] = { 192, 168,IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 9999; //AOG port that listens 

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
// UDP wifi Nano33

#include <SPI.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>

int ConnectionStatus = WL_IDLE_STATUS;
char ssid[] = WifiSSID;        // your network SSID (name)
char pass[] = WifiPassword;    // your network password (use for WPA, or use as key for WEP)

byte InBuffer[150];	 //buffer to hold incoming packet
byte OutBuffer[] = { 0,0,0,0,0,0,0,0,0,0 };	 // Array to send data back to AgOpenGPS

WiFiUDP UDPin;
WiFiUDP UDPout;

// UDP
unsigned int ListeningPort = 8888;      // local port to listen on
unsigned int SourcePort = 5577;

// UDP destination - AOG
static byte DestinationIP[] = { 192, 168,IPpart3, 255 };
unsigned int DestinationPort = 9999; //AOG port that listens

unsigned long CheckTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;

#endif

#if (PCBversion == 6)
#define EncoderPin  2
#define WORKSW_PIN  4
#define STEERSW_PIN  7
#define DIR_PIN  8
#define PWM_PIN  9
#define SSRX 5
#define SSTX 6
#define SteerSW_Relay 10
// SDA			A4
// SCL			A5

#endif

#if (PCBversion == 7)
#define EncoderPin 3
#define WORKSW_PIN 4
#define STEERSW_PIN A0
#define DIR_PIN 7
#define PWM_PIN 9
#define SSRX 5
#define SSTX 6
#define SteerSW_Relay 10
// SDA			A4
// SCL			A5

#endif

#if (IMUSource == 1)
// serial Nano

#include <SoftwareSerial.h>
SoftwareSerial IMUserial(SSRX, SSTX);

#endif

#if (IMUSource == 2)
// serial Nano33

// hardware serial for IMU, Nano33
// https://stackoverflow.com/questions/57175348/softwareserial-for-arduino-nano-33-iot
// format:
// byte 0, header high byte, 126 or 0x7E
// byte 1, header low byte, 244 or 0xF4, header = 32500 or 0x7EF4

#include <Arduino.h>
#include "wiring_private.h"
Uart IMUserial(&sercom0, SSRX, SSTX, SERCOM_RX_PAD_1, UART_TX_PAD_0);

#endif

#if (IMUSource == 3)
// Nano33 on-board IMU

// https://itp.nyu.edu/physcomp/lessons/accelerometers-gyros-and-imus-the-basics/
#include <Arduino_LSM6DS3.h>
#include <MadgwickAHRS.h>
Madgwick MKfilter;
const float SensorRate = 104.00;

#endif

// WAS
int AOGzeroAdjustment = 0;	// AOG value sent * 20 to give range of +-10 degrees
int SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;

//loop time variables in microseconds
const unsigned int LOOP_TIME = 100; // 10 hz
unsigned int lastTime = LOOP_TIME;
unsigned int currentTime = LOOP_TIME;
byte watchdogTimer = 12;
byte serialResetTimer = 0; //if serial buffer is getting full, empty it

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

float CurrentSpeed = 0.0;

//pwm variables
int pwmDrive = 0;
int pwmTmp = 0;
int pwmDir = 1;
byte MinPWMvalue = 10;
byte MaxPWMvalue = 255;
byte LowMaxPWM = 80;
byte HighMaxPWM = 255;

//IMU
float IMUheading = 9999;	// *******  if there is no gyro installed send 9999
float IMUroll = 9999;		//*******  if no roll is installed, send 9999
float IMUpitch = 9999;
boolean PGN32750Found = false;
int IMUheader;
int IMUtempHeader;
bool OnBoardIMUenabled = false;

float RawRoll = 0;
float FilteredRoll = 0;

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

//PID variables
float Kp = 0.0f;		//proportional gain

void setup()
{
	//set up communication
	Wire.begin();
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("ASarduino  :  11/Oct/2020");
	Serial.println();

	//keep pulled high and drag low to activate, noise free safe
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
	pinMode(DIR_PIN, OUTPUT);
	pinMode(EncoderPin, INPUT_PULLUP);
	pinMode(SteerSW_Relay, OUTPUT);
	SteerSwitch = LOW;

	ads.begin();

#if (UseEncoder)
	//Setup Interrupt -Steering Wheel encoder 
	attachInterrupt(digitalPinToInterrupt(EncoderPin), EncoderISR, FALLING);

#endif

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

#if (CommType == 2)
	// check for the WiFi module:
	Serial.println();
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

	//set up the pgn for returning data for autosteer
	OutBuffer[0] = 127;
	OutBuffer[1] = 253;

#endif

#if (IMUSource == 1)
	// software serial
	IMUserial.begin(38400);

#endif

#if (IMUSource == 2)
	// Nano 33 hardward serial, reassign pins 5 and 6 to SERCOM alt
	pinPeripheral(5, PIO_SERCOM_ALT);
	pinPeripheral(6, PIO_SERCOM_ALT);

	// Start the new hardware serial
	IMUserial.begin(38400);

#endif

#if (IMUSource == 3)
	// Nano 33 start on-board IMU
	OnBoardIMUenabled = IMU.begin();
	if (OnBoardIMUenabled)
	{
		MKfilter.begin(SensorRate);	// start MadgwickAHRS
		Serial.println("On-board IMU enabled.");
	}
	else
	{
		Serial.println("On-board IMU disabled.");
	}

#endif
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

#if (CommType == 2)
		SendUDPWifi();
	}
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

#if (IMUSource == 2)
//Attach the interrupt handler to the SERCOM
void SERCOM0_Handler()
{
	IMUserial.IrqHandler();
}
#endif
