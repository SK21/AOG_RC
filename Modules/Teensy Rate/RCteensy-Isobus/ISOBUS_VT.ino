uint8_t VT_SelectedField = 0;
bool VT_WorkingSetSelectPending = true;
uint16_t VT_LastActiveMask = UNDEFINED;

enum VTScreen : uint8_t
{
	VT_SCREEN_MAIN = 0,
	VT_SCREEN_SETTINGS,
	VT_SCREEN_SECTIONS,
	VT_SCREEN_RATE,
	VT_SCREEN_OUTPUTS,
	VT_SCREEN_TANK
};

VTScreen VT_CurrentScreen;
uint8_t VT_SettingsSelection = 0;
const uint8_t VT_SETTINGS_COUNT = 4;
float VT_TankFillAmountUnits = 0.0f;
const float VT_TANK_STEP_UNITS = 10.0f;
uint32_t VT_LastScreenButtonActionMs = 0;

enum VTTankView : uint8_t
{
	VT_TANK_VIEW_OVERVIEW = 0,
	VT_TANK_VIEW_ADD,
	VT_TANK_VIEW_CAPACITY
};

VTTankView VT_TankView;

const uint16_t VT_STATUS_INTERVAL_MS = 150;
const uint16_t VT_TANK_BUTTON_LEFT_X = 40;
const uint16_t VT_TANK_BUTTON_RIGHT_X = 255;
const uint16_t VT_TANK_BUTTON_ROW1_Y = 170;
const uint16_t VT_TANK_BUTTON_ROW2_Y = 260;
const uint16_t VT_TANK_BUTTON_ROW3_Y = 350;
const uint16_t VT_TANK_BUTTON_W = 185;
const uint16_t VT_TANK_BUTTON_H = 64;

FLASHMEM void VT_LogState()
{
	static uint32_t lastPrint = 0;
	static bool lastPartnerValid = false;
	static bool lastConnected = false;

	if (ISOBUSVirtualTerminal == nullptr) return;

	bool partnerValid = false;
	uint8_t partnerAddress = isobus::NULL_CAN_ADDRESS;
	auto partner = ISOBUSVirtualTerminal->get_partner_control_function();
	if (partner != nullptr)
	{
		partnerValid = partner->get_address_valid();
		if (partnerValid) partnerAddress = partner->get_address();
	}

	bool connected = ISOBUSVirtualTerminal->get_is_connected();
	if ((partnerValid != lastPartnerValid) ||
	    (connected != lastConnected) ||
	    (millis() - lastPrint >= 5000))
	{
		lastPrint = millis();
		lastPartnerValid = partnerValid;
		lastConnected = connected;

		Serial.print(F("VT: terminal "));
		Serial.print(partnerValid ? F("found") : F("waiting"));
		if (partnerValid)
		{
			Serial.print(F(" addr=0x"));
			Serial.print(partnerAddress, HEX);
		}
		Serial.print(F(" connected="));
		Serial.println(connected ? F("yes") : F("no"));
	}
}

uint8_t VT_ProductCount()
{
	uint8_t count = MDL.SensorCount;
	if (count < 1) count = 1;
	if (count > MaxProductCount) count = MaxProductCount;
	return count;
}

uint8_t VT_ProductFieldCount()
{
	return 5;
}

uint8_t VT_SectionCountField()
{
	return VT_ProductCount() * VT_ProductFieldCount();
}

uint8_t VT_SectionWidthBaseField()
{
	return VT_SectionCountField() + 1;
}

uint8_t VT_MasterField()
{
	return VT_SectionWidthBaseField() + Machine.SectionCount;
}

uint8_t VT_RelayPolarityField()
{
	return VT_MasterField() + 1;
}

uint8_t VT_OnboardRelayControlField()
{
	return VT_RelayPolarityField() + 1;
}

uint8_t VT_RemoteRelayControlField()
{
	return VT_OnboardRelayControlField() + 1;
}

uint8_t VT_RelayPinBaseField()
{
	return VT_RemoteRelayControlField() + 1;
}

uint8_t VT_RemoteRelayFirstIndex()
{
	return (MDL.OnboardRelayControl > 0) ? 8 : 0;
}

uint8_t VT_RemoteRelayPinCount()
{
	if (MDL.RemoteRelayControl != 4) return 0;
	return 16 - VT_RemoteRelayFirstIndex();
}

uint8_t VT_RelayPinFieldToRelayIndex()
{
	return VT_RemoteRelayFirstIndex() + (VT_SelectedField - VT_RelayPinBaseField());
}

uint8_t VT_RelayTestField()
{
	return VT_RelayPinBaseField() + VT_RemoteRelayPinCount();
}

uint8_t VT_FieldCount()
{
	return VT_RelayTestField() + 1;
}

const char *VT_ScreenTitle()
{
	switch (VT_CurrentScreen)
	{
	case VT_SCREEN_MAIN:
		return "RCteensy";
	case VT_SCREEN_SETTINGS:
		return "Settings";
	case VT_SCREEN_SECTIONS:
		return "Sections";
	case VT_SCREEN_RATE:
		return "Rate";
	case VT_SCREEN_OUTPUTS:
		return "Outputs";
	case VT_SCREEN_TANK:
		return "Tank";
	default:
		return "RCteensy";
	}
}

uint16_t VT_DataMaskForScreen()
{
	switch (VT_CurrentScreen)
	{
	case VT_SCREEN_SETTINGS:
		return settingsMenu_DataMask;
	case VT_SCREEN_SECTIONS:
		return sectionsConfig_DataMask;
	case VT_SCREEN_RATE:
		return rateConfig_DataMask;
	case VT_SCREEN_OUTPUTS:
		return outputsConfig_DataMask;
	case VT_SCREEN_TANK:
		return tankConfig_DataMask;
	case VT_SCREEN_MAIN:
	default:
		return mainRunscreen_DataMask;
	}
}

const char *VT_SettingsMenuName(uint8_t index)
{
	switch (index)
	{
	case 0:
		return "Section setup";
	case 1:
		return "Rate setup";
	case 2:
		return "Output setup";
	case 3:
		return "Tank setup";
	default:
		return "";
	}
}

bool VT_FieldInRange(uint8_t field, uint8_t first, uint8_t endExclusive)
{
	return (field >= first) && (field < endExclusive);
}

void VT_SelectFirstFieldForScreen(uint8_t screen)
{
	switch (screen)
	{
	case VT_SCREEN_SECTIONS:
		VT_SelectedField = VT_SectionCountField();
		break;

	case VT_SCREEN_RATE:
		VT_SelectedField = 0;
		break;

	case VT_SCREEN_OUTPUTS:
		VT_SelectedField = VT_RelayPolarityField();
		break;

	case VT_SCREEN_TANK:
		VT_SelectedField = 0;
		break;

	default:
		VT_SelectedField = 0;
		break;
	}
}

void VT_SelectNextFieldForScreen(uint8_t screen)
{
	uint8_t firstField = 0;
	uint8_t endField = 0;

	switch (screen)
	{
	case VT_SCREEN_SECTIONS:
		firstField = VT_SectionCountField();
		endField = VT_MasterField();
		break;

	case VT_SCREEN_RATE:
		firstField = 0;
		endField = VT_SectionCountField();
		break;

	case VT_SCREEN_OUTPUTS:
		firstField = VT_RelayPolarityField();
		endField = VT_FieldCount();
		break;

	case VT_SCREEN_TANK:
		firstField = 0;
		endField = 4;
		break;

	default:
		return;
	}

	if (endField <= firstField) return;

	if (!VT_FieldInRange(VT_SelectedField, firstField, endField))
	{
		VT_SelectedField = firstField;
	}
	else
	{
		VT_SelectedField++;
		if (VT_SelectedField >= endField) VT_SelectedField = firstField;
	}
}

bool VT_SelectedFieldChangesGeometry()
{
	return (VT_SelectedField == VT_SectionCountField()) ||
		       (VT_SelectedField >= VT_SectionWidthBaseField() && VT_SelectedField < VT_MasterField());
}

const char *VT_ControlTypeName(uint8_t controlType)
{
	switch (controlType)
	{
	case StandardValve_ct:
		return "Standard";
	case ComboClose_ct:
		return "ComboClose";
	case Motor_ct:
		return "Motor";
	default:
		return "Standard";
	}
}

const char *VT_ModeName(bool autoRate)
{
	return autoRate ? "AUTO" : "MAN";
}

const char *VT_UnitName()
{
	switch (Machine.UnitMode)
	{
	case 1:  return "kg";
	case 2:  return "gal";
	case 3:  return "lbs";
	default: return "l";
	}
}

