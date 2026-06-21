// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t PulseTime[2];

// Median is taken over pulses that arrived within FlowWindow (hybrid fixed-time window):
// at high flow the count cap (PulseSampleSize) binds and gives smoothing; at low flow the
// time window binds, so measurement lag stays ~FlowWindow/2 instead of ballooning, and stale
// samples age out by time (FlowWindow) rather than by pulse count. Lower FlowWindow = less
// lag but noisier at low flow; this is the main tuning knob.
const uint32_t FlowWindow = 150000;	// microseconds (150 ms)

volatile uint32_t Samples[2][MaxSampleSize];
volatile uint32_t SampleStamp[2][MaxSampleSize];	// micros() when each pulse arrived
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
			uint32_t Snapshot[MaxSampleSize];
			uint16_t count = 0;

			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseCount[i] = 0;
			uint8_t fill = SamplesCount[i];
			uint8_t idx = SamplesIndex[i];				// next write slot
			uint8_t cap = Sensor[i].PulseSampleSize;	// hybrid count limit
			if (cap > MaxSampleSize) cap = MaxSampleSize;
			// walk newest -> oldest, keep pulses inside the time window, capped by count
			for (uint8_t n = 0; n < fill && count < cap; n++)
			{
				uint8_t slot = (idx + MaxSampleSize - 1 - n) % MaxSampleSize;
				if (nowMicros - SampleStamp[i][slot] <= FlowWindow)
				{
					Snapshot[count++] = Samples[i][slot];
				}
				else
				{
					break;	// older than window; everything further back is older too
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

