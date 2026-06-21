# Minimum-UPM Floor → Over-Pressure Risk — Design Notes

**Date:** 2026-06-21
**Status:** Identified / **Deferred** (no code change made)

---

## Concern

A minimum-UPM floor commands a **fixed flow** that the PID will chase by opening the
valve. If that flow cannot be reached, the valve saturates wide open, which on some
plumbing drives system pressure past safe limits.

This was raised while reviewing the Min-UPM feature; the floor specifically widens the
risk window because it is a background setting that is active at **low ground speed**,
exactly when the operator expects little or no flow — so a wide-open valve is unexpected.

## Mechanism

The floor becomes unreachable (→ valve saturates open) in two cases:

1. **By-speed floor set high, ground speed low.** The floor = flow-at-min-speed, demanded
   *regardless* of how slowly the machine actually creeps. Forcing that flow through fixed
   nozzles needs higher pressure; the valve opens to push it and pressure climbs toward /
   over the system limit.
2. **A fault at low speed** (partial plug, filter, viscous product). The floor sustains a
   nonzero target while crawling, so the integral winds the valve fully open trying to hit
   a flow the system cannot pass.

Note this is not unique to the floor — any unachievable target saturates the actuator —
but the floor makes it insidious by sustaining a target at low speed when the operator
isn't expecting demand.

## Current protection status (as of 2026-06-21)

| Layer | State | Stops the valve? |
|---|---|---|
| Firmware anti-windup (Jun-20 `MaxIntegral` rework) | Implemented | **No** — total integral is clamped to ±(MaxPWM−MinPWM), so the valve can still reach full open on a *sustained* unreachable setpoint |
| Firmware pressure hard-gate (Layer 1, see `Pressure_Max_Gate_Design.md`) | **Designed, not built** (PGN 32505 reserved; 32503 is taken) | Would — but deferred |
| App pressure alarm (Layer 2, `clsAlarm` / `frmMain` / `frmMenuPressure`) | Implemented | **No** — alarm only; operator must react |

The rate command (`PGN32500`) has **no pressure awareness** — the floor is sent whenever
`Prod.ProductOn(false)` is true, independent of measured pressure.

## Options considered

1. **App-side, near-term (recommended first when picked up):** when the product's
   over-pressure alarm is active, force `RateSet = 0` in `PGN32500`. This is the
   "optionally send TargetUPM = 0" Layer-2 action from `Pressure_Max_Gate_Design.md`.
   Reuses existing alarm state; no firmware change. Downsides: ~200 ms comms latency and
   it is reactive (fires after pressure is already exceeded).
2. **Real fix — firmware hard gate (Layer 1).** Fast, comms-independent. Deferred because
   the gate's *action direction* is system-dependent: closing a valve on a PTO /
   independent-pump system raises pressure rather than lowering it, so a single
   "stop/close on over-pressure" rule is wrong for some plumbing. Needs a per-system gate
   action — the unresolved point that stalled `Pressure_Max_Gate_Design.md`.
3. **UI guard (complement).** Use the Min-UPM hint to warn when the floor UPM is unusually
   high relative to the nominal target (e.g., floor > target UPM at rated speed), so the
   operator sees a too-aggressive floor before it bites.

## Decision

**Deferred for now — no code change.** Tracked here so it can be picked up with the
pressure-gate work. When resumed, start with Option 1 (small, reuses existing alarm
state), and treat Option 2 as the proper follow-up once the system-type gate direction is
settled.

## Cross-references

- `Pressure_Max_Gate_Design.md` — the two-layer pressure gate this would plug into.
- PGN **32505** is reserved for the future firmware pressure gate (32503 is taken — subnet change).
- Min-UPM application site: `PGNs/PGN32500.cs` (floor applied under `Prod.ProductOn(false)`).
- Floor math: `Classes/clsProduct.cs` — `MinUPMinUse()` / `FloorUPMfromSpeed()`.
