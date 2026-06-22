# Pressure-Based Output Max Gate — Design Notes

**Date:** 2026-03-20
**Revised:** 2026-06-21 — reworked the gate to drive the actuator in the same direction normal PID control already uses to shed flow. This removes the per-topology "relief direction" blocker that previously stalled the firmware gate (CLAUDE.md, Apr 18).
**Status:** Research / Design (not yet implemented)

---

## Problem

The PID control loop has no visibility into system pressure. If a blockage, frozen filter, or kinked line causes pressure to rise, the PID will keep increasing PWM output trying to reach the target UPM — making the over-pressure condition worse. A pressure-based max gate intercepts this before hardware damage occurs.

---

## How Pressure and Output Are Coupled

- Pressure can rise **without** a corresponding flow increase (blocked nozzle, filter, etc.)
- Flow sensor measures volume (UPM); pressure reveals hydraulic **load**
- The PID has no safety limit on pressure — it will blindly increase output into a dangerous state

### The relieving direction is always "reduce measured flow" (topology-independent)

An earlier version of this design tried to pick a relief action per control type (force PWM = 0, or active close), and stalled on the fact that the *valve direction* that relieves pressure depends on hydraulic topology — closing an inline throttle relieves, but closing a PTO/bypass-regulator valve sends **more** flow to the booms and spikes pressure.

That reasoning was in valve-position terms. In **flow** terms the ambiguity disappears, because the flow sensor and the pressure sensor sit on the **same boom line**. Reducing flow at that point reduces pressure at that point, monotonically, on every topology:

| System | "Reduce flow" action | Effect on boom-line pressure |
|---|---|---|
| Inline throttle valve | close the valve | ↓ |
| PTO / fixed-disp. pump + bypass regulator | open the bypass (dump to tank) | ↓ |
| Pump-speed (Motor/Fan/PWM pump) | slow the pump | ↓ |

The firmware **already knows** which physical PWM direction reduces measured flow: in `PIDvalve()` (`PID.ino`), `RateError = TargetUPM − UPM`, and when the rate is above target the resulting `ChangeAmount` (and therefore `Result`) goes negative, driving the actuator in the flow-reducing direction. The fact that the loop reaches target in the field is proof the sign convention is already correct for that rig. The gate reuses that sign instead of trying to infer topology — so **no per-module relief-direction config is needed.**

---

## Architecture: Two-Layer Gate

### Layer 1 — Firmware Hard Gate (fast, comms-independent)

- Stores a `MaxPressureReading` threshold (raw ADC uint16) in `ModuleConfig`
- Checked inside `AdjustFlow()` every 50ms **before** writing to the PWM pin
- Works even if comms to PC are lost (stored in EEPROM)
- `0xFFFF` sentinel = gate disabled

**Behavior when triggered:**

Drive the actuator in the **same direction normal PID control uses to shed flow** — i.e. the negative-`Result` direction from `PIDvalve()`/`PIDmotor()` — and keep driving until pressure falls back under threshold. No control-type force table and no topology flag: the sign that reduces measured flow is the sign that relieves pressure (see above).

| Control Type | Gate Action |
|---|---|
| `StandardValve_ct` (velocity-form) | Drive the relieving sign each loop (negative PWM); **do not** set PWM = 0 — for a velocity-form valve PWM = 0 *holds* position and relieves nothing |
| `Motor_ct`, `Fan_ct` | Force PWM = 0 (stop the pump/fan — the actuator *is* the flow source, so stopping it always relieves) |
| `ComboClose_ct`, `TimedCombo_ct` | Drive active-close (negative PWM) until relieved |

- Resets `IntegralSum[i]` to prevent integral windup while the gate is active
- Holds the relieving command every loop while over pressure (does not rely on a one-shot write)
- Gate releases when `PressureReading` drops below `MaxPressureReading − hysteresis`
- Hysteresis band (e.g. 5%) prevents flutter on noisy sensors and stops the gate from chattering against the PID

