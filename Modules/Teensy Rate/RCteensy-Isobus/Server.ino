
// HTTP server (port 80) for OTA firmware update + minimal mDNS responder.
// Network init: DHCP first, link-local 169.254.1.(ID+1) on failure.
// Hostname: rcteensy-[ID].local
//
// Call Server_Begin() from DoSetup() (replaces old Ethernet init block).
// Call Server_Update() from loop().

static EthernetServer httpServer(80);
static EthernetUDP    mdnsUDP;
static char           mdnsHostname[24];  // "rcteensy-N"
static IPAddress      localIP;

// ─── Network init ─────────────────────────────────────────────────────────────

void Server_Begin()
{
    snprintf(mdnsHostname, sizeof(mdnsHostname), "rcteensy-%u", (unsigned)MDL.ID);

    // Locally-administered MAC derived from module ID (safe for LAN use)
    uint8_t mac[6] = { 0x02, 0x52, 0x43, 0x54, 0x4E, (uint8_t)(MDL.ID + 1) };

    Serial.printf("[NET] MAC %02X:%02X:%02X:%02X:%02X:%02X\n",
        mac[0], mac[1], mac[2], mac[3], mac[4], mac[5]);
    Serial.println("[NET] requesting DHCP...");

    if (Ethernet.begin(mac, 6000) == 0)
    {
        // Link-local fallback: 169.254.1.(ID+1) / 255.255.0.0
        IPAddress ll(169, 254, 1, (uint8_t)(MDL.ID + 1));
        IPAddress mask(255, 255, 0, 0);
        IPAddress gw(169, 254, 0, 1);
        Ethernet.begin(mac, ll, gw, gw, mask);
        Serial.print("[NET] link-local: ");
    }
    else
    {
        Serial.print("[NET] DHCP: ");
    }

    localIP = Ethernet.localIP();
    Serial.println(localIP);

    httpServer.begin();
    Serial.printf("[HTTP] http://%s.local/  (%u.%u.%u.%u)\n", mdnsHostname,
        (unsigned)localIP[0], (unsigned)localIP[1],
        (unsigned)localIP[2], (unsigned)localIP[3]);

    // Join mDNS multicast group 224.0.0.251 on port 5353
    mdnsUDP.beginMulticast(IPAddress(224, 0, 0, 251), 5353);
    Serial.printf("[mDNS] advertising %s.local\n", mdnsHostname);
}

// ─── mDNS ─────────────────────────────────────────────────────────────────────

static void MDNS_SendResponse()
{
    uint8_t resp[80];
    int pos = 0;

    // DNS header: response, authoritative, 1 answer
    resp[pos++] = 0x00; resp[pos++] = 0x00;  // Transaction ID = 0
    resp[pos++] = 0x84; resp[pos++] = 0x00;  // QR=1, AA=1
    resp[pos++] = 0x00; resp[pos++] = 0x00;  // QDCOUNT = 0
    resp[pos++] = 0x00; resp[pos++] = 0x01;  // ANCOUNT = 1
    resp[pos++] = 0x00; resp[pos++] = 0x00;  // NSCOUNT = 0
    resp[pos++] = 0x00; resp[pos++] = 0x00;  // ARCOUNT = 0

    // Answer name: <hostname> label + "local" label + root
    uint8_t hlen = (uint8_t)strlen(mdnsHostname);
    resp[pos++] = hlen;
    memcpy(resp + pos, mdnsHostname, hlen);  pos += hlen;
    resp[pos++] = 5;
    memcpy(resp + pos, "local", 5);          pos += 5;
    resp[pos++] = 0;  // root

    resp[pos++] = 0x00; resp[pos++] = 0x01;  // Type A
    resp[pos++] = 0x80; resp[pos++] = 0x01;  // Class IN + cache-flush
    // TTL = 120 s
    resp[pos++] = 0x00; resp[pos++] = 0x00; resp[pos++] = 0x00; resp[pos++] = 0x78;
    // RDLENGTH = 4
    resp[pos++] = 0x00; resp[pos++] = 0x04;
    // RDATA: IPv4
    resp[pos++] = localIP[0]; resp[pos++] = localIP[1];
    resp[pos++] = localIP[2]; resp[pos++] = localIP[3];

    mdnsUDP.beginPacket(IPAddress(224, 0, 0, 251), 5353);
    mdnsUDP.write(resp, pos);
    mdnsUDP.endPacket();
}

