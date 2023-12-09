
void SendUDPwired()
{
    if (Ethernet.linkStatus() == LinkON)
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
            Data[9] = Sensor[i].PWM;
            Data[10] = (int)Sensor[i].PWM >> 8;

            // status
            // bit 0    - sensor 0 receiving rate controller data
            // bit 1    - sensor 1 receiving rate controller data
            Data[11] = 0;
            if (millis() - Sensor[0].CommTime < 4000) Data[11] |= 0b00000001;
            if (millis() - Sensor[1].CommTime < 4000) Data[11] |= 0b00000010;

            // wifi
            if (ESPconnected)
            {
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
            }

            // crc
            Data[12] = CRC(Data, 12, 0);

            UDPcomm.beginPacket(DestinationIP, DestinationPort);
            UDPcomm.write(Data, 13);
            UDPcomm.endPacket();
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
        //14    CRC

        Data[0] = 145;
        Data[1] = 126;
        Data[2] = MDL.ID;

        Data[3] = (byte)AINs.AIN0;
        Data[4] = (byte)(AINs.AIN0 >> 8);
        Data[5] = (byte)AINs.AIN1;
        Data[6] = (byte)(AINs.AIN1 >> 8);
        Data[7] = (byte)AINs.AIN2;
        Data[8] = (byte)(AINs.AIN2 >> 8);
        Data[9] = (byte)AINs.AIN3;
        Data[10] = (byte)(AINs.AIN3 >> 8);

        Data[11] = (byte)InoID;
        Data[12] = InoID >> 8;

        // status
        Data[13] = 0;

        Data[14] = CRC(Data, 14, 0);

        UDPcomm.beginPacket(DestinationIP, DestinationPort);
        UDPcomm.write(Data, 15);
        UDPcomm.endPacket();
    }
}

void ReceiveUDPwired()
{
    if (Ethernet.linkStatus() == LinkON)
    {
        uint16_t PGN;
        byte PGNlength;
        byte Data[MaxReadBuffer];

        uint16_t len = UDPcomm.parsePacket();
        if (len)
        {
            UDPcomm.read(Data, MaxReadBuffer);
            PGN = Data[1] << 8 | Data[0];
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
                //          - bit 5         0 - time for one pulse, 1 - average time for multiple pulses
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

                                Sensor[SensorID].UseMultiPulses = ((InCommand & 32) == 32);

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
                //7     -
                //8     CRC

                PGNlength = 9;

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
                        }
                    }
                }
                break;

            case 32502:
                // PGN32502, PID from RC to module
                // 0    246
                // 1    126
                // 2    Mod/Sen ID     0-15/0-15
                // 3    KP 0    X 10000
                // 4    KP 1
                // 5    KP 2
                // 6    KP 3
                // 7    KI 0
                // 8    KI 1
                // 9    KI 2
                // 10   KI 3
                // 11   KD 0
                // 12   KD 1
                // 13   KD 2
                // 14   KD 3
                // 15   MinPWM
                // 16   MaxPWM
                // 17   -
                // 18   CRC

                PGNlength = 19;

                if (len > PGNlength - 1)
                {
                    if (GoodCRC(Data, PGNlength))
                    {
                        if (ParseModID(Data[2]) == MDL.ID)
                        {
                            byte SensorID = ParseSenID(Data[2]);
                            if (SensorID < MDL.SensorCount)
                            {
                                uint32_t tmp = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16 | (uint32_t)Data[6] << 24;
                                Sensor[SensorID].KP = (float)(tmp * 0.0001);

                                tmp = Data[7] | (uint32_t)Data[8] << 8 | (uint32_t)Data[9] << 16 | (uint32_t)Data[10] << 24;
                                Sensor[SensorID].KI = (float)(tmp * 0.0001);

                                tmp = Data[11] | (uint32_t)Data[12] << 8 | (uint32_t)Data[13] << 16 | (uint32_t)Data[14] << 24;
                                Sensor[SensorID].KD = (float)(tmp * 0.0001);

                                Sensor[SensorID].MinPWM = Data[15];
                                Sensor[SensorID].MaxPWM = Data[16];
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

                        EEPROM.put(110, MDL);

                        // restart 
                        SCB_AIRCR = 0x05FA0004;
                    }
                }
                break;

            case 32700:
                // module config
                // 0        188
                // 1        127
                // 2        ID
                // 3        Sensor count
                // 4        Commands
                //          - Relay on high
                //          - Flow on high
                // 5        Relay control type  0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays, 4 - MCP23017, 5 - Teensy GPIO
                // 6        wifi module serial port
                // 7        Sensor 0, flow pin
                // 8        Sensor 0, dir pin
                // 9        Sensor 0, pwm pin
                // 10       Sensor 1, flow pin
                // 11       Sensor 1, dir pin
                // 12       Sensor 1, pwm pin
                // 13-28    Relay pins 0-15\
        		// 29		-
                // 30       CRC

                PGNlength = 31;
                if (len > PGNlength - 1)
                {
                    if (GoodCRC(Data, PGNlength))
                    {
                        MDL.ID = Data[2];
                        MDL.SensorCount = Data[3];

                        byte tmp = Data[4];
                        if ((tmp & 1) == 1) MDL.RelayOnSignal = 1; else MDL.RelayOnSignal = 0;
                        if ((tmp & 2) == 2) MDL.FlowOnDirection = 1; else MDL.FlowOnDirection = 0;

                        MDL.RelayControl = Data[5];
                        Sensor[0].FlowPin = Data[7];
                        Sensor[0].DirPin = Data[8];
                        Sensor[0].PWMPin = Data[9];
                        Sensor[1].FlowPin = Data[10];
                        Sensor[1].DirPin = Data[11];
                        Sensor[1].PWMPin = Data[12];

                        for (int i = 0; i < 16; i++)
                        {
                            MDL.RelayPins[i] = Data[13 + i];
                        }

                        // update stored data
                        EEPROM.put(100, InoID);
                        EEPROM.put(110, MDL);

                        for (int i = 0; i < MaxProductCount; i++)
                        {
                            EEPROM.put(200 + i * 80, Sensor[i]);
                        }

                        // restart 
                        SCB_AIRCR = 0x05FA0004;
                    }
                }
                break;
            }
        }
    }
}

void ReceiveAGIO()
{
    if (Ethernet.linkStatus() == LinkON)
    {
        byte Data[MaxReadBuffer];

        uint16_t len = AGIOcomm.parsePacket();
        if (len)
        {
            AGIOcomm.read(Data, MaxReadBuffer);
            if ((Data[0] == 128) && (Data[1] == 129) && (Data[2] == 127))  // 127 is source, AGIO
            {
                switch (Data[3])
                {
                case 201:
                    if ((Data[4] == 5) && (Data[5] == 201) && (Data[6] == 201))
                    {
                        MDL.IP0 = Data[7];
                        MDL.IP1 = Data[8];
                        MDL.IP2 = Data[9];

                        EEPROM.put(110, MDL);

                        // restart the Teensy
                        SCB_AIRCR = 0x05FA0004;
                    }
                    break;
                }
            }
        }
    }
}
