# Prime: "Resume on movement" — Implementation Plan

Status: **planned, not started** (written 2026-11-03). App-only; no firmware change.

Replaces PR #65 ("Allow Application at Standstill", gunicsba, branch
`feat/allow-application-at-standstill`, commits 47a7a95f + 837e472e). That PR is
to be **closed unmerged**. The proposal below was posted on the PR on 2026-10-30.
As of 2026-11-03 nobody has replied.

---

## 1. Background

### What the farmer (Dzseff) asked for (PR #65 comment, 2026-10-30)
- A small fixed flow (Min UPM, e.g. 2.5) while stopped at the start of a run.
- When they drive off, sections come on and automatic control continues.
- No untreated strip at the start, and no long series of button presses.
- Complaints:
  - The Master hold delay can't go below 3 s.
  - Switching from PRM to MST needs too many presses. (Resume after prime already
    handles this. They may not know about it.)

### Why PR #65 is not merged
1. **The valve winds open.** With Auto Section on, `clsRelays.SetRelays` keeps
   the FlowMaster relay off below 0.1 km/h, but the PR still sends a non-zero
   RateSet. With no flow, the valve PID winds open while parked and overshoots
   on pull-away. This overlaps the open flow-path gate issue (Option B).
2. **It applies at every stop.** Once ticked, the floor runs at every stop while
   Master and sections are on, with no timeout. That means a pile from a
   spreader, or NH3 flowing while parked.
3. Minor: strings only in en and hu, an early return, no help page, untested on
   hardware.

### Why today's Prime doesn't fit
- `Props.Speed_KMH` returns `SimSpeed_KMH` while `PrimeOn`, so prime flows the
  **full target rate** at the simulated speed. It doesn't flow a small amount.
- Prime ends on a **timer** (`PrimeTime`), which is wrong either way:
  - **It ends before they move:** speed is 0, `ProductOn` is false, and flow
    stops. It restarts from zero on pull-away, which leaves the untreated strip.
  - **It's still running when they move:** the rate follows the simulated speed,
    not the real speed, until the timer ends.
- Area while priming is already excluded (committed 2026-10-30,
  `clsProduct.cs:1123`). Flow, quantity and hours coverage are unchanged.

---

## 2. Behaviour to build

"After prime" on the Prime settings page (`frmMenuPrimed`) becomes a choice of three:

| Mode | Behaviour |
|------|-----------|
| **Master off** | As today with Resume unticked: the timer ends, Master turns off. |
| **Resume (timed)** | As today with Resume ticked: the timer ends, Master stays on. |
| **Resume on movement** (new) | Described below. |

**Resume on movement:**
- It starts at standstill, like prime does now: the PRM button, or holding Master
  for `PrimeDelay` seconds.
- While stopped it flows only the **Min UPM floor**, scaled to the sections that
  are on.
- The sections come from the switchbox, ignoring AOG, the same as prime does now.
- The FlowMaster relay stays on.
- No area is counted. The amount of product used is counted.
- It **ends on real movement**: real speed of at least about 0.8 km/h, held for
  about 1 s.
  - When it ends, prime turns off and Master stays on. Normal or auto section
    control takes over with no extra presses.
  - The floor keeps flowing during that 1 s while moving, so there is no gap.
- **`PrimeTime` is a safety limit** while stopped. If they never move off, prime
  ends and Master turns off, so it can't pile product or leak.
- Nothing is saved apart from the mode setting itself. Each stop needs a new
  prime, so it does **not** run at every stop.

---

## 3. Edits

### 3.1 `Classes/Props.cs`
- Replace `ResumeAfterPrime` (bool, lines ~104, ~372–380, load at ~1316) with an
  enum setting:
  ```csharp
  public enum AfterPrimeMode { MasterOff, Resume, ResumeOnMovement }
  ```
  - Save it under a new key `AfterPrime`.
  - **Migration:** if `AfterPrime` is missing, read the old `ResumeAfterPrime`
    (true → `Resume`, otherwise `MasterOff`).
- `Speed_KMH` (~line 485): return `SimSpeed_KMH` only when
  `Core.SectionControl.PrimeOn && AfterPrime != ResumeOnMovement`. In the new
  mode the speed must stay real, so movement can be detected and the rate never
  follows the simulated speed.
- Optional, see 3.7: `PrimeDelay` setter (~line 326).

### 3.2 `Classes/clsSectionControl.cs`
- Add a `public bool PrimeHold => cPrimeOn && Props.AfterPrime == AfterPrimeMode.ResumeOnMovement;`
- **Movement detection:** `PrimeTimer` ticks every 1000 ms, which is too coarse.
  Use a faster check, either a second 200 ms timer or a shorter interval for the
  timer with a separate seconds count:
  - Count consecutive checks with `Props.Speed_KMH >= ~0.8`, and reset the count
    on any check below.
  - At about 1 s of movement: `cPrimeOn = false`, stop the timer,
    `PrimeInitialized = false`, `ForceOff = false` (Master stays on).
- **Timeout:** when `TimerCount > Props.PrimeTime` (`PrimingTimerTick`, ~line 204):
  - `ForceOff = (AfterPrime != Resume)`. The new mode times out to Master off.
  - Keep the existing `RemotePrimeEnded()` / `PressSwitch(SwIDs.MasterOff)`
    branch for the Master off cases.
