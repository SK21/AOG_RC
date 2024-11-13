
void AdjustFlow()
{
    switch (FlowSensor.ControlType)
    {
    case 0:
        // standard valve, flow control only
        if (FlowSensor.FlowEnabled)
        {
            SetPWM(FlowSensor.PWM);
        }
        break;

    case 1:
    case 5:
        // fast close valve or combo close timed, used for flow control and on/off
        if (FlowSensor.FlowEnabled)
        {
            SetPWM(FlowSensor.PWM);
        }
        else
        {
            // stop flow, close valve
            SetPWM(-255);
        }
        break;

    case 2:
    case 4:
        // motor control
        if (FlowSensor.FlowEnabled)
        {
            SetPWM(FlowSensor.PWM);
        }
        else
        {
            // stop motor
            SetPWM(0);
        }
        break;
    }
}

void SetPWM(double PWM)
{
    if (MDL.FlowOnDirection == 0) PWM *= -1;    // flow on low

    if (PWM > 0)
    {
        ledcWrite(0, PWM);     // IN1
        ledcWrite(1, 0);   // IN2
    }
    else
    {
        PWM = abs(PWM);
        ledcWrite(1, PWM); // IN2
        ledcWrite(0, 0);       // IN1
    }
}


