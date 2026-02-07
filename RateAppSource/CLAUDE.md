# AOG RateController Project Notes

## Current Status (Feb 6, 2026)
**ISOBUS Phase 1 COMPLETE** - Bidirectional communication with Teensy modules via proprietary PGNs.
- Multi-driver support: SLCAN, InnoMaker USB2CAN, PCAN
- UI configuration for driver selection, COM port, and diagnostics
- Status indicators show actual ISOBUS module data flow
- PGN traffic logging in Help form

**Teensy TC Client Implementation IN PROGRESS** - ISO 11783-10 TC Client on Teensy modules.
- DDOP (Device Description Object Pool) builder
- Transport Protocol for multi-frame messages
- TC Client state machine with Working Set Master, activation sequence
- Process data exchange (setpoints, actual rates, section control)

**Next Phase: TC Server Implementation** (Option A - Gateway as Translator)
- Goal: Level 4 - RC as full Task Controller Server for external ISOBUS implements
- Architecture: Gateway handles TC protocol, translates to/from RC
- Teensy modules can use either proprietary messages or full TC Client

## Documentation
- **TC Server Design:** `docs/TC_Server_Design.md` (architecture for standard ISOBUS compliance)
- **ISOBUS Integration Design:** `docs/ISOBUS_Integration_Design.md` (Phase 1, ~2700 lines)
- **Gateway Notes:** See separate Gateway repository

## Project Structure
```
AOG_RC/
├── RateAppSource/           # RateController Windows app (C# WinForms)
├── Modules/Teensy Rate/     # Teensy 4.1 rate module firmware
│   └── RCteensy/
│       ├── RCteensy.ino     # Main firmware
│       ├── CANBus.ino       # ISOBUS CAN implementation
│       ├── TCClient.ino     # TC Client state machine (ISO 11783-10)
│       ├── DDOP.ino         # Device Description Object Pool builder
│       ├── TP.ino           # Transport Protocol for multi-frame messages
│       ├── TCDefs.h         # Shared TC/DDI definitions
│       └── Send.ino         # UDP/Ethernet send
└── RateControllerApp/       # Runtime output directory
```

## IsobusGateway - MOVED TO SEPARATE REPOSITORY
The IsobusGateway project has been moved to its own repository:
**Location:** `F:\Documents\GitHub\RateControl\Gateway`

The RateController project includes pre-built copies of:
- `RateAppSource/RateController/IsobusGateway.exe`
- `RateAppSource/RateController/gateway.json`

These are copied to the output directory at build time. If modifying the gateway,
rebuild from the Gateway repository and copy the updated exe.

## RateController App
Windows application for agricultural rate control. Communicates with:
- Rate control modules via UDP (Ethernet) or CAN (ISOBUS)
- Teensy 4.1 modules with MCP2562 CAN transceiver
- IsobusGateway for ISOBUS ↔ UDP translation

## IsobusGateway
**Now in separate repository:** `F:\Documents\GitHub\RateControl\Gateway`

Key points:
- Translates between RC UDP PGNs (32xxx) and ISOBUS proprietary PGNs (0xFFxx)
- Uses AgIsoStack++ for ISOBUS protocol
- Supports multiple CAN drivers: SLCAN, InnoMaker USB2CAN, PCAN
- Configuration via gateway.json (auto-updated by RateController)

## UDP Protocol Summary

### RC → Module PGNs (commands)
| PGN | Size | Description |
|-----|------|-------------|
| 32500 | 14 | Rate settings (setpoint, flow cal, command flags) |
| 32501 | 11 | Relay/section states |
| 32502 | 24 | PID settings |
| 32504 | 9 | Wheel speed config |

### Module → RC PGNs (data)
| PGN | Size | Description |
|-----|------|-------------|
| 32400 | 15 | Sensor data (rate, qty, PWM, Hz) |
| 32401 | 15 | Module status (pressure, wheel speed, flags) |

### Gateway PGNs
| PGN | Direction | Description |
|-----|-----------|-------------|
| 32600 | Gateway → RC | ISOBUS rate from Task Controller |
| 32604 | Gateway → RC | ISOBUS speed from tractor |
| 32605 | Gateway → RC | Gateway status flags |

## Architecture
```
RateController (C#) ──UDP──► IsobusGateway (C++) ──CAN──► Teensy Modules
  TASK CONTROLLER            TC SERVER PROXY              TC CLIENTS
```

## Implementation Status
| Phase | Status | Notes |
|-------|--------|-------|
| 1. Gateway Foundation | ✓ Complete | CAN, PGN translation, speed |
| 2. Gateway TC Server | Not Started | TC protocol proxy for RC |
| 3. RateController Integration | ✓ Complete | Full bidirectional, multi-driver |
| 4. Speed & Diagnostics | ✓ Complete | Speed, status indicators, debug toggle |
| 5. Testing | ✓ Complete | Green light, 200ms timing verified |
| 6. Teensy TC Client | In Progress | ISO 11783-10 implementation started |

