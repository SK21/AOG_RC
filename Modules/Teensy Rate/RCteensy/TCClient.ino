
// TCClient.ino - TC Client state machine for ISO 11783-10 Task Controller
// Implements TC Client protocol to communicate with Gateway TC Server
// Note: Shared definitions are in TCDefs.h (included by RCteensy.ino)

//=============================================================================
// TC Client Constants (not shared)
//=============================================================================

// TC Server Status bits (from PGN 65272)
#define TC_STATUS_BUSY                    0x01
#define TC_STATUS_OUT_OF_MEMORY           0x80

// TC Version
#define TC_VERSION_DIS                    0   // Draft International Standard
#define TC_VERSION_FDIS                   1   // Final Draft
#define TC_VERSION_SECOND_EDITION         2   // Second Edition
#define TC_VERSION_SECOND_EDITION_REV1    3   // Second Edition Rev 1
#define TC_VERSION_THIRD_EDITION          4   // Third Edition

// Process Data Commands
#define PD_CMD_TECH_CAPABILITIES          0x00
#define PD_CMD_DEVICE_DESCRIPTOR          0x01
#define PD_CMD_REQUEST_LOCALIZATION       0x02
#define PD_CMD_OBJECT_POOL_TRANSFER       0x11
#define PD_CMD_OBJECT_POOL_TRANSFER_RESP  0x12
#define PD_CMD_OBJECT_POOL_ACTIVATE       0x21
#define PD_CMD_OBJECT_POOL_ACTIVATE_RESP  0x22
#define PD_CMD_OBJECT_POOL_DELETE         0x31
#define PD_CMD_OBJECT_POOL_DELETE_RESP    0x32
#define PD_CMD_REQUEST_VALUE              0x02  // TC requests value from client
#define PD_CMD_VALUE                      0x03  // Value command (set value or response)
#define PD_CMD_SET_VALUE_AND_ACK          0x0A  // Set value and request acknowledgement
#define PD_CMD_MEASUREMENT_TIME_INTERVAL  0xE0
#define PD_CMD_MEASUREMENT_DISTANCE       0xE1
#define PD_CMD_MEASUREMENT_MIN_THRESHOLD  0xE2
#define PD_CMD_MEASUREMENT_MAX_THRESHOLD  0xE3
#define PD_CMD_MEASUREMENT_CHANGE_THRESH  0xE4
#define PD_CMD_PROCESS_DATA_ACK           0x2F
#define PD_CMD_STATUS                     0x0E  // TC Status message (byte[0] = 0xFE)
#define PD_CMD_DEVICE_DESCRIPTOR          0x01  // Device Descriptor commands

// Device Descriptor sub-commands (combined with PD_CMD_DEVICE_DESCRIPTOR)
#define DD_REQUEST_OBJECT_POOL_TRANSFER       0x41  // 0x01 | (0x04 << 4)
#define DD_REQUEST_OBJECT_POOL_TRANSFER_RESP  0x51  // 0x01 | (0x05 << 4)
#define DD_OBJECT_POOL_TRANSFER               0x61  // 0x01 | (0x06 << 4)
#define DD_OBJECT_POOL_TRANSFER_RESP          0x71  // 0x01 | (0x07 << 4)
#define DD_OBJECT_POOL_ACTIVATE               0x81  // 0x01 | (0x08 << 4)
#define DD_OBJECT_POOL_ACTIVATE_RESP          0x91  // 0x01 | (0x09 << 4)

//=============================================================================
// TC Client State Variables
//=============================================================================

struct TCClientData {
    TCClientState state = TC_IDLE;
    uint8_t tcServerAddress = 0xFF;        // Address of TC Server (Gateway)
    uint8_t tcVersion = 0;                 // TC version from status message
    uint8_t tcStatus = 0;                  // Last received status byte
    uint8_t tcNumberBoomsSupported = 0;
    uint8_t tcNumberSectionsSupported = 0;
    uint8_t tcNumberChannelsSupported = 0;
    uint32_t lastTCStatusTime = 0;         // Last time we received TC status
    uint32_t stateEntryTime = 0;           // When we entered current state
    bool structureLabelMatched = false;    // TC has our DDOP cached
    uint8_t activationErrorCode = 0;
    bool wsAnnounced = false;              // Working Set Master announced
    uint16_t processDataInterval = 200;    // Send process data every 200ms
    uint32_t lastProcessDataTime = 0;
    uint16_t clientTaskInterval = 2000;    // Send ClientTask status every 2s (ISO 11783-10)
    uint32_t lastClientTaskTime = 0;       // Last time we sent ClientTask status

