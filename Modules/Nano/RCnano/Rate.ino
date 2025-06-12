const uint32_t PulseMin = 250;      // micros
const uint32_t PulseMax = 250000;	// 4 Hz
const int SampleSize = 12;
const uint32_t FlowTimeout = 4000;

uint32_t Samples[2][SampleSize];
uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t ReadTime[2];
uint32_t PulseTime[2];

volatile uint16_t PulseCount[2];
volatile int SamplesCount[2];
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
	ReadLast[ID] = ReadTime[ID];

	if (PulseTime[ID] > PulseMin && PulseTime[ID] < PulseMax)
	{
		PulseCount[ID]++;
		Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
		SamplesIndex[ID] = (SamplesIndex[ID] + 1) % SampleSize;
		if (SamplesCount[ID] < SampleSize) SamplesCount[ID]++;
	}
}

uint32_t GetMedianPulseTime(uint8_t ID)
{
	uint32_t Result = 0;
	if (SamplesCount[ID] > 0)
	{
		uint32_t sorted[SampleSize];
		memcpy(sorted, Samples[ID], sizeof(sorted));

		// Insertion sort
		for (int i = 1; i < SamplesCount[ID]; i++)
		{
			uint32_t key = sorted[i];
			int j = i - 1;
			while (j >= 0 && sorted[j] > key)
			{
				sorted[j + 1] = sorted[j];
				j--;
			}
			sorted[j + 1] = key;
		}

		if (SamplesCount[ID] % 2 == 1)
		{
			Result = sorted[SamplesCount[ID] / 2];
		}
		else
		{
			int mid = SamplesCount[ID] / 2;
			Result = (sorted[mid - 1] + sorted[mid]) / 2;
		}
	}
	return Result;
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
			uint32_t median = GetMedianPulseTime(i);
			interrupts();

			if (median > 0)
			{
				double hz = 1000000.0 / median;
				Sensor[i].Hz = hz * 0.8 + Sensor[i].Hz * 0.2;
				Sensor[i].UPM = (60.0 * Sensor[i].Hz) / Sensor[i].MeterCal;
			}
		}

		// Check for no flow
		if (millis() - LastPulse[i] > FlowTimeout || (!Sensor[i].FlowEnabled))
		{
			Sensor[i].UPM = 0;
			Sensor[i].Hz = 0;
			SamplesCount[i] = 0;
			SamplesIndex[i] = 0;
			memset(Samples[i], 0, sizeof(Samples[i]));
		}
	}
}
