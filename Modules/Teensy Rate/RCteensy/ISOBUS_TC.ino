// ISOBUS_TC.ino - Task Controller client (ISOBUS build only)
// From jaworeA's RC11-isobus. The module uploads a DDOP (one boom with its sections, a bin per
// rate sensor, rate and speed process data) to a TC: a tractor terminal or AOG-TaskController.
// While the TC commands, it switches the sections (setpoint condensed work state, DDI 290; the
// module reports the actual state, DDI 161) and sets each bin's rate, which becomes that
// sensor's TargetUPM from speed and active width.
//
// Bins: sensors 0 and 1 (TCmaxRates), as many as the module has, each a bin element under the
// boom ("Rate 1", "Rate 2") with its own target and actual rate DDIs, so a TC can give each its
// own product (Group2_Step1_Channels_Design.md D3, bench stand-in for a multi-tank OEM implement).
// Both share the boom's sections.
//
// Rate units: the TC sends the rate in the ISO unit of its DDI (1 litres, 6 kg, 11 seeds per
// ha); the board meters in RC's units (its flow cal is pulses per RC unit). RC sends, per sensor,
// the type and how many metered units are one litre, kg or seed (PGN 32509 / CAN 0xFF1D,
// TC_SetUnits), stored in TCsettings so a board under a terminal without RC keeps them. Each
// bin declares its type's rate DDIs. Until RC has sent a sensor's units, its rate is not applied.
//
// Command authority (ISOBUS_Path_Forward.md "Command authority"):
// - RC: at power-up and after a TC session. RC's rate and relay commands apply, as today.
//   A connected TC is not a command source.
// - TC: from the TC's first section or rate value command. RC's rate and relay commands
//   (0xFF03/0xFF04, UDP 32500/32501) are ignored for the whole module. Entering clears every
//   target, master and the sections; only the bins' sensors are driven, in auto, and only their
//   comm times are kept fresh, so the other products stop and then go to comm loss.
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
// - the TC's target rates are kept in RAM only (the TC sends them again); section geometry is
//   stored in EEPROM at TCsettingsAddr (RC11-isobus used 640, inside mainline's sensors 3-6)
// - no VT client yet (Phase E run screen); no manual mode from the terminal yet

#if ISOBUS_ENABLED

const uint16_t TC_ELEMENT_BOOM = 1;
const uint16_t TC_ELEMENT_BIN_BASE = 20;	// after the sections (2-17): bin of sensor r = 20 + r
const uint16_t TC_OBJ_BOOM = 100;
const uint16_t TC_OBJ_BIN_BASE = 110;
const uint16_t TC_OBJ_SECTION_BASE = 200;
const uint16_t TC_OBJ_DPD_SETPOINT_WORK_STATE = 1000;
const uint16_t TC_OBJ_DPD_ACTUAL_WORK_STATE = 1001;
const uint16_t TC_OBJ_DPD_ACTUAL_SPEED = 1004;
const uint16_t TC_OBJ_DPD_SETPOINT_RATE_BASE = 1010;
const uint16_t TC_OBJ_DPD_ACTUAL_RATE_BASE = 1020;
const uint16_t TC_OBJ_DPD_TOTAL_BASE = 1030;
const uint16_t TC_OBJ_DVP_WIDTH = 3000;
const uint16_t TC_OBJ_DPT_WIDTH_BASE = 3100;

const uint8_t TCmaxSections = 16;
const uint8_t TCmaxRates = 2;			// bins: sensors 0 and 1
const int TCsettingsAddr = 1024;		// after the sensors (253 + 6 x 124 = 997)
const uint16_t TCsettingsID = 0x5447;	// change when TCsettings changes shape (0x5444: one rate; 0x5445: no meter cal; 0x5446: no control type)
const uint8_t TCnoControlType = 255;	// TCsettings.ControlType not set by RC yet

struct TCsettings
{
	uint16_t Identifier = TCsettingsID;
	uint8_t SectionCount = 8;
	uint16_t SectionWidthCm[TCmaxSections] = { 50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50 };
	uint16_t RateDDI[TCmaxRates] = { 1, 1 };		// per sensor, setpoint rate DDI: 1 volume, 6 mass, 11 count per area (from RC)
	float UnitsPerISO[TCmaxRates] = { 0.0f, 0.0f };	// per sensor, metered units per litre, kg or seed (from RC); 0 = not set
	float MeterCal[TCmaxRates] = { 0.0f, 0.0f };	// per sensor, pulses per metered unit (from RC's 32509, which an ISOBUS product
													// gets instead of 32500); 0 = not set
	uint8_t ControlType[TCmaxRates] = { TCnoControlType, TCnoControlType };	// per sensor, RC's control type (32509 byte 13,
													// which an ISOBUS product gets instead of 32500): 0 valve ... 5 timed combo
};