    // Current setpoint from TC
    float setpointRate[MaxProductCount];   // Target rate from DDI 1
    uint8_t sectionState = 0;              // Section states from DDI 157

    // Measurement request tracking (per DDI)
    uint32_t measurementIntervalDDI2 = 0;   // Requested interval for DDI 2 (ms), 0 = not requested
    uint32_t measurementIntervalDDI48 = 0;  // Requested interval for DDI 48 (ms), 0 = not requested
    uint32_t measurementIntervalDDI157 = 0; // Requested interval for DDI 157 (ms), 0 = not requested
    bool measurementsRequested = false;     // True if any measurement has been requested
};

TCClientData tcClient;

//=============================================================================
// Forward Declarations
//=============================================================================

void TCClient_SendWorkingSetMaster();
void TCClient_SendStructureLabelRequest();
void TCClient_SendDDOP();
void TCClient_SendActivationRequest();
void TCClient_SendProcessData();
void TCClient_SendValueForDDI(uint16_t elementNumber, uint16_t ddi);
void TCClient_HandleTCStatus(const CAN_message_t& msg);
void TCClient_HandleProcessData(const CAN_message_t& msg);
void TCClient_HandleTPData(const CAN_message_t& msg);
void TCClient_SetState(TCClientState newState);
uint64_t TCClient_GetNAME();

//=============================================================================
// Initialization
//=============================================================================

void TCClient_Begin() {
    tcClient.state = TC_IDLE;
    tcClient.tcServerAddress = 0xFF;
    tcClient.lastTCStatusTime = 0;
    tcClient.stateEntryTime = millis();
    tcClient.wsAnnounced = false;
    tcClient.structureLabelMatched = false;

    for (int i = 0; i < MaxProductCount; i++) {
        tcClient.setpointRate[i] = 0;
    }
    tcClient.sectionState = 0;

    // Build DDOP at startup
    DDOP_Build();

    Serial.println("TC Client initialized");
}

//=============================================================================
// State Machine
//=============================================================================

