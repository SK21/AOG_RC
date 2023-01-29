
unsigned long CurrentAdjustTime[MaxProductCount];
float ErrorPercentLast[2];
float Integral;
float LastPWM[MaxProductCount];

int PIDvalve(byte ID)
{
    int Result = 0;
    if (Sensor[ID].FlowEnabled)
    {
        float ErrorPercent = abs(Sensor[ID].RateError / Sensor[ID].RateSetting);
        float ErrorBrake = (float)((float)(Sensor[ID].BrakePoint / 100.0));
        float Max = (float)Sensor[ID].MaxPWM;

        if (ErrorPercent > ((float)(Sensor[ID].Deadband / 100.0)))
        {
            if (ErrorPercent <= ErrorBrake) Max = Sensor[ID].MinPWM * 3.0;

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
            Integral = 0;
        }
    }
    return Result;
}

int PIDmotor(byte ID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (Sensor[ID].FlowEnabled)
    {
        Result = LastPWM[ID];
        ErrorPercent = Sensor[ID].RateError / Sensor[ID].RateSetting * 100.0;

        if (abs(ErrorPercent) > (float)Sensor[ID].Deadband)
        {
            Result += Sensor[ID].KP * ErrorPercent;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[ID];
            CurrentAdjustTime[ID] = millis();

            Result += (float)Sensor[ID].KD * (ErrorPercent - ErrorPercentLast[ID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[ID] = ErrorPercent;

            if (Result < Sensor[ID].MinPWM) Result = Sensor[ID].MinPWM;
            if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM;
        }
    }
    LastPWM[ID] = Result;
    return (int)Result;
}

