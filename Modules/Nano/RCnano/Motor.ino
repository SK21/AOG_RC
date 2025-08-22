
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
            if (PWM >= 0)
            {
                //increase
                digitalWrite(Sensor[i].DirPin, MDL.InvertFlow);
                analogWrite(Sensor[i].PWMPin, PWM);
            }
            else
            {
                //decrease
                digitalWrite(Sensor[i].DirPin, !MDL.InvertFlow);
                analogWrite(Sensor[i].PWMPin, -PWM);	// offsets the negative pwm value
            }
            break;

        case 1:
        case 5:
            // fast close valve or combo close timed, used for flow control and on/off
            if (Sensor[i].FlowEnabled)
            {
                if (PWM >= 0)
                {
                    digitalWrite(Sensor[i].DirPin, MDL.InvertFlow);
                    analogWrite(Sensor[i].PWMPin, PWM);
                }
                else
                {
                    //decrease
                    digitalWrite(Sensor[i].DirPin, !MDL.InvertFlow);
                    analogWrite(Sensor[i].PWMPin, -PWM);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop flow
                digitalWrite(Sensor[i].DirPin, !MDL.InvertFlow);
                analogWrite(Sensor[i].PWMPin, 255);
            }
            break;

        case 2:
        case 4:
            // motor control
            if (Sensor[i].FlowEnabled)
            {
                if (PWM >= 0)
                {
                    //increase
                    digitalWrite(Sensor[i].DirPin, MDL.InvertFlow);
                    analogWrite(Sensor[i].PWMPin, PWM);
                }
                else
                {
                    //decrease
                    digitalWrite(Sensor[i].DirPin, !MDL.InvertFlow);
                    analogWrite(Sensor[i].PWMPin, -PWM);	// offsets the negative pwm value
                }
            }
            else
            {
                // stop motor
                analogWrite(Sensor[i].PWMPin, 0);
            }
            break;

        default:
            // type 3 not used
            break;
        }
    }
}

