
volatile uint32_t Duration[MaxFlowSensorCount];
volatile uint32_t PulseCount[MaxFlowSensorCount];
uint32_t LastPulse[MaxFlowSensorCount];

uint32_t TimedCounts[MaxFlowSensorCount];
uint32_t RateInterval[MaxFlowSensorCount];
uint32_t RateTimeLast[MaxFlowSensorCount];

uint32_t CurrentCount;
uint32_t CurrentDuration;

uint32_t PPM[MaxFlowSensorCount];		// pulse per minute * 100
uint32_t Osum[MaxFlowSensorCount];
uint32_t Omax[MaxFlowSensorCount];
uint32_t Omin[MaxFlowSensorCount];
byte Ocount[MaxFlowSensorCount];
float Oave[MaxFlowSensorCount];

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
		if (PPM[i] > Omax[i]) Omax[i] = PPM[i];
		if (PPM[i] < Omin[i]) Omin[i] = PPM[i];

		Ocount[i]++;
		if (Ocount[i] > 4)
		{
			Osum[i] -= Omax[i];
			Osum[i] -= Omin[i];
			Oave[i] = (float)Osum[i] / 300.0;	// divide by 3 and divide by 100 
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

