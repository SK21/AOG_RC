using RateController.Classes;

namespace RateController.PGNs
{
    public class PGN32508
    {
        //PGN32508, assign module ID by MAC, RC to module
        //0     HeaderLo    252
        //1     HeaderHi    126
        //2-7   MAC         the board that takes the ID; every other board ignores it
        //8     new module ID 0-7
        //9-11  network a.b.c, 0.0.0 = the board keeps its network
        //12    CRC
        // The board saves the ID (and the network) and restarts after about 1.5 s, so the
        // rest of the same Send (32506, 32507, 32700) reaches it at the new ID. Over CAN it
        // goes as 0xFF19, without the network. An ESP32 applies the network only over Ethernet.

        private const byte cByteCount = 13;

        // Network: "a.b.c" (or "a.b.c.d", the first three used) to move the board, null to keep it
        public void Send(ulong MAC, byte NewID, string Network = null)
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = 252;
            Data[1] = 126;
            for (int i = 0; i < 6; i++) Data[2 + i] = (byte)(MAC >> (40 - 8 * i));
            Data[8] = NewID;
            if (!string.IsNullOrEmpty(Network))
            {
                string[] B = Network.Split('.');
                if (B.Length >= 3 && byte.TryParse(B[0], out byte b0) && byte.TryParse(B[1], out byte b1)
                    && byte.TryParse(B[2], out byte b2))
                {
                    Data[9] = b0;
                    Data[10] = b1;
                    Data[11] = b2;
                }
            }
            Data[12] = Core.Tls.CRC(Data, cByteCount - 1);

            if (Props.CanEnabled) Core.CanBridgeComm?.SendModuleCommand(Data);
            Core.UDPmodules.Send(Data);
            Core.LastIDAssign = System.DateTime.Now;   // holds off the new board setup prompt
        }
    }
}
