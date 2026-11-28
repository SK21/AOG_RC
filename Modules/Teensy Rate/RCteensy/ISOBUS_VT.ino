// ISOBUS_VT.ino - run screen and controls on a virtual terminal (ISOBUS build with ISOBUS_VT only)
// Group 3 step 1 (IsoBus\Group3_Step1_VT_Run_Screen_Design.md): the run screen. Per product:
// target, actual, unit and state; the sections; speed, width, the TC link and the command source
// (RC / TC / Term / None). No RC link line: RC sends no rate commands to an ISOBUS product's board
// (it goes through AOG-TaskController), and Source already shows RC when RC drives the board.
// Group 3 step 2a-2 (Group3_Step2_Rate_Source_Design.md § 14): soft keys and pages.
// - Run: keys Master, VR (all products), Setup; header master and VR state. Each product name is a
//   button (the VT's own border): touch it, or on a VT without touch move the VT's focus to it and
//   press Enter, to open that product's page.
// - Product page (selected product): values, rate source, operator rate input, VR, auto / manual
//   and the manual PWM; keys VR, Auto/Man, +, -, Back.
// - Setup: control mode, firmware, module, description; keys Mode, Back. Mode opens a Yes / No
//   confirm page.
// The keys call ISOBUS_TC.ino (TC_SetMaster, TC_SetVrOff, TC_SetAuto, TC_SetManualPWM,
// TC_SetOperatorRate, TC_SetControlMode). Master, auto / manual and + / - work in Terminal mode
// only (RC sets them in RC app mode); a refused key shows a footer message for 3 s.
//
// The object pool is built in code at start-up (ISO 11783-6 binary records: object ID, type,
// attributes), the same method as AOG-TaskController's rate source page (assign_page.cpp). All 6
// rows and 16 sections are always in the pool; rows past the sensor count and sections past the
// section count are hidden at run time, so the pool only changes with the firmware. Its 7
// character version label ("RC" + 5 hex digits of the pool's CRC) lets the VT store it, and works
// with VT versions 3 and 4 as well as 5+. Layout is in 480 x 480 units; the client scales it.
//
// Updates: nothing is sent before the client is connected. After each connect every value goes
// once; then a scan every 500 ms sends only what changed. Numbers, section colours and hide/show
// are single frames (at most VT_SCAN_BUDGET per scan). Text longer than 3 characters needs a
// transport session, and only one can run to the VT at a time, so text goes one string every
// VT_STRING_GAP_MS. A send that fails is not marked as sent and is tried again later.
//
// Not here yet: the tank (step 2b), imperial units, the address claim re-request
// (AOG-TaskController bdf83e4; only if bench test 4 shows the race).

#if ISOBUS_ENABLED && ISOBUS_VT

// ---- ISO 11783-6 object types ----
const uint8_t VT_TYPE_WORKING_SET = 0;
const uint8_t VT_TYPE_DATA_MASK = 1;
const uint8_t VT_TYPE_CONTAINER = 3;
const uint8_t VT_TYPE_SOFT_KEY_MASK = 4;
const uint8_t VT_TYPE_KEY = 5;
const uint8_t VT_TYPE_BUTTON = 6;
const uint8_t VT_TYPE_INPUT_NUMBER = 9;
const uint8_t VT_TYPE_OUTPUT_STRING = 11;
const uint8_t VT_TYPE_OUTPUT_NUMBER = 12;
const uint8_t VT_TYPE_OUTPUT_RECTANGLE = 14;
const uint8_t VT_TYPE_NUMBER_VARIABLE = 21;
const uint8_t VT_TYPE_FONT_ATTRIBUTES = 23;
const uint8_t VT_TYPE_LINE_ATTRIBUTES = 24;
const uint8_t VT_TYPE_FILL_ATTRIBUTES = 25;

// ---- colours (VT standard palette), fonts, justification ----
const uint8_t VT_BLACK = 0;
const uint8_t VT_WHITE = 1;			// input fields
const uint8_t VT_SILVER = 7;		// mask background, as AOG-TaskController's pool
const uint8_t VT_GREY = 8;			// section off
const uint8_t VT_LIME = 10;			// section on
const uint8_t VT_FONT_8X12 = 2;
const uint8_t VT_FONT_12X16 = 3;
const uint8_t VT_FONT_16X24 = 5;
const uint8_t VT_LEFT_MIDDLE = 0x04;
const uint8_t VT_CENTRE_MIDDLE = 0x05;
const uint8_t VT_RIGHT_MIDDLE = 0x06;
const uint8_t VT_TRANSPARENT = 0x01;	// output string / number option bit 0

// ---- object IDs ----
const uint16_t VT_ID_WORKING_SET = 0;
const uint16_t VT_ID_DESIGNATOR = 1;		// "RC <module ID>" in the VT's working set list
const uint16_t VT_ID_FONT_NORMAL = 100;		// 12 x 16
const uint16_t VT_ID_FONT_LARGE = 101;		// 16 x 24
const uint16_t VT_ID_FONT_SMALL = 102;		// 8 x 12
const uint16_t VT_ID_LINE_BLACK = 110;
const uint16_t VT_ID_MASK = 1000;			// run screen
const uint16_t VT_ID_MASK_PRODUCT = 1001;
const uint16_t VT_ID_MASK_SETUP = 1002;
const uint16_t VT_ID_MASK_CONFIRM = 1003;
const uint16_t VT_ID_SKM_RUN = 1100;		// soft key masks
const uint16_t VT_ID_SKM_PRODUCT = 1101;
const uint16_t VT_ID_SKM_SETUP = 1102;
const uint16_t VT_ID_SKM_CONFIRM = 1103;
const uint16_t VT_ID_KEY_BASE = 1200;		// key k: 1200 + k, its label 1300 + k
const uint16_t VT_ID_KEY_LABEL_BASE = 1300;
const uint16_t VT_ID_TITLE = 2000;
const uint16_t VT_ID_SOURCE = 2002;
const uint16_t VT_ID_MASTER = 2003;			// "Master: On/Off", run screen top right
const uint16_t VT_ID_MESSAGE = 2004;		// footer message (refused key), on every mask
const uint16_t VT_ID_HEAD_BASE = 2090;		// 2090-2094 column heads
const uint16_t VT_ID_ROW_BASE = 2100;		// row r: 2100 + 10 r + offset below
const uint16_t VT_ID_ROW_BUTTON_BASE = 2200;	// row r's name as a button: touch selects the product
const uint8_t VT_KEYCODE_ROW_BASE = 20;		// its key code: 20 + r (soft keys use 1-14)
const uint16_t VT_ID_VR_ALL = 2005;			// "VR: On/Off/Mixed", run screen header
const uint8_t VT_ROW_CONTAINER = 0;
const uint8_t VT_ROW_NAME = 1;
const uint8_t VT_ROW_TARGET = 2;
const uint8_t VT_ROW_TARGET_VAR = 3;
const uint8_t VT_ROW_ACTUAL = 4;
const uint8_t VT_ROW_ACTUAL_VAR = 5;
const uint8_t VT_ROW_UNIT = 6;
const uint8_t VT_ROW_STATE = 7;
const uint16_t VT_ID_SECTION_CONTAINER_BASE = 2300;	// + s
const uint16_t VT_ID_SECTION_RECT_BASE = 2320;		// + s
const uint16_t VT_ID_SECTION_FILL_BASE = 2340;		// + s
const uint16_t VT_ID_SPEED_LABEL = 2400;
const uint16_t VT_ID_SPEED = 2401;
const uint16_t VT_ID_SPEED_VAR = 2402;
const uint16_t VT_ID_SPEED_UNIT = 2403;
const uint16_t VT_ID_SPEED_SOURCE = 2404;
const uint16_t VT_ID_WIDTH_LABEL = 2405;
const uint16_t VT_ID_WIDTH = 2406;
const uint16_t VT_ID_WIDTH_VAR = 2407;
const uint16_t VT_ID_WIDTH_UNIT = 2408;
const uint16_t VT_ID_TC_LINK = 2409;

// product page: label k at 2500 + k, value objects below
const uint16_t VT_ID_P_TITLE = 2490;
const uint16_t VT_ID_P_LABEL_BASE = 2500;
const uint16_t VT_ID_P_TARGET = 2520;
const uint16_t VT_ID_P_TARGET_VAR = 2521;
const uint16_t VT_ID_P_TARGET_UNIT = 2522;
const uint16_t VT_ID_P_ACTUAL = 2523;
const uint16_t VT_ID_P_ACTUAL_VAR = 2524;
const uint16_t VT_ID_P_ACTUAL_UNIT = 2525;
const uint16_t VT_ID_P_STATE = 2526;
const uint16_t VT_ID_P_VARRATE = 2527;		// "On (map rate)", "Off (base rate)", ...
const uint16_t VT_ID_P_OPRATE = 2528;		// input number: the base (operator) rate, tenths per ha
const uint16_t VT_ID_P_OPRATE_VAR = 2529;
const uint16_t VT_ID_P_OPRATE_UNIT = 2530;
const uint16_t VT_ID_P_CONTROL = 2533;
const uint16_t VT_ID_P_PWM = 2534;			// output in %, variable = % + 100 (offset -100)
const uint16_t VT_ID_P_PWM_VAR = 2535;
const uint16_t VT_ID_P_PWM_UNIT = 2536;
const uint8_t VT_P_LINES = 7;

