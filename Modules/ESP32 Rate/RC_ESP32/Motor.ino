
void AdjustFlow()
{
	CheckPressure();
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		// Over-pressure gate overrides normal control: drive the flow-reducing direction.
		// Runs in auto and manual, and even if PID is gated off. SetPWM applies MDL.InvertFlow
		// uniformly, so the negative sign maps to the same physical direction the PID uses to
		// shed flow on this rig.
		if (PressureGateActive)
		{
			IntegralSum[i] = 0;	// no windup while gating
			switch (Sensor[i].ControlType)
			{
			case Motor_ct:
			case Fan_ct:
				Sensor[i].PWM = 0.0f;		// stop the pump/fan (the actuator is the flow source)
				break;

			default:
				Sensor[i].PWM = -255.0f;	// active relief: valve / combo close
				break;
			}
			SetPWM(i, Sensor[i].PWM);		// store in Sensor[i].PWM so PGN32400 reports the real output
		}
		else
		{
			float clamped = constrain(Sensor[i].PWM, -255.0f, 255.0f);

			switch (Sensor[i].ControlType)
			{
			case StandardValve_ct:
				SetPWM(i, SensorConnected[i] ? clamped : 0.0f);
				break;

			case Motor_ct:
			case Fan_ct:
				SetPWM(i, (SensorConnected[i] && Applying[i]) ? clamped : 0.0f);
				break;

			case ComboClose_ct:
			case TimedCombo_ct:
				// fast close valve or combo close timed, used for flow control and on/off
				SetPWM(i, SensorConnected[i] && Applying[i] ? clamped : -255.0f);
				break;

			default:
				break;
			}
		}
	}
}

void SetPWM(byte ID, float pwmVal)
{
	const int maxDuty = (1 << PWM_BITS) - 1;
	int duty = (int)floorf(fabsf(pwmVal) * maxDuty / 255.0f);

	bool Direction = (pwmVal >= 0.0f);
	if (MDL.InvertFlow) Direction = !Direction;

#if PWM_BITS == 8
	duty = ditherAdjust(duty, fabsf(pwmVal));
#endif


#if defined(ESP32)
	if (Direction)
	{
		analogWrite(Sensor[ID].IN1, duty);
		analogWrite(Sensor[ID].IN2, 0);
	}
	else
	{
		analogWrite(Sensor[ID].IN1, 0);
		analogWrite(Sensor[ID].IN2, duty);
	}
#else
	digitalWrite(Sensor[ID].DirPin, Direction);
	analogWrite(Sensor[ID].PWMPin, duty);
#endif
}

#if PWM_BITS == 8
int ditherAdjust(int base, float val255)
{
	const int maxDuty = 255;
	float exactDuty = val255 * maxDuty / 255.0f;
	float frac = exactDuty - base;

	ditherCounter = (ditherCounter + 1) & 0x0F; // 16 step cycle
	if (frac > 0 && ditherCounter < (uint8_t)(frac * 16)) {
		base = min(base + 1, maxDuty);
	}
	return base;
}
#endif

void CheckPressure()
{
	// Layer 1 over-pressure gate. Sets PressureGateActive; AdjustFlow() drives the
	// flow-reducing direction while it is set. Topology-independent: reducing flow at
	// the boom-line sensor reduces pressure at that sensor on every plumbing layout.
	//
	// Recovery is NOT pressure-alone: stopping the source collapses pressure, which would
	// otherwise release the gate and let the loop drive straight back into the blockage
	// (a bang-bang limit cycle). Instead:
	//   - hysteresis band stops chatter at the threshold
	//   - a minimum hold time rate-limits cycling on a transient
	//   - repeated trips in a short window escalate to a hard latch that only an operator
	//     reset (master off) clears - a persistent over-pressure is a fault, not a transient.

	// Disabled sentinel - clear all state and leave normal control alone.
	if (MDL.MaxPressureReading == 0xFFFF)
	{
		PressureGateActive = false;
		PressureGateLatched = false;
		PressureTripCount = 0;
		return;
	}

	// Operator reset: master off clears a hard latch and re-arms the gate.
	if (!MasterOn)
	{
		PressureGateActive = false;
		PressureGateLatched = false;
		PressureTripCount = 0;
		return;
	}

	// Hard latch holds relief until the reset above.
	if (PressureGateLatched)
	{
		PressureGateActive = true;
		return;
	}

	uint16_t releaseLevel = MDL.MaxPressureReading - (MDL.MaxPressureReading / 20);	// 5% hysteresis below trip

	if (PressureGateActive)
	{
		// Currently relieving - release only after the min hold AND pressure clearly below trip.
		if (millis() - PressureGateStart >= PressureMinHold && PressureReading < releaseLevel)
		{
			PressureGateActive = false;
		}
	}
	else
	{
		// Not relieving - trip on over-pressure.
		if (PressureReading > MDL.MaxPressureReading)
		{
			PressureGateActive = true;
			PressureGateStart = millis();

			// Count trips toward the hard latch: a re-trip within PressureTripResetMs
			// of the previous one is the same ongoing fault; a longer quiet spell
			// means recovery happened and earlier trips are forgiven. (A window
			// anchored at the first trip could never latch on slow steady cycling -
			// the count reset every time the window expired.)
			if (millis() - PressureTripLast > PressureTripResetMs) PressureTripCount = 0;
			PressureTripLast = millis();
			PressureTripCount++;

			// Repeated trips = persistent fault -> hard latch (require operator reset).
			if (PressureTripCount >= PressureMaxTrips) PressureGateLatched = true;
		}
	}
}