TCsettings TCset;
float TC_TargetRate[TCmaxRates];		// from the TC, litres, kg or seeds per ha; RAM only
bool TC_UnitsWarned[TCmaxRates];		// "units not set" printed once

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
	for (uint8_t r = 0; r < TCmaxRates; r++)
	{
		if (!TC_IsRateDDI(TCset.RateDDI[r])) TCset.RateDDI[r] = 1;
		if (!(TCset.UnitsPerISO[r] > 0.0f && TCset.UnitsPerISO[r] < 1.0e7f)) TCset.UnitsPerISO[r] = 0.0f;	// also catches NaN
		if (!(TCset.MeterCal[r] > 0.0f && TCset.MeterCal[r] < 16701.0f)) TCset.MeterCal[r] = 0.0f;			// RC's range
		if (TCset.MeterCal[r] > 0.0f) Sensor[r].MeterCal = TCset.MeterCal[r];	// no 32500 comes for an ISOBUS product
		if (TCset.ControlType[r] > TimedCombo_ct) TCset.ControlType[r] = TCnoControlType;
		if (TCset.ControlType[r] != TCnoControlType) Sensor[r].ControlType = TCset.ControlType[r];
		TC_TargetRate[r] = 0.0f;
		TC_UnitsWarned[r] = false;
	}
}

// the bins: the module's sensors, at most TCmaxRates
uint8_t TC_RateCount()
{
	return (MDL.SensorCount < TCmaxRates) ? MDL.SensorCount : TCmaxRates;
}

// the bin (sensor) of an element number, -1 when it isn't a bin
int8_t TC_RateIndex(std::uint16_t elementNumber)
{
	int8_t Result = -1;
	if (elementNumber >= TC_ELEMENT_BIN_BASE && elementNumber < TC_ELEMENT_BIN_BASE + TC_RateCount())
	{
		Result = static_cast<int8_t>(elementNumber - TC_ELEMENT_BIN_BASE);
	}
	return Result;
}

// the setpoint rate DDIs a product can use: 1 volume, 6 mass, 11 count per area
bool TC_IsRateDDI(uint16_t DDI)
{
	return DDI == 1 || DDI == 6 || DDI == 11;
}

// the matching actual rate DDI: 2, 7 or 12
uint16_t TC_ActualRateDDI(uint8_t r)
{
	return TCset.RateDDI[r] + 1;
}

// the rate DDI's raw value <-> litres, kg or seeds per ha
// DDI 1: 0.01 mm3/m2 per bit (1 L/ha = 10000), DDI 6: 1 mg/m2 (1 kg/ha = 100), DDI 11: 0.001 /m2 (1 seed/ha = 0.1)
float TC_ValueToPerHa(uint8_t r, std::int32_t value)
{
	float Result = 0.0f;
	if (value > 0)
	{
		if (TCset.RateDDI[r] == 6) Result = value * 0.01f;
		else if (TCset.RateDDI[r] == 11) Result = value * 10.0f;
		else Result = value * 0.0001f;
	}
	return Result;
}

std::int32_t TC_PerHaToValue(uint8_t r, float perHa)
{
	float Raw;
	if (TCset.RateDDI[r] == 6) Raw = perHa * 100.0f;
	else if (TCset.RateDDI[r] == 11) Raw = perHa * 0.1f;
	else Raw = perHa * 10000.0f;
	if (Raw < 0.0f) Raw = 0.0f;
	if (Raw > 2.0e9f) Raw = 2.0e9f;
	return static_cast<std::int32_t>(Raw + 0.5f);
}

// the bin's application total DDI (bench stand-in, Group2_Step2_Recording_Design.md E5):
// 351 mL for volume, 352 g for mass, 82 count
uint16_t TC_TotalDDI(uint8_t r)
{
	uint16_t Result = static_cast<uint16_t>(isobus::DataDescriptionIndex::ApplicationTotalVolume_ml);
	if (TCset.RateDDI[r] == 6) Result = static_cast<uint16_t>(isobus::DataDescriptionIndex::ApplicationTotalMassGram_g);
	else if (TCset.RateDDI[r] == 11) Result = static_cast<uint16_t>(isobus::DataDescriptionIndex::ApplicationTotalCount);
	return Result;
}

