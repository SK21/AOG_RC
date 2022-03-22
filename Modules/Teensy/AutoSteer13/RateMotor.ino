#if(UseRateControl)
void AdjustFlow()
{
    for (int i = 0; i < SensorCount; i++)
    {
        switch (ControlType[i])
        {
        case 1:
            // fast close valve, used for flow control and on/off
            if (FlowEnabled[i])
            {
                if (RatePWM[i] >= 0)
                {
                    //increase
                    digitalWrite(FlowDir[i], FlowOn[i]);
                    analogWrite(FlowPWM[i], RatePWM[i]);
                }
                else
                {
                    //decrease
                    digitalWrite(FlowDir[i], !FlowOn[i]);
                    analogWrite(FlowPWM[i], -RatePWM[i]);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop flow
                analogWrite(FlowPWM[i], 255);
                digitalWrite(FlowDir[i], !FlowOn[i]);
            }
            break;
        case 2:
            // motor control
            if (FlowEnabled[i])
            {
                if (RatePWM[i] >= 0)
                {
                    //increase
                    digitalWrite(FlowDir[i], FlowOn[i]);
                    analogWrite(FlowPWM[i], RatePWM[i]);
                }
                else
                {
                    //decrease
                    digitalWrite(FlowDir[i], !FlowOn[i]);
                    analogWrite(FlowPWM[i], -RatePWM[i]);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop motor
                analogWrite(FlowPWM[i], 0);
            }
            break;
        default:
            // standard valve, flow control only
            if (RatePWM[i] >= 0)
            {
                //increase
                digitalWrite(FlowDir[i], FlowOn[i]);
                analogWrite(FlowPWM[i], RatePWM[i]);
            }
            else
            {
                //decrease
                digitalWrite(FlowDir[i], !FlowOn[i]);
                analogWrite(FlowPWM[i], -RatePWM[i]);	// offsets the negative pwm value
            }
            break;
        }
    }
}
#endif


