
// ── WiFi station retry policy ────────────────────────────────────────────
// "Use this Network" is the user's intent, and the firmware never changes it —
// it stays ticked until the user unticks it on the network page. What the
// firmware manages instead is how OFTEN it retries and how EXPENSIVE each
// attempt is.
//
// Keeping those two separate is the whole point. The old code counted
// disconnects and, at six, wrote WifiModeUseStation = false and restarted —
// reading "the network is unreachable" as "the user no longer wants station
// mode", and rebooting a module that may be mid-application to act on it.
//
// The problem it was working around is real, though. The ESP32 has one radio
// shared by the softAP and the station, so every connect attempt takes the radio
// off the softAP's channel and the hotspot goes deaf for the duration:
//     directed attempt (cached channel + BSSID)            ~100-500 ms
//     full scan (IDF default ~120 ms dwell × 13 channels)   ~1.5-2 s
// The old handler called WiFi.begin() on every disconnect event with no gap at
// all, while the core's own auto-reconnect fired on that same event — two retry
// drivers racing, so the radio was off-channel essentially all of the time. On a
// rate module that window is not telemetry, it is rate commands going missing.
// Pacing the attempts, and preferring the cheap kind, is the actual fix;
// setAutoReconnect(false) in StartWifiStation() leaves exactly one retry driver,
// which is ServiceWifiStation().
//
// Note the asymmetry between the two attempt types: they are not the same event
// and must not be paced by the same rule. A directed attempt costs about as much
// hotspot airtime as one dropped UDP packet. A full scan costs ten times that,
// so it is rationed separately below and suppressed entirely while anyone is
// actually using the portal.
//
// Idle hotspot (nobody associated) — revised 2026-11-01 for the tablet hotspot,
// ported from YieldFlo. The August timings backed off to one attempt a minute after
// 30 s, and scanned only every 5th attempt. With the tablet as the network, nobody
// is ever on this hotspot, so that rationing protected no one and cost up to a
// minute after every outage (a module WiFi setup run, an app or PC restart), and
// minutes more when the tablet's hotspot came back on a different channel. Now:
// fast for the first two minutes after the link is lost or the module boots — when
// a reconnect is most likely to succeed — then a steady slower pace, never giving
// up. Off-air time is the cost: someone joining this hotspot while the station is
// searching can land in a gap and fail. A full scan (~2 s) is the big gap, so full
// scans come no more than once every 15 s. That is ~13% off-air at worst in the
// fast window, under 10% after it.
const uint32_t StaFastWindowMs     = 120000;	// fast pace this long after link loss / boot
const uint32_t StaRetryFastMs      = 5000;		// idle, fast window, channel cached (full scan every 3rd = 15 s)
const uint32_t StaRetrySlowMs      = 15000;		// idle, after the fast window, channel cached
const uint32_t StaRetryScanFastMs  = 15000;		// idle, fast window, nothing cached (all full scans)
const uint32_t StaRetryScanSlowMs  = 30000;		// idle, after the fast window, nothing cached
const uint8_t  StaScanEveryFast    = 3;			// full scan every Nth attempt in the fast window
const uint8_t  StaScanEverySlow    = 4;			// and after it (once a minute at StaRetrySlowMs)
const uint32_t StaRetryBusyMs  = 120000;	// a client is associated but not using the portal
const uint32_t StaRetryScanOnlyMs = 300000;	// client associated AND no channel cached, so every
											// attempt is necessarily the expensive kind
const uint32_t StaScanEarnedMs = 1800000;	// 30 min of failed directed attempts earns one full scan
const uint32_t PortalActiveMs  = 60000;		// the portal counts as in use this long after a request

// Credentials refused is a different failure from "the network is not there",
// and deserves a different answer. Retrying a password the router has already
// rejected achieves nothing, and some routers temporarily blacklist a client
// that keeps failing authentication. So back off hard — but never give up: the
// router may later be reconfigured to accept us, and the user's tick stays
// ticked regardless. What the user gets instead is a plain statement on the
// network page that the password was refused, which is the actionable fact.
const uint32_t StaRetryAuthMs   = 600000;	// 10 min once the password looks wrong
const uint8_t  StaAuthFailLimit = 3;		// consecutive auth failures before believing it

