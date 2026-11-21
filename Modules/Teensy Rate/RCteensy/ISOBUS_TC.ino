// ISOBUS_TC.ino - Task Controller client (ISOBUS build only)
// From jaworeA's RC11-isobus. The module uploads a DDOP (one boom, its sections, rate and
// speed process data) to a TC: a tractor terminal or AOG-TaskController. While the TC
// commands, it switches the sections (setpoint condensed work state, DDI 290; the module
// reports the actual state, DDI 161) and sets product 1's rate, which becomes
// Sensor[0].TargetUPM from speed and active width.
//
// Rate units: the TC sends the rate in the ISO unit of its DDI (1 litres, 6 kg, 11 seeds per
// ha); the board meters in RC's units (its flow cal is pulses per RC unit). RC sends the type
// and how many metered units are one litre, kg or seed (PGN 32509 / CAN 0xFF1D, TC_SetUnits),
// stored in TCsettings so a board under a terminal without RC keeps them. The DDOP declares
// that type's rate DDIs. Until RC has sent units, the TC's rate is not applied.
//
// Command authority (ISOBUS_Path_Forward.md "Command authority"):
// - RC: at power-up and after a TC session. RC's rate and relay commands apply, as today.
//   A connected TC is not a command source.
// - TC: from the TC's first section or rate value command. RC's rate and relay commands
//   (0xFF03/0xFF04, UDP 32500/32501) are ignored for the whole module. Entering clears every
//   target, master and the sections; only product 1 (Sensor[0]) is driven, in auto, and only
//   its comm time is kept fresh, so products 2-6 stop and then go to comm loss.
// - Back to RC when the TC disconnects: everything cleared and every sensor's comm time
//   expired, so the module is in RC's comm-loss state at once until RC's next command.
//   A TC that stops sending is noticed after the TC client's 6 s status timeout (AgIsoStack).
// Value commands are accepted as soon as the client is connected (AOG-TaskController sends
// its first section state about 2 s after the DDOP is activated, and only resends on change);
// the module's own reports wait 3 s (TC_ProcessDataReady).
// Config, telemetry and firmware update are never blocked.
//
// Changed from RC11-isobus:
// - no output is touched while the TC isn't commanding (RC11-isobus cleared them every loop,
//   which would stop RC's own control on a board with no terminal)
// - the TC's target rate is kept in RAM only (the TC sends it again); section geometry is
//   stored in EEPROM at TCsettingsAddr (RC11-isobus used 640, inside mainline's sensors 3-6)
// - no VT client yet (Phase E run screen); no manual mode from the terminal yet

#if ISOBUS_ENABLED

const uint16_t TC_ELEMENT_BOOM = 1;
const uint16_t TC_OBJ_BOOM = 100;
const uint16_t TC_OBJ_SECTION_BASE = 200;
const uint16_t TC_OBJ_DPD_SETPOINT_WORK_STATE = 1000;
const uint16_t TC_OBJ_DPD_ACTUAL_WORK_STATE = 1001;
const uint16_t TC_OBJ_DPD_SETPOINT_AREA_RATE = 1002;
const uint16_t TC_OBJ_DPD_ACTUAL_AREA_RATE = 1003;
const uint16_t TC_OBJ_DPD_ACTUAL_SPEED = 1004;
const uint16_t TC_OBJ_DVP_WIDTH = 3000;
const uint16_t TC_OBJ_DPT_WIDTH_BASE = 3100;

const uint8_t TCmaxSections = 16;
const int TCsettingsAddr = 1024;		// after the sensors (253 + 6 x 124 = 997)
const uint16_t TCsettingsID = 0x5444;	// change when TCsettings changes shape (0x5443: no units)

struct TCsettings
{
	uint16_t Identifier = TCsettingsID;
	uint8_t SectionCount = 8;
	uint16_t SectionWidthCm[TCmaxSections] = { 50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50 };
	uint16_t RateDDI = 1;			// setpoint rate DDI: 1 volume, 6 mass, 11 count per area (from RC)
	float UnitsPerISO = 0.0f;		// metered units per litre, kg or seed (from RC); 0 = not set
};

TCsettings TCset;
float TC_TargetRate = 0.0f;			// from the TC, litres, kg or seeds per ha; RAM only
bool TC_UnitsWarned = false;		// "units not set" printed once

extern uint8_t NewLo;	// Relays.ino: outputs actually driven (that file comes later in the build)
extern uint8_t NewHi;

