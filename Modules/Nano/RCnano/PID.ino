
unsigned long CurrentAdjustTime[2];
float ErrorPercentLast[2];
float ErrorPercentCum[2];
float Integral;
float LastPWM[2];

//BELT CONTROL
int PIDvalve(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    float Result = 0;

    if (FlowEnabled[SensorID] && sSetPoint > 0)
    {
        float ErrorPercent = sError / sSetPoint;

        if (abs(ErrorPercent) > (float)Deadband / 100.0)
        {


            Result = (sKP * ErrorPercent);

            unsigned long elapsedTime = millis() - CurrentAdjustTime[SensorID];
            CurrentAdjustTime[SensorID] = millis();

            ErrorPercentCum[SensorID] += ErrorPercent * (elapsedTime * 0.001);

            Integral += sKI * ErrorPercentCum[SensorID];
            if (Integral > 50) Integral = 50;
            if (Integral < -50) Integral = -50;

            Result += Integral;

            //add in derivative term to dampen effect of the correction.
            Result += (float)sKD * (ErrorPercent - ErrorPercentLast[SensorID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[SensorID] = ErrorPercent;

            bool IsPositive = (Result > 0);
            Result = abs(Result);

            if (Result < (float)sMinPWM)
            {
                Result = (float)sMinPWM;
            }
            else
            {
                if (abs(ErrorPercent) < (float)BrakePoint / 100.0)
                {
                    if (Result > (float)sMinPWM * 3.0) Result = (float)sMinPWM * 3.0;
                }
                else
                {
                    if (Result > (float)sMaxPWM) Result = (float)sMaxPWM;
                }
            }

            if (!IsPositive) Result *= -1;
        }
        //else
        //{
        //    Integral = 0;
        //}
    }
    return (int)Result;
}


// FAN CONTROL
int PIDmotor(float sKP, float sKI, float sKD, float sError, float sSetPoint, byte sMinPWM, byte sMaxPWM, byte SensorID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (FlowEnabled[SensorID] && sSetPoint > 0)
    {
        Result = LastPWM[SensorID];
        ErrorPercent = sError / sSetPoint;  // could change this to just the ratio to remove the '* 100.0'

        if (abs(ErrorPercent) > (float)Deadband / 100.0)
        {
            //Result += ((float)sKP / 255.0) * ErrorPercent * 0.1;
            Result += sKP * ErrorPercent / 25.5;

            unsigned long elapsedTime = millis() - CurrentAdjustTime[SensorID];
            CurrentAdjustTime[SensorID] = millis();

            ErrorPercentCum[SensorID] += ErrorPercent * (elapsedTime * 0.001);

            Integral += sKI * ErrorPercentCum[SensorID];
            if (Integral > 50) Integral = 50;
            if (Integral < -50) Integral = -50;

            Result += Integral;

            //add in derivative term to dampen effect of the correction.
            Result += (float)sKD * (ErrorPercent - ErrorPercentLast[SensorID]) / (elapsedTime * 0.001) * 0.001;

            ErrorPercentLast[SensorID] = ErrorPercent;

            if (Result > (float)sMaxPWM) Result = (float)sMaxPWM;
            if (Result < (float)sMinPWM) Result = (float)sMinPWM;
        }
    }
    LastPWM[SensorID] = Result;
    return (int)Result;
}