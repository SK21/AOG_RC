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

					digitalWrite(PINS.FlowDir, PCB.FlowOnDirection);
					analogWrite(PINS.FlowPWM, RatePWM[i]);
				}
				else
				{
					//decrease
					if (RatePWM[i] < -250) RatePWM[i] = -256;

					digitalWrite(PINS.FlowDir, !PCB.FlowOnDirection);
					analogWrite(PINS.FlowPWM, -RatePWM[i]);	// offsets the negative pwm value
				}
			}
			else
			{
				// stop flow
				digitalWrite(PINS.FlowDir, !PCB.FlowOnDirection);
				analogWrite(PINS.FlowPWM, 256);
			}
			break;
		case 2:
			// motor control
			if (FlowEnabled[i])
			{
				if (RatePWM[i] >= 0)
				{
					//increase
					digitalWrite(PINS.FlowDir, PCB.FlowOnDirection);
					analogWrite(PINS.FlowPWM, RatePWM[i]);
				}
				else
				{
					//decrease
					digitalWrite(PINS.FlowDir, !PCB.FlowOnDirection);
					analogWrite(PINS.FlowPWM, -RatePWM[i]);	// offsets the negative pwm value
				}
			}
			else
			{
				// stop motor
				analogWrite(PINS.FlowPWM, 0);
			}
			break;
		default:
			// standard valve, flow control only
			if (RatePWM[i] >= 0)
			{
				//increase
				digitalWrite(PINS.FlowDir, PCB.FlowOnDirection);
				analogWrite(PINS.FlowPWM, RatePWM[i]);
			}
			else
			{
				//decrease
				digitalWrite(PINS.FlowDir, !PCB.FlowOnDirection);
				analogWrite(PINS.FlowPWM, -RatePWM[i]);	// offsets the negative pwm value
			}
			break;
		}
	}
}
