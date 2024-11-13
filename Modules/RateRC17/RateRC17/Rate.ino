
const uint16_t Debounce = 250;		// micros
const uint32_t TimedLimit = 250000;	// # of micros to average counts, 1/4 of a second

volatile byte PulseCount;
double CurrentDuration;
uint16_t TimedCounts;
uint32_t CountsTime;
uint32_t CountsInterval;
uint32_t LastPulse;

double Hz;
double Osum;
double Omax;
double Omin;
double Oave;
byte Ocount;

void IRAM_ATTR ISR0()
{
	static uint32_t PulseTime;
	if (micros() - PulseTime > Debounce)
	{
		PulseTime = micros();
		PulseCount++;
	}
}

void GetUPM()
{
	if (PulseCount)
	{
		noInterrupts();
		FlowSensor.TotalPulses += PulseCount;
		TimedCounts += PulseCount;
		PulseCount = 0;
		interrupts();

		CountsInterval = micros() - CountsTime;
		if (CountsInterval > TimedLimit)
		{
			CountsTime = micros();
			if (TimedCounts > 0)
			{
				CurrentDuration = (double)(CountsInterval / TimedCounts);
			}
			else
			{
				CurrentDuration = 0;
			}
			TimedCounts = 0;
		}

		if (CurrentDuration > 0)
		{
			Hz = (double)(1000000.0 / CurrentDuration);
		}
		else
		{
			Hz = 0;
		}

		LastPulse = millis();
	}

	// check for no flow
	if (millis() - LastPulse > 4000)
	{
		Hz = 0;
		Osum = 0;
		Oave = 0;
		Ocount = 0;
	}

	// olympic average
	Osum += Hz;
	if (Omax < Hz) Omax = Hz;
	if (Omin > Hz) Omin = Hz;

	Ocount++;
	if (Ocount > 4)
	{
		Osum -= Omax;
		Osum -= Omin;
		Oave = Osum / 3.0;	// divide by 3 samples 
		Osum = 0;
		Omax = 0;
		Omin = 5000000.0;
		Ocount = 0;
	}

	// units per minute
	if (FlowSensor.MeterCal > 0)
	{
		FlowSensor.UPM = (Oave * 60.0) / FlowSensor.MeterCal;
	}
	else
	{
		FlowSensor.UPM = 0;
	}
}