## Key Files

### RateController (C#)
- `RateController/Classes/IsobusComm.cs` - Gateway UDP communication, status tracking
- `RateController/Classes/Props.cs` - CanDriver enum, CanPort, ShowCanDiagnostics
- `RateController/Menu/frmMenuOptions.cs` - ISOBUS config UI (driver, port, diagnostics, indicators)

### Teensy Firmware
- `Modules/Teensy Rate/RCteensy/RCteensy.ino` - Main firmware, CommMode setting
- `Modules/Teensy Rate/RCteensy/CANBus.ino` - ISOBUS CAN (FlexCAN_T4), address claim, PGN routing
- `Modules/Teensy Rate/RCteensy/TCClient.ino` - TC Client state machine (ISO 11783-10)
- `Modules/Teensy Rate/RCteensy/DDOP.ino` - Device Description Object Pool builder
- `Modules/Teensy Rate/RCteensy/TP.ino` - Transport Protocol for multi-frame messages
- `Modules/Teensy Rate/RCteensy/TCDefs.h` - Shared DDI/element/state definitions
- `Modules/Teensy Rate/RCteensy/Send.ino` - UDP Ethernet send

### IsobusGateway (separate repo)
See `F:\Documents\GitHub\RateControl\Gateway` for gateway source code.

## Future Work - TC Server Implementation

See `docs/TC_Server_Design.md` for detailed architecture.

| Phase | Description | Status |
|-------|-------------|--------|
| 1 | Gateway TC Server Foundation - Working Set detection | Not Started |
| 2 | DDOP Parsing & Capabilities - Report implement structure to RC | Not Started |
| 3 | Process Data Receive - Actual rates from external implements | Not Started |
| 4 | Process Data Send - Setpoints and section control to implements | Not Started |
| 5 | Prescription/Task Data - Variable rate from maps | Not Started |
| 6 | Integration & Testing - External implements | Not Started |
| 7 | Teensy TC Client - Full ISOBUS compliance for Teensy modules | In Progress |

**Additional future items:**
- ISOBUS Speed Source - Use tractor ground speed (PGN 65267)
- ISOBUS Diagnostics - DM1/DM2 fault codes from bus

## Known Issues

### Gateway CPU Usage - NEEDS INVESTIGATION
Gateway uses ~20-23% CPU even after optimizations. Initial changes (1ms→5ms main loop, 1ms→2ms SLCAN thread, debug→info logging) did not significantly reduce usage. Further investigation needed:
- Profile to identify hotspots
- Check AgIsoStack++ internal update loops
- Consider event-driven vs polling architecture
- Investigate serial port read efficiency

## Recent Changes

### Feb 6, 2026 - Session 2: Bug Fixes & Performance

**Working Set Master PGN Correction:**
- Fixed incorrect PGN 0xFE8F → correct 0xFE0D (65037)
- Files corrected: `can_general_parameter_group_numbers.hpp`, `TCClient.ino`, `Gateway.cpp`
- AgIsoStack tests confirmed 0xFE0D was the original correct value

**frmMenuOptions Event Handler Fix:**
- Added missing `ckIsoBus.CheckedChanged` event wire-up in Designer.cs
- ISOBUS enable checkbox now properly triggers SetButtons()

**Gateway Runtime Fix:**
- InnoMaker DLLs required even when using SLCAN driver
- Copied `InnoMakerUsb2CanLib.dll` and `InnoMakerUsb2CanLib64.dll` to RateControllerApp

**CPU Usage Optimization Attempt:**
- Gateway.cpp main loop: 1ms → 5ms sleep
- slcan_interface.cpp read thread: 1ms → 2ms sleep
- gateway.json logging: "debug" → "info"
- Result: Still ~23% CPU - needs further investigation

### Feb 6, 2026 - Session 1: Teensy TC Client & ISOBUS Options Form

**IsobusGateway Moved to Separate Repository:**
- Gateway project now at `F:\Documents\GitHub\RateControl\Gateway`
- Pre-built `IsobusGateway.exe` and `gateway.json` remain in RateController for runtime use

**ISOBUS Options Form (`frmMenuOptions.cs`):**
- New ISOBUS configuration tab with driver selection (SLCAN, InnoMaker, PCAN)
- COM port dropdown with refresh button (SLCAN only)
- Diagnostics toggle for gateway console visibility
- Status indicators: Gateway connected, Module data receiving
- ISOBUS enable/disable with proper start/stop sequencing
- ISOBUS speed source option