bool VT_LabelHasContent(const char *text)
{
	while (*text != '\0')
	{
		if (*text != ' ') return true;
		text++;
	}
	return false;
}

void VT_SendSoftKeyBackgroundsIfNeeded(const char *settingsButton,
                                       const char *masterButton,
                                       const char *plusButton,
                                       const char *minusButton,
                                       const char *softKey04,
                                       const char *softKey05,
                                       const char *softKey06,
                                       const char *softKey07,
                                       const char *softKey08,
                                       const char *softKey09,
                                       const char *softKey10,
                                       const char *softKey11,
                                       bool force)
{
	if (ISOBUSVirtualTerminal == nullptr) return;

	const uint16_t objectIDs[] = {
		alarm_SoftKey,
		acknowledgeAlarm_SoftKey,
		Plus_Button,
		Minus_Button,
		SoftKey_04,
		SoftKey_05,
		SoftKey_06,
		SoftKey_07,
		SoftKey_08,
		SoftKey_09,
		SoftKey_10,
		SoftKey_11
	};
	const char *labels[] = {
		settingsButton,
		masterButton,
		plusButton,
		minusButton,
		softKey04,
		softKey05,
		softKey06,
		softKey07,
		softKey08,
		softKey09,
		softKey10,
		softKey11
	};
	static uint8_t lastColours[12] = { 0 };
	static bool lastColoursValid = false;

	for (uint8_t i = 0; i < 12; i++)
	{
		uint8_t colour = VT_LabelHasContent(labels[i]) ? 1 : 8;
		if ((i == 0) && (labels[i][0] == '@'))
		{
			colour = 0;
		}
		if (force || !lastColoursValid || (lastColours[i] != colour))
		{
			ISOBUSVirtualTerminal->send_change_background_colour(objectIDs[i], colour);
			lastColours[i] = colour;
		}
	}
	lastColoursValid = true;
}

float VT_ConfiguredWidthM()
{
	float widthM = 0.0f;
	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		widthM += Machine.SectionWidthCm[i] * 0.01f;
	}
	return widthM;
}

void VT_BuildSectionMap(char *buffer, size_t bufferSize)
{
	if (bufferSize < 3) return;

	const uint8_t displayLo = RelayEffectiveLo();
	const uint8_t displayHi = RelayEffectiveHi();
	size_t pos = 0;
	buffer[pos++] = '[';
	for (uint8_t i = 0; i < Machine.SectionCount && pos < (bufferSize - 2); i++)
	{
		const bool sectionOn = (i < 8) ? bitRead(displayLo, i) : bitRead(displayHi, i - 8);
		buffer[pos++] = sectionOn ? 'X' : '.';
	}
	buffer[pos++] = ']';
	buffer[pos] = '\0';
}

void VT_BuildBoomGraphic(char *boomBuffer, size_t boomSize, char *nozzleBuffer, size_t nozzleSize)
{
	if ((boomSize < 3) || (nozzleSize < 2)) return;

	const uint8_t displayLo = RelayEffectiveLo();
	const uint8_t displayHi = RelayEffectiveHi();
	size_t boomPos = 0;
	size_t nozzlePos = 0;
	boomBuffer[boomPos++] = '[';
	for (uint8_t i = 0; i < Machine.SectionCount && boomPos < (boomSize - 2) && nozzlePos < (nozzleSize - 1); i++)
	{
		const bool sectionOn = (i < 8) ? bitRead(displayLo, i) : bitRead(displayHi, i - 8);
		boomBuffer[boomPos++] = sectionOn ? '=' : '-';
		nozzleBuffer[nozzlePos++] = sectionOn ? '^' : '.';
	}
	boomBuffer[boomPos++] = ']';
	boomBuffer[boomPos] = '\0';
	nozzleBuffer[nozzlePos] = '\0';
}

uint8_t VT_ControlTypeStep(uint8_t current, int8_t direction)
{
	const uint8_t options[] = { StandardValve_ct, ComboClose_ct, Motor_ct };
	const uint8_t optionCount = sizeof(options) / sizeof(options[0]);
	uint8_t index = 0;

	for (uint8_t i = 0; i < optionCount; i++)
	{
		if (options[i] == current)
		{
			index = i;
			break;
		}
	}

	if (direction >= 0)
	{
		index = (index + 1) % optionCount;
	}
	else
	{
		index = (index == 0) ? (optionCount - 1) : (index - 1);
	}
	return options[index];
}

void VT_SaveMachineSettings(bool ddopGeometryChanged)
{
	ApplyMachineSettings();
	SaveMachineSettings();
	TC_MachineSettingsChanged(ddopGeometryChanged);
}

void VT_SetMachineAutoMode(bool autoMode)
{
	if (MDL.SensorCount == 0) return;

	AutoOn = autoMode;

	VT_SaveMachineSettings(false);

	Serial.print(F("VT mode: "));
	Serial.println(autoMode ? F("AUTO") : F("MANUAL"));
}

void VT_ToggleAutoManualMode()
{
	VT_SetMachineAutoMode(!TC_MachineAutoMode());
}

bool VT_SectionBit(byte lo, byte hi, uint8_t sectionIndex)
{
	return (sectionIndex < 8) ? bitRead(lo, sectionIndex) : bitRead(hi, sectionIndex - 8);
}

void VT_SetSectionBit(byte &lo, byte &hi, uint8_t sectionIndex, bool enabled)
{
	if (sectionIndex < 8)
	{
		if (enabled) bitSet(lo, sectionIndex); else bitClear(lo, sectionIndex);
	}
	else
	{
		if (enabled) bitSet(hi, sectionIndex - 8); else bitClear(hi, sectionIndex - 8);
	}
}

bool VT_FindFirstActiveSection(byte lo, byte hi, uint8_t &sectionIndex)
{
	for (uint8_t i = 0; i < Machine.SectionCount; i++)
	{
		if (VT_SectionBit(lo, hi, i))
		{
			sectionIndex = i;
			return true;
		}
	}
	return false;
}

bool VT_FindLastActiveSection(byte lo, byte hi, uint8_t &sectionIndex)
{
	for (uint8_t i = Machine.SectionCount; i > 0; i--)
	{
		const uint8_t candidate = i - 1;
		if (VT_SectionBit(lo, hi, candidate))
		{
			sectionIndex = candidate;
			return true;
		}
	}
	return false;
}

void VT_ApplyManualSectionMask(byte lo, byte hi)
{
	RelayLo = lo;
	RelayHi = hi;
	MasterOn = (RelayLo > 0) || (RelayHi > 0);
	TC_RefreshMachineCommandTime();
	TC_ReportActualSectionState(true);

	Serial.print(F("VT manual sections: relayLo=0x"));
	Serial.print(RelayLo, HEX);
	Serial.print(F(" relayHi=0x"));
	Serial.print(RelayHi, HEX);
	Serial.print(F(" master="));
	Serial.println(MasterOn ? F("ON") : F("OFF"));
}

void VT_AdjustManualSections(uint8_t action)
{
	if (TC_MachineAutoMode()) return;
	if (Machine.SectionCount == 0) return;

	byte newLo = RelayLo;
	byte newHi = RelayHi;
	uint8_t sectionIndex = 0;

	switch (action)
	{
	case 0:
		if (VT_FindFirstActiveSection(newLo, newHi, sectionIndex))
		{
			VT_SetSectionBit(newLo, newHi, sectionIndex, false);
		}
		break;

	case 1:
		if (VT_FindLastActiveSection(newLo, newHi, sectionIndex))
		{
			VT_SetSectionBit(newLo, newHi, sectionIndex, false);
		}
		break;

	case 2:
		if (VT_FindFirstActiveSection(newLo, newHi, sectionIndex))
		{
			if (sectionIndex > 0) VT_SetSectionBit(newLo, newHi, sectionIndex - 1, true);
		}
		else
		{
			VT_SetSectionBit(newLo, newHi, Machine.SectionCount - 1, true);
		}
		break;

	case 3:
		if (VT_FindLastActiveSection(newLo, newHi, sectionIndex))
		{
			if (sectionIndex < (Machine.SectionCount - 1)) VT_SetSectionBit(newLo, newHi, sectionIndex + 1, true);
		}
		else
		{
			VT_SetSectionBit(newLo, newHi, 0, true);
		}
		break;

	default:
		return;
	}

	if ((newLo != RelayLo) || (newHi != RelayHi))
	{
		VT_ApplyManualSectionMask(newLo, newHi);
	}
}

