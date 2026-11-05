using RateController.Classes;

namespace RateController.PGNs
{
    public class PGN32508
    {
        //PGN32508, assign module ID by MAC, RC to module
        //0     HeaderLo    252
        //1     HeaderHi    126
        //2-7   MAC         the board that takes the ID; every other board ignores it
        //8     new module ID 0-15
        //9     CRC
        // The board saves the ID and restarts. Its settings go with the next Send.

        private const byte cByteCount = 10;

        public void Send(ulong MAC, byte NewID)
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = 252;
            Data[1] = 126;
            for (int i = 0; i < 6; i++) Data[2 + i] = (byte)(MAC >> (40 - 8 * i));
            Data[8] = NewID;
            Data[9] = Core.Tls.CRC(Data, cByteCount - 1);

            if (Props.CanEnabled) Core.CanBridgeComm?.SendModuleCommand(Data);
            Core.UDPmodules.Send(Data);
        }
    }
}
