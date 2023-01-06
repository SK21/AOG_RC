namespace RateController
{
    public class PGN32620
    {
        // section switch IDs to arduino
        // ex: byte 2: bits 0-3 identify switch # (0-15) for sec 0
        // ex: byte 2: bits 4-7 identify switch # (0-15) for sec 1

        // 0    108
        // 1    127
        // 2    sec 0-1
        // 3    sec 2-3
        // 4    sec 4-5
        // 5    sec 6-7
        // 6    sec 8-9
        // 7    sec 10-11
        // 8    sec 12-13
        // 9    sec 14-15
        // 10   crc

        private const byte cByteCount = 11;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 108;
        private byte[] Data = new byte[cByteCount];
        private readonly FormStart mf;

        public PGN32620(FormStart CallingForm)
        {
            mf = CallingForm;
            Data[0] = HeaderHi;
            Data[1] = HeaderLo;
        }

        public void Send()
        {
            int ByteCount = 2;
            int BitCount = 0;
            for (int i = 0; i < 8; i++)
            {
                Data[i + 2] = 0;
            }

            foreach (clsSection Sec in mf.Sections.Items)
            {
                byte SwitchID = (byte)Sec.SwitchID;
                SwitchID = (byte)(SwitchID << BitCount);
                Data[ByteCount] |= SwitchID;
                BitCount += 4;

                if (BitCount > 4)
                {
                    ByteCount++;
                    BitCount = 0;
                }
            }
            Data[cByteCount - 1] = mf.Tls.CRC(Data, cByteCount - 1);

            mf.SendSerial(Data);
            mf.UDPmodules.SendUDPMessage(Data);
        }
    }
}