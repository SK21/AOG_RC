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
| 32505 | 6 | Max pressure gate threshold (raw ADC) |
| 32506 | 20 | Board ID label set (16-char description → module EEPROM) |
| 32700 | 32 | Module config (pins, relay types, CommMode) |

### Module → RC PGNs (data)
| PGN | Size | Description |
|-----|------|-------------|
| 32400 | 15 | Sensor data (rate, qty, PWM, Hz) |
| 32401 | 15 | Module status (pressure, wheel speed, flags) |
| 32402 | 24 | PID diagnostics log (per-sensor, PID-loop cadence) |
| 32403 | 20 | Board ID label report (16-char description, ~2 s cyclic) |

### CAN proprietary frame mapping (CommMode 1/2, via `CanFrameTranslator`)
Each UDP PGN above maps to one or more 8-byte Proprietary-B frames (PF=0xFF). Board label:
- RC → module: PGN 32506 → **0xFF11/0xFF12/0xFF13** (16 chars in 3 frames, `data[0]`=ModID + 7 chars, commit on 0xFF13)
- module → RC: **0xFF14/0xFF15/0xFF16** → PGN 32403 (assembled on 0xFF16)
- Stored in a dedicated EEPROM slot (offset 3) that **survives a firmware reflash** (unlike settings).

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

### Jul 9, 2026 — Multi-Sensor Calibration (per-sensor AutoOn) — firmware ×3 + app

Motivated by PR #57 (gunicsba): on a multi-sensor module, calibration flags flapped because
`MasterOn`/`AutoOn` are module-global in the firmware but written by every per-sensor PGN 32500 —
last packet wins. The PR's app-side fix was adopted but was only safe for the unlocked phase; the
locked (Testing Rate) phase needs `AutoOn = 0` for manual CalPWM, which idle sensors' packets
would overwrite. Root fix implemented instead:

**Firmware (RCteensy, RC_ESP32, RCnano):** `AutoOn` is now **per-sensor** — `bool AutoOn[MaxProductCount]`,
decoded from each sensor's own packet bit 6 (`Receive.ino`; Teensy also `CANBus.ino` 0xFF03).
Used in `PIDenabled[i]`, `Applying[i]`, and `DoPID()`. Initialized true in the `DoSetup` loop
(NOT an aggregate initializer — `{ true, true }` silently under-fills on the 6-product ESP32).
`MasterOn` stays module-global (machine-level, identical in both calibration phases). No EEPROM
layout change. Teensy + Nano compile-verified; ESP32 needs a Visual Micro build.

**App:**
- `clsProducts.CalibrationOnModule(moduleID)` — true when a product on that module is calibrating.
- `PGN32500.cs`: calibration branch scoped per module (uninvolved modules take the normal path).
  All products on a calibrating module assert `MasterOnMode` (hoisted, set once); the calibrating
  product adds `CalibrationOn` (+`AutoOn` unlocked / manual CalPWM bytes locked); idle products add
  `AutoOn` — parks them (rate 0 → PID gated off, combo-close valves driven closed). Safe because
  AutoOn is per-sensor now.
- `clsRelays.SetRelays`: calibration relay forcing (Master/FlowMaster/Slave/Bypass/Section) scoped
  via a single `CalibratingModule` local — idle modules' relays are untouched during calibration.

Result: any number of sensors can calibrate simultaneously, in any phase mix, same or different
modules. **Deploy firmware + app together** — a new app with old firmware reintroduces the
clobbering. Supersedes PR #57 (credit gunicsba for report + diagnosis).

Known pre-existing asymmetry (not fixed): `Invert_Section` relays are not forced off during
calibration while `Section` relays are forced on — paired NC/NO section hardware sees
contradictory states while calibrating.

### Jul 8, 2026 — Valve-Path Gain Fix, Firmware Sync, Dev-vs-Main Review Fixes

- **Scale-tuned PID decode** (field session showed valve sluggish at max sliders): uniform `/100`
  Kp/Ki decode with per-actuator scales in `PID.ino` (`ValveKpScale 1.0/ValveKiScale 0.1`,
  `MotorKpScale 0.1/MotorKiScale 0.01` — motor numerically unchanged). Authority≥1 guard. Stale
  exponential EEPROM defaults fixed. InoID 8076. Valve tunes land mid-slider (~35/40) after re-tune.
- **ESP32 sync fixes:** `SendPIDlog()` was never called; PID log gated on Ethernet only (dead on
  WiFi); board-label report transmitted the wrong buffer; `CalibrationOn[]` sized 2 with 6 products
  (out-of-bounds); `PulseISR` shadowed its timestamp parameter.
- **Nano brought up to date** (memory-tiered): normalized PID (no logging), `/100` decode, master-off
  windup fix, ISR ring fix (fixed modulus — byte 22 acts as a median pulse-count cap, no room for
  timestamps), pressure max gate (`CheckPressureGate()` — `CheckPressure()` is the ADC reader there).
  78% flash, 614 B stack free. Skipped: time-window median, PID logging, board label.