// setup page
const uint16_t VT_ID_S_TITLE = 2600;
const uint16_t VT_ID_S_LABEL_BASE = 2610;
const uint16_t VT_ID_S_MODE = 2620;
const uint16_t VT_ID_S_FIRMWARE = 2621;
const uint16_t VT_ID_S_MODULE = 2622;
const uint16_t VT_ID_S_DESCRIPTION = 2623;
const uint16_t VT_ID_S_HINT = 2624;
const uint8_t VT_S_LINES = 4;

// mode change confirm page
const uint16_t VT_ID_C_TITLE = 2700;
const uint16_t VT_ID_C_QUESTION = 2701;
const uint16_t VT_ID_C_LINE1 = 2702;
const uint16_t VT_ID_C_LINE2 = 2703;

// keys (index k: object 1200 + k, label 1300 + k, key code k + 1)
enum VT_Key : uint8_t
{
	VT_KEY_MASTER = 0,
	VT_KEY_SETUP,
	VT_KEY_VR,
	VT_KEY_AUTO,
	VT_KEY_PLUS,
	VT_KEY_MINUS,
	VT_KEY_BACK,
	VT_KEY_MODE,
	VT_KEY_YES,
	VT_KEY_NO,
	VT_KEY_VR_ALL,		// run page: VR for every product (as RC's one VR button)
	VT_KEY_COUNT
};

const uint8_t VT_ROWS = 6;
const uint8_t VT_SECTIONS = 16;
const uint16_t VT_MASK_SIZE = 480;
const uint16_t VT_DESIGNATOR_HEIGHT = 80;
const uint32_t VT_SCAN_MS = 500;
const uint8_t VT_SCAN_BUDGET = 24;			// single-frame sends per scan (CAN transmit queue is 64)
const uint32_t VT_STRING_GAP_MS = 50;
const uint32_t VT_UNSENT = 0xFFFFFFFF;		// "last sent" before the first send (values are clamped below it)

std::shared_ptr<isobus::VirtualTerminalClient> ISOBUSvt;
std::vector<uint8_t> VT_Pool;		// kept: the client uploads from it, again after a VT restart
char VT_Label[8];
bool VT_WasConnected = false;
uint32_t VT_LastScan = 0;
uint32_t VT_LastStringSend = 0;

// numbers, colours and hide/show: last value sent (VT_UNSENT = send it)
uint32_t VT_SentTarget[VT_ROWS];
uint32_t VT_SentActual[VT_ROWS];
uint32_t VT_SentRowShown[VT_ROWS];
uint32_t VT_SentSectionShown[VT_SECTIONS];
uint32_t VT_SentSectionColour[VT_SECTIONS];
uint32_t VT_SentSpeed;
uint32_t VT_SentWidth;
uint32_t VT_SentPTarget;
uint32_t VT_SentPActual;
uint32_t VT_SentPOpRate;
uint32_t VT_SentPPWM;

// pages and keys
uint16_t VT_ActiveMask = VT_ID_MASK;
uint8_t VT_Selected = 0;				// selected product (row)
uint32_t VT_MessageUntil = 0;			// footer message shown until (millis), 0 = none
int8_t VT_HeldDir = 0;					// + / - held: 1 / -1, 0 = none
uint32_t VT_HeldSince = 0;
uint32_t VT_HeldLastEvent = 0;			// last pressed / still-held event: a lost release times out
uint32_t VT_HeldLastStep = 0;
uint8_t VT_HeldStep = 0;				// RC's RateStep: grows 1 every 2 s held, up to 10
const uint32_t VT_HOLD_TICK_MS = 250;	// RC's AdjustDelay
const uint32_t VT_HOLD_STEP_MS = 2000;	// RC's StepDelay
const uint8_t VT_HOLD_MAX_STEP = 10;
const int16_t VT_MOTOR_STEP = 10;		// RC's ManualMotorStepSize
const int16_t VT_VALVE_RAMP = 15;		// RC's ValveRampPerTick
const uint32_t VT_HOLD_TIMEOUT_MS = 1500;
const uint32_t VT_MESSAGE_MS = 3000;
isobus::EventCallbackHandle VT_KeyListener;
isobus::EventCallbackHandle VT_NumberListener;
isobus::EventCallbackHandle VT_ButtonListener;

// text: the scan sets Want, VT_SendOneString sends the first that differs from Sent
const uint8_t VT_MAX_TEXT = 60;
struct VT_StringSlot
{
	uint16_t ID;
	uint8_t Length;					// the output string's fixed length; text is padded with spaces
	char Want[VT_MAX_TEXT + 1];
	char Sent[VT_MAX_TEXT + 1];
	bool SentValid;
	bool Centred;					// centre-justified string: pad on both sides, not only after
};

enum VT_Slot : uint8_t
{
	VT_SLOT_DESIGNATOR = 0,
	VT_SLOT_SOURCE,
	VT_SLOT_UNIT_BASE,							// + r
	VT_SLOT_STATE_BASE = VT_SLOT_UNIT_BASE + VT_ROWS,	// + r
	VT_SLOT_SPEED_SOURCE = VT_SLOT_STATE_BASE + VT_ROWS,
	VT_SLOT_TC_LINK,
	VT_SLOT_MASTER,
	VT_SLOT_VR_ALL,
	VT_SLOT_MESSAGE,
	VT_SLOT_P_TITLE,
	VT_SLOT_P_TARGET_UNIT,
	VT_SLOT_P_ACTUAL_UNIT,
	VT_SLOT_P_STATE,
	VT_SLOT_P_VARRATE,
	VT_SLOT_P_OPRATE_UNIT,
	VT_SLOT_KEY_AUTO,
	VT_SLOT_P_CONTROL,
	VT_SLOT_S_MODE,
	VT_SLOT_S_MODULE,
	VT_SLOT_S_DESCRIPTION,
	VT_SLOT_S_HINT,
	VT_SLOT_C_QUESTION,
	VT_SLOT_C_LINE1,
	VT_SLOT_C_LINE2,
	VT_SLOT_COUNT
};

VT_StringSlot VT_Str[VT_SLOT_COUNT];

// ---- pool writer ----

struct VT_Child
{
	uint16_t ID;
	int16_t X;
	int16_t Y;
};

// The object writers are members, not free functions: the Arduino IDE puts prototypes of free
// functions at the top of the sketch, before these types exist.
class VT_Writer
{
public:
	explicit VT_Writer(std::vector<uint8_t>& out) : bytes(out) {}

	void u8(uint8_t value) { bytes.push_back(value); }

	void u16(uint16_t value)
	{
		bytes.push_back(value & 0xFF);
		bytes.push_back(value >> 8);
	}

	void u32(uint32_t value)
	{
		for (uint8_t i = 0; i < 4; i++) bytes.push_back((value >> (8 * i)) & 0xFF);
	}

	void f32(float value)
	{
		uint32_t raw;
		memcpy(&raw, &value, 4);	// IEEE 754, little endian like the pool
		u32(raw);
	}

	void header(uint16_t id, uint8_t type)
	{
		u16(id);
		u8(type);
	}

	void children(const VT_Child* list, uint8_t count)
	{
		for (uint8_t i = 0; i < count; i++)
		{
			u16(list[i].ID);
			u16(static_cast<uint16_t>(list[i].X));
			u16(static_cast<uint16_t>(list[i].Y));
		}
	}

	void WorkingSet(const VT_Child* list, uint8_t count)
	{
		header(VT_ID_WORKING_SET, VT_TYPE_WORKING_SET);
		u8(VT_SILVER);		// background
		u8(1);				// selectable
		u16(VT_ID_MASK);	// active mask
		u8(count);
		u8(0);				// macros
		u8(0);				// languages
		children(list, count);
	}

	void DataMask(uint16_t id, uint16_t softKeyMask, const VT_Child* list, uint8_t count)
	{
		header(id, VT_TYPE_DATA_MASK);
		u8(VT_SILVER);
		u16(softKeyMask);
		u8(count);
		u8(0);				// macros
		children(list, count);
	}

	// soft key mask: key object IDs only, no positions
	void SoftKeyMask(uint16_t id, const uint16_t* keys, uint8_t count)
	{
		header(id, VT_TYPE_SOFT_KEY_MASK);
		u8(VT_SILVER);
		u8(count);
		u8(0);				// macros
		for (uint8_t i = 0; i < count; i++) u16(keys[i]);
	}

