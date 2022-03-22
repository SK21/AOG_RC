# define InoDescription "SWarduino  :  28-Jan-2022"
// to control switches on pcb SW1 or pcb SW2
// Nano board

#include <EtherCard.h>

// user settings ****************************

#define CommType 0			// 0 Serial/USB , 1 UDP wired Nano
#define IPpart3 1			// ex: 192.168.IPpart3.255, 0-255
#define IPMac 188			// unique number for Arduino IP address and Mac part 6, 0-255

// ******************************************

#if(CommType == 1)
// ethernet interface ip address
static byte ArduinoIP[] = { 192,168,IPpart3,IPMac };

// ethernet interface Mac address
static byte LocalMac[] = { 0x70,0x2D,0x31,0x21,0x62,IPMac };

// gateway ip address
static byte gwip[] = { 192,168,IPpart3,1 };
//DNS- you just need one anyway
static byte myDNS[] = { 8,8,8,8 };
//mask
static byte mask[] = { 255,255,255,0 };

// local ports on Arduino
unsigned int SourcePort = 6200;		// to send from 

// ethernet destination - Rate Controller
static byte DestinationIP[] = { 192, 168, IPpart3, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port 

byte Ethernet::buffer[500]; // udp send and receive buffer
#endif

//PGN 32618 to send data back 
byte toSend[] = { 106,127,0,0,0 };
byte Pins[] = { 0,0,0,0,0,0,0,0,0 };

// SW1 PCB
//#define SW0pin	A4
//#define SW1pin	9
//#define SW2pin	6
//
//#define SW3pin 4
//#define AutoPin 5
//#define MasterOnPin A5
//
//#define MasterOffPin 3	 
//#define RateUpPin A3
//#define RateDownPin A2

// SW2 PCB
#define SW0pin	A4
#define SW1pin	9
#define SW2pin	6

#define SW3pin 4
#define AutoPin A5
#define MasterOnPin 5

#define MasterOffPin 3	 
#define RateUpPin A3
#define RateDownPin A2

unsigned long LastTime;
bool EthernetEnabled = false;

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	pinMode(SW0pin, INPUT_PULLUP);
	pinMode(SW1pin, INPUT_PULLUP);
	pinMode(SW2pin, INPUT_PULLUP);

	pinMode(SW3pin, INPUT_PULLUP);
	pinMode(AutoPin, INPUT_PULLUP);
	pinMode(MasterOnPin, INPUT_PULLUP);

	pinMode(MasterOffPin, INPUT_PULLUP);
	pinMode(RateUpPin, INPUT_PULLUP);
	pinMode(RateDownPin, INPUT_PULLUP);

#if(CommType==1)
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
#endif

	Serial.println("Finished Setup.");
}

void loop()
{
	if (millis() - LastTime > 200)
	{
		LastTime = millis();

		// read switches
		// toSend[2]
		Pins[0] = !digitalRead(AutoPin);			
		Pins[1] = !digitalRead(MasterOnPin);		
		Pins[2] = !digitalRead(MasterOffPin);	
		Pins[3] = !digitalRead(RateUpPin);		
		Pins[4] = !digitalRead(RateDownPin);		

		// toSend[3]
		Pins[5] = !digitalRead(SW0pin);			
		Pins[6] = !digitalRead(SW1pin);			
		Pins[7] = !digitalRead(SW2pin);			
		Pins[8] = !digitalRead(SW3pin);			


		// build data
		toSend[2] = 0;
		toSend[3] = 0;

		for (int i = 0; i < 5; i++)
		{
			if (Pins[i]) toSend[2] = toSend[2] | (1 << i);
		}

		for (int i = 0; i < 4; i++)
		{
			if (Pins[i+5]) toSend[3] = toSend[3] | (1 << i);
		}

		// PGN 32618

#if(CommType==1)
		// send UDP
		ether.sendUdp(toSend, sizeof(toSend), SourcePort, DestinationIP, DestinationPort);
#endif

		// send serial
		for (int i = 0; i < 5; i++)
		{
			Serial.print(toSend[i]);
			if (i < 4) Serial.print(",");
		}
		Serial.println();
		Serial.flush();
	}
#if(CommType==1)
	//this must be called for ethercard functions to work.
	ether.packetLoop(ether.packetReceive());
#endif
}

String IPadd(byte Address[])
{
	return String(Address[0]) + "." + String(Address[1]) + "." + String(Address[2]) + "." + String(Address[3]);
}
