// user settings ****************************
#define CommType 1          // 0 Serial/USB , 1 UDP wired Nano, 2 UDP wifi Nano33

#define ModuleID 0			// unique ID 0-15
#define IPMac 110			// unique number for Arduino IP address and Mac part 6, 0-255
#define IPpart3 1			// ex: 192.168.IPpart3.255, 0-255

#define WifiSSID "tractor"
#define WifiPassword ""

const unsigned long LOOP_TIME = 200; //in msec = 5hz

#define UseSwitchedPowerPin 1	// 0 use Relay8 as a normal relay
// 1 use Relay8 as a switched power pin - turns on when sketch starts, required for Raven valve

byte FlowOn[] = {HIGH, HIGH};		// on value for flowmeter or motor direction
byte SlowSpeed[] = { 9,9 };		// for vcn, low pwm rate, 0 fast, 9 slow
byte LowMsPulseTrigger[] = {50, 50}; 	// ms/pulse below which is low ms/pulse flow sensor

#define SensorCount 2

// ******************************************

#if (CommType == 1)
#include <EtherCard.h>
// ethernet interface ip address
static byte ArduinoIP[] = { 192, 168, IPpart3, IPMac };

// ethernet interface Mac address
static byte LocalMac[] = { 0x70, 0x2D, 0x31, 0x21, 0x62, IPMac };

// gateway ip address
static byte gwip[] = { 192, 168, IPpart3, 1 };
//DNS- you just need one anyway
static byte myDNS[] = { 8, 8, 8, 8 };
//mask
static byte mask[] = { 255, 255, 255, 0 };

// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
unsigned int SourcePort = 6100;		// to send from

// ethernet destination - Rate Controller
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[500]; // udp send and receive buffer

//Array to send data back to AgOpenGPS
byte toSend[2][10] = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
#endif

#if (CommType == 2)
#include <SPI.h>
#include <WiFiNINA.h>
#include <WiFiUdp.h>

int ConnectionStatus = WL_IDLE_STATUS;
char ssid[] = WifiSSID;        // your network SSID (name)
char pass[] = WifiPassword;    // your network password (use for WPA, or use as key for WEP)

char InBuffer[150];	 //buffer to hold incoming packet
byte toSend[2][10] = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } }; //Array to send data back to AgOpenGPS

WiFiUDP UDPin;
WiFiUDP UDPout;

unsigned int ListeningPort = 28888;	// local port to listen on
unsigned int SourcePort = 6100;		// local port to send from

// ethernet destination - AOG
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // RateControl port that listens

unsigned long CheckTime;
unsigned long ConnectedCount = 0;
unsigned long ReconnectCount = 0;
#endif

#include "Adafruit_MCP23017.h"

Adafruit_MCP23017 mcp;

// MCP23017 pins
#define Relay1 8
#define Relay2 9
#define Relay3 10
#define Relay4 11
#define Relay5 12
#define Relay6 13
#define Relay7 14
#define Relay8 15

#define Relay9 0
#define Relay10 1
#define Relay11 2
#define Relay12 3
#define Relay13 4
#define Relay14 5
#define Relay15 6
#define Relay16 7

// Nano pins
byte FlowPin[] = {3, 2}; // interrupt on this pin
byte FlowDir[] = {4, 6};
byte FlowPWM[] = {5, 9};

bool ApplicationOn[] = {false, false};
float rateError[] = {0, 0}; //for PID

float UPM[SensorCount];   // UPM rate
int pwmSetting[SensorCount];

// VCN
long VCN[] = {343, 343};
long SendTime[] = {400, 400};	// ms pwm is sent to valve
long WaitTime[] = {500, 500};	// ms to wait before adjusting valve again
byte VCNminPWM[] = {200, 200};
byte VCNmaxPWM[] = {255, 255};

// PID
byte PIDkp[] = {20, 20};
byte PIDminPWM[] = {50, 50};
byte PIDLowMax[] = {100, 100};
byte PIDHighMax[] = { 255, 255};
byte PIDdeadband[] = {3, 3};
byte PIDbrakePoint[] = {20, 20};

