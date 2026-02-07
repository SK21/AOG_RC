
// DDOP.ino - Device Descriptor Object Pool builder for TC Client
// Builds ISO 11783-10 DDOP for rate control application

//=============================================================================
// DDOP Constants
//=============================================================================

// Object Types (ISO 11783-10)
#define OBJECT_TYPE_DEVICE                    0
#define OBJECT_TYPE_DEVICE_ELEMENT            1
#define OBJECT_TYPE_DEVICE_PROCESS_DATA       2
#define OBJECT_TYPE_DEVICE_PROPERTY           3
#define OBJECT_TYPE_DEVICE_VALUE_PRESENTATION 4

// Device Element Types
#define ELEMENT_TYPE_DEVICE                   1   // Root device
#define ELEMENT_TYPE_FUNCTION                 2   // Functional element (boom, bin, etc.)
#define ELEMENT_TYPE_CONNECTOR                4   // Connector
#define ELEMENT_TYPE_SECTION                  6   // Section/bin

// Process Data Trigger Methods
#define TRIGGER_TIME_INTERVAL                 1
#define TRIGGER_DISTANCE_INTERVAL             2
#define TRIGGER_THRESHOLD_LIMITS              4
#define TRIGGER_ON_CHANGE                     8
#define TRIGGER_TOTAL                         16

// DDOP Buffer
#define MAX_DDOP_SIZE                         512
uint8_t ddopBuffer[MAX_DDOP_SIZE];
uint16_t ddopSize = 0;
uint16_t ddopWritePos = 0;

// Object IDs - must be unique within DDOP
#define OBJECT_ID_DEVICE                      0x0001
#define OBJECT_ID_BOOM                        0x0002
#define OBJECT_ID_SECTION_1                   0x0003
#define OBJECT_ID_SECTION_2                   0x0004
#define OBJECT_ID_DPD_SETPOINT_1              0x0010   // DDI 1 - Setpoint Volume Per Area
#define OBJECT_ID_DPD_ACTUAL_RATE_1           0x0011   // DDI 2 - Actual Volume Per Area
#define OBJECT_ID_DPD_QUANTITY_1              0x0012   // DDI 48 - Actual Volume
#define OBJECT_ID_DPD_SECTION_STATE_1         0x0013   // DDI 157 - Section Control State
#define OBJECT_ID_DPD_SETPOINT_2              0x0020
#define OBJECT_ID_DPD_ACTUAL_RATE_2           0x0021
#define OBJECT_ID_DPD_QUANTITY_2              0x0022
#define OBJECT_ID_DPD_SECTION_STATE_2         0x0023
#define OBJECT_ID_DVP_RATE                    0x0100   // Value presentation for rate
#define OBJECT_ID_DVP_VOLUME                  0x0101   // Value presentation for volume

// Structure Label - change if DDOP format changes
const char STRUCTURE_LABEL[8] = "RCTNSY1";

//=============================================================================
// DDOP Builder Functions
//=============================================================================

void DDOP_ResetBuffer() {
    ddopWritePos = 0;
    ddopSize = 0;
    memset(ddopBuffer, 0xFF, MAX_DDOP_SIZE);
}

void DDOP_WriteByte(uint8_t value) {
    if (ddopWritePos < MAX_DDOP_SIZE) {
        ddopBuffer[ddopWritePos++] = value;
    }
}

void DDOP_WriteUint16(uint16_t value) {
    DDOP_WriteByte(value & 0xFF);
    DDOP_WriteByte((value >> 8) & 0xFF);
}

void DDOP_WriteUint32(uint32_t value) {
    DDOP_WriteByte(value & 0xFF);
    DDOP_WriteByte((value >> 8) & 0xFF);
    DDOP_WriteByte((value >> 16) & 0xFF);
    DDOP_WriteByte((value >> 24) & 0xFF);
}

void DDOP_WriteString(const char* str, uint8_t maxLen) {
    uint8_t len = strlen(str);
    if (len > maxLen) len = maxLen;
    DDOP_WriteByte(len);  // Length prefix
    for (int i = 0; i < len; i++) {
        DDOP_WriteByte(str[i]);
    }
}

void DDOP_WriteBytes(const uint8_t* data, uint8_t len) {
    for (int i = 0; i < len; i++) {
        DDOP_WriteByte(data[i]);
    }
}

//=============================================================================
// Object Builders
//=============================================================================

