#if(UseRateControl)
volatile unsigned long Duration[SensorCount];
volatile unsigned long PulseCount[SensorCount];
float PPM[SensorCount];
unsigned long LastPulse[SensorCount];

float KalResult[SensorCount];
float KalPC[SensorCount];
float KalG[SensorCount];
float KalP[] = { 1.0,1.0,1.0,1.0,1.0 };
float KalVariance[] = { 0.01,0.01,0.01,0.01,0.01 };
float KalProcess[] = { 0.005,0.005,0.005,0.005,0.005 };

unsigned long TimedCounts[SensorCount];
unsigned long RateInterval[SensorCount];
unsigned long RateTimeLast[SensorCount];

unsigned long CurrentCount;
unsigned long CurrentDuration;

void ISR0()
{
	static unsigned long PulseTime;
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
					PPM[i] = (60000.0 * (float)TimedCounts[i]) / (float)RateInterval[i];
					TimedCounts[i] = 0;
				}
			}
			else
			{
				// high ms/pulse, use time for one pulse
				PPM[i] = 60000.0 / (float)CurrentDuration;
			}

			LastPulse[i] = millis();
			TotalPulses[i] += CurrentCount;
		}

		if (millis() - LastPulse[i] > 4000)	PPM[i] = 0;	// check for no flow

		// Kalmen filter
		KalPC[i] = KalP[i] + KalProcess[i];
		KalG[i] = KalPC[i] / (KalPC[i] + KalVariance[i]);
		KalP[i] = (1.0 - KalG[i]) * KalPC[i];
		KalResult[i] = KalG[i] * (PPM[i] - KalResult[i]) + KalResult[i];
		PPM[i] = KalResult[i];

		// units per minute
		if (MeterCal[i] > 0)
		{
			UPM[i] = PPM[i] / MeterCal[i];
		}
		else
		{
			UPM[i] = 0;
		}
	}
}
#endif

