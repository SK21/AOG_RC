
// TP.ino - ISO 11783-3 Transport Protocol (TP) and Extended Transport Protocol (ETP)
// TP handles messages 9-1785 bytes (up to 255 packets)
// ETP handles messages 1786-117,440,505 bytes (up to 16,777,215 packets)
// Note: Shared definitions (TPState, TPReceiveCallback) are in TCDefs.h

//=============================================================================
// Transport Protocol Constants
//=============================================================================

// TP Connection Management PGN (EC00)
#define PGN_TP_CM                           0xEC00
// TP Data Transfer PGN (EB00)
#define PGN_TP_DT                           0xEB00

// ETP Connection Management PGN (C700)
#define PGN_ETP_CM                          0xC700
// ETP Data Transfer PGN (C600)
#define PGN_ETP_DT                          0xC600

// TP.CM Control Bytes
#define TP_CM_RTS                           16    // Request To Send
#define TP_CM_CTS                           17    // Clear To Send
#define TP_CM_EOM                           19    // End Of Message ACK
#define TP_CM_ABORT                         255   // Connection Abort
#define TP_CM_BAM                           32    // Broadcast Announce Message

// ETP.CM Control Bytes
#define ETP_CM_RTS                          20    // Extended Request To Send
#define ETP_CM_CTS                          21    // Extended Clear To Send
#define ETP_CM_DPO                          22    // Data Packet Offset
#define ETP_CM_EOM                          23    // Extended End Of Message ACK
#define ETP_CM_ABORT                        255   // Connection Abort (same as TP)

// Abort Reasons
#define TP_ABORT_BUSY                       1     // Already in session
#define TP_ABORT_RESOURCES                  2     // Resources not available
#define TP_ABORT_TIMEOUT                    3     // Timeout
#define TP_ABORT_CTS_WHILE_DT              4     // CTS received during data transfer

// TP Timeouts (milliseconds)
#define TP_TIMEOUT_T1                       750   // Time between CTS and first DT
#define TP_TIMEOUT_T2                       1250  // Time between DTs
#define TP_TIMEOUT_T3                       1250  // Time for response to CTS
#define TP_TIMEOUT_T4                       1050  // Time to receive more CTS

// Maximum message sizes
#define TP_MAX_BYTES                        1785  // 255 packets * 7 bytes (standard TP limit)
#define TP_MAX_RECEIVE_SIZE                 1785  // Receive buffer for incoming TP messages

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
    uint32_t totalBytes = 0;       // Total message size (uint32 for ETP)
    uint16_t totalPackets = 0;     // Total number of packets (uint16 for ETP)
    uint16_t nextPacket = 1;       // Next packet sequence number (1-based, uint16 for ETP)
    uint8_t packetsRemaining = 0;  // Packets remaining in current CTS window (max 255)
    uint32_t lastTime = 0;         // Timeout tracking
    bool useETP = false;           // True if using Extended Transport Protocol

    // ETP-specific: Data Packet Offset
    uint32_t etpDPO = 0;           // Current data packet offset for ETP

    // Send buffer
    const uint8_t* sendBuffer = nullptr;
    uint32_t sendOffset = 0;
    bool hasCmdByte = false;       // True if command byte needs to be prepended
    uint8_t cmdByte = 0;           // Command byte to prepend (e.g. 0x61 for DDOP)

    // Receive buffer (only for standard TP)
    uint8_t receiveBuffer[TP_MAX_RECEIVE_SIZE];
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
void ETP_SendRTS(uint8_t destAddr, uint32_t pgn, uint32_t size);
void ETP_SendDPO(uint8_t destAddr, uint8_t numPackets, uint32_t dataOffset);
void ETP_SendEOM(uint8_t destAddr, uint32_t size, uint32_t pgn);
void ETP_SendAbort(uint8_t destAddr, uint8_t reason, uint32_t pgn);
void ETP_SendDataPacket(uint8_t destAddr, uint8_t seqNum, const uint8_t* data);
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
    tpSession.useETP = false;
    tpSession.etpDPO = 0;
}

void TP_SetReceiveCallback(TPReceiveCallback callback) {
    tpReceiveCallback = callback;
}

//=============================================================================
// Send Initiation Functions
//=============================================================================

// Command byte for DDOP transfer: DeviceDescriptor (0x01) | ObjectPoolTransfer (0x06) << 4 = 0x61
#define DDOP_TRANSFER_CMD  0x61

