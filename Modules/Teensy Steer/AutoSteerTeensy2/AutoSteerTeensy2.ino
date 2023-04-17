#define InoDescription "AutoSteerTeensy2   16-Apr-2023"
// autosteer for Teensy 4.1

#define InoID 1604	// if not in eeprom, overwrite

#include <Wire.h>
#include <EEPROM.h> 
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
#include <Watchdog_t4.h>	// https://github.com/tonton81/WDT_T4
#include "BNO08x_AOG.h"		// https://github.com/Math-51/Autosteer_USB_4.3.10_BN08x
#include "zNMEAParser.h"	
#include "USBHost_t36.h"

#define MaxReadBuffer 100	// bytes
#define MaxHostBuffer 50
#define LOW_HIGH_DEGREES 5.0	//How many degrees before decreasing Max PWM
#define CMPS14_ADDRESS 0x60 
#define ADS1115_Address 0x48
#define USBBAUD 38400
#define CNT_DEVICES (sizeof(drivers)/sizeof(drivers[0]))

// usb host
uint32_t baud = USBBAUD;
uint32_t format = USBHOST_SERIAL_8N1;
USBHost myusb;
USBHub hub1(myusb);
USBHub hub2(myusb);
USBHIDParser hid1(myusb);
USBHIDParser hid2(myusb);
USBHIDParser hid3(myusb);
USBSerial userial(myusb);  // works only for those Serial devices who transfer <=64 bytes (like T3.x, FTDI...)
USBDriver* drivers[] = { &hub1, &hub2, &hid1, &hid2, &hid3, &userial };
const char* driver_names[CNT_DEVICES] = { "Hub1", "Hub2",  "HID1", "HID2", "HID3", "USERIAL1" };
bool driver_active[CNT_DEVICES] = { false, false, false, false };

struct ModuleConfig	// 34 bytes, AS14 default
{
	uint8_t Receiver = 1;			// 0 none, 1 SimpleRTK2B, 2 Sparkfun F9p
	uint8_t SerialReceiveGPS = 4;		// from receiver
	uint8_t	SerialSendNtrip = 5;		// to receiver
	uint8_t WemosSerialPort = 3;	// serial port connected to Wemos D1 Mini
	uint16_t NtripPort = 2233;		// local port to listen on for NTRIP data
	uint8_t IMU = 1;				// 0 none, 1 Sparkfun BNO, 2 CMPS14, 3 Adafruit BNO
	uint8_t IMUdelay = 90;			// how many ms after last sentence should imu sample, 90 for SparkFun, 4 for CMPS14   
	uint8_t IMU_Interval = 40;		// for Sparkfun 
	uint16_t ZeroOffset = 6500;
	uint8_t MinSpeed = 1;
	uint8_t MaxSpeed = 15;
	uint16_t PulseCal = 255;		// Hz/KMH X 10
	uint8_t	AnalogMethod = 2;		// 0 use ADS1115 for WAS(AIN0), AIN1, current(AIN2), 1 use Teensy analog pin for WAS, 2 use ADS1115 from Wemos D1 Mini
	uint8_t SwapRollPitch = 0;		// 0 use roll value for roll, 1 use pitch value for roll
	uint8_t InvertRoll = 0;
	uint8_t UseTB6612 = 0;			// 0 - don't use TB6612 motor driver, 1 - use TB6612 motor driver for motor 2
	uint8_t GyroOn = 0;
	uint8_t UseLinearActuator = 0;	// to engage or retract steering motor, uses motor 2
	uint8_t	GGAlast = 1;
	uint8_t Dir1 = 26;
	uint8_t PWM1 = 25;
	uint8_t Dir2 = 28;				// motor 2 direction, TB6612 IN1
	uint8_t PWM2 = 29;				// motor 2 pwm, TB6612 PWM	
	uint8_t SteerSw_Relay = 36;		// pin for steering disconnect relay
	uint8_t SteerSw = 39;
	uint8_t WorkSw = 27;
	uint8_t CurrentSensor = 10;
	uint8_t PressureSensor = 26;
	uint8_t Encoder = 38;
	uint8_t SpeedPulse = 37;
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 126;
};

ModuleConfig MDL;

struct PCBanalog
{
	int16_t AIN0;	// WAS
	int16_t AIN1;	// linear actuator position or pressure sensor
	int16_t AIN2;	// current sensor
	int16_t AIN3;
};

PCBanalog AINs;

struct Storage 
{
	uint8_t Kp = 40;  //proportional gain
	uint8_t lowPWM = 10;  //band of no action
	int16_t wasOffset = 0;
	uint8_t minPWM = 9;
	uint8_t highPWM = 60;//max PWM value
	float steerSensorCounts = 30;
	float AckermanFix = 1;     //sent as percent
};  

Storage steerSettings;  //11 bytes

struct Setup
{
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
	uint8_t IsUseY_Axis = 0;     //Set to 0 to use X Axis, 1 to use Y avis
};

Setup steerConfig;          //9 bytes

// Ethernet steering
EthernetUDP UDPsteering;	// UDP Steering traffic, to and from AGIO
uint16_t ListeningPort = 8888;
uint16_t DestinationPort = 9999;	// port that AGIO listens on
IPAddress DestinationIP(MDL.IP0, MDL.IP1, MDL.IP2, 255);

// Ethernet switchbox
EthernetUDP UDPswitches;
uint16_t ListeningPortSwitches = 28888;
uint16_t DestinationPortSwitches = 29999;

EthernetUDP UDPntrip;	// from AGIO to receiver
char NtripBuffer[512];	// buffer for ntrip data

