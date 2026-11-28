// ISOBUS_VT.ino - run screen on a virtual terminal (ISOBUS build with ISOBUS_VT only)
// Group 3 step 1 (IsoBus\Group3_Step1_VT_Run_Screen_Design.md): one data mask, display only, no
// soft keys or inputs. Per product: target, actual, unit and state; the sections; the command
// source (RC / TC / None); speed, width and the TC link. No RC link line: RC sends no rate
// commands to an ISOBUS product's board (it goes through AOG-TaskController), and Source already
// shows RC when RC drives the board.
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
// Not here yet: soft keys and inputs (step 2), imperial units, the address claim re-request
// (AOG-TaskController bdf83e4; only if bench test 4 shows the race).

#if ISOBUS_ENABLED && ISOBUS_VT

// ---- ISO 11783-6 object types ----
const uint8_t VT_TYPE_WORKING_SET = 0;
const uint8_t VT_TYPE_DATA_MASK = 1;
const uint8_t VT_TYPE_CONTAINER = 3;
const uint8_t VT_TYPE_OUTPUT_STRING = 11;
const uint8_t VT_TYPE_OUTPUT_NUMBER = 12;
const uint8_t VT_TYPE_OUTPUT_RECTANGLE = 14;
const uint8_t VT_TYPE_NUMBER_VARIABLE = 21;
const uint8_t VT_TYPE_FONT_ATTRIBUTES = 23;
const uint8_t VT_TYPE_LINE_ATTRIBUTES = 24;
const uint8_t VT_TYPE_FILL_ATTRIBUTES = 25;

// ---- colours (VT standard palette), fonts, justification ----
const uint8_t VT_BLACK = 0;
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
const uint16_t VT_ID_MASK = 1000;
const uint16_t VT_ID_TITLE = 2000;
const uint16_t VT_ID_MODULE = 2001;
const uint16_t VT_ID_SOURCE = 2002;
const uint16_t VT_ID_HEAD_BASE = 2090;		// 2090-2094 column heads
const uint16_t VT_ID_ROW_BASE = 2100;		// row r: 2100 + 10 r + offset below
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

// text: the scan sets Want, VT_SendOneString sends the first that differs from Sent
const uint8_t VT_MAX_TEXT = 15;
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
	VT_SLOT_MODULE,
	VT_SLOT_SOURCE,
	VT_SLOT_UNIT_BASE,							// + r
	VT_SLOT_STATE_BASE = VT_SLOT_UNIT_BASE + VT_ROWS,	// + r
	VT_SLOT_SPEED_SOURCE = VT_SLOT_STATE_BASE + VT_ROWS,
	VT_SLOT_TC_LINK,
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

	void DataMask(uint16_t id, const VT_Child* list, uint8_t count)
	{
		header(id, VT_TYPE_DATA_MASK);
		u8(VT_SILVER);
		u16(isobus::NULL_OBJECT_ID);	// no soft key mask (display only)
		u8(count);
		u8(0);				// macros
		children(list, count);
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

	// one decimal: the variable holds tenths
	void OutputNumber(uint16_t id, uint16_t width, uint16_t height, uint16_t font, uint16_t variable)
	{
		header(id, VT_TYPE_OUTPUT_NUMBER);
		u16(width);
		u16(height);
		u8(VT_SILVER);
		u16(font);
		u8(VT_TRANSPARENT);
		u16(variable);
		u32(0);				// value (the variable's is used)
		u32(0);				// offset
		f32(0.1f);			// scale
		u8(1);				// decimals
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
	const uint8_t MaskCount = 3 + 5 + VT_ROWS + VT_SECTIONS + 8;	// header, heads, rows, sections, footer (number variables are not children)
	VT_Child mask[MaskCount];
	uint8_t n = 0;
	mask[n++] = { VT_ID_TITLE, 5, 10 };
	mask[n++] = { VT_ID_MODULE, 195, 14 };
	mask[n++] = { VT_ID_SOURCE, 330, 14 };
	const int16_t HeadX[5] = { 5, 105, 185, 280, 380 };
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
	w.DataMask(VT_ID_MASK, mask, n);

	// header and column heads
	w.OutputString(VT_ID_TITLE, 180, 30, VT_ID_FONT_LARGE, 11, "RateControl");
	w.OutputString(VT_ID_MODULE, 110, 20, VT_ID_FONT_NORMAL, 9, "Module");
	w.OutputString(VT_ID_SOURCE, 145, 20, VT_ID_FONT_NORMAL, 12, "Source:");
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
			{ VT_RowID(r, VT_ROW_NAME), 5, 10 },
			{ VT_RowID(r, VT_ROW_TARGET), 105, 10 },
			{ VT_RowID(r, VT_ROW_ACTUAL), 185, 10 },
			{ VT_RowID(r, VT_ROW_UNIT), 280, 10 },		// 15 gap after the right-aligned actual (ends at 265)
			{ VT_RowID(r, VT_ROW_STATE), 380, 10 } };	// 8 characters x 12 = 96, ends at 476
		w.Container(VT_RowID(r, VT_ROW_CONTAINER), VT_MASK_SIZE, 40, row, 5);

		char name[10];
		snprintf(name, sizeof(name), "Product %u", r + 1);
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
	VT_SetSlot(VT_SLOT_MODULE, VT_ID_MODULE, 9);
	VT_SetSlot(VT_SLOT_SOURCE, VT_ID_SOURCE, 12);
	for (uint8_t r = 0; r < VT_ROWS; r++)
	{
		VT_SetSlot(VT_SLOT_UNIT_BASE + r, VT_RowID(r, VT_ROW_UNIT), 8);
		VT_SetSlot(VT_SLOT_STATE_BASE + r, VT_RowID(r, VT_ROW_STATE), 8);
	}
	VT_SetSlot(VT_SLOT_SPEED_SOURCE, VT_ID_SPEED_SOURCE, 14);
	VT_SetSlot(VT_SLOT_TC_LINK, VT_ID_TC_LINK, 13);
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

void VT_Scan()
{
	uint8_t budget = VT_SCAN_BUDGET;
	char text[VT_MAX_TEXT + 1];

	// header and designator
	snprintf(text, sizeof(text), "RC %u", MDL.ID);
	VT_Want(VT_SLOT_DESIGNATOR, text);
	snprintf(text, sizeof(text), "Module %u", MDL.ID);
	VT_Want(VT_SLOT_MODULE, text);
	const char* source = TC_Terminal() ? "Term" : (TC_InCommand ? "TC" : (VT_AnySensorConnected() ? "RC" : "None"));
	snprintf(text, sizeof(text), "Source: %s", source);
	VT_Want(VT_SLOT_SOURCE, text);

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
			VT_Want(VT_SLOT_UNIT_BASE + r, VT_UnitText(r));
			VT_Want(VT_SLOT_STATE_BASE + r, VT_StateText(r));
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
	VT_Want(VT_SLOT_SPEED_SOURCE, VT_SpeedSourceText());
	VT_Want(VT_SLOT_TC_LINK, ISOBUStc->get_is_connected() ? "TC: connected" : "TC: waiting");
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
		VT_LastScan = millis() - VT_SCAN_MS;	// scan at once
	}
	VT_WasConnected = connected;

	if (connected)
	{
		if (millis() - VT_LastScan >= VT_SCAN_MS)
		{
			VT_LastScan = millis();
			VT_Scan();
		}
		VT_SendOneString();
	}
}

#endif
