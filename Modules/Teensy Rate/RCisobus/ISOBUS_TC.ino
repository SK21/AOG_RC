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

uint32_t TC_LastReport = 0;
uint32_t TC_ProcessDataReadyAt = 0;
uint32_t TC_LastSectionSetpoint = 0;
bool TC_WasConnected = false;
bool TC_WasProcessDataReady = false;
bool TC_HasSectionSetpoint = false;
uint8_t TC_LastState = 255;
uint8_t TC_LastReportedActualLo = 0xFF;
uint8_t TC_LastReportedActualHi = 0xFF;
uint32_t TC_LastActualSectionReport = 0;

bool TC_ProcessDataReady()
{
	return (ISOBUSTaskController != nullptr) &&
	       ISOBUSTaskController->get_is_connected() &&
	       (TC_ProcessDataReadyAt != 0) &&
	       (static_cast<int32_t>(millis() - TC_ProcessDataReadyAt) >= 0);
}

void TC_UpdateConnectionState()
{
	if (ISOBUSTaskController == nullptr) return;

	uint8_t state = static_cast<uint8_t>(ISOBUSTaskController->get_state());
	if (state != TC_LastState)
	{
		TC_LastState = state;
		Serial.print(F("TC state: "));
		Serial.println(state);
	}

	bool connected = ISOBUSTaskController->get_is_connected();
	if (connected && !TC_WasConnected)
	{
		TC_ProcessDataReadyAt = millis() + 3000;
		TC_LastReport = millis();
		Serial.println(F("TC: DDOP active, waiting before process data."));
	}
	else if (!connected && TC_WasConnected)
	{
		TC_ProcessDataReadyAt = 0;
		TC_WasProcessDataReady = false;
		Serial.println(F("TC: disconnected."));
	}
	TC_WasConnected = connected;

	bool processDataReady = TC_ProcessDataReady();
	if (processDataReady && !TC_WasProcessDataReady)
	{
		Serial.println(F("TC: process data enabled."));
	}
	else if (!processDataReady && TC_WasProcessDataReady)
	{
		Serial.println(F("TC: process data disabled."));
	}
	TC_WasProcessDataReady = processDataReady;
}

void TC_RefreshMachineCommandTime()
{
	TC_LastSectionCommand = millis();
	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].CommTime = millis();
	}
}

bool TC_MachineAutoMode()
{
	if (MDL.SensorCount == 0) return true;
	return Machine.AutoRate[0];
}

void TC_ReportActualSectionState(bool force = false)
{
	if (!TC_ProcessDataReady()) return;

	const uint8_t actualLo = RelayEffectiveLo();
	const uint8_t actualHi = RelayEffectiveHi();
	const bool changed = (actualLo != TC_LastReportedActualLo) || (actualHi != TC_LastReportedActualHi);
	if (!force && !changed && (millis() - TC_LastActualSectionReport < 2000)) return;
	if (!force && (millis() - TC_LastActualSectionReport < 250)) return;

	TC_LastReportedActualLo = actualLo;
	TC_LastReportedActualHi = actualHi;
	TC_LastActualSectionReport = millis();
	ISOBUSTaskController->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16));
}

uint32_t TC_EncodeRelayWorkState(uint8_t lo, uint8_t hi)
{
	uint32_t value = 0;
	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		bool on = (i < 8) ? bitRead(lo, i) : bitRead(hi, i - 8);
		if (on) value |= (0x01UL << (i * 2));
	}
	return value;
}

void TC_SetAllConfiguredSections(bool enabled)
{
	byte newLo = 0;
	byte newHi = 0;

	if (enabled)
	{
		for (uint8_t i = 0; i < Machine.SectionCount; i++)
		{
			if (i < 8) bitSet(newLo, i); else bitSet(newHi, i - 8);
		}
	}

	RelayLo = newLo;
	RelayHi = newHi;
	MasterOn = enabled && ((RelayLo > 0) || (RelayHi > 0));
	TC_RefreshMachineCommandTime();
	TC_ReportActualSectionState(true);

	Serial.print(F("VT sections: master="));
	Serial.print(MasterOn ? F("ON") : F("OFF"));
	Serial.print(F(" relayLo=0x"));
	Serial.print(RelayLo, HEX);
	Serial.print(F(" relayHi=0x"));
	Serial.println(RelayHi, HEX);
}

uint32_t TC_EncodeActualWorkState()
{
	return TC_EncodeRelayWorkState(RelayEffectiveLo(), RelayEffectiveHi());
}

uint32_t TC_EncodeSetpointWorkState()
{
	return TC_EncodeRelayWorkState(RelayLo, RelayHi);
}

