# Task Controller Server Design Document

## Overview

This document describes the architecture for implementing RateController as an ISO 11783-10 compliant Task Controller (TC) Server, using the IsobusGateway as a protocol translator.

**Architecture: Option A - Gateway as Translator**

```
PHASES 1-6: Gateway translates for proprietary Teensy
══════════════════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────────────────┐
│                              RateController (C#)                            │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │  Products   │  │   Sections  │  │  Implement  │  │   Prescription      │ │
│  │  (Rate Ctrl)│  │   Control   │  │  Manager    │  │   Manager           │ │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────────────┘ │
│                              │                                              │
│                      IsobusComm (UDP)                                       │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │ UDP Port 32700/32701
                               │ (Extended Protocol - see Section 4)
                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           IsobusGateway (C++)                               │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                    AgIsoStack++ TC Server                            │   │
│  │  ┌──────────────┐  ┌──────────────┐  ┌────────────────────────────┐ │   │
│  │  │ Working Set  │  │    DDOP      │  │     Process Data           │ │   │
│  │  │   Manager    │  │   Parser     │  │     Handler                │ │   │
│  │  └──────────────┘  └──────────────┘  └────────────────────────────┘ │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────────────────────────────┐   │
│  │                      Protocol Translator                             │   │
│  │  - RC UDP ↔ ISOBUS Standard                                         │   │
│  │  - Teensy Proprietary ↔ ISOBUS Standard (Phases 1-6)                │   │
│  └─────────────────────────────────────────────────────────────────────┘   │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │ CAN Bus (250 kbps)
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
       ┌────────────┐   ┌────────────┐   ┌────────────┐
       │   Teensy   │   │  External  │   │  External  │
       │  Modules   │   │ Implement  │   │ Implement  │
       │(Proprietary│   │(TC Client) │   │(TC Client) │
       └────────────┘   └────────────┘   └────────────┘


PHASE 7: Full ISOBUS Compliance (End Goal)
══════════════════════════════════════════

┌─────────────────────────────────────────────────────────────────────────────┐
│                              RateController (C#)                            │
│                         (Implement Manager + UI)                            │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │ UDP Port 32700/32701
                               ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                           IsobusGateway (C++)                               │
│                      AgIsoStack++ TC Server                                 │
│              (Standard ISOBUS only - no translation needed)                 │
└──────────────────────────────┼──────────────────────────────────────────────┘
                               │ CAN Bus (250 kbps) - Standard ISOBUS
                               │
              ┌────────────────┼────────────────┐
              │                │                │
              ▼                ▼                ▼
       ┌────────────┐   ┌────────────┐   ┌────────────┐
       │   Teensy   │   │  External  │   │  External  │
       │  Modules   │   │ Implement  │   │ Implement  │
       │(TC Client) │   │(TC Client) │   │(TC Client) │
       └────────────┘   └────────────┘   └────────────┘
              │
              └── All devices speak standard ISOBUS
```

## 1. ISO 11783-10 Task Controller Overview

### 1.1 TC Server Responsibilities

The Task Controller Server (TC Server) is responsible for:

1. **Working Set Management** - Detecting and managing connected implements
2. **DDOP Processing** - Parsing Device Description Object Pools from implements
3. **Task Management** - Sending prescriptions (variable rate maps) to implements
4. **Data Logging** - Recording actual application data from implements
5. **Section Control** - Managing boom section on/off states
6. **Rate Control** - Sending target rates based on position and prescription

### 1.2 TC Client Responsibilities

TC Clients (implements) are responsible for:

1. **Announcing Working Set** - Broadcasting presence on the bus
2. **Uploading DDOP** - Describing device capabilities (booms, sections, products)
3. **Receiving Commands** - Processing rate setpoints and section states
4. **Reporting Actuals** - Sending measured rates, quantities, states
5. **Position Independent Control** - Executing TC commands

### 1.3 Key ISOBUS Concepts

| Concept | Description |
|---------|-------------|
| **Working Set** | A group of ECUs that work together (e.g., rate controller + valve driver) |
| **DDOP** | Device Description Object Pool - XML-like structure describing implement |
| **DDI** | Data Dictionary Identifier - Standardized data element IDs |
| **DET** | Device Element Type - Boom, section, bin, etc. |
| **Process Data** | Runtime values exchanged between TC and implement |

## 2. Data Dictionary Identifiers (DDIs)

