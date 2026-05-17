
// ASCII Intel hex OTA update using FlasherX
// Requires FXUtil.h and FlashTxx.h (already included in main .ino)
// NOTE: Delete EthernetUpdate.ino before compiling — it defines a conflicting hex_info_t.

static char     fwData[32];   // data bytes for one hex record (max 32 bytes)
static uint32_t fwBase   = 0;
static uint32_t fwMin    = 0xFFFFFFFF;
static uint32_t fwMax    = 0;
static int      fwLines  = 0;
static bool     fwEOF    = false;
static bool     fwActive = false;

// ─── Hex ASCII helpers ────────────────────────────────────────────────────────

static uint8_t FW_Nibble(char c)
{
    if (c >= '0' && c <= '9') return c - '0';
    if (c >= 'A' && c <= 'F') return c - 'A' + 10;
    return c - 'a' + 10;
}

static uint8_t FW_Byte(const char* p)
{
    return (FW_Nibble(p[0]) << 4) | FW_Nibble(p[1]);
}

// ─── Public API ───────────────────────────────────────────────────────────────

bool FirmwareUpdate_Begin()
{
    fwBase   = 0;
    fwMin    = 0xFFFFFFFF;
    fwMax    = 0;
    fwLines  = 0;
    fwEOF    = false;
    fwActive = false;

    if (firmware_buffer_init(&buffer_addr, &buffer_size))
    {
        fwActive = true;
        Serial.printf("[FW] buffer %08lX  %luK %s\n",
            buffer_addr, buffer_size / 1024,
            IN_FLASH(buffer_addr) ? "FLASH" : "RAM");
        return true;
    }
    Serial.println("[FW] buffer init failed");
    return false;
}

// Process one ASCII Intel hex line.
// line must start with ':' (as received from the .hex file).
// Returns true on success, false on checksum or flash error.
bool FirmwareUpdate_ProcessLine(const char* line)
{
    if (!fwActive || !line || line[0] != ':') return false;

    const char* p = line + 1;
    uint8_t  byteCount = FW_Byte(p);  p += 2;
    uint8_t  addrHi    = FW_Byte(p);  p += 2;
    uint8_t  addrLo    = FW_Byte(p);  p += 2;
    uint8_t  recType   = FW_Byte(p);  p += 2;
    uint16_t addr16    = ((uint16_t)addrHi << 8) | addrLo;

    uint8_t sum = byteCount + addrHi + addrLo + recType;
    for (int i = 0; i < byteCount; i++)
    {
        fwData[i] = (char)FW_Byte(p);  p += 2;
        sum += (uint8_t)fwData[i];
    }
    uint8_t checksum = FW_Byte(p);
    if ((uint8_t)(sum + checksum) != 0)
    {
        Serial.printf("[FW] checksum error line %d\n", fwLines);
        return false;
    }

    uint32_t fullAddr = fwBase + addr16;

    switch (recType)
    {
    case 0:  // data record
        {
            uint32_t end = fullAddr + byteCount;
            if (end    > fwMax) fwMax = end;
            if (fullAddr < fwMin) fwMin = fullAddr;

            uint32_t dst = buffer_addr + fullAddr - FLASH_BASE_ADDR;
            if (fwMax > FLASH_BASE_ADDR + buffer_size)
            {
                Serial.printf("[FW] address %08lX exceeds buffer\n", fwMax);
                return false;
            }
            if (IN_FLASH(buffer_addr))
            {
                if (flash_write_block(dst, fwData, byteCount))
                {
                    Serial.println("[FW] flash_write_block error");
                    return false;
                }
            }
            else
            {
                memcpy((void*)dst, fwData, byteCount);
            }
        }
        break;

    case 1:  // EOF record
        fwEOF = true;
        Serial.printf("[FW] EOF — %d lines, %lu bytes\n", fwLines, fwMax - fwMin);
        break;

    case 2:  // extended segment address
        fwBase = ((uint32_t)((uint8_t)fwData[0] << 8 | (uint8_t)fwData[1])) << 4;
        break;

    case 4:  // extended linear address
        fwBase = ((uint32_t)((uint8_t)fwData[0] << 8 | (uint8_t)fwData[1])) << 16;
        break;

    case 5:  // start linear address — ignored for Teensy 4.1
        break;

    default:
        break;
    }

    fwLines++;
    return true;
}

bool FirmwareUpdate_IsComplete()
{
    return fwActive && fwEOF;
}

// Call after IsComplete() returns true. Does not return — reboots into new firmware.
void FirmwareUpdate_Flash()
{
    if (!FirmwareUpdate_IsComplete()) return;
    Serial.println("[FW] flashing — do not power off");
    Serial.flush();
    delay(100);
    flash_move(FLASH_BASE_ADDR, buffer_addr, fwMax - fwMin);
    REBOOT;
}

void FirmwareUpdate_Abort()
{
    if (fwActive)
    {
        firmware_buffer_free(buffer_addr, buffer_size);
        fwActive = false;
        fwEOF    = false;
        fwLines  = 0;
        Serial.println("[FW] aborted");
    }
}
