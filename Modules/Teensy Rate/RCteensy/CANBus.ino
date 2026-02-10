
// CANBus.ino - ISOBUS CAN communication for Teensy 4.1
// Uses FlexCAN_T4 library (built-in for Teensy 4.x)
// CAN1 pins: TX=22, RX=23 (connected to MCP2562-E/P transceiver)

// CAN bus instance - using CAN1 at 250kbps (ISOBUS standard)
FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_16> ISOBUS;
uint8_t StandbyPin=6;

// ISOBUS address claiming
struct IsobusIdentity {
    uint8_t address = 0x80;          // Preferred address (128)
    bool addressClaimed = false;
    uint32_t lastClaimTime = 0;

    // ISO 11783 NAME fields (64-bit)
    uint32_t identityNumber = 1;      // Unique serial number (21 bits)
    uint16_t manufacturerCode = 0;    // Assigned by AEF (11 bits)
    uint8_t deviceClass = 25;         // 25 = Sprayer/Spreader control
    uint8_t deviceClassInstance = 0;
    uint8_t functionCode = 130;       // 130 = Rate Control
    uint8_t functionInstance = 0;
    uint8_t industryGroup = 2;        // 2 = Agricultural
    bool selfConfigurable = true;
};

IsobusIdentity ISOBUSid;

// CAN message statistics (struct defined in TCDefs.h)
CANStats canStats = {0, 0, 0, 0};

//-----------------------------------------------------------------------------
// Build 64-bit NAME from identity fields
//-----------------------------------------------------------------------------
uint64_t buildIsobusNAME() {
    uint64_t name = 0;

    // Bits 0-20: Identity number (21 bits)
    name |= (uint64_t)(ISOBUSid.identityNumber & 0x1FFFFF);

    // Bits 21-31: Manufacturer code (11 bits)
    name |= (uint64_t)(ISOBUSid.manufacturerCode & 0x7FF) << 21;

    // Bits 32-34: ECU instance (3 bits) - use 0
    // Bits 35-39: Function instance (5 bits)
    name |= (uint64_t)(ISOBUSid.functionInstance & 0x1F) << 35;

    // Bits 40-47: Function code (8 bits)
    name |= (uint64_t)(ISOBUSid.functionCode) << 40;

    // Bit 48: Reserved

    // Bits 49-55: Device class (7 bits)
    name |= (uint64_t)(ISOBUSid.deviceClass & 0x7F) << 49;

    // Bits 56-59: Device class instance (4 bits)
    name |= (uint64_t)(ISOBUSid.deviceClassInstance & 0x0F) << 56;

    // Bits 60-62: Industry group (3 bits)
    name |= (uint64_t)(ISOBUSid.industryGroup & 0x07) << 60;

    // Bit 63: Self-configurable address
    if (ISOBUSid.selfConfigurable) {
        name |= (uint64_t)1 << 63;
    }

    return name;
}

//-----------------------------------------------------------------------------
// Initialize CAN hardware
//-----------------------------------------------------------------------------
bool CANBus_Begin() {
    // Enable CAN transceiver (STBY pin LOW = active)
    pinMode(StandbyPin, OUTPUT);
    digitalWrite(StandbyPin, LOW);

    // Initialize FlexCAN at 250kbps (ISOBUS standard)
    ISOBUS.begin();
    ISOBUS.setBaudRate(250000);

    // Set up mailboxes for reception
    ISOBUS.setMaxMB(16);
    ISOBUS.setMBFilter(ACCEPT_ALL);

    // Set identity number from module ID
    ISOBUSid.identityNumber = MDL.ID + 1000;

    return true;
}

