# define InoDescription "AutoSteerTeensy   02-Apr-2022"
// autosteer and rate control
// for use with Teensy 4.1 

#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4
#include "BNO08x_AOG.h"		// https://github.com/Math-51/Autosteer_USB_4.3.10_BN08x
#include "zNMEAParser.h"	
#include <ADC.h>
#include <ADC_util.h>

struct PCBconfig	// 23 bytes
{
	uint8_t Receiver = 1;		// 0 none, 1 SimpleRTK2B, 2 Sparkfun F9p
	uint8_t NMEAserialPort = 8;	// from receiver
	uint8_t	RTCMserialPort = 3;	// to receiver
	uint16_t RTCMport = 5432;	// local port to listen on for RTCM data
	uint8_t IMU = 1;			// 0 none, 1 Sparkfun BNO, 2 CMPS14, 3 Adafruit BNO
	uint8_t IMUdelay = 90;		// how many ms after last sentence should imu sample, 90 for SparkFun, 4 for CMPS14   
	uint8_t IMU_Interval = 40;	// for Sparkfun 
	uint16_t ZeroOffset = 6100;
	uint8_t AdsWASpin = 0;		// ADS1115 pin for WAS
	uint8_t MinSpeed = 1;
	uint8_t MaxSpeed = 15;
	uint16_t PulseCal = 255;	// Hz/KMH X 10
	uint8_t	PowerRelay = 255;	// # of relay always on, needed for some raven valves. Use 255 for none.
	uint8_t RS485PortNumber = 7;	// serial port #
	uint8_t ModuleID = 0;
	uint8_t GyroOn = 0;
	uint8_t	GGAlast = 1;
	uint8_t UseRate = 0;
	uint8_t	UseAds = 0;			// 0 use ADS1115 for WAS, 1 use Teensy analog pin for WAS
	uint8_t RelayOnSignal = 0;	// value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t SwapRollPitch = 1;	// 0 use roll value for roll, 1 use pitch value for roll
	uint8_t InvertRoll = 0;
};

PCBconfig PCB;

struct PCBpinConfig	// 13 bytes
{
	uint8_t SteerDir = 22;			// motor 1 direction
	uint8_t	SteerPWM = 23;			// motor 1 pwm
	uint8_t	FlowDir = 36;			// motor 2 direction
	uint8_t	FlowPWM = 37;			// motor 2 pwm
	uint8_t	SteerSW_Relay = 2;		// pin for steering disconnect relay
	uint8_t	WORKSW = 32;
	uint8_t	STEERSW = 39;
	uint8_t	Encoder = 38;
	uint8_t CurrentSensor = 10;
	uint8_t	WAS = 25;				// wheel angle sensor
	uint8_t	PressureSensor = 26;
	uint8_t	SpeedPulse = 11;
	uint8_t	RS485SendEnable = 27;
};

PCBpinConfig PINS;

const uint16_t  LOOP_TIME = 25;	// 40 hz, main loop
const uint16_t  SteerCommSendInterval = 200;	// ms interval to send data back to AGIO
const int16_t AdsI2Caddress = 0x48;

#define LOW_HIGH_DEGREES 5.0	//How many degrees before decreasing Max PWM
#define MaxFlowSensorCount 2
#define CMPS14_ADDRESS 0x60 

const uint32_t RateLoopTime = 50;		//in msec = 20hz
WDT_T4<WDT1> wdt;

// ethernet
#define IPpart3 1	// ex: 192.168.IPpart3.255
#define IPMac 73	// unique number for Arduino IP address and Mac part 6

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
char GPSbuffer[512];	// buffer for ntrip data

float IMU_Heading = 0;
float IMU_Roll = 0;
float IMU_Pitch = 0;
float IMU_YawRate = 0;

//loop time variables in microseconds
uint32_t  lastTime = LOOP_TIME;

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
uint32_t  SWtime = 0;
const uint16_t  SWdebounce = 50;

int8_t switchByte = 0;
int8_t workSwitch = 0;

//pwm variables
int16_t pwmDrive = 0;
int16_t MaxPWMvalue = 255;

//fromAutoSteerData FD 253 - ActualSteerAngle*100 -5,6, Heading-7,8, 
//Roll-9,10, SwitchByte-11, pwmDisplay-12, CRC 13
uint8_t PGN_253[] = { 0x80,0x81, 0x7f, 0xFD, 8, 0, 0, 0, 0, 0,0,0,0, 0xCC };

