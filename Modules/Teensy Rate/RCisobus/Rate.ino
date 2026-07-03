// PulseMinHz       minimum Hz of the flow sensor, actual X 10
// PulseMaxHz       maximum Hz of the flow sensor
// PulseSampeSize   number of pulses used to get the median Hz reading

uint32_t LastPulse[2];
uint32_t ReadLast[2];
uint32_t PulseTime[2];

volatile uint32_t Samples[2][MaxSampleSize];
volatile uint16_t PulseCount[2];
volatile uint8_t SamplesCount[2];
volatile uint8_t SamplesIndex[2];

uint32_t RateControl_LastAreaMs = 0;

float RateControl_PulsesToUnits(uint8_t productIndex, uint32_t pulses)
{
	if (productIndex >= MDL.SensorCount) return 0.0f;
	if (Sensor[productIndex].MeterCal <= 0.0f) return 0.0f;
	return static_cast<float>(pulses) / Sensor[productIndex].MeterCal;
}

float RateControl_TotalAppliedUnits()
{
	return Machine.TripAppliedUnits;
}

float RateControl_TotalAreaHa()
{
	return Machine.TripAreaHa;
}

float RateControl_LifetimeAppliedUnits()
{
	return Machine.LifetimeAppliedUnits;
}

float RateControl_LifetimeAreaHa()
{
	return Machine.LifetimeAreaHa;
}

void RateControl_SaveTankIfNeeded(bool force)
{
	static uint32_t lastTankSave = 0;
	static float lastSavedRemaining = -1.0f;
	static float lastSavedTripUnits = -1.0f;
	static float lastSavedTripArea = -1.0f;
	static float lastSavedLifetimeUnits = -1.0f;
	static float lastSavedLifetimeArea = -1.0f;

	if (!force &&
	    ((millis() - lastTankSave) < 30000) &&
	    (fabsf(Machine.TankRemainingUnits - lastSavedRemaining) < 1.0f) &&
	    (fabsf(Machine.TripAppliedUnits - lastSavedTripUnits) < 1.0f) &&
	    (fabsf(Machine.TripAreaHa - lastSavedTripArea) < 0.01f) &&
	    (fabsf(Machine.LifetimeAppliedUnits - lastSavedLifetimeUnits) < 1.0f) &&
	    (fabsf(Machine.LifetimeAreaHa - lastSavedLifetimeArea) < 0.01f))
	{
		return;
	}

	lastTankSave = millis();
	lastSavedRemaining = Machine.TankRemainingUnits;
	lastSavedTripUnits = Machine.TripAppliedUnits;
	lastSavedTripArea = Machine.TripAreaHa;
	lastSavedLifetimeUnits = Machine.LifetimeAppliedUnits;
	lastSavedLifetimeArea = Machine.LifetimeAreaHa;
	SaveMachineSettings();
}

void RateControl_SubtractTankPulses(uint8_t productIndex, uint32_t pulses)
{
	if (pulses == 0) return;

	const float usedUnits = RateControl_PulsesToUnits(productIndex, pulses);
	if (usedUnits <= 0.0f) return;

	Machine.TripAppliedUnits += usedUnits;
	Machine.LifetimeAppliedUnits += usedUnits;
	if (Machine.TankRemainingUnits > 0.0f)
	{
		Machine.TankRemainingUnits -= usedUnits;
		if (Machine.TankRemainingUnits < 0.0f) Machine.TankRemainingUnits = 0.0f;
	}
	RateControl_SaveTankIfNeeded(false);
}

void RateControl_ResetTripCounters()
{
	Machine.TripAreaHa = 0.0f;
	Machine.TripAppliedUnits = 0.0f;
	RateControl_SaveTankIfNeeded(true);
}

void RateControl_AddTankUnits(float units)
{
	if (units <= 0.0f) return;

	Machine.TankRemainingUnits += units;
	if ((Machine.TankCapacityUnits > 0.0f) && (Machine.TankRemainingUnits > Machine.TankCapacityUnits))
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
	}
	RateControl_SaveTankIfNeeded(true);
}

void PulseISR(uint8_t ID)
{
	if (RelayLo > 0 || RelayHi > 0)
	{
		uint32_t ReadTime = micros();
		PulseTime[ID] = ReadTime - ReadLast[ID];
		ReadLast[ID] = ReadTime;

		if (PulseTime[ID] > Sensor[ID].PulseMin && PulseTime[ID] < Sensor[ID].PulseMax)			
		{
			// valid pulses
			PulseCount[ID]++;
			Samples[ID][SamplesIndex[ID]] = PulseTime[ID];
			SamplesIndex[ID] = (SamplesIndex[ID] + 1) % Sensor[ID].PulseSampleSize;
			if (SamplesCount[ID] < Sensor[ID].PulseSampleSize) SamplesCount[ID]++;
		}
	}
}