byte InCommand[] = {0, 0};		// command byte from RateController
byte ControlType[] = {0, 0};    	// 0 standard, 1 Fast Close, 2 Motor

float TotalPulses[SensorCount];
bool SimulateFlow[] = {true, true};
bool UseVCN[] = {1, 1};		// 0 PID, 1 VCN

byte ManualPWMsetting[] = {0, 0};
float RateSetting[] = {0.0, 0.0};	// auto UPM setting
float MeterCal[] = {17.0, 17.0};	// pulses per Unit

unsigned long CommTime[SensorCount];

//bit 0 is section 0
byte RelayHi = 0;	// sections 8-15
byte RelayLo = 0;	// sections 0-7

//loop time variables in microseconds
unsigned long lastTime = LOOP_TIME;
byte watchdogTimer = 0;

byte Temp = 0;
unsigned int UnSignedTemp = 0;

bool PGN32614Found;
bool PGN32615Found;
bool PGN32616Found;

byte MSB;
byte LSB;
unsigned int PGN;

bool AutoOn = true;

float NewRateFactor[2];
unsigned long ManualLast[2];

void setup()
{
  Serial.begin(38400);

  delay(5000);
  Serial.println();
  Serial.println("RCarduino  :  12-Mar-2021");
  Serial.println("Module ID: " + String(ModuleID));
  Serial.println();

  mcp.begin();

  // MCP20317 pins
  mcp.pinMode(Relay1, OUTPUT);
  mcp.pinMode(Relay2, OUTPUT);
  mcp.pinMode(Relay3, OUTPUT);
  mcp.pinMode(Relay4, OUTPUT);
  mcp.pinMode(Relay5, OUTPUT);
  mcp.pinMode(Relay6, OUTPUT);
  mcp.pinMode(Relay7, OUTPUT);
  mcp.pinMode(Relay8, OUTPUT);

  mcp.pinMode(Relay9, OUTPUT);
  mcp.pinMode(Relay10, OUTPUT);
  mcp.pinMode(Relay11, OUTPUT);
  mcp.pinMode(Relay12, OUTPUT);
  mcp.pinMode(Relay13, OUTPUT);
  mcp.pinMode(Relay14, OUTPUT);
  mcp.pinMode(Relay15, OUTPUT);
  mcp.pinMode(Relay16, OUTPUT);

  // Nano pins
  for (int i = 0; i < SensorCount; i++)
  {
      pinMode(FlowPin[i], INPUT_PULLUP);
      pinMode(FlowDir[i], OUTPUT);
      pinMode(FlowPWM[i], OUTPUT);
  }
#if(UseSwitchedPowerPin == 1)
  // turn on
  mcp.digitalWrite(Relay8, HIGH);
#endif

  attachInterrupt(digitalPinToInterrupt(FlowPin[0]), PPM0isr, RISING);
  attachInterrupt(digitalPinToInterrupt(FlowPin[1]), PPM1isr, RISING);

#if (CommType == 1)
  if (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) == 0) Serial.println(F("Failed to access Ethernet controller"));
  ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

  ether.printIp("IP Address:     ", ether.myip);
  Serial.println("Destination IP: " + IPadd(DestinationIP));

  //register sub for received data
  ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif

#if (CommType == 2)
  // check for the WiFi module:
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
#endif
  Serial.println("Finished Setup.");
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

    for (int i = 0; i < SensorCount; i++)
    {
        ApplicationOn[i] = (millis() - CommTime[i] < 4000) && (RateSetting[i] > 0);
    }

    SetRelays();

    motorDrive();

    if (millis() - lastTime >= LOOP_TIME)
    {
        lastTime = millis();
        if (AutoOn)
        {
            AutoControl();
        }
        else
        {
            ManualControl();
        }

        // check connection to AOG
        watchdogTimer++;
        if (watchdogTimer > 30)
        {
            //clean out serial buffer
            while (Serial.available() > 0) char t = Serial.read();

            watchdogTimer = 0;
        }

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

#if(CommType == 2)
SendUDPWifi();
}
#endif
}