bool     StaConnected = false;			// GOT_IP seen and not yet followed by a disconnect
uint32_t StaLastAttemptMs = 0;			// millis() of the last WiFi.begin()
uint32_t StaDownSinceMs = 0;			// millis() the link was lost / station started — starts the fast window
uint8_t  StaFailCount = 0;				// consecutive failed attempts
uint8_t  StaSinceScan = 0;				// directed attempts since the last full scan
uint32_t StaDirectedFailMs = 0;			// millis() directed-only failures began; 0 = not failing
bool     StaForceScan = false;			// network page "Retry Connection Now" pressed
uint8_t  StaChannel = 0;				// channel of the last successful join; 0 = none cached
uint8_t  StaBssid[6] = { 0 };			// BSSID of the last successful join
bool     StaHaveBssid = false;
bool     StaChannelDirty = false;		// cached channel changed and needs storing
bool     StaChannelStored = false;		// the cache has been written once this boot — see PersistStaChannel()
uint8_t  StaAuthFailCount = 0;			// consecutive credential rejections
bool     StaAuthSuspect = false;		// the password is being refused — back off, and say so
bool     StaAttemptPending = false;		// a WiFi.begin() is in flight; times the off-channel window
uint32_t LastPortalMs = 0;				// millis() of the last real portal page request; 0 = never
const uint8_t StaLogAttempts = 3;		// attempts printed in full; after that retries continue silently
uint8_t  StaLogCount = 0;				// attempts printed since the last success or Retry press

// The two codes that actually mean the PSK was rejected: the router refused the
// association outright (202), or the 4-way handshake never completed (15).
//
// AUTH_EXPIRE (2) and HANDSHAKE_TIMEOUT (204) are deliberately NOT here, even
// though a bad password can produce them. Both are also what a weak or fading
// link reports, and StaAuthFailCount only resets on a DIFFERENT reason — so a
// machine driving out of range produces a run of the same code and would latch
// a "password refused" verdict about a password that is perfectly correct,
// sending the user to fix the one thing that is not broken.
//
// The cost of leaving them out is a router that only ever reports 204 for a bad
// password: the module then never latches and keeps retrying at the normal
// cadence. That is the pre-rework behaviour — it spends retry airtime instead
// of making a confident false accusation, which is the better of the two.
static bool IsAuthFailureReason(uint8_t reason)
{
	return (reason == WIFI_REASON_4WAY_HANDSHAKE_TIMEOUT	// 15
		|| reason == WIFI_REASON_AUTH_FAIL);				// 202
}

// ── Startup ──────────────────────────────────────────────────────────────
// The access point itself is still brought up in Begin.ino, where it always
// was. It reads MDLnetwork.StaChannelCache directly for its channel: one radio
// serves both interfaces and they cannot sit on different channels, so joining a
// network on any other channel DRAGS the softAP across and disconnects everyone
// on it. Starting where the join will land makes that a non-event.

void StartWifiStation()
{
	if (!MDLnetwork.WifiModeUseStation) return;

	delay(1000);
	WiFi.onEvent(WiFiStationConnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_CONNECTED);
	WiFi.onEvent(WiFiGotIP, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_GOT_IP);
	WiFi.onEvent(WiFiStationDisconnected, WiFiEvent_t::ARDUINO_EVENT_WIFI_STA_DISCONNECTED);

	// Leave ServiceWifiStation() as the only thing that retries. The core's own
	// auto-reconnect fires straight off the disconnect event with no pacing, so
	// leaving it on would defeat every interval in this file — that second,
	// invisible retry driver is half of what made the hotspot unusable before.
	WiFi.setAutoReconnect(false);

	// Seed the directed-attempt channel from the last successful join, so the
	// first retry after a reboot is already the cheap kind.
	StaChannel = MDLnetwork.StaChannelCache;

	StaLastAttemptMs = millis();
	StaDownSinceMs = StaLastAttemptMs;		// boot opens the fast window
	StaAttemptPending = true;
	WiFi.begin(MDLnetwork.SSID, MDLnetwork.Password);
	Serial.println();
	Serial.println("Connecting to wifi network ...");
}

