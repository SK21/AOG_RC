
// VTClient.ino - ISO 11783-6 Virtual Terminal Client
// State machine, message handlers, interaction handling, and display update logic
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
#define VT_AOG_TIMEOUT            3000   // AOG connection timeout (ms)

//=============================================================================
// Section Button Mapping
//=============================================================================

struct SectionButtonMap {
    uint16_t sectionMask;     // bitmask of sections this button controls
};

SectionButtonMap sectionButtonMap[8];
uint8_t numSectionButtons = 8;

void VTClient_InitSectionMap() {
    // Default equal split: 16 sections / 8 buttons = 2 sections per button
    uint8_t sectionsPerButton = 16 / numSectionButtons;
    for (uint8_t i = 0; i < 8; i++) {
        sectionButtonMap[i].sectionMask = 0;
        for (uint8_t s = 0; s < sectionsPerButton; s++) {
            uint8_t section = i * sectionsPerButton + s;
            if (section < 16) {
                sectionButtonMap[i].sectionMask |= (1 << section);
            }
        }
    }
}

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

    // Interactive state
    uint8_t currentProduct = 0;        // Currently displayed product index
    float fixedSpeed = 0.0;            // User-entered fixed speed (km/h)

    // Last sent values (to avoid redundant updates)
    uint32_t lastRate1Actual = 0xFFFFFFFF;
    uint32_t lastRate1Target = 0xFFFFFFFF;
    uint32_t lastRate2Actual = 0xFFFFFFFF;
    uint32_t lastRate2Target = 0xFFFFFFFF;
    uint32_t lastQtyApplied = 0xFFFFFFFF;
    uint32_t lastAreaRemaining = 0xFFFFFFFF;
    uint32_t lastTankLevel = 0xFFFFFFFF;
    uint32_t lastSpeed = 0xFFFFFFFF;
    uint8_t lastSectionStates = 0xFF;   // Combined button states (8 buttons)
    uint8_t lastAOGState = 0xFF;        // 0xFF = unknown, 0 = disconnected, 1 = connected
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
void VTClient_SendChangeAttribute(uint16_t objId, uint8_t attrId, uint32_t value);
void VTClient_SetState(VTClientState newState);
void VTClient_HandleButtonActivation(const CAN_message_t& msg);
void VTClient_HandleSoftKeyActivation(const CAN_message_t& msg);
void VTClient_HandleChangeNumericValue(const CAN_message_t& msg);

//=============================================================================
// Initialization
//=============================================================================

