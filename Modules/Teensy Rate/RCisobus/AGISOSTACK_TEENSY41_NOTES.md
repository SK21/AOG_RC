# AgIsoStack-Arduino notes for this sketch

Build target:

- Board: Teensy 4.1
- Optimize: Smallest Code with LTO
- Library: local copy in `src/AgIsoStack`

This sketch uses its own lightweight CAN1 adapter in `CANBus.ino`. The stock
`flex_can_t4_plugin.cpp` from AgIsoStack-Arduino reserves RAM for CAN1, CAN2,
and CAN3 even when only CAN1 is used. On this project that makes Teensy 4.1
RAM1 overflow.

This sketch includes a local copy of AgIsoStack in `src/AgIsoStack`, with the
unused `flex_can_t4_plugin.cpp` disabled. This keeps the project independent of
the stock library installed in `Documents/Arduino/libraries`.

If Arduino IDE still compiles the stock AgIsoStack library and reports RAM1
overflow, restart Arduino IDE and make sure `RCteensy.ino` includes:

`#include "src/AgIsoStack/AgIsoStack.hpp"`

As an alternative, disable this unused file in the installed library:

`AgIsoStack/src/flex_can_t4_plugin.cpp`

For example, rename it to:

`flex_can_t4_plugin.cpp.disabled`

The sketch does not use that file; it only needs the rest of AgIsoStack and the
`FlexCAN_T4.hpp` header included with the library.

ISOBUS mode:

- `ETHERNET_COMM_ENABLED` is set to `0`; NativeEthernet, UDP telemetry, and UDP
  firmware update code are left out of the build.
- Virtual Terminal is the local machine setup screen. The current controls edit
  product mode, target dose in L/ha for automatic mode, manual PWM for manual
  mode, meter calibration, section count, individual section widths, and master
  state.
- In `ISOBUS_TC_MODE`, the old AgOpenGPS proprietary CAN command handlers are
  ignored so machine settings are not overwritten outside VT/TC.
- Task Controller receives section commands through
  `SetpointCondensedWorkState1_16` and reports
  `ActualCondensedWorkState1_16` plus actual/target volume-per-area rate and
  actual speed values.
- Changing section count or section width from VT marks the DDOP geometry for
  re-upload so the Task Controller sees the new machine width.
- Speed is read from ISO speed messages using AgIsoStack's
  `SpeedMessagesInterface`. Machine Selected Speed is preferred, then
  ground-based speed, then wheel-based speed. If no ISOBUS speed is available,
  the local wheel-speed input is used when configured.
- Automatic rate calculation uses:
  `TargetUPM = L/ha * km/h * activeWidthM / 600`, where `activeWidthM` is the
  sum of the sections currently enabled by TC.

---

## Fixes required to compile on Teensyduino 1.59 (May 2026)

### Fix 1 — FlexCAN_T4 return type mismatch

The system Teensyduino library (`FlexCAN_T4.h`) declares `struct2queueTx` as
`void`. The local copy (`src/AgIsoStack/FlexCAN_T4.hpp` / `.tpp`) originally
declared it as `bool`. Both files share the include guard `_FLEXCAN_T4_H_`, so
whichever is included first wins, causing a declaration/definition mismatch.

**Files edited** (local copies in `src/AgIsoStack/`):

`FlexCAN_T4.hpp` line 605 — change declaration from:
```cpp
bool struct2queueTx(const CAN_message_t &msg);
```
to:
```cpp
void struct2queueTx(const CAN_message_t &msg);
```

`FlexCAN_T4.tpp` — change the function definition and its four call sites in
`write()` from returning the bool result of `struct2queueTx` to:
```cpp
FCTP_FUNC void FCTP_OPT::struct2queueTx(const CAN_message_t &msg) {
  if (FLEXCANb_ESR1(_bus) & 0x20) return;
  if ( txBuffer.size() == txBuffer.capacity() ) return;
  uint8_t buf[sizeof(CAN_message_t)];
  memmove(buf, &msg, sizeof(msg));
  txBuffer.push_back(buf, sizeof(CAN_message_t));
}
```
Each call site in `write()` changes from `return struct2queueTx(msg_copy);` to:
```cpp
struct2queueTx(msg_copy);
return -1;
```

---

### Fix 2 — RAM1 overflow: AgIsoStack too large for ITCM

The Teensyduino linker script places all `.text*` code in ITCM (512 KB).
AgIsoStack's 41 source files overflow ITCM by ~52 KB. Additionally,
AgIsoStack's const data (`.rodata*`) and zero-init globals (`.bss*`) add
~55 KB to DTCM. `teensy_size` counts ITCM + DTCM together as RAM1 (512 KB
total), so both must be reduced.

**File to edit (requires admin/elevated Notepad):**
`C:\Program Files (x86)\Arduino\hardware\teensy\avr\cores\teensy4\imxrt1062_t41.ld`

Add `*AgIsoStack*(.text*)` and `*AgIsoStack*(.rodata*)` to `.text.code`
(runs AgIsoStack code and const data from XIP flash instead of ITCM/DTCM):
```
    .text.code : {
        KEEP(*(.startup))
        *(.flashmem*)
        *AgIsoStack*(.rodata*)
        *AgIsoStack*(.text*)      ← add these two lines
        . = ALIGN(4);
        ...
    } > FLASH
```

Add `*AgIsoStack*(.bss*)` to `.bss.dma`
(moves AgIsoStack zero-init globals to OCRAM instead of DTCM):
```
    .bss.dma (NOLOAD) : {
        *(.hab_log)
        *(.dmabuffers)
        *AgIsoStack*(.bss*)       ← add this line
        . = ALIGN(32);
    } > RAM
```

**Result after all fixes:** RAM1 free ~4,768 bytes; FLASH free ~7.4 MB; RAM2 free ~511 KB.