### 2.1 Commonly Used DDIs for Rate Control

| DDI | Name | Units | Description |
|-----|------|-------|-------------|
| 1 | Setpoint Volume Per Area | mm³/m² | Target liquid rate |
| 2 | Actual Volume Per Area | mm³/m² | Measured liquid rate |
| 6 | Setpoint Mass Per Area | mg/m² | Target granular rate |
| 7 | Actual Mass Per Area | mg/m² | Measured granular rate |
| 11 | Setpoint Volume Per Time | mm³/s | Target volume flow |
| 12 | Actual Volume Per Time | mm³/s | Measured volume flow |
| 21 | Setpoint Mass Per Time | mg/s | Target mass flow |
| 22 | Actual Mass Per Time | mg/s | Measured mass flow |
| 116 | Setpoint Count Per Area | /m² | Target seeds per area |
| 117 | Actual Count Per Area | /m² | Measured seeds per area |
| 141 | Prescription Control State | - | Enable/disable prescription |
| 157 | Section Control State | bitmap | Section on/off states |
| 158 | Actual Condensed Work State | bitmap | Section working states |

### 2.2 DDI Unit Conversions

```
Volume Rate:  1 L/ha = 100 mm³/m²
Mass Rate:    1 kg/ha = 100,000 mg/m² = 100 g/m²
Count Rate:   1 seeds/m² = 1 /m²
```

### 2.3 DDI to RC Product Mapping

| RC Product Type | Primary DDI | Units Conversion |
|-----------------|-------------|------------------|
| Liquid (L/ha) | DDI 1, 2 | × 100 |
| Granular (kg/ha) | DDI 6, 7 | × 100,000 |
| NH3 (kg/ha) | DDI 6, 7 | × 100,000 |
| Seeds (/ha) | DDI 116, 117 | ÷ 10,000 |

## 3. Gateway TC Server Architecture

### 3.1 AgIsoStack++ TC Server Components

AgIsoStack++ provides TC Server functionality through:

```cpp
// Key classes from AgIsoStack++
isobus::TaskControllerServer  // Main TC Server class
isobus::DeviceDescriptorObjectPool  // DDOP parser
isobus::ProcessDataValue  // Process data handling
```

### 3.2 Gateway TC Server Module

```cpp
// New gateway component: TCServerManager
class TCServerManager {
public:
    // Lifecycle
    void initialize(std::shared_ptr<isobus::InternalControlFunction> cf);
    void update();  // Called from main loop

    // Working Set Management
    void onWorkingSetConnected(WorkingSetInfo info);
    void onWorkingSetDisconnected(uint8_t address);
    std::vector<WorkingSetInfo> getConnectedWorkingSets();

    // DDOP Processing
    void onDDOPReceived(uint8_t address, const DeviceDescriptorObjectPool& ddop);
    ImplementCapabilities parseImplementCapabilities(const DDOP& ddop);

    // Process Data
    void sendSetpointRate(uint8_t element, uint16_t ddi, int32_t value);
    void sendSectionCommand(uint8_t element, uint16_t sectionStates);
    void onActualValueReceived(uint8_t element, uint16_t ddi, int32_t value);

    // RC Communication
    void setRCConnection(UDPBridge* bridge);
    void forwardImplementInfoToRC(const WorkingSetInfo& ws);
    void forwardActualDataToRC(uint8_t address, ProcessDataMessage msg);

private:
    std::shared_ptr<isobus::TaskControllerServer> tcServer;
    std::map<uint8_t, WorkingSetInfo> workingSets;
    std::map<uint8_t, ImplementCapabilities> implementCaps;
    UDPBridge* rcBridge;
};
```

### 3.3 Working Set Information

```cpp
struct WorkingSetInfo {
    uint8_t masterAddress;           // ISOBUS address of Working Set Master
    uint64_t name;                   // ISO NAME (64-bit identifier)
    std::string manufacturer;        // From NAME
    std::string designation;         // From DDOP
    std::vector<uint8_t> memberAddresses;  // Working Set Members
    bool ddopReceived;
    bool active;
};

struct ImplementCapabilities {
    uint8_t numberOfBooms;
    uint8_t numberOfSections;        // Per boom
    uint8_t numberOfBins;            // Product tanks
    std::vector<uint16_t> supportedDDIs;  // What data the implement can handle
    bool supportsVariableRate;
    bool supportsSectionControl;
    uint16_t boomWidth_mm;
    std::vector<uint16_t> sectionWidths_mm;
};
```

