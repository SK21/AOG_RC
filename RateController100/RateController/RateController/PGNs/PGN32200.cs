using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32200
    {
        //PGN from AOG
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

        public byte AutoButtonState;

        private byte AutoHi;
        private byte AutoLo;
        private byte OnHi;
        private byte OnLo;

        private byte[] PO2 = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 }; // powers of 2

        private byte Temp;
        private byte State;

        public byte ByteCount()
        {
            return cByteCount;
        }

        public byte SectionButtonState(byte ButtonNum)
        {
            State = Properties.Settings.Default.BtnOff;
            if (ButtonNum < 17)
            {
                if (ButtonNum > 7)
                {
                    // sections 8-15
                    ButtonNum -= 8;
                    Temp = PO2[ButtonNum];
                    if ((AutoHi & Temp) == Temp)
                    {
                        State = Properties.Settings.Default.BtnAuto;
                    }
                    else if ((OnHi & Temp) == Temp)
                    {
                        State = Properties.Settings.Default.BtnOn;
                    }
                }
                else
                {
                    // sections 0-7
                    Temp = PO2[ButtonNum];
                    if ((AutoLo & Temp) == Temp)
                    {
                        State = Properties.Settings.Default.BtnAuto;
                    }
                    else if ((OnLo & Temp) == Temp)
                    {
                        State = Properties.Settings.Default.BtnOn;
                    }
                }
            }
            return State;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                AutoHi = Data[2];
                AutoLo = Data[3];
                OnHi = Data[4];
                OnLo = Data[5];
                AutoButtonState = Data[6];

                Result = true;
            }
            return Result;
        }
    }
}