static void MDNS_Update()
{
    int pktLen = mdnsUDP.parsePacket();
    if (pktLen < 12) return;

    uint8_t buf[256];
    int len = mdnsUDP.read(buf, sizeof(buf));
    if (len < 12) return;
    if (buf[2] & 0x80) return;  // ignore responses (QR bit set)

    uint16_t qdCount = ((uint16_t)buf[4] << 8) | buf[5];
    if (qdCount == 0) return;

    // Decode first question name starting at byte 12
    int  pos  = 12;
    char qname[64];
    int  qlen = 0;

    while (pos < len && buf[pos] != 0)
    {
        if (buf[pos] & 0xC0) { pos += 2; break; }  // DNS pointer — skip
        uint8_t labelLen = buf[pos++];
        if (qlen > 0 && qlen < (int)sizeof(qname) - 1) qname[qlen++] = '.';
        for (int i = 0; i < labelLen && pos < len && qlen < (int)sizeof(qname) - 1; i++)
            qname[qlen++] = (char)tolower(buf[pos++]);
    }
    qname[qlen] = '\0';
    pos++;  // skip root 0

    char expected[32];
    snprintf(expected, sizeof(expected), "%s.local", mdnsHostname);
    if (strcasecmp(qname, expected) != 0) return;

    if (pos + 3 >= len) return;
    uint16_t qtype = ((uint16_t)buf[pos] << 8) | buf[pos + 1];
    if (qtype != 1 && qtype != 255) return;  // only A or ANY

    MDNS_SendResponse();
}

// ─── HTTP helpers ─────────────────────────────────────────────────────────────

static bool HTTP_ReadLine(EthernetClient& client, char* buf, int maxLen,
                          uint32_t timeoutMs = 4000)
{
    int      pos   = 0;
    uint32_t start = millis();
    while (millis() - start < timeoutMs)
    {
        if (client.available())
        {
            char c = (char)client.read();
            if (c == '\n')
            {
                if (pos > 0 && buf[pos - 1] == '\r') pos--;
                buf[pos] = '\0';
                return true;
            }
            if (pos < maxLen - 1) buf[pos++] = c;
        }
        else if (!client.connected()) break;
    }
    buf[pos] = '\0';
    return false;
}

static void HTTP_Send(EthernetClient& client, const char* status,
                      const char* contentType, const char* body)
{
    client.print("HTTP/1.1 ");
    client.print(status);
    client.print("\r\nContent-Type: ");
    client.print(contentType);
    client.print("\r\nConnection: close\r\n\r\n");
    if (body) client.print(body);
}

// ─── HTML pages ───────────────────────────────────────────────────────────────

FLASHMEM static void HTTP_ServeStatus(EthernetClient& client)
{
    char body[600];
    snprintf(body, sizeof(body),
        "<!DOCTYPE html><html><head>"
        "<meta name='viewport' content='width=device-width,initial-scale=1'>"
        "<style>body{font-family:sans-serif;text-align:center;background:#f0f0f0;padding:30px}"
        ".card{background:#fff;border-radius:10px;padding:24px;display:inline-block;min-width:300px}"
        "h1{color:#333;margin-top:0}p{margin:6px 0}"
        "a{display:inline-block;margin-top:18px;font-size:1.1em;color:#1565C0}</style></head>"
        "<body><div class='card'>"
        "<h1>%s</h1>"
        "<p>Module ID: %u</p>"
        "<p>Firmware: %u</p>"
        "<p>Sensors: %u</p>"
        "<p>IP: %u.%u.%u.%u</p>"
        "<a href='/update'>&#x1F4E6; Firmware Update</a>"
        "</div></body></html>",
        InoDescription, (unsigned)MDL.ID, (unsigned)InoID, (unsigned)MDL.SensorCount,
        (unsigned)localIP[0], (unsigned)localIP[1],
        (unsigned)localIP[2], (unsigned)localIP[3]);

    HTTP_Send(client, "200 OK", "text/html", body);
}

FLASHMEM static void HTTP_ServeUpdateForm(EthernetClient& client)
{
    HTTP_Send(client, "200 OK", "text/html",
        "<!DOCTYPE html><html><head>"
        "<meta name='viewport' content='width=device-width,initial-scale=1'>"
        "<style>"
        "body{font-family:sans-serif;text-align:center;background:#f0f0f0;padding:30px}"
        ".card{background:#fff;border-radius:10px;padding:30px;display:inline-block;min-width:320px}"
        "h1{color:#444;margin-top:0}"
        ".filebox{width:100%;font-size:1.05em;padding:10px;margin-bottom:18px;box-sizing:border-box}"
        ".btn{background:linear-gradient(#7e57c2,#4527a0);color:#fff;border:none;"
        "border-radius:22px;padding:13px 40px;font-size:1.05em;cursor:pointer;width:80%}"
        ".prog-bg{width:100%;background:#e0e0e0;border-radius:8px;height:26px;margin-top:16px}"
        ".prog{width:0%;background:#2196F3;height:26px;border-radius:8px;"
        "color:#fff;line-height:26px;font-size:.9em;transition:width .2s}"
        "a{display:block;margin-top:16px;color:#1565C0}</style></head>"
        "<body><div class='card'>"
        "<h1>Firmware Update</h1>"
        "<form method='POST' action='/update' enctype='multipart/form-data' id='f'>"
        "<input class='filebox' type='file' name='update' accept='.hex'>"
        "<button class='btn' type='submit'>Upload &amp; Flash</button>"
        "</form>"
        "<div class='prog-bg'><div id='p' class='prog'>0%</div></div>"
        "<a href='/'>&#x2190; Back</a>"
        "</div>"
        "<script>"
        "var p=document.getElementById('p');"
        "document.getElementById('f').addEventListener('submit',function(e){"
        "e.preventDefault();"
        "var r=new XMLHttpRequest();"
        "r.open('POST','/update');"
        "r.upload.addEventListener('progress',function(v){"
        "if(v.lengthComputable){"
        "var w=Math.round(v.loaded/v.total*100)+'%';"
        "p.textContent=w;p.style.width=w;"
        "if(w==='100%')p.style.background='#388E3C';}});"
        "r.onload=function(){document.body.innerHTML=r.responseText;};"
        "r.send(new FormData(this));});"
        "</script></body></html>");
}

