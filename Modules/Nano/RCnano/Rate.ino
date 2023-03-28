volatile unsigned long Duration[MaxProductCount];
volatile unsigned long Durations[2][12];
volatile byte DurCount[2];
volatile unsigned long avDurs[2];
volatile int avgPulses = 12;

volatile unsigned long PulseCount[MaxProductCount];
uint32_t LastPulse[MaxProductCount];

//unsigned long TimedCounts[MaxProductCount];
uint32_t RateInterval[MaxProductCount];
uint32_t RateTimeLast[MaxProductCount];
uint32_t PWMTimeLast[MaxProductCount];

unsigned long CurrentCount;
uint32_t CurrentDuration;

unsigned long PPM[MaxProductCount];		// pulse per minute * 100
//unsigned long Osum[MaxProductCount];
//unsigned long Omax[MaxProductCount];
//unsigned long Omin[MaxProductCount];
byte Ocount[MaxProductCount];
float Oave[MaxProductCount];

void ISR0()
{
	static unsigned long PulseTime;
	unsigned long micronow;
	unsigned long dur;

	micronow = micros();
	if (PulseTime > micronow)
	{
		dur = micronow - PulseTime;
	}
	else
	{
		dur = 0xFFFFFFFF + micronow - PulseTime;
	}

  if (dur > 1000000)
  {
    // the component was off so reset the values
    avDurs[0] = 0;
    dur = 50000;
    for(int i = 0; i < (avgPulses-1); i++)
    {
      Durations[0][i] = 0;
    }
    PulseTime = micronow;
    PulseCount[0]++;
  }
	else if (dur > MDL.Debounce * 1000)
	{
    PulseCount[0]++;
		// check to see if the dur value is too long like an interrupt was missed.
		if ((dur > (1.5 * avDurs[0])) && (avDurs[0] != 0))
		{
			Durations[0][DurCount[0]] = avDurs[0];
			Duration[0] = avDurs[0];
			dur = avDurs[0];
		}
		else
		{
			Durations[0][DurCount[0]] = dur;
			Duration[0] = dur;
		}

		PulseTime = micronow;
		if (DurCount[0] == 0)
		{
			DurCount[0]++;
			avDurs[0] = (Durations[0][avgPulses-1] + dur) / 2;
		}
		else if (DurCount[0] < avgPulses)
		{
			DurCount[0]++;
			avDurs[0] = (Durations[0][DurCount[0] - 1] + dur) / 2;
		}
		else
		{
			DurCount[0] = 0;
			avDurs[0] = (Durations[0][DurCount[0] - 1] + dur) / 2;
		}

	}
}

void ISR1()
{
	static unsigned long PulseTime;
	unsigned long micronow;
	unsigned long dur;

	micronow = micros();
	if (PulseTime > micronow)
	{
		dur = micronow - PulseTime;
	}
	else
	{
		dur = 0xFFFFFFFF + micronow - PulseTime;
	}
  //Serial.print("Duration: ");
  //Serial.println(dur);
	if (dur > 1000000)
  {
    // the component was off so reset the values
    //Serial.print("Reset durations");
    avDurs[1] = 0;
    dur = 50000;
    for(int i = 0; i< (avgPulses-1); i++)
    {
      Durations[1][i] = 0;
    }
    PulseTime = micronow;
    PulseCount[1]++;
  }
	
	else if (dur > MDL.Debounce * 1000)
	{
    PulseCount[1]++;
		// check to see if the dur value is too long like an interrupt was missed.
		if ((dur > (1.5 * avDurs[1])) && (avDurs[1] != 0))
		{
			Durations[1][DurCount[1]] = avDurs[1];
			Duration[1] = avDurs[1];
			dur = avDurs[1];
		}
		else
		{
			Durations[1][DurCount[1]] = dur;
			Duration[1] = dur;
		}

		PulseTime = micronow;
		if (DurCount[1] == 0)
		{
			DurCount[1]++;
			avDurs[1] = (Durations[1][avgPulses-1] + dur) / 2;
		}
		else if (DurCount[1] < (avgPulses-1))
		{
			DurCount[1]++;
			avDurs[1] = (Durations[1][DurCount[1] - 1] + dur) / 2;
		}
		else
		{
			DurCount[1] = 0;
			avDurs[1] = (Durations[1][DurCount[1] - 1] + dur) / 2;
		}

	}
}



void GetUPM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (Sensor[i].ControlType == 3)
		{
			// use weight
			Sensor[i].UPM = Sensor[i].MeterCal * (double)Sensor[i].pwmSetting;
		}
		else
		{
			// use flow meter
			GetUPMflow(i);
		}
	}
}

unsigned long GetAvgDuration(int ID)
{
	unsigned long dursum = 0;
  

	noInterrupts();
	for (int i = 0; i < avgPulses; i++)
	{
		dursum += Durations[ID][i];
	}
	interrupts();
	return dursum / avgPulses;
}

void GetUPMflow(int ID)
{
	if (PulseCount[ID])
	{
		noInterrupts();
		CurrentCount = PulseCount[ID];
		PulseCount[ID] = 0;
		CurrentDuration = Duration[ID];
		interrupts();

		if (Sensor[ID].UseMultiPulses)
		{
			// low ms/pulse, use pulses over time
			PPM[ID] = 60000000 / GetAvgDuration(ID);
      Serial.print("PPM: ");
      Serial.println(PPM[ID]);






			/*/
			TimedCounts[ID] += CurrentCount;
			RateInterval[ID] = millis() - RateTimeLast[ID];
			if (RateInterval[ID] > 500)
			{
				RateTimeLast[ID] = millis();
				PPM[ID] = (6000000 * TimedCounts[ID]) / RateInterval[ID];	// 100 X actual
				TimedCounts[ID] = 0;
			}
			*/

		}
		else
		{
			// high ms/pulse, use time for one pulse
			if (CurrentDuration == 0)
			{
				PPM[ID] = 0;
			}
			else
			{
				PPM[ID] = 60000000 / CurrentDuration;
			}
		}


		LastPulse[ID] = millis();
		Sensor[ID].TotalPulses += CurrentCount;
	}

	if (millis() - LastPulse[ID] > 4000)	PPM[ID] = 0;	// check for no flow
	/*
	// olympic average
	Osum[ID] += PPM[ID];
	if (Omax[ID] < PPM[ID]) Omax[ID] = PPM[ID];
	if (Omin[ID] > PPM[ID]) Omin[ID] = PPM[ID];

	Ocount[ID]++;
	if (Ocount[ID] > 4)
	{
		Osum[ID] -= Omax[ID];
		Osum[ID] -= Omin[ID];
 		Oave[ID] = (float)Osum[ID] / 300.0;	// divide by 3 samples and divide by 100 for decimal place
		Osum[ID] = 0;
		Omax[ID] = 0;
		Omin[ID] = 5000000000;
		Ocount[ID] = 0;
	}
	*/


	// units per minute
	if (Sensor[ID].MeterCal > 0)
	{
		Sensor[ID].UPM = (float)PPM[ID] / (float)Sensor[ID].MeterCal;
	}
	else
	{
		Sensor[ID].UPM = 0;
	}
}
