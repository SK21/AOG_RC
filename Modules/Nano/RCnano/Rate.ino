volatile unsigned long Duration[MaxProductCount];
volatile unsigned long Durations[2][12];
volatile int FilledCount[2];
volatile unsigned long PulseCount[MaxProductCount];

unsigned long avDurs[2];
byte DurCount[2];
uint32_t LastPulse[MaxProductCount];
uint32_t CurrentDuration;

double Hz[MaxProductCount];		
double Osum[MaxProductCount];
double Omax[MaxProductCount];
double Omin[MaxProductCount];
double Oave[MaxProductCount];
byte Ocount[MaxProductCount];

const uint16_t Debounce = 500;	// micros
const uint16_t avgPulses = 12;

void ISR0()
{
	static unsigned long PulseTime;
	unsigned long micronow;
	unsigned long dur;

	micronow = micros();
	dur = micronow - PulseTime;

	if (dur > 2000000)
	{
		// the component was off so reset the values
		avDurs[0] = 0;
		dur = 50000;
		for (int i = 0; i < avgPulses; i++)
		{
			Durations[0][i] = 0;
		}
		DurCount[0] = 0;
		FilledCount[0] = 0;

		PulseTime = micronow;
		PulseCount[0]++;
	}
	else if (dur > Debounce)
	{
		if (avDurs[0] == 0) avDurs[0] = dur;

		// check to see if the dur value is too long like an interrupt was missed.
		if (dur > (2.5 * avDurs[0]))
		{
			dur = avDurs[0] * 0.5 + dur * 0.5;	// prevent occasionally being stuck in error loop
		}

		Duration[0] = dur;
		Durations[0][DurCount[0]] = dur;
		FilledCount[0] += (FilledCount[0] < avgPulses);

		if (DurCount[0] > 0)
		{
			avDurs[0] = ((Durations[0][DurCount[0] - 1]) + dur) / 2;
		}
		else
		{
			// use last duration to average
			avDurs[0] = ((Durations[0][avgPulses - 1]) + dur) / 2;
		}

		if (++DurCount[0] >= avgPulses) DurCount[0] = 0;

		PulseTime = micronow;
		PulseCount[0]++;
	}
}

void ISR1()
{
	static unsigned long PulseTime;
	unsigned long micronow;
	unsigned long dur;

	micronow = micros();
	dur = micronow - PulseTime;

	if (dur > 2000000)
	{
		// the component was off so reset the values
		avDurs[1] = 0;
		dur = 50000;
		for (int i = 0; i < avgPulses; i++)
		{
			Durations[1][i] = 0;
		}
		DurCount[1] = 0;
		FilledCount[1] = 0;

		PulseTime = micronow;
		PulseCount[1]++;
	}
	else if (dur > Debounce)
	{
		if (avDurs[1] == 0) avDurs[1] = dur;

		// check to see if the dur value is too long like an interrupt was missed.
		if (dur > (2.5 * avDurs[1]))
		{
			dur = avDurs[1] * 0.5 + dur * 0.5;	// prevent occasionally being stuck in error loop
		}

		Duration[1] = dur;
		Durations[1][DurCount[1]] = dur;
		FilledCount[1] += (FilledCount[1] < avgPulses);

		if (DurCount[1] > 0)
		{
			avDurs[1] = ((Durations[1][DurCount[1] - 1]) + dur) / 2;
		}
		else
		{
			// use last duration to average
			avDurs[1] = ((Durations[1][avgPulses - 1]) + dur) / 2;
		}

		if (++DurCount[1] >= avgPulses) DurCount[1] = 0;

		PulseTime = micronow;
		PulseCount[1]++;
	}
}

unsigned long GetAvgDuration(int ID)
{
	unsigned long Result = 0;
	unsigned long dursum = 0;

	noInterrupts();
	if (FilledCount[ID] > 0)
	{
		for (int i = 0; i < FilledCount[ID]; i++)
		{
			dursum += Durations[ID][i];
		}
		Result = dursum / FilledCount[ID];
	}
	interrupts();
	return Result;
}

void GetUPM()
{
	for (int ID = 0; ID < MDL.SensorCount; ID++)
	{
		if (PulseCount[ID])
		{
			noInterrupts();
			Sensor[ID].TotalPulses += PulseCount[ID];
			PulseCount[ID] = 0;
			CurrentDuration = Duration[ID];
			interrupts();

			if (Sensor[ID].UseMultiPulses) CurrentDuration = GetAvgDuration(ID);

			if (CurrentDuration == 0)
			{
				Hz[ID] = 0;
			}
			else
			{
				Hz[ID] = (double)(1000000.0 / CurrentDuration);
			}

			LastPulse[ID] = millis();
		}

		// check for no flow
		if (millis() - LastPulse[ID] > 4000)
		{
			Hz[ID] = 0;
			Osum[ID] = 0;
			Oave[ID] = 0;
			Ocount[ID] = 0;
		}

		// olympic average
		Osum[ID] += Hz[ID];
		if (Omax[ID] < Hz[ID]) Omax[ID] = Hz[ID];
		if (Omin[ID] > Hz[ID]) Omin[ID] = Hz[ID];

		Ocount[ID]++;
		if (Ocount[ID] > 4)
		{
			Osum[ID] -= Omax[ID];
			Osum[ID] -= Omin[ID];
			Oave[ID] = Osum[ID] / 3.0;	// divide by 3 samples 
			Osum[ID] = 0;
			Omax[ID] = 0;
			Omin[ID] = 5000000.0;
			Ocount[ID] = 0;
		}

		// units per minute
		if (Sensor[ID].MeterCal > 0)
		{
			Sensor[ID].UPM = (Oave[ID] * 60.0) / Sensor[ID].MeterCal;
		}
		else
		{
			Sensor[ID].UPM = 0;
		}
	}
}
