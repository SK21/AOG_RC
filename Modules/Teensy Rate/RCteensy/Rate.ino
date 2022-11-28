volatile unsigned long Duration[MaxFlowSensorCount];
volatile unsigned long PulseCount[MaxFlowSensorCount];
unsigned long LastPulse[MaxFlowSensorCount];

unsigned long TimedCounts[MaxFlowSensorCount];
unsigned long RateInterval[MaxFlowSensorCount];
unsigned long RateTimeLast[MaxFlowSensorCount];

unsigned long CurrentCount;
unsigned long CurrentDuration;

unsigned long PPM[MaxFlowSensorCount];		// pulse per minute * 100
unsigned long Osum[MaxFlowSensorCount];
unsigned long Omax[MaxFlowSensorCount];
unsigned long Omin[MaxFlowSensorCount];
byte Ocount[MaxFlowSensorCount];
float Oave[MaxFlowSensorCount];
unsigned long Omax2[MaxFlowSensorCount];
unsigned long Omin2[MaxFlowSensorCount];

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

void ISR1()
{
	static unsigned long PulseTime;
	if (millis() - PulseTime > 10)
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

			if (Sensor[i].UseMultiPulses)
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
			Sensor[i].TotalPulses += CurrentCount;
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
		if (Sensor[i].MeterCal > 0)
		{
			Sensor[i].UPM = Oave[i] / Sensor[i].MeterCal;
		}
		else
		{
			Sensor[i].UPM = 0;
		}
	}
}