void VTClient_Begin() {
    vtClient.state = VT_IDLE;
    vtClient.vtServerAddress = 0xFF;
    vtClient.lastVTStatusTime = 0;
    vtClient.stateEntryTime = millis();
    vtClient.poolUploadStarted = false;
    vtClient.currentProduct = 0;
    vtClient.fixedSpeed = 0.0;
    vtClient.lastRate1Actual = 0xFFFFFFFF;
    vtClient.lastRate1Target = 0xFFFFFFFF;
    vtClient.lastRate2Actual = 0xFFFFFFFF;
    vtClient.lastRate2Target = 0xFFFFFFFF;
    vtClient.lastQtyApplied = 0xFFFFFFFF;
    vtClient.lastAreaRemaining = 0xFFFFFFFF;
    vtClient.lastTankLevel = 0xFFFFFFFF;
    vtClient.lastSpeed = 0xFFFFFFFF;
    vtClient.lastSectionStates = 0xFF;
    vtClient.lastAOGState = 0xFF;

    // Initialize default section button mapping
    VTClient_InitSectionMap();

    // Build VT object pool at startup (default 200x200, rebuilt after Get Hardware)
    VTPool_Build(200, 200);

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
                // Send Working Set Master if TC Client hasn't (standalone VT mode)
                if (!tcClient.wsAnnounced && stateTime >= 200) {
                    CANBus_SendAddressClaim();
                    delay(50);
                    TCClient_SendWorkingSetMaster();
                    tcClient.wsAnnounced = true;
                }
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
            vtClient.vtNumSoftKeys = msg.buf[4] * msg.buf[5];
            vtClient.vtNumPhysicalSoftKeys = msg.buf[6];
            Serial.print("VT: Soft keys: virtual=");
            Serial.print(vtClient.vtNumSoftKeys);
            Serial.print(" physical=");
            Serial.println(vtClient.vtNumPhysicalSoftKeys);
            VTClient_SetState(VT_SEND_GET_TEXT_FONT);
            break;

        case VT_FUNC_GET_TEXT_FONT_DATA_RESP:
            Serial.println("VT: Text Font Data received");
            VTClient_SetState(VT_SEND_GET_HARDWARE);
            break;

        case VT_FUNC_GET_HARDWARE_RESP:
            // Byte 0: Function code 0xC7
            // Byte 2: Graphic type, Byte 4-5: width LE, Byte 6-7: height LE
            vtClient.vtColourDepth = msg.buf[2];
            vtClient.vtWidth = msg.buf[4] | ((uint16_t)msg.buf[5] << 8);
            vtClient.vtHeight = msg.buf[6] | ((uint16_t)msg.buf[7] << 8);
            Serial.print("VT: Hardware - colours=");
            Serial.print(vtClient.vtColourDepth);
            Serial.print(" width=");
            Serial.print(vtClient.vtWidth);
            Serial.print(" height=");
            Serial.println(vtClient.vtHeight);
            // Rebuild pool scaled to actual VT display dimensions
            VTPool_Build(vtClient.vtWidth, vtClient.vtHeight);
            vtClient.poolUploadStarted = false;
            VTClient_SetState(VT_UPLOAD_OBJECT_POOL);
            break;

        case VT_FUNC_END_OF_OBJECT_POOL_RESP:
            if (msg.buf[1] == 0) {
                Serial.println("VT: Object pool accepted - VT Client CONNECTED");
                // Force initial display update
                vtClient.lastRate1Actual = 0xFFFFFFFF;
                vtClient.lastRate1Target = 0xFFFFFFFF;
                vtClient.lastRate2Actual = 0xFFFFFFFF;
                vtClient.lastRate2Target = 0xFFFFFFFF;
                vtClient.lastQtyApplied = 0xFFFFFFFF;
                vtClient.lastAreaRemaining = 0xFFFFFFFF;
                vtClient.lastTankLevel = 0xFFFFFFFF;
                vtClient.lastSpeed = 0xFFFFFFFF;
                vtClient.lastSectionStates = 0xFF;
                vtClient.lastAOGState = 0xFF;
                VTClient_SetState(VT_CONNECTED);
            } else {
                Serial.print("VT: Object pool rejected, error=");
                Serial.println(msg.buf[1]);
                VTClient_SetState(VT_ERROR);
            }
            break;

        case VT_FUNC_BUTTON_ACTIVATION:
            VTClient_HandleButtonActivation(msg);
            break;

        case VT_FUNC_SOFT_KEY_ACTIVATION:
            VTClient_HandleSoftKeyActivation(msg);
            break;

        case VT_FUNC_CHANGE_NUMERIC_VALUE:
            VTClient_HandleChangeNumericValue(msg);
            break;
    }
}

//=============================================================================
// Interaction Handlers
//=============================================================================

void VTClient_HandleButtonActivation(const CAN_message_t& msg) {
    // msg.buf[0] = 0x01 (Button Activation)
    // msg.buf[1-2] = button object ID (LE)
    // msg.buf[3] = key code (1-8 for section buttons)
    // msg.buf[4] = activation type: 0=released, 1=pressed, 2=held, 3=aborted
    uint8_t keyCode = msg.buf[3];
    uint8_t activation = msg.buf[4];

    // Only handle press events (activation == 1)
    if (activation != 1) return;

    if (keyCode >= 1 && keyCode <= 8) {
        uint8_t btnIdx = keyCode - 1;

        // If Auto is ON and AOG is connected, ignore section button presses
        // (AOG controls sections in auto mode)
        bool aogConnected = (millis() - Sensor[0].CommTime < VT_AOG_TIMEOUT);
        if (Sensor[0].AutoOn && aogConnected) {
            Serial.println("VT: Section button ignored (Auto+AOG active)");
            return;
        }

        // Toggle the sections assigned to this button
        uint16_t mask = sectionButtonMap[btnIdx].sectionMask;
        uint16_t currentSections = RelayLo | ((uint16_t)RelayHi << 8);

        // Check if any section in this group is currently on
        bool anyOn = (currentSections & mask) != 0;

        if (anyOn) {
            // Turn off all sections in this group
            currentSections &= ~mask;
        } else {
            // Turn on all sections in this group (only if master is on)
            if (MasterOn) {
                currentSections |= mask;
            }
        }

        RelayLo = currentSections & 0xFF;
        RelayHi = (currentSections >> 8) & 0xFF;

        Serial.print("VT: Section button ");
        Serial.print(keyCode);
        Serial.print(anyOn ? " OFF" : " ON");
        Serial.print(" mask=0x");
        Serial.println(mask, HEX);
    }
}