//-----------------------------------------------------------------------------
// Send address claim message (PGN 60928 / 0xEE00)
//-----------------------------------------------------------------------------
void CANBus_SendAddressClaim() {
    CAN_message_t msg;

    // Build 29-bit CAN ID for Address Claimed
    // Priority=6, PGN=0xEE00, SA=claimed address
    msg.id = (6UL << 26) | (0xEE00UL << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = 8;

    // Pack 64-bit NAME into 8 bytes (little-endian)
    uint64_t name = buildIsobusNAME();
    for (int i = 0; i < 8; i++) {
        msg.buf[i] = (name >> (i * 8)) & 0xFF;
    }

    if (ISOBUS.write(msg)) {
        ISOBUSid.lastClaimTime = millis();
        ISOBUSid.addressClaimed = true;
        canStats.txCount++;
        Serial.print("Address claim sent: 0x");
        Serial.println(ISOBUSid.address, HEX);
    } else {
        canStats.errorCount++;
        Serial.println("Address claim FAILED");
    }
}

//-----------------------------------------------------------------------------
// Maintain address claim (for TC Client mode - no proprietary data)
//-----------------------------------------------------------------------------
void CANBus_MaintainAddress() {
    static uint32_t lastClaimCheck = 0;

    // Address claiming - send initially and check every 5 seconds
    if (!ISOBUSid.addressClaimed || (millis() - lastClaimCheck > 5000)) {
        lastClaimCheck = millis();
        if (!ISOBUSid.addressClaimed) {
            CANBus_SendAddressClaim();
        }
    }
}

//-----------------------------------------------------------------------------
// Send proprietary message (PGN 0xFF00-0xFFFF range)
//-----------------------------------------------------------------------------
bool CANBus_SendProprietaryB(uint8_t pgnLow, const uint8_t* data, uint8_t len) {
    if (!ISOBUSid.addressClaimed) return false;

    CAN_message_t msg;

    // Build 29-bit CAN ID for Proprietary B
    // Priority=6, DP=0, PF=0xFF, PS=pgnLow, SA=our address
    msg.id = (6UL << 26) | (0xFFUL << 16) | ((uint32_t)pgnLow << 8) | ISOBUSid.address;
    msg.flags.extended = 1;
    msg.len = (len > 8) ? 8 : len;

    memcpy(msg.buf, data, msg.len);

    if (ISOBUS.write(msg)) {
        canStats.txCount++;
        return true;
    }

    canStats.errorCount++;
    return false;
}

//-----------------------------------------------------------------------------
// Process received CAN messages
//-----------------------------------------------------------------------------
void CANBus_Receive() {
    CAN_message_t msg;

    while (ISOBUS.read(msg)) {
        if (!msg.flags.extended) continue;  // Only process extended frames

        canStats.rxCount++;
        canStats.lastRxTime = millis();

        // Extract PGN from 29-bit ID
        // ID format: Priority(3) | R(1) | DP(1) | PF(8) | PS(8) | SA(8)
        uint8_t pf = (msg.id >> 16) & 0xFF;  // PDU Format
        uint8_t ps = (msg.id >> 8) & 0xFF;   // PDU Specific
        uint8_t sa = msg.id & 0xFF;          // Source Address

        uint32_t pgn;
        if (pf >= 240) {
            // PDU2 format: PGN = PF * 256 + PS
            pgn = ((uint32_t)pf << 8) | ps;
        } else {
            // PDU1 format: PGN = PF * 256, PS is destination
            pgn = (uint32_t)pf << 8;
        }

        // Debug: show received PGNs
        Serial.print("RX PGN 0x");
        Serial.print(pgn, HEX);
        Serial.print(" from 0x");
        Serial.print(sa, HEX);
        Serial.print(" to 0x");
        Serial.println(ps, HEX);

        // Handle specific PGNs
        switch (pgn) {
            case 0xEE00:  // Address Claimed
                CANBus_HandleAddressClaim(msg, sa);
                break;

            case 0xFF03:  // Rate Command (from Gateway)
                CANBus_HandleRateCommand(msg);
                break;

            case 0xFF04:  // Relay Command (from Gateway)
                CANBus_HandleRelayCommand(msg);
                break;

            case 0xFF05:  // PID Settings 1 (from Gateway)
                CANBus_HandlePidSettings1(msg);
                break;

            case 0xFF06:  // PID Settings 2 (from Gateway)
                CANBus_HandlePidSettings2(msg);
                break;

            case 0xFF07:  // Wheel Speed Config (from Gateway)
                CANBus_HandleWheelConfig(msg);
                break;

            case 0xFF0A:  // Flow Calibration (from Gateway)
                CANBus_HandleFlowCal(msg);
                break;

            case 0xFF0B:  // PID Settings 3 (from Gateway)
                CANBus_HandlePidSettings3(msg);
                break;

            case 0xFEF1:  // Machine Selected Speed
                CANBus_HandleSpeed(msg, 0xFEF1);
                break;

            case 0xFE48:  // Wheel Based Speed
                CANBus_HandleSpeed(msg, 0xFE48);
                break;

            case 0xFE49:  // Ground Based Speed
                CANBus_HandleSpeed(msg, 0xFE49);
                break;

            case 0xFE6E:  // VT Status (PGN 65134) - broadcast
                if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                    VTClient_HandleVTStatus(msg);
                }
                break;

            case 0xFEF8:  // Task Controller Status (PGN 65272)
                // Route to TC Client if in TC mode
                if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                    TCClient_HandleTCStatus(msg);
                }
                break;

            case 0xE800:  // Acknowledgment (ACKM) PGN 59392
                {
                    uint8_t ctrl = msg.buf[0];
                    uint32_t ackedPgn = msg.buf[5] | (msg.buf[6] << 8) | ((uint32_t)msg.buf[7] << 16);
                    Serial.print("  ACKM: ");
                    if (ctrl == 0) Serial.print("ACK");
                    else if (ctrl == 1) Serial.print("NACK");
                    else if (ctrl == 2) Serial.print("ACCESS_DENIED");
                    else if (ctrl == 3) Serial.print("CANNOT_RESPOND");
                    else Serial.print(ctrl);
                    Serial.print(" for PGN 0x");
                    Serial.println(ackedPgn, HEX);

                    // If TC Server NACKs our process data command, fast-fail to retry
                    if (ctrl == 1 && ackedPgn == 0xCB00 && (MDL.CommMode == 3 || MDL.CommMode == 4)) {
                        uint8_t tcState = TCClient_GetState();
                        if (tcState >= 4 && tcState <= 9) {  // Any waiting state after structure label
                            Serial.println("TC Server NACK - not registered, retrying");
                            TCClient_SetState(TC_ERROR);
                        }
                    }
                }
                break;

            case 0xEC00:  // Transport Protocol - Connection Management
                // Route to TP handler
                if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                    TP_HandleCM(msg);
                }
                break;

            case 0xEB00:  // Transport Protocol - Data Transfer
                // Route to TP handler
                if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                    TP_HandleDT(msg);
                }
                break;

            default:
                // Check for Process Data PGNs (0xCB00 range) - destination specific
                if (pf == 0xCB) {
                    // Check if this is addressed to us or broadcast
                    // PS is destination for PDU1 (pf < 240)
                    if (ps == ISOBUSid.address || ps == 0xFF || ps == 0x00) {
                        if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                            Serial.print("  -> ProcessData cmd=0x");
                            Serial.println(msg.buf[0], HEX);
                            TCClient_HandleProcessData(msg, pf, ps);
                        }
                    }
                }
                // Check for VT to ECU (PGN 0xE600, PDU1, pf=0xE6)
                if (pf == 0xE6) {
                    if (MDL.CommMode == 3 || MDL.CommMode == 4) {
                        if (ps == ISOBUSid.address) {
                            VTClient_HandleVTtoECU(msg);
                        } else if (ps == 0xFF && msg.buf[0] == VT_FUNC_VT_STATUS) {
                            // VT Status broadcast (AgIsoStack++ sends on 0xE600, not 0xFE6E)
                            VTClient_HandleVTStatus(msg);
                        }
                    }
                }
                break;
        }
    }
}