void TC_ApplyCondensedWorkState(int32_t processVariableValue)
{
	if (!TC_MachineAutoMode())
	{
		static uint32_t lastManualIgnoreLog = 0;
		if (millis() - lastManualIgnoreLog >= 2000)
		{
			lastManualIgnoreLog = millis();
			Serial.println(F("TC section setpoint ignored in MANUAL mode."));
		}
		TC_ReportActualSectionState();
		return;
	}

	byte newLo = 0;
	byte newHi = 0;

	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		uint8_t state = (processVariableValue >> (i * 2)) & 0x03;
		if (state == 0x01)
		{
			if (i < 8) bitSet(newLo, i); else bitSet(newHi, i - 8);
		}
	}

	RelayLo = newLo;
	RelayHi = newHi;
	MasterOn = (RelayLo > 0 || RelayHi > 0);
	TC_HasSectionSetpoint = true;
	TC_LastSectionSetpoint = millis();
	TC_RefreshMachineCommandTime();

	Serial.print(F("TC section setpoint: value=0x"));
	Serial.print(static_cast<uint32_t>(processVariableValue), HEX);
	Serial.print(F(" relayLo=0x"));
	Serial.print(RelayLo, HEX);
	Serial.print(F(" relayHi=0x"));
	Serial.println(RelayHi, HEX);

	TC_ReportActualSectionState(true);
}

bool TC_RequestValue(std::uint16_t elementNumber, std::uint16_t DDI, std::int32_t &processVariableValue, void *)
{
	if (!TC_ProcessDataReady()) return false;
	if (elementNumber != TC_ELEMENT_BOOM) return false;

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16) ||
	    DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16))
	{
		processVariableValue = (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16)) ?
		                       static_cast<std::int32_t>(TC_EncodeActualWorkState()) :
		                       static_cast<std::int32_t>(TC_EncodeSetpointWorkState());
		return true;
	}

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointVolumePerAreaApplicationRate))
	{
		processVariableValue = RateControl_LHaToTCValue(Machine.TargetRateLHa[0]);
		return true;
	}

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualVolumePerAreaApplicationRate))
	{
		processVariableValue = RateControl_LHaToTCValue(RateControl_ActualRateLHa(0));
		return true;
	}

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed))
	{
		processVariableValue = static_cast<std::int32_t>(SPEED_GetKmh() * 277.7778f);
		return true;
	}

	return false;
}

bool TC_ValueCommand(std::uint16_t elementNumber, std::uint16_t DDI, std::int32_t processVariableValue, void *)
{
	if (!TC_ProcessDataReady()) return false;
	if (elementNumber != TC_ELEMENT_BOOM) return false;

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16))
	{
		TC_ApplyCondensedWorkState(processVariableValue);
		return true;
	}

	if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointVolumePerAreaApplicationRate))
	{
		if (!TC_MachineAutoMode())
		{
			static uint32_t lastManualRateIgnoreLog = 0;
			if (millis() - lastManualRateIgnoreLog >= 2000)
			{
				lastManualRateIgnoreLog = millis();
				Serial.println(F("TC target rate ignored in MANUAL mode."));
			}
			return true;
		}

		Machine.TargetRateLHa[0] = RateControl_TCValueToLHa(processVariableValue);
		Machine.AutoRate[0] = true;
		VT_SaveMachineSettings(false);
		Serial.print(F("TC target rate: "));
		Serial.print(Machine.TargetRateLHa[0]);
		Serial.println(F(" L/ha"));
		return true;
	}

	return false;
}

