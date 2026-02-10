
// VTClient.ino - ISO 11783-6 Virtual Terminal Client
// State machine, message handlers, and display update logic
// Runs alongside TC Client on the same CAN address

//=============================================================================
// VT Client Constants
//=============================================================================

#define VT_TIMEOUT_RESPONSE       2000   // Timeout waiting for VT response (ms)
#define VT_TIMEOUT_END_OF_POOL    5000   // Timeout for end-of-pool response (ms)
#define VT_ERROR_WAIT             2000   // Wait in error state before retry (ms)
#define VT_IDLE_WAIT_FOR_TC       15000  // Max wait for TC_ACTIVE before starting VT (ms)
#define VT_WS_MAINTENANCE_INTERVAL 1000  // Working Set Maintenance every 1s
#define VT_DISPLAY_UPDATE_INTERVAL 250   // Display update every 250ms

//=============================================================================
// VT Client State Variables
//=============================================================================

struct VTClientData {
    VTClientState state = VT_IDLE;
    uint8_t vtServerAddress = 0xFF;    // Address of VT Server
    uint32_t lastVTStatusTime = 0;     // Last VT Status received
    uint32_t stateEntryTime = 0;       // When we entered current state
    uint32_t lastMaintenanceTime = 0;  // Last WS Maintenance sent
    uint32_t lastDisplayUpdate = 0;    // Last display values updated

    // VT capabilities (from Get Hardware response)
    uint16_t vtWidth = 200;            // VT display width
    uint16_t vtHeight = 200;           // VT display height
    uint8_t vtColourDepth = 8;         // Colour depth (bits)
    uint8_t vtVersion = 3;             // VT version from status
    uint8_t vtNumSoftKeys = 0;         // Number of soft keys
    uint8_t vtNumPhysicalSoftKeys = 0; // Number of physical soft keys

    // Upload tracking
    bool poolUploadStarted = false;

    // Last sent values (to avoid redundant updates)
    uint32_t lastRate1Actual = 0xFFFFFFFF;
    uint32_t lastRate1Target = 0xFFFFFFFF;
    uint32_t lastRate2Actual = 0xFFFFFFFF;
    uint32_t lastRate2Target = 0xFFFFFFFF;
    uint16_t lastSectionStates = 0xFFFF;
};

VTClientData vtClient;

//=============================================================================
// Forward Declarations
//=============================================================================

void VTClient_SendToVT(const uint8_t* data, uint8_t len);
void VTClient_SendGetMemory();
void VTClient_SendGetSoftKeys();
void VTClient_SendGetTextFont();
void VTClient_SendGetHardware();
void VTClient_SendEndOfPool();
void VTClient_SendWSMaintenance();
void VTClient_UpdateDisplay();
void VTClient_SendChangeNumericValue(uint16_t objId, uint32_t value);
void VTClient_SendChangeFillAttributes(uint16_t objId, uint8_t fillType, uint8_t colour);
void VTClient_SetState(VTClientState newState);

//=============================================================================
// Initialization
//=============================================================================

void VTClient_Begin() {
    vtClient.state = VT_IDLE;
    vtClient.vtServerAddress = 0xFF;
    vtClient.lastVTStatusTime = 0;
    vtClient.stateEntryTime = millis();
    vtClient.poolUploadStarted = false;
    vtClient.lastRate1Actual = 0xFFFFFFFF;
    vtClient.lastRate1Target = 0xFFFFFFFF;
    vtClient.lastRate2Actual = 0xFFFFFFFF;
    vtClient.lastRate2Target = 0xFFFFFFFF;
    vtClient.lastSectionStates = 0xFFFF;

    // Build VT object pool at startup
    VTPool_Build();

    Serial.println("VT Client initialized");
}

//=============================================================================
// State Machine
//=============================================================================

