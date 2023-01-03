
uint8_t CurrentPin = 0;
uint16_t Aread;
elapsedMicros ReadTime;
uint32_t Analogtime;

void ReadAnalog()
{
	// use ADS1115 through Teensy
	if (ADSfound && (millis() - Analogtime > 2) && MDL.AnalogMethod == 0)
	{
		Analogtime = millis();
		// based on https://github.com/RalphBacon/ADS1115-ADC/blob/master/ADS1115_ADC_16_bit_SingleEnded.ino

		// read current value
		Wire.beginTransmission(ADS1115_Address);
		Wire.write(0b00000000); //Point to Conversion register
		Wire.endTransmission();
		Wire.requestFrom(ADS1115_Address, 2);
		Aread = (Wire.read() << 8 | Wire.read());
		Aread = Aread >> 1;

		switch (CurrentPin)
		{
		case 0:
			AINs.AIN0 = Aread;
			break;
		case 1:
			AINs.AIN1 = Aread;
			break;
		case 2:
			AINs.AIN2 = Aread;
			break;
		default:
			AINs.AIN3 = Aread;
			break;
		}


		// do next conversion
		Wire.beginTransmission(ADS1115_Address);
		Wire.write(0b00000001); // Point to Config Register

		// Write the MSB + LSB of Config Register
		// MSB: Bits 15:8
		// Bit  15    0=No effect, 1=Begin Single Conversion (in power down mode)
		// Bits 14:12   How to configure A0 to A3 (comparator or single ended)
		// Bits 11:9  Programmable Gain 000=6.144v 001=4.096v 010=2.048v .... 111=0.256v
		// Bits 8     0=Continuous conversion mode, 1=Power down single shot

		CurrentPin++;
		if (CurrentPin > 2) CurrentPin = 0;	// AIN3 is not used
		switch (CurrentPin)
		{
			// single ended
		case 0:
			Wire.write(0b01000000);	// AIN0
			break;
		case 1:
			Wire.write(0b01010000);	// AIN1
			break;
		case 2:
			Wire.write(0b01100000);	// AIN2
			break;
		default:
			Wire.write(0b01110000);	// AIN3
			break;
		}

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
	}
}
