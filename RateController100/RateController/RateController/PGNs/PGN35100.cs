using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN35100
    {
        //PGN35100 to Arduino from Rate Controller				
        //0	HeaderHi		137	
        //1	HeaderLo		28	
        //2	KP		        10 X actual	
        //3	KI		        10000 X actual	
        //4	KD		        10 X actual	
        //5	Deadband		% error allowed	
        //6	MinPWM			
        //7	MaxPWM
        //8 adjustment factor

        private const byte cByteCount = 9;
        private const byte HeaderHi = 137;
        private const byte HeaderLo = 28;

        public byte KP;
        public byte KI;
        public byte KD;
        public byte Deadband;
        public byte MinPWM;
        public byte MaxPWM;
        public byte AdjustmentFactor;

        private readonly CRateCals RC;

        public PGN35100(CRateCals CalledFrom)
        {
            RC = CalledFrom;
            KP = 100;
            Deadband = 3;
            MinPWM = 50;
            MaxPWM = 255;
            AdjustmentFactor = 100;
        }

        public byte ByteCount()
        {
            return cByteCount;
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = KP;
            Vals[3] = KI;
            Vals[4] = KD;
            Vals[5] = Deadband;
            Vals[6] = MinPWM;
            Vals[7] = MaxPWM;
            Vals[8] = AdjustmentFactor;

            return Vals;
        }

        public void Send()
        {
            if (RC.SimulationType == SimType.VirtualNano)
            {
                RC.Nano.ReceiveSerial(Data());
            }
            else
            {
                RC.mf.SER.SendtoRC(Data());
                RC.mf.UDPnetwork.SendUDPMessage(Data());
            }
        }

    }
}
