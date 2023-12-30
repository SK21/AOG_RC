#include <EtherCard.h>
#include <EEPROM.h>
#include <SPI.h>

// Nano board for rate control switches
# define InoDescription "SWarduino  :  30-Dec-2023"
const int16_t InoID = 30123;	// change to send defaults to eeprom
int16_t StoredID;				// Defaults ID stored in eeprom

struct ModuleConfig
{
	// SW2 pcb
	uint8_t ID = 0;
	uint8_t	Auto = A5;
	uint8_t MasterOn = 5;
	uint8_t	MasterOff = 3;
	uint8_t	RateUp = A3;
	uint8_t RateDown = A2;
	uint8_t PinIDs[16];
	uint8_t IP0 = 192;
	uint8_t IP1 = 168;
	uint8_t IP2 = 1;
	uint8_t IP3 = 50;
};

ModuleConfig MDL;

// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
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

void setup()
{
	Serial.begin(38400);

	delay(2000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	// defaults
	MDL.PinIDs[0] = A4;
	MDL.PinIDs[1] = 9;
	MDL.PinIDs[2] = 6;
	MDL.PinIDs[3] = 4;

	for (int i = 4; i < 16; i++)
	{
		MDL.PinIDs[i] = 0;
	}

	//eeprom
	EEPROM.get(0, StoredID);

	if (StoredID == InoID)
	{
		// load stored data
		Serial.println("Loading stored settings.");
		EEPROM.get(10, MDL);
	}
	else
	{
		// update stored data
		Serial.println("Updating stored data.");
		EEPROM.put(0, InoID);
		EEPROM.put(10, MDL);
	}

	pinMode(MDL.Auto, INPUT_PULLUP);
	pinMode(MDL.MasterOn, INPUT_PULLUP);
	pinMode(MDL.MasterOff, INPUT_PULLUP);
	pinMode(MDL.RateUp, INPUT_PULLUP);
	pinMode(MDL.RateDown, INPUT_PULLUP);

	for (int i = 0; i < 16; i++)
	{
		if (MDL.PinIDs[i] > 0) pinMode(MDL.PinIDs[i], INPUT_PULLUP);
	}

	Serial.println("");

	// ethernet
	Serial.println("Starting Ethernet ...");
	MDL.IP3 = MDL.ID + 188;
	byte ArduinoIP[] = { MDL.IP0, MDL.IP1,MDL.IP2, MDL.IP3 };
	static uint8_t LocalMac[] = { 0x70,0x62,0x21,0x2D,0x31,MDL.IP3 };
	static byte gwip[] = { MDL.IP0, MDL.IP1,MDL.IP2, 1 };
	static byte myDNS[] = { 8, 8, 8, 8 };
	static byte mask[] = { 255, 255, 255, 0 };

	// update from saved data
	DestinationIP[0] = MDL.IP0;
	DestinationIP[1] = MDL.IP1;
	DestinationIP[2] = MDL.IP2;

	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println("Ethernet controller found.");
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
		ether.printIp("IP Address:     ", ether.myip);
		Serial.println("");
		Serial.println("Serial data disabled.");

		//register sub for received data
		//ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
	}
	else
	{
		Serial.println("");
		Serial.println("Ethernet controller not found.");
	}

	Serial.println("");
	Serial.println("Finished Setup.");
}

void loop()
{
	if (millis() - LastTime > 250)
	{
		LastTime = millis();
		SendData();
	}

	ReceiveSerial();

	if (ENCfound)
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
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