#include <Adafruit_MCP23X08.h>
#include <Adafruit_MCP23X17.h>
#include <Adafruit_MCP23XXX.h>
#include <EtherCard.h>
#include <Wire.h>
#include <EEPROM.h>

# define InoDescription "RCnano  :  29-Jun-2022"
# define UseEthernet 1

// debug variables
bool DebugOn = false;
byte DebugCount1;
byte DebugCount2;
byte DebugVal1;
byte DebugVal2;
uint32_t DebugTime;

struct PCBconfig    // 5 bytes
{
	uint8_t ModuleID = 0;
	uint8_t UseMCP23017 = 1;        // 0 use Nano pins for relays, 1 use MCP23017 for relays
	uint8_t RelayOnSignal = 0;	    // value that turns on relays
	uint8_t FlowOnDirection = 0;	// sets on value for flow valve or sets motor direction
	uint8_t SensorCount = 1;        // up to 2 sensors
	uint8_t	IPpart3 = 1;			// IP address, 3rd octet
};

PCBconfig PCB;

// If using the ENC28J60 ethernet shield these pins
// are used by it and unavailable for relays:
// 7,8,10,11,12,13. It also pulls pin D2 high.
// D2 can be used if pin D2 on the shield is cut off
// and then mount the shield on top of the Nano.

struct PCBpinConfig // 22 bytes
{
	uint8_t Flow1 = 2;
	uint8_t Flow2 = 3;
	uint8_t Dir1 = 4;
	uint8_t Dir2 = 6;
	uint8_t PWM1 = 5;
	uint8_t PWM2 = 9;
	uint8_t Relays[16];
};

PCBpinConfig PINS;

#if UseEthernet
// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
unsigned int SourcePort = 6100;		// to send from

// ethernet destination - Rate Controller
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[500]; // udp send and receive buffer
#endif

Adafruit_MCP23X17 mcp;

// Pin number is an integer in the range 0-15,
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

// flow
byte FlowPin[2];
byte FlowDir[2];
byte FlowPWM[2];

bool FlowEnabled[] = { false, false };
float rateError[] = { 0, 0 };

const uint16_t LOOP_TIME = 50;      //in msec = 20hz
uint32_t LoopLast = LOOP_TIME;

const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

float UPM[2];   // UPM rate
int pwmSetting[2];

// PID
byte PIDkp[] = { 20, 20 };
byte PIDki[] = { 0,0 };
byte PIDminPWM[] = { 50, 50 };
byte PIDLowMax[] = { 100, 100 };
byte PIDHighMax[] = { 255, 255 };
byte PIDdeadband[] = { 3, 3 };
byte PIDbrakePoint[] = { 20, 20 };
byte AdjustTime[2];

byte InCommand[] = { 0, 0 };		// command byte from RateController
byte ControlType[] = { 0, 0 };  // 0 standard, 1 Fast Close, 2 Motor

unsigned long TotalPulses[2];
unsigned long CommTime[2];

byte ManualPWMsetting[] = { 0, 0 };
float RateSetting[] = { 0.0, 0.0 };	// auto UPM setting
float MeterCal[] = { 1.0, 1.0 };	// pulses per Unit

//bit 0 is section 0
byte RelayLo = 0;	// sections 0-7
byte RelayHi = 0;	// sections 8-15
byte PowerRelayLo;
byte PowerRelayHi;

byte Temp = 0;
unsigned int UnSignedTemp = 0;
bool AutoOn = true;

float ManualAdjust[2];
unsigned long ManualLast[2];
float ManualFactor[2];

// WifiSwitches connection to Wemos D1 Mini
// Use Serial RX, remove RX wire before uploading
unsigned long WifiSwitchesTimer;
bool WifiSwitchesEnabled = false;
byte WifiSwitches[6];

