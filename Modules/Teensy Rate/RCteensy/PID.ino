// MaxPWM           maximum pwm value
// MinPWM           minimum pwm value
// KP               proportional adjustment
// KI               integral adjustment
// Deadband         error % below which no adjustment is made, actual X 10
// BrakePoint       error % where adjustment rate changes between 100% and the slow rate %
// PIDslowAdjust    slow rate %
// SlewRate         slew rate limit. Max total pwm change per loop. 
// MaxIntegral		max integral pwm change per loop. Ex: 0.1 = max 2 pwm/sec change at 50 ms sample time, actual X 10
// TimedMinStart    minimum start ratio %. Used to quickly increase from 0 for a timed combo valve.
// TimedAdjust      time in ms where there is adjustment of the combo valve.
// TimedPause       time in ms where there is no adjustment of the combo valve.
// PIDtime          time interval in ms the pid runs

bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
uint32_t LastCheck[MaxProductCount];
float LastPWM[MaxProductCount] = { 0 };
bool ErrorIsPositive[MaxProductCount] = { true };
float RefUPM[MaxProductCount] = { 0 };	// section-independent reference flow for gain normalization (peak target this run)

void DoPID()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (AutoOn)
		{
			// auto control
			switch (Sensor[i].ControlType)
			{
			case Motor_ct:
			case Fan_ct:
				Sensor[i].PWM = PIDmotor(i);
				break;

			case TimedCombo_ct:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, false);
				break;

			default:
				// valve
				Sensor[i].PWM = PIDvalve(i);
				break;
			}
		}
		else
		{
			// manual control
			switch (Sensor[i].ControlType)
			{
			case TimedCombo_ct:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, true);
				break;

			default:
				Sensor[i].PWM = Sensor[i].ManualAdjust;
				if (fabsf(Sensor[i].PWM) > Sensor[i].MaxPWM) Sensor[i].PWM = Sensor[i].MaxPWM * ((Sensor[i].PWM >= 0.0) ? 1.0 : -1.0);
				break;
			}
		}
	}
}

