
// uses SPI.h included with EtherCard.h
// use 'if(ShieldFound())' before 'ether.begin(sizeof Ethernet::buffer, LocalMac, selectPin);' to prevent hang

#define ADDR_MASK        0x1F
#define ENC28J60_SOFT_RESET          0xFF
#define ENC28J60_READ_CTRL_REG       0x00
#define ESTAT            0x1D
#define ESTAT_CLKRDY     0x01

bool ShieldFound()
{
    SPI.begin();
    delay(1000);
    writeOp(ENC28J60_SOFT_RESET, 0, ENC28J60_SOFT_RESET);
    delay(2); // errata B7/2

    return readOp(ENC28J60_READ_CTRL_REG, ESTAT) & ESTAT_CLKRDY;
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

