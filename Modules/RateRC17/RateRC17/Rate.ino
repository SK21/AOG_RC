
const uint32_t PulseMinSet = 1000;		// micros
const uint32_t PulseMaxSet = 500000;
uint32_t LastPulse;
double PulseAvg;
uint16_t SamplesIndex;

volatile uint16_t PulseCount;
volatile int SamplesCount;

void IRAM_ATTR ISR0()
{
	static uint32_t PulseLast;
	uint32_t PulseTime;
	uint32_t PulseCurrent;

	PulseTime = micros();
	PulseCurrent = PulseTime - PulseLast;
	PulseLast = PulseTime;
	debug1 = PulseCurrent;

	if (PulseCurrent > PulseMaxSet)
	{
		// flow was off
		SamplesCount = 0;
		SamplesIndex = 0;
	}
	else if (PulseCurrent > PulseMinSet)
	{
		PulseCount++;
		Samples[SamplesIndex] = PulseCurrent;
		SamplesIndex = (SamplesIndex + 1) % SampleSize;
		SamplesCount++;
	}
}

void GetUPM()
{
	if (PulseCount && FlowSensor.MeterCal>0)
	{
		LastPulse = millis();

		noInterrupts();
		FlowSensor.TotalPulses += PulseCount;
		PulseCount = 0;
		interrupts();

		if (SamplesCount > SampleSize) SamplesCount = SampleSize;
		int Sum = 0;
		int Reading = 0;
		int GoodCount = 0;
		for (int i = 0; i < SamplesCount; i++)
		{
			Reading = Samples[i];
			//if ((Reading < (PulseAvg * 2) && (Reading > (PulseAvg * 0.5))) || (PulseAvg == 0))
			//{
				Sum += Samples[i];
				GoodCount++;
			//}
		}
		if(GoodCount) PulseAvg = (double)(Sum / (GoodCount));
		FlowSensor.UPM = (double)(60000000.0 / PulseAvg) / FlowSensor.MeterCal;
		debug2 = PulseAvg;
		debug3 = 1000000 / PulseAvg;
		debug4 = GoodCount;
	}
	// check for no flow
	if (millis() - LastPulse > 4000) FlowSensor.UPM = 0;
}