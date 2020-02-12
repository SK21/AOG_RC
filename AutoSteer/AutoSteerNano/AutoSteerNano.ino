
// user settings ****************************

// WAS RTY090LVNAA voltage output is 0.5 (left) to 4.5 (right). +-45 degrees
// ADS reading of the WAS ranges from 2700 to 24000 (21300)
// counts per degree for this sensor is 237 (21300/90)
float SteerCPD = 237;	// AOG value sent * 2
int SteeringZeroOffset = 16500; 
int AOGzeroAdjustment = 0;	// AOG value sent * 20 to give range of +-10 degrees
int SteeringPositionZero = SteeringZeroOffset + AOGzeroAdjustment;
byte MinPWMvalue = 10;
	
#define UseDog2 0	// 0 false, 1 true (inclinometer)

#define UseSerialIMU 1	// 0 false, 1 true

#define UseEncoder 0	// Steering Wheel ENCODER Installed
int pulseCountMax = 3;	// Switch off Autosteer after X Pulses from Steering wheel encoder  

#define AdsWAS 3	// ADS1115 wheel angle sensor pin
#define AdsRoll 2	// ADS1115 roll pin

#define EncoderPin  2
#define WORKSW_PIN  4
#define STEERSW_PIN  7
#define DIR_PIN  8
#define PWM_PIN  9

// serial RX	5
// serial TX	6
// SDA			A4
// SCL			A5

// ******************************************

#include <SoftwareSerial.h>
SoftwareSerial IMUserial(5, 6);

#include <Wire.h>
#include <Adafruit_ADS1015.h>
Adafruit_ADS1115 ads(0x48);

//loop time variables in microseconds
const unsigned int LOOP_TIME = 100; // 10 hz
unsigned int lastTime = LOOP_TIME;
unsigned int currentTime = LOOP_TIME;
byte watchdogTimer = 12;
byte serialResetTimer = 0; //if serial buffer is getting full, empty it

//Kalman variables
float rollK = 0;
float XeRoll = 0;
int CurrentRoll = 0;
float Pc = 0.0;
float G = 0.0;
float P = 1.0;
float Xp = 0.0;
float Zp = 0.0;
const float varRoll = 0.1; // variance, smaller, more filtering
const float varProcess = 0.0001; //smaller is more filtering

 //program flow
bool isDataFound = false, isSettingFound = false;
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

float CurrentSpeed = 0.0;

//PID variables
float Ko = 0.0f;  //overall gain
float Kp = 0.0f;  //proportional gain
float Ki = 0.0f;//integral gain
float Kd = 0.0f;  //derivative gain

//integral values - **** change as required *****
float maxIntegralValue = 20; //max PWM value for integral PID component

//IMU
int IMUheading = 9999;	// *******  if there is no gyro installed send 9999
int IMUroll = 9999;	//*******  if no roll is installed, send 9999
boolean IMUdataFound = false;
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

void setup()
{
	//set up communication
	Wire.begin();
	Serial.begin(38400);

	// software serial
	IMUserial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println("AutoSteerNano  :  Version Date: 11/Feb/2020");
	Serial.println();

	//keep pulled high and drag low to activate, noise free safe
	pinMode(WORKSW_PIN, INPUT_PULLUP);
	pinMode(STEERSW_PIN, INPUT_PULLUP);
	pinMode(DIR_PIN, OUTPUT);
	pinMode(EncoderPin, INPUT_PULLUP);

	ads.begin();

	//Setup Interrupt -Steering Wheel encoder 
	attachInterrupt(digitalPinToInterrupt(EncoderPin), EncoderISR, FALLING);
}

void loop()
{
	CommFromAOG();
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
		CommToAOG();
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
