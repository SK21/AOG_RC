using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Text;

namespace RateController.PGNs
{
    // one physical board, identified by its MAC
    public class BoardInfo
    {
        public ulong MAC;
        public byte ID;             // 0-15, or Props.NewModuleID when unassigned
        public string Label = "";
        public byte InoType;
        public ushort InoID;        // ddmmy, no leading 0
        public DateTime LastHeard;

        public bool Live => (DateTime.Now - LastHeard).TotalSeconds < PGN32403.LiveSec;

        // last 3 bytes, e.g. "…0B:B0:12": enough to tell boards apart on screen
        public string ShortMAC => "…" + ((MAC >> 16) & 0xFF).ToString("X2") + ":"
            + ((MAC >> 8) & 0xFF).ToString("X2") + ":" + (MAC & 0xFF).ToString("X2");

        public string TypeName => (Enum.GetName(typeof(ModuleTypes), InoType) ?? "").Replace('_', ' ');

        // the board as the user sees it: type, end of the MAC, and its label when it has one
        public string Text => TypeName + " " + ShortMAC + (Label.Length > 0 ? " " + Label : "");
    }

    // a board change saved on the Boards page, sent with the next Send (Core.SetPending)
    public class PendingBoard
    {
        public ulong MAC;
        public int NewID = -1;              // -1 = keep the board's ID
        public string Description = null;   // null = keep the board's description
    }

    public class PGN32403
    {
        //PGN32403, identity from module to RC
        //0     HeaderLo    147
        //1     HeaderHi    126
        //2     module ID   0-15, or 25 unassigned
        //3-18  label       16 chars, ASCII, 0-padded
        //19-24 MAC         the board's base MAC
        //25    InoType
        //26    InoID lo
        //27    InoID hi
        //28    CRC
        // A shorter report (20 bytes, no MAC/type/firmware) is old firmware: rejected.

        private const byte cByteCount = 29;
        private const byte cOldByteCount = 20;
        private const byte HeaderHi = 126;
        private const byte HeaderLo = 147;

        public const double LiveSec = 5;    // 2 s report cycle, one missed report allowed

        // lowest firmware date accepted, yyyymmdd. 0 = any firmware that sends the
        // 29-byte report. Raise it to retire firmware in a later release.
        private const int MinInoDate = 0;

        // every board heard, keyed by MAC. Counted by MAC, not by packet: a PC on the
        // same LAN through two adapters receives each broadcast twice.
        private readonly Dictionary<ulong, BoardInfo> cBoards = new Dictionary<ulong, BoardInfo>();

        // last time old firmware reported at each ID (0-15 and 25)
        private readonly DateTime[] cRejectedTime = new DateTime[Props.NewModuleID + 1];

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            try
            {
                if (Data.Length >= cOldByteCount && Data[1] == HeaderHi && Data[0] == HeaderLo
                    && Core.Tls.GoodCRC(Data))
                {
                    byte ID = Data[2];
                    if (ID < Props.MaxModules || ID == Props.NewModuleID)
                    {
                        ushort inoID = 0;
                        if (Data.Length >= cByteCount) inoID = (ushort)(Data[26] | Data[27] << 8);

                        if (Data.Length < cByteCount || InoDate(inoID) < MinInoDate)
                        {
                            // old firmware
                            cRejectedTime[ID] = DateTime.Now;
                        }
                        else
                        {
                            ulong mac = 0;
                            for (int i = 0; i < 6; i++) mac = (mac << 8) | Data[19 + i];

                            if (!cBoards.TryGetValue(mac, out BoardInfo Brd))
                            {
                                Brd = new BoardInfo { MAC = mac };
                                cBoards[mac] = Brd;
                            }
                            Brd.ID = ID;            // a board that changes ID leaves its old ID at once
                            Brd.Label = ReadLabel(Data);
                            Brd.InoType = Data[25];
                            Brd.InoID = inoID;
                            Brd.LastHeard = DateTime.Now;
                        }
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

        // live boards at this ID, most recently heard first
        public List<BoardInfo> BoardsAt(int ID)
        {
            List<BoardInfo> Result = new List<BoardInfo>();
            foreach (BoardInfo Brd in cBoards.Values)
            {
                if (Brd.ID == ID && Brd.Live) Result.Add(Brd);
            }
            Result.Sort((a, b) => b.LastHeard.CompareTo(a.LastHeard));
            return Result;
        }

        // true when any live board reports this ID: Accepted() and the other yes/no checks,
        // which run many times a second, so no list is built or sorted
        public bool AnyLiveAt(int ID)
        {
            bool Result = false;
            foreach (BoardInfo Brd in cBoards.Values)
            {
                if (Brd.ID == ID && Brd.Live)
                {
                    Result = true;
                    break;
                }
            }
            return Result;
        }

        public List<BoardInfo> NewBoards() => BoardsAt(Props.NewModuleID);

        // the live board with this MAC, or null
        public BoardInfo LiveBoard(ulong MAC)
        {
            BoardInfo Result = null;
            if (cBoards.TryGetValue(MAC, out BoardInfo Brd) && Brd.Live) Result = Brd;
            return Result;
        }

        // old firmware reported at this ID in the last 5 s
        public bool Rejected(int ID)
        {
            return ID >= 0 && ID < cRejectedTime.Length
                && (DateTime.Now - cRejectedTime[ID]).TotalSeconds < LiveSec;
        }

        // a good board is live at this ID and no old firmware shares it (rejected wins)
        public bool Accepted(int ID) => !Rejected(ID) && AnyLiveAt(ID);

        // more than one board at this ID
        public bool DuplicateID(int Module)
        {
            return Module >= 0 && Module < Props.MaxModules && BoardsAt(Module).Count > 1;
        }

        // kept for current callers: a good board is reporting at this ID
        public bool Received(int Module)
        {
            return Module >= 0 && Module < Props.MaxModules && Accepted(Module);
        }

        public string BoardLabel(int Module) => Board(Module)?.Label ?? "";
        public ushort InoID(int Module) => Board(Module)?.InoID ?? 0;
        public ushort ModuleType(int Module) => Board(Module)?.InoType ?? 0;

        public string ModuleVersion(int Module)
        {
            BoardInfo Brd = Board(Module);
            return Brd == null ? "" : Props.ParseDate(Brd.InoID.ToString());
        }

        // the board serving this ID: the most recently heard, if accepted
        private BoardInfo Board(int Module)
        {
            BoardInfo Result = null;
            if (Module >= 0 && Module < Props.MaxModules && Accepted(Module)) Result = BoardsAt(Module)[0];
            return Result;
        }

        // ddmmy (no leading 0) -> yyyymmdd, so firmware dates compare in order
        private static int InoDate(ushort InoID)
        {
            int Result = 0;
            string s = InoID.ToString();
            if (s.Length > 3)
            {
                int YR = int.Parse(s.Substring(s.Length - 1)) + 2020;
                int MN = int.Parse(s.Substring(s.Length - 3, 2));
                int DY = int.Parse(s.Substring(0, s.Length - 3));
                Result = YR * 10000 + MN * 100 + DY;
            }
            return Result;
        }

        // bytes 3-18: 0-padded, printable ASCII only
        private static string ReadLabel(byte[] Data)
        {
            StringBuilder sb = new StringBuilder(16);
            for (int i = 0; i < 16; i++)
            {
                byte b = Data[3 + i];
                if (b == 0) break;
                if (b >= 32 && b < 127) sb.Append((char)b);
            }
            return sb.ToString().Trim();
        }
    }
}