// what the sensor has metered since the module started (as PGN 32400's quantity) in the total's unit:
// RC units / units per litre, kg or seed, x 1000 for mL and g; 0 until RC has sent the units
std::int32_t TC_TotalValue(uint8_t r)
{
	float Result = 0.0f;
	if (r < TC_RateCount() && Sensor[r].MeterCal > 0 && TCset.UnitsPerISO[r] > 0.0f)
	{
		Result = (Sensor[r].TotalPulses / Sensor[r].MeterCal) / TCset.UnitsPerISO[r];
		if (TCset.RateDDI[r] != 11) Result *= 1000.0f;
	}
	if (Result > 2.0e9f) Result = 2.0e9f;
	return static_cast<std::int32_t>(Result + 0.5f);
}

const __FlashStringHelper* TC_RateUnit(uint8_t r)
{
	if (TCset.RateDDI[r] == 6) return F(" kg/ha");
	if (TCset.RateDDI[r] == 11) return F(" seeds/ha");
	return F(" L/ha");
}

// PGN 32509 / CAN 0xFF1D from RC: a product's type and metered units per litre, kg or seed.
// Kept for sensors 0 and 1 (the bins). DDI 0 = the unit has no type: the TC's rate is not applied.
void TC_SetUnits(uint8_t SensorID, uint16_t DDI, float UnitsPerISO)
{
	if (SensorID < TCmaxRates)
	{
		uint8_t r = SensorID;
		float Factor = (TC_IsRateDDI(DDI) && UnitsPerISO > 0.0f && UnitsPerISO < 1.0e7f) ? UnitsPerISO : 0.0f;
		bool NewType = TC_IsRateDDI(DDI) && DDI != TCset.RateDDI[r];
		if (NewType || Factor != TCset.UnitsPerISO[r])
		{
			if (NewType) TCset.RateDDI[r] = DDI;
			TCset.UnitsPerISO[r] = Factor;
			TC_TargetRate[r] = 0.0f;		// a rate in the old type's unit no longer applies
			TC_UnitsWarned[r] = false;

			Serial.print(F("TC: units sensor "));
			Serial.print(r);
			Serial.print(F(", DDI "));
			Serial.print(TCset.RateDDI[r]);
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

// PGN 32509 bytes 9-12 / CAN 0xFF1E from RC: the sensor's meter cal (pulses per metered unit). RC sends
// no 32500 for an ISOBUS product, so this is the only place the measured rate and total get it from.
void TC_SetMeterCal(uint8_t SensorID, float Cal)
{
	if (SensorID < TCmaxRates && Cal > 0.0f && Cal < 16701.0f)
	{
		uint8_t r = SensorID;
		Sensor[r].MeterCal = Cal;
		if (Cal != TCset.MeterCal[r])
		{
			TCset.MeterCal[r] = Cal;
			Serial.print(F("TC: meter cal sensor "));
			Serial.print(r);
			Serial.print(F(", "));
			Serial.println(Cal, 3);
			TC_SaveSettings();
		}
	}
}

// PGN 32509 byte 13 / CAN 0xFF1D byte 7 from RC: the sensor's control type. RC sends no 32500 for an
// ISOBUS product, so without this the sensor runs whatever PID type it booted with (0, valve).
void TC_SetControlType(uint8_t SensorID, uint8_t Type)
{
	if (SensorID < TCmaxRates && Type <= TimedCombo_ct)
	{
		uint8_t r = SensorID;
		Sensor[r].ControlType = Type;
		if (Type != TCset.ControlType[r])
		{
			TCset.ControlType[r] = Type;
			Serial.print(F("TC: control type sensor "));
			Serial.print(r);
			Serial.print(F(", "));
			Serial.println(Type);
			TC_SaveSettings();
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
	for (uint8_t r = 0; r < TCmaxRates; r++) TC_TargetRate[r] = 0.0f;
	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].TargetUPM = 0.0f;
		Sensor[i].ManualAdjust = 0;
		AutoOn[i] = true;		// auto at target 0 applies nothing
		CalibrationOn[i] = false;
		FlowExpected[i] = false;	// set by RC's 32500/0xFF03, which the TC state ignores; TC_Update sets the bins'
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

// keeps the bins' sensors "connected" (RC's 4 s comm-loss rule) while the TC commands
void TC_RefreshCommandTime()
{
	for (uint8_t r = 0; r < TC_RateCount(); r++) Sensor[r].CommTime = millis();
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

// metered units per minute: the bin's rate (litres, kg or seeds per ha) x metered units per ISO unit
// x ha per minute; 0 until RC has sent the units
float TC_TargetUPM(uint8_t r)
{
	float Result = 0.0f;
	float speedKmh = SPEED_GetKmh();
	float widthM = TC_ActiveWidthM();
	if (MasterOn && TC_TargetRate[r] > 0.0f && TCset.UnitsPerISO[r] > 0.0f && speedKmh > 0.05f && widthM > 0.01f)
	{
		Result = (TC_TargetRate[r] * TCset.UnitsPerISO[r] * speedKmh * widthM) / 600.0f;
	}
	return Result;
}

// litres, kg or seeds per ha actually applied by the bin, the inverse of TC_TargetUPM
float TC_ActualRate(uint8_t r)
{
	float Result = 0.0f;
	float speedKmh = SPEED_GetKmh();
	float widthM = TC_ActiveWidthM();
	if (r < TC_RateCount() && TCset.UnitsPerISO[r] > 0.0f && speedKmh > 0.05f && widthM > 0.01f)
	{
		Result = (Sensor[r].UPM * 600.0f) / (speedKmh * widthM * TCset.UnitsPerISO[r]);
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
	if (TC_ProcessDataReady())
	{
		int8_t r = TC_RateIndex(elementNumber);
		if (elementNumber == TC_ELEMENT_BOOM)
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
			else if (DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed))
			{
				value = static_cast<std::int32_t>(SPEED_GetKmh() * 277.7778f);	// mm/s
			}
			else
			{
				Result = false;
			}
		}
		else if (r >= 0)
		{
			Result = true;
			if (DDI == TCset.RateDDI[r])
			{
				value = TC_PerHaToValue(r, TC_TargetRate[r]);
			}
			else if (DDI == TC_ActualRateDDI(r))
			{
				value = TC_PerHaToValue(r, TC_ActualRate(r));
			}
			else if (DDI == TC_TotalDDI(r))
			{
				value = TC_TotalValue(r);
			}
			else
			{
				Result = false;
			}
		}
	}
	return Result;
}

bool TC_ValueCommand(std::uint16_t elementNumber, std::uint16_t DDI, std::int32_t value, void*)
{
	bool Result = false;
	if (ISOBUStc->get_is_connected())
	{
		int8_t r = TC_RateIndex(elementNumber);
		if (elementNumber == TC_ELEMENT_BOOM && DDI == static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16))
		{
			TC_ApplyCondensedWorkState(value);
			Result = true;
		}
		else if (r >= 0 && DDI == TCset.RateDDI[r])
		{
			TC_EnterCommand();
			float NewRate = TC_ValueToPerHa(r, value);
			if (NewRate != TC_TargetRate[r])
			{
				Serial.print(F("TC: rate "));
				Serial.print(r + 1);
				Serial.print(F(": "));
				Serial.print(NewRate, 2);
				Serial.println(TC_RateUnit(r));
			}
			if (TCset.UnitsPerISO[r] <= 0.0f && NewRate > 0.0f && !TC_UnitsWarned[r])
			{
				Serial.print(F("TC: rate "));
				Serial.print(r + 1);
				Serial.println(F(" ignored, units not set in RC"));	// TC_TargetUPM stays 0
				TC_UnitsWarned[r] = true;
			}
			TC_TargetRate[r] = NewRate;
			TC_RefreshCommandTime();
			Result = true;
		}
	}
	return Result;
}

// structure label character of a bin's type: V volume, M mass, C count
char TC_TypeChar(uint8_t r)
{
	return (TCset.RateDDI[r] == 6) ? 'M' : ((TCset.RateDDI[r] == 11) ? 'C' : 'V');
}

std::shared_ptr<isobus::DeviceDescriptorObjectPool> TC_CreateDDOP()
{
	using DeviceElementType = isobus::task_controller_object::DeviceElementObject::Type;
	using PropertiesBit = isobus::task_controller_object::DeviceProcessDataObject::PropertiesBit;
	using Trigger = isobus::task_controller_object::DeviceProcessDataObject::AvailableTriggerMethods;

	const uint8_t Rates = TC_RateCount();
	auto ddop = std::make_shared<isobus::DeviceDescriptorObjectPool>(4);
	std::array<std::uint8_t, isobus::task_controller_object::DeviceObject::MAX_STRUCTURE_AND_LOCALIZATION_LABEL_LENGTH> localization = { 'e','n',0,0,0,0,0xFF };
	// change the structure label whenever the DDOP's structure changes, so a TC reloads it;
	// the last two characters are the bins' types (V volume, M mass, C count, - no bin)
	char Type1 = (Rates > 0) ? TC_TypeChar(0) : '-';
	char Type2 = (Rates > 1) ? TC_TypeChar(1) : '-';
	char structureLabel[8] = { 'R','C','T','C','8', Type1, Type2, 0 };
	std::vector<std::uint8_t> extendedLabel = { 'R','C','T','C','8', static_cast<std::uint8_t>(Type1), static_cast<std::uint8_t>(Type2) };

	ddop->add_device(InoDescription, String(InoID).c_str(), String(MDL.ID).c_str(), structureLabel, localization, extendedLabel, ISOBUSicf->get_NAME().get_full_name());
	ddop->add_device_element("Boom", TC_ELEMENT_BOOM, 0, DeviceElementType::Function, TC_OBJ_BOOM);

	const uint8_t settable = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet) | static_cast<std::uint8_t>(PropertiesBit::Settable);
	const uint8_t measurement = static_cast<std::uint8_t>(PropertiesBit::MemberOfDefaultSet);
	const uint8_t onChange = static_cast<std::uint8_t>(Trigger::OnChange);
	const uint8_t reported = static_cast<std::uint8_t>(Trigger::OnChange) | static_cast<std::uint8_t>(Trigger::TimeInterval) | static_cast<std::uint8_t>(Trigger::Total);

	ddop->add_device_process_data("Set Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::SetpointCondensedWorkState1_16), isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_WORK_STATE);
	ddop->add_device_process_data("Actual Section State", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_WORK_STATE);
	ddop->add_device_process_data("Actual Speed", static_cast<std::uint16_t>(isobus::DataDescriptionIndex::ActualSpeed), isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_ACTUAL_SPEED);

	auto boom = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(TC_OBJ_BOOM));
	if (boom != nullptr)
	{
		boom->add_reference_to_child_object(TC_OBJ_DPD_SETPOINT_WORK_STATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_WORK_STATE);
		boom->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_SPEED);
	}

	// a bin per rate sensor under the boom; rate DDIs follow the product's type from RC:
	// 1/2 volume, 6/7 mass, 11/12 count per area
	for (uint8_t r = 0; r < Rates; r++)
	{
		char binName[8];
		snprintf(binName, sizeof(binName), "Rate %u", r + 1);
		ddop->add_device_element(binName, TC_ELEMENT_BIN_BASE + r, TC_OBJ_BOOM, DeviceElementType::Bin, TC_OBJ_BIN_BASE + r);
		ddop->add_device_process_data("Target Rate", TCset.RateDDI[r], isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_SETPOINT_RATE_BASE + r);
		ddop->add_device_process_data("Actual Rate", TC_ActualRateDDI(r), isobus::NULL_OBJECT_ID, measurement, reported, TC_OBJ_DPD_ACTUAL_RATE_BASE + r);
		ddop->add_device_process_data("Total", TC_TotalDDI(r), isobus::NULL_OBJECT_ID, measurement, reported, TC_OBJ_DPD_TOTAL_BASE + r);

		auto bin = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(TC_OBJ_BIN_BASE + r));
		if (bin != nullptr)
		{
			bin->add_reference_to_child_object(TC_OBJ_DPD_SETPOINT_RATE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_RATE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_TOTAL_BASE + r);
		}
	}

	ddop->add_device_value_presentation("m", 0, 0.001f, 2, TC_OBJ_DVP_WIDTH);

	for (uint8_t i = 0; i < TCset.SectionCount; i++)
	{
		uint16_t sectionObjectID = TC_OBJ_SECTION_BASE + i;
		uint16_t widthPropertyID = TC_OBJ_DPT_WIDTH_BASE + i;
		char name[12];
		snprintf(name, sizeof(name), "Sec %02u", i + 1);

		// element numbers are unique in the DDOP: the device is 0, the boom 1, sections from 2, bins from 20
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
		// each bin's rate becomes its sensor's target as speed and active width change
		TC_RefreshCommandTime();
		for (uint8_t r = 0; r < TC_RateCount(); r++)
		{
			Sensor[r].TargetUPM = TC_TargetUPM(r);
			FlowExpected[r] = (RelayLo || RelayHi);	// RC's flow-path rule: a section is on
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
		for (uint8_t r = 0; r < TC_RateCount(); r++)
		{
			ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BIN_BASE + r, TC_ActualRateDDI(r));
			ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BIN_BASE + r, TC_TotalDDI(r));
		}
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
