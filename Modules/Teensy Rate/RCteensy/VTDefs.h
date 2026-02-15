// VTDefs.h - ISO 11783-6 Virtual Terminal Client definitions
// This header must be included AFTER FlexCAN_T4.h and TCDefs.h

#ifndef VT_DEFS_H
#define VT_DEFS_H

//=============================================================================
// VT PGNs
//=============================================================================
#define PGN_VT_TO_ECU             0xE600   // VT -> ECU (PDU1, destination-specific)
#define PGN_ECU_TO_VT             0xE700   // ECU -> VT (PDU1, destination-specific)
#define PGN_VT_STATUS             0xFE6E   // VT Status broadcast (PDU2)

//=============================================================================
// VT Function Codes (byte 0 of data)
//=============================================================================
#define VT_FUNC_SOFT_KEY_ACTIVATION       0x00
#define VT_FUNC_BUTTON_ACTIVATION         0x01
#define VT_FUNC_CHANGE_NUMERIC_VALUE      0xA8
#define VT_FUNC_CHANGE_ATTRIBUTE          0xAF
#define VT_FUNC_CHANGE_FILL_ATTRIBUTES    0xAB
#define VT_FUNC_OBJECT_POOL_TRANSFER      0x11
#define VT_FUNC_END_OF_OBJECT_POOL        0x12
#define VT_FUNC_END_OF_OBJECT_POOL_RESP   0x12
#define VT_FUNC_GET_MEMORY                0xC0
#define VT_FUNC_GET_MEMORY_RESP           0xC0
#define VT_FUNC_GET_NUM_SOFT_KEYS         0xC2
#define VT_FUNC_GET_NUM_SOFT_KEYS_RESP    0xC2
#define VT_FUNC_GET_TEXT_FONT_DATA        0xC3
#define VT_FUNC_GET_TEXT_FONT_DATA_RESP   0xC3
#define VT_FUNC_GET_HARDWARE              0xC7
#define VT_FUNC_GET_HARDWARE_RESP         0xC7
#define VT_FUNC_WORKING_SET_MAINTENANCE   0xFF
#define VT_FUNC_VT_STATUS                 0xFE

//=============================================================================
// VT Client State Machine
//=============================================================================
enum VTClientState {
    VT_IDLE = 0,
    VT_WAIT_FOR_VT,
    VT_SEND_GET_MEMORY,
    VT_WAIT_GET_MEMORY_RESP,
    VT_SEND_GET_SOFTKEYS,
    VT_WAIT_GET_SOFTKEYS_RESP,
    VT_SEND_GET_TEXT_FONT,
    VT_WAIT_GET_TEXT_FONT_RESP,
    VT_SEND_GET_HARDWARE,
    VT_WAIT_GET_HARDWARE_RESP,
    VT_UPLOAD_OBJECT_POOL,
    VT_SEND_END_OF_POOL,
    VT_WAIT_END_OF_POOL_RESP,
    VT_CONNECTED,
    VT_ERROR
};

//=============================================================================
// VT Object Types (ISO 11783-6)
//=============================================================================
#define VT_TYPE_WORKING_SET            0
#define VT_TYPE_DATA_MASK              1
#define VT_TYPE_SOFT_KEY_MASK          4
#define VT_TYPE_KEY                    5
#define VT_TYPE_BUTTON                 6
#define VT_TYPE_INPUT_NUMBER           9
#define VT_TYPE_OUTPUT_STRING          11
#define VT_TYPE_OUTPUT_NUMBER          12
#define VT_TYPE_OUTPUT_RECTANGLE       14
#define VT_TYPE_OUTPUT_LINEAR_BAR_GRAPH 18
#define VT_TYPE_NUMBER_VARIABLE        21
#define VT_TYPE_FONT_ATTRIBUTES        23
#define VT_TYPE_LINE_ATTRIBUTES        24
#define VT_TYPE_FILL_ATTRIBUTES        25

//=============================================================================
// VT Object Pool Object IDs
//=============================================================================
#define VT_OBJ_WORKING_SET        0x0000
#define VT_OBJ_DATA_MASK          0x1000
#define VT_OBJ_SOFT_KEY_MASK      0x2000

// OutputString objects - labels
#define VT_OBJ_STR_RATE1_LABEL    0x4000   // "Cur Rate"
#define VT_OBJ_STR_RATE1_UNIT     0x4004   // "L/ha"
#define VT_OBJ_STR_TARGET1_LABEL  0x4005   // "VR Target"
#define VT_OBJ_STR_TARGET1_UNIT   0x4006   // "L/ha"
#define VT_OBJ_STR_QTY_LABEL      0x4007   // "Applied"
#define VT_OBJ_STR_QTY_UNIT       0x4008   // "L"
#define VT_OBJ_STR_AREA_LABEL     0x400A   // "Area Rem"
#define VT_OBJ_STR_AREA_UNIT      0x400B   // "ha"
#define VT_OBJ_STR_SPEED_LABEL    0x400D   // "Speed"
#define VT_OBJ_STR_SPEED_UNIT     0x400E   // "km/h"
#define VT_OBJ_STR_RATE2_LABEL    0x4010   // "Rate 2"
#define VT_OBJ_STR_RATE2_UNIT     0x4014   // "L/ha"
#define VT_OBJ_STR_TARGET2_LABEL  0x4015   // "Target 2"
#define VT_OBJ_STR_TARGET2_UNIT   0x4016   // "L/ha"

