// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

const int MaxSampleSize = 25;
const uint32_t FlowTimeout = 4000;

uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t PulseTime[2];

volatile uint32_t Samples[2][MaxSampleSize];
volatile uint16_t PulseCount[2];
volatile uint8_t SamplesCount[2];
volatile uint8_t SamplesIndex[2];

IRAM_ATTR void PulseISR(uint8_t ID)
{
	uint32_t ReadTime = micros();
	PulseTime[ID] = ReadTime - ReadLast[ID];
	ReadLast[ID] = ReadTime;

	if (PulseTime[ID] > Sensor[ID].PulseMin && PulseTime[ID] < Sensor[ID].PulseMax
		&& (RelayLo > 0 || RelayHi > 0))
	{
		PulseCount[ID]++;
		Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
		SamplesIndex[ID] = (SamplesIndex[ID] + 1) % Sensor[ID].PulseSampleSize;
		if (SamplesCount[ID] < Sensor[ID].PulseSampleSize) SamplesCount[ID]++;
	}
}

void GetUPM()
{
	for (int i = 0; i < 2; i++)
	{
		if (PulseCount[i])
		{
			LastPulse[i] = millis();

			uint32_t Snapshot[MaxSampleSize];
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
				float hz = 1000000.0 / median;
				Sensor[i].Hz = hz * 0.8 + Sensor[i].Hz * 0.2;
				if (Sensor[i].MeterCal > 0) Sensor[i].UPM = (60.0 * Sensor[i].Hz) / Sensor[i].MeterCal;
			}

			// No flow check
			if (millis() - LastPulse[i] > FlowTimeout || (!Sensor[i].FlowEnabled))
			{
				Sensor[i].UPM = 0;
				Sensor[i].Hz = 0;

				noInterrupts();
				SamplesCount[i] = 0;
				SamplesIndex[i] = 0;
				for (uint8_t k = 0; k < Sensor[i].PulseSampleSize; k++)
				{
					Samples[i][k] = 0;
				}
				interrupts();
			}
		}
	}
}
IRAM_ATTR void ISR0()
{
	PulseISR(0);
}

IRAM_ATTR void ISR1()
{
	PulseISR(1);
}

uint32_t MedianFromArray(uint32_t buf[], int count)
{
	uint32_t Result = 0;
	if (count > 0)
	{
		uint32_t sorted[MaxSampleSize];
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