	// key with one label filling it
	void Key(uint16_t id, uint8_t keyCode, uint16_t label)
	{
		header(id, VT_TYPE_KEY);
		u8(VT_SILVER);
		u8(keyCode);
		u8(1);				// children
		u8(0);				// macros
		u16(label);
		u16(0);
		u16(0);
	}

	// button with one child, drawn with the VT's own border (the style every VT shows focus on)
	void Button(uint16_t id, uint16_t width, uint16_t height, uint8_t keyCode, uint16_t child, int16_t x, int16_t y)
	{
		header(id, VT_TYPE_BUTTON);
		u16(width);
		u16(height);
		u8(VT_SILVER);		// background
		u8(VT_GREY);		// border colour: a light frame (black was too heavy on the bench VT)
		u8(keyCode);
		u8(0x00);			// options: momentary, VT border, filled face
		u8(1);				// children
		u8(0);				// macros
		u16(child);
		u16(static_cast<uint16_t>(x));
		u16(static_cast<uint16_t>(y));
	}

	// input number: the VT's own keypad; Change Numeric Value comes back for this object
	void InputNumber(uint16_t id, uint16_t width, uint16_t height, uint16_t font, uint16_t variable, uint32_t minimum, uint32_t maximum, float scale, uint8_t decimals)
	{
		header(id, VT_TYPE_INPUT_NUMBER);
		u16(width);
		u16(height);
		u8(VT_WHITE);		// background: shows it is an input
		u16(font);
		u8(0);				// options: opaque
		u16(variable);
		u32(0);				// value (the variable's is used)
		u32(minimum);
		u32(maximum);
		u32(0);				// offset
		f32(scale);
		u8(decimals);
		u8(0);				// fixed format
		u8(VT_RIGHT_MIDDLE);
		u8(0x01);			// options 2: enabled
		u8(0);				// macros
	}

	void Container(uint16_t id, uint16_t width, uint16_t height, const VT_Child* list, uint8_t count)
	{
		header(id, VT_TYPE_CONTAINER);
		u16(width);
		u16(height);
		u8(0);				// not hidden; VT_Scan hides what isn't used
		u8(count);
		u8(0);				// macros
		children(list, count);
	}

	void OutputString(uint16_t id, uint16_t width, uint16_t height, uint16_t font, uint8_t length, const char* text, uint8_t justification = VT_LEFT_MIDDLE)
	{
		header(id, VT_TYPE_OUTPUT_STRING);
		u16(width);
		u16(height);
		u8(VT_SILVER);		// background (transparent below)
		u16(font);
		u8(VT_TRANSPARENT);
		u16(isobus::NULL_OBJECT_ID);	// no string variable
		u8(justification);
		u16(length);
		size_t textLength = strlen(text);
		for (uint8_t i = 0; i < length; i++) u8(i < textLength ? text[i] : ' ');	// exactly `length` bytes
		u8(0);				// macros
	}

	// default one decimal: the variable holds tenths. Shown = (variable + offset) x scale.
	void OutputNumber(uint16_t id, uint16_t width, uint16_t height, uint16_t font, uint16_t variable, int32_t offset = 0, float scale = 0.1f, uint8_t decimals = 1)
	{
		header(id, VT_TYPE_OUTPUT_NUMBER);
		u16(width);
		u16(height);
		u8(VT_SILVER);
		u16(font);
		u8(VT_TRANSPARENT);
		u16(variable);
		u32(0);				// value (the variable's is used)
		u32(static_cast<uint32_t>(offset));
		f32(scale);
		u8(decimals);
		u8(0);				// fixed format
		u8(VT_RIGHT_MIDDLE);
		u8(0);				// macros
	}

	void NumberVariable(uint16_t id)
	{
		header(id, VT_TYPE_NUMBER_VARIABLE);
		u32(0);
	}

	void Rectangle(uint16_t id, uint16_t width, uint16_t height, uint16_t line, uint16_t fill)
	{
		header(id, VT_TYPE_OUTPUT_RECTANGLE);
		u16(line);
		u16(width);
		u16(height);
		u8(0);				// no sides suppressed
		u16(fill);
		u8(0);				// macros
	}

	void FontAttributes(uint16_t id, uint8_t size)
	{
		header(id, VT_TYPE_FONT_ATTRIBUTES);
		u8(VT_BLACK);
		u8(size);
		u8(0);				// ISO 8859-1
		u8(0);				// style
		u8(0);				// macros
	}

	void LineAttributes(uint16_t id, uint8_t colour, uint8_t width)
	{
		header(id, VT_TYPE_LINE_ATTRIBUTES);
		u8(colour);
		u8(width);
		u16(0xFFFF);		// solid
		u8(0);				// macros
	}

	void FillAttributes(uint16_t id, uint8_t colour)
	{
		header(id, VT_TYPE_FILL_ATTRIBUTES);
		u8(2);				// fill with the fill colour
		u8(colour);
		u16(isobus::NULL_OBJECT_ID);	// no pattern
		u8(0);				// macros
	}

private:
	std::vector<uint8_t>& bytes;
};

uint16_t VT_RowID(uint8_t r, uint8_t offset)
{
	return VT_ID_ROW_BASE + 10 * r + offset;
}

uint32_t VT_CRC32(const std::vector<uint8_t>& data)
{
	uint32_t crc = 0xFFFFFFFF;
	for (uint8_t b : data)
	{
		crc ^= b;
		for (uint8_t k = 0; k < 8; k++) crc = (crc >> 1) ^ (0xEDB88320 & (0 - (crc & 1)));
	}
	return ~crc;
}

