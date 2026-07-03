
// Uncomment to enable verbose CAN receive logging on Serial (development only)
// #define CANBUS_DEBUG

// CANBus.ino - ISOBUS CAN communication for Teensy 4.1
// Uses FlexCAN_T4 library (built-in for Teensy 4.x)
// CAN1 pins: TX=22, RX=23 (connected to MCP2562-E/P transceiver)

uint8_t StandbyPin = 6;

FLASHMEM void CANBus_DebugStep(const __FlashStringHelper *step)
{
	Serial.print(F("CAN/ISOBUS step: "));
	Serial.println(step);
	Serial.flush();
}

class RCteensyCANPlugin : public isobus::CANHardwarePlugin
{
public:
	bool get_is_valid() const override
	{
		return isOpen;
	}

	void close() override
	{
		isOpen = false;
	}

	void open() override
	{
		can.begin();
		can.setBaudRate(250000);
		can.setMaxMB(16);
		can.setMBFilter(ACCEPT_ALL);
		isOpen = true;
	}

	bool read_frame(isobus::CANMessageFrame &canFrame) override
	{
		CAN_message_t message;
		if (!can.read(message)) return false;

		canFrame.identifier = message.id;
		canFrame.channel = 0;
		canFrame.dataLength = message.len;
		canFrame.isExtendedFrame = message.flags.extended;
		canFrame.timestamp_us = message.timestamp;
		memcpy(canFrame.data, message.buf, canFrame.dataLength);
		return true;
	}

	bool write_frame(const isobus::CANMessageFrame &canFrame) override
	{
		CAN_message_t message = {};
		message.id = canFrame.identifier;
		message.len = canFrame.dataLength;
		message.flags.extended = canFrame.isExtendedFrame;
		message.seq = true;
		memcpy(message.buf, canFrame.data, canFrame.dataLength);
		return can.write(message);
	}

private:
	FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_512> can;
	bool isOpen = false;
};

RCteensyCANPlugin rcTeensyCANPlugin;

FLASHMEM void CANBus_DoNotDeleteCANPlugin(isobus::CANHardwarePlugin *)
{
}

// ISOBUS address claiming
struct IsobusIdentity {
	uint8_t address = 0x90;          // Preferred self-configurable implement address
	bool addressClaimed = false;
	uint32_t lastClaimTime = 0;

	// ISO 11783 NAME fields (64-bit)
	uint32_t identityNumber = 1;      // Unique serial number (21 bits)
	uint16_t manufacturerCode = 0;    // Assigned by AEF (11 bits)
	uint8_t deviceClass = 25;         // 25 = Sprayer/Spreader control
	uint8_t deviceClassInstance = 0;
	uint8_t functionCode = 128;       // 128 = Rate Control
	uint8_t functionInstance = 0;
	uint8_t industryGroup = 2;        // 2 = Agricultural
	bool selfConfigurable = true;
};

DMAMEM IsobusIdentity ISOBUSid;

// Accumulation buffer for PGN 32700 module config received over CAN (4 × 8-byte frames)
// cfgBuf[0..29] maps to PGN32700 bytes [2..31]: ModID, SensorCount, commands, relay types,
//   sensor pins, relay pins [0-15], WorkPin, PressurePin, CommMode
// No ModID identity check — mirrors the UDP handler which also applies unconditionally.
uint8_t cfgBuf[30];
uint8_t cfgFrames = 0;  // bitmask: bit0=0xFF0C, bit1=0xFF0D, bit2=0xFF0E, bit3=0xFF0F

//-----------------------------------------------------------------------------
// Build 64-bit NAME from identity fields
//-----------------------------------------------------------------------------
FLASHMEM uint64_t buildIsobusNAME() {
	isobus::NAME name(0);
	name.set_arbitrary_address_capable(ISOBUSid.selfConfigurable);
	name.set_industry_group(ISOBUSid.industryGroup);
	name.set_device_class(ISOBUSid.deviceClass);
	name.set_function_code(ISOBUSid.functionCode);
	name.set_identity_number(ISOBUSid.identityNumber);
	name.set_ecu_instance(0);
	name.set_function_instance(ISOBUSid.functionInstance);
	name.set_device_class_instance(ISOBUSid.deviceClassInstance);
	name.set_manufacturer_code(ISOBUSid.manufacturerCode);
	return name.get_full_name();
}

