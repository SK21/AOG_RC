# App Web Server (Phone Remote Switches + Module Firmware Update) — Design Notes

**Date:** 2026-10-28
**Status:** Idea / **Not started** (no code change made)

---

## Origin

Forum question (user e3120): the RC15 (ESP32) lets a phone switch sections from its own
hotspot web page, handy for walking the boom to check for blocked nozzles. Can an RC11
(Teensy) do the same? Their setup: tablet, AIO, RC11 and switchbox on an Ethernet hub;
tablet gets internet from the phone's hotspot. Occasional use, doesn't need to be pretty.

Follow-on question: the RC15 also updates its firmware from its web page — could one app
web page update Teensy, ESP32 and Nano alike?

Short-term answer given to the user: remote desktop (AnyDesk / Chrome Remote Desktop) from
the phone to the tablet and use the app's on-screen section switches.

## Current state

### Phone switches — ESP32 only

- ESP32 runs `WIFI_MODE_APSTA` with its own softAP and web server (`Begin.ino`).
- `GUI.ino` `ButtonPressed()` toggles `WifiMasterOn` / `Button[0..15]` and restarts
  `WifiSwitchesTimer`.
- `Relays.ino`: while `WifiMasterOn`, relays are set **directly by relay number** (not
  section number), bypassing the app. **30 s** with no button press → `WifiMasterOn = false`,
  normal control resumes.
- Teensy (RC11) and Nano have no WiFi and no web server. An access point on the hub would
  not help — there is no page on the module to load.

### Firmware update paths

| Board | Today | Network-capable? |
|---|---|---|
| ESP32 | Own web page `/update` (OTA, app `.bin` only). PCBsetup `frmFWESP32` flashes over USB serial via esptool (bootloader + partitions + app). | Yes |
| Teensy 4.1 | PCBsetup `frmFWTeensyNetwork`: PGN **32800** enters update mode, then Intel-hex records streamed over UDP (module listens 29100, replies 29000). Firmware side `EthernetUpdate.ino` (FlasherX). Also `frmFWTeensySerial` over USB. | **Yes — already over Ethernet**, but only PCBsetup implements the sender |
| Nano | PCBsetup `frmFWNanoRate`, USB serial (ArduinoUploader → bootloader). | **No** — no flash room for a network bootloader |

## How a phone hotspot reaches the app

```
 Phone ──WiFi hotspot──▶ Tablet (RateController) ──Ethernet──▶ Hub ──▶ modules
 192.168.43.1 (Android)  192.168.43.x  |  192.168.1.x                192.168.1.x
 172.20.10.1  (iPhone)   172.20.10.x
```

- The hotspot phone is the router of that network, so it can open connections to the
  tablet's hotspot IP directly.
- The phone cannot reach the modules (different network); the **app is the bridge** — it
  already talks to every module.

## Proposed design

One small HTTP server inside RateController serving the remote switches page; firmware
updating is a separate in-app feature (below).

### Server

- `TcpListener` on a configurable port (e.g. 8080), all interfaces. Prefer `TcpListener`
  over `HttpListener` — the latter needs a one-time admin `netsh http add urlacl`.
- Off by default; enable checkbox on frmMenuOptions.
- App shows its URL (and optionally a QR code) — the tablet's hotspot IP changes per DHCP lease.
- Windows Firewall: new WiFi networks default to *Public* → inbound blocked. Needs a firewall
  rule (installer or first-enable prompt) or the user sets the network to Private.

### Remote switches page

- Master + section buttons only, plain HTML like the ESP32 page. Rate up/down and the auto
  buttons stay with the switchbox/app.
- Presses act on **sections, not relay numbers**, so the app sends its normal PGNs → works on
  Teensy, ESP32 and Nano with **no firmware change**.
- **Not** via the on-screen panel's path: `clsVirtualSwitchBox.SendPGN()` drops presses whenever
  `Core.SwitchBox.RealConnected()` (real PGN 32618 within 4 s), and `PGN32618.ParseByteData()`
  overwrites all of `SW[]` on every packet. With a switchbox on the hub (the forum user's setup)
  the page would silently do nothing. See *Switchbox reconciliation* below.
- **Dead-man timeout** (30 s without a press) — see below for what expiry does.
- The page polls a small status endpoint: live section/master states and a banner
  ("Remote in control" / "Cab switchbox took control").
- Unlike the RC15 page, requires the tablet + app running.

### Switchbox reconciliation

The two sources differ in kind: switchbox section switches are **maintained** (position = state,
resent every packet), master is **momentary edges** (a level if `MasterMaintained`); the page
has toggle buttons with no physical position. Level merging fails — OR can't turn off a section
that is physically on, AND can't turn on one that is physically off.

Model: **remote takeover with cab priority.**

1. **Split state in `PGN32618`.** The real box writes a *physical* array; a separate *remote*
   array holds page state. `SectionSwitchOn()` returns remote while a session is active, else
   physical. Downstream (`clsSectionControl`, relays, PGN 32501) unchanged. (Same split would also
   let the on-screen panel coexist with a real box — optional.)
2. **First press starts a session, seeded** from the current physical sections + master, so
   takeover changes nothing until a button is pressed.
3. **Any physical change ends the session immediately** ("cab wins"). Detect *edges* — a bit
   differing from the previous real packet — not levels, since the box resends continuously.
   First packet after a reconnect is a baseline, not an edge. This also handles
   `MasterMaintained` (resent level; only a flip counts).
4. **Master is edge-merged:** an on/off from either source takes effect, last edge wins.
   Switchbox master-off is always honoured and ends the session.
5. **Dead-man expiry → master off**, sections revert to physical. Unlike the ESP32 ("normal
   control resumes"), resuming here would restore every switch position with master still on
   while the operator is out of the cab. Operator re-presses master in the cab.

Note: the RC15's own web page drives relays directly at the module, bypassing the app — it has
the same conflict with a switchbox at module level. Out of scope here.

### Module firmware update — in the app, NOT on the web page

Decided 2026-10-28: the web page is switches only. Updating is done at the tablet anyway, and
a remote flash adds risk (phone drops off the hotspot mid-update, anyone on the hotspot could
start one) for no real gain.

Build it as an app menu page ("Update Modules"), per-board backend chosen from module type
(PGN 32401 InoType):

```
App ──▶ Teensy : PGN 32800 + hex records over UDP (port from PCBsetup frmFWTeensyNetwork)
    ├─▶ ESP32  : HTTP POST app .bin to the module's /update
    └─▶ Nano   : USB serial (PCBsetup frmFWNanoRate code), only if plugged into the tablet
```

- **Bundle firmware with the app release** and list each connected module:
  `Module 0 – Teensy, InoID 20106 → bundled 28106 [Update]`.
- **Safety gate:** refuse while Master is on / anything applying; confirm before flashing.
  ESP32 OTA (dual partition) and FlasherX (buffer then copy) both keep the old firmware if the
  transfer breaks mid-way.

## Recommended order

1. **In-app "Update Modules" page**: move PCBsetup's Teensy network sender into the app, add
   ESP32 `/update` forwarding, optionally Nano serial. Independent of the web server.
2. **Web server + remote switches page** — small, firmware-free.

## Open questions

- Port number and whether to add a simple PIN/password (anyone on the hotspot could take over
  section control, not just press buttons).
- Takeover/timeout policy: confirm 30 s dead-man and master-off on expiry; whether the on-screen
  panel should use the same physical/remote split.
- Does ESP32 `/update` accept the upload from the Ethernet side as well as its softAP? (Verify.)
- Some phones can't be hotspot and WiFi client at the same time — fine here, the phone is
  only ever the hotspot.
