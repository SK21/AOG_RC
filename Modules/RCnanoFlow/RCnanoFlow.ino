
#include <SPI.h>
#include <EtherCard.h>
#include <EEPROM.h>

// Rate monitoring with nano. Sends the Hz of the flow meter. No rate control.
# define InoDescription "RCnanoFlow :  01-Jul-2024"
const uint16_t InoID = 1074;	// change to send defaults to eeprom, ddmmy, no leading 0
const uint8_t InoType = 5;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate, 5 - Nano Flow monitor
#define MaxProductCount 2

//const int DirPin = 6;		
//const int PWMPin = 9;		

struct ModuleConfig
{
	uint8_t ID = 0;
	uint8_t SensorCount = 2;        // up to 2 sensors
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
};

ModuleConfig MDL;

struct SensorConfig
{
	uint8_t FlowPin;
	double Hz;				// flow meter Hz sent X 1000
	bool UseMultiPulses;	// 0 - time for one pulse, 1 - average time for multiple pulses
};

SensorConfig Sensor[MaxProductCount];

// ethernet
byte Ethernet::buffer[100];			// udp send and receive buffer
static byte selectPin = 10;
uint16_t ListeningPort = 28888;
uint16_t DestinationPort = 29999;
byte DestinationIP[] = { MDL.IP0, MDL.IP1, MDL.IP2, 255 };	// broadcast 255
unsigned int SourcePort = 5123;		// to send from
bool ENCfound;

const uint16_t SendTime = 200;
uint32_t SendLast = SendTime;

//reset function
void(*resetFunc) (void) = 0;

bool EthernetConnected()
{
	bool Result = false;
	if (ENCfound)
	{
		Result = ether.isLinkUp();
	}
	return Result;
}

void setup()
{
	DoSetup();
	//pinMode(DirPin, OUTPUT);
	//pinMode(PWMPin, OUTPUT);
}

void loop()
{
	if (millis() - SendLast > SendTime)
	{
		SendLast = millis();
		GetUPM();
		SendData();

		//analogWrite(PWMPin, 70);
		//digitalWrite(DirPin, 0);
	}

	if (EthernetConnected())
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}
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
