// TCDefs.h - Shared definitions for TC Client implementation
// This header must be included AFTER FlexCAN_T4.h

#ifndef TC_DEFS_H
#define TC_DEFS_H

// Forward declaration of CAN_message_t (defined in FlexCAN_T4.h)
// This allows us to use references to CAN_message_t in function declarations
struct CAN_message_t;

//=============================================================================
// DDI Definitions for Rate Control (ISO 11783-10)
//=============================================================================
#define DDI_SETPOINT_VOLUME_PER_AREA      1     // TC -> Teensy: Target rate (mm³/m²)
#define DDI_ACTUAL_VOLUME_PER_AREA        2     // Teensy -> TC: Actual rate (mm³/m²)
#define DDI_DEFAULT_VOLUME_PER_AREA       3     // Default rate
#define DDI_ACTUAL_VOLUME                 48    // Teensy -> TC: Total applied (mL)
#define DDI_SECTION_CONTROL_STATE         157   // TC -> Teensy: Section on/off
#define DDI_ACTUAL_WORK_STATE             141   // Teensy -> TC: Working state

//=============================================================================
// Element numbers in DDOP
//=============================================================================
#define ELEMENT_DEVICE                    0
#define ELEMENT_BOOM                      1
#define ELEMENT_SECTION_BASE              2     // Section 0 = element 2, Section 1 = element 3, etc.

//=============================================================================
// Transport Protocol Types
//=============================================================================
typedef void (*TPReceiveCallback)(uint32_t pgn, const uint8_t* data, uint16_t length);

//=============================================================================
// Transport Protocol States
//=============================================================================
enum TPState {
    TP_STATE_IDLE = 0,
    TP_STATE_SEND_RTS,
    TP_STATE_WAIT_CTS,
    TP_STATE_SEND_DATA,
    TP_STATE_WAIT_EOM,
    TP_STATE_RECEIVE_DATA,
    TP_STATE_COMPLETE,
    TP_STATE_ERROR
};

//=============================================================================
// TC Client States
//=============================================================================
enum TCClientState {
    TC_IDLE = 0,
    TC_WAIT_FOR_TC,
    TC_SEND_WORKING_SET_MASTER,
    TC_WAIT_STRUCTURE_LABEL,
    TC_REQUEST_OBJECT_POOL_TRANSFER,   // Send request to transfer DDOP
    TC_WAIT_OBJECT_POOL_TRANSFER_RESP, // Wait for permission
    TC_SEND_DDOP,                      // Actually send DDOP via TP
    TC_WAIT_DDOP_STORED,
    TC_REQUEST_ACTIVATION,
    TC_WAIT_ACTIVATION,
    TC_ACTIVE,
    TC_ERROR
};

//=============================================================================
// Forward declarations for TC Client functions (defined in TCClient.ino)
//=============================================================================
void TCClient_Begin();
void TCClient_Update();
void TCClient_HandleTCStatus(const CAN_message_t& msg);
void TCClient_HandleProcessData(const CAN_message_t& msg, uint8_t pf, uint8_t ps);
bool TCClient_IsActive();
uint8_t TCClient_GetState();
void TCClient_SetState(TCClientState newState);
uint8_t TCClient_GetTCAddress();
float TCClient_GetSetpointRate(uint8_t sensorId);

//=============================================================================
// Forward declarations for TP functions (defined in TP.ino)
//=============================================================================
void TP_Begin();
void TP_Update();
void TP_HandleCM(const CAN_message_t& msg);
void TP_HandleDT(const CAN_message_t& msg);
bool TP_SendLargeMessage(uint8_t destAddr, uint32_t pgn, uint8_t cmdByteVal, const uint8_t* data, uint16_t size);
bool TP_SendDDOP(uint8_t destAddr, const uint8_t* data, uint16_t size);
bool TP_SendVTPool(uint8_t destAddr, const uint8_t* data, uint16_t size);
bool TP_IsBusy();
bool TP_IsComplete();
bool TP_IsError();
TPState TP_GetState();
void TP_SetReceiveCallback(TPReceiveCallback callback);

//=============================================================================
// Forward declarations for DDOP functions (defined in DDOP.ino)
//=============================================================================
void DDOP_Build();
const uint8_t* DDOP_GetBuffer();
uint16_t DDOP_GetSize();
const char* DDOP_GetStructureLabel();

//=============================================================================
// External variables from DDOP.ino
//=============================================================================
extern uint8_t ddopBuffer[];
extern uint16_t ddopSize;

//=============================================================================
// External variables from CANBus.ino
//=============================================================================
struct CANStats {
    uint32_t rxCount;
    uint32_t txCount;
    uint32_t errorCount;
    uint32_t lastRxTime;
};
extern CANStats canStats;

//=============================================================================
// Forward declarations for CANBus functions
//=============================================================================
void CANBus_MaintainAddress();
void CANBus_Receive();
void CANBus_SendAddressClaim();
void CANBus_SendProprietaryStatus();

#endif // TC_DEFS_H