// Layout (design § 3): header y 0-50, column heads y 55, rows y 78 + 45 r, sections y 350,
// footer y 410 and 440. Text widths: 12 px a character in the normal font, 8 in the small one.
FLASHMEM void VT_BuildPool()
{
	VT_Pool.clear();
	VT_Pool.reserve(4096);
	VT_Writer w(VT_Pool);

	// the designator fills the working set's soft-key-sized area (80 x 80 at the scaling height), text centred
	const VT_Child wsChildren[] = { { VT_ID_DESIGNATOR, 0, 0 } };
	w.WorkingSet(wsChildren, 1);
	// 16 x 24 font: "RC 25" (5 characters) is 80 wide, the full button
	w.OutputString(VT_ID_DESIGNATOR, VT_DESIGNATOR_HEIGHT, VT_DESIGNATOR_HEIGHT, VT_ID_FONT_LARGE, 5, "RC", VT_CENTRE_MIDDLE);

	w.FontAttributes(VT_ID_FONT_NORMAL, VT_FONT_12X16);
	w.FontAttributes(VT_ID_FONT_LARGE, VT_FONT_16X24);
	w.FontAttributes(VT_ID_FONT_SMALL, VT_FONT_8X12);
	w.LineAttributes(VT_ID_LINE_BLACK, VT_BLACK, 1);

	// data mask children
	const uint8_t MaskCount = 3 + 5 + VT_ROWS + VT_SECTIONS + 10;	// title, master, VR, heads, rows, sections, footer (number variables are not children)
	VT_Child mask[MaskCount];
	uint8_t n = 0;
	mask[n++] = { VT_ID_TITLE, 5, 10 };		// the module is on the working set button
	mask[n++] = { VT_ID_MASTER, 215, 14 };
	mask[n++] = { VT_ID_VR_ALL, 365, 14 };
	const int16_t HeadX[5] = { 5, 122, 202, 290, 384 };
	for (uint8_t i = 0; i < 5; i++) mask[n++] = { static_cast<uint16_t>(VT_ID_HEAD_BASE + i), HeadX[i], 55 };
	for (uint8_t r = 0; r < VT_ROWS; r++) mask[n++] = { VT_RowID(r, VT_ROW_CONTAINER), 0, static_cast<int16_t>(78 + 45 * r) };
	for (uint8_t s = 0; s < VT_SECTIONS; s++) mask[n++] = { static_cast<uint16_t>(VT_ID_SECTION_CONTAINER_BASE + s), static_cast<int16_t>(16 + 28 * s), 350 };
	mask[n++] = { VT_ID_SPEED_LABEL, 5, 410 };
	mask[n++] = { VT_ID_SPEED, 50, 410 };
	mask[n++] = { VT_ID_SPEED_UNIT, 115, 410 };
	mask[n++] = { VT_ID_SPEED_SOURCE, 160, 410 };
	mask[n++] = { VT_ID_WIDTH_LABEL, 300, 410 };
	mask[n++] = { VT_ID_WIDTH, 345, 410 };
	mask[n++] = { VT_ID_WIDTH_UNIT, 410, 410 };
	mask[n++] = { VT_ID_TC_LINK, 5, 440 };
	mask[n++] = { VT_ID_SOURCE, 160, 440 };		// technical detail: in the footer, under the speed source
	mask[n++] = { VT_ID_MESSAGE, 300, 440 };
	w.DataMask(VT_ID_MASK, VT_ID_SKM_RUN, mask, n);

	// header and column heads
	w.OutputString(VT_ID_TITLE, 180, 30, VT_ID_FONT_LARGE, 11, "RateControl");
	w.OutputString(VT_ID_MASTER, 132, 20, VT_ID_FONT_NORMAL, 11, "Master:");
	w.OutputString(VT_ID_VR_ALL, 110, 20, VT_ID_FONT_NORMAL, 9, "VR:");
	w.OutputString(VT_ID_MESSAGE, 176, 16, VT_ID_FONT_SMALL, 22, "");
	const char* Heads[5] = { "Product", "Target", "Actual", "Unit", "State" };
	for (uint8_t i = 0; i < 5; i++)
	{
		// Target and Actual over their right-aligned numbers (same x and width)
		uint8_t justify = (i == 1 || i == 2) ? VT_RIGHT_MIDDLE : VT_LEFT_MIDDLE;
		w.OutputString(VT_ID_HEAD_BASE + i, 80, 16, VT_ID_FONT_SMALL, strlen(Heads[i]), Heads[i], justify);
	}

	// product rows
	for (uint8_t r = 0; r < VT_ROWS; r++)
	{
		const VT_Child row[] = {
			{ static_cast<uint16_t>(VT_ID_ROW_BUTTON_BASE + r), 2, 2 },	// the name: opens the product page
			{ VT_RowID(r, VT_ROW_TARGET), 122, 10 },	// right-aligned, ends at 202
			{ VT_RowID(r, VT_ROW_ACTUAL), 202, 10 },	// ends at 282
			{ VT_RowID(r, VT_ROW_UNIT), 290, 10 },
			{ VT_RowID(r, VT_ROW_STATE), 384, 10 } };	// 8 characters x 12 = 96, ends at 480
		w.Container(VT_RowID(r, VT_ROW_CONTAINER), VT_MASK_SIZE, 40, row, 5);

		char name[10];
		snprintf(name, sizeof(name), "Product %u", r + 1);
		w.Button(VT_ID_ROW_BUTTON_BASE + r, 118, 36, VT_KEYCODE_ROW_BASE + r, VT_RowID(r, VT_ROW_NAME), 4, 8);
		w.OutputString(VT_RowID(r, VT_ROW_NAME), 110, 20, VT_ID_FONT_NORMAL, 9, name);
		w.OutputNumber(VT_RowID(r, VT_ROW_TARGET), 80, 20, VT_ID_FONT_NORMAL, VT_RowID(r, VT_ROW_TARGET_VAR));
		w.NumberVariable(VT_RowID(r, VT_ROW_TARGET_VAR));
		w.OutputNumber(VT_RowID(r, VT_ROW_ACTUAL), 80, 20, VT_ID_FONT_NORMAL, VT_RowID(r, VT_ROW_ACTUAL_VAR));
		w.NumberVariable(VT_RowID(r, VT_ROW_ACTUAL_VAR));
		w.OutputString(VT_RowID(r, VT_ROW_UNIT), 96, 20, VT_ID_FONT_NORMAL, 8, "");
		w.OutputString(VT_RowID(r, VT_ROW_STATE), 96, 20, VT_ID_FONT_NORMAL, 8, "");
	}

	// sections: each its own fill, so its colour changes alone
	for (uint8_t s = 0; s < VT_SECTIONS; s++)
	{
		const VT_Child sec[] = { { static_cast<uint16_t>(VT_ID_SECTION_RECT_BASE + s), 0, 0 } };
		w.Container(VT_ID_SECTION_CONTAINER_BASE + s, 26, 40, sec, 1);
		w.Rectangle(VT_ID_SECTION_RECT_BASE + s, 26, 40, VT_ID_LINE_BLACK, VT_ID_SECTION_FILL_BASE + s);
		w.FillAttributes(VT_ID_SECTION_FILL_BASE + s, VT_GREY);
	}

	// footer
	w.OutputString(VT_ID_SPEED_LABEL, 45, 16, VT_ID_FONT_SMALL, 5, "Speed");
	w.OutputNumber(VT_ID_SPEED, 60, 16, VT_ID_FONT_SMALL, VT_ID_SPEED_VAR);
	w.NumberVariable(VT_ID_SPEED_VAR);
	w.OutputString(VT_ID_SPEED_UNIT, 40, 16, VT_ID_FONT_SMALL, 4, "km/h");
	w.OutputString(VT_ID_SPEED_SOURCE, 120, 16, VT_ID_FONT_SMALL, 14, "");
	w.OutputString(VT_ID_WIDTH_LABEL, 45, 16, VT_ID_FONT_SMALL, 5, "Width");
	w.OutputNumber(VT_ID_WIDTH, 60, 16, VT_ID_FONT_SMALL, VT_ID_WIDTH_VAR);
	w.NumberVariable(VT_ID_WIDTH_VAR);
	w.OutputString(VT_ID_WIDTH_UNIT, 16, 16, VT_ID_FONT_SMALL, 1, "m");
	w.OutputString(VT_ID_TC_LINK, 120, 16, VT_ID_FONT_SMALL, 13, "");
	w.OutputString(VT_ID_SOURCE, 96, 16, VT_ID_FONT_SMALL, 12, "Source:");

	// keys: a label filling each (key code = index + 1); + and - in the large font
	const char* KeyText[VT_KEY_COUNT] = { "Master", "Setup", "VR", "Manual", "+", "-", "Back", "Mode", "Yes", "No", "VR" };	// Auto key: what it does, "Manual" / "Auto" (VT_SLOT_KEY_AUTO)
	for (uint8_t k = 0; k < VT_KEY_COUNT; k++)
	{
		bool large = (k == VT_KEY_PLUS || k == VT_KEY_MINUS);
		w.Key(VT_ID_KEY_BASE + k, k + 1, VT_ID_KEY_LABEL_BASE + k);
		w.OutputString(VT_ID_KEY_LABEL_BASE + k, VT_DESIGNATOR_HEIGHT, VT_DESIGNATOR_HEIGHT, large ? VT_ID_FONT_LARGE : VT_ID_FONT_SMALL, strlen(KeyText[k]), KeyText[k], VT_CENTRE_MIDDLE);
	}
	const uint16_t RunKeys[] = { VT_ID_KEY_BASE + VT_KEY_MASTER, VT_ID_KEY_BASE + VT_KEY_VR_ALL, VT_ID_KEY_BASE + VT_KEY_SETUP };
	const uint16_t ProductKeys[] = { VT_ID_KEY_BASE + VT_KEY_VR, VT_ID_KEY_BASE + VT_KEY_AUTO, VT_ID_KEY_BASE + VT_KEY_PLUS, VT_ID_KEY_BASE + VT_KEY_MINUS, VT_ID_KEY_BASE + VT_KEY_BACK };
	const uint16_t SetupKeys[] = { VT_ID_KEY_BASE + VT_KEY_MODE, VT_ID_KEY_BASE + VT_KEY_BACK };
	const uint16_t ConfirmKeys[] = { VT_ID_KEY_BASE + VT_KEY_YES, VT_ID_KEY_BASE + VT_KEY_NO };
	w.SoftKeyMask(VT_ID_SKM_RUN, RunKeys, 3);
	w.SoftKeyMask(VT_ID_SKM_PRODUCT, ProductKeys, 5);
	w.SoftKeyMask(VT_ID_SKM_SETUP, SetupKeys, 2);
	w.SoftKeyMask(VT_ID_SKM_CONFIRM, ConfirmKeys, 2);

	// product page (the selected product): label at x 5, value at x 200, a line every 38 from y 60
	const char* PLabel[VT_P_LINES] = { "Target", "Actual", "State", "Variable rate", "Base rate", "Control", "PWM output" };
	VT_Child page[2 + 2 * VT_P_LINES + 6];
	n = 0;
	page[n++] = { VT_ID_P_TITLE, 0, 10 };
	for (uint8_t k = 0; k < VT_P_LINES; k++) page[n++] = { static_cast<uint16_t>(VT_ID_P_LABEL_BASE + k), 5, static_cast<int16_t>(60 + 38 * k) };
	page[n++] = { VT_ID_P_TARGET, 200, 60 };
	page[n++] = { VT_ID_P_TARGET_UNIT, 320, 60 };
	page[n++] = { VT_ID_P_ACTUAL, 200, 98 };
	page[n++] = { VT_ID_P_ACTUAL_UNIT, 320, 98 };
	page[n++] = { VT_ID_P_STATE, 200, 136 };
	page[n++] = { VT_ID_P_VARRATE, 200, 174 };
	page[n++] = { VT_ID_P_OPRATE, 200, 210 };
	page[n++] = { VT_ID_P_OPRATE_UNIT, 320, 212 };
	page[n++] = { VT_ID_P_CONTROL, 200, 250 };
	page[n++] = { VT_ID_P_PWM, 200, 288 };
	page[n++] = { VT_ID_P_PWM_UNIT, 320, 288 };
	page[n++] = { VT_ID_MESSAGE, 300, 440 };
	w.DataMask(VT_ID_MASK_PRODUCT, VT_ID_SKM_PRODUCT, page, n);
	w.OutputString(VT_ID_P_TITLE, VT_MASK_SIZE, 30, VT_ID_FONT_LARGE, 9, "", VT_CENTRE_MIDDLE);
	for (uint8_t k = 0; k < VT_P_LINES; k++) w.OutputString(VT_ID_P_LABEL_BASE + k, 180, 20, VT_ID_FONT_NORMAL, strlen(PLabel[k]), PLabel[k]);
	w.OutputNumber(VT_ID_P_TARGET, 110, 20, VT_ID_FONT_NORMAL, VT_ID_P_TARGET_VAR);
	w.NumberVariable(VT_ID_P_TARGET_VAR);
	w.OutputString(VT_ID_P_TARGET_UNIT, 96, 20, VT_ID_FONT_NORMAL, 8, "");
	w.OutputNumber(VT_ID_P_ACTUAL, 110, 20, VT_ID_FONT_NORMAL, VT_ID_P_ACTUAL_VAR);
	w.NumberVariable(VT_ID_P_ACTUAL_VAR);
	w.OutputString(VT_ID_P_ACTUAL_UNIT, 96, 20, VT_ID_FONT_NORMAL, 8, "");
	w.OutputString(VT_ID_P_STATE, 156, 20, VT_ID_FONT_NORMAL, 13, "");
	w.OutputString(VT_ID_P_VARRATE, 270, 20, VT_ID_FONT_NORMAL, 22, "");
	// operator rate in tenths per ha, 0 - 10,000,000 (TC_MaxOperatorRate)
	w.InputNumber(VT_ID_P_OPRATE, 110, 24, VT_ID_FONT_NORMAL, VT_ID_P_OPRATE_VAR, 0, 100000000UL, 0.1f, 1);
	w.NumberVariable(VT_ID_P_OPRATE_VAR);
	w.OutputString(VT_ID_P_OPRATE_UNIT, 96, 20, VT_ID_FONT_NORMAL, 8, "");
	w.OutputString(VT_ID_P_CONTROL, 156, 20, VT_ID_FONT_NORMAL, 13, "");
	w.OutputNumber(VT_ID_P_PWM, 110, 20, VT_ID_FONT_NORMAL, VT_ID_P_PWM_VAR, -100, 1.0f, 0);
	w.NumberVariable(VT_ID_P_PWM_VAR);
	w.OutputString(VT_ID_P_PWM_UNIT, 24, 20, VT_ID_FONT_NORMAL, 1, "%");

	// setup page: label at x 5, value at x 220, a line every 40 from y 70; hint under them
	const char* SLabel[VT_S_LINES] = { "Control mode", "Firmware", "Module", "Description" };
	VT_Child setup[2 + 2 * VT_S_LINES + 2];
	n = 0;
	setup[n++] = { VT_ID_S_TITLE, 0, 10 };
	for (uint8_t k = 0; k < VT_S_LINES; k++) setup[n++] = { static_cast<uint16_t>(VT_ID_S_LABEL_BASE + k), 5, static_cast<int16_t>(70 + 40 * k) };
	setup[n++] = { VT_ID_S_MODE, 220, 70 };
	setup[n++] = { VT_ID_S_FIRMWARE, 220, 110 };
	setup[n++] = { VT_ID_S_MODULE, 220, 150 };
	setup[n++] = { VT_ID_S_DESCRIPTION, 220, 190 };
	setup[n++] = { VT_ID_S_HINT, 5, 250 };
	setup[n++] = { VT_ID_MESSAGE, 300, 440 };
	w.DataMask(VT_ID_MASK_SETUP, VT_ID_SKM_SETUP, setup, n);
	w.OutputString(VT_ID_S_TITLE, VT_MASK_SIZE, 30, VT_ID_FONT_LARGE, 5, "Setup", VT_CENTRE_MIDDLE);
	for (uint8_t k = 0; k < VT_S_LINES; k++) w.OutputString(VT_ID_S_LABEL_BASE + k, 200, 20, VT_ID_FONT_NORMAL, strlen(SLabel[k]), SLabel[k]);
	w.OutputString(VT_ID_S_MODE, 120, 20, VT_ID_FONT_NORMAL, 8, "");
	char firmware[8];
	snprintf(firmware, sizeof(firmware), "%u", InoID);
	w.OutputString(VT_ID_S_FIRMWARE, 120, 20, VT_ID_FONT_NORMAL, strlen(firmware), firmware);
	w.OutputString(VT_ID_S_MODULE, 120, 20, VT_ID_FONT_NORMAL, 3, "");
	w.OutputString(VT_ID_S_DESCRIPTION, 256, 20, VT_ID_FONT_NORMAL, 16, "");
	w.OutputString(VT_ID_S_HINT, 470, 16, VT_ID_FONT_SMALL, 58, "");

	// mode change confirm page
	const VT_Child confirm[] = {
		{ VT_ID_C_TITLE, 0, 10 },
		{ VT_ID_C_QUESTION, 0, 100 },
		{ VT_ID_C_LINE1, 5, 160 },
		{ VT_ID_C_LINE2, 5, 185 } };
	w.DataMask(VT_ID_MASK_CONFIRM, VT_ID_SKM_CONFIRM, confirm, 4);
	w.OutputString(VT_ID_C_TITLE, VT_MASK_SIZE, 30, VT_ID_FONT_LARGE, 12, "Control mode", VT_CENTRE_MIDDLE);
	w.OutputString(VT_ID_C_QUESTION, VT_MASK_SIZE, 30, VT_ID_FONT_LARGE, 22, "", VT_CENTRE_MIDDLE);
	w.OutputString(VT_ID_C_LINE1, 470, 16, VT_ID_FONT_SMALL, 58, "");
	w.OutputString(VT_ID_C_LINE2, 470, 16, VT_ID_FONT_SMALL, 58, "");

	snprintf(VT_Label, sizeof(VT_Label), "RC%05lX", (unsigned long)(VT_CRC32(VT_Pool) & 0xFFFFF));
}

