
// based off of https://github.com/br3ttb/Arduino-PID-Library

uint32_t LastCheck[MaxProductCount];
const double SampleTime = 50;
const double Deadband = 0.03;		// % error below which no adjustment is made

const double BrakePoint = 0.3;		// % error below which reduced adjustment is used
const double BrakeSet = 0.75;		// low adjustment rate

double RateError;
double LastPWM[MaxProductCount];

bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
const double MinStart = 0.03;	// minimum start ratio. Used to quickly increase rate from 0.
int TimedAdjustTime = 80;		// milliseconds
int TimedPauseTime = 400;		// milliseconds

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
			case 3:
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
				double Direction = 1.0;
				if (Sensor[i].PWM < 0) Direction = -1.0;
				if (abs(Sensor[i].PWM) > Sensor[i].MaxPWM) Sensor[i].PWM = Sensor[i].MaxPWM * Direction;
				break;
			}
		}
	}
}

int PIDmotor(byte ID)
{
	double Result = 0;
	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= SampleTime)
		{
			LastCheck[ID] = millis();

			RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			// check deadband
			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				// check brakepoint
				if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
				{
					Result += Sensor[ID].KP * RateError;
				}
				else
				{
					Result += Sensor[ID].KP * RateError * BrakeSet;
				}

				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
			}
		}
		LastPWM[ID] = Result;
	}

	return (int)Result;
}

int PIDvalve(byte ID)
{
	double Result = 0;
	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if (millis() - LastCheck[ID] >= SampleTime)
		{
			LastCheck[ID] = millis();

			RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			// check deadband
			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				// check brakepoint
				if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
				{
					Result = Sensor[ID].KP * RateError;
				}
				else
				{
					Result = Sensor[ID].KP * RateError * BrakeSet;
				}

				bool IsPositive = (Result > 0);
				Result = abs(Result);
				Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
				if (!IsPositive) Result *= -1.0;
			}
			else
			{
				Result = 0;
			}
		}
	}

	LastPWM[ID] = Result;
	return (int)Result;
}

int TimedCombo(byte ID, bool ManualAdjust = false)
{
	double Result = 0;
	if ((Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0) || ManualAdjust)
	{
		if (Sensor[ID].UPM < (MinStart * Sensor[ID].TargetUPM))
		{
			// no pause when rate near 0
			ComboTime[ID] = millis();
			PauseAdjust[ID] = false;
		}

		if (PauseAdjust[ID])
		{
			// pausing state
			if (millis() - ComboTime[ID] > TimedPauseTime)
			{
				// switch state
				ComboTime[ID] = millis();
				PauseAdjust[ID] = !PauseAdjust[ID];
			}
		}
		else
		{
			// adjusting state
			if (millis() - ComboTime[ID] > TimedAdjustTime)
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
					double Direction = 1.0;
					if (Result < 0) Direction = -1.0;
					if (abs(Result) > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM * Direction;
				}
				else
				{
					// auto adjust
					RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

					// check deadband
					if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
					{
						RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

						// check brakepoint
						if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
						{
							Result = Sensor[ID].KP * RateError;
						}
						else
						{
							Result = Sensor[ID].KP * RateError * BrakeSet;
						}

						bool IsPositive = (Result > 0);
						Result = abs(Result);
						Result = constrain(Result, Sensor[ID].MinPWM, Sensor[ID].MaxPWM);
						if (!IsPositive) Result *= -1.0;
					}
					else
					{
						Result = 0;
					}
				}
			}
		}
	}
	return (int)Result;
}
