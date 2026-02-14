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
// VT Object Pool Object IDs
//=============================================================================
#define VT_OBJ_WORKING_SET        0x0000
#define VT_OBJ_DATA_MASK          0x1000
#define VT_OBJ_SOFT_KEY_MASK      0x2000

// OutputString objects
#define VT_OBJ_STR_RATE1_LABEL    0x4000
#define VT_OBJ_STR_RATE1_SLASH    0x4002
#define VT_OBJ_STR_RATE1_UNIT     0x4004
#define VT_OBJ_STR_RATE2_LABEL    0x4010
#define VT_OBJ_STR_RATE2_SLASH    0x4012
#define VT_OBJ_STR_RATE2_UNIT     0x4014
#define VT_OBJ_STR_WORK           0x4200

// OutputNumber objects
#define VT_OBJ_NUM_RATE1_ACTUAL   0x4001
#define VT_OBJ_NUM_RATE1_TARGET   0x4003
#define VT_OBJ_NUM_RATE2_ACTUAL   0x4011
#define VT_OBJ_NUM_RATE2_TARGET   0x4013

// Section rectangles (0x4100-0x410F)
#define VT_OBJ_SECTION_BASE       0x4100

// NumberVariable objects
#define VT_OBJ_VAR_RATE1_ACTUAL   0x5000
#define VT_OBJ_VAR_RATE1_TARGET   0x5001
#define VT_OBJ_VAR_RATE2_ACTUAL   0x5002
#define VT_OBJ_VAR_RATE2_TARGET   0x5003

// Attribute objects
#define VT_OBJ_FONT_LARGE         0x6000
#define VT_OBJ_FONT_SMALL         0x6001
#define VT_OBJ_LINE_THIN          0x6010
#define VT_OBJ_FILL_GREEN         0x6020
#define VT_OBJ_FILL_RED           0x6021

//=============================================================================
// VT Object Types (ISO 11783-6)
//=============================================================================
#define VT_TYPE_WORKING_SET            0
#define VT_TYPE_DATA_MASK              1
#define VT_TYPE_SOFT_KEY_MASK          4
#define VT_TYPE_OUTPUT_STRING          11
#define VT_TYPE_OUTPUT_NUMBER          12
#define VT_TYPE_OUTPUT_RECTANGLE       14
#define VT_TYPE_NUMBER_VARIABLE        21
#define VT_TYPE_FONT_ATTRIBUTES        23
#define VT_TYPE_LINE_ATTRIBUTES        24
#define VT_TYPE_FILL_ATTRIBUTES        25

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
void VTPool_Build();
const uint8_t* VTPool_GetBuffer();
uint16_t VTPool_GetSize();

//=============================================================================
// External variables from VTPool.ino
//=============================================================================
extern uint8_t vtPoolBuffer[];
extern uint16_t vtPoolSize;

#endif // VT_DEFS_H
