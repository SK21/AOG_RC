
const uint32_t SampleTime = 50;				// ms

const double Deadband = 0.015;				// error amount below which no adjustment is made
const double BrakePoint = 0.25;				// error amount where adjustment rate changes from fast to slow
const double FastAdjust = 100;				// fast adjustment factor
const double SlowAdjust = 30;				// slow adjustment factor

const double MaxMotorSlewRate = 2;			// slew rate limit. Max total pwm change per loop
const double MaxMotorIntegral = 0.05;		// for a motor  Ex: 0.05 = max 1 pwm/sec change at 50 ms sample time

const double MaxValveIntegral = 100;		// max total integral pwm adjustment

bool PauseAdjust[MaxProductCount];
uint32_t ComboTime[MaxProductCount];
const double MinStart = 0.03;				// minimum start ratio. Used to quickly increase rate from 0. 
uint32_t TimedAdjustTime = 80;				// milliseconds
uint32_t TimedPauseTime = 400;				// milliseconds

uint32_t LastCheck[MaxProductCount];
double LastPWM[MaxProductCount];
double IntegralSum[MaxProductCount];

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
				double Direction = 1.0;
				if (Sensor[i].PWM < 0) Direction = -1.0;
				if (abs(Sensor[i].PWM) > Sensor[i].MaxPower) Sensor[i].PWM = Sensor[i].MaxPower * Direction;
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

			double RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			// check deadband
			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

				if (Sensor[ID].Ki > 0)
				{
					IntegralSum[ID] += RateError * Sensor[ID].Ki;
					IntegralSum[ID] = constrain(IntegralSum[ID], -1 * MaxMotorIntegral, MaxMotorIntegral);
				}
				else
				{
					IntegralSum[ID] = 0;
				}

				// check brakepoint
				double BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * BrakePoint) ? FastAdjust : SlowAdjust;

				// slew rate limit
				double Change = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
				Change = constrain(Change, -1 * MaxMotorSlewRate, MaxMotorSlewRate);
				Result += Change;
				Result = constrain(Result, Sensor[ID].MinPower, Sensor[ID].MaxPower);
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
		LastPWM[ID] = 0;
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

			double RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

			if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
			{
				RateError = constrain(RateError, -Sensor[ID].TargetUPM, Sensor[ID].TargetUPM);

				if (Sensor[ID].Ki > 0)
				{
					IntegralSum[ID] += RateError * Sensor[ID].Ki;
					IntegralSum[ID] = constrain(IntegralSum[ID], -1 * MaxValveIntegral, MaxValveIntegral);  // max total integral pwm
				}
				else
				{
					IntegralSum[ID] = 0;
				}

				double BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * BrakePoint) ? FastAdjust : SlowAdjust;

				double Control = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
				double Sign = (Control >= 0) ? 1.0 : -1.0;

				Result = abs(Control) + Sensor[ID].MinPower;
				Result = constrain(Result, Sensor[ID].MinPower, Sensor[ID].MaxPower);
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
					if (abs(Result) > Sensor[ID].MaxPower) Result = Sensor[ID].MaxPower * Direction;
				}
				else
				{
					// auto adjust
					double RateError = Sensor[ID].TargetUPM - Sensor[ID].UPM;

					// check deadband
					if (abs(RateError) > Deadband * Sensor[ID].TargetUPM)
					{
						RateError = constrain(RateError, Sensor[ID].TargetUPM * -1, Sensor[ID].TargetUPM);

						if (Sensor[ID].Ki > 0)
						{
							IntegralSum[ID] += RateError * Sensor[ID].Ki;
							IntegralSum[ID] = constrain(IntegralSum[ID], -1 * MaxValveIntegral, MaxValveIntegral);  // max total integral pwm
						}
						else
						{
							IntegralSum[ID] = 0;
						}

						// check brakepoint
						double BrakeFactor = (abs(RateError) > Sensor[ID].TargetUPM * BrakePoint) ? FastAdjust : SlowAdjust;

						double Control = RateError * Sensor[ID].Kp * BrakeFactor + IntegralSum[ID];
						double Sign = (Control >= 0) ? 1.0 : -1.0;

						Result = abs(Control) + Sensor[ID].MinPower;
						Result = constrain(Result, Sensor[ID].MinPower, Sensor[ID].MaxPower);
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
	return (int)Result;
}
