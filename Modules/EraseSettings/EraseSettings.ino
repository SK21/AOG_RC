// EraseSettings - testing tool for the Teensy 4.1 and Nano rate modules.
// Sets the whole EEPROM to 0xFF (blank), then blinks the LED fast.
// Flash the rate firmware again afterwards: the board starts as new
// (ID 25, default settings, no label; a Nano also makes a new MAC).
#include <EEPROM.h>

void setup()
{
	pinMode(LED_BUILTIN, OUTPUT);
	Serial.begin(38400);
	for (int i = 0; i < (int)EEPROM.length(); i++)
	{
		if (EEPROM.read(i) != 0xFF) EEPROM.write(i, 0xFF);   // skip cells already blank
	}
	Serial.println("EEPROM erased. Flash the rate firmware.");
}

void loop()
{
	digitalWrite(LED_BUILTIN, !digitalRead(LED_BUILTIN));
	delay(250);
}
