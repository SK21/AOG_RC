# ISOBUS Firmware Design — RC11-isobus Module

## Goals

The module firmware must support two operating modes on the same CAN bus, transparently:

1. **OEM Terminal mode** — a tractor's built-in ISOBUS terminal provides both VT server (display) and TC server (rate setpoints, section control). No PC software required.
2. **RC mode** — RateController (RC) on a PC acts as the operator interface. The IsobusGateway process provides the TC server. AgIsoVirtualTerminal can optionally provide a VT server for monitoring.

The firmware does not need to detect which mode it is in. It speaks standard ISOBUS (AgIsoStack++) and connects to whatever VT and TC servers announce themselves on the bus. RC adapts to the firmware, not the other way around.

---

## Architecture

```
OEM Terminal (VT server + TC server)
  │
  └── CAN bus (250 kbps, 9-pin Deutsch connector)
        │
        └── Teensy (RC11-isobus)
              ├── VT Client → uploads object pool to terminal screen
              └── TC Client → receives setpoints, reports actual rates

PC with RC (no OEM terminal)
  │
  ├── IsobusGateway.exe (TC server) ──┐
  │                                   ├── SLCAN/CAN adapter
  └── AgIsoVirtualTerminal (VT server) ┘
        │
        └── CAN bus
              │
              └── Teensy (RC11-isobus) — same firmware, same behavior
```

RC receives process data from Gateway via UDP (existing PGN 32400/32401 format) and sends setpoints via UDP (PGN 32500/32501). The Gateway translates these to ISOBUS TC protocol. The firmware is unaware of this layer.

---

## Current State

### Communication Architecture — ISOBUS Only

This firmware has one communication mode: **ISOBUS over CAN**. There is no proprietary RC UDP protocol and no CommMode selection. CommMode is always ISOBUS.

- **Ethernet** is used only for OTA firmware update. No proprietary RC UDP communication via Ethernet — ever.
- **CAN bus** carries ISOBUS TC and VT traffic only. The proprietary 0xFF03–0xFF0F CAN frame handlers exist in `CANBus.ino` as dead code from an earlier design iteration and can be removed.
- RC communicates with the module exclusively via the **Gateway TC server**, which translates between RC's UDP PGNs and standard ISOBUS TC protocol.

The `ISOBUS_TC_MODE` and `ETHERNET_COMM_ENABLED` compile flags in `RCteensy.ino` reflect the current in-progress state of the codebase. The target architecture makes these permanent — they are not user-selectable options.

### TC Client (ISOBUS_TC.ino) — Mostly complete

What works:
- DDOP with boom element and per-section elements with width properties
- SetpointVolumePerAreaApplicationRate (receive target rate from TC)
- ActualVolumePerAreaApplicationRate (report actual rate to TC)
- SetpointCondensedWorkState1_16 (receive section on/off from TC)
- ActualCondensedWorkState1_16 (report actual section state to TC)
- ActualSpeed (report ground speed to TC)
- DDOP re-upload on section count/width change
- 3-second grace period after DDOP activation before process data begins

Gaps:
- **MasterOn has no TC source.** `MasterOn` is set only by the RC proprietary protocol (disabled). `RateControl_TargetUPMFromAreaRate()` returns 0 when `!MasterOn`, so TC rate commands have no effect even if AutoRate is on. Fix: set `MasterOn=true` when TC section control becomes active (`TC_SectionControlActive`), or add a MasterOn field to the VT MAIN screen. `SetSensorsEnabled()` already uses `TC_SectionControlActive` to keep sensors warm — apply the same logic to MasterOn.
- Only product 0 is wired to TC; MaxProductCount=2 but DDOP has one rate DPD
- No total applied volume DDI (TotalVolumePerArea or TotalMassPerArea) — TC cannot track tank/area without this
- No lifetime counters exposed via TC
- Section setpoint only applied when `Machine.AutoRate[0]=true`; manual mode silently drops TC commands
- No pressure DDI — ADS1115 reading available in RAM but not exposed to TC
- No GPS/AgOpenGPS speed — ISOBUS_Speed.ino reads ISOBUS speed messages (from tractor) and local wheel sensor only; no UDP path for AgOpenGPS speed (ETHERNET_COMM_ENABLED=0)

### VT Client (ISOBUS_VT.ino) — Logic complete, object pool incomplete

