
// VTPool.ino - ISO 11783-6 VT Object Pool Builder
// Builds binary VT object pool for rate display with interactive controls
// All positions and sizes scale proportionally to the VT display dimensions.
// Pool is rebuilt after Get Hardware response provides actual VT resolution.

//=============================================================================
// VT Pool Buffer
//=============================================================================
#define MAX_VT_POOL_SIZE          1800
uint8_t vtPoolBuffer[MAX_VT_POOL_SIZE];
uint16_t vtPoolSize = 0;
uint16_t vtPoolWritePos = 0;

//=============================================================================
// Pool Builder Helpers
//=============================================================================

void VTPool_ResetBuffer() {
    vtPoolWritePos = 0;
    vtPoolSize = 0;
    memset(vtPoolBuffer, 0xFF, MAX_VT_POOL_SIZE);
}

void VTPool_WriteByte(uint8_t value) {
    if (vtPoolWritePos < MAX_VT_POOL_SIZE) {
        vtPoolBuffer[vtPoolWritePos++] = value;
    }
}

void VTPool_WriteUint16(uint16_t value) {
    VTPool_WriteByte(value & 0xFF);
    VTPool_WriteByte((value >> 8) & 0xFF);
}

void VTPool_WriteUint32(uint32_t value) {
    VTPool_WriteByte(value & 0xFF);
    VTPool_WriteByte((value >> 8) & 0xFF);
    VTPool_WriteByte((value >> 16) & 0xFF);
    VTPool_WriteByte((value >> 24) & 0xFF);
}

void VTPool_WriteFloat(float value) {
    uint32_t bytes;
    memcpy(&bytes, &value, 4);
    VTPool_WriteUint32(bytes);
}

//=============================================================================
// Object Builders - ISO 11783-6 Binary Format
//=============================================================================

void VTPool_AddWorkingSet() {
    VTPool_WriteUint16(VT_OBJ_WORKING_SET);
    VTPool_WriteByte(VT_TYPE_WORKING_SET);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteByte(1);                    // Selectable
    VTPool_WriteUint16(VT_OBJ_DATA_MASK);  // Active mask
    VTPool_WriteByte(0);                    // No object refs
    VTPool_WriteByte(0);                    // No macros
    VTPool_WriteByte(1);                    // 1 language
    VTPool_WriteByte('e');
    VTPool_WriteByte('n');
}

void VTPool_AddOutputString(uint16_t objId, const char* text, uint16_t width,
                            uint16_t height, uint16_t fontRef, uint8_t bgColour) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_OUTPUT_STRING);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(bgColour);
    VTPool_WriteUint16(fontRef);
    VTPool_WriteByte(0);               // Options (transparent)
    VTPool_WriteUint16(0xFFFF);        // No variable ref
    VTPool_WriteByte(0);               // Justify: left
    uint16_t len = strlen(text);
    VTPool_WriteUint16(len);
    for (uint16_t i = 0; i < len; i++) {
        VTPool_WriteByte(text[i]);
    }
    VTPool_WriteByte(0);               // No macros
}

void VTPool_AddOutputNumber(uint16_t objId, uint16_t width, uint16_t height,
                            uint16_t varRef, uint16_t fontRef, uint8_t justify) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_OUTPUT_NUMBER);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteUint16(fontRef);
    VTPool_WriteByte(0);               // Options (transparent)
    VTPool_WriteUint16(varRef);
    VTPool_WriteUint32(0);             // Value (initial)
    VTPool_WriteUint32(0);             // Offset
    VTPool_WriteFloat(0.1f);           // Scale = 0.1
    VTPool_WriteByte(1);               // 1 decimal
    VTPool_WriteByte(0);               // Fixed decimal
    VTPool_WriteByte(justify);         // 0=left, 2=right
    VTPool_WriteByte(0);               // No macros
}

void VTPool_AddOutputRectangle(uint16_t objId, uint16_t width, uint16_t height,
                               uint16_t lineRef, uint16_t fillRef) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_OUTPUT_RECTANGLE);
    VTPool_WriteUint16(lineRef);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(1);               // Line suppression
    VTPool_WriteUint16(fillRef);
    VTPool_WriteByte(0);               // No macros
}

void VTPool_AddNumberVariable(uint16_t objId, uint32_t initialValue) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_NUMBER_VARIABLE);
    VTPool_WriteUint32(initialValue);
}

