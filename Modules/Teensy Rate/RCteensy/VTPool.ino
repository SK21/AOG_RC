
// VTPool.ino - ISO 11783-6 VT Object Pool Builder
// Builds binary VT object pool for rate display + section status
// Target: 200x200px minimum, 256-colour, VT version 3+

//=============================================================================
// VT Pool Buffer
//=============================================================================
#define MAX_VT_POOL_SIZE          1200
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

void VTPool_WriteString(const char* str) {
    uint16_t len = strlen(str);
    VTPool_WriteUint16(len);
    for (uint16_t i = 0; i < len; i++) {
        VTPool_WriteByte(str[i]);
    }
}

//=============================================================================
// Object Builders - ISO 11783-6 Binary Format
//=============================================================================

void VTPool_AddWorkingSet() {
    // Working Set object (type 0)
    VTPool_WriteByte(VT_TYPE_WORKING_SET);  // Object type
    VTPool_WriteUint16(VT_OBJ_WORKING_SET); // Object ID
    VTPool_WriteByte(VT_COLOUR_BLACK);      // Background colour
    VTPool_WriteByte(1);                    // Selectable: yes
    VTPool_WriteUint16(VT_OBJ_DATA_MASK);  // Active mask object ID
    // Number of object references (objects to include)
    VTPool_WriteByte(0);                    // No object refs at WS level
    // Number of macro references
    VTPool_WriteByte(0);
    // Number of language codes
    VTPool_WriteByte(1);
    // Language code "en"
    VTPool_WriteByte('e');
    VTPool_WriteByte('n');
}

void VTPool_AddDataMask() {
    // Data Mask object (type 1)
    VTPool_WriteByte(VT_TYPE_DATA_MASK);
    VTPool_WriteUint16(VT_OBJ_DATA_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);       // Background colour
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK); // Soft key mask reference

    // Count child objects - compute based on sensor count
    // Rate 1: label + actual + slash + target + unit = 5
    // Rate 2: label + actual + slash + target + unit = 5 (conditional)
    // Sections: up to 16 rectangles
    // Work indicator: 1
    uint8_t numObjects = 5 + 1;  // Rate1 (5) + work indicator (1)
    if (MDL.SensorCount > 1) numObjects += 5;  // Rate2
    // Add section rectangles (up to 16)
    uint8_t numSections = 16;  // Always define 16 section slots
    numObjects += numSections;

    VTPool_WriteByte(numObjects);

    // Rate 1 objects (x, y positions)
    // Label "Rate 1" at top-left
    VTPool_WriteUint16(VT_OBJ_STR_RATE1_LABEL);
    VTPool_WriteUint16(2);    // x
    VTPool_WriteUint16(2);    // y

    // Actual rate number
    VTPool_WriteUint16(VT_OBJ_NUM_RATE1_ACTUAL);
    VTPool_WriteUint16(50);   // x
    VTPool_WriteUint16(2);    // y

    // Slash separator
    VTPool_WriteUint16(VT_OBJ_STR_RATE1_SLASH);
    VTPool_WriteUint16(108);  // x
    VTPool_WriteUint16(2);    // y

    // Target rate number
    VTPool_WriteUint16(VT_OBJ_NUM_RATE1_TARGET);
    VTPool_WriteUint16(118);  // x
    VTPool_WriteUint16(2);    // y

    // Unit label
    VTPool_WriteUint16(VT_OBJ_STR_RATE1_UNIT);
    VTPool_WriteUint16(176);  // x
    VTPool_WriteUint16(2);    // y

    // Rate 2 objects (only if SensorCount > 1)
    if (MDL.SensorCount > 1) {
        VTPool_WriteUint16(VT_OBJ_STR_RATE2_LABEL);
        VTPool_WriteUint16(2);
        VTPool_WriteUint16(22);

        VTPool_WriteUint16(VT_OBJ_NUM_RATE2_ACTUAL);
        VTPool_WriteUint16(50);
        VTPool_WriteUint16(22);

        VTPool_WriteUint16(VT_OBJ_STR_RATE2_SLASH);
        VTPool_WriteUint16(108);
        VTPool_WriteUint16(22);

        VTPool_WriteUint16(VT_OBJ_NUM_RATE2_TARGET);
        VTPool_WriteUint16(118);
        VTPool_WriteUint16(22);

        VTPool_WriteUint16(VT_OBJ_STR_RATE2_UNIT);
        VTPool_WriteUint16(176);
        VTPool_WriteUint16(22);
    }

    // Section rectangles - arranged in a row near bottom
    uint16_t secY = (MDL.SensorCount > 1) ? 50 : 30;
    uint16_t secWidth = 12;
    uint16_t secSpacing = 12;
    for (uint8_t i = 0; i < numSections; i++) {
        VTPool_WriteUint16(VT_OBJ_SECTION_BASE + i);
        VTPool_WriteUint16(2 + i * secSpacing);  // x
        VTPool_WriteUint16(secY);                 // y
    }

    // Work switch indicator
    uint16_t workY = secY + 18;
    VTPool_WriteUint16(VT_OBJ_STR_WORK);
    VTPool_WriteUint16(2);
    VTPool_WriteUint16(workY);

    // Number of macro references
    VTPool_WriteByte(0);
}

