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
        // 2    Mod/Sen ID     0-15/0-15
        // 3    KP
        // 4    MinPWM
        // 5    LowMax
        // 6    HighMax
        // 7    Deadband
        // 8    BrakePoint
        // 9    Timed Adjustment    0-disabled

        public byte KP;
        public byte MinPWM;
        public byte LowMax;

        public byte HighMax;
        public byte DeadBand;
        public byte BrakePoint;
        public byte TimedAdjustment;

        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 104;
        private readonly clsProduct Prod;

        public PGN32616(clsProduct CalledFrom)
        {
            Prod = CalledFrom;

            KP = 50;
            MinPWM = 30;
            LowMax = 75;
            HighMax = 100;
            DeadBand = 4;
            BrakePoint = 20;
            TimedAdjustment = 0;
        }

        public void Send()
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = HeaderHi;
            Data[1] = HeaderLo;
            Data[2] = Prod.mf.Tls.BuildModSenID(Prod.ModuleID, Prod.SensorID);
            Data[3] = (byte)(255 * KP / 100);
            Data[4] = (byte)(255 * MinPWM / 100);
            Data[5] = (byte)(255 * LowMax / 100);
            Data[6] = (byte)(255 * HighMax / 100);
            Data[7] = DeadBand;
            Data[8] = BrakePoint;
            Data[9] = TimedAdjustment;

            if (Prod.SimulationType == SimType.VirtualNano)
            {
                Prod.VirtualNano.ReceiveSerial(Data);
            }
            else
            {
                Prod.mf.SendSerial(Data);
                Prod.mf.UDPnetwork.SendUDPMessage(Data);
            }
        }
    }
}
