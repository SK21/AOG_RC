using RateController.Classes;

namespace RateController.PGNs
{
    public class PGN32810
    {
        //PGN32810, firmware update control, RC to module
        //0     HeaderLo    42
        //1     HeaderHi    128
        //2     command     1 start, 3 commit, 4 abort
        //3-8   MAC         acted on only by the board with this MAC
        //9     InoType     expected board type, 0xFF = don't check
        //10    CRC
        // Over CAN it goes as 0xFF1A: [0] command, [1-6] MAC, [7] InoType.
        // Design: Design Docs\Teensy_CAN_Firmware_Update.md

        public const byte Start = 1;
        public const byte Commit = 3;
        public const byte Abort = 4;

        private const byte cByteCount = 11;

        // sent on the one link the update runs on, so a board on both links gets it once
        public void Send(byte Command, ulong MAC, byte InoType, bool OverCan)
        {
            byte[] Data = new byte[cByteCount];
            Data[0] = 42;
            Data[1] = 128;
            Data[2] = Command;
            for (int i = 0; i < 6; i++) Data[3 + i] = (byte)(MAC >> (40 - 8 * i));
            Data[9] = InoType;
            Data[10] = Core.Tls.CRC(Data, cByteCount - 1);

            if (OverCan)
            {
                Core.CanBridgeComm?.SendModuleCommand(Data);
            }
            else
            {
                Core.UDPmodules.Send(Data);
            }
        }
    }
}
