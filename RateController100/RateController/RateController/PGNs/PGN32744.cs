using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    class PGN32744
    {
        // to Arduino from Rate Controller
        // 0 HeaderHi       127
        // 1 HeaderLo       232
        // 2 VCN Hi         Valve Cal Number
        // 3 VCN Lo
        // 4 Send Hi        ms sending pwm
        // 5 Send Lo
        // 6 Wait Hi        ms to wait before sending pwm again
        // 7 Wait Lo
        // 8 Max PWM
        // 9 Min PWM

        public int VCN;
        public int SendTime;
        public int WaitTime;
        public byte MaxPWM;
        public byte MinPWM;
        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 232;
        private readonly CRateCals RC;

        public PGN32744(CRateCals CalledFrom)
        {
            RC = CalledFrom;
            VCN = 743;
            SendTime = 200;
            WaitTime = 750;
            MaxPWM = 255;
            MinPWM = 145;
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = (byte)(VCN >> 8);
            Vals[3] = (byte)VCN;
            Vals[4] = (byte)(SendTime >> 8);
            Vals[5] = (byte)SendTime;
            Vals[6] = (byte)(WaitTime >> 8);
            Vals[7] = (byte)WaitTime;
            Vals[8] = MaxPWM;
            Vals[9] = MinPWM;

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
                RC.mf.UDPnetwork.SendUDPMessage(Data());

                for (int i = 0; i < 4; i++)
                {
                    RC.mf.SER[i].SendtoRC(Data());
                }
            }
        }

    }
}
