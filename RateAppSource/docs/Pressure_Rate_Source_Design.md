# Pressure as a Rate Source — Design Notes

**Date:** 2026-10-28
**Status:** Design (not yet implemented)
**Origin:** Ported from the Clayton sprayer build (ClaytonSprayer firmware + ClaytonApp), where it has been field tested since 2026-10-06. Field figures quoted below come from there.

---

## Problem

The PID closes the loop on `Sensor[i].UPM`, which today can only come from the flow meter. A pulse meter has two weaknesses on a sprayer:

- **Lag at low flow.** The median needs several pulse periods. One 2 m section at a crawl is 2-4 Hz, so the reading is seconds behind the valve (Clayton: 3-6 s at 12 samples). The time-windowed median in `GetUPM()` bounds this, at the cost of noise.
- **Transients.** On a section change or a speed change, flow moves at once but the meter only follows after its window refills, so the loop overshoots.

Nozzle flow follows pressure almost instantly (tens to a few hundred ms), and RC already reads a pressure sensor on every module (`PressureReading`, PGN 32401). Using it as feedback gives a fast, smooth signal to control on.

---

## Rate Source Options (per sensor)

| Value | Name | Feedback the PID uses | Meter needed |
|---|---|---|---|
| 0 | Flow meter | meter UPM (today's behaviour, the default) | yes |
| 1 | Pressure, meter trimmed | pressure estimate × learned trim | yes |
| 2 | Pressure only | pressure estimate | no |

**Mode 1 is the one to recommend.** Pressure drives the fast loop and the meter slowly corrects the estimate, so it converges on meter accuracy and absorbs nozzle wear and product density by itself. Mode 2 is for machines with no working meter.

---

## Expected Accuracy

Rough figures from nozzle and sensor behaviour, not measured on RC hardware:

| Setup | Steady-state error | Transient behaviour |
|---|---|---|
| Meter (mode 0) | ±1-3 % | slow; noisy at low flow |
| Pressure only, K from nozzle charts | ±5-10 %, more with wear | fast |
| Pressure only, K calibrated in the field | ±3-5 % near the reference pressure | fast |
| Pressure + meter trim (mode 1) | approaches the meter figure | fast |

The pressure sensor itself is rarely the limit. Flow goes with √P, so a pressure error only costs half as much in flow: a ±0.1 bar sensor at 3 bar is about ±1.7 % flow. The real error is in **K**, the boom flow at the reference pressure:

| Source | Typical size | Notes |
|---|---|---|
| Nozzle wear | +5-15 % over life | flow rises at the same pressure, so the machine over-applies without knowing |
| Product density/viscosity | up to ~10 % | nozzle charts assume water; UAN (SG ~1.28) flows about 12 % less |
| Exponent not exactly 0.5 | 2-5 % away from reference | real nozzles 0.45-0.55, air-induction nozzles more |
| Losses between sensor and nozzles | varies with flow | a sensor at the pump sees hose, valve and check-valve losses that change with rate; a sensor on the boom is much better |
| Blocked nozzles | 1/N per nozzle | **invisible to mode 2**: pressure rises, the estimate reads high, the valve closes, and the machine under-applies while showing on rate |

Calibrating K on the real product (see Calibration below) removes the chart and density errors. Wear and blockages return over time, which is why mode 1 is preferred.

In mode 1 the learned trim is itself an accuracy report: a trim of 1.06 means pure pressure control would be 6 % low. Showing it to the operator, and watching it drift over a season, reveals nozzle wear.

---

## The Estimate

```
Q = K × √( (P − P0) / (Pref − P0) )
```

- `P` is the live reading in raw ADC counts. `P0` is the reading at zero pressure. `Pref` is the reference pressure, in counts.
- `K` is the flow of the **open** nozzles at the reference pressure, in UPM: `K = BoomFlowAtRef × OpenFraction`.
- The firmware works in counts throughout, as the max gate does. The app owns all the engineering units and sends counts, using the inverse calibration `PGN32505.RawThreshold()` already does.
- Subtracting `P0` matters. A 0.5-4.5 V sensor reads about a tenth of full scale at zero pressure; leaving that in skews every square root.
- Gauge pressure ≤ 0, `Pref` ≤ `P0`, or `K` ≤ 0 means "no estimate". The firmware then **falls back to the meter**, so a missing setting degrades to today's behaviour instead of to zero rate.

### Open fraction

Sections are machine-wide in RC, so the app already has what it needs: `Core.Sections.WorkingWidth(false) / Core.Sections.TotalWidth(false)`. It is sent live (PGN 32509, below) because it changes on every section switch.

The firmware should **not** work it out from relay state. `AnySectionOn()` is documented as imperfect (Master/Switch/Hyd/Tram/GeoStop relays are not sections), and the open fraction also needs the section widths, which only the app knows.

Products whose plumbing holds total flow constant regardless of sections send an open fraction of 1.0. Each product decides this in the app.

---

## Firmware (`RCteensy`)

### Where it plugs in

A new `ResolveRate()` between `GetUPM()` and `DoPID()` in the 50 ms loop:

```
ReadAnalog();
GetUPM();        // meter -> Sensor[i].UPM, unchanged
ResolveRate();   // new: picks Sensor[i].ControlUPM
DoPID();         // PIDvalve / PIDmotor / TimedCombo read ControlUPM instead of UPM
AdjustFlow();
```

**Keep `Sensor[i].UPM` as the meter reading** and add a separate `ControlUPM` for the PID. PGN 32400 reports `UPM` as the applied rate, and in mode 1 the screen and totals must keep showing what the meter measured, not the estimate. In mode 2 there is no meter, so `UPM` is set from the estimate as well.

`PIDvalve()` and `PIDmotor()` need no other change. The normalised error divides by `RefUPM` (peak target this run), which works the same with either feedback signal. See `PID_Normalized_Control_Rationale.md`.

### One pressure sensor per module

`PressureReading` is module-wide, while a module carries up to `MaxProductCount` sensors. Only one sensor per module can use a pressure rate source: the one on the boom line the pressure sensor sits on. The app enforces this (see App, below). The firmware should also refuse a pressure source on a second sensor and fall back to the meter for it.

`Fan_ct` is excluded: fan speed has nothing to do with nozzle pressure. `Motor_ct` (pump speed) is fine, since Q = K√P holds whatever moves the liquid.

### Trim learning (mode 1)

A first-order lag toward the meter, with time constant `TrimSeconds`:

```
k        = LoopTime / (TrimSeconds × 1000)
observed = MeterUPM / PressureUPM
Trim    += (observed − Trim) × k        clamped 0.5 .. 2.0
ControlUPM = PressureUPM × Trim
```

Learn **only** while all of these hold:

- the PID is enabled (`PIDenabled[i]`);
- the meter is genuinely reading: **a pulse inside the current flow window**, i.e. `MedianCount[i] > 0` *and* the newest sample is fresh. `UPM > 0` is not enough, because `GetUPM()` holds the last value for up to `FlowTimeout` (4 s) after pulses stop. The Clayton firmware learned against that held value: with a 4 s time constant, a stalled meter moved the trim about 63 % of the way to a wrong ratio, and the trim was then saved;
- the open fraction has not changed for `TrimHoldMs` (2 s). The estimate follows a section change at once, the meter only after its window refills. Comparing the two in that gap bends the trim on every section switch.

**Defaults.** `TrimSeconds` = 4. The Clayton's first build used a fixed ~25 s, which was far too slow; the 2026-10-07 field test settled on 3-5 s.

### Trim persistence

Without persistence, every power-up starts at 1.0 and the first minutes of each day spray on the raw estimate while the trim relearns. So the trim is kept in EEPROM, per sensor, together with the **basis** it was learned against (`PressureRefCounts`, `BoomFlowX10`). If either changes, the old trim no longer means anything and it resets to 1.0.

- Its own record, not part of `SensorConfig`. The app owns `SensorConfig`; the module learns the trim.
- Address: past the sensor block (`253 + MaxProductCount × 124` = 997), e.g. 1024 + i × 12. Check this against `FLASH_RESERVE` and the EEPROM emulation size before using it.
- Signature written last, so a write cut by a power loss leaves no valid record.
- Saved only with the master off (a headland), only when the trim has moved by more than 0.002, and at most once a minute.
- Judge the basis only against a config that was really received or stored. A module booting on defaults must not wipe a good trim before the app has sent the real config.

---

## Interaction With the Max Pressure Gate

The Layer-1 gate (`CheckPressure()` / `AdjustFlow()`, see `Pressure_Max_Gate_Design.md`) runs after `DoPID()` and overrides it. Nothing changes there, and **the gate keeps priority** over every feature in this note.

In modes 1 and 2 the loop sees over-pressure as "too much flow" and closes the valve by itself. That is a second line of defence, not a reason to drop the gate.

---

## Phase 3 Features (optional, after the core works)

### Minimum pressure floor

Below a minimum pressure the nozzles stop atomising. The floor raises the rate setpoint to at least the flow the open nozzles pass at the minimum pressure:

```
FloorUPM = K × √( (Pmin − P0) / (Pref − P0) ) × Trim
Target   = max(TargetUPM, FloorUPM)
```

The PID then holds it like any setpoint, and lets go once the real setpoint is above it again. (The Clayton first tried refusing closing commands below the minimum. That never opened a valve that was already sitting under the floor; replaced after the 2026-10-14 field test.)

This is the same hazard as `MinUPM_OverPressure_Risk.md`: a floor sustains demand at low speed. The app must refuse a minimum pressure at or above the max-gate pressure, and the gate still wins if the floor can't be reached.

### Standby pressure

Standby holds the boom primed at a set pressure while the master is on and every section is shut, so the spray pattern is right the moment a section opens. It doesn't fit the current enable logic: `PIDenabled` requires `AnySectionOn()` and `TargetUPM > 0`, and standby has neither. It needs its own PI branch on the pressure error, converted to flow terms so the same Kp/Ki apply:

```
ErrorFraction = 1 − √( (P − P0) / (Pstandby − P0) )
```

(10 % short on pressure is about 5 % on flow, which is the error the rate loop would have seen.) Standard valve and motor only. Combo-close types shut with the sections, so standby means nothing for them.

Lesson from the Clayton: the first standby loop was P-only. With MinPWM below the valve's breakaway point (field: the valve moves at 80, MinPWM was 70), it never reached the standby pressure. It needs the integral.

---

## Protocol

New PGNs, next to the existing 325xx range (32508 and 32509 are unused at the time of writing). CAN frames follow the existing pattern: byte 0 is `ModSenId` (nibble-swapped), then 7 data bytes. Confirm the numbering in `CanFrameTranslator.cs` before assigning it.

### PGN 32508 — Pressure rate config (RC → module, per sensor)

Sent on connect, on change, and with the other per-sensor settings.

```
0     HeaderLo         252
1     HeaderHi         126
2     Mod/Sen ID       0-15/0-15
3     RateSource       0 meter, 1 pressure + trim, 2 pressure only
4     TrimSeconds      1-120
5     RefCounts Lo     reference pressure, raw ADC
6     RefCounts Hi
7     ZeroCounts Lo    reading at zero pressure, raw ADC
8     ZeroCounts Hi
9     BoomFlow Lo      UPM at reference, x10 (trim basis; also K)
10    BoomFlow Hi
11    StandbyCounts Lo 0 = off (phase 3)
12    StandbyCounts Hi
13    MinCounts Lo     0 = off (phase 3)
14    MinCounts Hi
15    CRC
```

CAN: 0xFF18 carries bytes 3-8, and 0xFF19 carries bytes 9-14 (the module applies both on receiving 0xFF19).

### PGN 32509 — Open fraction (RC → module, per sensor, live)

Sent with every PGN 32500.

```
0     HeaderLo         253
1     HeaderHi         126
2     Mod/Sen ID
3     OpenFraction Lo  0-1000 = 0-100.0 %
4     OpenFraction Hi
5     CRC
```

CAN: 0xFF1A.

Before 32509 has been received, or if it stops arriving for longer than the rate-command timeout, the module treats the estimate as unavailable and falls back to the meter.

### PGN 32404 — Pressure rate report (module → RC, per sensor)

Sent at the PGN 32400 cadence, only for sensors whose RateSource ≠ 0.

```
0     HeaderLo         148
1     HeaderHi         126
2     Mod/Sen ID
3     Trim Lo          x1000, 1000 = no correction
4     Trim Hi
5     Status           bit 0 estimate in use (no fallback), bit 1 learning now,
                       bit 2 floor active, bit 3 standby active
6     PressureUPM Lo   the raw estimate, x100
7     PressureUPM Hi
8     CRC
```

CAN: 0xFF1B → PGN 32404.

---

## Windows App (`RateController`)

- **Per product:** RateSource, BoomFlowAtRef (UPM), ReferencePressure (engineering units), TrimSeconds; later StandbyPressure and MinPressure. These belong with the product's sensor settings (`clsSensorSettings`) and are saved with the profile.
- **Validation:**
  - at most one product per module with RateSource ≠ 0;
  - the module must have a calibrated pressure sensor (the 2-point calibration in `frmMenuPressure`);
  - not allowed for `Fan`;
  - minimum pressure must be below the max-gate pressure.
- **Counts conversion:** reuse the inverse calibration in `PGN32505.RawThreshold()` and factor it into a shared helper that turns engineering units into counts, so the gate and this feature can never disagree. Zero counts are the reading at 0 pressure from the same line.
- **Display:** the applied rate stays the meter (PGN 32400). Show the trim from PGN 32404 next to the rate graph, and mark "pressure estimate in use / fallback to meter".
- **Help text:** say plainly that mode 2 cannot see blocked nozzles or nozzle wear, and recommend mode 1.

### Calibration — measuring boom flow at the reference pressure

The ClaytonApp sweep, ported:

1. With the machine parked and the operator's confirmation (it sprays for about 30 s), take manual control, master on, all sections on.
2. Open the valve in 5 short steps (0.75 s of drive each, then hold). Let each step settle for 3 s, then sample for 2 s.
3. Each sample: `K = MeterUPM / √( (P − P0) / (Pref − P0) )`. Skip samples with no flow or no gauge pressure.
4. Take the median of all samples (≥ 15 needed). The 10-90 % spread is the quality signal: over 20 % means the square law fits poorly, so treat the result as approximate.
5. Apply on the operator's confirmation. A new figure resets the trim on the module (basis change).

Improvement over the Clayton version: drive the valve **closed first**, then sweep open. The Clayton starts from wherever the valve is, and a valve already near fully open gives almost no pressure range to fit.

Any way out of the calibration form must end the override and close everything, as the Clayton's `StopCalibration()` does.

---

## Implementation Order

1. Firmware modes 2 and 1 on RCteensy (`ResolveRate`, `ControlUPM`, trim learning with the freshness guard, trim persistence). App settings, PGN 32508/32509/32404, validation.
2. The calibration sweep.
3. Minimum pressure floor and standby pressure.
4. CAN mapping (0xFF18-0xFF1B), RC_ESP32, Nano. Or hide the option for module types that don't support it: the module should report support, e.g. by InoID/version, and the app should grey the setting out otherwise.

---

## Notes

- **Fallback is the safety property.** Whenever the estimate can't be made (no sensor, missing config, stale 32509, gauge ≤ 0), `ControlUPM` = meter UPM. In mode 2, with no meter either, the PID sees 0 and behaves as it does today with a dead meter.
- **ADS1115 staleness.** `ADSFresh()` already zeroes a frozen reading. Zero gauge pressure means "no estimate", so a wedged ADC falls back to the meter instead of driving the valve wide open on a reading of zero pressure.
- **Sensor placement.** Recommend the sensor on the boom side of the section valves, as close to the nozzles as practical. A sensor at the pump measures line losses that change with flow, and the trim can only correct a fixed error, not one that varies with rate.
- **Filtering.** RCteensy reads `PressureReading` unfiltered. The Clayton uses a light exponential filter (`P = (3·P + raw) / 4` per 50 ms loop), because pump ripple moved raw counts by several percent between loops. Filter the copy used for the estimate and leave the gate's input unfiltered, so the gate's response time is unchanged.
- **Mixed auto/manual.** `AutoOn` is per sensor. In manual, `ResolveRate()` still runs, so the displayed estimate is live, but trim learning is off (the PID isn't enabled).

## Cross-references

- `Pressure_Max_Gate_Design.md` — Layer-1 gate, which keeps priority
- `MinUPM_OverPressure_Risk.md` — same hazard as the minimum pressure floor
- `PID_Normalized_Control_Rationale.md` — why `RefUPM` normalisation needs no change
- Clayton reference implementation: `ClaytonSprayer/Flow.ino` (`PressureUPM`, `ResolveRate`), `PID.ino` (`RateTarget`, `StandbyPWM`), `Trim.ino`; `ClaytonApp/Views/SettingsWindow.Calibration.cs` (sweep)