void TCClient_Update() {
    uint32_t now = millis();
    uint32_t stateTime = now - tcClient.stateEntryTime;

    // Send ClientTask status every 2 seconds in all states after Working Set Master
    // This keeps the connection alive during DDOP upload which can take several seconds
    if (tcClient.state > TC_SEND_WORKING_SET_MASTER && tcClient.tcServerAddress != 0xFF) {
        if (now - tcClient.lastClientTaskTime >= tcClient.clientTaskInterval) {
            tcClient.lastClientTaskTime = now;
            TCClient_SendClientTaskStatus();
        }
    }

    switch (tcClient.state) {
        case TC_IDLE:
            // Wait for TC Server status message (PGN 65272)
            // TC Server broadcasts this periodically
            if (tcClient.tcServerAddress != 0xFF) {
                TCClient_SetState(TC_WAIT_FOR_TC);
            }
            break;

        case TC_WAIT_FOR_TC:
            // We have seen TC Server, brief wait for bus to stabilize
            if (stateTime >= 500) {
                TCClient_SetState(TC_SEND_WORKING_SET_MASTER);
            }
            // Check for TC timeout
            if (now - tcClient.lastTCStatusTime > 6000) {
                Serial.println("TC Server timeout, returning to IDLE");
                TCClient_SetState(TC_IDLE);
                tcClient.tcServerAddress = 0xFF;
            }
            break;

        case TC_SEND_WORKING_SET_MASTER:
            // Send Working Set Master message
            if (!tcClient.wsAnnounced) {
                // Re-send address claim to ensure Gateway has registered us
                CANBus_SendAddressClaim();
                delay(50);  // Brief wait for AgIsoStack++ to register external control function
                TCClient_SendWorkingSetMaster();
                tcClient.wsAnnounced = true;
            }
            // Wait for Gateway to process Working Set Master
            if (stateTime >= 500) {
                TCClient_SetState(TC_WAIT_STRUCTURE_LABEL);
                TCClient_SendStructureLabelRequest();
            }
            break;

        case TC_WAIT_STRUCTURE_LABEL:
            // Wait for structure label response or timeout
            // Gateway always returns "not cached", so short timeout is fine
            if (stateTime >= 500) {
                // No cached DDOP, need to upload - first request permission
                tcClient.structureLabelMatched = false;
                TCClient_SetState(TC_REQUEST_OBJECT_POOL_TRANSFER);
            }
            break;

        case TC_REQUEST_OBJECT_POOL_TRANSFER:
            // Send request to transfer object pool
            TCClient_SendRequestObjectPoolTransfer();
            TCClient_SetState(TC_WAIT_OBJECT_POOL_TRANSFER_RESP);
            break;

        case TC_WAIT_OBJECT_POOL_TRANSFER_RESP:
            // Wait for permission to send DDOP
            if (stateTime >= 2000) {
                Serial.println("Request Object Pool Transfer timeout");
                TCClient_SetState(TC_ERROR);
            }
            break;

        case TC_SEND_DDOP:
            // Upload DDOP using Transport Protocol (permission received)
            TCClient_SendDDOP();
            TCClient_SetState(TC_WAIT_DDOP_STORED);
            break;

        case TC_WAIT_DDOP_STORED:
            // Wait for Object Pool Transfer Response
            if (stateTime >= 10000) {
                Serial.println("DDOP upload timeout");
                TCClient_SetState(TC_ERROR);
            }
            break;

        case TC_REQUEST_ACTIVATION:
            TCClient_SendActivationRequest();
            TCClient_SetState(TC_WAIT_ACTIVATION);
            break;

        case TC_WAIT_ACTIVATION:
            // Wait for activation response
            if (stateTime >= 2000) {
                Serial.println("Activation timeout");
                TCClient_SetState(TC_ERROR);
            }
            break;

        case TC_ACTIVE:
            // Normal operation - send process data periodically
            // (ClientTask status is sent at the top of this function for all states)
            if (now - tcClient.lastProcessDataTime >= tcClient.processDataInterval) {
                tcClient.lastProcessDataTime = now;
                TCClient_SendProcessData();
            }
            // Check for TC timeout
            if (now - tcClient.lastTCStatusTime > 6000) {
                Serial.println("TC Server lost connection");
                TCClient_SetState(TC_IDLE);
                tcClient.tcServerAddress = 0xFF;
            }
            break;

        case TC_ERROR:
            // Wait before retry
            if (stateTime >= 2000) {
                tcClient.wsAnnounced = false;
                TCClient_SetState(TC_IDLE);
            }
            break;
    }
}

void TCClient_SetState(TCClientState newState) {
    if (tcClient.state != newState) {
        Serial.print("TC Client state: ");
        Serial.print(tcClient.state);
        Serial.print(" -> ");
        Serial.println(newState);
        tcClient.state = newState;
        tcClient.stateEntryTime = millis();
    }
}

//=============================================================================
// Message Handlers
//=============================================================================

void TCClient_HandleTCStatus(const CAN_message_t& msg) {
    // PGN 65272 (0xFEF8) - Task Controller Status
    // Byte 0: TC status bits
    // Byte 1: TC command source address
    // Byte 2: TC command (active task)
    // Byte 3: Reserved
    // Byte 4: TC version + options
    // Byte 5: Number of booms supported
    // Byte 6: Number of sections supported
    // Byte 7: Number of position control channels

    uint8_t sourceAddr = msg.id & 0xFF;

    tcClient.tcStatus = msg.buf[0];
    tcClient.tcVersion = msg.buf[4] & 0x0F;
    tcClient.tcNumberBoomsSupported = msg.buf[5];
    tcClient.tcNumberSectionsSupported = msg.buf[6];
    tcClient.tcNumberChannelsSupported = msg.buf[7];
    tcClient.lastTCStatusTime = millis();

    if (tcClient.tcServerAddress == 0xFF) {
        tcClient.tcServerAddress = sourceAddr;
        Serial.print("TC Server detected at address 0x");
        Serial.print(sourceAddr, HEX);
        Serial.print(", Version: ");
        Serial.print(tcClient.tcVersion);
        Serial.print(", Sections: ");
        Serial.println(tcClient.tcNumberSectionsSupported);
    }
}

