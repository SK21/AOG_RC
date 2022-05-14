﻿namespace RateController
{
    public class PGN32616
    {
        // PID to Arduino from RateController
        // 0    104
        // 1    127
        // 2    Mod/Sen ID     0-15/0-15
        // 3    KP
        // 4    MinPWM
        // 5    LowMax
        // 6    HighMax
        // 7    Deadband
        // 8    BrakePoint
        // 9    Timed Adjustment    0-disabled
        // 10   CRC

        private const byte cByteCount = 11;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 104;
        private readonly clsProduct Prod;
        private byte cBrakePoint;
        private byte cDeadBand;
        private byte cHighMax;
        private byte cKP;
        private byte cLowMax;
        private byte cMinPWM;
        private byte cTimedAdjustment;

        public PGN32616(clsProduct CalledFrom)
        {
            Prod = CalledFrom;

            cKP = 50;
            cMinPWM = 30;
            cLowMax = 75;
            cHighMax = 100;
            cDeadBand = 4;
            cBrakePoint = 20;
            cTimedAdjustment = 0;
        }

        public byte BrakePoint
        {
            get { return cBrakePoint; }
            set
            {
                if (value <= 100) cBrakePoint = value;
            }
        }

        public byte DeadBand
        {
            get { return cDeadBand; }
            set
            {
                if (value <= 100) cDeadBand = value;
            }
        }

        public byte HighMax
        {
            get { return cHighMax; }
            set
            {
                if (value <= 100) cHighMax = value;
            }
        }

        public byte KP
        {
            get { return cKP; }
            set
            {
                if (value <= 100) cKP = value;
            }
        }

        public byte LowMax
        {
            get { return cLowMax; }
            set
            {
                if (value <= 100) cLowMax = value;
            }
        }

        public byte MinPWM
        {
            get { return cMinPWM; }
            set
            {
                if (value <= 100) cMinPWM = value;
            }
        }

        public byte TimedAdjustment
        {
            get { return cTimedAdjustment; }
            set
            {
                if (value >= 50)
                {
                    cTimedAdjustment = value;
                }
                else
                {
                    cTimedAdjustment = 0;
                }
            }
        }

        public void Send()
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = HeaderLo;
            Data[1] = HeaderHi;
            Data[2] = Prod.mf.Tls.BuildModSenID(Prod.ModuleID, Prod.SensorID);
            Data[3] = (byte)(255 * cKP / 100);
            Data[4] = (byte)(255 * cMinPWM / 100);
            Data[5] = (byte)(255 * cLowMax / 100);
            Data[6] = (byte)(255 * cHighMax / 100);
            Data[7] = cDeadBand;
            Data[8] = cBrakePoint;
            Data[9] = cTimedAdjustment;

            // CRC
            Data[10] = Prod.mf.Tls.CRC(Data, cByteCount - 1);

            if (Prod.SimulationType == SimType.VirtualNano)
            {
                Prod.VirtualNano.ReceiveSerial(Data);
            }
            else
            {
                Prod.mf.SendSerial(Data);
                Prod.mf.UDPmodules.SendUDPMessage(Data);
            }
        }
    }
}