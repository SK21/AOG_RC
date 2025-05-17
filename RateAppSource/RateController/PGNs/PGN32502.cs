namespace RateController
{
    public class PGN32502
    {
        // PGN32502, Control settings from RC to module
        // 0   246
        // 1   126
        // 2   Mod/Sen ID     0-15/0-15
        // 3   Ki
        // 4   -
        // 5   -
        // 6   MinAdjust
        // 7   MaxAdjust
        // 8   Scale Factor
        // 9   CRC

        private const byte cByteCount = 10;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 246;
        private readonly clsProduct Prod;

        public PGN32502(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public void Send()
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = HeaderLo;
            Data[1] = HeaderHi;
            Data[2] = Prod.mf.Tls.BuildModSenID((byte)Prod.ModuleID, Prod.SensorID);

            Data[3] = (byte)Prod.Integral;
            Data[4] = 0;
            Data[5] = 0;
            Data[6] = (byte)Prod.MinAdjust;
            Data[7] = (byte)Prod.MaxAdjust;
            Data[8] = (byte)Prod.ScalingFactor;
            Data[9] = Prod.mf.Tls.CRC(Data, cByteCount - 1);

            Prod.mf.UDPmodules.SendUDPMessage(Data);
        }
    }
}