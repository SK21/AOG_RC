#if(UseRateControl)
volatile uint32_t Duration[SensorCount];
volatile uint32_t PulseCount[SensorCount];
uint32_t LastPulse[SensorCount];

uint32_t TimedCounts[SensorCount];
uint32_t RateInterval[SensorCount];
uint32_t RateTimeLast[SensorCount];

uint32_t CurrentCount;
uint32_t CurrentDuration;

uint32_t PPM[SensorCount];		// pulse per minute * 100
uint32_t Osum[SensorCount];
uint32_t Omax[SensorCount];
uint32_t Omin[SensorCount];
byte Ocount[SensorCount];
float Oave[SensorCount];

void ISR0()
{
	static uint32_t PulseTime;
	if (millis() - PulseTime > 10)
	{
		Duration[0] = millis() - PulseTime;
		PulseTime = millis();
		PulseCount[0]++;
	}
}

void GetUPM()
{
	for (int i = 0; i < SensorCount; i++)
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
				if (RateInterval[i] > 200)
				{
					RateTimeLast[i] = millis();
					PPM[i] = (6000000 * TimedCounts[i]) / RateInterval[i];	// 100 X actual
					TimedCounts[i] = 0;
				}
			}
			else
			{
				// high ms/pulse, use time for one pulse
				PPM[i] = 6000000 / CurrentDuration;	// 100 X actual
			}

			LastPulse[i] = millis();
			TotalPulses[i] += CurrentCount;
		}

		if (millis() - LastPulse[i] > 4000)	PPM[i] = 0;	// check for no flow


		// olympic average
		Osum[i] += PPM[i];
		if (PPM[i] > Omax[i]) Omax[i] = PPM[i];
		if (PPM[i] < Omin[i]) Omin[i] = PPM[i];

		Ocount[i]++;
		if (Ocount[i] > 9)
		{
			Osum[i] -= Omax[i];
			Osum[i] -= Omin[i];
			Oave[i] = (float)Osum[i] / 800.0;	// divide by 8 and divide by 100 
			Osum[i] = 0;
			Omax[i] = 0;
			Omin[i] = 50000;
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
#endif