void VTClient_HandleSoftKeyActivation(const CAN_message_t& msg) {
    // msg.buf[0] = 0x00 (Soft Key Activation)
    // msg.buf[1-2] = key object ID (LE)
    // msg.buf[3] = key code
    // msg.buf[4] = activation type: 0=released, 1=pressed, 2=held, 3=aborted
    uint8_t keyCode = msg.buf[3];
    uint8_t activation = msg.buf[4];

    // Only handle press events
    if (activation != 1) return;

    switch (keyCode) {
        case VT_KEYCODE_AUTO:
            // Toggle Auto mode for all sensors
            {
                bool newAuto = !Sensor[0].AutoOn;
                for (uint8_t i = 0; i < MDL.SensorCount; i++) {
                    Sensor[i].AutoOn = newAuto;
                }
                Serial.print("VT: Auto ");
                Serial.println(newAuto ? "ON" : "OFF");
            }
            break;

        case VT_KEYCODE_MASTER:
            // Toggle Master on/off
            MasterOn = !MasterOn;
            if (!MasterOn) {
                // Master off: turn off all relays
                RelayLo = 0;
                RelayHi = 0;
            }
            Serial.print("VT: Master ");
            Serial.println(MasterOn ? "ON" : "OFF");
            break;

        case VT_KEYCODE_PROD_NEXT:
            // Next product
            vtClient.currentProduct++;
            if (vtClient.currentProduct >= MDL.SensorCount) {
                vtClient.currentProduct = 0;
            }
            // Force display refresh
            vtClient.lastRate1Actual = 0xFFFFFFFF;
            vtClient.lastRate1Target = 0xFFFFFFFF;
            vtClient.lastQtyApplied = 0xFFFFFFFF;
            Serial.print("VT: Product -> ");
            Serial.println(vtClient.currentProduct);
            break;

        case VT_KEYCODE_PROD_PREV:
            // Previous product
            if (vtClient.currentProduct == 0) {
                vtClient.currentProduct = MDL.SensorCount - 1;
            } else {
                vtClient.currentProduct--;
            }
            // Force display refresh
            vtClient.lastRate1Actual = 0xFFFFFFFF;
            vtClient.lastRate1Target = 0xFFFFFFFF;
            vtClient.lastQtyApplied = 0xFFFFFFFF;
            Serial.print("VT: Product -> ");
            Serial.println(vtClient.currentProduct);
            break;
    }
}

