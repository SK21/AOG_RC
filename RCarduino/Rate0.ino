float CurrentCounts0;
unsigned long LastPulse0;
unsigned long TimedCounts0;

unsigned long RateInterval0;
unsigned long RateTimeLast0;
float CurrentDuration0;
float PPM0;

volatile float pulseDuration0;
volatile bool pulseState0;
volatile float pulseCount0 = 0;
int MinPulseTime0;

float KalResult0 = 0;
float KalPc0 = 0.0;
float KalG0 = 0.0;
float KalP0 = 1.0;
float KalVariance0 = 0.01;	// larger is more filtering
float KalProcess0 = 0.005;	// smaller is more filtering

void PPM0isr()
{
    // measure time for one pulse
    static unsigned long pulseStart = 0;

    if (millis() - pulseStart > 5)
    {
        pulseCount0++;

        pulseState0 = !pulseState0;
        if (pulseState0)
        {
            pulseStart = millis();
        }
        else
        {
            pulseDuration0 = millis() - pulseStart;
            pulseStart = 0;
        }
    }
}

void GetUPM0()
{
    CurrentCounts0 = pulseCount0;
    pulseCount0 = 0;

    if (pulseDuration0 > 5) CurrentDuration0 = pulseDuration0;

    // check for no PPM
    if (millis() - LastPulse0 > 4000)
    {
        pulseDuration0 = 0;
        CurrentDuration0 = 0;
        PPM0 = 0;
    }

    if (CurrentCounts0 > 0) LastPulse0 = millis();

    // accumulated total
    TotalPulses[0] += CurrentCounts0;

    // ppm
    if (UseMultiPulses[0])
    {
        // low ms/pulse, use pulses over time
        TimedCounts0 += CurrentCounts0;
        RateInterval0 = millis() - RateTimeLast0;
        if (RateInterval0 > 200)
        {
            RateTimeLast0 = millis();
            PPM0 = (60000.0 * TimedCounts0) / RateInterval0;
            TimedCounts0 = 0;
        }
    }
    else
    {
        // high ms/pulse, use time for one pulse
        if (CurrentDuration0 > 0) PPM0 = 60000.0 / CurrentDuration0;
    }

    // Kalmen filter
    KalPc0 = KalP0 + KalProcess0;
    KalG0 = KalPc0 / (KalPc0 + KalVariance0);
    KalP0 = (1.0 - KalG0) * KalPc0;
    KalResult0 = KalG0 * (PPM0 - KalResult0) + KalResult0;
    PPM0 = KalResult0;

    // units per minute
    if (MeterCal[0] > 0)
    {
        UPM[0] = PPM0 / MeterCal[0];
    }
    else
    {
        UPM[0] = 0;
    }
}