// ---- live values ----

void VT_SetSlot(uint8_t slot, uint16_t id, uint8_t length)
{
	VT_Str[slot].ID = id;
	VT_Str[slot].Length = length;
	VT_Str[slot].Want[0] = 0;
	VT_Str[slot].SentValid = false;
	VT_Str[slot].Centred = false;
}

FLASHMEM void VT_InitSlots()
{
	VT_SetSlot(VT_SLOT_DESIGNATOR, VT_ID_DESIGNATOR, 5);
	VT_Str[VT_SLOT_DESIGNATOR].Centred = true;
	VT_SetSlot(VT_SLOT_SOURCE, VT_ID_SOURCE, 12);
	for (uint8_t r = 0; r < VT_ROWS; r++)
	{
		VT_SetSlot(VT_SLOT_UNIT_BASE + r, VT_RowID(r, VT_ROW_UNIT), 8);
		VT_SetSlot(VT_SLOT_STATE_BASE + r, VT_RowID(r, VT_ROW_STATE), 8);
	}
	VT_SetSlot(VT_SLOT_SPEED_SOURCE, VT_ID_SPEED_SOURCE, 14);
	VT_SetSlot(VT_SLOT_TC_LINK, VT_ID_TC_LINK, 13);
	VT_SetSlot(VT_SLOT_MASTER, VT_ID_MASTER, 11);
	VT_SetSlot(VT_SLOT_VR_ALL, VT_ID_VR_ALL, 9);
	VT_SetSlot(VT_SLOT_MESSAGE, VT_ID_MESSAGE, 22);
	VT_SetSlot(VT_SLOT_P_TITLE, VT_ID_P_TITLE, 9);
	VT_Str[VT_SLOT_P_TITLE].Centred = true;
	VT_SetSlot(VT_SLOT_P_TARGET_UNIT, VT_ID_P_TARGET_UNIT, 8);
	VT_SetSlot(VT_SLOT_P_ACTUAL_UNIT, VT_ID_P_ACTUAL_UNIT, 8);
	VT_SetSlot(VT_SLOT_P_STATE, VT_ID_P_STATE, 13);
	VT_SetSlot(VT_SLOT_P_VARRATE, VT_ID_P_VARRATE, 22);
	VT_SetSlot(VT_SLOT_P_OPRATE_UNIT, VT_ID_P_OPRATE_UNIT, 8);
	VT_SetSlot(VT_SLOT_KEY_AUTO, VT_ID_KEY_LABEL_BASE + VT_KEY_AUTO, 6);
	VT_Str[VT_SLOT_KEY_AUTO].Centred = true;
	VT_SetSlot(VT_SLOT_P_CONTROL, VT_ID_P_CONTROL, 13);
	VT_SetSlot(VT_SLOT_S_MODE, VT_ID_S_MODE, 8);
	VT_SetSlot(VT_SLOT_S_MODULE, VT_ID_S_MODULE, 3);
	VT_SetSlot(VT_SLOT_S_DESCRIPTION, VT_ID_S_DESCRIPTION, 16);
	VT_SetSlot(VT_SLOT_S_HINT, VT_ID_S_HINT, 58);
	VT_SetSlot(VT_SLOT_C_QUESTION, VT_ID_C_QUESTION, 22);
	VT_Str[VT_SLOT_C_QUESTION].Centred = true;
	VT_SetSlot(VT_SLOT_C_LINE1, VT_ID_C_LINE1, 58);
	VT_SetSlot(VT_SLOT_C_LINE2, VT_ID_C_LINE2, 58);
}

