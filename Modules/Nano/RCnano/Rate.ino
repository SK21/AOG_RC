volatile unsigned long Duration[2];
volatile byte PulseCount[2];
unsigned long LastPulse[2];

unsigned long TimedCounts[2];
unsigned long RateInterval[2];
unsigned long RateTimeLast[2];

unsigned long CurrentCount;
unsigned long CurrentDuration;

unsigned long PPM[2];		// pulse per minute * 100
unsigned long Osum[2];
unsigned long Omax[2];
unsigned long Omin[2];
byte Ocount[2];
float Oave[2];

void ISR0()
{
	static unsigned long PulseTime;
	if (millis() - PulseTime > MDL.Debounce)
	{
		Duration[0] = millis() - PulseTime;
		PulseTime = millis();
		PulseCount[0]++;
	}
}

void ISR1()
{
	static unsigned long PulseTime;
	if (millis() - PulseTime > MDL.Debounce)
	{
		Duration[1] = millis() - PulseTime;
		PulseTime = millis();
		PulseCount[1]++;
	}
}

void GetUPM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (PulseCount[i])
		{
			noInterrupts();
			CurrentCount = PulseCount[i];
			PulseCount[i] = 0;
			CurrentDuration = Duration[i];
			interrupts();

			if (UseMultiPulses[i])
			{
				// low ms/pulse, use pulses over time
				TimedCounts[i] += CurrentCount;
				RateInterval[i] = millis() - RateTimeLast[i];
				if (RateInterval[i] > 500)
				{
					RateTimeLast[i] = millis();
					PPM[i] = (6000000 * TimedCounts[i]) / RateInterval[i];	// 100 X actual
					TimedCounts[i] = 0;
				}
			}
			else
			{
				// high ms/pulse, use time for one pulse
				if (CurrentDuration == 0)
				{
					PPM[i] = 0;
				}
				else
				{
					PPM[i] = 6000000 / CurrentDuration;	// 100 X actual
				}
			}


			LastPulse[i] = millis();
			TotalPulses[i] += CurrentCount;
		}

		if (millis() - LastPulse[i] > 4000)	PPM[i] = 0;	// check for no flow

		// olympic average
		Osum[i] += PPM[i];
		if (Omax[i] < PPM[i]) Omax[i] = PPM[i];
		if (Omin[i] > PPM[i]) Omin[i] = PPM[i];

		Ocount[i]++;
		if (Ocount[i] > 4)
		{
			Osum[i] -= Omax[i];
			Osum[i] -= Omin[i];
			Oave[i] = (float)Osum[i] / 300.0;	// divide by 3 and divide by 100 
			Osum[i] = 0;
			Omax[i] = 0;
			Omin[i] = 5000000;
			Ocount[i] = 0;
		}

		// units per minute
		if (MeterCal[i] > 0)
		{
			UPM[i] = Oave[i] / MeterCal[i];
		}
		else
		{
			UPM[i] = 0;
		}
	}
}

