
// Writes the ADS1115 config register to start a conversion. Retries a
// transient I2C NACK before giving up - a lost write must not be assumed
// to have succeeded, or ConversionPending gets set while the ADC is still
// idle on its old conversion.
static bool WriteADSConfig(byte hi, byte lo)
{
	for (uint8_t attempt = 0; attempt < 3; attempt++)
	{
		Wire.beginTransmission(ADS1115_Address);
		Wire.write(0b00000001); // Point to Config Register
		Wire.write(hi);
		Wire.write(lo);
		if (Wire.endTransmission() == 0) return true;
	}
	return false;
}

// ADS1115 is only trustworthy if it was found at boot AND has produced a
// conversion recently. A wedged I2C bus stops ReadAnalog() from completing
// a read without ever clearing ADSfound, which would otherwise let
// PressureReading freeze indefinitely and go unnoticed by CheckPressure().
const uint32_t ADS_STALE_MS = 1000;
bool ADSFresh()
{
	return ADSfound && (millis() - LastADSReadMs) < ADS_STALE_MS;
}

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
				uint8_t hiByte = Wire.read();
				uint8_t loByte = Wire.read();
				Aread = (int16_t)((hiByte << 8) | loByte);
				if (Aread < 0) Aread = 0;
				PressureReading = (uint16_t)((uint16_t)Aread >> 1);
				ConversionPending = false;
				LastADSReadMs = millis();
			}
		}
		else
		{
			// start new read
			// Config register, MSB then LSB:
			// MSB Bit  15    0=No effect, 1=Begin Single Conversion (in power down mode)
			// MSB Bits 14:12 How to configure A0 to A3 (comparator or single ended)
			// MSB Bits 11:9  Programmable Gain 000=6.144v 001=4.096v 010=2.048v .... 111=0.256v
			// MSB Bit  8     0=Continuous conversion mode, 1=Power down single shot
			// LSB Bits 7:5   Data Rate (Samples per second) 000=8, 001=16, 010=32, 011=64,
			//                100=128, 101=250, 110=475, 111=860
			// LSB Bit  4     Comparator Mode 0=Traditional, 1=Window
			// LSB Bit  3     Comparator Polarity 0=low, 1=high
			// LSB Bit  2     Latching 0=No, 1=Yes
			// LSB Bits 1:0   Comparator # before Alert pin goes high 00=1, 01=2, 10=4, 11=Disable
			if (WriteADSConfig(0b11000001, 0b11100011))	// AIN0, 860 samples/sec
			{
				ConversionPending = true;
			}
		}

		// Fail-open on staleness, same convention as ADSfound==false below:
		// a wedged/unreachable sensor means pressure is unknown, so the
		// over-pressure gate must not be allowed to trip on a frozen value.
		if (!ADSFresh()) PressureReading = 0;
	}
	else
	{
		// use Teensy analog pins
		if (MDL.PressurePin < NC) PressureReading = (uint16_t)analogRead(MDL.PressurePin);
	}
}




