// ISOBUS_Speed.ino - ground speed from ISOBUS speed messages (ISOBUS build only)
// From jaworeA's RC11-isobus. Machine-selected speed is preferred, then ground-based, then
// wheel-based; without ISOBUS speed the local wheel sensor is used when one is configured.
// Used by the TC client (ISOBUS_TC.ino); the RC app's own rate path does not use it.

#if ISOBUS_ENABLED

const uint32_t ISOBUS_SPEED_TIMEOUT_MS = 1000;
const uint8_t SPEED_SOURCE_NONE = 0;
const uint8_t SPEED_SOURCE_MACHINE_SELECTED = 1;
const uint8_t SPEED_SOURCE_GROUND = 2;
const uint8_t SPEED_SOURCE_WHEEL = 3;

std::shared_ptr<isobus::SpeedMessagesInterface> ISOBUSspeed;
float ISOBUSspeedKmh = 0.0f;
uint8_t ISOBUSspeedSource = SPEED_SOURCE_NONE;
uint32_t ISOBUSspeedLastMs = 0;

void SPEED_SetKmh(float speedKmh, uint8_t source)
{
	ISOBUSspeedKmh = speedKmh;
	ISOBUSspeedSource = source;
	ISOBUSspeedLastMs = millis();
}

FLASHMEM void SPEED_Begin()
{
	ISOBUSspeed = std::make_shared<isobus::SpeedMessagesInterface>(ISOBUSicf);
	ISOBUSspeed->initialize();
}

void SPEED_Update()
{
	ISOBUSspeed->update();

	// speeds arrive in mm/s
	auto machineSelected = ISOBUSspeed->get_received_machine_selected_speed(0);
	auto groundBased = ISOBUSspeed->get_received_ground_based_speed(0);
	auto wheelBased = ISOBUSspeed->get_received_wheel_based_speed(0);

	if (machineSelected != nullptr)
	{
		SPEED_SetKmh(machineSelected->get_machine_speed() * 0.0036f, SPEED_SOURCE_MACHINE_SELECTED);
	}
	else if (groundBased != nullptr)
	{
		SPEED_SetKmh(groundBased->get_machine_speed() * 0.0036f, SPEED_SOURCE_GROUND);
	}
	else if (wheelBased != nullptr)
	{
		SPEED_SetKmh(wheelBased->get_machine_speed() * 0.0036f, SPEED_SOURCE_WHEEL);
	}
	else
	{
		// AgIsoStack drops a speed message once it is stale: missing speed is no speed,
		// never the last value (ISOBUS_Path_Forward.md "Speed source")
		ISOBUSspeedKmh = 0.0f;
		ISOBUSspeedSource = SPEED_SOURCE_NONE;
	}
}

bool SPEED_HasISOBUSSpeed()
{
	return (ISOBUSspeedSource != SPEED_SOURCE_NONE) && (millis() - ISOBUSspeedLastMs <= ISOBUS_SPEED_TIMEOUT_MS);
}

float SPEED_GetKmh()
{
	float Result = 0.0f;
	if (SPEED_HasISOBUSSpeed())
	{
		Result = ISOBUSspeedKmh;
	}
	else if (MDL.WheelSpeedPin != NC)
	{
		Result = WheelSpeed;
	}
	return Result;
}

#endif