void TCClient_HandleProcessData(const CAN_message_t& msg, uint8_t pf, uint8_t ps) {
    // Process Data messages - PGN 0xCB00-0xCBFF (destination specific)
    // or broadcast depending on message type

    uint8_t cmdByte = msg.buf[0];
    uint8_t command = cmdByte & 0x0F;  // Lower nibble for process data commands
    uint8_t sourceAddr = msg.id & 0xFF;

    // Check for Device Descriptor commands (lower nibble = 0x01)
    if (command == PD_CMD_DEVICE_DESCRIPTOR) {
        // Handle full command byte for DD commands
        switch (cmdByte) {
            case DD_REQUEST_OBJECT_POOL_TRANSFER_RESP:  // 0x51
                // Response to request object pool transfer
                if (msg.buf[1] == 0) {
                    // Permission granted - proceed to send DDOP
                    Serial.println("Object Pool Transfer permitted");
                    TCClient_SetState(TC_SEND_DDOP);
                } else {
                    Serial.print("Object Pool Transfer denied, error: ");
                    Serial.println(msg.buf[1]);
                    TCClient_SetState(TC_ERROR);
                }
                break;

            case DD_OBJECT_POOL_TRANSFER_RESP:  // 0x71
                // Response to DDOP upload
                if (msg.buf[1] == 0) {
                    Serial.println("DDOP stored successfully");
                    TCClient_SetState(TC_REQUEST_ACTIVATION);
                } else {
                    Serial.print("DDOP store failed, error: ");
                    Serial.println(msg.buf[1]);
                    TCClient_SetState(TC_ERROR);
                }
                break;

            case DD_OBJECT_POOL_ACTIVATE_RESP:  // 0x91
                // Response to activation request
                if (msg.buf[1] == 0) {
                    Serial.println("DDOP activated - TC Client now ACTIVE");
                    TCClient_SetState(TC_ACTIVE);
                } else {
                    tcClient.activationErrorCode = msg.buf[1];
                    Serial.print("DDOP activation failed, error: ");
                    Serial.println(msg.buf[1]);
                    TCClient_SetState(TC_ERROR);
                }
                break;
        }
        return;
    }

    // Handle other process data commands
    switch (command) {
        case PD_CMD_STATUS:
            // TC Server Status message on PGN 0xCB00
            // Byte 0: 0xFE (command = 0x0E | 0xF0)
            // Byte 1-3: 0xFF reserved
            // Byte 4: Status byte
            // Byte 5: Command source address
            // Byte 6: Command byte
            // Byte 7: 0xFF reserved
            tcClient.tcStatus = msg.buf[4];
            tcClient.lastTCStatusTime = millis();

            if (tcClient.tcServerAddress == 0xFF) {
                tcClient.tcServerAddress = sourceAddr;
                Serial.print("TC Server detected at address 0x");
                Serial.println(sourceAddr, HEX);
            }
            break;

        case PD_CMD_REQUEST_VALUE: {
            // TC Server requesting current value for a DDI/element
            uint16_t reqElement = (msg.buf[0] >> 4) | ((uint16_t)msg.buf[1] << 4);
            uint16_t reqDdi = msg.buf[2] | ((uint16_t)msg.buf[3] << 8);
            Serial.print("TC Request Value DDI ");
            Serial.print(reqDdi);
            Serial.print(" Element ");
            Serial.println(reqElement);
            TCClient_SendValueForDDI(reqElement, reqDdi);
            break;
        }

        // PD_CMD_OBJECT_POOL_TRANSFER_RESP and PD_CMD_OBJECT_POOL_ACTIVATE_RESP
        // are now handled via DD commands above

        case PD_CMD_VALUE:
        case PD_CMD_SET_VALUE_AND_ACK:
            // TC sending us a value (setpoint rate or section control)
            TCClient_HandleValueCommand(msg);
            break;

        case PD_CMD_MEASUREMENT_TIME_INTERVAL:
        case PD_CMD_MEASUREMENT_DISTANCE:
        case PD_CMD_MEASUREMENT_MIN_THRESHOLD:
        case PD_CMD_MEASUREMENT_MAX_THRESHOLD:
        case PD_CMD_MEASUREMENT_CHANGE_THRESH:
            // TC requesting measurements - we respond with process data
            TCClient_HandleMeasurementRequest(msg);
            break;
    }
}

