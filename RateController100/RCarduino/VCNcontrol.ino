
// https://www.k-state.edu/precisionag/variable-rate/automatic-section-control/index.html
// https://talk.newagtalk.com/forums/thread-view.asp?tid=82927&mid=594748#M594748
// http://ravenprecision.force.com/knowledgebase/articles/Tech_Tip/Valve-Cal-Number-Explained/?q=valve+cal&l=en_US&fs=Search&pn=1

// VCN - valve cal number
// four digit number can be represented by A B C D
// A is Backlash (1-9). It is a pulse of electricity to account for slack
//   in the adjustment components when changing direction. 1 is a short
//   pulse and 9 is a long pulse.
// 
// B is Speed (0-9). It is the speed of adjustment. 0 (fast) 12V, 9 (slow) 
//   1.2V.
//
// C is Brake Point (0-9). It is the % away from the target rate where
//   the speed of adjustment changes to a preset low rate. 0 - 5%, 1 - 10%
//   9 - 90%
//
// D is Deadband (1-9). It is the allowable error % where no adjustment
//   is made.
//
// Example: VCN (0)743
//	0 - backlash not used
//	7 - ajustment speed, about 3.6V
//  4 - brake point, at 40% away from target rate speed is reduced
//  3 - deadband 3%

long OldVCN;
byte VCNbacklash;	// A
byte VCNspeed;		// B
byte VCNbrake;		// C
byte VCNdeadband;	// D

int NewPWM;
float VCNerror;

unsigned long SendStart;
unsigned long WaitStart;

byte AdjustmentState = 0;		// 0 waiting, 1 sending pwm
bool LastDirectionPositive;		// adjustment direction
bool UseBacklashAdjustment;

int PartsTemp;

int VCNpwm(float cError, float cSetPoint, byte MinPWM, byte MaxPWM, long cVCN,
	float cFlowRate, long cSendTime, long cWaitTime, byte cSlowSpeed, byte cValveType)
{
	VCNparts(cVCN);

	// deadband
	float DB = (float)(VCNdeadband / 100.0) * cSetPoint;
	if (abs(cError) <= DB)
	{
		// valve does not need to be adjusted
		NewPWM = 0;
	}
	else
	{
		// backlash
		if (!UseBacklashAdjustment && VCNbacklash > 0)
		{
			if ((cError >= 0 && !LastDirectionPositive) | (cError < 0 && LastDirectionPositive))
			{
				// direction changed, use backlash adjustment
				UseBacklashAdjustment = true;
				SendStart = millis();
			}
			LastDirectionPositive = (cError >= 0);
		}

		if (UseBacklashAdjustment)
		{
			// backlash adjustment
			if (millis() - SendStart > (VCNbacklash * 10))
			{
				UseBacklashAdjustment = false;
				LastDirectionPositive = (cError >= 0);
				SendStart = millis();
			}
			else
			{
				NewPWM = MaxPWM - ((MaxPWM - MinPWM) * cSlowSpeed / 9);
				if (cError < 0) NewPWM *= -1;
			}
		}
		else
		{
			// regular adjustment
			if (AdjustmentState == 0)
			{
				// waiting
				if (millis() - WaitStart > cWaitTime)
				{
					// waiting finished
					AdjustmentState = 1;
					SendStart = millis();
				}
			}

			if (AdjustmentState == 1)
			{
				// sending pwm
				if (millis() - SendStart > cSendTime)
				{
					// sending finished
					AdjustmentState = 0;
					WaitStart = millis();
					NewPWM = 0;
				}
				else
				{
					// get new pwm value to send
					if (cFlowRate == 0 && cValveType == 1)
					{
						// open 'fast close' valve
						NewPWM = MaxPWM;
					}
					else
					{
						// % error
						if (cSetPoint > 0)
						{
							VCNerror = (cError / cSetPoint) * 100.0;
						}
						else
						{
							VCNerror = 0;
						}

						// set pwm value
						if (abs(VCNerror) < VCNbrake)
						{
							// slow adjustment
							NewPWM = MaxPWM - ((MaxPWM - MinPWM) * cSlowSpeed / 9);
						}
						else
						{
							// normal adjustment
							NewPWM = MaxPWM - ((MaxPWM - MinPWM) * VCNspeed / 9);
						}

						if (cError < 0) NewPWM *= -1;
					}
				}
			}
		}
	}
	return NewPWM;
}

void VCNparts(long NewVCN)
{
	if ((NewVCN != OldVCN) && (NewVCN <= 9999) && (NewVCN >= 0))
	{
		VCNbacklash = NewVCN / 1000;
		PartsTemp = NewVCN - VCNbacklash * 1000;

		VCNspeed = PartsTemp / 100;
		PartsTemp = PartsTemp - VCNspeed * 100;

		VCNbrake = PartsTemp / 10;

		VCNdeadband = PartsTemp - VCNbrake * 10;

		VCNbrake *= 10;
		if (VCNbrake == 0) VCNbrake = 5;

		OldVCN = NewVCN;
	}
}



