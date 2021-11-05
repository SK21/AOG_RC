using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32613
    {
        // to Rate Controller from Arduino
        //0	HeaderHi		    127
        //1	HeaderLo		    101
        //2 Mod/Sen ID          0-15/0-15
        //3	rate applied Hi		10 X actual
        //4 rate applied Mid
        //5	rate applied Lo
        //6	acc.Quantity 3		100 X actual
        //7	acc.Quantity 2
        //8 acc.Quantity 1
        //9 PWM Hi              10 X actual
        //10 PWM Lo

        private const byte cByteCount = 11;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 101;
        private double cPWMsetting;
        private double cUPM;
        private double cQuantity;


        clsProduct Prod;

        private DateTime ReceiveTime;

        public PGN32613(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                int tmp = Prod.mf.Tls.ParseModID(Data[2]);
                if (Prod.ModuleID == tmp)
                {
                    tmp = Prod.mf.Tls.ParseSenID(Data[2]);
                    if (Prod.SensorID == tmp)
                    {
                        cUPM = (Data[3] << 16 | Data[4] << 8 | Data[5]) / 10.0;
                        cQuantity = (Data[6] << 16 | Data[7] | Data[8]) / 100.0;
                        cPWMsetting = (Int16)(Data[9] << 8 | Data[10]) / 10.0;  // need to cast to 16 bit integer to preserve the sign bit
                        ReceiveTime = DateTime.Now;
                        Result = true;
                    }
                }
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            byte pwmHI;
            byte pwmLo;
            byte QuantityB1;
            byte QuantityB2;
            byte QuantityB3;
            byte RateHi;
            byte RateMid;
            byte RateLo;
            int Temp;
            bool Result = false;

            if (Data.Length >= cByteCount)
            {
                int.TryParse(Data[0], out Temp);

                if (Temp == HeaderHi)
                {
                    int.TryParse(Data[1], out Temp);
                    if (Temp == HeaderLo)
                    {
                        int.TryParse(Data[2], out Temp);
                        int tmp = Prod.mf.Tls.ParseModID((byte)Temp);
                        if (Prod.ModuleID == tmp)
                        {
                            tmp = Prod.mf.Tls.ParseSenID((byte)Temp);
                            if (Prod.SensorID == tmp)
                            {
                                // rate applied, 10 X actual
                                byte.TryParse(Data[3], out RateHi);
                                byte.TryParse(Data[4], out RateMid);
                                byte.TryParse(Data[5], out RateLo);
                                cUPM = (RateHi << 16 | RateMid << 8 | RateLo) / 10.0;

                                // accumulated quantity
                                byte.TryParse(Data[6], out QuantityB3);
                                byte.TryParse(Data[7], out QuantityB2);
                                byte.TryParse(Data[8], out QuantityB1);
                                cQuantity = (QuantityB3 << 16 | QuantityB2 << 8 | QuantityB1) / 100.0;

                                // pwmSetting
                                byte.TryParse(Data[9], out pwmHI);
                                byte.TryParse(Data[10], out pwmLo);

                                cPWMsetting = (double)((Int16)(pwmHI << 8 | pwmLo)) / 10.0;

                                ReceiveTime = DateTime.Now;
                                Result = true;
                            }
                        }
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
            return cUPM;
        }

        public double AccumulatedQuantity()
        {
            return cQuantity;
        }

        public bool Connected()
        {
            if (Prod.SimulationType == SimType.VirtualNano)
            {
                return true;
            }
            else
            {
                return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
            }
        }
    }
}