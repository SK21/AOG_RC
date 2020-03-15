using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace AgOpenGPS
{
    public class PGN32100
    {
        //PGN out
        //0	header 0        125     0x7D
        //1	header 1        100     0x64
        //2	workedAreaTotal Hi      actual X 10
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

        private byte SpeedHi;
        private byte SpeedLo;
        private byte WidthHi;
        private byte WidthLo;
        private byte WorkedHi;
        private byte WorkedLo;
        private byte SectionControlByteHi;
        private byte SectionControlByteLo;

        byte[] Data = new byte[10];
        int Temp;

        private readonly FormGPS mf;

        public PGN32100(FormGPS CallingForm)
        {
            mf = CallingForm;
        }

        public void Send()
        {
            // worked area
            Temp = (int)(mf.fd.workedAreaTotal / 100.0);    // square meters/100 = hectares * 100
            WorkedHi = (byte)(Temp >> 8);
            WorkedLo = (byte)Temp;
            Debug.WriteLine("worked area " + (Temp/100).ToString());    // hectares

            // working width
            // is supersection on?
            if (mf.section[mf.tool.numOfSections].isSectionOn)
            {
                Temp = (int)(mf.tool.toolWidth * 100.0);
            }
            //individual sections are possibly on
            else
            {
                for (int i = 0; i < mf.tool.numOfSections; i++)
                {
                    if (mf.section[i].isSectionOn) Temp += (int)(mf.section[i].sectionWidth * 100.0);
                }
            }
            WidthHi = (byte)(Temp >> 8);
            WidthLo = (byte)Temp;


            // speed
            Temp = (int)(mf.pn.speed * 100);
            SpeedHi = (byte)(Temp >> 8);
            SpeedLo = (byte)Temp;


            // relay bytes
            mf.BuildMachineByte();
            SectionControlByteHi = mf.mc.machineData[mf.mc.mdSectionControlByteHi];
            SectionControlByteLo = mf.mc.machineData[mf.mc.mdSectionControlByteLo];

            // fill array
            Data[0] = HeaderHi;
            Data[1] = HeaderLo;
            Data[2] = WorkedHi;
            Data[3] = WorkedLo;
            Data[4] = WidthHi;
            Data[5] = WidthLo;
            Data[6] = SpeedHi;
            Data[7] = SpeedLo;
            Data[8] = SectionControlByteHi;
            Data[9] = SectionControlByteLo;

            // send data
            mf.SendUDPMessage(Data);
        }
    }
}