**Teensy TC Client Implementation (ISO 11783-10):**
| File | Lines | Purpose |
|------|-------|---------|
| `TCClient.ino` | 749 | TC Client state machine |
| `DDOP.ino` | 383 | Device Description Object Pool builder |
| `TP.ino` | 665 | Transport Protocol for multi-frame messages |
| `TCDefs.h` | 124 | Shared DDI/element/state definitions |
| `CANBus.ino` | 684 | FlexCAN_T4 CAN handling, address claim |
| `Begin.ino` | 39 | Initialization code |

TC Client features:
- Working Set Master announcement
- Structure label request
- DDOP upload via Transport Protocol
- Object pool activation sequence
- Process data exchange (DDI 1/2/48/157)
- ClientTask status keepalive (2 second interval)
- Setpoint rate and section control from TC

### Jan 31, 2026 - Multi-Driver Support & UI Configuration

**Gateway Multi-Driver Build:**
- AgIsoStack++ included in Gateway repository
- CMake configured to build PCAN and InnoMaker drivers by default
- Gateway.cpp uses `#ifdef USE_INNOMAKER` / `#ifdef USE_PCAN` guards

**frmMenuOptions ISOBUS Tab:**
- Driver selection: SLCAN, InnoMaker, PCAN radio buttons
- COM port dropdown with refresh button (SLCAN only)
- Diagnostics checkbox - toggles gateway console and activity log debug
- Status indicators: lbConnected (gateway), lbDriverFound (CAN hardware)

**Props.cs:**
- `CanDriver` enum: SLCAN, InnoMaker, PCAN
- `CanPort` property for COM port configuration
- `ShowCanDiagnostics` property for debug toggle

**IsobusComm.cs:**
- `GatewayConnected` property - true if status received within 4 sec
- `CANConnected` property - true if gateway reports CAN hardware OK
- `UpdateGatewayConfig()` - writes driver/port to gateway.json before start
- Debug logging conditional on `Props.ShowCanDiagnostics`

### IsobusGateway Build - COMPLETE
- **IsobusGateway.exe** built successfully with CandleLight USB-CAN support
- Gateway source now in separate repo: `F:\Documents\GitHub\RateControl\Gateway`
- Gateway files added to RateController.csproj (auto-copy to output on build):
  - `RateController/IsobusGateway.exe`
  - `RateController/gateway.json`

### IsobusComm.cs UDP Fixes - COMPLETE
- **StartUDP()** fixes:
  - Added guard check to prevent double-start
  - Send socket created BEFORE receive socket (fixes race condition)
  - `udpRunning` flag set AFTER sockets ready, BEFORE async starts
  - Retry logic (3 attempts, 2 sec delay) for port binding
- **StopUDP()** fixes:
  - Added guard check to prevent double-stop
  - Removed invalid `Shutdown()` call (UDP is connectionless)
  - Proper cleanup order

### Teensy CAN Integration - VERIFIED WORKING (Jan 24)
- **CANBus.ino** - Full ISOBUS CAN implementation with FlexCAN_T4
- **ModuleConfig.CommMode** - 0=UDP, 1=CAN, 2=Both
- **MCP2562-E/P** transceiver on CAN1 (TX=22, RX=23)
- **STBY pin fix** - Must set pin 6 LOW to enable transceiver (board-specific)
- Tested with Cangaroo: address claim (0x80), PGNs 0xFF00/01/02/08 transmitting
- **Teensy → Gateway → RC path VERIFIED** (module shows BLUE in RC)

### IsobusComm.cs Updates (Jan 24)
- **Fixed PGN constants**: 32400=0x7E90, 32401=0x7E91 (were incorrectly 0x7E50/0x7E51)
- **Added ForwardSensorData()**: Routes PGN 32400 to product handlers
- **Added ForwardModuleStatus()**: Routes PGN 32401 to ModulesStatus.ParseByteData()
- **Added debug logging**: Logs received UDP PGNs to activity log
- **Gateway console visible**: Set CreateNoWindow=false for debugging

### Gateway Config Fix (Jan 24)
- **gateway.json ports swapped**: listenPort=32700, sendPort=32701 (were reversed)

### Jan 30, 2026 - ADDRESS CLAIM FIXED, FULL PATH WORKING
**Breakthrough:** Gateway address claim now working! Full communication path verified.

**Root causes found and fixed:**
1. **gateway.json UDP ports swapped** in source file
   - Was: listenPort=32701, sendPort=32700
   - Fixed: listenPort=32700, sendPort=32701
2. **Address conflict** - Gateway and Teensy both used 0x80
   - Fixed: Gateway preferredAddress=129 (0x81)

