# Analyzing PID Log Data in Excel

The RateController can record per-loop PID diagnostics from a Teensy rate module and
save them as a CSV on the tablet. This document explains how to read that data in Excel,
with the goal of diagnosing why an applied rate is sometimes adjusted *down* while it is
still below target.

## Background

- The display refreshes every **1000 ms** and the normal telemetry (PGN 32400) is sent
  every **200 ms**, but the PID loop itself runs every `PIDtime` (≈100 ms by default,
  settable lower). Both the screen and the 200 ms telemetry therefore *alias* the real
  control loop — you cannot see the full picture at 1000 ms.
- When **Record PID log** is enabled (Help screen checkbox), the module streams **PGN 32402**
  over Ethernet at the true PID cadence. The app writes one row per sample to:

  ```
  <DataFolder>\PIDLogs\PIDlog_<yyyyMMdd_HHmmss>.csv
  ```

## CSV columns

| Column        | Meaning |
|---------------|---------|
| `PCTime`      | Tablet clock when the sample was received (`HH:mm:ss.fff`) |
| `ModuleMillis`| Teensy `millis()` at the PID loop — use this for the true time axis |
| `ModuleID`    | Module ID (0–15) |
| `SensorID`    | Sensor ID (0–15) |
| `Target`      | Target rate (UPM) |
| `Applied`     | Measured rate (UPM) |
| `Error`       | `Target - Applied`, raw before deadband/constrain |
| `Integral`    | Accumulated integral term (cleared on error sign-flip, by design) |
| `Change`      | PID change amount this loop |
| `PWM`         | Resulting valve PWM (signed) |

## 1. Open and filter

- Double-click the CSV, or use **Data → From Text/CSV** for more control.
- If the run used **two sensors**, filter to one at a time (**Data → Filter**, set `SensorID` = 0,
  then 1). Charting both sensors together looks like noise.

## 2. Build a real time axis

`ModuleMillis` is the Teensy clock at each PID loop — the column that exposes the sub-200 ms
behavior the display hides. Add a seconds-from-start column (assuming `ModuleMillis` is column B):

```
T_sec = (B2 - $B$2) / 1000
```

Two sanity checks:

- **Cadence:** `= B3 - B2` should sit near `PIDtime` (e.g. ~100 ms). Larger gaps are dropped UDP
  packets — note them; they are not control behavior.
- **Aliasing proof:** count rows per second. You'll see ~10–20 samples between each 1-second display
  tick — exactly the detail the screen could not show.

## 3. Main chart

Select `T_sec`, `Target`, `Applied`, `PWM` → **Insert → Scatter with Straight Lines**.

- Use **scatter**, not a plain line chart: scatter respects the real (uneven) time spacing and
  dropped-packet gaps; a line chart assumes equal spacing and distorts them.
- Put `PWM` on a **secondary axis** (right-click the series → Format Data Series → Secondary Axis),
  since its scale differs from rate.

This shows Target vs. Applied and whether PWM moves the correct direction when they diverge.

## 4. Diagnostic chart — the key step

Make a second scatter chart of `T_sec` vs `Error`, `Integral`, `Change`. Look for these signatures:

- **Flow-noise false reversals (most likely cause):** `Applied` jitters rapidly across `Target`, so
  `Error` flips sign every few samples. The firmware zeroes `Integral` on every sign-flip (to prevent
  overshoot), so you will see `Integral` repeatedly snap to 0 — accumulated correction is thrown away.
  This looks exactly like "it was low, then drove down."
  **Fix is upstream:** more flow filtering (`PulseSampleSize`) or a wider `Deadband` — *not* the gains.
  **Detect it:** add a helper column on `Error` (column G): `=IF(SIGN(G2)<>SIGN(G1),1,0)` and sum it.
  A high flip count over a short window confirms noise-driven integral resets.
- **Measurement lag:** `Applied` trends the right way but arrives late (flow median/exponential filter),
  so `Change`/`PWM` are still reacting to an error that is already gone — classic overshoot-then-correct.
- **Genuine over-correction:** modest `Error` but large `Change` → `Kp` (or the fast-adjust brake region)
  is too aggressive.
- **Deadband holds:** rows where `Change` = 0 while `Error` is small — the deadband working as intended,
  not a problem.

## 5. Tips

- **Zoom in.** Pick the 5–10 second window where the bad behavior occurred (use `PCTime` to line it up
  with what you saw in the field) and chart only those rows. Whole-file charts hide the mechanism.
- **Correlate to PWM response:** if `Change` is positive (commanding more) but `Applied` does not rise,
  that points at hardware/flow rather than the algorithm.
- **PivotTable** for quick stats: average and standard deviation of `Error`, and sign-flip count per
  10-second bucket, give an objective before/after comparison when tuning.

## Notes

- Logging is gated behind command **bit 5 of PGN 32500** and is off by default, so it adds no load in
  normal use. When on it is well under 1% of Teensy loop time and does not affect PID/flow timing.
- Only **valve** (and timed-combo) sensors emit log samples; the motor/fan PID path is not instrumented.