//-----------------------------------------------------------------------------
// Handle address claim from another device
//-----------------------------------------------------------------------------
void CANBus_HandleAddressClaim(const CAN_message_t& msg, uint8_t sourceAddr) {
    if (sourceAddr == ISOBUSid.address) {
        // Address conflict! Compare NAMEs
        uint64_t theirName = 0;
        for (int i = 0; i < 8; i++) {
            theirName |= (uint64_t)msg.buf[i] << (i * 8);
        }

        uint64_t ourName = buildIsobusNAME();

        if (theirName < ourName) {
            // They win, we must find new address
            ISOBUSid.address++;
            if (ISOBUSid.address > 247) ISOBUSid.address = 128;
            ISOBUSid.addressClaimed = false;
            CANBus_SendAddressClaim();
        } else {
            // We win, re-claim
            CANBus_SendAddressClaim();
        }
    }
}

//-----------------------------------------------------------------------------
// Message handlers - to be implemented
// These will parse CAN messages and update the same variables as UDP
//-----------------------------------------------------------------------------

void CANBus_HandleRateCommand(const CAN_message_t& msg) {
    // PGN 0xFF03 - Rate Command from Gateway
    // Byte 0: ModuleId(0-3) | SensorId(4-7)
    // Bytes 1-2: Rate setpoint (0.001 UPM per bit)
    // Bytes 3-4: Manual adjust (signed)
    // Byte 5: Command byte
    // Bytes 6-7: Reserved

    uint8_t modId = msg.buf[0] & 0x0F;
    uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

    if (modId != MDL.ID) return;  // Not for us
    if (senId >= MaxProductCount) return;

    // Parse rate setpoint (matches UDP PGN 32500 format: 1000 X actual)
    uint32_t rateRaw = msg.buf[1] | ((uint32_t)msg.buf[2] << 8) | ((uint32_t)msg.buf[3] << 16);
    Sensor[senId].TargetUPM = rateRaw * 0.001;

    // Parse manual adjust
    int16_t manualAdj = msg.buf[4] | (msg.buf[5] << 8);
    Sensor[senId].ManualAdjust = manualAdj;

    // Parse command byte
    uint8_t cmd = msg.buf[6];
    if (cmd & 0x01) Sensor[senId].TotalPulses = 0;  // Reset quantity
    Sensor[senId].ControlType = (cmd >> 1) & 0x07;
    MasterOn = ((cmd & 0x10) == 0x10);
    Sensor[senId].AutoOn = ((cmd & 0x40) == 0x40);
    CalibrationOn[senId] = ((cmd & 0x80) == 0x80);

    // Update timeout
    Sensor[senId].CommTime = millis();
}