**Verified working:**
- Gateway claims address 0x81 successfully
- Log shows: `Status: CAN=OK, Addr=0x81, RC=OK [CF addr=81 valid=Y]`
- Teensy data received: 0xFF00, 0xFF01, 0xFF02, 0xFF08
- Module identified: Mod:0 Type:1 Sensors:1 FW:22016

**Gateway exe locations (must copy after build):**
- `F:\Documents\GitHub\RateControl\Gateway\build\IsobusGateway.exe` (build output)
- `RateAppSource/RateController/IsobusGateway.exe` (copy here for build)
- `RateControllerApp/IsobusGateway.exe` (runtime location)

### Jan 30, 2026 - FULL BIDIRECTIONAL WORKING ✓
**Breakthrough:** Complete RC ↔ Gateway ↔ Teensy communication verified. Green status light!

**Root cause of CAN TX failure:** Candlelight firmware on SH-C30A adapter had broken TX.
**Solution:** Added SLCAN driver to Gateway, using original slcan firmware on adapter.

**SLCAN Configuration:**
- Driver: slcan (serial port based, not native USB)
- Port: COM7
- Serial baud: 115200
- CAN bitrate: 250000

**Key files added (now in Gateway repo):**
- `Gateway/src/slcan_interface.cpp` - SLCAN driver for AgIsoStack++
- `Gateway/include/IsobusGateway/slcan_interface.hpp`

### Jan 30, 2026 - TIMING FIX (200ms)
**Issue:** ISOBUS PGN timing was ~500ms vs Ethernet's 200ms.
**Root cause:** CANBus.ino had module status hardcoded to 500ms interval.
**Fix:** Changed to use `SendTime` (200ms) matching Ethernet timing.
- Also removed all debug Serial.print statements from CANBus.ino

### Jan 31, 2026 - ISOBUS UI & RELIABILITY FIXES

**1. PGN Byte Constants Fixed:**
- PGN 32600: Was 0x60, corrected to 0x58
- PGN 32601: Was 0x61, corrected to 0x59
- PGN 32604: Was 0x64, corrected to 0x5C
- PGN 32605: Was 0x65, corrected to 0x5D

**2. Indicator Logic Improved (frmMenuOptions.cs):**
- `lbConnected` = Green when actual module data (PGN 32400/32401) received within 2 sec
- `lbDriverFound` = Green when gateway process responding via UDP
- Previously showed green even with wrong COM port; now correctly reflects actual ISOBUS data flow

**3. Enable/Disable Reliability Fixed (frmMenuOptions.cs):**
- Driver and COM port now saved BEFORE starting gateway (was after, causing config mismatch)
- Always stop gateway/UDP before starting to ensure clean state
- `Props.IsobusEnabled` only set true AFTER successful start
- UI disables driver/port controls while ISOBUS enabled (user change)

**4. IsobusComm.cs State Management:**
- Added `lastModuleDataTime` field and `ModuleDataReceiving` property
- `StopUDP()` now always resets state (removed early return guard)
- `StopGateway()` always nulls process reference even on exception
- Socket references nulled outside try/catch for reliability

**Key behavior:**
- Wrong COM port: `lbConnected`=Red (no module data), `lbDriverFound`=Green (gateway running)
- Correct COM port: Both green when ISOBUS modules communicating

**5. PGN Traffic Logging (frmMenuHelp.cs):**
- Added `Log()` method to IsobusComm (same format as UDPcomm)
- frmMenuHelp shows ISOBUS log when enabled, Ethernet log otherwise
- Format: `< PGN` (received), `               > PGN` (sent)

**6. TC Server Architecture Decision:**
- Selected Option A: Gateway as Translator
- Goal: Level 4 - RC as full Task Controller Server
- Gateway handles TC protocol via AgIsoStack++, translates for RC
- Teensy modules continue proprietary communication
- Created design document: `docs/TC_Server_Design.md`

### Known Issue
- Windows can block UDP ports after improper shutdown, requiring computer restart
- Error: "Only one usage of each socket address...is normally permitted"
- Port not visible in netstat but still blocked - Windows network stack issue

## Hardware Setup
- **USB-CAN Adapter:** SH-C30A with SLCAN firmware (not Candlelight)
- **Teensy 4.1** with MCP2562-E/P CAN transceiver
- **CAN Bus:** 250kbps, CAN1 pins TX=22, RX=23
- **STBY Pin:** Pin 6 must be LOW to enable transceiver

## Notes
- CLAUDE.md files are NOT auto-updated. Ask Claude to update when making significant changes.
- Design document is large (~2700 lines) - use offset/limit when reading specific sections.
- Pre-built IsobusGateway.exe included in RateController folder; rebuild from Gateway repo if modifying.
- Gateway repository: `F:\Documents\GitHub\RateControl\Gateway`
