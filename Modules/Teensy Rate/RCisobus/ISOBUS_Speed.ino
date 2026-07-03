const uint32_t ISOBUS_SPEED_TIMEOUT_MS = 1000;
const uint8_t SPEED_SOURCE_NONE = 0;
const uint8_t SPEED_SOURCE_MACHINE_SELECTED = 1;
const uint8_t SPEED_SOURCE_GROUND = 2;
const uint8_t SPEED_SOURCE_WHEEL = 3;
const uint8_t SPEED_SOURCE_LOCAL_WHEEL = 4;

void SPEED_SetKmh(float speedKmh, uint8_t source)
{
	ISOBUSSpeedKmh = speedKmh;
	ISOBUSSpeedSource = source;
	ISOBUSSpeedLastMs = millis();
}

FLASHMEM void SPEED_Begin()
{
	ISOBUSSpeedMessages = std::make_shared<isobus::SpeedMessagesInterface>(ISOBUSControlFunction);
	ISOBUSSpeedMessages->initialize();
}

FLASHMEM void SPEED_Update()
{
	if (ISOBUSSpeedMessages == nullptr) return;

	ISOBUSSpeedMessages->update();

	auto machineSelected = ISOBUSSpeedMessages->get_received_machine_selected_speed(0);
	if (machineSelected != nullptr)
	{
		SPEED_SetKmh(machineSelected->get_machine_speed() * 0.0036f, SPEED_SOURCE_MACHINE_SELECTED);
		return;
	}

	auto groundBased = ISOBUSSpeedMessages->get_received_ground_based_speed(0);
	if (groundBased != nullptr)
	{
		SPEED_SetKmh(groundBased->get_machine_speed() * 0.0036f, SPEED_SOURCE_GROUND);
		return;
	}

	auto wheelBased = ISOBUSSpeedMessages->get_received_wheel_based_speed(0);
	if (wheelBased != nullptr)
	{
		SPEED_SetKmh(wheelBased->get_machine_speed() * 0.0036f, SPEED_SOURCE_WHEEL);
		return;
	}

	if (millis() - ISOBUSSpeedLastMs > ISOBUS_SPEED_TIMEOUT_MS)
	{
		ISOBUSSpeedKmh = 0.0f;
		ISOBUSSpeedSource = SPEED_SOURCE_NONE;
	}
}

bool SPEED_HasISOBUSSpeed()
{
	return (ISOBUSSpeedSource != SPEED_SOURCE_NONE) && (millis() - ISOBUSSpeedLastMs <= ISOBUS_SPEED_TIMEOUT_MS);
}

float SPEED_GetKmh()
{
	if (SPEED_HasISOBUSSpeed()) return ISOBUSSpeedKmh;
	if (MDL.WheelSpeedPin != NC) return WheelSpeed;
	return 0.0f;
}

const char *SPEED_SourceName()
{
	if (!SPEED_HasISOBUSSpeed() && MDL.WheelSpeedPin != NC) return "LOCAL";

	switch (ISOBUSSpeedSource)
	{
	case SPEED_SOURCE_MACHINE_SELECTED:
		return "MSS";
	case SPEED_SOURCE_GROUND:
		return "GROUND";
	case SPEED_SOURCE_WHEEL:
		return "WHEEL";
	default:
		return "NONE";
	}
}