void VT_Want(uint8_t slot, const char* text)
{
	strncpy(VT_Str[slot].Want, text, VT_MAX_TEXT);
	VT_Str[slot].Want[VT_MAX_TEXT] = 0;
}

// after a (re)connect: everything is sent again
void VT_ForgetSent()
{
	for (uint8_t r = 0; r < VT_ROWS; r++)
	{
		VT_SentTarget[r] = VT_UNSENT;
		VT_SentActual[r] = VT_UNSENT;
		VT_SentRowShown[r] = VT_UNSENT;
	}
	VT_SentPTarget = VT_UNSENT;
	VT_SentPActual = VT_UNSENT;
	VT_SentPOpRate = VT_UNSENT;
	VT_SentPPWM = VT_UNSENT;
	for (uint8_t s = 0; s < VT_SECTIONS; s++)
	{
		VT_SentSectionShown[s] = VT_UNSENT;
		VT_SentSectionColour[s] = VT_UNSENT;
	}
	VT_SentSpeed = VT_UNSENT;
	VT_SentWidth = VT_UNSENT;
	for (uint8_t i = 0; i < VT_SLOT_COUNT; i++) VT_Str[i].SentValid = false;
}

uint32_t VT_Tenths(float value)
{
	uint32_t Result = 0;
	if (value > 0.0f)	// also false for NaN
	{
		if (value > 9999999.0f) value = 9999999.0f;
		Result = static_cast<uint32_t>(value * 10.0f + 0.5f);
	}
	return Result;
}

// sensors with RC's units per litre, kg or seed (32509)
bool VT_HasUnits(uint8_t r)
{
	return (r < TCmaxRates) && (TCset.UnitsPerISO[r] > 0.0f);
}

// Per hectare while the board sets the product's rate (the TC commands, or Terminal mode: the
// rates are per hectare). Otherwise only with units and a speed on the board (ISOBUS or wheel
// sensor): RC works out its rate from its own speed, so without one here the conversion would
// show a false 0; then RC's per-minute values.
bool VT_PerHa(uint8_t r)
{
	bool Result = false;
	if (TC_DrivesProducts() && r < TCmaxRates) Result = true;
	else if (VT_HasUnits(r) && SPEED_GetKmh() > 0.05f) Result = true;
	return Result;
}

float VT_UPMtoPerHa(uint8_t r, float upm)
{
	float Result = 0.0f;
	float speedKmh = SPEED_GetKmh();
	float widthM = TC_ActiveWidthM();
	if (speedKmh > 0.05f && widthM > 0.01f) Result = (upm * 600.0f) / (speedKmh * widthM * TCset.UnitsPerISO[r]);
	return Result;
}

const char* VT_UnitText(uint8_t r)
{
	const char* Result = "/min";
	if (VT_PerHa(r))
	{
		if (TCset.RateDDI[r] == 6) Result = "kg/ha";
		else if (TCset.RateDDI[r] == 11) Result = "seeds/ha";
		else Result = "L/ha";
	}
	return Result;
}

const char* VT_StateText(uint8_t r)
{
	const char* Result = "Off";
	if (!SensorConnected[r]) Result = "No comm";
	else if (TC_DrivesProducts() && r < TCmaxRates && !VT_HasUnits(r)) Result = "No units";
	else if (Applying[r]) Result = "Applying";
	return Result;
}

void VT_RowValues(uint8_t r, uint32_t& target, uint32_t& actual)
{
	if (TC_DrivesProducts() && r < TCmaxRates)
	{
		target = VT_Tenths(TC_EffectiveRate(r));	// the TC's rate with 158 = 1, else the operator's
		actual = VT_Tenths(TC_ActualRate(r));
	}
	else if (VT_PerHa(r))
	{
		target = VT_Tenths(VT_UPMtoPerHa(r, Sensor[r].TargetUPM));
		actual = VT_Tenths(VT_UPMtoPerHa(r, Sensor[r].UPM));
	}
	else
	{
		target = VT_Tenths(Sensor[r].TargetUPM);
		actual = VT_Tenths(Sensor[r].UPM);
	}
}

bool VT_AnySensorConnected()
{
	bool Result = false;
	for (uint8_t i = 0; i < MaxProductCount; i++)
	{
		if (SensorConnected[i]) Result = true;
	}
	return Result;
}

const char* VT_SpeedSourceText()
{
	const char* Result = "None";
	if (SPEED_HasISOBUSSpeed())
	{
		if (ISOBUSspeedSource == SPEED_SOURCE_MACHINE_SELECTED) Result = "ISOBUS machine";
		else if (ISOBUSspeedSource == SPEED_SOURCE_GROUND) Result = "ISOBUS ground";
		else Result = "ISOBUS wheel";
	}
	else if (MDL.WheelSpeedPin != NC)
	{
		Result = "Wheel sensor";
	}
	return Result;
}

// one single-frame send if the value changed and the budget allows; true = budget left
bool VT_SendNumber(uint16_t variable, uint32_t value, uint32_t& sent, uint8_t& budget)
{
	if (value != sent && budget > 0)
	{
		budget--;
		if (ISOBUSvt->send_change_numeric_value(variable, value)) sent = value;
	}
	return budget > 0;
}

void VT_SendShown(uint16_t container, bool shown, uint32_t& sent, uint8_t& budget)
{
	uint32_t value = shown ? 1 : 0;
	if (value != sent && budget > 0)
	{
		budget--;
		auto command = shown ? isobus::VirtualTerminalClient::HideShowObjectCommand::ShowObject : isobus::VirtualTerminalClient::HideShowObjectCommand::HideObject;
		if (ISOBUSvt->send_hide_show_object(container, command)) sent = value;
	}
}

void VT_SendColour(uint16_t fill, uint8_t colour, uint32_t& sent, uint8_t& budget)
{
	if (colour != sent && budget > 0)
	{
		budget--;
		if (ISOBUSvt->send_change_fill_attributes(fill, isobus::VirtualTerminalClient::FillType::FillWithSpecifiedColourInFillColourAttribute, colour, isobus::NULL_OBJECT_ID)) sent = colour;
	}
}

// the products this board has (rows and selection)
uint8_t VT_ProductCount()
{
	return (MDL.SensorCount < VT_ROWS) ? MDL.SensorCount : VT_ROWS;
}

// products with the operator's VR on
uint8_t VT_ProductsWithVR()
{
	uint8_t Result = 0;
	for (uint8_t i = 0; i < VT_ProductCount(); i++)
	{
		if (!TCset.VrOff[i]) Result++;
	}
	return Result;
}

// the product page's per-ha unit (the operator rate is always per ha)
const char* VT_PerHaUnit(uint8_t r)
{
	const char* Result = "L/ha";
	if (TCset.RateDDI[r] == 6) Result = "kg/ha";
	else if (TCset.RateDDI[r] == 11) Result = "seeds/ha";
	return Result;
}

// in the operator's words: is the map driving the product, and if not, why the base rate is used
const char* VT_VariableRateText(uint8_t r)
{
	const char* Result = "Off (base rate)";
	if (!TC_DrivesProducts()) Result = "Set by RateControl";
	else if (TC_InCommand && TC_Rx[r] == 1) Result = "On (map rate)";
	else if (TCset.VrOff[r]) Result = "Off (base rate)";
	else if (!TC_InCommand) Result = "On, no job (base rate)";
	else Result = "On, no map (base rate)";
	return Result;
}

void VT_Message(const char* text)
{
	VT_Want(VT_SLOT_MESSAGE, text);
	VT_MessageUntil = millis() + VT_MESSAGE_MS;
	if (VT_MessageUntil == 0) VT_MessageUntil = 1;
}