void TCClient_HandleValueCommand(const CAN_message_t& msg) {
    // ISO 11783-10 Process Data format:
    // Byte 0: Command (low nibble) | Element bits 0-3 (high nibble)
    // Byte 1: Element bits 4-11
    // Bytes 2-3: DDI (little endian)
    // Bytes 4-7: Value (little endian, 32-bit signed)

    uint16_t elementNumber = (msg.buf[0] >> 4) | ((uint16_t)msg.buf[1] << 4);
    uint16_t ddi = msg.buf[2] | ((uint16_t)msg.buf[3] << 8);
    int32_t value = msg.buf[4] | ((uint32_t)msg.buf[5] << 8) |
                    ((uint32_t)msg.buf[6] << 16) | ((uint32_t)msg.buf[7] << 24);

    switch (ddi) {
        case DDI_SETPOINT_VOLUME_PER_AREA:
            // Setpoint rate in mm³/m² (= 0.01 L/ha, or 100 = 1 L/ha)
            // Convert to UPM for our system
            {
                uint8_t senId = 0;
                if (elementNumber >= ELEMENT_SECTION_BASE) {
                    // Section-specific setpoint
                    senId = (elementNumber - ELEMENT_SECTION_BASE) < MaxProductCount ?
                            (elementNumber - ELEMENT_SECTION_BASE) : 0;
                }

                // Convert mm³/m² to L/ha: divide by 100
                // Then convert to UPM using our calibration
                float rate_Lha = value / 100.0;
                tcClient.setpointRate[senId] = rate_Lha;

                // Apply to sensor target
                Sensor[senId].TargetUPM = rate_Lha;
                Sensor[senId].CommTime = millis();

                Serial.print("TC Setpoint DDI ");
                Serial.print(ddi);
                Serial.print(" Element ");
                Serial.print(elementNumber);
                Serial.print(" = ");
                Serial.print(rate_Lha);
                Serial.println(" L/ha");
            }
            break;

        case DDI_SECTION_CONTROL_STATE:
            // Section control - bit field
            // Value contains on/off states for sections
            {
                // Apply to relays
                // Map sections to relay bits based on element number
                if (elementNumber >= ELEMENT_SECTION_BASE) {
                    uint8_t section = elementNumber - ELEMENT_SECTION_BASE;
                    if (section < 8) {
                        if (value) {
                            RelayLo |= (1 << section);
                        } else {
                            RelayLo &= ~(1 << section);
                        }
                    } else if (section < 16) {
                        if (value) {
                            RelayHi |= (1 << (section - 8));
                        } else {
                            RelayHi &= ~(1 << (section - 8));
                        }
                    }
                } else if (elementNumber == ELEMENT_BOOM) {
                    // Boom-level section control - apply to all sections
                    RelayLo = value & 0xFF;
                    RelayHi = (value >> 8) & 0xFF;
                }

                Serial.print("TC Section control Element ");
                Serial.print(elementNumber);
                Serial.print(" = 0x");
                Serial.println(value, HEX);
            }
            break;
    }

    // Send acknowledgement if requested
    if ((msg.buf[0] & 0x0F) == PD_CMD_SET_VALUE_AND_ACK) {
        TCClient_SendProcessDataAck(elementNumber, ddi, 0);
    }
}

