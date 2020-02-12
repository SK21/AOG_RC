void ReadRateSwitch()
{
	SWreadTime = millis();

	RateUpPressed = !digitalRead(RateUpPin);
	RateDownPressed = !digitalRead(RateDownPin);

	// rate up
	if (RateUpPressed)
	{
		if (SWreadTime - RateLastTime > RateDelayTime)
		{
			if (AutoOn)
			{
				if (bitRead(OutCommand, 2))
				{
					if (!bitRead(OutCommand, 3))
					{
						bitSet(OutCommand, 3);
						bitClear(OutCommand, 2);
					}
				}
				else
				{
					bitSet(OutCommand, 2);
				}
				bitClear(OutCommand, 4); // left
				bitSet(OutCommand, 5); // rate up
			}
			else
			{
				RateUpMan = true;
				RateDownMan = false;
				switch (pwmManualRatio)
				{
				case 10:
					pwmManualRatio = 50;
					break;
				case 50:
					pwmManualRatio = 100;
					break;
				default:
					pwmManualRatio = 10;
					break;
				}
			}
			RateLastTime = SWreadTime;
		}
	}

	if (RateDownPressed)
	{
		if (SWreadTime - RateLastTime > RateDelayTime)
		{
			if (AutoOn)
			{
				if (bitRead(OutCommand, 2))
				{
					if (!bitRead(OutCommand, 3))
					{
						bitSet(OutCommand, 3);
						bitClear(OutCommand, 2);
					}
				}
				else
				{
					bitSet(OutCommand, 2);
				}
				bitClear(OutCommand, 4); // left
				bitClear(OutCommand, 5); // rate down
			}
			else
			{
				RateUpMan = false;
				RateDownMan = true;
				switch (pwmManualRatio)
				{
				case 10:
					pwmManualRatio = 50;
					break;
				case 50:
					pwmManualRatio = 100;
					break;
				default:
					pwmManualRatio = 10;
					break;
				}
			}
			RateLastTime = SWreadTime;
		}
	}

	// rate button not pressed
	if (!RateUpPressed && !RateDownPressed)
	{
		RateDownMan = false;
		RateUpMan = false;
		pwmManualRatio = 0;
		// clear rate values after delay
		if (SWreadTime - RateLastTime > SWdelay)
		{
			bitClear(OutCommand, 2);
			bitClear(OutCommand, 3);
			bitClear(OutCommand, 4);
			bitClear(OutCommand, 5);
		}
	}
}


