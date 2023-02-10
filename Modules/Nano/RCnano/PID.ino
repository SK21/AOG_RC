
unsigned long CurrentAdjustTime[2]; 
float ErrorPercentLast[2];
float ErrorPercentCum[2];
float Integral;
float LastPWM[2];

int PIDvalve(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    float Result = 0;

    if (FlowEnabled[SensorID] && sSetPoint > 0)
    {
        float ErrorPercent = sError / sSetPoint * 100.0;

        if (abs(ErrorPercent) > (float)Deadband)
        {
            Result = (sKP * ErrorPercent) + Integral;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[SensorID];
            CurrentAdjustTime[SensorID] = millis();

            ErrorPercentCum[SensorID] += ErrorPercent * (elapsedTime * 0.001) * 0.001;

            Integral += sKI * ErrorPercentCum[SensorID];
            if (Integral > 10) Integral = 10;
            if (Integral < -10) Integral = -10;
            if (sKI == 0)
            {
                Integral = 0;
                ErrorPercentCum[SensorID] = 0;
            }

            Result += Integral;

            //add in derivative term to dampen effect of the correction.
            Result += (float)sKD * (ErrorPercent - ErrorPercentLast[SensorID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[SensorID] = ErrorPercent;

            bool IsPositive = (Result > 0);
            Result = abs(Result);

            if (Result < sMinPWM)
            {
                Result = sMinPWM;
            }
            else
            {
                if (ErrorPercent < BrakePoint)
                {
                    if (Result > sMinPWM * 3.0) Result = sMinPWM * 3.0;
                }
                else
                {
                    if (Result > sMaxPWM) Result = sMaxPWM;
                }
            }

            if (!IsPositive) Result *= -1;
        }
        else
        {
            Integral = 0;
        }
    }
    return (int)Result;
}

int PIDmotor(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (FlowEnabled[SensorID] && sSetPoint > 0)
    {
        Result = LastPWM[SensorID];
        ErrorPercent = sError / sSetPoint * 100.0;

        if (abs(ErrorPercent) > (float)Deadband)
        {
            Result += sKP * ErrorPercent;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[SensorID];
            CurrentAdjustTime[SensorID] = millis();

            ErrorPercentCum[SensorID] += ErrorPercent * (elapsedTime * 0.001) * 0.001;

            Integral += sKI * ErrorPercentCum[SensorID];
            if (Integral > 10) Integral = 10;
            if (Integral < -10) Integral = -10;
            if (sKI == 0)
            {
                Integral = 0;
                ErrorPercentCum[SensorID] = 0;
            }

            Result += Integral;

            //add in derivative term to dampen effect of the correction.
            Result += (float)sKD * (ErrorPercent - ErrorPercentLast[SensorID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[SensorID] = ErrorPercent;

            if (Result > sMaxPWM) Result = (float)sMaxPWM;
            if (Result < sMinPWM) Result = (float)sMinPWM;
        }
    }
    else
    {
        Integral = 0;
    }
    LastPWM[SensorID] = Result;
    return (int)Result;
}
