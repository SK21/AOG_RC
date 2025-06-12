
const double KiMultiplier = 10;

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
            bool Sent = false;

            // ethernet
            if (ChipFound)
            {
                if (Ethernet.linkStatus() == LinkON)
                {

                    UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
                    UDP_Ethernet.write(Data, 13);
                    UDP_Ethernet.endPacket();
                    Sent = true;
                }
            }

            // wifi
            if (!Sent)
            {
                UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
                UDP_Wifi.write(Data, 15);
                UDP_Wifi.endPacket();
            }
        }

        //PGN32401, module info from module to RC
        //0     145
        //1     126
        //2     module ID
        //3     Pressure Lo 
        //4     Pressure Hi
        //5     -
        //6     -
        //7     -
        //8     -
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
        Data[5] = 0;
        Data[6] = 0;
        Data[7] = 0;
        Data[8] = 0;
        Data[9] = 0;
        Data[10] = 0;
        Data[11] = (byte)InoID;
        Data[12] = (byte)(InoID >> 8);

        // status
        Data[13] = 0;
        if (WorkSwitchOn) Data[13] |= 0b00000001;

        if (WiFi.isConnected())
        {
            int8_t WifiStrength = WiFi.RSSI();
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

        if (ChipFound)
        {
            if (Ethernet.linkStatus() == LinkON) Data[13] |= 0b00010000;
        }

        if (GoodPins) Data[13] |= 0b00100000;

        Data[14] = CRC(Data, 14, 0);

        bool Sent = false;
        // ethernet
        if (ChipFound)
        {
            if (Ethernet.linkStatus() == LinkON)
            {

                UDP_Ethernet.beginPacket(Ethernet_DestinationIP, DestinationPort);
                UDP_Ethernet.write(Data, 15);
                UDP_Ethernet.endPacket();
                Sent = true;
            }
        }

        // wifi
        if (!Sent)
        {
            UDP_Wifi.beginPacket(Wifi_DestinationIP, DestinationPort);
            UDP_Wifi.write(Data, 15);
            UDP_Wifi.endPacket();
        }
    }
}

void ReceiveUDP()
{
    if (ChipFound)
    {
        if (Ethernet.linkStatus() == LinkON)
        {
            uint16_t len = UDP_Ethernet.parsePacket();
            if (len)
            {
                byte Data[MaxReadBuffer];
                UDP_Ethernet.read(Data, MaxReadBuffer);
                ParseData(Data, len);
            }
        }
    }

    uint16_t len = UDP_Wifi.parsePacket();
    if (len)
    {
        byte Data[MaxReadBuffer];
        UDP_Wifi.read(Data, MaxReadBuffer);
        ParseData(Data, len);
    }
}

