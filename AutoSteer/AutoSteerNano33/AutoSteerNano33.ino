#include <SPI.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>
#include <Wire.h>
#include <Adafruit_ADS1015.h>
#include <Arduino_LSM6DS3.h>
#include <MadgwickAHRS.h>

// user settings ****************************

#define UseWifi 0	// 0 Serial/USB, 1 wifi
#define WifiSSID "tractor"
#define WifiPassword "450450450"

#define UseSteerSwitch 0		// 1 - steer switch, 0 - steer momentary button
#define UseEncoder 0			// Steering Wheel ENCODER Installed
#define PulseCountMax  3

// ****** select only one source of roll
#define UseDog2 0		// 0 false, 1 true (inclinometer)
#define UseIMUroll 1	//0 false, 1 true
// ******

// ****** select only one IMU
#define UseSerialIMU 1		// 0 false, 1 true 
#define UseOnBoardIMU 0		// 0 false, 1 true 
// ******

// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)

float SteerCPD = 237;	// AOG value sent * 2
int SteeringZeroOffset = 15000; 
int AOGzeroAdjustment = 0;	// AOG value sent * 20 to give range of +-10 degrees
int SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;

byte MinPWMvalue = 10;
int SteerDeadband = 3;	// % error allowed
#define UsePitch 0	// 1 - use pitch, 0 - use roll

#define InvertWAS  0
#define InvertRoll  0
#define InvertMotorDrive 0

#define AckermanFix  100     //sent as percent
#define MinSpeed  1
#define MaxSpeed  20

#define AdsPitch 1	// ADS1115 pitch pin
#define AdsRoll 2	// ADS1115 roll pin
#define AdsWAS 3	// ADS1115 wheel angle sensor pin

// end user settings ******************************************

// PCB 6
#define EncoderPin  2
#define WORKSW_PIN  4
#define STEERSW_PIN  7
#define DIR_PIN  8
#define PWM_PIN  9
#define SteerSW_Relay 10	// pin 10 not connected, could use pin 2 if not using encoder

// PCB 7
//#define EncoderPin 3
//#define WORKSW_PIN 4
//#define STEERSW_PIN A0
//#define DIR_PIN 7
//#define PWM_PIN 9
//#define SteerSW_Relay 10


// serial RX	5
// serial TX	6
// SDA			A4
// SCL			A5

Adafruit_ADS1115 ads(0x48);

#if (UseSerialIMU)
// hardware serial for IMU
// https://stackoverflow.com/questions/57175348/softwareserial-for-arduino-nano-33-iot
// IMU serial data on pins RX 5, TX 6
// format:
// byte 0, header high byte, 126 or 0x7E
// byte 1, header low byte, 244 or 0xF4, header = 32500 or 0x7EF4
#include "wiring_private.h"
Uart IMUserial(&sercom0, 5, 6, SERCOM_RX_PAD_1, UART_TX_PAD_0);

// Attach the interrupt handler to the SERCOM
void SERCOM0_Handler()
{
	IMUserial.IrqHandler();
}
#endif

#if (UseOnBoardIMU)
// setup Nano33 on-board IMU
// https://itp.nyu.edu/physcomp/lessons/accelerometers-gyros-and-imus-the-basics/
Madgwick MKfilter;
const float SensorRate = 104.00;
#endif

//loop time variables in microseconds
const unsigned int LOOP_TIME = 100; // 10 hz
unsigned int lastTime = LOOP_TIME;
unsigned int currentTime = LOOP_TIME;
byte watchdogTimer = 12;
byte serialResetTimer = 0; //if serial buffer is getting full, empty it

//Kalman variables
float XeRoll = 0;
float RawRoll = 9999;
float FilteredRoll = 9999;
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

int header = 0, tempHeader = 0, temp;

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
int pwmTmp = 0;

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
float IMUroll = 9999;	//*******  if no roll is installed, send 9999
float IMUpitch = 9999;
boolean PGN32750Found = false;
int IMUheader;
int IMUtempHeader;
bool OnBoardIMUenabled = false;

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

#if (UseWifi)
// wifi
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
static byte DestinationIP[] = { 192, 168, 1, 255 };
unsigned int DestinationPort = 9999; //AOG port that listens

unsigned long CheckTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;
#endif

void setup()
{
	//set up communication
	Wire.begin();
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("AutoSteer Nano33  :  02/May/2020");
	Serial.println();

	//keep pulled high and drag low to activate, noise free safe
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
	pinMode(DIR_PIN, OUTPUT);
	pinMode(EncoderPin, INPUT_PULLUP);
	pinMode(SteerSW_Relay, OUTPUT);
	SteerSwitch = LOW;

	ads.begin();

	//Setup Interrupt -Steering Wheel encoder 
	attachInterrupt(digitalPinToInterrupt(EncoderPin), EncoderISR, FALLING);

#if(UseOnBoardIMU)
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

#if(UseSerialIMU)
	// Nano 33 hardward serial, reassign pins 5 and 6 to SERCOM alt
	pinPeripheral(5, PIO_SERCOM_ALT);
	pinPeripheral(6, PIO_SERCOM_ALT);

	// Start the new hardware serial
	IMUserial.begin(38400);
#endif

#if (UseWifi)
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
}

void loop()
{
#if (UseWifi)
	CheckWifi();
	ReceiveUDPWifi();
#else
	CommFromAOG();
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

#if (UseWifi)
		SendUDPWifi();
#else
		CommToAOG();
#endif
	}
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

