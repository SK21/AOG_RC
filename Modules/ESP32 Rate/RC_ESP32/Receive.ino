
void ReceiveUDP()
{
    const uint8_t MaxReadBuffer = 100;	// bytes

    if (ChipFound)
    {
        if (Ethernet.linkStatus() == LinkON)
        {
            uint16_t len = UDP_Ethernet.parsePacket();
            if (len)
            {
                byte Data[MaxReadBuffer];
                UDP_Ethernet.read(Data, MaxReadBuffer);
                ReadPGNs(Data, len);
            }
        }
    }

    uint16_t len = UDP_Wifi.parsePacket();
    if (len)
    {
        byte Data[MaxReadBuffer];
        UDP_Wifi.read(Data, MaxReadBuffer);
        ReadPGNs(Data, len);
    }
}

void ReadPGNs(byte data[], uint16_t len)
{
    uint16_t PGN = data[1] << 8 | data[0];
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
            if (GoodCRC(data, PGNlength))
            {
                if (ParseModID(data[2]) == MDL.ID)
                {
                    byte SensorID = ParseSenID(data[2]);
                    if (SensorID < MDL.SensorCount)
                    {
                        // rate setting, 1000 times actual
                        uint32_t RateSet = data[3] | (uint32_t)data[4] << 8 | (uint32_t)data[5] << 16;
                        Sensor[SensorID].TargetUPM = (float)(RateSet * 0.001);

                        // Meter Cal, 1000 times actual
                        uint32_t Temp = data[6] | (uint32_t)data[7] << 8 | (uint32_t)data[8] << 16;
                        Sensor[SensorID].MeterCal = Temp * 0.001;

                        // command byte
                        byte InCommand = data[9];
                        if ((InCommand & 1) == 1) Sensor[SensorID].TotalPulses = 0;	// reset accumulated count

                        Sensor[SensorID].ControlType = 0;
                        if ((InCommand & 2) == 2) Sensor[SensorID].ControlType += 1;
                        if ((InCommand & 4) == 4) Sensor[SensorID].ControlType += 2;
                        if ((InCommand & 8) == 8) Sensor[SensorID].ControlType += 4;

                        MasterOn = ((InCommand & 16) == 16);

                        Sensor[SensorID].AutoOn = ((InCommand & 64) == 64);

                        int16_t tmp = data[10] | data[11] << 8;
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
            if (GoodCRC(data, PGNlength))
            {
                if (ParseModID(data[2]) == MDL.ID)
                {
                    RelayLo = data[3];
                    RelayHi = data[4];
                    PowerRelayLo = data[5];
                    PowerRelayHi = data[6];
                    InvertedLo = data[7];
                    InvertedHi = data[8];
                }
            }
        }
        break;

    case 32502:
        // PGN32502, Control settings from RC to module
        // 0    246
        // 1    126
        // 2    Mod/Sen ID     0-15/0-15
        // 3    MaxPWM
        // 4    MinPWM
        // 5    Kp
        // 6    Ki
        // 7    Deadband        %       actual X 10
        // 8    Brakepoint      %
        // 9    PIDslowAdjust   %
        // 10   Slew Rate
        // 11   Max Integral      actual X 10
        // 12   -
        // 13   TimedMinStart
        // 14   TimedAdjust Lo
        // 15   TimedAdjust Hi
        // 16   TimedPause Lo
        // 17   TimedPause Hi
        // 18   PIDtime
        // 19   PulseMinHz              actual X 10
        // 20   PulseMaxHz Lo
        // 21   PulseMaxHz Hi
        // 22   PulseSampleSize
        // 23   CRC

        PGNlength = 24;

        if (len > PGNlength - 1)
        {
            if (GoodCRC(data, PGNlength))
            {
                if (ParseModID(data[2]) == MDL.ID)
                {
                    byte SensorID = ParseSenID(data[2]);
                    if (SensorID < MDL.SensorCount)
                    {
                        Sensor[SensorID].MaxPWM = (float)(255.0 * data[3] / 100.0);
                        Sensor[SensorID].MinPWM = (float)(255.0 * data[4] / 100.0);

                        // 1.1 ^ (gain scroll bar value - 120) gives a scale range of 0.00001 to 0.1486
                        Sensor[SensorID].Kp = pow(1.1, data[5] - 120);

                        if (data[6] > 0)
                        {
                            Sensor[SensorID].Ki = pow(1.1, data[6] - 120);
                        }
                        else
                        {
                            Sensor[SensorID].Ki = 0;
                        }

                        Sensor[SensorID].Deadband = data[7] / 1000.0;
                        Sensor[SensorID].BrakePoint = data[8] / 100.0;
                        Sensor[SensorID].PIDslowAdjust = data[9] / 100.0;
                        Sensor[SensorID].SlewRate = data[10];
                        Sensor[SensorID].MaxIntegral = data[11] / 10.0;
                        Sensor[SensorID].TimedMinStart = data[13] / 100.0;
                        Sensor[SensorID].TimedAdjust = data[14] | data[15] << 8;
                        Sensor[SensorID].TimedPause = data[16] | data[17] << 8;
                        Sensor[SensorID].PIDtime = data[18];
                        Sensor[SensorID].PulseMax = 10000000 / data[19];	//Hz * 10 to micros, minimum Hz - maximum pulse time
                        uint32_t tmp = data[20] | data[21] << 8;
                        Sensor[SensorID].PulseMin = 1000000 / tmp;
                        Sensor[SensorID].PulseSampleSize = data[22];

                        SaveData();
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
            if (GoodCRC(data, PGNlength))
            {
                MDLnetwork.IP0 = data[2];
                MDLnetwork.IP1 = data[3];
                MDLnetwork.IP2 = data[4];

                SaveNetworks();
                ESP.restart();
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
        //      bit 2 - 
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
            if (GoodCRC(data, PGNlength))
            {
                MDL.ID = data[2];
                MDL.SensorCount = data[3];

                byte tmp = data[4];
                MDL.InvertRelay = ((tmp & 1) == 1);
                MDL.InvertFlow = ((tmp & 2) == 2);
                MDL.WorkPinIsMomentary = ((tmp & 8) == 8);
                MDL.Is3Wire = ((tmp & 16) == 16);
                MDL.ADS1115Enabled = ((tmp & 32) == 32);

                MDL.RelayControl = data[5];
                Sensor[0].FlowPin = data[7];
                Sensor[0].IN1 = data[8];
                Sensor[0].IN2 = data[9];
                Sensor[1].FlowPin = data[10];
                Sensor[1].IN1 = data[11];
                Sensor[1].IN2 = data[12];

                for (int i = 0; i < 16; i++)
                {
                    MDL.RelayControlPins[i] = data[13 + i];
                }

                MDL.WorkPin = data[29];
                MDL.PressurePin = data[30];

                SaveData(); 
                ESP.restart();
            }
        }
        break;
    }
}