std::shared_ptr<isobus::DeviceDescriptorObjectPool> TC_CreateDDOP()
{
	using DeviceElementType = isobus::task_controller_object::DeviceElementObject::Type;
	using PropertiesBit = isobus::task_controller_object::DeviceProcessDataObject::PropertiesBit;
	using Trigger = isobus::task_controller_object::DeviceProcessDataObject::AvailableTriggerMethods;

	auto ddop = std::make_shared<isobus::DeviceDescriptorObjectPool>(4);
	std::array<std::uint8_t, isobus::task_controller_object::DeviceObject::MAX_STRUCTURE_AND_LOCALIZATION_LABEL_LENGTH> localization = { 'e','n',0,0,0,0,0xFF };
	std::vector<std::uint8_t> extendedLabel = { 'R','C','T','C','0','4' };
	uint64_t clientName = buildIsobusNAME();
	if (ISOBUSControlFunction != nullptr)
	{
		clientName = ISOBUSControlFunction->get_NAME().get_full_name();
	}

	ddop->add_device("RCteensy", String(InoID).c_str(), String(MDL.ID).c_str(), "RCTC004", localization, extendedLabel, clientName);
	ddop->add_device_element("Boom", TC_ELEMENT_BOOM, 0, DeviceElementType::Function, TC_OBJ_BOOM);

	const uint8_t settable = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet) | static_cast<std::uint8_t>(PropertiesBit::Settable);
	const uint8_t measurement = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet);
	const uint8_t onChange = static_cast<std::uint8_t>(Trigger::OnChange);

	ddop->add_device_process_data("Set Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16), isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_WORK_STATE);
	ddop->add_device_process_data("Actual Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_WORK_STATE);
	ddop->add_device_process_data("Target LHa", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointVolumePerAreaApplicationRate), isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_AREA_RATE);
	ddop->add_device_process_data("Actual LHa", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualVolumePerAreaApplicationRate), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_AREA_RATE);
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

	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		uint16_t sectionObjectID = TC_OBJ_SECTION_BASE + i;
		uint16_t widthPropertyID = TC_OBJ_DPT_WIDTH_BASE + i;
		char name[12];
		snprintf(name, sizeof(name), "Sec %02u", i + 1);

		ddop->add_device_element(name, i + 1, TC_OBJ_BOOM, DeviceElementType::Section, sectionObjectID);
		ddop->add_device_property("Width", static_cast<std::int32_t>(Machine.SectionWidthCm[i]) * 10, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::DefaultWorkingWidth), TC_OBJ_DVP_WIDTH, widthPropertyID);

		auto section = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(sectionObjectID));
		if (section != nullptr)
		{
			section->add_reference_to_child_object(widthPropertyID);
		}
	}

	return ddop;
}

void TC_ConfigureClient()
{
	if (ISOBUSTaskController == nullptr) return;

	auto ddop = TC_CreateDDOP();
	ISOBUSTaskController->configure(ddop, 1, Machine.SectionCount, 0, true, false, false, false, true);
}

FLASHMEM void TC_Begin()
{
	if (ISOBUSControlFunction == nullptr) return;

	const isobus::NAMEFilter filterTaskController(isobus::NAME::NAMEParameters::FunctionCode, static_cast<std::uint8_t>(isobus::NAME::Function::TaskController));
	const std::vector<isobus::NAMEFilter> tcNameFilters = { filterTaskController };
	auto partnerTC = isobus::CANNetworkManager::CANNetwork.create_partnered_control_function(0, tcNameFilters);
	std::shared_ptr<isobus::PartneredControlFunction> primaryVT = nullptr;
	if (ISOBUSVirtualTerminal != nullptr)
	{
		primaryVT = ISOBUSVirtualTerminal->get_partner_control_function();
	}

	ISOBUSTaskController = std::make_shared<isobus::TaskControllerClient>(partnerTC, ISOBUSControlFunction, primaryVT);
	Serial.println(primaryVT != nullptr ? F("TC: primary VT partner configured.") : F("TC: no primary VT partner."));
	TC_ConfigureClient();
	ISOBUSTaskController->add_request_value_callback(TC_RequestValue, nullptr);
	ISOBUSTaskController->add_value_command_callback(TC_ValueCommand, nullptr);
	ISOBUSTaskController->initialize(false);
}

FLASHMEM void TC_Update()
{
	if (ISOBUSTaskController == nullptr) return;

	TC_UpdateConnectionState();
	ISOBUSTaskController->update();
	TC_UpdateConnectionState();
	const bool autoMode = TC_MachineAutoMode();
	TC_SectionControlActive = TC_ProcessDataReady() && autoMode;

	if (TC_SectionControlActive)
	{
		TC_RefreshMachineCommandTime();
	}
	else if (autoMode)
	{
		RelayLo = 0;
		RelayHi = 0;
		MasterOn = false;
	}

	if (TC_DDOPNeedsReupload && ISOBUSTaskController->get_is_connected())
	{
		TC_DDOPNeedsReupload = false;
		ISOBUSTaskController->reupload_device_descriptor_object_pool(TC_CreateDDOP());
	}

	if (TC_ProcessDataReady() && (millis() - TC_LastReport >= 1000))
	{
		TC_LastReport = millis();
		TC_ReportActualSectionState(false);
		ISOBUSTaskController->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualVolumePerAreaApplicationRate));
		ISOBUSTaskController->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed));
	}
}

void TC_MachineSettingsChanged(bool ddopGeometryChanged)
{
	if (ddopGeometryChanged)
	{
		TC_DDOPNeedsReupload = true;
	}

	if (TC_ProcessDataReady())
	{
		ISOBUSTaskController->on_value_changed_trigger(TC_ELEMENT_BOOM, static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointVolumePerAreaApplicationRate));
	}
}
