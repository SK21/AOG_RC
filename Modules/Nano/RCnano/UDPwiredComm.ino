#if UseEthernet

byte UDPpacket[30];
uint16_t UDPpgn;

void SendUDPwired()
{
    //PGN32613 to Rate Controller from Arduino
    //0	HeaderLo		101
    //1	HeaderHi		127
    //2 Controller ID
    //3	rate applied Lo 	1000 X actual
    //4 rate applied Mid
    //5	rate applied Hi
    //6	acc.Quantity Lo		10 X actual
    //7	acc.Quantity Mid
    //8	acc.Quantity Hi
    //9 PWM Lo
    //10 PWM Hi
    //11 Status
    //12 crc


    for (int i = 0; i < MDL.SensorCount; i++)
    {
        UDPpacket[0] = 101;
        UDPpacket[1] = 127;

        UDPpacket[2] = BuildModSenID(MDL.ModuleID, i);

        // rate applied, 1000 X actual
        uint32_t Applied = UPM[i] * 1000;
        UDPpacket[3] = Applied;
        UDPpacket[4] = Applied >> 8;
        UDPpacket[5] = Applied >> 16;

        // accumulated quantity, 10 X actual
        if (MeterCal[i] > 0)
        {
            uint32_t Units = TotalPulses[i] * 10.0 / MeterCal[i];
            UDPpacket[6] = Units;
            UDPpacket[7] = Units >> 8;
            UDPpacket[8] = Units >> 16;
        }
        else
        {
            UDPpacket[6] = 0;
            UDPpacket[7] = 0;
            UDPpacket[8] = 0;
        }

        //pwmSetting
        UDPpacket[9] = pwmSetting[i];
        UDPpacket[10] = pwmSetting[i] >> 8;

        // status
        // bit 0    - sensor 0 receiving rate controller data
        // bit 1    - sensor 1 receiving rate controller data
        UDPpacket[11] = 0;
        if (millis() - CommTime[0] < 4000) UDPpacket[11] |= 0b00000001;
        if (millis() - CommTime[1] < 4000) UDPpacket[11] |= 0b00000010;

        // crc
        UDPpacket[12] = CRC(UDPpacket, 12, 0);

        //off to AOG
        ether.sendUdp(UDPpacket, 13, SourcePort, DestinationIP, DestinationPort);
    }
}

