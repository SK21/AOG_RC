unsigned long CurrentCounts;
unsigned long LastPulse;
unsigned long TimedCounts;

unsigned long RateInterval;
unsigned long RateTimeLast;
float CurrentDuration;
float PPM;

volatile unsigned long pulseDuration;
volatile bool pulseState;
volatile unsigned long pulseCount = 0;
volatile int MinPulseTime;

float KalResult = 0;
float KalPc = 0.0;
float KalG = 0.0;
float KalP = 1.0;
float KalVariance = 0.01;	// larger is more filtering
float KalProcess = 0.005;	// smaller is more filtering

void PPMisr()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    pulseState = !pulseState;
    if (pulseState)
    {
        pulseStart = millis();
    }
    else
    {
        pulseDuration = millis() - pulseStart;
        pulseCount += (pulseDuration > MinPulseTime);
        pulseStart = 0;
    }
}

float GetUPM()
{
    SetMinPulseTime();

    // check for no PPM
    if (millis() - LastPulse > 4000)
    {
        pulseDuration = 0;
        CurrentDuration = 0;
        PPM = 0;
    }
    if (pulseCount > 0)
    {
        LastPulse = millis();
    }


    // accumulated total
    CurrentCounts = pulseCount;
    pulseCount = 0;
    TotalPulses += CurrentCounts;

    // ppm
    if (MinPulseTime == 0)
    {
        // low ms/pulse
        TimedCounts += CurrentCounts;
        RateInterval = millis() - RateTimeLast;
        if (RateInterval > 200)
        {
            RateTimeLast = millis();
            PPM = (60000.0 * TimedCounts) / RateInterval;
            TimedCounts = 0;
        }
    }
    else
    {
        // high ms/pulse
        if (pulseDuration > MinPulseTime) CurrentDuration = pulseDuration;
        if (CurrentDuration > 0) PPM = 60000 / CurrentDuration;
    }

    // Kalmen filter
    KalPc = KalP + KalProcess;
    KalG = KalPc / (KalPc + KalVariance);
    KalP = (1 - KalG) * KalPc;
    KalResult = KalG * (PPM - KalResult) + KalResult;
    PPM = KalResult;

    // units per minute
    if (MeterCal > 0)
    {
        UPM = PPM / MeterCal;
    }
    else
    {
        UPM = 0;
    }
    return UPM;
}

void SetMinPulseTime()
{
    // ms/pulse = 60000 / ((units per minute) * (counts per unit))
    float Ms = rateSetPoint * MeterCal;
    if (Ms > 0)
    {
        Ms = 60000.0 / Ms;
    }
    else
    {
        Ms = 0;
    }

    if (Ms < LowMsPulseTrigger)
    {
        // low ms/pulse
        MinPulseTime = 0;
    }
    else
    {
        // high ms/pulse
        MinPulseTime = 5;
    }
}