std::shared_ptr<isobus::TaskControllerClient> ISOBUStc;
bool TC_InCommand = false;			// command authority: true = TC, false = RC
bool TC_DDOPNeedsReupload = false;
uint32_t TC_LastReport = 0;
uint32_t TC_ProcessDataReadyAt = 0;
bool TC_WasConnected = false;
uint8_t TC_LastState = 255;
uint8_t TC_LastReportedActualLo = 0xFF;
uint8_t TC_LastReportedActualHi = 0xFF;
uint32_t TC_LastActualSectionReport = 0;

void TC_LoadSettings()
{
	TCsettings Stored;
	EEPROM.get(TCsettingsAddr, Stored);
	if (Stored.Identifier == TCsettingsID && Stored.SectionCount >= 1 && Stored.SectionCount <= TCmaxSections)
	{
		TCset = Stored;
	}
	else
	{
		TCset = TCsettings();	// defaults, not written until settings are saved
	}
	if (!TC_IsRateDDI(TCset.RateDDI)) TCset.RateDDI = 1;
	if (!(TCset.UnitsPerISO > 0.0f && TCset.UnitsPerISO < 1.0e7f)) TCset.UnitsPerISO = 0.0f;	// also catches NaN
}

// the setpoint rate DDIs a product can use: 1 volume, 6 mass, 11 count per area
bool TC_IsRateDDI(uint16_t DDI)
{
	return DDI == 1 || DDI == 6 || DDI == 11;
}

// the matching actual rate DDI: 2, 7 or 12
uint16_t TC_ActualRateDDI()
{
	return TCset.RateDDI + 1;
}

// the rate DDI's raw value <-> litres, kg or seeds per ha
// DDI 1: 0.01 mm3/m2 per bit (1 L/ha = 10000), DDI 6: 1 mg/m2 (1 kg/ha = 100), DDI 11: 0.001 /m2 (1 seed/ha = 0.1)
float TC_ValueToPerHa(std::int32_t value)
{
	float Result = 0.0f;
	if (value > 0)
	{
		if (TCset.RateDDI == 6) Result = value * 0.01f;
		else if (TCset.RateDDI == 11) Result = value * 10.0f;
		else Result = value * 0.0001f;
	}
	return Result;
}

std::int32_t TC_PerHaToValue(float perHa)
{
	float Raw;
	if (TCset.RateDDI == 6) Raw = perHa * 100.0f;
	else if (TCset.RateDDI == 11) Raw = perHa * 0.1f;
	else Raw = perHa * 10000.0f;
	if (Raw < 0.0f) Raw = 0.0f;
	if (Raw > 2.0e9f) Raw = 2.0e9f;
	return static_cast<std::int32_t>(Raw + 0.5f);
}

const __FlashStringHelper* TC_RateUnit()
{
	if (TCset.RateDDI == 6) return F(" kg/ha");
	if (TCset.RateDDI == 11) return F(" seeds/ha");
	return F(" L/ha");
}

// PGN 32509 / CAN 0xFF1D from RC: the product's type and metered units per litre, kg or seed.
// Only sensor 0 is driven by the TC. DDI 0 = the unit has no type: the TC's rate is not applied.
void TC_SetUnits(uint8_t SensorID, uint16_t DDI, float UnitsPerISO)
{
	if (SensorID == 0)
	{
		float Factor = (TC_IsRateDDI(DDI) && UnitsPerISO > 0.0f && UnitsPerISO < 1.0e7f) ? UnitsPerISO : 0.0f;
		bool NewType = TC_IsRateDDI(DDI) && DDI != TCset.RateDDI;
		if (NewType || Factor != TCset.UnitsPerISO)
		{
			if (NewType) TCset.RateDDI = DDI;
			TCset.UnitsPerISO = Factor;
			TC_TargetRate = 0.0f;		// a rate in the old type's unit no longer applies
			TC_UnitsWarned = false;

			Serial.print(F("TC: units DDI "));
			Serial.print(TCset.RateDDI);
			Serial.print(F(", "));
			Serial.print(Factor, 6);
			Serial.println(F(" per litre, kg or seed"));

			if (NewType)
			{
				TC_GeometryChanged();	// saves, and uploads the DDOP with the new rate DDIs
			}
			else
			{
				TC_SaveSettings();
			}
		}
	}
}

void TC_SaveSettings()
{
	TCset.Identifier = TCsettingsID;
	EEPROM.put(TCsettingsAddr, TCset);
}

// DDOP active for 3 s before the module reports process data (gives the TC time after the
// upload); value commands from the TC don't wait for this
bool TC_ProcessDataReady()
{
	return ISOBUStc->get_is_connected() &&
		(TC_ProcessDataReadyAt != 0) &&
		(static_cast<int32_t>(millis() - TC_ProcessDataReadyAt) >= 0);
}

