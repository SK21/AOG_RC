#include <EtherCard.h>
#include <EEPROM.h>

# define InoDescription "SWarduino  :  09-May-2022"
// Nano board for rate control switches

#define UseEthernet 0

struct PCBconfig
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

PCBconfig PCB;

#if UseEthernet
// local ports on Arduino
unsigned int ListeningPort = 28888;	// to listen on
unsigned int SourcePort = 6200;		// to send from

// ethernet destination - Rate Controller
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast 255
unsigned int DestinationPort = 29999; // Rate Controller listening port

byte Ethernet::buffer[500]; // udp send and receive buffer
bool EthernetEnabled = false;
#endif

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

//EEPROM
int16_t EEread = 0;
#define PCB_Ident 8120

void setup()
{
	Serial.begin(38400);

	delay(5000);
	Serial.println();
	Serial.println(InoDescription);
	Serial.println();

	// defaults
	PCB.PinIDs[0] = A4;
	PCB.PinIDs[1] = 9;
	PCB.PinIDs[2] = 6;
	PCB.PinIDs[3] = 4;

	for (int i = 4; i < 16; i++)
	{
		PCB.PinIDs[i] = 0;
	}

	// stored pcb data
	EEPROM.get(0, EEread);              // read identifier
	if (EEread != PCB_Ident)
	{
		EEPROM.put(0, PCB_Ident);
		EEPROM.put(10, PCB);
	}
	else
	{
		EEPROM.get(10, PCB);
	}

	pinMode(PCB.Auto, INPUT_PULLUP);
	pinMode(PCB.MasterOn, INPUT_PULLUP);
	pinMode(PCB.MasterOff, INPUT_PULLUP);
	pinMode(PCB.RateUp, INPUT_PULLUP);
	pinMode(PCB.RateDown, INPUT_PULLUP);

	for (int i = 0; i < 16; i++)
	{
		if (PCB.PinIDs[i] > 0) pinMode(PCB.PinIDs[i], INPUT_PULLUP);
	}

#if UseEthernet
	Serial.println("Starting Ethernet ...");
	// ethernet interface ip address
	byte ArduinoIP[] = { 192,168,PCB.IPpart3,188 };

	// ethernet interface Mac address
	byte LocalMac[] = { 0x70,0x62,0x21,0x2D,0x31,188 };

	// gateway ip address
	byte gwip[] = { 192,168,PCB.IPpart3,1 };

	//DNS- you just need one anyway
	static byte myDNS[] = { 8,8,8,8 };

	//mask
	static byte mask[] = { 255,255,255,0 };

	DestinationIP[2] = PCB.IPpart3;

	while (!EthernetEnabled)
	{
		EthernetEnabled = (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) != 0);
		Serial.print(".");
	}

	Serial.println("");
	Serial.println("Ethernet controller found.");
	ether.staticSetup(ArduinoIP, gwip, myDNS, mask);
	ether.printIp("IP Address:     ", ether.myip);

	//register sub for received data
	//ether.udpServerListenOnPort(&ReceiveUDPwired, ListeningPort);
#endif
	pinMode(LED_BUILTIN, OUTPUT);
	digitalWrite(LED_BUILTIN, LOW);

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
		Pins[0] = !digitalRead(PCB.Auto);
		Pins[1] = !digitalRead(PCB.MasterOn);
		Pins[2] = !digitalRead(PCB.MasterOff);
		Pins[3] = !digitalRead(PCB.RateUp);
		Pins[4] = !digitalRead(PCB.RateDown);

		Packet[2] = 0;
		for (int i = 0; i < 5; i++)
		{
			if (Pins[i]) Packet[2] = Packet[2] | (1 << i);
		}


		Packet[3] = 0;
		Packet[4] = 0;
		for (int i = 0; i < 16; i++)
		{
			if (PCB.PinIDs[i] > 0)
			{
				if (i < 8)
				{
					if (!digitalRead(PCB.PinIDs[i])) Packet[3] = Packet[3] | (1 << i);
				}
				else
				{
					if (!digitalRead(PCB.PinIDs[i])) Packet[4] = Packet[4] | (1 << (i - 8));
				}
			}
		}

		Packet[5] = CRC(5, 0);

		// PGN 32618
#if UseEthernet
			// send UDP
		ether.sendUdp(Packet, 6, SourcePort, DestinationIP, DestinationPort);
#endif

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

#if UseEthernet
	//this must be called for ethercard functions to work.
	ether.packetLoop(ether.packetReceive());
#endif
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
					PCB.Auto = Packet[2];
					PCB.MasterOn = Packet[3];
					PCB.MasterOff = Packet[4];
					PCB.RateUp = Packet[5];
					PCB.RateDown = Packet[6];
					PCB.IPpart3 = Packet[7];

					for (int i = 8; i < 24; i++)
					{
						PCB.PinIDs[i - 8] = Packet[i];
					}
					EEPROM.put(10, PCB);
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