//-----------------------------------------------------------------------------
// Initialize CAN hardware
//-----------------------------------------------------------------------------
FLASHMEM bool CANBus_Begin() {
	CANBus_DebugStep(F("transceiver"));
	// Enable CAN transceiver (STBY pin LOW = active)
	pinMode(StandbyPin, OUTPUT);
	digitalWrite(StandbyPin, LOW);

	// Set identity number from module ID
	ISOBUSid.identityNumber = MDL.ID + 1000;

	CANBus_DebugStep(F("AgIsoStack setup"));
	setupAgIsoStack();

	return true;
}

FLASHMEM void setupAgIsoStack() {
	CANBus_DebugStep(F("CAN plugin"));
	canPlugin = std::shared_ptr<isobus::CANHardwarePlugin>(&rcTeensyCANPlugin, CANBus_DoNotDeleteCANPlugin);
	CANBus_DebugStep(F("CAN channel"));
	isobus::CANHardwareInterface::set_number_of_can_channels(1);
	isobus::CANHardwareInterface::assign_can_channel_frame_handler(0, canPlugin);
	isobus::CANHardwareInterface::get_can_frame_received_event_dispatcher().add_listener(CANBus_HandleFrame);
	CANBus_DebugStep(F("CAN hardware start"));
	isobus::CANHardwareInterface::start();
	CANBus_DebugStep(F("CAN hardware update"));
	isobus::CANHardwareInterface::update();

	CANBus_DebugStep(F("NAME"));
	isobus::NAME myNAME(buildIsobusNAME());

	CANBus_DebugStep(F("internal control function"));
	ISOBUSControlFunction = isobus::CANNetworkManager::CANNetwork.create_internal_control_function(myNAME, 0, ISOBUSid.address);
	ISOBUSControlFunction->get_address_claimed_event_dispatcher().add_listener(CANBus_HandleAddressClaimed);
	CANBus_DebugStep(F("diagnostics"));
	ISOBUSDiagnostics = std::make_shared<isobus::DiagnosticProtocol>(ISOBUSControlFunction);
	ISOBUSDiagnostics->initialize();
	ISOBUSDiagnostics->set_product_identification_brand("AgOpenGPS");
	ISOBUSDiagnostics->set_product_identification_code("RCteensy");
	ISOBUSDiagnostics->set_product_identification_model("Teensy Rate");
	ISOBUSDiagnostics->set_software_id_field(0, String(InoID).c_str());
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::HardwareID, "Teensy 4.1");
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::Location, "Implement");
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::ManufacturerName, "AgOpenGPS");
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::PartNumber, "RCteensy");
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::SerialNumber, String(MDL.ID).c_str());
	ISOBUSDiagnostics->set_ecu_id_field(isobus::DiagnosticProtocol::ECUIdentificationFields::Type, "RateControl");

	CANBus_DebugStep(F("speed client"));
	SPEED_Begin();
	CANBus_DebugStep(F("VT client"));
	VT_Begin();
	CANBus_DebugStep(F("TC client"));
	TC_Begin();
	CANBus_DebugStep(F("ready"));
}

//-----------------------------------------------------------------------------
// Send address claim message (PGN 60928 / 0xEE00)
//-----------------------------------------------------------------------------
FLASHMEM void CANBus_SendAddressClaim() {
	CAN_message_t msg;

	// Build 29-bit CAN ID for Address Claimed
	// Priority=6, PF=0xEE, DA=0xFF (global broadcast), SA=claimed address
	msg.id = (6UL << 26) | (0xEEUL << 16) | (0xFFUL << 8) | ISOBUSid.address;
	msg.flags.extended = 1;
	msg.len = 8;

	// Pack 64-bit NAME into 8 bytes (little-endian)
	uint64_t name = buildIsobusNAME();
	for (int i = 0; i < 8; i++) {
		msg.buf[i] = (name >> (i * 8)) & 0xFF;
	}

#ifdef CANBUS_DEBUG
	Serial.println("Address claim is handled by AgIsoStack.");
#endif
}

