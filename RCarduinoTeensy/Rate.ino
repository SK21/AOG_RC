float CurrentCounts[SensorCount];
unsigned long LastPulse[SensorCount];
unsigned long TimedCounts[SensorCount];

unsigned long RateInterval[SensorCount];
unsigned long RateTimeLast[SensorCount];
float CurrentDuration[SensorCount];
float PPM[SensorCount];

volatile float pulseDuration[SensorCount];
volatile bool pulseState[SensorCount];
volatile float pulseCount[SensorCount];
int MinPulseTime[SensorCount];

float KalResult[SensorCount];
float KalPC[SensorCount];
float KalG[SensorCount];
float KalP[] = { 1.0,1.0,1.0,1.0,1.0 };
float KalVariance[] = { 0.01,0.01,0.01,0.01,0.01 };
float KalProcess[] = { 0.005,0.005,0.005,0.005,0.005 };

void ISR0()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount[0]++;

        pulseState[0] = !pulseState[0];

        if (pulseState[0])
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration[0] = millis() - pulseStart;
            pulseStart = 1;
        }
    }
}

void ISR1()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount[1]++;

        pulseState[1] = !pulseState[1];

        if (pulseState[1])
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration[1] = millis() - pulseStart;
            pulseStart = 1;
        }
    }
}

void ISR2()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount[2]++;

        pulseState[2] = !pulseState[2];

        if (pulseState[2])
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration[2] = millis() - pulseStart;
            pulseStart = 1;
        }
    }
}

void ISR3()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount[3]++;

        pulseState[3] = !pulseState[3];

        if (pulseState[3])
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration[3] = millis() - pulseStart;
            pulseStart = 1;
        }
    }
}

void ISR4()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount[4]++;

        pulseState[4] = !pulseState[4];

        if (pulseState[4])
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration[4] = millis() - pulseStart;
            pulseStart = 1;
        }
    }
}

void GetUPM()
{
    for (int i = 0; i < SensorCount; i++)
    {
        CurrentCounts[i] = pulseCount[i];
        pulseCount[i] = 0;
        if (pulseDuration[i] > 5)CurrentDuration[i] = pulseDuration[i];

        // check for no PPM
        if (millis() - LastPulse[i] > 4000)
        {
            pulseDuration[i] = 0;
            CurrentDuration[i] = 0;
            PPM[i] = 0;
        }

        if (CurrentCounts[i] > 0) LastPulse[i] = millis();

        // accumulated total
        TotalPulses[i] += CurrentCounts[i];

        // ppm
        if (UseMultiPulses[i])
        {
            // low ms/pulse, use pulses over time
            TimedCounts[i] += CurrentCounts[i];
            RateInterval[i] = millis() - RateTimeLast[i];
            if (RateInterval[i] > 200)
            {
                RateTimeLast[i] = millis();
                PPM[i] = (60000.0 * TimedCounts[i]) / RateInterval[i];
                TimedCounts[i] = 0;
            }
        }
        else
        {
            // high ms/pulse, use time for one pulse
            if (CurrentDuration[i] > 0)PPM[i] = 60000.0 / CurrentDuration[i];
        }

        // Kalmen filter
        KalPC[i] = KalP[i] + KalProcess[i];
        KalG[i] = KalPC[i] / (KalPC[i] + KalVariance[i]);
        KalP[i] = (1.0 - KalG[i]) * KalPC[i];
        KalResult[i] = KalG[i] * (PPM[i] - KalResult[i]) + KalResult[i];
        PPM[i] = KalResult[i];

        // units per minute
        if (MeterCal[i] > 0)
        {
            UPM[i] = PPM[i] / MeterCal[i];
        }
        else
        {
            UPM[i] = 0;
        }
    }
}