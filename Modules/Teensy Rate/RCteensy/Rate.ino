const uint32_t PulseMin = 250;      // micros
const uint32_t PulseMax = 1000000;	// 1 Hz
const int SampleSize = 12;
const uint32_t FlowTimeout = 4000;

uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t ReadTime[2];
uint32_t PulseTime[2];

volatile uint32_t Samples[2][SampleSize];
volatile uint16_t PulseCount[2];
volatile uint8_t SamplesCount[2];
volatile uint8_t SamplesIndex[2];

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

uint32_t MedianFromArray(uint32_t buf[], int count)
{
	uint32_t Result = 0;
	if (count > 0)
	{
		uint32_t sorted[SampleSize];
		for (int i = 0; i < count; i++) sorted[i] = buf[i];

		// insertion sort
		for (int i = 1; i < count; i++)
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

		if (count % 2 == 1)
		{
			Result = sorted[count / 2];
		}
		else
		{
			int mid = count / 2;
			// average of middle two
			Result = (sorted[mid - 1] + sorted[mid]) / 2;
		}
	}
	return Result;
}

void GetUPM()
{
	for (int i = 0; i < 2; i++)
	{
		if (PulseCount[i])
		{
			LastPulse[i] = millis();

			uint32_t Snapshot[SampleSize];
			int count = 0;

			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseCount[i] = 0;
			count = SamplesCount[i];
			for (uint8_t k = 0; k < count; k++)
			{
				Snapshot[k] = Samples[i][k];
			}
			interrupts();

			uint32_t median = MedianFromArray(Snapshot, count);

			if (median > 0)
			{
				double hz = 1000000.0 / median;
				Sensor[i].Hz = hz * 0.8 + Sensor[i].Hz * 0.2;
				if (Sensor[i].MeterCal > 0) Sensor[i].UPM = (60.0 * Sensor[i].Hz) / Sensor[i].MeterCal;
			}
		}

		// No flow check
		if (millis() - LastPulse[i] > FlowTimeout || (!Sensor[i].FlowEnabled))
		{
			Sensor[i].UPM = 0;
			Sensor[i].Hz = 0;

			noInterrupts();
			SamplesCount[i] = 0;
			SamplesIndex[i] = 0;
			for (uint8_t k = 0; k < SampleSize; k++) 
			{
				Samples[i][k] = 0;
			}
			interrupts();
		}
	}
}
