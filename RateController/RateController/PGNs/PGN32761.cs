using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32761
    {
        //to Rate Controller from arduino, to AOG from Rate Controller
        //0	HeaderHi		127
        //1	HeaderLo		249
        //2	-
        //3	-
        //4	-
        //5	SecOn Hi		8-15
        //6	SecOn Lo		0-7
        //7	SecOff Hi
        //8	SecOff Lo
        //9	Command
        //	    - bit 0		auto button on
        //	    - bit 1		auto button off

        public byte Command;
        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 249;

        private readonly CRateCals RC;
        private byte[] cData = new byte[10];
        private float RateCalcFactor = 1.03F;   // rate change amount for each step.  ex: 1.10 means 10% for each step
        private int Temp;

        public PGN32761(CRateCals CalledFrom)
        {
            RC = CalledFrom;
        }

        public double NewRate(double CurrentRate)
        {
            // rate change amount
            int RateSteps = 0;
            if ((Command & 4) == 4) RateSteps = 1;
            if ((Command & 8) == 8) RateSteps += 2;

            if (RateSteps > 0)
            {
                // rate direction
                bool RateUp = false;
                RateUp = (Command & 32) == 32;

                // change rate
                float ChangeAmount = 1;
                for (byte a = 1; a <= RateSteps; a++)
                {
                    ChangeAmount *= RateCalcFactor;
                }
                if (RateUp)
                {
                    CurrentRate = Math.Round(CurrentRate * ChangeAmount, 1);
                }
                else
                {
                    CurrentRate = Math.Round(CurrentRate / ChangeAmount, 1);
                }
            }
            return CurrentRate;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                for (int i = 0; i < cByteCount; i++)
                {
                    cData[i] = Data[i];
                }
                Command = cData[9];
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
                        for (int i = 0; i < cByteCount; i++)
                        {
                            byte.TryParse(Data[i], out cData[i]);
                        }
                        Command = cData[9];
                        Result = true;
                    }
                }
            }
            return Result;
        }

        public void Send()
        {
            RC.mf.UDPLoopBack.SendUDPMessage(cData);
        }
    }
}