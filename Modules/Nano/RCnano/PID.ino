
unsigned long CurrentAdjustTime[MaxProductCount];
float ErrorPercentLast[MaxProductCount];
float ErrorPercentCum[MaxProductCount];
float Integral[MaxProductCount];
float LastPWM[MaxProductCount];

int PIDvalve(byte ID)
{
    float Result = 0;

    if (Sensor[ID].FlowEnabled && Sensor[ID].RateSetting > 0)
    {
        float ErrorPercent = Sensor[ID].RateError / Sensor[ID].RateSetting * 100.0;

        if (abs(ErrorPercent) > (float)Sensor[ID].Deadband)
        {
            Result = Sensor[ID].KP * ErrorPercent;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[ID];
            CurrentAdjustTime[ID] = millis();

            ErrorPercentCum[ID] += ErrorPercent * (elapsedTime * 0.001) * 0.001;

            Integral[ID] += Sensor[ID].KI * ErrorPercentCum[ID];
            if (Integral[ID] > 10) Integral[ID] = 10;
            if (Integral[ID] < -10) Integral[ID] = -10;
            if (Sensor[ID].KI == 0)
            {
                Integral[ID] = 0;
                ErrorPercentCum[ID] = 0;
            }

            Result += Integral[ID];

            //add in derivative term to dampen effect of the correction.
            Result += (float)Sensor[ID].KD * (ErrorPercent - ErrorPercentLast[ID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[ID] = ErrorPercent;

            bool IsPositive = (Result > 0);
            Result = abs(Result);

            if (Result < Sensor[ID].MinPWM)
            {
                Result = Sensor[ID].MinPWM;
            }
            else
            {
                if (ErrorPercent < Sensor[ID].BrakePoint)
                {
                    if (Result > Sensor[ID].MinPWM * 3.0) Result = Sensor[ID].MinPWM * 3.0;
                }
                else
                {
                    if (Result > Sensor[ID].MaxPWM) Result = Sensor[ID].MaxPWM;
                }
            }

            if (!IsPositive) Result *= -1;
        }
        else
        {
            Integral[ID] = 0;
        }
    }
    return (int)Result;
}

int PIDmotor(byte ID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (Sensor[ID].FlowEnabled && Sensor[ID].RateSetting > 0)
    {
        Result = LastPWM[ID];
        ErrorPercent = Sensor[ID].RateError / Sensor[ID].RateSetting * 100.0;

        if (abs(ErrorPercent) > (float)Sensor[ID].Deadband)
        {
            Result += Sensor[ID].KP * ErrorPercent;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[ID];
            CurrentAdjustTime[ID] = millis();

            ErrorPercentCum[ID] += ErrorPercent * (elapsedTime * 0.001) * 0.001;

            Integral[ID] += Sensor[ID].KI * ErrorPercentCum[ID];
            if (Integral[ID] > 10) Integral[ID] = 10;
            if (Integral[ID] < -10) Integral[ID] = -10;
            if (Sensor[ID].KI == 0)
            {
                Integral[ID] = 0;
                ErrorPercentCum[ID] = 0;
            }

            Result += Integral[ID];

            //add in derivative term to dampen effect of the correction.
            Result += (float)Sensor[ID].KD * (ErrorPercent - ErrorPercentLast[ID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[ID] = ErrorPercent;

            if (Result > Sensor[ID].MaxPWM) Result = (float)Sensor[ID].MaxPWM;
            if (Result < Sensor[ID].MinPWM) Result = (float)Sensor[ID].MinPWM;
        }
    }
    else
    {
        Integral[ID] = 0;
    }

    LastPWM[ID] = Result;
    return (int)Result;
}
