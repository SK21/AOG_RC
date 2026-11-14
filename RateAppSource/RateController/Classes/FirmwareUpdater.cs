using RateController.PGNs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RateController.Classes
{
    // Firmware update of a Teensy rate board, started from the Boards page (frmFirmwareUpdate).
    // Design: Design Docs\Teensy_CAN_Firmware_Update.md
    //
    // New method (PGN 32810-32812, CAN 0xFF1A-0xFF1C): the hex file goes as numbered blocks of
    // 11 records, each with a CRC-16, and the next block goes only after the board has written
    // this one. The board is chosen by MAC. Works on whichever link the app uses.
    //
    // Old method (firmware older than MinProtocolDate, Ethernet only): what PCBsetup does. PGN
    // 32800 to UDP 29100, replies on UDP 29000, 11 records every 10 ms with no acknowledgement.
    //
    // Either way the board's FlasherX replaces the running firmware only when everything arrived
    // and the record count matched; otherwise the board restarts on its old firmware.
    public class FirmwareUpdater
    {
        // first Teensy firmware with PGN 32810-32812 (yyyymmdd, as PGN32403.InoDate)
        public const int MinProtocolDate = 20261114;

        public enum Outcome
        { Done, NotBack, NoReply, WrongType, WrongModule, Refused, NoAnswer, CountWrong, Cancelled, Failed }

        public enum Stage
        { Starting, Sending, Installing }

        private const int BlockTimeoutMs = 500;
        private const int BlockTries = 5;
        private const int StartWaitMs = 1500;       // twice: no Ready in 3 s = no reply
        private const int EOFWaitMs = 5000;
        private const int BackWaitSec = 120;        // the board copies the new firmware, then restarts
        private const int LegacyPacketMs = 10;      // PCBsetup's pace
        private const int LegacyReceivePort = 29000;
        private const int LegacySendPort = 29100;

        private readonly BlockingCollection<Status> cStatus = new BlockingCollection<Status>();
        private ulong cMAC;

        public event Action<int> ProgressChanged;   // percent of the file sent
        public event Action<Stage> StageChanged;

        private class Status
        {
            public byte Code;
            public int Block;
            public uint Count;
        }

        // ':LLAAAATT<data>CC' lines to records of bytes (len, addrHi, addrLo, type, data, checksum).
        // Null unless every record is well formed, its checksum is right and it ends with EOF.
        public static List<byte[]> ReadHex(string FileName)
        {
            List<byte[]> Result = null;
            try
            {
                var Records = new List<byte[]>();
                bool Good = true;
                bool EOF = false;
                foreach (string Raw in File.ReadAllLines(FileName))
                {
                    string Line = Raw.Trim();
                    if (Good && !EOF && Line.Length > 0)
                    {
                        if (Line[0] != ':' || Line.Length < 11 || (Line.Length - 1) % 2 != 0)
                        {
                            Good = false;
                        }
                        else
                        {
                            byte[] R = new byte[(Line.Length - 1) / 2];
                            for (int i = 0; Good && i < R.Length; i++)
                            {
                                Good = byte.TryParse(Line.Substring(1 + i * 2, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out R[i]);
                            }

                            if (Good)
                            {
                                byte Sum = 0;
                                foreach (byte b in R) Sum += b;
                                Good = R.Length == R[0] + 5 && R[0] <= 16 && Sum == 0;
                            }

                            if (Good)
                            {
                                Records.Add(R);
                                EOF = R[3] == 1;
                            }
                        }
                    }
                }
                if (Good && EOF) Result = Records;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareUpdater/ReadHex: " + ex.Message);
            }
            return Result;
        }

        // The firmware identity tag (FwTag in RCteensy.ino): "RCFWID01", InoType, InoID lo, InoID hi.
        // Searched in the program bytes rebuilt by address from the records; a run of records that
        // isn't contiguous starts a new search, so a match never spans a gap. False when there is none
        // (another kind of firmware, or a rate build from before the tag).
        public static bool FindTag(List<byte[]> Records, out byte Type, out ushort InoID)
        {
            bool Result = false;
            Type = 0;
            InoID = 0;
            byte[] Magic = { (byte)'R', (byte)'C', (byte)'F', (byte)'W', (byte)'I', (byte)'D', (byte)'0', (byte)'1' };
            var Run = new List<byte>();
            long Base = 0;
            long Next = -1;     // address the run continues at
            for (int k = 0; !Result && k < Records.Count; k++)
            {
                byte[] R = Records[k];
                if (R[3] == 0)
                {
                    long Addr = Base + ((R[1] << 8) | R[2]);
                    if (Addr != Next)
                    {
                        Result = SearchTag(Run, Magic, ref Type, ref InoID);
                        Run.Clear();
                    }
                    for (int i = 0; i < R[0]; i++) Run.Add(R[4 + i]);
                    Next = Addr + R[0];
                }
                else if (R[3] == 4 && R[0] == 2)
                {
                    Base = (long)((R[4] << 8) | R[5]) << 16;
                }
                else if (R[3] == 2 && R[0] == 2)
                {
                    Base = (long)((R[4] << 8) | R[5]) << 4;
                }
            }
            if (!Result) Result = SearchTag(Run, Magic, ref Type, ref InoID);
            return Result;
        }

        private static bool SearchTag(List<byte> Run, byte[] Magic, ref byte Type, ref ushort InoID)
        {
            bool Result = false;
            for (int i = 0; !Result && i + Magic.Length + 3 <= Run.Count; i++)
            {
                bool Match = true;
                for (int j = 0; Match && j < Magic.Length; j++) Match = Run[i + j] == Magic[j];
                if (Match)
                {
                    Type = Run[i + 8];
                    InoID = (ushort)(Run[i + 9] | (Run[i + 10] << 8));
                    Result = true;
                }
            }
            return Result;
        }

        // PGN 32812 from UDPcomm (UI thread) or CanBridgeComm (driver thread)
        public void StatusReceived(byte[] Data)
        {
            if (Data.Length >= 16 && Core.Tls.GoodCRC(Data))
            {
                ulong mac = 0;
                for (int i = 0; i < 6; i++) mac = (mac << 8) | Data[2 + i];
                if (mac == cMAC)
                {
                    cStatus.Add(new Status
                    {
                        Code = Data[8],
                        Block = Data[9] | (Data[10] << 8),
                        Count = (uint)(Data[11] | (Data[12] << 8) | (Data[13] << 16) | (Data[14] << 24))
                    });
                }
            }
        }

        // the new method, on the app's link
        public Task<Outcome> RunAsync(List<byte[]> Records, ulong MAC, bool OverCan, CancellationToken Ct)
        {
            return Task.Run(() => Run(Records, MAC, OverCan, Ct));
        }

        // the old method (PCBsetup's), Ethernet only, by module ID
        public Task<Outcome> RunLegacyAsync(List<byte[]> Records, int ModuleID, CancellationToken Ct)
        {
            return Task.Run(() => RunLegacy(Records, ModuleID, Ct));
        }

        private Outcome Run(List<byte[]> Records, ulong MAC, bool OverCan, CancellationToken Ct)
        {
            Outcome Result = Outcome.Failed;
            bool Started = false;
            PGN32810 Control = new PGN32810();
            PGN32811 Blocks = new PGN32811();
            try
            {
                cMAC = MAC;
                while (cStatus.TryTake(out _)) { }
                StageChanged?.Invoke(Stage.Starting);

                Status S = null;
                for (int i = 0; S == null && i < 2; i++)
                {
                    Control.Send(PGN32810.Start, MAC, (byte)ModuleTypes.Teensy_Rate, OverCan);
                    S = WaitFor(StartWaitMs, Ct, 100, 102, 106, 107);
                }

                if (S == null)
                {
                    Result = Outcome.NoReply;
                }
                else if (S.Code == 102)
                {
                    Result = Outcome.WrongType;
                }
                else if (S.Code == 107)
                {
                    Result = Outcome.Refused;
                }
                else if (S.Code == 106)
                {
                    Result = Outcome.Failed;
                }
                else
                {
                    Started = true;
                    StageChanged?.Invoke(Stage.Sending);
                    List<byte[]> Packed = BuildBlocks(Records);
                    Result = SendBlocks(Packed, Blocks, OverCan, Ct, out uint Count, out bool HaveCount);

                    if (Result == Outcome.Done && !HaveCount)
                    {
                        Status E = WaitFor(EOFWaitMs, Ct, 102, 105, 106);
                        HaveCount = E != null && E.Code == 105;
                        if (HaveCount) Count = E.Count;
                        if (!HaveCount) Result = E != null && E.Code == 102 ? Outcome.WrongType : Outcome.NoAnswer;
                    }

                    if (Result == Outcome.Done)
                    {
                        if (Count == Records.Count)
                        {
                            // sent twice: the board installs on the first, and once restarted it
                            // ignores the second
                            DateTime Sent = DateTime.Now;
                            Control.Send(PGN32810.Commit, MAC, (byte)ModuleTypes.Teensy_Rate, OverCan);
                            Thread.Sleep(100);
                            Control.Send(PGN32810.Commit, MAC, (byte)ModuleTypes.Teensy_Rate, OverCan);
                            Started = false;
                            StageChanged?.Invoke(Stage.Installing);
                            Result = WaitForBoard(Sent, () => Core.ModulesBoardID.LiveBoard(MAC)?.LastHeard ?? DateTime.MinValue, Ct);
                        }
                        else
                        {
                            Result = Outcome.CountWrong;
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Result = Outcome.Cancelled;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareUpdater/Run: " + ex.Message);
                Result = Outcome.Failed;
            }

            // anything short of the commit: the board goes back to its old firmware now, not after 10 s
            if (Started && Result != Outcome.Done) Control.Send(PGN32810.Abort, MAC, (byte)ModuleTypes.Teensy_Rate, OverCan);
            return Result;
        }

        private Outcome SendBlocks(List<byte[]> Packed, PGN32811 Blocks, bool OverCan, CancellationToken Ct, out uint Count, out bool HaveCount)
        {
            Outcome Result = Outcome.Done;
            Count = 0;
            HaveCount = false;
            int i = 0;
            int Tries = 0;
            while (Result == Outcome.Done && i < Packed.Count)
            {
                Ct.ThrowIfCancellationRequested();
                Blocks.Send(i, Packed[i], OverCan);

                bool Written = false;
                bool Resend = false;
                DateTime Until = DateTime.Now.AddMilliseconds(BlockTimeoutMs);
                while (!Written && !Resend && Result == Outcome.Done && DateTime.Now < Until)
                {
                    int Left = Math.Max(1, (int)(Until - DateTime.Now).TotalMilliseconds);
                    if (cStatus.TryTake(out Status S, Left, Ct))
                    {
                        switch (S.Code)
                        {
                            case 103:
                                Written = S.Block == i;     // an older block's repeat is ignored
                                break;

                            case 104:
                                // the board wants this block (again)
                                if (S.Block >= 0 && S.Block < Packed.Count && S.Block <= i) i = S.Block;
                                Resend = true;
                                break;

                            case 105:
                                Count = S.Count;
                                HaveCount = true;
                                break;

                            case 102:
                                // the whole file is in, and it isn't Teensy rate firmware: the board
                                // didn't install it and restarts on its old firmware
                                Result = Outcome.WrongType;
                                break;

                            case 106:
                                Result = Outcome.Failed;
                                break;
                        }
                    }
                }

                if (Written)
                {
                    i++;
                    Tries = 0;
                    ProgressChanged?.Invoke(i * 100 / Packed.Count);
                }
                else if (Result == Outcome.Done && ++Tries > BlockTries)
                {
                    Result = Outcome.NoAnswer;
                }
            }
            return Result;
        }

        // up to 11 records and PGN32811.MaxBlock bytes per block
        private static List<byte[]> BuildBlocks(List<byte[]> Records)
        {
            var Result = new List<byte[]>();
            var Current = new List<byte>();
            int InBlock = 0;
            foreach (byte[] R in Records)
            {
                if (InBlock == 11 || Current.Count + R.Length > PGN32811.MaxBlock)
                {
                    Result.Add(Current.ToArray());
                    Current.Clear();
                    InBlock = 0;
                }
                Current.AddRange(R);
                InBlock++;
            }
            if (Current.Count > 0) Result.Add(Current.ToArray());
            return Result;
        }

        // the next status with one of these codes, or null after Ms
        private Status WaitFor(int Ms, CancellationToken Ct, params byte[] Codes)
        {
            Status Result = null;
            DateTime Until = DateTime.Now.AddMilliseconds(Ms);
            while (Result == null && DateTime.Now < Until)
            {
                int Left = Math.Max(1, (int)(Until - DateTime.Now).TotalMilliseconds);
                if (cStatus.TryTake(out Status S, Left, Ct) && Array.IndexOf(Codes, S.Code) >= 0) Result = S;
            }
            return Result;
        }

        // after the commit the board goes quiet while it copies the new firmware, then restarts and
        // reports again. LastHeard is read on the UI thread, where the board list is kept.
        private Outcome WaitForBoard(DateTime Since, Func<DateTime> LastHeard, CancellationToken Ct)
        {
            Outcome Result = Outcome.NotBack;
            DateTime Until = Since.AddSeconds(BackWaitSec);
            while (Result == Outcome.NotBack && DateTime.Now < Until && !Ct.IsCancellationRequested)
            {
                Thread.Sleep(500);
                DateTime Heard = DateTime.MinValue;
                try
                {
                    Heard = (DateTime)Core.MainForm.Invoke(LastHeard);
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("FirmwareUpdater/WaitForBoard: " + ex.Message);
                }
                if (Heard > Since.AddSeconds(2)) Result = Outcome.Done;
            }
            return Result;
        }

        private Outcome RunLegacy(List<byte[]> Records, int ModuleID, CancellationToken Ct)
        {
            Outcome Result = Outcome.Failed;
            bool Started = false;
            UdpClient Udp = null;
            IPEndPoint Board = null;
            var Replies = new BlockingCollection<byte[]>();
            try
            {
                StageChanged?.Invoke(Stage.Starting);

                Udp = new UdpClient();
                Udp.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                Udp.Client.Bind(new IPEndPoint(IPAddress.Any, LegacyReceivePort));
                Udp.EnableBroadcast = true;
                Board = new IPEndPoint(IPAddress.Parse(Core.UDPmodules.NetworkEP), LegacySendPort);
                UdpClient Rx = Udp;
                Task.Run(async () =>
                {
                    try
                    {
                        while (true)
                        {
                            UdpReceiveResult R = await Rx.ReceiveAsync();
                            Replies.Add(R.Buffer);
                        }
                    }
                    catch (Exception)
                    {
                        // socket closed at the end
                    }
                });

                // PGN 32800: the old firmware compares the HIGH nibble of byte 2 with its ID, so the
                // ID goes there; that is how it matches every module, not just module 0
                byte[] Begin = { 32, 128, (byte)(ModuleID << 4), (byte)ModuleTypes.Teensy_Rate, 0, 0 };
                Begin[5] = Core.Tls.CRC(Begin, 5);

                byte[] Reply = null;
                for (int i = 0; Reply == null && i < 2; i++)
                {
                    Udp.Send(Begin, Begin.Length, Board);
                    Reply = WaitLegacy(Replies, 34, StartWaitMs, Ct);
                }

                if (Reply == null)
                {
                    Result = Outcome.NoReply;
                }
                else if (Reply[3] == 101)
                {
                    Result = Outcome.WrongModule;
                }
                else if (Reply[3] == 102)
                {
                    Result = Outcome.WrongType;
                }
                else if (Reply[3] == 100)
                {
                    Started = true;
                    StageChanged?.Invoke(Stage.Sending);
                    for (int i = 0; i < Records.Count; i += 11)
                    {
                        Ct.ThrowIfCancellationRequested();
                        var Packet = new List<byte>();
                        for (int r = i; r < Math.Min(i + 11, Records.Count); r++)
                        {
                            Packet.Add(0x3A);
                            Packet.AddRange(Records[r]);
                        }
                        Udp.Send(Packet.ToArray(), Packet.Count, Board);
                        ProgressChanged?.Invoke(Math.Min(i + 11, Records.Count) * 100 / Records.Count);
                        Thread.Sleep(LegacyPacketMs);
                    }

                    // PGN 32801: the records the board received
                    byte[] Lines = WaitLegacy(Replies, 33, EOFWaitMs, Ct);
                    if (Lines == null)
                    {
                        Result = Outcome.NoAnswer;
                    }
                    else if ((Lines[2] | (Lines[3] << 8) | (Lines[4] << 16) | (Lines[5] << 24)) != Records.Count)
                    {
                        Result = Outcome.CountWrong;
                    }
                    else
                    {
                        DateTime Sent = DateTime.Now;
                        byte[] Commit = { 0x3A, 0x00, 0x00, 0x00, 0x06, 0xFA };
                        Udp.Send(Commit, Commit.Length, Board);
                        Started = false;
                        StageChanged?.Invoke(Stage.Installing);
                        Result = WaitForBoard(Sent, () => LastHeardAt(ModuleID), Ct);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Result = Outcome.Cancelled;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareUpdater/RunLegacy: " + ex.Message);
                Result = Outcome.Failed;
            }

            try
            {
                if (Started && Result != Outcome.Done && Udp != null && Board != null)
                {
                    // the abort record: the board restarts on its old firmware
                    byte[] Abort = { 0x3A, 0x00, 0x00, 0x00, 0x07, 0xF9 };
                    Udp.Send(Abort, Abort.Length, Board);
                }
                Udp?.Close();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareUpdater/RunLegacy close: " + ex.Message);
            }
            return Result;
        }

        // the newest report of any board at this module (an old board comes back with a MAC)
        private static DateTime LastHeardAt(int ModuleID)
        {
            DateTime Result = DateTime.MinValue;
            foreach (BoardInfo B in Core.ModulesBoardID.BoardsAt(ModuleID))
            {
                if (B.LastHeard > Result) Result = B.LastHeard;
            }
            return Result;
        }

        // the next PGN 3280x reply (HeaderLo = Lo, HeaderHi 128) with a good CRC
        private static byte[] WaitLegacy(BlockingCollection<byte[]> Replies, byte Lo, int Ms, CancellationToken Ct)
        {
            byte[] Result = null;
            DateTime Until = DateTime.Now.AddMilliseconds(Ms);
            while (Result == null && DateTime.Now < Until)
            {
                int Left = Math.Max(1, (int)(Until - DateTime.Now).TotalMilliseconds);
                if (Replies.TryTake(out byte[] D, Left, Ct) && D.Length >= 5 && D[0] == Lo && D[1] == 128
                    && Core.Tls.GoodCRC(D)) Result = D;
            }
            return Result;
        }
    }
}
