
// uses SPI.h 
// use 'if(ShieldFound())' before 'ether.begin(sizeof Ethernet::buffer, BoardMAC, selectPin);' to prevent hang

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
#define ENC_EIR             0x1C    // every bank
#define ENC_EIR_RXERIF      0x01    // receive buffer overflowed
#define ENC_ECON1           0x1F    // every bank
#define ENC_RXEN            0x04    // ECON1 receive enable

// Receive on/off. ether.begin() switches receiving on at once, but the Nano reads nothing until
// loop() runs, so on a busy network (AgOpenWeb broadcasts PGNs to port 8888 about 300 times a
// second) the ENC28J60 buffer overflows during setup, which can leave it unable to receive.
// So receiving is switched off after ether.begin() and on when loop() is about to run.
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

#define ENC_WRITE_CTRL_REG  0x40
#define ENC_ERXRDPTL        0x0C    // bank 0, receive read limit: the chip doesn't write past it
#define ENC_ERXRDPTH        0x0D
#define ENC_ERXWRPTL        0x0E    // bank 0, where the next received frame is written
#define ENC_ERXWRPTH        0x0F
#define ENC_ERXFCON         0x18    // bank 1, receive filters
#define ENC_EPKTCNT         0x19    // bank 1, frames waiting
#define ENC_MACON1          0x00    // bank 2, MAC
#define ENC_MICMD           0x12    // bank 2, MII
#define ENC_MIREGADR        0x14    // bank 2, MII
#define ENC_MIWRL           0x16    // bank 2, MII
#define ENC_MIWRH           0x17    // bank 2, MII; writing it starts the PHY write
#define ENC_MIRDL           0x18    // bank 2, MII
#define ENC_MIRDH           0x19    // bank 2, MII
#define ENC_MISTAT          0x0A    // bank 3, MII
#define ENC_MII             0x80    // readOp: MAC/MII registers need a dummy byte
#define ENC_PHCON1          0x00    // PHY registers
#define ENC_PHSTAT1         0x01
#define ENC_PHCON2          0x10
#define ENC_PHSTAT2         0x11
#define ENC_PHLCON          0x14
#define ENC_PHCON1_PRST_H   0x80    // PHCON1 bit 15 (PRST), high byte
#define ENC_PHCON2_HDLDIS   0x0100  // no half-duplex loopback, as EtherCard sets it
#define ENC_PHLCON_ETHERCARD 0x0476 // LED A link, LED B activity, as EtherCard sets it

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

static uint16_t EncPhyRead(byte reg)
{
    EncBank(2);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIREGADR, reg);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MICMD, 0x01);     // MIIRD
    delayMicroseconds(15);
    EncMiiWait();
    EncBank(2);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MICMD, 0x00);
    uint16_t Result = readOp(ENC28J60_READ_CTRL_REG, ENC_MIRDL | ENC_MII);
    Result |= (uint16_t)readOp(ENC28J60_READ_CTRL_REG, ENC_MIRDH | ENC_MII) << 8;
    return Result;
}

static void EncPhyWrite(byte reg, uint16_t Value)
{
    EncBank(2);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIREGADR, reg);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIWRL, Value & 0xFF);
    writeOp(ENC_WRITE_CTRL_REG, ENC_MIWRH, Value >> 8);     // starts the write
    EncMiiWait();
}

// Reset the ENC28J60's PHY. The chip's soft reset (in ShieldFound and ether.begin) leaves the
// PHY as it was. A Nano stuck unable to receive (not even ARP) while still sending was not
// cleared by unplugging the cable, only by a power-cycle or upload. The PHY reset is a test of
// whether the PHY is the cause (not confirmed). At boot it runs before ether.begin(), which then
// sets the PHY up. Leaves bank 0 selected, as after a soft reset, which is what EtherCard expects.
static void EncPhyReset()
{
    EncPhyWrite(ENC_PHCON1, (uint16_t)ENC_PHCON1_PRST_H << 8);

    // PRST clears itself when the reset is done
    uint8_t n = 0;
    do
    {
        delay(1);
    } while ((EncPhyRead(ENC_PHCON1) & 0x8000) && (n++ < 50));
    EncBank(0);
}

// Bank bits of ECON1. EtherCard remembers which bank it selected and only selects again when it
// needs another, so code here that changes the bank while EtherCard is running puts it back.
static byte EncGetBank()
{
    return readOp(ENC28J60_READ_CTRL_REG, ENC_ECON1) & 0x03;
}

// Receive recovery while running: reset the PHY only, then set it up as ether.begin() does
// (the reset returns PHLCON and PHCON2 to defaults; PHCON1's default, half duplex, matches
// EtherCard's MAC setting). No soft reset, so EtherCard's receive position stays right.
void EncPhyRecover()
{
    byte Bank = EncGetBank();
    EncPhyReset();
    EncPhyWrite(ENC_PHLCON, ENC_PHLCON_ETHERCARD);
    EncPhyWrite(ENC_PHCON2, ENC_PHCON2_HDLDIS);
    EncBank(Bank);
}

static void EncPrint(const __FlashStringHelper* Name, uint16_t Value)
{
    Serial.print(Name);
    Serial.print(Value, HEX);
}

static uint16_t EncRead16(byte regL)
{
    uint16_t Result = readOp(ENC28J60_READ_CTRL_REG, regL);
    Result |= (uint16_t)readOp(ENC28J60_READ_CTRL_REG, regL + 1) << 8;
    return Result;
}

// Before a receive recovery, print what the ENC28J60 shows, so a lockup is recorded before the
// PHY reset. WRPT (where the chip writes the next frame) is read twice 100 ms apart: if it
// moves, frames still reach the buffer. RDPT is the limit the chip won't write past, set by
// EtherCard as it releases frames; it doesn't move here, since nothing is read meanwhile.
void EncReport()
{
    byte Bank = EncGetBank();
    EncBank(0);
    uint16_t W1 = EncRead16(ENC_ERXWRPTL);
    delay(100);
    uint16_t W2 = EncRead16(ENC_ERXWRPTL);

    EncPrint(F("ENC ECON1="), readOp(ENC28J60_READ_CTRL_REG, ENC_ECON1));
    EncPrint(F(" ESTAT="), readOp(ENC28J60_READ_CTRL_REG, ESTAT));
    EncPrint(F(" EIR="), readOp(ENC28J60_READ_CTRL_REG, ENC_EIR));
    EncPrint(F(" WRPT="), W1);
    EncPrint(F(">"), W2);
    EncPrint(F(" RDPT="), EncRead16(ENC_ERXRDPTL));
    EncBank(1);
    EncPrint(F(" PKT="), readOp(ENC28J60_READ_CTRL_REG, ENC_EPKTCNT));
    EncPrint(F(" RXFCON="), readOp(ENC28J60_READ_CTRL_REG, ENC_ERXFCON));
    EncBank(2);
    EncPrint(F(" MACON1="), readOp(ENC28J60_READ_CTRL_REG, ENC_MACON1 | ENC_MII));
    EncPrint(F(" PHCON1="), EncPhyRead(ENC_PHCON1));
    EncPrint(F(" PHSTAT1="), EncPhyRead(ENC_PHSTAT1));
    EncPrint(F(" PHSTAT2="), EncPhyRead(ENC_PHSTAT2));
    EncPrint(F(" PHLCON="), EncPhyRead(ENC_PHLCON));
    Serial.println();
    EncBank(Bank);
}