byte SwitchBytes[8];
byte SectionSwitchID[] = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
bool UseMultiPulses[2] = { 0, 0 };   //  0 - average time for multiple pulses, 1 - time for one pulse

  //reset function
void(*resetFunc) (void) = 0;

//EEPROM
int16_t EEread = 0;
#define PCB_Ident 2388

bool MasterOn[2];
byte Packet[30];
bool IOexpanderFound;
uint8_t ErrorCount;

unsigned int PGNudp;
unsigned int PGNserial;
byte MSB;
byte LSB;

void setup()
{
	Serial.begin(38400);

	// pcb data
	EEPROM.get(0, EEread);              // read identifier
	if (EEread != PCB_Ident)
	{
		EEPROM.put(0, PCB_Ident);
		EEPROM.put(10, PCB);
		EEPROM.put(40, PINS);
	}
	else
	{
		EEPROM.get(10, PCB);
		EEPROM.get(40, PINS);
	}

	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.print("Module ID: ");
	Serial.println(PCB.ModuleID);
	Serial.println();

	if (PCB.SensorCount < 1) PCB.SensorCount = 1;
	if (PCB.SensorCount > 2) PCB.SensorCount = 2;

	if (PCB.UseMCP23017)
	{
		Serial.println("Using MCP23017 for relays.");
	}
	else
	{
		Serial.println("Using Nano pins for relays.");
	}

	Wire.begin();
	if (PCB.UseMCP23017)
	{
		// I/O expander on default address 0x20
		Serial.println("");
		Serial.println("Starting I/O Expander ...");
		ErrorCount = 0;
		while (!IOexpanderFound)
		{
			Serial.print(".");
			Wire.beginTransmission(0x20);
			IOexpanderFound = (Wire.endTransmission() == 0);
			ErrorCount++;
			delay(500);
			if (ErrorCount > 5) break;
		}

		Serial.println("");
		if (IOexpanderFound)
		{
			Serial.println("I/O Expander found.");
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

		}
		else
		{
			Serial.println("I/O Expander not found.");
		}
	}
	else
	{
		// Nano pins
		for (int i = 0; i < 16; i++)
		{
			// check if relay is enabled (pins 0 and 1 are for comm) and set pin mode
			if (PINS.Relays[i] > 1) pinMode(PINS.Relays[i], OUTPUT);
		}
	}

	// flow
	FlowPin[0] = PINS.Flow1;
	FlowPin[1] = PINS.Flow2;
	FlowDir[0] = PINS.Dir1;
	FlowDir[1] = PINS.Dir2;
	FlowPWM[0] = PINS.PWM1;
	FlowPWM[1] = PINS.PWM2;

	for (int i = 0; i < PCB.SensorCount; i++)
	{
		pinMode(FlowPin[i], INPUT_PULLUP);
		pinMode(FlowDir[i], OUTPUT);
		pinMode(FlowPWM[i], OUTPUT);
	}

	attachInterrupt(digitalPinToInterrupt(FlowPin[0]), ISR0, FALLING);
	attachInterrupt(digitalPinToInterrupt(FlowPin[1]), ISR1, FALLING);

#if UseEthernet
	Serial.println("");
	Serial.println("Starting Ethernet ...");
	// ethernet interface ip address
	byte ArduinoIP[] = { 192, 168,PCB.IPpart3, 207 + PCB.ModuleID };

	// ethernet interface Mac address
	byte LocalMac[] = { 0x70, 0x31, 0x21, 0x2D, 0x62, PCB.ModuleID };

	// gateway ip address
	static byte gwip[] = { 192, 168,PCB.IPpart3, 1 };

	//DNS- you just need one anyway
	static byte myDNS[] = { 8, 8, 8, 8 };

	//mask
	static byte mask[] = { 255, 255, 255, 0 };

	DestinationIP[2] = PCB.IPpart3;

	ether.begin(sizeof Ethernet::buffer, LocalMac, 10) != 0;

	Serial.println("");
	Serial.println("Ethernet controller found.");
	ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
	ether.printIp("IP Address:     ", ether.myip);

	//register sub for received data
	ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif

	Serial.println("");
	Serial.println("Finished Setup.");
}