> **Velocity-form caveat (from the Jun 20 Bug 1 finding):** a `StandardValve_ct` PWM of 0 stops the valve motor and *holds* the last position. The relief therefore has to be an active negative command sustained each loop, not a zero. This is the same actuator semantics already documented for `PIDenabled` gating.

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
| `RCteensy.ino` | Add `uint16_t MaxPressureReading = 0xFFFF` to `ModuleConfig` struct; add `bool PressureGateActive[MaxProductCount]` latch state |
| `Motor.ino` — `AdjustFlow()` | Add latched gate check before each `SetPWM()` call; drive the relieving sign if active |
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
// In AdjustFlow(), per-sensor loop.
// Gate is "latched" with hysteresis so it keeps relieving until pressure clearly drops.
if (MDL.MaxPressureReading != 0xFFFF)
{
    if (PressureReading > MDL.MaxPressureReading)
        PressureGateActive[i] = true;
    else if (PressureReading < MDL.MaxPressureReading - PressureHyst)  // e.g. 5% of threshold
        PressureGateActive[i] = false;
}

if (PressureGateActive[i])
{
    // Over-pressure: drive the SAME direction normal control uses to shed flow.
    // The relieving sign is topology-independent because flow and pressure are
    // coupled at the boom-line sensor (see "How Pressure and Output Are Coupled").
    IntegralSum[i] = 0;  // no windup while gating

    switch (Sensor[i].ControlType)
    {
    case Motor_ct:
    case Fan_ct:
        // stop the pump/fan — the actuator is the flow source
        SetPWM(i, 0.0f);
        break;
    default:
        // velocity-form valve / combo: hold an active relieving command every loop.
        // Negative = the proven flow-reducing direction (matches PIDvalve's sign on UPM > Target).
        SetPWM(i, -Sensor[i].MaxPWM);
        break;
    }
    continue;  // skip normal PWM write for this sensor
}
// ... normal AdjustFlow logic follows
```

---

## Example Implementation (firmware, fitting current code)

Most of the plumbing already exists in `RCteensy`:

- `MDL.MaxPressureReading` is in `ModuleConfig` (`RCteensy.ino:71`)
- PGN 32505 already parses and persists it (`Receive.ino:271`, calls `SaveData()`)
- `PressureReading` is updated each loop by `ReadAnalog()`
- `AdjustFlow()` already calls an empty `CheckPressure()` stub (`Motor.ino:4` / `Motor.ino:77`)

So the gate is: (1) one latch global, (2) fill in `CheckPressure()`, (3) an override at the top of the `AdjustFlow()` loop. No new PGN, no EEPROM change.

### 1. Latch global — `RCteensy.ino` (near the PID damper globals, ~line 175)

```cpp
// Pressure max gate — latched per sensor with hysteresis so it keeps relieving
// until pressure clearly drops, rather than chattering against the PID at the threshold.
bool PressureGateActive[MaxProductCount];
```

### 2. Fill in the stub — `Motor.ino` (`CheckPressure()`)

```cpp
void CheckPressure()
{
    // Disabled sentinel: clear the gate and leave normal control alone.
    if (MDL.MaxPressureReading == 0xFFFF)
    {
        for (int i = 0; i < MDL.SensorCount; i++) PressureGateActive[i] = false;
        return;
    }

    // 5% hysteresis band below the threshold (integer math; threshold is raw ADC counts).
    uint16_t releaseLevel = MDL.MaxPressureReading - (MDL.MaxPressureReading / 20);

    // PressureReading is module-wide (one sensor per module), so every sensor latches together.
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        if (PressureReading > MDL.MaxPressureReading)
        {
            PressureGateActive[i] = true;
        }
        else if (PressureReading < releaseLevel)
        {
            PressureGateActive[i] = false;
        }
        // between releaseLevel and threshold: hold previous state (hysteresis)
    }
}
```

### 3. Override in `AdjustFlow()` — `Motor.ino`

```cpp
void AdjustFlow()
{
    CheckPressure();
    for (int i = 0; i < MDL.SensorCount; i++)
    {
        // Pressure max gate (Layer 1). Drives the same direction normal PID control uses
        // to shed flow — negative PWM for a valve, stop for a pump. Topology-independent
        // because the flow sensor and pressure sensor share the boom line.
        if (PressureGateActive[i])
        {
            IntegralSum[i] = 0;  // no windup while gating

            switch (Sensor[i].ControlType)
            {
            case Motor_ct:
            case Fan_ct:
                SetPWM(i, 0.0f);     // stop the pump/fan
                break;
            default:
                // velocity-form valve / combo: hold an active relieving command each loop.
                // SetPWM applies MDL.InvertFlow uniformly, so the same negative sign the PID
                // uses to reduce flow maps to the correct physical direction on this rig.
                SetPWM(i, -255.0f);
                break;
            }
            continue;  // skip normal PWM write for this sensor
        }

        float clamped = constrain(Sensor[i].PWM, -255.0f, 255.0f);

        switch (Sensor[i].ControlType)
        {
        case StandardValve_ct:
            SetPWM(i, SensorConnected[i] ? clamped : 0.0f);
            break;

        case Motor_ct:
        case Fan_ct:
            SetPWM(i, (SensorConnected[i] && Applying[i]) ? clamped : 0.0f);
            break;

        case ComboClose_ct:
        case TimedCombo_ct:
            SetPWM(i, SensorConnected[i] && Applying[i] ? clamped : -255.0f);
            break;

        default:
            break;
        }
    }
}
```

### Optional — report gate state to the PC (`Send.ino`, PGN 32401)

To drive a distinct UI indicator, OR a "pressure gate active" bit into an existing status byte in PGN32401 (e.g. set when `PressureGateActive[0] || PressureGateActive[1]`). The PC already has the over-pressure alarm from the Apr 18 work; this just confirms the firmware acted.

### Notes on this implementation

- **No `PIDenabled` dependency:** the override sits *before* the normal `SetPWM` calls and runs whenever `PressureGateActive[i]` is set, so it acts in auto **and** manual mode, and even if the PID loop is gated off.
- **Velocity-form stall is fine:** once the valve reaches its mechanical relieving stop the motor stalls at `-255`; that's the intended hold. The gate releases when `PressureReading` falls below `releaseLevel`.
- **`-255` vs `-MaxPWM`:** `-255.0f` matches the existing combo-close convention in this function (`Motor.ino:23`); `SetPWM` already scales to the PWM resolution. Use `-Sensor[i].MaxPWM` instead only if you want the relief capped at the configured max.

---

## Notes

- The gate applies to **all sensors on the module** (shared pressure sensor per module)
- Consider whether gate should apply during manual mode as well as auto — likely yes for safety. In manual mode there is no PID sign to borrow, so the gate's own fixed relieving sign (per the table above) is what acts.
- If pressure sensor is disconnected (`PressurePin == NC`), `PressureReading` stays 0, so the gate will never trigger — safe default behavior
- **Validity precondition:** the "reduce flow ⇒ reduce pressure" guarantee holds only when the flow sensor is downstream of the actuator on the pressurized line — the normal RC plumbing. The blocked-nozzle case (pressure up, flow flat) is still covered: backing the actuator off relieves the trapped pressure.
- **Why not just borrow the PID sign at runtime instead of a per-type fixed sign?** When over pressure the measured flow may already be low (blockage) or the loop may be gated off (`PIDenabled` false), so `RateError` isn't a reliable live source. Using a fixed relieving sign per control type — chosen to match the PID's flow-reducing direction — is more robust than reading the instantaneous PID output.
- ADS1115 conversion is non-blocking (alternates request/read each loop), so `PressureReading` may be up to ~2ms stale — negligible for safety purposes at 50ms loop rate
