
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

// Metric (L/ha, kg/ha): rate × speed(km/h) × width(m) / 600 = UPM
// Imperial (gal/ac, lbs/ac): rate × speed(km/h) × width(m) / 242.811 = UPM
// 242.811 = 4046.856 m²/ac × 60 min/h / 1000 m/km
static float RateControl_UnitFactor()
{
	return (Machine.UnitMode <= 1) ? 600.0f : 242.811f;
}

float RateControl_TargetUPMFromAreaRate(float targetRate, float speedKmh, float activeWidthM)
{
	if (!MasterOn || targetRate <= 0.0f || speedKmh <= 0.05f || activeWidthM <= 0.01f) return 0.0f;
	return (targetRate * speedKmh * activeWidthM) / RateControl_UnitFactor();
}

float RateControl_UnitsPerArea(uint8_t productIndex)
{
	float speedKmh = SPEED_GetKmh();
	float activeWidthM = RateControl_ActiveWidthM();

	if (productIndex >= MDL.SensorCount || speedKmh <= 0.05f || activeWidthM <= 0.01f) return 0.0f;
	return (Sensor[productIndex].UPM * RateControl_UnitFactor()) / (speedKmh * activeWidthM);
}

void RateControl_UpdateTargets()
{
	float speedKmh = SPEED_GetKmh();
	float activeWidthM = RateControl_ActiveWidthM();

	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		if (AutoOn)
		{
			Sensor[i].TargetUPM = RateControl_TargetUPMFromAreaRate(Machine.TargetRateLHa[i], speedKmh, activeWidthM);
		}
		else
		{
			Sensor[i].ManualAdjust = Machine.ManualPWM[i];
			Sensor[i].TargetUPM = 0.0f;
		}
	}
}
