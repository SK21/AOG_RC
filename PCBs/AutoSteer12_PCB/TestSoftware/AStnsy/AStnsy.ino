#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4
#include "BNO08x_AOG.h"		// https://github.com/Math-51/Autosteer_USB_4.3.10_BN08x
#include "zADS1115.h"		
#include "zNMEAParser.h"	

# define InoDescription "AStnsy  :  16-Jan-2022"
// for use with Teensy 4.1 and AS12 PCB

// user settings ****************************
#define ReceiverType 1		// 0 None, 1 SimpleRTK2B, 2 SparkFun F9P
#define Hz_Per_KMH 25.5		// 25.5 Hz/KMH = 41.0 Hz/MPH, depends on sensor  

#define IMUtype	1			// 0 None, 1 SparkFun BNO08x, 2 CMPS14
#define EnableGyro 0
#define SwapPitchRoll 1		// 0 use roll value for roll, 1 use pitch value for roll
#define InvertRoll  0

#define MinSpeed  1
#define MaxSpeed  15

#define IPpart3 1	// ex: 192.168.IPpart3.255
#define IPMac 73	// unique number for Arduino IP address and Mac part 6

//How many degrees before decreasing Max PWM
#define LOW_HIGH_DEGREES 5.0

// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)
int16_t SteeringZeroOffset = 6500;

const uint16_t  LOOP_TIME = 25;	// 40 hz, main loop
const uint16_t  SendTime = 250;	// 4 hz, send data back to AGIO

#define EEP_Ident 4410	// if not in eeprom, overwrite


#define UseRateControl 1				
#define SensorCount 1
#define ModuleID 0						// unique ID 0-15
#define RelayOn LOW						// sections relays on signal 
byte FlowOn[] = { LOW };				// on value for flowmeter or motor direction
const unsigned long RateLoopTime = 50;	//in msec = 20hz
byte SwitchedPowerRelay = 255;			// # of relay always on, needed for some raven valves. Use 255 for none.

// ******************************************

ADS1115_lite adc(ADS1115_DEFAULT_ADDRESS);     

WDT_T4<WDT1> wdt;

// motor 1
#define DIR1_PIN 22		
#define PWM1_PIN 23

// motor 2
#define DIR2_PIN 36	
#define PWM2_PIN 37	

#define SteerSW_Relay 2	
#define WORKSW_PIN 30	
#define STEERSW_PIN 31	
#define Encoder_Pin 26		
#define CurrentSensorPin 10

#define SpeedPulsePin 11
#define RS485SendEnable 27
#define PressureSensorPin 25	// 3.3v

// Receiver serial connection
#if(ReceiverType==1)
// simpleRTK2B
#define SerialRTCM Serial3	// RTCM
#define SerialNMEA Serial8	// NMEA
#endif

#if(ReceiverType==2)
// SparkFun F9P
#define SerialRTCM Serial4	// RTCM
#define SerialNMEA Serial5	// NMEA
#endif

#if(ReceiverType !=0)
NMEAParser<2> parser;
#endif

// ethernet
// ethernet interface ip address
IPAddress LocalIP(192, 168, IPpart3, IPMac);

// ethernet mac address - must be unique on your network
static uint8_t LocalMac[] = { 0x70,0x30,IPMac,0x2D,0x31,0x69 };

// ethernet destination - AGIO
IPAddress AGIOip(192, 168, IPpart3, 255);
uint16_t  AGIOport = 9999; // port that AGIO listens on

// UDP Steering traffic
EthernetUDP UDPsteering;
uint16_t  UDPsteeringPort = 8888;		// local port to listen on
uint8_t data[UDP_TX_PACKET_MAX_SIZE];  // Buffer For Receiving UDP Data

// UDP GPS traffic
EthernetUDP UDPgps;
uint16_t UDPrtcmPort = 5432;	// local port to listen on for RTCM data


//IMU
#if(IMUtype == 1)
	BNO080 myIMU;
#define BNO08x_ADRESS 0x4B //Use 0x4A for Adafruit BNO085 board, use 0x4B for Sparkfun BNO080 board
#define REPORT_INTERVAL 40 //Report interval in ms
#endif

#if(IMUtype == 2)
#define CMPS14_ADDRESS 0x60 
#endif

float IMU_Heading = 0;
float IMU_Roll = 0;
float IMU_Pitch = 0;
float IMU_YawRate = 0;


//loop time variables in microseconds
uint16_t  lastTime = LOOP_TIME;

int8_t guidanceStatus;

//steering variables
float steerAngleActual = 0;
float steerAngleSetPoint = 0; //the desired angle from AgOpen
int16_t steeringPosition = 0;
float steerAngleError = 0; //setpoint - actual
float Speed_KMH = 0.0;

