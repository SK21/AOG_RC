void SendComm()
{
    if (millis() - SendLast > SendTime)
    {
        SendLast = millis();

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
        //      bit 0   sensor connected
        //12    Hz Lo
        //13    Hz Hi
        //14    CRC

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
            if (millis() - Sensor[i].CommTime < 4000) Data[11] |= 0b00000001;

            // Hz
            uint16_t Hz = Sensor[i].Hz * 10;
            Data[12] = Hz;
            Data[13] = Hz >> 8;

            // crc
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

        //PGN32401, module info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     Pressure Lo 
        //4     Pressure Hi
        //5     gain adjust 0
        //6     integral adjust 0
        //7     gain adjust 1
        //8     integral adjust 1
        //9     -
        //10    -
        //11    InoID lo
        //12    InoID hi
        //13    status
        //      bit 0   work switch
        //      bit 1   wifi rssi < -80
        //      bit 2	wifi rssi < -70
        //      bit 3	wifi rssi < -65
        //      bit 4   ethernet connected
        //      bit 5   good pin configuration
        //14    CRC

        Data[0] = 145;
        Data[1] = 126;
        Data[2] = MDL.ID;
        Data[3] = (byte)PressureReading;
        Data[4] = (byte)(PressureReading >> 8);
        Data[5] = GainAdjust[0];
        Data[6] = IntegralAdjust[0];
        Data[7] = GainAdjust[1];
        Data[8] = IntegralAdjust[1];
        Data[9] = 0;
        Data[10] = 0;
        Data[11] = (byte)InoID;
        Data[12] = InoID >> 8;

        // status
        Data[13] = 0;
        if (WorkPinOn()) Data[13] |= 0b00000001;

        if (millis() - ESPtime < 5000)
        {
            if (WifiStrength < -80)
            {
                Data[13] |= 0b00000010;
            }
            else if (WifiStrength < -70)
            {
                Data[13] |= 0b00000100;
            }
            else
            {
                Data[13] |= 0b00001000;
            }
        }

        if (Ethernet.linkStatus() == LinkON) Data[13] |= 0b00010000;
        if (GoodPins) Data[13] |= 0b00100000;

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
        Data[i + 2] = MDL.SSID[i];
        Data[i + 17] = MDL.Password[i];
    }

    Data[32] = CRC(Data, PGNlength - 1, 0);
    Serial.println(Data[32]);
    Serial.println(GoodCRC(Data, PGNlength));
    if (MDL.ESPserialPort != NC) SerialESP->write(Data, PGNlength);
}
