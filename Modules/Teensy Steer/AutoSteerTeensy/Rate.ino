
volatile uint32_t Duration[MaxFlowSensorCount];
volatile uint32_t PulseCount[MaxFlowSensorCount];
uint32_t LastPulse[MaxFlowSensorCount];

uint32_t TimedCounts[MaxFlowSensorCount];
uint32_t RateInterval[MaxFlowSensorCount];
uint32_t RateTimeLast[MaxFlowSensorCount];

uint32_t CurrentCount;
uint32_t CurrentDuration;

unsigned long PPM[2];		// pulse per minute * 100
unsigned long Osum[2];
unsigned long Omax[2];
unsigned long Omin[2];
byte Ocount[2];
float Oave[2];
unsigned long Omax2[2];
unsigned long Omin2[2];

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
	for (int i = 0; i < 1; i++)
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


		// double olympic average
		Osum[i] += PPM[i];
		if (Omax[i] < PPM[i])
		{
			Omax2[i] = Omax[i];
			Omax[i] = PPM[i];
		}
		else if (Omax2[i] < PPM[i]) Omax2[i] = PPM[i];

		if (Omin[i] > PPM[i])
		{
			Omin2[i] = Omin[i];
			Omin[i] = PPM[i];
		}
		else if (Omin2[i] > PPM[i]) Omin2[i] = PPM[i];

		Ocount[i]++;
		if (Ocount[i] > 9)
		{
			Osum[i] -= Omax[i];
			Osum[i] -= Omin[i];
			Osum[i] -= Omax2[i];
			Osum[i] -= Omin2[i];
			Oave[i] = (float)Osum[i] / 600.0;	// divide by 6 and divide by 100 
			Osum[i] = 0;
			Omax[i] = 0;
			Omin[i] = 5000000;
			Omax2[i] = 0;
			Omin2[i] = 5000000;
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

