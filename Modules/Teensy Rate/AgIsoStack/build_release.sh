#!/usr/bin/env bash
# Builds both RCteensy release hex files from the sketch as committed. Run from Git Bash.
#   RCteensy.ino.hex     normal build (ISOBUS_ENABLED 0, as in the repo)
#   RCteensy_ISOBUS.hex  ISOBUS build (a temporary copy of the sketch with ISOBUS_ENABLED 1)
# The working copy is not changed. The ISOBUS build needs the AgIsoStack library (make_lib.sh).
# Usually started from BuildRelease.bat in the Teensy Rate folder.
#
# Usage: build_release.sh [output folder]
#   default: D:\Temp\RCteensy_release, or %TEMP%\RCteensy_release when D:\Temp doesn't exist.
#   Only this script's own files in the folder are replaced; anything else there is left alone.

set -euo pipefail

HERE="$(cd "$(dirname "$0")" && pwd)"
SKETCH="$(cd "$HERE/../RCteensy" && pwd)"
REPO_HEX="$HERE/../RCteensy.ino.hex"
if [ -n "${1:-}" ]; then
	OUT="$(cygpath -u "$1")"
elif [ -d /d/Temp ]; then
	OUT="/d/Temp/RCteensy_release"
else
	OUT="$(cygpath -u "$TEMP")/RCteensy_release"
fi

ARDUINO="/c/Program Files (x86)/Arduino/arduino_debug.exe"
OBJCOPY="/c/Program Files (x86)/Arduino/hardware/tools/arm/bin/arm-none-eabi-objcopy.exe"
BOARD="teensy:avr:teensy41:usb=serial,speed=600,opt=oslto"
PREFS="$LOCALAPPDATA/Arduino15/preferences.txt"

fail() { echo "build_release: $*"; exit 1; }

[ -x "$ARDUINO" ] || fail "Arduino IDE not found: $ARDUINO"
grep -q '^#define ISOBUS_ENABLED 0' "$SKETCH/RCteensy.ino" || fail "RCteensy.ino must have #define ISOBUS_ENABLED 0 (the normal build is made from the sketch as it is)"

SKETCHBOOK=$(grep '^sketchbook.path=' "$PREFS" | cut -d= -f2- | tr -d '\r')
LIB="$(cygpath -u "$SKETCHBOOK")/libraries/AgIsoStack"
[ -f "$LIB/src/AgIsoStack.hpp" ] || fail "AgIsoStack library not installed in $SKETCHBOOK\\libraries. Run AgIsoStack\\MakeLib.bat first."

# the library must be from the tested commit: library.properties "sentence=AgIsoStack++ <commit> ..."
PINNED=$(grep '^PINNED=' "$HERE/make_lib.sh" | cut -d= -f2 | awk '{print $1}')
INSTALLED=$(grep '^sentence=' "$LIB/library.properties" 2>/dev/null | tr -d '\r' | awk '{print $2}')
[ "$INSTALLED" = "$PINNED" ] || fail "the AgIsoStack library is from ${INSTALLED:-an unknown commit}, the tested commit is $PINNED. Run AgIsoStack\\MakeLib.bat first."

# only this script's files: a failed build must not leave an older hex looking current
mkdir -p "$OUT"
WORK="$OUT/work"
rm -f "$OUT/RCteensy.ino.hex" "$OUT/RCteensy_ISOBUS.hex" "$OUT/normal.log" "$OUT/isobus.log"
rm -rf "$WORK"

# build <sketch .ino> <build folder> <log>: true when a hex came out. arduino-builder sometimes
# crashes on a first build (Go runtime exception), so it is retried once.
build() {
	local ino="$1" dir="$2" log="$3" try
	for try in 1 2; do
		rm -rf "$dir"
		"$ARDUINO" --verify --board "$BOARD" --pref build.path="$(cygpath -w "$dir")" "$(cygpath -w "$ino")" > "$log" 2>&1 || true
		[ -f "$dir/RCteensy.ino.hex" ] && return 0
		echo "  build attempt $try failed, see $log"
	done
	return 1
}

# the build flags byte of a hex file's firmware tag ("RCFWID01", type, InoID lo, InoID hi, flags)
tag_flags() {
	local bin="$WORK/tag.bin"
	"$OBJCOPY" -I ihex -O binary "$(cygpath -w "$1")" "$(cygpath -w "$bin")"
	local off
	off=$(LC_ALL=C grep -obUa 'RCFWID01' "$bin" | head -1 | cut -d: -f1)
	[ -n "$off" ] || { echo "none"; return; }
	od -An -tu1 -j $((off + 11)) -N 1 "$bin" | tr -d ' '
}

mkdir -p "$WORK"

echo "Normal build..."
build "$SKETCH/RCteensy.ino" "$WORK/normal" "$OUT/normal.log" || fail "normal build failed"
cp "$WORK/normal/RCteensy.ino.hex" "$OUT/RCteensy.ino.hex"

echo "ISOBUS build..."
COPY="$WORK/isobus/RCteensy"
mkdir -p "$COPY"
cp "$SKETCH"/*.ino "$SKETCH"/*.h "$SKETCH"/*.c "$SKETCH"/*.cpp "$COPY/"
sed -i 's/^#define ISOBUS_ENABLED 0/#define ISOBUS_ENABLED 1/' "$COPY/RCteensy.ino"
grep -q '^#define ISOBUS_ENABLED 1' "$COPY/RCteensy.ino" || fail "could not set ISOBUS_ENABLED 1 in the copy"
build "$COPY/RCteensy.ino" "$WORK/isobus_build" "$OUT/isobus.log" || fail "ISOBUS build failed"
cp "$WORK/isobus_build/RCteensy.ino.hex" "$OUT/RCteensy_ISOBUS.hex"

# each file must carry its own build flag
[ "$(tag_flags "$OUT/RCteensy.ino.hex")" = "0" ] || fail "RCteensy.ino.hex: firmware tag build flags are not 0"
[ "$(tag_flags "$OUT/RCteensy_ISOBUS.hex")" = "1" ] || fail "RCteensy_ISOBUS.hex: firmware tag build flags are not 1"

echo
for f in RCteensy.ino.hex RCteensy_ISOBUS.hex; do
	log="$OUT/normal.log"; [ "$f" = "RCteensy_ISOBUS.hex" ] && log="$OUT/isobus.log"
	echo "$f"
	grep -a -E '^ *(FLASH|RAM1|RAM2):' "$log" | sed 's/^/  /'
	echo "  SHA256 $(sha256sum "$OUT/$f" | cut -d' ' -f1)"
done

echo
if cmp -s "$OUT/RCteensy.ino.hex" "$REPO_HEX"; then
	echo "RCteensy.ino.hex matches the hex committed in the repo."
else
	echo "WARNING: RCteensy.ino.hex differs from the hex committed in the repo ($REPO_HEX)."
fi

rm -rf "$WORK"
echo "Output: $(cygpath -w "$OUT")"
