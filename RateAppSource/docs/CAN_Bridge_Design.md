# CAN Bridge Design Document

## RateController — Replace ISOBUS Gateway with Native C# CAN Bridge

**Version:** 1.0
**Date:** February 2026
**Status:** Draft

---

## Table of Contents

1. [Overview](#1-overview)
2. [Goals and Requirements](#2-goals-and-requirements)
3. [Current Architecture](#3-current-architecture)
4. [Target Architecture](#4-target-architecture)
5. [What Is Removed](#5-what-is-removed)
6. [What Is Retained](#6-what-is-retained)
7. [New C# Components](#7-new-c-components)
8. [CAN Frame Protocol Reference](#8-can-frame-protocol-reference)
9. [Modified Existing Files](#9-modified-existing-files)
10. [Teensy Firmware Changes](#10-teensy-firmware-changes)
11. [CommMode Simplification](#11-commmode-simplification)
12. [Configuration UI Changes](#12-configuration-ui-changes)
13. [Testing Strategy](#13-testing-strategy)
14. [Migration Path](#14-migration-path)

---

## 1. Overview

### 1.1 Purpose

This document describes the design for replacing the external `IsobusGateway.exe` process and its ISOBUS (ISO 11783) stack with a native C# CAN bridge built directly into RateController. The Teensy firmware is simultaneously simplified by removing the ISO 11783-10 TC Client state machine.

The result is a two-path communication architecture:

- **Path A — Ethernet/UDP**: Teensy communicates directly with RateController over the local network using UDP broadcasts. No intermediary. Unchanged from the original design.
- **Path B — CAN Bus**: RateController communicates directly with Teensy modules over a USB-CAN adapter using a simple proprietary CAN frame protocol. No ISOBUS stack, no subprocess, no loopback UDP.

Either path works independently. Neither requires the other to be present or enabled. Both paths carry identical operational data using the same PGN byte format.

### 1.2 Background

The original ISOBUS integration (documented in `ISOBUS_Integration_Design.md`) added a C++ gateway process that bridged RateController's UDP protocol to an ISOBUS CAN bus using the AgIsoStack++ library. On top of the proprietary CAN bridge, a full ISO 11783-10 TC Client was implemented on the Teensy, adding DDOP, transport protocol, and TC Server state machines.

Experience with the system revealed that the ISOBUS overhead (address claiming, DDOP exchange, TC connection state machine, ~23% CPU usage on the gateway) adds significant complexity without providing a benefit over the simpler proprietary CAN protocol that was already working. The proprietary CAN frames (0xFF00–0xFF0B) already carry all the operational data needed. The TC Client path adds no additional functionality for this application.

### 1.3 Scope

| In Scope | Out of Scope |
|---|---|
| Remove `IsobusGateway.exe` subprocess | Changes to RateMap, AOG GPS, shapefile logic |
| Remove AgIsoStack++ dependency | Changes to RC product/relay/section control logic |
| Remove TC Client from Teensy firmware | Changes to RC rate algorithm |
| Add `CanBridgeComm.cs` to RateController | Firmware update (OTA) protocol |
| Add SLCAN serial interface in C# | Multi-module daisy-chain topology |
| Retain full Ethernet/UDP path unchanged | ISOBUS speed source (handled by AOG GPS) |
| Simplify CommMode to 0=Ethernet, 1=CAN | |
| Remove TC Client/DDOP/TP from Teensy | |

---

## 2. Goals and Requirements

### 2.1 Functional Requirements

| ID | Requirement |
|---|---|
| FR-01 | RateController shall communicate with Teensy modules over CAN bus without launching any external process. |
| FR-02 | RateController shall communicate with Teensy modules over Ethernet/UDP without a CAN adapter present. |
| FR-03 | Either communication path shall work independently. Connecting both simultaneously is not required. |
| FR-04 | The CAN path shall carry the same operational data as the Ethernet path: rate setpoints, flow calibration, relay states, PID settings, wheel speed config, sensor data, and module status. |
| FR-05 | Module discovery over CAN shall use the 0xFF08 identification heartbeat — no DDOP or TC handshake required. |
| FR-06 | Tractor speed shall be sourced from AOG GPS when available, not from the CAN bus. The CAN speed frames (0xFEF1/0xFE48/0xFE49) are optionally parsed as a fallback. |
| FR-07 | The CAN adapter type (SLCAN, InnoMaker, PCAN) and port shall be configurable in the RC Options menu. |
| FR-08 | The Teensy firmware shall support CommMode 0 (Ethernet) and CommMode 1 (CAN) only. |
| FR-09 | All existing PGN class byte formats (32400, 32401, 32500–32504, 32700) shall be unchanged. |
| FR-10 | Module config (PGN 32700) shall be sent over Ethernet when in CAN mode, using the module's known IP address. CommMode and pin configuration are stored in EEPROM on the Teensy. |

### 2.2 Non-Functional Requirements

| ID | Requirement |
|---|---|
| NF-01 | CAN receive-to-RC-handler latency shall be ≤ 50 ms (current Gateway path is ~30 ms). |
| NF-02 | The C# CAN bridge shall not block the UI thread. All CAN I/O runs on a background thread. |
| NF-03 | CPU usage of RateController with CAN bridge active shall be < 5% on a modern PC at 200 ms module update rate. |
| NF-04 | The solution shall not require the InnoMaker or PCAN DLLs if SLCAN mode is selected. |

---

## 3. Current Architecture

```
RateController (C#, WinForms .NET 4.8)
│
├─ UDPmodules (UDPComm)          ← recv:29999  send:28888  (Ethernet modules)
│   └─ 32400/32401 → product handlers, ModulesStatus
│
└─ IsobusComm                    ← recv:32701  send:32700  (localhost loopback)
    │
    └─ IsobusGateway.exe (C++ subprocess)
        │
        ├─ AgIsoStack++ (ISO 11783 stack)
        │   ├─ Address claiming
        │   ├─ TC Server state machine
        │   ├─ DDOP parsing
        │   └─ Transport Protocol (TP/ETP)
        │
        └─ CAN bus (250 kbps)
            │
            └─ Teensy 4.1
                ├─ TCClient.ino    ← ISO 11783-10 TC Client
                ├─ DDOP.ino        ← Device Description Object Pool
                ├─ TP.ino          ← Transport Protocol
                ├─ CANBus.ino      ← FlexCAN_T4 + address claim + PGN routing
                └─ CommMode 3/4 logic

PGN routing in each Send() method:
  if (IsobusEnabled) → IsobusComm.SendModuleCommand() → [loopback UDP] → Gateway → CAN
  else               → UDPmodules.Send()                                → Ethernet
```

**Known issues with current architecture:**
- Gateway CPU usage ~23% idle (AgIsoStack++ internal polling loops)
- Gateway subprocess must be launched, monitored, and killed by RC
- Two separate UDP loopback sockets (32700/32701) add latency and fragility
- TC Client on Teensy adds ~1800 lines of state machine code (TCClient, DDOP, TP)
- Address claiming on a private bus adds unnecessary complexity
- Gateway port conflict on Windows after improper shutdown requires restart

---

## 4. Target Architecture

```
RateController (C#, WinForms .NET 4.8)
│
├─ UDPmodules (UDPComm)               ← recv:29999  send:28888  (Ethernet — unchanged)
│   └─ 32400/32401 → product handlers, ModulesStatus
│
└─ CanBridgeComm                      ← NEW: replaces IsobusComm entirely
    │
    ├─ ICanInterface (abstraction)
    │   ├─ SlcanInterface.cs           ← SerialPort SLCAN  (no native DLL)
    │   ├─ InnoMakerInterface.cs       ← P/Invoke InnoMakerUsb2CanLib.dll
    │   └─ PcanInterface.cs            ← PCANBasic.cs wrapper
    │
    └─ CAN bus (250 kbps)
        │
        └─ Teensy 4.1
            ├─ CANBus.ino              ← proprietary frames only (0xFF00–0xFF0B)
            ├─ Send.ino                ← Ethernet send (CommMode 0)
            └─ CommMode 0=Ethernet, 1=CAN only

PGN routing in each Send() method (unchanged pattern, new flag name):
  if (Props.CanEnabled) → CanBridgeComm.SendModuleCommand() → CAN frames
  else                  → UDPmodules.Send()                  → Ethernet
```

**No subprocess. No loopback UDP. No ISOBUS stack.**

---

## 5. What Is Removed

### 5.1 From RateController (C#)

| Item | Location | Reason |
|---|---|---|
| `IsobusComm.cs` | `Classes/` | Replaced by `CanBridgeComm.cs` |
| Gateway subprocess launch/kill logic | `IsobusComm.cs` | No subprocess |
| Loopback UDP sockets (32700/32701) | `IsobusComm.cs` | No Gateway |
| `PGN32605` handler (Gateway Status) | `IsobusComm.cs` | Replaced by `CanBridgeStatus` |
| `PGN32610/32611` handler (TC implement connect/disconnect) | `IsobusComm.cs` | Replaced by 0xFF08 heartbeat tracking |
| `PGN32617` handler (TC Server status) | `IsobusComm.cs` | No TC Server |
| `PGN32604` handler (ISOBUS speed) | `IsobusComm.cs` | Speed from AOG GPS |
| `IsobusGateway.exe` + `gateway.json` | Build output | No subprocess |
| `InnoMakerUsb2CanLib.dll` / `InnoMakerUsb2CanLib64.dll` | Build output | Moved inside CanBridgeComm |
| `Props.IsobusEnabled` | `Props.cs` | Renamed `Props.CanEnabled` |

### 5.2 From IsobusGateway (C++)

The Gateway repository (`F:\Documents\GitHub\RateControl\Gateway`) is superseded. It may be archived but is no longer built or deployed. Specific components removed:

| Component | File | Lines |
|---|---|---|
| AgIsoStack++ library | `AgIsoStack/` | ~50,000 |
| TC Server state machine | `RCTaskControllerServer.cpp/.hpp` | ~700 |
| Gateway main + ISOBUS init | `Gateway.cpp` | ~800 |
| UDP bridge (loopback) | `UDPBridge.cpp/.hpp` | ~400 |
| SLCAN driver (C++) | `slcan_interface.cpp/.hpp` | ~300 |
| CMake build system | `CMakeLists.txt` | ~150 |

Total removed: ~52,000 lines of C++ / CMake.

### 5.3 From Teensy Firmware

| File | Lines | Action |
|---|---|---|
| `TCClient.ino` | ~400 | Delete |
| `DDOP.ino` | ~383 | Delete |
| `TP.ino` | ~665 | Delete (transport protocol only needed for DDOP) |
| `TCDefs.h` | ~124 | Delete |
| `VTClient.ino` | stub | Already deleted Feb 26, 2026 |
| `VTPool.ino` | stub | Already deleted |
| `VTDefs.h` | stub | Already deleted |
| `VTPoolData.h` | stub | Already deleted |

Lines removed from existing files:

| File | Lines Removed | What |
|---|---|---|
| `CANBus.ino` | ~150 | ISOBUS PGN handlers: 0xFEF8 (TC Status), 0xEC00/0xEB00 (TP CM/DT), 0xC700/0xC600 (ETP), 0xCBxx (Process Data), 0xFEF1/0xFE48/0xFE49 (Speed — Gateway handled this), 0xFE6E guard (already removed) |
| `RCteensy.ino` | ~60 | CommMode 3, CommMode 4 branches in main loop |
| `Begin.ino` | ~10 | TC Client begin call (already removed) |

Total removed from Teensy: ~1,800 lines.

---

## 6. What Is Retained

### 6.1 RateController — Unchanged

- All PGN classes: `PGN32400.cs`, `PGN32401.cs`, `PGN32500.cs`, `PGN32501.cs`, `PGN32502.cs`, `PGN32503.cs`, `PGN32504.cs`, `PGN32618.cs`, `PGN32700.cs`
- All byte formats, field offsets, CRC calculation — identical
- `UDPComm.cs` — Ethernet path completely unchanged
- `UDPmodules` (recv 29999 / send 28888) — unchanged
- All PGN `Send()` routing pattern — `if (CanEnabled)` replaces `if (IsobusEnabled)`, same structure
- `clsProduct`, `clsProducts`, `clsRelays`, `clsSections`, `clsSectionControl` — unchanged
- `Core.cs` — initialization updated to use `CanBridgeComm` instead of `IsobusComm`
- `frmMain.cs` — unchanged
- `RateMap/`, `AOG GPS PGNs`, `SQLite` — unchanged

### 6.2 Teensy Firmware — Unchanged

- `CANBus.ino` — 0xFF00/01/02/03/04/05/06/07/08/09/0A/0B frame handlers (core proprietary protocol)
- `Send.ino` — Ethernet UDP send (CommMode 0)
- `Receive.ino` — Ethernet UDP receive (CommMode 0)
- `clsModule.h` — module data structure
- `Sensors.ino` — sensor/PWM/flow logic
- `Relay.ino` — relay state application
- PID control logic

### 6.3 CAN Frame Protocol

The proprietary CAN frame protocol (Proprietary B, 250 kbps, 8-byte frames) is unchanged. All 0xFF00–0xFF0B frames continue to be used exactly as currently implemented. See Section 8 for the full reference.

---

## 7. New C# Components

### 7.1 Component Overview

```
CanBridgeComm.cs           — public API, mirrors IsobusComm public interface
CanFrameTranslator.cs      — bidirectional CAN frame ↔ PGN byte array translation
ICanInterface.cs           — hardware abstraction interface
SlcanInterface.cs          — SLCAN over SerialPort (pure managed, no DLL)
InnoMakerInterface.cs      — P/Invoke into InnoMakerUsb2CanLib.dll
PcanInterface.cs           — wrapper for PCANBasic.cs (Peak SDK)
```

All files go in `RateController/Classes/Can/`.

---

### 7.2 `ICanInterface.cs`

Hardware abstraction. All driver implementations implement this interface.

```csharp
namespace RateController.Classes.Can
{
    public interface ICanInterface : IDisposable
    {
        bool Open(string port, int bitrate);
        void Close();
        bool IsOpen { get; }
        bool Send(CanFrame frame);
        event EventHandler<CanFrame> FrameReceived;
    }

    public struct CanFrame
    {
        public uint Id;        // 29-bit CAN ID (Proprietary B uses extended frame)
        public byte Dlc;       // Data length code (0–8)
        public byte[] Data;    // Frame data, length = Dlc
        public bool IsExtended;
    }
}
```

---

### 7.3 `SlcanInterface.cs`

Pure managed C# implementation of the SLCAN serial protocol. No native DLL required. Works with any SLCAN-compatible USB-CAN adapter (SH-C30A, Canable, etc.).

**SLCAN protocol summary** (derived from existing `slcan_interface.cpp`):

| Command | Format | Description |
|---|---|---|
| Open channel | `O\r` | Opens CAN channel |
| Close channel | `C\r` | Closes CAN channel |
| Set bitrate | `S5\r` | 250 kbps (S0=10k, S5=250k, S6=500k, S8=1M) |
| Send extended | `T` + 8-char ID + 1-char DLC + data bytes + `\r` | e.g., `T1CF00B801020304050607\r` |
| Receive extended | `T` + same format | Received from adapter |
| Receive standard | `t` + 3-char ID + data | Standard (11-bit) frame |

**Key design points:**
- `SerialPort.DataReceived` event feeds a `StringBuilder` line buffer
- Complete frames (terminated by `\r`) are parsed in the event handler
- `FrameReceived` event raised from the `DataReceived` callback thread
- `CanBridgeComm` marshals to UI thread via `MainForm.BeginInvoke`
- `Send()` formats the SLCAN string and writes to the serial port synchronously

---

### 7.4 `InnoMakerInterface.cs`

P/Invoke wrapper for the existing `InnoMakerUsb2CanLib.dll` / `InnoMakerUsb2CanLib64.dll` already present in the output folder.

**Key P/Invoke signatures** (from the DLL's existing usage in the Gateway C++ code):

```csharp
[DllImport("InnoMakerUsb2CanLib.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int InnoMaker_OpenDevice(uint devIndex, uint canIndex, ref InnoMakerCanConfig config);

[DllImport("InnoMakerUsb2CanLib.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int InnoMaker_CloseDevice(uint devIndex, uint canIndex);

[DllImport("InnoMakerUsb2CanLib.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int InnoMaker_TransmitFrame(uint devIndex, uint canIndex, ref InnoMakerFrame frame);

[DllImport("InnoMakerUsb2CanLib.dll", CallingConvention = CallingConvention.Cdecl)]
private static extern int InnoMaker_ReceiveFrame(uint devIndex, uint canIndex, ref InnoMakerFrame frame, int timeout);
```

The receive call is blocking with a timeout. `InnoMakerInterface` runs a dedicated background thread calling `InnoMaker_ReceiveFrame` with a 50 ms timeout in a loop, raising `FrameReceived` for each frame.

Note: 32-bit vs 64-bit DLL selection is determined at runtime by `IntPtr.Size`.

---

### 7.5 `PcanInterface.cs`

Thin wrapper around `PCANBasic.cs` from the Peak PCAN-Basic SDK. Implements `ICanInterface` using `PCANBasic.Read()` in a polling background thread (10 ms interval) or via the PCAN event handle.

---

### 7.6 `CanFrameTranslator.cs`

Stateful translator. Maintains per-module frame caches to assemble multi-frame PGNs. Called by `CanBridgeComm`.

#### 7.6.1 Inbound Translation (CAN frame → PGN byte array)

The translator holds per-module state:

```csharp
private class ModuleState
{
    public byte[]  Last0xFF00 = null;   // SensorRateQty (8 bytes)
    public byte[]  Last0xFF01 = null;   // SensorPwmHz (8 bytes)
    public byte[]  Last0xFF02 = null;   // ModuleStatus (8 bytes)
    public byte[]  Last0xFF08 = null;   // ModuleIdentification (8 bytes)
    public byte[]  Last0xFF09 = null;   // WheelCounts (8 bytes)
    public DateTime Last0xFF08Time;     // For connect/disconnect detection
}
```

**Frame assembly rules** (matching Gateway.cpp logic):

| Trigger | Requires | Produces | Action |
|---|---|---|---|
| 0xFF00 received | Cache + wait for 0xFF01 | — | Store in `Last0xFF00` |
| 0xFF01 received | `Last0xFF00` present | PGN 32400 (15 bytes) | Assemble and raise |
| 0xFF02 received | `Last0xFF08` present | PGN 32401 (15 bytes) | Assemble and raise |
| 0xFF08 received | — | PGN 32401 if 0xFF02 cached, + connect event | Update `Last0xFF08`, raise connect if first time |
| 0xFF09 received | — | Merge into next PGN 32401 | Store wheel counts |
| 0xFF08 timeout (2 s) | — | Disconnect event | Raise `ModuleDisconnected` |

**PGN 32400 assembly** from 0xFF00 + 0xFF01:

```
Byte 0:  0x90  (PGN 32400 header lo)
Byte 1:  0x7E  (PGN 32400 header hi)
Byte 2:  0xFF00[0]   ModSenId
Byte 3:  0xFF00[1]   rateApplied lo
Byte 4:  0xFF00[2]   rateApplied mid
Byte 5:  0xFF00[3]   rateApplied hi
Byte 6:  0xFF00[4]   accumulatedQty lo
Byte 7:  0xFF00[5]   accumulatedQty mid
Byte 8:  0xFF00[6]   accumulatedQty hi
Byte 9:  0xFF00[7]   status (sensor connected)
         (NOTE: in the wire format byte 9 is the status byte from 0xFF00[7])
         (pwmSetting and pulseHz come from 0xFF01)
Byte 9:  0xFF01[1]   pwmSetting lo      ← overrides, see note
Byte 10: 0xFF01[2]   pwmSetting hi
Byte 11: 0xFF01[0]   status (from 0xFF00[7])
Byte 12: 0xFF01[3]   pulseHz lo
Byte 13: 0xFF01[4]   pulseHz hi
Byte 14: CRC8        sum of bytes 0–13
```

> **Note:** Cross-reference `Gateway.cpp` `onRatePwmHz()` to confirm exact byte mapping before coding. The Gateway code is the authoritative source for this assembly.

**PGN 32401 assembly** from 0xFF02 + 0xFF08 + 0xFF09:

```
Byte 0:  0x91  (PGN 32401 header lo)
Byte 1:  0x7E  (PGN 32401 header hi)
Byte 2:  0xFF02[0] & 0x0F             moduleId (lower nibble)
Byte 3:  0xFF02[2]                    pressure lo
Byte 4:  0xFF02[3]                    pressure hi
Byte 5:  0xFF02[4]                    wheelSpeed lo
Byte 6:  0xFF02[5]                    wheelSpeed hi
Byte 7:  0xFF09[1]                    wheelCounts lo   (0 if no 0xFF09)
Byte 8:  0xFF09[2]                    wheelCounts mid
Byte 9:  0xFF09[3]                    wheelCounts hi
Byte 10: 0xFF08[1]                    inoType (1 = Teensy Rate)
Byte 11: 0xFF08[2]                    inoId lo (firmwareVersion)
Byte 12: 0xFF08[3]                    inoId hi
Byte 13: (0xFF02[0] >> 4) & 0x07 |   wifiRSSI (bits 0-2) from 0xFF02[1]
          (0xFF02[0] & 0x30) |        workSwitch (bit0), ethernetConnected (bit4),
          (0xFF02[0] & 0x40)          goodPinConfig (bit5) — re-mapped to status byte
Byte 14: CRC8
```

> **Note:** Cross-reference `Gateway.cpp` `onModuleStatus()` and `onModuleIdentification()` for the exact status byte bit re-mapping. The C++ source is authoritative.

#### 7.6.2 Outbound Translation (PGN byte array → CAN frame(s))

`SendModuleCommand(byte[] pgn)` reads the PGN header (bytes 0–1 as LE uint16) and dispatches:

| PGN | CAN frames emitted | Split rule |
|---|---|---|
| 32500 | 0xFF03 (rate cmd, 8 bytes) + 0xFF0A (flow cal, 8 bytes) | Bytes 2–8 → 0xFF03; bytes 2, 9–11 → 0xFF0A |
| 32501 | 0xFF04 (relay states, 8 bytes) | Direct mapping bytes 2–8 |
| 32502 | 0xFF05 + 0xFF06 + 0xFF0B (PID, 8 bytes each) | Bytes 2–9 → 0xFF05; 2, 10–16 → 0xFF06; 2, 17–22 → 0xFF0B |
| 32504 | 0xFF07 (wheel config, 8 bytes) | Bytes 2–8 |
| 32700 | Not sent via CAN — see Section 7.8 | — |

> **Note:** Cross-reference `Gateway.cpp` `onRateSettingsReceived()`, `onRelayCommandReceived()`, `onPidSettingsReceived()`, `onWheelConfigReceived()` for exact byte-to-field mappings before coding.

#### 7.6.3 CAN ID Construction

All proprietary frames use **Proprietary B** (PGN 0xFF00–0xFFFF), extended 29-bit CAN ID:

```
CAN ID (29-bit) = (Priority << 26) | (0xFF << 8) | (PS byte) | (SA)
  Priority = 6 (0b110)
  DP = 0
  PF = 0xFF (255) — Proprietary B
  PS = low byte of PGN (e.g., 0x00 for 0xFF00)
  SA = source address (0x81 for RC/Gateway, 0x80 for module 0)
```

For frames sent by RC: SA = 0xF9 (industry convention for diagnostic tool, or use 0x81 matching old Gateway address). The Teensy checks module ID from the frame data, not the CAN SA, so SA choice does not affect functionality.

---

### 7.7 `CanBridgeComm.cs`

The main class. Public interface matches `IsobusComm` so all call sites in `Core.cs` and PGN `Send()` methods require only a name change.

#### 7.7.1 Public Interface

```csharp
public class CanBridgeComm
{
    // Lifecycle
    public bool Start(CanDriverType driver, string port);
    public void Stop();

    // Status (read by frmMenuOptions indicators)
    public bool CanAdapterConnected { get; }   // adapter open and frames flowing
    public bool ModuleDataReceiving { get; }   // 0xFF02 received within 2 s
    public DateTime LastModuleDataTime { get; }

    // Send path (called by PGN32500, 32501, 32502, 32504 Send() methods)
    public void SendModuleCommand(byte[] pgnData);

    // Module discovery (called by Core to display connected modules)
    public IReadOnlyList<CanModule> ConnectedModules { get; }
    public event EventHandler<CanModule> ModuleConnected;
    public event EventHandler<CanModule> ModuleDisconnected;
}

public class CanModule
{
    public byte ModuleId;
    public byte ModuleType;      // 1 = Teensy Rate
    public ushort FirmwareVersion;
    public DateTime LastSeen;
}
```

#### 7.7.2 Threading Model

```
Background thread (CAN receive loop):
  ICanInterface.FrameReceived event (from SerialPort.DataReceived or polling loop)
    → CanFrameTranslator.ProcessFrame(frame)
      → if PGN assembled:
          MainForm.BeginInvoke(() => {
              // exactly same calls as UDPmodules.HandleData():
              if (pgn == 32400) clsProduct.UDPcommFromArduino(data, pgn)
              if (pgn == 32401) Core.ModulesStatus.ParseByteData(data)
          })
      → if module connected/disconnected:
          MainForm.BeginInvoke(() => ConnectedModules update, event raise)

UI thread:
  SendModuleCommand(byte[]) — called from PGN Send() methods
    → CanFrameTranslator.TranslateOutbound(pgnData)
    → ICanInterface.Send(frame) for each resulting frame
    (CAN send is fast/non-blocking; no need to offload to background thread)
```

#### 7.7.3 Module Heartbeat Timeout

A `System.Timers.Timer` fires every 500 ms. For each tracked module, if `LastSeen > 2 s`, the module is removed from `ConnectedModules` and `ModuleDisconnected` is raised. This replaces PGN 32611 (TC disconnect) from the old Gateway.

---

### 7.8 Module Config (PGN 32700) Strategy

PGN 32700 (33 bytes) configures pin assignments, CommMode, relay type, and module ID. It currently goes directly from RC to the Teensy's IP address, bypassing the Gateway.

**Decision: Use EEPROM persistence with Ethernet-only config.**

- CommMode and pin configuration are set once via Ethernet (CommMode 0) and stored in Teensy EEPROM.
- When operating in CAN mode (CommMode 1), the Teensy uses EEPROM values.
- PGN 32700 is never sent over CAN. The existing Ethernet-direct send path is retained as-is.
- If a Teensy needs reconfiguration, it is temporarily set to CommMode 0 (Ethernet), reconfigured, then set back to CommMode 1 (CAN) — all via PGN 32700 over UDP.

This means RC must know the Teensy's IP address (192.168.1.(ID+50)) to send PGN 32700 even when in CAN mode. The existing `UDPmodules.Send()` path already handles this when `CanEnabled` is false. RC can temporarily switch the send path just for PGN 32700 sends regardless of current mode.

---

## 8. CAN Frame Protocol Reference

All frames are 8 bytes, 250 kbps, extended 29-bit CAN ID, Proprietary B (PF=0xFF).

### 8.1 Teensy → RC (Upstream)

#### 0xFF00 — Sensor Rate / Quantity (200 ms cyclic)

| Byte | Field | Encoding |
|---|---|---|
| 0 | ModSenId | bits 0–3 = ModuleID, bits 4–7 = SensorID |
| 1 | rateApplied lo | uint24 LE, units × 0.001 UPM |
| 2 | rateApplied mid | |
| 3 | rateApplied hi | |
| 4 | accumulatedQty lo | uint24 LE, units × 0.1 |
| 5 | accumulatedQty mid | |
| 6 | accumulatedQty hi | |
| 7 | status | bit 0 = sensor connected (CommTime < 4 s) |

#### 0xFF01 — Sensor PWM / Hz (200 ms cyclic, sent with 0xFF00)

| Byte | Field | Encoding |
|---|---|---|
| 0 | ModSenId | |
| 1 | pwmSetting lo | int16 LE (signed) |
| 2 | pwmSetting hi | |
| 3 | pulseHz lo | uint16 LE, Hz × 0.1 |
| 4 | pulseHz hi | |
| 5–7 | reserved | |

#### 0xFF02 — Module Status (200 ms cyclic)

| Byte | Field | Encoding |
|---|---|---|
| 0 | moduleIdStatus | bits 0–3 = ModuleID; bit4=workSwitch; bit5=ethernetConnected; bit6=goodPinConfig |
| 1 | wifiStrength | bits 0–2 |
| 2 | pressure lo | uint16 LE, kPa |
| 3 | pressure hi | |
| 4 | wheelSpeed lo | uint16 LE, × 0.1 km/h |
| 5 | wheelSpeed hi | |
| 6–7 | reserved | |

#### 0xFF08 — Module Identification (500 ms cyclic)

| Byte | Field | Encoding |
|---|---|---|
| 0 | moduleIdSensorCount | bits 0–3 = ModuleID, bits 4–7 = SensorCount |
| 1 | moduleType | 1 = Teensy Rate |
| 2 | firmwareVersion lo | uint16 LE (DDMMY numeric) |
| 3 | firmwareVersion hi | |
| 4–7 | reserved | |

**Module discovery is based entirely on this frame.** First reception → `ModuleConnected` event. Absence > 2 s → `ModuleDisconnected` event.

#### 0xFF09 — Wheel Counts (500 ms cyclic, during calibration)

| Byte | Field | Encoding |
|---|---|---|
| 0 | moduleId | lower nibble |
| 1 | wheelCounts lo | uint24 LE |
| 2 | wheelCounts mid | |
| 3 | wheelCounts hi | |
| 4–7 | reserved | |

---

### 8.2 RC → Teensy (Downstream)

#### 0xFF03 — Rate Command (100 ms cyclic)

| Byte | Field | Encoding |
|---|---|---|
| 0 | ModSenId | bits 0–3 = ModuleID, bits 4–7 = SensorID |
| 1 | rateSetpoint lo | uint24 LE, × 0.001 UPM |
| 2 | rateSetpoint mid | |
| 3 | rateSetpoint hi | |
| 4 | manualPwm lo | int16 LE (signed) |
| 5 | manualPwm hi | |
| 6 | command | bit0=resetQty, bits1–3=controlType(0–5), bit4=MasterOn, bit5=MasterOnPos, bit6=AutoOn, bit7=CalibrationOn |
| 7 | reserved | |

#### 0xFF04 — Relay Command (on-change)

| Byte | Field |
|---|---|
| 0 | moduleId (lower nibble) |
| 1 | relayStatesLo (sections 0–7) |
| 2 | relayStatesHi (sections 8–15) |
| 3 | powerRelaysLo |
| 4 | powerRelaysHi |
| 5 | invertedLo |
| 6 | invertedHi |
| 7 | reserved |

#### 0xFF05 — PID Settings Part 1 (on-change)

| Byte | Field |
|---|---|
| 0 | ModSenId |
| 1 | maxPwm % (255 × val / 100) |
| 2 | minPwm % |
| 3 | Kp (scrollbar encoding: pow(1.1, val-120)) |
| 4 | Ki (same) |
| 5 | deadband (% × 10) |
| 6 | brakepoint % |
| 7 | slowAdjust % |

#### 0xFF06 — PID Settings Part 2 (on-change)

| Byte | Field |
|---|---|
| 0 | ModSenId |
| 1 | slewRate |
| 2 | maxIntegral (× 10) |
| 3 | timedAdjust lo (ms) |
| 4 | timedAdjust hi |
| 5 | timedPause lo (ms) |
| 6 | timedPause hi |
| 7 | pidTime (ms) |

#### 0xFF07 — Wheel Speed Config (on-change)

| Byte | Field |
|---|---|
| 0 | moduleId (lower nibble) |
| 1 | gpioPin (0–50, 255=disabled) |
| 2 | calibration lo (pulses/km uint24 LE) |
| 3 | calibration mid |
| 4 | calibration hi |
| 5 | command (bit0=eraseCounts) |
| 6–7 | reserved |

#### 0xFF0A — Flow Calibration (on-change)

| Byte | Field |
|---|---|
| 0 | ModSenId |
| 1 | flowCal lo (uint24 LE × 0.001) |
| 2 | flowCal mid |
| 3 | flowCal hi |
| 4–7 | reserved |

#### 0xFF0B — PID Settings Part 3 (on-change)

| Byte | Field |
|---|---|
| 0 | ModSenId |
| 1 | timedMinStart |
| 2 | pulseMinHz (Hz × 10 → stored as PulseMax µs on Teensy) |
| 3 | pulseMaxHz lo (→ stored as PulseMin µs on Teensy) |
| 4 | pulseMaxHz hi |
| 5 | pulseSampleSize |
| 6–7 | reserved |

---

## 9. Modified Existing Files

### 9.1 `Classes/Core.cs`

| Change | Detail |
|---|---|
| Replace `IsobusComm` field | `public static CanBridgeComm CanBridgeComm { get; private set; }` |
| `Initialize()` | Do not instantiate `IsobusComm`. Wire `CanBridgeComm.ModuleConnected/Disconnected` events. |
| Auto-start on load | If `Props.CanEnabled` is true, call `CanBridgeComm.Start()` during init. |
| `MainTimer_Elapsed` | Update status indicators from `CanBridgeComm` properties instead of `IsobusComm`. |

### 9.2 `Classes/Props.cs`

| Change | Detail |
|---|---|
| Rename `IsobusEnabled` | → `CanEnabled` (or add as new property, deprecate old) |
| Retain `CanDriver` enum | `SLCAN`, `InnoMaker`, `PCAN` — unchanged |
| Retain `CanPort` | COM port string — unchanged |
| Retain `ShowCanDiagnostics` | Debug toggle — unchanged |

### 9.3 `PGNs/PGN32500.cs` (and 32501, 32502, 32504)

Each `Send()` method changes one line:

```csharp
// Before:
if (Props.IsobusEnabled && Core.IsobusComm != null)
    Core.IsobusComm.SendModuleCommand(cData);
else
    Core.UDPmodules.Send(cData);

// After:
if (Props.CanEnabled && Core.CanBridgeComm != null)
    Core.CanBridgeComm.SendModuleCommand(cData);
else
    Core.UDPmodules.Send(cData);
```

### 9.4 `PGNs/PGN32700.cs`

Module config is always sent over Ethernet (see Section 7.8). No change to routing logic — it already sends directly via `UDPmodules` to the module's specific IP. Confirm that the existing send does not go through `IsobusComm` and retain that behavior.

### 9.5 `Menu/frmMenuOptions.cs`

See Section 12 for full UI changes. The ISOBUS tab title and wording changes; all control bindings update to reference `CanBridgeComm` instead of `IsobusComm`.

### 9.6 `RateController.csproj`

| Change | Detail |
|---|---|
| Remove `<Content>IsobusGateway.exe</Content>` | No subprocess |
| Remove `<Content>gateway.json</Content>` | No gateway config |
| Retain `<Content>InnoMakerUsb2CanLib.dll</Content>` | Used by `InnoMakerInterface.cs` |
| Retain `<Content>InnoMakerUsb2CanLib64.dll</Content>` | |
| Add new files in `Classes/Can/` | `CanBridgeComm.cs`, `CanFrameTranslator.cs`, `ICanInterface.cs`, `SlcanInterface.cs`, `InnoMakerInterface.cs`, `PcanInterface.cs` |

---

## 10. Teensy Firmware Changes

### 10.1 Files to Delete (in Arduino IDE: right-click tab → Delete)

| File | Reason |
|---|---|
| `TCClient.ino` | ISO 11783-10 TC Client state machine — no TC Server to connect to |
| `DDOP.ino` | Device Description Object Pool — no TC Server to send it to |
| `TP.ino` | Transport Protocol — only needed to send DDOP |
| `TCDefs.h` | TC/DDI type definitions — unused after above deletions |

### 10.2 `CANBus.ino` — Handlers to Remove

Remove the `case` branches or `if` blocks handling these PGN/PF values (they will no longer appear on the bus):

| PGN/PF | Name | Reason for removal |
|---|---|---|
| `pf == 0xFE && ps == 0xF8` (0xFEF8) | TC Status | No TC Server present |
| `pf == 0xEC` (0xEC00) | TP Connection Management | No DDOP transport |
| `pf == 0xEB` (0xEB00) | TP Data Transfer | No DDOP transport |
| `pf == 0xC7` (0xC700) | ETP Connection Management | No DDOP transport |
| `pf == 0xC6` (0xC600) | ETP Data Transfer | No DDOP transport |
| `pf == 0xCB` (0xCBxx) | Process Data (TC DDI) | No TC Server present |
| `pf == 0xFE && ps == 0xF1` (0xFEF1) | Machine Selected Speed | Gateway handled speed; RC gets it from AOG GPS |
| `pf == 0xFE && ps == 0x48` (0xFE48) | Wheel Based Speed | Same |
| `pf == 0xFE && ps == 0x49` (0xFE49) | Ground Based Speed | Same |

Address claiming (0xEE00, 0xEA00) may be retained as a minimal stub or removed entirely since this is a private bus with no address conflicts when the Gateway is gone.

### 10.3 `RCteensy.ino` — CommMode Cleanup

**Remove** the CommMode 3 and CommMode 4 branches from the main loop and any switch/case structures:

```cpp
// Remove these blocks:
case 3:  // TC Client only
    CANBus_MaintainAddress();
    CANBus_Receive();
    TP_Update();
    TCClient_Update();
    break;

case 4:  // UDP + TC Client
    CANBus_MaintainAddress();
    CANBus_Receive();
    TP_Update();
    TCClient_Update();
    ReceiveUDP();
    SendComm();
    break;
```

**Retain:**

```cpp
case 0:  // Ethernet only
    ReceiveUDP();
    SendComm();
    break;

case 1:  // CAN proprietary only
    CANBus_Update();
    break;

case 2:  // Both (optional — may retain for diagnostics)
    CANBus_Update();
    ReceiveUDP();
    SendComm();
    break;
```

### 10.4 `Begin.ino`

Confirm that `TCClient_Begin()` and `VTClient_Begin()` calls are already removed (both were removed Feb 26, 2026 per MEMORY.md). No additional changes needed.

### 10.5 `Receive.ino`

The guards `if (MDL.CommMode != 1 && MDL.CommMode != 3)` on PGN 32500/32501/32502/32504 handlers become `if (MDL.CommMode != 1)` — remove the `!= 3` condition since CommMode 3 no longer exists.

---

## 11. CommMode Simplification

| CommMode | Old Name | New Name | Behaviour |
|---|---|---|---|
| 0 | UDP only | **Ethernet** | Teensy uses NativeEthernet directly with RateController |
| 1 | CAN Proprietary only | **CAN** | Teensy uses CAN bus; RC uses `CanBridgeComm` |
| 2 | UDP + CAN Proprietary | **Both** (optional) | Both active simultaneously — useful for diagnostics |
| 3 | TC Client only | *Removed* | |
| 4 | UDP + TC Client | *Removed* | |

CommMode is still stored in Teensy EEPROM (`MDL.CommMode`). The PGN 32700 module config byte 31 still sets it. Default should be `0` (Ethernet) for new modules until CAN is configured.

---

## 12. Configuration UI Changes

### 12.1 `frmMenuOptions` — CAN Tab

The existing ISOBUS tab is repurposed. Changes:

| Control | Old (ISOBUS) | New (CAN Bridge) |
|---|---|---|
| Tab title | "ISOBUS" | "CAN Bus" |
| Section heading | "ISOBUS Gateway" | "CAN Adapter" |
| Enable checkbox label | "Enable ISOBUS" | "Enable CAN Bus" |
| Driver selection | SLCAN / InnoMaker / PCAN radio buttons | Unchanged |
| COM port | Dropdown + refresh | Unchanged (SLCAN only) |
| Diagnostics checkbox | "Show Gateway Console" | "Show CAN Diagnostics" |
| `lbConnected` label | "Gateway connected" | "CAN adapter connected" |
| `lbDriverFound` label | "Module data receiving" | "Module data receiving" (unchanged) |

**Removed controls:**
- Any display of TC address, VT address, TC connected count (TC-specific status from PGN 32605/32617)

**Retained logic:**
- Driver save-before-start ordering
- Start/stop sequencing with `Props.CanEnabled` flag
- Indicator colour logic (green if data within 2 s)

### 12.2 No Changes

- "Rate Control" tab — unchanged
- "Products" tab — unchanged
- "Sections" tab — unchanged
- All other menu forms — unchanged

---

## 13. Testing Strategy

### 13.1 Unit Tests — `CanFrameTranslator`

Write standalone tests (NUnit or MSTest) for `CanFrameTranslator` with known byte vectors:

1. Feed 0xFF00 + 0xFF01 frames → verify assembled PGN 32400 matches byte-for-byte the output of `UDPmodules.HandleData()` for the same data in Ethernet mode
2. Feed 0xFF02 + 0xFF08 frames → verify PGN 32401 matches Ethernet mode
3. Feed PGN 32500 → verify 0xFF03 + 0xFF0A frame bytes
4. Feed PGN 32502 → verify 0xFF05 + 0xFF06 + 0xFF0B bytes
5. Feed 0xFF08 twice, then stop for 2.5 s → verify `ModuleConnected` then `ModuleDisconnected` events

Reference byte vectors: capture live traffic from the current Gateway → RC loopback UDP using Wireshark (localhost), then compare against `CanFrameTranslator` output for the same CAN input.

### 13.2 Integration Test — SLCAN Loopback

1. Connect two USB-CAN adapters in loopback (or use a SLCAN loopback capability)
2. Send known CAN frames via one port; verify `CanBridgeComm` raises correct PGNs on the other
3. Call `SendModuleCommand()` with PGN 32500; verify correct CAN frame received on second port

### 13.3 End-to-End Test — Teensy CommMode 1

1. Set Teensy CommMode 1 (CAN only) via PGN 32700 over Ethernet
2. Disconnect Ethernet from Teensy
3. Enable CAN in RC, select SLCAN + correct COM port
4. Verify module appears in RC (equivalent to blue status light in current system)
5. Send rate setpoint from RC; verify Teensy applies it (sensor output changes)
6. Verify relay command from RC activates sections on Teensy

### 13.4 End-to-End Test — Teensy CommMode 0

1. Disable CAN in RC
2. Set Teensy CommMode 0 (Ethernet)
3. Verify module appears via UDP exactly as before (no regression)

### 13.5 Parallel Mode Test — CommMode 2

1. Enable both CAN and Ethernet in RC; set Teensy CommMode 2
2. Verify no duplicate data processing (both paths update the same product state, last-write-wins is acceptable; or gate one path in RC when both are active)

---

## 14. Migration Path

The changes can be made in the following order, with each step independently testable:

### Step 1 — Create CAN Interface Layer (no RC changes yet)

Create `ICanInterface.cs` and `SlcanInterface.cs`. Write a standalone console test app that opens the SLCAN port, prints received frames, and sends a test frame. Verify CAN frames appear in Cangaroo.

**Deliverable:** SLCAN reads and writes CAN frames correctly from C#.

### Step 2 — Create `CanFrameTranslator` with Unit Tests

Implement translator with full unit test coverage using byte vectors captured from the current Gateway. No RC integration yet.

**Deliverable:** Translator produces correct PGN bytes for all frame types.

### Step 3 — Create `CanBridgeComm` and Wire into RC

Add `CanBridgeComm.cs`. Update `Core.cs` to instantiate it. Update PGN `Send()` routing. Update `frmMenuOptions` CAN tab. Keep `IsobusComm.cs` present but unused (or guard behind a `#if` define).

**Deliverable:** RC can enable CAN bridge; modules show up; rate commands work. IsobusGateway.exe is no longer launched.

### Step 4 — Verify Ethernet Path Unaffected

Disable CAN, use Ethernet-only Teensy (CommMode 0). Confirm everything works identically to before this change.

**Deliverable:** No regression on Ethernet path.

### Step 5 — Clean Up

Remove `IsobusComm.cs`. Remove `IsobusGateway.exe` and `gateway.json` from project build. Update `RateController.csproj`. Update `CLAUDE.md` and `MEMORY.md`.

**Deliverable:** No ISOBUS artefacts remain in RC build.

### Step 6 — Teensy Firmware Cleanup

Delete `TCClient.ino`, `DDOP.ino`, `TP.ino`, `TCDefs.h` from Arduino project. Remove CommMode 3/4. Remove ISOBUS PGN handlers from `CANBus.ino`. Test with CommMode 0 and CommMode 1.

**Deliverable:** Firmware compiles and functions with ~1,800 fewer lines. CommMode 3/4 no longer selectable.

### Step 7 — InnoMaker and PCAN Interfaces (optional)

Add `InnoMakerInterface.cs` and `PcanInterface.cs` if those adapters are needed. SLCAN covers the primary use case.

---

## Appendix A — File Inventory

### New Files

| File | Location | Purpose |
|---|---|---|
| `ICanInterface.cs` | `Classes/Can/` | Hardware abstraction interface |
| `SlcanInterface.cs` | `Classes/Can/` | SLCAN over SerialPort |
| `InnoMakerInterface.cs` | `Classes/Can/` | InnoMaker USB2CAN P/Invoke |
| `PcanInterface.cs` | `Classes/Can/` | Peak PCAN wrapper |
| `CanFrameTranslator.cs` | `Classes/Can/` | Bidirectional CAN ↔ PGN translation |
| `CanBridgeComm.cs` | `Classes/Can/` | Public API, module tracking, threading |

### Modified Files

| File | Change Summary |
|---|---|
| `Classes/Core.cs` | Replace `IsobusComm` with `CanBridgeComm` |
| `Classes/Props.cs` | Rename `IsobusEnabled` → `CanEnabled` |
| `PGNs/PGN32500.cs` | Route via `CanBridgeComm` when `CanEnabled` |
| `PGNs/PGN32501.cs` | Same |
| `PGNs/PGN32502.cs` | Same |
| `PGNs/PGN32504.cs` | Same |
| `Menu/frmMenuOptions.cs` | Rename ISOBUS tab → CAN Bus; wire to `CanBridgeComm` |
| `RateController.csproj` | Remove gateway files, add `Classes/Can/*.cs` |
| `Teensy/CANBus.ino` | Remove ISOBUS PGN handlers |
| `Teensy/RCteensy.ino` | Remove CommMode 3, 4 |
| `Teensy/Receive.ino` | Update CommMode guards |

### Deleted Files

| File | Location |
|---|---|
| `Classes/IsobusComm.cs` | RateController |
| `IsobusGateway.exe` | Build output |
| `gateway.json` | Build output |
| `TCClient.ino` | Teensy firmware |
| `DDOP.ino` | Teensy firmware |
| `TP.ino` | Teensy firmware |
| `TCDefs.h` | Teensy firmware |

### Superseded Repository

| Repository | Status |
|---|---|
| `F:\Documents\GitHub\RateControl\Gateway` | Superseded — archive only |

---

## Appendix B — CAN Bus Hardware Notes

- **Adapter:** SH-C30A with SLCAN firmware (not Candlelight — Candlelight TX was broken in testing)
- **Bus speed:** 250 kbps
- **Teensy transceiver:** MCP2562-E/P on CAN1 (TX=pin 22, RX=pin 23, STBY=pin 6 must be LOW)
- **Termination:** 120 Ω at each end of the bus
- **Serial baud (SLCAN):** 115,200 bps

---

*End of document.*