void CANBus_HandleRelayCommand(const CAN_message_t& msg) {
    // PGN 0xFF04 - Relay Command from Gateway
    // Matches UDP PGN 32501 format
    uint8_t modId = msg.buf[0] & 0x0F;
    if (modId != MDL.ID) return;

    RelayLo = msg.buf[1];
    RelayHi = msg.buf[2];
    PowerRelayLo = msg.buf[3];
    PowerRelayHi = msg.buf[4];
    InvertedLo = msg.buf[5];
    InvertedHi = msg.buf[6];
}

void CANBus_HandlePidSettings1(const CAN_message_t& msg) {
    // PGN 0xFF05 - PID Settings Part 1
    // Matches UDP PGN 32502 format (bytes 3-9)
    uint8_t modId = msg.buf[0] & 0x0F;
    uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

    if (modId != MDL.ID) return;
    if (senId >= MaxProductCount) return;

    // MaxPWM/MinPWM sent as percentage, convert to 0-255 range
    Sensor[senId].MaxPWM = (255.0 * msg.buf[1] / 100.0);
    Sensor[senId].MinPWM = (255.0 * msg.buf[2] / 100.0);

    // Kp/Ki use exponential scaling: 1.1^(value-120)
    if (msg.buf[3] > 0) {
        Sensor[senId].Kp = pow(1.1, msg.buf[3] - 120);
    } else {
        Sensor[senId].Kp = 0;
    }
    if (msg.buf[4] > 0) {
        Sensor[senId].Ki = pow(1.1, msg.buf[4] - 120);
    } else {
        Sensor[senId].Ki = 0;
    }

    Sensor[senId].Deadband = msg.buf[5] / 1000.0;  // Actual X 10, convert to fraction
    Sensor[senId].BrakePoint = msg.buf[6];
    Sensor[senId].PIDslowAdjust = msg.buf[7];
}

