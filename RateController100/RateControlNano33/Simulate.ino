float ValveAdjust = 0;   // % amount to open/close valve
float ValveOpen = 0;      // % valve is open
float Pulses = 0.0;
float ValveOpenTime = 2000;  // ms to fully open valve at max opening rate
float UPM = 0.00;     // simulated units per minute
float MaxRate = 120;  // max rate of system in UPM
float ErrorRange = 8;  // % random error in flow rate, above and below target
float PulseTime = 0.0;
float PWMnet = 0;	// pwmSetting - minPWM to account for motor lag

unsigned long SimulateInterval;
unsigned long SimulateTimeLast;

void DoSimulate()
{
	SimulateInterval = millis() - SimulateTimeLast;
	SimulateTimeLast = millis();

	if (RelaysOn)
	{

		// relays on
		if (AutoOn)
		{
			PWMnet = pwmSetting;
			if (PWMnet < 0)
			{
				PWMnet += (MinPWMvalue * .5);
				if (PWMnet > 0) PWMnet = 0;
			}
			else
			{
				PWMnet -= (MinPWMvalue * .5);
				if (PWMnet < 0) PWMnet = 0;
			}

			ValveAdjust = (float)(PWMnet / 255) * (float)(SimulateInterval / ValveOpenTime) * 100.0;
		}
		else
		{
			// manual control
			ValveAdjust = 0;
			if (RateUpMan) ValveAdjust = (float)(SimulateInterval / ValveOpenTime) * 100.0 * (pwmManualRatio / 100);
			if (RateDownMan) ValveAdjust = (float)(SimulateInterval / ValveOpenTime) * -100.0 * (pwmManualRatio / 100);
		}

		ValveOpen += ValveAdjust;
		if (ValveOpen < 0) ValveOpen = 0;
		if (ValveOpen > 100) ValveOpen = 100;
	}
	else
	{
		// relays off
		ValveOpen = 0;
	}

	UPM = MaxRate * ValveOpen / 100.0;

	Pulses = (UPM * MeterCal) / 60000.0;  // (Units/min * pulses/Unit) = pulses/min / 60000 = pulses/millisecond
	if (Pulses == 0)
	{
		pulseCount = 0;
		pulseDuration = 0;
	}
	else
	{
		PulseTime = 1.0 / Pulses;	// milliseconds for each pulse

		PulseTime = (random(ErrorRange * 2.0) / 100.0) * PulseTime + ((100.0 - ErrorRange) / 100.0) * PulseTime;
		pulseCount = SimulateInterval / PulseTime;	// milliseconds * pulses/millsecond = pulses

		// pulse duration is the total time for all pulses in the loop
		pulseDuration = PulseTime * pulseCount;
	}
}