void VT_AdjustMainDose(int8_t direction)
{
	const uint8_t productIndex = 0;
	if (MDL.SensorCount <= productIndex) return;

	if (AutoOn)
	{
		Machine.TargetRateLHa[productIndex] += direction * 5.0f;
		if (Machine.TargetRateLHa[productIndex] < 0.0f) Machine.TargetRateLHa[productIndex] = 0.0f;
	}
	else
	{
		Machine.ManualPWM[productIndex] = constrain(static_cast<int>(Machine.ManualPWM[productIndex]) + direction * 5, -4095, 4095);
	}

	VT_SaveMachineSettings(false);
}

void VT_ToggleMasterFromVT()
{
	TC_SetAllConfiguredSections(!MasterOn);
}

void VT_AdjustTankField(int8_t direction)
{
	switch (VT_SelectedField)
	{
	case 0:
		VT_AdjustTankCapacity(direction);
		break;

	case 1:
		VT_AdjustTankRemaining(direction);
		break;

	case 2:
		VT_AdjustTankFillAmount(direction);
		break;

	case 3:
		if (direction >= 0)
		{
			VT_ApplyTankFill();
		}
		else
		{
			VT_ClearTankFillAmount();
		}
		break;
	}
}

void VT_AdjustTankCapacity(int8_t direction)
{
	Machine.TankCapacityUnits = constrain(Machine.TankCapacityUnits + direction * VT_TANK_STEP_UNITS, 0.0f, 100000.0f);
	if ((Machine.TankCapacityUnits > 0.0f) && (Machine.TankRemainingUnits > Machine.TankCapacityUnits))
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
	}
	VT_SaveMachineSettings(false);
}

void VT_AdjustTankCapacityUnits(float deltaUnits)
{
	Machine.TankCapacityUnits = constrain(Machine.TankCapacityUnits + deltaUnits, 0.0f, 100000.0f);
	if ((Machine.TankCapacityUnits > 0.0f) && (Machine.TankRemainingUnits > Machine.TankCapacityUnits))
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
	}
	VT_SaveMachineSettings(false);
}

void VT_AdjustTankRemaining(int8_t direction)
{
	Machine.TankRemainingUnits = constrain(Machine.TankRemainingUnits + direction * VT_TANK_STEP_UNITS, 0.0f, 100000.0f);
	if ((Machine.TankCapacityUnits > 0.0f) && (Machine.TankRemainingUnits > Machine.TankCapacityUnits))
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
	}
	VT_SaveMachineSettings(false);
}

void VT_AdjustTankFillAmount(int8_t direction)
{
	VT_TankFillAmountUnits = constrain(VT_TankFillAmountUnits + direction * VT_TANK_STEP_UNITS, 0.0f, 100000.0f);
}

void VT_AdjustTankFillAmountUnits(float deltaUnits)
{
	VT_TankFillAmountUnits = constrain(VT_TankFillAmountUnits + deltaUnits, 0.0f, 100000.0f);
}

void VT_ApplyTankFill()
{
	RateControl_AddTankUnits(VT_TankFillAmountUnits);
	VT_TankFillAmountUnits = 0.0f;
	VT_TankView = VT_TANK_VIEW_OVERVIEW;
}

void VT_ClearTankFillAmount()
{
	VT_TankFillAmountUnits = 0.0f;
}

void VT_FillTankToCapacity()
{
	if (Machine.TankCapacityUnits > 0.0f)
	{
		Machine.TankRemainingUnits = Machine.TankCapacityUnits;
		VT_TankFillAmountUnits = 0.0f;
		VT_SaveMachineSettings(false);
	}
}

void VT_EmptyTank()
{
	Machine.TankRemainingUnits = 0.0f;
	VT_TankFillAmountUnits = 0.0f;
	VT_SaveMachineSettings(false);
}

void VT_ResetTripCountersFromVT()
{
	RateControl_ResetTripCounters();
	Serial.println(F("VT trip counters reset."));
}

void VT_MoveSettingsSelection(int8_t direction)
{
	if (direction >= 0)
	{
		VT_SettingsSelection = (VT_SettingsSelection + 1) % VT_SETTINGS_COUNT;
	}
	else
	{
		VT_SettingsSelection = (VT_SettingsSelection == 0) ? (VT_SETTINGS_COUNT - 1) : (VT_SettingsSelection - 1);
	}
}

void VT_OpenSettingsSelection()
{
	switch (VT_SettingsSelection)
	{
	case 0:
		VT_CurrentScreen = VT_SCREEN_SECTIONS;
		break;

	case 1:
		VT_CurrentScreen = VT_SCREEN_RATE;
		break;

	case 2:
		VT_CurrentScreen = VT_SCREEN_OUTPUTS;
		break;

	case 3:
		VT_CurrentScreen = VT_SCREEN_TANK;
		VT_TankView = VT_TANK_VIEW_OVERVIEW;
		break;

	default:
		VT_CurrentScreen = VT_SCREEN_SETTINGS;
		break;
	}
	VT_SelectFirstFieldForScreen(VT_CurrentScreen);
}

void VT_OpenSettingsTile(uint8_t tileIndex)
{
	if (tileIndex >= VT_SETTINGS_COUNT) return;
	VT_SettingsSelection = tileIndex;
	VT_OpenSettingsSelection();
}

void VT_AdjustSelectedField(int8_t direction)
{
	bool geometryChanged = VT_SelectedFieldChangesGeometry();
	bool moduleConfigChanged = false;
	bool sensorConfigChanged = false;

	if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		VT_AdjustTankField(direction);
		VT_SendStatus(true);
		return;
	}

	if (VT_SelectedField < VT_SectionCountField())
	{
		uint8_t productIndex = VT_SelectedField / VT_ProductFieldCount();
		uint8_t productField = VT_SelectedField % VT_ProductFieldCount();

		if (productField == 0)
		{
			Sensor[productIndex].ControlType = VT_ControlTypeStep(Sensor[productIndex].ControlType, direction);
			sensorConfigChanged = true;
		}
		else if (productField == 1)
		{
			AutoOn = !AutoOn;
		}
		else if (productField == 2)
		{
			if (AutoOn)
			{
				Machine.TargetRateLHa[productIndex] += direction * 1.0f;
				if (Machine.TargetRateLHa[productIndex] < 0.0f) Machine.TargetRateLHa[productIndex] = 0.0f;
			}
			else
			{
				Machine.ManualPWM[productIndex] = constrain(static_cast<int>(Machine.ManualPWM[productIndex]) + direction * 5, -4095, 4095);
			}
		}
		else if (productField == 3)
		{
			Machine.MeterCal[productIndex] += direction * 10.0f;
			if (Machine.MeterCal[productIndex] < 1.0f) Machine.MeterCal[productIndex] = 1.0f;
			Sensor[productIndex].MeterCal = Machine.MeterCal[productIndex];
		}
		else
		{
			Machine.UnitMode = (Machine.UnitMode + 1) % 4;
		}
	}
	else if (VT_SelectedField == VT_SectionCountField())
	{
		Machine.SectionCount = constrain(static_cast<int>(Machine.SectionCount) + direction, 1, 16);
		if (VT_SelectedField >= VT_FieldCount()) VT_SelectedField = VT_FieldCount() - 1;
	}
	else if (VT_SelectedField < VT_MasterField())
	{
		uint8_t sectionIndex = VT_SelectedField - VT_SectionWidthBaseField();
		Machine.SectionWidthCm[sectionIndex] = constrain(static_cast<int>(Machine.SectionWidthCm[sectionIndex]) + direction, 1, 2000);
	}
	else if (VT_SelectedField == VT_MasterField())
	{
		MasterOn = !MasterOn;
		TC_SetAllConfiguredSections(MasterOn);
	}
	else if (VT_SelectedField == VT_RelayPolarityField())
	{
		MDL.InvertRelay = !MDL.InvertRelay;
		moduleConfigChanged = true;
	}
	else if (VT_SelectedField == VT_OnboardRelayControlField())
	{
		MDL.OnboardRelayControl = (MDL.OnboardRelayControl == 0) ? 1 : 0;
		if (MDL.OnboardRelayControl == 1)
		{
			RelayLoadDefaultOnboardPins();
			RelayEnsureGPIOPins();
		}
		if (VT_SelectedField >= VT_FieldCount()) VT_SelectedField = VT_FieldCount() - 1;
		moduleConfigChanged = true;
	}
	else if (VT_SelectedField == VT_RemoteRelayControlField())
	{
		MDL.RemoteRelayControl = RemoteRelayControlStep(MDL.RemoteRelayControl, direction);
		if (VT_SelectedField >= VT_FieldCount()) VT_SelectedField = VT_FieldCount() - 1;
		moduleConfigChanged = true;
	}
	else if (VT_SelectedField < VT_RelayTestField())
	{
		uint8_t relayIndex = VT_RelayPinFieldToRelayIndex();
		MDL.RelayControlPins[relayIndex] = RemoteRelayPinStep(MDL.RelayControlPins[relayIndex], direction);
		moduleConfigChanged = true;
	}
	else
	{
		RelayTestForce = !RelayTestForce;
	}

	if (moduleConfigChanged)
	{
		SaveData();
		SplitRelayControl();
	}
	else if (sensorConfigChanged)
	{
		SaveData();
	}
	else
	{
		VT_SaveMachineSettings(geometryChanged);
	}
	VT_SendStatus(true);
}

