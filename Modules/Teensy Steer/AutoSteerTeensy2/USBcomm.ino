
byte DataUSB[MaxReadBuffer];
uint16_t PGNusb;
byte MSBusb;
byte LSBusb;

void ReceiveConfigData()
{
    if (Serial.available())
    {
        if (Serial.available() > MaxReadBuffer)
        {
            // clear buffer
            while (Serial.available())
            {
                Serial.available();
            }
            PGNusb = 0;
            LSBusb = 0;
        }

        switch (PGNusb)
        {
        case 32622:
            // teensy config 1
            PGNlength = 16;
            if (Serial.available() > PGNlength - 3)
            {
                DataUSB[0] = 110;
                DataUSB[1] = 127;
                for (int i = 2; i < PGNlength; i++)
                {
                    DataUSB[i] = Serial.read();
                }
                if (GoodCRC(DataUSB, PGNlength))
                {
                    MDL.Receiver = DataUSB[2];
                    MDL.SerialReceiveGPS = DataUSB[3];
                    MDL.SerialSendNtrip = DataUSB[4];
                    MDL.NtripPort = DataUSB[5] | DataUSB[6] << 8;
                    MDL.IMU = DataUSB[7];
                    MDL.IMUdelay = DataUSB[8];
                    MDL.IMU_Interval = DataUSB[9];
                    MDL.ZeroOffset = DataUSB[10] | DataUSB[11] << 8;
                    MDL.WemosSerialPort = DataUSB[14];

                    EEPROM.put(110, MDL);
                }
                PGNusb = 0;
                LSBusb = 0;
            }
            break;

        case 32623:
            // Teensy config 2
            PGNlength = 11;
            if (Serial.available() > PGNlength - 3)
            {
                DataUSB[0] = 111;
                DataUSB[1] = 127;
                for (int i = 2; i < PGNlength; i++)
                {
                    DataUSB[i] = Serial.read();
                }

                if (GoodCRC(DataUSB, PGNlength))
                {
                    MDL.MinSpeed = DataUSB[2];
                    MDL.MaxSpeed = DataUSB[3];
                    MDL.PulseCal = DataUSB[4] | DataUSB[5] << 8;
                    MDL.AnalogMethod = DataUSB[6];

                    uint8_t Commands = DataUSB[9];
                    if (bitRead(Commands, 1)) MDL.UseTB6612 = 1; else MDL.UseTB6612 = 0;
                    if (bitRead(Commands, 4)) MDL.SwapRollPitch = 1; else MDL.SwapRollPitch = 0;
                    if (bitRead(Commands, 5)) MDL.InvertRoll = 1; else MDL.InvertRoll = 0;
                    if (bitRead(Commands, 6)) MDL.GyroOn = 1; else MDL.GyroOn = 0;
                    if (bitRead(Commands, 7)) MDL.UseLinearActuator = 1; else MDL.UseLinearActuator = 0;

                    EEPROM.put(110, MDL);
                }
                PGNusb = 0;
                LSBusb = 0;
            }
            break;

        case 32624:
            // Teensy Pins, 16 bytes
            PGNlength = 16;
            if (Serial.available() > PGNlength - 3)
            {
                DataUSB[0] = 112;
                DataUSB[1] = 127;
                for (int i = 2; i < PGNlength; i++)
                {
                    DataUSB[i] = Serial.read();
                }

                if (GoodCRC(DataUSB, PGNlength))
                {
                    MDL.Dir1 = DataUSB[2];
                    MDL.PWM1 = DataUSB[3];
                    MDL.SteerSw = DataUSB[4];
                    MDL.SteerSw_Relay = DataUSB[6];
                    MDL.WorkSw = DataUSB[7];
                    MDL.CurrentSensor = DataUSB[8];
                    MDL.PressureSensor = DataUSB[9];
                    MDL.Encoder = DataUSB[10];
                    MDL.Dir2 = DataUSB[11];
                    MDL.PWM2 = DataUSB[12];
                    MDL.SpeedPulse = DataUSB[13];

                    EEPROM.put(150, MDL);

                    SCB_AIRCR = 0x05FA0004; //reset the Teensy   
                }
                PGNusb = 0;
                LSBusb = 0;
            }
            break;

        default:
            // find pgn
            MSBusb = Serial.read();
            PGNusb = MSBusb << 8 | LSBusb;
            Serial.println(PGNusb);
            LSBusb = MSBusb;
            break;
        }
    }

}