void loop()
{
	ReceiveSerial();

	if (millis() - LoopLast >= LOOP_TIME)
	{
		LoopLast = millis();
		GetUPM();

		for (int i = 0; i < PCB.SensorCount; i++)
		{
			FlowEnabled[i] = (millis() - CommTime[i] < 4000) && (RateSetting[i] > 0) && MasterOn[i];
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
	}

	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		SendSerial();
#if UseEthernet
		SendUDPwired();
#endif
	}

#if UseEthernet
	delay(10);

	//this must be called for ethercard functions to work.
	ether.packetLoop(ether.packetReceive());
#endif

	if (DebugOn)DebugTheINO();
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
	for (int i = 0; i < PCB.SensorCount; i++)
	{
		rateError[i] = RateSetting[i] - UPM[i];

		switch (ControlType[i])
		{
		case 2:
			// motor control
			pwmSetting[i] = ControlMotor(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i],
				PIDHighMax[i], PIDdeadband[i], i);
			break;

		default:
			// valve control
			pwmSetting[i] = DoPID(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i], PIDLowMax[i],
				PIDHighMax[i], PIDbrakePoint[i], PIDdeadband[i], i, PIDki[i]);
			break;
		}
	}
}

void ManualControl()
{
	for (int i = 0; i < PCB.SensorCount; i++)
	{
		rateError[i] = RateSetting[i] - UPM[i];

		if (millis() - ManualLast[i] > 1000)
		{
			ManualLast[i] = millis();

			// adjust rate
			if (RateSetting[i] == 0) RateSetting[i] = 1; // to make FlowEnabled

			switch (ControlType[i])
			{
			case 2:
				// motor control
				if (ManualAdjust[i] > 0)
				{
					pwmSetting[i] *= 1.10;
					if (pwmSetting[i] < 1) pwmSetting[i] = PIDminPWM[i];
					if (pwmSetting[i] > 255) pwmSetting[i] = 255;
				}
				else if (ManualAdjust[i] < 0)
				{
					pwmSetting[i] *= 0.90;
					if (pwmSetting[i] < PIDminPWM[i]) pwmSetting[i] = 0;
				}
				break;

			default:
				// valve control
				pwmSetting[i] = ManualAdjust[i];
				break;
			}
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
bool GoodCRC(uint16_t Length)
{
	byte ck = CRC(Length - 1, 0);
	bool Result = (ck == Packet[Length - 1]);
	return Result;
}

byte CRC(int Length, byte Start)
{
	byte Result = 0;
	if (Length <= sizeof(Packet))
	{
		int CK = 0;
		for (int i = Start; i < Length; i++)
		{
			CK += Packet[i];
		}
		Result = (byte)CK;
	}
	return Result;
}

void DebugTheINO()
{
	// send debug info to RateController
	if (millis() - DebugTime > 1000)
	{
		DebugTime = millis();

		// Serial
		// PGNudp 2748 - 0xABC
		Serial.print(0xBC);
		Serial.print(",");
		Serial.print(0xA);
		Serial.print(",");
		Serial.print(DebugVal1);
		Serial.print(",");
		Serial.print(DebugVal2);
		Serial.print(",");
		Serial.print(DebugCount1);
		Serial.print(",");
		Serial.print(DebugCount2);

		Serial.println("");

#if UseEthernet
		// UDP
		Packet[0] = 0xBC;
		Packet[1] = 0xA;
		Packet[2] = DebugVal1;
		Packet[3] = DebugVal2;
		Packet[4] = DebugCount1;
		Packet[5] = DebugCount2;
		ether.sendUdp(Packet, 12, SourcePort, DestinationIP, DestinationPort);
#endif
	}
}