FLASHMEM void VT_HandleKeyEvent(const isobus::VirtualTerminalClient::VTKeyEvent &event)
{
	const bool screenButton = (event.objectID >= SettingsSections_Button) && (event.objectID <= TankButton_06);
	if (screenButton)
	{
		if ((event.keyEvent == isobus::VirtualTerminalClient::KeyActivationCode::ButtonStillHeld) ||
		    (event.keyEvent == isobus::VirtualTerminalClient::KeyActivationCode::ButtonPressAborted))
		{
			return;
		}

		const uint32_t now = millis();
		if (now - VT_LastScreenButtonActionMs < 250) return;
		VT_LastScreenButtonActionMs = now;
	}
	else if (event.keyEvent != isobus::VirtualTerminalClient::KeyActivationCode::ButtonUnlatchedOrReleased)
	{
		return;
	}

	if (VT_CurrentScreen == VT_SCREEN_MAIN)
	{
		switch (event.objectID)
		{
		case Plus_Button:
			VT_AdjustMainDose(-1);
			break;

		case Minus_Button:
			break;

		case alarm_SoftKey:
			VT_CurrentScreen = VT_SCREEN_SETTINGS;
			break;

		case acknowledgeAlarm_SoftKey:
			VT_ToggleMasterFromVT();
			break;

		case SoftKey_04:
			VT_AdjustManualSections(1);
			break;

		case SoftKey_05:
			VT_AdjustManualSections(3);
			break;

		case SoftKey_06:
			VT_ToggleAutoManualMode();
			break;

		case SoftKey_07:
			break;

		case SoftKey_08:
			VT_AdjustMainDose(1);
			break;

		case SoftKey_09:
			break;

		case SoftKey_10:
			VT_AdjustManualSections(0);
			break;

		case SoftKey_11:
			VT_AdjustManualSections(2);
			break;

		default:
			break;
		}
	}
	else if (VT_CurrentScreen == VT_SCREEN_SETTINGS)
	{
		switch (event.objectID)
		{
		case SettingsSections_Button:
			VT_OpenSettingsTile(0);
			break;

		case SettingsRate_Button:
			VT_OpenSettingsTile(1);
			break;

		case SettingsOutputs_Button:
			VT_OpenSettingsTile(2);
			break;

		case SettingsTank_Button:
			VT_OpenSettingsTile(3);
			break;

		case Plus_Button:
			VT_MoveSettingsSelection(-1);
			break;

		case Minus_Button:
			VT_MoveSettingsSelection(1);
			break;

		case alarm_SoftKey:
			VT_OpenSettingsSelection();
			break;

		case acknowledgeAlarm_SoftKey:
			VT_CurrentScreen = VT_SCREEN_MAIN;
			break;

		default:
			break;
		}
	}
	else if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		switch (event.objectID)
		{
		case TankButton_01:
			VT_HandleTankButton(0);
			break;

		case TankButton_02:
			VT_HandleTankButton(1);
			break;

		case TankButton_03:
			VT_HandleTankButton(2);
			break;

	case TankButton_04:
		VT_HandleTankButton(3);
		break;

	case TankButton_05:
		break;

	case TankButton_06:
		break;

		case alarm_SoftKey:
			VT_TankView = VT_TANK_VIEW_CAPACITY;
			break;

		case acknowledgeAlarm_SoftKey:
			if (VT_TankView == VT_TANK_VIEW_OVERVIEW)
			{
				VT_CurrentScreen = VT_SCREEN_SETTINGS;
			}
			else
			{
				VT_TankView = VT_TANK_VIEW_OVERVIEW;
			}
			break;

		default:
			break;
		}
	}
	else
	{
		switch (event.objectID)
		{
		case Plus_Button:
			VT_AdjustSelectedField(1);
			break;

		case Minus_Button:
			VT_AdjustSelectedField(-1);
			break;

		case alarm_SoftKey:
			VT_SelectNextFieldForScreen(VT_CurrentScreen);
			break;

		case acknowledgeAlarm_SoftKey:
			VT_CurrentScreen = VT_SCREEN_SETTINGS;
			break;

		default:
			break;
		}
	}

	VT_SendStatus(true);
}

bool VT_PointInBox(uint16_t x, uint16_t y, uint16_t left, uint16_t top, uint16_t width, uint16_t height)
{
	return (x >= left) && (x < (left + width)) && (y >= top) && (y < (top + height));
}

bool VT_HandleSettingsPoint(uint16_t x, uint16_t y)
{
	if (VT_PointInBox(x, y, 30, 98, 190, 84))
	{
		VT_OpenSettingsTile(0);
		return true;
	}
	if (VT_PointInBox(x, y, 260, 98, 190, 84))
	{
		VT_OpenSettingsTile(1);
		return true;
	}
	if (VT_PointInBox(x, y, 30, 220, 190, 84))
	{
		VT_OpenSettingsTile(2);
		return true;
	}
	if (VT_PointInBox(x, y, 260, 220, 190, 84))
	{
		VT_OpenSettingsTile(3);
		return true;
	}
	return false;
}

bool VT_HandleTankButton(uint8_t buttonIndex)
{
	Serial.print(F("VT tank button: view="));
	Serial.print(static_cast<uint8_t>(VT_TankView));
	Serial.print(F(" button="));
	Serial.println(buttonIndex);

	if (VT_TankView == VT_TANK_VIEW_OVERVIEW)
	{
		switch (buttonIndex)
		{
		case 0:
			VT_FillTankToCapacity();
			return true;

		case 3:
			VT_TankView = VT_TANK_VIEW_ADD;
			return true;

		case 1:
			VT_EmptyTank();
			return true;

		case 2:
			VT_ResetTripCountersFromVT();
			return true;
		}
	}
	else if (VT_TankView == VT_TANK_VIEW_ADD)
	{
		switch (buttonIndex)
		{
		case 0:
			VT_AdjustTankFillAmountUnits(-10.0f);
			return true;

		case 1:
			VT_ApplyTankFill();
			return true;

		case 2:
			VT_ClearTankFillAmount();
			return true;

		case 3:
			VT_AdjustTankFillAmountUnits(10.0f);
			return true;
		}
	}
	else if (VT_TankView == VT_TANK_VIEW_CAPACITY)
	{
		switch (buttonIndex)
		{
		case 0:
			VT_AdjustTankCapacityUnits(-100.0f);
			return true;

		case 1:
			VT_AdjustTankCapacityUnits(-10.0f);
			return true;

		case 2:
			VT_AdjustTankCapacityUnits(10.0f);
			return true;

		case 3:
			VT_AdjustTankCapacityUnits(100.0f);
			return true;

		}
	}

	return false;
}

bool VT_HandleTankPoint(uint16_t x, uint16_t y)
{
	if (VT_TankView == VT_TANK_VIEW_OVERVIEW)
	{
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(0);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(1);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(2);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(3);
	}
	else if (VT_TankView == VT_TANK_VIEW_ADD)
	{
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(0);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(1);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(2);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(3);
	}
	else if (VT_TankView == VT_TANK_VIEW_CAPACITY)
	{
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(0);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_LEFT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(1);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW2_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(2);
		if (VT_PointInBox(x, y, VT_TANK_BUTTON_RIGHT_X, VT_TANK_BUTTON_ROW1_Y, VT_TANK_BUTTON_W, VT_TANK_BUTTON_H)) return VT_HandleTankButton(3);
	}

	return false;
}

