namespace RateController
{
    public class PGN32741
    {
        // to Rate Controller from Arduino
        //0	HeaderHi		    127
        //1	HeaderLo		    229
        //2	rate applied Hi		100 X actual
        //3	rate applied Lo
        //4	acc.Quantity 3		100 X actual
        //5	acc.Quantity 2
        //6 acc.Quantity 1

        private const byte cByteCount = 7;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 229;
        private double cPWMsetting;
        private double NewRateWeight = 20;  // amount new UPM value changes the average (1-100);
        private byte pwmHI;
        private byte pwmLo;
        private byte QuantityB1;
        private byte QuantityB2;
        private byte QuantityB3;
        private double RateAve;
        private byte RateHi;
        private byte RateLo;
        private int Temp;
        private double Tmp;

        public double AccumulatedQuantity()
        {
            Temp = QuantityB3 << 16;
            Temp |= QuantityB2 << 8;
            Temp |= QuantityB1;
            return Temp / 100.0;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                RateHi = Data[2];
                RateLo = Data[3];

                QuantityB3 = Data[4];
                QuantityB2 = Data[5];
                QuantityB1 = Data[6];

                UpdateAve();

                pwmHI = Data[7];
                pwmLo = Data[8];
                cPWMsetting = 300.0 - ((pwmHI << 8 | pwmLo) / 10.0);

                Result = true;
            }
            return Result;
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
                        byte.TryParse(Data[4], out QuantityB3);
                        byte.TryParse(Data[5], out QuantityB2);
                        byte.TryParse(Data[6], out QuantityB1);

                        UpdateAve();

                        // pwmSetting
                        byte.TryParse(Data[7], out pwmHI);
                        byte.TryParse(Data[8], out pwmLo);
                        cPWMsetting = 300.0 - ((pwmHI << 8 | pwmLo) / 10.0);

                        Result = true;
                    }
                }
            }
            return Result;
        }

        public double PWMsetting()
        {
            return cPWMsetting;
        }

        public double UPM()
        {
            Temp = RateHi << 8;
            Temp |= RateLo;
            return Temp / 100.0;
        }

        public double UPMaverage()
        {
            return RateAve;
        }

        private void UpdateAve()
        {
            Tmp = (RateHi << 8 | RateLo) / 100.0;
            RateAve = (RateAve * ((100.0 - NewRateWeight) / 100.0)) + (Tmp * (NewRateWeight / 100.0));
        }
    }
}