//-----------------------------------------------------------------------------
// Send proprietary message (PGN 0xFF00-0xFFFF range)
//-----------------------------------------------------------------------------
FLASHMEM bool CANBus_SendProprietaryB(uint8_t pgnLow, const uint8_t* data, uint8_t len) {
	if (!ISOBUSid.addressClaimed) return false;

	isobus::CANMessageFrame frame = {};

	// Build 29-bit CAN ID for Proprietary B
	// Priority=6, DP=0, PF=0xFF, PS=pgnLow, SA=our address
	frame.identifier = (6UL << 26) | (0xFFUL << 16) | ((uint32_t)pgnLow << 8) | ISOBUSid.address;
	frame.channel = 0;
	frame.isExtendedFrame = true;
	frame.dataLength = (len > 8) ? 8 : len;
	frame.timestamp_us = micros();

	memcpy(frame.data, data, frame.dataLength);

	return isobus::CANHardwareInterface::transmit_can_frame(frame);
}

//-----------------------------------------------------------------------------
// Process received CAN messages
//-----------------------------------------------------------------------------
FLASHMEM void CANBus_Receive() {
	isobus::CANHardwareInterface::update();
}

FLASHMEM void CANBus_HandleAddressClaimed(const std::uint8_t address) {
	ISOBUSid.address = address;
	ISOBUSid.addressClaimed = (address <= 0xFD);
}

FLASHMEM void CANBus_HandleFrame(const isobus::CANMessageFrame &frame) {
	if (!frame.isExtendedFrame) return;

	CAN_message_t msg = {};
	msg.id = frame.identifier;
	msg.flags.extended = 1;
	msg.len = frame.dataLength;
	memcpy(msg.buf, frame.data, msg.len);

	// Extract PGN from 29-bit ID
	// ID format: Priority(3) | R(1) | DP(1) | PF(8) | PS(8) | SA(8)
	uint8_t pf = (msg.id >> 16) & 0xFF;  // PDU Format
	uint8_t ps = (msg.id >> 8) & 0xFF;   // PDU Specific
#ifdef CANBUS_DEBUG
	uint8_t sa = msg.id & 0xFF;          // Source Address
#endif

	uint32_t pgn;
	if (pf >= 240) {
		// PDU2 format: PGN = PF * 256 + PS
		pgn = ((uint32_t)pf << 8) | ps;
	}
	else {
		// PDU1 format: PGN = PF * 256, PS is destination
		pgn = (uint32_t)pf << 8;
	}

#ifdef CANBUS_DEBUG
	Serial.print("RX PGN 0x");
	Serial.print(pgn, HEX);
	Serial.print(" from 0x");
	Serial.print(sa, HEX);
	Serial.print(" to 0x");
	Serial.println(ps, HEX);
	#endif

#if ISOBUS_TC_MODE
	return;
#endif

	// Handle specific PGNs
	switch (pgn) {
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

	case 0xFF0C:  // Module config part 1 (from PGN 32700)
		CANBus_HandleModuleConfig1(msg);
		break;

	case 0xFF0D:  // Module config part 2 (from PGN 32700)
		CANBus_HandleModuleConfig2(msg);
		break;

	case 0xFF0E:  // Module config part 3 (from PGN 32700)
		CANBus_HandleModuleConfig3(msg);
		break;

	case 0xFF0F:  // Module config part 4 / commit (from PGN 32700)
		CANBus_HandleModuleConfig4(msg);
		break;
	}
}

//-----------------------------------------------------------------------------
// Handle address claim from another device
//-----------------------------------------------------------------------------
FLASHMEM void CANBus_HandleAddressClaim(const CAN_message_t& msg, uint8_t sourceAddr) {
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
		}
		else {
			// We win, re-claim
			CANBus_SendAddressClaim();
		}
	}
}

