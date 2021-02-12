void motorDrive()
{
	switch (ControlType)
	{
	case 1:
		// fast close valve, used for flow control and on/off
		if (ApplicationOn)
		{
			if (pwmSetting >= 0)
			{
				//increase
				digitalWrite(FlowDIR, FlowOn);
				analogWrite(FlowPWM, pwmSetting);
			}
			else
			{
				//decrease
				digitalWrite(FlowDIR, !FlowOn);
				analogWrite(FlowPWM, -pwmSetting);	// offsets the negative pwm value
			}
		}
		else
		{
			// stop flow
			analogWrite(FlowPWM, 255);
			digitalWrite(FlowDIR, !FlowOn);
		}
		break;
	case 2:
		// motor control
		if (ApplicationOn)
		{
			if (pwmSetting >= 0)
			{
				//increase
				digitalWrite(FlowDIR, FlowOn);
				analogWrite(FlowPWM, pwmSetting);
			}
			else
			{
				//decrease
				digitalWrite(FlowDIR, !FlowOn);
				analogWrite(FlowPWM, -pwmSetting);	// offsets the negative pwm value
			}
		}
		else
		{
			// stop motor
			analogWrite(FlowPWM, 0);
		}
		break;
	default:
		// standard valve, flow control only
		if (pwmSetting >= 0)
		{
			//increase
			digitalWrite(FlowDIR, FlowOn);
			analogWrite(FlowPWM, pwmSetting);
		}
		else
		{
			//decrease
			digitalWrite(FlowDIR, !FlowOn);
			analogWrite(FlowPWM, -pwmSetting);	// offsets the negative pwm value
		}
		break;
	}
}

