# define InoDescription "RCarduinoTeensy  :  04-Nov-2021"
// for use with PCB 7 and Teensy 4.1

// user settings ********************

#define ModuleID 0			// unique ID 0-15
#define IPMac 110			// unique number for Arduino IP address and Mac part 6, 0-255
#define IPpart3 1			// ex: 192.168.IPpart3.255, 0-255

// defined relay to be used as switched power relay - turns on when sketch starts, required for some Raven valves
// use a number greater than 15 to disable
#define SwitchedPowerRelay 7  // relay 0-15

byte FlowOn[] = { LOW, LOW };		// on value for flowmeter or motor direction
#define RelayOn LOW     
#define SensorCount 5           // rate sensors
#define ManualSwitchDelay 500   // milliseconds between manual adjustments
const unsigned long LOOP_TIME = 50; //in msec = 10hz

// **********************************
#include <Wire.h>
#include <Adafruit_ADS1015.h>
Adafruit_ADS1115 Pressures;
int16_t adc[4];
uint8_t PressureSend[] = { 127,109,ModuleID,0,0,0,0,0,0,0,0 };

#include <ArduinoModBus.h>
byte SlaveID = 0x01;

byte FlowPin[] = { 14,40,39,38,37 }; 
byte FlowDir[] = { 12,25,27,29,31 };
byte FlowPWM[] = { 24,26,28,30,32 };

bool FlowEnabled[] = { false, false,false,false,false };
float rateError[] = { 0, 0,0,0,0 }; //for PID
float UPM[SensorCount];   // UPM rate
int pwmSetting[SensorCount];

byte PIDkp[] = { 20, 20,20,20,20 };
byte PIDminPWM[] = { 50, 50,50,50,50 };
byte PIDLowMax[] = { 100, 100,100,100,100 };
byte PIDHighMax[] = { 255, 255,255,255,255 };
byte PIDdeadband[] = { 3, 3,3,3,3 };
byte PIDbrakePoint[] = { 20, 20,20,20,20 };
byte AdjustTime[5];

byte InCommand[] = { 0, 0,0,0,0 };		// command byte from RateController
byte ControlType[] = { 0, 0,0,0,0 };    // 0 standard, 1 Fast Close, 2 Motor

float TotalPulses[SensorCount];
bool SimulateFlow[] = { true, true,true,true,true };

byte ManualPWMsetting[] = { 0, 0,0,0,0 };
float RateSetting[] = { 0.0, 0.0,0.0,0.0,0.0 };	// auto UPM setting
float MeterCal[] = { 1.0, 1.0,1.0,1.0,1.0 };	// pulses per Unit

bool UseMultiPulses[SensorCount] = { 0, 0,0,0,0 };   //  0 - average time for multiple pulses, 1 - time for one pulse
float NewRateFactor[SensorCount];
unsigned long ManualLast[SensorCount];
float ManualFactor[SensorCount];

unsigned long CommTime[SensorCount];
bool AutoOn = true;

uint8_t RelayHi = 0;	// sections 8-15
uint8_t RelayLo = 0;	// sections 0-7

uint8_t MSB;
uint8_t LSB;
uint16_t PGN;

bool PGN32619Found;
uint16_t WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
uint8_t WifiSwitches[5];
uint8_t SwitchBytes[4];
uint8_t SwitchID[] = { 0,1,2,3,9,9,9,9,9,9,9,9,9,9,9,9 };

uint16_t RateLoopLast;
uint8_t RateSend[SensorCount][11];

// ethernet
#include <NativeEthernet.h>
#include <NativeEthernetUdp.h>
// ethernet interface ip address
IPAddress SourceIP(192, 168, IPpart3, IPMac);

// ethernet mac address - must be unique on your network
static uint8_t LocalMac[] = { 0x70,0x69,IPMac,0x2D,0x30,0x31 };

// ethernet destination - Rate app
IPAddress DestinationIP(192, 168, IPpart3, 255);
uint16_t  DestinationPort = 29999; //AOG port that listens 

