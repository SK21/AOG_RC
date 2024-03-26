volatile uint32_t PulseTime[2];
volatile uint32_t PulseCount[2];
uint32_t PulseLast[2];
double PulseHz[2];

double HzSum[2];
double HzMax[2];
double HzMin[2];
uint16_t HzCount[2];
double HzAvg[2];

const uint16_t Debounce = 500;	// micros
const double MinDuration = 0.5;	// seconds
const uint8_t MinPulses = 12;

void ISR0()
{
	if (micros() - PulseTime[0] > Debounce)
	{
		PulseCount[0]++;
		PulseTime[0] = micros();
	}
}

void ISR1()
{
	if (micros() - PulseTime[1] > Debounce)
	{
		PulseCount[1]++;
		PulseTime[1] = micros();
	}
}

void GetUPM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		double PulsesDuration = (double)((PulseTime[i] - PulseLast[i]) / 1000000.0);	// seconds
		if (PulseCount[i] > MinPulses && PulsesDuration > MinDuration)
		{
			noInterrupts();
			Sensor[i].TotalPulses += PulseCount[i];
			PulseHz[i] = (double)(PulseCount[i] / PulsesDuration);
			PulseCount[i] = 0;
			PulseLast[i] = micros();
			interrupts();
		}

		// check for no flow
		if (micros() - PulseLast[i] > 4000000)
		{
			PulseHz[i] = 0;
			HzSum[i] = 0;
			HzAvg[i] = 0;
			HzCount[i] = 0;
			HzMax[i] = 0;
			HzMin[i] = 5000000;
		}

		// olymic average
		HzSum[i] += PulseHz[i];
		if (HzMax[i] < PulseHz[i]) HzMax[i] = PulseHz[i];
		if (HzMin[i] > PulseHz[i]) HzMin[i] = PulseHz[i];

		HzCount[i]++;
		if (HzCount[i] > 4)
		{
			HzSum[i] -= HzMax[i];
			HzSum[i] -= HzMin[i];
			HzAvg[i] = HzSum[i] / 3.0;
			HzSum[i] = 0;
			HzMax[i] = 0;
			HzMin[i] = 5000000;
			HzCount[i] = 0;
		}

		// units per minute
		if (Sensor[i].MeterCal > 0)
		{
			Sensor[i].UPM = (HzAvg[i] * 60.0) / Sensor[i].MeterCal;
		}
		else
		{
			Sensor[i].UPM = 0;
		}
	}
}
