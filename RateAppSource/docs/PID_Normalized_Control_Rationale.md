# PID Control: Why It Was Changed From Absolute Error to a Ratio

**Module:** Teensy Rate firmware — `AOG_RC/Modules/Teensy Rate/RCteensy/PID.ino` (`PIDvalve`, `PIDmotor`)
**Date:** 2026-06-25
**Status:** Bench- and field-validated on the test branch.

---

## TL;DR

The flow PID was converted from an **absolute** formulation (error in units/min, output in raw PWM) to a
**normalized / ratio** formulation:

- **Error** is now a fraction of a reference flow: `FracError = RateError / RefUPM`
- **Output** is now a fraction of the valve's PWM authority: `... * (MaxPWM - MinPWM)`

This makes the proportional and integral gains (`Kp`, `Ki`) **dimensionless**, so a given gain setting
behaves consistently across very different hardware and across rate/section changes on one machine,
instead of needing to be re-derived for every valve and operating point.

---

## 1. What the old absolute code did, and why it was a problem

The original loop computed the correction directly in physical units:

```
RateError  = TargetUPM - MeasuredUPM        // units/min (UPM)
ChangeAmount = RateError * Kp * 100 * BrakeFactor + Integral   // raw PWM counts
```

`RateError` is an absolute flow error in UPM, and the output is a raw PWM count. The problem is that the
**right value of `Kp` depends entirely on the hardware**, because it has to bridge two unrelated scales:

- the flow scale (a small implement might run **5 UPM**, a large one **300 UPM**), and
- the PWM scale (0–255 counts driving the valve).

A valve that needs a 5-UPM error to produce a meaningful PWM move needs a `Kp` roughly **60× larger** than
one operating at 300 UPM. So every machine — and even the same machine at a different application rate —
needed a different `Kp`/`Ki`.

To paper over that range, the firmware used an **exponential UI decode**:

```
Kp = 1.1 ^ (slider - 120)
Ki = 1.1 ^ (slider - 108)
```

This had two practical failures:

1. **Dead slider.** The bottom half of the 0–100 slider produced gains so tiny they did nothing; the entire
   usable band was crammed into the top, so operators ended up running 80–85 just to get response.
2. **Inconsistent meaning.** "Gain 80" on one machine was nothing like "gain 80" on another — the number
   carried no portable meaning.

## 2. The core idea: make both sides of the equation dimensionless

If we express the error as a **fraction** and the output as a **fraction**, the gain that connects them is a
pure number with no hidden units:

```
FracError = RateError / RefUPM            // dimensionless, ~[-1, 1]
Authority = MaxPWM - MinPWM               // the PWM range the valve actually uses
ChangeAmount = FracError * Kp * BrakeScale * Authority + Integral
```

- `FracError` answers "how far off are we, as a fraction of where we should be?" — the same question
  whether the machine runs at 5 or 300 UPM.
- `Authority` answers "how much PWM travel does this valve have?" — so the output automatically scales to the
  hardware. `Kp = 1.0` means "full error commands roughly full travel" on **any** valve.

The old `KpMultiplier = 100` and `FastAdjustValve = 40` constants — which only existed to bridge the
absolute UPM→PWM scale mismatch — were **removed**. The `Authority` factor replaces them; it does not stack
on top of them (keeping all three would explode the output by ~1e6).

`BrakeScale` is now a clean 0–1 multiplier: `1.0` far from target (full response), `PIDslowAdjust/100`
inside the brakepoint band (gentle near the setpoint).

## 3. Why the reference is *peak target*, not the *live* target

The first attempt divided by the live `TargetUPM`. That broke section control. Because gain then scaled as
`1/target`, **closing sections lowered the target and multiplied the loop gain**:

| Sections off (of 4) | Target | Effective gain vs. full |
|--------------------:|-------:|------------------------:|
| 0                   |  ~99   | 1.0× (stable)           |
| 1                   |  ~74   | 1.3×                    |
| 2                   |  ~49   | 2.0×                    |
| 3                   |  ~25   | ~4.0×                   |

A gain tuned to be stable with all sections on became wildly over-gained — and oscillated — with sections
off. No single setting could be both stable at 4× and responsive at 1×.

The fix: normalize by a **section-independent reference**, `RefUPM`, which the firmware latches to the
**peak target seen during the run** (`if (TargetUPM > RefUPM) RefUPM = TargetUPM;`, reset when the loop is
disabled). At full sections `RefUPM == TargetUPM`, so behavior is identical to a straightforward
normalization; as sections close, `RefUPM` holds, so `FracError` gets *smaller* — the loop becomes
**gentler, not hotter**. The gain stays constant regardless of how many sections are on.

(Edge case, benign and self-healing: if a run *starts* with sections off and full width is never reached,
`RefUPM` latches the reduced target and the loop is slightly hot — but only until all sections come on once,
after which the peak-latch corrects permanently. The realistic field sequence, start full and peel sections
off, is handled exactly.)

## 4. Companion change: conditional integration (anti-windup)

Normalizing the gains exposed a second issue. The integral term, now scaled by `Authority`, would **wind up
during the approach** to a new setpoint — pinning PWM at its limit and causing overshoot on capture, and an
exaggerated "drop" when several sections closed at once.

So the integral now accumulates **only near target** (inside the brakepoint band, `nearTarget`). Far from
target the proportional term alone drives the approach; the integral engages only to trim the final offset.
This removes the wind-up overshoot and makes `Ki` behave consistently regardless of the path taken to the
setpoint. A practical consequence: since P alone carries the approach, the proportional **gain must be set
higher** than it was when the integral helped during the approach.

## 5. UI decode after normalization

With dimensionless gains, the exponential decode was replaced with a simple linear scale that spreads the
usable range across the slider:

```
Kp = slider / 1000          // e.g. slider 35 -> Kp 0.035
Ki = slider / 10000          // finer; the integral only trims near target so it wants a small value
```

The CAN (`CANBus.ino`) and UDP (`Receive.ino`) decode paths are kept identical to each other.

## 6. Net benefits

- **Portable gains** — a setting means the same thing across different valves/implements; "high is high."
- **Stable across rate and section changes** — loop gain no longer drifts with the operating point.
- **Usable slider resolution** — no dead bottom half; comfortable operating point near mid-dial.
- **Well-behaved transients** — conditional integration removes capture overshoot and the multi-section
  "excessive drop."
- **Robust at the authority extreme** — validated stable even with `MaxPWM = 100%` (largest gain term).

## 7. What deliberately did *not* change

- **No derivative (KD) term** — it was dropped in an earlier rework because it amplified flow-pulse noise; it
  is intentionally not reintroduced.
- **Deadband, brakepoint, and the `±target` error clamp** were already expressed as percentages of target —
  this change finishes a conversion that was already half done.
- **Scope is the Teensy module only.** The ESP32 and Nano flow modules still use the older absolute
  formulation and are deferred.

## 8. Reading the PID logs after this change

The `Integral` and `Change` columns in `PIDlog_*.csv` are still in PWM units, but their magnitudes differ
from the old scheme (they now derive from `FracError * ... * Authority`). Diagnostic thresholds based on the
old absolute magnitudes need reinterpreting — see `PID_Log_Excel_Analysis.md`.

---

*Related design notes: `PID_Log_Excel_Analysis.md`. Firmware: `PID.ino`, `Receive.ino`, `CANBus.ino`.*
