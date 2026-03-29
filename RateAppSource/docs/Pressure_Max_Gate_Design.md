# Pressure-Based Output Max Gate — Design Notes

**Date:** 2026-03-20
**Status:** Research / Design (not yet implemented)

---

## Problem

The PID control loop has no visibility into system pressure. If a blockage, frozen filter, or kinked line causes pressure to rise, the PID will keep increasing PWM output trying to reach the target UPM — making the over-pressure condition worse. A pressure-based max gate intercepts this before hardware damage occurs.

---

## How Pressure and Output Are Coupled

- Higher PWM → more open valve / faster pump → higher flow → higher line pressure
- Pressure can rise **without** a corresponding flow increase (blocked nozzle, filter, etc.)
- Flow sensor measures volume (UPM); pressure reveals hydraulic **load**
- The PID has no safety limit on pressure — it will blindly increase output into a dangerous state

---

## Architecture: Two-Layer Gate

### Layer 1 — Firmware Hard Gate (fast, comms-independent)

- Stores a `MaxPressureReading` threshold (raw ADC uint16) in `ModuleConfig`
- Checked inside `AdjustFlow()` every 50ms **before** writing to the PWM pin
- Works even if comms to PC are lost (stored in EEPROM)
- `0xFFFF` sentinel = gate disabled

**Behavior when triggered:**

| Control Type | Gate Action |
|---|---|
| `StandardValve_ct`, `Motor_ct`, `Fan_ct` | Force PWM = 0 (stop output) |
| `ComboClose_ct`, `TimedCombo_ct` | Force PWM = -255 (active close) |

- Resets `IntegralSum[i]` to prevent integral windup while gate is active
- Gate releases immediately when `PressureReading` drops back below threshold
- Optional: add 5% hysteresis band on re-enable to prevent flutter on noisy sensors

### Layer 2 — PC Alarm Gate (slower, user-visible)

- PC compares current calibrated pressure (engineering units) against `MaxPressure` setting
- If exceeded: fires "OVER PRESSURE" alarm (distinct from rate off-alarm)
- Optionally sends `TargetUPM = 0` via PGN32500 to halt the sensor from the app side

---

## Where Pressure Data Lives

| Location | Format | Update Rate |
|---|---|---|
| `Analog.ino` — `PressureReading` (global uint) | Raw ADC uint16 | Every 50ms loop |
| ADS1115 path | 15-bit (int16 >> 1), max 32767 | |
| Teensy analog pin path | 12-bit, max 4095 | |
| PGN32401 bytes 3-4 | Raw uint16 sent to PC | Every 200ms |
| `Props.cs` — `PressureCals[]` | 2-point linear cal | On settings change |

**Key gap:** The firmware never converts to engineering units. Calibration lives on the PC only.

---

## Threshold Conversion: PC → Firmware

PC calibration formula (forward):
```
M = (MaxPres - MinPres) / (MaxVol - MinVol)
B = MinPres - M × MinVol
Pressure = M × Reading + B   (only if Reading ≥ ZeroValue)
```

Inverse (PC → raw threshold for firmware):
```
MaxPressureReading = (MaxPressure - B) / M
                   = (MaxPressure - MinPres) / M + MinVol
```

**Example** using `Default.rcs` calibration:
```
MinVol  = 1066,  MinPres = 20 PSI
MaxVol  = 2098,  MaxPres = 50 PSI
M = (50 - 20) / (2098 - 1066) = 0.02907 PSI/count

User sets MaxPressure = 45 PSI:
MaxPressureReading = (45 - 20) / 0.02907 + 1066 = 1926 (raw ADC)
```

This raw threshold is what is stored and compared in firmware.

---

## `0xFFFF` Disabled Sentinel

Safe because:
- ADS1115 path: 15-bit max = 32767 (0x7FFF) — well below 0xFFFF
- Teensy 12-bit ADC path: max = 4095 (0x0FFF) — well below 0xFFFF

Neither hardware path can produce a reading of 0xFFFF naturally.

---

## New PGN: 32503 (RC → Module, Pressure Gate Config)

Chosen because 32503 is unused. Follows the same pattern as PGN32502/32504.

```
Byte 0:  Header Lo  (247 = 0xF7)
Byte 1:  Header Hi  (126 = 0x7E)
Byte 2:  Module ID
Byte 3:  MaxPressureReading Lo  (uint16, raw ADC)
Byte 4:  MaxPressureReading Hi
Byte 5:  CRC
```

- `0xFFFF` = gate disabled
- Sent by PC when settings are saved and periodically (like PID settings)
- Stored in Teensy EEPROM so it persists across power cycles

---

## Implementation Touch Points

### Firmware (`RCteensy`)

| File | Change |
|---|---|
| `RCteensy.ino` | Add `uint16_t MaxPressureReading = 0xFFFF` to `ModuleConfig` struct |
| `Motor.ino` — `AdjustFlow()` | Add gate check before each `SetPWM()` call; force close/stop if triggered |
| `PID.ino` — `SetPWM()` | Reset `IntegralSum[i]` when gate active (prevent windup) |
| `Receive.ino` | Add `case 32503:` parser for new PGN |
| `Begin.ino` | Add EEPROM save/load for `MaxPressureReading` |
| `CANBus.ino` | Add CAN frame handling for new PGN |
| `Send.ino` | Optionally report gate-active state as a status bit in PGN32401 |

### Windows App (`RateController`)

| File | Change |
|---|---|
| `Props.cs` | Add `MaxPressure` per-module setting (engineering units, persisted to .rcs) |
| `frmMenuPressure.cs` | Add MaxPressure input field; compute and send raw threshold on save |
| New `PGN32503.cs` | Build and send the pressure gate threshold PGN |
| `clsAlarm.cs` | Add over-pressure alarm case (separate from rate off-alarm) |

---

## Gate Logic Pseudocode (Firmware)

```cpp
// In AdjustFlow(), per-sensor loop:
if (MDL.MaxPressureReading != 0xFFFF && PressureReading > MDL.MaxPressureReading)
{
    // Over-pressure: close/stop this sensor's output
    IntegralSum[i] = 0;  // reset integral to prevent windup
    switch (Sensor[i].ControlType)
    {
    case ComboClose_ct:
    case TimedCombo_ct:
        SetPWM(i, -255.0f);  // active close
        break;
    default:
        SetPWM(i, 0.0f);     // stop output
        break;
    }
    continue;  // skip normal PWM write for this sensor
}
// ... normal AdjustFlow logic follows
```

---

## Notes

- The gate applies to **all sensors on the module** (shared pressure sensor per module)
- Consider whether gate should apply during manual mode as well as auto — likely yes for safety
- If pressure sensor is disconnected (`PressurePin == NC`), `PressureReading` stays 0, so the gate will never trigger — safe default behavior
- ADS1115 conversion is non-blocking (alternates request/read each loop), so `PressureReading` may be up to ~2ms stale — negligible for safety purposes at 50ms loop rate