// An EthernetUDP instance to let us send and receive packets over UDP
EthernetUDP UDPmain;
uint16_t  ListeningPort = 28888;		// local port to listen on
uint8_t RateData[UDP_TX_PACKET_MAX_SIZE];  // Buffer For Receiving UDP Data

bool FlowEnabledAny;

void setup()
{
    Serial.begin(38400);

    delay(5000);
    Serial.println();
    Serial.println(InoDescription);
    Serial.print("Module ID: ");
    Serial.println(ModuleID);
    Serial.println();

    // ethernet start
    Ethernet.begin(LocalMac, SourceIP);
    UDPmain.begin(ListeningPort);

    // flow
    for (int i = 0; i < SensorCount; i++)
    {
        pinMode(FlowPin[i], INPUT_PULLUP);
        pinMode(FlowDir[i], OUTPUT);
        pinMode(FlowPWM[i], OUTPUT);
    }

    // interrupts
    attachInterrupt(digitalPinToInterrupt(FlowPin[0]), ISR0, RISING);
    attachInterrupt(digitalPinToInterrupt(FlowPin[1]), ISR1, RISING);
    attachInterrupt(digitalPinToInterrupt(FlowPin[2]), ISR2, RISING);
    attachInterrupt(digitalPinToInterrupt(FlowPin[3]), ISR3, RISING);
    attachInterrupt(digitalPinToInterrupt(FlowPin[4]), ISR4, RISING);

    // start modbus using Serial1
    if (!ModbusRTUClient.begin(9600))
    {
        Serial.println("Failed to start Modbus RTU Client!");
    }

#if(UseSwitchedPowerRelay < 16)
    // turn on
    SetRelays(0, 0);
#endif

    Pressures.begin();

    Serial.println("Finished setup.");
}

void loop()
{
    ReceiveUDP();
    ReceiveWemos();
    GetUPM();

    FlowEnabledAny = false;
    for (int i = 0; i < SensorCount; i++)
    {
        FlowEnabled[i] = (millis() - CommTime[i] < 4000) && (RateSetting[i] > 0);
        FlowEnabledAny = FlowEnabledAny || FlowEnabled[i];
    }

    // Relays
    if (WifiSwitchesEnabled)
    {
        if (millis() - WifiSwitchesTimer > 30000)   // 30 second timer
        {
            // wifi switches have timed out
            WifiSwitchesEnabled = false;
            SetRelays(0, 0);
        }
    }
    else
    {
        if (FlowEnabledAny)
        {
            SetRelays(RelayHi, RelayLo);
        }
        else
        {
            SetRelays(0, 0);
        }
    }

    AdjustFlow();


    if (millis() - RateLoopLast >= LOOP_TIME)
    {
        RateLoopLast = millis();
        if (AutoOn)
        {
            AutoControl();
        }
        else
        {
            ManualControl();
        }

        // read pressures
        for (int i = 0; i < 4; i++)
        {
            adc[i] = Pressures.readADC_SingleEnded(i);
        }

        SendUDP();
    }
}

void AutoControl()
{
    for (int i = 0; i < SensorCount; i++)
    {
        switch (ControlType[i])
        {
        case 2:
            // motor control
            if (SimulateFlow[i]) SimulateMotor(PIDHighMax[i], i);
            rateError[i] = RateSetting[i] - UPM[i];

            // calculate new value
            pwmSetting[i] = ControlMotor(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i],
                PIDHighMax[i], PIDdeadband[i], i);
            break;

        default:
            // valve control
            // calculate new value
            if (SimulateFlow[i]) SimulateValve(PIDminPWM[i], PIDHighMax[i], i);
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
        if (millis() - ManualLast[i] > ManualSwitchDelay)
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
            if (SimulateFlow[i]) SimulateMotor(PIDHighMax[i], i);
            rateError[i] = RateSetting[i] - UPM[i];
            break;

        default:
            // valve control
            if (SimulateFlow[i])
            {
                SimulateValve(PIDminPWM[i], PIDHighMax[i], i);
            }
            rateError[i] = RateSetting[i] - UPM[i];
            break;
        }
    }
}

