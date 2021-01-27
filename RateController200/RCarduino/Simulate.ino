float ValveAdjust = 0;   // % amount to open/close valve
float ValveOpen = 0;      // % valve is open
float Pulses = 0.0;
float ValveOpenTime = 4000;  // ms to fully open valve at max opening rate
float UPM = 0.00;     // simulated units per minute
float MaxRate = 120;  // max rate of system in UPM
int ErrorRange = 4;  // % random error in flow rate
float PulseTime = 0.0;

unsigned long SimulateInterval;
unsigned long SimulateTimeLast;
float RandomError = 0;

void DoSimulate()
{
	SimulateInterval = millis() - SimulateTimeLast;
	SimulateTimeLast = millis();

	if (RelaysOn)
	{
		// relays on
		float Range = MaxPWMvalue - MinPWMvalue + 5;
		if (Range == 0 || pwmSetting == 0)
		{
			ValveAdjust = 0;
		}
		else
		{
			float Percent = (float)((abs(pwmSetting) - MinPWMvalue + 5) / Range);
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

		RandomError = (100 - ErrorRange) + (random(ErrorRange * 2));
		PulseTime = (float)(PulseTime * RandomError / 100);

		pulseCount = SimulateInterval / PulseTime;	// milliseconds * pulses/millsecond = pulses

		// pulse duration is the time for one pulse
		pulseDuration = PulseTime;
	}
}



