using RateController.Classes;
using System;
using System.Text;

namespace RateController.PGNs
{
    public class PGN32403
    {
        //PGN32403, board ID label from module to RC (stored in the module's EEPROM)
        //0     HeaderLo    147
        //1     HeaderHi    126
        //2     ModuleID
        //3-18  16 chars    board label, ASCII, 0-padded
        //19    CRC

        private const byte cByteCount = 20;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 147;

        private string[] cBoardLabel;
        private DateTime[] cReceiveTime;

        public PGN32403()
        {
            cBoardLabel = new string[Props.MaxModules];
            cReceiveTime = new DateTime[Props.MaxModules];
            for (int i = 0; i < Props.MaxModules; i++) cBoardLabel[i] = "";
        }

        public string BoardLabel(int Module)
        {
            string Result = "";
            if (Module >= 0 && Module < Props.MaxModules) Result = cBoardLabel[Module];
            return Result;
        }

        // Optimistically set the cached label after the app sends a new one (PGN 32506), so the
        // read-back reflects it immediately instead of showing the stale value until the module
        // reports back. Normalized to match exactly what the module will store and report.
        public void SetLabel(int Module, string Label)
        {
            if (Module >= 0 && Module < Props.MaxModules)
            {
                if (Label == null) Label = "";
                StringBuilder sb = new StringBuilder(16);
                for (int i = 0; i < 16 && i < Label.Length; i++)
                {
                    char ch = Label[i];
                    if (ch < 32 || ch >= 127) break;   // sent as a 0 byte -> module truncates here
                    sb.Append(ch);
                }
                cBoardLabel[Module] = sb.ToString().Trim();
                cReceiveTime[Module] = DateTime.Now;
            }
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            try
            {
                if (Data[1] == HeaderHi && Data[0] == HeaderLo && Data.Length >= cByteCount && Core.Tls.GoodCRC(Data))
                {
                    byte ModuleID = Data[2];
                    if (ModuleID < Props.MaxModules)
                    {
                        StringBuilder sb = new StringBuilder(16);
                        for (int i = 0; i < 16; i++)
                        {
                            byte b = Data[3 + i];
                            if (b == 0) break;                          // 0-padded, stop at first null
                            if (b >= 32 && b < 127) sb.Append((char)b);  // printable ASCII only
                        }
                        cBoardLabel[ModuleID] = sb.ToString().Trim();
                        cReceiveTime[ModuleID] = DateTime.Now;
                        Result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("PGN32403/ParseByteData: " + ex.Message);
            }
            return Result;
        }

        public bool Received(int Module)
        {
            return (Module >= 0 && Module < Props.MaxModules
                    && (DateTime.Now - cReceiveTime[Module]).TotalSeconds < 6);
        }
    }
}
