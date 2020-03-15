using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgOpenGPS
{
    public class PGN32200
    {
        //PGN out
        //0	header 0        125     0x7D
        //1	header 1        200     0xC8
        //2	manBtnStateIsAuto Hi    whether manual section button is in auto state
        //3	manBtnStateIsAuto Lo
        //4	manBtnStateIsOn Hi
        //5	manBtnStateIsOn Lo
        //6	autoBtnState		0 - off, 1 - auto

        // infer manBtnState Off if not Auto and not On

        // total 7 bytes

        private const byte cByteCount = 7;
        private const byte HeaderHi = 125;
        private const byte HeaderLo = 200;

        private byte AutoHi;
        private byte AutoLo;
        private byte OnHi;
        private byte OnLo;
        private byte AutoButtonState;

        byte[] Data = new byte[10];
        int MaxSections;
        byte Bit;

        private byte[] PO2 = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }; // powers of 2

        private readonly FormGPS mf;

        public PGN32200(FormGPS CallingForm)
        {
            mf = CallingForm;
        }

        public void Send()
        {
            // buttons state
            AutoHi = 0;
            AutoLo = 0;
            OnHi = 0;
            OnLo = 0;
            for (int r = 0; r < 2; r++)
            {
                MaxSections = mf.tool.numOfSections;
                if (MaxSections > 7 + r * 8) MaxSections = 7 + r * 8;

                for (int i = 0 + r * 8; i < MaxSections; i++)
                {
                    Bit = PO2[i - r * 8];
                    if (r == 0)
                    {
                        // sections 0-7
                        if (mf.section[i].manBtnState == FormGPS.manBtn.Auto) AutoLo = (byte)(AutoLo | Bit);    // set bit for section
                        if (mf.section[i].manBtnState == FormGPS.manBtn.On) OnLo = (byte)(OnLo | Bit);
                    }
                    else
                    {
                        // sections 8-16
                        if (mf.section[i].manBtnState == FormGPS.manBtn.Auto) AutoHi = (byte)(AutoHi | Bit);    
                        if (mf.section[i].manBtnState == FormGPS.manBtn.On) OnHi = (byte)(OnHi | Bit);
                    }

                }
            }

            // auto
            AutoButtonState = 0;
            if (mf.autoBtnState == FormGPS.btnStates.Auto) AutoButtonState = 1;

            // fill array
            Data[0] = HeaderHi;
            Data[1] = HeaderLo;
            Data[2] = AutoHi;
            Data[3] = AutoLo;
            Data[4] = OnHi;
            Data[5] = OnLo;
            Data[6] = AutoButtonState;

            // send data
            mf.SendUDPMessage(Data);
        }
    }
}
