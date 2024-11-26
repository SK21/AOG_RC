
const uint32_t PulseMin = 250;		// micros
const int SampleSize = 24;
uint32_t Samples[2][SampleSize];
uint32_t LastPulse[2];
double PulseAvg[2];
uint16_t CurrentCount[2];
uint32_t CurrentTotal[2];
uint32_t PulseLast[2];
uint32_t PulseTime[2];
uint32_t PulseCurrent[2];

volatile uint32_t PulseMax[2] = { 50000,50000 };
volatile uint16_t PulseCount[2];
volatile int SamplesCount[2];
volatile uint32_t SamplesTotal[2];
volatile uint16_t SamplesIndex[2];

void ISR0()
{
	PulseTime[0] = micros();
	PulseCurrent[0] = PulseTime[0] - PulseLast[0];

	if (PulseCurrent[0] > PulseMin)
	{
		PulseLast[0] = PulseTime[0];
		if (PulseCurrent[0] < PulseMax[0])
		{
			PulseCount[0]++;
			SamplesTotal[0] -= Samples[0][SamplesIndex[0]];
			Samples[0][SamplesIndex[0]] = PulseCurrent[0];
			SamplesTotal[0] += Samples[0][SamplesIndex[0]];
			SamplesIndex[0] = (SamplesIndex[0] + 1) % SampleSize;
			SamplesCount[0]++;
		}
	}
}

void ISR1()
{
	PulseTime[1] = micros();
	PulseCurrent[1] = PulseTime[1] - PulseLast[1];

	if (PulseCurrent[1] > PulseMin)
	{
		PulseLast[1] = PulseTime[1];
		if (PulseCurrent[1] < PulseMax[1])
		{
			PulseCount[1]++;
			SamplesTotal[1] -= Samples[1][SamplesIndex[1]];
			Samples[1][SamplesIndex[1]] = PulseCurrent[1];
			SamplesTotal[1] += Samples[1][SamplesIndex[1]];
			SamplesIndex[1] = (SamplesIndex[1] + 1) % SampleSize;
			SamplesCount[1]++;
		}
	}
}

void GetUPM()
{
	for (int i = 0; i < 2; i++)
	{
		if (PulseCount[i] && Sensor[i].MeterCal > 0)
		{
			LastPulse[i] = millis();

			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseCount[i] = 0;
			if (SamplesCount[i] > SampleSize) SamplesCount[i] = SampleSize;
			CurrentCount[i] = SamplesCount[i];
			CurrentTotal[i] = SamplesTotal[i];
			interrupts();

			PulseAvg[i] = ((double)CurrentTotal[i] / CurrentCount[i]) * 0.8 + PulseAvg[i] * 0.2;
			Sensor[i].UPM = (double)(60000000.0 / PulseAvg[i]) / Sensor[i].MeterCal;
			PulseMax[i] = PulseAvg[i] * 1.5;
		}

		// check for no flow
		if (millis() - LastPulse[i] > 4000)
		{
			Sensor[i].UPM = 0;
			PulseMax[i] = 500000;
			SamplesCount[i] = 0;
			SamplesIndex[i] = 0;
			SamplesTotal[i] = 0;
			memset(Samples[i], 0, sizeof(Samples[i]));
		}
	}
}