float PIDvalve(byte ID)
{
	// Normalized PID: error is a fraction of a section-independent REFERENCE flow, output a
	// fraction of PWM authority, so Kp/Ki are dimensionless and one UI scale fits any hardware.
	// Reference = peak target this run (NOT the live target): dividing by live target made loop
	// gain ~1/target, so closing sections multiplied the gain and caused oscillation. Peak-target
	// reference keeps gain constant as sections close while staying identical at full sections.
	float Result = 0;

	if (PIDenabled[ID])
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= Sensor[ID].PIDtime)
		{
			LastCheck[ID] = millis();

			if (Sensor[ID].TargetUPM > RefUPM[ID]) RefUPM[ID] = Sensor[ID].TargetUPM;	// latch peak target as the fixed gain reference

			float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;
			DiagError[ID] = RateError;	// raw error before deadband/constrain, for PGN 32402 logging
			DiagTarget[ID] = Sensor[ID].TargetUPM;	// snapshot with the same UPM used for the error
			DiagApplied[ID] = Sensor[ID].UPM;
			DiagSamples[ID] = MedianCount[ID];	// pulse samples behind this UPM
			DiagChange[ID] = 0.0f;

			// Reset the integral only on a genuine overshoot - the rate must clearly cross to
			// the other side of target (beyond the deadband). Hysteresis stops small target
			// wobble near the setpoint from repeatedly wiping accumulated correction.
			float ResetBand = Sensor[ID].Deadband * Sensor[ID].TargetUPM;
			if (RateError > ResetBand && !ErrorIsPositive[ID])
			{
				ErrorIsPositive[ID] = true;
				IntegralSum[ID] = 0;
			}
			else if (RateError < -ResetBand && ErrorIsPositive[ID])
			{
				ErrorIsPositive[ID] = false;
				IntegralSum[ID] = 0;
			}

			if (fabsf(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				// RefUPM is the section-independent reference (>= TargetUPM > 0), so this is safe and
				// gives constant gain across section states. At full sections RefUPM == TargetUPM.
				float FracError = RateError / RefUPM[ID];	// bounded to [-1, 1]; smaller at reduced sections -> gentler, not hotter
				float Authority = Sensor[ID].MaxPWM - Sensor[ID].MinPWM;

				// Conditional integration (anti-windup): only accumulate inside the brakepoint band
				// (near target). Far from target the P term drives the approach; integrating there
				// winds up and causes capture overshoot plus extra over-travel ("excessive drop") on
				// large setpoint steps such as a quick multi-section close.
				bool nearTarget = (fabsf(FracError) <= Sensor[ID].BrakePoint / 100.0);
				if (nearTarget)
				{
					IntegralSum[ID] += constrain(FracError * Sensor[ID].Ki * Authority, -1 * Sensor[ID].MaxIntegral, Sensor[ID].MaxIntegral);	// MaxIntegral limits integral change per loop (PWM units)
				}
				IntegralSum[ID] *= (Sensor[ID].Ki > 0);	// zero out if not using integral
				IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Authority, Authority);  // total bounded by pwm range so integral can reach full authority (e.g. to free a stuck valve)

				// BrakeScale is a 0-1 multiplier (replaces FastAdjustValve=40): full authority when far
				// from target, scaled down inside the brakepoint band.
				float BrakeScale = nearTarget ? Sensor[ID].PIDslowAdjust / 100.0 : 1.0;

				float ChangeAmount = FracError * Sensor[ID].Kp * BrakeScale * Authority + IntegralSum[ID];
				DiagChange[ID] = ChangeAmount;

				if (fabsf(ChangeAmount) < 0.1)
				{
					Result = 0.0f;
				}
				else
				{
					Result = constrain(fabsf(ChangeAmount) + Sensor[ID].MinPWM, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
					Result *= (ChangeAmount >= 0.0f) ? 1.0f : -1.0f;
				}
			}
			else
			{
				Result = 0.0f;
				IntegralSum[ID] = 0.0f;
			}

			DiagIntegral[ID] = IntegralSum[ID];
			DiagPWM[ID] = Result;	// final computed PWM for this sample
			DiagMillis[ID] = LastCheck[ID];
			PidSampleReady[ID] = true;
		}
	}
	else
	{
		IntegralSum[ID] = 0;
		RefUPM[ID] = 0;	// re-latch the reference on the next enabled run
	}

	LastPWM[ID] = Result;
	return Result;
}

