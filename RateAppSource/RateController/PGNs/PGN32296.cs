using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN32296
    {
        //PGN32296, scale indicator reading
        //0     40
        //1     126
        //2 -   reading + 13,10

        private const byte HeaderHi = 126;
        private const byte HeaderLo = 40;
        private readonly FormStart mf;
        private double cValue;

        public PGN32296(FormStart CalledFrom)
        {
            mf = CalledFrom;
        }

        public double Value
        { get { return cValue; } }

        public bool ParseStringData(string Sentence)
        {
            bool Result = false;
            if (Sentence.Length > 5)
            {
                byte[] Data = Encoding.UTF8.GetBytes(Sentence);
                if (Data[0] == HeaderLo && Data[1] == HeaderHi)
                {
                    int End = Sentence.IndexOf('\r');
                    if (End > 1)
                    {
                        string Reading = Sentence.Substring(2, End - 2);
                        cValue = Convert.ToDouble(Reading);
                        Result = true;
                    }
                }
            }
            return Result;
        }
    }
}