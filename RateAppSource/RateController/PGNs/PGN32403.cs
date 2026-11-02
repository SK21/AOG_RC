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
        private DateTime[] cDupLogTime;     // DIAGNOSTIC (duplicate-ID test)
        private bool[] cLabelFromModule;

        public PGN32403()
        {
            cBoardLabel = new string[Props.MaxModules];
            cReceiveTime = new DateTime[Props.MaxModules];
            cDuplicateTime = new DateTime[Props.MaxModules];
            cDupLogTime = new DateTime[Props.MaxModules];
            cLabelFromModule = new bool[Props.MaxModules];
            for (int i = 0; i < Props.MaxModules; i++) cBoardLabel[i] = "";
        }

        public string BoardLabel(int Module)
        {
            string Result = "";
            if (Module >= 0 && Module < Props.MaxModules) Result = cBoardLabel[Module];
            return Result;
        }

        // Called after the app sends a new label (PGN 32506): the module's next report will
        // legitimately differ from the last one, so it must not be compared (no false
        // duplicate alarm). Nothing is cached - only what the module reports is shown.
        public void ExpectLabelChange(int Module)
        {
            if (Module >= 0 && Module < Props.MaxModules)
            {
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
                        // report cycle. The first report after the app sends a new
                        // label is not compared (ExpectLabelChange), so a rename is
                        // not a false alarm. Empty
                        // labels are skipped - old firmware without the MAC fallback
                        // reports blank and two blanks are indistinguishable.
                        if (cLabelFromModule[ModuleID] && label.Length > 0
                            && cBoardLabel[ModuleID].Length > 0 && label != cBoardLabel[ModuleID]
                            && (DateTime.Now - cReceiveTime[ModuleID]).TotalSeconds < 6)
                        {
                            cDuplicateTime[ModuleID] = DateTime.Now;

                            // DIAGNOSTIC (duplicate-ID test): what conflicted, max once per 5 s per module
                            if ((DateTime.Now - cDupLogTime[ModuleID]).TotalSeconds > 5)
                            {
                                cDupLogTime[ModuleID] = DateTime.Now;
                                Props.WriteActivityLog("DupID 32403: module " + ModuleID.ToString()
                                    + " label '" + cBoardLabel[ModuleID] + "' -> '" + label + "', "
                                    + (DateTime.Now - cReceiveTime[ModuleID]).TotalMilliseconds.ToString("N0") + " ms apart", false, true);
                            }
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
