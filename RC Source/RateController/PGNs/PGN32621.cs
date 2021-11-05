using System;

namespace RateController
{
    public class PGN32621
    {
        // pressure sensor data from arduino
        // 0    127
        // 1    109
        // 2    Module ID
        // 3    sensor 0 Hi
        // 4    sensor 0 Lo
        // 5    sensor 1 Hi
        // 6    sensor 1 Lo
        // 7    sensor 2 Hi
        // 8    sensor 2 Lo
        // 9    sensor 3 Hi
        // 10   sensor 3 Lo

        private const byte cByteCount = 11;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 109;

        private FormStart mf;

        private Int16[,] cPressure = new short[255, 4]; // modules, pressures
        private int Temp;
        private byte HiByte;
        private byte LoByte;
        private byte ModuleID;

        public PGN32621(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                ModuleID = Data[3];
                for (int i = 0; i < 4; i++)
                {
                    cPressure[ModuleID, i] = (Int16)(Data[i * 2 + 3] << 8 | Data[i * 2 + 4]);
                }
                Result = true;
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            if (Data.Length >= cByteCount)
            {
                int.TryParse(Data[0], out Temp);

                if (Temp == HeaderHi)
                {
                    int.TryParse(Data[1], out Temp);
                    if (Temp == HeaderLo)
                    {
                        int.TryParse(Data[2], out Temp);
                        ModuleID = (byte)Temp;
                        for (int i = 0; i < 4; i++)
                        {
                            byte.TryParse(Data[i * 2 + 3], out HiByte);
                            byte.TryParse(Data[i * 2 + 4], out LoByte);
                            cPressure[ModuleID, i] = (Int16)(HiByte << 8 | LoByte);
                        }
                        Result = true;
                    }
                }
            }
            return Result;
        }

        public Int16 Pressure(byte ModuleID, byte SensorID)
        {
            if (SensorID < 4 & ModuleID < 255)
            {
                return cPressure[ModuleID, SensorID];
            }
            else
            {
                return 0;
            }
        }
    }
}