void VTPool_AddFontAttributes(uint16_t objId, uint8_t fontSize, uint8_t colour) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_FONT_ATTRIBUTES);
    VTPool_WriteByte(colour);
    VTPool_WriteByte(fontSize);
    VTPool_WriteByte(0);  // Latin 1
    VTPool_WriteByte(0);  // Normal style
    VTPool_WriteByte(0);  // No macros
}

void VTPool_AddLineAttributes(uint16_t objId, uint8_t colour, uint8_t width) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_LINE_ATTRIBUTES);
    VTPool_WriteByte(colour);
    VTPool_WriteByte(width);
    VTPool_WriteUint16(0xFFFF);  // Solid line
    VTPool_WriteByte(0);         // No macros
}

void VTPool_AddFillAttributes(uint16_t objId, uint8_t fillType, uint8_t colour) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_FILL_ATTRIBUTES);
    VTPool_WriteByte(fillType);
    VTPool_WriteByte(colour);
    VTPool_WriteUint16(0xFFFF);  // No pattern
    VTPool_WriteByte(0);         // No macros
}

// Button (type 6) - 13 bytes + children
// Options: bit0=latchable, bit3=transparent bg, bit5=no border
void VTPool_AddButton(uint16_t objId, uint16_t width, uint16_t height,
                      uint8_t bgColour, uint8_t borderColour, uint8_t keyCode,
                      uint8_t options) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_BUTTON);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(bgColour);
    VTPool_WriteByte(borderColour);
    VTPool_WriteByte(keyCode);
    VTPool_WriteByte(options);
    VTPool_WriteByte(0);  // No child objects
    VTPool_WriteByte(0);  // No macros
}

// Key/SoftKey (type 5) - 7 bytes min + children
void VTPool_AddKey(uint16_t objId, uint8_t bgColour, uint8_t keyCode,
                   uint8_t numChildren) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_KEY);
    VTPool_WriteByte(bgColour);
    VTPool_WriteByte(keyCode);
    VTPool_WriteByte(numChildren);
    VTPool_WriteByte(0);  // No macros
    // Caller writes child refs: ObjID(2LE)+X(2LE)+Y(2LE) each
}

// OutputLinearBarGraph (type 18) - 24 bytes
void VTPool_AddBarGraph(uint16_t objId, uint16_t width, uint16_t height,
                        uint8_t colour, uint16_t minVal, uint16_t maxVal,
                        uint16_t varRef, uint8_t options) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_OUTPUT_LINEAR_BAR_GRAPH);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(colour);            // Bar colour
    VTPool_WriteByte(VT_COLOUR_WHITE);   // Target line colour
    VTPool_WriteByte(options);           // bit0=border, bit4=horiz, bit5=grow positive
    VTPool_WriteByte(0);                 // No ticks
    VTPool_WriteUint16(minVal);
    VTPool_WriteUint16(maxVal);
    VTPool_WriteUint16(varRef);          // Variable reference
    VTPool_WriteUint16(0xFFFF);          // No target variable ref
    VTPool_WriteUint16(0);               // Target value (unused)
    VTPool_WriteUint16(0);               // Initial value
    VTPool_WriteByte(0);                 // No macros
}

// InputNumber (type 9) - 38 bytes
void VTPool_AddInputNumber(uint16_t objId, uint16_t width, uint16_t height,
                           uint16_t fontRef, uint16_t varRef,
                           uint32_t minVal, uint32_t maxVal,
                           uint8_t numDecimals) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_INPUT_NUMBER);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(VT_COLOUR_BLACK);   // BG colour (transparent)
    VTPool_WriteUint16(fontRef);
    VTPool_WriteByte(0x01);              // Options: bit0=transparent
    VTPool_WriteUint16(varRef);
    VTPool_WriteUint32(minVal);          // Min value
    VTPool_WriteUint32(maxVal);          // Max value
    VTPool_WriteUint32(0);               // Offset
    VTPool_WriteFloat(0.1f);             // Scale = 0.1
    VTPool_WriteByte(numDecimals);       // Number of decimals
    VTPool_WriteByte(0);                 // Format: fixed decimal
    VTPool_WriteByte(2);                 // Justify: right
    VTPool_WriteUint32(0);               // Initial value
    VTPool_WriteByte(0);                 // Options2
    VTPool_WriteByte(0);                 // No macros
}

