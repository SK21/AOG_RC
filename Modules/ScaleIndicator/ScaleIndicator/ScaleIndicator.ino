#include <EtherCard.h>
#include <SoftwareSerial.h>
#include <SPI.h>

// Nano + ENC28J60 scale module. Reads a scale indicator's serial output
// and sends the reading to the Rate Controller as PGN32296 over UDP.
// The indicator's RS232 needs a MAX3232 (or similar) to TTL level.
# define InoDescription "ScaleIndicator"
const uint16_t InoID = 30106;	// ddmmy, no leading 0

// scale formats
#define FormatGeneric 0		// digits only, e.g. "   12.5" CR. No units
#define FormatLP7515 1		// "ST,GS,+1234.5lb" CR LF

// settings, compiled in. Change ProductID for each scale, 0-3 (product 1-4 in the app)
#define ProductID 3
#define ScaleFormat FormatGeneric
#define ScaleBaud 9600
#define ScaleRX 3		// from indicator TX (via MAX3232). Not 10-13, the ENC28J60 uses them
#define ScaleTX 4		// not used, SoftwareSerial needs a pin

// units sent in PGN32296 byte 7
#define UnitsUnknown 0
#define UnitsLB 1
#define UnitsKG 2

// network, same subnet as the Rate Controller
byte ArduinoIP[] = { 192, 168, 1, 170 + ProductID };
byte DestinationIP[] = { 192, 168, 1, 255 };	// broadcast
unsigned int SourcePort = 6300;			// to send from
unsigned int DestinationPort = 29999;	// Rate Controller listening port

static uint8_t LocalMac[] = { 0x70,0x62,0x21,0x2D,0x32,170 + ProductID };
static byte gwip[] = { 192, 168, 1, 1 };
static byte myDNS[] = { 8, 8, 8, 8 };
static byte mask[] = { 255, 255, 255, 0 };

byte Ethernet::buffer[300]; // udp send and receive buffer
bool ENCfound;
static byte selectPin = 10;

SoftwareSerial ScaleSerial(ScaleRX, ScaleTX);

// line being received from the indicator
#define LineSize 32
char Line[LineSize];
uint8_t LineLength = 0;

void setup()
{
	Serial.begin(38400);
	delay(3000);
	Serial.println("");
	Serial.println(InoDescription);

	// version
	uint16_t yr = InoID % 10 + 2020;
	uint16_t rest = InoID / 10;
	uint8_t mn = rest % 100;
	uint16_t dy = rest / 100;

	Serial.print("Module Version: v");
	Serial.print(yr);
	Serial.print(".");
	Serial.print(mn);
	Serial.print(".");
	Serial.println(dy);

	Serial.print("Product: ");
	Serial.println(ProductID + 1);

	Serial.print("Scale format: ");
	Serial.println(ScaleFormat == FormatLP7515 ? "LP7515" : "Generic");

	ScaleSerial.begin(ScaleBaud);

	Serial.println("");
	Serial.println("Starting Ethernet ...");
	ENCfound = ShieldFound();
	if (ENCfound)
	{
		ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);
		Serial.println("");
		Serial.println("Ethernet controller found.");
		ether.staticSetup(ArduinoIP, gwip, myDNS, mask);

		delay(500);
		if (EthernetConnected())
		{
			ether.printIp("IP Address:     ", ether.myip);
		}
		else
		{
			Serial.println("Ethernet cable not connected.");
		}
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
	ReadScale();

	if (EthernetConnected())
	{
		//this must be called for ethercard functions to work.
		ether.packetLoop(ether.packetReceive());
	}
}

bool EthernetConnected()
{
	bool Result = false;
	if (ENCfound)
	{
		Result = ether.isLinkUp();
	}
	return Result;
}

void ReadScale()
{
	// one line per reading, ended by CR or LF. An empty line (the LF after a CR) is skipped.
	while (ScaleSerial.available())
	{
		char inChar = (char)ScaleSerial.read();
		if (inChar == 13 || inChar == 10)
		{
			if (LineLength > 0)
			{
				Line[LineLength] = 0;
				if (ScaleFormat == FormatLP7515)
				{
					ParseLP7515();
				}
				else
				{
					ParseGeneric();
				}
			}
			LineLength = 0;
		}
		else if (LineLength < LineSize - 1)
		{
			Line[LineLength++] = inChar;
		}
	}
}

