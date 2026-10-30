# App Web Server (Phone Pages + Module Firmware Update) — Design Notes

**Date:** 2026-10-28, updated 2026-10-30
**Status:**
- **Phone pages (switches + calibration) and tablet hotspot: BUILT** and bench-tested on a tablet
  with a Nano module (commit `bd34842e` for the switches, 2026-10-30 commit for the rest).
  Help pages + user manual updated 2026-10-30.
- **In-app module firmware update: not started** (design below unchanged).

---

## Origin

Forum question (user e3120): the RC15 (ESP32) lets a phone switch sections from its own
hotspot web page, handy for walking the boom to check for blocked nozzles. Can an RC11
(Teensy) do the same? Their setup: tablet, AIO, RC11 and switchbox on an Ethernet hub;
tablet gets internet from the phone's hotspot. Occasional use, doesn't need to be pretty.

Follow-on question: the RC15 also updates its firmware from its web page — could one app
web page update Teensy, ESP32 and Nano alike?

## Module-side background

### Phone switches on the ESP32 module

- ESP32 runs `WIFI_MODE_APSTA` with its own softAP and web server (`Begin.ino`).
- `GUI.ino` `ButtonPressed()` toggles `WifiMasterOn` / `Button[0..15]` and restarts
  `WifiSwitchesTimer`.
- `Relays.ino`: while `WifiMasterOn`, relays are set **directly by relay number** (not
  section number), bypassing the app. **30 s** with no button press → `WifiMasterOn = false`,
  normal control resumes.
- Teensy (RC11) and Nano have no WiFi and no web server. The app is the bridge instead (below).

### Firmware update paths

| Board | Today | Network-capable? |
|---|---|---|
| ESP32 | Own web page `/update` (OTA, app `.bin` only). PCBsetup `frmFWESP32` flashes over USB serial via esptool (bootloader + partitions + app). | Yes |
| Teensy 4.1 | PCBsetup `frmFWTeensyNetwork`: PGN **32800** enters update mode, then Intel-hex records streamed over UDP (module listens 29100, replies 29000). Firmware side `EthernetUpdate.ino` (FlasherX). Also `frmFWTeensySerial` over USB. | **Yes — already over Ethernet**, but only PCBsetup implements the sender |
| Nano | PCBsetup `frmFWNanoRate`, USB serial (ArduinoUploader → bootloader). | **No** — no flash room for a network bootloader |

## Network: how a phone reaches the app

```
 Phone ──WiFi──▶ Tablet (RateController) ──Ethernet──▶ Hub ──▶ modules
                 phone-hotspot / router / tablet-hotspot IP     module subnet (e.g. 192.168.1.x)
```

The phone never talks to the modules; the app sends its normal PGNs, so **every board works
with no firmware change**. Three supported ways to share a network:

| Network | Tablet address | Notes |
|---|---|---|
| Phone's hotspot | DHCP from the phone (Android now uses random 10.x subnets) | Tablet must set the network **Private** (see Firewall) |
| Any WiFi router (Starlink, cellular, …) | DHCP from the router | Private; main network not guest; AP/client isolation off; DHCP reservation keeps the QR stable |
| Tablet's own Windows Mobile Hotspot | always **192.168.137.1** | Started by the app (Tablet Hotspot checkbox); needs the firewall rule |

Not documented for users (works, but not chosen): phone hotspot used only as a link while the
tablet gets internet from Starlink by Ethernet; Tailscale/VPN control from anywhere (out-of-sight
control, rejected).

## Implementation

### Files

| File | Role |
|---|---|
| `Classes/clsWebServer.cs` | HTTP server, pages, endpoints, address list, firewall rule, tablet hotspot + watchdog |
| `Classes/clsQrCode.cs` | Hand-written QR encoder (no NuGet dependency) |
| `PGNs/PGN32618.cs` | Remote switch session merged into the switchbox state |
| `Classes/clsVirtualSwitchBox.cs` | Master presses made momentary pulses (see Bugs) |
| `Classes/clsSectionControl.cs` | Prime end inside a phone session |
| `Classes/clsCalibrationSession.cs` | Shared calibration owner for the tablet form and the phone |
| `Classes/clsCalibrate.cs`, `clsProducts.cs` | Recovery records, Base Rate, Meter set persistence |
| `Menu/frmMenuOptions(.Designer).cs` | Speed / Phone tabs: QR, address, Tablet Hotspot, links |
| `Menu/frmMenuCalibrate.cs` | Now a view on `Core.CalSession` |
| `Classes/Core.cs`, `Props.cs` | Start/stop, `WebServerPort`, `TabletHotspot`, recovery at startup |

