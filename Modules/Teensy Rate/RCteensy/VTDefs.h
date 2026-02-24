// VTDefs.h - ISO 11783-6 Virtual Terminal Client definitions
// This header must be included AFTER FlexCAN_T4.h and TCDefs.h

#ifndef VT_DEFS_H
#define VT_DEFS_H

//=============================================================================
// VT Object Pool
//=============================================================================
// Pool is loaded from the const array in VTPoolData.h, generated from an
// .iop file created in AgIsoTerminalDesigner (converted via iop_to_header.ps1).
// The VT terminal handles scaling.
// Object IDs in the .iop MUST match the IDs defined below.

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
#define VT_FUNC_CHANGE_STRING_VALUE       0xB3
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
// VT Object Pool Object IDs - must match IDs from AgIsoTerminalDesigner
// See object_pool.h for the designer-exported ID list
//=============================================================================

// Structural objects
#define VT_OBJ_WORKING_SET        0       // WORKING_SET
#define VT_OBJ_DATA_MASK          1000    // DATA_MASK

// Section buttons (6 buttons) and their labels
#define VT_OBJ_BTN_SECTION_BASE   28000   // SW1-SW6 (28000-28005)
#define VT_OBJ_STR_BTN_SEC_BASE   11001   // SW1_LABEL-SW6_LABEL (11001-11006)
#define VT_NUM_SECTION_BUTTONS    6

// Product buttons (6 buttons) and their labels
#define VT_OBJ_BTN_PROD_BASE      28100   // PROD1-PROD5,FANS (28100-28105)
#define VT_OBJ_STR_BTN_PROD_BASE  11100   // PROD1_LABEL-FANS_LABEL (11100-11105)

// Product name display
#define VT_OBJ_STR_PRODUCT        11000   // PRODUCTNAME_LABEL
#define VT_OBJ_VAR_PRODUCT_NAME   22000   // PRODUCTNAMEVAR (StringVariable)

// Tank
#define VT_OBJ_RECT_TANK_BORDER   14001   // TANKBORDER
#define VT_OBJ_RECT_TANK_FILL     14002   // TANKFILL
#define VT_OBJ_FILL_TANK          26000   // TANKFILLATTR
#define VT_OBJ_VAR_TANK_LEVEL     27000   // TANKLEVEL (NumberVariable)

// Background
#define VT_OBJ_RECT_BG            14000   // RECTANGLE

// Data row labels (OutputString with StringVariable)
#define VT_OBJ_STR_RATE_LABEL         11110   // RATE_LABEL
#define VT_OBJ_STR_RATE_UNIT          11111   // RATE_UNITS_LABEL
#define VT_OBJ_STR_TARGET_LABEL       11112   // TARGET_RATE_LABEL
#define VT_OBJ_STR_TARGET_UNIT        11113   // TARGET_RATE_UNITS_LABEL
#define VT_OBJ_STR_QTY_UNIT           11114   // QUANTITY_UNITS_LABEL
#define VT_OBJ_STR_QTY_LABEL          11115   // QUANTITY_LABEL
#define VT_OBJ_STR_AREA_LABEL         11116   // AREA_LABEL
#define VT_OBJ_STR_AREA_UNIT          11117   // AREA_UNITS_LABEL
#define VT_OBJ_STR_SPEED_LABEL        11118   // SPEED_LABEL
#define VT_OBJ_STR_SPEED_UNIT         11119   // SPEED_UNITS_LABEL

// Data value OutputNumber objects
#define VT_OBJ_NUM_RATE               12000   // RATE_VALUE_BOX
#define VT_OBJ_NUM_TARGET_RATE        12001   // TARGET_RATE_VALUE_BOX
#define VT_OBJ_NUM_QTY               12002   // QUANTITY_VALUE_BOX
#define VT_OBJ_NUM_AREA               12003   // AREA_VALUE_BOX
#define VT_OBJ_NUM_SPEED              12004   // SPEED_VALUE_BOX

// StringVariable objects (for label/unit text)
#define VT_OBJ_VAR_RATE_STR           22001   // RATE_STRING
#define VT_OBJ_VAR_RATE_UNIT_STR      22002   // RATE_UNITS_STRING
#define VT_OBJ_VAR_TARGET_STR         22003   // TARGET_RATE_STRING
#define VT_OBJ_VAR_TARGET_UNIT_STR    22004   // TARGET_RATE_UNITS_STRING
#define VT_OBJ_VAR_QTY_UNIT_STR       22005   // QUANTITY_UNITS_STRING
#define VT_OBJ_VAR_AREA_UNIT_STR      22006   // AREA_UNITS_STRING
#define VT_OBJ_VAR_SPEED_UNIT_STR     22007   // SPEED_UNITS_STRING
#define VT_OBJ_VAR_QTY_STR            22008   // QUANTITY_STRING
#define VT_OBJ_VAR_AREA_STR           22009   // AREA_STRING

// NumberVariable objects (for OutputNumber values)
#define VT_OBJ_VAR_RATE               22100   // RATE_VALUENUMBER
#define VT_OBJ_VAR_TARGET_RATE        22101   // TARGET_RATE_VALUE_NUMBER
#define VT_OBJ_VAR_QTY                22102   // QUANTITY_VALUE_NUMBER
#define VT_OBJ_VAR_AREA               22103   // AREA_VALUE_NUMBER
#define VT_OBJ_VAR_SPEED              22104   // SPEED_VALUE_NUMBER

// Action buttons (on data mask)
#define VT_OBJ_BTN_TARGET_RATE        28106   // TARGET_RATE_BUTTON
#define VT_OBJ_BTN_QTY                28107   // QUANTITY_BUTTON
#define VT_OBJ_BTN_AREA               28108   // AREA_BUTTON

// Font attributes
#define VT_OBJ_FONT_LARGE         23000   // LARGEFONT
#define VT_OBJ_FONT_SMALL         23001   // SMALLFONT
#define VT_OBJ_FONT_LARGE_WHITE   23002   // LARGEWHITE
#define VT_OBJ_FONT_SMALL_WHITE   23003   // SMALLWHITE

// Line attributes
#define VT_OBJ_LINE_THIN          24000   // LINE_ATTRIBUTES
#define VT_OBJ_LINE_2             24001   // LINE_ATTRIBUTES_2

// Soft Key Mask and Keys
//#define VT_OBJ_SOFT_KEY_MASK
//#define VT_OBJ_SK_AUTO
//#define VT_OBJ_SK_MASTER
//#define VT_OBJ_SK_MENU
//#define VT_OBJ_SK_RQTY
//#define VT_OBJ_SK_RAREA
//#define VT_OBJ_SK_RX

// Key codes - update these to match what you set in the designer
// NOTE: Section and product buttons must have DIFFERENT key codes
#define VT_KEYCODE_SECTION_BASE   1        // Section buttons: key codes 1-6
#define VT_KEYCODE_PROD_BASE      20       // Product buttons: key codes 20-25

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
void VTPool_LoadIOP();
const uint8_t* VTPool_GetBuffer();
uint16_t VTPool_GetSize();

//=============================================================================
// External variables from VTPool.ino
//=============================================================================
extern uint8_t vtPoolBuffer[];
extern uint16_t vtPoolSize;

#endif // VT_DEFS_H