Screens implemented:
- MAIN — rate, sections, speed, tank remaining, boom graphic
- SETTINGS — tile menu to sub-screens
- SECTIONS — section count and per-section width
- RATE — control type, mode, target rate or PWM, meter cal, units
- OUTPUTS — relay polarity, onboard/remote relay type, pin assignment, relay test
- TANK — capacity, remaining, fill, trip/lifetime counters

MAIN screen key notes:
- `Plus_Button` and `Minus_Button` are object IDs, not visual labels. The firmware overrides their displayed text dynamically: `Plus_Button` is labeled "-" (decreases dose); `SoftKey_08` is labeled "+" (increases dose). Confirmed working correctly.
- `SoftKey_07` and `SoftKey_09` are unassigned in the MAIN screen handler

Polish labels hardcoded in `VT_SendStatus` (DAWKA, ZBIORNIK, CISNIENIE, PREDKOSC, SEKCJE, ZAD). These are string values sent to the VT object pool — fine for any terminal, but the language should be English for broad OEM compatibility. Label strings in the object pool itself (the binary blob) may also be in Polish.

The VT object pool (`VT3TestPool` in `ISOBUS_VT_ObjectPool.cpp`) is a binary blob designed for a 480×480 data mask with 1 column of soft keys. The content and completeness of this pool are separate from the logic in `ISOBUS_VT.ino`. The pool must be redesigned or completed using AgIsoTerminalDesigner to match the screens described above.

---

## Memory Management

### Teensy 4.1 Memory Map

| Region | Size | Usage |
|--------|------|-------|
| RAM1 (DTCM) | 512 KB | Default for all variables, stack, heap |
| RAM2 (OCRAM) | 512 KB | `DMAMEM` — slower, DMA-accessible |
| Flash | 8 MB | Code; `FLASHMEM` runs from flash, not ITCM |

RAM1 is nearly full. The main contributors are:

**AgIsoStack++ heap allocations (RAM1):**
- DDOP object pool (built at runtime via `make_shared`, vectors, strings)
- Transport protocol session buffers
- Partnered control functions, shared_ptr control blocks
- CAN message queues in the hardware interface

**Static variables in `VT_SendStatus` (RAM1 BSS):**
The function has ~40 static `char` arrays totaling approximately 1,100 bytes. These live permanently in RAM1 BSS because they are function-static, not stack. Additionally, `char status[320]` and `char selected[42]` are stack-allocated each call.

**`MachineSettings Machine`** — ~120 bytes, currently not DMAMEM.

**`SensorConfig Sensor[2]`** — ~208 bytes, currently not DMAMEM (it is DMAMEM in the AOG_RC firmware but check RC11-isobus).

### Memory Strategy

**Rule: anything that is not accessed in a tight timing loop belongs in RAM2 (DMAMEM) or flash.**

#### Move to DMAMEM

VT string cache buffers — convert function-static in `VT_SendStatus` to a file-scope `DMAMEM` struct:

```cpp
DMAMEM struct {
    char title[18];
    char modeValue[48];
    char pressureValue[12];
    char doseValue[12];
    char doseTarget[16];
    char tankValue[12];
    char areaValue[12];
    char speedValue[12];
    char volumeValue[12];
    char doseLabel[16];
    char tankLabel[16];
    char volumeLabel[8];
    char sectionMap[20];
    char boomBar[24];
    char nozzleBar[24];
    char settingsButton[8];
    char masterButton[8];
    char plusButton[8];
    char minusButton[8];
    char softKey[8][8];       // softKey04..softKey11
    char tankButton[6][12];   // tankButton01..tankButton06
    char configCardTitle[4][16];
    char configCardValue[4][40];
    char description[320];
    char status[320];
    char selected[42];
    uint32_t displayedValue;
    uint32_t lastVTStatus;
    uint8_t lastColours[12];
    bool lastColoursValid;
    bool initialised;
} VTCache;
```

This moves ~1,100+ bytes from RAM1 BSS to RAM2.

Also move to DMAMEM:
- `Machine` (MachineSettings, ~120 bytes)
- `Sensor[2]` (SensorConfig ×2, ~208 bytes) — if not already
- TC timing variables (`TC_LastReport`, `TC_ProcessDataReadyAt`, etc.)
- VT state variables (`VT_SelectedField`, `VT_CurrentScreen`, etc.)

