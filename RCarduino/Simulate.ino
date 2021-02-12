float ValveAdjust = 0.0;   // % amount to open/close valve
float ValveOpen = 0.0;      // % valve is open
float Pulses = 0.0;
float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate
float simUPM = 0.00;     // simulated units per minute
float MaxRate = 120.0;  // max rate of system in UPM
float ErrorRange = 4.0;  // % random error in flow rate
float PulseTime = 0.0;

unsigned long SimulateInterval;
unsigned long SimulateTimeLast;
float RandomError = 0.0;

void SimulateValve(byte sMin, byte sMax)
{
	SimulateInterval = millis() - SimulateTimeLast;
	SimulateTimeLast = millis();

	if (ApplicationOn)
	{
		float Range = sMax - sMin + 5;
		if (Range == 0 || pwmSetting == 0)
		{
			ValveAdjust = 0;
		}
		else
		{
			float Percent = (float)((abs(pwmSetting) - sMin + 5) / Range);
			if (pwmSetting < 0)
			{
				Percent *= -1;
			}

			ValveAdjust = (float)(Percent * (float)(SimulateInterval / ValveOpenTime) * 100.0);
		}

		ValveOpen += ValveAdjust;
		if (ValveOpen < 0) ValveOpen = 0;
		if (ValveOpen > 100) ValveOpen = 100;
	}
	else
	{
		ValveOpen = 0;
	}

	simUPM = MaxRate * ValveOpen / 100.0;

	Pulses = (simUPM * MeterCal) / 60000.0;  // (Units/min * pulses/Unit) = pulses/min / 60000 = pulses/millisecond
	if (Pulses == 0)
	{
		pulseCount = 0;
		pulseDuration = 0;
	}
	else
	{
		PulseTime = 1.0 / Pulses;	// milliseconds for each pulse

		RandomError = (100.0 - ErrorRange) + (random(ErrorRange * 2.0));
		PulseTime = (float)(PulseTime * RandomError / 100.0);

		pulseCount = SimulateInterval / PulseTime;	// milliseconds * pulses/millsecond = pulses

		// pulse duration is the time for one pulse
		pulseDuration = PulseTime;
	}
}

float MaxRPM = 100.0;
float PPR = 50;	// pulses per revolution
float SimRPM = 0.0;
float SimTmp;

void SimulateMotor(byte sMin, byte sMax)
{
	if (ApplicationOn)
	{
		SimulateInterval = millis() - SimulateTimeLast;
		SimulateTimeLast = millis();

		SimRPM += ((pwmSetting / (float)sMax) * MaxRPM - SimRPM) * 0.25;	// update rpm
		RandomError = (100.0 - ErrorRange) + (random(ErrorRange * 2.0));
		SimRPM = SimRPM * RandomError / 100.0;
		if (SimRPM < sMin) SimRPM = (float)sMin;

		SimTmp = PPR * SimRPM;
		if (SimTmp > 0)
		{
			pulseDuration = 60000 / SimTmp;
		}
		else
		{
			pulseDuration = 0;
		}

		pulseCount = SimRPM * PPR;
		pulseCount = pulseCount * (SimulateInterval / 60000.0);	// counts for time slice
	}
	else
	{
		pulseCount = 0;
	}
}



