
int DoPID(byte sKP, float sError, float sSetPoint, byte sMinPWM,
    byte sLowMax, byte sHighMax, byte sBrakePoint, byte sDeadband)
{
    int Result = 0;
    if (sSetPoint <= 0) sSetPoint = (float)0.01;
    float ErrorPercent = abs(sError / sSetPoint);
    float ErrorBrake = (float)((float)(sBrakePoint / 100.0));
    float Max = (float)sHighMax;

    if (ErrorPercent > ((float)(sDeadband / 100.0)))
    {
        if (ErrorPercent <= ErrorBrake)
        {
            Max = (ErrorPercent / ErrorBrake) * sLowMax;
        }

        Result = (int)(sKP * sError);

        bool IsPositive = (Result > 0);
        Result = abs(Result);
        if (Result > Max) Result = (int)Max;
        if (Result < sMinPWM) Result = sMinPWM;
        if (!IsPositive) Result *= -1;
    }
    return Result;
}
