// CAN Loopback Test v3 - Simplest possible
#include <FlexCAN_T4.h>

FlexCAN_T4<CAN1, RX_SIZE_256, TX_SIZE_16> Can1;

void setup() {
    Serial.begin(115200);
    while (!Serial && millis() < 3000);

    Serial.println("=== CAN Loopback Test v3 ===");

    Can1.begin();
    Can1.setBaudRate(250000);
    Can1.enableLoopBack(true);
    Can1.setMaxMB(16);

    Serial.println("Sending and polling...");
}

uint32_t tx = 0, rx = 0;

void loop() {
    static uint32_t lastSend = 0;

    if (millis() - lastSend >= 500) {
        lastSend = millis();

        CAN_message_t msg;
        msg.id = 0x100;
        msg.flags.extended = 0;  // Standard ID
        msg.len = 4;
        msg.buf[0] = tx;
        msg.buf[1] = 0xAA;
        msg.buf[2] = 0xBB;
        msg.buf[3] = 0xCC;

        if (Can1.write(msg)) {
            tx++;
            Serial.print("TX:");
            Serial.print(tx);
        }
    }

    // Poll for RX
    CAN_message_t rxMsg;
    if (Can1.read(rxMsg)) {
        rx++;
        Serial.print(" RX:");
        Serial.print(rx);
        Serial.print(" id=0x");
        Serial.print(rxMsg.id, HEX);
    }

    static uint32_t lastPrint = 0;
    if (millis() - lastPrint >= 2000) {
        lastPrint = millis();
        Serial.print("  [TX=");
        Serial.print(tx);
        Serial.print(" RX=");
        Serial.print(rx);
        Serial.println("]");
    }
}
