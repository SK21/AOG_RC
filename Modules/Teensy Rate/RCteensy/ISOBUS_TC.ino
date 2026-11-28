// ISOBUS_TC.ino - Task Controller client (ISOBUS build only)
// From jaworeA's RC11-isobus. The module uploads a DDOP (one boom with its sections, a bin per
// rate sensor, rate and speed process data) to a TC: a tractor terminal or AOG-TaskController.
// While the TC commands, it switches the sections (setpoint condensed work state, DDI 290; the
// module reports the actual state, DDI 161) and sets each bin's rate, which becomes that
// sensor's TargetUPM from speed and active width.
//
// Bins: one per sensor the module has, up to MaxProductCount (TCmaxRates), each a bin element
// under the boom ("Rate 1" ... "Rate 6") with its own target and actual rate DDIs, so a TC can
// give each its own product (Group2_Step1_Channels_Design.md D3). All share the boom's sections.
//
// Control mode (TCset.ControlMode, Group3_Step2_Rate_Source_Design.md § 2):
// - RC app (0, default): command authority as below.
// - Terminal (1): RC's rate and relay commands are always ignored (config, cal, telemetry and
//   firmware update still accepted). The board drives its products itself, with or without a TC:
//   master from the operator (serial M, later the VT), sections only from the TC, each product's
//   rate from the rate rule below. TC lost: sections cleared, the board stays in Terminal.
//   Changed only with master off; a change clears targets, master and sections.
//
// Rate rule (both modes, § 3): TC rate while prescription control is on (DDI 158 = 1), else the
// operator's rate for the product (TCset.OperatorRate, per ha). 158 is reported 1 only when the TC
// asked for 1 and the operator hasn't turned VR off (TCset.VrOff, D2). The operator rate is also
// the bin's DDOP default rate.
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
// - run screen on the VT in ISOBUS_VT.ino (display only, Group3_Step1_VT_Run_Screen_Design.md);
//   no manual mode from the terminal yet

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
const uint8_t TCmaxRates = MaxProductCount;	// bins: one per sensor, up to 6
const int TCsettingsAddr = 1024;		// after the sensors (253 + 6 x 124 = 997); nothing after it
const uint16_t TCsettingsID = 0x5448;	// change when TCsettings changes shape (0x5444: one rate; 0x5445: no meter cal;
										// 0x5446: no control type; 0x5447: 2 bins, no operator rate / VR / mode)
const uint8_t TCnoControlType = 255;	// TCsettings.ControlType not set by RC yet
const uint8_t TC_MODE_RC_APP = 0;
const uint8_t TC_MODE_TERMINAL = 1;
const float TC_MaxOperatorRate = 1.0e7f;	// per ha (seeds per ha reach millions)

// Prescription control (DDI 158 / 287 / 288) and the default rate per bin (Group2_Step3_Prescription_State_Design.md,
// Group3_Step2_Rate_Source_Design.md § 3). 0: build without them (Group 2 step 3 test 10, legacy implement: the TC's rate
// always applies while it commands).
#define TC_RX_DDIS 1
const uint16_t DDI_RX_STATE = 158;			// Prescription Control State: 0 off, 1 on, 2 error, 3 not installed
const uint16_t DDI_RX_SOURCE = 287;			// Setpoint Prescription Mode: 1 TC rate, 2 default rate ...
const uint16_t DDI_RX_MODE = 288;			// Actual Prescription Mode: 1 TC rate, 2 manual entry ...
const uint16_t TC_OBJ_DPD_RX_STATE_BASE = 1040;
const uint16_t TC_OBJ_DPD_RX_SOURCE_BASE = 1050;
const uint16_t TC_OBJ_DPD_RX_MODE_BASE = 1060;
const uint16_t TC_OBJ_DPT_DEFAULT_BASE = 3200;
uint8_t TC_Rx[TCmaxRates];					// 158 as reported per bin: TC asked for 1 and VR not off; 0 at boot and when the TC is lost
bool TC_RxRequest[TCmaxRates];				// the TC's last 158 command was 1
int32_t TC_RxSource[TCmaxRates];			// 287 as the TC set it
bool TC_RateIgnoredLogged[TCmaxRates];
bool TC_DefaultOn = true;					// serial d (bench tool): the default rate property in the DDOP
bool TC_DefaultChanged = false;				// operator rate changed: DDOP re-upload once master is off