//=============================================================================
// Main Build Function - scales to VT display dimensions
//=============================================================================

void VTPool_Build(uint16_t dispW, uint16_t dispH) {
    Serial.print("Building VT Pool for ");
    Serial.print(dispW);
    Serial.print("x");
    Serial.println(dispH);

    // Scale macros: map 200-baseline coordinates to actual display size
    #define SX(x) ((uint16_t)((uint32_t)(x) * dispW / 200))
    #define SY(y) ((uint16_t)((uint32_t)(y) * dispH / 200))

    // Choose font sizes based on display width
    // Size 2=8x12, 3=12x16, 5=16x24, 6=24x32
    uint8_t largeFontSize, smallFontSize;
    if (dispW >= 400) {
        largeFontSize = 6;  // 24x32
        smallFontSize = 5;  // 16x24
    } else if (dispW >= 280) {
        largeFontSize = 5;  // 16x24
        smallFontSize = 3;  // 12x16
    } else {
        largeFontSize = 3;  // 12x16
        smallFontSize = 2;  // 8x12
    }

    VTPool_ResetBuffer();

    // === Structural objects ===
    VTPool_AddWorkingSet();

    // --- Data Mask with child positions ---
    VTPool_WriteUint16(VT_OBJ_DATA_MASK);
    VTPool_WriteByte(VT_TYPE_DATA_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK);

    // Count children:
    // Header: AOG rect + AOG text + product text = 3
    // Section buttons: 8
    // Tank bar: 1
    // SensorCount=1: rate(label+unit+num) + target(label+unit+num) + applied(label+unit+num) + area(label+unit+num) + speed(label+unit+input) = 15
    // SensorCount=2: rate1(label+actual+target+unit) + rate2(label+actual+target+unit) + applied(label+num+unit) + area(label+num+unit) + speed(label+unit+input) = 17
    uint8_t numChildren;
    if (MDL.SensorCount > 1) {
        numChildren = 3 + 8 + 1 + 17;  // 29
    } else {
        numChildren = 3 + 8 + 1 + 15;  // 27
    }
    VTPool_WriteByte(numChildren);
    VTPool_WriteByte(0);  // No macros

    // --- Header row (y=2) ---
    // AOG indicator rectangle
    VTPool_WriteUint16(VT_OBJ_RECT_AOG);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(2));

    // AOG text overlay
    VTPool_WriteUint16(VT_OBJ_STR_AOG);
    VTPool_WriteUint16(SX(4));
    VTPool_WriteUint16(SY(2));

    // Product name
    VTPool_WriteUint16(VT_OBJ_STR_PRODUCT);
    VTPool_WriteUint16(SX(40));
    VTPool_WriteUint16(SY(2));

    // --- 8 Section buttons (y=152) ---
    for (uint8_t i = 0; i < 8; i++) {
        VTPool_WriteUint16(VT_OBJ_BTN_SECTION_BASE + i);
        VTPool_WriteUint16(SX(3 + i * 21));
        VTPool_WriteUint16(SY(152));
    }

    // --- Tank bar graph on right (x=176, y=18) ---
    VTPool_WriteUint16(VT_OBJ_BAR_TANK);
    VTPool_WriteUint16(SX(176));
    VTPool_WriteUint16(SY(18));

    if (MDL.SensorCount > 1) {
        // === Compact 2-sensor layout ===
        // Row 1 (y=20): Rate 1 - label + actual + target + unit
        VTPool_WriteUint16(VT_OBJ_STR_RATE1_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(22));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE1_ACTUAL);
        VTPool_WriteUint16(SX(50));
        VTPool_WriteUint16(SY(20));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE1_TARGET);
        VTPool_WriteUint16(SX(100));
        VTPool_WriteUint16(SY(20));

        VTPool_WriteUint16(VT_OBJ_STR_RATE1_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(22));

        // Row 2 (y=40): Rate 2
        VTPool_WriteUint16(VT_OBJ_STR_RATE2_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(42));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE2_ACTUAL);
        VTPool_WriteUint16(SX(50));
        VTPool_WriteUint16(SY(40));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE2_TARGET);
        VTPool_WriteUint16(SX(100));
        VTPool_WriteUint16(SY(40));

        VTPool_WriteUint16(VT_OBJ_STR_RATE2_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(42));

        // Row 3 (y=60): Applied
        VTPool_WriteUint16(VT_OBJ_STR_QTY_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(62));

        VTPool_WriteUint16(VT_OBJ_NUM_QTY_APPLIED);
        VTPool_WriteUint16(SX(50));
        VTPool_WriteUint16(SY(60));

        VTPool_WriteUint16(VT_OBJ_STR_QTY_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(62));

        // Row 4 (y=80): Area Rem
        VTPool_WriteUint16(VT_OBJ_STR_AREA_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(82));

        VTPool_WriteUint16(VT_OBJ_NUM_AREA_REM);
        VTPool_WriteUint16(SX(50));
        VTPool_WriteUint16(SY(80));

        VTPool_WriteUint16(VT_OBJ_STR_AREA_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(82));

        // Row 5 (y=100): Speed
        VTPool_WriteUint16(VT_OBJ_STR_SPEED_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(102));

        VTPool_WriteUint16(VT_OBJ_INPUT_SPEED);
        VTPool_WriteUint16(SX(50));
        VTPool_WriteUint16(SY(100));

        VTPool_WriteUint16(VT_OBJ_STR_SPEED_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(102));

    } else {
        // === Full 1-sensor layout ===
        // "Cur Rate" label (y=18) + value (y=30)
        VTPool_WriteUint16(VT_OBJ_STR_RATE1_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(18));

        VTPool_WriteUint16(VT_OBJ_STR_RATE1_UNIT);
        VTPool_WriteUint16(SX(138));
        VTPool_WriteUint16(SY(18));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE1_ACTUAL);
        VTPool_WriteUint16(SX(4));
        VTPool_WriteUint16(SY(30));

        // "VR Target" label (y=44) + value (y=56)
        VTPool_WriteUint16(VT_OBJ_STR_TARGET1_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(44));

        VTPool_WriteUint16(VT_OBJ_STR_TARGET1_UNIT);
        VTPool_WriteUint16(SX(138));
        VTPool_WriteUint16(SY(44));

        VTPool_WriteUint16(VT_OBJ_NUM_RATE1_TARGET);
        VTPool_WriteUint16(SX(4));
        VTPool_WriteUint16(SY(56));

        // "Applied" label (y=70) + value (y=82)
        VTPool_WriteUint16(VT_OBJ_STR_QTY_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(70));

        VTPool_WriteUint16(VT_OBJ_STR_QTY_UNIT);
        VTPool_WriteUint16(SX(156));
        VTPool_WriteUint16(SY(70));

        VTPool_WriteUint16(VT_OBJ_NUM_QTY_APPLIED);
        VTPool_WriteUint16(SX(4));
        VTPool_WriteUint16(SY(82));

        // "Area Rem" label (y=96) + value (y=108)
        VTPool_WriteUint16(VT_OBJ_STR_AREA_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(96));

        VTPool_WriteUint16(VT_OBJ_STR_AREA_UNIT);
        VTPool_WriteUint16(SX(148));
        VTPool_WriteUint16(SY(96));

        VTPool_WriteUint16(VT_OBJ_NUM_AREA_REM);
        VTPool_WriteUint16(SX(4));
        VTPool_WriteUint16(SY(108));

        // "Speed" label (y=122) + InputNumber (y=134) + unit
        VTPool_WriteUint16(VT_OBJ_STR_SPEED_LABEL);
        VTPool_WriteUint16(SX(2));
        VTPool_WriteUint16(SY(122));

        VTPool_WriteUint16(VT_OBJ_STR_SPEED_UNIT);
        VTPool_WriteUint16(SX(138));
        VTPool_WriteUint16(SY(122));

        VTPool_WriteUint16(VT_OBJ_INPUT_SPEED);
        VTPool_WriteUint16(SX(4));
        VTPool_WriteUint16(SY(134));
    }

    // === End of Data Mask children ===

    // --- Soft Key Mask with 4 keys (no X/Y, VT positions them) ---
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK);
    VTPool_WriteByte(VT_TYPE_SOFT_KEY_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteByte(4);  // 4 soft key children
    VTPool_WriteByte(0);  // No macros
    // Children are Key object IDs only (2 bytes each, no X/Y)
    VTPool_WriteUint16(VT_OBJ_SK_AUTO);
    VTPool_WriteUint16(VT_OBJ_SK_MASTER);
    VTPool_WriteUint16(VT_OBJ_SK_PROD_NEXT);
    VTPool_WriteUint16(VT_OBJ_SK_PROD_PREV);

    // === Attribute objects ===
    VTPool_AddFontAttributes(VT_OBJ_FONT_LARGE, largeFontSize, VT_COLOUR_WHITE);
    VTPool_AddFontAttributes(VT_OBJ_FONT_SMALL, smallFontSize, VT_COLOUR_WHITE);
    VTPool_AddFontAttributes(VT_OBJ_FONT_YELLOW, smallFontSize, VT_COLOUR_YELLOW);
    VTPool_AddLineAttributes(VT_OBJ_LINE_THIN, VT_COLOUR_WHITE, 1);
    VTPool_AddFillAttributes(VT_OBJ_FILL_GREEN, 1, VT_COLOUR_GREEN);
    VTPool_AddFillAttributes(VT_OBJ_FILL_RED, 1, VT_COLOUR_RED);
    VTPool_AddFillAttributes(VT_OBJ_FILL_GREY, 1, VT_COLOUR_GREY);

    // === Number Variables (8) ===
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_ACTUAL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_TARGET, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE2_ACTUAL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE2_TARGET, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_QTY_APPLIED, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_AREA_REM, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_TANK_LEVEL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_SPEED, 0);

    // === AOG indicator (rectangle + text) ===
    VTPool_AddOutputRectangle(VT_OBJ_RECT_AOG, SX(34), SY(14),
                              VT_OBJ_LINE_THIN, VT_OBJ_FILL_RED);
    VTPool_AddOutputString(VT_OBJ_STR_AOG, "AOG",
                           SX(30), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    // === Product name string ===
    VTPool_AddOutputString(VT_OBJ_STR_PRODUCT, "Product 1",
                           SX(80), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    // === 8 Section buttons ===
    for (uint8_t i = 0; i < 8; i++) {
        VTPool_AddButton(VT_OBJ_BTN_SECTION_BASE + i,
                         SX(19), SY(16),
                         VT_COLOUR_RED,       // BG colour (red = off)
                         VT_COLOUR_WHITE,     // Border colour
                         i + 1,               // Key codes 1-8
                         0);                  // Options: not latchable
    }

    // === Tank bar graph ===
    // Vertical, grows upward: options = bit0(border) | bit5(grow positive) = 0x21
    VTPool_AddBarGraph(VT_OBJ_BAR_TANK, SX(22), SY(130),
                       VT_COLOUR_GREEN, 0, 1000,
                       VT_OBJ_VAR_TANK_LEVEL, 0x21);

    // === Speed InputNumber ===
    if (MDL.SensorCount > 1) {
        VTPool_AddInputNumber(VT_OBJ_INPUT_SPEED, SX(90), SY(16),
                              VT_OBJ_FONT_LARGE, VT_OBJ_VAR_SPEED,
                              0, 500, 1);  // 0.0 - 50.0 km/h
    } else {
        VTPool_AddInputNumber(VT_OBJ_INPUT_SPEED, SX(130), SY(16),
                              VT_OBJ_FONT_LARGE, VT_OBJ_VAR_SPEED,
                              0, 500, 1);  // 0.0 - 50.0 km/h
    }

    // === Display strings and numbers (scaled) ===
    if (MDL.SensorCount > 1) {
        // Compact 2-sensor layout
        VTPool_AddOutputString(VT_OBJ_STR_RATE1_LABEL, "Rate 1",
                               SX(44), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_ACTUAL, SX(46), SY(16),
                               VT_OBJ_VAR_RATE1_ACTUAL, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_TARGET, SX(46), SY(16),
                               VT_OBJ_VAR_RATE1_TARGET, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputString(VT_OBJ_STR_RATE1_UNIT, "L/ha",
                               SX(28), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

        VTPool_AddOutputString(VT_OBJ_STR_RATE2_LABEL, "Rate 2",
                               SX(44), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE2_ACTUAL, SX(46), SY(16),
                               VT_OBJ_VAR_RATE2_ACTUAL, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE2_TARGET, SX(46), SY(16),
                               VT_OBJ_VAR_RATE2_TARGET, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputString(VT_OBJ_STR_RATE2_UNIT, "L/ha",
                               SX(28), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

        VTPool_AddOutputString(VT_OBJ_STR_QTY_LABEL, "Applied",
                               SX(44), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_QTY_APPLIED, SX(90), SY(16),
                               VT_OBJ_VAR_QTY_APPLIED, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputString(VT_OBJ_STR_QTY_UNIT, "L",
                               SX(16), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

        VTPool_AddOutputString(VT_OBJ_STR_AREA_LABEL, "Area Rem",
                               SX(56), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_AREA_REM, SX(90), SY(16),
                               VT_OBJ_VAR_AREA_REM, VT_OBJ_FONT_LARGE, 2);
        VTPool_AddOutputString(VT_OBJ_STR_AREA_UNIT, "ha",
                               SX(20), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

        VTPool_AddOutputString(VT_OBJ_STR_SPEED_LABEL, "Speed",
                               SX(44), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_SPEED_UNIT, "km/h",
                               SX(28), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    } else {
        // Full 1-sensor layout
        VTPool_AddOutputString(VT_OBJ_STR_RATE1_LABEL, "Cur Rate",
                               SX(64), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_RATE1_UNIT, "L/ha",
                               SX(32), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_ACTUAL, SX(130), SY(16),
                               VT_OBJ_VAR_RATE1_ACTUAL, VT_OBJ_FONT_LARGE, 2);

        VTPool_AddOutputString(VT_OBJ_STR_TARGET1_LABEL, "VR Target",
                               SX(72), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_TARGET1_UNIT, "L/ha",
                               SX(32), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_TARGET, SX(130), SY(16),
                               VT_OBJ_VAR_RATE1_TARGET, VT_OBJ_FONT_LARGE, 2);

        VTPool_AddOutputString(VT_OBJ_STR_QTY_LABEL, "Applied",
                               SX(56), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_QTY_UNIT, "L",
                               SX(16), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_QTY_APPLIED, SX(130), SY(16),
                               VT_OBJ_VAR_QTY_APPLIED, VT_OBJ_FONT_LARGE, 2);

        VTPool_AddOutputString(VT_OBJ_STR_AREA_LABEL, "Area Rem",
                               SX(64), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_AREA_UNIT, "ha",
                               SX(20), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
        VTPool_AddOutputNumber(VT_OBJ_NUM_AREA_REM, SX(130), SY(16),
                               VT_OBJ_VAR_AREA_REM, VT_OBJ_FONT_LARGE, 2);

        VTPool_AddOutputString(VT_OBJ_STR_SPEED_LABEL, "Speed",
                               SX(40), SY(12), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
        VTPool_AddOutputString(VT_OBJ_STR_SPEED_UNIT, "km/h",
                               SX(32), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    }

    // === Soft Key objects (4 keys with label children) ===

    // Auto key - 1 child (label string)
    VTPool_AddKey(VT_OBJ_SK_AUTO, VT_COLOUR_BLACK, VT_KEYCODE_AUTO, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_AUTO);
    VTPool_WriteUint16(0);  // X
    VTPool_WriteUint16(0);  // Y

    // Master key
    VTPool_AddKey(VT_OBJ_SK_MASTER, VT_COLOUR_BLACK, VT_KEYCODE_MASTER, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_MASTER);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(0);

    // Product Next key
    VTPool_AddKey(VT_OBJ_SK_PROD_NEXT, VT_COLOUR_BLACK, VT_KEYCODE_PROD_NEXT, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_PROD_NEXT);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(0);

    // Product Prev key
    VTPool_AddKey(VT_OBJ_SK_PROD_PREV, VT_COLOUR_BLACK, VT_KEYCODE_PROD_PREV, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_PROD_PREV);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(0);

    // Soft key label strings (small, VT sizes them to fit key area)
    VTPool_AddOutputString(VT_OBJ_STR_SK_AUTO, "Auto",
                           SX(48), SY(14), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_SK_MASTER, "Master",
                           SX(48), SY(14), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_SK_PROD_NEXT, "Prod+",
                           SX(48), SY(14), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_SK_PROD_PREV, "Prod-",
                           SX(48), SY(14), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    vtPoolSize = vtPoolWritePos;

    Serial.print("VT Pool built, size: ");
    Serial.print(vtPoolSize);
    Serial.println(" bytes");

    #undef SX
    #undef SY
}

//=============================================================================
// Access Functions
//=============================================================================

const uint8_t* VTPool_GetBuffer() {
    return vtPoolBuffer;
}

uint16_t VTPool_GetSize() {
    return vtPoolSize;
}
