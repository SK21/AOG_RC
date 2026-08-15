// An SSID is text chosen by whoever owns the other access point, and it lands in
// our HTML and our URLs. Escape it in both places rather than trusting it.
static String HtmlEscape(const String& s)
{
    String out;
    out.reserve(s.length() + 8);
    for (unsigned int i = 0; i < s.length(); i++)
    {
        char c = s[i];
        switch (c)
        {
        case '&':  out += "&amp;";  break;
        case '<':  out += "&lt;";   break;
        case '>':  out += "&gt;";   break;
        case '"':  out += "&quot;"; break;
        case '\'': out += "&#39;";  break;
        default:   out += c;        break;
        }
    }
    return out;
}

static String UrlEncode(const String& s)
{
    String out;
    out.reserve(s.length() + 8);
    for (unsigned int i = 0; i < s.length(); i++)
    {
        char c = s[i];
        if (isalnum((unsigned char)c) || c == '-' || c == '_' || c == '.' || c == '~')
        {
            out += c;
        }
        else
        {
            char b[4];
            sprintf(b, "%%%02X", (unsigned char)c);
            out += b;
        }
    }
    return out;
}

// Shown while an async scan is in flight. A meta refresh rather than AJAX keeps
// this page in the same server-rendered style as the rest of the portal.
String GetPageScanning()
{
    String st = "<HTML><head>";
    st += "<META content='text/html; charset=utf-8' http-equiv=Content-Type>";
    st += "<meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    st += "<meta http-equiv='refresh' content='4;url=/page2'>";
    st += "<title>Scanning</title>";
    st += "<style>";
    st += "html { font-family:Helvetica, Arial, sans-serif; text-align:center; }";
    st += "body { margin-top:50px; background-color:wheat; }";
    st += "h1 { color:#444; margin:50px auto 12px; text-decoration:underline; }";
    st += ".status { margin:2px auto 16px; font-size:16px; }";
    st += "a:link { font-size:150%; }";
    st += "</style></head><BODY>";
    st += "<h1>Scanning</h1>";
    st += "<p class='status'>Looking for networks.</p>";
    st += "<p class='status'>The hotspot pauses for a few seconds while the radio scans.</p>";
    st += "<p><a href='/page2'>Continue</a></p>";
    st += "</BODY></HTML>";
    return st;
}

// The results of the last completed scan, strongest first, one row per network
// name. Duplicates are collapsed because a mesh or a repeater lists the same
// name from every radio it has, which is noise to someone picking a network.
static String ScanResultsHtml()
{
    int16_t n = WiFi.scanComplete();

    if (n == WIFI_SCAN_RUNNING)
    {
        String st = "<meta http-equiv='refresh' content='3;url=/page2'>";
        st += "<p class='status'>Scanning ...</p>";
        return st;
    }

    if (n < 0) return "";		// no scan has been run this session
    if (n == 0) return "<p class='status'>No networks found.</p>";

    String st = "<h1 class='subhead'>Networks Found</h1>";
    st += "<table class='nets'>";

    for (int16_t i = 0; i < n; i++)
    {
        String ssid = WiFi.SSID(i);
        if (ssid.length() == 0) continue;		// hidden network — no name to offer

        // Keep only the strongest entry for each name. The tie-break on index
        // guarantees exactly one survivor rather than none or both.
        bool weaker = false;
        for (int16_t j = 0; j < n; j++)
        {
            if (j == i) continue;
            if (WiFi.SSID(j) != ssid) continue;
            if (WiFi.RSSI(j) > WiFi.RSSI(i) || (WiFi.RSSI(j) == WiFi.RSSI(i) && j < i))
            {
                weaker = true;
                break;
            }
        }
        if (weaker) continue;

        int32_t rssi = WiFi.RSSI(i);
        const char* bars = (rssi >= -55) ? "||||" : (rssi >= -65) ? "|||" : (rssi >= -75) ? "||" : "|";
        bool locked = (WiFi.encryptionType(i) != WIFI_AUTH_OPEN);

        st += "<tr><td><a href='/page2?pick=" + UrlEncode(ssid);
        st += "&ch=" + String(WiFi.channel(i)) + "'>";
        st += HtmlEscape(ssid);
        st += "</a></td>";
        st += "<td class='sig'>";
        if (locked) st += "&#128274; ";		// padlock
        st += String(bars) + " " + String(rssi) + " dBm ch" + String(WiFi.channel(i));
        st += "</td></tr>";
    }

    st += "</table>";
    st += "<p class='hint'>Tap a network to fill in its name, then enter the password above.</p>";
    return st;
}

