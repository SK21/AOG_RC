using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
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
        private double cLspeed;
        private double cRspeed;
        private bool[] cSection;
        private DateTime ReceiveTime;
        private bool[] SectionLast;

        public PGN229()
        {
            cSection = new bool[SectionCount];
            SectionLast = new bool[SectionCount];
            Core.AutoSteerPGN.SectionsChanged += AutoSteerPGN_SectionsChanged;
        }

        public event EventHandler SectionsChanged;

        public double Lspeed
        { get { return cLspeed; } }

        public double Rspeed
        { get { return cRspeed; } }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public void ParseByteData(byte[] Data)
        {
            if ((Data.Length > HeaderCount) && (Data.Length == Data[4] + HeaderCount + 1))
            {
                if (Core.Tls.GoodCRC(Data, 2))
                {
                    for (int i = 5; i < 13; i++)
                    {
                        for (int j = 0; j < 8; j++)
                        {
                            cSection[(i - 5) * 8 + j] = Core.Tls.BitRead(Data[i], j);
                        }
                    }
                    cLspeed = Data[13] / 10.0;
                    cRspeed = Data[14] / 10.0;
                    if (Changed()) SectionsChanged?.Invoke(this, EventArgs.Empty);

                    ReceiveTime = DateTime.Now;
                }
            }
        }

        public bool SectionIsOn(int ID)
        {
            byte Bit;
            bool Result = false;
            if (Connected())
            {
                if (ID < SectionCount) Result = cSection[ID];
            }
            else
            {
                //return section status from PGN254
                if (ID < 8)
                {
                    Bit = (byte)Math.Pow(2, ID);
                    Result = ((Core.AutoSteerPGN.RelayLo & Bit) == Bit);
                }
                else if (ID < 16)
                {
                    Bit = (byte)Math.Pow(2, ID - 8);
                    Result = ((Core.AutoSteerPGN.RelayHi & Bit) == Bit);
                }
            }
            return Result;
        }

        private void AutoSteerPGN_SectionsChanged(object sender, EventArgs e)
        {
            // re-raise PGN254 event
            if (!Connected()) SectionsChanged?.Invoke(this, EventArgs.Empty);
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
