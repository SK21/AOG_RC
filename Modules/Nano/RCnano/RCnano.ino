
#include <Adafruit_MCP23X08.h>
#include <Adafruit_MCP23X17.h>
#include <Adafruit_MCP23XXX.h>

#include <EtherCard.h>
#include <Wire.h>

# define InoDescription "RCnano  :  11-Apr-2022"

// user settings ****************************
#define CommType 0                  // 0 Serial USB, 1 UDP wired 
#define ModuleID 0			        // unique ID 0-15
#define SensorCount 1

#define IPMac 110			        // unique number for Arduino IP address and Mac part 6, 0-255
#define IPpart3 5			        // ex: 192.168.IPpart3.255, 0-255
const unsigned long LOOP_TIME = 50; //in msec = 20hz

#define UseMCP23017 1               // 0 use Nano pins for relays, 1 use MCP23017 to control relays
byte FlowOn[] = {LOW, LOW};		    // on value for flowmeter or motor direction
#define RelayOn LOW
// ******************************************

#if (CommType == 1)
// ethernet interface ip address
static byte ArduinoIP[] = { 192, 168, IPpart3, IPMac };

// ethernet interface Mac address
static byte LocalMac[] = { 0x70, 0x2D, IPMac, 0x31, 0x21, 0x62 };

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
byte toSend[2][11] = { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
#endif

#if (UseMCP23017 == 1)
Adafruit_MCP23X17 mcp;

// Pin number is an integer in the range 0..15,
// where pins numbered from 0 to 7 are on Port A, GPA0 = 0,
// and pins numbered from 8 to 15 are on Port B, GPB0 = 8.

// MCP23017 pins RC5, RC8
#define Relay1 8
#define Relay2 9
#define Relay3 10
#define Relay4 11
#define Relay5 12
#define Relay6 13
#define Relay7 14
#define Relay8 15

#define Relay9 7
#define Relay10 6
#define Relay11 5
#define Relay12 4
#define Relay13 3
#define Relay14 2
#define Relay15 1
#define Relay16 0


// Nano pins
byte FlowPin[] = {2, 3}; // interrupt on this pin
byte FlowDir[] = {4, 6};
byte FlowPWM[] = {5, 9};

#else
// use Nano pins for relays

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

byte FlowPin[] = { 2, 3 }; // interrupt on this pin
byte FlowDir[] = { 4, 6 };
byte FlowPWM[] = { 5, 9 };

#define Relay1 A0
#define Relay2 A1
#define Relay3 A2
#define Relay4 A3
//#define Relay5 12
//#define Relay6 13
//#define Relay7 14
//#define Relay8 15

#endif

bool FlowEnabled[] = {false, false};
float rateError[] = {0, 0}; //for PID

float UPM[SensorCount];   // UPM rate
int pwmSetting[SensorCount];

// PID
byte PIDkp[] = {20, 20};
byte PIDminPWM[] = {50, 50};
byte PIDLowMax[] = {100, 100};
byte PIDHighMax[] = { 255, 255};
byte PIDdeadband[] = {3, 3};
byte PIDbrakePoint[] = {20, 20};
byte AdjustTime[2];

byte InCommand[] = {0, 0};		// command byte from RateController
byte ControlType[] = {0, 0};  // 0 standard, 1 Fast Close, 2 Motor

unsigned long TotalPulses[SensorCount];

byte ManualPWMsetting[] = {0, 0};
float RateSetting[] = {0.0, 0.0};	// auto UPM setting
float MeterCal[] = {1.0, 1.0};	// pulses per Unit

unsigned long CommTime[SensorCount];

//bit 0 is section 0
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15

//loop time variables in microseconds
unsigned long lastTime = LOOP_TIME;
byte watchdogTimer = 0;

byte Temp = 0;
unsigned int UnSignedTemp = 0;
bool AutoOn = true;

unsigned int PGN;
bool PGN32614Found;
bool PGN32616Found;

float NewRateFactor[2];
unsigned long ManualLast[2];
float ManualFactor[2];

// WifiSwitches connection to Wemos D1 Mini
// Use Serial RX, remove RX wire before uploading
bool PGN32619Found;

bool PGN32620Found;
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[5];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

bool EthernetEnabled = false;
bool UseMultiPulses[2] = { 0, 0 };   //  0 - average time for multiple pulses, 1 - time for one pulse

void setup()
{
    Serial.begin(38400);

    delay(5000);
    Serial.println();
    Serial.println(InoDescription);
    Serial.print("Module ID: ");
    Serial.println(ModuleID);
    Serial.println();

    Wire.begin();
#if (UseMCP23017 == 1)
    Wire.beginTransmission(0x20);
    if (Wire.endTransmission() == 0)
    {
        Serial.println("MCP23017 Found.");
    }
    else
    {
        Serial.println("MCP23017 not found.");
    }

    mcp.begin_I2C();

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

#else
    // Nano pins
    pinMode(Relay1, OUTPUT);
    pinMode(Relay2, OUTPUT);
    pinMode(Relay3, OUTPUT);
    pinMode(Relay4, OUTPUT);
    //pinMode(Relay5, OUTPUT);
    //pinMode(Relay6, OUTPUT);
    //pinMode(Relay7, OUTPUT);
    //pinMode(Relay8, OUTPUT);

    for (int i = 0; i < SensorCount; i++)
    {
        pinMode(FlowPin[i], INPUT_PULLUP);
        pinMode(FlowDir[i], OUTPUT);
        pinMode(FlowPWM[i], OUTPUT);
    }
#endif

    attachInterrupt(digitalPinToInterrupt(FlowPin[0]), ISR0, FALLING);
    attachInterrupt(digitalPinToInterrupt(FlowPin[1]), ISR1, FALLING);

#if (CommType == 1)
    while (!EthernetEnabled)
    {
        EthernetEnabled = (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) != 0);
        Serial.print(".");
    }

    Serial.println("");
    Serial.println("Ethernet controller found.");

    ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

    ether.printIp("IP Address:     ", ether.myip);
    Serial.print("Destination IP: ");
    Serial.println(IPadd(DestinationIP));

    //register sub for received data
    ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif

    Serial.println("Finished Setup.");
}