#### Keep in RAM1

Variables accessed every loop iteration or from ISR context (FlexCAN RX):
- `RelayLo`, `RelayHi` (volatile, accessed in ISR)
- `MasterOn`
- `Sensor[i].UPM`, `.Hz`, `.PWM`, `.TargetUPM` (read every PID cycle)
- CAN hardware plugin internal state

#### Flash (FLASHMEM)

Already applied to `TC_Begin`, `VT_Begin`, `VT_HandleKeyEvent`, `VT_HandlePointingEvent`. Continue applying to any function called only at startup or on user events, not in the main loop.

Do NOT mark `TC_Update()` or `VT_Update()` as FLASHMEM — they run every loop and flash execution adds latency.

#### DDOP Memory

`TC_CreateDDOP()` allocates heap (RAM1) for the DDOP at startup and on re-upload. With 16 sections plus boom element plus DVPs this is ~30–50 objects. The DDOP is held alive by the `shared_ptr` passed to the TC client. Once uploaded and acknowledged, the TC client retains it. Consider calling `TC_CreateDDOP()` once at `TC_Begin()` and storing the result, rather than rebuilding on re-upload — `ISOBUSTaskController->reupload_device_descriptor_object_pool()` can reuse the same object if section geometry hasn't changed.

---

## TC Client — Required Additions

### Multi-Product Support

For two-product modules (`MDL.SensorCount == 2`), the DDOP must include a second function element and matching DPDs. The TC server then sends two setpoints and reads two actual rates.

Proposed DDOP structure:
```
Device (RCteensy)
  └── Function element: Boom (ID=TC_OBJ_BOOM)
        ├── DPD: SetpointCondensedWorkState1_16   (sections 1-16)
        ├── DPD: ActualCondensedWorkState1_16
        ├── DPD: ActualSpeed
        └── Bin element: Product 1 (ID=TC_OBJ_PRODUCT1)
              ├── DPD: SetpointVolumePerAreaApplicationRate  (or Mass equiv.)
              ├── DPD: ActualVolumePerAreaApplicationRate
              └── DPD: TotalVolumePerArea  (accumulated — for tank tracking)
        └── Bin element: Product 2 (ID=TC_OBJ_PRODUCT2, if SensorCount==2)
              ├── DPD: SetpointVolumePerAreaApplicationRate
              ├── DPD: ActualVolumePerAreaApplicationRate
              └── DPD: TotalVolumePerArea
```

Section control remains at boom level (shared across products).

### Total Applied Volume DDI

Add `TotalVolumePerArea` (or `TotalVolume`) DPD per product. This lets the TC server track cumulative applied product, which RC uses for tank remaining. The value resets when RC sends a reset command — the TC protocol handles this via the `RequestValue` with reset flag.

Alternatively, use the existing `Machine.TripAppliedUnits` for this and report it via a custom DPD or via the DDI for `LifetimeTotalVolume`.

### Rate Unit Support

#### DDI Scaling — Volume Units

The firmware uses DDI 1 (setpoint) and DDI 2 (actual) — **Volume Per Area Application Rate**, unit mm³/m², resolution 0.01.

```
1 L/ha = 1,000,000 mm³ / 10,000 m² = 100 mm³/m² = 10,000 counts
```

Encode: `TC value = rateLHa × 10000`  
Decode: `rateLHa = TC value × 0.0001`

This is correct per the ISOBUS standard. An OEM terminal implementing DDI 1/2 to standard will interpret the values correctly.

**Gallons/acre** uses the same DDI 1/2 — volume/area is the same physical quantity regardless of display unit. The OEM terminal handles display conversion internally. No firmware change needed.

```
1 gal/acre ≈ 935.4 mm³/m² = 93,540 counts
```

The Gateway converts: `gal_per_acre = TC value / 93540.0`  
Or more usefully, convert to L/ha first: `L_ha = TC_value / 10000.0`, then display conversion in RC.

#### DDI Scaling — Mass Units

**kg/ha and lbs/acre** require DDI 6 (setpoint) and DDI 7 (actual) — **Mass Per Area Application Rate**, unit mg/m², resolution 1.0.

```
1 kg/ha  = 1,000,000 mg / 10,000 m² = 100 mg/m²  = 100 counts
1 lb/acre = 453,592 mg / 4,046.86 m² ≈ 112.1 mg/m² ≈ 112 counts
```

