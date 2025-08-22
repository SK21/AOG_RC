
void AdjustFlow()
{
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        float clamped = constrain(Sensor[i].PWM, -255.0, 255.0);
        int PWM = (clamped >= 0.0) ? (int)(clamped + 0.5) : (int)(clamped - 0.5);

        switch (Sensor[i].ControlType)
        {
        case 0:
            // standard valve, flow control only
            if (Sensor[i].FlowEnabled)
            {
                SetPWM(i,PWM);
            }
            break;

        case 1:
        case 5:
            // fast close valve or combo close timed, used for flow control and on/off
            if (Sensor[i].FlowEnabled)
            {
                SetPWM(i, PWM);
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
                SetPWM(i, PWM);
            }
            else
            {
                // stop motor
                SetPWM(i, 0);
            }
            break;

        default:
            // type 3 not used
            break;
        }
    }
}

void SetPWM(byte ID, int PWM)
{
    if (MDL.InvertFlow) PWM *= -1;    // flow on low

    if (PWM > 0)
    {
		analogWrite(Sensor[ID].IN1, PWM); 
		analogWrite(Sensor[ID].IN2, 0);   
	}
	else 
	{
		PWM = abs(PWM);
		analogWrite(Sensor[ID].IN1, 0); 
		analogWrite(Sensor[ID].IN2, PWM); 
    }
}

