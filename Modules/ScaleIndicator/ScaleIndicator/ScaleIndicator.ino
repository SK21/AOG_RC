
#include <SoftwareSerial.h>

SoftwareSerial mySerial(10, 11); // RX, TX
#define BufferSize 50

char inChar;
char inData[BufferSize];
byte inPointer = 0;
uint32_t LoopTime;

void setup()
{
	// Open serial communications and wait for port to open :
	Serial.begin(57600);
	while (!Serial) {
		; // wait for serial port to connect. Needed for native USB port only
	}
	Serial.println("Start of Programm");

	// set the data rate for the SoftwareSerial port
	mySerial.begin(9600);

	//Set Buffer, PGN32296
	inData[0] = 40;
	inData[1] = 126;
	inPointer = 2;
}

void loop()
{
	// run over and over
	if (mySerial.available())
	{
		inChar = (char)mySerial.read();
		if (inChar == '.') inChar = ',';
		switch (inChar)
		{   //  Only Numbers & . & , 
		case '0':
		case '1':
		case '2':
		case '3':
		case '4':
		case '5':
		case '6':
		case '7':
		case '8':
		case '9':
		case ',':
			if (inPointer < (BufferSize - 2))
			{
				inData[inPointer++] = inChar;
			}
			break;

		case 13:        // now we are ready
			inData[inPointer++] = 13;  // add <CR>
			inData[inPointer++] = 10;  // add <NL>
			Serial.write(inData, inPointer);               // Send to USB
			inData[0] = 40;
			inData[1] = 126;
			inPointer = 2;
			break;
		}
	}

	if (millis() - LoopTime > 1000)
	{
		LoopTime = millis();
		inData[0] = 40;
		inData[1] = 126;
		inData[2] = '1';
		inData[3] = '5';
		inData[4] = '4';
		inData[5] = '.';
		inData[6] = '2';
		inData[7] = 13;
		inData[8] = 10;
		Serial.write(inData, 9);
	}
}