float PIDmotor(byte ID)
{
	float Result = 0;

	if (PIDenabled[ID])
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= Sensor[ID].PIDtime)
		{
			LastCheck[ID] = millis();

			if (Sensor[ID].TargetUPM > RefUPM[ID]) RefUPM[ID] = Sensor[ID].TargetUPM;	// latch peak target as the fixed gain reference

			float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;
			DiagError[ID] = RateError;	// raw error before deadband/constrain, for PGN 32402 logging
			DiagTarget[ID] = Sensor[ID].TargetUPM;	// snapshot with the same UPM used for the error
			DiagApplied[ID] = Sensor[ID].UPM;
			DiagSamples[ID] = MedianCount[ID];	// pulse samples behind this UPM
			DiagChange[ID] = 0.0f;

			// Reset integral only on a genuine overshoot (rate clearly crosses target beyond
			// the deadband); hysteresis ignores small target wobble near the setpoint.
			float ResetBand = Sensor[ID].Deadband * Sensor[ID].TargetUPM;
			if (RateError > ResetBand && !ErrorIsPositive[ID])
			{
				ErrorIsPositive[ID] = true;
				IntegralSum[ID] = 0;
			}
			else if (RateError < -ResetBand && ErrorIsPositive[ID])
			{
				ErrorIsPositive[ID] = false;
				IntegralSum[ID] = 0;
			}

			if (fabsf(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				// RefUPM is the section-independent reference (>= TargetUPM > 0), so this is safe and
				// gives constant gain across section states. At full sections RefUPM == TargetUPM.
				float FracError = RateError / RefUPM[ID];	// bounded to [-1, 1]; smaller at reduced sections -> gentler, not hotter
				float Authority = Sensor[ID].MaxPWM - Sensor[ID].MinPWM;

				// Conditional integration (anti-windup): only accumulate inside the brakepoint band
				// (near target). Far from target the P term drives the approach; integrating there
				// winds up and causes capture overshoot plus extra over-travel ("excessive drop") on
				// large setpoint steps such as a quick multi-section close.
				bool nearTarget = (fabsf(FracError) <= Sensor[ID].BrakePoint / 100.0);
				if (nearTarget)
				{
					IntegralSum[ID] += constrain(FracError * Sensor[ID].Ki * Authority, -1 * Sensor[ID].MaxIntegral, Sensor[ID].MaxIntegral);	// MaxIntegral limits integral change per loop (PWM units)
				}
				IntegralSum[ID] *= (Sensor[ID].Ki > 0);	// zero out if not using integral
				IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Authority, Authority);  // total bounded by pwm range so integral can reach full authority (e.g. to free a stuck valve)

				// BrakeScale is a 0-1 multiplier (replaces FastAdjustMotor): full authority when far
				// from target, scaled down inside the brakepoint band.
				float BrakeScale = nearTarget ? Sensor[ID].PIDslowAdjust / 100.0 : 1.0;

				float ChangeAmount = FracError * Sensor[ID].Kp * BrakeScale * Authority + IntegralSum[ID];
				ChangeAmount = constrain(ChangeAmount, -1 * Sensor[ID].SlewRate, Sensor[ID].SlewRate);
				DiagChange[ID] = ChangeAmount;	// slew-limited per-loop change actually applied

				Result += ChangeAmount;
				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
				LastPWM[ID] = Result;
			}
			else
			{
				IntegralSum[ID] = 0.0f;
			}

			DiagIntegral[ID] = IntegralSum[ID];
			DiagPWM[ID] = Result;	// final computed PWM for this sample
			DiagMillis[ID] = LastCheck[ID];
			PidSampleReady[ID] = true;
		}
	}
	else
	{
		IntegralSum[ID] = 0;
		RefUPM[ID] = 0;	// re-latch the reference on the next enabled run
	}

	return Result;
}

float TimedCombo(byte ID, bool ManualAdjust = false)
{
	float Result = 0;
	if (PIDenabled[ID] || ManualAdjust)
	{
		if (Sensor[ID].UPM < (Sensor[ID].TimedMinStart * Sensor[ID].TargetUPM))
		{
			// no pause when rate near 0
			ComboTime[ID] = millis();
			PauseAdjust[ID] = false;
		}

		if (PauseAdjust[ID])
		{
			// pausing state
			if (millis() - ComboTime[ID] > Sensor[ID].TimedPause)
			{
				// switch state
				ComboTime[ID] = millis();
				PauseAdjust[ID] = !PauseAdjust[ID];
			}
		}
		else
		{
			// adjusting state
			if (millis() - ComboTime[ID] > Sensor[ID].TimedAdjust)
			{
				// detect oscillation at end of adjust burst
				bool aboveTarget = (Sensor[ID].UPM > Sensor[ID].TargetUPM);
				if (aboveTarget != LastAboveTarget[ID])
				{
					OscDamp[ID] = max(OscDamp[ID] * 0.7f, 0.1f);
				}
				else
				{
					OscDamp[ID] = min(OscDamp[ID] * 1.1f, 1.0f);
				}
				LastAboveTarget[ID] = aboveTarget;

				// switch state
				ComboTime[ID] = millis();
				PauseAdjust[ID] = !PauseAdjust[ID];
			}
			else
			{
				if (ManualAdjust)
				{
					Result = Sensor[ID].ManualAdjust;
					if (fabsf(Result) > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM * ((Result >= 0.0) ? 1.0 : -1.0);
				}
				else
				{
					// auto adjust
					Result = PIDvalve(ID) * OscDamp[ID];
				}
			}
		}
	}
	else
	{
		IntegralSum[ID] = 0;
		OscDamp[ID] = 1.0f;
	}
	return Result;
}
