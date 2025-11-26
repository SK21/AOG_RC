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

const float FastAdjustMotor = 1.0;
const float FastAdjustValve = 40.0;
bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
uint32_t LastCheck[MaxProductCount];
float LastPWM[MaxProductCount] = { 0 };
float IntegralSum[MaxProductCount];
bool ErrorIsPositive[MaxProductCount] = { true };

void SetPWM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (Sensor[i].AutoOn)
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
	float Result = 0;

	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= Sensor[ID].PIDtime)
		{
			LastCheck[ID] = millis();

			float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			bool IsPositive = (RateError > 0);
			if (IsPositive != ErrorIsPositive[ID])
			{
				// prevent integral overshoot on zero crossing
				ErrorIsPositive[ID] = IsPositive;
				IntegralSum[ID] = 0;
			}

			if (fabsf(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				IntegralSum[ID] += RateError * Sensor[ID].Ki;
				IntegralSum[ID] *= (Sensor[ID].Ki > 0);	// zero out if not using integral
				IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxIntegral, Sensor[ID].MaxIntegral);  // max total integral pwm

				float BrakeFactor = (fabsf(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint / 100.0) ? FastAdjustValve : Sensor[ID].PIDslowAdjust / 100.0 * FastAdjustValve;

				float ChangeAmount = RateError * Sensor[ID].Kp * BrakeFactor * 100.0 + IntegralSum[ID];

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
		}
	}
	else
	{
		IntegralSum[ID] = 0;
	}

	LastPWM[ID] = Result;
	return Result;
}

float PIDmotor(byte ID)
{
	float Result = 0;

	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= Sensor[ID].PIDtime)
		{
			LastCheck[ID] = millis();

			float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			bool IsPositive = (RateError > 0);
			if (IsPositive != ErrorIsPositive[ID])
			{
				// prevent integral overshoot on zero crossing
				ErrorIsPositive[ID] = IsPositive;
				IntegralSum[ID] = 0;
			}

			if (fabsf(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				IntegralSum[ID] += RateError * Sensor[ID].Ki;
				IntegralSum[ID] *= (Sensor[ID].Ki > 0);	// zero out if not using integral
				IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxIntegral, Sensor[ID].MaxIntegral);  // max total integral pwm

				float BrakeFactor = (fabsf(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint / 100.0) ? FastAdjustMotor : Sensor[ID].PIDslowAdjust / 100.0 * FastAdjustMotor;

				float ChangeAmount = RateError * Sensor[ID].Kp * BrakeFactor * 100.0 + IntegralSum[ID];
				ChangeAmount = constrain(ChangeAmount, -1 * Sensor[ID].SlewRate, Sensor[ID].SlewRate);

				Result += ChangeAmount;
				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
				LastPWM[ID] = Result;
			}
			else
			{
				IntegralSum[ID] = 0.0f;
			}
		}
	}
	else
	{
		IntegralSum[ID] = 0;
	}

	return Result;
}

float TimedCombo(byte ID, bool ManualAdjust = false)
{
	float Result = 0;
	if ((Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0) || ManualAdjust)
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
					Result = PIDvalve(ID);
				}
			}
		}
	}
	else
	{
		IntegralSum[ID] = 0;
	}
	return Result;
}
