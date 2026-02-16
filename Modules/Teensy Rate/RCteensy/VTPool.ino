
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
                            uint16_t height, uint16_t fontRef, uint8_t bgColour,
                            uint8_t justify = 0, uint8_t options = 1) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_OUTPUT_STRING);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(bgColour);
    VTPool_WriteUint16(fontRef);
    VTPool_WriteByte(options);         // Options: bit0: 0=opaque, 1=transparent bg
    VTPool_WriteUint16(0xFFFF);        // No variable ref
    VTPool_WriteByte(justify);         // 0=left, 1=center, 2=right
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
    VTPool_WriteByte(1);               // Options: bit0=1 = transparent background
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

// Button (type 6) - 13 bytes + 6 bytes per child
// Options: bit0=latchable, bit3=transparent bg, bit5=no border
// Caller writes child refs after: ObjID(2LE)+X(2LE)+Y(2LE) each
void VTPool_AddButton(uint16_t objId, uint16_t width, uint16_t height,
                      uint8_t bgColour, uint8_t borderColour, uint8_t keyCode,
                      uint8_t options, uint8_t numChildren) {
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(VT_TYPE_BUTTON);
    VTPool_WriteUint16(width);
    VTPool_WriteUint16(height);
    VTPool_WriteByte(bgColour);
    VTPool_WriteByte(borderColour);
    VTPool_WriteByte(keyCode);
    VTPool_WriteByte(options);
    VTPool_WriteByte(numChildren);
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
// Unified layout for all SensorCount values. Product buttons switch
// which product's data is displayed.
//=============================================================================

void VTPool_Build(uint16_t dispW, uint16_t dispH) {
    Serial.print("Building VT Pool for ");
    Serial.print(dispW);
    Serial.print("x");
    Serial.println(dispH);

    // Scale macros: map 200-baseline coordinates to actual display size
#define SX(x) ((uint16_t)((uint32_t)(x) * dispW / 200))
#define SY(y) ((uint16_t)((uint32_t)(y) * dispH / 200))

// ------------------------------------------------------------
// Soft-key label helper macros
// ------------------------------------------------------------

// Center a single-line soft key label vertically
#define ADD_SOFTKEY_LABEL(objId, text, fontObj)        \
    VTPool_AddOutputString(objId, text, SX(24), SY(7), fontObj, VT_COLOUR_BLACK, 1); \
    VTPool_WriteUint16(objId);                         \
    VTPool_WriteUint16(0);                             \
    VTPool_WriteUint16(SY(6));   /* vertical center */

// Center a two-line soft key label vertically
#define ADD_SOFTKEY_LABEL_2LINE(objId1, text1, objId2, text2, fontObj) \
    /* Line 1 */                                                        \
    VTPool_AddOutputString(objId1, text1, SX(24), SY(7), fontObj, VT_COLOUR_BLACK, 1); \
    /* Line 2 */                                                        \
    VTPool_AddOutputString(objId2, text2, SX(24), SY(7), fontObj, VT_COLOUR_BLACK, 1); \
    /* Attach children */                                               \
    VTPool_WriteUint16(objId1);                                         \
    VTPool_WriteUint16(0);                                              \
    VTPool_WriteUint16(SY(5));   /* top line */                         \
    VTPool_WriteUint16(objId2);                                         \
    VTPool_WriteUint16(0);                                              \
    VTPool_WriteUint16(SY(13));  /* bottom line */

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

    // --- Data Mask (32 children) ---
    VTPool_WriteUint16(VT_OBJ_DATA_MASK);
    VTPool_WriteByte(VT_TYPE_DATA_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK);
    VTPool_WriteByte(31);  // Total children
    VTPool_WriteByte(0);   // No macros

    // --- Product buttons (6) at y=7 ---
    for (uint8_t i = 0; i < 6; i++) {
        VTPool_WriteUint16(VT_OBJ_BTN_PROD_BASE + i);
        VTPool_WriteUint16(SX(1 + i * 33));
        VTPool_WriteUint16(SY(7));
    }

    // --- Header (4 children) at y=27 ---
    VTPool_WriteUint16(VT_OBJ_STR_AOG);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(27));

    VTPool_WriteUint16(VT_OBJ_NUM_SPEED);
    VTPool_WriteUint16(SX(22));
    VTPool_WriteUint16(SY(27));

    VTPool_WriteUint16(VT_OBJ_STR_SPEED_UNIT);
    VTPool_WriteUint16(SX(55));
    VTPool_WriteUint16(SY(29));

    // --- Product name on second header line (y=45) ---
    VTPool_WriteUint16(VT_OBJ_STR_PRODUCT);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(45));

    // --- Current Rate row (y=67) ---
    VTPool_WriteUint16(VT_OBJ_STR_RATE1_LABEL);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(69));

    VTPool_WriteUint16(VT_OBJ_NUM_RATE1_ACTUAL);
    VTPool_WriteUint16(SX(56));
    VTPool_WriteUint16(SY(67));

    VTPool_WriteUint16(VT_OBJ_STR_RATE1_UNIT);
    VTPool_WriteUint16(SX(130));
    VTPool_WriteUint16(SY(69));

    // --- Target Rate row (y=93) ---
    VTPool_WriteUint16(VT_OBJ_STR_TARGET1_LABEL);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(95));

    VTPool_WriteUint16(VT_OBJ_NUM_RATE1_TARGET);
    VTPool_WriteUint16(SX(56));
    VTPool_WriteUint16(SY(93));

    VTPool_WriteUint16(VT_OBJ_STR_TARGET1_UNIT);
    VTPool_WriteUint16(SX(130));
    VTPool_WriteUint16(SY(95));

    // --- Qty Applied row (y=120) ---
    VTPool_WriteUint16(VT_OBJ_STR_QTY_LABEL);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(122));

    VTPool_WriteUint16(VT_OBJ_NUM_QTY_APPLIED);
    VTPool_WriteUint16(SX(56));
    VTPool_WriteUint16(SY(120));

    VTPool_WriteUint16(VT_OBJ_STR_QTY_UNIT);
    VTPool_WriteUint16(SX(130));
    VTPool_WriteUint16(SY(122));

    // --- Area Remain row (y=147) ---
    VTPool_WriteUint16(VT_OBJ_STR_AREA_LABEL);
    VTPool_WriteUint16(SX(2));
    VTPool_WriteUint16(SY(149));

    VTPool_WriteUint16(VT_OBJ_NUM_AREA_REM);
    VTPool_WriteUint16(SX(56));
    VTPool_WriteUint16(SY(147));

    VTPool_WriteUint16(VT_OBJ_STR_AREA_UNIT);
    VTPool_WriteUint16(SX(130));
    VTPool_WriteUint16(SY(149));

    // --- Tank bar (right side) ---
    VTPool_WriteUint16(VT_OBJ_BAR_TANK);
    VTPool_WriteUint16(SX(172));
    VTPool_WriteUint16(SY(33));

    // --- Section buttons (8) at y=173 ---
    for (uint8_t i = 0; i < 8; i++) {
        VTPool_WriteUint16(VT_OBJ_BTN_SECTION_BASE + i);
        VTPool_WriteUint16(SX(1 + i * 25));
        VTPool_WriteUint16(SY(173));
    }

    // === End of Data Mask children (32 total) ===

    // --- Soft Key Mask with 6 action keys ---
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK);
    VTPool_WriteByte(VT_TYPE_SOFT_KEY_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);
    VTPool_WriteByte(6);   // 6 soft key children
    VTPool_WriteByte(0);   // No macros
    // Children are Key object IDs only (2 bytes each, no X/Y)
    VTPool_WriteUint16(VT_OBJ_SK_MENU);
    VTPool_WriteUint16(VT_OBJ_SK_RQTY);
    VTPool_WriteUint16(VT_OBJ_SK_RAREA);
    VTPool_WriteUint16(VT_OBJ_SK_RX);
    VTPool_WriteUint16(VT_OBJ_SK_AUTO);
    VTPool_WriteUint16(VT_OBJ_SK_MASTER);

    // === Attribute objects ===
    VTPool_AddFontAttributes(VT_OBJ_FONT_LARGE, largeFontSize, VT_COLOUR_YELLOW);
    VTPool_AddFontAttributes(VT_OBJ_FONT_SMALL, smallFontSize, VT_COLOUR_WHITE);
    VTPool_AddFontAttributes(VT_OBJ_FONT_YELLOW, smallFontSize, VT_COLOUR_YELLOW);

    uint8_t tinyFontSize = (smallFontSize > 2) ? (smallFontSize - 1) : smallFontSize;
    VTPool_AddFontAttributes(VT_OBJ_FONT_TINY, tinyFontSize, VT_COLOUR_WHITE);

    VTPool_AddLineAttributes(VT_OBJ_LINE_THIN, VT_COLOUR_WHITE, 1);
    VTPool_AddFillAttributes(VT_OBJ_FILL_GREEN, 1, VT_COLOUR_GREEN);
    VTPool_AddFillAttributes(VT_OBJ_FILL_RED, 1, VT_COLOUR_RED);
    VTPool_AddFillAttributes(VT_OBJ_FILL_GREY, 1, VT_COLOUR_GREY);

    // === Number Variables (6) ===
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_ACTUAL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_TARGET, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_QTY_APPLIED, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_AREA_REM, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_TANK_LEVEL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_SPEED, 0);

    // === AOG indicator (opaque background string, red=disconnected, green=connected) ===
    // Using OutputString with opaque bg (options=0) - change bg via attrID=2
    VTPool_AddOutputString(VT_OBJ_STR_AOG, "AOG",
                           SX(18), SY(14), VT_OBJ_FONT_SMALL, VT_COLOUR_RED,
                           1, 0);  // justify=center, options=0 (opaque bg)

    // === Product description string (updated via Change String Value) ===
    VTPool_AddOutputString(VT_OBJ_STR_PRODUCT, "P1",
                           SX(80), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);

    // === Speed unit and OutputNumber ===
    VTPool_AddOutputString(VT_OBJ_STR_SPEED_UNIT, "km/h",
                           SX(25), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputNumber(VT_OBJ_NUM_SPEED, SX(30), SY(14),
                           VT_OBJ_VAR_SPEED, VT_OBJ_FONT_YELLOW, 2);

    // === Data row labels (yellow small font) ===
    VTPool_AddOutputString(VT_OBJ_STR_RATE1_LABEL, "Cur Rate",
                           SX(52), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_TARGET1_LABEL, "Tgt Rate",
                           SX(52), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_QTY_LABEL, "Applied",
                           SX(52), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_AREA_LABEL, "Area Rem",
                           SX(52), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);

    // === Data row units (yellow small font) ===
    VTPool_AddOutputString(VT_OBJ_STR_RATE1_UNIT, "G/ac",
                           SX(38), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_TARGET1_UNIT, "G/ac",
                           SX(38), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_QTY_UNIT, "Gallons",
                           SX(38), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);
    VTPool_AddOutputString(VT_OBJ_STR_AREA_UNIT, "Acres",
                           SX(38), SY(14), VT_OBJ_FONT_YELLOW, VT_COLOUR_BLACK);

    // === Data row OutputNumbers (large yellow font, right-justified) ===
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_ACTUAL, SX(72), SY(18),
                           VT_OBJ_VAR_RATE1_ACTUAL, VT_OBJ_FONT_LARGE, 2);
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_TARGET, SX(72), SY(18),
                           VT_OBJ_VAR_RATE1_TARGET, VT_OBJ_FONT_LARGE, 2);
    VTPool_AddOutputNumber(VT_OBJ_NUM_QTY_APPLIED, SX(72), SY(18),
                           VT_OBJ_VAR_QTY_APPLIED, VT_OBJ_FONT_LARGE, 2);
    VTPool_AddOutputNumber(VT_OBJ_NUM_AREA_REM, SX(72), SY(18),
                           VT_OBJ_VAR_AREA_REM, VT_OBJ_FONT_LARGE, 2);

    // === 6 Product button label strings (centered) ===
    {
        const char* prodLabels[] = {"P1", "P2", "P3", "P4", "P5", "F"};
        for (uint8_t i = 0; i < 6; i++) {
            VTPool_AddOutputString(VT_OBJ_STR_BTN_PROD_BASE + i, prodLabels[i],
                                   SX(32), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK,
                                   1);  // justify=center
        }
    }

    // === 6 Product buttons (each with label child) ===
    for (uint8_t i = 0; i < 6; i++) {
        VTPool_AddButton(VT_OBJ_BTN_PROD_BASE + i,
                         SX(32), SY(13),
                         VT_COLOUR_BLUE,         // Inactive default (blue)
                         VT_COLOUR_BLACK,        // Border colour (hidden by no-border option)
                         VT_KEYCODE_PROD_BASE + i,  // Key codes 20-25
                         0x20, 1);               // bit5=no border, 1 child
        // Child: label string fills button width (centered via justify)
        VTPool_WriteUint16(VT_OBJ_STR_BTN_PROD_BASE + i);
        VTPool_WriteUint16(0);
        VTPool_WriteUint16(0);
    }

    // === 8 Section button label strings (centered) ===
    for (uint8_t i = 0; i < 8; i++) {
        char label[2] = { (char)('1' + i), '\0' };
        VTPool_AddOutputString(VT_OBJ_STR_BTN_SEC_BASE + i, label,
                               SX(24), SY(12), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK,
                               1);  // justify=center
    }

    // === 8 Section buttons (each with number label child) ===
    for (uint8_t i = 0; i < 8; i++) {
        VTPool_AddButton(VT_OBJ_BTN_SECTION_BASE + i,
                         SX(24), SY(20),
                         VT_COLOUR_RED,       // BG colour (red = off)
                         VT_COLOUR_BLACK,     // Border colour (hidden by no-border option)
                         i + 1,               // Key codes 1-8
                         0x20, 1);            // bit5=no border, 1 child
        // Child: label string fills button width (centered via justify)
        VTPool_WriteUint16(VT_OBJ_STR_BTN_SEC_BASE + i);
        VTPool_WriteUint16(0);
        VTPool_WriteUint16(SY(4));
    }

    // === Tank bar graph ===
    // Vertical, grows upward: options = bit0(border) | bit5(grow positive) = 0x21
    VTPool_AddBarGraph(VT_OBJ_BAR_TANK, SX(26), SY(135),
                       VT_COLOUR_GREEN, 0, 1000,
                       VT_OBJ_VAR_TANK_LEVEL, 0x21);

    // === Soft Key objects (6 action keys with label children) ===

    // MENU (1 child)
    VTPool_AddKey(VT_OBJ_SK_MENU, VT_COLOUR_BLUE, VT_KEYCODE_MENU, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_MENU);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(6));

    // RQTY (2 children)
    VTPool_AddKey(VT_OBJ_SK_RQTY, VT_COLOUR_BLUE, VT_KEYCODE_RQTY, 2);
    VTPool_WriteUint16(VT_OBJ_STR_SK_RQTY_L1);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(5));
    VTPool_WriteUint16(VT_OBJ_STR_SK_RQTY_L2);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(13));

    // RAREA (1 child)
    VTPool_AddKey(VT_OBJ_SK_RAREA, VT_COLOUR_BLUE, VT_KEYCODE_RAREA, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_RAREA);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(6));

    // RX (1 child)
    VTPool_AddKey(VT_OBJ_SK_RX, VT_COLOUR_BLUE, VT_KEYCODE_RX, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_RX);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(6));

    // AUTO (1 child)
    VTPool_AddKey(VT_OBJ_SK_AUTO, VT_COLOUR_BLUE, VT_KEYCODE_AUTO, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_AUTO);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(6));

    // MASTER (1 child)
    VTPool_AddKey(VT_OBJ_SK_MASTER, VT_COLOUR_BLUE, VT_KEYCODE_MASTER, 1);
    VTPool_WriteUint16(VT_OBJ_STR_SK_MASTER);
    VTPool_WriteUint16(0);
    VTPool_WriteUint16(SY(6));


    // === Soft Key Label Strings (must come AFTER all Key objects) ===

    // Single-line labels
    VTPool_AddOutputString(VT_OBJ_STR_SK_MENU, "Menu",
        SX(24), SY(7), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK, 1);

    VTPool_AddOutputString(VT_OBJ_STR_SK_RAREA, "RAra",
        SX(24), SY(7), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK, 1);

    VTPool_AddOutputString(VT_OBJ_STR_SK_RX, "Rx",
        SX(24), SY(7), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK, 1);

    VTPool_AddOutputString(VT_OBJ_STR_SK_AUTO, "Auto",
        SX(24), SY(7), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK, 1);

    VTPool_AddOutputString(VT_OBJ_STR_SK_MASTER, "Mstr",
        SX(24), SY(7), VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK, 1);

    // Two-line Rst Qty
    VTPool_AddOutputString(VT_OBJ_STR_SK_RQTY_L1, "Rst",
        SX(24), SY(7), VT_OBJ_FONT_TINY, VT_COLOUR_BLACK, 1);

    VTPool_AddOutputString(VT_OBJ_STR_SK_RQTY_L2, "Qty",
        SX(24), SY(7), VT_OBJ_FONT_TINY, VT_COLOUR_BLACK, 1);


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