void CANBus_HandlePidSettings2(const CAN_message_t& msg) {
    // PGN 0xFF06 - PID Settings Part 2
    // Matches UDP PGN 32502 format (bytes 10-18)
    uint8_t modId = msg.buf[0] & 0x0F;
    uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

    if (modId != MDL.ID) return;
    if (senId >= MaxProductCount) return;

    Sensor[senId].SlewRate = msg.buf[1];
    Sensor[senId].MaxIntegral = msg.buf[2] / 10.0;  // Actual X 10
    Sensor[senId].TimedAdjust = msg.buf[3] | (msg.buf[4] << 8);
    Sensor[senId].TimedPause = msg.buf[5] | (msg.buf[6] << 8);
    Sensor[senId].PIDtime = msg.buf[7];
}

void CANBus_HandlePidSettings3(const CAN_message_t& msg) {
    // PGN 0xFF0B - PID Settings Part 3
    // Matches UDP PGN 32502 format (bytes 13, 19-22)
    uint8_t modId = msg.buf[0] & 0x0F;
    uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

    if (modId != MDL.ID) return;
    if (senId >= MaxProductCount) return;

    Sensor[senId].TimedMinStart = msg.buf[1] / 100.0;
    // PulseMinHz sent as Hz*10, convert to microseconds (max pulse time)
    if (msg.buf[2] > 0) {
        Sensor[senId].PulseMax = 10000000 / msg.buf[2];  // Hz*10 to micros
    }
    // PulseMaxHz sent as Hz, convert to microseconds (min pulse time)
    uint16_t pulseMaxHz = msg.buf[3] | (msg.buf[4] << 8);
    if (pulseMaxHz > 0) {
        Sensor[senId].PulseMin = 1000000 / pulseMaxHz;  // Hz to micros
    }
    Sensor[senId].PulseSampleSize = msg.buf[5];
    if (Sensor[senId].PulseSampleSize > MaxSampleSize) {
        Sensor[senId].PulseSampleSize = MaxSampleSize;
    }
}

void CANBus_HandleWheelConfig(const CAN_message_t& msg) {
    // PGN 0xFF07 - Wheel Speed Config
    // Matches UDP PGN 32504 format
    uint8_t modId = msg.buf[0] & 0x0F;
    if (modId != MDL.ID) return;

    MDL.WheelSpeedPin = msg.buf[1];
    uint32_t cal = msg.buf[2] | ((uint32_t)msg.buf[3] << 8) | ((uint32_t)msg.buf[4] << 16);
    MDL.WheelCal = (float)cal;

    if (msg.buf[5] & 0x01) {
        WheelCounts = 0;  // Erase counts
    }
}

void CANBus_HandleFlowCal(const CAN_message_t& msg) {
    // PGN 0xFF0A - Flow Calibration
    uint8_t modId = msg.buf[0] & 0x0F;
    uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

    if (modId != MDL.ID) return;
    if (senId >= MaxProductCount) return;

    uint32_t calRaw = msg.buf[1] | (msg.buf[2] << 8) | ((uint32_t)msg.buf[3] << 16);
    Sensor[senId].MeterCal = calRaw / 1000.0;
}

void CANBus_HandleSpeed(const CAN_message_t& msg, uint32_t pgn) {
    // Speed messages from tractor ECU
    // Bytes 0-1: Speed in 0.001 m/s (mm/s)
    uint16_t speed_mmps = msg.buf[0] | (msg.buf[1] << 8);

    if (speed_mmps != 0xFFFF) {
        // Convert to km/h: (mm/s) * 3600 / 1000000 = (mm/s) * 0.0036
        float speed_kmh = speed_mmps * 0.0036;

        // Store as ISOBUS speed source (could be used instead of wheel sensor)
        // IsobusSpeed = speed_kmh;
        // IsobusSpeedTime = millis();
    }
}