- **Module-level PGN convention fixed (firmware):** 32504/32505/32506 handlers compared
  `ParseModID(data[2])` (high nibble) but module-level PGNs carry the RAW module ID in byte 2
  (like 32700/32401/32403) — worked for module 0 only. Now `data[2] == MDL.ID` on Teensy + ESP32 + Nano.
  Per-sensor PGNs (32500/32501/32502) still use `BuildModSenID`/`ParseModID`.
- **AgGrow XML import:** `double.TryParse` now uses `NumberStyles.Float, CultureInfo.InvariantCulture`
  (comma-decimal locales misparsed rates/geometry).
- PID replay validation: dev firmware math reproduced 89% of 38k logged field samples within ±1 PWM
  count (mismatches = mid-day settings changes, not math). See `docs/` PID analysis material.

### Jun 24, 2026 — Min-UPM Floor Scaled by Active Working Width (app only)

Driven by analysis of today's 100 ms-loop PID logs in `D:\Sync\RATE CONTROL\PID logs\`.
The Min-UPM floor over-applied on **partial-width** passes: the flow target already scales
with sections currently on (`cHectaresPerMinute = WorkingWidth() * speed / 600`), but the
floor did not. Fixed `cMinUPM` had no width term; by-speed `FloorUPMfromSpeed` used
`Core.Sections.TotalWidth(false)` (all *configured* sections — whole implement) instead of
`WorkingWidth` (sections ON now). With 1 of N sections on, target dropped to ~1/N but the
floor stayed full-width, so the `RateSet < MinUPM` clamp in `PGN32500.cs` won → over-apply
on the active section. Smoking gun in `PIDlog_20260624_182648.csv`: recurring flat
`Target = 4.682` with `Applied 0 / Samples 0` driving PWM to +255 (full-width floor binding
at low real demand; also fed integral windup → ±255 overshoot reversals).

**Fix (`Classes/clsProduct.cs` — `MinUPMinUse()`):** scale the floor by the active-width
fraction on the runtime-only path (only `PGN32500` calls it, via `ProductOn(false)`):
```csharp
double fullWidth = Core.Sections.TotalWidth(false);
if (fullWidth > 0) Result *= Core.Sections.WorkingWidth(false) / fullWidth;
```
Both modes handled at once: by-speed's `TotalWidth × (WorkingWidth/TotalWidth)` nets to
`WorkingWidth`; fixed `cMinUPM` gets the active fraction. All sections on → factor 1 →
unchanged. **Deliberately NOT** placed inside `FloorUPMfromSpeed`/`SpeedFromFloorUPM` —
those also feed the Settings UI floor↔speed hint (`frmMenuSettings.cs:207/213`), where the
operator is parked with sections off and `WorkingWidth` would be 0; that preview stays on
full configured width.

App-side (RateController) only — no firmware. **Needs a Visual Studio rebuild of
`RateController.sln`.** Reduces (does not eliminate) the over-pressure exposure tracked in
`docs/MinUPM_OverPressure_Risk.md` (now has a matching 2026-06-24 update section).

### Jun 20, 2026 — Teensy Rate PID/Flow Fixes & Log Instrumentation

