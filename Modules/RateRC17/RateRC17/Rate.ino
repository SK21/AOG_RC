
const uint16_t PulseMin = 250;		// micros
const uint32_t PulseMax = 200000;
const uint16_t OffTime = 2000000;	// pulse time greater than this number means flow is off

uint32_t LastPulse;
double Hz;

const int SampleSize = 16;
double Sample[SampleSize];
int SampleIndex;
volatile double SampleTotal;
volatile int SampleCount;
volatile byte PulseCount;

void IRAM_ATTR ISR0()
{
	static uint32_t PulseLast;
	static uint32_t PulseTime;
	static uint32_t PulseCurrent;

	PulseTime = micros();
	PulseCurrent = PulseTime - PulseLast;

	if (PulseCurrent > OffTime)
	{
		// flow was off
		PulseLast = PulseTime;
		SampleCount = 0;
		SampleIndex = 0;
		SampleTotal = 0;
		memset(Sample, 0, sizeof(Sample));
	}
	else if (PulseCurrent > PulseMin)
	{
		PulseLast = PulseTime;
		if (PulseCurrent < PulseMax)
		{
			PulseCount++;

			SampleTotal -= Sample[SampleIndex];
			Sample[SampleIndex] = PulseCurrent;
			SampleTotal += Sample[SampleIndex];
			SampleIndex = (SampleIndex + 1) % SampleSize;
			SampleCount++;
		}
	}
}

void GetUPM()
{
	if (PulseCount)
	{
		LastPulse = millis();

		noInterrupts();
		FlowSensor.TotalPulses += PulseCount;
		PulseCount = 0;
		if (SampleCount > SampleSize) SampleCount = SampleSize;
		Hz = (double)(1000000.0 / (SampleTotal / SampleCount)) * 0.2 + Hz * 0.8;
		interrupts();

		FlowSensor.UPM = (Hz * 60.0) / FlowSensor.MeterCal;
	}

	// check for no flow
	if (millis() - LastPulse > 4000) FlowSensor.UPM = 0;
}