// OutputNumber objects
#define VT_OBJ_NUM_RATE1_ACTUAL   0x4001
#define VT_OBJ_NUM_RATE1_TARGET   0x4003
#define VT_OBJ_NUM_QTY_APPLIED    0x4009
#define VT_OBJ_NUM_AREA_REM       0x400C
#define VT_OBJ_NUM_RATE2_ACTUAL   0x4011
#define VT_OBJ_NUM_RATE2_TARGET   0x4013

// InputNumber object
#define VT_OBJ_INPUT_SPEED        0x400F   // InputNumber for speed entry

// Section buttons (8 interactive buttons, replacing 16 indicator rectangles)
#define VT_OBJ_BTN_SECTION_BASE   0x4100   // 0x4100-0x4107

// AOG connection indicator
#define VT_OBJ_RECT_AOG           0x4221   // AOG status rectangle
#define VT_OBJ_STR_AOG            0x4220   // "AOG" text
#define VT_OBJ_STR_PRODUCT        0x4222   // Product name string

// Tank bar graph
#define VT_OBJ_BAR_TANK           0x4230   // Vertical tank level bar

// NumberVariable objects
#define VT_OBJ_VAR_RATE1_ACTUAL   0x5000
#define VT_OBJ_VAR_RATE1_TARGET   0x5001
#define VT_OBJ_VAR_RATE2_ACTUAL   0x5002
#define VT_OBJ_VAR_RATE2_TARGET   0x5003
#define VT_OBJ_VAR_QTY_APPLIED    0x5004
#define VT_OBJ_VAR_AREA_REM       0x5005
#define VT_OBJ_VAR_TANK_LEVEL     0x5006
#define VT_OBJ_VAR_SPEED          0x5007

// Attribute objects
#define VT_OBJ_FONT_LARGE         0x6000   // 12x16 white - values
#define VT_OBJ_FONT_SMALL         0x6001   // 8x12 white - units, status text
#define VT_OBJ_FONT_YELLOW        0x6002   // 8x12 yellow - labels
#define VT_OBJ_LINE_THIN          0x6010
#define VT_OBJ_FILL_GREEN         0x6020
#define VT_OBJ_FILL_RED           0x6021
#define VT_OBJ_FILL_GREY          0x6022

// Soft Key objects
#define VT_OBJ_SK_AUTO            0x7000
#define VT_OBJ_SK_MASTER          0x7001
#define VT_OBJ_SK_PROD_NEXT       0x7002
#define VT_OBJ_SK_PROD_PREV       0x7003

// Soft Key label strings
#define VT_OBJ_STR_SK_AUTO        0x7010
#define VT_OBJ_STR_SK_MASTER      0x7011
#define VT_OBJ_STR_SK_PROD_NEXT   0x7012
#define VT_OBJ_STR_SK_PROD_PREV   0x7013

// Soft Key key codes (unique per interaction type)
#define VT_KEYCODE_AUTO           10
#define VT_KEYCODE_MASTER         11
#define VT_KEYCODE_PROD_NEXT      12
#define VT_KEYCODE_PROD_PREV      13

//=============================================================================
// VT Colour Constants (ISO 11783-6 standard colour table)
//=============================================================================
#define VT_COLOUR_BLACK               0
#define VT_COLOUR_WHITE               1
#define VT_COLOUR_GREEN               2
#define VT_COLOUR_TEAL                3
#define VT_COLOUR_MAROON              4
#define VT_COLOUR_PURPLE              5
#define VT_COLOUR_OLIVE               6
#define VT_COLOUR_SILVER              7
#define VT_COLOUR_GREY                8
#define VT_COLOUR_BLUE                9
#define VT_COLOUR_LIME                10
#define VT_COLOUR_CYAN                11
#define VT_COLOUR_RED                 12
#define VT_COLOUR_MAGENTA             13
#define VT_COLOUR_YELLOW              14
#define VT_COLOUR_NAV_BLUE            15

//=============================================================================
// VT Pool Transfer Command Byte (for TP)
//=============================================================================
#define VT_POOL_TRANSFER_CMD          0x11

//=============================================================================
// Forward declarations for VT Client functions (defined in VTClient.ino)
//=============================================================================
void VTClient_Begin();
void VTClient_Update();
void VTClient_HandleVTStatus(const CAN_message_t& msg);
void VTClient_HandleVTtoECU(const CAN_message_t& msg);
uint8_t VTClient_GetState();

//=============================================================================
// Forward declarations for VT Pool functions (defined in VTPool.ino)
//=============================================================================
void VTPool_Build(uint16_t dispW, uint16_t dispH);
const uint8_t* VTPool_GetBuffer();
uint16_t VTPool_GetSize();

//=============================================================================
// External variables from VTPool.ino
//=============================================================================
extern uint8_t vtPoolBuffer[];
extern uint16_t vtPoolSize;

#endif // VT_DEFS_H
