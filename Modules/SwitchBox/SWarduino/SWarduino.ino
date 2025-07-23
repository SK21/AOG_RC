#include <EtherCard.h>
#include <EEPROM.h>
#include <SPI.h>

// Nano board for rate control switches
# define InoDescription "SWarduino"
const int16_t InoID = 23075;	// change to send defaults to eeprom
const uint8_t InoType = 3;		// 0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate

#define NC 255		// not connected

struct ModuleConfig
{
	// SW2 pcb
	uint8_t ID = 0;
	uint8_t MasterOn = 5;
	uint8_t	MasterOff = 3;
	uint8_t	RateUp = A3;
	uint8_t RateDown = A2;
	uint8_t AutoSection = A5;
	uint8_t AutoRate = A5;
	uint8_t WorkPin = NC;
	uint8_t SectionPins[16];
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 98;
};

ModuleConfig MDL;

// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen from PCBsetup
unsigned int SourcePort = 6200;		// to send from

// ethernet destination - Rate Controller
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[500]; // udp send and receive buffer
bool ENCfound;
static byte selectPin = 10;

unsigned long LastTime;

//reset function
void(*resetFunc) (void) = 0;

uint32_t BlinkTime;

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
}

void loop()
{
	if (millis() - LastTime > 250)
	{
		LastTime = millis();
		SendData();
	}

	if (EthernetConnected())
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}

	//Blink();
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

//double debug1;
//double debug2;
//void Blink()
//{
//	if (millis() - BlinkTime > 1000)
//	{
//		BlinkTime = millis();
//
//		Serial.println("");
//		Serial.print(debug1);
//
//		Serial.print(", ");
//		Serial.print(debug2);
//
//		Serial.println("");
//	}
//}