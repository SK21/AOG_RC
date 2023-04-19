
byte DataEthernet[MaxReadBuffer];

uint8_t sett;

void ReceiveSteerData()
{
    // ethernet
    if (Ethernet.linkStatus() == LinkON)
    {
        uint16_t len = UDPsteering.parsePacket();
        if (len)
        {
            UDPsteering.read(DataEthernet, MaxReadBuffer);
            if ((DataEthernet[0] == 0x80) && (DataEthernet[1] == 0x81 && DataEthernet[2] == 0x7F))  // 0x7F is source, AGIO
            {
                switch (DataEthernet[3])
                {
                case 200:
                    // hello from AGIO
                    SendHelloReply();
                    break;

                case 201:
                    // update IP
                    if (DataEthernet[4] == 5 && DataEthernet[5] == 201 && DataEthernet[6] == 201)
                    {
                        MDL.IP0 = DataEthernet[7];
                        MDL.IP1 = DataEthernet[8];
                        MDL.IP2 = DataEthernet[9];
                        EEPROM.put(110, MDL);

                        //reset the Teensy
                        SCB_AIRCR = 0x05FA0004;
                    }
                    break;

                case 202:
                    // ID scan request
                    if (DataEthernet[4] == 3 && DataEthernet[5] == 202 && DataEthernet[6] == 202)
                    {
                        SendScanIDreply();
                    }
                    break;

                case 251:
                    // steer config
                    sett = DataEthernet[5];

                    if (bitRead(sett, 0)) steerConfig.InvertWAS = 1; else steerConfig.InvertWAS = 0;
                    if (bitRead(sett, 1)) steerConfig.IsRelayActiveHigh = 1; else steerConfig.IsRelayActiveHigh = 0;
                    if (bitRead(sett, 2)) steerConfig.MotorDriveDirection = 1; else steerConfig.MotorDriveDirection = 0;
                    if (bitRead(sett, 3)) steerConfig.SingleInputWAS = 1; else steerConfig.SingleInputWAS = 0;
                    if (bitRead(sett, 4)) steerConfig.CytronDriver = 1; else steerConfig.CytronDriver = 0;
                    if (bitRead(sett, 5)) steerConfig.SteerSwitch = 1; else steerConfig.SteerSwitch = 0;
                    if (bitRead(sett, 6)) steerConfig.SteerButton = 1; else steerConfig.SteerButton = 0;
                    if (bitRead(sett, 7)) steerConfig.ShaftEncoder = 1; else steerConfig.ShaftEncoder = 0;

                    steerConfig.PulseCountMax = DataEthernet[6];

                    sett = DataEthernet[8]; //setting1 - Danfoss valve etc

                    if (bitRead(sett, 0)) steerConfig.IsDanfoss = 1; else steerConfig.IsDanfoss = 0;
                    if (bitRead(sett, 1)) steerConfig.PressureSensor = 1; else steerConfig.PressureSensor = 0;
                    if (bitRead(sett, 2)) steerConfig.CurrentSensor = 1; else steerConfig.CurrentSensor = 0;
                    if (bitRead(sett, 3)) MDL.SwapRollPitch = 1; else MDL.SwapRollPitch = 0;

                    EEPROM.put(40, steerConfig);
                    EEPROM.put(110, MDL);

                    //reset the Teensy
                    SCB_AIRCR = 0x05FA0004;
                    break;

                case 252:
                    // autosteer settings
                    steerSettings.Kp = DataEthernet[5];
                    steerSettings.highPWM = DataEthernet[6];
                    steerSettings.minPWM = DataEthernet[8];

                    //steerSettings.lowPWM = DataEthernet[7];
                    steerSettings.lowPWM = (byte)((float)steerSettings.minPWM * 1.2);

                    steerSettings.steerSensorCounts = (float)DataEthernet[9];
                    steerSettings.wasOffset = DataEthernet[10] | DataEthernet[11] << 8;
                    steerSettings.AckermanFix = (float)DataEthernet[12] * 0.01;

                    EEPROM.put(10, steerSettings);
                    break;

                case 254:
                    // autosteer data
                    Speed_KMH = (DataEthernet[6] << 8 | DataEthernet[5]) * 0.1;
                    guidanceStatus = DataEthernet[7];
                    steerAngleSetPoint = (float)((int16_t)(DataEthernet[9] << 8 | DataEthernet[8])) * 0.01;
                    SendSteerData();
                    CommTime = millis();
                    break;
                }
            }
        }
    }
}