Encode: `TC value = kg_per_ha × 100`  
Decode: `kg_per_ha = TC value / 100.0`

#### Current Bug — UnitMode=1 (kg) Not Reflected in DDOP

`Machine.UnitMode = 1` causes the VT to display kg values, but `TC_CreateDDOP()` always declares DDI 1/2 (volume) regardless of UnitMode. An OEM terminal configured for mass-based application sends DDI 6 setpoints — the firmware's `TC_ValueCommand` returns `false` and silently ignores them.

#### Required Fix

Two options:

**Option A — Switch DDIs based on UnitMode** (simpler, less flexible):  
When `UnitMode == 1`, declare DDI 6/7 in the DDOP instead of DDI 1/2. `TC_RequestValue` and `TC_ValueCommand` handle DDI 6/7 with mass conversion. The DDOP re-uploads when UnitMode changes (already triggers `TC_DDOPNeedsReupload` via `TC_MachineSettingsChanged`).

Mass↔volume conversion requires a user-configurable product density (g/L). Add `float Density_gL` to `MachineSettings`, default 1.0 (water). The VT RATE screen exposes this.

**Option B — Expose both DDI 1/2 and DDI 6/7 simultaneously** (robust, OEM-preferred):  
Declare all four DPDs on each product element. The OEM terminal selects whichever DDI matches its configuration. The firmware responds to whichever setpoint arrives first, using density to convert between mass and volume internally. This is the correct approach for broad OEM terminal compatibility but requires more DDOP complexity and careful handling of conflicting setpoints.

**Recommended: Option A** for initial implementation. Option B deferred until a real OEM terminal integration requires it.

#### Actual Rate Limitation at Standstill

`RateControl_ActualRateLHa()` computes rate from UPM, speed, and active width. When speed ≤ 0.05 km/h the function returns 0 regardless of actual flow. This is correct agronomic behaviour (L/ha is undefined at zero speed) but means the TC server sees zero actual rate at standstill. RC and OEM terminals should treat zero-speed actual rate as indeterminate, not as "no flow".

### Pressure DDI

`PressureReading` (from ADS1115 or analog pin) is measured and available but not reported via TC. Add DDI 130 (ActualWorkingPressure, resolution 1 Pa) as a DPD on the boom element. In `TC_RequestValue()`, convert the ADC reading to Pa using the existing `ReadAnalog()` calibration. No setpoint DDI needed — pressure is read-only from TC's perspective.

### Manual Mode Behaviour

When `Machine.AutoRate[i]` is false, TC section setpoints are currently silently dropped. Instead, report actual section state back but do not apply TC setpoints. Log once every 2 seconds (already done). This is correct behaviour — document it in the DDOP label as "manual override active".

---

## VT Client — Required Changes

### Bug Fixes

Note: `Plus_Button` and `Minus_Button` are object pool IDs, not visual labels. The firmware overrides their displayed text: `Plus_Button` is labeled "-" and decreases dose; `SoftKey_08` is labeled "+" and increases dose. This is intentional and confirmed working.

| Location | Issue | Fix |
|----------|-------|-----|
| `VT_HandleKeyEvent` MAIN, `SoftKey_07` | Unused | Assign or remove from object pool |
| `VT_HandleKeyEvent` MAIN, `SoftKey_09` | Unused | Assign or remove from object pool |

### Language

Change Polish label strings to English in `VT_SendStatus`:
- `DAWKA [%s/ha]` → `RATE [%s/ha]`
- `ZBIORNIK [%s]` → `TANK [%s]`
- `D [%s]` → `VOL [%s]`
- `ZAD %s` → `TGT %s`
- `CISNIENIE` label in object pool → `PRESSURE`
- `PREDKOSC` label in object pool → `SPEED`
- `SEKCJE` label in object pool → `SECTIONS`

The object pool binary must also be rebuilt with English label strings using AgIsoTerminalDesigner.

### Object Pool

The current `VT3TestPool` binary needs to be rebuilt to:
- Match all object IDs referenced in `ISOBUS_VT.ino` (`ISOBUS_VT_ObjectPool.cpp` defines these)
- Include all screens: MAIN, SETTINGS, SECTIONS, RATE, OUTPUTS, TANK data masks
- Use English text labels
- Be sized for a 480×480 data mask (standard ISOBUS VT3 minimum)
- Include all soft key masks referenced in the key handler