void VT_ScanRun(uint8_t& budget)
{
	// product rows (rows past this board's product count are never read)
	for (uint8_t r = 0; r < VT_ROWS; r++)
	{
		bool shown = (r < MDL.SensorCount) && (r < MaxProductCount);
		VT_SendShown(VT_RowID(r, VT_ROW_CONTAINER), shown, VT_SentRowShown[r], budget);
		if (shown)
		{
			uint32_t target, actual;
			VT_RowValues(r, target, actual);
			VT_SendNumber(VT_RowID(r, VT_ROW_TARGET_VAR), target, VT_SentTarget[r], budget);
			VT_SendNumber(VT_RowID(r, VT_ROW_ACTUAL_VAR), actual, VT_SentActual[r], budget);
		}
	}

	// sections: the outputs actually driven
	uint16_t driven = NewLo | (static_cast<uint16_t>(NewHi) << 8);
	for (uint8_t s = 0; s < VT_SECTIONS; s++)
	{
		bool shown = s < TCset.SectionCount;
		VT_SendShown(VT_ID_SECTION_CONTAINER_BASE + s, shown, VT_SentSectionShown[s], budget);
		if (shown) VT_SendColour(VT_ID_SECTION_FILL_BASE + s, bitRead(driven, s) ? VT_LIME : VT_GREY, VT_SentSectionColour[s], budget);
	}

	// footer
	VT_SendNumber(VT_ID_SPEED_VAR, VT_Tenths(SPEED_GetKmh()), VT_SentSpeed, budget);
	VT_SendNumber(VT_ID_WIDTH_VAR, VT_Tenths(TC_ActiveWidthM()), VT_SentWidth, budget);
}

void VT_ScanProduct(uint8_t& budget)
{
	uint8_t r = VT_Selected;
	uint32_t target, actual;
	VT_RowValues(r, target, actual);
	VT_SendNumber(VT_ID_P_TARGET_VAR, target, VT_SentPTarget, budget);
	VT_SendNumber(VT_ID_P_ACTUAL_VAR, actual, VT_SentPActual, budget);
	VT_SendNumber(VT_ID_P_OPRATE_VAR, VT_Tenths(TCset.OperatorRate[r]), VT_SentPOpRate, budget);
	int16_t pwm = AutoOn[r] ? static_cast<int16_t>(constrain(Sensor[r].PWM, -255.0f, 255.0f)) : TC_ManualPWM[r];
	int16_t percent = (pwm >= 0) ? (pwm * 100 + 127) / 255 : -((-pwm * 100 + 127) / 255);	// as RC shows PWM, in %
	VT_SendNumber(VT_ID_P_PWM_VAR, static_cast<uint32_t>(percent + 100), VT_SentPPWM, budget);
}

// strings of every page (sent only when they change); numbers only for the page shown (budget)
void VT_Scan()
{
	uint8_t budget = VT_SCAN_BUDGET;
	char text[VT_MAX_TEXT + 1];
	if (VT_Selected >= VT_ProductCount()) VT_Selected = 0;	// the product count went down
	uint8_t r = VT_Selected;

	// designator, header and footer
	snprintf(text, sizeof(text), "RC %u", MDL.ID);
	VT_Want(VT_SLOT_DESIGNATOR, text);
	const char* source = TC_Terminal() ? "Term" : (TC_InCommand ? "TC" : (VT_AnySensorConnected() ? "RC" : "None"));
	snprintf(text, sizeof(text), "Source: %s", source);
	VT_Want(VT_SLOT_SOURCE, text);
	VT_Want(VT_SLOT_MASTER, MasterOn ? "Master: On" : "Master: Off");
	uint8_t vrOn = VT_ProductsWithVR();
	VT_Want(VT_SLOT_VR_ALL, (vrOn == 0) ? "VR: Off" : ((vrOn == VT_ProductCount()) ? "VR: On" : "VR: Mixed"));
	VT_Want(VT_SLOT_SPEED_SOURCE, VT_SpeedSourceText());
	VT_Want(VT_SLOT_TC_LINK, ISOBUStc->get_is_connected() ? "TC: connected" : "TC: waiting");
	if (VT_MessageUntil != 0 && static_cast<int32_t>(millis() - VT_MessageUntil) >= 0)
	{
		VT_MessageUntil = 0;
		VT_Want(VT_SLOT_MESSAGE, " ");
	}
	for (uint8_t i = 0; i < VT_ProductCount(); i++)
	{
		VT_Want(VT_SLOT_UNIT_BASE + i, VT_UnitText(i));
		VT_Want(VT_SLOT_STATE_BASE + i, VT_StateText(i));
	}

	// product page
	snprintf(text, sizeof(text), "Product %u", r + 1);
	VT_Want(VT_SLOT_P_TITLE, text);
	VT_Want(VT_SLOT_P_TARGET_UNIT, VT_UnitText(r));
	VT_Want(VT_SLOT_P_ACTUAL_UNIT, VT_UnitText(r));
	VT_Want(VT_SLOT_P_STATE, VT_StateText(r));
	VT_Want(VT_SLOT_P_VARRATE, VT_VariableRateText(r));
	VT_Want(VT_SLOT_P_OPRATE_UNIT, VT_PerHaUnit(r));
	VT_Want(VT_SLOT_P_CONTROL, AutoOn[r] ? "Auto" : "Manual");
	VT_Want(VT_SLOT_KEY_AUTO, AutoOn[r] ? "Manual" : "Auto");	// what the key does

	// setup and confirm pages
	VT_Want(VT_SLOT_S_MODE, TC_Terminal() ? "Terminal" : "RC app");
	snprintf(text, sizeof(text), "%u", MDL.ID);
	VT_Want(VT_SLOT_S_MODULE, text);
	char description[17];
	memcpy(description, MDLboard.Text, 16);
	description[16] = 0;
	VT_Want(VT_SLOT_S_DESCRIPTION, description[0] ? description : "-");
	VT_Want(VT_SLOT_S_HINT, TC_Terminal() ? "This terminal sets master, rates, VR and auto / manual."
		: "RateControl sets master and auto / manual.");
	if (TC_Terminal())
	{
		VT_Want(VT_SLOT_C_QUESTION, "Change to RC app?");
		VT_Want(VT_SLOT_C_LINE1, "The board takes RateControl's commands again.");
		VT_Want(VT_SLOT_C_LINE2, "Its products show No comm until RateControl sends them.");
	}
	else
	{
		VT_Want(VT_SLOT_C_QUESTION, "Change to Terminal?");
		VT_Want(VT_SLOT_C_LINE1, "RateControl's rate, master and section commands");
		VT_Want(VT_SLOT_C_LINE2, "are ignored; this terminal and the TC take over.");
	}

	if (VT_ActiveMask == VT_ID_MASK_PRODUCT) VT_ScanProduct(budget);
	else if (VT_ActiveMask == VT_ID_MASK) VT_ScanRun(budget);
}

// ---- pages and keys ----

void VT_ShowMask(uint16_t mask)
{
	if (ISOBUSvt->send_change_active_mask(VT_ID_WORKING_SET, mask))
	{
		VT_ActiveMask = mask;
		VT_LastScan = millis() - VT_SCAN_MS;	// its values at once
	}
}

void VT_StopHold()
{
	if (VT_HeldDir != 0)
	{
		VT_HeldDir = 0;
		if (TC_IsValveType(VT_Selected)) TC_SetManualPWM(VT_Selected, 0);	// released: the valve holds
	}
}

// + / - held: RC's rate buttons in manual (clsRateAdjustController), every 250 ms
void VT_UpdateHold()
{
	if (VT_HeldDir != 0)
	{
		uint8_t r = VT_Selected;
		if (millis() - VT_HeldLastEvent >= VT_HOLD_TIMEOUT_MS || VT_ActiveMask != VT_ID_MASK_PRODUCT || AutoOn[r])
		{
			VT_StopHold();	// release lost, page left or back in auto
		}
		else if (millis() - VT_HeldLastStep >= VT_HOLD_TICK_MS)
		{
			VT_HeldLastStep = millis();
			if (VT_HeldStep == 0 || (millis() - VT_HeldSince >= static_cast<uint32_t>(VT_HeldStep) * VT_HOLD_STEP_MS && VT_HeldStep < VT_HOLD_MAX_STEP)) VT_HeldStep++;
			if (TC_IsValveType(r))
			{
				int16_t magnitude = abs(TC_ManualPWM[r]);
				if (magnitude < Sensor[r].MinPWM) magnitude = Sensor[r].MinPWM;	// first: jump to the minimum
				else magnitude = (magnitude + VT_VALVE_RAMP > 255) ? 255 : magnitude + VT_VALVE_RAMP;
				TC_SetManualPWM(r, magnitude * VT_HeldDir);
			}
			else
			{
				TC_SetManualPWM(r, TC_ManualPWM[r] + VT_HeldDir * VT_HeldStep * VT_MOTOR_STEP);
			}
		}
	}
}

void VT_PlusMinus(int8_t dir, isobus::VirtualTerminalClient::KeyActivationCode code)
{
	using Code = isobus::VirtualTerminalClient::KeyActivationCode;
	uint8_t r = VT_Selected;
	if (code == Code::ButtonPressedOrLatched)
	{
		if (!TC_Terminal()) VT_Message("RC app mode");
		else if (AutoOn[r]) VT_Message("Auto: press Auto/Man");
		else
		{
			VT_HeldDir = dir;
			VT_HeldSince = millis();
			VT_HeldLastEvent = millis();
			VT_HeldLastStep = millis() - VT_HOLD_TICK_MS;	// first step at once
			VT_HeldStep = 0;
		}
	}
	else if (code == Code::ButtonStillHeld)
	{
		if (VT_HeldDir == dir) VT_HeldLastEvent = millis();
	}
	else
	{
		VT_StopHold();	// released or aborted
	}
}

