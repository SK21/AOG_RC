# Contributing Guidelines

## Overview
Thank you for contributing. This document defines the mandatory standards for this project.

## Coding Style
- Language: C++14 (Arduino / embedded context).
- Indentation: Match existing tabs in `.ino` files.
- Braces: K&R style (opening brace on same line).
- Naming:
  - Constants: `CamelCase` or `UPPER_SNAKE` when global and immutable.
  - Arrays/state per sensor: descriptive `CamelCase`.
  - Booleans: affirmative form (`FlowWasEnabled`).
- Avoid magic numbers; introduce `const` globals when reused.
- Use `fabsf` for float absolute values.
- Enforce bounds with `constrain()` where hardware limits apply.
- Single exit rule: Do NOT use early returns inside functions. Each function must have exactly one `return` statement at the end. Nest conditionals or use local variables to accumulate results instead of returning early. (Preference: "refrain from using early exists").
- Prefer explicit `if` blocks over ternaries when logic clarity is improved.

## PID Controller Practices
- Keep proportional, integral, and brake/slow adjust logic readable.
- Document units: `TargetUPM`, `UPM` are in units-per-minute; gains (`Kp`, `Ki`) must reflect sampling interval assumptions.
- Integral anti-windup: clamp using `MaxIntegral`.
- Deadband: express as fraction of target; zero or hold output when inside deadband depending on actuator type.

## Performance & Safety
- Never block with long `delay()` inside control code; rely on `millis()` scheduling.
- Guard array access with sensor count (`MDL.SensorCount`).
- Initialize all per-sensor state arrays before use.

## File Organization
- Keep related control logic (`DoPID`, timed combo) in `PID.ino`.
- Place communication in `Send.ino` / `Receive.ino`.

## Contributions
1. Open an issue describing change before large refactors.
2. Provide test scenario or calibration notes for PID changes.
3. Run build and ensure no warnings are introduced (-Wall recommended locally).
4. Adhere strictly to style (especially single return rule).

## Commit Messages
- Use imperative mood: "Add valve slew limiting".
- Reference issue numbers where applicable.

## Testing
- Bench test new PID tuning with simulated sensor inputs before field deployment.

## Licensing & Attribution
- Ensure all added code is original or properly licensed for inclusion.

## Pull Request Checklist
- [ ] Follows single return rule.
- [ ] No uncontrolled pointer arithmetic.
- [ ] PID changes documented.
- [ ] Constants justified.
- [ ] Builds without warnings.

---
By contributing you agree to follow these guidelines.