//fromAutoSteerData FD 250 - sensor values etc
uint8_t PGN_250[] = { 0x80,0x81, 0x7f, 0xFA, 8, 0, 0, 0, 0, 0,0,0,0, 0xCC };
float SensorReading;

//Variables for settings  
struct Storage {
	uint8_t Kp = 40;  //proportional gain
	uint8_t lowPWM = 10;  //band of no action
	int16_t wasOffset = 0;
	uint8_t minPWM = 9;
	uint8_t highPWM = 60;//max PWM value
	float steerSensorCounts = 30;
	float AckermanFix = 1;     //sent as percent
};  Storage steerSettings;  //11 bytes

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
#define EEP_Ident 4450	// if not in eeprom, overwrite
#define PCB_Ident 2388

uint32_t CommTime;
uint32_t LastSend;

bool IMUstarted = false;
bool ADSfound = false;	

ADC* adc = new ADC(); // adc object;

// Rate control
byte RateSend[11];
bool FlowEnabled[MaxFlowSensorCount];
float rateError[MaxFlowSensorCount];
float UPM[MaxFlowSensorCount];
int RatePWM[MaxFlowSensorCount];
uint32_t RateCommTime[MaxFlowSensorCount];

byte PIDkp[] = { 20 };
byte PIDminPWM[] = { 50 };
byte PIDLowMax[] = { 100 };
byte PIDHighMax[] = { 255 };
byte PIDdeadband[] = { 3 };
byte PIDbrakePoint[] = { 20 };
byte AdjustTime[MaxFlowSensorCount];

byte InCommand;
byte ControlType[MaxFlowSensorCount];	// 0 standard, 1 fast close, 2 motor
uint32_t TotalPulses[MaxFlowSensorCount];
byte ManualPWMsetting;
float RateSetting[MaxFlowSensorCount];
float MeterCal[MaxFlowSensorCount];

byte RelayLo;
byte RelayHi;
uint32_t RateLoopLast = RateLoopTime;
bool AutoOn = true;
unsigned int PGN;
bool PGN32614Found;
bool PGN32616Found;
bool PGN32619Found;
bool PGN32620Found;

float NewRateFactor[MaxFlowSensorCount];
uint32_t ManualLast[MaxFlowSensorCount];
float ManualFactor;
uint32_t WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[5];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };
bool UseMultiPulses[MaxFlowSensorCount];

// Rate port
EthernetUDP UDPrate;
uint16_t ListeningPortRate = 28888;
uint16_t DestinationPortRate = 29999;
uint8_t RateData[UDP_TX_PACKET_MAX_SIZE];  // Buffer For Receiving UDP Data

// Config port
EthernetUDP UDPconfig;
uint16_t ListeningPortConfig = 28800;
uint16_t DestinationPortConfig = 29900;
uint8_t ConfigData[UDP_TX_PACKET_MAX_SIZE];

uint8_t ErrorCount;
NMEAParser<2> parser;
BNO080 myIMU;

HardwareSerial* SerialRTCM;
HardwareSerial* SerialNMEA;
HardwareSerial* SerialRS485;

uint32_t LoopLast;
uint32_t ShowLoopTime;

uint32_t SketchStartTime = millis();
void PrintRunTime(uint32_t StartTime = SketchStartTime)
{
	uint32_t time = millis() - StartTime;
	uint32_t minutes = time / 60000;
	uint32_t secs = (time - minutes * 60000) / 1000;
	uint32_t ms = time - minutes * 60000 - secs * 1000;
	Serial.print(minutes);
	Serial.print(":");
	Serial.print(secs);
	Serial.print(".");
	Serial.print(ms);
	Serial.print(">  \t");
}

