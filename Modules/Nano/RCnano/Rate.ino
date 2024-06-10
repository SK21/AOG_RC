
const uint16_t Debounce = 250;		// micros
const uint32_t TimedLimit = 500000;	// # of micros to average counts

volatile uint32_t Duration[MaxProductCount];
volatile byte PulseCount[MaxProductCount];
uint32_t CurrentCount;
double CurrentDuration;
uint16_t TimedCounts[MaxProductCount];
uint32_t CountsTime[MaxProductCount];
uint32_t CountsInterval;

double Hz[MaxProductCount];		
double Osum[MaxProductCount];
double Omax[MaxProductCount];
double Omin[MaxProductCount];
double Oave[MaxProductCount];
byte Ocount[MaxProductCount];
uint32_t LastPulse[MaxProductCount];

void ISR0()
{
	static uint32_t PulseTime;
	if (micros() - PulseTime > Debounce)
	{
		Duration[0] = micros() - PulseTime;
		PulseTime = micros();
		PulseCount[0]++;
	}
}

void ISR1()
{
	static uint32_t PulseTime;
	if (micros() - PulseTime > Debounce)
	{
		Duration[1] = micros() - PulseTime;
		PulseTime = micros();
		PulseCount[1]++;
	}
}

void GetUPM()
{
	for (int ID = 0; ID < MDL.SensorCount; ID++)
	{
		if (PulseCount[ID])
		{
			noInterrupts();
			CurrentCount = PulseCount[ID];
			PulseCount[ID] = 0;
			CurrentDuration = Duration[ID];
			interrupts();

			Sensor[ID].TotalPulses += CurrentCount;

			if (Sensor[ID].UseMultiPulses)
			{
				TimedCounts[ID] += CurrentCount;
				CountsInterval = micros() - CountsTime[ID];
				if (CountsInterval > TimedLimit)
				{
					CountsTime[ID] = micros();
					CurrentDuration = (double)(CountsInterval / TimedCounts[ID]);
					TimedCounts[ID] = 0;
				}
			}

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