// steer switch
int8_t SteerSwitch = LOW;	// Low on, High off
int8_t SWreading = HIGH;
int8_t SWprevious = LOW;
uint16_t  SWtime = 0;
uint16_t  SWdebounce = 50;

int8_t switchByte = 0;
int8_t workSwitch = 0;

//pwm variables
int16_t pwmDrive = 0;
int16_t pwmTmp = 0;
int16_t pwmDir = 1;
int8_t MaxPWMvalue = 255;

//fromAutoSteerData FD 253 - ActualSteerAngle*100 -5,6, Heading-7,8, 
//Roll-9,10, SwitchByte-11, pwmDisplay-12, CRC 13
byte AOG[] = { 0x80,0x81, 0x7f, 0xFD, 8, 0, 0, 0, 0, 0,0,0,0, 0xCC };

//Variables for settings  
struct Storage {
	int8_t Kp = 40;  //proportional gain
	int8_t lowPWM = 10;  //band of no action
	int16_t wasOffset = 0;
	int8_t minPWM = 9;
	int8_t highPWM = 60;//max PWM value
	float steerSensorCounts = 30;
	float AckermanFix = 1;     //sent as percent
};  Storage steerSettings;  //14 bytes

 //Variables for settings - 0 is false  
struct Setup {
	uint8_t InvertWAS = 0;
	uint8_t IsRelayActiveHigh = 0; //if zero, active low (default)
	uint8_t MotorDriveDirection = 0;
	uint8_t SingleInputWAS = 1;
	uint8_t CytronDriver = 1;
	uint8_t SteerSwitch = 0;  //1 if switch selected
	uint8_t SteerButton = 0;  //1 if button selected
	uint8_t ShaftEncoder = 0;
	uint8_t PressureSensor = 0;
	uint8_t CurrentSensor = 0;
	uint8_t PulseCountMax = 5;
	uint8_t IsDanfoss = 0;
};  Setup steerConfig;          //9 bytes

//EEPROM
int16_t EEread = 0;

unsigned long CommTime;
unsigned long LastSend;

bool EthernetConnected = false;
bool IMUstarted = false;
bool ADSfound = false;	

//reset function
void(*resetFunc) (void) = 0;

#if(UseRateControl)
byte RateSend[11];
bool FlowEnabled[SensorCount];
float rateError[SensorCount];
float UPM[SensorCount];
int RatePWM[SensorCount];
unsigned long RateCommTime[SensorCount];

byte PIDkp[] = { 20 };
byte PIDminPWM[] = { 50 };
byte PIDLowMax[] = { 100 };
byte PIDHighMax[] = { 255 };
byte PIDdeadband[] = { 3 };
byte PIDbrakePoint[] = { 20 };
byte AdjustTime[SensorCount];

byte InCommand;
byte ControlType[SensorCount];	// 0 standard, 1 fast close, 2 motor
unsigned long TotalPulses[SensorCount];
byte ManualPWMsetting;
float RateSetting[SensorCount];
float MeterCal[SensorCount];

byte RelayLo;
byte RelayHi;
unsigned long RateLoopLast = RateLoopTime;
bool AutoOn = true;
unsigned int PGN;
bool PGN32614Found;
bool PGN32616Found;
bool PGN32619Found;
bool PGN32620Found;

float NewRateFactor[SensorCount];
unsigned long ManualLast[SensorCount];
float ManualFactor;
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[5];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };
bool UseMultiPulses[SensorCount];

// Rate port
EthernetUDP UDPrate;
uint16_t ListeningPortRate = 28888;
uint16_t DestinationPortRate = 29999;
uint8_t RateData[UDP_TX_PACKET_MAX_SIZE];  // Buffer For Receiving UDP Data

byte FlowDir[] = { 36 };
byte FlowPWM[] = { 37 };
byte FlowPin[] = { 26 };

#endif

void setup()
{
	// watchdog timer
	WDT_timings_t config;
	config.timeout = 20;
	wdt.begin(config);

	static char ReceiveBuffer[100];
	SerialNMEA.addMemoryForRead(ReceiveBuffer, 100);

	pinMode(LED_BUILTIN, OUTPUT);
	noTone(SpeedPulsePin);

	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	EEPROM.get(0, EEread);              // read identifier

	if (EEread != EEP_Ident)   // check on first start and write EEPROM
	{
		EEPROM.put(0, EEP_Ident);
		EEPROM.put(10, steerSettings);
		EEPROM.put(40, steerConfig);
	}
	else
	{
		EEPROM.get(10, steerSettings);     // read the Settings
		EEPROM.get(40, steerConfig);
	}

	pinMode(Encoder_Pin, INPUT_PULLUP);
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
	pinMode(DIR1_PIN, OUTPUT);
	pinMode(DIR2_PIN, OUTPUT);
	pinMode(SteerSW_Relay, OUTPUT);
	pinMode(SpeedPulsePin, OUTPUT);

	SteerSwitch = LOW;

	// ethernet start
	Serial.println("Starting Ethernet ...");
	EthernetConnected = Ethernet.begin(LocalMac);
	Ethernet.setLocalIP(LocalIP);
	if (EthernetConnected)
	{
		Serial.println("Ethernet connected.");
	}
	else
	{
		Serial.println("Ethernet not connected.");
	}
	Serial.println("");

	// main port
	UDPsteering.begin(UDPsteeringPort);

	// GPS port
	UDPgps.begin(UDPrtcmPort);

	//set up communication
	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000); //Increase I2C data rate to 400kHz

