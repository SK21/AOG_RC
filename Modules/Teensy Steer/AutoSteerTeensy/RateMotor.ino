void AdjustFlow()
{
	for (int i = 0; i < 1; i++)
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
					if (RatePWM[i] > 250)	RatePWM[i] = 256;

					digitalWrite(PINS.Motor2Dir, PCB.FlowOnDirection);
					analogWrite(PINS.Motor2PWM, RatePWM[i]);
				}
				else
				{
					//decrease
					if (RatePWM[i] < -250) RatePWM[i] = -256;

					digitalWrite(PINS.Motor2Dir, !PCB.FlowOnDirection);
					analogWrite(PINS.Motor2PWM, -RatePWM[i]);	// offsets the negative pwm value
				}
			}
			else
			{
				// stop flow
				digitalWrite(PINS.Motor2Dir, !PCB.FlowOnDirection);
				analogWrite(PINS.Motor2PWM, 256);
			}
			break;
		case 2:
			// motor control
			if (FlowEnabled[i])
			{
				if (RatePWM[i] >= 0)
				{
					//increase
					digitalWrite(PINS.Motor2Dir, PCB.FlowOnDirection);
					analogWrite(PINS.Motor2PWM, RatePWM[i]);
				}
				else
				{
					//decrease
					digitalWrite(PINS.Motor2Dir, !PCB.FlowOnDirection);
					analogWrite(PINS.Motor2PWM, -RatePWM[i]);	// offsets the negative pwm value
				}
			}
			else
			{
				// stop motor
				analogWrite(PINS.Motor2PWM, 0);
			}
			break;
		default:
			// standard valve, flow control only
			if (RatePWM[i] >= 0)
			{
				//increase
				digitalWrite(PINS.Motor2Dir, PCB.FlowOnDirection);
				analogWrite(PINS.Motor2PWM, RatePWM[i]);
			}
			else
			{
				//decrease
				digitalWrite(PINS.Motor2Dir, !PCB.FlowOnDirection);
				analogWrite(PINS.Motor2PWM, -RatePWM[i]);	// offsets the negative pwm value
			}
			break;
		}
	}
}