void VTPool_AddSoftKeyMask() {
    // Soft Key Mask (type 4) - empty for v1
    VTPool_WriteByte(VT_TYPE_SOFT_KEY_MASK);
    VTPool_WriteUint16(VT_OBJ_SOFT_KEY_MASK);
    VTPool_WriteByte(VT_COLOUR_BLACK);  // Background colour
    VTPool_WriteByte(0);                // Number of objects (empty)
    VTPool_WriteByte(0);                // Number of macro refs
}

void VTPool_AddOutputString(uint16_t objId, const char* text, uint16_t width,
                            uint16_t fontRef, uint8_t bgColour) {
    // Output String (type 11)
    VTPool_WriteByte(VT_TYPE_OUTPUT_STRING);
    VTPool_WriteUint16(objId);
    VTPool_WriteUint16(width);         // Width
    VTPool_WriteUint16(16);            // Height
    VTPool_WriteByte(bgColour);        // Background colour
    VTPool_WriteUint16(fontRef);       // Font attributes reference
    VTPool_WriteByte(0);               // Options (0 = transparent bg if bgColour == VT_COLOUR_BLACK)
    VTPool_WriteUint16(0xFFFF);        // Variable reference (none)
    VTPool_WriteByte(0);               // Justification: left, top
    uint16_t len = strlen(text);
    VTPool_WriteUint16(len);           // String length
    for (uint16_t i = 0; i < len; i++) {
        VTPool_WriteByte(text[i]);
    }
    VTPool_WriteByte(0);               // Number of macro refs
}

void VTPool_AddOutputNumber(uint16_t objId, uint16_t width, uint16_t varRef,
                            uint16_t fontRef) {
    // Output Number (type 12)
    VTPool_WriteByte(VT_TYPE_OUTPUT_NUMBER);
    VTPool_WriteUint16(objId);
    VTPool_WriteUint16(width);         // Width
    VTPool_WriteUint16(16);            // Height
    VTPool_WriteByte(VT_COLOUR_BLACK); // Background colour
    VTPool_WriteUint16(fontRef);       // Font attributes reference
    VTPool_WriteByte(0);               // Options (transparent bg)
    VTPool_WriteUint16(varRef);        // Variable reference
    VTPool_WriteUint32(0);             // Offset (0)
    // Scale: 0.1 as IEEE 754 float
    float scale = 0.1f;
    uint32_t scaleBytes;
    memcpy(&scaleBytes, &scale, 4);
    VTPool_WriteUint32(scaleBytes);    // Scale
    VTPool_WriteByte(1);               // Number of decimals
    VTPool_WriteByte(0);               // Format: fixed decimal
    VTPool_WriteByte(0);               // Justification: left
    VTPool_WriteUint32(0);             // Min value
    VTPool_WriteUint32(99999);         // Max value
    VTPool_WriteByte(0);               // Number of macro refs
}

void VTPool_AddOutputRectangle(uint16_t objId, uint16_t width, uint16_t height,
                               uint16_t lineRef, uint16_t fillRef) {
    // Output Rectangle (type 14)
    VTPool_WriteByte(VT_TYPE_OUTPUT_RECTANGLE);
    VTPool_WriteUint16(objId);
    VTPool_WriteUint16(lineRef);       // Line attributes reference
    VTPool_WriteUint16(width);         // Width
    VTPool_WriteUint16(height);        // Height
    VTPool_WriteByte(1);               // Line suppression (0 = show all)
    VTPool_WriteUint16(fillRef);       // Fill attributes reference
    VTPool_WriteByte(0);               // Number of macro refs
}

void VTPool_AddNumberVariable(uint16_t objId, uint32_t initialValue) {
    // Number Variable (type 21)
    VTPool_WriteByte(VT_TYPE_NUMBER_VARIABLE);
    VTPool_WriteUint16(objId);
    VTPool_WriteUint32(initialValue);  // Value
}

void VTPool_AddFontAttributes(uint16_t objId, uint8_t fontSize, uint8_t colour) {
    // Font Attributes (type 24)
    // fontSize: 0=6x8, 1=8x8, 2=8x12, 3=12x16, 4=16x16, 5=16x24, 6=24x32, 7=32x32
    VTPool_WriteByte(VT_TYPE_FONT_ATTRIBUTES);
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(colour);          // Font colour
    VTPool_WriteByte(fontSize);        // Font size
    VTPool_WriteByte(0);               // Font type (0 = Latin 1)
    VTPool_WriteByte(0);               // Font style (0 = normal)
    VTPool_WriteByte(0);               // Number of macro refs
}

