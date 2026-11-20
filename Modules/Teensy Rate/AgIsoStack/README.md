# AgIsoStack library for RCteensy ISOBUS builds

RCteensy builds with ISOBUS (`#define ISOBUS_ENABLED 1` in `RCteensy.ino`) need the AgIsoStack
library. Normal builds (`ISOBUS_ENABLED 0`) don't use it.

The published AgIsoStack-Arduino release is out of date (2024-10) and lacks the current Teensy
CAN driver, so the library is generated from AgIsoStack++ with `make_lib.sh` and installed as
a normal Arduino library. It isn't kept in the sketch folder: the Arduino IDE compiles
everything in a sketch's `src` folder, which would change the normal build.

## Make the library (Git Bash)

```
git clone https://github.com/Open-Agriculture/AgIsoStack-plus-plus.git
git -C AgIsoStack-plus-plus checkout d29e670
"Modules/Teensy Rate/AgIsoStack/make_lib.sh" AgIsoStack-plus-plus
```

The output goes to `D:/Sync/Documents/Arduino/libraries/AgIsoStack` unless a second argument
gives another folder. Use your own sketchbook's `libraries` folder. Restart the Arduino IDE or
Visual Micro afterwards.

## What the script changes from upstream

- Takes the source files listed in upstream `library.json` (Teensy CAN driver only).
- Flattens the folders and removes the `isobus/isobus/` style include prefixes.
- Adds `DISABLE_ISOBUS_DATA_DICTIONARY`: the DDI name table is only used for log text and
  costs 124 KB of RAM1.
- Leaves the VT and TC server headers out of `AgIsoStack.hpp` (not used on a module).

No Teensyduino linker script patch is needed (checked 2026-11-20: with LTO the stock and
patched scripts give the same firmware).

`d29e670` (2026-11-18) is the tested commit. The script warns if the checkout is elsewhere.
