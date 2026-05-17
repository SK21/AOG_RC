
void ReadAnalog()
{
	static int16_t Aread;
	static bool ConversionPending = false;

	if (ADSfound)
	{
		// use ADS1115
		//	AS15 config
		//	AIN0	pressure
		//	AIN1	
		//	AIN2	
		//	AIN3	
		// Only do one of either a read or a request per loop. Saves loop time and
		// doesn't affect ADC read time that much.

		if (ConversionPending)
		{
			// read value if available
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0b00000000); //Point to Conversion register
			Wire.endTransmission();
			if (Wire.requestFrom(ADS1115_Address, 2) == 2)
			{
				Aread = (int16_t)(Wire.read() << 8 | Wire.read());
				if (Aread < 0) Aread = 0;
				PressureReading = (uint16_t)((uint16_t)Aread >> 1);
				ConversionPending = false;
			}
		}
		else
		{
			// start new read
			Wire.beginTransmission(ADS1115_Address);
			Wire.write(0b00000001); // Point to Config Register

			// Write the MSB + LSB of Config Register
			// MSB: Bits 15:8
			// Bit  15    0=No effect, 1=Begin Single Conversion (in power down mode)
			// Bits 14:12   How to configure A0 to A3 (comparator or single ended)
			// Bits 11:9  Programmable Gain 000=6.144v 001=4.096v 010=2.048v .... 111=0.256v
			// Bits 8     0=Continuous conversion mode, 1=Power down single shot

			Wire.write(0b11000001);	// AIN0

			// LSB: Bits 7:0
			// Bits 7:5 Data Rate (Samples per second) 000=8, 001=16, 010=32, 011=64,
			//      100=128, 101=250, 110=475, 111=860
			// Bit  4   Comparator Mode 0=Traditional, 1=Window
			// Bit  3   Comparator Polarity 0=low, 1=high
			// Bit  2   Latching 0=No, 1=Yes
			// Bits 1:0 Comparator # before Alert pin goes high
			//      00=1, 01=2, 10=4, 11=Disable this feature
			Wire.write(0b11100011);	//860 samples/sec
			Wire.endTransmission();

			ConversionPending = true;
		}
	}
	else
	{
		// use Teensy analog pins
		if (MDL.PressurePin < NC) PressureReading = (uint16_t)analogRead(MDL.PressurePin);
	}
}