FLASHMEM void VT_HandlePointingEvent(const isobus::VirtualTerminalClient::VTPointingEvent &event)
{
	if ((event.keyEvent == isobus::VirtualTerminalClient::KeyActivationCode::ButtonStillHeld) ||
	    (event.keyEvent == isobus::VirtualTerminalClient::KeyActivationCode::ButtonPressAborted))
	{
		return;
	}

	static uint32_t lastPointAction = 0;
	const uint32_t now = millis();
	if (now - lastPointAction < 250) return;
	if (now - VT_LastScreenButtonActionMs < 250) return;

	bool handled = false;
	if (VT_CurrentScreen == VT_SCREEN_SETTINGS)
	{
		handled = VT_HandleSettingsPoint(event.xPosition, event.yPosition);
	}
	else if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		handled = VT_HandleTankPoint(event.xPosition, event.yPosition);
	}

	if (handled)
	{
		lastPointAction = now;
		VT_SendStatus(true);
	}
}

FLASHMEM void VT_Begin()
{
	if (ISOBUSControlFunction == nullptr) return;

	const isobus::NAMEFilter filterVirtualTerminal(isobus::NAME::NAMEParameters::FunctionCode, static_cast<std::uint8_t>(isobus::NAME::Function::VirtualTerminal));
	const std::vector<isobus::NAMEFilter> vtNameFilters = { filterVirtualTerminal };
	auto partnerVT = isobus::CANNetworkManager::CANNetwork.create_partnered_control_function(0, vtNameFilters);

	ISOBUSVirtualTerminal = std::make_shared<isobus::VirtualTerminalClient>(partnerVT, ISOBUSControlFunction);
	// ISOBUSVirtualTerminal->set_object_pool(0, VT3TestPool, sizeof(VT3TestPool), "AOG1");
	ISOBUSVirtualTerminal->set_object_pool(0, VT3TestPool, sizeof(VT3TestPool), "RCTVT30");
	ISOBUSVirtualTerminal->get_vt_button_event_dispatcher().add_listener(VT_HandleKeyEvent);
	ISOBUSVirtualTerminal->get_vt_soft_key_event_dispatcher().add_listener(VT_HandleKeyEvent);
	ISOBUSVirtualTerminal->get_vt_pointing_event_dispatcher().add_listener(VT_HandlePointingEvent);
	ISOBUSVirtualTerminal->initialize(false);
	VT_WorkingSetSelectPending = true;
	Serial.print(F("VT object pool bytes: "));
	Serial.println(sizeof(VT3TestPool));
}

FLASHMEM void VT_Update()
{
	if (ISOBUSVirtualTerminal == nullptr) return;

	ISOBUSVirtualTerminal->update();
	VT_LogState();

	if (ISOBUSVirtualTerminal->get_is_connected())
	{
		if (!TC_MachineAutoMode())
		{
			TC_RefreshMachineCommandTime();
		}

		if (VT_WorkingSetSelectPending)
		{
			ISOBUSVirtualTerminal->send_select_active_working_set(ISOBUSControlFunction->get_NAME().get_full_name());
			VT_WorkingSetSelectPending = false;
			VT_SendStatus(true);
		}
		else
		{
			VT_SendStatus(false);
		}
	}
	else
	{
		VT_WorkingSetSelectPending = true;
		VT_LastActiveMask = UNDEFINED;
	}
}

void VT_SendStringValueIfNeeded(uint16_t objectID, char *lastValue, size_t lastValueSize, const char *value, bool force)
{
	if (lastValueSize == 0) return;

	if (force || (strncmp(lastValue, value, lastValueSize) != 0))
	{
		ISOBUSVirtualTerminal->send_change_string_value(objectID, strlen(value), value);
		strncpy(lastValue, value, lastValueSize - 1);
		lastValue[lastValueSize - 1] = '\0';
	}
}

void VT_FormatFloatValue(char *buffer, size_t bufferSize, float value, uint8_t decimals)
{
	if (bufferSize == 0) return;

	if (isnan(value) || isinf(value))
	{
		strncpy(buffer, "--", bufferSize - 1);
		buffer[bufferSize - 1] = '\0';
		return;
	}

	char temp[24];
	dtostrf(value, 1, decimals, temp);
	char *start = temp;
	while (*start == ' ') start++;
	strncpy(buffer, start, bufferSize - 1);
	buffer[bufferSize - 1] = '\0';
}

