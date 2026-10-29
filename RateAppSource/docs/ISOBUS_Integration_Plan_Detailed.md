# Implementation Plan: Merge ISOBUS TC Client into Mainline RCteensy

Target: F:\Documents\GitHub\RateControl\AOG_RC\Modules\Teensy Rate\RCteensy\ (mainline, InoID 26066)
Source: F:\Documents\GitHub\RC11-isobus\ (fork of ~InoID 25046, stale vs mainline)

---

## 0. Verified findings (corrections to the task brief)

Facts confirmed by reading the actual sources:

1. **CAN port**: Both firmwares use **CAN1** (TX=22, RX=23, MCP2562 transceiver, standby pin 6, 250 kbps) - not CAN3.
   - Mainline: RCteensy\CANBus.ino:10 `FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_16> ISOBUS;`
   - RC11-isobus: CANBus.ino:66 `FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_512> can;` inside a local RCteensyCANPlugin.
2. **RC11-isobus does NOT use the stock AgIsoStack flex_can_t4_plugin.** It defines its own lightweight
   `RCteensyCANPlugin : isobus::CANHardwarePlugin` in CANBus.ino:18-68 (the stock plugin reserves RAM for
   CAN1+2+3 and was disabled: flex_can_t4_plugin.cpp.disabled). The "both cannot own the peripheral"
   problem is already solved in the fork - one FlexCAN instance lives inside the plugin, and everything
   (ISOBUS + proprietary) goes through AgIsoStack CANHardwareInterface.
3. **Raw-frame coexistence APIs exist and are already used** (verified in
   src/AgIsoStack/can_hardware_interface_single_thread.cpp:191-206):
   - RX: `CANHardwareInterface::update()` Stage 1 calls `frameReceivedEventDispatcher.invoke(frame)` for
     **every** received frame *before* handing it to the network manager (receive_can_message_frame_from_hardware).
     Proprietary-B frames with no registered PGN callback are simply ignored by the network manager.
     RC11-isobus registers CANBus_HandleFrame on this dispatcher (CANBus.ino:143).
   - TX: `CANHardwareInterface::transmit_can_frame(frame)` queues raw frames (Stage 3 writes them out).
     RC11-isobus CANBus_SendProprietaryB() (CANBus.ino:204-220) already uses it.
   - The stack periodic work runs from the same update() via periodic_update_from_hardware() ->
     CANNetworkManager::CANNetwork.update() (can_network_manager.cpp:292-295).
