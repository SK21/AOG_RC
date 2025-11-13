
uint32_t LastPulseWhl;
uint32_t ReadLastWhl;
uint32_t PulseTimeWhl;
float HzWhl = 0;
const uint32_t PulseMinWhl = 250;		// 4000 Hz
const uint32_t PulseMaxWhl = 1000000;	// 1 Hz
const uint8_t PulseSampleSizeWhl = 11;

volatile uint32_t SamplesWhl[MaxSampleSize];
volatile uint16_t PulseCountWhl;
volatile uint8_t SamplesCountWhl;
volatile uint8_t SamplesIndexWhl;

void ISR_Speed()
{
	uint32_t ReadTime = micros();
	PulseTimeWhl = ReadTime - ReadLastWhl;
	ReadLastWhl = ReadTime;

	if (PulseTimeWhl > PulseMinWhl && PulseTimeWhl < PulseMaxWhl)
	{
		// valid pulses
		PulseCountWhl++;
		SamplesWhl[SamplesIndexWhl] = PulseTimeWhl;
		SamplesIndexWhl = (SamplesIndexWhl + 1) % PulseSampleSizeWhl;
		if (SamplesCountWhl < PulseSampleSizeWhl) SamplesCountWhl++;
	}
}

void GetSpeed()
{
	if (PulseCountWhl)
	{
		LastPulseWhl = millis();

		noInterrupts();
		PulseCountWhl = 0;
		uint16_t count = SamplesCountWhl;
		uint32_t Snapshot[MaxSampleSize];
		for (uint16_t k = 0; k < count; k++)
		{
			Snapshot[k] = SamplesWhl[k];
		}
		interrupts();

		uint32_t median = MedianFromArray(Snapshot, count);

		if (median > 0 && MDL.WheelCal > 0)
		{
			HzWhl = (HzWhl * 0.8) + (1000000.0 / median) * 0.2;
			WheelSpeed = (HzWhl * 3600) / MDL.WheelCal;
		}
	}
	else
	{
		if (millis() - LastPulseWhl > FlowTimeout)
		{
			HzWhl = 0;
			WheelSpeed = 0;

			noInterrupts();
			SamplesCountWhl = 0;
			SamplesIndexWhl = 0;
			interrupts();
		}
	}
}

