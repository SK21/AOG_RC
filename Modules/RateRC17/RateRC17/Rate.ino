
const uint32_t PulseMin = 250;		// micros
const int SampleSize = 24;
uint32_t Samples[SampleSize];
uint32_t LastPulse;
double PulseAvg = 0;
uint16_t CurrentCount;
uint32_t CurrentTotal;
uint32_t PulseLast;
uint32_t PulseTime;
uint32_t PulseCurrent;

volatile uint32_t PulseMax = 500000;
volatile uint16_t PulseCount;
volatile int SamplesCount;
volatile uint32_t SamplesTotal;
volatile uint16_t SamplesIndex;

void IRAM_ATTR ISR0()
{
	PulseTime = micros();
	PulseCurrent = PulseTime - PulseLast;

	if (PulseCurrent > PulseMin)
	{
		PulseLast = PulseTime;
		if (PulseCurrent < PulseMax)
		{
			PulseCount++;
			SamplesTotal -= Samples[SamplesIndex];
			Samples[SamplesIndex] = PulseCurrent;
			SamplesTotal += Samples[SamplesIndex];
			SamplesIndex = (SamplesIndex + 1) % SampleSize;
			SamplesCount++;
		}
	}
}

void GetUPM()
{
	if (PulseCount && FlowSensor.MeterCal > 0)
	{
		LastPulse = millis();

		noInterrupts();
		FlowSensor.TotalPulses += PulseCount;
		PulseCount = 0;
		if (SamplesCount > SampleSize) SamplesCount = SampleSize;
		CurrentCount = SamplesCount;
		CurrentTotal = SamplesTotal;
		interrupts();

		PulseAvg = ((double)CurrentTotal / CurrentCount) * 0.8 + PulseAvg * 0.2;
		FlowSensor.UPM = (double)(60000000.0 / PulseAvg) / FlowSensor.MeterCal;
		PulseMax = PulseAvg * 1.5;

		debug1 = (double)PulseAvg / 1000.0;
		debug2 = FlowSensor.UPM;
		debug3 = (double)1000000.0 / PulseAvg;
		debug4 =(double) PulseMax / 1000.0;
	}

	// check for no flow
	if (millis() - LastPulse > 4000)
	{
		FlowSensor.UPM = 0;
		PulseMax = 500000;
		SamplesCount = 0;
		SamplesIndex = 0;
		SamplesTotal = 0;
		memset(Samples, 0, sizeof(Samples));
	}
}