void DDOP_AddDevice() {
    // Device Object (DVC) - ISO 11783-10 Table A.1
    // This is the root object describing the entire device

    DDOP_WriteByte(OBJECT_TYPE_DEVICE);  // Object type
    DDOP_WriteUint16(OBJECT_ID_DEVICE);  // Object ID

    // Designator (device name)
    const char* designator = "RC Module";
    DDOP_WriteString(designator, 32);

    // Software version
    char version[8];
    snprintf(version, sizeof(version), "v%d", InoID);
    DDOP_WriteString(version, 32);

    // Working Set Master NAME (8 bytes) - our ISO NAME
    uint64_t name = buildIsobusNAME();
    for (int i = 0; i < 8; i++) {
        DDOP_WriteByte((name >> (i * 8)) & 0xFF);
    }

    // Serial number
    char serial[16];
    snprintf(serial, sizeof(serial), "M%02d", MDL.ID);
    DDOP_WriteString(serial, 32);

    // Structure label (7 bytes)
    DDOP_WriteBytes((const uint8_t*)STRUCTURE_LABEL, 7);

    // Localization label (7 bytes) - default
    uint8_t locLabel[7] = {0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF};
    DDOP_WriteBytes(locLabel, 7);

    // Extended structure label (empty)
    DDOP_WriteByte(0);

    // Device Element object references - list of child elements
    uint8_t numElements = 1 + MDL.SensorCount;  // Boom + sections
    DDOP_WriteByte(numElements);
    DDOP_WriteUint16(OBJECT_ID_BOOM);
    for (int i = 0; i < MDL.SensorCount; i++) {
        DDOP_WriteUint16(OBJECT_ID_SECTION_1 + i);
    }
}

void DDOP_AddBoom() {
    // Device Element (DET) - Boom
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_ELEMENT);
    DDOP_WriteUint16(OBJECT_ID_BOOM);

    // Element type: Function (boom)
    DDOP_WriteByte(ELEMENT_TYPE_FUNCTION);

    // Designator
    DDOP_WriteString("Boom", 32);

    // Element number (matches ELEMENT_BOOM constant)
    DDOP_WriteUint16(ELEMENT_BOOM);

    // Parent object ID (device)
    DDOP_WriteUint16(OBJECT_ID_DEVICE);

    // Number of device process data objects
    DDOP_WriteByte(0);  // Boom has no process data, sections do
}

void DDOP_AddSection(uint8_t sensorId) {
    // Device Element (DET) - Section
    uint16_t objectId = OBJECT_ID_SECTION_1 + sensorId;
    uint16_t dpdSetpoint = OBJECT_ID_DPD_SETPOINT_1 + (sensorId * 0x10);
    uint16_t dpdActualRate = OBJECT_ID_DPD_ACTUAL_RATE_1 + (sensorId * 0x10);
    uint16_t dpdQuantity = OBJECT_ID_DPD_QUANTITY_1 + (sensorId * 0x10);
    uint16_t dpdSectionState = OBJECT_ID_DPD_SECTION_STATE_1 + (sensorId * 0x10);

    DDOP_WriteByte(OBJECT_TYPE_DEVICE_ELEMENT);
    DDOP_WriteUint16(objectId);

    // Element type: Section
    DDOP_WriteByte(ELEMENT_TYPE_SECTION);

    // Designator
    char name[16];
    snprintf(name, sizeof(name), "Section %d", sensorId + 1);
    DDOP_WriteString(name, 32);

    // Element number
    DDOP_WriteUint16(ELEMENT_SECTION_BASE + sensorId);

    // Parent object ID (boom)
    DDOP_WriteUint16(OBJECT_ID_BOOM);

    // Number of device process data objects
    DDOP_WriteByte(4);
    DDOP_WriteUint16(dpdSetpoint);
    DDOP_WriteUint16(dpdActualRate);
    DDOP_WriteUint16(dpdQuantity);
    DDOP_WriteUint16(dpdSectionState);

    // Now add the process data objects for this section
    DDOP_AddProcessDataSetpoint(sensorId, dpdSetpoint);
    DDOP_AddProcessDataActualRate(sensorId, dpdActualRate);
    DDOP_AddProcessDataQuantity(sensorId, dpdQuantity);
    DDOP_AddProcessDataSectionState(sensorId, dpdSectionState);
}

void DDOP_AddProcessDataSetpoint(uint8_t sensorId, uint16_t objectId) {
    // DDI 1: Setpoint Volume Per Area Application Rate (TC -> Device)
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_PROCESS_DATA);
    DDOP_WriteUint16(objectId);

    // DDI
    DDOP_WriteUint16(DDI_SETPOINT_VOLUME_PER_AREA);

    // Properties (bit mask)
    // Bit 0: Property is settable
    // Bit 1: Property is settable without starting task
    uint8_t properties = 0x03;  // Settable
    DDOP_WriteByte(properties);

    // Trigger methods
    DDOP_WriteByte(TRIGGER_ON_CHANGE);

    // Designator
    DDOP_WriteString("Setpoint Rate", 32);

    // Device value presentation object reference
    DDOP_WriteUint16(OBJECT_ID_DVP_RATE);
}

