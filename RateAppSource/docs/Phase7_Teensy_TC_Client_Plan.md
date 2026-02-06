# Phase 7: Teensy TC Client Implementation Plan

## Overview

Transform Teensy rate modules from proprietary CAN (0xFFxx PGNs) to standard ISOBUS TC Client protocol while preserving Ethernet UDP and proprietary PGNs for custom features.

## Architecture

```
Teensy Rate Module
├── CommMode 0: Ethernet UDP → RateController (unchanged)
├── CommMode 1: CAN Proprietary (0xFFxx) → Gateway (existing)
├── CommMode 2: UDP + CAN Proprietary (existing)
├── CommMode 3: TC Client → Gateway TC Server (NEW)
└── CommMode 4: UDP + TC Client (NEW)
```

## Key DDIs for Rate Control

| DDI | Name | Direction | Teensy Mapping |
|-----|------|-----------|----------------|
| 1 | Setpoint Volume Per Area | TC→Teensy | Sensor[x].TargetUPM |
| 2 | Actual Volume Per Area | Teensy→TC | Sensor[x].UPM |
| 48 | Actual Volume | Teensy→TC | TotalPulses / MeterCal |
| 157 | Section Control State | TC→Teensy | RelayLo/RelayHi |

**Note:** Additional standard DDIs can be added as needed. Common candidates include:
- DDI 6/7: Mass per area (granular)
- DDI 11/12: Volume per time
- DDI 116/117: Count per area (seeding)
- DDI 161: Actual condensed work state

## Implementation Phases

### Phase 7.1: TC Client Foundation
**New file: `TCClient.ino`**

- TC Client state machine (IDLE → CONNECTING → DDOP_UPLOAD → ACTIVE)
- Handle TC Server status message (PGN 65272)
- Working Set Master announcement
- Connection timeout handling

### Phase 7.2: DDOP Builder
**New file: `DDOP.ino`**

- Build Device Descriptor Object Pool at startup
- Device object with serial number
- Device elements for boom/sections
- Process data objects for DDI 1, 2, 48, 157
- Extensible structure for adding more DDIs later

### Phase 7.3: Transport Protocol
**New file: `TP.ino`**

- ISO 11783-3 Transport Protocol for DDOP upload
- Multi-packet message handling (RTS/CTS/EOM)
- Both send and receive capability

### Phase 7.4: Process Data Interface
**Modify: `TCClient.ino`**

- Send actual rate (DDI 2) every 200ms
- Send accumulated quantity (DDI 48)
- Receive setpoint (DDI 1) and apply to TargetUPM
- Receive section control (DDI 157) and apply to relays

### Phase 7.5: CommMode Integration
**Modify: `RCteensy.ino`**

```cpp
switch (MDL.CommMode) {
    case 0: ReceiveUDP(); SendComm(); break;           // UDP only
    case 1: CANBus_Update(); break;                    // CAN Proprietary
    case 2: ReceiveUDP(); CANBus_Update(); SendComm(); break; // Both
    case 3: TCClient_Update(); break;                  // TC Client only
    case 4: ReceiveUDP(); TCClient_Update(); SendComm(); break; // UDP + TC
}
```

### Phase 7.6: Gateway Updates
**Modify: `RCTaskControllerServer.cpp`**

- Parse Teensy DDOP to extract capabilities
- Translate DDI values to/from RC UDP PGNs
- Add PGN 32618/32619 for TC Client data to RC

## Files to Create

| File | Purpose |
|------|---------|
| `Modules/Teensy Rate/RCteensy/TCClient.ino` | TC Client state machine |
| `Modules/Teensy Rate/RCteensy/DDOP.ino` | DDOP builder |
| `Modules/Teensy Rate/RCteensy/TP.ino` | Transport Protocol |

## Files to Modify

| File | Changes |
|------|---------|
| `Modules/Teensy Rate/RCteensy/RCteensy.ino` | CommMode 3/4, TCClient_Update() |
| `Modules/Teensy Rate/RCteensy/CANBus.ino` | Route TC PGNs, add TP handling |
| `Modules/Teensy Rate/RCteensy/Begin.ino` | TCClient_Begin() init |
| `IsobusGateway/src/RCTaskControllerServer.cpp` | DDOP parsing, DDI translation |
| `IsobusGateway/include/IsobusGateway/Protocol.hpp` | PGN 32618/32619 |

## Proprietary PGNs Retained

These custom features stay as proprietary messages (no standard DDI equivalent):
- PID tuning parameters (0xFF05/06/0B)
- Flow calibration (0xFF0A)
- Wheel speed config (0xFF07)
- Debug/diagnostic data

## Unit Conversions

```
Liquid Rate:
  1 L/ha = 100 mm³/m²
  DDI value = UPM * 100 / area_factor

Accumulated Quantity:
  DDI 48 in mL
  DDI value = (TotalPulses / MeterCal) * 1000
```

## Verification

1. Build Teensy firmware with TCClient - verify compiles
2. Connect Teensy (CommMode=3) to Gateway
3. Verify DDOP upload and activation in Gateway logs
4. Check process data flow: setpoint → Teensy → actual rate → Gateway → RC
5. Test section control via DDI 157
6. Verify UDP backup works (CommMode=4)
7. Test proprietary fallback for PID settings

## Future Extensions

Additional standard ISOBUS messages that may be added:
- Speed messages (PGN 65267 Machine Selected Speed)
- Position-based section control
- Prescription map support via TC
- DM1/DM2 diagnostic messages
- Additional DDIs for different product types (seed, granular, liquid)

## Related Documentation

- `TC_Server_Design.md` - Gateway TC Server architecture
- `ISOBUS_Integration_Design.md` - Overall ISOBUS integration design
- `../IsobusGateway/docs/PGN_Mapping.md` - PGN translation tables
- `../IsobusGateway/CLAUDE.md` - Gateway implementation notes
