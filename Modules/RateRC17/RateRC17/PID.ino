
// based off of https://github.com/br3ttb/Arduino-PID-Library

uint32_t LastCheck;
const double SampleTime = 10;
const double Deadband = 0.03;		// % error below which no adjustment is made

const double BrakePoint = 0.3;		// % error below which reduced adjustment is used
const double BrakeSet = 0.75;		// low adjustment rate

double RateError;
double LastPWM;

bool PauseAdjust;
uint32_t ComboTime;
const double MinStart = 0.03;	// minimum start ratio. Used to quickly increase rate from 0.
int TimedAdjustTime = 80;		// milliseconds
int TimedPauseTime = 400;		// milliseconds

void SetPWM()
{
	if (AutoOn)
	{
		// auto control
		switch (FlowSensor.ControlType)
		{
		case 5:
			// combo close timed adjustment
			FlowSensor.PWM = TimedCombo(false);
			break;

		case 2:
		case 3:
		case 4:
			// motor control
			FlowSensor.PWM = PIDmotor();
			break;

		default:
			// valve control
			FlowSensor.PWM = PIDvalve();
			break;
		}
	}
	else
	{
		// manual control
		switch (FlowSensor.ControlType)
		{
		case 5:
			// combo close timed adjustment
			FlowSensor.PWM = TimedCombo(true);
			break;

		default:
			FlowSensor.PWM = FlowSensor.ManualAdjust;
			double Direction = 1.0;
			if (FlowSensor.PWM < 0) Direction = -1.0;
			if (abs(FlowSensor.PWM) > FlowSensor.MaxPWM) FlowSensor.PWM = FlowSensor.MaxPWM * Direction;
			break;
		}
	}
}

int PIDmotor()
{
	double Result = 0;
	if (FlowSensor.FlowEnabled && FlowSensor.TargetUPM > 0)
	{
		Result = LastPWM;
		if (millis() - LastCheck >= SampleTime)
		{
			LastCheck = millis();

			RateError = FlowSensor.TargetUPM - FlowSensor.UPM;

			// check deadband
			if (abs(RateError) > Deadband * FlowSensor.TargetUPM)
			{
				RateError = constrain(RateError, FlowSensor.TargetUPM * -1, FlowSensor.TargetUPM);

				// check brakepoint
				if (abs(RateError) > BrakePoint * FlowSensor.TargetUPM)
				{
					Result += FlowSensor.KP * RateError;
				}
				else
				{
					Result += FlowSensor.KP * RateError * BrakeSet;
				}

				Result = constrain(Result, FlowSensor.MinPWM, FlowSensor.MaxPWM);
			}
		}
		LastPWM = Result;
	}

	return (int)Result;
}

int PIDvalve()
{
	double Result = 0;
	if (FlowSensor.FlowEnabled && FlowSensor.TargetUPM > 0)
	{
		Result = LastPWM;
		if (millis() - LastCheck >= SampleTime)
		{
			LastCheck = millis();

			RateError = FlowSensor.TargetUPM - FlowSensor.UPM;

			// check deadband
			if (abs(RateError) > Deadband * FlowSensor.TargetUPM)
			{
				RateError = constrain(RateError, FlowSensor.TargetUPM * -1, FlowSensor.TargetUPM);

				// check brakepoint
				if (abs(RateError) > BrakePoint * FlowSensor.TargetUPM)
				{
					Result = FlowSensor.KP * RateError;
				}
				else
				{
					Result = FlowSensor.KP * RateError * BrakeSet;
				}

				bool IsPositive = (Result > 0);
				Result = abs(Result);
				Result = constrain(Result, FlowSensor.MinPWM, FlowSensor.MaxPWM);
				if (!IsPositive) Result *= -1.0;
			}
			else
			{
				Result = 0;
			}
		}
	}

	LastPWM = Result;
	return (int)Result;
}

int TimedCombo(bool ManualAdjust = false)
{
	double Result = 0;
	if ((FlowSensor.FlowEnabled && FlowSensor.TargetUPM > 0) || ManualAdjust)
	{
		if (FlowSensor.UPM < (MinStart * FlowSensor.TargetUPM))
		{
			// no pause when rate near 0
			ComboTime = millis();
			PauseAdjust = false;
		}

		if (PauseAdjust)
		{
			// pausing state
			if (millis() - ComboTime > TimedPauseTime)
			{
				// switch state
				ComboTime = millis();
				PauseAdjust = !PauseAdjust;
			}
		}
		else
		{
			// adjusting state
			if (millis() - ComboTime > TimedAdjustTime)
			{
				// switch state
				ComboTime = millis();
				PauseAdjust = !PauseAdjust;
			}
			else
			{
				if (ManualAdjust)
				{
					Result = FlowSensor.ManualAdjust;
					double Direction = 1.0;
					if (Result < 0) Direction = -1.0;
					if (abs(Result) > FlowSensor.MaxPWM) Result = FlowSensor.MaxPWM * Direction;
				}
				else
				{
					// auto adjust
					RateError = FlowSensor.TargetUPM - FlowSensor.UPM;

					// check deadband
					if (abs(RateError) > Deadband * FlowSensor.TargetUPM)
					{
						RateError = constrain(RateError, FlowSensor.TargetUPM * -1, FlowSensor.TargetUPM);

						// check brakepoint
						if (abs(RateError) > BrakePoint * FlowSensor.TargetUPM)
						{
							Result = FlowSensor.KP * RateError;
						}
						else
						{
							Result = FlowSensor.KP * RateError * BrakeSet;
						}

						bool IsPositive = (Result > 0);
						Result = abs(Result);
						Result = constrain(Result, FlowSensor.MinPWM, FlowSensor.MaxPWM);
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
