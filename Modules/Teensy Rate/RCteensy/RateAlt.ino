
const uint16_t Debounce_Alt = 250;		// micros
const uint32_t TimedLimit = 500000;	// # of micros to average counts

volatile uint32_t Duration_Alt[MaxProductCount];
volatile byte PulseCount_Alt[MaxProductCount];
uint32_t CurrentCount;
double CurrentDuration_Alt;
uint16_t TimedCounts[MaxProductCount];
uint32_t CountsTime[MaxProductCount];
uint32_t CountsInterval;

double Hz_Alt[MaxProductCount];
double Osum_Alt[MaxProductCount];
double Omax_Alt[MaxProductCount];
double Omin_Alt[MaxProductCount];
double Oave_Alt[MaxProductCount];
byte Ocount_Alt[MaxProductCount];
uint32_t LastPulse_Alt[MaxProductCount];

void ISR0_Alt ()
{
	static uint32_t PulseTime;
	if (micros() - PulseTime > Debounce_Alt)
	{
		Duration_Alt[0] = micros() - PulseTime;
		PulseTime = micros();
		PulseCount_Alt[0]++;
	}
}

void ISR1_Alt()
{
	static uint32_t PulseTime;
	if (micros() - PulseTime > Debounce_Alt)
	{
		Duration_Alt[1] = micros() - PulseTime;
		PulseTime = micros();
		PulseCount_Alt[1]++;
	}
}

void GetUPM_Alt()
{
	for (int ID = 0; ID < MDL.SensorCount; ID++)
	{
		if (PulseCount_Alt[ID])
		{
			noInterrupts();
			CurrentCount = PulseCount_Alt[ID];
			PulseCount_Alt[ID] = 0;
			CurrentDuration_Alt = Duration_Alt[ID];
			interrupts();

			Sensor[ID].TotalPulses += CurrentCount;

			if (Sensor[ID].UseMultiPulses)
			{
				TimedCounts[ID] += CurrentCount;
				CountsInterval = micros() - CountsTime[ID];
				if (CountsInterval > TimedLimit)
				{
					CountsTime[ID] = micros();
					CurrentDuration_Alt = (double)(CountsInterval / TimedCounts[ID]);
					TimedCounts[ID] = 0;
				}
			}

			if (CurrentDuration_Alt == 0)
			{
				Hz_Alt[ID] = 0;
			}
			else
			{
				Hz_Alt[ID] = (double)(1000000.0 / CurrentDuration_Alt);
			}

			LastPulse_Alt[ID] = millis();
		}

		// check for no flow
		if (millis() - LastPulse_Alt[ID] > 4000)
		{
			Hz_Alt[ID] = 0;
			Osum_Alt[ID] = 0;
			Oave_Alt[ID] = 0;
			Ocount_Alt[ID] = 0;
		}

		// olympic average
		Osum_Alt[ID] += Hz_Alt[ID];
		if (Omax_Alt[ID] < Hz_Alt[ID]) Omax_Alt[ID] = Hz_Alt[ID];
		if (Omin_Alt[ID] > Hz_Alt[ID]) Omin_Alt[ID] = Hz_Alt[ID];

		Ocount_Alt[ID]++;
		if (Ocount_Alt[ID] > 4)
		{
			Osum_Alt[ID] -= Omax_Alt[ID];
			Osum_Alt[ID] -= Omin_Alt[ID];
			Oave_Alt[ID] = Osum_Alt[ID] / 3.0;	// divide by 3 samples 
			Osum_Alt[ID] = 0;
			Omax_Alt[ID] = 0;
			Omin_Alt[ID] = 5000000.0;
			Ocount_Alt[ID] = 0;
		}

		// units per minute
		if (Sensor[ID].MeterCal > 0)
		{
			Sensor[ID].UPM = (Oave_Alt[ID] * 60.0) / Sensor[ID].MeterCal;
		}
		else
		{
			Sensor[ID].UPM = 0;
		}
	}
}