String IPadd(byte Address[])
{
  return String(Address[0]) + "." + String(Address[1]) + "." + String(Address[2]) + "." + String(Address[3]);
}

bool IsBitSet(byte b, int pos)
{
  return ((b >> pos) & 1) != 0;
}

byte ParseModID(byte ID)
{
  // top 4 bits
  return ID >> 4;
}

byte ParseSenID(byte ID)
{
  // bottom 4 bits
  return (ID & 0b00001111);
}

byte BuildModSenID(byte Mod_ID, byte Sen_ID)
{
  return ((Mod_ID << 4) | (Sen_ID & 0b00001111));
}

void AutoControl()
{
    for (int i = 0; i < SensorCount; i++)
    {
        switch (ControlType[i])
        {
        case 2:
            // motor control
            if (SimulateFlow[i]) SimulateMotor(PIDminPWM[i], PIDHighMax[i], i);
            rateError[i] = RateSetting[i] - GetUPM(i);

            // calculate new value
            pwmSetting[i] = ControlMotor(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i],
                PIDHighMax[i], PIDdeadband[i], i);
            break;

        default:
            // valve control
            // calculate new value
            if (UseVCN[i])
            {
                if (SimulateFlow[i]) SimulateValve(VCNminPWM[i], VCNmaxPWM[i], i);
                rateError[i] = RateSetting[i] - GetUPM(i);

                pwmSetting[i] = VCNpwm(rateError[i], RateSetting[i], VCNminPWM[i], VCNmaxPWM[i],
                    VCN[i], UPM[i], SendTime[i], WaitTime[i], SlowSpeed[i], ControlType[i], i);
            }
            else
            {
                if (SimulateFlow[i]) SimulateValve(PIDminPWM[i], PIDHighMax[i], i);
                rateError[i] = RateSetting[i] - GetUPM(i);

                pwmSetting[i] = DoPID(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i], PIDLowMax[i],
                    PIDHighMax[i], PIDbrakePoint[i], PIDdeadband[i], i);
            }
            break;
        }
    }
}

void ManualControl()
{
    for (int i = 0; i < SensorCount; i++)
    {
        if (millis() - ManualLast[i] > 1500)
        {
            ManualLast[i] = millis();

            // adjust rate
            if (RateSetting[i] == 0) RateSetting[i] = 1; // to make ApplicationOn

            switch (ControlType[i])
            {
            case 2:
                // motor control
                pwmSetting[i] *= NewRateFactor[i];
                if (pwmSetting[i] == 0 && NewRateFactor[i] > 0) pwmSetting[i] = PIDminPWM[i];
                break;

            default:
                // valve control
                if (NewRateFactor[i] < 1)
                {
                    // rate down
                    pwmSetting[i] = (1 - NewRateFactor[i]) * ((PIDHighMax[i] + PIDminPWM[i]) / 2) * -1;
                }
                else
                {
                    // rate up
                    pwmSetting[i] = (NewRateFactor[i] - 1) * ((PIDHighMax[i] + PIDminPWM[i]) / 2);
                }
                break;
            }
        }

        switch (ControlType[i])
        {
            // calculate application rate
        case 2:
            // motor control
            if (SimulateFlow[i]) SimulateMotor(PIDminPWM[i], PIDHighMax[i], i);
            rateError[i] = RateSetting[i] - GetUPM(i);
            break;

        default:
            // valve control
            if (SimulateFlow[i])
            {
                if (UseVCN[i])
                {
                    SimulateValve(VCNminPWM[i], VCNmaxPWM[i], i);
                }
                else
                {
                    SimulateValve(PIDminPWM[i], PIDHighMax[i], i);
                }
            }
            rateError[i] = RateSetting[i] - GetUPM(i);
            break;
        }
    }
}
