
void SendData()
{
    //PGN32400, Rate info from module to RC
    //0     HeaderLo    144
    //1     HeaderHi    126
    //2     Mod/Sen ID          0-15/0-15
    //3	    rate applied Lo 	1000 X actual
    //4     rate applied Mid
    //5	    rate applied Hi
    //6	    acc.Quantity Lo		10 X actual
    //7	    acc.Quantity Mid
    //8     acc.Quantity Hi
    //9     PWM Lo
    //10    PWM Hi
    //11    Status
    //      bit 0 - sensor 0 connected
    //      bit 1 - sensor 1 connected
    //      bit 2   - wifi rssi < -80
    //      bit 3	- wifi rssi < -70
    //      bit 4	- wifi rssi < -65
    //      bit 5   wifi connected
    //      bit 6   ethernet connected
    //      bit 7   good pin configuration
    //12    CRC

    byte Data[20];

    for (int i = 0; i < MDL.SensorCount; i++)
    {
        Data[0] = 144;
        Data[1] = 126;
        Data[2] = BuildModSenID(MDL.ID, i);

        // rate applied, 1000 X actual
        uint32_t Applied = Sensor[i].UPM * 1000;
        Data[3] = Applied;
        Data[4] = Applied >> 8;
        Data[5] = Applied >> 16;

        // accumulated quantity, 10 X actual
        if (Sensor[i].MeterCal > 0)
        {
            uint32_t Units = Sensor[i].TotalPulses * 10.0 / Sensor[i].MeterCal;
            Data[6] = Units;
            Data[7] = Units >> 8;
            Data[8] = Units >> 16;
        }
        else
        {
            Data[6] = 0;
            Data[7] = 0;
            Data[8] = 0;
        }

        //PWM
        Data[9] = (int)Sensor[i].PWM;
        Data[10] = (int)Sensor[i].PWM >> 8;

        // status
        Data[11] = 0;
        if (millis() - Sensor[0].CommTime < 5000) Data[11] |= 0b00000001;
        if (millis() - Sensor[1].CommTime < 5000) Data[11] |= 0b00000010;

        // wifi
        if (millis() - ESPtime < 5000)
        {
            // ESP connected
            if (WifiStrength < -80)
            {
                Data[11] |= 0b00000100;
            }
            else if (WifiStrength < -70)
            {
                Data[11] |= 0b00001000;
            }
            else
            {
                Data[11] |= 0b00010000;
            }
            Data[11] |= 0b00100000;
        }

        if (Ethernet.linkStatus() == LinkON) Data[11] |= 0b01000000;
        if (GoodPins) Data[11] |= 0b10000000;

        // crc
        Data[12] = CRC(Data, 12, 0);

        if (Ethernet.linkStatus() == LinkON)
        {
            // send ethernet
            UDPcomm.beginPacket(DestinationIP, DestinationPort);
            UDPcomm.write(Data, 13);
            UDPcomm.endPacket();
        }
        else if (millis() - ESPtime > 5000)
        {
            // send serial
            Serial.print(Data[0]);
            for (int i = 1; i < 13; i++)
            {
                Serial.print(",");
                Serial.print(Data[i]);
            }
            Serial.println("");
        }

        // send wifi
        if (MDL.ESPserialPort != NC) SerialESP->write(Data, 13);
    }

    //PGN32401, module, analog info from module to RC
    //0     145
    //1     126
    //2     module ID
    //3     analog 0, Lo
    //4     analog 0, Hi
    //5     analog 1, Lo
    //6     analog 1, Hi
    //7     analog 2, Lo
    //8     analog 2, Hi
    //9     analog 3, Lo
    //10    analog 3, Hi
    //11    InoID lo
    //12    InoID hi
    //13    status
    //      - bit 0, work switch
    //14    CRC

    Data[0] = 145;
    Data[1] = 126;
    Data[2] = MDL.ID;

    Data[3] = 0;
    Data[4] = 0;
    Data[5] = 0;
    Data[6] = 0;
    Data[7] = 0;
    Data[8] = 0;
    Data[9] = 0;
    Data[10] = 0;

    Data[11] = (byte)InoID;
    Data[12] = InoID >> 8;

    // status
    Data[13] = 0;
    if (WrkOn) Data[13] |= 0b00000001;

    Data[14] = CRC(Data, 14, 0);

    if (Ethernet.linkStatus() == LinkON)
    {
        // send ethernet
        UDPcomm.beginPacket(DestinationIP, DestinationPort);
        UDPcomm.write(Data, 15);
        UDPcomm.endPacket();
    }
    else if (millis() - ESPtime > 5000)
    {
        // send serial
        Serial.print(Data[0]);
        for (int i = 1; i < 15; i++)
        {
            Serial.print(",");
            Serial.print(Data[i]);
        }
        Serial.println("");
    }

    // send wifi
    if (MDL.ESPserialPort != NC) SerialESP->write(Data, 15);
}

void SendNetworkConfig()
{
    // PGN32702, network config to esp
    // 0        190
    // 1        127
    // 2-16     Network Name
    // 17-31    Newtwork password
    // 32       CRC

    int PGNlength = 33;
    byte Data[PGNlength];
    Data[0] = 190;
    Data[1] = 127;

    for (int i = 0; i < 15; i++)
    {
        Data[i + 2] = MDL.NetName[i];
        Data[i + 17] = MDL.NetPassword[i];
    }

    Data[32] = CRC(Data, PGNlength - 1, 0);
    Serial.println(Data[32]);
    Serial.println(GoodCRC(Data, PGNlength));
    if (MDL.ESPserialPort != NC) SerialESP->write(Data, PGNlength);
}



