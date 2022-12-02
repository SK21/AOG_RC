IPAddress SteerSendIP(192, 168, PCB.IPpart3, 255);

void ReceiveSteerUDP()
{
    uint16_t len = UDPsteering.parsePacket();
    if (len > 2)    // needed to ensure there is new data
    {
        UDPsteering.read(data, MaxReadBuffer);

        if ((data[0] == 0x80) && (data[1] == 0x81 && data[2] == 0x7F))  // 0x7F is source, AGIO
        {
            if (data[3] == 254)
            {
                // autosteer data
                Speed_KMH = (data[6] << 8 | data[5]) * 0.1;
                guidanceStatus = data[7];
                steerAngleSetPoint = (float)((int16_t)(data[9] << 8 | data[8])) * 0.01;
                SendSteerUDP();
            }

            else if (data[3] == 252)
            {
                // autosteer settings
                steerSettings.Kp = data[5];
                steerSettings.highPWM = data[6];
                steerSettings.lowPWM = data[7];
                steerSettings.minPWM = data[8];
                steerSettings.steerSensorCounts = (float)data[9];
                steerSettings.wasOffset = data[10] | data[11] << 8;
                steerSettings.AckermanFix = (float)data[12] * 0.01;

                EEPROM.put(10, steerSettings);
            }

            else if (data[3] == 251)
            {
                // steer config
                uint8_t sett = data[5];

                if (bitRead(sett, 0)) steerConfig.InvertWAS = 1; else steerConfig.InvertWAS = 0;
                if (bitRead(sett, 1)) steerConfig.IsRelayActiveHigh = 1; else steerConfig.IsRelayActiveHigh = 0;
                if (bitRead(sett, 2)) steerConfig.MotorDriveDirection = 1; else steerConfig.MotorDriveDirection = 0;
                if (bitRead(sett, 3)) steerConfig.SingleInputWAS = 1; else steerConfig.SingleInputWAS = 0;
                if (bitRead(sett, 4)) steerConfig.CytronDriver = 1; else steerConfig.CytronDriver = 0;
                if (bitRead(sett, 5)) steerConfig.SteerSwitch = 1; else steerConfig.SteerSwitch = 0;
                if (bitRead(sett, 6)) steerConfig.SteerButton = 1; else steerConfig.SteerButton = 0;
                if (bitRead(sett, 7)) steerConfig.ShaftEncoder = 1; else steerConfig.ShaftEncoder = 0;

                steerConfig.PulseCountMax = data[6];

                sett = data[8]; //setting1 - Danfoss valve etc

                if (bitRead(sett, 0)) steerConfig.IsDanfoss = 1; else steerConfig.IsDanfoss = 0;
                if (bitRead(sett, 1)) steerConfig.PressureSensor = 1; else steerConfig.PressureSensor = 0;
                if (bitRead(sett, 2)) steerConfig.CurrentSensor = 1; else steerConfig.CurrentSensor = 0;

                EEPROM.put(40, steerConfig);

                //reset the Teensy
                SCB_AIRCR = 0x05FA0004;
            }

            else if (data[3] == 200)    // hello from AGIO
            {
                int16_t sa = (int16_t)(steerAngleActual * 100);

                helloFromAutoSteer[5] = (uint8_t)sa;
                helloFromAutoSteer[6] = sa >> 8;

                helloFromAutoSteer[7] = (uint8_t)helloSteerPosition;
                helloFromAutoSteer[8] = helloSteerPosition >> 8;
                helloFromAutoSteer[9] = switchByte;

                //off to AOG
                UDPsteering.beginPacket(SteerSendIP, AGIOport);
                UDPsteering.write(helloFromAutoSteer, sizeof(helloFromAutoSteer));
                UDPsteering.endPacket();

                CommTime = millis();
            }

            else if (data[3] == 201)
            {
                // update IP
                if (data[4] == 5 && data[5] == 201 && data[6] == 201)
                {
                    PCB.IPpart3 = data[9];
                    EEPROM.put(110, PCB);

                    //reset the Teensy
                    SCB_AIRCR = 0x05FA0004;
                }
            }

            else if (data[3] == 202)
            {
                // ID scan request
                if (data[4] == 3 && data[5] == 202 && data[6] == 202)
                {
                    uint8_t scanReply[] = { 128, 129, 126, 203, 4, 192, 168, PCB.IPpart3, 126, 23 };

                    //checksum
                    int16_t CK_A = 0;
                    for (uint8_t i = 2; i < sizeof(scanReply) - 1; i++)
                    {
                        CK_A = (CK_A + scanReply[i]);
                    }
                    scanReply[sizeof(scanReply) - 1] = CK_A;

                    static uint8_t ipDest[] = { 255,255,255,255 };
                    uint16_t portDest = 9999; //AOG port that listens

                    //off to AOG
                    UDPsteering.beginPacket(ipDest, portDest);
                    UDPsteering.write(scanReply, sizeof(scanReply));
                    UDPsteering.endPacket();
                }
            }
        }
    }
}

void SendSteerUDP()
{
    // Steer Data 1
    // steer angle
    int16_t tmp = (int)(steerAngleActual * 100);
    PGN_253[5] = (byte)tmp;
    PGN_253[6] = tmp >> 8;

    // heading
    if (PCB.Receiver == 0 && PCB.IMU != 0)
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
    if (PCB.Receiver == 0 && PCB.IMU != 0)
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

    //off to AOG
    UDPsteering.beginPacket(SteerSendIP, AGIOport);
    UDPsteering.write(PGN_253, sizeof(PGN_253));
    UDPsteering.endPacket();

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

        //off to AOG
        UDPsteering.beginPacket(SteerSendIP, AGIOport);
        UDPsteering.write(PGN_250, sizeof(PGN_250));
        UDPsteering.endPacket();
    }
}