FLASHMEM void VT_SendStatus(bool force)
{
	static uint32_t lastVTStatus = 0;
	static char lastTitle[18] = "";
	static char lastModeValue[48] = "";
	static char lastPressureValue[12] = "";
	static char lastDoseValue[12] = "";
	static char lastDoseTarget[16] = "";
	static char lastTankValue[12] = "";
	static char lastAreaValue[12] = "";
	static char lastSpeedValue[12] = "";
	static char lastVolumeValue[12] = "";
	static char lastDoseLabel[16] = "";
	static char lastTankLabel[16] = "";
	static char lastVolumeLabel[8] = "";
	static char lastSectionMap[20] = "";
	static char lastBoomBar[24] = "";
	static char lastNozzleBar[24] = "";
	static char lastSettingsButton[8] = "";
	static char lastMasterButton[8] = "";
	static char lastPlusButton[8] = "";
	static char lastMinusButton[8] = "";
	static char lastSoftKey04[8] = "";
	static char lastSoftKey05[8] = "";
	static char lastSoftKey06[8] = "";
	static char lastSoftKey07[8] = "";
	static char lastSoftKey08[8] = "";
	static char lastSoftKey09[8] = "";
	static char lastSoftKey10[8] = "";
	static char lastSoftKey11[8] = "";
	static char lastTankButton01[12] = "";
	static char lastTankButton02[12] = "";
	static char lastTankButton03[12] = "";
	static char lastTankButton04[12] = "";
	static char lastTankButton05[12] = "";
	static char lastTankButton06[12] = "";
	static char lastConfigCard01Title[16] = "";
	static char lastConfigCard02Title[16] = "";
	static char lastConfigCard03Title[16] = "";
	static char lastConfigCard04Title[16] = "";
	static char lastConfigCard01Value[40] = "";
	static char lastConfigCard02Value[40] = "";
	static char lastConfigCard03Value[40] = "";
	static char lastConfigCard04Value[40] = "";
	static char lastDescription[320] = "";
	static uint32_t lastDisplayedValue = 0xFFFFFFFFUL;

	if (ISOBUSVirtualTerminal == nullptr) return;
	if (!ISOBUSVirtualTerminal->get_is_connected()) return;
	if (!force && (millis() - lastVTStatus < VT_STATUS_INTERVAL_MS)) return;
	lastVTStatus = millis();

	const uint8_t sensorIndex = 0;
	const bool hasSensor = (MDL.SensorCount > sensorIndex);
	const float target = hasSensor ? Sensor[sensorIndex].TargetUPM : 0.0f;
	const float applied = hasSensor ? Sensor[sensorIndex].UPM : 0.0f;
	const float pwm = hasSensor ? Sensor[sensorIndex].PWM : 0.0f;
	const float hz = hasSensor ? Sensor[sensorIndex].Hz : 0.0f;
	const bool workOn = WorkPinOn();
	const bool autoMode = TC_MachineAutoMode();
	const bool tcReady = TC_ProcessDataReady();
	const uint8_t displayRelayLo = RelayEffectiveLo();
	const uint8_t displayRelayHi = RelayEffectiveHi();
	const uint16_t activeMask = VT_DataMaskForScreen();
	const uint32_t tankCapacityDisplay = static_cast<uint32_t>(Machine.TankCapacityUnits + 0.5f);
	const uint32_t tankRemainingDisplay = static_cast<uint32_t>(Machine.TankRemainingUnits + 0.5f);
	const uint32_t tankFillDisplay = static_cast<uint32_t>(VT_TankFillAmountUnits + 0.5f);
	const uint32_t tankUsedDisplay = static_cast<uint32_t>(RateControl_TotalAppliedUnits() + 0.5f);
	const uint32_t lifetimeUsedDisplay = static_cast<uint32_t>(RateControl_LifetimeAppliedUnits() + 0.5f);
	const float actualDose = hasSensor ? RateControl_UnitsPerArea(sensorIndex) : 0.0f;
	char sectionMap[20];
	char boomBar[24];
	char nozzleBar[24];
	char selected[42];
	uint32_t displayedValue = 214748364UL;
	VT_BuildSectionMap(sectionMap, sizeof(sectionMap));
	VT_BuildBoomGraphic(boomBar, sizeof(boomBar), nozzleBar, sizeof(nozzleBar));

	if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		switch (VT_TankView)
		{
		case VT_TANK_VIEW_ADD:
			snprintf(selected, sizeof(selected), "Add %lu units", tankFillDisplay);
			displayedValue += 100000UL + tankFillDisplay;
			break;

		case VT_TANK_VIEW_CAPACITY:
			snprintf(selected, sizeof(selected), "Capacity %lu units", tankCapacityDisplay);
			displayedValue += 200000UL + tankCapacityDisplay;
			break;

		case VT_TANK_VIEW_OVERVIEW:
		default:
			snprintf(selected, sizeof(selected), "Remaining %lu units", tankRemainingDisplay);
			displayedValue += tankRemainingDisplay;
			break;
		}
	}
	else if (VT_SelectedField < VT_SectionCountField())
	{
		uint8_t productIndex = VT_SelectedField / VT_ProductFieldCount();
		uint8_t productField = VT_SelectedField % VT_ProductFieldCount();

		if (productField == 0)
		{
			snprintf(selected, sizeof(selected), "Prod %u Type %s", productIndex + 1, VT_ControlTypeName(Sensor[productIndex].ControlType));
			displayedValue += Sensor[productIndex].ControlType;
		}
		else if (productField == 1)
		{
			snprintf(selected, sizeof(selected), "Prod %u Mode %s", productIndex + 1, AutoOn ? "AUTO" : "MAN");
			displayedValue += AutoOn ? 1 : 0;
		}
		else if (productField == 2)
		{
			if (AutoOn)
			{
				snprintf(selected, sizeof(selected), "Prod %u Dose %.0f %s/ha", productIndex + 1, Machine.TargetRateLHa[productIndex], VT_UnitName());
				displayedValue += static_cast<uint32_t>(Machine.TargetRateLHa[productIndex]);
			}
			else
			{
				snprintf(selected, sizeof(selected), "Prod %u PWM %d", productIndex + 1, Machine.ManualPWM[productIndex]);
				displayedValue += static_cast<uint32_t>(Machine.ManualPWM[productIndex] + 4095);
			}
		}
		else if (productField == 3)
		{
			snprintf(selected, sizeof(selected), "Prod %u MeterCal %.0f", productIndex + 1, Machine.MeterCal[productIndex]);
			displayedValue += static_cast<uint32_t>(Machine.MeterCal[productIndex]);
		}
		else
		{
			snprintf(selected, sizeof(selected), "Unit %s", VT_UnitName());
			displayedValue += Machine.UnitMode;
		}
	}
	else if (VT_SelectedField == VT_SectionCountField())
	{
		snprintf(selected, sizeof(selected), "Sections %u", Machine.SectionCount);
		displayedValue += Machine.SectionCount;
	}
	else if (VT_SelectedField < VT_MasterField())
	{
		uint8_t sectionIndex = VT_SelectedField - VT_SectionWidthBaseField();
		snprintf(selected, sizeof(selected), "Sec %u width %u cm", sectionIndex + 1, Machine.SectionWidthCm[sectionIndex]);
		displayedValue += Machine.SectionWidthCm[sectionIndex];
	}
	else if (VT_SelectedField == VT_MasterField())
	{
		snprintf(selected, sizeof(selected), "Master %s", MasterOn ? "ON" : "OFF");
		displayedValue += MasterOn ? 1 : 0;
	}
	else if (VT_SelectedField == VT_RelayPolarityField())
	{
		snprintf(selected, sizeof(selected), "Relay ON %s", MDL.InvertRelay ? "HIGH" : "LOW");
		displayedValue += MDL.InvertRelay ? 1 : 0;
	}
	else if (VT_SelectedField == VT_OnboardRelayControlField())
	{
		snprintf(selected, sizeof(selected), "Onboard %s", RelayControlName(MDL.OnboardRelayControl));
		displayedValue += MDL.OnboardRelayControl;
	}
	else if (VT_SelectedField == VT_RemoteRelayControlField())
	{
		snprintf(selected, sizeof(selected), "Remote %s", RelayControlName(MDL.RemoteRelayControl));
		displayedValue += MDL.RemoteRelayControl;
	}
	else if (VT_SelectedField < VT_RelayTestField())
	{
		uint8_t relayIndex = VT_RelayPinFieldToRelayIndex();
		if (MDL.RelayControlPins[relayIndex] == NC)
		{
			snprintf(selected, sizeof(selected), "Remote R%u pin OFF", relayIndex + 1);
			displayedValue += 255;
		}
		else
		{
			snprintf(selected, sizeof(selected), "Remote R%u pin %u", relayIndex + 1, MDL.RelayControlPins[relayIndex]);
			displayedValue += MDL.RelayControlPins[relayIndex];
		}
	}
	else
	{
		snprintf(selected, sizeof(selected), "Relay Test %s", RelayTestForce ? "ON" : "OFF");
		displayedValue += RelayTestForce ? 1 : 0;
	}

	const char *tankButton01 = " ";
	const char *tankButton02 = " ";
	const char *tankButton03 = " ";
	const char *tankButton04 = " ";
	const char *tankButton05 = " ";
	const char *tankButton06 = " ";
	char configCard01Title[16] = " ";
	char configCard02Title[16] = " ";
	char configCard03Title[16] = " ";
	char configCard04Title[16] = " ";
	char configCard01Value[40] = " ";
	char configCard02Value[40] = " ";
	char configCard03Value[40] = " ";
	char configCard04Value[40] = " ";
	char status[320];
	if (VT_CurrentScreen == VT_SCREEN_MAIN)
	{
		char rateLine[82];
		if (hasSensor && autoMode)
		{
			snprintf(rateLine, sizeof(rateLine), "Dose %.0f/%.0f %s/ha  UPM %.1f/%.1f  Hz %.1f",
			         Machine.TargetRateLHa[sensorIndex],
			         actualDose,
			         VT_UnitName(),
			         applied,
			         target,
			         hz);
			displayedValue = 214748364UL + static_cast<uint32_t>(Machine.TargetRateLHa[sensorIndex]);
		}
		else if (hasSensor)
		{
			snprintf(rateLine, sizeof(rateLine), "PWM %d -> %.0f  Dose %.0f %s/ha  Hz %.1f",
			         Machine.ManualPWM[sensorIndex],
			         pwm,
			         actualDose,
			         VT_UnitName(),
			         hz);
			displayedValue = 214748364UL + static_cast<uint32_t>(Machine.ManualPWM[sensorIndex] + 4095);
		}
		else
		{
			snprintf(rateLine, sizeof(rateLine), "No product configured");
			displayedValue = 214748364UL;
		}

		snprintf(status, sizeof(status),
		         "MAIN\r\n"
		         "MASTER:%s  WORK:%s  TC:%s\r\n"
		         "P1 %s %s\r\n"
		         "%s\r\n"
		         "%.1fkm/h  %.2fm  SEC %s\r\n"
		         "Relays %02X/%02X",
		         MasterOn ? "ON" : "OFF",
		         workOn ? "ON" : "OFF",
		         tcReady ? "ON" : "WAIT",
		         hasSensor ? VT_ControlTypeName(Sensor[sensorIndex].ControlType) : "None",
		         hasSensor ? VT_ModeName(autoMode) : "-",
		         rateLine,
		         SPEED_GetKmh(),
		         RateControl_ActiveWidthM(),
		         sectionMap,
		         displayRelayLo,
		         displayRelayHi);
	}
	else if (VT_CurrentScreen == VT_SCREEN_SETTINGS)
	{
		displayedValue = 214748364UL + VT_SettingsSelection;
		snprintf(status, sizeof(status),
		         "SETTINGS\r\n"
		         "[====] SECTIONS          [l/ha] RATE\r\n"
		         "setup sections           dose setup\r\n\r\n\r\n"
		         "[I/O ] OUTPUTS           [____] TANK\r\n"
		         "relay outputs            tank setup");
	}
	else if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		displayedValue = 214748364UL + (static_cast<uint32_t>(VT_TankView) * 100000UL) + tankRemainingDisplay;
		if (VT_TankView == VT_TANK_VIEW_ADD)
		{
			tankButton01 = "-10";
			tankButton02 = "ADD";
			tankButton03 = "CLEAR";
			tankButton04 = "+10";
			tankButton05 = " ";
			tankButton06 = " ";
			snprintf(status, sizeof(status),
			         "TANK - ADD\r\n"
			         "Remaining %lu / %lu %s\r\n"
			         "Amount to add: %lu %s",
			         tankRemainingDisplay,
			         tankCapacityDisplay,
			         VT_UnitName(),
			         tankFillDisplay,
			         VT_UnitName());
		}
		else if (VT_TankView == VT_TANK_VIEW_CAPACITY)
		{
			tankButton01 = "-100";
			tankButton02 = "-10";
			tankButton03 = "+10";
			tankButton04 = "+100";
			tankButton05 = " ";
			tankButton06 = " ";
			snprintf(status, sizeof(status),
			         "TANK - CAPACITY\r\n"
			         "Tank capacity: %lu %s\r\n"
			         "Remaining: %lu %s",
			         tankCapacityDisplay,
			         VT_UnitName(),
			         tankRemainingDisplay,
			         VT_UnitName());
		}
		else
		{
			tankButton01 = "FULL";
			tankButton02 = "EMPTY";
			tankButton03 = "RESET";
			tankButton04 = "ADD";
			tankButton05 = " ";
			tankButton06 = " ";
			snprintf(status, sizeof(status),
			         "TANK\r\n"
			         "CAP %lu %s  LEFT %lu %s\r\n"
			         "TRIP %lu %s %.2f ha\r\n"
			         "LIFE %lu %s %.1f ha",
			         tankCapacityDisplay,
			         VT_UnitName(),
			         tankRemainingDisplay,
			         VT_UnitName(),
			         tankUsedDisplay,
			         VT_UnitName(),
			         RateControl_TotalAreaHa(),
			         lifetimeUsedDisplay,
			         VT_UnitName(),
			         RateControl_LifetimeAreaHa());
		}
	}
	else
	{
		char detail[96];
		if (VT_CurrentScreen == VT_SCREEN_SECTIONS)
		{
			char activeWidthText[12];
			VT_FormatFloatValue(activeWidthText, sizeof(activeWidthText), VT_ConfiguredWidthM(), 2);
			snprintf(detail, sizeof(detail), "Count:%u  Width:%sm  %s", Machine.SectionCount, activeWidthText, sectionMap);
			snprintf(configCard01Title, sizeof(configCard01Title), "COUNT");
			snprintf(configCard01Value, sizeof(configCard01Value), "%u sections", Machine.SectionCount);
			snprintf(configCard02Title, sizeof(configCard02Title), "WIDTH");
			snprintf(configCard02Value, sizeof(configCard02Value), "%s m", activeWidthText);
			snprintf(configCard03Title, sizeof(configCard03Title), "ACTIVE");
			snprintf(configCard03Value, sizeof(configCard03Value), "%s", sectionMap);
			snprintf(configCard04Title, sizeof(configCard04Title), "SELECTED");
			snprintf(configCard04Value, sizeof(configCard04Value), "%s", selected);
		}
		else if (VT_CurrentScreen == VT_SCREEN_RATE)
		{
			snprintf(detail, sizeof(detail), "P1 %s %s  Dose %.0f  PWM %d",
			         hasSensor ? VT_ControlTypeName(Sensor[sensorIndex].ControlType) : "None",
			         hasSensor ? VT_ModeName(AutoOn) : "-",
			         Machine.TargetRateLHa[sensorIndex],
			         Machine.ManualPWM[sensorIndex]);
			snprintf(configCard01Title, sizeof(configCard01Title), "PRODUCT");
			snprintf(configCard01Value, sizeof(configCard01Value), "%s", hasSensor ? VT_ControlTypeName(Sensor[sensorIndex].ControlType) : "None");
			snprintf(configCard02Title, sizeof(configCard02Title), "MODE");
			snprintf(configCard02Value, sizeof(configCard02Value), "%s", hasSensor ? VT_ModeName(AutoOn) : "-");
			snprintf(configCard03Title, sizeof(configCard03Title), "RATE");
			if (hasSensor && AutoOn)
			{
				char targetRateText[12];
				VT_FormatFloatValue(targetRateText, sizeof(targetRateText), Machine.TargetRateLHa[sensorIndex], 0);
				snprintf(configCard03Value, sizeof(configCard03Value), "%s %s/ha", targetRateText, VT_UnitName());
			}
			else
			{
				snprintf(configCard03Value, sizeof(configCard03Value), "PWM %d", Machine.ManualPWM[sensorIndex]);
			}
			snprintf(configCard04Title, sizeof(configCard04Title), "SELECTED");
			snprintf(configCard04Value, sizeof(configCard04Value), "%s", selected);
		}
		else
		{
			snprintf(detail, sizeof(detail), "Onboard:%s  Remote:%s  ON:%s",
			         RelayControlName(MDL.OnboardRelayControl),
			         RelayControlName(MDL.RemoteRelayControl),
			         MDL.InvertRelay ? "HIGH" : "LOW");
			snprintf(configCard01Title, sizeof(configCard01Title), "ONBOARD");
			snprintf(configCard01Value, sizeof(configCard01Value), "%s", RelayControlName(MDL.OnboardRelayControl));
			snprintf(configCard02Title, sizeof(configCard02Title), "REMOTE");
			snprintf(configCard02Value, sizeof(configCard02Value), "%s", RelayControlName(MDL.RemoteRelayControl));
			snprintf(configCard03Title, sizeof(configCard03Title), "ON LEVEL");
			snprintf(configCard03Value, sizeof(configCard03Value), "%s", MDL.InvertRelay ? "HIGH" : "LOW");
			snprintf(configCard04Title, sizeof(configCard04Title), "SELECTED");
			snprintf(configCard04Value, sizeof(configCard04Value), "%s", selected);
		}

		snprintf(status, sizeof(status),
		         "%s CONFIG\r\n"
		         "%s\r\n"
		         "> %s",
		         VT_ScreenTitle(),
		         detail,
		         selected);
	}

	const char *title = VT_ScreenTitle();
	char modeValue[48];
	char pressureValue[12];
	char doseValue[12];
	char doseTarget[16];
	char tankValue[12];
	char areaValue[12];
	char speedValue[12];
	char volumeValue[12];
	char doseLabel[16];
	char tankLabel[16];
	char volumeLabel[8];
	snprintf(modeValue, sizeof(modeValue), "%s  MASTER:%s  TC:%s  WORK:%s",
	         autoMode ? "AUTO" : "MAN",
	         MasterOn ? "ON" : "OFF",
	         tcReady ? "ON" : "WAIT",
	         workOn ? "ON" : "OFF");
	snprintf(pressureValue, sizeof(pressureValue), "%u", PressureReading);
	if (hasSensor && autoMode)
	{
		char targetDoseText[6];
		VT_FormatFloatValue(targetDoseText, sizeof(targetDoseText), Machine.TargetRateLHa[sensorIndex], 0);
		VT_FormatFloatValue(doseValue, sizeof(doseValue), actualDose, 0);
		snprintf(doseTarget, sizeof(doseTarget), "ZAD %s", targetDoseText);
	}
	else if (hasSensor)
	{
		VT_FormatFloatValue(doseValue, sizeof(doseValue), actualDose, 0);
		snprintf(doseTarget, sizeof(doseTarget), "PWM %d", Machine.ManualPWM[sensorIndex]);
	}
	else
	{
		snprintf(doseValue, sizeof(doseValue), "--");
		snprintf(doseTarget, sizeof(doseTarget), "ZAD --");
	}
	snprintf(tankValue, sizeof(tankValue), "%lu", tankRemainingDisplay);
	VT_FormatFloatValue(areaValue, sizeof(areaValue), RateControl_TotalAreaHa(), 2);
	VT_FormatFloatValue(speedValue, sizeof(speedValue), SPEED_GetKmh(), 1);
	VT_FormatFloatValue(volumeValue, sizeof(volumeValue), RateControl_TotalAppliedUnits(), 0);
	snprintf(doseLabel, sizeof(doseLabel), "DAWKA [%s/ha]", VT_UnitName());
	snprintf(tankLabel, sizeof(tankLabel), "ZBIORNIK [%s]", VT_UnitName());
	snprintf(volumeLabel, sizeof(volumeLabel), "D [%s]", VT_UnitName());

	const char *settingsButton = "@";
	const char *masterButton = "I/O";
	const char *plusButton = "-";
	const char *minusButton = " ";
	const char *softKey04 = autoMode ? " " : "\xE2\x86\x92\xE2\x97\xA3";
	const char *softKey05 = autoMode ? " " : "\xE2\x86\x90\xE2\x97\xA3";
	const char *softKey06 = autoMode ? "A/M" : "M/A";
	const char *softKey07 = " ";
	const char *softKey08 = "+";
	const char *softKey09 = " ";
	const char *softKey10 = autoMode ? " " : "\xE2\x97\xA2\xE2\x86\x90";
	const char *softKey11 = autoMode ? " " : "\xE2\x97\xA2\xE2\x86\x92";
	const char *description = " ";

	if (VT_CurrentScreen == VT_SCREEN_SETTINGS)
	{
		snprintf(modeValue, sizeof(modeValue), "User settings");
		settingsButton = ">";
		masterButton = "<";
		plusButton = "^";
		minusButton = "v";
		softKey04 = " ";
		softKey05 = " ";
		softKey06 = " ";
		softKey07 = " ";
		softKey08 = " ";
		description = " ";
	}
	else if (VT_CurrentScreen == VT_SCREEN_TANK)
	{
		if (VT_TankView == VT_TANK_VIEW_ADD)
		{
			snprintf(modeValue, sizeof(modeValue), "Tank add amount");
		}
		else if (VT_TankView == VT_TANK_VIEW_CAPACITY)
		{
			snprintf(modeValue, sizeof(modeValue), "Tank units setup");
		}
		else
		{
			snprintf(modeValue, sizeof(modeValue), "Tank");
		}
		settingsButton = "@";
		masterButton = "<";
		plusButton = " ";
		minusButton = " ";
		softKey04 = " ";
		softKey05 = " ";
		softKey06 = " ";
		softKey07 = " ";
		softKey08 = " ";
		description = status;
	}
	else if (VT_CurrentScreen != VT_SCREEN_MAIN)
	{
		snprintf(modeValue, sizeof(modeValue), "%s configuration", VT_ScreenTitle());
		settingsButton = ">";
		masterButton = "<";
		plusButton = "+";
		minusButton = "-";
		softKey04 = " ";
		softKey05 = " ";
		softKey06 = " ";
		softKey07 = " ";
		softKey08 = " ";
		description = " ";
	}

	const bool updateAll = (VT_LastActiveMask != activeMask);
	if (updateAll)
	{
		ISOBUSVirtualTerminal->send_change_active_mask(example_WorkingSet, activeMask);
		VT_LastActiveMask = activeMask;
	}

	VT_SendStringValueIfNeeded(title_OutStr, lastTitle, sizeof(lastTitle), title, updateAll);
	VT_SendStringValueIfNeeded(ModeValue_VarStr, lastModeValue, sizeof(lastModeValue), modeValue, updateAll);
	VT_SendStringValueIfNeeded(DoseLabel_VarStr, lastDoseLabel, sizeof(lastDoseLabel), doseLabel, updateAll);
	VT_SendStringValueIfNeeded(TankLabel_VarStr, lastTankLabel, sizeof(lastTankLabel), tankLabel, updateAll);
	VT_SendStringValueIfNeeded(VolumeLabel_VarStr, lastVolumeLabel, sizeof(lastVolumeLabel), volumeLabel, updateAll);
	VT_SendStringValueIfNeeded(PressureValue_VarStr, lastPressureValue, sizeof(lastPressureValue), pressureValue, updateAll);
	VT_SendStringValueIfNeeded(DoseTarget_VarStr, lastDoseTarget, sizeof(lastDoseTarget), doseTarget, updateAll);
	VT_SendStringValueIfNeeded(DoseValue_VarStr, lastDoseValue, sizeof(lastDoseValue), doseValue, updateAll);
	VT_SendStringValueIfNeeded(TankValue_VarStr, lastTankValue, sizeof(lastTankValue), tankValue, updateAll);
	VT_SendStringValueIfNeeded(AreaValue_VarStr, lastAreaValue, sizeof(lastAreaValue), areaValue, updateAll);
	VT_SendStringValueIfNeeded(SpeedValue_VarStr, lastSpeedValue, sizeof(lastSpeedValue), speedValue, updateAll);
	VT_SendStringValueIfNeeded(VolumeValue_VarStr, lastVolumeValue, sizeof(lastVolumeValue), volumeValue, updateAll);
	VT_SendStringValueIfNeeded(SectionMap_VarStr, lastSectionMap, sizeof(lastSectionMap), sectionMap, updateAll);
	VT_SendStringValueIfNeeded(BoomBar_VarStr, lastBoomBar, sizeof(lastBoomBar), boomBar, updateAll);
	VT_SendStringValueIfNeeded(NozzleBar_VarStr, lastNozzleBar, sizeof(lastNozzleBar), nozzleBar, updateAll);
	VT_SendStringValueIfNeeded(SettingsButton_VarStr, lastSettingsButton, sizeof(lastSettingsButton), settingsButton, updateAll);
	VT_SendStringValueIfNeeded(MasterButton_VarStr, lastMasterButton, sizeof(lastMasterButton), masterButton, updateAll);
	VT_SendStringValueIfNeeded(PlusButton_VarStr, lastPlusButton, sizeof(lastPlusButton), plusButton, updateAll);
	VT_SendStringValueIfNeeded(MinusButton_VarStr, lastMinusButton, sizeof(lastMinusButton), minusButton, updateAll);
	VT_SendStringValueIfNeeded(SoftKey04_VarStr, lastSoftKey04, sizeof(lastSoftKey04), softKey04, updateAll);
	VT_SendStringValueIfNeeded(SoftKey05_VarStr, lastSoftKey05, sizeof(lastSoftKey05), softKey05, updateAll);
	VT_SendStringValueIfNeeded(SoftKey06_VarStr, lastSoftKey06, sizeof(lastSoftKey06), softKey06, updateAll);
	VT_SendStringValueIfNeeded(SoftKey07_VarStr, lastSoftKey07, sizeof(lastSoftKey07), softKey07, updateAll);
	VT_SendStringValueIfNeeded(SoftKey08_VarStr, lastSoftKey08, sizeof(lastSoftKey08), softKey08, updateAll);
	VT_SendStringValueIfNeeded(SoftKey09_VarStr, lastSoftKey09, sizeof(lastSoftKey09), softKey09, updateAll);
	VT_SendStringValueIfNeeded(SoftKey10_VarStr, lastSoftKey10, sizeof(lastSoftKey10), softKey10, updateAll);
	VT_SendStringValueIfNeeded(SoftKey11_VarStr, lastSoftKey11, sizeof(lastSoftKey11), softKey11, updateAll);
	VT_SendSoftKeyBackgroundsIfNeeded(settingsButton,
	                                  masterButton,
	                                  plusButton,
	                                  minusButton,
	                                  softKey04,
	                                  softKey05,
	                                  softKey06,
	                                  softKey07,
	                                  softKey08,
	                                  softKey09,
	                                  softKey10,
	                                  softKey11,
	                                  updateAll);
	VT_SendStringValueIfNeeded(TankButton01_VarStr, lastTankButton01, sizeof(lastTankButton01), tankButton01, updateAll);
	VT_SendStringValueIfNeeded(TankButton02_VarStr, lastTankButton02, sizeof(lastTankButton02), tankButton02, updateAll);
	VT_SendStringValueIfNeeded(TankButton03_VarStr, lastTankButton03, sizeof(lastTankButton03), tankButton03, updateAll);
	VT_SendStringValueIfNeeded(TankButton04_VarStr, lastTankButton04, sizeof(lastTankButton04), tankButton04, updateAll);
	VT_SendStringValueIfNeeded(TankButton05_VarStr, lastTankButton05, sizeof(lastTankButton05), tankButton05, updateAll);
	VT_SendStringValueIfNeeded(TankButton06_VarStr, lastTankButton06, sizeof(lastTankButton06), tankButton06, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard01Title_VarStr, lastConfigCard01Title, sizeof(lastConfigCard01Title), configCard01Title, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard02Title_VarStr, lastConfigCard02Title, sizeof(lastConfigCard02Title), configCard02Title, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard03Title_VarStr, lastConfigCard03Title, sizeof(lastConfigCard03Title), configCard03Title, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard04Title_VarStr, lastConfigCard04Title, sizeof(lastConfigCard04Title), configCard04Title, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard01Value_VarStr, lastConfigCard01Value, sizeof(lastConfigCard01Value), configCard01Value, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard02Value_VarStr, lastConfigCard02Value, sizeof(lastConfigCard02Value), configCard02Value, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard03Value_VarStr, lastConfigCard03Value, sizeof(lastConfigCard03Value), configCard03Value, updateAll);
	VT_SendStringValueIfNeeded(ConfigCard04Value_VarStr, lastConfigCard04Value, sizeof(lastConfigCard04Value), configCard04Value, updateAll);
	VT_SendStringValueIfNeeded(MainDescription_VarStr, lastDescription, sizeof(lastDescription), description, updateAll);

	if (updateAll || (displayedValue != lastDisplayedValue))
	{
		lastDisplayedValue = displayedValue;
		ISOBUSVirtualTerminal->send_change_numeric_value(ButtonExampleNumber_VarNum, displayedValue);
	}
}
