using System;

namespace RateController
{
    public class PGN32502
    {
        // PGN32502, PID from RC to module
        // 0    246
        // 1    126
        // 2    Mod/Sen ID     0-15/0-15
        // 3    KP 0    X 10000
        // 4    KP 1
        // 5    KP 2
        // 6    KP 3
        // 7    KI 0
        // 8    KI 1
        // 9    KI 2
        // 10   KI 3
        // 11   KD 0
        // 12   KD 1
        // 13   KD 2
        // 14   KD 3
        // 15   MinPWM
        // 16   MaxPWM
        // 17   -
        // 18   CRC

        private const byte cByteCount = 19;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 246;
        private readonly clsProduct Prod;
        private double cKD;
        private double cKI;
        private double cKP;
        private byte cMaxPWM;
        private byte cMinPWM;

        private UInt32 Temp;

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

            // KP
            Temp = (uint)(cKP * 10000);
            Data[3] = (byte)Temp;
            Data[4] = (byte)((UInt32)Temp >> 8);
            Data[5] = (byte)((UInt32)Temp >> 16);
            Data[6] = (byte)((UInt32)Temp >> 24);

            // KI
            Temp = (uint)(cKI * 10000);
            Data[7] = (byte)Temp;
            Data[8] = (byte)((UInt32)Temp >> 8);
            Data[9] = (byte)((UInt32)Temp >> 16);
            Data[10] = (byte)((UInt32)Temp >> 24);

            // KD
            Temp = (uint)(cKD * 10000);
            Data[11] = (byte)Temp;
            Data[12] = (byte)((UInt32)Temp >> 8);
            Data[13] = (byte)((UInt32)Temp >> 16);
            Data[14] = (byte)((UInt32)Temp >> 24);

            Data[15] = MinPWM;
            Data[16] = MaxPWM;

            Data[17] = 0;

            // CRC
            Data[18] = Prod.mf.Tls.CRC(Data, cByteCount - 1);

            if (Prod.mf.SimMode == SimType.VirtualNano)
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