void DDOP_AddProcessDataActualRate(uint8_t sensorId, uint16_t objectId) {
    // DDI 2: Actual Volume Per Area Application Rate (Device -> TC)
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_PROCESS_DATA);
    DDOP_WriteUint16(objectId);

    // DDI
    DDOP_WriteUint16(DDI_ACTUAL_VOLUME_PER_AREA);

    // Properties
    uint8_t properties = 0x00;  // Read-only (TC reads from us)
    DDOP_WriteByte(properties);

    // Trigger methods - we send on time interval
    DDOP_WriteByte(TRIGGER_TIME_INTERVAL | TRIGGER_ON_CHANGE);

    // Designator
    DDOP_WriteString("Actual Rate", 32);

    // Device value presentation object reference
    DDOP_WriteUint16(OBJECT_ID_DVP_RATE);
}

void DDOP_AddProcessDataQuantity(uint8_t sensorId, uint16_t objectId) {
    // DDI 48: Actual Volume (Device -> TC)
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_PROCESS_DATA);
    DDOP_WriteUint16(objectId);

    // DDI
    DDOP_WriteUint16(DDI_ACTUAL_VOLUME);

    // Properties
    uint8_t properties = 0x00;  // Read-only
    DDOP_WriteByte(properties);

    // Trigger methods
    DDOP_WriteByte(TRIGGER_TIME_INTERVAL | TRIGGER_TOTAL);

    // Designator
    DDOP_WriteString("Total Applied", 32);

    // Device value presentation object reference
    DDOP_WriteUint16(OBJECT_ID_DVP_VOLUME);
}

void DDOP_AddProcessDataSectionState(uint8_t sensorId, uint16_t objectId) {
    // DDI 157: Section Control State (TC -> Device)
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_PROCESS_DATA);
    DDOP_WriteUint16(objectId);

    // DDI
    DDOP_WriteUint16(DDI_SECTION_CONTROL_STATE);

    // Properties
    uint8_t properties = 0x03;  // Settable
    DDOP_WriteByte(properties);

    // Trigger methods
    DDOP_WriteByte(TRIGGER_ON_CHANGE);

    // Designator
    DDOP_WriteString("Section State", 32);

    // No value presentation for state
    DDOP_WriteUint16(0xFFFF);
}

void DDOP_AddValuePresentationRate() {
    // Device Value Presentation (DVP) for rate values
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_VALUE_PRESENTATION);
    DDOP_WriteUint16(OBJECT_ID_DVP_RATE);

    // Offset (int32)
    DDOP_WriteUint32(0);

    // Scale (float as fixed point: actual = raw * scale + offset)
    // DDI 1,2 are in mm³/m² = 0.01 L/ha, so scale = 0.01
    // Store as 0.01 in IEEE 754 float format
    float scale = 0.01f;
    uint32_t scaleBytes;
    memcpy(&scaleBytes, &scale, 4);
    DDOP_WriteUint32(scaleBytes);

    // Number of decimals
    DDOP_WriteByte(2);

    // Unit designator
    DDOP_WriteString("L/ha", 32);
}

void DDOP_AddValuePresentationVolume() {
    // Device Value Presentation (DVP) for volume values
    DDOP_WriteByte(OBJECT_TYPE_DEVICE_VALUE_PRESENTATION);
    DDOP_WriteUint16(OBJECT_ID_DVP_VOLUME);

    // Offset
    DDOP_WriteUint32(0);

    // Scale: DDI 48 is in mL, we want to display L
    float scale = 0.001f;
    uint32_t scaleBytes;
    memcpy(&scaleBytes, &scale, 4);
    DDOP_WriteUint32(scaleBytes);

    // Number of decimals
    DDOP_WriteByte(3);

    // Unit designator
    DDOP_WriteString("L", 32);
}

//=============================================================================
// Main Build Function
//=============================================================================

void DDOP_Build() {
    Serial.println("Building DDOP...");

    DDOP_ResetBuffer();

    // Add all objects in order
    DDOP_AddDevice();
    DDOP_AddBoom();

    for (int i = 0; i < MDL.SensorCount && i < MaxProductCount; i++) {
        DDOP_AddSection(i);
    }

    // Add value presentations
    DDOP_AddValuePresentationRate();
    DDOP_AddValuePresentationVolume();

    ddopSize = ddopWritePos;

    Serial.print("DDOP built, size: ");
    Serial.print(ddopSize);
    Serial.println(" bytes");
}

//=============================================================================
// Access Functions
//=============================================================================

const uint8_t* DDOP_GetBuffer() {
    return ddopBuffer;
}

uint16_t DDOP_GetSize() {
    return ddopSize;
}

const char* DDOP_GetStructureLabel() {
    return STRUCTURE_LABEL;
}
