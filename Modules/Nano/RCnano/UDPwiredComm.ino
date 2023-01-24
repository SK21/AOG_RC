#if UseEthernet

byte UDPpacket[30];
uint16_t UDPpgn;

void SendUDPwired()
{
    //PGN32613 to Rate Controller from Arduino
    //0	HeaderLo		101
    //1	HeaderHi		127
    //2 Controller ID
    //3	rate applied Lo 	10 X actual
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

        // rate applied, 10 X actual
        Temp = (UPM[i] * 10);
        UDPpacket[3] = Temp;
        Temp = (int)(UPM[i] * 10) >> 8;
        UDPpacket[4] = Temp;
        Temp = (int)(UPM[i] * 10) >> 16;
        UDPpacket[5] = Temp;

        // accumulated quantity, 10 X actual
        long Units = TotalPulses[i] * 10.0 / MeterCal[i];
        Temp = Units;
        UDPpacket[6] = Temp;
        Temp = Units >> 8;
        UDPpacket[7] = Temp;
        Temp = Units >> 16;
        UDPpacket[8] = Temp;

        //pwmSetting
        Temp = (byte)(pwmSetting[i] * 10);
        UDPpacket[9] = Temp;
        Temp = (byte)((pwmSetting[i] * 10) >> 8);
        UDPpacket[10] = Temp;

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
            //5	rate set Lo		10 X actual
            //6 rate set Mid
            //7	rate set Hi		10 X actual
            //8	Flow Cal Lo		100 X actual
            //9 Flow Cal Mid
            //10 Flow Cal Hi
            //11 Command
            //- bit 0		    reset acc.Quantity
            //- bit 1, 2		valve type 0 - 3
            //- bit 3		    MasterOn
            //- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
            //- bit 5           AutoOn
            //- bit 6           Debug pgn on
            //- bit 7           Calibration on
            //12    power relay Lo      list of power type relays 0-7
            //13    power relay Hi      list of power type relays 8-15
            //14	Manual PWM		calibration pwm
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

                            // rate setting, 10 times actual
                            int RateSet = Data[5] | Data[6] << 8 | Data[7] << 16;
                            RateSetting[SensorID] = (float)(RateSet * 0.1);

                            // Meter Cal, 1000 times actual
                            uint32_t Temp = Data[8] | Data[9] << 8 | Data[10] << 16;
                            MeterCal[SensorID] = (float)(Temp * 0.001);

                            // command byte
                            InCommand[SensorID] = Data[11];
                            if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

                            ControlType[SensorID] = 0;
                            if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
                            if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;

                            MasterOn[SensorID] = ((InCommand[SensorID] & 8) == 8);
                            UseMultiPulses[SensorID] = ((InCommand[SensorID] & 16) == 16);
                            AutoOn = ((InCommand[SensorID] & 32) == 32);

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
            // PID to Arduino from RateController, 12 bytes
            PGNlength = 12;

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
                            PIDkp[SensorID] = Data[3];
                            PIDminPWM[SensorID] = Data[4];
                            PIDLowMax[SensorID] = Data[5];
                            PIDHighMax[SensorID] = Data[6];
                            PIDdeadband[SensorID] = Data[7];
                            PIDbrakePoint[SensorID] = Data[8];
                            AdjustTime[SensorID] = Data[9];
                            PIDki[SensorID] = Data[10];

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
            // from rate controller, 7 bytes
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
            // 6    crc
            PGNlength = 7;

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