#if (IMUtype == 1)
	//BNO085 init
	Serial.println("Starting IMU ...");
	while (!IMUstarted)
	{
		IMUstarted = myIMU.begin(BNO08x_ADRESS, Wire);	
		Serial.print(".");
		delay(500);
	}
	Serial.println("");

#if(EnableGyro)
	myIMU.enableGyro(REPORT_INTERVAL-1);
#endif
	myIMU.enableGameRotationVector(REPORT_INTERVAL); //Send data update every REPORT_INTERVAL in ms for BNO085

	//Retrieve the getFeatureResponse report to check if Rotation vector report is corectly enable
	if (myIMU.getFeatureResponseAvailable() == true)
	{
		if (myIMU.checkReportEnable(SENSOR_REPORTID_GAME_ROTATION_VECTOR, REPORT_INTERVAL) == false) myIMU.printGetFeatureResponse();
		Serial.println("BNO08x init succeeded.");
	}
	else
	{
		Serial.println(F("BNO08x init fails!!"));
	}
#endif

	// ADS1115
	Serial.println("");
	Serial.println("Starting ADS ...");
	while (!ADSfound)
	{
		ADSfound = adc.testConnection();
		Serial.print(".");
		delay(500);
	}
	Serial.println("");
	Serial.println("ADS connected.");

	adc.setSampleRate(ADS1115_REG_CONFIG_DR_128SPS); //128 samples per second
	adc.setGain(ADS1115_REG_CONFIG_PGA_6_144V);

	// RS485
	pinMode(RS485SendEnable, OUTPUT);
	digitalWrite(RS485SendEnable, HIGH);

#if(ReceiverType !=0)
	SerialNMEA.begin(115200);
	SerialRTCM.begin(115200);

	parser.setErrorHandler(errorHandler);
	parser.addHandler("G-GGA", GGA_Handler);
	parser.addHandler("G-VTG", VTG_Handler);
#endif

#if(UseRateControl)
	Serial.print("Module ID: ");
	Serial.println(ModuleID);
	Serial.println();
	
	attachInterrupt(digitalPinToInterrupt(Encoder_Pin), ISR0, FALLING);
	UDPrate.begin(ListeningPortRate);
#endif

	Serial.println("");
	Serial.println("Finished setup.");
}

void loop()
{
	Blink();
	ReceiveSteerUDP();

	if (millis() - lastTime >= LOOP_TIME)
	{
		lastTime = millis();
		ReadSwitches();
		DoSteering();
	}

	if (millis() - LastSend > SendTime)
	{
		LastSend = millis();

#if (ReceiverType == 0)
			ReadIMU();
#endif
		SendSteerUDP();
	}

	SendSpeedPulse();

#if(ReceiverType !=0)
	DoPanda();
#endif

#if(UseRateControl)
	DoRate();
#endif

	wdt.feed();
}

bool State = 0;
uint32_t BlinkTime;
void Blink()
{
	if (millis() - BlinkTime > 1000)
	{
		State = !State;
		if (State) digitalWrite(LED_BUILTIN, HIGH);
		else digitalWrite(LED_BUILTIN, LOW);
		BlinkTime = millis();
	}
}
 

// https://discourse.agopengps.com/t/get-feed-rate-from-ago-and-transform-it-into-weedkiller-sprayer-computer-compatible-pulses/2958/39
unsigned long PulseTime;
void SendSpeedPulse()
{
	if (millis() - PulseTime > 400) //This section runs every 400 millis.  It gets speed and changes the frequency of the tone generator.
	{
		PulseTime = millis();
		if (Speed_KMH < 1.22) // If the speed is lower than (0.76 MPH, 1.22 KMH) or (31.1 Hz) it forces the output to 0. Tone will not work under 31 Hz 
		{
			noTone(SpeedPulsePin);
		}
		else {
			tone(SpeedPulsePin, (Speed_KMH * Hz_Per_KMH));
		}
	}
}