The object pool is the primary reason the VT screen appears incomplete — the logic in `ISOBUS_VT.ino` is largely correct, but the pool must provide the layout objects that the logic sends updates to.

### Second Product

When `MDL.SensorCount == 2`, the MAIN screen should show both product rates. Add a second dose/rate display row. `VT_SendStatus` already indexes `sensorIndex=0` only — extend to loop both sensors.

### Full Configuration Coverage

In ISOBUS_TC_MODE, the RC proprietary protocol is disabled. **All configuration that RC previously handled via PGN 32500/32501/32502/32504 must be accessible through the VT.** The current VT covers sensor count, section widths, relay type and pins, target rate, meter cal, units, and tank. Missing from VT:

| Parameter | Previously set by | VT screen needed |
|-----------|-------------------|-----------------|
| Kp, Ki | RC (PGN 32502) | New PID screen |
| Deadband, BrakePoint, PIDslowAdjust | RC (PGN 32502) | New PID screen |
| SlewRate, MaxIntegral, PIDtime | RC (PGN 32502) | New PID screen |
| MaxPWM, MinPWM | RC (PGN 32502) | New PID screen |
| TimedMinStart, TimedAdjust, TimedPause | RC (PGN 32502) | New PID screen |
| PowerRelayLo/Hi (power-to-close relays) | RC (PGN 32501) | OUTPUTS screen extension |
| InvertedLo/Hi (hold-open relays) | RC (PGN 32501) | OUTPUTS screen extension |
| FlowMasterValveIndex | RC (PGN 32501) | OUTPUTS screen extension |
| WheelSpeedPin, WheelCal | RC (PGN 32504) | New SPEED screen or SETTINGS tile |
| Product density (g/L) for kg mode | Not yet in either | RATE screen extension |

The PID screen is the most complex addition. Use the existing `VT_AdjustField` pattern: UP/DOWN keys step through fields, +/- keys adjust value.

---

## OTA Firmware Update

Ethernet is always enabled for OTA firmware update. No proprietary RC UDP communication is enabled alongside it — `ReceiveUDP()` and `SendComm()` are removed. Normal ISOBUS operation is unaffected since OTA is performed out-of-field with a laptop running RC.

### Ethernet Initialisation

Change `ETHERNET_COMM_ENABLED` to 1 in `RCteensy.ino`. Remove the `ReceiveUDP()` and `SendComm()` calls from the main loop entirely — they are not used in this architecture. `ReceiveUpdate()` (OTA) remains. `Receive.ino` and `Send.ino` can be deleted or left as stubs.

### IP Address for OTA

RC cannot auto-discover the module via UDP heartbeat since `SendComm()` is removed. The module uses a static IP stored in EEPROM (`MDLnetwork.IP0`–`IP2` with fourth octet derived from `MDL.ID`, same scheme as RCteensy). The user enters this IP manually in RC's OTA dialog. Network settings (subnet, module ID) are configured via the VT.

### OTA Procedure

1. Connect PC to same subnet as the module (direct cable or local switch)
2. Open RC → OTA tab → enter module IP
3. Select `.hex` file
4. RC sends firmware bytes via UDP; `ReceiveUpdate()` flashes flash memory via FXUtil/FlashTxx
5. Module restarts; ISOBUS operation resumes normally

---

## EEPROM Migration

RC11-isobus uses a different EEPROM layout from RCteensy:
- Machine settings moved to address 640+, identified by `MACHINE_SETTINGS_IDENTIFIER = 0x5445`
- Sensor block addresses differ
- New fields: `Density_gL`, `WheelSpeedPin` location changed

On first boot after flashing from RCteensy, `MACHINE_SETTINGS_IDENTIFIER` will not match. `Begin.ino` detects this and loads defaults — the module restarts cleanly with factory settings. **The operator must re-enter configuration via VT after firmware upgrade.** No automatic migration is planned; the configuration screens are the correct path.

---

## Onboard Relay I2C Support

`InitializeRelayOutputs()` in `Begin.ino` clamps `MDL.OnboardRelayControl` to max 1 (GPIO only). All six relay types (None, GPIO, PCA9555-8, PCA9555-16, MCP23017, PCA9685, PCF8574) are already implemented in `ControlSwitch()`. Remove the clamp to allow I2C expanders on the onboard relay bank. The RC11 PCB has 8 onboard GPIO relay pins fixed by hardware; the I2C path is for custom hardware variants. The VT OUTPUTS screen already cycles through all control types for remote relays — extend `RelayControlStep()` (onboard) to match `RemoteRelayControlStep()`.