void TCClient_HandleMeasurementRequest(const CAN_message_t& msg) {
    // TC is setting up a measurement trigger
    // ISO 11783-10 format: element in byte0 high nibble + byte1, DDI in bytes 2-3
    uint16_t elementNumber = (msg.buf[0] >> 4) | ((uint16_t)msg.buf[1] << 4);
    uint16_t ddi = msg.buf[2] | ((uint16_t)msg.buf[3] << 8);
    uint8_t command = msg.buf[0] & 0x0F;

    // Extract interval/threshold value from bytes 4-7
    uint32_t value = msg.buf[4] | ((uint32_t)msg.buf[5] << 8) |
                     ((uint32_t)msg.buf[6] << 16) | ((uint32_t)msg.buf[7] << 24);

    Serial.print("TC Measurement request cmd=0x");
    Serial.print(command, HEX);
    Serial.print(" DDI ");
    Serial.print(ddi);
    Serial.print(" Element ");
    Serial.print(elementNumber);
    Serial.print(" value=");
    Serial.println(value);

    // Store per-DDI measurement interval for time-based requests
    if (command == PD_CMD_MEASUREMENT_TIME_INTERVAL) {
        switch (ddi) {
            case DDI_ACTUAL_VOLUME_PER_AREA:
                tcClient.measurementIntervalDDI2 = value;
                break;
            case DDI_ACTUAL_VOLUME:
                tcClient.measurementIntervalDDI48 = value;
                break;
            case DDI_SECTION_CONTROL_STATE:
                tcClient.measurementIntervalDDI157 = value;
                break;
        }

        tcClient.measurementsRequested = true;

        // Use the shortest requested interval as the global processDataInterval
        uint32_t shortest = 0xFFFFFFFF;
        if (tcClient.measurementIntervalDDI2 > 0 && tcClient.measurementIntervalDDI2 < shortest)
            shortest = tcClient.measurementIntervalDDI2;
        if (tcClient.measurementIntervalDDI48 > 0 && tcClient.measurementIntervalDDI48 < shortest)
            shortest = tcClient.measurementIntervalDDI48;
        if (tcClient.measurementIntervalDDI157 > 0 && tcClient.measurementIntervalDDI157 < shortest)
            shortest = tcClient.measurementIntervalDDI157;

        if (shortest < 0xFFFFFFFF) {
            tcClient.processDataInterval = (uint16_t)shortest;
            Serial.print("TC processDataInterval updated to ");
            Serial.println(tcClient.processDataInterval);
        }
    }

    // Respond immediately with current value for this DDI
    TCClient_SendValueForDDI(elementNumber, ddi);
}

//=============================================================================
// Message Senders
//=============================================================================

void TCClient_SendWorkingSetMaster() {
    // PGN 65037 (0xFE0D) - Working Set Master
    // Announces this device as a Working Set Master

    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xFE0DUL << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // Byte 0: Number of Working Set members (1 = just us)
    msg.buf[0] = 1;
    // Bytes 1-7: Reserved
    for (int i = 1; i < 8; i++) {
        msg.buf[i] = 0xFF;
    }

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.println("Sent Working Set Master");
}

void TCClient_SendStructureLabelRequest() {
    // Request Structure Label from TC
    // PGN 0xCB00 (Process Data) with destination = TC

    CAN_message_t msg;

    // PDU1 format: PGN = PF << 8, destination in PS
    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // Request Structure Label command
    msg.buf[0] = PD_CMD_REQUEST_LOCALIZATION;
    for (int i = 1; i < 8; i++) {
        msg.buf[i] = 0xFF;
    }

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.println("Sent Structure Label Request");
}

void TCClient_SendRequestObjectPoolTransfer() {
    // Request permission to transfer object pool
    // Message format: cmd(1) + size(4) + reserved(3)
    CAN_message_t msg;

    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // Command: DeviceDescriptor | RequestObjectPoolTransfer << 4 = 0x41
    msg.buf[0] = DD_REQUEST_OBJECT_POOL_TRANSFER;
    // DDOP size (32-bit little endian)
    msg.buf[1] = ddopSize & 0xFF;
    msg.buf[2] = (ddopSize >> 8) & 0xFF;
    msg.buf[3] = 0;  // ddopSize is 16-bit, upper bytes are 0
    msg.buf[4] = 0;
    msg.buf[5] = 0xFF;
    msg.buf[6] = 0xFF;
    msg.buf[7] = 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.print("Sent Request Object Pool Transfer, size: ");
    Serial.println(ddopSize);
}

