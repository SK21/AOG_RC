void AdjustFlow()
{
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        switch (Sensor[i].ControlType)
        {
        case 1:
            // fast close valve, used for flow control and on/off
            if (Sensor[i].FlowEnabled)
            {
                if (Sensor[i].pwmSetting >= 0)
                {
                    //increase
                    if (Sensor[i].pwmSetting > 250)	Sensor[i].pwmSetting = 255;

                    digitalWrite(Sensor[i].DirPin, MDL.FlowOnDirection);
                    analogWrite(Sensor[i].PWMPin, Sensor[i].pwmSetting);
                }
                else
                {
                    //decrease
                    if (Sensor[i].pwmSetting < -250) Sensor[i].pwmSetting = -255;

                    digitalWrite(Sensor[i].DirPin, !MDL.FlowOnDirection);
                    analogWrite(Sensor[i].PWMPin, -Sensor[i].pwmSetting);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop flow
                digitalWrite(Sensor[i].DirPin, !MDL.FlowOnDirection);
                analogWrite(Sensor[i].PWMPin, 255);
            }
            break;
        case 2:
            // motor control
            if (Sensor[i].FlowEnabled)
            {
                if (Sensor[i].pwmSetting >= 0)
                {
                    //increase
                    digitalWrite(Sensor[i].DirPin, MDL.FlowOnDirection);
                    analogWrite(Sensor[i].PWMPin, Sensor[i].pwmSetting);
                }
                else
                {
                    //decrease
                    digitalWrite(Sensor[i].DirPin, !MDL.FlowOnDirection);
                    analogWrite(Sensor[i].PWMPin, -Sensor[i].pwmSetting);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop motor
                analogWrite (Sensor[i].PWMPin, 0);
            }
            break;
        default:
            // standard valve, flow control only
            if (Sensor[i].pwmSetting >= 0)
            {
                //increase
                digitalWrite(Sensor[i].DirPin, MDL.FlowOnDirection);
                analogWrite(Sensor[i].PWMPin, Sensor[i].pwmSetting);
            }
            else
            {
                //decrease
                digitalWrite(Sensor[i].DirPin, !MDL.FlowOnDirection);
                analogWrite(Sensor[i].PWMPin, -Sensor[i].pwmSetting);	// offsets the negative pwm value
            }
            break;
        }
    }
}
