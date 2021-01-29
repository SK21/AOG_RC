unsigned long LastFlowCal;
double Duration;
double Frequency;
unsigned long CurrentCounts;
double CurrentDuration;

float KalResult = 0;
float KalPc = 0.0;
float KalG = 0.0;
float KalP = 1.0;
float KalVariance = 0.01;	// larger is more filtering
float KalProcess = 0.005;	// smaller is more filtering

void FlowISR()
{
	// measure time for one pulse
	static unsigned long pulseStart = 0;
	pulseCount++;

	pulseState = !pulseState;
	if (pulseState)
	{
		pulseStart = millis();
	}
	else
	{
		pulseDuration = millis() - pulseStart;
		pulseStart = 0;
	}
}

float CalRateError()
{
	CurrentDuration = pulseDuration;
	CurrentCounts = pulseCount;
	pulseCount = 0;
	accumulatedCounts += CurrentCounts;

	if (CurrentDuration < 1 || MeterCal < 1)
	{
		FlowRate = 0;
	}
	else
	{
		Frequency = (1.0 / CurrentDuration) * 60000.0;	// pulses per minute
		FlowRate = Frequency / MeterCal;	// units per minute
	}

	// Kalmen filter
	KalPc = KalP + KalProcess;
	KalG = KalPc / (KalPc + KalVariance);
	KalP = (1 - KalG) * KalPc;
	KalResult = KalG * (FlowRate - KalResult) + KalResult;
	FlowRate = KalResult;

	return rateSetPoint - FlowRate;
}