void ParseGeneric()
{
	// digits, the decimal mark and a minus sign anywhere in the line, everything else ignored
	int32_t Hundredths;
	if (ToHundredths(Line, Hundredths))
	{
		SendReading(Hundredths, UnitsUnknown);
	}
}

void ParseLP7515()
{
	// S1,S2,S3DataS4  e.g. ST,GS,+1234.5lb
	// S1 ST stable, US unstable, OL overload. S2 GS gross, NT net. S3 sign. S4 lb or kg
	char* S1 = strtok(Line, ",");
	char* S2 = strtok(NULL, ",");
	char* Payload = strtok(NULL, ",");

	if (S1 != NULL && S2 != NULL && Payload != NULL)
	{
		uint8_t Length = strlen(Payload);
		if (Length >= 4 && (Payload[0] == '+' || Payload[0] == '-'))
		{
			uint8_t Units = UnitsUnknown;
			char U0 = tolower(Payload[Length - 2]);
			char U1 = tolower(Payload[Length - 1]);
			if (U0 == 'l' && U1 == 'b') Units = UnitsLB;
			if (U0 == 'k' && U1 == 'g') Units = UnitsKG;

			if (Units != UnitsUnknown)
			{
				Payload[Length - 2] = 0;	// drop the units
				int32_t Hundredths;
				if (ToHundredths(Payload + 1, Hundredths))
				{
					if (Payload[0] == '-') Hundredths = -Hundredths;
					SendReading(Hundredths, Units);
				}
			}
		}
	}
}

bool ToHundredths(const char* Text, int32_t& Hundredths)
{
	// a comma is taken as the decimal mark, as some indicators send one. 2 decimals kept
	int32_t Value = 0;
	uint8_t Decimals = 0;
	bool Negative = false;
	bool HasDigits = false;
	bool PastPoint = false;

	for (uint8_t i = 0; Text[i] != 0; i++)
	{
		char c = Text[i];
		if (c >= '0' && c <= '9')
		{
			HasDigits = true;
			if (!PastPoint)
			{
				if (Value < 20000000) Value = Value * 10 + (c - '0');
			}
			else if (Decimals < 2)
			{
				Value = Value * 10 + (c - '0');
				Decimals++;
			}
		}
		else if (c == '.' || c == ',')
		{
			PastPoint = true;
		}
		else if (c == '-')
		{
			Negative = true;
		}
	}

	while (Decimals < 2)
	{
		Value *= 10;
		Decimals++;
	}
	Hundredths = Negative ? -Value : Value;
	return HasDigits;
}

void SendReading(int32_t Hundredths, uint8_t Units)
{
	//PGN32296, scale indicator reading
	//0     40
	//1     126
	//2     product ID 0-3
	//3     weight Lo       signed 32 bit, hundredths of the indicator's units
	//4     weight Mid
	//5     weight Mid
	//6     weight Hi
	//7     units 0 unknown, 1 lb, 2 kg
	//8     InoID Lo
	//9     InoID Hi
	//10    CRC

	const byte PGNlength = 11;
	byte Data[PGNlength];

	Data[0] = 40;
	Data[1] = 126;
	Data[2] = ProductID;
	Data[3] = (byte)Hundredths;
	Data[4] = (byte)(Hundredths >> 8);
	Data[5] = (byte)(Hundredths >> 16);
	Data[6] = (byte)(Hundredths >> 24);
	Data[7] = Units;
	Data[8] = (byte)InoID;
	Data[9] = (byte)(InoID >> 8);
	Data[PGNlength - 1] = CRC(Data, PGNlength - 1, 0);

	if (EthernetConnected())
	{
		ether.sendUdp(Data, PGNlength, SourcePort, DestinationIP, DestinationPort);
	}
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
