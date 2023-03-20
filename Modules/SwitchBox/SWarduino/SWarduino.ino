#include <EtherCard.h>
#include <EEPROM.h>
#include <SPI.h>

// Nano board for rate control switches
# define InoDescription "SWarduino  :  20-Mar-2023"
const int16_t InoID = 2003;	// change to send defaults to eeprom
int16_t StoredID;			// Defaults ID stored in eeprom	

struct ModuleConfig
{
	// SW2 pcb
	uint8_t	Auto = A5;
	uint8_t MasterOn = 5;
	uint8_t	MasterOff = 3;
	uint8_t	RateUp = A3;
	uint8_t RateDown = A2;
	uint8_t	IPpart3 = 1;			// IP address, 3rd octet
	uint8_t PinIDs[16];
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

//PGN 32618 to send data back
byte Pins[5];
byte Packet[30];

unsigned long LastTime;
byte MSB;
byte LSB;
bool PGN32627Found;
uint16_t PGN;

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
	Serial.println("Starting Ethernet ...");
	// ethernet interface ip address
	byte ArduinoIP[] = { 192,168,MDL.IPpart3,188 };

	// ethernet interface Mac address
	byte LocalMac[] = { 0x70,0x62,0x21,0x2D,0x31,188 };

	// gateway ip address
	byte gwip[] = { 192,168,MDL.IPpart3,1 };

	//DNS- you just need one anyway
	static byte myDNS[] = { 8,8,8,8 };

	//mask
	static byte mask[] = { 255,255,255,0 };

	DestinationIP[2] = MDL.IPpart3;

	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println("Ethernet controller found.");
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
		ether.printIp("IP Address:     ", ether.myip);

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

		Packet[0] = 106;
		Packet[1] = 127;

		// read switches
		Pins[0] = !digitalRead(MDL.Auto);
		Pins[1] = !digitalRead(MDL.MasterOn);
		Pins[2] = !digitalRead(MDL.MasterOff);
		Pins[3] = !digitalRead(MDL.RateUp);
		Pins[4] = !digitalRead(MDL.RateDown);

		Packet[2] = 0;
		for (int i = 0; i < 5; i++)
		{
			if (Pins[i]) Packet[2] = Packet[2] | (1 << i);
		}


		Packet[3] = 0;
		Packet[4] = 0;
		for (int i = 0; i < 16; i++)
		{
			if (MDL.PinIDs[i] > 0)
			{
				if (i < 8)
				{
					if (!digitalRead(MDL.PinIDs[i])) Packet[3] = Packet[3] | (1 << i);
				}
				else
				{
					if (!digitalRead(MDL.PinIDs[i])) Packet[4] = Packet[4] | (1 << (i - 8));
				}
			}
		}

		Packet[5] = CRC(5, 0);

		// PGN 32618
		if (ENCfound)
		{
			// send UDP
			ether.sendUdp(Packet, 6, SourcePort, DestinationIP, DestinationPort);
		}

		// send serial
		for (int i = 0; i < 6; i++)
		{
			Serial.print(Packet[i]);
			if (i < 5) Serial.print(",");
		}

		Serial.println();
		Serial.flush();
	}

	ReceiveSerial();

	if (ENCfound)
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}
}

void ReceiveSerial()
{
	// pins config
	if (Serial.available())
	{
		if (PGN32627Found)
		{
			if (Serial.available() > 22)
			{
				PGN32627Found = false;
				Packet[0] = 115;
				Packet[1] = 127;
				for (int i = 2; i < 25; i++)
				{
					Packet[i] = Serial.read();
				}

				if (GoodCRC(25))
				{
					MDL.Auto = Packet[2];
					MDL.MasterOn = Packet[3];
					MDL.MasterOff = Packet[4];
					MDL.RateUp = Packet[5];
					MDL.RateDown = Packet[6];
					MDL.IPpart3 = Packet[7];

					for (int i = 8; i < 24; i++)
					{
						MDL.PinIDs[i - 8] = Packet[i];
					}
					EEPROM.put(10, MDL);
					resetFunc();
				}
			}
		}
		else
		{
			MSB = Serial.read();
			PGN = MSB << 8 | LSB;
			LSB = MSB;

			PGN32627Found = (PGN == 32627);
		}
	}
}

bool GoodCRC(uint16_t Length)
{
	byte ck = CRC(Length - 1, 0);
	bool Result = (ck == Packet[Length - 1]);
	return Result;
}

byte CRC(uint16_t Length, byte Start)
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
