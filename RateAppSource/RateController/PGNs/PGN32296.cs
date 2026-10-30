using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32296
    {
        //PGN32296, scale indicator reading, from the scale module (Modules/ScaleIndicator)
        //0     40
        //1     126
        //2     product ID 0-3
        //3     weight Lo       signed 32 bit, hundredths of the indicator's units
        //4     weight Mid
        //5     weight Mid
        //6     weight Hi
        //7     units 0 unknown, 1 lb, 2 kg
        //8     InoID Lo
        //9     InoID Hi
        //10    CRC

        public const int MaxScales = 4;
        private const byte cByteCount = 11;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 40;
        private const double TimeoutSeconds = 4;
        private DateTime[] cReceiveTime = new DateTime[MaxScales];
        private byte[] cUnits = new byte[MaxScales];
        private double[] cValue = new double[MaxScales];

        public bool Connected(int ProductID)
        {
            bool Result = false;
            if (ProductID >= 0 && ProductID < MaxScales)
            {
                Result = (DateTime.Now - cReceiveTime[ProductID]).TotalSeconds < TimeoutSeconds;
            }
            return Result;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            try
            {
                if (Data.Length == cByteCount && Data[0] == HeaderLo && Data[1] == HeaderHi && Core.Tls.GoodCRC(Data))
                {
                    int ProductID = Data[2];
                    if (ProductID < MaxScales)
                    {
                        int Hundredths = Data[3] | Data[4] << 8 | Data[5] << 16 | Data[6] << 24;
                        cValue[ProductID] = Hundredths / 100.0;
                        cUnits[ProductID] = Data[7];
                        cReceiveTime[ProductID] = DateTime.Now;
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32296/ParseByteData: " + ex.Message);
            }
            return Result;
        }

        public string Units(int ProductID)
        {
            // as reported by the indicator, no conversion. Empty when the format has no units
            string Result = "";
            if (ProductID >= 0 && ProductID < MaxScales)
            {
                switch (cUnits[ProductID])
                {
                    case 1:
                        Result = "lb";
                        break;

                    case 2:
                        Result = "kg";
                        break;
                }
            }
            return Result;
        }

        public double Value(int ProductID)
        {
            double Result = 0;
            if (ProductID >= 0 && ProductID < MaxScales) Result = cValue[ProductID];
            return Result;
        }
    }
}
