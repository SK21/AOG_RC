
void AdjustFlow()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		float clamped = constrain(Sensor[i].PWM, -255.0f, 255.0f);

		switch (Sensor[i].ControlType)
		{
		case StandardValve_ct:
			SetPWM(i, SensorConnected[i] ? clamped : 0.0f);
			break;

		case Motor_ct:
		case Fan_ct:
			SetPWM(i, (SensorConnected[i] && Applying[i]) ? clamped : 0.0f);
			break;

		case ComboClose_ct:
		case TimedCombo_ct:
			// fast close valve or combo close timed, used for flow control and on/off
			SetPWM(i, SensorConnected[i] && Applying[i] ? clamped : -255.0f);
			break;

		default:
			break;
		}
	}
}

void SetPWM(byte ID, float pwmVal)
{
	const int maxDuty = (1 << PWM_BITS) - 1;
	int duty = (int)floorf(fabsf(pwmVal) * maxDuty / 255.0f);

	bool Direction = (pwmVal >= 0.0f);
	if (MDL.InvertFlow) Direction = !Direction;

	digitalWrite(Sensor[ID].DirPin, Direction);
	analogWrite(Sensor[ID].PWMPin, duty);
}