### Server (`clsWebServer`)

- `TcpListener` on all interfaces, port **8080** (`Props.WebServerPort`, app prop, settings file
  only). Chosen over `HttpListener`, which needs an admin `urlacl`.
- **Always on, no enable switch, no PIN** (decision 2026-10-29). A failed start is only logged
  (`clsWebServer/Start`); the Phone tab shows "Not running, port 8080 may be in use".
- Accept thread → **each client on the ThreadPool** (an idle Chrome preconnect used to stall the
  single thread for the 2 s read timeout). `IOException` from idle/dropped connections is ignored.
- All app state is read and written on the UI thread via `Core.MainForm.Invoke`.
- **Every POST needs header `X-RC`** (else 403): a cross-site request guard in lieu of a PIN —
  another site can't add a custom header without a CORS preflight, which the server never
  answers. Status GETs are open.
- `Cache-Control: no-store`, `Connection: close`.

| Endpoint | Purpose |
|---|---|
| `GET /` | Menu page "RateController": Switches, Calibration |
| `GET /switches`, `GET /status` | Switches page; status JSON (states, session, colours) polled 1 s |
| `POST /press?id=master\|prime\|autorate\|autosection\|up\|down\|uprep\|downrep\|release\|N` | Switch presses |
| `GET /cal`, `GET /calstatus?p=N` | Calibration page; status JSON polled 1 s (also the phone's presence signal) |
| `POST /calpress?id=power\|lock\|start\|stop\|save&p=N[&s=&b=&m=&me=&w=]` | Calibration actions |

Pages use the tablet's display colours, a full-size "‹ Menu" button at the bottom, and stop
polling while `document.hidden`. JSON numbers are invariant culture; product names are escaped.

### Switches page and switchbox reconciliation (`PGN32618`)

The page is a replica of the on-screen panel: MST · PRM · ▲ · ▼, switches 4/row
(`Props.SwitchCount`, full panel even in Rate Only), Auto Rate · Auto Section.

The presses can't use the on-screen panel's path: `clsVirtualSwitchBox.SendPGN()` drops presses
while a real switchbox is connected, and `ParseByteData()` overwrites `SW[]` on every packet.
Model as built — **remote takeover with cab priority**:

1. **Overlay, not split arrays.** `ParseByteData()` still parses every packet into `SW[]`;
   while a session is active `ApplyRemote()` then overwrites sections, Auto Section, Auto Rate,
   rate up/down and master with the page's state. Downstream code is unchanged.
2. **First press starts a session, seeded** from the current `SW[]`/master, so takeover changes
   nothing until a button is pressed.
3. **Any cab change ends it ("cab wins").** `CheckCabChanges()` compares each packet with the
   previous packet **from the same source** (real box and on-screen panel tracked separately;
   a source silent for 4 s starts fresh). Section bytes or master/rate/auto bits changing ends the
   session; **the work switch is excluded**.
4. **Master:** the page owns master during a session. Momentary: a page press pulses
   `SW[MasterOn/Off]` and clears it again so it can't be read as a held press (prime); resent
   momentary bits are blanked so a level one source keeps resending can't act as a press.
   Maintained: the level follows the page.
5. **Timeout 120 s** with no press (was 30 s; raised 2026-10-30 — 30 s cut master mid boom-walk;
   status polls deliberately don't count as presence) → session ends, **master off**, switches
   revert to the newest source's positions. With a maintained master, `cRemoteForcedOff` keeps
   master off until the physical master bits change.
6. **▲/▼ held:** the page sends the press then a repeat every 250 ms; the app releases a hold not
   repeated within 750 ms (dropped phone). Repeats/release never restart a session the cab ended.
7. **Prime:** like the on-screen PRM (momentary master and stopped only). If prime ends without
   Resume After Prime during a session, `RemotePrimeEnded()` turns master off inside the session
   instead of pressing the on-screen MasterOff (which read as a cab change).

The RC15's own module web page drives relays directly at the module and has the same conflict with
a switchbox at module level — out of scope.

### Calibration page (`clsCalibrationSession`)

Calibration state used to live inside `frmMenuCalibrate` (created on Load, restored on close), so
nothing existed for a web page to drive. It now has one owner, `Core.CalSession`:

- **Open** while the tablet Calibrate form is open **or** the phone polled `/calstatus` within
  120 s; closed when both are done, on `ChangeProfile` (first, so products restore into their own
  profile) and in `AppShutDown` (after `WebServer.Stop`).
- `Start(product, FromWeb)` records the owner. **Dead-man only for phone-started runs:** no
  `/calstatus` for 5 s → `Stop()`, page shows "Last run stopped: phone connection lost".
  Tablet-started runs never auto-stop (operator chose the tablet). Closing the form stops a
  tablet-owned run; a phone-owned run continues under its dead-man.
- The form refreshes every second (shows phone-started runs) **only while it has no unsaved edits**.
- Page: product 1–5, Power, lock shown as **Meter set / Meter not set**, Start/Stop, Pulses, Hz,
  Speed, Base Rate, Meter Cal, Expected, Measured, PWM (motor/fan), Save/Cancel. Tapping a box clears
  it with the old value as placeholder (select-all popped the phone's copy/paste menu); blur without
  typing restores it; polling skips the focused box and stops refilling once anything is edited.
- **Base Rate** box now applies: `clsCalibrate.Save()` sets `Product.RateSet` when the product is
  powered on (it was stored in an unused field).
- **Meter set persists**, saved only on Save: profile prop `<ProductName>_CalLocked` beside
  `_CalPWM`.
- **`clsCalibrate.Close()` restores only powered-on products** (it used to restore all five,
  reverting product-enable changes made while a session was open).

**Interrupted-calibration recovery** (power loss, crash, Task Manager kill):
- Power-on writes profile prop `CalRestore<ID>` = `AppMode|Enabled|OffRateAlarm` **before** any
  calibration value is set; `RestoreState()` clears it. A run writes app prop `CalSpeedRestore` =
  original speed source; `Stop()` clears it. Both write to disk immediately (`SetProp`/`SetAppProp`).
- `clsProducts.Load()` (startup and profile change) restores any product with a record (ManualPWM 0),
  saves, clears, logs and shows "Product N restored after an interrupted calibration";
  `Core.Initialize` restores the speed source. All before any module traffic.

### Tablet side: Options → Phone tab

- frmMenuOptions is tabbed like frmMenuComm: **Speed** (old contents) + **Phone**.
  (A first version put the QR on frmMenuSwitches; moved here because the server is more than switches.)
- **QR** from `clsQrCode`: fixed version 3 (29×29), EC level M, byte mode, mask 0; 42-byte capacity
  (longest URL `http://255.255.255.255:65535` is 28). Reed-Solomon checked against the Thonky
  "HELLO WORLD" 1-M vector; scans on phones.
- **Address ranking** (`PhoneUrls()`): tablet hotspot (adapter "Wi-Fi Direct") > WiFi > adapters with
  a gateway > rest > virtual (VirtualBox, Hyper-V). Module subnet and 169.254.x excluded. Tap the
  QR/address to cycle. A 2 s timer refreshes the list while the tab is shown and **resets to the top
  on any change** (the hotspot adapter comes up seconds after Windows reports "On").
- Instructions (LinkLabel) in the order that works: same network → **Private** (link "WiFi settings"
  → `ms-settings:network-wifi`) → scan.

### Tablet hotspot

- **Tablet Hotspot** checkbox (`Props.TabletHotspot`, app prop), applied on Save like the other
  checkboxes. Started at app start when set (Windows doesn't keep it across a reboot); left running
  at exit.
- Control: hidden `powershell.exe -EncodedCommand` calling WinRT
  `NetworkOperatorTetheringManager` — **no admin needed**, no project references
  (tested: start 1.9 s, stop 2.6 s). Shares the internet profile, else the first tethering-enabled
  profile. `$ProgressPreference='SilentlyContinue'` is required or every call writes CLIXML progress
  to stderr. Runs on the ThreadPool; results applied on the UI thread.
- **Watchdog** (30 s): while the checkbox is set, a hotspot seen Off is restarted, **max 3 restarts
  since it was seen Off**; reset when seen On, on Save, at app start; "Unknown" not counted. Tab
  status then shows "Off, restarts failed". The checkbox is the intent, not a mirror of Windows'
  switch.
- Phone tab polls status every 6 s; shows network name + password, or Windows' failure reason.
- **Power saving** (hotspot off after 5 min with no device) **can't be changed by the app**:
  `HKLM\...\icssvc\Settings\PeerlessTimeoutEnabled = 0` was written but Settings still showed it on,
  even after restarting `icssvc`. Removed; the tab shows "If necessary, turn off Power saving in
  Hotspot settings" (link → `ms-settings:network-mobilehotspot`).

### Firewall

- Phone hotspots and new WiFi networks come up **Public**; the exe's allow rule is often
  Private-only → the page is blocked. Users set the network **Private** — again for every new phone
  or renamed hotspot (Windows treats it as a new network).
- The tablet hotspot's network has **no profile** (counts as Public, can't be set Private). On the
  first Save with Tablet Hotspot checked and the rule missing, one UAC prompt runs:
  `netsh advfirewall firewall add rule name="RateController phone hotspot" dir=in action=allow
  protocol=TCP localport=<port> profile=public remoteip=192.168.137.0/24`.
  Port-based (survives upgrades and folder moves), limited to the hotspot subnet. Checking for the
  rule (`netsh ... show rule`, exit 0/1) needs no admin.

## Bugs found and fixed during bench testing

| Symptom | Cause | Fix |
|---|---|---|
| Page master on, then off at once | On-screen panel left its Master-Off bit set; resent every 250 ms | Page owns master in `ApplyRemote()`; `PressSwitch(MasterOn/Off)` arms `ReleaseTimer` (pulse) |
| First on-screen master-off ignored during a phone session | Same stuck bit: the press didn't change the packet | Same `ReleaseTimer` fix |
| Prime end ended the phone session, sections off | `PrimingTimerTick` pressed the on-screen MasterOff | `RemotePrimeEnded()` when a session is active |
| Phone page wouldn't load | Firewall, Public network | Private network / hotspot rule (above) |
| QR showed an unreachable address | Hotspot adapter ranked last as "Virtual"; list read before the adapter was up | Ranking + 2 s refresh resetting to the top |
| Error log flooded | PowerShell progress on stderr | `$ProgressPreference` |
| After restart, motor ran as in calibration | Shutdown never closed the calibration session; calibration settings stayed saved | `CalSession.Close()` in `AppShutDown` |
| Product enable changes reverted | `clsCalibrate.Close()` restored all products | Restore only powered-on products |
| Tablet Calibrate edits lost | Always-on refresh rewrote the boxes | Refresh only without unsaved edits |

## Module firmware update — in the app, NOT on the web page (not started)

Decided 2026-10-28: the web page has no firmware update. Updating is done at the tablet anyway,
and a remote flash adds risk (phone drops off the network mid-update, anyone on the network could
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
- Open: does ESP32 `/update` accept the upload from the Ethernet side as well as its softAP? (Verify.)

## Decisions log

| Date | Decision |
|---|---|
| 2026-10-28 | Web page is switches only (later + calibration); firmware update stays in the app |
| 2026-10-29 | Page = replica of the on-screen panel; title "RateController Switches" |
| 2026-10-29 | Work switch excluded from "cab took control"; Auto Section on the page (operator can turn auto off from the boom) |
| 2026-10-29 | Always on, no PIN, no enable button; DIY QR encoder over QRCoder |
| 2026-10-30 | Remote timeout 120 s (not "polls count as presence") |
| 2026-10-30 | `X-RC` header on POSTs; ThreadPool clients |
| 2026-10-30 | Calibration page; dead-man only for phone-started runs; Base Rate applies on Save |
| 2026-10-30 | QR moved to Options → Phone tab; menu page at `/` |
| 2026-10-30 | Tablet Hotspot as a checkbox applied on Save, restarted at app start; watchdog max 3 restarts |
| 2026-10-30 | Power saving left to the user (registry approach failed) |
| 2026-10-30 | Meter set persists, saved only on Save (first chosen save-on-change, then revised) |
| 2026-10-30 | Interrupted-calibration recovery records |

## Open / not done

- Translations: all new text is English only.
- Field tests: real switchbox + phone together, maintained master, ▲/▼ hold with the phone dropping,
  tablet hotspot with **no internet at all** (whether Windows starts it is untested).
- Random ~0.5 s Nano motor runs seen once during testing, not reproduced after a restart. The Nano
  can't run a motor without the app sending master on; the "switchbox missing → master on" path in
  `PGN32500` was ruled out. Possibly the stuck-calibration-settings bug (since fixed). A logger of
  master-bit changes in `PGN32500` was offered and deferred.
- A power loss while a profile file is being written can still damage the file (pre-existing).
- PIN / password: not implemented (accepted; the `X-RC` header only blocks cross-site requests).