//steering variables
float steerAngleActual = 0;
float steerAngleSetPoint = 0; //the desired angle from AgOpen
int16_t steeringPosition = 0;
float steerAngleError = 0; //setpoint - actual
float Speed_KMH = 0.0;
int8_t guidanceStatus;

float IMU_Heading = 0;
float IMU_Roll = 0;
float IMU_Pitch = 0;
float IMU_YawRate = 0;

// switches
int8_t SteerSwitch = LOW;	// Low on, High off
int8_t SWreading = HIGH;
int8_t SWprevious = LOW;
uint32_t  SWtime = 0;
uint8_t  SWdebounce = 50;
int8_t switchByte = 0;
int8_t workSwitch = 0;
float SensorReading;

//pwm variables
int16_t pwmDrive = 0;
int16_t MaxPWMvalue = 255;

//Heart beat hello AgIO
uint8_t helloFromIMU[] = { 128, 129, 121, 121, 5, 0, 0, 0, 0, 0, 71 };
uint8_t helloFromAutoSteer[] = { 128, 129, 126, 126, 5, 0, 0, 0, 0, 0, 71 };
int16_t helloSteerPosition = 0;

//fromAutoSteerData FD 253 - ActualSteerAngle*100 -5,6, SwitchByte-7, pwmDisplay-8
uint8_t PGN_253[] = { 128, 129, 123, 253, 8, 0, 0, 0, 0, 0,0,0,0, 12 };

//fromAutoSteerData FD 250 - sensor values etc
uint8_t PGN_250[] = { 128, 129, 123, 250, 8, 0, 0, 0, 0, 0,0,0,0, 12 };

WDT_T4<WDT1> wdt;
const uint16_t  LOOP_TIME = 25;	// 40 hz, main loop
uint32_t  LoopLast = LOOP_TIME;

uint8_t ErrorCount;
NMEAParser<2> parser;
BNO080 myIMU;

uint32_t CommTime;
bool ADSfound = false;

HardwareSerial* SerialNtrip;
HardwareSerial* SerialNMEA;
HardwareSerial* SerialWemos;

byte PGNlength;

void setup()
{
	DoSetup();
}

void loop()
{
	if (millis() - LoopLast >= LOOP_TIME)
	{
		LoopLast = millis();
		ReadSwitches();
		DoSteering();
		if (MDL.UseLinearActuator) PositionActuator();
		if (MDL.Receiver == 0) ReadIMU();
	}

	ReadAnalog();
	ReceiveSteerData();
	ReceiveWemos();
	ReceiveConfigData();
	SendSpeedPulse();

	if (MDL.Receiver != 0) DoPanda();

	DoHost();

	Blink();
	wdt.feed();
}

bool State = false;
elapsedMillis BlinkTmr;
byte ResetRead;
elapsedMicros LoopTmr;
uint32_t MaxLoopTime;

void Blink()
{
	if (BlinkTmr > 1000)
	{
		BlinkTmr = 0;
		State = !State;
		digitalWrite(LED_BUILTIN, State);
		Serial.println(".");	// needed to allow PCBsetup to connect

		Serial.print(" Loop Time (Micros): ");
		Serial.print(MaxLoopTime);

		Serial.print(", WAS: ");
		Serial.print(AINs.AIN0);

		Serial.print(", Heading: ");
		Serial.print(IMU_Heading / 10.0);

		Serial.println("");

		if (ResetRead++ > 10)
		{
			MaxLoopTime = 0;
			ResetRead = 0;
		}
	}
	if (LoopTmr > MaxLoopTime) MaxLoopTime = LoopTmr;
	LoopTmr = 0;
}

uint32_t SpeedPulseTime;
void SendSpeedPulse()
{
	// https://discourse.agopengps.com/t/get-feed-rate-from-ago-and-transform-it-into-weedkiller-sprayer-computer-compatible-pulses/2958/39
	// PulseCal: hz/mph - 41.0, hz/kmh - 25.5

	if (millis() - SpeedPulseTime > 400) //This section runs every 400 millis.  It gets speed and changes the frequency of the tone generator.
	{
		SpeedPulseTime = millis();
		if (Speed_KMH < 1.22) // If the speed is lower than (0.76 MPH, 1.22 KMH) or (31.1 Hz) it forces the output to 0. Tone will not work under 31 Hz 
		{
			noTone(MDL.SpeedPulse);
		}
		else
		{
			tone(MDL.SpeedPulse, (Speed_KMH * MDL.PulseCal / 10));
		}
	}
}

bool GoodCRC(byte Data[], byte Length)
{
	byte ck = CRC(Data, Length - 1, 0);
	bool Result = (ck == Data[Length - 1]);
	return Result;
}

byte CRC(byte Chk[], byte Length, byte Start)
{
	byte Result = 0;
	int CK = 0;
	for (int i = Start; i < Length; i++)
	{
		CK += Chk[i];
	}
	Result = (byte)CK;
	return Result;
}

void ControlMotor2(uint8_t Speed, uint8_t Direction)
{
	if (MDL.UseTB6612)
	{
		// on-board TB6612FNG motor driver
		if (Direction)
		{
			// clockwise
			digitalWrite(29, LOW);
			digitalWrite(9, HIGH);
		}
		else
		{
			// counter-clockwise
			digitalWrite(29, HIGH);
			digitalWrite(9, LOW);
		}
		analogWrite(28, Speed);
	}
	else
	{
		// external motor driver
		digitalWrite(MDL.Dir2, Direction);
		analogWrite(MDL.PWM2, Speed);
	}
}




