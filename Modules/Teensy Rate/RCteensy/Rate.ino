// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

uint32_t LastPulse[MaxProductCount];
uint32_t ReadLast[MaxProductCount];
uint32_t PulseTime[MaxProductCount];

// Median is taken over pulses that arrived within the flow window (hybrid fixed-time window):
// at high flow the count cap (MaxSampleSize, via the ring buffer) binds and gives smoothing;
// at low flow the time window binds, so measurement lag stays ~window/2 instead of ballooning,
// and stale samples age out by time rather than by pulse count. Smaller window = less lag but
// noisier at low flow; this is the main tuning knob.
// The window is user-adjustable per sensor: Sensor[i].PulseSampleSize carries it as
// centiseconds (x10 ms), so flowWindowUs = SampleWindow * 10000. 

volatile uint32_t Samples[MaxProductCount][MaxSampleSize];
volatile uint32_t SampleStamp[MaxProductCount][MaxSampleSize];	// micros() when each pulse arrived
volatile uint16_t PulseCount[MaxProductCount];
volatile uint8_t SamplesCount[MaxProductCount];
volatile uint8_t SamplesIndex[MaxProductCount];

void PulseISR(uint8_t ID)
{
	if (RelayLo > 0 || RelayHi > 0)
	{
		uint32_t ReadTime = micros();
		PulseTime[ID] = ReadTime - ReadLast[ID];
		ReadLast[ID] = ReadTime;

		if (PulseTime[ID] > Sensor[ID].PulseMin && PulseTime[ID] < Sensor[ID].PulseMax)
		{
			// valid pulses - store period + arrival time in a fixed-size ring (decoupled
			// from PulseSampleSize so changing that setting can't scramble the buffer)
			PulseCount[ID]++;
			Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
			SampleStamp[ID][SamplesIndex[ID]] = ReadTime;
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

			uint32_t nowMicros = micros();
			uint32_t flowWindowUs = (uint32_t)Sensor[i].SampleWindow * 10000UL;	// centiseconds -> microseconds
			uint32_t Snapshot[MaxSampleSize];
			uint16_t count = 0;

			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseCount[i] = 0;

			// walk newest -> oldest. Keep pulses inside the time window; but if the window
			// holds fewer than MinMedianSamples, keep reaching back past the window until we
			// have the floor, so the median always has enough samples to reject a single
			// outlier (a 2-sample median is just the mean -> one long period halves the reading).
			uint8_t fill = SamplesCount[i];
			uint8_t idx = SamplesIndex[i];				// next write slot
			for (uint8_t n = 0; n < fill && count < MaxSampleSize; n++)
			{
				uint8_t slot = (idx + MaxSampleSize - 1 - n) % MaxSampleSize;
				if (nowMicros - SampleStamp[i][slot] <= flowWindowUs || count < MinMedianSamples)
				{
					Snapshot[count++] = Samples[i][slot];
				}
				else
				{
					break;	// older than window AND floor met; everything further back is older too
				}
			}

			interrupts();

			uint32_t median = (count > 0) ? MedianFromArray(Snapshot, count) : 0;
			MedianCount[i] = count;

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
			if (millis() - LastPulse[i] > FlowTimeout || (RelayLo == 0 && RelayHi == 0))
			{
				Sensor[i].UPM = 0;
				Sensor[i].Hz = 0;
				MedianCount[i] = 0;

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

void ISR2()
{
	PulseISR(2);
}

void ISR3()
{
	PulseISR(3);
}

void ISR4()
{
	PulseISR(4);
}

void ISR5()
{
	PulseISR(5);
}

