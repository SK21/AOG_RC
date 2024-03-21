using System;

namespace RateController
{
    public class PGN32502
    {
        // PGN32502, PID from RC to module
        // 0    246
        // 1    126
        // 2    Mod/Sen ID     0-15/0-15
        // 3    KP 
        // 4    KI
        // 5    KD
        // 6   MinPWM
        // 7   MaxPWM
        // 8   PID scale
        // 9   CRC

        private const byte cByteCount = 10;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 246;
        private readonly clsProduct Prod;
        private double cKD;
        private double cKI;
        private double cKP;
        private byte cMaxPWM;
        private byte cMinPWM;

        public PGN32502(clsProduct CalledFrom)
        {
            Prod = CalledFrom;

            cMinPWM = 0;
            cMaxPWM = 0;
            cKP = 1;
            cKI = 0;
            cKD = 0;
        }

        public double KD
        {
            get { return cKD; }
            set { cKD = value; }
        }

        public double KI
        {
            get { return cKI; }
            set { cKI = value; }
        }

        public double KP
        {
            get { return cKP; }
            set { cKP = value; }
        }

        public byte MaxPWM
        {
            get { return cMaxPWM; }
            set { cMaxPWM = value; }
        }

        public byte MinPWM
        {
            get { return cMinPWM; }
            set { cMinPWM = value; }
        }

        public void Send()
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = HeaderLo;
            Data[1] = HeaderHi;
            Data[2] = Prod.mf.Tls.BuildModSenID((byte)Prod.ModuleID, Prod.SensorID);

            Data[3] = (byte)cKP;
            Data[4] = (byte)cKI;
            Data[5] = (byte)cKD;

            Data[6] = MinPWM;
            Data[7] = MaxPWM;
            Data[8] = (byte)Prod.PIDscale;

            Data[9] = Prod.mf.Tls.CRC(Data, cByteCount - 1);

            Prod.mf.SendSerial(Data);
            Prod.mf.UDPmodules.SendUDPMessage(Data);
        }
    }
}