
void AdjustFlow()
{
    CheckPressureGate();
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        // Over-pressure gate overrides normal control: drive the flow-reducing direction.
        // Runs in auto and manual, and even if PID is gated off. SetPWM applies MDL.InvertFlow
        // uniformly, so the negative sign maps to the same physical direction the PID uses to
        // shed flow on this rig.
        if (PressureGateActive)
        {
            IntegralSum[i] = 0;	// no windup while gating
            switch (Sensor[i].ControlType)
            {
            case Motor_ct:
            case Fan_ct:
                Sensor[i].PWM = 0.0f;		// stop the pump/fan (the actuator is the flow source)
                break;

            default:
                Sensor[i].PWM = -255.0f;	// active relief: valve / combo close
                break;
            }
            SetPWM(i, Sensor[i].PWM);		// store in Sensor[i].PWM so PGN32400 reports the real output
        }
        else
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
}

void CheckPressureGate()
{
    // Layer 1 over-pressure gate. Sets PressureGateActive; AdjustFlow() drives the
    // flow-reducing direction while it is set. (Named CheckPressureGate here because the
    // Nano's CheckPressure() is the ADC reader.) Recovery is NOT pressure-alone: stopping
    // the source collapses pressure, which would release the gate and drive straight back
    // into the blockage. Instead: hysteresis band + minimum hold time + repeated trips in
    // a short window escalate to a hard latch that only an operator reset (master off) clears.

    // Disabled sentinel - clear all state and leave normal control alone.
    if (MDL.MaxPressureReading == 0xFFFF)
    {
        PressureGateActive = false;
        PressureGateLatched = false;
        PressureTripCount = 0;
        return;
    }

    // Operator reset: master off clears a hard latch and re-arms the gate.
    if (!MasterOn)
    {
        PressureGateActive = false;
        PressureGateLatched = false;
        PressureTripCount = 0;
        return;
    }

    // Hard latch holds relief until the reset above.
    if (PressureGateLatched)
    {
        PressureGateActive = true;
        return;
    }

    uint16_t releaseLevel = MDL.MaxPressureReading - (MDL.MaxPressureReading / 20);	// 5% hysteresis below trip

    if (PressureGateActive)
    {
        // Currently relieving - release only after the min hold AND pressure clearly below trip.
        if (millis() - PressureGateStart >= PressureMinHold && (uint16_t)PressureReading < releaseLevel)
        {
            PressureGateActive = false;
        }
    }
    else
    {
        // Not relieving - trip on over-pressure.
        if ((uint16_t)PressureReading > MDL.MaxPressureReading)
        {
            PressureGateActive = true;
            PressureGateStart = millis();

            // Count trips toward the hard latch: a re-trip within PressureTripResetMs
            // of the previous one is the same ongoing fault; a longer quiet spell
            // means recovery happened and earlier trips are forgiven. (A window
            // anchored at the first trip could never latch on slow steady cycling -
            // the count reset every time the window expired.)
            if (millis() - PressureTripLast > PressureTripResetMs) PressureTripCount = 0;
            PressureTripLast = millis();
            PressureTripCount++;

            // Repeated trips = persistent fault -> hard latch (require operator reset).
            if (PressureTripCount >= PressureMaxTrips) PressureGateLatched = true;
        }
    }
}

void SetPWM(byte ID, float pwmVal)
{
    const int maxDuty = (1 << PWM_BITS) - 1;
    int duty = (int)floorf(fabsf(pwmVal) * maxDuty / 255.0f);

    bool Increase = (pwmVal >= 0.0f);
    if (MDL.InvertFlow) Increase = !Increase;

#if PWM_BITS == 8
    duty = ditherAdjust(duty, fabsf(pwmVal));
#endif


#if defined(ESP32)
    if (Increase)
    {
        ledcWrite(Sensor[ID].IN1, duty);
        ledcWrite(Sensor[ID].IN2, 0);
    }
    else
    {
        ledcWrite(Sensor[ID].IN1, 0);
        ledcWrite(Sensor[ID].IN2, duty);
    }

#else
    digitalWrite(Sensor[ID].DirPin, Increase);
    analogWrite(Sensor[ID].PWMPin, duty);
#endif
}

#if PWM_BITS == 8
int ditherAdjust(int base, float val255)
{
    const int maxDuty = 255;
    float exactDuty = val255 * maxDuty / 255.0f;
    float frac = exactDuty - base;

    ditherCounter = (ditherCounter + 1) & 0x0F; // 16 step cycle
    if (frac > 0 && ditherCounter < (uint8_t)(frac * 16)) {
        base = min(base + 1, maxDuty);
    }
    return base;
}
#endif
