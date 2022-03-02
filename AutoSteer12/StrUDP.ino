void SendSteerUDP()
{
    // steer angle
    int16_t tmp = (int)(steerAngleActual * 100);
    AOG[5] = (byte)tmp;
    AOG[6] = tmp >> 8;

    // heading
    if (ReceiverType == 0 && IMUtype != 0)
    {
        tmp = (int)(IMU_Heading);
    }
    else
    {
        tmp = 9999;
    }
    AOG[7] = (byte)tmp;
    AOG[8] = tmp >> 8;

    // roll
    if (ReceiverType == 0 && IMUtype != 0)
    {
        tmp = (int)(IMU_Roll);
    }
    else
    {
        tmp = 8888;
    }

    AOG[9] = (byte)tmp;
    AOG[10] = tmp >> 8;

    AOG[11] = switchByte;
    AOG[12] = pwmDrive;

    //add the checksum
    int16_t CK_A = 0;
    for (uint8_t i = 2; i < sizeof(AOG) - 1; i++)
    {
        CK_A = (CK_A + AOG[i]);
    }
    AOG[sizeof(AOG) - 1] = CK_A;

    //off to AOG
    UDPsteering.beginPacket(AGIOip, AGIOport);
    UDPsteering.write(AOG, sizeof(AOG));
    UDPsteering.endPacket();
}

void ReceiveSteerUDP()
{
    uint16_t len = UDPsteering.parsePacket();
    if (len > 13)
    {
        UDPsteering.read(data, UDP_TX_PACKET_MAX_SIZE);
        if ((data[0] == 0x80) && (data[1] == 0x81))
        {
            if (data[3] == 254)
            {
                // autosteer data
                Speed_KMH = (data[6] << 8 | data[5]) * 0.1;
                guidanceStatus = data[7];
                steerAngleSetPoint = (data[9] << 8 | data[8]) * 0.01;
                CommTime = millis();
            }

            else if (data[3] == 252)
            {
                // autosteer settings
                steerSettings.Kp = data[5];
                steerSettings.highPWM = data[6];
                steerSettings.lowPWM = data[7];
                steerSettings.minPWM = data[8];
                steerSettings.steerSensorCounts = data[9];
                steerSettings.wasOffset = data[10] | data[11] << 8;
                steerSettings.AckermanFix = data[12] * 0.01;

                EEPROM.put(10, steerSettings);
            }

            else if (data[3] == 251)
            {
                // steer config
                int8_t sett = data[5];

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

                //reset the arduino
                //resetFunc();
            }
        }
    }
}

