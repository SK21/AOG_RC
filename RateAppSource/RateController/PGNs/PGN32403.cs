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
        private DateTime[] cDuplicateTime;
        private bool[] cLabelFromModule;

        public PGN32403()
        {
            cBoardLabel = new string[Props.MaxModules];
            cReceiveTime = new DateTime[Props.MaxModules];
            cDuplicateTime = new DateTime[Props.MaxModules];
            cLabelFromModule = new bool[Props.MaxModules];
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
                cLabelFromModule[Module] = false;
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
                        string label = sb.ToString().Trim();

                        // Two boards sharing an ID reveal themselves by their reports
                        // flip-flopping between two different labels within the ~2 s
                        // report cycle. Only module reports are compared (not the
                        // optimistic SetLabel cache), so the old label read back right
                        // after the app sends a new one is not a false alarm. Empty
                        // labels are skipped - old firmware without the MAC fallback
                        // reports blank and two blanks are indistinguishable.
                        if (cLabelFromModule[ModuleID] && label.Length > 0
                            && cBoardLabel[ModuleID].Length > 0 && label != cBoardLabel[ModuleID]
                            && (DateTime.Now - cReceiveTime[ModuleID]).TotalSeconds < 6)
                        {
                            cDuplicateTime[ModuleID] = DateTime.Now;
                        }

                        cBoardLabel[ModuleID] = label;
                        cReceiveTime[ModuleID] = DateTime.Now;
                        cLabelFromModule[ModuleID] = true;
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

        // Latched for a while after the last conflicting report so the warning is
        // stable rather than flickering with the alternating reports; clears itself
        // once one of the boards is disconnected.
        public bool DuplicateID(int Module)
        {
            return (Module >= 0 && Module < Props.MaxModules
                    && (DateTime.Now - cDuplicateTime[Module]).TotalSeconds < 15);
        }

        public bool Received(int Module)
        {
            return (Module >= 0 && Module < Props.MaxModules
                    && (DateTime.Now - cReceiveTime[Module]).TotalSeconds < 6);
        }
    }
}
