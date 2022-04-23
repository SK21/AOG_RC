
#include <EtherCard.h>
#include <EEPROM.h> 

# define InoDescription "SWarduino  :  22-Apr-2022"
// Nano board for rate control switches

#define UseEthernet 0

struct PCBconfig
{
	uint8_t	SW0 = A4;
	uint8_t	SW1 = 9;
	uint8_t	SW2 = 6;
	uint8_t	SW3 = 4;
	uint8_t	Auto = A5;
	uint8_t MasterOn = 5;
	uint8_t	MasterOff = 3;
	uint8_t	RateUp = A3;
	uint8_t RateDown = A2;
};

PCBconfig PCB;

#if UseEthernet
	// ethernet interface ip address
	static byte ArduinoIP[] = { 192,168,5,188 };

	// ethernet interface Mac address
	static byte LocalMac[] = { 0x70,0x62,0x21,0x2D,0x31,188 };

	// gateway ip address
	static byte gwip[] = { 192,168,5,1 };
	//DNS- you just need one anyway
	static byte myDNS[] = { 8,8,8,8 };
	//mask
	static byte mask[] = { 255,255,255,0 };

	// local ports on Arduino
	unsigned int SourcePort = 6200;		// to send from 

	// ethernet destination - Rate Controller
	static byte DestinationIP[] = { 192, 168, 5, 255 };	// broadcast 255
	unsigned int DestinationPort = 29999; // Rate Controller listening port 

	byte Ethernet::buffer[500]; // udp send and receive buffer
#endif

//PGN 32618 to send data back 
byte toSend[] = { 106,127,0,0,0 };
byte Pins[] = { 0,0,0,0,0,0,0,0,0 };

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
	
	// pcb data
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

	pinMode(PCB.SW0, INPUT_PULLUP);
	pinMode(PCB.SW1, INPUT_PULLUP);
	pinMode(PCB.SW2, INPUT_PULLUP);

	pinMode(PCB.SW3, INPUT_PULLUP);
	pinMode(PCB.Auto, INPUT_PULLUP);
	pinMode(PCB.MasterOn, INPUT_PULLUP);

	pinMode(PCB.MasterOff, INPUT_PULLUP);
	pinMode(PCB.RateUp, INPUT_PULLUP);
	pinMode(PCB.RateDown, INPUT_PULLUP);

#if UseEthernet
	Serial.println("Starting Ethernet ...");
	if (ether.begin(sizeof Ethernet::buffer, LocalMac, 10) == 0)
		Serial.println(F("Failed to access Ethernet controller"));
#endif

	Serial.println("");
	Serial.println("Finished Setup.");
}

void loop()
{
	if (millis() - LastTime > 250)
	{
		LastTime = millis();

		// read switches
		// toSend[2]
		Pins[0] = !digitalRead(PCB.Auto);			
		Pins[1] = !digitalRead(PCB.MasterOn);		
		Pins[2] = !digitalRead(PCB.MasterOff);	
		Pins[3] = !digitalRead(PCB.RateUp);		
		Pins[4] = !digitalRead(PCB.RateDown);		

		// toSend[3]
		Pins[5] = !digitalRead(PCB.SW0);			
		Pins[6] = !digitalRead(PCB.SW1);			
		Pins[7] = !digitalRead(PCB.SW2);			
		Pins[8] = !digitalRead(PCB.SW3);			


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
#if UseEthernet
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

	ReceiveSerial();

#if UseEthernet
	//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
#endif
}

void ReceiveSerial()
{
	// pins config
	if (Serial.available() > 0)
	{
		if (PGN32627Found)
		{
			if (Serial.available() > 8)
			{
				PGN32627Found = false;
				PCB.SW0 = Serial.read();
				PCB.SW1 = Serial.read();
				PCB.SW2 = Serial.read();
				PCB.SW3 = Serial.read();
				PCB.Auto = Serial.read();
				PCB.MasterOn = Serial.read();
				PCB.MasterOff = Serial.read();
				PCB.RateUp = Serial.read();
				PCB.RateDown = Serial.read();

				EEPROM.put(10, PCB);
				resetFunc();
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
