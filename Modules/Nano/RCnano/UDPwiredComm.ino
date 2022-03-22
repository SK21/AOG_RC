#if(CommType == 1)
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
    for (int i = 0; i < SensorCount; i++)
    {
        toSend[i][0] = 101;
        toSend[i][1] = 127;

        toSend[i][2] = BuildModSenID(ModuleID, i);

        // rate applied, 10 X actual
        Temp = (UPM[i] * 10);
        toSend[i][3] = Temp;
        Temp = (int)(UPM[i] * 10) >> 8;
        toSend[i][4] = Temp;
        Temp = (int)(UPM[i] * 10) >> 16;
        toSend[i][5] = Temp;

        // accumulated quantity, 10 X actual
        long Units = TotalPulses[i] * 10.0 / MeterCal[i];
        Temp = Units;
        toSend[i][6] = Temp;
        Temp = Units >> 8;
        toSend[i][7] = Temp;
        Temp = Units >> 16;
        toSend[i][8] = Temp;

        //pwmSetting
        Temp = (byte)(pwmSetting[i] * 10);
        toSend[i][9] = Temp;
        Temp = (byte)((pwmSetting[i] * 10) >> 8);
        toSend[i][10] = Temp;

        //off to AOG
        ether.sendUdp(toSend[i], sizeof(toSend[i]), SourcePort, DestinationIP, DestinationPort);
    }
}
void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[4], uint16_t src_port, byte* data, uint16_t len)

//void ReceiveUDPwired(uint16_t dest_port, uint8_t src_ip[IP_LEN], uint16_t src_port, byte* data, uint16_t len)
{
    //PGN32614 to Arduino from Rate Controller
    //0	HeaderLo		102
    //1	HeaderHi		127
    //2 Controller ID
    //3	relay Lo		0 - 7
    //4	relay Hi		8 - 15
    //5	rate set Lo		10 X actual
    //6 rate set Mid
    //7	rate set Hi		10 X actual
    //8	Flow Cal Lo		100 X actual
    //9	Flow Cal Hi		
    //10	Command
    //- bit 0		    reset acc.Quantity
    //- bit 1, 2		valve type 0 - 3
    //- bit 3		    simulate flow
    //- bit 4           0 - average time for multiple pulses, 1 - time for one pulse
    //- bit 5           AutoOn

    PGN = data[1] << 8 | data[0];

    if (len > 10 && PGN == 32614)
    {
        byte tmp = data[2];

        if (ParseModID(tmp) == ModuleID)
        {
            byte SensorID = ParseSenID(tmp);
            if (SensorID < SensorCount)
            {
                RelayLo = data[3];
                RelayHi = data[4];

                // rate setting, 10 times actual
                UnSignedTemp = data[5] | data[6] << 8 | data[7] << 16;
                float TmpSet = (float)UnSignedTemp * 0.1;

                // Meter Cal, 100 times actual
                UnSignedTemp = data[8] | data[9] << 8;
                MeterCal[SensorID] = (float)UnSignedTemp * 0.01;

                // command byte
                InCommand[SensorID] = data[10];
                if ((InCommand[SensorID] & 1) == 1) TotalPulses[SensorID] = 0;	// reset accumulated count

                ControlType[SensorID] = 0;
                if ((InCommand[SensorID] & 2) == 2) ControlType[SensorID] += 1;
                if ((InCommand[SensorID] & 4) == 4) ControlType[SensorID] += 2;

                UseMultiPulses[SensorID] = ((InCommand[SensorID] & 16) == 16);

                AutoOn = ((InCommand[SensorID] & 32) == 32);
                if (AutoOn)
                {
                    RateSetting[SensorID] = TmpSet;
                }
                else
                {
                    NewRateFactor[SensorID] = TmpSet;
                }

                //reset watchdog as we just heard from AgOpenGPS
                watchdogTimer = 0;
                CommTime[SensorID] = millis();
            }
        }
    }

    if (len > 9 && PGN == 32616)
    {
        // PID to Arduino from RateController
        byte tmp = data[2];
        if (ParseModID(tmp) == ModuleID)
        {
            byte SensorID = ParseSenID(tmp);
            if (SensorID < SensorCount)
            {
                PIDkp[SensorID] = data[3];
                PIDminPWM[SensorID] = data[4];
                PIDLowMax[SensorID] = data[5];
                PIDHighMax[SensorID] = data[6];
                PIDdeadband[SensorID] = data[7];
                PIDbrakePoint[SensorID] = data[8];
                AdjustTime[SensorID] = data[9];

                watchdogTimer = 0;
                CommTime[SensorID] = millis();
            }
        }
    }

    if (len > 9 && PGN == 32620)
    {
        // section switch IDs to arduino
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

        for (int i = 0; i < 8; i++)
        {
            SwitchBytes[i] = data[i + 2];
        }
        TranslateSwitchBytes();
    }
}
#endif