void loop()
{
    ReceiveSerial();

    if (millis() - lastTime >= LOOP_TIME)
    {
        lastTime = millis();
        GetUPM();

        for (int i = 0; i < SensorCount; i++)
        {
            FlowEnabled[i] = (millis() - CommTime[i] < 4000) && (RateSetting[i] > 0);
        }

        CheckRelays();
        AdjustFlow();

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
            rateError[i] = RateSetting[i] - UPM[i];

            // calculate new value
            pwmSetting[i] = ControlMotor(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i],
                PIDHighMax[i], PIDdeadband[i], i);
            break;

        default:
            // valve control
            // calculate new value
            rateError[i] = RateSetting[i] - UPM[i];

            pwmSetting[i] = DoPID(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i], PIDLowMax[i],
                PIDHighMax[i], PIDbrakePoint[i], PIDdeadband[i], i);
            break;
        }
    }
}

void ManualControl()
{
    for (int i = 0; i < SensorCount; i++)
    {
        if (millis() - ManualLast[i] > 1000)
        {
            ManualLast[i] = millis();

            // adjust rate
            if (RateSetting[i] == 0) RateSetting[i] = 1; // to make FlowEnabled

            switch (ControlType[i])
            {
            case 2:
                // motor control
                pwmSetting[i] *= NewRateFactor[i];
                if (pwmSetting[i] == 0 && NewRateFactor[i] > 0) pwmSetting[i] = PIDminPWM[i];
                break;

            default:
                // valve control
                pwmSetting[i] = 0;

                if (NewRateFactor[i] < 1)
                {
                    // rate down
                    pwmSetting[i] = -PIDminPWM[i];
                }
                else if (NewRateFactor[i] > 1)
                {
                    // rate up
                    pwmSetting[i] = PIDminPWM[i];
                }

                break;
            }
        }

        switch (ControlType[i])
        {
            // calculate application rate
        case 2:
            // motor control
            rateError[i] = RateSetting[i] - UPM[i];
            break;

        default:
            // valve control
            rateError[i] = RateSetting[i] - UPM[i];
            break;
        }
    }
}

void TranslateSwitchBytes()
{
    // Switch IDs from Rate Controller
    // ex: byte 2: bits 0-3 identify switch # (0-15) for sec 0
    // ex: byte 2: bits 4-7 identify switch # (0-15) for sec 1

    for (int i = 0; i < 16; i++)
    {
        byte ByteID = i / 2;
        byte Mask = 15 << (4 * (i - 2 * ByteID));    // move mask to correct bits
        SectionSwitchID[i] = SwitchBytes[ByteID] & Mask;    // mask out bits
        SectionSwitchID[i] = SectionSwitchID[i] >> (4 * (i - 2 * ByteID)); // move bits for number
    }
}

void ToSerial(char* Description, float Val)
{
    Serial.print(Description);
    Serial.println(Val);
}