//-----------------------------------------------------------------------------
// Send sensor data to Gateway (PGN 0xFF00 - Rate/Quantity)
//-----------------------------------------------------------------------------
void CANBus_SendSensorRateQty(uint8_t sensorId) {
    if (sensorId >= MaxProductCount) return;

    uint8_t data[8];

    // Byte 0: ModuleId | SensorId
    data[0] = (MDL.ID & 0x0F) | ((sensorId & 0x0F) << 4);

    // Bytes 1-3: Rate applied (0.001 UPM per bit, matches UDP format)
    uint32_t rateRaw = (uint32_t)(Sensor[sensorId].UPM * 1000.0);
    data[1] = rateRaw & 0xFF;
    data[2] = (rateRaw >> 8) & 0xFF;
    data[3] = (rateRaw >> 16) & 0xFF;

    // Bytes 4-6: Accumulated quantity (calculated from TotalPulses/MeterCal)
    // MeterCal = pulses per unit, so quantity = TotalPulses / MeterCal
    float quantity = 0;
    if (Sensor[sensorId].MeterCal > 0) {
        quantity = Sensor[sensorId].TotalPulses / Sensor[sensorId].MeterCal;
    }
    uint32_t qtyRaw = (uint32_t)(quantity * 10.0);  // 0.1 units per bit
    data[4] = qtyRaw & 0xFF;
    data[5] = (qtyRaw >> 8) & 0xFF;
    data[6] = (qtyRaw >> 16) & 0xFF;

    // Byte 7: Status (bit 0 = sensor connected)
    data[7]=(millis()- Sensor[sensorId].CommTime<4000)?0x01:0x00;

    CANBus_SendProprietaryB(0x00, data, 8);
}

//-----------------------------------------------------------------------------
// Send sensor PWM/Hz data to Gateway (PGN 0xFF01)
//-----------------------------------------------------------------------------
void CANBus_SendSensorPwmHz(uint8_t sensorId) {
    if (sensorId >= MaxProductCount) return;

    uint8_t data[8];

    // Byte 0: ModuleId | SensorId
    data[0] = (MDL.ID & 0x0F) | ((sensorId & 0x0F) << 4);

    // Bytes 1-2: PWM setting (float stored, send as signed int)
    int16_t pwm = (int16_t)Sensor[sensorId].PWM;
    data[1] = pwm & 0xFF;
    data[2] = (pwm >> 8) & 0xFF;

    // Bytes 3-4: Pulse Hz (0.1 Hz per bit)
    uint16_t hzRaw = (uint16_t)(Sensor[sensorId].Hz * 10.0);
    data[3] = hzRaw & 0xFF;
    data[4] = (hzRaw >> 8) & 0xFF;

    // Bytes 5-7: Reserved
    data[5] = 0;
    data[6] = 0;
    data[7] = 0;

    CANBus_SendProprietaryB(0x01, data, 8);
}

//-----------------------------------------------------------------------------
// Send module status to Gateway (PGN 0xFF02)
//-----------------------------------------------------------------------------
void CANBus_SendModuleStatus() {
    uint8_t data[8];

    // Byte 0: ModuleId | Status bits
    data[0] = (MDL.ID & 0x0F);
    if (MDL.WorkPin < NC && digitalRead(MDL.WorkPin) == LOW) data[0] |= 0x10;  // Work switch
    if (Ethernet.linkStatus() == LinkON) data[0] |= 0x20;  // Ethernet connected
    if (GoodPins) data[0] |= 0x40;  // Good pin config

    // Byte 1: WiFi strength (N/A for Teensy - use 0)
    data[1] = 0;

    // Bytes 2-3: Pressure reading
    data[2] = PressureReading & 0xFF;
    data[3] = (PressureReading >> 8) & 0xFF;

    // Bytes 4-5: Wheel speed (0.1 km/h per bit)
    uint16_t wsRaw = (uint16_t)(WheelSpeed * 10.0);
    data[4] = wsRaw & 0xFF;
    data[5] = (wsRaw >> 8) & 0xFF;

    // Bytes 6-7: Wheel counts
    data[6] = WheelCounts & 0xFF;
    data[7] = (WheelCounts >> 8) & 0xFF;

    CANBus_SendProprietaryB(0x02, data, 8);
}

