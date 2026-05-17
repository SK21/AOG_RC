
// Uncomment to enable verbose CAN receive logging on Serial (development only)
// #define CANBUS_DEBUG

// CANBus.ino - ISOBUS CAN communication for Teensy 4.1
// Uses AgIsoStack++ via RCteensyCANPlugin wrapping FlexCAN_T4
// CAN1 pins: TX=22, RX=23 (connected to MCP2562-E/P transceiver)

uint8_t StandbyPin = 6;

FLASHMEM void CANBus_DebugStep(const __FlashStringHelper* step)
{
	Serial.print(F("CAN/ISOBUS step: "));
	Serial.println(step);
	Serial.flush();
}

// ─── AgIsoStack++ CAN hardware plugin ────────────────────────────────────────

class RCteensyCANPlugin : public isobus::CANHardwarePlugin
{
public:
	bool get_is_valid() const override { return isOpen; }

	void close() override { isOpen = false; }

	void open() override
	{
		can.begin();
		can.setBaudRate(250000);
		can.setMaxMB(16);
		can.setMBFilter(ACCEPT_ALL);
		isOpen = true;
	}

	bool read_frame(isobus::CANMessageFrame& canFrame) override
	{
		CAN_message_t message;
		if (!can.read(message)) return false;

		canFrame.identifier      = message.id;
		canFrame.channel         = 0;
		canFrame.dataLength      = message.len;
		canFrame.isExtendedFrame = message.flags.extended;
		canFrame.timestamp_us    = message.timestamp;
		memcpy(canFrame.data, message.buf, canFrame.dataLength);
		return true;
	}

	bool write_frame(const isobus::CANMessageFrame& canFrame) override
	{
		CAN_message_t message = {};
		message.id             = canFrame.identifier;
		message.len            = canFrame.dataLength;
		message.flags.extended = canFrame.isExtendedFrame;
		message.seq            = true;
		memcpy(message.buf, canFrame.data, canFrame.dataLength);
		return can.write(message);
	}

private:
	FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_512> can;
	bool isOpen = false;
};

RCteensyCANPlugin rcTeensyCANPlugin;

FLASHMEM void CANBus_DoNotDeleteCANPlugin(isobus::CANHardwarePlugin*) {}

// ─── ISOBUS identity ──────────────────────────────────────────────────────────

struct IsobusIdentity
{
	uint8_t  address              = 0x90;   // preferred self-configurable implement address
	bool     addressClaimed       = false;
	uint32_t lastClaimTime        = 0;

	uint32_t identityNumber       = 1;      // unique serial number (21 bits)
	uint16_t manufacturerCode     = 0;      // assigned by AEF (11 bits)
	uint8_t  deviceClass          = 25;     // 25 = Sprayer/Spreader control
	uint8_t  deviceClassInstance  = 0;
	uint8_t  functionCode         = 128;    // 128 = Rate Control
	uint8_t  functionInstance     = 0;
	uint8_t  industryGroup        = 2;      // 2 = Agricultural
	bool     selfConfigurable     = true;
};

DMAMEM IsobusIdentity ISOBUSid;

// ─── NAME builder ─────────────────────────────────────────────────────────────