## 4. RC ↔ Gateway Protocol Extension

### 4.1 New PGN Definitions

Extend the existing UDP protocol with TC-specific messages:

| PGN | Direction | Size | Description |
|-----|-----------|------|-------------|
| 32610 | GW → RC | var | Implement Connected |
| 32611 | GW → RC | 8 | Implement Disconnected |
| 32612 | GW → RC | var | Implement Capabilities |
| 32613 | GW → RC | 12 | Actual Process Data |
| 32614 | RC → GW | 12 | Setpoint Command |
| 32615 | RC → GW | 8 | Section Command |
| 32616 | RC → GW | var | Task Data (prescription) |
| 32617 | GW → RC | 8 | TC Status |

### 4.2 PGN 32610 - Implement Connected

Sent when a TC Client (implement) connects and uploads DDOP.

```
Byte 0-1:  PGN (0x7F62)
Byte 2:    Implement Address (ISOBUS address)
Byte 3:    Implement Index (0-based, assigned by gateway)
Byte 4-11: ISO NAME (64-bit)
Byte 12-N: Designation string (null-terminated)
```

### 4.3 PGN 32611 - Implement Disconnected

```
Byte 0-1:  PGN (0x7F63)
Byte 2:    Implement Address
Byte 3:    Implement Index
Byte 4:    Reason (0=normal, 1=timeout, 2=error)
Byte 5-7:  Reserved
```

### 4.4 PGN 32612 - Implement Capabilities

Sent after DDOP is parsed, describes implement structure.

```
Byte 0-1:  PGN (0x7F64)
Byte 2:    Implement Index
Byte 3:    Number of Booms
Byte 4:    Number of Sections (total)
Byte 5:    Number of Bins
Byte 6-7:  Boom Width (mm, uint16)
Byte 8:    Capabilities Flags
           Bit 0: Supports Variable Rate
           Bit 1: Supports Section Control
           Bit 2: Supports Prescription
           Bit 3-7: Reserved
Byte 9-N:  Section widths (uint16 each, mm)
```

### 4.5 PGN 32613 - Actual Process Data

Forwarded from implement to RC, contains measured values.

```
Byte 0-1:  PGN (0x7F65)
Byte 2:    Implement Index
Byte 3:    Element Number (boom/section index)
Byte 4-5:  DDI (uint16)
Byte 6-9:  Value (int32, in DDI units)
Byte 10:   Status (0=valid, 1=error, 2=not available)
Byte 11:   Reserved
```

### 4.6 PGN 32614 - Setpoint Command

RC sends target rate to implement via gateway.

```
Byte 0-1:  PGN (0x7F66)
Byte 2:    Implement Index
Byte 3:    Element Number
Byte 4-5:  DDI (uint16)
Byte 6-9:  Setpoint Value (int32, in DDI units)
Byte 10:   Command Flags
           Bit 0: Enable
           Bit 1-7: Reserved
Byte 11:   Reserved
```

### 4.7 PGN 32615 - Section Command

RC sends section on/off states to implement.

```
Byte 0-1:  PGN (0x7F67)
Byte 2:    Implement Index
Byte 3:    Boom Number
Byte 4-7:  Section States (uint32 bitmap, bit 0 = section 1)
```

### 4.8 PGN 32617 - TC Status

Gateway sends TC Server status to RC.

```
Byte 0-1:  PGN (0x7F69)
Byte 2:    TC Server State
           0 = Idle
           1 = Active
           2 = Error
Byte 3:    Connected Implement Count
Byte 4:    Active Implement Count (receiving commands)
Byte 5:    Error Code (0 = none)
Byte 6-7:  Reserved
```

## 5. RC Implementation Changes

### 5.1 New Classes