// Store the cached channel. It lives in the identity record, so the whole record is
// rewritten (its CRC covers every byte); everything else in it already matches what is
// stored, because each other change to those fields saves and restarts at once.
// Called from loop(), not from the event handler, so the flash write never stalls event dispatch.
//
// Once per boot, and no more. EEPROM.commit() erases and rewrites a whole flash
// sector, and two access points sharing one SSID on different channels — the
// mesh that ScanResultsHtml() collapses into a single row — would otherwise
// alternate this byte and cost a sector erase on every reconnection, with
// nothing bounding it. Freezing after the first write loses nothing, because
// the stored byte is only ever READ at boot: Begin.ino picks the softAP channel
// from it and StartWifiStation() seeds StaChannel from it. StaChannel itself
// keeps following the live channel, so directed retries still track a roam.
void PersistStaChannel()
{
	if (!StaChannelDirty) return;
	StaChannelDirty = false;
	if (StaChannelStored) return;								// already written this boot
	if (MDLnetwork.StaChannelCache == StaChannel) return;		// nothing to write

	StaChannelStored = true;
	MDLnetwork.StaChannelCache = StaChannel;
	SaveIdentity();
	Serial.print("Stored network channel ");
	Serial.println(StaChannel);
}

// ── Event handlers ───────────────────────────────────────────────────────

void WiFiStationConnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Connected to '");
	Serial.print(MDLnetwork.SSID);
	Serial.println("'");
}

void WiFiGotIP(WiFiEvent_t event, WiFiEventInfo_t info)
{
	Serial.print("Network IP: ");
	Serial.println(WiFi.localIP());
	// subnet broadcast from the real mask, not an assumed /24: an iPhone hotspot is
	// 172.20.10.0/28, where .255 is off-subnet and goes to the phone as gateway (dropped)
	IPAddress Wifi_LocalIP = WiFi.localIP();
	IPAddress Wifi_Mask = WiFi.subnetMask();
	for (int i = 0; i < 4; i++)
	{
		Wifi_DestinationIP[i] = Wifi_LocalIP[i] | (uint8_t)~Wifi_Mask[i];
	}

	StaConnected = true;
	StaFailCount = 0;
	StaSinceScan = 0;
	StaDirectedFailMs = 0;
	StaAuthFailCount = 0;
	StaAuthSuspect = false;
	StaAttemptPending = false;
	StaLogCount = 0;		// a later dropout prints its first attempts again

	// Cache where this network actually lives. The next reconnect can then skip
	// the scan entirely — the difference between ~300 ms and ~2 s of the hotspot
	// being deaf — and the AP can be brought up on this channel next boot, so a
	// successful join no longer drags the softAP across channels and drops every
	// client already on it.
	StaChannel = (uint8_t)WiFi.channel();
	StaHaveBssid = (WiFi.BSSID(StaBssid) != NULL);
	StaChannelDirty = true;		// PersistStaChannel() writes it from loop()
	Serial.print("Network channel: ");
	Serial.println(StaChannel);
}

void WiFiStationDisconnected(WiFiEvent_t event, WiFiEventInfo_t info)
{
	// Runs on the Arduino event task, so it only records state — the retry
	// itself is paced from loop() by ServiceWifiStation(). Nothing here blocks
	// event dispatch and nothing here touches flash, and nothing here restarts
	// the module.
	//
	// Losing a working link opens the fast window. A failed attempt while already
	// down does not — otherwise the window would never close.
	bool wasConnected = StaConnected;	// read before it's cleared below
	if (StaConnected) StaDownSinceMs = millis();
	StaConnected = false;

	uint8_t reason = info.wifi_sta_disconnected.reason;
	bool authFail = IsAuthFailureReason(reason);

	if (authFail)
	{
		if (StaAuthFailCount < 255) StaAuthFailCount++;
		if (StaAuthFailCount >= StaAuthFailLimit) StaAuthSuspect = true;
	}
	else
	{
		// A different kind of failure means the situation changed — an absent
		// network is not evidence about the password, so start that count over.
		StaAuthFailCount = 0;
		StaAuthSuspect = false;
	}

	// A working link that dropped is always reported; a failed attempt only while
	// attempts are still being printed (see StaLogAttempts in ServiceWifiStation)
	if (wasConnected || StaLogCount <= StaLogAttempts)
	{
		Serial.print("WiFi lost connection. Reason: ");
		Serial.print(reason);
		if (authFail) Serial.print(" (auth)");

		// Time from WiFi.begin() to the verdict — this is the window the radio spent
		// off the softAP's channel, and the number every pacing decision in this file
		// was sized against. Printed only when an attempt was actually in flight, so
		// a beacon timeout on a healthy link does not report a meaningless duration.
		if (StaAttemptPending)
		{
			Serial.print(" after ");
			Serial.print(millis() - StaLastAttemptMs);
			Serial.print(" ms");
		}
		Serial.println();
	}
	StaAttemptPending = false;

	// The station's subnet broadcast died with the link, so sending there is a
	// black hole. Put the destination back on the module's own AP subnet and a
	// tablet sitting on the hotspot keeps receiving through the outage.
	Wifi_DestinationIP = IPAddress(192, 168, MDL.ID + 200, 255);
}

