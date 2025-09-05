namespace RateController
{
    public class PGN32502
    {
        // PGN32502, Control settings from RC to module
        // 0    246
        // 1    126
        // 2    Mod/Sen ID     0-15/0-15
        // 3    Kp
        // 4    Ki
        // 5    MinAdjust
        // 6    MaxAdjust
        // 7    Deadband     %          actual X 10
        // 8    Brakepoint   %
        // 9    Slow Adjust  %
        // 10   Slew Rate
        // 11   Max Motor Integral      actual X 100
        // 12   Max Valve Integral
        // 13   Min Start
        // 14   Adjust time Lo
        // 15   Adjust time Hi
        // 16   Pause time Lo
        // 17   Pause time Hi
        // 18   Sample time
        // 19   CRC

        private const byte cByteCount = 20;
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

            Data[3] = (byte)Prod.KP;
            Data[4] = (byte)Prod.KI;
            Data[5] = (byte)Prod.MinPWMadjust;
            Data[6] = (byte)Prod.MaxPWMadjust;
            Data[7] = 0;
            Data[8] = 0;

            Data[19] = Prod.mf.Tls.CRC(Data, cByteCount - 1);

            Prod.mf.UDPmodules.SendUDPMessage(Data);
        }
    }
}