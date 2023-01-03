volatile unsigned long Duration[MaxProductCount];
volatile unsigned long PulseCount[MaxProductCount];
uint32_t LastPulse[MaxProductCount];

unsigned long TimedCounts[MaxProductCount];
uint32_t RateInterval[MaxProductCount];
uint32_t RateTimeLast[MaxProductCount];
uint32_t PWMTimeLast[MaxProductCount];

unsigned long CurrentCount;
uint32_t CurrentDuration;

unsigned long PPM[MaxProductCount];		// pulse per minute * 100
unsigned long Osum[MaxProductCount];
unsigned long Omax[MaxProductCount];
unsigned long Omin[MaxProductCount];
byte Ocount[MaxProductCount];
float Oave[MaxProductCount];
unsigned long Omax2[MaxProductCount];
unsigned long Omin2[MaxProductCount];

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
	for (int i = 0; i < MDL.ProductCount; i++)
	{
		if (Sensor[i].ControlType == 3)
		{
			// use weight
			Sensor[i].UPM = Sensor[i].MeterCal * (double)Sensor[i].pwmSetting;
		}
		else
		{
			// use flow meter
			GetUPMflow(i);
		}
	}
}

void GetUPMflow(int ID)
{
	if (PulseCount[ID])
	{
		noInterrupts();
		CurrentCount = PulseCount[ID];
		PulseCount[ID] = 0;
		CurrentDuration = Duration[ID];
		interrupts();

		if (Sensor[ID].UseMultiPulses)
		{
			// low ms/pulse, use pulses over time
			TimedCounts[ID] += CurrentCount;
			RateInterval[ID] = millis() - RateTimeLast[ID];
			if (RateInterval[ID] > 500)
			{
				RateTimeLast[ID] = millis();
				PPM[ID] = (6000000 * TimedCounts[ID]) / RateInterval[ID];	// 100 X actual
				TimedCounts[ID] = 0;
			}
		}
		else
		{
			// high ms/pulse, use time for one pulse
			if (CurrentDuration == 0)
			{
				PPM[ID] = 0;
			}
			else
			{
				PPM[ID] = 6000000 / CurrentDuration;	// 100 X actual
			}
		}


		LastPulse[ID] = millis();
		Sensor[ID].TotalPulses += CurrentCount;
	}

	if (millis() - LastPulse[ID] > 4000)	PPM[ID] = 0;	// check for no flow

	// double olympic average
	Osum[ID] += PPM[ID];
	if (Omax[ID] < PPM[ID])
	{
		Omax2[ID] = Omax[ID];
		Omax[ID] = PPM[ID];
	}
	else if (Omax2[ID] < PPM[ID]) Omax2[ID] = PPM[ID];

	if (Omin[ID] > PPM[ID])
	{
		Omin2[ID] = Omin[ID];
		Omin[ID] = PPM[ID];
	}
	else if (Omin2[ID] > PPM[ID]) Omin2[ID] = PPM[ID];

	Ocount[ID]++;
	if (Ocount[ID] > 9)
	{
		Osum[ID] -= Omax[ID];
		Osum[ID] -= Omin[ID];
		Osum[ID] -= Omax2[ID];
		Osum[ID] -= Omin2[ID];
		Oave[ID] = (float)Osum[ID] / 600.0;	// divide by 6 samples and divide by 100 for decimal place
		Osum[ID] = 0;
		Omax[ID] = 0;
		Omin[ID] = 5000000;
		Omax2[ID] = 0;
		Omin2[ID] = 5000000;
		Ocount[ID] = 0;
	}

	// units per minute
	if (Sensor[ID].MeterCal > 0)
	{
		Sensor[ID].UPM = Oave[ID] / Sensor[ID].MeterCal;
	}
	else
	{
		Sensor[ID].UPM = 0;
	}
}


