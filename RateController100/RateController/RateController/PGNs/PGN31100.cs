namespace RateController
{
    class PGN31100
    {
        //  Comm data from arduino 
        // PGN 31100 (A)
        //	0. HeaderHi			121		0x79
        //	1. HeaderLo			124		0x7C
        //	2. rate applied	Hi	100 times actual
        //  3. rate applied Lo	100 times actual
        //	4. acc.quantity	Hi	
        //  5. acc.quantity Lo
        //  6. error HiByte
        //  7. error LoByte

        // total 8 bytes
        // UDP port 6100

        private const byte cByteCount = 8;
        private const byte HeaderHi = 121;
        private const byte HeaderLo = 124;

        public byte RateHi;
        public byte RateLo;
        public byte QuantityHi;
        public byte QuantityLo;
        public byte ErrorHi;
        public byte ErrorLo;
        private byte ErrorHiTemp;

        private int Temp;
        private double Err;

        public double RateApplied()
        {
            Temp = RateHi << 8;
            Temp |= RateLo;
            return Temp / 100.0;
        }

        public double AccumulatedQuantity()
        {
            Temp = QuantityHi << 8;
            Temp |= QuantityLo;
            return Temp;
        }

        public double PercentError()
        {
            if ((ErrorHi & 0b10000000) == 0b10000000)
            {
                ErrorHiTemp = (byte)(ErrorHi & 0b01111111); // remove sign bit 8
                Err = (ErrorHiTemp << 8 | ErrorLo) * -1;
            }
            else
            {
                Err = ErrorHi << 8 | ErrorLo;
            }
            return Err / 100.0;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            if (Data.Length >= cByteCount)
            {
                int.TryParse(Data[0], out Temp);
                if (Temp == HeaderHi)
                {
                    int.TryParse(Data[1], out Temp);
                    if (Temp == HeaderLo)
                    {
                        // rate applied, 100 X actual
                        byte.TryParse(Data[2], out RateHi);
                        byte.TryParse(Data[3], out RateLo);

                        // accumulated quantity
                        byte.TryParse(Data[4], out QuantityHi);
                        byte.TryParse(Data[5], out QuantityLo);

                        // error, 100 X actual
                        byte.TryParse(Data[6], out ErrorHi);
                        byte.TryParse(Data[7], out ErrorLo);

                        Result = true;
                    }
                }
            }
            return Result;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                RateHi = Data[2];
                RateLo = Data[3];
                QuantityHi = Data[4];
                QuantityLo = Data[5];
                ErrorHi = Data[6];
                ErrorLo = Data[7];

                Result = true;
            }
            return Result;
        }
    }
}