void SendSteerData()
{
    // Steer Data 1
    // steer angle
    int16_t tmp = (int)(steerAngleActual * 100);
    PGN_253[5] = (byte)tmp;
    PGN_253[6] = tmp >> 8;

    // heading
    if (MDL.Receiver == 0 && MDL.IMU != 0)
    {
        tmp = (int)(IMU_Heading);
    }
    else
    {
        tmp = 9999;
    }
    PGN_253[7] = (byte)tmp;
    PGN_253[8] = tmp >> 8;

    // roll
    if (MDL.Receiver == 0 && MDL.IMU != 0)
    {
        tmp = (int)(IMU_Roll);
    }
    else
    {
        tmp = 8888;
    }

    PGN_253[9] = (byte)tmp;
    PGN_253[10] = tmp >> 8;

    PGN_253[11] = switchByte;
    PGN_253[12] = abs(pwmDrive);    // only works for positive values

    //add the checksum
    int16_t CK_A = 0;
    for (uint8_t i = 2; i < sizeof(PGN_253) - 1; i++)
    {
        CK_A = (CK_A + PGN_253[i]);
    }
    PGN_253[sizeof(PGN_253) - 1] = CK_A;

    // to ethernet
    if (Ethernet.linkStatus() == LinkON)
    {
        UDPsteering.beginPacket(DestinationIP, DestinationPort);
        UDPsteering.write(PGN_253, sizeof(PGN_253));
        UDPsteering.endPacket();
    }

    // Steer Data 2
    if (steerConfig.PressureSensor || steerConfig.CurrentSensor)
    {
        PGN_250[5] = (byte)SensorReading;

        //add the checksum for AOG2
        CK_A = 0;
        for (uint8_t i = 2; i < sizeof(PGN_250) - 1; i++)
        {
            CK_A = (CK_A + PGN_250[i]);
        }
        PGN_250[sizeof(PGN_250) - 1] = CK_A;

        // to ethernet
        if (Ethernet.linkStatus() == LinkON)
        {
            UDPsteering.beginPacket(DestinationIP, DestinationPort);
            UDPsteering.write(PGN_250, sizeof(PGN_250));
            UDPsteering.endPacket();
        }
    }
}

void SendHelloReply()
{
    int16_t sa = (int16_t)(steerAngleActual * 100);

    helloFromAutoSteer[5] = (uint8_t)sa;
    helloFromAutoSteer[6] = sa >> 8;

    helloFromAutoSteer[7] = (uint8_t)helloSteerPosition;
    helloFromAutoSteer[8] = helloSteerPosition >> 8;
    helloFromAutoSteer[9] = switchByte;

    ////add the checksum
    //int16_t CK_A = 0;
    //for (uint8_t i = 2; i < sizeof(helloFromAutoSteer) - 1; i++)
    //{
    //    CK_A = (CK_A + helloFromAutoSteer[i]);
    //}
    //helloFromAutoSteer[sizeof(helloFromAutoSteer) - 1] = CK_A;

    // to ethernet
    if (Ethernet.linkStatus() == LinkON)
    {
        UDPsteering.beginPacket(DestinationIP, DestinationPort);
        UDPsteering.write(helloFromAutoSteer, sizeof(helloFromAutoSteer));
        UDPsteering.endPacket();

        if (MDL.IMU > 0)
        {
            UDPsteering.beginPacket(DestinationIP, DestinationPort);
            UDPsteering.write(helloFromIMU, sizeof(helloFromIMU));
            UDPsteering.endPacket();
        }
    }
}

void SendScanIDreply()
{
    IPAddress rem_ip = UDPsteering.remoteIP();

    uint8_t scanReply[] = { 128, 129, MDL.IP3, 203, 7, MDL.IP0, MDL.IP1, MDL.IP2, MDL.IP3,
                            rem_ip[0],rem_ip[1],rem_ip[2], 23 };

    //checksum
    int16_t CK_A = 0;
    for (uint8_t i = 2; i < sizeof(scanReply) - 1; i++)
    {
        CK_A = (CK_A + scanReply[i]);
    }
    scanReply[sizeof(scanReply) - 1] = CK_A;

    static uint8_t ipDest[] = { 255,255,255,255 };
    uint16_t portDest = 9999; //AOG port that listens

    // to ethernet
    if (Ethernet.linkStatus() == LinkON)
    {
        UDPsteering.beginPacket(ipDest, portDest);
        UDPsteering.write(scanReply, sizeof(scanReply));
        UDPsteering.endPacket();
    }
}

