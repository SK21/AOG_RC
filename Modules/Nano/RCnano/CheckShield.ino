
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



