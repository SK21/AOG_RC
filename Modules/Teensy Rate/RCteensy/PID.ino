bool PauseAdjust[MaxProductCount];
unsigned long CurrentAdjustTime[MaxProductCount];
float Integral;

int DoPID(byte ID)
{
    int Result = 0;
    if (!PauseAdjust[ID] || Sensor[ID].AdjustTime == 0) // AdjustTime==0 disables timed adjustment
    {
        // adjusting rate
        if (Sensor[ID].FlowEnabled)
        {
            float ErrorPercent = abs(Sensor[ID].RateError / Sensor[ID].RateSetting);
            float ErrorBrake = (float)((float)(Sensor[ID].BrakePoint / 100.0));
            float Max = (float)Sensor[ID].HighMax;

            if (ErrorPercent > ((float)(Sensor[ID].Deadband / 100.0)))
            {
                if (ErrorPercent <= ErrorBrake) Max = Sensor[ID].LowMax;

                Result = (int)((Sensor[ID].KP * Sensor[ID].RateError) + (Integral * Sensor[ID].KI / 255.0));

                bool IsPositive = (Result > 0);
                Result = abs(Result);

                if (Result != 0)
                {
                    // limit integral size
                    if ((Integral / Result) < 4) Integral += Sensor[ID].RateError / 3.0;
                }

                if (Result > Max) Result = (int)Max;
                else if (Result < Sensor[ID].MinPWM) Result = (int)Sensor[ID].MinPWM;

                if (!IsPositive) Result *= -1;
            }
            else
            {
                // reset time since no adjustment was made
                CurrentAdjustTime[ID] = millis();

                Integral = 0;
            }
        }

        if ((millis() - CurrentAdjustTime[ID]) > Sensor[ID].AdjustTime)
        {
            // switch state
            CurrentAdjustTime[ID] = millis();
            PauseAdjust[ID] = !PauseAdjust[ID];
        }
    }
    else
    {
        // pausing adjustment, 3 X AdjustTime
        if ((millis() - CurrentAdjustTime[ID]) > Sensor[ID].AdjustTime * 3)
        {
            // switch state
            CurrentAdjustTime[ID] = millis();
            PauseAdjust[ID] = !PauseAdjust[ID];
        }
    }
    return Result;
}

float LastPWM[MaxProductCount];
int ControlMotor(byte ID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (Sensor[ID].FlowEnabled && Sensor[ID].RateSetting > 0)
    {
        Result = LastPWM[ID];
        ErrorPercent = abs(Sensor[ID].RateError / Sensor[ID].RateSetting) * 100.0;
        if (ErrorPercent > (float)Sensor[ID].Deadband)
        {
            Result += ((float)Sensor[ID].KP / 255.0) * Sensor[ID].RateError * 5.0;

            if (Result > (float)Sensor[ID].HighMax) Result = (float)Sensor[ID].HighMax;
            if (Result < Sensor[ID].MinPWM) Result = (float)Sensor[ID].MinPWM;
        }
    }

    LastPWM[ID] = Result;
    return (int)Result;
}