// ---- bench tools (Group 2 step 3 tests), serial r / x / d ----
bool TC_RefuseNext[TCmaxRates];				// serial r<bin>: answer the next 158 = 1 with 0
uint8_t TC_IgnoreCount[TCmaxRates];			// serial x<bin> [n]: ignore the next n 158 commands completely
uint32_t TC_IgnoreRequestUntil[TCmaxRates];	// ...and no answer to 158 requests until then (millis), so the TC's
											// confirm request 1 s after an ignored command gets none. A window, not a one-shot:
											// the stack also polls this callback every loop for an on-change subscription
const uint32_t TC_IgnoreRequestMs = 1500;
// ---- end bench tools ----

struct TCsettings
{
	uint16_t Identifier = TCsettingsID;
	uint8_t SectionCount = 8;
	uint16_t SectionWidthCm[TCmaxSections] = { 50,50,50,50,50,50,50,50,50,50,50,50,50,50,50,50 };
	uint16_t RateDDI[TCmaxRates] = { 1, 1, 1, 1, 1, 1 };	// per sensor, setpoint rate DDI: 1 volume, 6 mass, 11 count per area (from RC)
	float UnitsPerISO[TCmaxRates] = { 0.0f };		// per sensor, metered units per litre, kg or seed (from RC); 0 = not set
	float MeterCal[TCmaxRates] = { 0.0f };			// per sensor, pulses per metered unit (from RC's 32509, which an ISOBUS product
													// gets instead of 32500); 0 = not set
	uint8_t ControlType[TCmaxRates] = { TCnoControlType, TCnoControlType, TCnoControlType,
		TCnoControlType, TCnoControlType, TCnoControlType };	// per sensor, RC's control type (32509 byte 13,
													// which an ISOBUS product gets instead of 32500): 0 valve ... 5 timed combo
	float OperatorRate[TCmaxRates] = { 0.0f };		// per sensor, the operator's rate per ha; 0 = nothing applies until set
	uint8_t VrOff[TCmaxRates] = { 0 };				// per sensor, 1 = the operator turned VR off (158 stays 0)
	uint8_t ControlMode = TC_MODE_RC_APP;			// TC_MODE_RC_APP or TC_MODE_TERMINAL
};

TCsettings TCset;
float TC_TargetRate[TCmaxRates];		// from the TC, litres, kg or seeds per ha; RAM only
bool TC_UnitsWarned[TCmaxRates];		// "units not set" printed once
byte TC_SectionLo = 0;					// Terminal mode: the TC's sections, driven while master is on
byte TC_SectionHi = 0;

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
		if (!(TCset.OperatorRate[r] >= 0.0f && TCset.OperatorRate[r] <= TC_MaxOperatorRate)) TCset.OperatorRate[r] = 0.0f;
		if (TCset.VrOff[r] > 1) TCset.VrOff[r] = 0;
		TC_TargetRate[r] = 0.0f;
		TC_UnitsWarned[r] = false;
		TC_Rx[r] = 0;
		TC_RxRequest[r] = false;
		TC_RxSource[r] = 0;
		TC_RateIgnoredLogged[r] = false;
		TC_RefuseNext[r] = false;
		TC_IgnoreCount[r] = 0;
		TC_IgnoreRequestUntil[r] = 0;
	}
	if (TCset.ControlMode > TC_MODE_TERMINAL) TCset.ControlMode = TC_MODE_RC_APP;
}

bool TC_Terminal()
{
	return TCset.ControlMode == TC_MODE_TERMINAL;
}