void TCClient_SendDDOP() {
    // Send DDOP using Transport Protocol if > 8 bytes
    if (ddopSize > 8) {
        // Use TP (Transport Protocol)
        TP_SendDDOP(tcClient.tcServerAddress, ddopBuffer, ddopSize);
    } else {
        // Single frame - unlikely for DDOP but handle it
        CAN_message_t msg;
        msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
        msg.flags.extended = 1;
        msg.len = 8;

        msg.buf[0] = PD_CMD_OBJECT_POOL_TRANSFER;
        for (int i = 0; i < 7 && i < ddopSize; i++) {
            msg.buf[i + 1] = ddopBuffer[i];
        }

        ISOBUS.write(msg);
        canStats.txCount++;
    }

    Serial.print("Sending DDOP, size: ");
    Serial.println(ddopSize);
}

void TCClient_SendActivationRequest() {
    // Request DDOP activation
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // Command: DeviceDescriptor | ObjectPoolActivate << 4 = 0x81
    msg.buf[0] = DD_OBJECT_POOL_ACTIVATE;
    for (int i = 1; i < 8; i++) {
        msg.buf[i] = 0xFF;
    }

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.println("Sent DDOP Activation Request");
}

void TCClient_SendClientTaskStatus() {
    // Send TC Client Task status message to keep connection alive
    // Must be sent every 2 seconds per ISO 11783-10
    // PGN 0xCB00 (Process Data) to TC Server

    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // ClientTask message format:
    // Byte 0: 0xFF (0x0F | 0xF0) - ClientTask command with element N/A
    // Bytes 1-3: 0xFF (Element and DDI not applicable)
    // Byte 4: Status byte (bit 0 = task active)
    // Bytes 5-7: Reserved (0x00)
    msg.buf[0] = 0xFF;  // 0x0F | 0xF0
    msg.buf[1] = 0xFF;
    msg.buf[2] = 0xFF;
    msg.buf[3] = 0xFF;
    msg.buf[4] = 0x01;  // Task active
    msg.buf[5] = 0x00;
    msg.buf[6] = 0x00;
    msg.buf[7] = 0x00;

    ISOBUS.write(msg);
    canStats.txCount++;
}

void TCClient_SendProcessData() {
    // Send actual rate (DDI 2) and accumulated quantity (DDI 48) for each sensor

    Serial.print("TC: SendProcessData sensors=");
    Serial.println(MDL.SensorCount);

    for (int i = 0; i < MDL.SensorCount; i++) {
        // Send Actual Volume Per Area (DDI 2)
        int32_t rateValue = TCClient_ConvertRateToDDI(Sensor[i].UPM);
        TCClient_SendValueMessage(ELEMENT_SECTION_BASE + i, DDI_ACTUAL_VOLUME_PER_AREA, rateValue);

        // Send Actual Volume (DDI 48) in mL
        float quantity_ml = 0;
        if (Sensor[i].MeterCal > 0) {
            quantity_ml = (Sensor[i].TotalPulses / Sensor[i].MeterCal) * 1000.0;
        }
        TCClient_SendValueMessage(ELEMENT_SECTION_BASE + i, DDI_ACTUAL_VOLUME, (int32_t)quantity_ml);

        Serial.print("  Sensor ");
        Serial.print(i);
        Serial.print(": rate=");
        Serial.print(rateValue);
        Serial.print(" qty=");
        Serial.println((int32_t)quantity_ml);
    }
}

void TCClient_SendValueMessage(uint16_t element, uint16_t ddi, int32_t value) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // ISO 11783-10 Process Data format:
    // Byte 0: Command (low nibble) | Element bits 0-3 (high nibble)
    // Byte 1: Element bits 4-11
    // Bytes 2-3: DDI (little endian)
    // Bytes 4-7: Value (little endian, 32-bit signed)
    msg.buf[0] = (PD_CMD_VALUE & 0x0F) | ((element & 0x0F) << 4);
    msg.buf[1] = (element >> 4) & 0xFF;
    msg.buf[2] = ddi & 0xFF;
    msg.buf[3] = (ddi >> 8) & 0xFF;
    msg.buf[4] = value & 0xFF;
    msg.buf[5] = (value >> 8) & 0xFF;
    msg.buf[6] = (value >> 16) & 0xFF;
    msg.buf[7] = (value >> 24) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
}