---

## Gateway TC Server — Required Implementation

The Gateway (`IsobusGateway.exe`) must implement an AgIsoStack++ TC server that:

### Inbound (Teensy → Gateway → RC)

| TC process data received | Translate to |
|--------------------------|--------------|
| ActualVolumePerAreaApplicationRate (product 1) | PGN 32400 byte fields: UPM, Hz (derive or scale) |
| ActualVolumePerAreaApplicationRate (product 2) | PGN 32400 for sensor 1 |
| ActualCondensedWorkState1_16 | PGN 32401 relay state bytes |
| ActualSpeed | PGN 32401 wheel speed field |
| TotalVolumePerArea (if implemented) | PGN 32400 total pulses field (scaled) |

### Outbound (RC → Gateway → Teensy)

| UDP PGN received from RC | Translate to |
|--------------------------|--------------|
| PGN 32500 TargetUPM (sensor 0) | TC SetpointVolumePerAreaApplicationRate (product 1) |
| PGN 32500 TargetUPM (sensor 1) | TC SetpointVolumePerAreaApplicationRate (product 2) |
| PGN 32501 relay bytes | TC SetpointCondensedWorkState1_16 |
| PGN 32500 MasterOn bit | TC master section enable (all sections on/off) |

### DDOP Parsing

On TC client connection, the Gateway receives the Teensy's DDOP. It must parse the DDOP to find:
- Which elements map to which DDIs
- Section count and section widths (to relay to RC for section UI sizing)
- Whether one or two products are present

RC can receive section count and width from the Gateway via a new or extended PGN (or via the existing PGN 32605 gateway status flags, extended).

### Speed Source

The Teensy reports `ActualSpeed` via TC (from ISOBUS speed messages received from the tractor). When connected to RC instead of an OEM tractor, the speed source comes from AgOpenGPS via RC's existing UDP path. The Gateway should:
1. Receive speed from RC (existing UDP PGN 254/255 path)
2. Broadcast as ISOBUS **ground-based speed (PGN 0xFE48)** at 100ms intervals so the Teensy's `SPEED_Update()` picks it up via `SpeedMessagesInterface`

AgIsoStack++ `SpeedMessagesInterface` reads PGN 0xFE48 (GroundBasedSpeedAndDistance) natively — the Gateway uses the same library to send it.

### Module Registration with RC

RC discovers modules by receiving PGN 32401 UDP heartbeats from modules. The Gateway must send PGN 32401 on behalf of each connected TC client so RC sees it as a module on the network. Key fields to populate:

- `ModuleID` — derived from the TC client's ISOBUS identity number (set to `MDL.ID + 1000`)
- `InoType = 1` (Teensy Rate)
- `InoID` — firmware version from diagnostics protocol
- `Status bit 4` — set (Ethernet connected, via Gateway)
- Section count and widths — included in a new or extended PGN sent once after DDOP parse

### Multiple Module Support

Each RC11-isobus module is an independent ISOBUS node with its own NAME and TC client session. The Gateway maintains one TC client session per connected module. Each session maps to a unique `ModuleID` in RC UDP PGNs, using the module's `MDL.ID` field (encoded in the ISOBUS identity number). RC already supports multiple modules by module ID — no RC changes are needed if the Gateway correctly populates the module ID field in each PGN.

---

## Implementation Plan

### Firmware — Blocking (required before field use)

| # | Item | File | Edit |
|---|------|------|------|
| F1 | Enable Ethernet for OTA | `RCteensy.ino`, main loop | Change `ETHERNET_COMM_ENABLED` to 1. Remove `ReceiveUDP()` and `SendComm()` calls from main loop. Keep `ReceiveUpdate()`. Delete or stub `Receive.ino` and `Send.ino`. |
| F2 | MasterOn from TC | `ISOBUS_TC.ino` | In `TC_Update()` or `TC_ValueCommand()` section state handler: `if (TC_SectionControlActive) MasterOn = true;` Reset `MasterOn = false` only when TC disconnects or all sections commanded off and no rate active. |
| F3 | RAM1 relief — VTCache to DMAMEM | `ISOBUS_VT.ino` | Convert ~40 function-static char arrays in `VT_SendStatus()` to a file-scope `DMAMEM struct VTCache`. Replace all `static char foo[]` with `VTCache.foo`. Also mark `Machine`, `Sensor[2]`, and TC/VT state variables as `DMAMEM`. |

