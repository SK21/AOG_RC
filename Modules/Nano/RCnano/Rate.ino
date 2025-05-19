
const uint32_t PulseMin = 250;		// micros
const int SampleSize = 8;
const uint32_t FlowTimeout = 4000;
const uint32_t PulseMaxReset = 500000;

uint32_t Samples[2][SampleSize];
uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t ReadTime[2];
uint32_t PulseTime[2];

volatile uint32_t PulseMax[2] = { 0,0 };
volatile uint16_t PulseCount[2];
volatile int SamplesCount[2];
volatile uint32_t SamplesTime[2];
volatile uint16_t SamplesIndex[2];

void ISR0()
{
	PulseISR(0);
}

void ISR1()
{
	PulseISR(1);
}

void PulseISR(uint8_t ID)
{
	ReadTime[ID] = micros();
	PulseTime[ID] = ReadTime[ID] - ReadLast[ID];

	if (PulseTime[ID] > PulseMin)
	{
		ReadLast[ID] = ReadTime[ID];
		PulseMax[ID] = (PulseMax[ID] * 8 + PulseTime[ID] * 3) / 10;	// 1.5 X average

		if (PulseTime[ID] < PulseMax[ID])
		{
			PulseCount[ID]++;
			SamplesTime[ID] -= Samples[ID][SamplesIndex[ID]];
			Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
			SamplesTime[ID] += Samples[ID][SamplesIndex[ID]];
			SamplesIndex[ID] = (SamplesIndex[ID] + 1) % SampleSize;
			if (SamplesCount[ID] < SampleSize) SamplesCount[ID]++;
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
			uint16_t TotalCounts = SamplesCount[i];
			uint32_t TotalTime = SamplesTime[i];
			interrupts();

			Sensor[i].Hz = (1000000.0 * TotalCounts / TotalTime) * 0.8 + Sensor[i].Hz * 0.2;
			Sensor[i].UPM = (60.0 * Sensor[i].Hz) / Sensor[i].MeterCal;
		}

		// check for no flow
		if (millis() - LastPulse[i] > FlowTimeout || (!Sensor[i].FlowEnabled))
		{
			Sensor[i].UPM = 0;
			Sensor[i].Hz = 0;
			PulseMax[i] = PulseMaxReset;
			SamplesCount[i] = 0;
			SamplesIndex[i] = 0;
			SamplesTime[i] = 0;
			memset(Samples[i], 0, sizeof(Samples[i]));
		}
	}
}
