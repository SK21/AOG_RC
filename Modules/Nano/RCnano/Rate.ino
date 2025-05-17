
const uint32_t PulseMin = 250;		// micros
const int SampleSize = 8;
const double PulseLimit = 2000000.0;	// PulseMax is limited to ? X pulse time (1000000/Hz)

uint32_t Samples[2][SampleSize];
uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t ReadTime[2];
uint32_t PulseTime[2];

volatile uint32_t PulseMax[2] = { 50000,50000 };
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
	debug2 = PulseTime[0];

	if (PulseTime[ID] > PulseMin)
	{
		ReadLast[ID] = ReadTime[ID];
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
			PulseMax[i] = constrain(PulseLimit / Sensor[i].Hz, 5000, PulseLimit);	// max limit set to pulse time X 1.5 (within 5000 to 1500000 micros)
			//PulseMax[i] = 500000;
			if (i == 0)
			{
				//debug1 = (double)PulseMax[0] / (TotalTime / TotalCounts);
				//debug3 = TotalTime;
				//debug4 = TotalCounts;
			}
		}

		// check for no flow
		if (millis() - LastPulse[i] > 4000)
		{
			Sensor[i].UPM = 0;
			Sensor[i].Hz = 0;
			PulseMax[i] = 500000;
			SamplesCount[i] = 0;
			SamplesIndex[i] = 0;
			SamplesTime[i] = 0;
			memset(Samples[i], 0, sizeof(Samples[i]));
		}
	}
}
