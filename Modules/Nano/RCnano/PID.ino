
// based off of https://github.com/br3ttb/Arduino-PID-Library

uint32_t LastCheck[MaxProductCount];
const double SampleTime = 50;
const double Deadband = 0.04;		// % error below which no adjustment is made
const double BrakePoint = 0.20;		// % error below which reduced adjustment is used
const double BrakeSet = 0.75;		// low adjustment rate factor
double SF;							// Settings Factor used to reduce adjustment when close to target rate
double DifValue;					// differential value on UPM

double RateError;
double LastPWM[MaxProductCount];
double IntegralSum[MaxProductCount];
double LastUPM[MaxProductCount];

const byte AdjustTime = 15;
const byte PauseTime = 300;
bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];

void SetPWM()
{
	if (AutoOn)
	{
		// auto control
		for (int i = 0; i < MDL.SensorCount; i++)
		{
			switch (Sensor[i].ControlType)
			{
			case 2:
			case 3:
			case 4:
				// motor control
				Sensor[i].PWM = PIDmotor(i);
				break;

			case 5:
				// combo close timed adjustment
				Sensor[i].PWM = TimedCombo(i, false);
				break;

			default:
				// valve control
				Sensor[i].PWM = PIDvalve(i, false);
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
				debug1 = Sensor[i].PWM;
				debug2++;
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

			// check deadband
			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				IntegralSum[ID] += Sensor[ID].KI * RateError;
				IntegralSum[ID] *= (Sensor[ID].KI > 0);	// zero out if not using KI

				DifValue = Sensor[ID].KD * (LastUPM[ID] - Sensor[ID].UPM);
				Result += Sensor[ID].KP * SF * RateError + IntegralSum[ID] + DifValue;

				if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM;
				if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;
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

int PIDvalve(byte ID, bool SkipSampleTime)
{
	double Result = 0;
	if (Sensor[ID].FlowEnabled && Sensor[ID].TargetUPM > 0)
	{
		Result = LastPWM[ID];
		if ((millis() - LastCheck[ID] >= SampleTime)||SkipSampleTime)
		{
			LastCheck[ID] = millis();

			RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;
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

			// check deadband
			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				IntegralSum[ID] += Sensor[ID].KI * RateError;
				IntegralSum[ID] *= (Sensor[ID].KI > 0);	// zero out if not using KI

				DifValue = Sensor[ID].KD * (LastUPM[ID] - Sensor[ID].UPM);
				Result = Sensor[ID].MinPWM + Sensor[ID].KP * SF * RateError + IntegralSum[ID] + DifValue;

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

int TimedCombo(byte ID, bool ManualAdjust)
{
	int Result = 0;
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
			// set pwm
			if (ManualAdjust)
			{
				Result = 255;
				if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM;
				if (Sensor[ID].ManualAdjust < 0) Result *= -1.0;
			}
			else
			{
				Result = PIDvalve(ID, true);
			}
		}
	}
	return Result;
}

int TimedComboOld(byte ID, bool ManualAdjust = false)
{
	int Result = 0;
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
			RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;
			double Direction = 1.0;
			if (RateError < 0) Direction = -1.0;

			if (abs(RateError) > BrakePoint * Sensor[ID].TargetUPM)
			{
				SF = 1;
			}
			else
			{
				SF = BrakeSet;
			}

			if (ManualAdjust)
			{
				Result = 255;
			}
			else
			{
				// auto adjust, check deadband
				if (Sensor[ID].TargetUPM > 0)
				{
					if (abs(RateError / Sensor[ID].TargetUPM) > Deadband)  Result = 255;
				}
			}
			Result *= SF;
			if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM * SF;
			if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;
			Result *= Direction;
			debug1 = Result;
			debug2 = SF;
		}
	}
	return Result;
}

