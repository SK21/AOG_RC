// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t PulseTime[2];

volatile uint32_t Samples[2][MaxSampleSize];
volatile uint16_t PulseCount[2];
volatile uint8_t SamplesCount[2];
volatile uint8_t SamplesIndex[2];

void PulseISR(uint8_t ID)
{
	if (RelayLo > 0 || RelayHi > 0)
	{
		uint32_t ReadTime = micros();
		PulseTime[ID] = ReadTime - ReadLast[ID];
		ReadLast[ID] = ReadTime;

		if (PulseTime[ID] > Sensor[ID].PulseMin && PulseTime[ID] < Sensor[ID].PulseMax)
		{
			// valid pulses - fixed-size ring, decoupled from PulseSampleSize so a
			// setting change (or a 0 on the wire) can never scramble the buffer or
			// divide by zero inside the ISR
			PulseCount[ID]++;
			Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
			SamplesIndex[ID] = (SamplesIndex[ID] + 1) % MaxSampleSize;
			if (SamplesCount[ID] < MaxSampleSize) SamplesCount[ID]++;
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

			// median over the NEWEST PulseSampleSize pulses (walk back from the next
			// write slot) so the setting still controls smoothing with the fixed ring
			uint8_t fill = SamplesCount[i];
			uint8_t idx = SamplesIndex[i];		// next write slot
			uint8_t count = fill;
			if (count > Sensor[i].PulseSampleSize) count = Sensor[i].PulseSampleSize;
			uint32_t Snapshot[MaxSampleSize];
			for (uint8_t n = 0; n < count; n++)
			{
				uint8_t slot = (idx + MaxSampleSize - 1 - n) % MaxSampleSize;
				Snapshot[n] = Samples[i][slot];
			}
			interrupts();

			uint32_t median = MedianFromArray(Snapshot, count);

			if (median > 0)
			{
				float hz = 1000000.0f / (float)median;
				Sensor[i].Hz = hz * 0.8f + Sensor[i].Hz * 0.2f;
				if (Sensor[i].MeterCal > 0.0f) Sensor[i].UPM = (60.0f * Sensor[i].Hz) / Sensor[i].MeterCal;
			}
		}
		else
		{
			// No flow check
			if (millis() - LastPulse[i] > FlowTimeout || (RelayLo == 0 && RelayHi == 0))
			{
				Sensor[i].UPM = 0.0f;
				Sensor[i].Hz = 0.0f;

				noInterrupts();
				SamplesCount[i] = 0;
				SamplesIndex[i] = 0;
				interrupts();
			}
		}
	}
}

void ISR0()
{
	PulseISR(0);
}

void ISR1()
{
	PulseISR(1);
}