void TC_UpdateConnectionState()
{
	uint8_t state = static_cast<uint8_t>(ISOBUStc->get_state());
	if (state != TC_LastState)
	{
		TC_LastState = state;
		Serial.print(F("TC state: "));
		Serial.println(state);
	}

	bool connected = ISOBUStc->get_is_connected();
	if (connected && !TC_WasConnected)
	{
		TC_ProcessDataReadyAt = millis() + 3000;
		TC_LastReport = millis();
		Serial.println(F("TC: DDOP active, waiting before process data."));
	}
	else if (!connected && TC_WasConnected)
	{
		TC_ProcessDataReadyAt = 0;
		Serial.println(F("TC: disconnected."));
	}
	TC_WasConnected = connected;
}

// true while the TC has command authority: RC's rate and relay commands are ignored
// (CANBus.ino 0xFF03/0xFF04, Receive.ino 32500/32501)
bool TC_Commanding()
{
	return TC_InCommand;
}

// every target, master and the sections cleared: no command from the old source survives
void TC_ClearCommands()
{
	RelayLo = 0;
	RelayHi = 0;
	MasterOn = false;
	TC_TargetRate = 0.0f;
	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].TargetUPM = 0.0f;
		Sensor[i].ManualAdjust = 0;
		AutoOn[i] = true;		// auto at target 0 applies nothing
		CalibrationOn[i] = false;
		FlowExpected[i] = false;	// set by RC's 32500/0xFF03, which the TC state ignores; TC_Update sets product 1's
	}
}

// the TC's first value command: authority RC -> TC
void TC_EnterCommand()
{
	if (!TC_InCommand)
	{
		TC_ClearCommands();
		TC_InCommand = true;
		Serial.println(F("TC: commanding, RC rate and relay commands ignored."));
	}
}

// TC lost: authority TC -> RC, in RC's comm-loss state until RC's next command
void TC_LeaveCommand()
{
	if (TC_InCommand)
	{
		TC_ClearCommands();
		for (uint8_t i = 0; i < MDL.SensorCount; i++)
		{
			Sensor[i].CommTime = millis() - 5000;	// older than the 4 s comm-loss limit
		}
		TC_InCommand = false;
		Serial.println(F("TC: stopped commanding, back to RC."));
	}
}

// keeps product 1 "connected" (RC's 4 s comm-loss rule) while the TC commands
void TC_RefreshCommandTime()
{
	if (MDL.SensorCount > 0) Sensor[0].CommTime = millis();
}

float TC_ActiveWidthM()
{
	float Result = 0.0f;
	for (uint8_t i = 0; i < TCset.SectionCount; i++)
	{
		bool on = (i < 8) ? bitRead(RelayLo, i) : bitRead(RelayHi, i - 8);
		if (on) Result += TCset.SectionWidthCm[i] * 0.01f;
	}
	return Result;
}

// metered units per minute: the TC's rate (litres, kg or seeds per ha) x metered units per ISO unit
// x ha per minute; 0 until RC has sent the units
float TC_TargetUPM()
{
	float Result = 0.0f;
	float speedKmh = SPEED_GetKmh();
	float widthM = TC_ActiveWidthM();
	if (MasterOn && TC_TargetRate > 0.0f && TCset.UnitsPerISO > 0.0f && speedKmh > 0.05f && widthM > 0.01f)
	{
		Result = (TC_TargetRate * TCset.UnitsPerISO * speedKmh * widthM) / 600.0f;
	}
	return Result;
}

// litres, kg or seeds per ha actually applied, the inverse of TC_TargetUPM
float TC_ActualRate()
{
	float Result = 0.0f;
	float speedKmh = SPEED_GetKmh();
	float widthM = TC_ActiveWidthM();
	if (MDL.SensorCount > 0 && TCset.UnitsPerISO > 0.0f && speedKmh > 0.05f && widthM > 0.01f)
	{
		Result = (Sensor[0].UPM * 600.0f) / (speedKmh * widthM * TCset.UnitsPerISO);
	}
	return Result;
}

uint32_t TC_EncodeWorkState(uint8_t lo, uint8_t hi)
{
	uint32_t value = 0;
	for (uint8_t i = 0; i < TCset.SectionCount; i++)
	{
		bool on = (i < 8) ? bitRead(lo, i) : bitRead(hi, i - 8);
		if (on) value |= (0x01UL << (i * 2));
	}
	return value;
}

