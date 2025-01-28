namespace RateController
{
    public class PGN234
    {
        // to AOG from Rate Controller
        // 0    HeaderHi    128
        // 1    HeaderLo    129
        // 2    source
        // 3    AGIO PGN    234 ss 0
        // 4    length
        // 5    swMain          ss 1
        //	    - bit 0		auto button on
        //	    - bit 1		auto button off
        //      - bits 2/3  rate change steps
        //      - bit 4     0-left, 1-right
        //      - bit 5     0-rate down, 1-rate up
        // 6    swReserve       ss 2
        // 7    swReserve2      ss 3
        // 8    swNumSections   ss 4
        // 9    swOnGr0         ss 5
        // 10   swOffGr0        ss 6
        // 11   swOnGr1         ss 7
        // 12   swOffGr1        ss 8
        // 13   CRC

        private byte[] cData = new byte[14];
        private FormStart mf;

        public PGN234(FormStart CalledFrom)
        {
            mf = CalledFrom;

            cData[0] = 128;
            cData[1] = 129;
            cData[3] = 234;
            cData[4] = 8;
        }

        public byte Command
        { get { return cData[5]; } set { cData[5] = value; } }

        public byte OffHi
        { get { return cData[12]; } set { cData[12] = value; } }
        public byte OffLo
        { get { return cData[10]; } set { cData[10] = value; } }
        public byte OnHi
        { get { return cData[11]; } set { cData[11] = value; } }
        public byte OnLo
        { get { return cData[9]; } set { cData[9] = value; } }

        public void Send()
        {
            cData[13] = mf.Tls.CRC(cData, cData.Length - 1, 2);
            mf.UDPaog.SendUDPMessage(cData);
        }
    }
}