void VTClient_HandleChangeNumericValue(const CAN_message_t& msg) {
    // VT sends this when user edits an InputNumber
    // msg.buf[0] = 0xA8
    // msg.buf[1-2] = object ID (LE)
    // msg.buf[3] = reserved (0xFF)
    // msg.buf[4-7] = new value (32-bit LE)
    uint16_t objId = msg.buf[1] | ((uint16_t)msg.buf[2] << 8);
    uint32_t newValue = msg.buf[4] | ((uint32_t)msg.buf[5] << 8) |
                        ((uint32_t)msg.buf[6] << 16) | ((uint32_t)msg.buf[7] << 24);

    if (objId == VT_OBJ_VAR_SPEED) {
        // Speed value is stored as raw * 10 (1 decimal place)
        vtClient.fixedSpeed = newValue / 10.0f;
        Serial.print("VT: Fixed speed set to ");
        Serial.println(vtClient.fixedSpeed);
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

void VTClient_SendChangeAttribute(uint16_t objId, uint8_t attrId, uint32_t value) {
    // Change Attribute command (func 0xAF)
    uint8_t data[8];
    data[0] = VT_FUNC_CHANGE_ATTRIBUTE;
    data[1] = objId & 0xFF;
    data[2] = (objId >> 8) & 0xFF;
    data[3] = attrId;
    data[4] = value & 0xFF;
    data[5] = (value >> 8) & 0xFF;
    data[6] = (value >> 16) & 0xFF;
    data[7] = (value >> 24) & 0xFF;

    VTClient_SendToVT(data, 8);
}

void VTClient_UpdateDisplay() {
    // --- Rate values ---
    // For SensorCount=1 or product switching, use currentProduct index
    uint8_t prodIdx = vtClient.currentProduct;
    if (prodIdx >= MDL.SensorCount) prodIdx = 0;

    // Rate 1 Actual (stored as value * 10 for 1 decimal place)
    uint32_t rate1Actual = (uint32_t)(Sensor[prodIdx].UPM * 10.0);
    if (rate1Actual != vtClient.lastRate1Actual) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE1_ACTUAL, rate1Actual);
        vtClient.lastRate1Actual = rate1Actual;
    }

    // Rate 1 Target
    uint32_t rate1Target = (uint32_t)(Sensor[prodIdx].TargetUPM * 10.0);
    if (rate1Target != vtClient.lastRate1Target) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_RATE1_TARGET, rate1Target);
        vtClient.lastRate1Target = rate1Target;
    }

    // Rate 2 (if 2-sensor layout and showing both)
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

    // Quantity applied (TotalPulses / MeterCal, displayed as liters * 10)
    uint32_t qtyApplied = 0;
    if (Sensor[prodIdx].MeterCal > 0.0) {
        qtyApplied = (uint32_t)((float)Sensor[prodIdx].TotalPulses / Sensor[prodIdx].MeterCal * 10.0);
    }
    if (qtyApplied != vtClient.lastQtyApplied) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_QTY_APPLIED, qtyApplied);
        vtClient.lastQtyApplied = qtyApplied;
    }

    // Area remaining (placeholder - calculation TBD)
    uint32_t areaRemaining = 0;  // TODO: compute area remaining in ha * 10
    if (areaRemaining != vtClient.lastAreaRemaining) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_AREA_REM, areaRemaining);
        vtClient.lastAreaRemaining = areaRemaining;
    }

    // Tank level (placeholder - needs tank capacity config)
    uint32_t tankLevel = 0;  // TODO: compute tank level 0-1000 (0-100.0%)
    if (tankLevel != vtClient.lastTankLevel) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_TANK_LEVEL, tankLevel);
        vtClient.lastTankLevel = tankLevel;
    }

    // Speed display
    // Priority: WheelSpeed > fixedSpeed (both are in km/h, stored * 10)
    float currentSpeed = (WheelSpeed > 0.0) ? WheelSpeed : vtClient.fixedSpeed;
    uint32_t speedVal = (uint32_t)(currentSpeed * 10.0);
    if (speedVal != vtClient.lastSpeed) {
        VTClient_SendChangeNumericValue(VT_OBJ_VAR_SPEED, speedVal);
        vtClient.lastSpeed = speedVal;
    }

    // --- AOG connection indicator ---
    bool aogConnected = (millis() - Sensor[0].CommTime < VT_AOG_TIMEOUT);
    uint8_t aogState = aogConnected ? 1 : 0;
    if (aogState != vtClient.lastAOGState) {
        // Change AOG rectangle fill: green if connected, red if not
        VTClient_SendChangeAttribute(VT_OBJ_RECT_AOG, 5,
            aogConnected ? VT_OBJ_FILL_GREEN : VT_OBJ_FILL_RED);
        vtClient.lastAOGState = aogState;
    }

    // --- Section button colours (8 buttons) ---
    // Compute button states from relay bits using section mapping
    uint16_t currentSections = RelayLo | ((uint16_t)RelayHi << 8);
    uint8_t buttonStates = 0;
    for (uint8_t i = 0; i < 8; i++) {
        if ((currentSections & sectionButtonMap[i].sectionMask) != 0) {
            buttonStates |= (1 << i);
        }
    }

    if (buttonStates != vtClient.lastSectionStates) {
        for (uint8_t i = 0; i < 8; i++) {
            bool btnOn = (buttonStates >> i) & 0x01;
            bool wasOn = (vtClient.lastSectionStates >> i) & 0x01;

            if (btnOn != wasOn || vtClient.lastSectionStates == 0xFF) {
                // Change button background colour (attrID=3 for Button)
                VTClient_SendChangeAttribute(VT_OBJ_BTN_SECTION_BASE + i, 3,
                    btnOn ? VT_COLOUR_GREEN : VT_COLOUR_RED);
            }
        }
        vtClient.lastSectionStates = buttonStates;
    }
}

//=============================================================================
// Access Functions
//=============================================================================

uint8_t VTClient_GetState() {
    return vtClient.state;
}