//-----------------------------------------------------------------------------
// Send module identification (PGN 0xFF08)
//-----------------------------------------------------------------------------
void CANBus_SendModuleIdent() {
    uint8_t data[8];

    // Byte 0: ModuleId | SensorCount
    data[0] = (MDL.ID & 0x0F) | ((MDL.SensorCount & 0x0F) << 4);

    // Byte 1: Module type
    data[1] = InoType;

    // Bytes 2-3: Firmware version
    data[2] = InoID & 0xFF;
    data[3] = (InoID >> 8) & 0xFF;

    // Bytes 4-7: Reserved
    data[4] = 0;
    data[5] = 0;
    data[6] = 0;
    data[7] = 0;

    CANBus_SendProprietaryB(0x08, data, 8);
}

//-----------------------------------------------------------------------------
// Periodic CAN communication (call from main loop)
//-----------------------------------------------------------------------------
void CANBus_Update() {
    static uint32_t lastSensorSend = 0;
    static uint32_t lastStatusSend = 0;
    static uint32_t lastClaimCheck = 0;
    static uint32_t lastIdentSend = 0;

    // Receive any pending messages
    CANBus_Receive();

    // Address claiming - check every 5 seconds
    if (!ISOBUSid.addressClaimed || (millis() - lastClaimCheck > 5000)) {
        lastClaimCheck = millis();
        if (!ISOBUSid.addressClaimed) {
            CANBus_SendAddressClaim();
        }
    }

    // Send sensor data at same rate as UDP (SendTime = 200ms)
    if (millis() - lastSensorSend >= SendTime) {
        lastSensorSend = millis();
        for (int i = 0; i < MDL.SensorCount; i++) {
            CANBus_SendSensorRateQty(i);
            CANBus_SendSensorPwmHz(i);
        }
    }

    // Send status at same rate as sensor data (SendTime = 200ms)
    if (millis() - lastStatusSend >= SendTime) {
        lastStatusSend = millis();
        CANBus_SendModuleStatus();
    }

    // Send module identification every 500ms
    if (millis() - lastIdentSend >= 500) {
        lastIdentSend = millis();
        CANBus_SendModuleIdent();
    }
}

//-----------------------------------------------------------------------------
// Send proprietary status messages for TC Client mode
// Sends PWM/Hz (0xFF01), module status (0xFF02), and module ident (0xFF08)
// so Gateway can forward complete status to RC
// Rate/qty is handled by TC Client process data (DDI 2, DDI 48)
//-----------------------------------------------------------------------------
void CANBus_SendProprietaryStatus() {
    static uint32_t lastPwmHzSend = 0;
    static uint32_t lastStatusSend = 0;
    static uint32_t lastIdentSend = 0;

    if (!ISOBUSid.addressClaimed) return;

    // Send PWM/Hz at same rate as sensor data (SendTime = 200ms)
    if (millis() - lastPwmHzSend >= SendTime) {
        lastPwmHzSend = millis();
        for (int i = 0; i < MDL.SensorCount; i++) {
            CANBus_SendSensorPwmHz(i);
        }
    }

    // Send module status at same rate (triggers Gateway to send PGN 32401 with full data)
    if (millis() - lastStatusSend >= SendTime) {
        lastStatusSend = millis();
        CANBus_SendModuleStatus();
    }

    // Send module identification every 500ms
    if (millis() - lastIdentSend >= 500) {
        lastIdentSend = millis();
        CANBus_SendModuleIdent();
    }
}