```csharp
// RateController/Classes/ImplementManager.cs
public class ImplementManager
{
    public List<IsobusImplement> Implements { get; }

    public void OnImplementConnected(byte[] data);
    public void OnImplementDisconnected(byte[] data);
    public void OnCapabilitiesReceived(byte[] data);
    public void OnActualDataReceived(byte[] data);

    public void SendSetpoint(int implementIndex, int element, ushort ddi, int value);
    public void SendSectionCommand(int implementIndex, int boom, uint sectionStates);
}

// RateController/Classes/IsobusImplement.cs
public class IsobusImplement
{
    public byte Address { get; set; }
    public byte Index { get; set; }
    public ulong IsoName { get; set; }
    public string Designation { get; set; }

    public byte NumberOfBooms { get; set; }
    public byte NumberOfSections { get; set; }
    public byte NumberOfBins { get; set; }
    public ushort BoomWidth_mm { get; set; }
    public List<ushort> SectionWidths_mm { get; set; }

    public bool SupportsVariableRate { get; set; }
    public bool SupportsSectionControl { get; set; }

    public bool IsConnected { get; set; }
    public DateTime LastMessageTime { get; set; }
}
```

### 5.2 IsobusComm Extensions

```csharp
// Add to IsobusComm.cs
public event Action<IsobusImplement> ImplementConnected;
public event Action<byte> ImplementDisconnected;
public event Action<byte, ImplementCapabilities> CapabilitiesReceived;
public event Action<byte, ProcessDataMessage> ActualDataReceived;

public void SendSetpointCommand(byte implementIndex, byte element, ushort ddi, int value);
public void SendSectionCommand(byte implementIndex, byte boom, uint sectionStates);
```

### 5.3 Integration with Existing Products

The existing `clsProduct` class can be extended to support ISOBUS implements:

```csharp
public class clsProduct
{
    // Existing properties...

    // New ISOBUS properties
    public bool UseIsobusImplement { get; set; }
    public byte IsobusImplementIndex { get; set; }
    public byte IsobusElementNumber { get; set; }  // Boom/section in implement
    public ushort IsobusDDI { get; set; }          // DDI for this product type

    // Convert RC rate to DDI units
    public int RateToDDIUnits(double rate);
    public double DDIUnitsToRate(int ddiValue);
}
```

## 6. Teensy Module Compatibility

### 6.1 Approach: Gateway Translation

Existing Teensy modules continue using proprietary messages. The gateway:

1. Treats Teensy modules as "internal" devices
2. Translates proprietary PGNs to/from standard ISOBUS when needed
3. Reports Teensy modules to RC as a special implement type

### 6.2 Teensy as TC Client (Phase 7)

Teensy firmware will be upgraded to proper TC Client for full ISOBUS compliance:

```cpp
// Teensy TC Client implementation requires:
// - ISO NAME assignment (unique 64-bit identifier)
// - DDOP generation (describe sections, sensors, capabilities)
// - Standard Process Data messages (DDI-based)
// - Working Set announcement
// - Address claim procedure
```

**Benefits of Teensy TC Client:**
- Teensy modules work with any ISOBUS Task Controller (not just RC)
- Standard diagnostic messages (DM1/DM2)
- Interoperability with other ISOBUS equipment
- No proprietary protocol dependency

**Implementation approach:**
- AgIsoStack++ has Arduino/Teensy support via `isobus::CANHardwareInterface`
- FlexCAN_T4 can be wrapped as AgIsoStack hardware plugin
- DDOP can be generated at compile time or runtime based on module config

## 7. Implementation Phases

### Phase 1: Gateway TC Server Foundation
**Estimated scope: Gateway changes only**

- [ ] Enable AgIsoStack++ TC Server in gateway
- [ ] Implement Working Set detection callbacks
- [ ] Basic DDOP reception (no parsing yet)
- [ ] Forward connection events to RC via new PGNs (32610, 32611)
- [ ] Add TC Status PGN (32617)

**Deliverable:** Gateway reports when implements connect/disconnect

### Phase 2: DDOP Parsing & Capabilities
**Estimated scope: Gateway + RC changes**

- [ ] Parse DDOP to extract implement structure
- [ ] Generate ImplementCapabilities from DDOP
- [ ] Send capabilities to RC via PGN 32612
- [ ] RC: Add ImplementManager class
- [ ] RC: Display connected implements in UI

**Deliverable:** RC shows connected implement details (sections, booms, etc.)

### Phase 3: Process Data - Receive Actuals
**Estimated scope: Gateway + RC changes**

- [ ] Gateway: Handle incoming Process Data from implements
- [ ] Gateway: Forward actual values to RC via PGN 32613
- [ ] RC: Receive and display actual rates from ISOBUS implements
- [ ] RC: Map ISOBUS implement to Product for display

**Deliverable:** RC displays actual rates from external ISOBUS implements

### Phase 4: Process Data - Send Setpoints
**Estimated scope: Gateway + RC changes**