Driven by analysis of PID logs in `D:\Temp\PIDLogs\` (rate "adjusting down while below target", valve overshoot on start, and "stuck valve"). All firmware edits in `Modules/Teensy Rate/RCteensy/`. **NOT YET FLASHED**; item 7 also needs an app rebuild. Full investigation detail in the auto-memory `project_pid_logging.md`.

Note: the no-arg PID entry `SetPWM()` was renamed to `DoPID()` (disambiguates from the hardware writer `SetPWM(byte,float)` in `Motor.ino`); called at top of `loop()`.

1. **Bug 1 — auto PID drove the valve while master OFF, and (1a) while all sections OFF (`RCteensy.ino`).** `PIDenabled` didn't include `MasterOn`, so with `AutoOn` + the `MinUPM` target floor the loop wound up and drove the valve open while off → ~10× overshoot when flow primed. **1a:** the same windup occurs when auto turns all sections off but the **master stays on** (e.g. spraying over already-sprayed ground) — no flow path, `UPM`=0, target nonzero → valve winds open. Fix (covers both): `PIDenabled[i] = SensorConnected[i] && AutoOn && MasterOn && (RelayLo || RelayHi) && (Sensor[i].TargetUPM > 0);` — the `(RelayLo || RelayHi)` term matches how `PulseISR`/`GetUPM` already define "no flow." Confirmed safe: prime and calibration assert master-on; manual adjust is the `AutoOn==false` path that bypasses `PIDenabled`. (`PIDenabled` vs `Applying` are intentionally separate — `Applying` = "energize output", used for Motor/Fan/ComboClose in `Motor.ino`; standard valve is a velocity-form actuator — PWM drives the valve motor's speed/direction, so PWM=0 means the motor stops and the valve HOLDS its last position, not closes. So gating PID off makes the valve hold its last position → no windup AND no re-entry lag when a section reopens.)

2. **Logging consistency — snapshot (`RCteensy.ino`, `PID.ino`, `Send.ino`).** `SendPIDlog` had mixed snapshot fields (captured at PID-compute) with live `Sensor[i].*` (read at send) → impossible rows (Target 2, Applied 10.3, Error +2) in ~18% of samples. Fix: added `DiagTarget/DiagApplied/DiagPWM`, captured in `PIDvalve` with the rest of the `Diag*` set; `SendPIDlog` transmits those, not live `Sensor` values. (ChatGPT independently confirmed this diagnosis.)

3. **Bug 2 — flow-measurement lag: hybrid fixed-time-window median (`Rate.ino`).** Measured ~200 ms response lag vs 100 ms `PIDtime` → overshoot/limit-cycle (the "down while below target" symptom). The median was fixed-COUNT (`PulseSampleSize` periods) so lag `≈ (PulseSampleSize/2)/Hz` ballooned at low flow and stale samples lingered on shutoff. Fix: per-pulse timestamps (`SampleStamp[]`), ring modulus changed to `MaxSampleSize` (decouples from `PulseSampleSize`, removes a `%0` risk), and `GetUPM` now takes the median of pulses within `FlowWindow` (150 ms, the tuning knob), capped at `PulseSampleSize`. Lag bounded ~`FlowWindow/2` at any flow; high flow still smooths, low flow stays responsive.

4. **Stuck-valve — MaxIntegral semantics (`PID.ino`, valve & motor).** `MaxIntegral` (default 25) was documented as "per-loop change" but used as an absolute clamp on the total `IntegralSum`, capping integral authority at ±25 — too little to overcome valve stiction at small error. Fix (Option B, no app change): `MaxIntegral` now limits the per-loop increment (`IntegralSum += constrain(RateError*Ki, ±MaxIntegral)`) and the TOTAL is clamped to ±(MaxPWM−MinPWM) so the integral can reach full PWM authority while wind-up rate stays bounded.

5. **Ki decode mismatch (`CANBus.ino`).** UDP (`Receive.ino`) decoded Ki as `1.1^(byte-108)`, CAN as `1.1^(byte-120)` — a 3.1× discrepancy per transport. The `-108` was a deliberate bump (integral too weak); changed CAN to `-108` to match UDP. (May be revisited after field-testing the MaxIntegral fix, since it likely compensated for the cap.)

6. **Hysteresis integral reset (`PID.ino`, valve & motor).** Old reset fired on every raw `Error` sign flip = every `Target−Applied` zero crossing, which target wobble triggered (one log: 170 error flips, Applied reversed only 1×) → integral never accumulated. Fix: reset only when the rate clearly crosses to the other side of target beyond a band (`Deadband*Target`); small jitter no longer wipes the integral, genuine overshoots still do.

7. **Median sample-count added to PID log — firmware + APP (`RCteensy.ino`, `Rate.ino`, `PID.ino`, `Send.ino`; `PGNs/PGN32402.cs`, `Classes/PidLogger.cs`, `docs/PID_Log_Excel_Analysis.md`).** PGN 32402 grew 23→24 bytes: new byte `[22] = Samples` (pulse count used in the median that loop), CRC moved to `[23]`. App parses it and writes a new `Samples` CSV column. Lets a field log confirm the fixed-time-window is binding (Samples < PulseSampleSize at low flow). `clsTools.GoodCRC` is length-agnostic. **Firmware and app must be deployed together** — the old app rejects the wider packet.

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

### Apr 18, 2026 — Pressure Alarm, Speed Filter, Area Investigation

**Pressure Alarm (alarm-only — firmware gate deferred):**
- `clsAlarm.cs`: Added `cPressureAlarms[]`, `PressureAlarmIsOn`, `PressureAlarms` properties
- `CheckAlarms()` loops all modules, compares `Props.PressureReading(i, raw)` vs `Props.GetMaxPressure(i)`
- `cAlarmIsOn = CurrentState || PressureAlarmIsOn`
- `frmMain.cs` `ShowAlarmButton()`: "Pressure Alarm" / "Rate + Pressure" / "High Pressure" states added
- Props.cs and frmMenuPressure.cs edits done by user (GetMaxPressure, MaxPressure per-module)
- Firmware gate dropped: closing valve on PTO/independent-pump systems raises pressure, not lowers it
- PGN 32503 is taken (subnet change) — PGN **32505** reserved for any future firmware gate

**Speed Filter Fix:**
- `PGN254.cs`: Added 25/75 averaging (`cSpeed * 0.75 + newSpeed * 0.25`) — previously no filter
- `PGN208.cs`: Removed 10/90 filter — TWOL/RTK GPS already high quality, double-filter unnecessary

**Applied Area Discrepancy Investigation:**
- RC showed 18.8 ac, AOG showed 17.67 ac (6.4% over) on 2-pass test with sections on at turns
- Root cause: RC integrates full width × speed × turn time; AOG draws GPS triangle geometry of curved path (inside sections overlap already-covered ground, so AOG counts much less)
- In normal practice sections are off at headlands — discrepancy does not occur
- No code change needed

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
