using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN35200
    {
        //PGN35200 to Rate Controller from Arduino			
        //0	HeaderHi		    137
        //1	HeaderLo		    128
        //2	rate applied Hi		100 X actual
        //3	rate applied Lo		100 X actual
        //4	acc.Quantity Hi		
        //5	acc.Quantity Lo		
        //6	error Hi		
        //7	error Lo

        private const byte cByteCount = 8;
        private const byte HeaderHi = 137;
        private const byte HeaderLo = 128;

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
                // negative number
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
