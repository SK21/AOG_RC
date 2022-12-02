
bool AdjustState[] = { true,true,true,true,true }; // false - pause adjusting, true - adjust
uint32_t CurrentAdjustTime[2];
float Integral;

int DoPID(byte sKP, float sError, float sSetPoint, byte sMinPWM,
    byte sLowMax, byte sHighMax, byte sBrakePoint, byte sDeadband,
    byte SensorID, byte sKI)
{
    int Result = 0;
    if (AdjustState[SensorID] || AdjustTime[SensorID] == 0) // AdjustTime==0 disables timed adjustment
    {
        // adjusting rate
        if (FlowEnabled[SensorID])
        {
            float ErrorPercent = abs(sError / sSetPoint);
            float ErrorBrake = (float)((float)(sBrakePoint / 100.0));
            float Max = (float)sHighMax;

            if (ErrorPercent > ((float)(sDeadband / 100.0)))
            {
                if (ErrorPercent <= ErrorBrake)
                {
                    Max = (ErrorPercent / ErrorBrake) * sLowMax;
                }

                Result = (int)((sKP * sError) + (Integral * sKI / 255.0));

                bool IsPositive = (Result > 0);
                Result = abs(Result);

                if (Result != 0)
                {
                    // limit integral size
                    if ((Integral / Result) < 4) Integral += sError / 3.0;
                }

                if (Result > Max) Result = (int)Max;
                else if (Result < sMinPWM) Result = sMinPWM;

                if (!IsPositive) Result *= -1;
            }
            else
            {
                // reset time since no adjustment was made
                CurrentAdjustTime[SensorID] = millis();

                Integral = 0;
            }
        }

        if ((millis() - CurrentAdjustTime[SensorID]) > AdjustTime[SensorID])
        {
            // switch state
            CurrentAdjustTime[SensorID] = millis();
            AdjustState[SensorID] = !AdjustState[SensorID];
        }
    }
    else
    {
        // pausing adjustment, 3 X AdjustTime
        if ((millis() - CurrentAdjustTime[SensorID]) > AdjustTime[SensorID] * 3)
        {
            // switch state
            CurrentAdjustTime[SensorID] = millis();
            AdjustState[SensorID] = !AdjustState[SensorID];
        }
    }
    return Result;
}

float LastPWM[MaxFlowSensorCount];
int ControlMotor(byte sKP, float sError, float sSetPoint, byte sMinPWM,
    byte sHighMax, byte sDeadband, byte SensorID)
{
    float Result = 0;
    float ErrorPercent = 0;

    if (FlowEnabled[SensorID])
    {
        Result = LastPWM[SensorID];
        ErrorPercent = abs(sError / sSetPoint) * 100.0;
        float Max = (float)sHighMax;

        if (ErrorPercent > (float)sDeadband)
        {
            Result += ((float)sKP / 255.0) * sError * 5.0;

            if (Result > Max) Result = Max;
            if (Result < sMinPWM) Result = (float)sMinPWM;
        }
    }

    LastPWM[SensorID] = Result;
    return (int)Result;
}