void VTPool_AddLineAttributes(uint16_t objId, uint8_t colour, uint8_t width) {
    // Line Attributes (type 25)
    VTPool_WriteByte(VT_TYPE_LINE_ATTRIBUTES);
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(colour);          // Line colour
    VTPool_WriteByte(width);           // Line width
    VTPool_WriteUint16(0xFFFF);        // Line art (solid)
    VTPool_WriteByte(0);               // Number of macro refs
}

void VTPool_AddFillAttributes(uint16_t objId, uint8_t fillType, uint8_t colour) {
    // Fill Attributes (type 26)
    // fillType: 0=no fill, 1=fill with colour, 2=fill with pattern
    VTPool_WriteByte(VT_TYPE_FILL_ATTRIBUTES);
    VTPool_WriteUint16(objId);
    VTPool_WriteByte(fillType);        // Fill type
    VTPool_WriteByte(colour);          // Fill colour
    VTPool_WriteUint16(0xFFFF);        // Fill pattern reference (none)
    VTPool_WriteByte(0);               // Number of macro refs
}

//=============================================================================
// Main Build Function
//=============================================================================

void VTPool_Build() {
    Serial.println("Building VT Object Pool...");

    VTPool_ResetBuffer();

    // Working Set
    VTPool_AddWorkingSet();

    // Data Mask (main screen)
    VTPool_AddDataMask();

    // Soft Key Mask (empty)
    VTPool_AddSoftKeyMask();

    // Font Attributes
    VTPool_AddFontAttributes(VT_OBJ_FONT_LARGE, 3, VT_COLOUR_WHITE);  // 12x16 white
    VTPool_AddFontAttributes(VT_OBJ_FONT_SMALL, 2, VT_COLOUR_WHITE);  // 8x12 white

    // Line Attributes
    VTPool_AddLineAttributes(VT_OBJ_LINE_THIN, VT_COLOUR_WHITE, 1);

    // Fill Attributes
    VTPool_AddFillAttributes(VT_OBJ_FILL_GREEN, 1, VT_COLOUR_GREEN);
    VTPool_AddFillAttributes(VT_OBJ_FILL_RED, 1, VT_COLOUR_RED);

    // Number Variables
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_ACTUAL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE1_TARGET, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE2_ACTUAL, 0);
    VTPool_AddNumberVariable(VT_OBJ_VAR_RATE2_TARGET, 0);

    // Rate 1 display objects
    VTPool_AddOutputString(VT_OBJ_STR_RATE1_LABEL, "Rate 1", 48,
                           VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_ACTUAL, 56,
                           VT_OBJ_VAR_RATE1_ACTUAL, VT_OBJ_FONT_LARGE);
    VTPool_AddOutputString(VT_OBJ_STR_RATE1_SLASH, "/", 10,
                           VT_OBJ_FONT_LARGE, VT_COLOUR_BLACK);
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE1_TARGET, 56,
                           VT_OBJ_VAR_RATE1_TARGET, VT_OBJ_FONT_LARGE);
    VTPool_AddOutputString(VT_OBJ_STR_RATE1_UNIT, "L/ha", 32,
                           VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    // Rate 2 display objects (always included in pool, visibility managed by VT)
    VTPool_AddOutputString(VT_OBJ_STR_RATE2_LABEL, "Rate 2", 48,
                           VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE2_ACTUAL, 56,
                           VT_OBJ_VAR_RATE2_ACTUAL, VT_OBJ_FONT_LARGE);
    VTPool_AddOutputString(VT_OBJ_STR_RATE2_SLASH, "/", 10,
                           VT_OBJ_FONT_LARGE, VT_COLOUR_BLACK);
    VTPool_AddOutputNumber(VT_OBJ_NUM_RATE2_TARGET, 56,
                           VT_OBJ_VAR_RATE2_TARGET, VT_OBJ_FONT_LARGE);
    VTPool_AddOutputString(VT_OBJ_STR_RATE2_UNIT, "L/ha", 32,
                           VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    // Section rectangles (16 sections)
    for (uint8_t i = 0; i < 16; i++) {
        // Start with red fill (section off)
        VTPool_AddOutputRectangle(VT_OBJ_SECTION_BASE + i, 10, 14,
                                  VT_OBJ_LINE_THIN, VT_OBJ_FILL_RED);
    }

    // Work switch indicator
    VTPool_AddOutputString(VT_OBJ_STR_WORK, "WORK", 40,
                           VT_OBJ_FONT_SMALL, VT_COLOUR_BLACK);

    vtPoolSize = vtPoolWritePos;

    Serial.print("VT Pool built, size: ");
    Serial.print(vtPoolSize);
    Serial.println(" bytes");
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
