using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RateController.Classes
{
    public enum PgnLink : byte
    {
        UDP,    // RC modules over Ethernet / WiFi (UDPmodules)
        CAN,    // RC modules over CAN (CanBridgeComm), as their UDP-equivalent PGNs
        TC,     // AOG-TaskController (clsTaskControllerRate): 0xF2, 0xF4, 0xF5
        AOG     // AgOpenGPS / AgIO (UDPaog)
    }

    // One log of the PGNs every link sends and receives (Design Docs\PGN_Log_Design.md).
    // Writers call Add / Note from any thread; the PGN Log page and the help page read Snapshot().
    // New entries are appended to "PGN Log.txt" every 60 s (Core's timer calls FlushIfDue) and on exit.
    // The buffer holds well over a minute of traffic with AgIO on, so nothing is overwritten before it's written.
    // AgIO traffic (~10 Hz) is recorded only while AogOn.
    public class clsPgnLog
    {
        public const string FileName = "PGN Log.txt";
        private const int Capacity = 10000;
        private const int FlushSeconds = 60;
        private const long MaxFileBytes = 2000000;
        private const char NoteMark = '·';     // middle dot

        private readonly Entry[] Buffer = new Entry[Capacity];
        private readonly object LogLock = new object();
        private long cAdded;            // entries added since the start; the newest is at (cAdded - 1) % Capacity
        private long cWritten;          // entries already appended to the file
        private DateTime LastFlush = DateTime.Now;

        public struct Entry
        {
            public char Dir;            // '>' sent, '<' received, NoteMark note
            public PgnLink Link;
            public int Module;          // -1 = none
            public string Name;         // short name, or the note's text
            public int NewModule;       // 32508: the ID assigned, -1 = none
            public string PgnText;      // "32500", "0xF2", "254"; "" for a note
            public int Sensor;          // sensor, or the TC channel; -1 = none
            public DateTime Time;
        }

        /// <summary>AgIO traffic is recorded only while this is on (PGN Log page, AOG button). Off at start.</summary>
        public bool AogOn { get; set; } = false;

        public static string Line(Entry E)
        {
            string Result;
            string Time = E.Time.ToString("HH:mm:ss.f");
            if (E.Dir == NoteMark)
            {
                Result = Time + "  " + NoteMark + "  " + E.Name + "  " + E.Link.ToString();
            }
            else
            {
                Result = Time + "  " + E.Dir + "  " + E.PgnText.PadRight(6) + " " + (E.Name ?? "").PadRight(16) + " " + E.Link.ToString().PadRight(4) + " " + Target(E);
            }
            return Result.TrimEnd();
        }

        /// <summary>A packet sent (Sent true) or received on Link. The PGN, its name and target are read from the bytes.
        /// An AgIO-framed packet (80 81 src pgn ...) on UDP or AOG is logged as AOG, and only while AogOn.</summary>
        public void Add(PgnLink Link, bool Sent, byte[] Data)
        {
            try
            {
                if (Data != null && Data.Length >= 2)
                {
                    Entry E = new Entry
                    {
                        Time = DateTime.Now,
                        Dir = Sent ? '>' : '<',
                        Link = Link,
                        Module = -1,
                        Sensor = -1,
                        NewModule = -1
                    };

                    bool Keep = true;
                    bool AgIO = Data.Length >= 5 && Data[0] == 0x80 && Data[1] == 0x81;
                    if (AgIO)
                    {
                        int Pgn = Data[3];
                        if (Link == PgnLink.TC)
                        {
                            E.PgnText = "0x" + Pgn.ToString("X2");
                            E.Name = TCname(Data);
                            E.Sensor = TCchannel(Data);
                        }
                        else
                        {
                            E.Link = PgnLink.AOG;
                            E.PgnText = Pgn.ToString();
                            E.Name = AOGname(Data);
                            Keep = AogOn;
                        }
                    }
                    else
                    {
                        int Pgn = Data[0] | Data[1] << 8;
                        E.PgnText = Pgn.ToString();
                        E.Name = ModuleName(Pgn);
                        SetModuleTarget(ref E, Pgn, Data);
                    }

                    if (Keep) Store(E);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsPgnLog/Add: " + ex.Message);
            }
        }

        public void Clear()
        {
            lock (LogLock)
            {
                // entries not yet in the file are dropped with the rest
                cAdded = 0;
                cWritten = 0;
            }
        }

        /// <summary>Empties the file (help page Reset).</summary>
        public void DeleteFile()
        {
            try
            {
                lock (LogLock)
                {
                    File.WriteAllText(FilePath(), "");
                    cWritten = cAdded;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsPgnLog/DeleteFile: " + ex.Message);
            }
        }

        /// <summary>Appends the entries not yet written to the file.</summary>
        public void Flush()
        {
            try
            {
                StringBuilder Sb = new StringBuilder();
                lock (LogLock)
                {
                    long First = Math.Max(cWritten, cAdded - Capacity);    // older ones were overwritten unwritten
                    for (long i = First; i < cAdded; i++)
                    {
                        Sb.Append(Line(Buffer[i % Capacity])).Append("\r\n");
                    }
                    cWritten = cAdded;
                }
                LastFlush = DateTime.Now;

                if (Sb.Length > 0)
                {
                    string Path = FilePath();
                    Trim(Path);
                    File.AppendAllText(Path, Sb.ToString());
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsPgnLog/Flush: " + ex.Message);
            }
        }

        /// <summary>Core's 1 s timer: writes to the file every FlushSeconds.</summary>
        public void FlushIfDue()
        {
            if ((DateTime.Now - LastFlush).TotalSeconds >= FlushSeconds) Flush();
        }

        /// <summary>Something that isn't a packet: a send failure, a subnet learned or dropped.</summary>
        public void Note(PgnLink Link, string Text)
        {
            Entry E = new Entry
            {
                Time = DateTime.Now,
                Dir = NoteMark,
                Link = Link,
                Module = -1,
                Sensor = -1,
                NewModule = -1,
                PgnText = "",
                Name = Text
            };
            Store(E);
        }

        /// <summary>The entries in the buffer, oldest first.</summary>
        public List<Entry> Snapshot()
        {
            List<Entry> Result;
            lock (LogLock)
            {
                long First = Math.Max(0, cAdded - Capacity);
                Result = new List<Entry>((int)(cAdded - First));
                for (long i = First; i < cAdded; i++) Result.Add(Buffer[i % Capacity]);
            }
            return Result;
        }

        // AgIO-framed: 80 81 [source] [PGN] [length] [payload] [checksum]
        private static string AOGname(byte[] Data)
        {
            string Result;
            switch (Data[3])
            {
                case 100: Result = "position"; break;
                case 200: Result = "hello"; break;
                case 201: Result = "subnet"; break;
                case 208: Result = "TWOL GPS"; break;
                case 221: Result = "hardware message"; break;
                case 229: Result = "sections"; break;
                case 240: Result = "SC state"; break;
                case 241: Result = "SC mode"; break;

                case 242:
                    // process data to AOG-TaskController: [DDI lo, hi][int32 value]...
                    int DDI = Data.Length > 6 ? Data[5] | Data[6] << 8 : 0;
                    if (DDI == 397) Result = "speed";
                    else if (DDI == 597) Result = "distance";
                    else Result = "process data";
                    break;

                case 234: Result = "switches to AOG"; break;
                case 235: Result = "section widths"; break;
                case 238: Result = "machine config"; break;
                case 239: Result = "machine data"; break;
                case 254: Result = "AutoSteer data"; break;
                default: Result = ""; break;
            }
            return Result;
        }

        private static string FilePath()
        {
            return Path.Combine(Props.ApplicationFolder, FileName);
        }

        private static string ModuleName(int Pgn)
        {
            string Result;
            switch (Pgn)
            {
                case 32296: Result = "scale"; break;
                case 32400: Result = "rate applied"; break;
                case 32401: Result = "module status"; break;
                case 32402: Result = "PID log"; break;
                case 32403: Result = "identity"; break;
                case 32500: Result = "rate settings"; break;
                case 32501: Result = "relays"; break;
                case 32502: Result = "PID settings"; break;
                case 32503: Result = "subnet change"; break;
                case 32504: Result = "wheel speed"; break;
                case 32505: Result = "pressure gate"; break;
                case 32506: Result = "description"; break;
                case 32507: Result = "sensor pins"; break;
                case 32508: Result = "assign ID"; break;
                case 32509: Result = "TC units"; break;
                case 32618: Result = "switchbox"; break;
                case 32700: Result = "module config"; break;
                case 32810: Result = "fw update"; break;
                case 32811: Result = "fw block"; break;
                case 32812: Result = "fw status"; break;
                default: Result = ""; break;
            }
            return Result;
        }

        // module / sensor from byte 2: high nibble module, low nibble sensor for per-sensor PGNs; the raw
        // module ID for module-level PGNs; 32501 carries the module in the high nibble only
        private static void SetModuleTarget(ref Entry E, int Pgn, byte[] Data)
        {
            if (Data.Length > 2)
            {
                switch (Pgn)
                {
                    case 32400:
                    case 32402:
                    case 32500:
                    case 32502:
                    case 32507:
                    case 32509:
                        E.Module = Data[2] >> 4;
                        E.Sensor = Data[2] & 0x0F;
                        break;

                    case 32501:
                        E.Module = Data[2] >> 4;
                        break;

                    case 32401:
                    case 32403:
                    case 32504:
                    case 32505:
                    case 32506:
                    case 32700:
                        E.Module = Data[2];
                        break;

                    case 32508:
                        if (Data.Length > 8) E.NewModule = Data[8];
                        break;
                }
            }
        }

        private void Store(Entry E)
        {
            lock (LogLock)
            {
                Buffer[cAdded % Capacity] = E;
                cAdded++;
            }
        }

        private static string Target(Entry E)
        {
            string Result = "";
            if (E.Link == PgnLink.TC)
            {
                if (E.Sensor >= 0) Result = "ch" + E.Sensor;
            }
            else if (E.NewModule >= 0)
            {
                Result = "-> m" + E.NewModule;
            }
            else if (E.Module >= 0)
            {
                Result = "m" + E.Module;
                if (E.Sensor >= 0) Result += " s" + E.Sensor;
            }
            return Result;
        }

        // 0xF2: channel is the last payload byte; 0xF4 / 0xF5: the first
        private static int TCchannel(byte[] Data)
        {
            int Result = -1;
            int Len = Data[4];
            if (Data[3] == 0xF2)
            {
                if (Data.Length > 4 + Len && Len > 0) Result = Data[4 + Len];
            }
            else if (Data[3] == 0xF4 || Data[3] == 0xF5)
            {
                if (Data.Length > 5) Result = Data[5];
            }
            return Result;
        }

        private static string TCname(byte[] Data)
        {
            string Result;
            switch (Data[3])
            {
                case 0xF2:
                    int DDI = Data.Length > 6 ? Data[5] | Data[6] << 8 : 0;
                    if (DDI == 158) Result = "TC 158 request";
                    else if (DDI == 397) Result = "TC speed";
                    else Result = "TC setpoint";
                    break;

                case 0xF4: Result = "TC channel info"; break;
                case 0xF5: Result = "TC actuals"; break;
                default: Result = "TC"; break;
            }
            return Result;
        }

        // keeps the file under MaxFileBytes by dropping its oldest 10%
        private static void Trim(string Path)
        {
            try
            {
                if (File.Exists(Path) && new FileInfo(Path).Length > MaxFileBytes)
                {
                    string[] Lines = File.ReadAllLines(Path);
                    int Skip = Lines.Length / 10;
                    string[] Kept = new string[Lines.Length - Skip];
                    Array.Copy(Lines, Skip, Kept, 0, Kept.Length);
                    File.WriteAllLines(Path, Kept);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsPgnLog/Trim: " + ex.Message);
            }
        }
    }
}