FLASHMEM uint64_t buildIsobusNAME()
{
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

// ─── Initialisation ───────────────────────────────────────────────────────────

FLASHMEM bool CANBus_Begin()
{
	CANBus_DebugStep(F("transceiver"));
	pinMode(StandbyPin, OUTPUT);
	digitalWrite(StandbyPin, LOW);  // STBY LOW = transceiver active

	ISOBUSid.identityNumber = MDL.ID + 1000;

	CANBus_DebugStep(F("AgIsoStack setup"));
	setupAgIsoStack();
	return true;
}

FLASHMEM void setupAgIsoStack()
{
	CANBus_DebugStep(F("CAN plugin"));
	canPlugin = std::shared_ptr<isobus::CANHardwarePlugin>(
		&rcTeensyCANPlugin, CANBus_DoNotDeleteCANPlugin);

	CANBus_DebugStep(F("CAN channel"));
	isobus::CANHardwareInterface::set_number_of_can_channels(1);
	isobus::CANHardwareInterface::assign_can_channel_frame_handler(0, canPlugin);
	isobus::CANHardwareInterface::get_can_frame_received_event_dispatcher()
		.add_listener(CANBus_HandleFrame);

	CANBus_DebugStep(F("CAN hardware start"));
	isobus::CANHardwareInterface::start();
	CANBus_DebugStep(F("CAN hardware update"));
	isobus::CANHardwareInterface::update();

	CANBus_DebugStep(F("NAME"));
	isobus::NAME myNAME(buildIsobusNAME());

	CANBus_DebugStep(F("internal control function"));
	ISOBUSControlFunction =
		isobus::CANNetworkManager::CANNetwork.create_internal_control_function(
			myNAME, 0, ISOBUSid.address);
	ISOBUSControlFunction->get_address_claimed_event_dispatcher()
		.add_listener(CANBus_HandleAddressClaimed);

	CANBus_DebugStep(F("diagnostics"));
	ISOBUSDiagnostics = std::make_shared<isobus::DiagnosticProtocol>(ISOBUSControlFunction);
	ISOBUSDiagnostics->initialize();
	ISOBUSDiagnostics->set_product_identification_brand("AgOpenGPS");
	ISOBUSDiagnostics->set_product_identification_code("RCteensy");
	ISOBUSDiagnostics->set_product_identification_model("Teensy Rate");
	ISOBUSDiagnostics->set_software_id_field(0, String(InoID).c_str());
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::HardwareID, "Teensy 4.1");
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::Location, "Implement");
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::ManufacturerName, "AgOpenGPS");
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::PartNumber, "RCteensy");
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::SerialNumber,
		String(MDL.ID).c_str());
	ISOBUSDiagnostics->set_ecu_id_field(
		isobus::DiagnosticProtocol::ECUIdentificationFields::Type, "RateControl");

	CANBus_DebugStep(F("speed client"));
	SPEED_Begin();
	CANBus_DebugStep(F("VT client"));
	VT_Begin();
	CANBus_DebugStep(F("TC client"));
	TC_Begin();
	CANBus_DebugStep(F("ready"));
}

// ─── Address claimed callback ─────────────────────────────────────────────────

FLASHMEM void CANBus_HandleAddressClaimed(const std::uint8_t address)
{
	ISOBUSid.address       = address;
	ISOBUSid.addressClaimed = (address <= 0xFD);
}

// ─── Frame receive callback ───────────────────────────────────────────────────

FLASHMEM void CANBus_HandleFrame(const isobus::CANMessageFrame& frame)
{
	// AgIsoStack++ handles all ISOBUS frames internally (TC, VT, speed, address claiming).
	// No proprietary frame handling in ISOBUS-only firmware.
#ifdef CANBUS_DEBUG
	if (!frame.isExtendedFrame) return;
	uint8_t pf  = (frame.identifier >> 16) & 0xFF;
	uint8_t ps  = (frame.identifier >> 8)  & 0xFF;
	uint8_t sa  =  frame.identifier        & 0xFF;
	uint32_t pgn = (pf >= 240) ? (((uint32_t)pf << 8) | ps) : ((uint32_t)pf << 8);
	Serial.printf("RX PGN 0x%04lX from 0x%02X\n", pgn, sa);
#endif
}

// ─── Receive (drives AgIsoStack++ stack) ─────────────────────────────────────

FLASHMEM void CANBus_Receive()
{
	isobus::CANHardwareInterface::update();
}

// ─── Main update (call every loop iteration) ──────────────────────────────────

FLASHMEM void CANBus_Update()
{
	if (ISOBUSDiagnostics != nullptr) ISOBUSDiagnostics->update();
	CANBus_Receive();
	SPEED_Update();
	VT_Update();
	TC_Update();
}
