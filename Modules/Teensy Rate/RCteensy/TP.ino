
// TP.ino - ISO 11783-3 Transport Protocol for multi-packet messages
// Handles RTS/CTS/EOM/Abort for DDOP upload and large message reception
// Note: Shared definitions (TPState, TPReceiveCallback) are in TCDefs.h

//=============================================================================
// Transport Protocol Constants
//=============================================================================

// TP Connection Management PGN (EC00)
#define PGN_TP_CM                           0xEC00
// TP Data Transfer PGN (EB00)
#define PGN_TP_DT                           0xEB00

// TP.CM Control Bytes
#define TP_CM_RTS                           16    // Request To Send
#define TP_CM_CTS                           17    // Clear To Send
#define TP_CM_EOM                           19    // End Of Message ACK
#define TP_CM_ABORT                         255   // Connection Abort
#define TP_CM_BAM                           32    // Broadcast Announce Message

// Abort Reasons
#define TP_ABORT_BUSY                       1     // Already in session
#define TP_ABORT_TIMEOUT                    3     // Timeout
#define TP_ABORT_CTS_WHILE_DT               4     // CTS received during data transfer
#define TP_ABORT_RESOURCES                  2     // Resources not available

// TP Timeouts (milliseconds)
#define TP_TIMEOUT_T1                       750   // Time between CTS and first DT
#define TP_TIMEOUT_T2                       1250  // Time between DTs
#define TP_TIMEOUT_T3                       1250  // Time for response to CTS
#define TP_TIMEOUT_T4                       1050  // Time to receive more CTS

// Maximum message size
#define TP_MAX_MESSAGE_SIZE                 1785  // 255 packets * 7 bytes

//=============================================================================
// Transport Protocol Direction (local to TP.ino)
//=============================================================================

enum TPDirection {
    TP_DIR_NONE = 0,
    TP_DIR_SEND,
    TP_DIR_RECEIVE
};

struct TPSession {
    TPState state = TP_STATE_IDLE;
    TPDirection direction = TP_DIR_NONE;
    uint8_t partnerAddress = 0xFF;
    uint32_t pgn = 0;              // PGN of the message being transferred
    uint16_t totalBytes = 0;       // Total message size
    uint8_t totalPackets = 0;      // Total number of packets
    uint8_t nextPacket = 1;        // Next packet sequence number (1-based)
    uint8_t packetsRemaining = 0;  // Packets remaining in current CTS window
    uint32_t lastTime = 0;         // Timeout tracking

    // Send buffer (points to DDOP buffer, VT pool, or other source)
    const uint8_t* sendBuffer = nullptr;
    uint16_t sendOffset = 0;
    bool hasCmdByte = false;       // True if command byte needs to be prepended
    uint8_t cmdByte = 0;           // Command byte to prepend (e.g. 0x61 for DDOP, 0x11 for VT pool)

    // Receive buffer
    uint8_t receiveBuffer[TP_MAX_MESSAGE_SIZE];
    uint16_t receiveOffset = 0;
};

TPSession tpSession;

// Callback for received TP messages (typedef in TCDefs.h)
TPReceiveCallback tpReceiveCallback = nullptr;

//=============================================================================
// Forward Declarations
//=============================================================================

void TP_SendRTS(uint8_t destAddr, uint32_t pgn, uint16_t size, uint8_t numPackets);
void TP_SendCTS(uint8_t destAddr, uint8_t numPackets, uint8_t nextPacket);
void TP_SendEOM(uint8_t destAddr, uint16_t size, uint8_t numPackets, uint32_t pgn);
void TP_SendAbort(uint8_t destAddr, uint8_t reason, uint32_t pgn);
void TP_SendDataPacket(uint8_t destAddr, uint8_t seqNum, const uint8_t* data);
void TP_Reset();

//=============================================================================
// Initialization
//=============================================================================

void TP_Begin() {
    TP_Reset();
}

