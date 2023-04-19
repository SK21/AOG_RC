
byte DataHost[MaxHostBuffer];
uint16_t PGNhost;
char DataIn;
byte CountHost;
String Field;

void DoHost()
{
    myusb.Task();
    // Print out information about different devices.

    for (uint8_t i = 0; i < CNT_DEVICES; i++) 
    {
        if (*drivers[i] != driver_active[i])
        {
            if (driver_active[i]) 
            {
                Serial.printf("*** Device %s - disconnected ***\n", driver_names[i]);
                driver_active[i] = false;
            }
            else 
            {
                Serial.printf("*** Device %s %x:%x - connected ***\n", driver_names[i], drivers[i]->idVendor(), drivers[i]->idProduct());
                driver_active[i] = true;

                const uint8_t* psz = drivers[i]->manufacturer();
                if (psz && *psz) Serial.printf("  manufacturer: %s\n", psz);
                psz = drivers[i]->product();
                if (psz && *psz) Serial.printf("  product: %s\n", psz);
                psz = drivers[i]->serialNumber();
                if (psz && *psz) Serial.printf("  Serial: %s\n", psz);

                // If this is a new Serial device.
                if (drivers[i] == &userial)
                {
                    // Lets try first outputting something to our USerial to see if it will go out...
                    userial.begin(baud);

                }
            }
        }
    }

    if (userial.available())
    {
        DataIn = userial.read();
        switch (DataIn)
        {
        case '\n':
            if (CountHost < MaxHostBuffer) DataHost[CountHost] = Field.toInt();

            PGNhost = DataHost[1] << 8 | DataHost[0];
            if (PGNhost == 32618)
            {
                if (GoodCRC(DataHost, 6)) SendUSBhost(6);
            }

            Field = "";
            CountHost = 0;
            break;

        case ',':
            DataHost[CountHost] = Field.toInt();
            Field = "";
            CountHost++;
            if (CountHost >= MaxHostBuffer) CountHost = 0;
            break;

        default:
            Field += DataIn;
            if (Field.length() > 10) Field = "";
            break;
        }
    }
}

void SendUSBhost(byte Length)
{
    if (Ethernet.linkStatus() == LinkON)
    {
        UDPswitches.beginPacket(DestinationIP, DestinationPortSwitches);
        UDPswitches.write(DataHost, Length);
        UDPswitches.endPacket();
    }
}