String GetPage2()
{
    // A picked network arrives in the query string rather than being staged on
    // the module, so two browser tabs cannot overwrite each other's choice.
    String pickSSID = server.hasArg("pick") ? server.arg("pick") : "";
    String pickCh   = server.hasArg("ch")   ? server.arg("ch")   : "";
    String ssidValue = pickSSID.length() ? pickSSID : String(MDLnetwork.SSID);

    String st = "<HTML>";
    st += "  <head>";
    st += "    <META content='text/html; charset=utf-8' http-equiv=Content-Type>";
    st += "    <meta name=vs_targetSchema content='HTML 4.0'>";
    st += "    <meta name='viewport' content='width=device-width, initial-scale=1.0'>";
    st += "    <title>Rate Control</title>";
    st += "    <style>";
    st += "      html { font-family: Helvetica, Arial, sans-serif; display:inline-block; margin:0 auto; text-align:center; }";
    st += "      body { margin-top:50px; background-color:wheat; font-family:Arial, Helvetica, Sans-Serif; }";
    // underline main headings
    st += "      h1 { color:#444444; margin:50px auto 12px; text-decoration: underline; }";
    st += "      h1.subhead { margin:20px auto 12px; }";
    st += "";
    // Proportional columns, not the 200px + 320px they used to be. That came to
    // 568px with the padding, against a 360-412px phone viewport — a browser you
    // can pinch-zoom hides it, a captive-portal mini browser cannot, and the
    // right-hand column was simply clipped. The max-width keeps the old
    // appearance on anything wide enough to have shown it correctly before.
    st += "      table.center { margin-left:auto; margin-right:auto; border-collapse:collapse; table-layout:fixed; width:100%; max-width:568px; }";
    st += "      td.label-col { width:40%; text-align:left; padding:8px 12px; vertical-align:middle; }";
    st += "      td.input-col { width:60%; padding:8px 12px; vertical-align:middle; }";
    st += "";
    st += "      .control-width { width:320px; max-width:90%; margin:0 auto; box-sizing:border-box; }";
    st += "      .InputCell { display:block; width:100%; height:36px; box-sizing:border-box; text-align:center; font-size:18px; font-weight:700; padding:4px 6px; }";
    st += "";
    st += "      /* REVERTED: original purple gradient + shadows from your earlier CSS for button */";
    st += "      .button-72 {";
    st += "        align-items: center;";
    st += "        background-color: initial;";
    st += "        background-image: linear-gradient(rgba(179, 132, 201, .84), rgba(57, 31, 91, .84) 50%);";
    st += "        border-radius: 42px;";
    st += "        border-width: 0;";
    st += "        box-shadow: rgba(57, 31, 91, 0.24) 0 2px 2px, rgba(179, 132, 201, 0.4) 0 8px 12px;";
    st += "        color: #FFFFFF;";
    st += "        cursor: pointer;";
    st += "        display: inline-flex;";
    st += "        font-family: Quicksand, sans-serif;";
    st += "        font-size: 18px;";
    st += "        font-weight: 700;";
    st += "        justify-content: center;";
    st += "        letter-spacing: .04em;";
    st += "        line-height: 16px;";
    st += "        margin: 12px auto;";
    st += "        padding: 12px 18px;";
    st += "        text-align: center;";
    st += "        text-decoration: none;";
    st += "        text-shadow: rgba(255, 255, 255, 0.4) 0 0 4px, rgba(255, 255, 255, 0.2) 0 0 12px, rgba(57, 31, 91, 0.6) 1px 1px 4px, rgba(57, 31, 91, 0.32) 4px 4px 16px;";
    st += "        user-select: none;";
    st += "        -webkit-user-select: none;";
    st += "        touch-action: manipulation;";
    st += "        vertical-align: baseline;";
    st += "        width:320px; /* match control area */";
    st += "        max-width:90%;";
    st += "      }";
    st += "      #submitBtn { margin-top: 36px; }";
    st += "";
    st += "      a:link { font-size:150%; }";
    st += "";
    st += "      /* Left-aligned checkbox inside control area */";
    st += "      .checkbox-row { display:flex; align-items:center; height:44px; box-sizing:border-box; }";
    st += "      .checkbox-left { display:flex; align-items:center; justify-content:flex-start; }";
    st += "";
    st += "      /* Custom checkbox to match button: gradient background, shadow, rounded corners */";
    st += "      input[type=checkbox].styled {";
    st += "        -webkit-appearance: none;";
    st += "        appearance: none;";
    st += "        width:44px; height:44px; display:inline-block; position:relative; margin:0; padding:0; box-sizing:border-box;";
    st += "        border-radius:10px;";
    st += "        background-image: linear-gradient(rgba(179,132,201,.84), rgba(57,31,91,.84) 50%);";
    st += "        box-shadow: rgba(57,31,91,0.24) 0 2px 2px, rgba(179,132,201,0.4) 0 8px 12px;";
    st += "        cursor:pointer;";
    st += "        vertical-align:middle;";
    st += "        outline: none;";
    st += "        border: 1px solid rgba(57,31,91,0.25);";
    st += "      }";
    st += "";
    st += "      /* checkmark using an :after pseudo-element (correct orientation) */";
    st += "      input[type=checkbox].styled::after {";
    st += "        content: '';";
    st += "        position: absolute;";
    st += "        left: 50%; top: 50%;";
    st += "        width: 12px; height: 22px;";            // size of the mark stem+arm
    st += "        border-right: 4px solid white;";
    st += "        border-bottom: 4px solid white;";
    st += "        transform: translate(-50%,-60%) rotate(45deg) scale(0);";
    st += "        transform-origin: center;";
    st += "        transition: transform 0.12s ease-in-out;";
    st += "        border-radius:2px;";
    st += "      }";
    st += "      input[type=checkbox].styled:checked::after { transform: translate(-50%,-60%) rotate(45deg) scale(1); }";
    st += "";
    st += "      /* keep keyboard focus visible */";
    st += "      input[type=checkbox].styled:focus { box-shadow: 0 0 0 3px rgba(179,132,201,0.22); }";
    st += "";
    st += "      .label-normal { font-weight:normal; }";
    st += "      .hint { font-size: 12px; color: #333; margin-top: 4px; }";
    st += "      .status { margin: 2px auto 16px; font-size: 16px; }";
    // Network scan results — rows are links, sized for a gloved finger.
    st += "      table.nets { margin:0 auto; border-collapse:collapse; width:320px; max-width:90%; }";
    st += "      table.nets td { padding:10px 8px; border-bottom:1px solid rgba(57,31,91,0.2); text-align:left; font-size:16px; }";
    st += "      table.nets td.sig { text-align:right; white-space:nowrap; color:#333; font-size:14px; }";
    st += "      table.nets a { font-size:100%; font-weight:700; text-decoration:none; color:#391f5b; }";
    st += "    </style>";
    st += "  </head>";
    st += "";
    st += "  <BODY>";
    st += "    <h1 align=center>Wifi Network </h1>";
    st += "    <form id=FORM1 method=post action='/'>&nbsp;";
    st += "      <table class='center'>";
    st += "        <tr>";
    st += "          <td class='label-col'><span class='label-normal'>Network</span></td>";
    st += "          <td class='input-col'><div class='control-width'><input class='InputCell' id='ssid' size='20' name='prop1' value='" + HtmlEscape(ssidValue) + "'></div></td>";
    st += "        </tr>";
    st += "        <tr>";
    st += "          <td class='label-col'><span class='label-normal'>Password</span></td>";
    st += "          <td class='input-col'><div class='control-width'><input class='InputCell' id='pass' size='20' name='prop2' value='" + HtmlEscape(String(MDLnetwork.Password)) + "'></div></td>";
    st += "        </tr>";
    st += "        <tr>";
    st += "          <td class='label-col'><span class='label-normal'>Use this Network</span></td>";
    st += "          <td class='input-col'>";
    st += "            <div class='control-width'>";
    st += "              <div class='checkbox-row'>";
    st += "                <div class='checkbox-left'>";
    st += "                  <label style='display:inline-flex; align-items:center; gap:8px; cursor:pointer;'>";
    st += "                    <input class='styled' type='checkbox' name='connect' value='1' " + String(MDLnetwork.WifiModeUseStation ? "checked" : "") + ">";
    st += "                  </label>";
    st += "                </div>";
    st += "              </div>";
    st += "            </div>";
    st += "          </td>";
    st += "        </tr>";
    // WiFi status row centered across page
    st += "        <tr><td colspan='2' style='text-align:center;'>";
    if (WiFi.isConnected())
    {
        st += "<div class='status'>Wifi Connected to " + HtmlEscape(String(MDLnetwork.SSID));
        st += " (" + WiFi.localIP().toString() + ") on channel " + String(WiFi.channel()) + "</div>";
    }
    else if (MDLnetwork.WifiModeUseStation && StaAuthRejected())
    {
        // Name the actual fault. "Not connected" otherwise covers a wrong
        // password, an absent router and a dead aerial alike, and only one of
        // those is fixed on this page.
        st += "<div class='status'>Password refused by " + HtmlEscape(String(MDLnetwork.SSID)) + ".</div>";
        st += "<div class='hint'>Check the password above, then Save. Still trying every 10 minutes.</div>";
    }
    else if (MDLnetwork.WifiModeUseStation)
    {
        st += "<div class='status'>Wifi Not Connected — retrying in the background.</div>";
    }
    else
    {
        st += "<div class='status'>Hotspot only. Tick Use this Network to join a network.</div>";
    }
    if (MDLnetwork.WifiModeUseStation && !WiFi.isConnected())
    {
        // A retry while a network scan runs is the expensive kind, so it is
        // rationed everywhere except here — see Wifi.ino. Expect the hotspot to
        // stall for a second or two after pressing it.
        st += "<div class='hint'><a href='/page2?retry=1'>Retry Connection Now</a></div>";
    }
    st += "        </td></tr>";
    st += "        <tr><td colspan='2'><hr></td></tr>";
    // New Hotspot heading row (renamed and underlined via h1 style) with zero td padding to match spacing
    st += "        <tr><td colspan='2' style='text-align:center; padding:0;'><h1 class='subhead'>Hotspot</h1></td></tr>";
    st += "        <tr>";
    st += "          <td class='label-col'><span class='label-normal'>Password</span></td>";
    st += "          <td class='input-col'><div class='control-width'><input class='InputCell' id='ap_pass' size='20' name='prop3' value='" + HtmlEscape(String(MDL.APpassword)) + "'></div></td>";
    st += "        </tr>";
    st += "        <tr>";
    st += "          <td colspan='2'><div class='control-width'><div class='hint'>Module Access Point. Use 8 to 10 characters. Leave empty for an open hotspot.</div></div></td>";
    st += "        </tr>";
    st += "      </table>";
    st += "";
    // Carried through the save so the channel a tapped network was found on can
    // prime the cache — see handleCredentials(). The name goes with it because
    // the channel is only meaningful for the network it came from.
    st += "      <input type='hidden' name='pickssid' value='" + HtmlEscape(pickSSID) + "'>";
    st += "      <input type='hidden' name='pickch' value='" + HtmlEscape(pickCh) + "'>";
    st += "";
    st += "      <p><div class='control-width'><input class='button-72' id='submitBtn' type='submit' value='Save/Restart'></div></p>";
    st += "    </form>";
    st += "";
    // Outside the form, so they cannot trigger the save-and-restart path. They
    // do still navigate, and the page that comes back is rebuilt from
    // MDLnetwork — so anything typed and not yet saved is gone either way. The
    // hint under the results states the order that works: tap a network first,
    // enter the password after.
    st += "    <p><a href='/page2?scan=1'>Scan for Networks</a></p>";
    st += ScanResultsHtml();
    st += "    <p> <a href='/page0'>Back</a> </p>";
    st += "  </BODY>";
    st += "</HTML>";

    return st;
}