void ParseData(byte Data[], uint16_t len)
{
    uint16_t PGN = Data[1] << 8 | Data[0];
    byte PGNlength;
    switch (PGN)
    {
    case 32500:
        //PGN32500, Rate settings from RC to module
        //0	    HeaderLo		    244
        //1	    HeaderHi		    126
        //2     Mod/Sen ID          0-15/0-15
        //3	    rate set Lo		    1000 X actual
        //4     rate set Mid
        //5	    rate set Hi
        //6	    Flow Cal Lo	        1000 X actual
        //7     Flow Cal Mid
        //8     Flow Cal Hi
        //9	    Command
        //	        - bit 0		    reset acc.Quantity
        //	        - bit 1,2,3		control type 0-4
        //	        - bit 4		    MasterOn
        //          - bit 5         -
        //          - bit 6         AutoOn
        //          - bit 7         -
        //10    manual pwm Lo
        //11    manual pwm Hi
        //12    -
        //13    CRC

        PGNlength = 14;

        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                if (ParseModID(Data[2]) == MDL.ID)
                {
                    byte SensorID = ParseSenID(Data[2]);
                    if (SensorID < MDL.SensorCount)
                    {
                        // rate setting, 1000 times actual
                        uint32_t RateSet = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16;
                        Sensor[SensorID].TargetUPM = (float)(RateSet * 0.001);

                        // Meter Cal, 1000 times actual
                        uint32_t Temp = Data[6] | (uint32_t)Data[7] << 8 | (uint32_t)Data[8] << 16;
                        Sensor[SensorID].MeterCal = Temp * 0.001;

                        // command byte
                        byte InCommand = Data[9];
                        if ((InCommand & 1) == 1) Sensor[SensorID].TotalPulses = 0;	// reset accumulated count

                        Sensor[SensorID].ControlType = 0;
                        if ((InCommand & 2) == 2) Sensor[SensorID].ControlType += 1;
                        if ((InCommand & 4) == 4) Sensor[SensorID].ControlType += 2;
                        if ((InCommand & 8) == 8) Sensor[SensorID].ControlType += 4;

                        MasterOn = ((InCommand & 16) == 16);

                        AutoOn = ((InCommand & 64) == 64);

                        int16_t tmp = Data[10] | Data[11] << 8;
                        Sensor[SensorID].ManualAdjust = tmp;

                        Sensor[SensorID].CommTime = millis();
                    }
                }
            }
        }
        break;

    case 32501:
        //PGN32501, Relay settings from RC to module
        //0	    HeaderLo		    245
        //1	    HeaderHi		    126
        //2     Module ID
        //3	    relay Lo		    0-7
        //4 	relay Hi		    8-15
        //5     power relay Lo      list of power type relays 0-7
        //6     power relay Hi      list of power type relays 8-15
        //7     Inverted Lo         
        //8     Inverted Hi
        //9     CRC

        PGNlength = 10;

        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                if (ParseModID(Data[2]) == MDL.ID)
                {
                    RelayLo = Data[3];
                    RelayHi = Data[4];
                    PowerRelayLo = Data[5];
                    PowerRelayHi = Data[6];
                    InvertedLo = Data[7];
                    InvertedHi = Data[8];
                }
            }
        }
        break;

    case 32502:
        // PGN32502, Control settings from RC to module
        // 0   246
        // 1   126
        // 2   Mod/Sen ID     0-15/0-15
        // 3   Ki
        // 4   -
        // 5   -
        // 6   MinAdjust
        // 7   MaxAdjust
        // 8   Kp
        // 9   CRC

        PGNlength = 10;

        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                if (ParseModID(Data[2]) == MDL.ID)
                {
                    byte SensorID = ParseSenID(Data[2]);
                    if (SensorID < MDL.SensorCount)
                    {
                        if (Data[3] > 0)
                        {
                            Sensor[SensorID].Ki = pow(1.06, Data[3] - 120);
                        }
                        else
                        {
                            Sensor[SensorID].Ki = 0;
                        }

                        Sensor[SensorID].MinPower = (double)(255.0 * Data[6] / 100.0);
                        Sensor[SensorID].MaxPower = (double)(255.0 * Data[7] / 100.0);

                        // 1.1 ^ (gain scroll bar value - 120) gives a scale range of 0.00001 to 0.1486
                        Sensor[SensorID].Kp = pow(1.1, Data[8] - 120);
                    }
                }
            }
        }
        break;

    case 32503:
        //PGN32503, Subnet change
        //0     HeaderLo    247
        //1     HeaderHI    126
        //2     IP 0
        //3     IP 1
        //4     IP 2
        //5     CRC

        PGNlength = 6;
        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                MDL.IP0 = Data[2];
                MDL.IP1 = Data[3];
                MDL.IP2 = Data[4];

                SaveData();
                esp_restart();
            }
        }
        break;

    case 32700:
        // module config
        //0     HeaderLo    188
        //1     HeaderHi    127
        //2     Module ID   0-15
        //3	    sensor count
        //4     commands
        //      bit 0 - Invert relay control
        //      bit 1 - Invert flow control
        //      bit 2 - wifi station/client mode enabled
        //      bit 3 - work pin is momentary
        //      bit 4 - Is3Wire valve
        //      bit 5 - ADS1115 enabled
        //5	    relay control type   0 - no relays, 1 - GPIOs, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017
        //                           , 5 - PCA9685, 6 - PCF8574
        //6	    wifi module serial port
        //7	    Sensor 0, Flow pin
        //8     Sensor 0, Dir pin
        //9     Sensor 0, PWM pin
        //10    Sensor 1, Flow pin
        //11    Sensor 1, Dir pin
        //12    Sensor 1, PWM pin
        //13    Relay pins 0-15, bytes 13-28
        //29    work pin
        //30    pressure pin
        //31    -
        //32    CRC

        PGNlength = 33;
        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                MDL.ID = Data[2];
                MDL.SensorCount = Data[3];

                byte tmp = Data[4];
                MDL.InvertRelay = ((tmp & 1) == 1);
                MDL.InvertFlow = ((tmp & 2) == 2);
                MDL.WifiModeUseStation = ((tmp & 4) == 4);
                MDL.WorkPinIsMomentary = ((tmp & 8) == 8);
                MDL.Is3Wire = ((tmp & 16) == 16);
                MDL.ADS1115Enabled = ((tmp & 32) == 32);

                MDL.RelayControl = Data[5];
                Sensor[0].FlowPin = Data[7];
                Sensor[0].IN1 = Data[8];
                Sensor[0].IN2 = Data[9];
                Sensor[1].FlowPin = Data[10];
                Sensor[1].IN1 = Data[11];
                Sensor[1].IN2 = Data[12];

                for (int i = 0; i < 16; i++)
                {
                    MDL.RelayControlPins[i] = Data[13 + i];
                }

                MDL.WorkPin = Data[29];
                MDL.PressurePin = Data[30];

                //SaveData();	saved in pgn 3702
            }
        }
        break;

    case 32702:
        // PGN32702, wifi network config
        // 0        190
        // 1        127
        // 2-16     Network Name (14 bytes)
        // 17-31    Network password (14 bytes)
        // 32       CRC

        PGNlength = 33;

        if (len > PGNlength - 1)
        {
            if (GoodCRC(Data, PGNlength))
            {
                char newSSID[15];
                char newPassword[15];

                memset(newSSID, '\0', sizeof(newSSID));
                memcpy(newSSID, &Data[2], 14);

                memset(newPassword, '\0', sizeof(newPassword));
                memcpy(newPassword, &Data[17], 14);

                // Check if SSID or password has changed by comparing to MDL's stored values
                if (strcmp(newSSID, MDL.SSID) != 0 || strcmp(newPassword, MDL.Password) != 0)
                {
                    // Update MDL only if there is a change
                    memset(MDL.SSID, '\0', sizeof(MDL.SSID));
                    strncpy(MDL.SSID, newSSID, sizeof(MDL.SSID) - 1);

                    memset(MDL.Password, '\0', sizeof(MDL.Password));
                    strncpy(MDL.Password, newPassword, sizeof(MDL.Password) - 1);

                    MDL.WifiModeUseStation = true;   // try the new access point
                }

                SaveData(); // needed here to save 3700 changes
                ESP.restart();
            }
        }
        break;
    }
}


