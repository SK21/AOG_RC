// MaxPWM           maximum pwm value
// MinPWM           minimum pwm value
// KP               proportional adjustment
// KI               integral adjustment
// Deadband         error % below which no adjustment is made, actual X 10
// BrakePoint       error % where adjustment rate changes between 100% and the slow rate %
// PIDslowAdjust    slow rate %
// SlewRate         slew rate limit. Max total pwm change per loop. Used for motor only.
// MaxIntegral		max integral pwm change per loop. Ex: 0.1 = max 2 pwm/sec change at 50 ms sample time, actual X 10
// TimedMinStart    minimum start ratio %. Used to quickly increase from 0 for a timed combo valve.
// TimedAdjust      time in ms where there is adjustment of the combo valve.
// TimedPause       time in ms where there is no adjustment of the combo valve.
// PIDtime          time interval in ms the pid runs

const float NormalAdjust = 1.0;				
bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
uint32_t LastCheck[MaxProductCount];
float LastPWM[MaxProductCount];
float IntegralSum[MaxProductCount];

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
				Sensor[i].PWM = DoPID(i, true);
				break;

			case TimedCombo_ct:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, false);
				break;

			default:
				// valve
				Sensor[i].PWM = DoPID(i, false);
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
				float Direction = 1.0;
				if (Sensor[i].PWM < 0) Direction = -1.0;
				if (fabsf(Sensor[i].PWM) > Sensor[i].MaxPWM) Sensor[i].PWM = Sensor[i].MaxPWM * Direction;
				break;
			}
		}
	}
}
float DoPID(byte ID, bool IsMotor)
{
	float Result = 0;

	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= Sensor[ID].PIDtime)
		{
			LastCheck[ID] = millis();

			float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			if (fabsf(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				if (Sensor[ID].Ki > 0)
				{
					IntegralSum[ID] += RateError * Sensor[ID].Ki;
					IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxIntegral, Sensor[ID].MaxIntegral);  // max total integral pwm
				}
				else
				{
					IntegralSum[ID] = 0;
				}

				float BrakeFactor = (fabsf(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint) ? NormalAdjust : Sensor[ID].PIDslowAdjust;

				float Change = RateError * Sensor[ID].Kp * BrakeFactor * 100.0 + IntegralSum[ID];
				Change = constrain(Change, -1 * Sensor[ID].SlewRate, Sensor[ID].SlewRate);

				if (IsMotor)
				{
					Result += Change;
					Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
				}
				else
				{
					// valve
					float Sign = (Change >= 0.0f) ? 1.0f : -1.0f;
					Result = fabsf(Change) + Sensor[ID].MinPWM;
					Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
					Result *= Sign;
				}
				LastPWM[ID] = Result;
			}
			else
			{
				if (!IsMotor)
				{
					Result = 0.0f;
					LastPWM[ID] = 0;
				}
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
					float Direction = 1.0;
					if (Result < 0) Direction = -1.0;
					if (fabsf(Result) > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM * Direction;
				}
				else
				{
					// auto adjust
					Result = DoPID(ID, false);
				}
			}
		}
	}
	return Result;
}
