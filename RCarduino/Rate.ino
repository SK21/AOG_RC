float CurrentCounts[SensorCount];
unsigned long LastPulse[SensorCount];
unsigned long TimedCounts[SensorCount];

unsigned long RateInterval[SensorCount];
unsigned long RateTimeLast[SensorCount];
float CurrentDuration[SensorCount];
float PPM[SensorCount];

volatile float pulseDuration[SensorCount];
volatile bool pulseState[SensorCount];
volatile float pulseCount[SensorCount] = {0, 0};
volatile int MinPulseTime[SensorCount];

float KalResult[SensorCount] = {0, 0};
float KalPc[SensorCount] = {0.0, 0.0};
float KalG[SensorCount] = {0.0, 0.0};
float KalP[SensorCount] = {1.0, 1.0};
float KalVariance = 0.01;	// larger is more filtering
float KalProcess = 0.005;	// smaller is more filtering

void PPM0isr()
{
  // measure time for one pulse
  static unsigned long pulseStart = 0;

  if (millis() - pulseStart > MinPulseTime[0])
  {
    pulseState[0] = !pulseState[0];

    if (pulseState[0])
    {
      pulseStart = millis();
    }
    else
    {
      pulseDuration[0] = millis() - pulseStart;
      pulseCount[0] ++;
      pulseStart = 0;
    }
  }
}

void PPM1isr()
{
  // measure time for one pulse
  static unsigned long pulseStart = 0;

  if (millis() - pulseStart > MinPulseTime[1])
  {
    pulseState[1] = !pulseState[1];

    if (pulseState[1])
    {
      pulseStart = millis();
    }
    else
    {
      pulseDuration[1] = millis() - pulseStart;
      pulseCount[1] ++;
      pulseStart = 0;
    }
  }
}

float GetUPM(byte SensorID)
{
  SetMinPulseTime(SensorID);

  // check for no PPM
  if (millis() - LastPulse[SensorID] > 4000)
  {
    pulseDuration[SensorID] = 0;
    CurrentDuration[SensorID] = 0;
    PPM[SensorID] = 0;
  }
  if (pulseCount[SensorID] > 0)
  {
    LastPulse[SensorID] = millis();
  }


  // accumulated total
  CurrentCounts[SensorID] = pulseCount[SensorID];
  pulseCount[SensorID] = 0;
  TotalPulses[SensorID] += CurrentCounts[SensorID];

  // ppm
  if (MinPulseTime[SensorID] == 0)
  {
      // low ms/pulse, use pulses over time
    TimedCounts[SensorID] += CurrentCounts[SensorID];
    RateInterval[SensorID] = millis() - RateTimeLast[SensorID];
    if (RateInterval[SensorID] > 200)
    {
      RateTimeLast[SensorID] = millis();
      PPM[SensorID] = (60000.0 * TimedCounts[SensorID]) / RateInterval[SensorID];
      TimedCounts[SensorID] = 0;
    }
  }
  else
  {
      // high ms/pulse, use time for one pulse
    if (pulseDuration[SensorID] > MinPulseTime[SensorID]) CurrentDuration[SensorID] = pulseDuration[SensorID];
    if (CurrentDuration[SensorID] > 0) PPM[SensorID] = 60000.0 / CurrentDuration[SensorID];
  }

  // Kalmen filter
  KalPc[SensorID] = KalP[SensorID] + KalProcess;
  KalG[SensorID] = KalPc[SensorID] / (KalPc[SensorID] + KalVariance);
  KalP[SensorID] = (1.0 - KalG[SensorID]) * KalPc[SensorID];
  KalResult[SensorID] = KalG[SensorID] * (PPM[SensorID] - KalResult[SensorID]) + KalResult[SensorID];
  PPM[SensorID] = KalResult[SensorID];

  // units per minute
  if (MeterCal[SensorID] > 0)
  {
    UPM[SensorID] = PPM[SensorID] / MeterCal[SensorID];
  }
  else
  {
    UPM[SensorID] = 0;
  }

  return UPM[SensorID];
}

void SetMinPulseTime(byte SensorID)
{
  // ms/pulse = 60000 / ((units per minute) * (counts per unit))
  float Ms = RateSetting[SensorID] * MeterCal[SensorID];
  if (Ms > 0)
  {
    Ms = 60000.0 / Ms;
  }
  else
  {
    Ms = 0;
  }

  if (Ms < LowMsPulseTrigger[SensorID])
  {
    // low ms/pulse
    MinPulseTime[SensorID] = 0;
  }
  else
  {
    // high ms/pulse
    MinPulseTime[SensorID] = 5;
  }
}