bool TP_SendLargeMessage(uint8_t destAddr, uint32_t pgn, uint8_t cmdByteVal, const uint8_t* data, uint16_t size) {
    // Initiate a TP or ETP session to send a large message with a command byte prepended

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
    uint32_t totalSize = (uint32_t)size + 1;
    bool needETP = (totalSize > TP_MAX_BYTES);

    tpSession.direction = TP_DIR_SEND;
    tpSession.partnerAddress = destAddr;
    tpSession.pgn = pgn;
    tpSession.totalBytes = totalSize;
    tpSession.totalPackets = (totalSize + 6) / 7;  // 7 bytes per packet, round up
    tpSession.nextPacket = 1;
    tpSession.packetsRemaining = 0;
    tpSession.sendBuffer = data;
    tpSession.sendOffset = 0;
    tpSession.hasCmdByte = true;
    tpSession.cmdByte = cmdByteVal;
    tpSession.lastTime = millis();
    tpSession.useETP = needETP;
    tpSession.etpDPO = 0;

    tpSession.state = TP_STATE_SEND_RTS;

    Serial.print(needETP ? "ETP" : "TP");
    Serial.print(": Initialized - totalBytes=");
    Serial.print(totalSize);
    Serial.print(" totalPackets=");
    Serial.print(tpSession.totalPackets);
    Serial.println();

    return true;
}

bool TP_SendDDOP(uint8_t destAddr, const uint8_t* data, uint16_t size) {
    // DDOP transfer: PGN 0xCB00, command byte 0x61
    return TP_SendLargeMessage(destAddr, 0xCB00, DDOP_TRANSFER_CMD, data, size);
}


//=============================================================================
// Standard TP Send Functions
//=============================================================================

void TP_SendRTS(uint8_t destAddr, uint32_t pgn, uint16_t size, uint8_t numPackets) {
    CAN_message_t msg;
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

    Serial.print("TP: TX RTS size=");
    Serial.print(size);
    Serial.print(" packets=");
    Serial.println(numPackets);

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
// Extended TP Send Functions
//=============================================================================

void ETP_SendRTS(uint8_t destAddr, uint32_t pgn, uint32_t size) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC7UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = ETP_CM_RTS;
    msg.buf[1] = size & 0xFF;
    msg.buf[2] = (size >> 8) & 0xFF;
    msg.buf[3] = (size >> 16) & 0xFF;
    msg.buf[4] = (size >> 24) & 0xFF;
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    Serial.print("ETP: TX RTS size=");
    Serial.println(size);

    ISOBUS.write(msg);
    canStats.txCount++;
}

void ETP_SendCTS(uint8_t destAddr, uint8_t numPackets, uint32_t nextPacket) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC7UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = ETP_CM_CTS;
    msg.buf[1] = numPackets;
    msg.buf[2] = nextPacket & 0xFF;
    msg.buf[3] = (nextPacket >> 8) & 0xFF;
    msg.buf[4] = (nextPacket >> 16) & 0xFF;
    msg.buf[5] = tpSession.pgn & 0xFF;
    msg.buf[6] = (tpSession.pgn >> 8) & 0xFF;
    msg.buf[7] = (tpSession.pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
}

void ETP_SendDPO(uint8_t destAddr, uint8_t numPackets, uint32_t dataOffset) {
    // Data Packet Offset: tells receiver the byte offset for the next burst of packets
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC7UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = ETP_CM_DPO;
    msg.buf[1] = numPackets;
    msg.buf[2] = dataOffset & 0xFF;
    msg.buf[3] = (dataOffset >> 8) & 0xFF;
    msg.buf[4] = (dataOffset >> 16) & 0xFF;
    msg.buf[5] = tpSession.pgn & 0xFF;
    msg.buf[6] = (tpSession.pgn >> 8) & 0xFF;
    msg.buf[7] = (tpSession.pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
}

void ETP_SendEOM(uint8_t destAddr, uint32_t size, uint32_t pgn) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC7UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = ETP_CM_EOM;
    msg.buf[1] = size & 0xFF;
    msg.buf[2] = (size >> 8) & 0xFF;
    msg.buf[3] = (size >> 16) & 0xFF;
    msg.buf[4] = (size >> 24) & 0xFF;
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
    Serial.println("ETP: Sent EOM");
}