void TCClient_SendProcessDataAck(uint16_t element, uint16_t ddi, uint8_t errorCode) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xCBUL << 16) | ((uint32_t)tcClient.tcServerAddress << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // ISO 11783-10 Process Data ACK format:
    // Byte 0: Command (low nibble) | Element bits 0-3 (high nibble)
    // Byte 1: Element bits 4-11
    // Bytes 2-3: DDI (little endian)
    // Bytes 4-7: PDACK, error code, reserved
    msg.buf[0] = (PD_CMD_PROCESS_DATA_ACK & 0x0F) | ((element & 0x0F) << 4);
    msg.buf[1] = (element >> 4) & 0xFF;
    msg.buf[2] = ddi & 0xFF;
    msg.buf[3] = (ddi >> 8) & 0xFF;
    msg.buf[4] = errorCode;
    msg.buf[5] = PD_CMD_SET_VALUE_AND_ACK;  // Echo command
    msg.buf[6] = 0xFF;
    msg.buf[7] = 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
}

//=============================================================================
// Value Response Helper
//=============================================================================

void TCClient_SendValueForDDI(uint16_t elementNumber, uint16_t ddi) {
    // Determine sensor index from element number
    uint8_t senId = 0;
    if (elementNumber >= ELEMENT_SECTION_BASE) {
        senId = (elementNumber - ELEMENT_SECTION_BASE);
        if (senId >= MaxProductCount) senId = 0;
    }

    int32_t value = 0;
    switch (ddi) {
        case DDI_ACTUAL_VOLUME_PER_AREA:
            // Current rate in mm³/m² (1 L/ha = 100 mm³/m²)
            value = TCClient_ConvertRateToDDI(Sensor[senId].UPM);
            break;
        case DDI_ACTUAL_VOLUME:
            // Accumulated volume in mL
            if (Sensor[senId].MeterCal > 0) {
                value = (int32_t)((Sensor[senId].TotalPulses / Sensor[senId].MeterCal) * 1000.0);
            }
            break;
        case DDI_SECTION_CONTROL_STATE:
            // Current section state
            if (elementNumber >= ELEMENT_SECTION_BASE) {
                uint8_t section = elementNumber - ELEMENT_SECTION_BASE;
                if (section < 8) {
                    value = (RelayLo & (1 << section)) ? 1 : 0;
                } else if (section < 16) {
                    value = (RelayHi & (1 << (section - 8))) ? 1 : 0;
                }
            } else if (elementNumber == ELEMENT_BOOM) {
                value = RelayLo | ((uint16_t)RelayHi << 8);
            }
            break;
        default:
            return;  // Unknown DDI, don't respond
    }

    TCClient_SendValueMessage(elementNumber, ddi, value);
}

//=============================================================================
// Unit Conversion Helpers
//=============================================================================

int32_t TCClient_ConvertRateToDDI(float upm) {
    // Convert UPM (Units Per Minute, typically L/ha) to DDI 2 format (mm³/m²)
    // 1 L/ha = 100 mm³/m²
    return (int32_t)(upm * 100.0);
}

float TCClient_ConvertDDIToRate(int32_t ddiValue) {
    // Convert DDI format (mm³/m²) to L/ha
    return ddiValue / 100.0;
}

int32_t TCClient_ConvertQuantityToDDI(float liters) {
    // Convert liters to mL for DDI 48
    return (int32_t)(liters * 1000.0);
}

//=============================================================================
// Access functions for other modules
//=============================================================================

bool TCClient_IsActive() {
    return tcClient.state == TC_ACTIVE;
}

uint8_t TCClient_GetState() {
    return tcClient.state;
}

uint8_t TCClient_GetTCAddress() {
    return tcClient.tcServerAddress;
}

float TCClient_GetSetpointRate(uint8_t sensorId) {
    if (sensorId < MaxProductCount) {
        return tcClient.setpointRate[sensorId];
    }
    return 0;
}
