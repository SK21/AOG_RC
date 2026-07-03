
void AdjustFlow()
{
    const bool sectionsOn = ((RelayLo > 0) || (RelayHi > 0));
    static uint32_t lastStateLog = 0;
    if (millis() - lastStateLog >= 3000)
    {
        lastStateLog = millis();
        Serial.print(F("PWM state: sensors="));
        Serial.print(MDL.SensorCount);
        Serial.print(F(" master="));
        Serial.print(MasterOn ? F("ON") : F("OFF"));
        Serial.print(F(" sections="));
        Serial.print(sectionsOn ? F("ON") : F("OFF"));
        if (MDL.SensorCount > 0)
        {
            Serial.print(F(" S1 type="));
            Serial.print(Sensor[0].ControlType);
            Serial.print(F("("));
            Serial.print(VT_ControlTypeName(Sensor[0].ControlType));
            Serial.print(F(")"));
            Serial.print(F(" enabled="));
            Serial.print(Sensor[0].AdjustmentEnabled ? F("yes") : F("no"));
            Serial.print(F(" mode="));
            Serial.print(Machine.AutoOn ? F("AUTO") : F("MAN"));
            Serial.print(F(" manual="));
            Serial.print(Sensor[0].ManualAdjust);
            Serial.print(F(" pwm="));
            Serial.print(Sensor[0].PWM);
        }
        Serial.println();
    }

    for (int i = 0; i < MDL.SensorCount; i++)
    {
        float clamped = constrain(Sensor[i].PWM, -255.0f, 255.0f);

        switch (Sensor[i].ControlType)
        {
        case StandardValve_ct:
            SetPWM(i, Sensor[i].AdjustmentEnabled ? clamped : 0.0f);
            break;

        case Motor_ct:
        case Fan_ct:
            SetPWM(i, (Sensor[i].AdjustmentEnabled && MasterOn) ? clamped : 0.0f);
            break;

        case ComboClose_ct:
        case TimedCombo_ct:
            // fast close valve or combo close timed, used for flow control and on/off
            SetPWM(i, (MasterOn && sectionsOn && Sensor[i].AdjustmentEnabled) ? clamped : -255.0f);
            break;

        default:
            SetPWM(i, (Sensor[i].AdjustmentEnabled && MasterOn) ? clamped : 0.0f);
            break;
        }
    }
}

void LogPWMWrite(byte ID, float pwmVal, int duty, bool direction)
{
    if (ID >= MaxProductCount) return;

    static int lastDuty[MaxProductCount] = { -1, -1 };
    static bool lastDirection[MaxProductCount] = { false, false };
    static uint32_t lastPrint[MaxProductCount] = { 0, 0 };

    if ((duty == lastDuty[ID]) &&
        (direction == lastDirection[ID]) &&
        (millis() - lastPrint[ID] < 5000))
    {
        return;
    }

    lastDuty[ID] = duty;
    lastDirection[ID] = direction;
    lastPrint[ID] = millis();

    Serial.print(F("PWM output S"));
    Serial.print(ID + 1);
    Serial.print(F(": pwm="));
    Serial.print(pwmVal);
    Serial.print(F(" duty="));
    Serial.print(duty);
    Serial.print(F(" pwmPin="));
    Serial.print(Sensor[ID].PWMPin);
    Serial.print(F(" dirPin="));
    Serial.print(Sensor[ID].DirPin);
    Serial.print(F(" dir="));
    Serial.print(direction ? F("HIGH") : F("LOW"));
    Serial.print(F(" enabled="));
    Serial.print(Sensor[ID].AdjustmentEnabled ? F("yes") : F("no"));
    Serial.print(F(" master="));
    Serial.print(MasterOn ? F("ON") : F("OFF"));
    Serial.print(F(" mode="));
    Serial.println(Machine.AutoOn ? F("AUTO") : F("MAN"));
}

void SetPWM(byte ID, float pwmVal)
{
    const int maxDuty = (1 << PWM_BITS) - 1;
    int duty = (int)floorf(fabsf(pwmVal) * maxDuty / 255.0f);

    bool Direction = (pwmVal >= 0.0f);
    if (MDL.InvertFlow) Direction = !Direction;

#if PWM_BITS == 8
    duty = ditherAdjust(duty, fabsf(pwmVal));
#endif


#if defined(ESP32)
    if (Direction)
    {
        analogWrite(Sensor[ID].IN1, duty);
        analogWrite(Sensor[ID].IN2, 0);
    }
    else
    {
        analogWrite(Sensor[ID].IN1, 0);
        analogWrite(Sensor[ID].IN2, duty);
    }
#else
    digitalWrite(Sensor[ID].DirPin, Direction);
    analogWrite(Sensor[ID].PWMPin, duty);
#endif
    LogPWMWrite(ID, pwmVal, duty, Direction);
}

#if PWM_BITS == 8
int ditherAdjust(int base, float val255)
{
    const int maxDuty = 255;
    float exactDuty = val255 * maxDuty / 255.0f;
    float frac = exactDuty - base;

    ditherCounter = (ditherCounter + 1) & 0x0F; // 16 step cycle
    if (frac > 0 && ditherCounter < (uint8_t)(frac * 16)) {
        base = min(base + 1, maxDuty);
    }
    return base;
}
#endif
