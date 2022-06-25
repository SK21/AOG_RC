#include <PCA95x5.h>

PCA9555 ioex;
uint8_t IOpin;
bool State = HIGH;
uint8_t Relays8[] = { 7,5,3,1,8,10,12,14 };
uint8_t Relays16[] = { 15,14,13,12,11,10,9,8,0,1,2,3,4,5,6,7 };

void setup() {
    Serial.begin(38400);
    delay(2000);

    Wire.begin();
    ioex.attach(Wire);
    ioex.polarity(PCA95x5::Polarity::ORIGINAL_ALL);
    ioex.direction(PCA95x5::Direction::OUT_ALL);
    ioex.write(PCA95x5::Level::H_ALL);

    Serial.println("Finished Setup");
}

void loop()
{
    // 8 relay module
    for (int i = 0; i < 8; i++)
    {
        IOpin = Relays8[i];

        if (State)
        {
            // on
            ioex.write(IOpin, PCA95x5::Level::L);
        }
        else
        {
            // off
            ioex.write(IOpin, PCA95x5::Level::H);
        }
        delay(1000);
    }
    State = !State;
    if (State)
    {
        Serial.println("on");
    }
    else
    {
        Serial.println("off");
    }
}