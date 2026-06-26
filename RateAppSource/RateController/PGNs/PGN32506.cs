using RateController.Classes;
using System;

namespace RateController.PGNs
{
    public class PGN32506
    {
        // PGN32506, Board ID label from RC to module (stored in module EEPROM, survives reflash)
        //0     HeaderLo    250
        //1     HeaderHi    126
        //2     ModuleID    0-7
        //3-18  16 chars    board label, ASCII, 0-padded
        //19    CRC

        public const int TextLength = 16;

        private const byte cByteCount = 20;
        private byte[] cData = new byte[cByteCount];

        public void Send(int moduleID, string description)
        {
            if (moduleID >= 0 && moduleID < Props.MaxModules)
            {
                if (description == null) description = "";

                Array.Clear(cData, 0, cByteCount);
                cData[0] = 250;
                cData[1] = 126;
                cData[2] = (byte)moduleID;

                // up to 16 printable-ASCII chars, 0-padded; longer text is truncated
                for (int i = 0; i < TextLength && i < description.Length; i++)
                {
                    char ch = description[i];
                    cData[3 + i] = (ch >= 32 && ch < 127) ? (byte)ch : (byte)0;
                }

                cData[cByteCount - 1] = Core.Tls.CRC(cData, cByteCount - 1);

                // UDP only for now - the firmware board-label path (PGN 32403/32506) is UDP only.
                // CAN support needs a CanFrameTranslator mapping + firmware CANBus handling (TODO).
                Core.UDPmodules.Send(cData);
            }
        }
    }
}
