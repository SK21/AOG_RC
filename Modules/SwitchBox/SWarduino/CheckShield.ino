
// uses SPI.h 
// use 'if(ShieldFound())' before 'ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);' to prevent hang

#define ADDR_MASK        0x1F
#define ENC28J60_SOFT_RESET          0xFF
#define ENC28J60_READ_CTRL_REG       0x00
#define ESTAT            0x1D
#define ESTAT_CLKRDY     0x01

bool Fnd;
uint8_t Ecount;

bool ShieldFound()
{
    SPI.begin();

    Ecount = 0;
    Fnd = false;
    while (!Fnd)
    {
        delay(500);
        Serial.print(".");

        writeOp(ENC28J60_SOFT_RESET, 0, ENC28J60_SOFT_RESET);
        delay(2); // errata B7/2
        Fnd = readOp(ENC28J60_READ_CTRL_REG, ESTAT) & ESTAT_CLKRDY;

        Ecount++;
        if (Ecount > 5) break;
    }
    if (Fnd) EncPhyReset();
    return Fnd;
}

static void writeOp(byte op, byte address, byte data) {
    enableChip();
    xferSPI(op | (address & ADDR_MASK));
    xferSPI(data);
    disableChip();
}

static void enableChip() {
    cli();
    digitalWrite(selectPin, LOW);
}

static void xferSPI(byte data) {
    SPDR = data;
    while (!(SPSR & (1 << SPIF)))
        ;
}

static void disableChip() {
    digitalWrite(selectPin, HIGH);
    sei();
}

static byte readOp(byte op, byte address) {
    enableChip();
    xferSPI(op | (address & ADDR_MASK));
    xferSPI(0x00);
    if (address & 0x80)
        xferSPI(0x00);
    byte result = SPDR;
    disableChip();
    return result;
}

#define ENC_BIT_FIELD_SET   0x80
#define ENC_BIT_FIELD_CLR   0xA0
#define ENC_WRITE_CTRL_REG  0x40
#define ENC_EIR             0x1C    // every bank
#define ENC_EIR_RXERIF      0x01    // receive buffer overflowed
#define ENC_ECON1           0x1F    // every bank
#define ENC_RXEN            0x04    // ECON1 receive enable
#define ENC_MICMD           0x12    // bank 2, MII
#define ENC_MIREGADR        0x14    // bank 2, MII
#define ENC_MIWRL           0x16    // bank 2, MII
#define ENC_MIWRH           0x17    // bank 2, MII; writing it starts the PHY write
#define ENC_MIRDH           0x19    // bank 2, MII
#define ENC_MISTAT          0x0A    // bank 3, MII
#define ENC_MII             0x80    // readOp: MAC/MII registers need a dummy byte
#define ENC_PHCON1          0x00    // PHY register
#define ENC_PHCON1_PRST_H   0x80    // PHCON1 bit 15 (PRST), high byte

// Receive on/off. ether.begin() switches receiving on at once, but nothing is read until loop()
// runs, so on a busy network (AgOpenWeb) the ENC28J60 buffer overflows during setup, which can
// leave it unable to receive. So receiving is off after ether.begin() and on at the end of setup.
// ECON1 and EIR are in every bank, so EtherCard's bank selection is not disturbed.
void SetReceive(bool On)
{
    if (On)
    {
        writeOp(ENC_BIT_FIELD_CLR, ENC_EIR, ENC_EIR_RXERIF);    // forget an overflow from before
        writeOp(ENC_BIT_FIELD_SET, ENC_ECON1, ENC_RXEN);
    }
    else
    {
        writeOp(ENC_BIT_FIELD_CLR, ENC_ECON1, ENC_RXEN);
    }
}

static void EncBank(byte bank)
{
    writeOp(ENC_BIT_FIELD_CLR, ENC_ECON1, 0x03);
    writeOp(ENC_BIT_FIELD_SET, ENC_ECON1, bank);
}

static void EncMiiWait()
{
    EncBank(3);
    uint8_t n = 0;
    while ((readOp(ENC28J60_READ_CTRL_REG, ENC_MISTAT | ENC_MII) & 0x01) && (n++ < 100)) delayMicroseconds(10);    // MISTAT.BUSY
}

// Reset the ENC28J60's PHY. The chip's soft reset (in ShieldFound and ether.begin) leaves the
// PHY as it was; a Nano rate module stuck unable to receive under AgOpenWeb's load was not
// cleared by soft resets, only by a power-cycle or the reset line. The PHY reset is a guess at
// that (not confirmed on the bench). Runs before ether.begin(), which then sets the PHY up, and
// on every boot, including the restart after a config or subnet change (resetFunc). Leaves bank
// 0 selected, as after a soft reset, which is what EtherCard expects.
static void EncPhyReset()
{
    EncBank(2);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIREGADR, ENC_PHCON1);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIWRL, 0x00);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIWRH, ENC_PHCON1_PRST_H);
    EncMiiWait();

    // PRST clears itself when the reset is done
    bool Resetting = true;
    uint8_t n = 0;
    while (Resetting && (n++ < 50))
    {
        delay(1);
        EncBank(2);
        writeOp(ENC_WRITE_CTRL_REG, ENC_MIREGADR, ENC_PHCON1);
        writeOp(ENC_WRITE_CTRL_REG, ENC_MICMD, 0x01);     // MIIRD
        delayMicroseconds(15);
        EncMiiWait();
        EncBank(2);
        writeOp(ENC_WRITE_CTRL_REG, ENC_MICMD, 0x00);
        Resetting = readOp(ENC28J60_READ_CTRL_REG, ENC_MIRDH | ENC_MII) & ENC_PHCON1_PRST_H;
    }
    EncBank(0);
}