void VT_OnKey(const isobus::VirtualTerminalClient::VTKeyEvent& event)
{
	using Code = isobus::VirtualTerminalClient::KeyActivationCode;
	if (event.objectID >= VT_ID_KEY_BASE && event.objectID < VT_ID_KEY_BASE + VT_KEY_COUNT)
	{
		uint8_t key = static_cast<uint8_t>(event.objectID - VT_ID_KEY_BASE);
		uint8_t r = VT_Selected;
		if (key == VT_KEY_PLUS) VT_PlusMinus(1, event.keyEvent);
		else if (key == VT_KEY_MINUS) VT_PlusMinus(-1, event.keyEvent);
		else if (event.keyEvent == Code::ButtonPressedOrLatched)
		{
			switch (key)
			{
			case VT_KEY_MASTER:
				if (!TC_SetMaster(!MasterOn)) VT_Message("RC app mode");
				else if (MasterOn && !TC_ProcessDataReady()) VT_Message("Master on: no TC");
				else if (MasterOn && !TC_AogTcSeen()) VT_Message("Sections: on terminal");	// not AOG-TC: no PGN 234 to AOG
				break;
			case VT_KEY_SETUP:
				VT_ShowMask(VT_ID_MASK_SETUP);
				break;
			case VT_KEY_VR:
#if TC_RX_DDIS
				TC_SetVrOff(r, !TCset.VrOff[r]);
#else
				VT_Message("No VR in this build");
#endif
				break;
			case VT_KEY_VR_ALL:
#if TC_RX_DDIS
			{
				// as RC's one VR button: all on -> all off, otherwise all on
				bool AllOn = (VT_ProductsWithVR() == VT_ProductCount());
				for (uint8_t i = 0; i < VT_ProductCount(); i++) TC_SetVrOff(i, AllOn);
			}
#else
				VT_Message("No VR in this build");
#endif
				break;
			case VT_KEY_AUTO:
				if (!TC_Terminal()) VT_Message("RC app mode");
				else if (Sensor[r].ControlType == Fan_ct && AutoOn[r]) VT_Message("A fan is always auto");
				else
				{
					VT_StopHold();
					TC_SetAuto(r, !AutoOn[r]);
				}
				break;
			case VT_KEY_BACK:
				VT_StopHold();
				VT_ShowMask(VT_ID_MASK);
				break;
			case VT_KEY_MODE:
				if (MasterOn) VT_Message("Turn master off first");
				else VT_ShowMask(VT_ID_MASK_CONFIRM);
				break;
			case VT_KEY_YES:
				if (!TC_SetControlMode(TC_Terminal() ? TC_MODE_RC_APP : TC_MODE_TERMINAL)) VT_Message("Turn master off first");
				VT_ShowMask(VT_ID_MASK_SETUP);
				break;
			case VT_KEY_NO:
				VT_ShowMask(VT_ID_MASK_SETUP);
				break;
			}
			VT_LastScan = millis() - VT_SCAN_MS;	// show the result at once
		}
	}
}

// a product name on the run page touched: select that product and open its page
void VT_OnButton(const isobus::VirtualTerminalClient::VTKeyEvent& event)
{
	using Code = isobus::VirtualTerminalClient::KeyActivationCode;
	if (event.keyEvent == Code::ButtonPressedOrLatched && event.objectID >= VT_ID_ROW_BUTTON_BASE && event.objectID < VT_ID_ROW_BUTTON_BASE + VT_ROWS)
	{
		uint8_t r = static_cast<uint8_t>(event.objectID - VT_ID_ROW_BUTTON_BASE);
		if (r < VT_ProductCount())
		{
			VT_Selected = r;
			VT_ShowMask(VT_ID_MASK_PRODUCT);
		}
	}
}

// the operator rate entered on the product page (tenths per ha); the VT keeps it in range. Terminals differ
// in the ID they send: the input object's (ISO 11783-6) or its number variable's (AgIsoVirtualTerminal).
void VT_OnNumber(const isobus::VirtualTerminalClient::VTChangeNumericValueEvent& event)
{
	if (event.objectID == VT_ID_P_OPRATE || event.objectID == VT_ID_P_OPRATE_VAR)
	{
		TC_SetOperatorRate(VT_Selected, event.value * 0.1f);
		VT_SentPOpRate = VT_UNSENT;		// shows the value kept (the old one if refused)
		VT_LastScan = millis() - VT_SCAN_MS;
	}
}

// the first string that changed, at most one every VT_STRING_GAP_MS (one transport session at a time)
void VT_SendOneString()
{
	if (millis() - VT_LastStringSend >= VT_STRING_GAP_MS)
	{
		bool done = false;
		for (uint8_t i = 0; i < VT_SLOT_COUNT && !done; i++)
		{
			VT_StringSlot& slot = VT_Str[i];
			if (slot.Want[0] != 0 && (!slot.SentValid || strcmp(slot.Want, slot.Sent) != 0))
			{
				done = true;
				VT_LastStringSend = millis();
				char padded[VT_MAX_TEXT + 1];
				uint8_t length = strlen(slot.Want);
				uint8_t lead = slot.Centred ? (slot.Length - length) / 2 : 0;
				memset(padded, ' ', slot.Length);					// also clears the old text's tail
				memcpy(padded + lead, slot.Want, length);
				if (ISOBUSvt->send_change_string_value(slot.ID, slot.Length, padded))
				{
					strcpy(slot.Sent, slot.Want);
					slot.SentValid = true;
				}
			}
		}
	}
}

// ---- connection ----

FLASHMEM void VT_LogState()
{
	static uint32_t lastPrint = 0;
	static bool lastPartnerValid = false;
	static bool lastConnected = false;

	bool partnerValid = (ISOBUSvtPartner != nullptr) && ISOBUSvtPartner->get_address_valid();
	bool connected = ISOBUSvt->get_is_connected();
	if ((partnerValid != lastPartnerValid) || (connected != lastConnected) || (!connected && (millis() - lastPrint >= 5000)))
	{
		lastPrint = millis();
		lastPartnerValid = partnerValid;
		lastConnected = connected;

		Serial.print(F("VT: terminal "));
		Serial.print(partnerValid ? F("found") : F("waiting"));
		if (partnerValid)
		{
			Serial.print(F(" addr=0x"));
			Serial.print(ISOBUSvtPartner->get_address(), HEX);
		}
		Serial.print(F(" connected="));
		Serial.println(connected ? F("yes") : F("no"));
	}
}

FLASHMEM void VT_Begin()
{
	const isobus::NAMEFilter filterVT(isobus::NAME::NAMEParameters::FunctionCode, static_cast<std::uint8_t>(isobus::NAME::Function::VirtualTerminal));
	const std::vector<isobus::NAMEFilter> vtNameFilters = { filterVT };
	ISOBUSvtPartner = isobus::CANNetworkManager::CANNetwork.create_partnered_control_function(0, vtNameFilters);

	ISOBUSvt = std::make_shared<isobus::VirtualTerminalClient>(ISOBUSvtPartner, ISOBUSicf);
	VT_BuildPool();
	VT_InitSlots();
	VT_ForgetSent();
	// pointer form: the stack's vector overload is never uploaded (it leaves the data pointer null).
	// VT_Pool is not changed after this, so the pointer stays valid.
	ISOBUSvt->set_object_pool(0, VT_Pool.data(), static_cast<uint32_t>(VT_Pool.size()), VT_Label);
	ISOBUSvt->set_object_pool_scaling(0, VT_MASK_SIZE, VT_DESIGNATOR_HEIGHT);
	VT_KeyListener = ISOBUSvt->get_vt_soft_key_event_dispatcher().add_listener(VT_OnKey);
	VT_NumberListener = ISOBUSvt->get_vt_change_numeric_value_event_dispatcher().add_listener(VT_OnNumber);
	VT_ButtonListener = ISOBUSvt->get_vt_button_event_dispatcher().add_listener(VT_OnButton);
	ISOBUSvt->initialize(false);

	Serial.print(F("VT: pool "));
	Serial.print(VT_Pool.size());
	Serial.print(F(" B, label "));
	Serial.println(VT_Label);
}

void VT_Update()
{
	ISOBUSvt->update();
	VT_LogState();

	bool connected = ISOBUSvt->get_is_connected();
	if (connected && !VT_WasConnected)
	{
		VT_ForgetSent();
		VT_ActiveMask = VT_ID_MASK;				// the working set's active mask after a (re)load
		VT_LastScan = millis() - VT_SCAN_MS;	// scan at once
	}
	if (!connected) VT_StopHold();
	VT_WasConnected = connected;

	if (connected)
	{
		VT_UpdateHold();
		if (millis() - VT_LastScan >= VT_SCAN_MS)
		{
			VT_LastScan = millis();
			VT_Scan();
		}
		VT_SendOneString();
	}
}

#endif
