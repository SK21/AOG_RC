# ISOBUS Rate Control Integration — Plan

## Context

Goal: use ISOBUS (ISO 11783) for rate control. Previous efforts reviewed:

- **IsobusComm + C++ Gateway** — removed Feb 2026, replaced by proprietary-CAN `CanBridgeComm` (works today). History on `IsoBus`/`Isobus2` branches.
- **RC11-isobus** (`F:\Documents\GitHub\RC11-isobus\`) — near-complete ISOBUS firmware port: AgIsoStack++ on Teensy 4.1, functional TC client (rate setpoint DDI, condensed-work-state sections, actual rate/speed feedback, DDOP) and VT client (~1750 lines of terminal config screens). **Stale since ~May 16**: missing normalized PID, pressure gate 32505, SampleWindow median filter.
- **AOG-TaskController** (GwnDaan, local clone) — working Windows TC master, but **section-control-only**; no rate setpoint DDIs, no prescriptions.
- **XCP plan** — never started; now judged unnecessary (see decisions).

## Decisions (made with user AFK — confirm at review)

1. **Target Group 1 first** (ISOBUS tractor terminal = TC master; rate + sections from the tractor; RC app not required). Group 2 (RC as TC master via an extended AOG-TaskController with DDI-1 rate support) is a documented future phase; nothing in this work blocks it — the TC client is master-agnostic.
2. **Merge ISOBUS into mainline `RCteensy\`** behind a compile flag; retire RC11-isobus afterward. Prevents the drift that made RC11-isobus stale in 6 weeks.
3. **Keep proprietary 0xFF frames working alongside ISOBUS** on the same bus (Proprietary B is legal ISO 11783). RC's config UI + telemetry work unchanged; XCP and HTTP /config dropped.

## Verified technical foundations

- Both firmwares use **CAN1** (TX 22/RX 23, 250 kbps).
- The CAN-ownership problem is **already solved** in the fork: custom `RCteensyCANPlugin : isobus::CANHardwarePlugin` (RC11 `CANBus.ino:18-68`) owns one FlexCAN instance; proprietary TX via `CANHardwareInterface::transmit_can_frame()` and RX via `get_can_frame_received_event_dispatcher()` (every frame hits the listener before the network manager — verified `can_hardware_interface_single_thread.cpp:191-206`). Coexistence = delete the `#if ISOBUS_TC_MODE return;` early-out at RC11 `CANBus.ino:270-272` and use **mainline** handler bodies (fork's are stale: old exponential Kp/Ki, missing 0xFF10/0xFF11-16).
- **Build prerequisite**: AgIsoStack needs the patched Teensy core linker script (moves `*AgIsoStack*` sections out of ITCM/DTCM) — already patched on this machine (`imxrt1062_t41.ld`). Locally-modified `FlexCAN_T4.hpp/.tpp` in `src/AgIsoStack/` must be the only FlexCAN include in ISOBUS builds.
- MachineSettings is a **new** EEPROM block (mainline has none): place at 640 (mainline map ends at 480), bump identifier 0x5445→0x5446 with the AutoOn fix.

## Design decisions

**D1 — CAN coexistence:** RC11 plugin architecture + mainline handler bodies. Address claim delegated to AgIsoStack. Arbitration: in ISOBUS builds, ignore proprietary *command* frames 0xFF03 (rate) / 0xFF04 (relays) while a TC session actively commands (`TC_ProcessDataReady()`); all config/telemetry frames stay live.

**D2 — Compile-time `ISOBUS_TC_MODE` flag** (new `isobus_config.h`, checked-in default 0), NOT a runtime CommMode 3. Reasons: linker-script prerequisite would hit every mainline build; ~55 KB of AgIsoStack statics survive gc-sections; flag-0 parity trivially auditable. Arduino compiles everything under `src/`, so each of the ~41 AgIsoStack .cpp files gets a mechanical 3-line gate wrap (`#include "agisostack_gate.hpp"` / `#if ISOBUS_TC_MODE` / `#endif`) — scriptable. CommMode 0/1/2 keeps its runtime meaning inside ISOBUS builds (gates UDP vs proprietary-CAN telemetry; ISOBUS stack always runs).

**D3 — EEPROM:** mainline map untouched; MachineSettings (fork struct minus `AutoRate[]` → single `bool AutoOn`) at 640, id 0x5446. Mainline SensorConfig layout kept byte-for-byte so sensor settings survive flag flips.

**D4 — AutoOn regression fixes** (restore mainline global `bool AutoOn`): do not port `Sensor[i].AutoOn`/`AdjustmentEnabled`; edit fork sites `Begin.ino:718/720`, `ISOBUS_TC.ino:79-83/252`, `ISOBUS_VT.ino:475,611,805-809,1436-1441,1672,1678-1680`, `RateControl_UpdateTargets()`; **delete `MDL.CommMode = 1;` at fork `Begin.ino:735`**. Mainline gating (`PIDenabled = SensorConnected && AutoOn && MasterOn && relays && TargetUPM>0`) is unchanged; TC drives all inputs via `TC_RefreshMachineCommandTime()` + `TC_ApplyCondensedWorkState()` + `RateControl_UpdateTargets()`.

**D5 — Ethernet stays fully enabled** in ISOBUS builds (OTA, PID log 32402, UDP config). Do not port `ETHERNET_COMM_ENABLED`.

## File-by-file merge (target: `AOG_RC\Modules\Teensy Rate\RCteensy\`)

| File | Action |
|---|---|
| `isobus_config.h` | NEW — master flag, default 0 |
| `RCteensy.ino` | 3-way: base mainline; add under flag: AgIsoStack includes, 5 shared_ptr stack globals, MachineSettings struct (D3), TC/VT/speed state; loop under flag → `CANBus_Update()` + `RateControl_UpdateTargets()`/`UpdateAreaCounter()` in 50 ms block |
| `CANBus.ino` | 3-way (core work): shared mainline handler bodies; under flag: RC11 plugin + `setupAgIsoStack()` + `CANBus_SendProprietaryB` + `CANBus_HandleFrame` (early-return deleted, D1 arbitration added); else: mainline direct-FlexCAN verbatim |
| `Begin.ino` | 3-way: base mainline; add CrashReport print; under flag: Load/Save/ApplyMachineSettings (0x5446, no legacy chain, D4 edits) |
| `Rate.ino` | 3-way: base mainline (SampleWindow median kept); append `RateControl_*` block under flag; guarded tank-pulse capture in `GetUPM()` |
| `PID.ino`, `Motor.ino`, `Receive.ino`, `Send.ino`, `WheelSpeed.ino`, `Analog.ino`, `EthernetUpdate.ino` | Mainline kept verbatim (fork's PID/Motor rewrites discarded) |
| `Relays.ino` | 3-way: base mainline; add PCA9685/PCF8574 cases 5/6 unguarded; under flag: `RelayEffectiveLo/Hi()` (return `NewLo`/`NewHi`), `RelayTestForce`, VT helper steppers |
| `ISOBUS_TC.ino`, `ISOBUS_VT.ino`, `ISOBUS_Speed.ino` | Port from fork, wrap in flag, apply D4 edits |
| `ISOBUS_VT_ObjectPool.cpp` | Port + rename to `.h` (it's #included; avoid double-compile) |
| `src/AgIsoStack/**` (~106 files) | Copy minus hygiene deletions; script gate-wrap ~41 .cpp files |
| Docs | Copy `ISOBUS_Design.md`, `AGISOSTACK_TEENSY41_NOTES.md`; document ld patch + FlexCAN divergence |

**Hygiene (do not copy):** `flex_can_t4_plugin.{cpp,hpp}` + `.bak`/`.disabled` variants (remove `#include` from `AgIsoStack.hpp:14`), `ISOBUS_VT_ObjectPool.cpp_copy.cpp`, fork `__vm/`/`.vcxproj`.

## Milestones (~8–13 bench days)

- **M0 Scaffolding (0.5–1 d):** flag + gated AgIsoStack copy; **gate: flag-0 build byte-identical to pre-merge** (`teensy_size` match).
- **M1 Transport + coexistence (2–3 d):** CANBus/RCteensy/Begin merges; **gate: ISOBUS build is a superset of proprietary-CAN mainline** — RC app round-trips 0xFF0C-0F config, 0xFF05/06/0B PID, 0xFF10, 0xFF11-16 labels; telemetry 0xFF00-02/08; UDP per CommMode; OTA works.
- **M2 TC client (2–3 d):** port TC/Speed/Rate/Relays additions; **gate: rate setpoint → TargetUPM → DoPID → PWM; sections → relays; feedback out — with proprietary frames still live**. Bench master: AgIsoVirtualTerminal/AgIsoStack TC server or tractor terminal.
- **M3 VT client (2–4 d):** port VT + pool; measure RAM1, VTCache→DMAMEM if tight; **gate: Group-1 feature-complete**, settings persist across power cycle.
- **M4 Regression (1–2 d):** flag-0 AND flag-1 × CommMode 0/1/2: normalized PID unchanged, SampleWindow, pressure gate trip/latch, PID log, wheel cal 32504/0xFF07, relay types 1-6. Dual-master test: RC sends 0xFF03/04 during TC session → TC wins, no MasterOn oscillation.

## Risks / open questions

- **R1 (top): RAM1** — fork had ~4.7 KB free *without* NativeEthernet. Ladder: VTCache→DMAMEM, DMAMEM sweep, ld rules for fnet, last-resort VT sub-flag. Measure at M1.
- **R2:** toolchain — ld patch per-machine until a `boards.local.txt` override is proven (experiment).
- **R3:** ISOBUS build claims a different source address (AgIsoStack arbitration); RC app keys on PGN+ModID not SA — verify on bench at M1.
- **R4:** loop budget during VT pool upload vs PID cadence — bench-verify.
- **Q:** NAME manufacturer code 0 (fine for tools; AEF code before field release?); keep one-release 0x5445→0x5446 migration for fork test units (recommend yes); release process ships two hex artifacts (flag 0/1)?

## Future phase (not in this effort): Group 2

Fork AOG-TaskController; add DDI 1 (SetpointVolumePerArea) process data + a UDP rate-target message from RateController (AgIO framing, ~200–400 lines C++), small UDP sender in RC. Teensy side already handles the DDI. Candidate for upstream contribution.

## Verification

Milestone gates above are the test plan. Primary end-to-end: bench TC master sends rate setpoint + section commands → module PWM/relays respond, actual rate reports back — while the RC app simultaneously configures/monitors over proprietary 0xFF frames on the same bus. Full detail in the design plan: `drifting-mixing-snowglobe-agent-ae4ddbf71440c760e.md`.