// the board's ISOBUS side sets the products' targets: the TC commands, or Terminal mode
bool TC_DrivesProducts()
{
	return TC_InCommand || TC_Terminal();
}

// RC's rate and relay commands (0xFF03/0xFF04, UDP 32500/32501) are ignored: the TC commands, or Terminal mode
bool RC_RateCommandsIgnored()
{
	return TC_InCommand || TC_Terminal();
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
// Kept for sensors 0-5 (the bins). DDI 0 = the unit has no type: the TC's rate is not applied.
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

// every target, master and the sections cleared (the products' outputs), the TC's process data kept
void TC_ClearOutputs()
{
	RelayLo = 0;
	RelayHi = 0;
	TC_SectionLo = 0;
	TC_SectionHi = 0;
	MasterOn = false;
	for (uint8_t i = 0; i < MDL.SensorCount; i++)
	{
		Sensor[i].TargetUPM = 0.0f;
		Sensor[i].ManualAdjust = 0;
		AutoOn[i] = true;		// auto at target 0 applies nothing
		CalibrationOn[i] = false;
		FlowExpected[i] = false;	// set by RC's 32500/0xFF03, which the TC state ignores; TC_Update sets the bins'
	}
}

// every target, master and the sections cleared: no command from the old source survives
void TC_ClearCommands()
{
	TC_ClearOutputs();
	for (uint8_t r = 0; r < TCmaxRates; r++)
	{
		TC_TargetRate[r] = 0.0f;
		// prescription control back to off with the TC
		TC_Rx[r] = 0;
		TC_RxRequest[r] = false;
		TC_RxSource[r] = 0;
		TC_RefuseNext[r] = false;
		TC_IgnoreCount[r] = 0;
		TC_IgnoreRequestUntil[r] = 0;
		TC_RateIgnoredLogged[r] = false;
	}
}

// TC lost in Terminal mode: its sections and process data go, the operator's master stays
void TC_ClearTCState()
{
	bool Master = MasterOn;
	TC_ClearCommands();
	MasterOn = Master;
}

// the TC's first value command: authority RC -> TC (Terminal mode: the TC joins, nothing from RC to clear)
void TC_EnterCommand()
{
	if (!TC_InCommand)
	{
		if (TC_Terminal())
		{
			TC_ClearTCState();
			TC_InCommand = true;
			Serial.println(F("TC: commanding (Terminal mode)."));
		}
		else
		{
			TC_ClearCommands();
			TC_InCommand = true;
			Serial.println(F("TC: commanding, RC rate and relay commands ignored."));
		}
	}
}

// TC lost: RC app mode, authority TC -> RC, in RC's comm-loss state until RC's next command;
// Terminal mode, sections cleared, the board stays in Terminal (no RC comm-loss state)
void TC_LeaveCommand()
{
	if (TC_InCommand)
	{
		TC_InCommand = false;
		if (TC_Terminal())
		{
			TC_ClearTCState();
			Serial.println(F("TC: stopped commanding, sections off (Terminal mode)."));
		}
		else
		{
			TC_ClearCommands();
			for (uint8_t i = 0; i < MDL.SensorCount; i++)
			{
				Sensor[i].CommTime = millis() - 5000;	// older than the 4 s comm-loss limit
			}
			Serial.println(F("TC: stopped commanding, back to RC."));
		}
	}
}

// keeps the bins' sensors "connected" (RC's 4 s comm-loss rule) while the board drives them
void TC_RefreshCommandTime()
{
	for (uint8_t r = 0; r < TC_RateCount(); r++) Sensor[r].CommTime = millis();
}

// control mode RC app <-> Terminal; refused with master on. Clears targets, master and sections.
bool TC_SetControlMode(uint8_t Mode)
{
	bool Result = false;
	if (Mode > TC_MODE_TERMINAL)
	{
		Serial.println(F("TC: unknown control mode"));
	}
	else if (MasterOn)
	{
		Serial.println(F("TC: control mode not changed, turn master off first"));
	}
	else if (Mode != TCset.ControlMode)
	{
		TC_ClearOutputs();
		TCset.ControlMode = Mode;
		TC_SaveSettings();
		if (!TC_Terminal() && !TC_InCommand)
		{
			// back to RC app: RC's comm-loss state until RC's next command
			for (uint8_t i = 0; i < MDL.SensorCount; i++) Sensor[i].CommTime = millis() - 5000;
		}
		Serial.println(TC_Terminal() ? F("TC: control mode Terminal, RC rate and relay commands ignored")
			: F("TC: control mode RC app"));
		Result = true;
	}
	else
	{
		Result = true;
	}
	return Result;
}

// the operator's master (Terminal mode only; in RC app mode RC or the TC's sections set it)
void TC_SetMaster(bool On)
{
	if (TC_Terminal())
	{
		MasterOn = On;
		Serial.println(On ? F("TC: master on") : F("TC: master off"));
	}
	else
	{
		Serial.println(F("TC: master comes from RC or the TC in RC app mode"));
	}
}

// the operator's rate for a product, per ha; also the DDOP default rate (re-uploaded once master is off)
void TC_SetOperatorRate(uint8_t r, float Rate)
{
	if (r < TCmaxRates && Rate >= 0.0f && Rate <= TC_MaxOperatorRate)
	{
		if (Rate != TCset.OperatorRate[r])
		{
			TCset.OperatorRate[r] = Rate;
			TC_SaveSettings();
			TC_DefaultChanged = true;
		}
		Serial.print(F("TC: operator rate "));
		Serial.print(r + 1);
		Serial.print(F(": "));
		Serial.print(Rate, 2);
		Serial.println(TC_RateUnit(r));
	}
	else
	{
		Serial.println(F("TC: operator rate out of range"));
	}
}

#if TC_RX_DDIS
// 158 as reported: the TC asked for 1 and the operator hasn't turned VR off. A change is reported.
// Answer = called for the TC's own 158 command.
void TC_UpdateRx(uint8_t r, bool Answer)
{
	uint8_t NewRx = (TC_RxRequest[r] && !TCset.VrOff[r]) ? 1 : 0;
	bool Changed = NewRx != TC_Rx[r];
	// Q-a: on the TC's own 158 = 1 the setpoint resets and the TC sends it straight after. When the operator turns
	// VR back on the TC sends nothing (AOG-TC, bench 2026-11-27), so the TC's latest rate, kept while 158 = 0, applies.
	if (Answer && NewRx == 1 && TC_Rx[r] != 1) TC_TargetRate[r] = 0.0f;
	TC_Rx[r] = NewRx;
	if (Changed)
	{
		Serial.print(F("TC: bin "));
		Serial.print(r);
		Serial.print(F(" 158 = "));
		Serial.println(NewRx);
	}
	if ((Changed || Answer) && ISOBUStc->get_is_connected())
	{
		ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BIN_BASE + r, DDI_RX_STATE);	// reported even when unchanged: an answer
		ISOBUStc->on_value_changed_trigger(TC_ELEMENT_BIN_BASE + r, DDI_RX_MODE);
	}
}