//-----------------------------------------------------------------------------
// Message handlers - to be implemented
// These will parse CAN messages and update the same variables as UDP
//-----------------------------------------------------------------------------

FLASHMEM void CANBus_HandleRateCommand(const CAN_message_t& msg) {
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

FLASHMEM void CANBus_HandleRelayCommand(const CAN_message_t& msg) {
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
	FlowMasterValveIndex = (msg.len > 7) ? msg.buf[7] : 255;
}

FLASHMEM void CANBus_HandlePidSettings1(const CAN_message_t& msg) {
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
	}
	else {
		Sensor[senId].Kp = 0;
	}
	if (msg.buf[4] > 0) {
		Sensor[senId].Ki = pow(1.1, msg.buf[4] - 120);
	}
	else {
		Sensor[senId].Ki = 0;
	}

	Sensor[senId].Deadband = msg.buf[5] / 1000.0;  // Actual X 10, convert to fraction
	Sensor[senId].BrakePoint = msg.buf[6];
	Sensor[senId].PIDslowAdjust = msg.buf[7];
}

FLASHMEM void CANBus_HandlePidSettings2(const CAN_message_t& msg) {
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

FLASHMEM void CANBus_HandlePidSettings3(const CAN_message_t& msg) {
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

FLASHMEM void CANBus_HandleWheelConfig(const CAN_message_t& msg) {
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

FLASHMEM void CANBus_HandleFlowCal(const CAN_message_t& msg) {
	// PGN 0xFF0A - Flow Calibration
	uint8_t modId = msg.buf[0] & 0x0F;
	uint8_t senId = (msg.buf[0] >> 4) & 0x0F;

	if (modId != MDL.ID) return;
	if (senId >= MaxProductCount) return;

	uint32_t calRaw = msg.buf[1] | (msg.buf[2] << 8) | ((uint32_t)msg.buf[3] << 16);
	Sensor[senId].MeterCal = calRaw / 1000.0;
}

//-----------------------------------------------------------------------------
// Module config over CAN — 4-frame accumulation of PGN 32700
// Frame layout (cfgBuf offset = PGN32700 byte - 2):
//   0xFF0C → cfgBuf[0..7]  = pgnData[2..9]  (ModID, SensorCount, commands, relay types, Sensor0 pins)
//   0xFF0D → cfgBuf[8..15] = pgnData[10..17] (Sensor1 pins, RelayPins[0-4])
//   0xFF0E → cfgBuf[16..23]= pgnData[18..25] (RelayPins[5-12])
//   0xFF0F → cfgBuf[24..29]= pgnData[26..31] (RelayPins[13-15], WorkPin, PressurePin, CommMode)
//            buf[6] = ModID (identity check), buf[7] = 0
//-----------------------------------------------------------------------------

FLASHMEM void CANBus_HandleModuleConfig1(const CAN_message_t& msg) {
	cfgFrames = 0;  // restart sequence — discard any partial previous collection
	memcpy(cfgBuf, msg.buf, 8);
	cfgFrames = 0x01;
}

FLASHMEM void CANBus_HandleModuleConfig2(const CAN_message_t& msg) {
	memcpy(cfgBuf + 8, msg.buf, 8);
	cfgFrames |= 0x02;
}

FLASHMEM void CANBus_HandleModuleConfig3(const CAN_message_t& msg) {
	memcpy(cfgBuf + 16, msg.buf, 8);
	cfgFrames |= 0x04;
}

FLASHMEM void CANBus_HandleModuleConfig4(const CAN_message_t& msg) {
	memcpy(cfgBuf + 24, msg.buf, 6);  // only 6 config bytes; buf[6]=ModID, buf[7]=0
	cfgFrames |= 0x08;

	// All 4 frames must have arrived (no ModID identity check — mirrors UDP handler behaviour)
	if (cfgFrames != 0x0F) { cfgFrames = 0; return; }
	cfgFrames = 0;

	// Apply config (mirrors UDP PGN 32700 handler in Receive.ino case 32700)
	MDL.ID = cfgBuf[0];
	MDL.SensorCount = cfgBuf[1];

	uint8_t tmp = cfgBuf[2];
	MDL.InvertRelay = ((tmp & 1) == 1);
	MDL.InvertFlow = ((tmp & 2) == 2);
	MDL.WorkPinIsMomentary = ((tmp & 8) == 8);
	MDL.Is3Wire = ((tmp & 16) == 16);
	MDL.ADS1115Enabled = ((tmp & 32) == 32);

	MDL.OnboardRelayControl = cfgBuf[3];
	MDL.RemoteRelayControl = cfgBuf[4];

	Sensor[0].FlowPin = cfgBuf[5];
	Sensor[0].DirPin = cfgBuf[6];
	Sensor[0].PWMPin = cfgBuf[7];
	Sensor[1].FlowPin = cfgBuf[8];
	Sensor[1].DirPin = cfgBuf[9];
	Sensor[1].PWMPin = cfgBuf[10];

	for (int i = 0; i < 16; i++) MDL.RelayControlPins[i] = cfgBuf[11 + i];

	MDL.WorkPin = cfgBuf[27];
	MDL.PressurePin = cfgBuf[28];
	MDL.CommMode = cfgBuf[29];

	SaveData();
	SCB_AIRCR = 0x05FA0004;  // restart Teensy to apply new config
}

//-----------------------------------------------------------------------------
// Send sensor data to Gateway (PGN 0xFF00 - Rate/Quantity)
//-----------------------------------------------------------------------------
FLASHMEM void CANBus_SendSensorRateQty(uint8_t sensorId) {
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
	data[7] = (millis() - Sensor[sensorId].CommTime < 4000) ? 0x01 : 0x00;

	CANBus_SendProprietaryB(0x00, data, 8);
}

//-----------------------------------------------------------------------------
// Send sensor PWM/Hz data to Gateway (PGN 0xFF01)
//-----------------------------------------------------------------------------
FLASHMEM void CANBus_SendSensorPwmHz(uint8_t sensorId) {
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
FLASHMEM void CANBus_SendModuleStatus() {
	uint8_t data[8];

	// Byte 0: ModuleId | Status bits
	data[0] = (MDL.ID & 0x0F);
	if (MDL.WorkPin < NC && digitalRead(MDL.WorkPin) == LOW) data[0] |= 0x10;  // Work switch
#if ETHERNET_COMM_ENABLED
	if (Ethernet.linkStatus() == LinkON) data[0] |= 0x20;  // Ethernet connected
#endif
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
FLASHMEM void CANBus_SendModuleIdent() {
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
FLASHMEM void CANBus_Update() {
	static uint32_t lastSensorSend = 0;
	static uint32_t lastStatusSend = 0;
	static uint32_t lastIdentSend = 0;

	if (ISOBUSDiagnostics != nullptr) {
		ISOBUSDiagnostics->update();
	}

	CANBus_Receive();
	SPEED_Update();
	VT_Update();
	TC_Update();

	// Send sensor data at same rate as UDP (SendTime = 200ms)
	if (CANProprietaryTelemetryEnabled && millis() - lastSensorSend >= SendTime) {
		lastSensorSend = millis();
		for (int i = 0; i < MDL.SensorCount; i++) {
			CANBus_SendSensorRateQty(i);
			CANBus_SendSensorPwmHz(i);
		}
	}

	// Send status at same rate as sensor data (SendTime = 200ms)
	if (CANProprietaryTelemetryEnabled && millis() - lastStatusSend >= SendTime) {
		lastStatusSend = millis();
		CANBus_SendModuleStatus();
	}

	// Send module identification every 500ms
	if (CANProprietaryTelemetryEnabled && millis() - lastIdentSend >= 500) {
		lastIdentSend = millis();
		CANBus_SendModuleIdent();
	}
}
