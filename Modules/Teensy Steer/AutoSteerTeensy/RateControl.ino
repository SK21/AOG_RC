void DoRate()
{
    ReceiveRateUDP();

    if (millis() - RateLoopLast >= RateLoopTime)
    {
        RateLoopLast = millis();
        GetUPM();

        for (int i = 0; i < 1; i++)
        {
            FlowEnabled[i] = (millis() - RateCommTime[i] < 4000) && (RateSetting[i] > 0) && MasterOn[i];
        }

        CheckRelays();
        AdjustFlow();

        if (AutoOn)
        {
            AutoControl();
        }
        else
        {
            ManualControl();
        }
    }

    if (millis() - RateSendLast > RateSendTime)
    {
        RateSendLast = millis();
        SendRateUDP();
    }
}

byte ParseModID(byte ID)
{
	// top 4 bits
	return ID >> 4;
}

byte ParseSenID(byte ID)
{
	// bottom 4 bits
	return (ID & 0b00001111);
}

byte BuildModSenID(byte Mod_ID, byte Sen_ID)
{
	return ((Mod_ID << 4) | (Sen_ID & 0b00001111));
}

void AutoControl()
{
    for (int i = 0; i < 1; i++)
    {
        rateError[i] = RateSetting[i] - UPM[i];

        switch (ControlType[i])
        {
        case 2:
            // motor control
            RatePWM[i] = ControlMotor(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i],
                PIDHighMax[i], PIDdeadband[i], i);
            break;

        default:
            // valve control
            RatePWM[i] = DoPID(PIDkp[i], rateError[i], RateSetting[i], PIDminPWM[i], PIDLowMax[i],
                PIDHighMax[i], PIDbrakePoint[i], PIDdeadband[i], i, PIDki[i]);
            break;
        }
    }
}

void ManualControl()
{
    for (int i = 0; i < 1; i++)
    {
        rateError[i] = RateSetting[i] - UPM[i];

        if (millis() - ManualLast[i] > 1000)
        {
            ManualLast[i] = millis();

            // adjust rate
            if (RateSetting[i] == 0) RateSetting[i] = 1; // to make FlowEnabled

            switch (ControlType[i])
            {
            case 2:
                // motor control
                if (ManualAdjust[i] > 0)
                {
                    RatePWM[i] *= 1.10;
                    if (RatePWM[i] < 1) RatePWM[i] = PIDminPWM[i];
                    if (RatePWM[i] > 256) RatePWM[i] = 256;
                }
                else if (ManualAdjust[i] < 0)
                {
                    RatePWM[i] *= 0.90;
                    if (RatePWM[i] < PIDminPWM[i]) RatePWM[i] = 0;
                }
                break;

            default:
                // valve control
                RatePWM[i] = ManualAdjust[i];
                break;
            }
        }
    }
}