// NewLo/NewHi (Relays.ino) are the outputs actually driven
void TC_ReportActualSectionState(bool force)
{
	if (TC_ProcessDataReady())
	{
		const bool changed = (NewLo != TC_LastReportedActualLo) || (NewHi != TC_LastReportedActualHi);
		const uint32_t since = millis() - TC_LastActualSectionReport;
		if (force || (since >= 250 && (changed || since >= 2000)))
		{
			TC_LastReportedActualLo = NewLo;
			TC_LastReportedActualHi = NewHi;
			TC_LastActualSectionReport = millis();
			ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16));
		}
	}
}

void TC_ApplyCondensedWorkState(int32_t value)
{
	TC_EnterCommand();
	byte Lo = 0;
	byte Hi = 0;
	for (uint8_t i = 0; i < TCset.SectionCount; i++)
	{
		if (((value >> (i * 2)) & 0x03) == 0x01)
		{
			if (i < 8) bitSet(Lo, i); else bitSet(Hi, i - 8);
		}
	}
	RelayLo = Lo;
	RelayHi = Hi;
	MasterOn = (RelayLo > 0 || RelayHi > 0);
	TC_RefreshCommandTime();
	TC_ReportActualSectionState(true);
}

bool TC_RequestValue(std::uint16_t elementNumber, std::uint16_t DDI, std::int32_t& value, void*)
{
	bool Result = false;
	if (TC_ProcessDataReady() && elementNumber == TC_ELEMENT_BOOM)
	{
		Result = true;
		if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16))
		{
			value = static_cast<std::int32_t>(TC_EncodeWorkState(NewLo, NewHi));
		}
		else if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16))
		{
			value = static_cast<std::int32_t>(TC_EncodeWorkState(RelayLo, RelayHi));
		}
		else if (DDI == TCset.RateDDI)
		{
			value = TC_PerHaToValue(TC_TargetRate);
		}
		else if (DDI == TC_ActualRateDDI())
		{
			value = TC_PerHaToValue(TC_ActualRate());
		}
		else if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed))
		{
			value = static_cast<std::int32_t>(SPEED_GetKmh() * 277.7778f);	// mm/s
		}
		else
		{
			Result = false;
		}
	}
	return Result;
}

bool TC_ValueCommand(std::uint16_t elementNumber, std::uint16_t DDI, std::int32_t value, void*)
{
	bool Result = false;
	if (ISOBUStc->get_is_connected() && elementNumber == TC_ELEMENT_BOOM)
	{
		if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16))
		{
			TC_ApplyCondensedWorkState(value);
			Result = true;
		}
		else if (DDI == TCset.RateDDI)
		{
			TC_EnterCommand();
			float NewRate = TC_ValueToPerHa(value);
			if (NewRate != TC_TargetRate)
			{
				Serial.print(F("TC: rate "));
				Serial.print(NewRate, 2);
				Serial.println(TC_RateUnit());
			}
			if (TCset.UnitsPerISO <= 0.0f && NewRate > 0.0f && !TC_UnitsWarned)
			{
				Serial.println(F("TC: rate ignored, units not set in RC"));	// TC_TargetUPM stays 0
				TC_UnitsWarned = true;
			}
			TC_TargetRate = NewRate;
			TC_RefreshCommandTime();
			Result = true;
		}
	}
	return Result;
}