void TP_Reset() {
    tpSession.state = TP_STATE_IDLE;
    tpSession.direction = TP_DIR_NONE;
    tpSession.partnerAddress = 0xFF;
    tpSession.pgn = 0;
    tpSession.totalBytes = 0;
    tpSession.totalPackets = 0;
    tpSession.nextPacket = 1;
    tpSession.packetsRemaining = 0;
    tpSession.sendBuffer = nullptr;
    tpSession.sendOffset = 0;
    tpSession.hasCmdByte = false;
    tpSession.cmdByte = 0;
    tpSession.receiveOffset = 0;
}

void TP_SetReceiveCallback(TPReceiveCallback callback) {
    tpReceiveCallback = callback;
}

//=============================================================================
// Send Functions
//=============================================================================

// Command byte for DDOP transfer: DeviceDescriptor (0x01) | ObjectPoolTransfer (0x06) << 4 = 0x61
#define DDOP_TRANSFER_CMD  0x61

bool TP_SendLargeMessage(uint8_t destAddr, uint32_t pgn, uint8_t cmdByteVal, const uint8_t* data, uint16_t size) {
    // Initiate a TP session to send a large message with a command byte prepended

    Serial.print("TP_SendLargeMessage: dest=0x");
    Serial.print(destAddr, HEX);
    Serial.print(" pgn=0x");
    Serial.print(pgn, HEX);
    Serial.print(" cmd=0x");
    Serial.print(cmdByteVal, HEX);
    Serial.print(" size=");
    Serial.print(size);
    Serial.print(" state=");
    Serial.println(tpSession.state);

    if (tpSession.state != TP_STATE_IDLE) {
        Serial.println("TP: Cannot start send, session busy");
        return false;
    }

    // Total size includes command byte
    uint16_t totalSize = size + 1;

    if (totalSize > TP_MAX_MESSAGE_SIZE) {
        Serial.println("TP: Message too large");
        return false;
    }

    tpSession.direction = TP_DIR_SEND;
    tpSession.partnerAddress = destAddr;
    tpSession.pgn = pgn;
    tpSession.totalBytes = totalSize;  // Includes command byte
    tpSession.totalPackets = (totalSize + 6) / 7;  // 7 bytes per packet, round up
    tpSession.nextPacket = 1;
    tpSession.packetsRemaining = 0;
    tpSession.sendBuffer = data;
    tpSession.sendOffset = 0;
    tpSession.hasCmdByte = true;       // Flag to prepend command byte
    tpSession.cmdByte = cmdByteVal;    // Store command byte
    tpSession.lastTime = millis();

    tpSession.state = TP_STATE_SEND_RTS;

    Serial.print("TP: Initialized - totalBytes=");
    Serial.print(totalSize);
    Serial.print(" totalPackets=");
    Serial.print(tpSession.totalPackets);
    Serial.print(" nextPacket=");
    Serial.print(tpSession.nextPacket);
    Serial.print(" sendOffset=");
    Serial.println(tpSession.sendOffset);

    return true;
}

bool TP_SendDDOP(uint8_t destAddr, const uint8_t* data, uint16_t size) {
    // DDOP transfer: PGN 0xCB00, command byte 0x61
    return TP_SendLargeMessage(destAddr, 0xCB00, DDOP_TRANSFER_CMD, data, size);
}

bool TP_SendVTPool(uint8_t destAddr, const uint8_t* data, uint16_t size) {
    // VT Object Pool transfer: PGN 0xE700, command byte 0x11
    return TP_SendLargeMessage(destAddr, 0xE700, VT_POOL_TRANSFER_CMD, data, size);
}