- `StartPrime()` and `SetPriming()` check `Props.Speed_KMH < 0.1`. That still
  works, since speed is real in the new mode and only timed prime fakes it.
- `ApplySectionsNoZones` / `ApplySectionsWithZones`: no change. The `cPrimeOn`
  branch already uses the switchbox sections and ignores AOG.

### 3.3 `Classes/clsRelays.cs` (~line 165)
```csharp
bool FlowEnabled = Props.Speed_KMH > 0.1 || Core.SectionControl.PrimeOn;
```
In the new mode the speed is a real 0. Without this the FlowMaster relay would
be off, the same windup as PR #65. It does no harm to timed prime, which already
fakes the speed.

### 3.4 `PGNs/PGN32500.cs` (~line 160): the Min-UPM floor gate
Currently:
```csharp
if (Prod.ProductOn(false)) { double MinUPM = Prod.MinUPMinUse() * 1000.0; if (RateSet < MinUPM) RateSet = MinUPM; }
```
At 0 speed the target is 0 and `ProductOn(false)` is false
(`cHectaresPerMinute == 0`). Change the gate to:
```csharp
if (Prod.ProductOn(false) || (Core.SectionControl.PrimeHold && Prod.RateSensorData.Connected()))
```
- `MinUPMinUse()` already scales by `WorkingWidth / TotalWidth`, so only the
  share for the sections that are on flows.
- If Min UPM is 0, nothing flows in this mode (see 3.5).
- Check the other `ProductOn` callers, such as the display and alarms. The
  off-rate alarm during the hold should probably stay quiet.

### 3.5 `Menu/frmMenuPrimed.cs` + `.Designer.cs`
- Replace `ckResume` with three radio buttons inside a group box ("After prime").
  Put them in Designer.cs as real controls, never created at runtime.
  - Load from `Props.AfterPrime` in `UpdateForm()`.
  - Save in `btnOK_Click`.
  - Each radio's `CheckedChanged` calls `SetButtons(true)`.
- The On Time label or hint: in the new mode it is the **maximum time while
  stopped**.
- Warn, with a hint line or a message on Save, when the new mode is chosen and a
  product has Min UPM 0. Nothing would flow.
- Lang keys in all 8 languages, UTF-8 without BOM, using ReadAllText /
  WriteAllText; add the properties to Lang.Designer.cs by hand. Keys needed:
  - `lgAfterPrime`
  - `lgResumeOnMovement`
  - a key for "Master off"
  - the max-time hint
  - `lgResume` already exists.

### 3.6 Documentation
- Help page `D:\Sync\RATE CONTROL\User Manual\Help pages\frmMenuPrimed.html`,
  plus `frmMenuSwitches.html` where it mentions PRM.
- `UserManual.html` §16 Machine – Primed Start.
- Regenerate the PDFs with Edge headless and copy them to both app destinations
  (see the memory note reference_user_manual).
- Changes.txt:
  - The new mode.
  - Prime no longer counts area. This is already committed and is a change in
    behaviour.

### 3.7 Optional: the Master hold delay
- `frmMenuPrimed` accepts 0–8 (`tbDelay_Enter` / `_Validating`), but the
  `Props.PrimeDelay` setter ignores anything outside 3–8. Typing 1 or 2 does
  nothing, with no error. That is an existing bug.
- Decide which fix to make:
  - Allow 1–8, keeping at least 1 so a quick Master press doesn't start a
    prime. This also addresses Dzseff's 3 s complaint.
  - Make the form's limits match 3–8.

---

## 4. Test checklist (bench, then machine)
1. **New mode, stopped:**
   - Prime starts from the PRM button and from holding Master.
   - Flow is the Min UPM floor, scaled to the sections that are on.
   - The FlowMaster relay is on.
   - No area is counted, and the quantity goes up.
2. **Drive off:** prime ends after about 1 s at 0.8 km/h or more. Master stays on.
   The rate follows the real speed. No dip in the PID log.
3. **Never move:** after `PrimeTime`, Master turns off. This applies to a phone in
   control too (the `RemotePrimeEnded` path).
4. **GPS jitter while parked:** prime does **not** end.
5. **AOG auto sections at pull-away:** if AOG keeps sections off until its own
   minimum speed, there is a short dip after the handover. Test with AOG's
   slow-speed cutoff set.
6. **PRM button (`StartPrime`):** check that `Core.SwitchBox.MasterOn` is true,
   since `clsRelays` reads it for FlowMasterOn.
7. **Timed modes** (Master off / Resume): exactly as before.
8. **Old profile with `ResumeAfterPrime=True`:** it opens as Resume (timed).
9. **Valve product with a FlowMaster relay:** no windup in the PID log while
   parked in the new mode.
10. **Simulated speed mode:** the speed is always above 0, so prime can't start.
    That's acceptable; just confirm nothing breaks.

## 5. PR #65 wrap-up
Once this is built, close PR #65 unmerged with a short comment:
- The new Prime option covers the request.
- Thank gunicsba and Dzseff.
- Credit the report.

Optionally ask Dzseff to test it before release.
