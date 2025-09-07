// MaxPWM           maximum pwm value
// MinPWM           minimum pwm value
// KP               proportional adjustment
// KI               integral adjustment
// Deadband         error % below which no adjustment is made, actual X 10
// BrakePoint       error % where adjustment rate changes between 100% and the slow rate %
// PIDslowAdjust    slow rate %
// SlewRate         slew rate limit. Max total pwm change per loop. Used for motor only.
// MaxMotorIntegral max integral pwm change per loop. Ex: 0.1 = max 2 pwm/sec change at 50 ms sample time, actual X 10
// MaxValveIntegral max total integral pwm change per loop for valve
// TimedMinStart    minimum start ratio %. Used to quickly increase from 0 for a timed combo valve.
// TimedAdjust      time in ms where there is adjustment of the combo valve.
// TimedPause       time in ms where there is no adjustment of the combo valve.
// PIDtime          time interval in ms the pid runs

const float FastAdjust = 100;				// fast adjustment factor
bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
uint32_t LastCheck[MaxProductCount];
float LastPWM[MaxProductCount];
float IntegralSum[MaxProductCount];

void SetPWM()
{
	if (AutoOn)
	{
		// auto control
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			switch (Sensor[i].ControlType)
			{
			case 5:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, false);
				break;

			case 2:
			case 4:
				// motor control
				Sensor[i].PWM = PIDmotor(i);
				break;

			default:
				// valve control
				Sensor[i].PWM = PIDvalve(i);
				break;
			}
		}
	}
	else
	{
		// manual control
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			switch (Sensor[i].ControlType)
			{
			case 5:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, true);
				break;

			default:
				Sensor[i].PWM = Sensor[i].ManualAdjust;
				float Direction = 1.0;
				if (Sensor[i].PWM < 0) Direction = -1.0;
				if (abs(Sensor[i].PWM) > Sensor[i].MaxPWM) Sensor[i].PWM = Sensor[i].MaxPWM * Direction;
				break;
			}
		}
	}
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

			// check deadband
			if (abs(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				if (Sensor[ID].Ki > 0)
				{
					IntegralSum[ID] += RateError * Sensor[ID].Ki;
					IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxMotorIntegral, Sensor[ID].MaxMotorIntegral);
				}
				else
				{
					IntegralSum[ID] = 0;
				}

				// check brakepoint
				float BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint) ? FastAdjust : Sensor[ID].PIDslowAdjust;

				// slew rate limit
				float Change = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
				Change = constrain(Change, -1 * Sensor[ID].SlewRate, Sensor[ID].SlewRate);
				Result += Change;
				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
			}
			else
			{
				IntegralSum[ID] = 0;
			}
		}
		LastPWM[ID] = Result;
	}
	else
	{
		IntegralSum[ID] = 0;
	}
	return Result;
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

			if (abs(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, -Sensor[ID].TargetUPM, Sensor[ID].TargetUPM);

				if (Sensor[ID].Ki > 0)
				{
					IntegralSum[ID] += RateError * Sensor[ID].Ki;
					IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxValveIntegral, Sensor[ID].MaxValveIntegral);  // max total integral pwm
				}
				else
				{
					IntegralSum[ID] = 0;
				}

				float BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint) ? FastAdjust : Sensor[ID].PIDslowAdjust;

				float Control = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
				float Sign = (Control >= 0) ? 1.0 : -1.0;

				Result = abs(Control) + Sensor[ID].MinPWM;
				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
				Result *= Sign;
			}
			else
			{
				Result = 0;
				IntegralSum[ID] = 0;
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
					if (abs(Result) > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM * Direction;
				}
				else
				{
					// auto adjust
					float RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

					// check deadband
					if (abs(RateError) > Sensor[ID].Deadband * Sensor[ID].TargetUPM)
					{
						RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

						if (Sensor[ID].Ki > 0)
						{
							IntegralSum[ID] += RateError * Sensor[ID].Ki;
							IntegralSum[ID] = constrain(IntegralSum[ID], -1 * Sensor[ID].MaxValveIntegral, Sensor[ID].MaxValveIntegral);  // max total integral pwm
						}
						else
						{
							IntegralSum[ID] = 0;
						}

						// check brakepoint
						float BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * Sensor[ID].BrakePoint) ? FastAdjust : Sensor[ID].PIDslowAdjust;

						float Control = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
						float Sign = (Control >= 0) ? 1.0 : -1.0;

						Result = abs(Control) + Sensor[ID].MinPWM;
						Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
						Result *= Sign;
					}
					else
					{
						Result = 0;
						IntegralSum[ID] = 0;
					}
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