void GetUPM()
{
	for (int i = 0; i < MDL.SensorCount; i++)
	{
		if (PulseCount[i])
		{
			LastPulse[i] = millis();
			uint16_t pulses = 0;

			noInterrupts();
			pulses = PulseCount[i];
			Sensor[i].TotalPulses += pulses;
			PulseCount[i] = 0;
			uint16_t count = SamplesCount[i];
			uint32_t Snapshot[MaxSampleSize];
			for (uint16_t k = 0; k < count; k++)
			{
				Snapshot[k] = Samples[i][k];
			}
			interrupts();
			RateControl_SubtractTankPulses(i, pulses);

			uint32_t median = MedianFromArray(Snapshot, count);

			if (median > 0)
			{
				float hz = 1000000.0 / median;
				Sensor[i].Hz = hz * 0.8 + Sensor[i].Hz * 0.2;
				if (Sensor[i].MeterCal > 0) Sensor[i].UPM = (60.0 * Sensor[i].Hz) / Sensor[i].MeterCal;
			}
		}
		else
		{
			// No flow check
			if (millis() - LastPulse[i] > FlowTimeout || (RelayLo == 0 && RelayHi == 0))
			{
				Sensor[i].UPM = 0;
				Sensor[i].Hz = 0;

				noInterrupts();
				SamplesCount[i] = 0;
				SamplesIndex[i] = 0;
				interrupts();
			}
		}
	}
}

float RateControl_ActiveWidthM()
{
	float widthM = 0.0f;

	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		bool sectionOn = (i < 8) ? bitRead(RelayLo, i) : bitRead(RelayHi, i - 8);
		if (sectionOn) widthM += Machine.SectionWidthCm[i] * 0.01f;
	}

	return widthM;
}

void RateControl_UpdateAreaCounter()
{
	const uint32_t now = millis();
	if (RateControl_LastAreaMs == 0)
	{
		RateControl_LastAreaMs = now;
		return;
	}

	uint32_t elapsedMs = now - RateControl_LastAreaMs;
	RateControl_LastAreaMs = now;
	if (elapsedMs > 1000) elapsedMs = 1000;

	const float speedKmh = SPEED_GetKmh();
	const float activeWidthM = RateControl_ActiveWidthM();
	if (!MasterOn || !WorkPinOn() || speedKmh <= 0.05f || activeWidthM <= 0.01f) return;

	const float deltaHa = (speedKmh * activeWidthM * static_cast<float>(elapsedMs)) / 36000000.0f;
	Machine.TripAreaHa += deltaHa;
	Machine.LifetimeAreaHa += deltaHa;
	RateControl_SaveTankIfNeeded(false);
}

float RateControl_TargetUPMFromAreaRate(float targetLHa, float speedKmh, float activeWidthM)
{
	if (!MasterOn || targetLHa <= 0.0f || speedKmh <= 0.05f || activeWidthM <= 0.01f) return 0.0f;
	return (targetLHa * speedKmh * activeWidthM) / 600.0f;
}

float RateControl_ActualRateLHa(uint8_t productIndex)
{
	float speedKmh = SPEED_GetKmh();
	float activeWidthM = RateControl_ActiveWidthM();

	if (productIndex >= MDL.SensorCount || speedKmh <= 0.05f || activeWidthM <= 0.01f) return 0.0f;
	return (Sensor[productIndex].UPM * 600.0f) / (speedKmh * activeWidthM);
}

void RateControl_UpdateTargets()
{
	float speedKmh = SPEED_GetKmh();
	float activeWidthM = RateControl_ActiveWidthM();

	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		if (Machine.AutoRate[i])
		{
			Sensor[i].AutoOn = true;
			Sensor[i].TargetUPM = RateControl_TargetUPMFromAreaRate(Machine.TargetRateLHa[i], speedKmh, activeWidthM);
		}
		else
		{
			Sensor[i].AutoOn = false;
			Sensor[i].ManualAdjust = Machine.ManualPWM[i];
			Sensor[i].TargetUPM = 0.0f;
		}
	}
}

std::int32_t RateControl_LHaToTCValue(float rateLHa)
{
	return static_cast<std::int32_t>(rateLHa * 10000.0f);
}

float RateControl_TCValueToLHa(std::int32_t processVariableValue)
{
	if (processVariableValue < 0) return 0.0f;
	return processVariableValue * 0.0001f;
}

void ISR0()
{
	PulseISR(0);
}

void ISR1()
{
	PulseISR(1);
}