- [ ] RC: Send setpoint commands via PGN 32614
- [ ] Gateway: Translate to ISOBUS Process Data setpoint
- [ ] RC: Send section commands via PGN 32615
- [ ] Gateway: Translate to ISOBUS section control
- [ ] Test with simulator or real implement

**Deliverable:** RC can control external ISOBUS implements

### Phase 5: Prescription/Task Data (Future)
**Estimated scope: Major feature**

- [ ] Define task data format (PGN 32616)
- [ ] RC: Export prescription maps in TC format
- [ ] Gateway: Handle task activation
- [ ] Gateway: Send position-based rates from prescription

**Deliverable:** Variable rate from prescription maps via ISOBUS

### Phase 6: Integration & Testing
**Estimated scope: Testing + refinement**

- [ ] Test with real ISOBUS implements
- [ ] Handle edge cases (reconnection, errors)
- [ ] Performance optimization
- [ ] Documentation

**Deliverable:** Stable TC Server with external implement support

### Phase 7: Teensy TC Client
**Estimated scope: Teensy firmware rewrite**

- [ ] Integrate AgIsoStack++ (or subset) into Teensy firmware
- [ ] Implement FlexCAN_T4 hardware plugin for AgIsoStack
- [ ] Generate DDOP describing Teensy module capabilities
- [ ] Implement ISO NAME and address claim
- [ ] Replace proprietary PGNs with standard Process Data (DDIs)
- [ ] Add DM1/DM2 diagnostic message support
- [ ] Maintain backwards compatibility mode (optional)
- [ ] Test Teensy with RC gateway and third-party Task Controllers

**Deliverable:** Teensy modules are fully ISOBUS compliant TC Clients

**Teensy DDOP Structure:**
```
Device
├── DeviceElement (type: Device)
│   ├── DeviceProcessData (DDI 1/6: Setpoint Rate)
│   ├── DeviceProcessData (DDI 2/7: Actual Rate)
│   └── DeviceProcessData (DDI 157: Section Control)
└── DeviceElement (type: Bin/Tank)
    └── DeviceProcessData (DDI 48: Actual Volume)
```

## 8. Testing Strategy

### 8.1 Simulation Tools

- **AgIsoStack++ Virtual CAN** - Software CAN for testing without hardware
- **TC Client Simulator** - Simulate implements for development
- **Wireshark + CAN plugin** - Capture and analyze CAN traffic

### 8.2 Test Scenarios

| Scenario | Description |
|----------|-------------|
| Basic Connection | Implement connects, DDOP received, capabilities parsed |
| Reconnection | Implement disconnects and reconnects |
| Multiple Implements | Two or more implements on bus |
| Rate Control | RC sends setpoint, implement receives |
| Section Control | RC sends section states, implement responds |
| Mixed Mode | Teensy + external implement on same bus |
| Error Handling | Implement errors, timeouts, invalid data |

### 8.3 Reference Implements for Testing

- Raven Viper 4 (sprayer controller)
- Ag Leader Integra (planter controller)
- John Deere Rate Controller
- ISOBUS simulator software

## 9. Risk Assessment

| Risk | Impact | Mitigation |
|------|--------|------------|
| DDOP complexity | High | Start with simple implements, add complexity gradually |
| Timing constraints | Medium | Test with real hardware early |
| Implement compatibility | Medium | Test with multiple brands |
| AgIsoStack++ TC Server maturity | Medium | Engage with library maintainers |
| Protocol edge cases | Low | Comprehensive testing |

## 10. References

- ISO 11783-10:2015 - Task Controller and Management Information System Data Interchange
- ISO 11783-7:2015 - Implement Messages Application Layer
- ISO 11783-11:2011 - Mobile Data Element Dictionary
- AgIsoStack++ Documentation: https://github.com/Open-Agriculture/AgIsoStack-plus-plus
- AEF ISOBUS Guidelines: https://www.aef-online.org

## 11. Glossary

| Term | Definition |
|------|------------|
| TC | Task Controller |
| DDOP | Device Descriptor Object Pool |
| DDI | Data Dictionary Identifier |
| DET | Device Element Type |
| WSM | Working Set Master |
| PD | Process Data |
| ECU | Electronic Control Unit |
| CF | Control Function |

---

*Document created: January 31, 2026*
*Architecture: Option A - Gateway as Translator*
*Target: Level 4 - Full TC Server*
