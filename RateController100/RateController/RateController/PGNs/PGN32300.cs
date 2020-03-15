using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32300
    {
        //PGN to AOG
        //0	header 0        126     0x7E
        //1	header 1         44     0x2C
        //2	autoBtnSetState	0 - no change, 1 - On, 2 - Off
        //3	manBtnSetOn Hi(16-9)
        //4	manBtnSetOn Lo(8-1)
        //5	manBtnSetAuto Hi
        //6	manBtnSetAuto Lo
        //7	manBtnSetOff Hi
        //8	manBtnSetOff Lo

        // total 9 bytes

        private const byte cByteCount = 9;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 44;

        public byte autoBtnSetState;

        public byte manBtnSetOnHi;
        public byte manBtnSetOnLo;
        private byte manBtnSetAutoHi;
        private byte manBtnSetAutoLo;
        private byte manBtnSetOffHi;
        private byte manBtnSetOffLo;

        private byte Temp;
        private byte[] PO2 = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }; // powers of 2

        public byte ByteCount()
        {
            return cByteCount;
        }

        public void SetSectionOn(byte SectionNum)
        {
            if (SectionNum<9)
            {
                Temp = PO2[SectionNum];
                manBtnSetOnLo = (byte)(manBtnSetOnLo | Temp);
            }
            else if(SectionNum<17)
            {
                SectionNum -= 8;
                Temp = PO2[SectionNum];
                manBtnSetOnHi = (byte)(manBtnSetOnHi | Temp);
            }
        }

        public void SetSectionOff(byte SectionNum)
        {
            if (SectionNum < 9)
            {
                Temp = PO2[SectionNum];
                manBtnSetOffLo = (byte)(manBtnSetOffLo | Temp);
            }
            else if (SectionNum < 17)
            {
                SectionNum -= 8;
                Temp = PO2[SectionNum];
                manBtnSetOffHi = (byte)(manBtnSetOffHi | Temp);
            }
        }

        public void SetSectionAuto(byte SectionNum)
        {
            if (SectionNum < 9)
            {
                Temp = PO2[SectionNum];
                manBtnSetAutoLo = (byte)(manBtnSetAutoLo | Temp);
            }
            else if (SectionNum < 17)
            {
                SectionNum -= 8;
                Temp = PO2[SectionNum];
                manBtnSetAutoHi = (byte)(manBtnSetAutoHi | Temp);
            }
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[10];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = autoBtnSetState;
            //autoBtnSetState = 0;
            Vals[3] = manBtnSetOnHi;
            Vals[4] = manBtnSetOnLo;
            Vals[5] = manBtnSetAutoHi;
            Vals[6] = manBtnSetAutoLo;
            Vals[7] = manBtnSetOffHi;
            Vals[8] = manBtnSetOffLo;
            Reset();
            return Vals;
        }

        public void Reset()
        {
            autoBtnSetState = 0;
            manBtnSetOnLo = 0;
            manBtnSetOnHi = 0;
            manBtnSetOffLo = 0;
            manBtnSetOffHi = 0;
            manBtnSetAutoLo = 0;
            manBtnSetAutoHi = 0;
        }
    }
}
