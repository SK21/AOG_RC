using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32617
    {
        // switch control from RateController to Arduino
        // which switch (0-3) controls the section

        // byte 0 127
        // byte 1 105

        // byte 2, section 0, bits 0-3
        // byte 2, section 1, bits 4-7
        // byte 3, section 2, bits 0-3
        // byte 3, section 3, bits 4-7
        // byte 4, section 4, bits 0-3
        // byte 4, section 5, bits 4-7
        // byte 5, section 6, bits 0-3
        // btye 5, section 7, bits 4-7

        // byte 6, section 8, bits 0-3
        // byte 6, section 9, bits 4-7
        // byte 7, section 10, bits 0-3
        // byte 7, section 11, bits 4-7
        // byte 8, section 12, bits 0-3
        // byte 8, section 13, bits 4-7
        // byte 9, section 14, bits 0-3
        // btye 9, section 15, bits 4-7

        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 105;
        private readonly FormStart mf;

        byte[] Data;
        byte SwByte;

        public PGN32617(FormStart MainForm)
        {
            mf = MainForm;
            Data = new byte[cByteCount];

            Data[0] = HeaderHi;
            Data[1] = HeaderLo;

            for (int i = 0; i < 8; i++)
            {
                Data[i + 2] = 17;   // set default for each section to switch 1
            }
        }

        private void SetSwitchID(byte SectionID, byte SwitchID)
        {
            if (SwitchID > 3) throw new IndexOutOfRangeException();
            if (SectionID > 15) throw new IndexOutOfRangeException();

            byte ByteID = (byte)(SectionID / 2 + 2);

            if (SectionID % 2 == 0) // modulus
            {
                // even numbered section, bits 0-3
                SwByte = (byte)(1 << SwitchID);
                Data[ByteID] &= 240;    // clear bottom bits
                Data[ByteID] |= SwByte; // set new bottom bits
            }
            else
            {
                // odd numbered section, bits 4-7
                SwByte = (byte)(1 << (SwitchID + 4));
                Data[ByteID] &= 15; // clear top bits
                Data[ByteID] |= SwByte; // set new top bits
            }
        }

        public void Send()
        {
            for (int i = 0; i < 16; i++)
            {
                SetSwitchID((byte)i, (byte)mf.Sections.Item(i).SwitchID);
            }

            mf.UDPnetwork.SendUDPMessage(Data);

            for (int i = 0; i < 5; i++)
            {
                if (mf.RateCals[i].SimulationType == SimType.VirtualNano)
                {
                    mf.RateCals[i].Nano.ReceiveSerial(Data);
                }
                else
                {
                    mf.SER[i].SendToArduino(Data);
                }
            }
        }
    }
}
