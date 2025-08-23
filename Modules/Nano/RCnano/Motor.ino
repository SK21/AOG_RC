
void AdjustFlow()
{
    for (int i = 0; i < MDL.SensorCount; i++) 
    {
        float clamped = constrain(Sensor[i].PWM, -255.0f, 255.0f);

        switch (Sensor[i].ControlType)
        {
        case 0:
            // standard valve, flow control only
            if (Sensor[i].FlowEnabled) SetPWM(i, clamped);
            break;

        case 1:
        case 5:
            // fast close valve or combo close timed, used for flow control and on/off
            SetPWM(i, Sensor[i].FlowEnabled ? clamped : -255.0f);
            break;

        case 2:
        case 4:
            // motor control
            SetPWM(i, Sensor[i].FlowEnabled ? clamped : 0.0f);
            break;

        default:
            break;
        }
    }
}

void SetPWM(byte ID, float pwmVal)
{
    const int maxDuty = (1 << PWM_BITS) - 1;
    int duty = (int)floorf(fabsf(pwmVal) * maxDuty / 255.0f);

    bool Increase = (pwmVal >= 0.0f);
    if (MDL.InvertFlow) Increase = !Increase;

#if PWM_BITS == 8
    duty = ditherAdjust(duty, fabsf(pwmVal));
#endif


#if defined(ESP32)
    if (Increase) 
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
    digitalWrite(Sensor[ID].DirPin, Increase);
    analogWrite(Sensor[ID].PWMPin, duty);
#endif
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
