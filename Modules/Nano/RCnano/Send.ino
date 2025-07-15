
void SendData()
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
        //9     PWM Lo  10 X actual
        //10    PWM Hi
        //11    Status
        //      bit 0   sensor connected
        //12    Hz Lo
        //13    Hz Hi
        //14    CRC

        byte PGNlength = 15;

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
            Data[PGNlength - 1] = CRC(Data, PGNlength - 1, 0);

            if (EthernetConnected())
            {
                // send ethernet
                ether.sendUdp(Data, PGNlength, SourcePort, DestinationIP, DestinationPort);
            }
        }

        //PGN32401, module info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     BuildDay
        //4     BuildMonth
        //5     BuildYear
        //6     BuildType        0 - Teensy AutoSteer, 1 - Teensy Rate, 2 - Nano Rate, 3 - Nano SwitchBox, 4 - ESP Rate
        //7     Pressure Lo 
        //8     Pressure Hi
        //9     status
        //      bit 0   work switch
        //      bit 1   wifi rssi < -80
        //      bit 2	wifi rssi < -70
        //      bit 3	wifi rssi < -65
        //      bit 4   ethernet connected
        //      bit 5   good pin configuration
        //10    -
        //11    CRC

        PGNlength = 12;

        Data[0] = 145;
        Data[1] = 126;
        Data[2] = MDL.ID;
        Data[3] = BuildDay;
        Data[4] = BuildMonth;
        Data[5] = BuildYear;
        Data[6] = BuildType;
        Data[7] = (byte)PressureReading;
        Data[8] = (byte)(PressureReading >> 8);
        Data[9] = 0;
        Data[10] = 0;

        // status
        if (WorkSwitchOn) Data[9] |= 0b00000001;

        if (EthernetConnected()) Data[9] |= 0b00010000;

        if (GoodPins) Data[9] |= 0b00100000;

        Data[PGNlength - 1] = CRC(Data, PGNlength - 1, 0);

        if (EthernetConnected())
        {
            // send ethernet
            ether.sendUdp(Data, PGNlength, SourcePort, DestinationIP, DestinationPort);
        }
    }
}