std::shared_ptr<isobus::DeviceDescriptorObjectPool> TC_CreateDDOP()
{
	using DeviceElementType = isobus::task_controller_object::DeviceElementObject::Type;
	using PropertiesBit = isobus::task_controller_object::DeviceProcessDataObject::PropertiesBit;
	using Trigger = isobus::task_controller_object::DeviceProcessDataObject::AvailableTriggerMethods;

	auto ddop = std::make_shared<isobus::DeviceDescriptorObjectPool>(4);
	std::array<std::uint8_t, isobus::task_controller_object::DeviceObject::MAX_STRUCTURE_AND_LOCALIZATION_LABEL_LENGTH> localization = { 'e','n',0,0,0,0,0xFF };
	// change the structure label whenever the DDOP's structure changes, so a TC reloads it;
	// the last character is the rate type (V volume, M mass, C count)
	char TypeChar = (TCset.RateDDI == 6) ? 'M' : ((TCset.RateDDI == 11) ? 'C' : 'V');
	char structureLabel[8] = { 'R','C','T','C','0','6', TypeChar, 0 };
	std::vector<std::uint8_t> extendedLabel = { 'R','C','T','C','0','6', static_cast<std::uint8_t>(TypeChar) };

	ddop->add_device(InoDescription, String(InoID).c_str(), String(MDL.ID).c_str(), structureLabel, localization, extendedLabel, ISOBUSicf->get_NAME().get_full_name());
	ddop->add_device_element("Boom", TC_ELEMENT_BOOM, 0, DeviceElementType::Function, TC_OBJ_BOOM);

	const uint8_t settable = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet) | static_cast<std::uint8_t>(PropertiesBit::Settable);
	const uint8_t measurement = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet);
	const uint8_t onChange = static_cast<std::uint8_t>(Trigger::OnChange);

	ddop->add_device_process_data("Set Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16), isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_WORK_STATE);
	ddop->add_device_process_data("Actual Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_WORK_STATE);
	// rate DDIs follow the product's type from RC: 1/2 volume, 6/7 mass, 11/12 count per area
	ddop->add_device_process_data("Target Rate", TCset.RateDDI, isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_AREA_RATE);
	ddop->add_device_process_data("Actual Rate", TC_ActualRateDDI(), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_AREA_RATE);
	ddop->add_device_process_data("Actual Speed", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_SPEED);

	auto boom = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(TC_OBJ_BOOM));
	if (boom != nullptr)
	{
		boom->add_reference_to_child_object(TC_OBJ_DPD_SETPOINT_WORK_STATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_WORK_STATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_SETPOINT_AREA_RATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_AREA_RATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_SPEED);
	}

	ddop->add_device_value_presentation("m", 0, 0.001f, 2, TC_OBJ_DVP_WIDTH);

	for (uint8_t i = 0; i < TCset.SectionCount; i++)
	{
		uint16_t sectionObjectID = TC_OBJ_SECTION_BASE + i;
		uint16_t widthPropertyID = TC_OBJ_DPT_WIDTH_BASE + i;
		char name[12];
		snprintf(name, sizeof(name), "Sec %02u", i + 1);

		// element numbers are unique in the DDOP: the device is 0, the boom 1, sections from 2
		ddop->add_device_element(name, TC_ELEMENT_BOOM + 1 + i, TC_OBJ_BOOM, DeviceElementType::Section, sectionObjectID);
		ddop->add_device_property("Width", static_cast<std::int32_t>(TCset.SectionWidthCm[i]) * 10, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::DefaultWorkingWidth), TC_OBJ_DVP_WIDTH, widthPropertyID);

		auto section = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(sectionObjectID));
		if (section != nullptr)
		{
			section->add_reference_to_child_object(widthPropertyID);
		}
	}

	return ddop;
}

FLASHMEM void TC_Begin()
{
	TC_LoadSettings();

	const isobus::NAMEFilter filterTC(isobus::NAME::NAMEParameters::FunctionCode, static_cast<std::uint8_t>(isobus::NAME::Function::TaskController));
	const std::vector<isobus::NAMEFilter> tcNameFilters = { filterTC };
	auto partnerTC = isobus::CANNetworkManager::CANNetwork.create_partnered_control_function(0, tcNameFilters);

	ISOBUStc = std::make_shared<isobus::TaskControllerClient>(partnerTC, ISOBUSicf, nullptr);	// no VT client yet
	ISOBUStc->configure(TC_CreateDDOP(), 1, TCset.SectionCount, 0, true, false, false, false, true);
	ISOBUStc->add_request_value_callback(TC_RequestValue, nullptr);
	ISOBUStc->add_value_command_callback(TC_ValueCommand, nullptr);
	ISOBUStc->initialize(false);
}

void TC_Update()
{
	TC_UpdateConnectionState();
	ISOBUStc->update();
	TC_UpdateConnectionState();

	if (!ISOBUStc->get_is_connected())
	{
		TC_LeaveCommand();		// does nothing unless the TC was commanding
	}
	else if (TC_InCommand)
	{
		// the TC's rate becomes product 1's target as speed and active width change
		TC_RefreshCommandTime();
		if (MDL.SensorCount > 0)
		{
			Sensor[0].TargetUPM = TC_TargetUPM();
			FlowExpected[0] = (RelayLo || RelayHi);	// RC's flow-path rule: a section is on
		}
	}

	if (TC_DDOPNeedsReupload && ISOBUStc->get_is_connected())
	{
		TC_DDOPNeedsReupload = false;
		ISOBUStc->reupload_device_descriptor_object_pool(TC_CreateDDOP());
	}

	if (TC_ProcessDataReady() && (millis() - TC_LastReport >= 1000))
	{
		TC_LastReport = millis();
		TC_ReportActualSectionState(false);
		ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BOOM, TC_ActualRateDDI());
		ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed));
	}
}

// after section count or widths change (later: from RC or the VT)
void TC_GeometryChanged()
{
	TC_SaveSettings();
	TC_DDOPNeedsReupload = true;
}

#endif