void VTClient_Update() {
    uint32_t now = millis();
    uint32_t stateTime = now - vtClient.stateEntryTime;

    // Send Working Set Maintenance every 1s during handshake and while connected
    // Must be sent continuously from VT_WAIT_FOR_VT onwards to stay registered
    if (vtClient.state >= VT_WAIT_FOR_VT && vtClient.state != VT_ERROR) {
        if (now - vtClient.lastMaintenanceTime >= VT_WS_MAINTENANCE_INTERVAL) {
            vtClient.lastMaintenanceTime = now;
            VTClient_SendWSMaintenance();
        }
    }

    switch (vtClient.state) {
        case VT_IDLE:
            // Wait for TC Client to become active (or timeout)
            // This ensures Working Set Master is already announced
            if (TCClient_IsActive() || stateTime >= VT_IDLE_WAIT_FOR_TC) {
                if (vtClient.vtServerAddress != 0xFF) {
                    // VT already detected, proceed
                    VTClient_SetState(VT_WAIT_FOR_VT);
                }
                // else: stay idle until VT Status is received
            }
            // Check if VT was detected while waiting
            if (vtClient.vtServerAddress != 0xFF && stateTime >= 1000) {
                VTClient_SetState(VT_WAIT_FOR_VT);
            }
            break;

        case VT_WAIT_FOR_VT:
            // Wait for VT Status to confirm VT is available
            if (vtClient.vtServerAddress != 0xFF) {
                // Brief stabilization wait
                if (stateTime >= 500) {
                    VTClient_SetState(VT_SEND_GET_MEMORY);
                }
            }
            // Timeout - no VT found
            if (now - vtClient.lastVTStatusTime > 6000 && vtClient.lastVTStatusTime > 0) {
                Serial.println("VT: No VT Status received, returning to IDLE");
                vtClient.vtServerAddress = 0xFF;
                VTClient_SetState(VT_IDLE);
            }
            break;

        case VT_SEND_GET_MEMORY:
            VTClient_SendGetMemory();
            VTClient_SetState(VT_WAIT_GET_MEMORY_RESP);
            break;

        case VT_WAIT_GET_MEMORY_RESP:
            if (stateTime >= VT_TIMEOUT_RESPONSE) {
                Serial.println("VT: Get Memory timeout");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_SEND_GET_SOFTKEYS:
            VTClient_SendGetSoftKeys();
            VTClient_SetState(VT_WAIT_GET_SOFTKEYS_RESP);
            break;

        case VT_WAIT_GET_SOFTKEYS_RESP:
            if (stateTime >= VT_TIMEOUT_RESPONSE) {
                Serial.println("VT: Get Soft Keys timeout");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_SEND_GET_TEXT_FONT:
            VTClient_SendGetTextFont();
            VTClient_SetState(VT_WAIT_GET_TEXT_FONT_RESP);
            break;

        case VT_WAIT_GET_TEXT_FONT_RESP:
            if (stateTime >= VT_TIMEOUT_RESPONSE) {
                Serial.println("VT: Get Text Font timeout");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_SEND_GET_HARDWARE:
            VTClient_SendGetHardware();
            VTClient_SetState(VT_WAIT_GET_HARDWARE_RESP);
            break;

        case VT_WAIT_GET_HARDWARE_RESP:
            if (stateTime >= VT_TIMEOUT_RESPONSE) {
                Serial.println("VT: Get Hardware timeout");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_UPLOAD_OBJECT_POOL:
            // Wait for TP to be free, then upload
            if (!vtClient.poolUploadStarted) {
                if (!TP_IsBusy()) {
                    Serial.print("VT: Uploading object pool, size=");
                    Serial.println(vtPoolSize);
                    TP_SendVTPool(vtClient.vtServerAddress, vtPoolBuffer, vtPoolSize);
                    vtClient.poolUploadStarted = true;
                }
                // else: wait for TP to finish (e.g. DDOP transfer)
            } else {
                // Check TP status
                if (TP_IsComplete() || (!TP_IsBusy() && !TP_IsError())) {
                    // TP transfer done, send End of Object Pool
                    VTClient_SetState(VT_SEND_END_OF_POOL);
                } else if (TP_IsError()) {
                    Serial.println("VT: Pool upload TP error");
                    vtClient.poolUploadStarted = false;
                    VTClient_SetState(VT_ERROR);
                }
            }
            // Safety timeout
            if (stateTime >= 15000) {
                Serial.println("VT: Pool upload timeout");
                vtClient.poolUploadStarted = false;
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_SEND_END_OF_POOL:
            VTClient_SendEndOfPool();
            VTClient_SetState(VT_WAIT_END_OF_POOL_RESP);
            break;

        case VT_WAIT_END_OF_POOL_RESP:
            if (stateTime >= VT_TIMEOUT_END_OF_POOL) {
                Serial.println("VT: End of Pool response timeout");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_CONNECTED:
            // Normal operation - update display periodically
            if (now - vtClient.lastDisplayUpdate >= VT_DISPLAY_UPDATE_INTERVAL) {
                vtClient.lastDisplayUpdate = now;
                VTClient_UpdateDisplay();
            }
            // Check for VT timeout (no VT Status for 6s)
            if (now - vtClient.lastVTStatusTime > 6000) {
                Serial.println("VT: VT Server lost connection");
                vtClient.vtServerAddress = 0xFF;
                VTClient_SetState(VT_IDLE);
            }
            break;

        case VT_ERROR:
            if (stateTime >= VT_ERROR_WAIT) {
                vtClient.poolUploadStarted = false;
                VTClient_SetState(VT_IDLE);
            }
            break;
    }
}

void VTClient_SetState(VTClientState newState) {
    if (vtClient.state != newState) {
        Serial.print("VT Client state: ");
        Serial.print(vtClient.state);
        Serial.print(" -> ");
        Serial.println(newState);
        vtClient.state = newState;
        vtClient.stateEntryTime = millis();
    }
}

//=============================================================================
// Message Handlers
//=============================================================================

void VTClient_HandleVTStatus(const CAN_message_t& msg) {
    // PGN 0xFE6E - VT Status broadcast
    // Byte 0: Active Working Set Master address
    // Byte 1-2: Active Data/Alarm mask object ID
    // Byte 3-4: Active Soft Key mask object ID
    // Byte 5: VT busy coding (bit field)
    // Byte 6: VT function code of current cmd being processed
    // Byte 7: Reserved

    uint8_t sourceAddr = msg.id & 0xFF;
    vtClient.lastVTStatusTime = millis();

    if (vtClient.vtServerAddress == 0xFF) {
        vtClient.vtServerAddress = sourceAddr;
        Serial.print("VT Server detected at address 0x");
        Serial.println(sourceAddr, HEX);
    }
}

void VTClient_HandleVTtoECU(const CAN_message_t& msg) {
    // PGN 0xE600 - VT to ECU messages
    uint8_t funcCode = msg.buf[0];

    switch (funcCode) {
        case VT_FUNC_GET_MEMORY_RESP:
            // Get Memory Response
            // Byte 1: Version (VT version)
            // Byte 2: 0=enough memory, 1=not enough
            vtClient.vtVersion = msg.buf[1];
            if (msg.buf[2] == 0) {
                Serial.print("VT: Get Memory OK, VT version=");
                Serial.println(vtClient.vtVersion);
                VTClient_SetState(VT_SEND_GET_SOFTKEYS);
            } else {
                Serial.println("VT: Not enough memory for object pool");
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_FUNC_GET_NUM_SOFT_KEYS_RESP:
            // Get Number of Soft Keys Response
            // Byte 1: Navigation soft keys (x)
            // Byte 2: (reserved)
            // Byte 3: (reserved)
            // Byte 4: Virtual soft keys (x)
            // Byte 5: Virtual soft keys (y)
            // Byte 6: Physical soft keys (number)
            vtClient.vtNumSoftKeys = msg.buf[4] * msg.buf[5];
            vtClient.vtNumPhysicalSoftKeys = msg.buf[6];
            Serial.print("VT: Soft keys: virtual=");
            Serial.print(vtClient.vtNumSoftKeys);
            Serial.print(" physical=");
            Serial.println(vtClient.vtNumPhysicalSoftKeys);
            VTClient_SetState(VT_SEND_GET_TEXT_FONT);
            break;

        case VT_FUNC_GET_TEXT_FONT_DATA_RESP:
            // Get Text Font Data Response
            // Bytes 1-8 contain font size flags
            Serial.println("VT: Text Font Data received");
            VTClient_SetState(VT_SEND_GET_HARDWARE);
            break;

        case VT_FUNC_GET_HARDWARE_RESP:
            // Get Hardware Response
            // Byte 1: Graphic type (0=monochrome, 1=16 colour, 2=256 colour)
            // Byte 2: Hardware (bit field)
            // Byte 3-4: X pixels (width)
            // Byte 5-6: Y pixels (height)
            vtClient.vtColourDepth = msg.buf[1];
            vtClient.vtWidth = msg.buf[3] | ((uint16_t)msg.buf[4] << 8);
            vtClient.vtHeight = msg.buf[5] | ((uint16_t)msg.buf[6] << 8);
            Serial.print("VT: Hardware - colours=");
            Serial.print(vtClient.vtColourDepth);
            Serial.print(" width=");
            Serial.print(vtClient.vtWidth);
            Serial.print(" height=");
            Serial.println(vtClient.vtHeight);
            // Ready to upload object pool
            vtClient.poolUploadStarted = false;
            VTClient_SetState(VT_UPLOAD_OBJECT_POOL);
            break;

        case VT_FUNC_END_OF_OBJECT_POOL_RESP:
            // End of Object Pool Response
            // Byte 1: Error code (0 = success)
            if (msg.buf[1] == 0) {
                Serial.println("VT: Object pool accepted - VT Client CONNECTED");
                // Force initial display update
                vtClient.lastRate1Actual = 0xFFFFFFFF;
                vtClient.lastRate1Target = 0xFFFFFFFF;
                vtClient.lastRate2Actual = 0xFFFFFFFF;
                vtClient.lastRate2Target = 0xFFFFFFFF;
                vtClient.lastSectionStates = 0xFFFF;
                VTClient_SetState(VT_CONNECTED);
            } else {
                Serial.print("VT: Object pool rejected, error=");
                Serial.println(msg.buf[1]);
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_FUNC_SOFT_KEY_ACTIVATION:
        case VT_FUNC_BUTTON_ACTIVATION:
            // User interaction - not handling in v1
            break;
    }
}

//=============================================================================
// Message Senders
//=============================================================================

void VTClient_SendToVT(const uint8_t* data, uint8_t len) {
    CAN_message_t msg;

    // PGN 0xE700 (ECU to VT), PDU1 format
    msg.id = (7UL << 26) | (0xE7UL << 16) | ((uint32_t)vtClient.vtServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    for (uint8_t i = 0; i < 8; i++) {
        msg.buf[i] = (i < len) ? data[i] : 0xFF;
    }

    ISOBUS.write(msg);
    canStats.txCount++;
}

void VTClient_SendGetMemory() {
    // Get Memory request (ISO 11783-6)
    // Byte 0: Function code 0xC0
    // Byte 1: Reserved (0xFF)
    // Bytes 2-5: Required memory in bytes (32-bit LE)
    // Bytes 6-7: Reserved (0xFF)
    uint8_t data[8];
    data[0] = VT_FUNC_GET_MEMORY;
    data[1] = 0xFF;
    data[2] = vtPoolSize & 0xFF;
    data[3] = (vtPoolSize >> 8) & 0xFF;
    data[4] = 0;
    data[5] = 0;
    data[6] = 0xFF;
    data[7] = 0xFF;

    VTClient_SendToVT(data, 8);
    Serial.print("VT: Sent Get Memory, pool size=");
    Serial.println(vtPoolSize);
}

void VTClient_SendGetSoftKeys() {
    uint8_t data[8];
    data[0] = VT_FUNC_GET_NUM_SOFT_KEYS;
    for (uint8_t i = 1; i < 8; i++) data[i] = 0xFF;

    VTClient_SendToVT(data, 8);
    Serial.println("VT: Sent Get Number of Soft Keys");
}

void VTClient_SendGetTextFont() {
    uint8_t data[8];
    data[0] = VT_FUNC_GET_TEXT_FONT_DATA;
    for (uint8_t i = 1; i < 8; i++) data[i] = 0xFF;

    VTClient_SendToVT(data, 8);
    Serial.println("VT: Sent Get Text Font Data");
}

void VTClient_SendGetHardware() {
    uint8_t data[8];
    data[0] = VT_FUNC_GET_HARDWARE;
    for (uint8_t i = 1; i < 8; i++) data[i] = 0xFF;

    VTClient_SendToVT(data, 8);
    Serial.println("VT: Sent Get Hardware");
}

void VTClient_SendEndOfPool() {
    uint8_t data[8];
    data[0] = VT_FUNC_END_OF_OBJECT_POOL;
    for (uint8_t i = 1; i < 8; i++) data[i] = 0xFF;

    VTClient_SendToVT(data, 8);
    Serial.println("VT: Sent End of Object Pool");
}

void VTClient_SendWSMaintenance() {
    // Working Set Maintenance - must be sent every 1s
    // Byte 0: Function code 0xFF
    // Byte 1: Bitmask (bit 0 = initiating WS, set until pool accepted)
    // Byte 2: VT version we support
    uint8_t data[8];
    data[0] = VT_FUNC_WORKING_SET_MAINTENANCE;
    data[1] = (vtClient.state == VT_CONNECTED) ? 0x00 : 0x01;  // Init bit set during handshake
    data[2] = vtClient.vtVersion;
    for (uint8_t i = 3; i < 8; i++) data[i] = 0xFF;

    VTClient_SendToVT(data, 8);
}

//=============================================================================
// Display Update Logic
//=============================================================================

void VTClient_SendChangeNumericValue(uint16_t objId, uint32_t value) {
    // Change Numeric Value command (func 0xA8)
    // Byte 0: Function code
    // Bytes 1-2: Object ID (little endian)
    // Bytes 3-6: New value (little endian, 32-bit)
    // Byte 7: Reserved
    uint8_t data[8];
    data[0] = VT_FUNC_CHANGE_NUMERIC_VALUE;
    data[1] = objId & 0xFF;
    data[2] = (objId >> 8) & 0xFF;
    data[3] = 0xFF;  // Reserved
    data[4] = value & 0xFF;
    data[5] = (value >> 8) & 0xFF;
    data[6] = (value >> 16) & 0xFF;
    data[7] = (value >> 24) & 0xFF;

    VTClient_SendToVT(data, 8);
}

void VTClient_SendChangeFillAttributes(uint16_t objId, uint8_t fillType, uint8_t colour) {
    // Change Fill Attributes command (func 0xAB)
    // Byte 0: Function code
    // Bytes 1-2: Object ID (little endian)
    // Byte 3: Fill type
    // Byte 4: Fill colour
    // Bytes 5-6: Fill pattern object ID (0xFFFF = none)
    // Byte 7: Reserved
    uint8_t data[8];
    data[0] = VT_FUNC_CHANGE_FILL_ATTRIBUTES;
    data[1] = objId & 0xFF;
    data[2] = (objId >> 8) & 0xFF;
    data[3] = fillType;
    data[4] = colour;
    data[5] = 0xFF;
    data[6] = 0xFF;
    data[7] = 0xFF;

    VTClient_SendToVT(data, 8);
}

void VTClient_UpdateDisplay() {
    // Update rate values - only send if changed to reduce bus traffic

    // Rate 1 Actual (stored as value * 10 for 1 decimal place)
    uint32_t rate1Actual = (uint32_t)(Sensor[0].UPM * 10.0);
    if (rate1Actual != vtClient.lastRate1Actual) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE1_ACTUAL, rate1Actual);
        vtClient.lastRate1Actual = rate1Actual;
    }

    // Rate 1 Target
    uint32_t rate1Target = (uint32_t)(Sensor[0].TargetUPM * 10.0);
    if (rate1Target != vtClient.lastRate1Target) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE1_TARGET, rate1Target);
        vtClient.lastRate1Target = rate1Target;
    }

    // Rate 2 (if second sensor present)
    if (MDL.SensorCount > 1) {
        uint32_t rate2Actual = (uint32_t)(Sensor[1].UPM * 10.0);
        if (rate2Actual != vtClient.lastRate2Actual) {
            VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE2_ACTUAL, rate2Actual);
            vtClient.lastRate2Actual = rate2Actual;
        }

        uint32_t rate2Target = (uint32_t)(Sensor[1].TargetUPM * 10.0);
        if (rate2Target != vtClient.lastRate2Target) {
            VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE2_TARGET, rate2Target);
            vtClient.lastRate2Target = rate2Target;
        }
    }

    // Section states - update rectangle fill colours
    uint16_t currentSections = RelayLo | ((uint16_t)RelayHi << 8);
    if (currentSections != vtClient.lastSectionStates) {
        for (uint8_t i = 0; i < 16; i++) {
            bool sectionOn = (currentSections >> i) & 0x01;
            bool wasOn = (vtClient.lastSectionStates >> i) & 0x01;

            // Only update sections that changed (or on first update when lastSectionStates == 0xFFFF)
            if (sectionOn != wasOn || vtClient.lastSectionStates == 0xFFFF) {
                uint16_t fillObjId = sectionOn ? VT_OBJ_FILL_GREEN : VT_OBJ_FILL_RED;
                // Use Change Fill Attributes on the rectangle's fill reference
                // We need to change the fill attribute reference of each rectangle
                // Since all sections share fill objects, we use Change Attribute
                // to change the rectangle's fill attribute object reference
                uint8_t data[8];
                data[0] = VT_FUNC_CHANGE_ATTRIBUTE;
                data[1] = (VT_OBJ_SECTION_BASE + i) & 0xFF;
                data[2] = ((VT_OBJ_SECTION_BASE + i) >> 8) & 0xFF;
                data[3] = 5;  // Attribute ID 5 = fill attributes for OutputRectangle
                data[4] = fillObjId & 0xFF;
                data[5] = (fillObjId >> 8) & 0xFF;
                data[6] = 0x00;
                data[7] = 0x00;
                VTClient_SendToVT(data, 8);
            }
        }
        vtClient.lastSectionStates = currentSections;
    }
}

//=============================================================================
// Access Functions
//=============================================================================

uint8_t VTClient_GetState() {
    return vtClient.state;
}