// the operator's VR choice for a product: off holds 158 at 0 (the TC's 158 = 1 refused, D2)
void TC_SetVrOff(uint8_t r, bool Off)
{
	if (r < TCmaxRates)
	{
		if ((Off ? 1 : 0) != TCset.VrOff[r])
		{
			TCset.VrOff[r] = Off ? 1 : 0;
			TC_SaveSettings();
		}
		Serial.print(F("TC: bin "));
		Serial.print(r);
		Serial.println(Off ? F(" VR off (operator rate)") : F(" VR on"));
		TC_UpdateRx(r, false);
	}
}
#endif

// the rate the bin applies, per ha: the TC's while it commands with 158 = 1, else the operator's
float TC_EffectiveRate(uint8_t r)
{
	float Result = TCset.OperatorRate[r];
#if TC_RX_DDIS
	if (TC_InCommand && TC_Rx[r] == 1) Result = TC_TargetRate[r];
#else
	if (TC_InCommand) Result = TC_TargetRate[r];	// legacy implement: the TC's rate always
#endif
	return Result;
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
	float Rate = TC_EffectiveRate(r);
	if (MasterOn && Rate > 0.0f && TCset.UnitsPerISO[r] > 0.0f && speedKmh > 0.05f && widthM > 0.01f)
	{
		Result = (Rate * TCset.UnitsPerISO[r] * speedKmh * widthM) / 600.0f;
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
	if (TC_Terminal())
	{
		// the operator's master gates the TC's sections (TC_Update)
		TC_SectionLo = Lo;
		TC_SectionHi = Hi;
		RelayLo = MasterOn ? Lo : 0;
		RelayHi = MasterOn ? Hi : 0;
	}
	else
	{
		RelayLo = Lo;
		RelayHi = Hi;
		MasterOn = (RelayLo > 0 || RelayHi > 0);
	}
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
#if TC_RX_DDIS
			else if (DDI == DDI_RX_STATE)
			{
				// bench tool: x<bin> also swallows the requests in the window after an ignored command
				Result = !(TC_IgnoreRequestUntil[r] != 0 && static_cast<int32_t>(millis() - TC_IgnoreRequestUntil[r]) < 0);
				value = TC_Rx[r];
			}
			else if (DDI == DDI_RX_SOURCE)
			{
				value = TC_RxSource[r];
			}
			else if (DDI == DDI_RX_MODE)
			{
				value = (TC_Rx[r] == 1) ? 1 : 2;	// 1 TC rate, 2 manual entry
			}
#endif
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
#if TC_RX_DDIS
		else if (r >= 0 && DDI == DDI_RX_STATE)
		{
			// prescription control state from the TC
			TC_EnterCommand();
			Result = true;
			if (TC_IgnoreCount[r] > 0)
			{
				TC_IgnoreCount[r]--;
				TC_IgnoreRequestUntil[r] = millis() + TC_IgnoreRequestMs;	// no change, no report, no answer to the request that follows
				if (TC_IgnoreRequestUntil[r] == 0) TC_IgnoreRequestUntil[r] = 1;	// 0 = no window
				Serial.print(F("TC: bin "));
				Serial.print(r);
				Serial.print(F(" 158 = "));
				Serial.print(value);
				Serial.println(F(" ignored (x)"));
			}
			else
			{
				bool Request = (value == 1);
				if (Request && TC_RefuseNext[r])
				{
					TC_RefuseNext[r] = false;
					Request = false;
					Serial.print(F("TC: bin "));
					Serial.print(r);
					Serial.println(F(" refuses 158 = 1 (r)"));
				}
				if (Request && TCset.VrOff[r])
				{
					Serial.print(F("TC: bin "));
					Serial.print(r);
					Serial.println(F(" refuses 158 = 1, VR off by the operator"));
				}
				TC_RxRequest[r] = Request;
				TC_UpdateRx(r, true);
			}
		}
		else if (r >= 0 && DDI == DDI_RX_SOURCE)
		{
			TC_EnterCommand();
			TC_RxSource[r] = value;
			Serial.print(F("TC: bin "));
			Serial.print(r);
			Serial.print(F(" 287 = "));
			Serial.println(value);
			Result = true;
		}
		else if (r >= 0 && DDI == TCset.RateDDI[r] && TC_Rx[r] != 1)
		{
			// a strict implement ignores rate commands while prescription control is off; the rate is kept
			// (not applied) so an operator VR on goes straight to the TC's latest rate
			TC_EnterCommand();
			TC_TargetRate[r] = TC_ValueToPerHa(r, value);
			if (!TC_RateIgnoredLogged[r])
			{
				Serial.print(F("TC: rate "));
				Serial.print(r + 1);
				Serial.println(F(" ignored, 158 = 0 (operator rate)"));
				TC_RateIgnoredLogged[r] = true;
			}
			Result = true;
		}
#endif
		else if (r >= 0 && DDI == TCset.RateDDI[r])
		{
			TC_EnterCommand();
			TC_RateIgnoredLogged[r] = false;
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
	// change the structure label whenever the DDOP changes (ISO 11783-10; AgIsoStack logs an error and a TC may
	// keep its stored copy otherwise): "RC" + 5 base-36 characters from the bins' types, the bin count, the
	// default rate flag, the 158 / 287 / 288 flag and a checksum of the default rates (the operator rates, as
	// sent). Same content, same label. Extended label: the same 7 characters + one type letter per bin
	// (V volume, M mass, C count), readable in a TC's log.
	const uint32_t RateChecksums = 2958;	// 20412 structure codes x 2958 < 36^5
	uint32_t Checksum = 0;
#if TC_RX_DDIS
	if (TC_DefaultOn)
	{
		uint32_t Hash = 2166136261UL;		// FNV-1a over the default rates' raw values
		for (uint8_t r = 0; r < Rates; r++)
		{
			uint32_t Value = static_cast<uint32_t>(TC_PerHaToValue(r, TCset.OperatorRate[r]));
			for (uint8_t b = 0; b < 4; b++)
			{
				Hash ^= (Value >> (b * 8)) & 0xFF;
				Hash *= 16777619UL;
			}
		}
		Checksum = Hash % RateChecksums;
	}
#endif
	uint32_t Code = 0;
	for (uint8_t r = 0; r < TCmaxRates; r++)
	{
		uint8_t Type = 0;
		if (r < Rates) Type = (TCset.RateDDI[r] == 6) ? 1 : ((TCset.RateDDI[r] == 11) ? 2 : 0);
		Code = Code * 3 + Type;
	}
	Code = Code * (TCmaxRates + 1) + Rates;
#if TC_RX_DDIS
	Code = Code * 2 + (TC_DefaultOn ? 1 : 0);
	Code = Code * 2 + 1;
#else
	Code = Code * 2;
	Code = Code * 2;
#endif
	Code = Code * RateChecksums + Checksum;
	const char Base36[] = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	char structureLabel[8] = { 'R','C',0,0,0,0,0,0 };
	for (int8_t i = 6; i >= 2; i--)
	{
		structureLabel[i] = Base36[Code % 36];
		Code /= 36;
	}
	std::vector<std::uint8_t> extendedLabel(structureLabel, structureLabel + 7);
	for (uint8_t r = 0; r < Rates; r++) extendedLabel.push_back(static_cast<std::uint8_t>(TC_TypeChar(r)));
	Serial.print(F("TC: DDOP label "));
	Serial.println(structureLabel);

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

#if TC_RX_DDIS
		// prescription control and the default rate (3 / 8 / 13, the rate DDI + 2) = the operator's rate
		ddop->add_device_process_data("Rx Control State", DDI_RX_STATE, isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_RX_STATE_BASE + r);
		ddop->add_device_process_data("Rx Source", DDI_RX_SOURCE, isobus::NULL_OBJECT_ID, settable, onChange, TC_OBJ_DPD_RX_SOURCE_BASE + r);
		ddop->add_device_process_data("Rx Mode", DDI_RX_MODE, isobus::NULL_OBJECT_ID, measurement, onChange, TC_OBJ_DPD_RX_MODE_BASE + r);
		if (TC_DefaultOn)
		{
			ddop->add_device_property("Default Rate", TC_PerHaToValue(r, TCset.OperatorRate[r]), TCset.RateDDI[r] + 2, isobus::NULL_OBJECT_ID, TC_OBJ_DPT_DEFAULT_BASE + r);
		}
#endif

		auto bin = std::static_pointer_cast<isobus::task_controller_object::DeviceElementObject>(ddop->get_object_by_id(TC_OBJ_BIN_BASE + r));
		if (bin != nullptr)
		{
			bin->add_reference_to_child_object(TC_OBJ_DPD_SETPOINT_RATE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_ACTUAL_RATE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_TOTAL_BASE + r);
#if TC_RX_DDIS
			bin->add_reference_to_child_object(TC_OBJ_DPD_RX_STATE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_RX_SOURCE_BASE + r);
			bin->add_reference_to_child_object(TC_OBJ_DPD_RX_MODE_BASE + r);
			if (TC_DefaultOn) bin->add_reference_to_child_object(TC_OBJ_DPT_DEFAULT_BASE + r);
#endif
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

	// No VT partner, also with ISOBUS_VT: for a TC below version 4 AgIsoStack then asks that VT for the language
	// command, and with no VT on the bus the request can't be sent and the TC client resets every 6 s (bench
	// 2026-11-27). nullptr asks globally, which any terminal answers.
	ISOBUStc = std::make_shared<isobus::TaskControllerClient>(partnerTC, ISOBUSicf, nullptr);
	ISOBUStc->configure(TC_CreateDDOP(), 1, TCset.SectionCount, 0, true, false, false, false, true);
	ISOBUStc->add_request_value_callback(TC_RequestValue, nullptr);
	ISOBUStc->add_value_command_callback(TC_ValueCommand, nullptr);
	ISOBUStc->initialize(false);
}

void TC_PrintStatus()
{
	Serial.print(F("TC status: mode "));
	Serial.print(TC_Terminal() ? F("Terminal") : F("RC app"));
	Serial.print(F(", master "));
	Serial.print(MasterOn ? F("on") : F("off"));
	Serial.print(F(", TC "));
	Serial.print(ISOBUStc->get_is_connected() ? (TC_InCommand ? F("commanding") : F("connected")) : F("not connected"));
	Serial.print(F(", sections 0x"));
	Serial.print(RelayHi, HEX);
	Serial.print(F(" "));
	Serial.print(RelayLo, HEX);
	Serial.print(F(", width "));
	Serial.print(TC_ActiveWidthM(), 2);
	Serial.print(F(" m, speed "));
	Serial.print(SPEED_GetKmh(), 1);
	Serial.println(F(" km/h"));
	for (uint8_t r = 0; r < TC_RateCount(); r++)
	{
		Serial.print(F("  bin "));
		Serial.print(r);
		Serial.print(F(": operator "));
		Serial.print(TCset.OperatorRate[r], 2);
		Serial.print(F(", TC "));
		Serial.print(TC_TargetRate[r], 2);
		Serial.print(F(", applied "));
		Serial.print(TC_EffectiveRate(r), 2);
		Serial.print(TC_RateUnit(r));
#if TC_RX_DDIS
		Serial.print(F(", VR "));
		Serial.print(TCset.VrOff[r] ? F("off") : F("on"));
		Serial.print(F(", TC asks "));
		Serial.print(TC_RxRequest[r] ? 1 : 0);
		Serial.print(F(", 158 = "));
		Serial.print(TC_Rx[r]);
#endif
		Serial.print(F(", target "));
		Serial.print(Sensor[r].TargetUPM, 3);
		Serial.print(F(" UPM, actual "));
		Serial.print(Sensor[r].UPM, 3);
		Serial.println((TCset.UnitsPerISO[r] > 0.0f) ? F("") : F(", units not set"));
	}
}

// Serial commands, stand-ins for the VT keys until batch 2a-2, then service tools:
//   t           control mode RC app <-> Terminal (master off only)
//   M           master on / off (Terminal mode)
//   o<bin> <r>  operator rate for the bin, per ha (e.g. o0 120)
//   v<bin>      the operator's VR on / off for the bin (off: 158 stays 0, operator rate)
//   ?           status
// Bench tools for the Group 2 step 3 tests (TC_RX_DDIS):
//   r<bin>      refuse the next 158 = 1 (answer 0)
//   x<bin> [n]  ignore the next n 158 commands (default 1): no change, no report, no answer to the request after
//   d           default rate property in the DDOP on / off (DDOP re-uploaded)
// <bin> is 0-5 (Rate 1 - Rate 6). One command per line.
void TC_SerialCommands()
{
	static char Line[16];
	static uint8_t Len = 0;
	while (Serial.available())
	{
		char c = (char)Serial.read();
		if (c == '\r' || c == '\n')
		{
			Line[Len] = 0;
			if (Len > 0)
			{
				char Cmd = Line[0];
				int Bin = (Len > 1 && Line[1] >= '0' && Line[1] <= '9') ? Line[1] - '0' : -1;
				bool BinOk = Bin >= 0 && Bin < TC_RateCount();
				if (Cmd == 't')
				{
					TC_SetControlMode(TC_Terminal() ? TC_MODE_RC_APP : TC_MODE_TERMINAL);
				}
				else if (Cmd == 'M')
				{
					TC_SetMaster(!MasterOn);
				}
				else if (Cmd == 'o' && BinOk)
				{
					TC_SetOperatorRate(Bin, (Len > 2) ? atof(Line + 2) : -1.0f);
				}
				else if (Cmd == '?')
				{
					TC_PrintStatus();
				}
#if TC_RX_DDIS
				else if (Cmd == 'v' && BinOk)
				{
					TC_SetVrOff(Bin, !TCset.VrOff[Bin]);
				}
				else if (Cmd == 'd')
				{
					TC_DefaultOn = !TC_DefaultOn;
					TC_DDOPNeedsReupload = true;
					Serial.println(TC_DefaultOn ? F("TC bench: default rate on, DDOP re-upload") : F("TC bench: default rate off, DDOP re-upload"));
				}
				else if (Cmd == 'r' && BinOk)
				{
					TC_RefuseNext[Bin] = true;
					Serial.print(F("TC bench: bin "));
					Serial.print(Bin);
					Serial.println(F(" refuses the next 158 = 1"));
				}
				else if (Cmd == 'x' && BinOk)
				{
					int n = (Len > 2) ? atoi(Line + 2) : 0;
					if (n < 1) n = 1;
					if (n > 9) n = 9;
					TC_IgnoreCount[Bin] = (uint8_t)n;
					Serial.print(F("TC bench: bin "));
					Serial.print(Bin);
					Serial.print(F(" ignores the next "));
					Serial.print(n);
					Serial.println(F(" 158 command(s)"));
				}
#endif
				else
				{
#if TC_RX_DDIS
					Serial.println(F("TC: t, M, o<bin> <rate>, v<bin>, ?; bench r<bin>, x<bin> [n], d"));
#else
					Serial.println(F("TC: t, M, o<bin> <rate>, ?"));
#endif
				}
			}
			Len = 0;
		}
		else if (Len < sizeof(Line) - 1)
		{
			Line[Len++] = c;
		}
	}
}

void TC_Update()
{
	TC_SerialCommands();
	TC_UpdateConnectionState();
	ISOBUStc->update();
	TC_UpdateConnectionState();

	if (!ISOBUStc->get_is_connected())
	{
		TC_LeaveCommand();		// does nothing unless the TC was commanding
	}

	if (TC_Terminal())
	{
		// the operator's master gates the TC's sections
		RelayLo = MasterOn ? TC_SectionLo : 0;
		RelayHi = MasterOn ? TC_SectionHi : 0;
	}

	if (TC_DrivesProducts())
	{
		// each bin's rate becomes its sensor's target as speed and active width change
		TC_RefreshCommandTime();
		for (uint8_t r = 0; r < TC_RateCount(); r++)
		{
			Sensor[r].TargetUPM = TC_TargetUPM(r);
			FlowExpected[r] = (RelayLo || RelayHi);	// RC's flow-path rule: a section is on
		}
	}

#if TC_RX_DDIS
	// an operator rate change goes into the DDOP's default rate, re-uploaded only while master is off
	if (TC_DefaultChanged && !MasterOn)
	{
		TC_DefaultChanged = false;
		if (TC_DefaultOn) TC_DDOPNeedsReupload = true;
	}
#endif

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