### Firmware — TC Client additions

| # | Item | File | Edit |
|---|------|------|------|
| F4 | Total volume DDI | `ISOBUS_TC.ino` | Add `TotalVolumePerArea` DPD per product in `TC_CreateDDOP()`. In `TC_RequestValue()`, return `(int32_t)(Machine.TripAppliedUnits * Sensor[i].MeterCal * 10000.0f)` for this DDI. Handle reset flag to call `RateControl_ResetTripCounters()`. |
| F5 | Rate units — DDI 6/7 for kg | `ISOBUS_TC.ino` | In `TC_CreateDDOP()`: when `Machine.UnitMode == 1`, declare DDI 6/7 instead of DDI 1/2. In `TC_RequestValue()`/`TC_ValueCommand()`: DDI 6 setpoint → `kg_per_ha = value / 100.0f`, convert to L/ha via `Density_gL`, call `RateControl_LHaToTCValue`. DDI 7 actual → reverse. Set `TC_DDOPNeedsReupload` when UnitMode changes. |
| F6 | Product density field | `RCteensy.ino`, `ISOBUS_VT.ino` | Add `float Density_gL` to `MachineSettings` struct (default 1.0). Add to EEPROM save/load in `SaveMachineSettings()`. Add density adjustment field to VT RATE screen. |
| F7 | Multi-product DDOP | `ISOBUS_TC.ino` | In `TC_CreateDDOP()`: when `MDL.SensorCount == 2`, add second product bin element with matching DDI DPDs. Extend `TC_RequestValue()` and `TC_ValueCommand()` to check element ID and route to `Sensor[1]`. |
| F8 | Pressure DDI | `ISOBUS_TC.ino` | Add DDI 130 (ActualWorkingPressure, Pa) DPD to boom element in `TC_CreateDDOP()`. In `TC_RequestValue()`, return `(int32_t)PressureReading` converted to Pa via existing calibration. |

### Firmware — VT additions

| # | Item | File | Edit |
|---|------|------|------|
| F9 | English labels | `ISOBUS_VT.ino` | Replace Polish string literals in `VT_SendStatus()`: DAWKA→RATE, ZBIORNIK→TANK, D→VOL, ZAD→TGT. Rebuild object pool with English text. |
| F10 | SoftKey_07/09 assignment | `ISOBUS_VT.ino` | In MAIN screen key handler: assign SoftKey_07 to reset trip counters, SoftKey_09 to toggle manual/auto mode (or remove from pool). |
| F11 | MAIN screen second product | `ISOBUS_VT.ino` | Extend `VT_SendStatus()` to loop both sensors when `MDL.SensorCount == 2`. Add second rate/dose row to MAIN data mask in object pool. |
| F12 | VT object pool rebuild | `ISOBUS_VT_ObjectPool.cpp` | Rebuild binary in AgIsoTerminalDesigner. Match all object IDs in `ISOBUS_VT.ino`. All screens: MAIN, SETTINGS, SECTIONS, RATE, OUTPUTS, TANK, PID. English labels. 480×480 data mask. Commit source `.ato` project file alongside binary. |
| F13 | PID configuration screen | `ISOBUS_VT.ino` | New VT screen (data mask) accessible from SETTINGS. Fields: Kp, Ki, Deadband, BrakePoint, PIDslowAdjust, SlewRate, MaxIntegral, PIDtime, MaxPWM, MinPWM, TimedMinStart, TimedAdjust, TimedPause. Use `VT_AdjustField` step pattern. `SaveData()` on exit. |
| F14 | OUTPUTS screen — power/inverted relays | `ISOBUS_VT.ino` | Extend OUTPUTS screen to configure `PowerRelayLo/Hi` (bitmask), `InvertedLo/Hi` (bitmask), and `FlowMasterValveIndex`. These are bitfield fields; use a relay index + toggle pattern. |
| F15 | Wheel speed screen | `ISOBUS_VT.ino` | New screen (or SETTINGS tile): WheelSpeedPin selector (0–42/NC), WheelCal adjustment. Restart Teensy on pin change (matches existing `PGN 32504` behaviour). |

