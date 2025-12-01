// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

uint32_t LastPulse[MaxProductCount];
uint32_t ReadLast[MaxProductCount];
uint32_t PulseTime[MaxProductCount];

volatile uint32_t Samples[MaxProductCount][MaxSampleSize];
volatile uint16_t PulseCount[MaxProductCount];
volatile uint8_t SamplesCount[MaxProductCount];
volatile uint8_t SamplesIndex[MaxProductCount];

IRAM_ATTR void PulseISR(uint8_t ID,uint32_t ReadTime)
{
	if (RelayLo > 0 || RelayHi > 0)
	{
		PulseTime[ID] = ReadTime - ReadLast[ID];
		ReadLast[ID] = ReadTime;

		if (PulseTime[ID] > Sensor[ID].PulseMin && PulseTime[ID] < Sensor[ID].PulseMax)			
		{
			PulseCount[ID]++;
			Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
			SamplesIndex[ID] = (SamplesIndex[ID] + 1) % Sensor[ID].PulseSampleSize;
			if (SamplesCount[ID] < Sensor[ID].PulseSampleSize) SamplesCount[ID]++;
		}
	}
}

void GetUPM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (PulseCount[i])
		{
			LastPulse[i] = millis();

			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseCount[i] = 0;
			uint16_t count = SamplesCount[i];
			uint32_t Snapshot[MaxSampleSize];
			for (uint16_t k = 0; k < count; k++)
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
		}
		else
		{
			// No flow check
			if (millis() - LastPulse[i] > FlowTimeout || (!Sensor[i].FlowEnabled))
			{
				Sensor[i].UPM = 0;
				Sensor[i].Hz = 0;

				noInterrupts();
				SamplesCount[i] = 0;
				SamplesIndex[i] = 0;
				interrupts();
			}
		}
	}
}

IRAM_ATTR void ISR0()
{
	PulseISR(0,micros());
}

IRAM_ATTR void ISR1()
{
	PulseISR(1, micros());
}

IRAM_ATTR void ISR2()
{
	PulseISR(2, micros());
}

IRAM_ATTR void ISR3()
{
	PulseISR(3, micros());
}

IRAM_ATTR void ISR4()
{
	PulseISR(4, micros());
}

IRAM_ATTR void ISR5()
{
	PulseISR(5, micros());
}

