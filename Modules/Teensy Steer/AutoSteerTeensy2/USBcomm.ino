
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
            //0     HeaderLo    110
            //1     HeaderHi    127
            //2     Receiver    0-None, 1-SimpleRTK2B, 2-Sparkfun F9P
            //3     NMEA serial port, 1-8
            //4     RTCM serial port, 1-8
            //5     RTCM UDP port # lo
            //6     RTCM UDP port # Hi
            //7     IMU         0-None, 1-Sparkfun BNO, 2-CMPS14, 3-Adafruit BNO, 4 serial
            //8     Read Delay
            //9     Report Interval
            //10    Zero offset Lo
            //11    Zero offset Hi
            //12    RS485 Serial port, 1-8
            //13    IP address, 3rd octet
            //14    Wemos serial port
            //15    CRC

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
                    MDL.IP2 = DataUSB[13];
                    MDL.WemosSerialPort = DataUSB[14];

                    EEPROM.put(110, MDL);
                }
                PGNusb = 0;
                LSBusb = 0;
            }
            break;

        case 32623:
            // Teensy config 2
            //0     HeaderLo    111
            //1     HeaderHi    127
            //2     Minimum speed
            //3     Maximum speed
            //4     Speed pulse cal X 10 Lo
            //5     Speed pulse cal X 10 Hi
            //6     Analog method 0 ADS1115 (Teensy), 1 pins (Teensy), 2 ADS1115 (D1 Mini)
            //7     RelayControl 0 - no relays, 1 - RS485, 2 - PCA9555 8 relays, 3 - PCA9555 16 relays
            //8     Module ID
            //9     Commands
            //          - use rate control
            //          - use TB6612 motor controller
            //          - relay on high
            //          - flow on high
            //          - swap pitch for roll
            //          - invert roll
            //          - Gyro on
            //          - use linear actuator
            //10    CRC

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
            //0     HeaderLo    112
            //1     HeaderHi    127
            //2     Steer DIR
            //3     Steer PWM
            //4     Steer switch
            //5     Wheel angle sensor
            //6     Steer relay
            //7     Work switch
            //8     Current sensor
            //9     Pressure sensor
            //10    Encoder
            //11    Rate DIR
            //12    Rate PWM
            //13    Speed Pulse
            //14    RS485 send enable
            //15    CRC

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

                    EEPROM.put(110, MDL);

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
