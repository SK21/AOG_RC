namespace RateController
{
    class PGN31400
    {
        //  Comm data to arduino
        //  PGN 31400, (B)
        //	0. HeaderHi		122		0x7A
        //	1. HeaderLo		168		0xA8
        //  2. KP			P control gain, 10 times actual
        //  3. KI			I control gain, 10000 times actual
        //  4. KD			D control gain, 10 times actual
        //  5. Deadband		% error that can still be present when the motor stops, 10 times actual
        //  6. MinPWM			
        //  7. MaxPWM		
        //  total 8 bytes

        private const byte cByteCount = 8;
        private const byte HeaderHi = 122;
        private const byte HeaderLo = 168;

        public byte KP;
        public byte KI;
        public byte KD;
        public byte Deadband;
        public byte MinPWM;
        public byte MaxPWM;

        public PGN31400()
        {
            KP = 100;
            Deadband = 3;
            MinPWM = 50;
            MaxPWM = 255;
        }

        public byte ByteCount()
        {
            return cByteCount;
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = KP;
            Vals[3] = KI;
            Vals[4] = KD;
            Vals[5] = Deadband;
            Vals[6] = MinPWM;
            Vals[7] = MaxPWM;
            return Vals;
        }
    }
}