### Firmware — Platform fixes

| # | Item | File | Edit |
|---|------|------|------|
| F16 | Onboard relay I2C support | `Begin.ino` | Remove clamp `if (MDL.OnboardRelayControl > 1) MDL.OnboardRelayControl = 1` in `InitializeRelayOutputs()`. Update `RelayControlStep()` to cycle all 7 types (not 0–1 only), matching `RemoteRelayControlStep()`. |
| F17 | EEPROM migration note | `Begin.ino` | No code change needed — `MACHINE_SETTINGS_IDENTIFIER` mismatch already triggers defaults load. Add `Serial.println(F("EEPROM from previous firmware — defaults loaded, please reconfigure via VT"))` in the reset path so the operator knows to reconfigure after flashing. |

### Gateway

| # | Item | File | Edit |
|---|------|------|------|
| G1 | TC server init + DDOP receive | `Gateway.cpp` | Create AgIsoStack++ `TaskControllerServer`. On DDOP upload from TC client, parse element/DPD tree: extract section count, section widths, product count, DDI types (volume vs mass). Store per-module. |
| G2 | Module registration — RC heartbeat | `Gateway.cpp` | After DDOP parse, begin sending PGN 32401 UDP heartbeat to RC at 500ms interval: ModuleID from TC client identity number, InoType=1, section count in status byte, GoodPins=true. |
| G3 | Actual data → RC UDP | `Gateway.cpp` | On `ActualVolumePerAreaApplicationRate` change: build and send PGN 32400 (derive UPM from rate × speed × width, or send rate directly if RC is adapted). On `ActualCondensedWorkState` change: build PGN 32401 relay bytes. On `ActualSpeed` change: populate PGN 32401 speed field. |
| G4 | RC UDP → TC setpoints | `Gateway.cpp` | On PGN 32500 from RC: compute target rate, send `SetpointVolumePerAreaApplicationRate` via TC for matching product. Extract `MasterOn` — when true and speed > 0, send section enable; when false, send all sections off. On PGN 32501: send `SetpointCondensedWorkState`. |
| G5 | ISOBUS speed broadcast | `Gateway.cpp` | Subscribe to RC's GPS speed UDP. Use AgIsoStack++ to broadcast `GroundBasedSpeedAndDistance` (PGN 0xFE48) at 100ms. Module's `SPEED_Update()` picks it up via `SpeedMessagesInterface`. |
| G6 | Multiple module support | `Gateway.cpp` | Map each ISOBUS TC client session to a module ID slot. Multiplex PGN 32400/32401 to RC using per-session module ID. RC supports up to 16 module IDs natively. |

---

## File Summary

| File | Role | Status |
|------|------|--------|
| `ISOBUS_TC.ino` | TC client — DDOP, process data, section control | Mostly complete; needs F2, F4–F8 |
| `ISOBUS_VT.ino` | VT client — screen logic, key/point handling, status send | Needs F3, F9–F15 |
| `ISOBUS_VT_ObjectPool.cpp` | VT object pool binary | Incomplete; rebuild required (F12) |
| `ISOBUS_Speed.ino` | Speed source from ISOBUS messages | Complete; Gateway provides speed (G5) |
| `Begin.ino` | AgIsoStack++ init; relay init | Needs F1 (Ethernet init gate), F16, F17 |
| `CANBus.ino` | FlexCAN_T4 hardware plugin; ISOBUS CAN update | Complete; proprietary handlers disabled by design |
| `RCteensy.ino` | Main loop, compile flags, shared structs | Needs F1 (ETHERNET_OTA_ENABLED flag) |
| `EthernetUpdate.ino` | OTA firmware update receive | Gate on ETHERNET_OTA_ENABLED (F1) |
| `Receive.ino` / `Send.ino` | Proprietary UDP — not used in this architecture | Delete (F1) |
| `Rate.ino` | Flow sensing, rate calc, area/tank tracking | Complete |
| `Motor.ino` / `PID.ino` | Valve/motor control, PID | Complete |
| `Relays.ino` / `Analog.ino` | Relay switching, ADC | Complete |
| Gateway `Gateway.cpp` | TC server, process data bridge to RC | **Not yet implemented** (G1–G6) |
