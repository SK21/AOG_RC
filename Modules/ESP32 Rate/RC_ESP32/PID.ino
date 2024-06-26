
// based off of https://github.com/br3ttb/Arduino-PID-Library

uint32_t LastCheck[MaxProductCount];
const double SampleTime = 50;
const double Deadband = 0.02;		// % error below which no adjustment is made
const double BrakePoint = 0.20;		// % error below which reduced adjustment is used
const double BrakeSet = 0.75;		// low adjustment rate
double SF;							// Settings Factor used to reduce adjustment when close to set rate
double DifValue;					// differential value on UPM

double RateError;
double LastPWM[MaxProductCount];
double IntegralSum[MaxProductCount];
double LastUPM[MaxProductCount];

bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
byte AdjustTime;
byte PauseTime;
const double MinStart = 0.03;	// minimum start ratio. Used to quickly increase rate from off.

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
				AdjustTime = 20;
				PauseTime = 200;
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
				AdjustTime = 100;
				PauseTime = 50;
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
				if (abs(RateError) > Sensor[ID].TargetUPM)
				{
					if (RateError > 0)
					{
						RateError = Sensor[ID].TargetUPM;
					}
					else
					{
						RateError = Sensor[ID].TargetUPM * -1;
					}
				}

				// check brakepoint
				if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
				{
					SF = 1.0;
				}
				else
				{
					SF = BrakeSet;
				}

				IntegralSum[ID] += Sensor[ID].KI * RateError / 1000.0;
				IntegralSum[ID] *= (Sensor[ID].KI > 0);	// zero out if not using KI

				DifValue = Sensor[ID].KD * (LastUPM[ID] - Sensor[ID].UPM) * 10.0;
				Result += Sensor[ID].KP * SF * RateError + IntegralSum[ID] + DifValue;

				if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM;
				if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;
			}
			LastUPM[ID] = Sensor[ID].UPM;
		}
		LastPWM[ID] = Result;
	}
	else
	{
		IntegralSum[ID] = 0;
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
				if (abs(RateError) > Sensor[ID].TargetUPM)
				{
					if (RateError > 0)
					{
						RateError = Sensor[ID].TargetUPM;
					}
					else
					{
						RateError = Sensor[ID].TargetUPM * -1;
					}
				}

				// check brakepoint
				if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
				{
					SF = 1;
				}
				else
				{
					SF = BrakeSet;
				}

				IntegralSum[ID] += Sensor[ID].KI * RateError / 1000.0;
				IntegralSum[ID] *= (Sensor[ID].KI > 0);	// zero out if not using KI

				DifValue = Sensor[ID].KD * (LastUPM[ID] - Sensor[ID].UPM) * 10.0;
				Result = Sensor[ID].KP * SF * RateError + IntegralSum[ID] + DifValue;

				bool IsPositive = (Result > 0);
				Result = abs(Result);

				if (Result > Sensor[ID].MaxPWM * SF) Result = Sensor[ID].MaxPWM * SF;
				if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;

				if (!IsPositive) Result *= -1.0;
			}
			else
			{
				Result = 0;
			}
			LastUPM[ID] = Sensor[ID].UPM;
		}
	}
	else
	{
		IntegralSum[ID] = 0;
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
			if (millis() - ComboTime[ID] > PauseTime)
			{
				// switch state
				ComboTime[ID] = millis();
				PauseAdjust[ID] = !PauseAdjust[ID];
			}
		}
		else
		{
			// adjusting state
			if (millis() - ComboTime[ID] > AdjustTime)
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
						if (abs(RateError) > Sensor[ID].TargetUPM)
						{
							if (RateError > 0)
							{
								RateError = Sensor[ID].TargetUPM;
							}
							else
							{
								RateError = Sensor[ID].TargetUPM * -1;
							}
						}

						// check brakepoint
						if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
						{
							SF = 1;
						}
						else
						{
							SF = BrakeSet;
						}

						IntegralSum[ID] += Sensor[ID].KI * RateError / 1000.0;
						IntegralSum[ID] *= (Sensor[ID].KI > 0);	// zero out if not using KI

						DifValue = Sensor[ID].KD * (LastUPM[ID] - Sensor[ID].UPM) * 10.0;
						Result = Sensor[ID].KP * SF * RateError + IntegralSum[ID] + DifValue;

						bool IsPositive = (Result > 0);
						Result = abs(Result);

						if (Result > Sensor[ID].MaxPWM * SF) Result = Sensor[ID].MaxPWM * SF;
						if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;

						if (!IsPositive) Result *= -1;
					}
					else
					{
						Result = 0;
					}
				}
			}
		}
	}
	else
	{
		IntegralSum[ID] = 0;
	}
	return (int)Result;
}