// ── Accessors ────────────────────────────────────────────────────────────
// Wifi.ino sorts last, so the sketch preprocessor appends it after Begin.ino,
// GUI.ino and the page files. Those files can call functions declared here — the
// preprocessor generates prototypes — but cannot see the variables above, which
// is why anything they need goes through one of these.

// Called from the page handlers. A page load means the user is standing at the
// portal right now, which is exactly when a connect attempt would be noticed.
void NotePortalRequest()
{
	LastPortalMs = millis();
	if (LastPortalMs == 0) LastPortalMs = 1;	// 0 is reserved for "never"
}

// "Retry Connection Now" on the network page. A full scan is the only way to
// find a network that has moved channel or been replaced, and it is also the
// most disruptive thing this radio does — so it happens when the user asks for
// it and is expecting a hiccup, rather than silently underneath them.
void RequestNetworkRetry()
{
	StaForceScan = true;
	StaDownSinceMs = millis();		// and the fast pace again, as after a fresh loss

	// A deliberate press earns a clean slate — the user may have just fixed
	// something at the router end that this side cannot see.
	StaAuthFailCount = 0;
	StaAuthSuspect = false;
	StaLogCount = 0;			// and the attempts are printed again
}

// True once the network has refused our password often enough to believe it.
// The page says so plainly rather than leaving "not connected" to cover a wrong
// password, an absent router and a dead aerial alike.
bool StaAuthRejected()
{
	return StaAuthSuspect;
}