//void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, byte* UDPpacket, uint16_t len)
void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* Data, uint16_t len)
{
    if (len)
    {
        UDPpgn = Data[1] << 8 | Data[0];
        switch (UDPpgn)
        {
        case 32614:
            //PGN32614 to Arduino from Rate Controller, 14 bytes
            //0	HeaderLo		102
            //1	HeaderHi		127
            //2 Controller ID
            //3	relay Lo		0 - 7
            //4	relay Hi		8 - 15
            //5	rate set Lo		1000 X actual
            //6 rate set Mid
            //7	rate set Hi		
            //8	Flow Cal Lo		100 X actual
            //9 Flow Cal Mid
            //10 Flow Cal Hi
            //11 Command
            //	        - bit 0		    reset acc.Quantity
            //	        - bit 1,2,3		control type 0-4
            //	        - bit 4		    MasterOn, or true if no switchbox
            //          - bit 5         0 - average time for multiple pulses, 1 - time for one pulse
            //          - bit 6         AutoOn
            //          - bit 7         Calibration On
            //12    power relay Lo      list of power type relays 0-7
            //13    power relay Hi      list of power type relays 8-15
            //14	manual pwm
            //15	CRC
            PGNlength = 16;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    if (ParseModID(Data[2]) == MDL.ModuleID)
                    {
                        byte SensorID = ParseSenID(Data[2]);
                        if (SensorID < MDL.SensorCount)
                        {
                            RelayLo = Data[3];
                            RelayHi = Data[4];

                            // rate setting, 1000 times actual
                            uint32_t RateSet = Data[5] | (uint32_t)Data[6] << 8 | (uint32_t)Data[7] << 16;
                            RateSetting[SensorID] = (float)(RateSet * 0.001);

                            // Meter Cal, 1000 times actual
                            uint32_t Temp = Data[8] | (uint32_t)Data[9] << 8 | (uint32_t)Data[10] << 16;
                            MeterCal[SensorID] = Temp * 0.001;

                            // command byte
                            InCommand[SensorID] = Data[11];
                            if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

                            ControlType[SensorID] = 0;
                            if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
                            if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;
                            if ((InCommand[SensorID] & 8) == 8) ControlType[SensorID] += 4;

                            MasterOn[SensorID] = ((InCommand[SensorID] & 16) == 16);
                            UseMultiPulses[SensorID] = ((InCommand[SensorID] & 32) == 32);
                            AutoOn = ((InCommand[SensorID] & 64) == 64);

                            // power relays
                            PowerRelayLo = Data[12];
                            PowerRelayHi = Data[13];

                            ManualAdjust[SensorID] = Data[14];

                            CommTime[SensorID] = millis();
                        }
                    }
                }
            }
            break;

        case 32616:
            // PID to Arduino from RateController
            // 0    104
            // 1    127
            // 2    Mod/Sen ID     0-15/0-15
            // 3    KP 0
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
            // 17   CRC

            PGNlength = 18;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    byte tmp = Data[2];
                    if (ParseModID(tmp) == MDL.ModuleID)
                    {
                        byte SensorID = ParseSenID(tmp);
                        if (SensorID < MDL.SensorCount)
                        {
                            uint32_t tmp = Data[3] | (uint32_t)Data[4] << 8 | (uint32_t)Data[5] << 16 | (uint32_t)Data[6] << 24;
                            PIDkp[SensorID] = (float)(tmp * 0.0001);

                            tmp = Data[7] | (uint32_t)Data[8] << 8 | (uint32_t)Data[9] << 16 | (uint32_t)Data[10] << 24;
                            PIDki[SensorID] = (float)(tmp * 0.0001);

                            tmp = Data[11] | (uint32_t)Data[12] << 8 | (uint32_t)Data[13] << 16 | (uint32_t)Data[14] << 24;
                            PIDkd[SensorID] = (float)(tmp * 0.0001);

                            MinPWM[SensorID] = Data[15];
                            MaxPWM[SensorID] = Data[16];

                            CommTime[SensorID] = millis();
                        }
                    }
                }
            }
            break;

        case 32619:
            // from Wemos D1 mini, 6 bytes
            // section buttons
            PGNlength = 6;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    for (int i = 2; i < 6; i++)
                    {
                        WifiSwitches[i] = Data[i];
                    }
                    WifiSwitchesEnabled = true;
                    WifiSwitchesTimer = millis();
                }
            }
            break;

        case 32620:
            // section switch IDs to arduino, 11 bytes
            // 0    108
            // 1    127
            // 2    sec 0-1
            // 3    sec 2-3
            // 4    sec 4-5
            // 5    sec 6-7
            // 6    sec 8-9
            // 7    sec 10-11
            // 8    sec 12-13
            // 9    sec 14-15
            // 10   crc
            PGNlength = 11;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        SwitchBytes[i] = Data[i + 2];
                    }
                    TranslateSwitchBytes();
                }
            }
            break;

        case 32625:
            // from rate controller, 8 bytes
            // Nano config
            // 0    113
            // 1    127
            // 2    ModuleID
            // 3    SensorCount
            // 4    IP address
            // 5    Commands
            //      - UseMCP23017
            //      - RelyOnSignal
            //      - FlowOnSignal
            // 6    minimum ms debounce
            // 7    crc
            PGNlength = 8;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    MDL.ModuleID = Data[2];
                    MDL.SensorCount = Data[3];
                    MDL.IPpart3 = Data[4];

                    byte tmp = Data[5];
                    if ((tmp & 1) == 1) MDL.UseMCP23017 = 1; else MDL.UseMCP23017 = 0;
                    if ((tmp & 2) == 2) MDL.RelayOnSignal = 1; else MDL.RelayOnSignal = 0;
                    if ((tmp & 4) == 4) MDL.FlowOnDirection = 1; else MDL.FlowOnDirection = 0;

                    MDL.Debounce = Data[6];

                    EEPROM.put(10, MDL);
                }
            }
            break;

        case 32626:
            // from rate controller, 25 bytes
            // Nano pins
            // 0        114
            // 1        127
            // 2        Flow 1
            // 3        Flow 2
            // 4        Dir 1
            // 5        Dir 2
            // 6        PWM 1
            // 7        PWM 2
            // 8 - 23   Relays 1-16
            // 24       crc
            PGNlength = 25;

            if (len > PGNlength - 1)
            {
                if (GoodCRC(Data, PGNlength))
                {
                    MDL.Flow1 = Data[2];
                    MDL.Flow2 = Data[3];
                    MDL.Dir1 = Data[4];
                    MDL.Dir2 = Data[5];
                    MDL.PWM1 = Data[6];
                    MDL.PWM2 = Data[7];

                    for (int i = 0; i < 16; i++)
                    {
                        MDL.Relays[i] = Data[i + 8];
                    }

                    EEPROM.put(10, MDL);

                    //reset the arduino
                    resetFunc();
                }
            }
            break;

        default:
            break;
        }
    }
}
#endif
