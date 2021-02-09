using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    class PGN32616
    {
        // PID to Arduino from RateController
        // 0    127
        // 1    104
        // 2    sensor ID
        // 3    KP
        // 4    MinPWM
        // 5    LowMax
        // 6    HighMax
        // 7    Deadband
        // 8    BrakePoint
        // 9    -

        public byte KP;
        public byte MinPWM;
        public byte LowMax;

        public byte HighMax;
        public byte DeadBand;
        public byte BrakePoint;

        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 104;
        private readonly CRateCals RC;

        private byte cProdID;
        private byte cConID;

        public PGN32616(CRateCals CalledFrom, byte ProductID)
        {
            RC = CalledFrom;
            cProdID = ProductID;

            KP = 20;
            MinPWM = 50;
            LowMax = 150;
            HighMax = 255;
            DeadBand = 3;
            BrakePoint = 2;
        }

        public byte ControllerID { get { return cConID; } set { cConID = value; } }

        public void Send()
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = HeaderHi;
            Data[1] = HeaderLo;
            Data[2] = cConID;
            Data[3] = KP;
            Data[4] = MinPWM;
            Data[5] = LowMax;
            Data[6] = HighMax;
            Data[7] = DeadBand;
            Data[8] = BrakePoint;

            if (RC.SimulationType == SimType.VirtualNano)
            {
                RC.Nano.ReceiveSerial(Data);
            }
            else
            {
                RC.mf.SER[cProdID].SendToArduino(Data);
                RC.mf.UDPnetwork.SendUDPMessage(Data);
            }
        }
    }
}
