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
| `Integral`    | Accumulated integral term (reset only on a *genuine* overshoot — error crossing target beyond the deadband; small jitter no longer wipes it) |
| `Change`      | PID change amount this loop |
| `PWM`         | Resulting valve PWM (signed) |
| `Samples`     | Pulse samples used in the median this loop (fixed-time-window filter). Low = low flow / fewer samples in the window; confirms the time window is binding vs. the count cap |
| `InoID`       | Firmware build id that produced the row (e.g. `25066`, encoding build `25/06/2026`). **Identifies the exact firmware** — present from the InoID-change build onward; older logs omit this column |

## 0. Identify the firmware first

Before reading anything into the numbers, confirm **which firmware** produced the log — tuning
signatures only mean something if you know whether the normalized/conditional-integration
controller was actually running.

- **In Excel:** check the `InoID` column. It should be constant for the whole run; it names the
  exact build (`25066` → built `25/06/2026`). Compare it to the InoID of the build you intended
  to flash. If the column is absent, the log predates this feature.
- **Quick automated read:** run the bundled script (Git Bash), which prints the InoID, the
  CRAWL-vs-OVERSHOOT verdict, and the matching tuning lever:

  ```
  bash docs/pid_log_compare.sh "<path to PIDlog_*.csv>" [brakepoint%]
  ```

  On logs without an `InoID` column it falls back to fingerprinting `Change`/`PWM` magnitude
  (old absolute-error firmware pegs `PWM` at 255 and produces three-digit `Change`; the
  normalized build keeps `PWM` ≤ ~100 and `Change` small).

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

- **Flow-noise false reversals:** `Applied` jitters rapidly across `Target`, so `Error` flips sign
  every few samples. Current firmware only resets `Integral` on a *genuine* overshoot (error crossing
  target beyond the deadband), so jitter no longer wipes the integral the way it did in older builds —
  but heavy noise still drives `Change`/`PWM` chatter. If you see `Integral` snapping to 0 on nearly
  every sign-flip, you are looking at an **old build** (confirm with `InoID` / §0).
  **Fix is upstream:** more flow filtering (`PulseSampleSize`/flow window) or a wider `Deadband` — *not* the gains.
  **Detect it:** add a helper column on `Error` (column G): `=IF(SIGN(G2)<>SIGN(G1),1,0)` and sum it.
  A high flip count over a short window confirms noise-driven chatter.
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
- **Valve**, **timed-combo**, and **motor/fan** PID paths all emit log samples. Note the motor path is
  velocity-form (`PWM` accumulates) while the valve path is positional, so the `PWM`/`Change` columns
  read differently between them — interpret each in light of its control type.
