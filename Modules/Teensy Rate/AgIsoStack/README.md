# AgIsoStack library for RCteensy ISOBUS builds

RCteensy builds with ISOBUS (`#define ISOBUS_ENABLED 1` in `RCteensy.ino`) need the AgIsoStack
library. Normal builds (`ISOBUS_ENABLED 0`) don't use it.

The published AgIsoStack-Arduino release is out of date (2024-10) and lacks the current Teensy
CAN driver, so the library is generated from AgIsoStack++ with `make_lib.sh` and installed as
a normal Arduino library. It isn't kept in the sketch folder: the Arduino IDE compiles
everything in a sketch's `src` folder, which would change the normal build.

Both batch files need Git for Windows (Git Bash) and the Arduino IDE with Teensyduino. Each
window stays open at the end so its result can be read.

## Install the library: `MakeLib.bat` (this folder)

Double-click it, once per PC (again only when the tested commit changes). It gets AgIsoStack++
at the tested commit into `D:\Temp\AgIsoStack-plus-plus` (`%TEMP%` when there is no `D:\Temp`)
and installs the library into the Arduino sketchbook's `libraries\AgIsoStack`, the sketchbook
the Arduino IDE uses. Restart the Arduino IDE or Visual Micro afterwards.

By hand in Git Bash: `make_lib.sh <AgIsoStack++ checkout> [library folder]`; without the
second argument the library goes to `D:/Sync/Documents/Arduino/libraries/AgIsoStack`.

## Make a release: `BuildRelease.bat` (Teensy Rate folder)

Double-click it (about 8 minutes), or drag a folder onto it to choose where the files go.
It builds, from the sketch as committed:

- `RCteensy.ino.hex`: the normal build
- `RCteensy_ISOBUS.hex`: the ISOBUS build, from a temporary copy with `ISOBUS_ENABLED 1`

into `D:\Temp\RCteensy_release` (`%TEMP%\RCteensy_release` when there is no `D:\Temp`), with
`normal.log` and `isobus.log`. It shows the sizes and SHA256 of both files, checks each one's
build flag, says whether the normal hex matches the one committed in the repo, and opens the
folder. Only its own four files in the folder are replaced. The working copy isn't changed;
`RCteensy.ino` must have `#define ISOBUS_ENABLED 0`.

It runs `build_release.sh [output folder]` in this folder, which can also be run from Git Bash.

## What the script changes from upstream

- Takes the source files listed in upstream `library.json` (Teensy CAN driver only).
- Flattens the folders and removes the `isobus/isobus/` style include prefixes.
- Adds `DISABLE_ISOBUS_DATA_DICTIONARY`: the DDI name table is only used for log text and
  costs 124 KB of RAM1.
- Leaves the VT and TC server headers out of `AgIsoStack.hpp` (not used on a module).

No Teensyduino linker script patch is needed (checked 2026-11-20: with LTO the stock and
patched scripts give the same firmware).

`d29e670` (2026-11-18) is the tested commit. The script warns if the checkout is elsewhere.
