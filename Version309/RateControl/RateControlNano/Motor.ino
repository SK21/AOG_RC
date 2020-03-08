void motorDrive()
{
	if (ValveType == 1 && !RelaysOn)
	{
		// use fast close valve to turn off flow
		analogWrite(FlowPWM, 255);
		digitalWrite(FlowDIR, !FlowOn);
	}
	else
	{
		// adjust flow rate
		if (AutoOn)
		{
			// auto rate
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
			// manual rate
			if (RateUpMan)
			{
				digitalWrite(FlowDIR, FlowOn);
				analogWrite(FlowPWM, (MaxPWMvalue * pwmManualRatio / 100));
			}
			else if (RateDownMan)
			{
				digitalWrite(FlowDIR, !FlowOn);
				analogWrite(FlowPWM, (MaxPWMvalue * pwmManualRatio / 100));
			}
			else
			{
				// stop motor
				analogWrite(FlowPWM, 0);
			}
		}
	}
}