4. **RC11-isobus deliberately disabled proprietary handling in ISOBUS mode**: CANBus.ino:270-272 has
   `#if ISOBUS_TC_MODE return; #endif` before the PGN switch. Re-enabling coexistence = delete that early
   return and use the *mainline* handler bodies (see #5).
5. **RC11-isobus CANBus handlers are stale vs mainline**: they still use exponential Kp/Ki
   (pow(1.1, value-120), CANBus.ino:418-424) and PulseSampleSize (:470), and are missing
   0xFF10 (pressure gate), 0xFF11-13/0xFF14-16 (board label). Mainline CANBus.ino has the current
   normalized-PID decode (/1000, /10000), SampleWindow constrain, CANBus_HandleMaxPressure,
   board-label handlers and senders. **Mainline handler bodies win; RC11 transport plumbing wins.**
6. **MachineSettings brief is wrong**: RC11-isobus actually uses identifier **0x5445** (RCteensy.ino:40),
   UnitMode 0=L/1=kg only (:161, clamped at Begin.ino:725), and has **no** LifetimeHours. There is
   internal migration from earlier identifiers 0x5444/0x5443 (Begin.ino:612-695). Mainline has **no**
   MachineSettings struct at all - it is a new addition, stored at EEPROM 640 (mainline map ends at 480,
   board label uses 3-22, so no conflict).
7. **AutoOn regression confirmed**: RC11 SensorConfig adds `bool AutoOn` + `bool AdjustmentEnabled` and
   drops the mainline global `bool AutoOn` (RCteensy.ino:169 mainline). RC11 also renamed DoPID() ->
   SetPWM() and reverted the PID to the **old absolute-error algorithm** (FastAdjustValve=40,
   KpMultiplier=100) - the mainline normalized PID (RefUPM/FracError/OscDamp/conditional integration)
   must be kept wholesale.
8. **ApplyMachineSettings() force-sets MDL.CommMode = 1** (RC11 Begin.ino:735) - must be removed.
9. **Wheel-cal VT screen does NOT exist** (requirement 6 is wrong): grep WheelCal ISOBUS_VT.ino -> no hits;
   the fork design doc lists it as missing (F15). Wheel cal in ISOBUS mode is covered by re-enabled
   proprietary 0xFF07 (and UDP 32504 when Ethernet connected); a VT SPEED screen is a later work item.
10. **Build prerequisite**: AgIsoStack overflows ITCM/DTCM; the fork requires a patched Teensy core linker
    script. **This dev machine core is already patched**: C:\Program Files (x86)\Arduino\hardware\teensy\avr\cores\teensy4\imxrt1062_t41.ld
    lines 26-27 (*AgIsoStack*(.rodata*), *AgIsoStack*(.text*) -> FLASH) and line 74 (*AgIsoStack*(.bss*) -> RAM2).
    Also src/AgIsoStack/FlexCAN_T4.hpp/.tpp are locally modified (struct2queueTx void fix) vs Teensyduino 1.59 -
    see AGISOSTACK_TEENSY41_NOTES.md.
11. **Hygiene issues confirmed**: flex_can_t4_plugin.cpp.bak, .disabled, flex_can_t4_plugin.hpp.bak;
    ISOBUS_VT_ObjectPool.cpp_copy.cpp is byte-identical to ISOBUS_VT_ObjectPool.cpp; the pool is also
    #included as a .cpp from RCteensy.ino:23 while sitting in the sketch folder where the IDE compiles
    it again (harmless only because the array is static const).
12. Loop-structure divergence: RC11 loop calls RateControl_UpdateTargets(), SetPWM(), SetSensorsEnabled(),
    RateControl_UpdateAreaCounter(); mainline computes SensorConnected/PIDenabled/Applying in the 50 ms
    block and calls DoPID(). **Mainline loop structure wins**; TC glue integrates by (a) refreshing
    Sensor[i].CommTime (TC_RefreshMachineCommandTime(), ISOBUS_TC.ino:70-77) so SensorConnected[]
    stays true, and (b) writing Sensor[i].TargetUPM via RateControl_UpdateTargets().

---

## 1. Key design decisions

### D1 - CAN coexistence: one FlexCAN instance inside the AgIsoStack plugin; proprietary frames via raw-frame APIs

Adopt the RC11-isobus architecture, but keep mainline PGN handler bodies:

- RCteensyCANPlugin (from RC11 CANBus.ino:18-68) owns the single FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_512>.
- **Proprietary TX**: CANBus_SendProprietaryB(pgnLow, data, len) builds the 29-bit Prop-B ID with the
  stack-claimed source address and calls isobus::CANHardwareInterface::transmit_can_frame(frame)
  (RC11 CANBus.ino:204-220, verified against can_hardware_interface_single_thread.cpp:152-178).
- **Proprietary RX**: isobus::CANHardwareInterface::get_can_frame_received_event_dispatcher().add_listener(CANBus_HandleFrame)
  (RC11 CANBus.ino:143). CANBus_HandleFrame converts CANMessageFrame -> CAN_message_t and dispatches
  to the existing handler functions - **delete the `#if ISOBUS_TC_MODE return;` at RC11 CANBus.ino:270-272**
  and use the mainline switch (adds 0xFF10, 0xFF11-13; keeps 0xFF03-0xFF0F with current decodes).
- Address claim is delegated to AgIsoStack (create_internal_control_function); ISOBUSid.addressClaimed
  tracks it via CANBus_HandleAddressClaimed so CANBus_SendProprietaryB gates correctly. The mainline
  manual claim state machine is not compiled in ISOBUS builds.
- The network manager silently ignores proprietary PGNs it has no callbacks for; the raw listener sees every
  frame - no filtering conflict. Own TX frames do not appear on the RX dispatcher (no self-reception).
- ISO-legal note: Prop-B (0xFF00-0xFFFF, PDU2 broadcast) alongside TC/VT traffic is standard-conformant.
- Handler-level arbitration (new, small): in ISOBUS builds, ignore proprietary **command** frames
  0xFF03 (rate) and 0xFF04 (relays) while a TC session is actively commanding
  (TC_ProcessDataReady() true) to prevent dueling masters; all **config/telemetry** frames
  (0xFF05-0xFF0B config, 0xFF0C-0F module config, 0xFF10 pressure gate, 0xFF11-16 board label,
  0xFF00-02/08 telemetry out) stay live unconditionally. (Open question Q4 below if a different policy is wanted.)

### D2 - Compile-time flag, not a runtime CommMode 3

**Recommendation: compile-time ISOBUS_TC_MODE flag** (matching RC11 approach), master switch in a new
header `isobus_config.h` in the sketch folder (a plain `#define ISOBUS_TC_MODE 1` / 0), included by
RCteensy.ino **and** by a gate wrapper in every AgIsoStack .cpp.

Rationale (each alone is sufficient):
1. **Linker-script prerequisite**: any build that links AgIsoStack needs the patched imxrt1062_t41.ld
   (or a build.flags.ld override). A runtime CommMode 3 means *every* mainline build carries AgIsoStack ->
   every user/CI machine needs the patched core. Unacceptable for requirement 2 ("flag 0 behaves identically").
2. **RAM1**: the fork measured ~4.7 KB RAM1 free *with Ethernet disabled*. Always-compiled AgIsoStack + static
   instances (CANNetworkManager::CANNetwork, CANHardwareInterface::SINGLETON - kept by .init_array
   even with --gc-sections) would eat mainline headroom permanently.
3. Requirement 2 "identical when 0" is trivially auditable when the flag excludes whole translation units.

**Arduino gotcha that forces the gate-wrapper**: the IDE compiles every .cpp under src/ regardless of
sketch #defines. Therefore each of the ~41 AgIsoStack .cpp files gets a mechanical 3-line wrap:
first line `#include "agisostack_gate.hpp"`, then `#if ISOBUS_TC_MODE`, original content, `#endif`,
with src/AgIsoStack/agisostack_gate.hpp doing `#include "../../isobus_config.h"`. Scriptable; verify
the flag-0 build contains zero AgIsoStack symbols (teensy_size identical to pre-merge).

**CommMode keeps its runtime meaning inside the ISOBUS build** (no new value needed; PGN 32700 byte 31 unchanged):
- ISOBUS stack (address claim, TC, VT, speed) always runs - CANBus_Update() unconditionally in loop().
- CommMode 0: UDP telemetry on, proprietary-CAN telemetry frames (0xFF00-02/08/14-16) off.
- CommMode 1: proprietary-CAN telemetry on, UDP telemetry off.
- CommMode 2: both. (Replaces the RC11 hardwired `const bool CANProprietaryTelemetryEnabled = false;` RCteensy.ino:48.)
- Proprietary RX handlers always active (subject to D1 arbitration).

### D3 - EEPROM / MachineSettings reconciliation

Keep the **mainline** EEPROM map untouched: InoID 0-1, type 2, board label 3-22, MDL 23-147,
network 168-232, sensors 253-356 / 377-480. Add the ISOBUS MachineSettings block at **640** (free space;
Teensy 4.1 EEPROM = 4284 bytes; struct ~90 B -> 640-730).

- Struct = RC11 version minus the regression: replace `bool AutoRate[MaxProductCount]` with a single
  `bool AutoOn` (persisted copy of the mainline global; default true). Keep UnitMode as-is (0/1);
  gal/lbs expansion and Density_gL are later TC work (design doc F5/F6), not part of this merge.
- **Bump identifier to 0x5446** because the layout differs from the fork 0x5445. Mainline users have never
  written 0x5445/0x5444/0x5443, so drop the fork legacy-migration blocks (Begin.ino:623-690); optionally
  keep a one-shot 0x5445 read that maps AutoRate[0] -> AutoOn for fork test units, else defaults load.
- LoadMachineSettings()/SaveMachineSettings()/ApplyMachineSettings() are compiled only under the flag
  (guard in Begin.ino). Machine and Sensor structs: **mainline SensorConfig layout is kept byte-for-byte**
  (SampleWindow, no AutoOn/AdjustmentEnabled) so sensor EEPROM slots stay valid across flag flips.
- Flag-flip behavior: flashing an ISOBUS build over a mainline unit keeps all MDL/sensor settings (same
  slots, same InoID guard) and initializes Machine at 640 with defaults; flashing back to flag-0 leaves 640
  orphaned but harmless.

### D4 - AutoOn regression fixes (concrete)

Global `bool AutoOn` (mainline RCteensy.ino:169) is the single auto/manual authority; per-sensor
AutoOn/AdjustmentEnabled fields are NOT ported. Concrete edits when porting the ISOBUS files:

| Fork site | Edit |
|---|---|
| RCteensy.ino:144 `bool AutoOn;` in SensorConfig, :118 `bool AdjustmentEnabled;` | do not port (keep mainline struct) |
| RCteensy.ino:158 `bool AutoRate[MaxProductCount]` in MachineSettings | -> `bool AutoOn = true;` |
| Begin.ino:718 `Sensor[i].AutoOn = Machine.AutoRate[i];` | -> once: `AutoOn = Machine.AutoOn;` |
| Begin.ino:720 `if (!Machine.AutoRate[i]) Sensor[i].TargetUPM = 0` | -> `if (!Machine.AutoOn) Sensor[i].TargetUPM = 0;` |
| Begin.ino:735 `MDL.CommMode = 1;` | delete |
| ISOBUS_TC.ino:79-83 TC_MachineAutoMode() returns Machine.AutoRate[0] | -> `return AutoOn;` |
| ISOBUS_TC.ino:252 `Machine.AutoRate[0] = true;` | -> `AutoOn = true; Machine.AutoOn = true;` |
| Rate.ino(RC11):226-245 RateControl_UpdateTargets() per-sensor Machine.AutoRate[i] / Sensor[i].AutoOn | branch on global AutoOn; manual: `Sensor[i].ManualAdjust = Machine.ManualPWM[i]; Sensor[i].TargetUPM = 0;` (no per-sensor AutoOn writes) |
| ISOBUS_VT.ino:475, 611, 805-809, 1436-1441, 1678-1680 Machine.AutoRate[productIndex] | -> global AutoOn + Machine.AutoOn (VT toggle sets both, then VT_SaveMachineSettings) |
| ISOBUS_VT.ino:1672 Sensor[sensorIndex].AutoOn | -> AutoOn |
| RC11 CANBus.ino:381 `Sensor[senId].AutoOn = ...` | not applicable - the mainline handler body already does `AutoOn = ...` (RCteensy CANBus.ino:339) |

Interaction with mainline gating (unchanged, verified): `PIDenabled[i] = SensorConnected[i] && AutoOn &&
MasterOn && (RelayLo||RelayHi) && TargetUPM>0` (RCteensy.ino:262). TC drives all four inputs:
TC_RefreshMachineCommandTime() keeps SensorConnected true; TC_ApplyCondensedWorkState() sets
RelayLo/Hi + MasterOn; RateControl_UpdateTargets() sets TargetUPM from L/ha x speed x active width.

### D5 - Ethernet stays fully enabled in ISOBUS builds

Do **not** port ETHERNET_COMM_ENABLED. Mainline Begin.ino Ethernet init, ReceiveUDP(),
ReceiveUpdate() (OTA), SendComm(), SendPIDlog() all remain, gated at runtime by CommMode exactly as
today. This preserves: OTA (req 7), PID logging PGN 32402 when Ethernet connected (req 5), UDP config path,
and RC discovery. Risk: RAM1 (see Risks R1) - fallback is a sub-flag to trim the VT client, not Ethernet.

---

## 2. File-by-file merge strategy

Legend: M = stays mainline (no/minimal change) | P = port from RC11-isobus (with edits) |
3W = 3-way reconciliation | N = new file

| File | Action | Details |
|---|---|---|
| isobus_config.h | N | `#define ISOBUS_TC_MODE 0/1` master switch (checked-in default **0**). |
| RCteensy.ino | 3W | Base = mainline. Add under flag: AgIsoStack includes (`src/AgIsoStack/FlexCAN_T4.hpp` instead of `<FlexCAN_T4.h>`, AgIsoStack.hpp, `<memory> <vector>`), the five std::shared_ptr stack globals (RC11 :28-33), MachineSettings struct per D3, TC/VT/speed state vars (TC_LastSectionCommand, TC_SectionControlActive, TC_DDOPNeedsReupload, RelayTestForce, ISOBUSSpeedKmh/LastMs/Source, PCA9685/PCF8574 addresses+found flags). Loop: under flag -> CANBus_Update() unconditionally + RateControl_UpdateTargets() + RateControl_UpdateAreaCounter() inside the 50 ms block; else -> existing CommMode switch. UDP receive/send + OTA stay unconditional. Keep InoID bump on release. |
| CANBus.ino | 3W (core work) | Single file, two transport layers. Shared: all mainline handler bodies (CANBus_HandleRateCommand ... CANBus_HandleBoardLabel3, cfgBuf/boardBuf accumulators, CANBus_Send*) - they take CAN_message_t in both paths. Under flag: RC11 plugin + setupAgIsoStack() + NAME via isobus::NAME + CANBus_SendProprietaryB via transmit_can_frame + CANBus_HandleFrame (early-return deleted; add D1 arbitration for 0xFF03/04) + CANBus_Update() calling ISOBUSDiagnostics->update(), CANHardwareInterface::update(), SPEED_Update(), VT_Update(), TC_Update(), then telemetry sends gated on CommMode != 0. Else: mainline direct-FlexCAN code verbatim (manual address claim etc.). |
| Begin.ino | 3W | Base = mainline (keeps board label load, InitializeRelays with mainline End semantics, LoadDefaults with SampleWindow/normalized gains). Add: CrashReport print (RC11 :15-20, useful everywhere); under flag: LoadMachineSettings() (new identifier 0x5446, no legacy chain) + SaveMachineSettings() + ApplyMachineSettings() per D3/D4, MDL.SensorCount<1 -> 1 clamp (RC11 :60-66), "Starting CAN/ISOBUS" prints. Do NOT port the InitializeRelayOutputs() pin-policing rework in phase 1 (defer; it also fights RC-configured pin maps). |
| Rate.ino | 3W | Base = mainline (SampleWindow time-window median, SampleStamp, MinMedianSamples, MedianCount - all kept). Append under flag: RateControl_* block from RC11 Rate.ino:14-256 (tank/trip/lifetime counters, RateControl_ActiveWidthM, RateControl_UpdateAreaCounter, RateControl_TargetUPMFromAreaRate, RateControl_ActualRateLHa, RateControl_UpdateTargets per D4, LHaToTCValue/TCValueToLHa). In mainline GetUPM() insert (guarded): capture `uint16_t pulses = PulseCount[i];` before zeroing, and after interrupts() call RateControl_SubtractTankPulses(i, pulses). |
| PID.ino | M | Mainline normalized PID kept verbatim (DoPID, RefUPM, FracError, OscDamp, Diag* snapshots). RC11 SetPWM() rewrite is discarded. |
| Motor.ino | M | Mainline kept (pressure-gate override, SensorConnected/Applying gating). RC11 debug-print version discarded. TC works through it untouched because CommTime refresh keeps SensorConnected[] true. |
| Relays.ino | 3W | Base = mainline (CheckRelays, ControlSwitch cases 1-4). Add unguarded (benefits both modes, low risk): PCA9685/PCF8574 cases 5/6 + PCA9685_SetChannelHigh/PCF8574_WriteOutputs from RC11. Add under flag: RelayEffectiveLo()/RelayEffectiveHi() - implement as `return NewLo;`/`return NewHi;` (the actually-driven state; simpler and more truthful than RC11 re-derivation), RelayTestForce handling in CheckRelays() (`if (RelayTestForce) { NewLo=RelayTestMaskLo(); ... }`), and the VT helper steppers ISOBUS_VT.ino calls (RelayControlStep, RemoteRelayControlStep, RelayPinStep, RemoteRelayPinStep, RelayControlName, RelayTestMaskLo/Hi). Skip the pin-policing/RelayEnsure*/RelayLogOutputState machinery (phase-2 candidate). |
| Receive.ino | M | Mainline kept (global AutoOn bit6, PidLogEnabled bit5, SampleWindow byte 22, 32505 pressure gate, 32506 board label, 32700 CommMode byte 31). No ETHERNET_COMM_ENABLED wrappers. |
| Send.ino | M | Mainline kept (SendComm, board-label 32403, SendPIDlog 32402). |
| WheelSpeed.ino | M | Mainline kept; RC11 diff was DMAMEM-only. (Optional: adopt DMAMEM annotations - free RAM1, zero behavior change.) |
| Analog.ino | M | Identical already. |
| EthernetUpdate.ino | M | Mainline kept; RC11 ETHERNET_COMM_ENABLED wrapper not ported. |
| FXUtil.*, FlashTxx.*, PCA95x5_RC.h | M | Identical already. |
| ISOBUS_TC.ino | P | Copy, then: D4 edits (TC_MachineAutoMode -> global AutoOn; :252), RelayEffectiveLo/Hi per Relays note, keep DDOP/TC_ConfigureClient as-is (boom+sections+DefaultWorkingWidth, DDIs 1/2 + condensed work states + ActualSpeed). Whole file inside `#if ISOBUS_TC_MODE ... #endif` (an .ino always compiles). Master-agnostic already - nothing blocks Group 2. |
| ISOBUS_VT.ino | P | Copy, wrap in flag, apply D4 edits (7 sites), verify every helper it calls exists in merged Relays/Begin (RelayControlStep set, ApplyMachineSettings, TC_MachineSettingsChanged, VT_SaveMachineSettings at :462 stays here). Defer: English labels, DMAMEM VTCache (design doc F3/F9) to polish milestone. |
| ISOBUS_Speed.ino | P | Copy verbatim, wrap in flag. SPEED_GetKmh() fallback to local WheelSpeed retained. |
| ISOBUS_VT_ObjectPool.cpp | P + rename | Rename to ISOBUS_VT_ObjectPool.h (it is static const data, #included from RCteensy.ino - must not be double-compiled). Content unchanged. |
| src/AgIsoStack/** (106 files) | P + gate | Copy all except hygiene deletions; add agisostack_gate.hpp; script-wrap the ~41 .cpp files per D2. Includes the locally-modified FlexCAN_T4.hpp/.tpp (struct2queueTx fix - document in a README in that folder). |
| ISOBUS_Design.md, AGISOSTACK_TEENSY41_NOTES.md | P | Copy into a docs/ or sketch folder; update NOTES with the boards.local.txt option (R2). |
| RC11 repo | - | Retire after merge is field-verified. |

---

## 3. Hygiene cleanup (do NOT copy / delete at source)

- src/AgIsoStack/flex_can_t4_plugin.cpp.bak - drop.
- src/AgIsoStack/flex_can_t4_plugin.cpp.disabled - drop (we never use the stock plugin).
- src/AgIsoStack/flex_can_t4_plugin.hpp.bak - drop.
- src/AgIsoStack/flex_can_t4_plugin.hpp - drop AND remove its #include from AgIsoStack.hpp:14
  (only referenced there; keeps the disabled-cpp question from ever coming back).
- ISOBUS_VT_ObjectPool.cpp_copy.cpp - drop (byte-identical duplicate).
- ISOBUS_VT_ObjectPool.cpp -> .h rename (see table).
- RC11 EthernetUpdate.ino stub variants - not ported. ("EthernetUpdate vs FirmwareUpdate": mainline has a
  single EthernetUpdate.ino implementing the FXUtil/FlashTxx OTA; no second file exists - nothing to unify.)
- RC11 CANBus.ino stale handlers (exp. Kp/Ki, PulseSampleSize, missing 0xFF10/11-16) - superseded by mainline bodies.
- __vm/, .vcxproj* - keep mainline versions; do not import the fork copies.

---

## 4. Ordering, milestones, effort

**M0 - Scaffolding (0.5-1 day)**
1. Create isobus_config.h (flag 0), copy src/AgIsoStack/ with hygiene deletions, write + run the gate-wrap script, add agisostack_gate.hpp.
2. Build flag-0: must produce identical teensy_size output to pre-merge mainline; ideally identical .hex.
   *Milestone gate: flag-0 parity proven.*

**M1 - ISOBUS transport + coexistence (2-3 days)**
3. Merge CANBus.ino per D1 (plugin + setupAgIsoStack + mainline handlers + arbitration + CommMode-gated telemetry).
4. RCteensy.ino merge (globals, structs, loop), Begin.ino merge (MachineSettings per D3, CAN start).
5. Build flag-1 (linker script already patched on this machine). Bench: address claim visible on bus;
   RC app over proprietary CAN: module config 0xFF0C-0F, PID 0xFF05/06/0B, pressure gate 0xFF10,
   board label 0xFF11-16 round-trip, telemetry 0xFF00-02/08 at 200/500 ms; UDP still works per CommMode; OTA reflash works.
   *Milestone gate: ISOBUS build is a superset of proprietary-CAN mainline.*

**M2 - TC client (2-3 days)**
6. Port ISOBUS_Speed.ino, Rate.ino additions, Relays.ino additions, ISOBUS_TC.ino with D4 fixes.
7. Bench with a TC master (AgIsoStack TC server / AgIsoVirtualTerminal + task controller, or tractor terminal):
   DDOP uploads & activates; SetpointVolumePerAreaApplicationRate -> Machine.TargetRateLHa[0] ->
   RateControl_UpdateTargets() -> Sensor[0].TargetUPM -> DoPID() drives PWM; SetpointCondensedWorkState
   drives RelayLo/Hi/MasterOn -> relays click; ActualCondensedWorkState + actual rate + speed report back;
   speed priority MSS -> ground -> wheel -> local wheel with 1 s timeout.
   *Milestone gate: rate + sections from TC master, proprietary frames still live simultaneously.*

**M3 - VT client (2-4 days incl. RAM work)**
8. Port ISOBUS_VT.ino + pool (renamed .h), D4 edits, Relays VT helpers, RelayTestForce.
9. Measure RAM1; if tight, apply design-doc F3 (VTCache -> DMAMEM) immediately.
10. Bench: pool loads on VT server; MAIN/SETTINGS/SECTIONS/RATE/OUTPUTS/TANK screens function; section-width
    change triggers DDOP re-upload; settings persist (0x5446 block) across power cycle.
    *Milestone gate: Group-1 feature-complete.*

**M4 - Regression + polish (1-2 days)**
11. Full mainline regression at flag-0 AND flag-1 CommModes 0/1/2 (UDP-only rig, CAN-only rig, both):
    normalized PID behavior unchanged, SampleWindow, pressure gate trip/latch (32505 + 0xFF10), PID log 32402,
    wheel cal 32504/0xFF07 + WheelCounts 32401/0xFF02, work pin, relay types 1-4 (+5/6 new).
12. Polish backlog (separate PRs): English VT labels, VTCache DMAMEM if not done, wheel-cal VT screen (F15),
    kg DDI 6/7, multi-product DDOP, InoID bump, retire RC11 repo.

Total: **8-13 bench days** to Group-1 complete.

---

## 5. Build & verification specifics

- IDE: Arduino IDE + Teensyduino (this machine: core at C:\Program Files (x86)\Arduino\hardware\teensy\avr),
  board Teensy 4.1, "Smallest Code with LTO" per fork notes.
- Flag-1 builds require the ld patch (already present here). For other machines, document both options in
  AGISOSTACK_TEENSY41_NOTES.md: (a) patch cores/teensy4/imxrt1062_t41.ld (3 lines), or (b) verify whether a
  boards.local.txt/platform.local.txt override of build.flags.ld can point at a repo-local .ld
  (platform.txt:59 uses {build.flags.ld}; needs a quick experiment - open question Q2).
- Record teensy_size for both flag states in the PR description; flag-1 target >= 4 KB RAM1 free after M3.
- Bench checklist (condensed): see milestone gates above; plus dual-master test - RC app sending 0xFF03/04
  while a TC session is active -> TC wins, no oscillation of MasterOn.

---

## 6. Risks & open questions

**R1 (top risk) - RAM1 with Ethernet + AgIsoStack + VT.** The fork measured ~4.7 KB RAM1 free *without*
NativeEthernet. Re-adding NativeEthernet/fnet (code in ITCM, some state in DTCM) may overflow.
Mitigations, in order: VTCache -> DMAMEM (~1.1 KB), DMAMEM sweep of TC/VT/wheel state (the fork already marks
many), extend the ld rules to move *fnet*/*NativeEthernet* .text* to flash, more FLASHMEM on cold functions,
last resort: ISOBUS_VT_ENABLED sub-flag (TC without VT screens still meets Group-1 core; config then via
RC proprietary frames, which this merge restores). Measure at M1 before investing in M3.

**R2 - Toolchain fragility.** Local FlexCAN_T4 copy diverges from Teensyduino (struct2queueTx void fix);
same include guard _FLEXCAN_T4_H_ means include-order decides which wins - keep src/AgIsoStack/FlexCAN_T4.hpp
as the only FlexCAN include in flag-1 builds (never <FlexCAN_T4.h>), and the system header at flag-0.
The ld patch is per-machine until Q2 is resolved.

**R3 - Source address change.** Mainline claims 0x80/function 130 manually; the AgIsoStack build claims
0x90/function 128 per the fork (CANBus.ino:78,87) and may land elsewhere after arbitration. RC-side
gateway/app must key on PGN + ModID byte (it does today - handlers check buf[0] & 0x0F), not SA.
Verify RC app tolerance on bench (M1).

**R4 - 20 ms loop budget.** CANHardwareInterface::update() wants ~1 ms cadence; the mainline loop is
non-blocking so OK, but VT string building each loop plus TP sessions could stretch loop time and degrade
PID cadence (PIDtime). Bench-verify loop period during a VT pool upload.

**R5 - 32700 restart during an active TC/VT session** (SCB_AIRCR = 0x05FA0004 in ModuleConfig4): acceptable
(re-claims and re-connects), same behavior as today; note in docs.

**Q1** - Manufacturer code 0 in NAME (CANBus.ino:84): fine for AgIsoStack tools; some OEM terminals may care.
Obtain/borrow an AEF code before field release? (Decision, not code.)
**Q2** - Repo-local linker script via boards.local.txt override: experiment, then update NOTES (removes the
admin-edit of Program Files).
**Q3** - Keep an optional 0x5445 -> 0x5446 migration for fork test units, or accept defaults? (5 lines; recommend keep for one release.)
**Q4** - Arbitration policy for 0xFF03/0xFF04 vs TC (D1 proposes TC-wins-while-connected). Confirm with RC-app owner.
**Q5** - Checked-in isobus_config.h default 0 (recommended: mainline users unaffected; release ISOBUS hex
built with 1) - confirm the release process wants two hex artifacts.

---

## 7. Critical files for implementation

- F:\Documents\GitHub\RateControl\AOG_RC\Modules\Teensy Rate\RCteensy\CANBus.ino (transport merge - D1/D2)
- F:\Documents\GitHub\RC11-isobus\CANBus.ino (plugin + setupAgIsoStack to port)
- F:\Documents\GitHub\RC11-isobus\ISOBUS_TC.ino (TC client to port, D4 edits)
- F:\Documents\GitHub\RateControl\AOG_RC\Modules\Teensy Rate\RCteensy\RCteensy.ino (structs, globals, loop)
- F:\Documents\GitHub\RateControl\AOG_RC\Modules\Teensy Rate\RCteensy\Begin.ino (EEPROM/MachineSettings - D3)
- F:\Documents\GitHub\RC11-isobus\src\AgIsoStack\can_hardware_interface_single_thread.cpp (coexistence API, verified)
