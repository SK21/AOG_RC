using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
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
        private double[] cValue = new double[4];

        public bool ParseStringData(string Sentence)
        {
            byte cProductID;
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
                Props.WriteErrorLog("PGN32296/ParseStringData: " + ex.Message);
            }
            return Result;
        }

        public double Value(int ProductID)
        {
            return cValue[ProductID];
        }
    }
}