// ── Paced station retry ──────────────────────────────────────────────────
// Called every loop(). Returns immediately once connected: station mode costs
// nothing in steady state, and only the "configured but unreachable" state
// needs rationing — which is precisely the state that becomes permanent now
// that the user's tick is never cleared.
void ServiceWifiStation()
{
	if (!MDLnetwork.WifiModeUseStation) return;
	PersistStaChannel();		// no-op unless a join just changed it
	if (StaConnected) return;

	uint32_t now = millis();
	bool forced = StaForceScan;

	// Cheap gate FIRST. Everything below this line asks the WiFi driver
	// something, and this function runs on every loop(). In particular
	// softAPgetStationNum() calls esp_wifi_ap_get_sta_list(), which takes the
	// driver lock; calling that a thousand times a second starves the softAP's
	// own beacon and management work and leaves the hotspot unreachable for
	// minutes. No track can ever come due sooner than StaRetryFastMs, so until
	// that has elapsed there is nothing worth asking.
	if (!forced && (now - StaLastAttemptMs) < StaRetryFastMs) return;

	// An OTA upload must never be interrupted: it arrives over the hotspot, and
	// a channel excursion mid-transfer fails the flash write. Nothing about a
	// station retry is urgent enough to risk that.
	if (Update.isRunning()) return;

	// A scan owns the radio until it finishes. Starting a connect on top of it
	// makes the driver abort one or the other.
	if (WiFi.scanComplete() == WIFI_SCAN_RUNNING) return;

	// Someone loading pages is using the web UI right now. Suspend retries
	// outright rather than merely slowing them — the user is configuring for a
	// couple of minutes, losing retries for that window costs nothing, and it
	// removes any question of the radio competing with the page they are waiting
	// on. The retry button is the deliberate exception.
	if (!forced && LastPortalMs != 0 && (now - LastPortalMs) < PortalActiveMs) return;

	// A client associated to the hotspot but not loading pages is most likely
	// the tablet running the app. It can afford a directed attempt — about one
	// dropped packet — but not a 2 s scan, so stretch the interval and stay
	// cheap. This is also the normal all-day state, which is why the full scan
	// needs the earned-after-30-minutes escape below: without it, station mode
	// could never recover from a network that changed channel.
	// A refused password outranks everything else: no interval short enough to
	// matter is going to make the router change its mind.
	bool clientOnAp = (WiFi.softAPgetStationNum() > 0);

	// Whether a cheap attempt is available at all. Decided up here rather than
	// with the attempt-type logic below, because when the answer is no it has to
	// change the INTERVAL and not just the attempt: with nothing cached every
	// attempt is a full scan, and a 2 s scan every StaRetryBusyMs is the airtime
	// bill this whole file exists to avoid. Frequency is the only lever left.
	// Not an exotic state either — it is what a mistyped SSID leaves behind, or
	// ticking the box before the shed router is switched on.
	bool canDirect = (StaChannel != 0);
	bool fastWindow = (now - StaDownSinceMs) < StaFastWindowMs;

	uint32_t interval;
	if (StaAuthSuspect)                interval = StaRetryAuthMs;
	else if (clientOnAp && !canDirect) interval = StaRetryScanOnlyMs;
	else if (clientOnAp)               interval = StaRetryBusyMs;
	else if (!canDirect)               interval = fastWindow ? StaRetryScanFastMs : StaRetryScanSlowMs;
	else                               interval = fastWindow ? StaRetryFastMs : StaRetrySlowMs;

	if (!forced && (now - StaLastAttemptMs) < interval) return;

	// Attempt type. Directed whenever a channel is cached, because a full scan
	// costs roughly five times the hotspot airtime and is rationed accordingly.
	// The channel alone is enough — it narrows the scan from thirteen channels
	// to one, which is most of the saving — so a channel restored from EEPROM
	// works on the first attempt after a reboot, before any BSSID is known.
	bool fullScan;
	if (forced)          fullScan = true;		// user asked — find it wherever it moved to
	else if (!canDirect) fullScan = true;		// nothing cached, no cheaper option exists
	else if (clientOnAp) fullScan = (StaDirectedFailMs != 0 && (now - StaDirectedFailMs) >= StaScanEarnedMs);
	else                 fullScan = (StaSinceScan + 1 >= (fastWindow ? StaScanEveryFast : StaScanEverySlow));

	StaLastAttemptMs = now;
	StaForceScan = false;
	if (StaFailCount < 255) StaFailCount++;		// cleared by WiFiGotIP on success

	if (fullScan)
	{
		StaSinceScan = 0;
		StaDirectedFailMs = 0;
	}
	else
	{
		StaSinceScan++;
		if (StaDirectedFailMs == 0) StaDirectedFailMs = now;
	}

	// The first StaLogAttempts retries are printed, then one notice, then nothing until
	// a success or a Retry press resets StaLogCount — the retries themselves never stop
	if (StaLogCount < StaLogAttempts)
	{
		Serial.print("WiFi retry #");
		Serial.print(StaFailCount);
		if (fullScan)
		{
			Serial.print(" full scan");
		}
		else
		{
			Serial.print(" directed ch");
			Serial.print(StaChannel);
		}
		Serial.print(clientOnAp ? ", hotspot busy" : ", hotspot idle");
		Serial.println(StaAuthSuspect ? ", password refused" : "");
	}
	else if (StaLogCount == StaLogAttempts)
	{
		Serial.println("WiFi still not connected - retrying without further messages");
	}
	if (StaLogCount <= StaLogAttempts) StaLogCount++;

	StaAttemptPending = true;
	if (fullScan) WiFi.begin(MDLnetwork.SSID, MDLnetwork.Password);
	else          WiFi.begin(MDLnetwork.SSID, MDLnetwork.Password, StaChannel, StaHaveBssid ? StaBssid : NULL);
}