void setup()
{
	// watchdog timer
	WDT_timings_t config;
	config.timeout = 120;	// seconds
	wdt.begin(config);

	pinMode(LED_BUILTIN, OUTPUT);

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

	// pcb data
	EEPROM.get(100, EEread);

	if (EEread != PCB_Ident)
	{
		EEPROM.put(100, PCB_Ident);
		EEPROM.put(110, PCB);
		EEPROM.put(150, PINS);
	}
	else
	{
		EEPROM.get(110, PCB);
		EEPROM.get(150, PINS);
	}

	// gps receiver
	// SerialNMEA
	switch (PCB.NMEAserialPort)
	{
	case 1:
		SerialNMEA = &Serial1;
		break;
	case 2:
		SerialNMEA = &Serial2;
		break;
	case 3:
		SerialNMEA = &Serial3;
		break;
	case 4:
		SerialNMEA = &Serial4;
		break;
	case 5:
		SerialNMEA = &Serial5;
		break;
	case 6:
		SerialNMEA = &Serial6;
		break;
	case 7:
		SerialNMEA = &Serial7;
		break;
	default:
		SerialNMEA = &Serial8;
		break;
	}

	// SerialRTCM
	switch (PCB.RTCMserialPort)
	{
	case 1:
		SerialRTCM = &Serial1;
		break;
	case 2:
		SerialRTCM = &Serial2;
		break;
	case 3:
		SerialRTCM = &Serial3;
		break;
	case 4:
		SerialRTCM = &Serial4;
		break;
	case 5:
		SerialRTCM = &Serial5;
		break;
	case 6:
		SerialRTCM = &Serial6;
		break;
	case 7:
		SerialRTCM = &Serial7;
		break;
	default:
		SerialRTCM = &Serial8;
		break;
	}

	if (PCB.Receiver != 0)
	{
		SerialRTCM->begin(115200);	// RTCM
		SerialNMEA->begin(115200);	// NMEA

		parser.setErrorHandler(errorHandler);
		parser.addHandler("G-GGA", GGA_Handler);
		parser.addHandler("G-VTG", VTG_Handler);

		static char NMEAreceiveBuffer[512];
		static char NMEAsendBuffer[512];
		static char RTCMreceiveBuffer[512];
		static char RTCMsendBuffer[512];

		SerialNMEA->addMemoryForRead(NMEAreceiveBuffer, 512);
		SerialNMEA->addMemoryForWrite(NMEAsendBuffer, 512);
		SerialRTCM->addMemoryForRead(RTCMreceiveBuffer, 512);
		SerialRTCM->addMemoryForWrite(RTCMsendBuffer, 512);
	}

	pinMode(PINS.Encoder, INPUT_PULLUP);
	pinMode(PINS.WORKSW, INPUT_PULLUP);
	pinMode(PINS.STEERSW, INPUT_PULLUP);
	pinMode(PINS.SteerDir, OUTPUT);
	pinMode(PINS.FlowDir, OUTPUT);
	pinMode(PINS.SteerSW_Relay, OUTPUT);
	pinMode(PINS.SpeedPulse, OUTPUT);

	// analog pins
	adc->adc0->setAveraging(16); // set number of averages
	adc->adc0->setResolution(12); // set bits of resolution
	adc->adc0->setConversionSpeed(ADC_CONVERSION_SPEED::MED_SPEED); // change the conversion speed
	adc->adc0->setSamplingSpeed(ADC_SAMPLING_SPEED::MED_SPEED); // change the sampling speed

	Wire.begin();			// I2C on pins SCL 19, SDA 18
	Wire.setClock(400000); //Increase I2C data rate to 400kHz

	if (PCB.UseAds)
	{
		// ADS1115
		Serial.println("Starting ADS ...");
		ErrorCount = 0;
		while (!ADSfound)
		{
			Wire.beginTransmission(AdsI2Caddress);
			Wire.write(0b00000000);	//Point to Conversion register
			Wire.endTransmission();
			Wire.requestFrom(AdsI2Caddress, 2);
			ADSfound = Wire.available();
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");
		if (ADSfound)
		{
			Serial.println("ADS connected.");
			Serial.println("");
		}
		else
		{
			Serial.println("ADS not found.");
			Serial.println("");
		}
	}

	// ethernet start
	Serial.println("Starting Ethernet ...");
	Ethernet.begin(LocalMac, 0);
	Ethernet.setLocalIP(LocalIP);
	Serial.print("IP Address: ");
	Serial.println(Ethernet.localIP());
	Serial.println("");

	// main port
	UDPsteering.begin(UDPsteeringPort);

	// GPS port
	UDPgps.begin(PCB.RTCMport);

	// IMU
	uint8_t IMUaddress;
	switch (PCB.IMU)
	{
	case 1:
	case 3:
		// sparkfun BNO, Adafruit BNO
		IMUaddress = 0x4B;
		if (PCB.IMU == 3) IMUaddress = 0x4A;

		ErrorCount = 0;
		Serial.println("Starting IMU ...");
		while (!IMUstarted)
		{
			IMUstarted = myIMU.begin(IMUaddress, Wire);
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");

		if (IMUstarted)
		{
			if (PCB.GyroOn) myIMU.enableGyro(PCB.IMU_Interval - 1);

			myIMU.enableGameRotationVector(PCB.IMU_Interval); //Send data update every REPORT_INTERVAL in ms for BNO085

			//Retrieve the getFeatureResponse report to check if Rotation vector report is corectly enable
			if (myIMU.getFeatureResponseAvailable() == true)
			{
				if (myIMU.checkReportEnable(SENSOR_REPORTID_GAME_ROTATION_VECTOR, PCB.IMU_Interval) == false) myIMU.printGetFeatureResponse();
				Serial.println("BNO08x init succeeded.");
			}
			else
			{
				Serial.println(F("BNO08x init fails!!"));
			}
		}
		else
		{
			Serial.println("IMU failed to start.");
		}
		break;

	case 2:
		// CMPS14
		ErrorCount = 0;
		Serial.println("Starting IMU ...");
		while (!IMUstarted)
		{
			Wire.beginTransmission(CMPS14_ADDRESS);
			IMUstarted = !Wire.endTransmission();
			Serial.print(".");
			delay(500);
			if (ErrorCount++ > 10) break;
		}
		Serial.println("");
		if (IMUstarted)
		{
			Serial.println("IMU started.");
		}
		else
		{
			Serial.println("IMU failed to start.");
		}
		break;

	default:
		PCB.IMU = 0;
	}

	// RS485
	switch (PCB.RS485PortNumber)
	{
	case 1:
		SerialRS485 = &Serial1;
		break;
	case 2:
		SerialRS485 = &Serial2;
		break;
	case 3:
		SerialRS485 = &Serial3;
		break;
	case 4:
		SerialRS485 = &Serial4;
		break;
	case 5:
		SerialRS485 = &Serial5;
		break;
	case 6:
		SerialRS485 = &Serial6;
		break;
	case 7:
		SerialRS485 = &Serial7;
		break;
	default:
		SerialRS485 = &Serial8;
		break;
	}

	SerialRS485->begin(9600);
	pinMode(PINS.RS485SendEnable, OUTPUT);
	digitalWrite(PINS.RS485SendEnable, HIGH);

	if (PCB.UseRate)
	{
		Serial.println("");
		Serial.print("Rate Module ID: ");
		Serial.println(PCB.ModuleID);
		Serial.println();

		pinMode(PINS.Encoder, INPUT_PULLUP);
		attachInterrupt(digitalPinToInterrupt(PINS.Encoder), ISR0, FALLING);
		UDPrate.begin(ListeningPortRate);

		RateSend[0] = 101;
		RateSend[1] = 127;
		RateSend[2] = BuildModSenID(PCB.ModuleID, 0);
	}

	UDPconfig.begin(ListeningPortConfig);

	noTone(PINS.SpeedPulse);
	SteerSwitch = HIGH;

	Serial.println("");
	Serial.println("Finished setup.");
}

void loop()
{
	Blink();
	ReceiveConfig();
	ReceiveSteerUDP();

	if (millis() - lastTime >= LOOP_TIME)
	{
		lastTime = millis();
		ReadSwitches();
		DoSteering();
	}

	if (millis() - LastSend > SteerCommSendInterval)
	{
		LastSend = millis();
		if (PCB.Receiver == 0) ReadIMU();
		SendSteerUDP();
}

	SendSpeedPulse();
	if (PCB.Receiver != 0) DoPanda();
	if (PCB.UseRate) DoRate();
	wdt.feed();

	 // loop interval
	//if (millis() - ShowLoopTime > 1000)
	//{
	//	if (micros() - LoopLast > 5)
	//	{
	//		PrintRunTime();
	//		Serial.print("Loop interval (ms) ");
	//		Serial.println((float)(micros() - LoopLast) / 1000.0, 3);
	//		ShowLoopTime = millis();
	//	}
	//}
	//LoopLast = micros();
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
 

uint32_t SpeedPulseTime;
void SendSpeedPulse()
{
	// https://discourse.agopengps.com/t/get-feed-rate-from-ago-and-transform-it-into-weedkiller-sprayer-computer-compatible-pulses/2958/39

	if (millis() - SpeedPulseTime > 400) //This section runs every 400 millis.  It gets speed and changes the frequency of the tone generator.
	{
		SpeedPulseTime = millis();
		if (Speed_KMH < 1.22) // If the speed is lower than (0.76 MPH, 1.22 KMH) or (31.1 Hz) it forces the output to 0. Tone will not work under 31 Hz 
		{
			noTone(PINS.SpeedPulse);
		}
		else {
			tone(PINS.SpeedPulse, (Speed_KMH * PCB.PulseCal / 10));
		}
	}
}
