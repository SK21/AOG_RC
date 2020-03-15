namespace RateController
{
    public class PGN32100
    {
        //PGN from AOG
        //0	header 0        125     0x7D
        //1	header 1        100     0x64
        //2	workedAreaTotal Hi      hectares X 100
        //3	workedAreaTotal Lo
        //4	ToolWorkingWidth Hi	- only sections that are on, actual X 100
        //5	ToolWorkingWidth Lo
        //6	Speed Hi    actual X 100
        //7	Speed Lo
        //8	mdSectionControlByteHi
        //9	mdSectionControlByteLo

        // total 10 bytes

        private const byte cByteCount = 10;
        private const byte HeaderHi = 125;
        private const byte HeaderLo = 100;

        public byte SectionControlByteHi;
        public byte SectionControlByteLo;

        private byte SpeedHi;
        private byte SpeedLo;
        private byte WidthHi;
        private byte WidthLo;
        private byte WorkedHi;
        private byte WorkedLo;

        private int Temp;

        public byte ByteCount()
        {
            return cByteCount;
        }

        public double Speed()
        {
            Temp = SpeedHi << 8 | SpeedLo;
            return Temp / 100.0;
        }

        public double WorkedArea()
        {
            Temp = WorkedHi << 8 | WorkedLo;
            return Temp / 100.0;
        }

        public double WorkingWidth()
        {
            Temp = WidthHi << 8 | WidthLo;
            return Temp / 100.0;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                WorkedHi = Data[2];
                WorkedLo = Data[3];
                WidthHi = Data[4];
                WidthLo = Data[5];
                SpeedHi = Data[6];
                SpeedLo = Data[7];
                SectionControlByteHi = Data[8];
                SectionControlByteLo = Data[9];

                Result = true;
            }
            return Result;
        }
    }
}