void ETP_SendAbort(uint8_t destAddr, uint8_t reason, uint32_t pgn) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC7UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    msg.buf[0] = ETP_CM_ABORT;
    msg.buf[1] = reason;
    msg.buf[2] = 0xFF;
    msg.buf[3] = 0xFF;
    msg.buf[4] = 0xFF;
    msg.buf[5] = pgn & 0xFF;
    msg.buf[6] = (pgn >> 8) & 0xFF;
    msg.buf[7] = (pgn >> 16) & 0xFF;

    ISOBUS.write(msg);
    canStats.txCount++;
    Serial.print("ETP: Sent Abort, reason: ");
    Serial.println(reason);
}

void ETP_SendDataPacket(uint8_t destAddr, uint8_t seqNum, const uint8_t* data) {
    CAN_message_t msg;
    msg.id = (7UL << 26) | (0xC6UL << 16) | ((uint32_t)destAddr << 8) | ISOBUSid.address;
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
// Prepare 7 bytes of send data at current sendOffset
//=============================================================================

void TP_PrepareDataBytes(uint8_t* data) {
    for (int i = 0; i < 7; i++) {
        if (tpSession.sendOffset < tpSession.totalBytes) {
            if (tpSession.hasCmdByte && tpSession.sendOffset == 0) {
                data[i] = tpSession.cmdByte;
            } else {
                uint32_t bufIdx = tpSession.hasCmdByte ? (tpSession.sendOffset - 1) : tpSession.sendOffset;
                data[i] = tpSession.sendBuffer[bufIdx];
            }
            tpSession.sendOffset++;
        } else {
            data[i] = 0xFF;  // Padding
        }
    }
}

//=============================================================================
// Send all data packets in current CTS window immediately
//=============================================================================

void TP_SendDataPacketsNow() {
    Serial.print(tpSession.useETP ? "ETP" : "TP");
    Serial.print(": Sending packets ");
    Serial.print(tpSession.nextPacket);
    Serial.print("-");
    Serial.println(tpSession.nextPacket + tpSession.packetsRemaining - 1);

    // For ETP, send DPO before the data burst
    if (tpSession.useETP) {
        ETP_SendDPO(tpSession.partnerAddress, tpSession.packetsRemaining, tpSession.etpDPO);
        delay(10);
    }

    uint8_t seqInBurst = 0;  // ETP uses 0-based sequence within each DPO burst

    while (tpSession.state == TP_STATE_SEND_DATA &&
           tpSession.packetsRemaining > 0 &&
           tpSession.nextPacket <= tpSession.totalPackets) {

        uint8_t data[7];
        TP_PrepareDataBytes(data);

        if (tpSession.useETP) {
            ETP_SendDataPacket(tpSession.partnerAddress, seqInBurst, data);
            seqInBurst++;
        } else {
            TP_SendDataPacket(tpSession.partnerAddress, tpSession.nextPacket, data);
        }

        tpSession.nextPacket++;
        tpSession.packetsRemaining--;
        tpSession.lastTime = millis();

        // Delay to prevent SLCAN serial buffer overflow causing packet reordering
        delay(10);
    }

    Serial.print(tpSession.useETP ? "ETP" : "TP");
    Serial.print(": Sent up to packet ");
    Serial.println(tpSession.nextPacket - 1);

    // Update state after sending
    if (tpSession.nextPacket > tpSession.totalPackets) {
        tpSession.state = TP_STATE_WAIT_EOM;
    } else if (tpSession.packetsRemaining == 0) {
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
            break;

        case TP_STATE_SEND_RTS:
            if (tpSession.useETP) {
                ETP_SendRTS(tpSession.partnerAddress, tpSession.pgn, tpSession.totalBytes);
            } else {
                TP_SendRTS(tpSession.partnerAddress, tpSession.pgn,
                           tpSession.totalBytes, tpSession.totalPackets);
            }
            tpSession.state = TP_STATE_WAIT_CTS;
            tpSession.lastTime = now;
            break;

        case TP_STATE_WAIT_CTS:
            if (now - tpSession.lastTime > TP_TIMEOUT_T3) {
                Serial.println(tpSession.useETP ? "ETP: Timeout waiting for CTS" : "TP: Timeout waiting for CTS");
                if (tpSession.useETP) {
                    ETP_SendAbort(tpSession.partnerAddress, TP_ABORT_TIMEOUT, tpSession.pgn);
                } else {
                    TP_SendAbort(tpSession.partnerAddress, TP_ABORT_TIMEOUT, tpSession.pgn);
                }
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_SEND_DATA:
            // Fallback: send one packet per update cycle if TP_SendDataPacketsNow wasn't called
            if (tpSession.packetsRemaining > 0 && tpSession.nextPacket <= tpSession.totalPackets) {
                uint8_t data[7];
                TP_PrepareDataBytes(data);

                if (tpSession.useETP) {
                    ETP_SendDataPacket(tpSession.partnerAddress, 0, data);
                } else {
                    TP_SendDataPacket(tpSession.partnerAddress, tpSession.nextPacket, data);
                }
                tpSession.nextPacket++;
                tpSession.packetsRemaining--;
                tpSession.lastTime = now;

                if (tpSession.nextPacket > tpSession.totalPackets) {
                    tpSession.state = TP_STATE_WAIT_EOM;
                } else if (tpSession.packetsRemaining == 0) {
                    tpSession.state = TP_STATE_WAIT_CTS;
                }
            }
            break;

        case TP_STATE_WAIT_EOM:
            if (now - tpSession.lastTime > TP_TIMEOUT_T3) {
                Serial.println(tpSession.useETP ? "ETP: Timeout waiting for EOM" : "TP: Timeout waiting for EOM");
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_RECEIVE_DATA:
            if (now - tpSession.lastTime > TP_TIMEOUT_T2) {
                Serial.println("TP: Timeout receiving data");
                TP_SendAbort(tpSession.partnerAddress, TP_ABORT_TIMEOUT, tpSession.pgn);
                tpSession.state = TP_STATE_ERROR;
            }
            break;

        case TP_STATE_COMPLETE:
            Serial.println(tpSession.useETP ? "ETP: Transfer complete" : "TP: Transfer complete");
            TP_Reset();
            break;

        case TP_STATE_ERROR:
            if (now - tpSession.lastTime > 1000) {
                TP_Reset();
            }
            break;
    }
}

//=============================================================================
// Standard TP Message Handlers
//=============================================================================

void TP_HandleCM(const CAN_message_t& msg) {
    uint8_t sourceAddr = msg.id & 0xFF;
    uint8_t control = msg.buf[0];
    uint32_t pgn = msg.buf[5] | ((uint32_t)msg.buf[6] << 8) | ((uint32_t)msg.buf[7] << 16);

    switch (control) {
        case TP_CM_RTS:
            if (tpSession.state == TP_STATE_IDLE) {
                uint16_t size = msg.buf[1] | (msg.buf[2] << 8);
                uint8_t numPackets = msg.buf[3];

                Serial.print("TP: RTS received, ");
                Serial.print(size);
                Serial.print(" bytes from 0x");
                Serial.println(sourceAddr, HEX);

                if (size > TP_MAX_RECEIVE_SIZE) {
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
                tpSession.useETP = false;
                tpSession.state = TP_STATE_RECEIVE_DATA;

                uint8_t maxPackets = (numPackets > 16) ? 16 : numPackets;
                TP_SendCTS(sourceAddr, maxPackets, 1);
                tpSession.packetsRemaining = maxPackets;
            } else {
                TP_SendAbort(sourceAddr, TP_ABORT_BUSY, pgn);
            }
            break;

        case TP_CM_CTS:
            if (tpSession.state == TP_STATE_WAIT_CTS &&
                tpSession.direction == TP_DIR_SEND &&
                !tpSession.useETP &&
                sourceAddr == tpSession.partnerAddress) {

                uint8_t numPackets = msg.buf[1];
                uint8_t nextPacket = msg.buf[2];

                if (numPackets == 0) {
                    Serial.println("TP: CTS says wait");
                    tpSession.lastTime = millis();
                    return;
                }

                Serial.print("TP: CTS send ");
                Serial.print(numPackets);
                Serial.print(" from ");
                Serial.println(nextPacket);

                tpSession.packetsRemaining = numPackets;
                tpSession.nextPacket = nextPacket;
                tpSession.sendOffset = (nextPacket - 1) * 7;
                tpSession.lastTime = millis();
                tpSession.state = TP_STATE_SEND_DATA;

                TP_SendDataPacketsNow();
            }
            break;

        case TP_CM_EOM:
            if (tpSession.state == TP_STATE_WAIT_EOM &&
                tpSession.direction == TP_DIR_SEND &&
                !tpSession.useETP &&
                sourceAddr == tpSession.partnerAddress) {

                Serial.println("TP: EOM received - send complete");
                tpSession.state = TP_STATE_COMPLETE;
            }
            break;

        case TP_CM_ABORT:
            Serial.print("TP: Abort from 0x");
            Serial.print(sourceAddr, HEX);
            Serial.print(" reason=");
            Serial.println(msg.buf[1]);

            if (sourceAddr == tpSession.partnerAddress) {
                tpSession.state = TP_STATE_ERROR;
                tpSession.lastTime = millis();
            }
            break;

        case TP_CM_BAM:
            break;
    }
}

void TP_HandleDT(const CAN_message_t& msg) {
    if (tpSession.state != TP_STATE_RECEIVE_DATA ||
        tpSession.direction != TP_DIR_RECEIVE ||
        tpSession.useETP) {
        return;
    }

    uint8_t sourceAddr = msg.id & 0xFF;
    if (sourceAddr != tpSession.partnerAddress) {
        return;
    }

    // Copy data bytes
    for (int i = 0; i < 7 && tpSession.receiveOffset < tpSession.totalBytes; i++) {
        tpSession.receiveBuffer[tpSession.receiveOffset++] = msg.buf[i + 1];
    }

    tpSession.nextPacket++;
    tpSession.packetsRemaining--;
    tpSession.lastTime = millis();

    if (tpSession.receiveOffset >= tpSession.totalBytes) {
        TP_SendEOM(tpSession.partnerAddress, tpSession.totalBytes,
                   tpSession.totalPackets, tpSession.pgn);

        Serial.print("TP: Receive complete, ");
        Serial.print(tpSession.receiveOffset);
        Serial.println(" bytes");

        if (tpReceiveCallback) {
            tpReceiveCallback(tpSession.pgn, tpSession.receiveBuffer, tpSession.receiveOffset);
        }
        tpSession.state = TP_STATE_COMPLETE;
    } else if (tpSession.packetsRemaining == 0 && tpSession.nextPacket <= tpSession.totalPackets) {
        uint8_t remaining = tpSession.totalPackets - tpSession.nextPacket + 1;
        uint8_t toRequest = (remaining > 16) ? 16 : remaining;
        TP_SendCTS(tpSession.partnerAddress, toRequest, tpSession.nextPacket);
        tpSession.packetsRemaining = toRequest;
    }
}

//=============================================================================
// Extended TP Message Handlers
//=============================================================================

void TP_HandleETPCM(const CAN_message_t& msg) {
    uint8_t sourceAddr = msg.id & 0xFF;
    uint8_t control = msg.buf[0];

    switch (control) {
        case ETP_CM_CTS:
            if (tpSession.state == TP_STATE_WAIT_CTS &&
                tpSession.direction == TP_DIR_SEND &&
                tpSession.useETP &&
                sourceAddr == tpSession.partnerAddress) {

                uint8_t numPackets = msg.buf[1];
                uint32_t nextPacket = msg.buf[2] | ((uint32_t)msg.buf[3] << 8) | ((uint32_t)msg.buf[4] << 16);

                if (numPackets == 0) {
                    Serial.println("ETP: CTS says wait");
                    tpSession.lastTime = millis();
                    return;
                }

                Serial.print("ETP: CTS send ");
                Serial.print(numPackets);
                Serial.print(" from packet ");
                Serial.println(nextPacket);

                tpSession.packetsRemaining = numPackets;
                tpSession.nextPacket = nextPacket;
                tpSession.sendOffset = (nextPacket - 1) * 7;
                tpSession.etpDPO = (nextPacket - 1) * 7;  // Byte offset for DPO
                tpSession.lastTime = millis();
                tpSession.state = TP_STATE_SEND_DATA;

                TP_SendDataPacketsNow();
            }
            break;

        case ETP_CM_EOM:
            if (tpSession.state == TP_STATE_WAIT_EOM &&
                tpSession.direction == TP_DIR_SEND &&
                tpSession.useETP &&
                sourceAddr == tpSession.partnerAddress) {

                Serial.println("ETP: EOM received - send complete");
                tpSession.state = TP_STATE_COMPLETE;
            }
            break;

        case ETP_CM_ABORT:
            Serial.print("ETP: Abort from 0x");
            Serial.print(sourceAddr, HEX);
            Serial.print(" reason=");
            Serial.println(msg.buf[1]);

            if (sourceAddr == tpSession.partnerAddress && tpSession.useETP) {
                tpSession.state = TP_STATE_ERROR;
                tpSession.lastTime = millis();
            }
            break;
    }
}

void TP_HandleETPDT(const CAN_message_t& msg) {
    // ETP receive not implemented (we only send large pools, not receive them)
    // If needed in the future, add ETP receive buffer and state handling here
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
