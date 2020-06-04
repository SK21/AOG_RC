unsigned long LastFlowCal;
double Duration;
double Frequency;
unsigned long CurrentCounts;

void FlowISR()
{
	pulseCount++;
}

float CalRateError()
{
	Duration = (millis() - LastFlowCal) / 60000.0;	// minutes
	LastFlowCal = millis();
	CurrentCounts = pulseCount;
	pulseCount = 0;
	accumulatedCounts += CurrentCounts;

	if (Duration == 0 | MeterCal == 0)
	{
		FlowRate = 0;
	}
	else
	{
		Frequency = CurrentCounts / Duration;
		FlowRate = Frequency / MeterCal;	// units per minute
	}

	return rateSetPoint - FlowRate;
}