// ─── POST /update — multipart hex upload ─────────────────────────────────────

FLASHMEM static void HTTP_HandleUpload(EthernetClient& client,
                                       const char* contentType)
{
    // Extract boundary from Content-Type header value
    char boundary[80] = "";
    const char* bptr = strstr(contentType, "boundary=");
    if (bptr)
    {
        // Strip optional quotes
        const char* bval = bptr + 9;
        if (*bval == '"') bval++;
        snprintf(boundary, sizeof(boundary), "--%s", bval);
        // Remove trailing quote if present
        int blen = strlen(boundary);
        if (boundary[blen - 1] == '"') boundary[blen - 1] = '\0';
    }

    if (boundary[0] == '\0')
    {
        HTTP_Send(client, "400 Bad Request", "text/plain", "Missing boundary");
        return;
    }

    if (!FirmwareUpdate_Begin())
    {
        HTTP_Send(client, "500 Internal Server Error", "text/plain",
                  "Flash buffer init failed");
        return;
    }

    // Multipart state machine
    enum { FIND_BOUNDARY, SKIP_HEADERS, READ_DATA, DONE } state = FIND_BOUNDARY;
    char line[128];
    bool error = false;

    while (state != DONE && HTTP_ReadLine(client, line, sizeof(line), 60000))
    {
        switch (state)
        {
        case FIND_BOUNDARY:
            if (strcmp(line, boundary) == 0) state = SKIP_HEADERS;
            break;

        case SKIP_HEADERS:
            if (line[0] == '\0') state = READ_DATA;  // blank line ends part headers
            break;

        case READ_DATA:
            if (line[0] == '-' && line[1] == '-')
            {
                state = DONE;
            }
            else if (line[0] == ':')
            {
                if (!FirmwareUpdate_ProcessLine(line))
                {
                    error = true;
                    state = DONE;
                }
            }
            // else: blank lines or non-hex content — skip
            break;

        case DONE:
            break;
        }
    }

    if (error || !FirmwareUpdate_IsComplete())
    {
        FirmwareUpdate_Abort();
        HTTP_Send(client, "400 Bad Request", "text/plain",
                  "Update failed — invalid or incomplete hex file");
        return;
    }

    HTTP_Send(client, "200 OK", "text/html",
        "<!DOCTYPE html><html><body style='font-family:sans-serif;text-align:center;padding:40px'>"
        "<h2 style='color:#2E7D32'>&#x2705; Update complete</h2>"
        "<p>Rebooting — reconnect in a few seconds.</p>"
        "<p><a href='/'>Refresh</a></p>"
        "</body></html>");

    client.flush();
    delay(300);

    FirmwareUpdate_Flash();  // does not return
}

// ─── Main entry points ────────────────────────────────────────────────────────

void Server_Update()
{
    MDNS_Update();

    EthernetClient client = httpServer.accept();
    if (!client) return;

    char line[128];
    if (!HTTP_ReadLine(client, line, sizeof(line))) { client.stop(); return; }

    // Parse request line: METHOD PATH HTTP/x.x
    char method[8] = "";
    char path[64]  = "";
    sscanf(line, "%7s %63s", method, path);

    // Read and parse remaining request headers
    int32_t contentLength = 0;
    char    contentType[128] = "";
    char    hdr[128];

    while (HTTP_ReadLine(client, hdr, sizeof(hdr)))
    {
        if (hdr[0] == '\0') break;  // blank line = end of headers, body follows
        if (strncasecmp(hdr, "Content-Length:", 15) == 0)
        {
            const char* v = hdr + 15;
            while (*v == ' ') v++;
            contentLength = atol(v);
        }
        else if (strncasecmp(hdr, "Content-Type:", 13) == 0)
        {
            const char* v = hdr + 13;
            while (*v == ' ') v++;
            snprintf(contentType, sizeof(contentType), "%s", v);
        }
    }

    // Route
    if (strcmp(method, "GET") == 0)
    {
        if (strcmp(path, "/") == 0 || strcmp(path, "/index.html") == 0)
            HTTP_ServeStatus(client);
        else if (strcmp(path, "/update") == 0)
            HTTP_ServeUpdateForm(client);
        else
            HTTP_Send(client, "404 Not Found", "text/plain", "Not found");
    }
    else if (strcmp(method, "POST") == 0 && strcmp(path, "/update") == 0)
    {
        HTTP_HandleUpload(client, contentType);
    }
    else
    {
        HTTP_Send(client, "405 Method Not Allowed", "text/plain", "Method not allowed");
    }

    client.flush();
    delay(10);
    client.stop();
}
