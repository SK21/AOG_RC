void motorDrive()
{
	if (ValveType == 1 && !RelaysOn)
	{
		// use Raven 'fast close' valve to turn off flow
		analogWrite(FlowPWM, 255);
		digitalWrite(FlowDIR, !FlowOn);
	}
	else
	{
		// adjust flow rate
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
}

