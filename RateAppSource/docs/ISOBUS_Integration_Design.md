# ISOBUS Integration Design Document

## RateController + AgIsoStack++ Integration

**Version:** 1.0
**Date:** January 2026
**Status:** Draft

---

## Table of Contents

1. [Overview](#1-overview)
2. [Goals and Requirements](#2-goals-and-requirements)
3. [System Architecture](#3-system-architecture)
4. [Component Design](#4-component-design)
5. [Protocol Mappings](#5-protocol-mappings)
6. [Configuration](#6-configuration)
7. [Implementation Phases](#7-implementation-phases)
8. [Hardware Requirements](#8-hardware-requirements)
9. [Testing Strategy](#9-testing-strategy)

---

## 1. Overview

### 1.1 Purpose

This document describes the design for adding ISOBUS (ISO 11783) support to the RateController system while preserving the existing UDP communication between the RateController application and Teensy 4.1 rate control modules.

### 1.2 Background

**Current System:**
- RateController (C# Windows Forms application)
- Teensy 4.1 modules running custom firmware
- Communication via UDP over Ethernet
- Custom PGN protocol (32400, 32500, etc.)

**Target System:**
- All existing functionality preserved
- Additional ISOBUS connectivity for:
  - Receiving prescription maps from Task Controllers
  - Reporting actual rates to ISOBUS terminals
  - Receiving speed data from ISOBUS
  - Integration with tractor Virtual Terminals

### 1.3 Reference Documents

| Document | Source |
|----------|--------|
| ISO 11783 (all parts) | ISO Standards |
| AgIsoStack++ (local) | G:\Sync\Documents\GitHub\SK21\AgIsoStack-plus-plus |
| AgIsoStack++ (upstream) | https://github.com/Open-Agriculture/AgIsoStack-plus-plus |
| AgIsoStack-Arduino | https://github.com/Open-Agriculture/AgIsoStack-Arduino |
| RateController Source | Current codebase |

---

## 2. Goals and Requirements

### 2.1 Functional Requirements

| ID | Requirement | Priority |
|----|-------------|----------|
| FR-01 | Preserve existing UDP communication | Must Have |
| FR-02 | Claim address on ISOBUS network | Must Have |
| FR-03 | Receive Task Controller prescriptions (VRA maps) | Must Have |
| FR-04 | Report actual application rates to ISOBUS | Must Have |
| FR-05 | Report accumulated quantities to ISOBUS | Must Have |
| FR-06 | Receive ground/wheel speed from ISOBUS | Should Have |
| FR-07 | Display on ISOBUS Virtual Terminal | Could Have |
| FR-08 | Support section control from ISOBUS | Could Have |
| FR-09 | Send/receive diagnostic messages (DM1/DM2) | Should Have |

### 2.2 Non-Functional Requirements

| ID | Requirement |
|----|-------------|
| NFR-01 | ISOBUS communication must not degrade UDP performance |
| NFR-02 | System must handle ISOBUS network disconnection gracefully |
| NFR-03 | Configuration must be persistable and user-editable |
| NFR-04 | Gateway must run as standalone Windows application |
| NFR-05 | Teensy firmware must fit in available flash/RAM |

### 2.3 Constraints

- RateController is C# (.NET Framework 4.8)
- AgIsoStack++ is C++11
- Teensy 4.1 has limited resources (1MB flash, 512KB RAM)
- ISOBUS runs at 250 kbps CAN

---

## 3. System Architecture

### 3.1 High-Level Architecture

**RateController acts as Task Controller (TC)**, sending setpoints to and receiving actuals from
Teensy modules which act as **Task Controller Clients**. The IsobusGateway bridges the UDP-based
RateController to the CAN-based ISOBUS network.

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              ISOBUS CAN Network (250 kbps)                      │
│                                                                                 │
│    ┌──────────────┐                                        ┌────────────────┐  │
│    │   Tractor    │                                        │  Teensy 4.1    │  │
│    │     ECU      │◄─── Speed Messages (0xFEF1, etc.) ───► │  Module 0..N   │  │
│    │              │                                        │                │  │
│    └──────┬───────┘                                        │  TC CLIENT     │  │
│           │                                                │  - DDOP        │  │
│           │                                                │  - Process Data│  │
│           │                                                │  - Diagnostics │  │
│           │                                                └───────┬────────┘  │
│           │                                                        │           │
│           │              ┌────────────────────┐                    │           │
│           │              │   IsobusGateway    │◄───────────────────┘           │
│           └─────────────►│   (C++ App)        │    Proprietary PGNs            │
│              Speed       │                    │    (0xFF00-0xFF0B)             │
│                          │   TC SERVER        │    + TC Protocol               │
│                          │   (on behalf of RC)│                                │
│                          └─────────┬──────────┘                                │
│                                    │                                           │
└────────────────────────────────────┼───────────────────────────────────────────┘
                                     │ USB-CAN (Candlelight)
                                     │
┌────────────────────────────────────┼───────────────────────────────────────────┐
│                                    │                            Windows PC     │
│                         ┌──────────┴──────────┐                                │
│                         │   IsobusGateway     │                                │
│                         │   (AgIsoStack++)    │                                │
│                         │                     │                                │
│                         │  - CAN Hardware     │                                │
│                         │  - Address Claiming │                                │
│                         │  - TC Server Proxy  │                                │
│                         │  - Speed Interface  │                                │
│                         │  - PGN Translation  │                                │
│                         └──────────┬──────────┘                                │
│                                    │                                           │
│                           UDP (localhost:32700/32701)                          │
│                                    │                                           │
│                         ┌──────────┴──────────┐                                │
│                         │   RateController    │                                │
│                         │   (C# WinForms)     │                                │
│                         │                     │                                │
│                         │   TASK CONTROLLER   │                                │
│                         │   - Rate Setpoints  │                                │
│                         │   - Section Control │                                │
│                         │   - Data Logging    │                                │
│                         └─────────────────────┘                                │
│                                                                                │
└────────────────────────────────────────────────────────────────────────────────┘
```

### 3.2 Component Roles

| Component | Role | Protocol | Responsibilities |
|-----------|------|----------|------------------|
| RateController | Task Controller | UDP | Setpoints, section control, logging, UI |
| IsobusGateway | TC Server Proxy | UDP ↔ CAN | Bridges RC to ISOBUS, handles TC protocol |
| Teensy Module | TC Client | CAN/ISOBUS | DDOP, process data, rate control hardware |
| Tractor ECU | Speed Source | CAN/ISOBUS | Provides speed messages to network |

### 3.3 Communication Flow

#### 3.3.1 Setpoint Flow (RateController → Teensy via Gateway)

```
┌────────────────┐     ┌─────────────┐     ┌────────────┐
│ RateController │     │   ISOBUS    │     │   Teensy   │
│ (TC Server)    │     │   Gateway   │     │ (TC Client)│
└───────┬────────┘     └──────┬──────┘     └─────┬──────┘
        │                     │                   │
        │ UDP PGN 32500       │                   │
        │ (Rate Settings)     │                   │
        │────────────────────►│                   │
        │                     │                   │
        │                     │ CAN 0xFF03        │
        │                     │ (Rate Command)    │
        │                     │──────────────────►│
        │                     │                   │
        │                     │ TC Process Data   │
        │                     │ (DDI Setpoints)   │
        │                     │──────────────────►│
        │                     │                   │
```

#### 3.3.2 Actual Rate Flow (Teensy → RateController via Gateway)

```
┌────────────┐     ┌─────────────┐     ┌────────────────┐
│   Teensy   │     │   ISOBUS    │     │ RateController │
│ (TC Client)│     │   Gateway   │     │ (TC Server)    │
└─────┬──────┘     └──────┬──────┘     └───────┬────────┘
      │                   │                    │
      │ CAN 0xFF00/01     │                    │
      │ (Sensor Data)     │                    │
      │──────────────────►│                    │
      │                   │                    │
      │ TC Process Data   │                    │
      │ (DDI Actuals)     │                    │
      │──────────────────►│                    │
      │                   │                    │
      │                   │ UDP PGN 32400      │
      │                   │ (Sensor Data)      │
      │                   │───────────────────►│
      │                   │                    │
      │ (Sensor Data)      │                     │                   │
      │───────────────────►│                     │                   │
      │                    │                     │                   │
      │                    │ UDP PGN 32601       │                   │
      │                    │ (Actual Rate)       │                   │
      │                    │────────────────────►│                   │
      │                    │                     │                   │
      │                    │                     │ Process Data      │
      │                    │                     │ (DDI 2: Actual)   │
      │                    │                     │──────────────────►│
      │                    │                     │                   │
```

### 3.3 Data Flow Modes

The system supports multiple operational modes:

| Mode | Rate Source | Description |
|------|-------------|-------------|
| **UDP Only** | RateController | Traditional operation, ISOBUS disabled |
| **ISOBUS Priority** | Task Controller | TC prescription overrides RC rate |
| **RC Priority** | RateController | RC rate used, ISOBUS reports only |
| **Blended** | Both | Manual override via RC, auto from TC |

---

## 4. Component Design

### 4.1 ISOBUS Gateway (C++ Application)

#### 4.1.1 Overview

The gateway is a standalone C++ application using AgIsoStack++ that:
- Connects to the ISOBUS CAN network
- Communicates with RateController via UDP
- Acts as a protocol translator

#### 4.1.2 Class Structure

```
IsobusGateway/
├── src/
│   ├── main.cpp                    # Entry point
│   ├── Gateway.hpp                 # Main gateway class
│   ├── Gateway.cpp
│   ├── CANInterface.hpp            # CAN hardware abstraction
│   ├── CANInterface.cpp
│   ├── UDPBridge.hpp               # UDP communication with RC
│   ├── UDPBridge.cpp
│   ├── TaskControllerHandler.hpp   # TC client logic
│   ├── TaskControllerHandler.cpp
│   ├── SpeedHandler.hpp            # Speed message handling
│   ├── SpeedHandler.cpp
│   ├── DiagnosticsHandler.hpp      # DM message handling
│   ├── DiagnosticsHandler.cpp
│   ├── Configuration.hpp           # Settings management
│   ├── Configuration.cpp
│   ├── Logger.hpp                  # Logging utility
│   └── Logger.cpp
├── include/
│   └── IsobusGateway/
│       └── Protocol.hpp            # Shared protocol definitions
├── config/
│   └── gateway.json                # Configuration file
└── CMakeLists.txt
```

#### 4.1.3 CAN Hardware Interfaces (AgIsoStack++)

The gateway leverages CAN hardware interfaces from the local AgIsoStack++ repository at `G:\Sync\Documents\GitHub\SK21\AgIsoStack-plus-plus`. The following interfaces are available:

##### Available CAN Plugins

| Interface | Platform | Source Files | Use Case |
|-----------|----------|--------------|----------|
| **CandleCANInterface** | Windows | `candle_can_interface.hpp/cpp` | **Recommended** - Low-cost USB adapters |
| SocketCANInterface | Linux | `socket_can_interface.hpp/cpp` | Native Linux CAN |
| MCP2515CANInterface | Embedded | `mcp2515_can_interface.hpp/cpp` | SPI-based CAN controllers |

##### Candlelight USB-CAN Adapters (Recommended)

The `CandleCANInterface` class provides native Windows support for USB-CAN adapters running **Candlelight/gs_usb firmware**. This is the recommended option due to:

- **Low cost**: CANable, CANtact, and clones available for $15-50
- **No drivers required**: Uses WinUSB (built into Windows)
- **Open source firmware**: Can be flashed onto many STM32-based adapters
- **Wide compatibility**: Works with many generic USB-CAN adapters

**Compatible Hardware:**

| Adapter | Approx. Cost | Notes |
|---------|--------------|-------|
| CANable Pro | $40 | Official, isolated option available |
| CANable 2.0 | $25 | USB-C, CAN FD capable |
| CANtact | $30 | Original Candlelight reference design |
| Canable clones | $10-20 | Many AliExpress/Amazon options |
| USBtin | $25 | Alternative firmware available |
| Any gs_usb compatible | Varies | STM32F042/F072 based devices |

**Candlelight Interface Usage:**

```cpp
#include "isobus/hardware_integration/candle_can_interface.hpp"

// Create interface for first Candlelight device, channel 0, 250kbps (ISOBUS)
auto candlePlugin = std::make_shared<isobus::CandleCANInterface>(
    0,       // deviceIndex - first USB device found
    0,       // channel - first CAN channel on device
    250000   // bitrate - ISOBUS standard is 250 kbps
);

// Open the device
candlePlugin->open();

// Check if ready
if (candlePlugin->get_is_valid()) {
    // Register with AgIsoStack++
    isobus::CANHardwareInterface::set_number_of_can_channels(1);
    isobus::CANHardwareInterface::assign_can_channel_frame_handler(0, candlePlugin);
    isobus::CANHardwareInterface::start();
}
```

**CandleCANInterface Class (from AgIsoStack++):**

Location: `AgIsoStack-plus-plus/hardware_integration/include/isobus/hardware_integration/candle_can_interface.hpp`

```cpp
class CandleCANInterface : public CANHardwarePlugin {
public:
    // Constructor
    explicit CandleCANInterface(
        std::uint8_t deviceIndex = 0,    // Which USB device (0 = first)
        std::uint8_t channel = 0,        // CAN channel on device
        std::uint32_t bitrate = 250000   // Bitrate (250000 for ISOBUS)
    );

    // CANHardwarePlugin interface
    std::string get_name() const override;      // Returns "CandleCAN"
    bool get_is_valid() const override;         // True if device open and ready
    void open() override;                       // Connect to USB device
    void close() override;                      // Disconnect from device
    bool read_frame(CANMessageFrame& frame) override;   // Receive CAN frame
    bool write_frame(const CANMessageFrame& frame) override;  // Send CAN frame

    // Accessors
    std::uint8_t get_device_index() const;
    std::uint8_t get_channel() const;
    std::uint32_t get_bitrate() const;
};
```

**Supporting Candle Library Files:**

The Candlelight implementation includes a complete low-level USB driver:

```
AgIsoStack-plus-plus/hardware_integration/
├── include/isobus/hardware_integration/
│   ├── candle_can_interface.hpp          # Main interface class
│   └── candle/
│       ├── candle.h                      # Core Candle API
│       ├── candle_defs.h                 # Constants and enums
│       ├── candle_ctrl_req.h             # USB control requests
│       └── ch_9.h                        # USB chapter 9 definitions
└── src/
    ├── candle_can_interface.cpp          # Interface implementation
    └── candle/
        ├── candle.cpp                    # Core Candle implementation
        └── candle_ctrl_req.cpp           # USB control implementation
```

**Error Handling:**

The Candle library provides detailed error codes:

| Error Code | Description |
|------------|-------------|
| `CANDLE_ERR_OK` | Success |
| `CANDLE_ERR_CREATE_FILE` | Failed to open USB device |
| `CANDLE_ERR_WINUSB_INITIALIZE` | WinUSB initialization failed |
| `CANDLE_ERR_BITRATE_UNSUPPORTED` | Requested bitrate not supported |
| `CANDLE_ERR_READ_TIMEOUT` | No message received (normal) |
| `CANDLE_ERR_SEND_FRAME` | Failed to transmit frame |
| `CANDLE_ERR_DEV_OUT_OF_RANGE` | Invalid device index |

##### Alternative: PCAN Interface

For users with Peak Systems hardware, PCAN is also supported (requires separate PCAN-Basic library installation):

```cpp
#include "isobus/hardware_integration/pcan_basic_interface.hpp"

auto pcanPlugin = std::make_shared<isobus::PCANBasicInterface>(PCAN_USBBUS1);
pcanPlugin->open();
```

#### 4.1.4 Gateway Class Design

```cpp
// Gateway.hpp
#pragma once

#include <memory>
#include <atomic>
#include "isobus/isobus/can_network_manager.hpp"
#include "isobus/isobus/isobus_task_controller_client.hpp"
#include "isobus/isobus/isobus_speed_distance_messages.hpp"
#include "isobus/isobus/isobus_diagnostic_protocol.hpp"

namespace aog {

class Gateway {
public:
    Gateway();
    ~Gateway();

    // Lifecycle
    bool initialize(const std::string& configPath);
    void run();
    void shutdown();

    // Status
    bool isRunning() const { return m_running; }
    bool isIsobusConnected() const;
    bool isRateControllerConnected() const;

private:
    // Initialization
    bool initializeCAN();
    bool initializeUDP();
    bool initializeIsobus();
    bool createDeviceDescriptor();

    // Update loops
    void updateLoop();
    void processUDPMessages();
    void processIsobusCallbacks();
    void sendPeriodicData();

    // UDP message handlers (from RateController)
    void handlePGN32601(const uint8_t* data, size_t len);  // Actual rate report
    void handlePGN32602(const uint8_t* data, size_t len);  // Status report
    void handlePGN32603(const uint8_t* data, size_t len);  // Section status

    // ISOBUS callbacks
    static void onTCValueCommand(uint16_t element, uint16_t ddi,
                                  int32_t value, bool& ack, void* parent);
    static void onSpeedMessage(const isobus::SpeedMessagesInterface::
                                MachineSelectedSpeedData& data, void* parent);

    // Send to RateController
    void sendIsobusRateToRC(uint8_t product, double rate);
    void sendIsobusSpeedToRC(double speed_mps);
    void sendIsobusStatusToRC();

private:
    std::atomic<bool> m_running{false};

    // Configuration
    std::unique_ptr<Configuration> m_config;

    // AgIsoStack++ components
    std::shared_ptr<isobus::InternalControlFunction> m_controlFunction;
    std::shared_ptr<isobus::TaskControllerClient> m_tcClient;
    std::shared_ptr<isobus::SpeedMessagesInterface> m_speedInterface;
    std::shared_ptr<isobus::DiagnosticProtocol> m_diagnostics;
    std::shared_ptr<isobus::DeviceDescriptorObjectPool> m_ddop;

    // UDP bridge
    std::unique_ptr<UDPBridge> m_udpBridge;

    // State
    struct ProductState {
        double targetRate{0};       // From TC
        double actualRate{0};       // From RC
        double totalQuantity{0};    // From RC
        bool sectionOn{false};      // From RC
    };
    std::array<ProductState, 6> m_products;

    double m_groundSpeed{0};        // From ISOBUS
    double m_wheelSpeed{0};         // From ISOBUS
};

} // namespace aog
```

#### 4.1.4 Device Descriptor Object Pool (DDOP)

The DDOP defines the device structure for Task Controller communication:

```cpp
bool Gateway::createDeviceDescriptor() {
    m_ddop = std::make_shared<isobus::DeviceDescriptorObjectPool>();

    // Device (DVC) - Root object
    // Represents the entire rate controller system
    m_ddop->add_device(
        "AOG RateController",           // Designator
        "1.0.0",                        // Software version
        "1234567",                      // Serial number
        "",                             // Structure label
        {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00}, // Localization
        {},                             // Extended structure
        0                               // Client NAME
    );

    // Device Element (DET) - Main boom/implement
    auto mainElement = m_ddop->add_device_element(
        "Main Controller",              // Designator
        1,                              // Element number
        0,                              // Parent object ID (device)
        isobus::task_controller_object::DeviceElementType::Device,
        0                               // Object ID
    );

    // For each product (up to 6)
    for (int i = 0; i < m_config->numProducts(); i++) {
        uint16_t elementNum = 100 + i;

        // Device Element for product section
        auto productElement = m_ddop->add_device_element(
            "Product " + std::to_string(i + 1),
            elementNum,
            mainElement,                // Parent = main element
            isobus::task_controller_object::DeviceElementType::Section,
            0
        );

        // Device Process Data (DPD) - Setpoint Rate (settable)
        m_ddop->add_device_process_data(
            "Target Rate",
            static_cast<uint16_t>(isobus::DataDescriptionIndex::SetpointVolumePerAreaApplicationRate),
            isobus::task_controller_object::ProcessDataProperties::Settable |
            isobus::task_controller_object::ProcessDataProperties::ControlSource,
            isobus::task_controller_object::ProcessDataTriggertMethods::OnChange,
            productElement
        );

        // Device Process Data (DPD) - Actual Rate (reportable)
        m_ddop->add_device_process_data(
            "Actual Rate",
            static_cast<uint16_t>(isobus::DataDescriptionIndex::ActualVolumePerAreaApplicationRate),
            isobus::task_controller_object::ProcessDataProperties::MemberOfDefaultSet,
            isobus::task_controller_object::ProcessDataTriggertMethods::TimeInterval,
            productElement
        );

        // Device Process Data (DPD) - Total Volume (accumulator)
        m_ddop->add_device_process_data(
            "Total Volume",
            static_cast<uint16_t>(isobus::DataDescriptionIndex::TotalVolumeContent),
            isobus::task_controller_object::ProcessDataProperties::MemberOfDefaultSet,
            isobus::task_controller_object::ProcessDataTriggertMethods::Total,
            productElement
        );

        // Device Process Data (DPD) - Section Control State
        m_ddop->add_device_process_data(
            "Section State",
            static_cast<uint16_t>(isobus::DataDescriptionIndex::ActualCondensedWorkState1_16),
            isobus::task_controller_object::ProcessDataProperties::MemberOfDefaultSet,
            isobus::task_controller_object::ProcessDataTriggertMethods::OnChange,
            productElement
        );
    }

    return true;
}
```

#### 4.1.5 UDP Protocol with RateController

New PGNs for gateway-RateController communication:

```cpp
// Protocol.hpp
#pragma once
#include <cstdint>

namespace aog::protocol {

// Gateway -> RateController PGNs
constexpr uint16_t PGN_ISOBUS_RATE     = 32600;  // Target rate from TC
constexpr uint16_t PGN_ISOBUS_SPEED    = 32604;  // Speed from ISOBUS
constexpr uint16_t PGN_ISOBUS_STATUS   = 32605;  // Gateway status

// RateController -> Gateway PGNs
constexpr uint16_t PGN_ACTUAL_RATE     = 32601;  // Actual rate to report
constexpr uint16_t PGN_SECTION_STATUS  = 32602;  // Section on/off status
constexpr uint16_t PGN_QUANTITY        = 32603;  // Accumulated quantity

// PGN 32600: ISOBUS Rate (Gateway -> RC)
// Sent when TC provides a new setpoint
struct PGN32600_IsobusRate {
    uint8_t headerLo = 0x88;        // 32600 & 0xFF
    uint8_t headerHi = 0x7F;        // 32600 >> 8
    uint8_t productId;              // 0-5
    uint8_t rateLo;                 // Rate * 1000, low byte
    uint8_t rateMid;                // Rate * 1000, mid byte
    uint8_t rateHi;                 // Rate * 1000, high byte
    uint8_t flags;                  // bit 0: rate valid
    uint8_t crc;
};

// PGN 32601: Actual Rate (RC -> Gateway)
// Sent periodically by RC to report to TC
struct PGN32601_ActualRate {
    uint8_t headerLo = 0x89;        // 32601 & 0xFF
    uint8_t headerHi = 0x7F;        // 32601 >> 8
    uint8_t productId;              // 0-5
    uint8_t rateLo;                 // Rate * 1000, low byte
    uint8_t rateMid;                // Rate * 1000, mid byte
    uint8_t rateHi;                 // Rate * 1000, high byte
    uint8_t quantityLo;             // Quantity * 10, low byte
    uint8_t quantityMid;            // Quantity * 10, mid byte
    uint8_t quantityHi;             // Quantity * 10, high byte
    uint8_t flags;                  // bit 0: section on
    uint8_t crc;
};

// PGN 32604: ISOBUS Speed (Gateway -> RC)
struct PGN32604_IsobusSpeed {
    uint8_t headerLo = 0x8C;        // 32604 & 0xFF
    uint8_t headerHi = 0x7F;        // 32604 >> 8
    uint8_t speedLo;                // Speed mm/s, low byte
    uint8_t speedMid;               // Speed mm/s, mid byte
    uint8_t speedHi;                // Speed mm/s, high byte
    uint8_t source;                 // 0=wheel, 1=ground, 2=machine selected
    uint8_t flags;                  // bit 0: speed valid
    uint8_t crc;
};

// PGN 32605: Gateway Status (Gateway -> RC)
struct PGN32605_GatewayStatus {
    uint8_t headerLo = 0x8D;        // 32605 & 0xFF
    uint8_t headerHi = 0x7F;        // 32605 >> 8
    uint8_t status;                 // bit 0: CAN connected
                                    // bit 1: address claimed
                                    // bit 2: TC connected
                                    // bit 3: VT connected
    uint8_t address;                // Claimed ISOBUS address
    uint8_t tcAddress;              // TC address (0xFF if not connected)
    uint8_t reserved[3];
    uint8_t crc;
};

} // namespace aog::protocol
```

#### 4.1.6 Configuration File

**Primary Configuration (Candlelight - Recommended):**

```json
{
    "can": {
        "driver": "Candle",
        "deviceIndex": 0,
        "channel": 0,
        "bitrate": 250000
    },
    "udp": {
        "listenPort": 28900,
        "sendPort": 28901,
        "rateControllerIP": "127.0.0.1"
    },
    "isobus": {
        "name": {
            "industryGroup": 2,
            "deviceClass": 4,
            "deviceClassInstance": 0,
            "function": 130,
            "functionInstance": 0,
            "manufacturerCode": 1407,
            "identityNumber": 1
        },
        "preferredAddress": 128
    },
    "products": {
        "count": 2,
        "reportIntervalMs": 100
    },
    "logging": {
        "level": "info",
        "file": "gateway.log"
    }
}
```

**Alternative Configuration (PCAN):**

```json
{
    "can": {
        "driver": "PCAN",
        "channel": "PCAN_USBBUS1",
        "bitrate": 250000
    }
}
```

**CAN Driver Selection Logic:**

```cpp
// In Gateway initialization
std::shared_ptr<isobus::CANHardwarePlugin> createCANPlugin(const Configuration& config) {
    std::string driver = config.getString("can.driver");

    if (driver == "Candle" || driver == "Candlelight") {
        // Recommended: Candlelight USB-CAN adapter
        return std::make_shared<isobus::CandleCANInterface>(
            config.getInt("can.deviceIndex", 0),
            config.getInt("can.channel", 0),
            config.getInt("can.bitrate", 250000)
        );
    }
    else if (driver == "PCAN") {
        // Alternative: PCAN-USB adapter
        return std::make_shared<isobus::PCANBasicInterface>(
            config.getString("can.channel", "PCAN_USBBUS1")
        );
    }

    LOG_ERROR("Unknown CAN driver: " + driver);
    return nullptr;
}
```

### 4.2 RateController Modifications

#### 4.2.1 New Classes

```
RateController/
├── Classes/
│   ├── IsobusGateway.cs           # NEW: Gateway communication
│   ├── IsobusProduct.cs           # NEW: ISOBUS rate per product
│   └── ... (existing)
├── PGNs/
│   ├── PGN32600.cs                # NEW: ISOBUS rate from gateway
│   ├── PGN32601.cs                # NEW: Actual rate to gateway
│   ├── PGN32604.cs                # NEW: ISOBUS speed
│   ├── PGN32605.cs                # NEW: Gateway status
│   └── ... (existing)
└── Forms/
    └── Menu/
        └── frmMenuIsobus.cs       # NEW: ISOBUS settings form
```

#### 4.2.2 IsobusGateway Class

```csharp
// IsobusGateway.cs
using System;
using System.Net;
using System.Net.Sockets;

namespace RateController
{
    public class IsobusGateway
    {
        private readonly FormStart mf;
        private UdpClient udpClient;
        private IPEndPoint gatewayEndpoint;

        // Configuration
        public bool Enabled { get; set; }
        public string GatewayIP { get; set; } = "127.0.0.1";
        public int ReceivePort { get; set; } = 28901;
        public int SendPort { get; set; } = 28900;

        // Status
        public bool GatewayConnected { get; private set; }
        public bool CANConnected { get; private set; }
        public bool TCConnected { get; private set; }
        public byte ClaimedAddress { get; private set; }

        // Rate source mode
        public IsobusRateMode RateMode { get; set; } = IsobusRateMode.Disabled;

        // ISOBUS rates per product
        public double[] IsobusRate { get; } = new double[6];
        public bool[] IsobusRateValid { get; } = new bool[6];

        // ISOBUS speed
        public double IsobusSpeed { get; private set; }
        public bool IsobusSpeedValid { get; private set; }

        public IsobusGateway(FormStart callingForm)
        {
            mf = callingForm;
        }

        public void Start()
        {
            if (!Enabled) return;

            try
            {
                udpClient = new UdpClient(ReceivePort);
                gatewayEndpoint = new IPEndPoint(IPAddress.Parse(GatewayIP), SendPort);
                udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusGateway/Start: " + ex.Message);
            }
        }

        public void Stop()
        {
            udpClient?.Close();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.EndReceive(ar, ref remoteEP);

                if (data.Length > 2)
                {
                    int pgn = data[0] | (data[1] << 8);

                    switch (pgn)
                    {
                        case 32600:
                            ParseIsobusRate(data);
                            break;
                        case 32604:
                            ParseIsobusSpeed(data);
                            break;
                        case 32605:
                            ParseGatewayStatus(data);
                            break;
                    }
                }

                // Continue receiving
                udpClient.BeginReceive(ReceiveCallback, null);
            }
            catch (ObjectDisposedException)
            {
                // Socket closed
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusGateway/Receive: " + ex.Message);
            }
        }

        private void ParseIsobusRate(byte[] data)
        {
            if (data.Length >= 8)
            {
                int productId = data[2];
                if (productId < 6)
                {
                    double rate = (data[3] | (data[4] << 8) | (data[5] << 16)) / 1000.0;
                    IsobusRate[productId] = rate;
                    IsobusRateValid[productId] = (data[6] & 0x01) == 0x01;
                }
            }
        }

        private void ParseIsobusSpeed(byte[] data)
        {
            if (data.Length >= 8)
            {
                double speed_mms = data[2] | (data[3] << 8) | (data[4] << 16);
                IsobusSpeed = speed_mms / 1000.0;  // Convert to m/s
                IsobusSpeedValid = (data[6] & 0x01) == 0x01;
            }
        }

        private void ParseGatewayStatus(byte[] data)
        {
            if (data.Length >= 8)
            {
                CANConnected = (data[2] & 0x01) == 0x01;
                bool addressClaimed = (data[2] & 0x02) == 0x02;
                TCConnected = (data[2] & 0x04) == 0x04;
                ClaimedAddress = data[3];

                GatewayConnected = CANConnected && addressClaimed;
            }
        }

        // Send actual rate to gateway for ISOBUS reporting
        public void SendActualRate(int productId, double rate, double quantity, bool sectionOn)
        {
            if (!Enabled || !GatewayConnected) return;

            byte[] data = new byte[11];
            data[0] = 0x89;  // PGN 32601 low
            data[1] = 0x7F;  // PGN 32601 high
            data[2] = (byte)productId;

            int rateInt = (int)(rate * 1000);
            data[3] = (byte)rateInt;
            data[4] = (byte)(rateInt >> 8);
            data[5] = (byte)(rateInt >> 16);

            int qtyInt = (int)(quantity * 10);
            data[6] = (byte)qtyInt;
            data[7] = (byte)(qtyInt >> 8);
            data[8] = (byte)(qtyInt >> 16);

            data[9] = (byte)(sectionOn ? 0x01 : 0x00);
            data[10] = mf.Tls.CRC(data, 10);

            try
            {
                udpClient.Send(data, data.Length, gatewayEndpoint);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("IsobusGateway/SendActualRate: " + ex.Message);
            }
        }
    }

    public enum IsobusRateMode
    {
        Disabled,           // ISOBUS rates ignored
        IsobusPriority,     // ISOBUS rate overrides RC rate
        RCPriority,         // RC rate used, ISOBUS reports only
        Manual              // User selects per-product
    }
}
```

#### 4.2.3 Product Class Modifications

```csharp
// In clsProduct.cs - add ISOBUS rate handling

public double TargetUPM()
{
    double result = 0;

    // Check ISOBUS rate source
    if (mf.IsobusGateway.Enabled &&
        mf.IsobusGateway.RateMode == IsobusRateMode.IsobusPriority &&
        mf.IsobusGateway.IsobusRateValid[ProductID])
    {
        // Use ISOBUS rate from Task Controller
        result = mf.IsobusGateway.IsobusRate[ProductID];
    }
    else
    {
        // Use existing rate calculation
        result = CalculateRateFromZones();  // existing logic
    }

    return result;
}

// In update loop, send actual rate to gateway
public void SendToIsobus()
{
    if (mf.IsobusGateway.Enabled)
    {
        mf.IsobusGateway.SendActualRate(
            ProductID,
            CurrentUPM,
            AccumulatedQuantity,
            SectionOn
        );
    }
}
```

### 4.3 Teensy 4.1 ISOBUS Extension

The Teensy 4.1 rate control module can be extended with direct ISOBUS connectivity using the **AgIsoStack-Arduino** library. This enables the module to:

- Receive speed data directly from the ISOBUS network
- Report actual rates to ISOBUS Task Controllers
- Receive section control commands from ISOBUS
- Act as an independent ISOBUS implement (without PC gateway)

#### 4.3.1 Overview

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Teensy 4.1 Module                               │
│                                                                             │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                        Dual Protocol Firmware                        │   │
│  │                                                                      │   │
│  │  ┌──────────────────┐              ┌──────────────────────────┐     │   │
│  │  │  Existing UDP    │              │   ISOBUS Layer           │     │   │
│  │  │  Communication   │              │   (AgIsoStack-Arduino)   │     │   │
│  │  │                  │              │                          │     │   │
│  │  │  - PGN 32400 TX  │              │  - Address Claiming      │     │   │
│  │  │  - PGN 32500 RX  │              │  - Speed Messages RX     │     │   │
│  │  │  - PGN 32401 TX  │              │  - Diagnostics TX        │     │   │
│  │  │                  │              │  - TC Client (optional)  │     │   │
│  │  └────────┬─────────┘              └────────────┬─────────────┘     │   │
│  │           │                                     │                    │   │
│  │  ┌────────┴─────────────────────────────────────┴─────────────┐     │   │
│  │  │                    Rate Control Core                        │     │   │
│  │  │                                                             │     │   │
│  │  │   - PWM Output          - Flow Sensor Input                 │     │   │
│  │  │   - Section Control     - Speed Calculation                 │     │   │
│  │  │   - PID Control         - Accumulated Quantity              │     │   │
│  │  └─────────────────────────────────────────────────────────────┘     │   │
│  └──────────────────────────────────────────────────────────────────────┘   │
│                                                                             │
│      Ethernet                    CAN1                     GPIO/PWM          │
│         │                          │                          │             │
└─────────┼──────────────────────────┼──────────────────────────┼─────────────┘
          │                          │                          │
          ▼                          ▼                          ▼
    ┌───────────┐            ┌─────────────┐            ┌─────────────┐
    │    UDP    │            │ SN65HVD230  │            │ Rate Control│
    │    to     │            │ Transceiver │            │  Hardware   │
    │RateControl│            │     │       │            │ (valves,    │
    │   App     │            │     ▼       │            │  sensors)   │
    └───────────┘            │  ISOBUS     │            └─────────────┘
                             │  Network    │
                             └─────────────┘
```

#### 4.3.2 Teensy 4.1 Hardware Specifications

| Feature | Teensy 4.1 Capability | ISOBUS Use |
|---------|----------------------|------------|
| **CAN Controllers** | 3x FlexCAN (CAN1, CAN2, CAN3) | CAN1 for ISOBUS |
| **CAN Pins** | CAN1: TX=22, RX=23 | Connect to transceiver |
| **Processor** | ARM Cortex-M7 @ 600MHz | Ample for dual protocol |
| **RAM** | 1MB (512KB tightly coupled) | ~50KB for AgIsoStack |
| **Flash** | 8MB (with QSPI chip) | ~200KB for AgIsoStack |
| **Ethernet** | 10/100 Mbps native | Existing UDP retained |

#### 4.3.3 Hardware Requirements

##### CAN Transceiver Selection

| Transceiver | Voltage | Features | Recommended |
|-------------|---------|----------|-------------|
| **SN65HVD230** | 3.3V | Basic, low cost | Yes - Primary |
| **MCP2562** | 5V tolerant | Robust, common | Yes |
| **ISO1050** | Isolated | Galvanic isolation | For harsh environments |
| **TCAN330** | 3.3V | Low power | Alternative |

##### Bill of Materials (ISOBUS Addition)

| Component | Quantity | Part Number | Notes |
|-----------|----------|-------------|-------|
| CAN Transceiver | 1 | SN65HVD230DR | 3.3V, SOIC-8 |
| Bypass Capacitor | 1 | 100nF 0603 | Near transceiver VCC |
| ESD Protection | 1 | PESD1CAN | Optional but recommended |
| ISOBUS Connector | 1 | Deutsch DT04-3P | Or breakout wires |
| Termination Resistor | 1 | 120Ω (optional) | If end of bus |

##### Wiring Diagram

```
                                    ISOBUS Network
                                         │
                           ┌─────────────┴─────────────┐
                           │     ISOBUS Connector      │
                           │    (Deutsch 9-pin or      │
                           │     breakout wires)       │
                           │                           │
                           │  Pin 3: Shield/GND ───────┼──┐
                           │  Pin 4: CAN_H ────────────┼──┼──┐
                           │  Pin 5: CAN_L ────────────┼──┼──┼──┐
                           └───────────────────────────┘  │  │  │
                                                          │  │  │
┌─────────────────────────────────────────────────────────┼──┼──┼──────────┐
│                        Teensy 4.1 Board                 │  │  │          │
│                                                         │  │  │          │
│   ┌─────────────────────────────────────────────────┐   │  │  │          │
│   │                   SN65HVD230                    │   │  │  │          │
│   │                                                 │   │  │  │          │
│   │   VCC ◄────── 3.3V (Teensy pin)                │   │  │  │          │
│   │   GND ◄────── GND  ◄────────────────────────────┼───┘  │  │          │
│   │                                                 │      │  │          │
│   │   TXD ◄────── Pin 22 (CTX1)                    │      │  │          │
│   │   RXD ──────► Pin 23 (CRX1)                    │      │  │          │
│   │                                                 │      │  │          │
│   │   CANH ─────────────────────────────────────────┼──────┘  │          │
│   │   CANL ─────────────────────────────────────────┼─────────┘          │
│   │                                                 │                    │
│   │   [100nF cap between VCC and GND]              │                    │
│   └─────────────────────────────────────────────────┘                    │
│                                                                          │
│   Existing connections (unchanged):                                      │
│   - Ethernet: Pins for W5500 or native                                  │
│   - PWM outputs: Rate control valves                                    │
│   - Digital inputs: Flow sensors                                        │
│   - Analog inputs: Pressure sensors                                     │
│                                                                          │
└──────────────────────────────────────────────────────────────────────────┘

Optional Termination (if Teensy is at end of ISOBUS):
    CAN_H ────┬──── 120Ω ────┬──── CAN_L
              │              │
         (at connector or on PCB)
```

#### 4.3.4 Software Architecture

##### Library Dependencies

```cpp
// platformio.ini or Arduino IDE Library Manager
// Required libraries:
// 1. AgIsoStack-Arduino (from Arduino Library Manager)
// 2. NativeEthernet (for Teensy 4.1 Ethernet)
// 3. FlexCAN_T4 (bundled with AgIsoStack-Arduino)
```

**PlatformIO Configuration:**

```ini
; platformio.ini
[env:teensy41]
platform = teensy
board = teensy41
framework = arduino

lib_deps =
    AgIsoStack-Arduino
    NativeEthernet

build_flags =
    -D ISOBUS_ENABLED=1
    -D CAN_CHANNEL=1
```

##### Firmware File Structure

```
RateModule_Teensy41/
├── RateModule_Teensy41.ino      # Main sketch
├── config.h                      # Configuration defines
├── rate_control.h/.cpp          # Existing rate control logic
├── udp_comm.h/.cpp              # Existing UDP communication
├── isobus_handler.h/.cpp        # NEW: ISOBUS integration
├── isobus_speed.h/.cpp          # NEW: Speed message handling
└── isobus_diagnostics.h/.cpp    # NEW: Diagnostic messages
```

##### Main Firmware Implementation

```cpp
//=============================================================================
// RateModule_Teensy41.ino
// Dual-protocol rate control module: UDP + ISOBUS
//=============================================================================

#include <NativeEthernet.h>
#include <AgIsoStack.hpp>
#include "config.h"
#include "rate_control.h"
#include "udp_comm.h"
#include "isobus_handler.h"

//=============================================================================
// Configuration
//=============================================================================

// Module identity
const uint8_t MODULE_ID = 0;        // 0-7, set via DIP switch or EEPROM
const uint8_t NUM_SENSORS = 2;      // Sensors on this module

// Network configuration (existing)
byte mac[] = { 0xDE, 0xAD, 0xBE, 0xEF, 0xFE, 0x00 };
IPAddress ip(192, 168, 1, 100);
IPAddress rcIP(192, 168, 1, 255);   // RateController broadcast

// ISOBUS configuration
const bool ISOBUS_ENABLED = true;
const uint8_t ISOBUS_PREFERRED_ADDRESS = 0x80;  // Starting address

//=============================================================================
// Global Objects
//=============================================================================

// UDP Communication (existing)
EthernetUDP udpRC;
UDPComm udpComm(udpRC, rcIP);

// Rate Control (existing)
RateControl rateControl[NUM_SENSORS];

// ISOBUS Handler (new)
#if ISOBUS_ENABLED
IsobusHandler isobusHandler(MODULE_ID, ISOBUS_PREFERRED_ADDRESS);
#endif

//=============================================================================
// Setup
//=============================================================================

void setup() {
    Serial.begin(115200);
    delay(1000);
    Serial.println("Rate Control Module Starting...");

    // Initialize Ethernet (existing)
    Ethernet.begin(mac, ip);
    udpRC.begin(28888);
    Serial.print("UDP initialized at ");
    Serial.println(Ethernet.localIP());

    // Initialize rate control hardware (existing)
    for (int i = 0; i < NUM_SENSORS; i++) {
        rateControl[i].begin(i);
    }

    // Initialize ISOBUS (new)
    #if ISOBUS_ENABLED
    if (isobusHandler.begin()) {
        Serial.println("ISOBUS initialized successfully");
    } else {
        Serial.println("ISOBUS initialization failed!");
    }
    #endif

    Serial.println("Setup complete");
}

//=============================================================================
// Main Loop
//=============================================================================

void loop() {
    static uint32_t lastUDPsend = 0;
    static uint32_t lastRateUpdate = 0;

    uint32_t now = millis();

    // === Process UDP Communication (existing, ~100Hz) ===
    udpComm.receive();  // Check for incoming PGN 32500

    // === Update Rate Control (existing, ~100Hz) ===
    if (now - lastRateUpdate >= 10) {
        lastRateUpdate = now;

        for (int i = 0; i < NUM_SENSORS; i++) {
            // Get speed - prefer ISOBUS if available
            float speed_mps;
            #if ISOBUS_ENABLED
            if (isobusHandler.hasValidSpeed()) {
                speed_mps = isobusHandler.getSpeed_mps();
            } else {
                speed_mps = udpComm.getSpeed_mps();
            }
            #else
            speed_mps = udpComm.getSpeed_mps();
            #endif

            // Update rate control
            rateControl[i].setSpeed(speed_mps);
            rateControl[i].update();
        }
    }

    // === Send UDP Status (existing, ~10Hz) ===
    if (now - lastUDPsend >= 100) {
        lastUDPsend = now;

        for (int i = 0; i < NUM_SENSORS; i++) {
            udpComm.sendPGN32400(
                MODULE_ID,
                i,  // sensor ID
                rateControl[i].getActualRate(),
                rateControl[i].getAccumulatedQty(),
                rateControl[i].getPWM(),
                rateControl[i].getSensorConnected(),
                rateControl[i].getHz()
            );
        }
    }

    // === Update ISOBUS (new) ===
    #if ISOBUS_ENABLED
    isobusHandler.update();

    // Report actual rates to ISOBUS (if TC connected)
    if (isobusHandler.isTaskControllerConnected()) {
        for (int i = 0; i < NUM_SENSORS; i++) {
            isobusHandler.reportActualRate(
                i,
                rateControl[i].getActualRate(),
                rateControl[i].getAccumulatedQty()
            );
        }
    }
    #endif
}
```

##### ISOBUS Handler Class

```cpp
//=============================================================================
// isobus_handler.h
//=============================================================================

#ifndef ISOBUS_HANDLER_H
#define ISOBUS_HANDLER_H

#include <AgIsoStack.hpp>

class IsobusHandler {
public:
    IsobusHandler(uint8_t moduleId, uint8_t preferredAddress);

    bool begin();
    void update();

    // Speed interface
    bool hasValidSpeed() const;
    float getSpeed_mps() const;
    float getSpeed_mmps() const;

    // Status
    bool isAddressClaimed() const;
    bool isTaskControllerConnected() const;
    uint8_t getClaimedAddress() const;

    // Rate reporting (to Task Controller)
    void reportActualRate(uint8_t sensorId, float rate, float quantity);
    void reportSectionState(uint8_t sensorId, bool isOn);

    // Diagnostics
    void setFault(uint32_t spn, uint8_t fmi, bool active);

private:
    void setupControlFunction();
    void setupSpeedInterface();
    void setupDiagnostics();

    // Callbacks
    static void onSpeedUpdate(
        const isobus::SpeedMessagesInterface::MachineSelectedSpeedData& data,
        void* parent
    );
    static void onWheelSpeedUpdate(
        const isobus::SpeedMessagesInterface::WheelBasedMachineSpeedData& data,
        void* parent
    );
    static void onGroundSpeedUpdate(
        const isobus::SpeedMessagesInterface::GroundBasedSpeedData& data,
        void* parent
    );

private:
    uint8_t m_moduleId;
    uint8_t m_preferredAddress;

    // AgIsoStack components
    std::shared_ptr<isobus::FlexCANT4Plugin> m_canPlugin;
    std::shared_ptr<isobus::InternalControlFunction> m_controlFunction;
    std::shared_ptr<isobus::SpeedMessagesInterface> m_speedInterface;
    std::shared_ptr<isobus::DiagnosticProtocol> m_diagnostics;

    // Speed data
    float m_machineSpeed_mmps;
    float m_wheelSpeed_mmps;
    float m_groundSpeed_mmps;
    bool m_speedValid;
    uint32_t m_lastSpeedUpdate;

    // Status
    bool m_initialized;
};

#endif // ISOBUS_HANDLER_H
```

```cpp
//=============================================================================
// isobus_handler.cpp
//=============================================================================

#include "isobus_handler.h"

using namespace isobus;

IsobusHandler::IsobusHandler(uint8_t moduleId, uint8_t preferredAddress)
    : m_moduleId(moduleId)
    , m_preferredAddress(preferredAddress)
    , m_machineSpeed_mmps(0)
    , m_wheelSpeed_mmps(0)
    , m_groundSpeed_mmps(0)
    , m_speedValid(false)
    , m_lastSpeedUpdate(0)
    , m_initialized(false)
{
}

bool IsobusHandler::begin() {
    // Create FlexCAN plugin for CAN1
    // Teensy 4.1 CAN1 uses pins 22 (TX) and 23 (RX)
    m_canPlugin = std::make_shared<FlexCANT4Plugin>(0);  // Channel 0 = CAN1

    // Initialize CAN hardware interface
    CANHardwareInterface::set_number_of_can_channels(1);
    CANHardwareInterface::assign_can_channel_frame_handler(0, m_canPlugin);

    if (!CANHardwareInterface::start()) {
        Serial.println("Failed to start CAN hardware");
        return false;
    }

    // Setup ISOBUS components
    setupControlFunction();
    setupSpeedInterface();
    setupDiagnostics();

    m_initialized = true;
    return true;
}

void IsobusHandler::setupControlFunction() {
    // Create device NAME (ISO 11783-5)
    NAME deviceName(0);

    // Identity
    deviceName.set_arbitrary_address_capable(true);
    deviceName.set_industry_group(2);                    // Agricultural
    deviceName.set_device_class(4);                      // Sprayers
    deviceName.set_device_class_instance(0);
    deviceName.set_function_code(130);                   // Rate Controller
    deviceName.set_function_instance(m_moduleId);        // Unique per module
    deviceName.set_manufacturer_code(1407);              // Open-Agriculture
    deviceName.set_identity_number(1000 + m_moduleId);   // Unique serial

    // Create internal control function (claims address on bus)
    m_controlFunction = CANNetworkManager::CANNetwork.create_internal_control_function(
        deviceName,
        0,                              // CAN channel
        m_preferredAddress + m_moduleId // Preferred address (0x80, 0x81, etc.)
    );

    Serial.print("ISOBUS NAME created, preferred address: 0x");
    Serial.println(m_preferredAddress + m_moduleId, HEX);
}

void IsobusHandler::setupSpeedInterface() {
    // Create speed message interface
    m_speedInterface = std::make_shared<SpeedMessagesInterface>(m_controlFunction);

    // Register callbacks for different speed sources
    m_speedInterface->get_machine_selected_speed_data_event_publisher()
        .add_listener(onSpeedUpdate, this);

    m_speedInterface->get_wheel_based_machine_speed_data_event_publisher()
        .add_listener(onWheelSpeedUpdate, this);

    m_speedInterface->get_ground_based_machine_speed_data_event_publisher()
        .add_listener(onGroundSpeedUpdate, this);

    m_speedInterface->initialize();

    Serial.println("Speed interface initialized");
}

void IsobusHandler::setupDiagnostics() {
    // Create diagnostic protocol handler
    m_diagnostics = std::make_shared<DiagnosticProtocol>(m_controlFunction);

    // Set product identification
    m_diagnostics->set_product_identification_brand("AOG");
    m_diagnostics->set_product_identification_model("RateModule");
    m_diagnostics->set_product_identification_serial_number(
        String(1000 + m_moduleId).c_str()
    );
    m_diagnostics->set_software_id_field(0, "1.0.0");

    m_diagnostics->initialize();

    Serial.println("Diagnostics initialized");
}

void IsobusHandler::update() {
    if (!m_initialized) return;

    // Update the CAN network manager (processes all ISOBUS traffic)
    CANNetworkManager::CANNetwork.update();

    // Update speed interface
    if (m_speedInterface) {
        m_speedInterface->update();
    }

    // Update diagnostics
    if (m_diagnostics) {
        m_diagnostics->update();
    }

    // Check speed validity (timeout after 1 second)
    if (millis() - m_lastSpeedUpdate > 1000) {
        m_speedValid = false;
    }
}

// Speed getters
bool IsobusHandler::hasValidSpeed() const {
    return m_speedValid;
}

float IsobusHandler::getSpeed_mps() const {
    return m_machineSpeed_mmps / 1000.0f;
}

float IsobusHandler::getSpeed_mmps() const {
    return m_machineSpeed_mmps;
}

// Status getters
bool IsobusHandler::isAddressClaimed() const {
    if (m_controlFunction) {
        return m_controlFunction->get_address_valid();
    }
    return false;
}

uint8_t IsobusHandler::getClaimedAddress() const {
    if (m_controlFunction) {
        return m_controlFunction->get_address();
    }
    return 0xFE;  // NULL address
}

bool IsobusHandler::isTaskControllerConnected() const {
    // Check if we've received TC messages recently
    // (Simplified - full implementation would track TC status)
    return isAddressClaimed();
}

// Rate reporting
void IsobusHandler::reportActualRate(uint8_t sensorId, float rate, float quantity) {
    // In full implementation, this would send Process Data to TC
    // For now, just update internal state
    // TC Client implementation would handle this via DDOP
}

void IsobusHandler::reportSectionState(uint8_t sensorId, bool isOn) {
    // Report section state to TC
}

// Diagnostics
void IsobusHandler::setFault(uint32_t spn, uint8_t fmi, bool active) {
    if (m_diagnostics) {
        if (active) {
            // Add DTC (Diagnostic Trouble Code)
            m_diagnostics->set_diagnostic_trouble_code(
                DiagnosticProtocol::DiagnosticTroubleCode(
                    spn,
                    static_cast<DiagnosticProtocol::FailureModeIdentifier>(fmi),
                    DiagnosticProtocol::LampStatus::None
                )
            );
        } else {
            // Clear DTC
            m_diagnostics->clear_diagnostic_trouble_code(
                DiagnosticProtocol::DiagnosticTroubleCode(
                    spn,
                    static_cast<DiagnosticProtocol::FailureModeIdentifier>(fmi),
                    DiagnosticProtocol::LampStatus::None
                )
            );
        }
    }
}

// Static callbacks
void IsobusHandler::onSpeedUpdate(
    const SpeedMessagesInterface::MachineSelectedSpeedData& data,
    void* parent
) {
    IsobusHandler* self = static_cast<IsobusHandler*>(parent);
    if (data.machineSelectedSpeed_mm_per_s_raw != 0xFFFF) {
        self->m_machineSpeed_mmps = data.machineSelectedSpeed_mm_per_s_raw;
        self->m_speedValid = true;
        self->m_lastSpeedUpdate = millis();

        Serial.print("ISOBUS Speed (MSS): ");
        Serial.print(self->m_machineSpeed_mmps / 1000.0f);
        Serial.println(" m/s");
    }
}

void IsobusHandler::onWheelSpeedUpdate(
    const SpeedMessagesInterface::WheelBasedMachineSpeedData& data,
    void* parent
) {
    IsobusHandler* self = static_cast<IsobusHandler*>(parent);
    if (data.wheelBasedMachineSpeed_mm_per_s_raw != 0xFFFF) {
        self->m_wheelSpeed_mmps = data.wheelBasedMachineSpeed_mm_per_s_raw;

        // Use wheel speed if no machine selected speed
        if (!self->m_speedValid) {
            self->m_machineSpeed_mmps = self->m_wheelSpeed_mmps;
            self->m_speedValid = true;
            self->m_lastSpeedUpdate = millis();
        }
    }
}

void IsobusHandler::onGroundSpeedUpdate(
    const SpeedMessagesInterface::GroundBasedSpeedData& data,
    void* parent
) {
    IsobusHandler* self = static_cast<IsobusHandler*>(parent);
    if (data.groundBasedMachineSpeed_mm_per_s_raw != 0xFFFF) {
        self->m_groundSpeed_mmps = data.groundBasedMachineSpeed_mm_per_s_raw;
    }
}
```

#### 4.3.5 Speed Source Priority

The Teensy module implements a speed source priority system:

```cpp
// Speed source selection (in rate_control.cpp)
float RateControl::getEffectiveSpeed() {
    // Priority 1: ISOBUS Machine Selected Speed
    if (isobusHandler.hasValidSpeed()) {
        return isobusHandler.getSpeed_mps();
    }

    // Priority 2: UDP speed from RateController (from GPS)
    if (udpComm.hasValidSpeed()) {
        return udpComm.getSpeed_mps();
    }

    // Priority 3: Wheel speed sensor (if equipped)
    if (wheelSensor.hasValidSpeed()) {
        return wheelSensor.getSpeed_mps();
    }

    // No valid speed source
    return 0.0f;
}
```

| Priority | Source | Description |
|----------|--------|-------------|
| 1 | ISOBUS MSS | Machine Selected Speed from tractor |
| 2 | UDP/GPS | Speed from RateController via AgOpenGPS |
| 3 | Wheel Sensor | Local wheel speed sensor (if equipped) |
| 4 | None | Zero speed (safety stop) |

#### 4.3.6 ISOBUS Features Supported on Teensy

| Feature | Support | Notes |
|---------|---------|-------|
| Address Claiming | ✅ Full | Automatic via AgIsoStack |
| Speed Messages (RX) | ✅ Full | MSS, WBS, GBS |
| Diagnostics (DM1/DM2) | ✅ Full | Fault reporting |
| Task Controller Client | ⚠️ Partial | Requires DDOP, more RAM |
| Virtual Terminal Client | ❌ Not recommended | RAM/complexity constraints |
| Guidance Messages | ✅ Full | If needed for steering |

#### 4.3.7 Memory Considerations

```
Teensy 4.1 Memory Usage (approximate):

Flash (8MB available):
├── Existing firmware:     ~150 KB
├── AgIsoStack-Arduino:    ~200 KB
├── NativeEthernet:        ~50 KB
└── Total:                 ~400 KB (5% of flash)

RAM (1MB available):
├── Existing variables:    ~20 KB
├── Ethernet buffers:      ~30 KB
├── AgIsoStack runtime:    ~50 KB
├── CAN buffers:           ~10 KB
└── Total:                 ~110 KB (11% of RAM)

✅ Ample resources for dual-protocol operation
```

#### 4.3.8 Teensy 4.1 ISOBUS Configuration Options

```cpp
// config.h - Teensy ISOBUS configuration

//=============================================================================
// ISOBUS Enable/Disable
//=============================================================================
#define ISOBUS_ENABLED          1       // Set to 0 to disable ISOBUS

//=============================================================================
// CAN Configuration
//=============================================================================
#define ISOBUS_CAN_CHANNEL      1       // CAN1=1, CAN2=2, CAN3=3
#define ISOBUS_BITRATE          250000  // Always 250kbps for ISOBUS

//=============================================================================
// ISOBUS Identity (ISO 11783-5 NAME)
//=============================================================================
#define ISOBUS_INDUSTRY_GROUP   2       // 2 = Agricultural
#define ISOBUS_DEVICE_CLASS     4       // 4 = Sprayers
#define ISOBUS_FUNCTION_CODE    130     // 130 = Rate Controller
#define ISOBUS_MANUFACTURER     1407    // Open-Agriculture

// Preferred starting address (modules will use BASE + MODULE_ID)
#define ISOBUS_PREFERRED_ADDR   0x80

//=============================================================================
// Speed Source Configuration
//=============================================================================
#define SPEED_USE_ISOBUS        1       // Use ISOBUS speed if available
#define SPEED_USE_UDP           1       // Use UDP speed as fallback
#define SPEED_USE_WHEEL_SENSOR  0       // Use local wheel sensor

// Speed timeout (ms) - revert to next source if no update
#define SPEED_TIMEOUT_MS        1000

//=============================================================================
// Feature Enables
//=============================================================================
#define ISOBUS_DIAGNOSTICS      1       // Enable DM1/DM2 messages
#define ISOBUS_SPEED_RX         1       // Receive speed from ISOBUS
#define ISOBUS_TC_CLIENT        0       // Task Controller (requires more RAM)
```

#### 4.3.9 Diagnostic Fault Codes

The module reports faults via ISOBUS diagnostic messages:

| SPN | FMI | Description | Condition |
|-----|-----|-------------|-----------|
| 1000 | 2 | Flow sensor error | No pulses when expected |
| 1001 | 3 | Pressure out of range | Sensor reading invalid |
| 1002 | 5 | PWM output fault | Actuator not responding |
| 1003 | 7 | Speed signal lost | No speed input |
| 1004 | 12 | Communication error | UDP timeout |

```cpp
// Example: Report flow sensor fault
if (flowSensorFailed) {
    isobusHandler.setFault(1000, 2, true);   // SPN 1000, FMI 2, active
} else {
    isobusHandler.setFault(1000, 2, false);  // Clear fault
}
```

---

## 5. Protocol Mappings

### 5.1 DDI to RateController Mapping

| DDI | ISO Name | RC Equivalent | Direction |
|-----|----------|---------------|-----------|
| 1 (0x0001) | Setpoint Volume Per Area | TargetUPM | TC → RC |
| 2 (0x0002) | Actual Volume Per Area | CurrentUPM | RC → TC |
| 6 (0x0006) | Setpoint Mass Per Area | TargetUPM (dry) | TC → RC |
| 7 (0x0007) | Actual Mass Per Area | CurrentUPM (dry) | RC → TC |
| 48 (0x0030) | Actual Work State | SectionOn | RC → TC |
| 72 (0x0048) | Total Volume | AccumulatedQuantity | RC → TC |
| 117 (0x0075) | Actual Condensed Work State | AllSectionsStatus | RC → TC |

### 5.2 Unit Conversions

| Parameter | RateController Unit | ISOBUS Unit | Conversion |
|-----------|--------------------|--------------| -----------|
| Liquid Rate | gal/ac or L/ha | mm³/m² | 1 L/ha = 100 mm³/m² |
| Dry Rate | lb/ac or kg/ha | mg/m² | 1 kg/ha = 100 mg/m² |
| Speed | mph or km/h | mm/s | 1 km/h = 277.78 mm/s |
| Area | ac or ha | m² | 1 ha = 10000 m² |
| Volume | gal or L | mL | 1 L = 1000 mL |

### 5.3 PGN Summary

| PGN | Name | Direction | Description |
|-----|------|-----------|-------------|
| 32400 | Sensor Data | Module → RC | Existing: rate, quantity, PWM |
| 32401 | Module Status | Module → RC | Existing: module health |
| 32500 | Rate Settings | RC → Module | Existing: target rate, commands |
| 32600 | ISOBUS Rate | Gateway → RC | **NEW**: TC prescription rate |
| 32601 | Actual Rate | RC → Gateway | **NEW**: Rate to report to TC |
| 32602 | Section Status | RC → Gateway | **NEW**: Section states |
| 32603 | Quantity | RC → Gateway | **NEW**: Accumulated totals |
| 32604 | ISOBUS Speed | Gateway → RC | **NEW**: Speed from ISOBUS |
| 32605 | Gateway Status | Gateway → RC | **NEW**: Connection status |

### 5.4 ISOBUS Module PGN Translation

For ISOBUS-based rate control modules, the Gateway translates between RC UDP PGNs and ISOBUS proprietary PGNs (0xFF00-0xFF0B range).

| CAN PGN | Direction | UDP PGN | Description |
|---------|-----------|---------|-------------|
| 0xFF00 | Module→GW | 32400 | Sensor rate/quantity |
| 0xFF01 | Module→GW | 32400 | Sensor PWM/Hz |
| 0xFF02 | Module→GW | 32401 | Module status |
| 0xFF03 | GW→Module | 32500 | Rate command |
| 0xFF04 | GW→Module | 32501 | Relay command |
| 0xFF05 | GW→Module | 32502 | PID settings part 1 |
| 0xFF06 | GW→Module | 32502 | PID settings part 2 |
| 0xFF07 | GW→Module | 32504 | Wheel speed config |
| 0xFF08 | Module→GW | 32401 | Module identification |
| 0xFF09 | Module→GW | 32401 | Wheel counts |
| 0xFF0A | GW→Module | 32500 | Flow calibration |
| 0xFF0B | GW→Module | 32502 | PID settings part 3 |

**Design Decisions:**
- PGN 0xFF08 bytes 4-5 marked reserved (hardware version = duplicate of module type; relay count unused)
- UDP PGN 32502 split into 3 CAN messages due to 8-byte CAN limit
- CAN messages 0xFF00+0xFF01 combined into single UDP PGN 32400

> **Detailed byte-level mapping:** See `IsobusGateway/docs/PGN_Mapping.md`

---

## 6. Configuration

### 6.1 RateController Settings

New settings in RateController for ISOBUS:

```csharp
// Props additions
public static bool IsobusEnabled { get; set; }
public static string IsobusGatewayIP { get; set; }
public static int IsobusGatewayPort { get; set; }
public static IsobusRateMode IsobusRateMode { get; set; }
public static bool IsobusSpeedEnabled { get; set; }
```

### 6.2 Settings UI (frmMenuIsobus)

```
┌─────────────────────────────────────────────────────────┐
│  ISOBUS Settings                                        │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  [ ] Enable ISOBUS Gateway                              │
│                                                         │
│  Gateway IP: [127.0.0.1    ]  Port: [28900]            │
│                                                         │
│  Rate Source:                                           │
│  ( ) Disabled - Ignore ISOBUS rates                     │
│  ( ) ISOBUS Priority - TC prescription overrides        │
│  (*) RC Priority - Report only, RC controls rate        │
│                                                         │
│  [ ] Use ISOBUS Speed (when available)                  │
│                                                         │
│  ─── Status ────────────────────────────────────────    │
│  Gateway: [Connected    ]  CAN: [OK]                    │
│  Address: [0x80]           TC:  [Connected]             │
│                                                         │
│           [Save]              [Cancel]                  │
└─────────────────────────────────────────────────────────┘
```

---

## 7. Implementation Phases

### Phase 1: Gateway Foundation ✓ COMPLETE

**Deliverables:**
- [x] Gateway project structure with CMake
- [x] Link against local AgIsoStack++ (`G:\Sync\Documents\GitHub\SK21\AgIsoStack-plus-plus`)
- [x] CAN hardware integration (Candlelight primary, SocketCAN for Linux)
- [x] Basic address claiming
- [x] UDP communication framework
- [x] Configuration file loading (JSON)
- [x] Basic logging
- [x] Bidirectional PGN translation (RC UDP ↔ ISOBUS proprietary)
- [x] Speed message reception (MSS, WBS, GBS) with priority

**Implementation Notes:**
- Gateway source: `IsobusGateway/src/Gateway.cpp`
- PGN structures: `IsobusGateway/include/IsobusGateway/RateControllerPGN.hpp`
- UDP protocol: `IsobusGateway/include/IsobusGateway/Protocol.hpp`
- See `IsobusGateway/docs/PGN_Mapping.md` for detailed byte-level mapping

**Key Implementation: CAN Hardware Setup**

```cpp
// main.cpp - Gateway initialization
#include "isobus/hardware_integration/candle_can_interface.hpp"
#include "isobus/hardware_integration/can_hardware_interface.hpp"
#include "isobus/isobus/can_network_manager.hpp"

int main() {
    // Create Candlelight CAN interface (device 0, channel 0, 250kbps)
    auto canDriver = std::make_shared<isobus::CandleCANInterface>(0, 0, 250000);

    // Initialize hardware interface
    isobus::CANHardwareInterface::set_number_of_can_channels(1);
    isobus::CANHardwareInterface::assign_can_channel_frame_handler(0, canDriver);

    // Start CAN communication
    if (!isobus::CANHardwareInterface::start()) {
        std::cerr << "Failed to start CAN interface" << std::endl;
        return 1;
    }

    // Main loop
    while (running) {
        isobus::CANNetworkManager::CANNetwork.update();
        // ... UDP processing
    }

    isobus::CANHardwareInterface::stop();
    return 0;
}
```

**CMakeLists.txt Integration:**

```cmake
cmake_minimum_required(VERSION 3.16)
project(IsobusGateway)

# Path to local AgIsoStack++
set(AGISOSTACK_PATH "G:/Sync/Documents/GitHub/SK21/AgIsoStack-plus-plus")

# Add AgIsoStack++ as subdirectory
add_subdirectory(${AGISOSTACK_PATH} ${CMAKE_BINARY_DIR}/agisostack)

# Gateway executable
add_executable(IsobusGateway
    src/main.cpp
    src/Gateway.cpp
    src/UDPBridge.cpp
    # ...
)

# Link against AgIsoStack++ libraries
target_link_libraries(IsobusGateway
    PRIVATE
        isobus::Isobus
        isobus::HardwareIntegration
)

# Enable Candlelight support (Windows)
target_compile_definitions(IsobusGateway PRIVATE USE_CANDLE_CAN)
```

**Acceptance Criteria:**
- Gateway starts and claims address on ISOBUS via Candlelight adapter
- UDP messages sent/received with RateController
- Configuration loaded from JSON file
- Graceful handling of CAN adapter disconnect/reconnect

### Phase 2: Gateway TC Server Protocol

**Purpose:** Gateway acts as TC Server proxy on behalf of RateController, communicating
with Teensy TC Clients via standard ISOBUS Task Controller protocol.

**Deliverables:**
- [ ] TC Server implementation in Gateway (AgIsoStack++)
- [ ] Handle TC Client connections (Teensy modules)
- [ ] DDOP reception and parsing from TC Clients
- [ ] Process Data value commands (setpoints to clients)
- [ ] Process Data value responses (actuals from clients)
- [ ] Forward setpoints/actuals between RC (UDP) and Teensy (CAN)

**TC Protocol Messages:**
| Message | Direction | Purpose |
|---------|-----------|---------|
| Working Set Master | GW → Teensy | Announce TC presence |
| Request Object Pool | GW → Teensy | Request DDOP from client |
| Object Pool Transfer | Teensy → GW | DDOP upload |
| Task Start/Stop | GW → Teensy | Begin/end task |
| Process Data Command | GW → Teensy | Send setpoint values |
| Process Data Value | Teensy → GW | Report actual values |

**Acceptance Criteria:**
- Gateway discovers Teensy TC Clients on ISOBUS
- Gateway receives DDOP from each client
- Setpoints from RateController reach Teensy via TC protocol
- Actuals from Teensy reach RateController via TC protocol

### Phase 3: RateController Integration ✓ MOSTLY COMPLETE

**Actual Implementation:**
- [x] `IsobusComm` class in `RateController/Classes/IsobusComm.cs`
- [x] PGN handlers for 32600, 32604, 32605 (in IsobusComm, not separate files)
- [x] Gateway process management (starts/stops IsobusGateway.exe)
- [x] UDP communication on ports 32700/32701
- [x] Speed mode switching (`SpeedType.ISOBUS` in Props.cs)
- [x] Settings in `frmMenuOptions.cs` (checkbox + radio button)
- [x] `Props.IsobusEnabled` persisted
- [ ] Status display on main form (properties exist but NOT displayed)
- [ ] `SendActualRate()` implemented but never called

**Implementation Notes:**
- Gateway launched as external process, not embedded library
- Settings are minimal (no dedicated ISOBUS form) - just "ISOBUS Comm" checkbox and speed source radio
- Status properties exist: `GatewayConnected`, `CANConnected`, `AddressClaimed`, `TCAddress`, etc.
- ISOBUS speed integrated into `Props.Speed_KMH` with fallback to GPS

**Remaining Work:**
- [ ] Display gateway status somewhere in UI (optional)
- [ ] Call `SendActualRate()` to report rates to TC
- [ ] Handle TC setpoint rates in `ParseRateMessage()`

### Phase 4: Speed & Diagnostics - PARTIAL

**Completed:**
- [x] Speed message reception in gateway (MSS, WBS, GBS)
- [x] Speed forwarding to RateController (PGN 32604)
- [x] ISOBUS speed as selectable source in Props

**Remaining:**
- [ ] DM1/DM2 diagnostic support in gateway
- [ ] Error reporting to ISOBUS
- [ ] Section state reporting to TC

### Phase 5: Testing & Refinement (Week 9-10)

**Deliverables:**
- [ ] Integration testing with real TC
- [ ] Performance optimization
- [ ] Documentation
- [ ] Error handling improvements
- [ ] User guide

**Acceptance Criteria:**
- System works with commercial TC
- No performance degradation
- Documentation complete

### Phase 6: Teensy 4.1 TC Client Implementation ⚠️ ESSENTIAL

**Purpose:** Teensy modules become ISOBUS Task Controller Clients, receiving setpoints
from and reporting actuals to RateController (via Gateway). This replaces/augments
the existing UDP communication for ISOBUS-equipped installations.

**Deliverables:**
- [ ] Add CAN transceiver hardware (SN65HVD230) to Teensy module
- [ ] Integrate AgIsoStack-Arduino library
- [ ] Implement TC Client with DDOP
- [ ] Create Device Description Object Pool (DDOP) for rate module
- [ ] Handle Process Data commands (receive setpoints)
- [ ] Report Process Data values (send actuals)
- [ ] Add ISOBUS speed reception as alternative source
- [ ] Add diagnostic message support (DM1/DM2)

**DDOP Structure for Rate Module:**

```
Device (Teensy Rate Module)
├── DeviceElement: Module (type: device)
│   ├── DeviceProcessData: Setpoint Rate (DDI 1, input)
│   ├── DeviceProcessData: Actual Rate (DDI 2, output)
│   ├── DeviceProcessData: Total Quantity (DDI 72, output)
│   └── DeviceProcessData: Section State (DDI 48, output)
└── DeviceElement: Section 1..N (type: section)
    ├── DeviceProcessData: Section Setpoint (DDI 1, input)
    └── DeviceProcessData: Section Actual (DDI 2, output)
```

**Key Implementation Steps:**

```
Week 11: Hardware & TC Client Foundation
├── Add SN65HVD230 transceiver to Teensy PCB
├── Install AgIsoStack-Arduino library
├── Implement address claiming
├── Create basic DDOP structure
└── Test TC Client connection to Gateway

Week 12: Process Data Integration
├── Receive setpoint commands from TC (Gateway)
├── Map DDI values to rate control variables
├── Report actual rates via Process Data
├── Report accumulated quantities
└── Test bidirectional data flow

Week 13: Speed & Diagnostics
├── Add speed message reception (MSS, WBS, GBS)
├── Implement speed source priority (ISOBUS vs wheel sensor)
├── Add diagnostic protocol (DM1/DM2)
├── Define fault codes (SPN/FMI) for rate control errors
└── Test fault reporting

Week 14: Integration Testing
├── Full system test: RC → Gateway → Teensy → Actuator
├── Verify rate control accuracy with ISOBUS setpoints
├── Test section control via TC protocol
├── Verify backwards compatibility with UDP mode
└── Document configuration and wiring
```

**TC Client Messages (Teensy ↔ Gateway):**

| Message | Direction | Purpose |
|---------|-----------|---------|
| Working Set Master Msg | Teensy → GW | Announce presence |
| Object Pool Transfer | Teensy → GW | Send DDOP |
| Object Pool Activate | GW → Teensy | Activate DDOP |
| Process Data Command | GW → Teensy | Receive setpoints |
| Process Data Value | Teensy → GW | Report actuals |
| Task Start/Stop | GW → Teensy | Begin/end logging |

**Acceptance Criteria:**
- Teensy claims address and registers as TC Client
- Gateway receives and parses Teensy DDOP
- Setpoints from RateController control Teensy rate output
- Actual rates reported back to RateController
- Speed from ISOBUS usable as alternative source
- Faults visible on Gateway/RateController
- RAM usage <150KB total

---

## 8. Hardware Requirements

### 8.1 PC Gateway Hardware

#### 8.1.1 CAN Interface Options

| Interface | Cost | Platform | Recommendation |
|-----------|------|----------|----------------|
| **Candlelight/CANable** | $15-50 | Windows | **Primary - Recommended** |
| PCAN-USB | $250+ | Windows/Linux | Alternative - Commercial |
| SocketCAN | N/A | Linux only | Linux users |

#### 8.1.2 Recommended: Candlelight USB-CAN Adapters

These adapters use open-source Candlelight/gs_usb firmware and are directly supported by the local AgIsoStack++ codebase:

| Adapter | Price | Features | Purchase |
|---------|-------|----------|----------|
| **CANable Pro** | ~$40 | Isolated, reliable | canable.io |
| **CANable 2.0** | ~$25 | USB-C, CAN FD ready | canable.io |
| **Generic gs_usb** | ~$15 | Budget option | AliExpress/Amazon |
| **USBtin** | ~$25 | Well documented | fischl.de |
| **CANtact** | ~$30 | Original reference | linklayer.com |

**Requirements for Candlelight adapters:**
- Windows 10/11 (WinUSB driver built-in)
- No additional driver installation needed
- USB 2.0 or higher port

**Verifying Candlelight Compatibility:**

Check Windows Device Manager for "gs_usb" or "CANable" under "Universal Serial Bus devices" (not COM ports).

#### 8.1.3 Alternative: PCAN-USB

| Component | Part Number | Notes |
|-----------|-------------|-------|
| PCAN-USB | IPEH-002021 | Basic non-isolated |
| PCAN-USB Pro | IPEH-002022 | Isolated, 2-channel |

Requires PCAN-Basic driver installation from Peak Systems.

#### 8.1.4 ISOBUS Connection

| Component | Options | Notes |
|-----------|---------|-------|
| ISOBUS Cable | Standard 9-pin Deutsch | Tractor implement connector |
| Termination | 120Ω if end of bus | May be built into connector |

#### 8.1.5 Wiring Diagram (Candlelight USB Adapter)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                              Windows PC                                  │
│                                                                         │
│    ┌─────────────────────┐                                              │
│    │   ISOBUS Gateway    │                                              │
│    │   (AgIsoStack++)    │                                              │
│    └──────────┬──────────┘                                              │
│               │                                                         │
└───────────────┼─────────────────────────────────────────────────────────┘
                │ USB
                │
┌───────────────┴─────────────────┐
│   Candlelight USB-CAN Adapter   │
│   (CANable, CANtact, etc.)      │
│                                 │
│   ┌─────────────────────────┐   │
│   │  STM32 + Candlelight FW │   │
│   │  (gs_usb compatible)    │   │
│   └───────────┬─────────────┘   │
│               │                 │
│   ┌───────────┴─────────────┐   │
│   │   CAN Transceiver       │   │
│   │   (built-in)            │   │
│   └───────────┬─────────────┘   │
│            CAN_H CAN_L          │
└───────────────┬─────────────────┘
                │
                │  ┌─────────────────────────────┐
                │  │  ISOBUS Breakout Cable      │
                │  │                             │
                │  │  CAN_H ──── Yellow/Green    │
                │  │  CAN_L ──── Yellow/Orange   │
                │  │  GND ────── Black           │
                │  └─────────────────────────────┘
                │
┌───────────────┴──────────────────────────────────────────────────────────┐
│                         ISOBUS / Tractor CAN                             │
│                                                                          │
│   Pin 4 (CAN_H) ────────────────────────────────────────────────────    │
│   Pin 5 (CAN_L) ────────────────────────────────────────────────────    │
│   Pin 3 (GND)   ────────────────────────────────────────────────────    │
│                                                                          │
│   ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐         │
│   │ Tractor  │    │   Task   │    │  Virtual │    │  Other   │         │
│   │   ECU    │    │Controller│    │ Terminal │    │Implements│         │
│   └──────────┘    └──────────┘    └──────────┘    └──────────┘         │
│                                                                          │
│                           120Ω                     120Ω                  │
│                        Termination              Termination              │
│                        (if needed)              (if needed)              │
└──────────────────────────────────────────────────────────────────────────┘
```

**Connection Notes:**

1. **USB Connection**: Standard USB-A or USB-C depending on adapter
2. **CAN Wiring**: Only 3 wires needed (CAN_H, CAN_L, GND)
3. **Termination**: Most ISOBUS networks are already terminated. Only add 120Ω if the gateway is at the physical end of the bus.
4. **Power**: Candlelight adapters are USB-powered, no external power needed
5. **Isolation**: For production use, consider isolated adapters (CANable Pro) to protect PC

### 8.2 Teensy 4.1 ISOBUS Hardware

#### 8.2.1 Teensy 4.1 CAN Capabilities

| Feature | Specification |
|---------|---------------|
| CAN Controllers | 3x FlexCAN (CAN1, CAN2, CAN3) |
| CAN Standard | CAN 2.0B (11-bit and 29-bit IDs) |
| Max Bitrate | 1 Mbps (ISOBUS uses 250 kbps) |
| CAN1 Pins | TX: Pin 22, RX: Pin 23 |
| CAN2 Pins | TX: Pin 1, RX: Pin 0 |
| CAN3 Pins | TX: Pin 31, RX: Pin 30 |

**Recommended: Use CAN1** (pins 22/23) for ISOBUS - leaves other CAN channels available.

#### 8.2.2 Bill of Materials (ISOBUS Addition to Teensy Module)

| Component | Qty | Part Number | Description | Cost |
|-----------|-----|-------------|-------------|------|
| **CAN Transceiver** | 1 | SN65HVD230DR | 3.3V CAN transceiver, SOIC-8 | $2 |
| Bypass Capacitor | 1 | 100nF 0603 | Decoupling for transceiver | $0.01 |
| ESD Protection | 1 | PESD1CAN | TVS diode for CAN bus | $0.50 |
| Header Pins | 2 | Standard 0.1" | For CAN_H, CAN_L connection | $0.10 |
| **Total additional cost** | | | | **~$3** |

**Optional Components:**

| Component | Part Number | Purpose | Cost |
|-----------|-------------|---------|------|
| Isolated Transceiver | ISO1050DUB | Galvanic isolation | $8 |
| ISOBUS Connector | Deutsch DT04-3P | Standard implement plug | $15 |
| Termination Resistor | 120Ω 1/4W | If end of bus | $0.05 |

#### 8.2.3 CAN Transceiver Options

| Transceiver | VCC | Features | Recommendation |
|-------------|-----|----------|----------------|
| **SN65HVD230** | 3.3V | Basic, common, low cost | **Primary choice** |
| MCP2562 | 5V | 5V tolerant I/O, robust | Alternative |
| TCAN330 | 3.3V | Very low power | Battery applications |
| ISO1050 | 3.3V-5V | Isolated, 2.5kV | Harsh environments |

#### 8.2.4 Schematic

```
                                          To ISOBUS Network
                                               │
┌──────────────────────────────────────────────┼────────────────────┐
│                     Teensy 4.1               │                    │
│                                              │                    │
│                                     ┌────────┴────────┐           │
│                                     │   ISOBUS Conn   │           │
│                                     │  or Terminals   │           │
│    ┌─────────────────────────┐      │                 │           │
│    │       SN65HVD230        │      │  CAN_H ─────────┤           │
│    │                         │      │  CAN_L ─────────┤           │
│    │  1 TXD ◄──── Pin 22     │      │  GND ───────────┤           │
│    │  2 GND ────── GND       │      └─────────────────┘           │
│    │  3 VCC ────── 3.3V      │              │                     │
│    │        │                │              │                     │
│    │      ──┴── 100nF        │       ┌──────┴──────┐              │
│    │        │                │       │ Optional:   │              │
│    │       GND               │       │ 120Ω term   │              │
│    │                         │       │ (if end of  │              │
│    │  4 RXD ────► Pin 23     │       │  bus)       │              │
│    │  5 CANL ────────────────┼───────┴─────────────┘              │
│    │  6 CANH ────────────────┼───────┬─────────────               │
│    │  7 NC                   │       │                            │
│    │  8 Rs ─────── GND       │       │                            │
│    │     (slope control)     │    ┌──┴──┐                         │
│    └─────────────────────────┘    │PESD1│ ESD                     │
│                                   │ CAN │ Protection              │
│                                   └──┬──┘ (optional)              │
│                                      │                            │
│                                     GND                           │
│                                                                   │
│    Existing connections (unchanged):                              │
│    ├── Ethernet: W5500 or native Teensy Ethernet                 │
│    ├── PWM: Rate control valve outputs                           │
│    ├── Digital In: Flow sensor pulses                            │
│    └── Analog In: Pressure sensors                               │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

#### 8.2.5 PCB Layout Recommendations

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│    Best practices for CAN transceiver placement:                │
│                                                                 │
│    1. Place transceiver close to Teensy (minimize trace length) │
│    2. Keep CAN_H and CAN_L traces parallel and close together   │
│    3. Use ground plane under CAN traces                         │
│    4. Place 100nF capacitor within 5mm of transceiver VCC pin  │
│    5. Route CAN traces away from high-frequency signals         │
│    6. ESD protection should be at connector, not transceiver    │
│                                                                 │
│    Trace recommendations:                                       │
│    - CAN_H/CAN_L: 0.3mm width minimum                          │
│    - Differential impedance: ~120Ω (matched to bus)            │
│    - Keep trace length < 50mm from transceiver to connector    │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

#### 8.2.6 Multiple Modules on Same ISOBUS

When multiple Teensy modules share the same ISOBUS network:

```
ISOBUS Bus (CAN_H / CAN_L)
    │
    ├───────────────────────────────────────────────────────────┐
    │                                                           │
┌───┴───┐         ┌───────┐         ┌───────┐         ┌───────┐│
│Tractor│         │Teensy │         │Teensy │         │Teensy ││
│  ECU  │         │Module0│         │Module1│         │Module2││
│       │         │Addr:80│         │Addr:81│         │Addr:82││
└───┬───┘         └───┬───┘         └───┬───┘         └───┬───┘│
    │                 │                 │                 │    │
   120Ω              (no term)         (no term)         120Ω │
    │                                                     │    │
    └─────────────────────────────────────────────────────┘    │
                                                               │
    Notes:                                                     │
    - Each module uses unique address (0x80 + MODULE_ID)       │
    - Only endpoints need 120Ω termination                     │
    - All modules share CAN_H and CAN_L (parallel connection) │
    - Maximum 30 nodes per ISOBUS segment                      │
    - Maximum bus length ~40m at 250kbps                       │
                                                               │
```

### 8.3 ISOBUS Connector Pinout

Standard ISOBUS (ISO 11783) connector pinout:

```
┌─────────────────────────────────────────────────────────────────┐
│                                                                 │
│   Implement Connector (DT04-12PA or similar)                   │
│                                                                 │
│   ┌─────────────────────────────────────────┐                  │
│   │                                         │                  │
│   │   1: TBC+ (Tractor Bus Power +12V)      │  Not used by     │
│   │   2: TBC- (Tractor Bus Power GND)       │  rate control    │
│   │   3: ECU_GND (Signal Ground) ◄──────────┼── Connect        │
│   │   4: CAN_H (High) ◄─────────────────────┼── Connect        │
│   │   5: CAN_L (Low)  ◄─────────────────────┼── Connect        │
│   │   6: TBC+ (alternate)                   │  Not used        │
│   │   7: Reserved                           │                  │
│   │   8: Reserved                           │                  │
│   │   9: Reserved                           │                  │
│   │                                         │                  │
│   └─────────────────────────────────────────┘                  │
│                                                                 │
│   Minimum connections for ISOBUS:                              │
│   - Pin 3 (ECU_GND) → Teensy GND                              │
│   - Pin 4 (CAN_H)   → Transceiver CANH                        │
│   - Pin 5 (CAN_L)   → Transceiver CANL                        │
│                                                                 │
│   Note: Do NOT connect TBC power pins to Teensy!              │
│   Teensy is powered via USB or separate supply.                │
│                                                                 │
└─────────────────────────────────────────────────────────────────┘
```

---

## 9. Testing Strategy

### 9.1 Unit Testing

| Component | Test Cases |
|-----------|------------|
| DDOP Creation | Valid structure, correct DDIs, proper hierarchy |
| UDP Protocol | Message encoding/decoding, CRC validation |
| Configuration | Load/save, validation, defaults |

### 9.2 Integration Testing

| Test | Description | Expected Result |
|------|-------------|-----------------|
| Address Claim | Connect gateway to ISOBUS | Address claimed successfully |
| TC Connection | Connect to Task Controller | DDOP uploaded, connected state |
| Rate Reception | TC sends setpoint | RC receives via gateway |
| Rate Reporting | RC sends actual rate | TC displays correct value |
| Speed Forward | ISOBUS speed message | RC receives speed |

### 9.3 System Testing

| Scenario | Steps | Verification |
|----------|-------|--------------|
| Full VRA Operation | Load prescription, run field | Rates match prescription |
| Failover | Disconnect gateway | RC continues with internal rates |
| Reconnection | Restore gateway | Communication resumes |
| Multi-product | Run 2+ products with TC | All rates independent |

### 9.4 Test Equipment

- ISOBUS simulator (Vector CANoe or similar)
- Commercial Task Controller (John Deere, Trimble, etc.)
- ISOBUS VT for diagnostics display
- CAN bus analyzer

---

## Appendix A: Reference PGN Structures

### A.1 Existing PGN 32400 (Sensor Data)

```
Byte 0:  Header Lo (144)
Byte 1:  Header Hi (126)
Byte 2:  Module/Sensor ID
Byte 3:  Rate Applied Lo (×1000)
Byte 4:  Rate Applied Mid
Byte 5:  Rate Applied Hi
Byte 6:  Acc. Quantity Lo (×10)
Byte 7:  Acc. Quantity Mid
Byte 8:  Acc. Quantity Hi
Byte 9:  PWM Lo
Byte 10: PWM Hi
Byte 11: Status (bit 0: sensor connected)
Byte 12: Hz Lo (×10)
Byte 13: Hz Hi
Byte 14: CRC
```

### A.2 Existing PGN 32500 (Rate Settings)

```
Byte 0:  Header Lo (244)
Byte 1:  Header Hi (126)
Byte 2:  Module/Sensor ID
Byte 3:  Rate Set Lo (×1000)
Byte 4:  Rate Set Mid
Byte 5:  Rate Set Hi
Byte 6:  Flow Cal Lo (×1000)
Byte 7:  Flow Cal Mid
Byte 8:  Flow Cal Hi
Byte 9:  Command byte
Byte 10: Manual PWM Lo
Byte 11: Manual PWM Hi
Byte 12: Reserved
Byte 13: CRC
```

---

## Appendix B: ISOBUS NAME Structure

```
Bits 0-20:   Identity Number (21 bits)
Bits 21-25:  Manufacturer Code (11 bits) - 1407 = Open Agriculture
Bits 26-28:  Device Class Instance (3 bits)
Bits 29-35:  Device Class (7 bits) - 4 = Sprayers
Bit 36:      Reserved
Bits 37-44:  Function (8 bits) - 130 = Rate Controller
Bits 45-49:  Function Instance (5 bits)
Bits 50-52:  ECU Instance (3 bits)
Bits 53-55:  Industry Group (3 bits) - 2 = Agricultural
Bit 56:      Arbitrary Address Capable
```

---

## Appendix C: Candlelight CAN API Reference

The Candlelight support in AgIsoStack++ is located at:
`G:\Sync\Documents\GitHub\SK21\AgIsoStack-plus-plus\hardware_integration\`

### C.1 Core API Functions

```cpp
// Device enumeration
bool candle_list_scan(candle_list_handle* list);
bool candle_list_length(candle_list_handle list, uint8_t* len);
bool candle_list_free(candle_list_handle list);

// Device access
bool candle_dev_get(candle_list_handle list, uint8_t index, candle_handle* hdev);
bool candle_dev_get_state(candle_handle hdev, candle_devstate_t* state);
bool candle_dev_open(candle_handle hdev);
bool candle_dev_close(candle_handle hdev);
bool candle_dev_free(candle_handle hdev);
candle_err_t candle_dev_last_error(candle_handle hdev);

// Channel configuration
bool candle_channel_count(candle_handle hdev, uint8_t* count);
bool candle_channel_set_bitrate(candle_handle hdev, uint8_t ch, uint32_t bitrate);
bool candle_channel_start(candle_handle hdev, uint8_t ch, candle_mode_t mode);
bool candle_channel_stop(candle_handle hdev, uint8_t ch);

// Frame I/O
bool candle_frame_send(candle_handle hdev, uint8_t ch, candle_frame_t* frame);
bool candle_frame_read(candle_handle hdev, candle_frame_t* frame, uint32_t timeout_ms);
```

### C.2 Device States

```cpp
typedef enum {
    CANDLE_DEVSTATE_AVAIL,   // Device available for use
    CANDLE_DEVSTATE_INUSE    // Device already in use by another process
} candle_devstate_t;
```

### C.3 CAN Frame Structure

```cpp
typedef struct {
    uint32_t echo_id;    // Echo ID (for TX confirmation)
    uint32_t can_id;     // CAN identifier (with flags)
    uint8_t can_dlc;     // Data length code (0-8)
    uint8_t channel;     // Channel number
    uint8_t flags;       // Frame flags
    uint8_t reserved;
    uint8_t data[8];     // CAN data bytes
    uint32_t timestamp;  // Hardware timestamp
} candle_frame_t;

// CAN ID flags (OR with can_id)
#define CANDLE_ID_EXTENDED  0x80000000  // Extended (29-bit) ID
#define CANDLE_ID_RTR       0x40000000  // Remote transmission request
#define CANDLE_ID_ERR       0x20000000  // Error frame
```

### C.4 Channel Modes

```cpp
typedef enum {
    CANDLE_MODE_NORMAL        = 0x00,  // Normal operation
    CANDLE_MODE_LISTEN_ONLY   = 0x01,  // Listen only (no ACK)
    CANDLE_MODE_LOOP_BACK     = 0x02,  // Internal loopback
    CANDLE_MODE_TRIPLE_SAMPLE = 0x04,  // Triple sampling
    CANDLE_MODE_ONE_SHOT      = 0x08,  // No automatic retransmission
    CANDLE_MODE_HW_TIMESTAMP  = 0x10,  // Hardware timestamps
} candle_mode_t;
```

### C.5 Error Codes

| Code | Name | Description |
|------|------|-------------|
| 0 | `CANDLE_ERR_OK` | Success |
| 1 | `CANDLE_ERR_CREATE_FILE` | Failed to open USB device handle |
| 2 | `CANDLE_ERR_WINUSB_INITIALIZE` | WinUSB initialization failed |
| 13 | `CANDLE_ERR_BITRATE_UNSUPPORTED` | Bitrate not supported by device |
| 14 | `CANDLE_ERR_SEND_FRAME` | Failed to transmit CAN frame |
| 15 | `CANDLE_ERR_READ_TIMEOUT` | Read timeout (no message available) |
| 27 | `CANDLE_ERR_DEV_OUT_OF_RANGE` | Invalid device index |

### C.6 Troubleshooting

| Issue | Solution |
|-------|----------|
| Device not found | Check Device Manager for "gs_usb" device |
| Device in use | Close other CAN applications |
| Bitrate error | Use standard ISOBUS bitrate (250000) |
| No messages received | Check CAN wiring, verify bus termination |
| Send failures | Check bus load, verify other devices responding |

---

## Appendix D: Source Code Locations

### D.1 AgIsoStack++ (Local Repository)

```
G:\Sync\Documents\GitHub\SK21\AgIsoStack-plus-plus\
├── isobus/                          # Core ISOBUS stack
│   ├── include/isobus/isobus/
│   │   ├── can_network_manager.hpp
│   │   ├── isobus_task_controller_client.hpp
│   │   ├── isobus_speed_distance_messages.hpp
│   │   └── isobus_diagnostic_protocol.hpp
│   └── src/
│
├── hardware_integration/            # CAN hardware drivers
│   ├── include/isobus/hardware_integration/
│   │   ├── candle_can_interface.hpp      ◄── Primary CAN interface
│   │   ├── can_hardware_interface.hpp
│   │   └── candle/
│   │       ├── candle.h                  ◄── Low-level Candle API
│   │       ├── candle_defs.h
│   │       └── candle_ctrl_req.h
│   └── src/
│       ├── candle_can_interface.cpp      ◄── Interface implementation
│       └── candle/
│           ├── candle.cpp                ◄── USB communication
│           └── candle_ctrl_req.cpp
│
└── CMakeLists.txt
```

### D.2 RateController (This Repository)

```
G:\Sync\Documents\GitHub\SK21\AOG_RC\RateAppSource\
├── RateController/
│   ├── Classes/
│   │   ├── UDPcomm.cs               # Existing UDP communication
│   │   └── IsobusGateway.cs         # NEW: Gateway interface
│   ├── PGNs/
│   │   ├── PGN32400.cs              # Existing sensor data
│   │   ├── PGN32500.cs              # Existing rate settings
│   │   └── PGN32600.cs              # NEW: ISOBUS rate from gateway
│   └── Forms/
│       └── Menu/
│           └── frmMenuIsobus.cs     # NEW: ISOBUS settings
│
└── docs/
    └── ISOBUS_Integration_Design.md  # This document
```

### D.3 Teensy 4.1 Module Firmware (New Files)

```
RateModule_Teensy41/
├── RateModule_Teensy41.ino          # Main sketch (modified)
├── config.h                          # Configuration defines (modified)
├── rate_control.h                    # Existing rate control
├── rate_control.cpp
├── udp_comm.h                        # Existing UDP communication
├── udp_comm.cpp
├── isobus_handler.h                  # NEW: ISOBUS integration
├── isobus_handler.cpp
└── platformio.ini                    # PlatformIO config (if using)
```

---

## Appendix E: AgIsoStack-Arduino Reference

### E.1 Library Information

| Property | Value |
|----------|-------|
| Name | AgIsoStack-Arduino |
| Version | 0.1.5+ |
| Repository | https://github.com/Open-Agriculture/AgIsoStack-Arduino |
| License | MIT |
| Supported Boards | Teensy 4.x (Teensy 4.0, 4.1) |
| Main Header | `<AgIsoStack.hpp>` |

### E.2 Installation

**Arduino IDE:**
1. Open Library Manager (Sketch → Include Library → Manage Libraries)
2. Search for "AgIsoStack"
3. Click Install

**PlatformIO:**
```ini
[env:teensy41]
platform = teensy
board = teensy41
framework = arduino
lib_deps = AgIsoStack-Arduino
```

### E.3 Key Classes

| Class | Purpose | Header |
|-------|---------|--------|
| `FlexCANT4Plugin` | Teensy 4.x CAN hardware driver | `flex_can_t4_plugin.hpp` |
| `CANHardwareInterface` | Hardware abstraction layer | `can_hardware_interface.hpp` |
| `CANNetworkManager` | Network/protocol management | `can_network_manager.hpp` |
| `NAME` | ISO 11783 device identity | `can_NAME.hpp` |
| `InternalControlFunction` | Local ECU representation | `can_internal_control_function.hpp` |
| `SpeedMessagesInterface` | Speed message handling | `isobus_speed_distance_messages.hpp` |
| `DiagnosticProtocol` | DM1/DM2 diagnostics | `isobus_diagnostic_protocol.hpp` |
| `TaskControllerClient` | TC client (VRA) | `isobus_task_controller_client.hpp` |

### E.4 FlexCAN T4 Plugin

The `FlexCANT4Plugin` class interfaces AgIsoStack with Teensy 4.x native CAN:

```cpp
#include <AgIsoStack.hpp>

// Create plugin for CAN1 (channel 0)
// Teensy 4.1: CAN1=0, CAN2=1, CAN3=2
auto canPlugin = std::make_shared<isobus::FlexCANT4Plugin>(0);

// Initialize hardware
isobus::CANHardwareInterface::set_number_of_can_channels(1);
isobus::CANHardwareInterface::assign_can_channel_frame_handler(0, canPlugin);
isobus::CANHardwareInterface::start();
```

**Channel Mapping:**

| FlexCAN Channel | Teensy 4.1 Pins | Constructor Arg |
|-----------------|-----------------|-----------------|
| CAN1 | TX: 22, RX: 23 | `FlexCANT4Plugin(0)` |
| CAN2 | TX: 1, RX: 0 | `FlexCANT4Plugin(1)` |
| CAN3 | TX: 31, RX: 30 | `FlexCANT4Plugin(2)` |

### E.5 Speed Message Interface

Receiving ISOBUS speed messages:

```cpp
// Create speed interface
auto speedInterface = std::make_shared<isobus::SpeedMessagesInterface>(
    myControlFunction  // or nullptr for receive-only
);

// Register callbacks
speedInterface->get_machine_selected_speed_data_event_publisher()
    .add_listener([](const auto& data, void*) {
        if (data.machineSelectedSpeed_mm_per_s_raw != 0xFFFF) {
            float speed_mps = data.machineSelectedSpeed_mm_per_s_raw / 1000.0f;
            Serial.print("Speed: ");
            Serial.println(speed_mps);
        }
    }, nullptr);

speedInterface->initialize();

// In loop()
speedInterface->update();
```

**Speed Data Structures:**

| Structure | PGN | Description |
|-----------|-----|-------------|
| `MachineSelectedSpeedData` | 0xF022 | Preferred speed source |
| `WheelBasedMachineSpeedData` | 0xFE48 | Wheel encoder speed |
| `GroundBasedSpeedData` | 0xFE49 | Radar/GPS ground speed |

### E.6 Diagnostic Protocol

Reporting faults via DM1 messages:

```cpp
// Create diagnostic handler
auto diagnostics = std::make_shared<isobus::DiagnosticProtocol>(myControlFunction);

// Set product info
diagnostics->set_product_identification_brand("AOG");
diagnostics->set_product_identification_model("RateModule");
diagnostics->set_software_id_field(0, "1.0.0");

diagnostics->initialize();

// Report a fault (DM1)
diagnostics->set_diagnostic_trouble_code(
    isobus::DiagnosticProtocol::DiagnosticTroubleCode(
        1000,  // SPN (Suspect Parameter Number)
        isobus::DiagnosticProtocol::FailureModeIdentifier::DataErratic,
        isobus::DiagnosticProtocol::LampStatus::None
    )
);

// Clear a fault
diagnostics->clear_diagnostic_trouble_code(/* same DTC */);
```

**Failure Mode Identifiers (FMI):**

| FMI | Description | Typical Use |
|-----|-------------|-------------|
| 0 | Data valid but above normal | Over-range |
| 1 | Data valid but below normal | Under-range |
| 2 | Data erratic/intermittent | Noise/glitch |
| 3 | Voltage above normal | Sensor fault |
| 4 | Voltage below normal | Sensor fault |
| 5 | Current below normal | Open circuit |
| 7 | Mechanical system not responding | Actuator stuck |
| 12 | Bad intelligent device | Module fault |

### E.7 Memory Usage (Teensy 4.1)

| Component | Flash | RAM |
|-----------|-------|-----|
| AgIsoStack core | ~150 KB | ~30 KB |
| FlexCAN driver | ~20 KB | ~10 KB |
| Speed interface | ~10 KB | ~2 KB |
| Diagnostics | ~15 KB | ~3 KB |
| **Total** | **~195 KB** | **~45 KB** |

Teensy 4.1 has 8MB flash and 1MB RAM - plenty of headroom.

### E.8 Troubleshooting

| Issue | Possible Cause | Solution |
|-------|----------------|----------|
| No address claimed | CAN not initialized | Check `CANHardwareInterface::start()` |
| No speed messages | Wrong CAN channel | Verify channel matches wiring |
| Intermittent comms | Missing termination | Add 120Ω if at bus end |
| High error rate | Wrong bitrate | Must be 250000 for ISOBUS |
| Diagnostics not visible | Not initialized | Call `diagnostics->initialize()` |

---

## Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | Jan 2026 | - | Initial draft |
| 1.1 | Jan 2026 | - | Added Candlelight USB-CAN support from local AgIsoStack++ |
|     |          |   | Added CAN hardware interface documentation |
|     |          |   | Updated hardware requirements with Candlelight options |
|     |          |   | Added wiring diagrams for Candlelight adapter |
|     |          |   | Added Appendix C: Candlelight API Reference |
|     |          |   | Added Appendix D: Source Code Locations |
| 1.2 | Jan 2026 | - | Added comprehensive Teensy 4.1 ISOBUS design |
|     |          |   | Added IsobusHandler class implementation |
|     |          |   | Added speed source priority system |
|     |          |   | Added Teensy hardware BOM and schematics |
|     |          |   | Added Phase 6: Teensy implementation |
|     |          |   | Added Appendix E: AgIsoStack-Arduino Reference |
|     |          |   | Updated hardware section with Teensy details |
| 1.3 | Jan 2026 | - | Phase 1 marked complete - Gateway implementation done |
|     |          |   | Added Section 5.4: ISOBUS Module PGN Translation |
|     |          |   | Documented bidirectional PGN translation (RC UDP ↔ ISOBUS) |
|     |          |   | PGN 0xFF08 bytes 4-5 marked reserved (hardwareVersion, relayCount) |
|     |          |   | Referenced IsobusGateway/docs/PGN_Mapping.md for byte-level details |
| 1.4 | Jan 2026 | - | Restructured Phase 3 to reflect actual implementation |
|     |          |   | IsobusComm class documented (not IsobusGateway as planned) |
|     |          |   | Noted: settings in frmMenuOptions, no dedicated form |
|     |          |   | Noted: SendActualRate() exists but unused |
|     |          |   | Noted: status properties exist but not displayed in UI |
|     |          |   | Phase 4 split into completed (speed) and remaining (diagnostics) |
| 1.5 | Jan 2026 | - | **Major architecture clarification:** |
|     |          |   | - RateController is Task Controller (not external TC) |
|     |          |   | - Teensy modules are TC Clients (essential, not optional) |
|     |          |   | - Gateway is TC Server proxy bridging RC ↔ Teensy |
|     |          |   | Rewrote Section 3 architecture diagrams |
|     |          |   | Phase 2 renamed: "Gateway TC Server Protocol" |
|     |          |   | Phase 6 marked ESSENTIAL, added TC Client/DDOP details |
|     |          |   | Added DDOP structure and TC protocol message tables |
