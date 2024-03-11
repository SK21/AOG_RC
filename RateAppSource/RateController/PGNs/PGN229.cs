using System;

namespace RateController
{
    public class PGN229
    {
        // section data
        // 0    header Hi       128 0x80
        // 1    header Lo       129 0x81
        // 2    source          127 0x7F
        // 3    AGIO PGN        229 0xE5
        // 4    length          10
        // 5    sections 1-8
        // 6    9-16
        // 7    17-24
        // 8    25-32
        // 9    33-40
        // 10   41-48
        // 11   49-56
        // 12   57-64
        // 13   Lspeed  m/s *10
        // 14   Rspeed
        // 15   CRC

        public readonly int SectionCount = 64;
        private const byte HeaderCount = 5;
        private readonly FormStart mf;
        private double cLspeed;
        private double cRspeed;
        private bool[] cSection;
        private bool[] SectionLast;

        public PGN229(FormStart CalledFrom)
        {
            mf = CalledFrom;
            cSection = new bool[SectionCount];
            SectionLast = new bool[SectionCount];
        }

        public event EventHandler SectionsChanged;

        public double Lspeed
        { get { return cLspeed; } }

        public double Rspeed
        { get { return cRspeed; } }

        public void ParseByteData(byte[] Data)
        {
            if ((Data.Length > HeaderCount) && (Data.Length == Data[4] + HeaderCount + 1))
            {
                if (mf.Tls.GoodCRC(Data, 2))
                {
                    for (int i = 5; i < 13; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            cSection[(i - 5) * 8 + j] = mf.Tls.BitRead(Data[i], j);
                        }
                    }
                    cLspeed = Data[13] / 10.0;
                    cRspeed = Data[14] / 10.0;
                    if (Changed()) SectionsChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool SectionIsOn(int ID)
        {
            bool Result = false;
            if (ID < SectionCount) Result = cSection[ID];
            return Result;
        }

        private bool Changed()
        {
            bool Result = false;
            for (int i = 0; i < SectionCount; i++)
            {
                if (SectionLast[i] != cSection[i])
                {
                    Result = true;
                    break;
                }
            }

            if (Result)
            {
                for (int i = 0; i < SectionCount; i++)
                {
                    SectionLast[i] = cSection[i];
                }
            }
            return Result;
        }
    }
}