void ReadRateSwitch()
{
	SWreadTime = millis();

	RateUpPressed = !digitalRead(RateUpPin);
	RateDownPressed = !digitalRead(RateDownPin);

	// rate up
	if (RateUpPressed)
	{
		if (AutoOn)
		{
			// auto rate
			if (SWreadTime - RateLastTime > RateDelayTime)
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
				RateLastTime = SWreadTime;
			}
		}
		else
		{
			// manual rate
			RateUpMan = true;
			RateDownMan = false;
		}
	}

	if (RateDownPressed)
	{
		if (AutoOn)
		{
			// auto rate
			if (SWreadTime - RateLastTime > RateDelayTime)
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
				RateLastTime = SWreadTime;
			}
		}
		else
		{
			// manual rate
			RateUpMan = false;
			RateDownMan = true;
		}
	}

	// rate button not pressed
	if (!RateUpPressed && !RateDownPressed)
	{
		RateDownMan = false;
		RateUpMan = false;
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