void TP_SendRTS(uint8_t destAddr, uint32_t pgn, uint16_t size, uint8_t numPackets) {
    // Send Request To Send
    CAN_message_t msg;

    // TP.CM is destination-specific, so use PDU1 format
    msg.id = (7UL << 26) | (0xECUL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = TP_CM_RTS;
    msg.buf[1] = size & 0xFF;
    msg.buf[2] = (size >> 8) & 0xFF;
    msg.buf[3] = numPackets;
    msg.buf[4] = 0xFF;  // Max packets per CTS (255 = no limit)
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    Serial.print("TP: TX RTS to 0x");
    Serial.print(destAddr, HEX);
    Serial.print(" size=");
    Serial.print(size);
    Serial.print(" packets=");
    Serial.print(numPackets);
    Serial.print(" PGN=0x");
    Serial.print(pgn, HEX);
    Serial.print(" ID=0x");
    Serial.println(msg.id, HEX);

    ISOBUS.write(msg);
    canStats.txCount++;
}

void TP_SendCTS(uint8_t destAddr, uint8_t numPackets, uint8_t nextPacket) {
    CAN_message_t msg;

    msg.id = (7UL << 26) | (0xECUL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = TP_CM_CTS;
    msg.buf[1] = numPackets;
    msg.buf[2] = nextPacket;
    msg.buf[3] = 0xFF;
    msg.buf[4] = 0xFF;
    msg.buf[5] = tpSession.pgn & 0xFF;
    msg.buf[6] = (tpSession.pgn >> 8) & 0xFF;
    msg.buf[7] = (tpSession.pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
}

void TP_SendEOM(uint8_t destAddr, uint16_t size, uint8_t numPackets, uint32_t pgn) {
    CAN_message_t msg;

    msg.id = (7UL << 26) | (0xECUL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = TP_CM_EOM;
    msg.buf[1] = size & 0xFF;
    msg.buf[2] = (size >> 8) & 0xFF;
    msg.buf[3] = numPackets;
    msg.buf[4] = 0xFF;
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.println("TP: Sent EOM");
}

void TP_SendAbort(uint8_t destAddr, uint8_t reason, uint32_t pgn) {
    CAN_message_t msg;

    msg.id = (7UL << 26) | (0xECUL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = TP_CM_ABORT;
    msg.buf[1] = reason;
    msg.buf[2] = 0xFF;
    msg.buf[3] = 0xFF;
    msg.buf[4] = 0xFF;
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;

    Serial.print("TP: Sent Abort, reason: ");
    Serial.println(reason);
}

void TP_SendDataPacket(uint8_t destAddr, uint8_t seqNum, const uint8_t* data) {
    CAN_message_t msg;

    msg.id = (7UL << 26) | (0xEBUL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = seqNum;
    for (int i = 0; i < 7; i++) {
        msg.buf[i + 1] = data[i];
    }

    ISOBUS.write(msg);
    canStats.txCount++;
}

//=============================================================================
// Send all data packets in current CTS window immediately
//=============================================================================

void TP_SendDataPacketsNow() {
    Serial.print("TP: Sending packets ");
    Serial.print(tpSession.nextPacket);
    Serial.print("-");
    Serial.println(tpSession.nextPacket + tpSession.packetsRemaining - 1);

    while (tpSession.state == TP_STATE_SEND_DATA &&
           tpSession.packetsRemaining > 0 &&
           tpSession.nextPacket <= tpSession.totalPackets) {

        // Prepare 7 bytes of data
        uint8_t data[7];
        for (int i = 0; i < 7; i++) {
            if (tpSession.sendOffset < tpSession.totalBytes) {
                // For DDOP transfer, first byte is command byte
                if (tpSession.hasCmdByte && tpSession.sendOffset == 0) {
                    data[i] = tpSession.cmdByte;
                } else {
                    // Adjust buffer index: if hasCmdByte, subtract 1 from sendOffset
                    uint16_t bufIdx = tpSession.hasCmdByte ? (tpSession.sendOffset - 1) : tpSession.sendOffset;
                    data[i] = tpSession.sendBuffer[bufIdx];
                }
                tpSession.sendOffset++;
            } else {
                data[i] = 0xFF;  // Padding
            }
        }

        // Log each packet being sent
        Serial.print("TP: TX seq=");
        Serial.print(tpSession.nextPacket);
        Serial.print(" offset=");
        Serial.println(tpSession.sendOffset - 7);  // Show offset before this packet

        TP_SendDataPacket(tpSession.partnerAddress, tpSession.nextPacket, data);
        tpSession.nextPacket++;
        tpSession.packetsRemaining--;
        tpSession.lastTime = millis();

        // Delay to prevent SLCAN serial buffer overflow causing packet reordering
        // 10ms gives SLCAN time to transmit each packet at 115200 baud
        delay(10);
    }

    Serial.print("TP: Sent up to seq=");
    Serial.println(tpSession.nextPacket - 1);

    // Update state after sending
    if (tpSession.nextPacket > tpSession.totalPackets) {
        // All data sent, wait for EOM
        tpSession.state = TP_STATE_WAIT_EOM;
    } else if (tpSession.packetsRemaining == 0) {
        // Wait for next CTS
        tpSession.state = TP_STATE_WAIT_CTS;
    }
}

//=============================================================================
// State Machine Update
//=============================================================================

void TP_Update() {
    uint32_t now = millis();

    switch (tpSession.state) {
        case TP_STATE_IDLE:
            // Nothing to do
            break;

        case TP_STATE_SEND_RTS:
            // Send RTS and wait for CTS
            TP_SendRTS(tpSession.partnerAddress, tpSession.pgn,
                       tpSession.totalBytes, tpSession.totalPackets);
            tpSession.state = TP_STATE_WAIT_CTS;
            tpSession.lastTime = now;
            break;

        case TP_STATE_WAIT_CTS:
            // Timeout waiting for CTS
            if (now - tpSession.lastTime > TP_TIMEOUT_T3) {
                Serial.println("TP: Timeout waiting for CTS");
                TP_SendAbort(tpSession.partnerAddress, TP_ABORT_TIMEOUT, tpSession.pgn);
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_SEND_DATA:
            // Send data packets (should not normally reach here - TP_SendDataPacketsNow handles it)
            Serial.print("TP_Update: SEND_DATA - remaining=");
            Serial.print(tpSession.packetsRemaining);
            Serial.print(" next=");
            Serial.println(tpSession.nextPacket);

            if (tpSession.packetsRemaining > 0 && tpSession.nextPacket <= tpSession.totalPackets) {
                // Prepare 7 bytes of data
                uint8_t data[7];
                for (int i = 0; i < 7; i++) {
                    if (tpSession.sendOffset < tpSession.totalBytes) {
                        // For large message transfer, first byte is command byte
                        if (tpSession.hasCmdByte && tpSession.sendOffset == 0) {
                            data[i] = tpSession.cmdByte;
                        } else {
                            // Adjust buffer index: if hasCmdByte, subtract 1 from sendOffset
                            uint16_t bufIdx = tpSession.hasCmdByte ? (tpSession.sendOffset - 1) : tpSession.sendOffset;
                            data[i] = tpSession.sendBuffer[bufIdx];
                        }
                        tpSession.sendOffset++;
                    } else {
                        data[i] = 0xFF;  // Padding
                    }
                }

                Serial.print("TP_Update: TX seq=");
                Serial.println(tpSession.nextPacket);
                TP_SendDataPacket(tpSession.partnerAddress, tpSession.nextPacket, data);
                tpSession.nextPacket++;
                tpSession.packetsRemaining--;
                tpSession.lastTime = now;

                // Check if we're done sending
                if (tpSession.nextPacket > tpSession.totalPackets) {
                    // All data sent, wait for EOM
                    tpSession.state = TP_STATE_WAIT_EOM;
                } else if (tpSession.packetsRemaining == 0) {
                    // Wait for next CTS
                    tpSession.state = TP_STATE_WAIT_CTS;
                }
            }
            break;

        case TP_STATE_WAIT_EOM:
            // Timeout waiting for EOM
            if (now - tpSession.lastTime > TP_TIMEOUT_T3) {
                Serial.println("TP: Timeout waiting for EOM");
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_RECEIVE_DATA:
            // Timeout waiting for data
            if (now - tpSession.lastTime > TP_TIMEOUT_T2) {
                Serial.println("TP: Timeout receiving data");
                TP_SendAbort(tpSession.partnerAddress, TP_ABORT_TIMEOUT, tpSession.pgn);
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_COMPLETE:
            // Done, reset for next transfer
            Serial.println("TP: Transfer complete");
            TP_Reset();
            break;

        case TP_STATE_ERROR:
            // Error state, wait a bit then reset
            if (now - tpSession.lastTime > 1000) {
                TP_Reset();
            }
            break;
    }
}

//=============================================================================
// Message Handlers
//=============================================================================

void TP_HandleCM(const CAN_message_t& msg) {
    uint8_t sourceAddr = msg.id & 0xFF;
    uint8_t control = msg.buf[0];
    uint32_t pgn = msg.buf[5] | ((uint32_t)msg.buf[6] << 8) | ((uint32_t)msg.buf[7] << 16);

    switch (control) {
        case TP_CM_RTS:
            // Someone wants to send us data
            if (tpSession.state == TP_STATE_IDLE) {
                uint16_t size = msg.buf[1] | (msg.buf[2] << 8);
                uint8_t numPackets = msg.buf[3];

                Serial.print("TP: RTS received, ");
                Serial.print(size);
                Serial.print(" bytes from 0x");
                Serial.println(sourceAddr, HEX);

                if (size > TP_MAX_MESSAGE_SIZE) {
                    TP_SendAbort(sourceAddr, TP_ABORT_RESOURCES, pgn);
                    return;
                }

                tpSession.direction = TP_DIR_RECEIVE;
                tpSession.partnerAddress = sourceAddr;
                tpSession.pgn = pgn;
                tpSession.totalBytes = size;
                tpSession.totalPackets = numPackets;
                tpSession.nextPacket = 1;
                tpSession.receiveOffset = 0;
                tpSession.lastTime = millis();
                tpSession.state = TP_STATE_RECEIVE_DATA;

                // Send CTS - request all packets
                uint8_t maxPackets = (numPackets > 16) ? 16 : numPackets;
                TP_SendCTS(sourceAddr, maxPackets, 1);
                tpSession.packetsRemaining = maxPackets;
            } else {
                // Already in a session
                TP_SendAbort(sourceAddr, TP_ABORT_BUSY, pgn);
            }
            break;

        case TP_CM_CTS:
            // Clear to send more data
            Serial.print("TP: CTS from 0x");
            Serial.print(sourceAddr, HEX);
            Serial.print(" state=");
            Serial.print(tpSession.state);
            Serial.print(" dir=");
            Serial.print(tpSession.direction);
            Serial.print(" partner=0x");
            Serial.println(tpSession.partnerAddress, HEX);

            if (tpSession.state == TP_STATE_WAIT_CTS &&
                tpSession.direction == TP_DIR_SEND &&
                sourceAddr == tpSession.partnerAddress) {

                uint8_t numPackets = msg.buf[1];
                uint8_t nextPacket = msg.buf[2];

                if (numPackets == 0) {
                    // "Wait" - hold off
                    Serial.println("TP: CTS says wait");
                    tpSession.lastTime = millis();
                    return;
                }

                Serial.print("TP: CTS accepted, send ");
                Serial.print(numPackets);
                Serial.print(" packets starting at ");
                Serial.print(nextPacket);
                Serial.print(" (current next=");
                Serial.print(tpSession.nextPacket);
                Serial.print(" offset=");
                Serial.print(tpSession.sendOffset);
                Serial.println(")");

                tpSession.packetsRemaining = numPackets;
                tpSession.nextPacket = nextPacket;
                tpSession.sendOffset = (nextPacket - 1) * 7;
                tpSession.lastTime = millis();
                tpSession.state = TP_STATE_SEND_DATA;

                // Send data packets immediately (don't wait for TP_Update)
                TP_SendDataPacketsNow();
            } else {
                Serial.println("TP: CTS ignored (wrong state/dir/partner)");
            }
            break;

        case TP_CM_EOM:
            // End of message acknowledgment
            if (tpSession.state == TP_STATE_WAIT_EOM &&
                tpSession.direction == TP_DIR_SEND &&
                sourceAddr == tpSession.partnerAddress) {

                Serial.println("TP: EOM received - send complete");
                tpSession.state = TP_STATE_COMPLETE;
            }
            break;

        case TP_CM_ABORT:
            // Connection aborted
            Serial.print("TP: Abort from 0x");
            Serial.print(sourceAddr, HEX);
            Serial.print(" reason=");
            Serial.print(msg.buf[1]);
            Serial.print(" (partner=0x");
            Serial.print(tpSession.partnerAddress, HEX);
            Serial.print(" state=");
            Serial.print(tpSession.state);
            Serial.println(")");

            if (sourceAddr == tpSession.partnerAddress) {
                // Decode abort reason
                switch (msg.buf[1]) {
                    case 1: Serial.println("  -> Already in session"); break;
                    case 2: Serial.println("  -> Resources not available"); break;
                    case 3: Serial.println("  -> Timeout"); break;
                    case 4: Serial.println("  -> CTS while DT"); break;
                    case 5: Serial.println("  -> Max retransmit"); break;
                    case 6: Serial.println("  -> Unexpected DT"); break;
                    case 7: Serial.println("  -> Bad sequence number"); break;
                    case 8: Serial.println("  -> Duplicate sequence number"); break;
                    case 9: Serial.println("  -> Unexpected ECTS"); break;
                    case 10: Serial.println("  -> Message too big"); break;
                    default: Serial.println("  -> Unknown reason"); break;
                }
                tpSession.state = TP_STATE_ERROR;
                tpSession.lastTime = millis();
            }
            break;

        case TP_CM_BAM:
            // Broadcast Announce - not typically used with DDOP but handle anyway
            Serial.println("TP: BAM received (ignoring)");
            break;
    }
}

void TP_HandleDT(const CAN_message_t& msg) {
    // Data Transfer packet
    if (tpSession.state != TP_STATE_RECEIVE_DATA ||
        tpSession.direction != TP_DIR_RECEIVE) {
        return;
    }

    uint8_t sourceAddr = msg.id & 0xFF;
    if (sourceAddr != tpSession.partnerAddress) {
        return;
    }

    uint8_t seqNum = msg.buf[0];

    if (seqNum != tpSession.nextPacket) {
        Serial.print("TP: Unexpected packet ");
        Serial.print(seqNum);
        Serial.print(", expected ");
        Serial.println(tpSession.nextPacket);
        // Could send abort here, but try to continue
    }

    // Copy data bytes
    for (int i = 0; i < 7 && tpSession.receiveOffset < tpSession.totalBytes; i++) {
        tpSession.receiveBuffer[tpSession.receiveOffset++] = msg.buf[i + 1];
    }

    tpSession.nextPacket++;
    tpSession.packetsRemaining--;
    tpSession.lastTime = millis();

    // Check if we received all packets
    if (tpSession.receiveOffset >= tpSession.totalBytes) {
        // All data received, send EOM
        TP_SendEOM(tpSession.partnerAddress, tpSession.totalBytes,
                   tpSession.totalPackets, tpSession.pgn);

        Serial.print("TP: Receive complete, ");
        Serial.print(tpSession.receiveOffset);
        Serial.println(" bytes");

        // Call callback with received data
        if (tpReceiveCallback) {
            tpReceiveCallback(tpSession.pgn, tpSession.receiveBuffer, tpSession.receiveOffset);
        }

        tpSession.state = TP_STATE_COMPLETE;
    } else if (tpSession.packetsRemaining == 0 && tpSession.nextPacket <= tpSession.totalPackets) {
        // Need to request more packets
        uint8_t remaining = tpSession.totalPackets - tpSession.nextPacket + 1;
        uint8_t toRequest = (remaining > 16) ? 16 : remaining;
        TP_SendCTS(tpSession.partnerAddress, toRequest, tpSession.nextPacket);
        tpSession.packetsRemaining = toRequest;
    }
}

//=============================================================================
// Status Functions
//=============================================================================

bool TP_IsBusy() {
    return tpSession.state != TP_STATE_IDLE;
}

bool TP_IsComplete() {
    return tpSession.state == TP_STATE_COMPLETE;
}

bool TP_IsError() {
    return tpSession.state == TP_STATE_ERROR;
}

TPState TP_GetState() {
    return tpSession.state;
}
