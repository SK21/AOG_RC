//ISR
void FlowPinISR()
{
	static unsigned long pulseStart = 0;
	pulseCount++;
	pulseDuration += (millis() - pulseStart); // get the pulse length
	pulseStart = millis(); // store the current microseconds and start clock again
}

float CalRateError()
{
	//reset ISR
	countsThisLoop = pulseCount;
	pulseCount = 0;

	//// adjust for zero flow
	//countsThisLoop = countsThisLoop - 100;
	//if (countsThisLoop < 0) countsThisLoop = 0;

	//accumulated counts from this cycle
	accumulatedCounts += countsThisLoop;

	if (countsThisLoop && MeterCal)	// Is there flow?
	{
		pulseAverage = pulseDuration / countsThisLoop;
		pulseDuration = 0;

		//what is current flowrate from meter, Units/minute
		FlowRate = (float)pulseAverage * 0.001;	// change from milliseconds/pulse to seconds/pulse

		if (FlowRate < .001) FlowRate = 0.1;	//prevent divide by zero      
		else FlowRate = ((1.0 / FlowRate) * 60) / MeterCal;	//pulses/minute divided by pulses/Unit (pulses/minute * Units/pulse = Units/minute)

		//Kalman filter
		Pc = P + varProcess;
		G = Pc / (Pc + varRate);
		P = (1 - G) * Pc; Xp = XeRate; Zp = Xp;
		XeRate = G * (FlowRate - Zp) + Xp;
		rateAppliedUPM = XeRate * 100; //fill in formatted rateApplied
		return rateSetPoint - XeRate;
	}
	else
	{
		rateAppliedUPM = 0;
		return rateSetPoint;
	}
}
