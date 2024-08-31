using System;
using System.Text;

namespace RateController
{
    public class PGN32296
    {
        //PGN32296, scale indicator reading
        //0     40
        //1     126
        //2     product ID 0-4
        //3 -   reading + 13,10

        private const byte HeaderHi = 126;
        private const byte HeaderLo = 40;
        private readonly FormStart mf;
        private byte cProductID;
        private double[] cValue = new double[4];

        public PGN32296(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public bool ParseStringData(string Sentence)
        {
            bool Result = false;
            try
            {
                if (Sentence.Length > 6)
                {
                    byte[] Data = Encoding.UTF8.GetBytes(Sentence);
                    if (Data[0] == HeaderLo && Data[1] == HeaderHi)
                    {
                        int End = Sentence.IndexOf('\r');
                        if (End > 1)
                        {
                            cProductID = Convert.ToByte(Sentence.Substring(2, 1));
                            string Reading = Sentence.Substring(3, End - 3);
                            cValue[cProductID] = Convert.ToDouble(Reading);
                            Result = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("PGN32296/ParseStringData: " + ex.Message);
            }
            return Result;
        }

        public double Value(int ProductID)
        {
            return cValue[ProductID];
        }
    }
}