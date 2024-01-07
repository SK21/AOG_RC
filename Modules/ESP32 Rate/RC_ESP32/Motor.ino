
void AdjustFlow()
{
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        switch (Sensor[i].ControlType)
        {
        case 0:
            // standard valve, flow control only
            if (Sensor[i].FlowEnabled)
            {
                SetPWM(i, Sensor[i].PWM);
            }
            break;

        case 1:
        case 5:
            // fast close valve or combo close timed, used for flow control and on/off
            if (Sensor[i].FlowEnabled)
            {
                SetPWM(i, Sensor[i].PWM);
            }
            else
            {
                // stop flow, close valve
                SetPWM(i, -255);
            }
            break;

        case 2:
        case 4:
            // motor control
            if (Sensor[i].FlowEnabled)
            {
                SetPWM(i, Sensor[i].PWM);
            }
            else
            {
                // stop motor
                SetPWM(i, 0);
            }
            break;
        }
    }
}

void SetPWM(byte ID, double PWM)
{
    if (MDL.FlowOnDirection == 0) PWM *= -1;    // flow on low
    if (PWM > 0)
    {
        ledcWrite(ID * 2, PWM);     // IN1
        ledcWrite(ID * 2 + 1, 0);   // IN2
    }
    else
    {
        PWM = abs(PWM);
        ledcWrite(ID * 2 + 1, PWM); // IN2
        ledcWrite(ID * 2, 0);       // IN1
    }
}

