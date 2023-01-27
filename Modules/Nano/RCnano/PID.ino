
unsigned long CurrentAdjustTime[2]; 
float ErrorPercentLast[2];
float Integral;

int PIDvalve(float sKP, float sKI, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    int Result = 0;
    if (FlowEnabled[SensorID])
    {
        float ErrorPercent = abs(sError / sSetPoint);
        float ErrorBrake = (float)((float)(BrakePoint / 100.0));
        float Max = (float)sMaxPWM;

        if (ErrorPercent > ((float)(Deadband / 100.0)))
        {
            if (ErrorPercent <= ErrorBrake) Max = sMinPWM * 3.0;

            Result = (int)((sKP * sError) + (Integral * sKI / 255.0));

            bool IsPositive = (Result > 0);
            Result = abs(Result);

            if (Result != 0)
            {
                // limit integral size
                if ((Integral / Result) < 4) Integral += sError / 3.0;
            }

            if (Result > Max) Result = Max;
            else if (Result < sMinPWM) Result = sMinPWM;

            if (!IsPositive) Result *= -1;
        }
        else
        {
            Integral = 0;
        }
    }
    return Result;
}



float LastPWM[2];
int PIDmotor(float sKP, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (FlowEnabled[SensorID])
    {
        Result = LastPWM[SensorID];
        ErrorPercent = sError / sSetPoint * 100.0;  // could change this to just the ratio to remove the '* 100.0'

        if (abs(ErrorPercent) > (float)Deadband)
        {
            //Result += ((float)sKP / 255.0) * ErrorPercent * 0.1;
            Result += sKP * ErrorPercent;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[SensorID];
            CurrentAdjustTime[SensorID] = millis();

            //add in derivative term to dampen effect of the correction.
            Result += (float)sKD * (ErrorPercent - ErrorPercentLast[SensorID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[SensorID] = ErrorPercent;

            if (Result > sMaxPWM) Result = (float)sMaxPWM;
            if (Result < sMinPWM) Result = (float)sMinPWM;
        }
    }
    LastPWM[SensorID] = Result;
    return (int)Result;
}
