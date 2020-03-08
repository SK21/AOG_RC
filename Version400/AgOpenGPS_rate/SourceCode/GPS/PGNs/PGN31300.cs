namespace AgOpenGPS
{
    class PGN31300
    {
        //  Comm data from AgOpenGPs to arduino
        //  PGN 31300 (A)
        //	0. HeaderHi		122     0x7A
        //	1. HeaderLo		68      0x44
        //	2. Relay Hi		sections 8-15
        //  3. Relay Lo     sections 0-7
        //	4. Rate Set Hi	100 times actual rate
        //  5. Rate Set Lo  100 times actual rate
        //	6. Flow Cal	Hi	100 times actual
        //  7. Flow Cal Lo  100 times actual
        //	8. Command 		
        //			- bit 0 		reset accumulated quantity
        //			- bits 1 + 2 	0 (xxxxx00x) standard valve, 1 (xxxxx01x) fast close valve, 2 (xxxxx10x) valve type 2, 3 (xxxxx11x) valve type 3
        //			- bit 3			simulate flow
        // total 9 bytes

        private const byte cByteCount = 9;
        private const byte HeaderHi = 122;
        private const byte HeaderLo = 68;

        public byte RelayHi;
        public byte RelayLo;
        public byte RateSetHi;
        public byte RateSetLo;
        public byte FlowCalHi;
        public byte FlowCalLo;
        public byte Command;


        public byte ByteCount()
        {
            return cByteCount;
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = RelayHi;
            Vals[3] = RelayLo;
            Vals[4] = RateSetHi;
            Vals[5] = RateSetLo;
            Vals[6] = FlowCalHi;
            Vals[7] = FlowCalLo;
            Vals[8] = Command;
            return Vals;
        }
    }
}
