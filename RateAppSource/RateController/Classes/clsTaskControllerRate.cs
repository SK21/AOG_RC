using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RateController.Classes
{
    // Sends each product's target rate to AOG-TaskController, which passes it to the implement's
    // settable rate DDI over ISOBUS (ISOBUS_Path_Forward.md, Phase D).
    // Frame: 80 81 7F F2 07 [DDI lo, hi] [value int32 LE] [channel] [checksum], channel = product + 1.
    // Sent every second (AOG-TaskController sends 0 after 4 s without a setpoint) and at once on a change.
    // Source 0x7F: AOG-TaskController only accepts AgIO's source today; RC's own source is a question for upstream.
    // Sent to AOG-TaskController on this PC: to 127.0.0.1:8888 and to each of this PC's own IPv4
    // addresses on 8888. AOG-TaskController reads 8888 through a socket bound to ONE address: loopback
    // when no adapter matches its subnet, the adapter's address once AgIO sends its subnet (it rebinds),
    // and then packets to 127.0.0.1 never reach it. Packets to the PC's own address stay on the PC.
    // Another PC would need the subnet broadcast.
    //
    // Channel info (Group2_Step1_Channels_Design.md): 80 81 7F F4 <len> [channel] [DDI lo, hi]
    // [product name, UTF-8, 0-24 bytes] [checksum], len = 3 + name bytes. DDI 0 = the channel is not
    // in use. AOG-TaskController matches the implement's rate elements to channels by DDI. While any
    // product sends, every channel's info goes out on a change and every 2 s; when the last one stops,
    // every channel gets DDI 0 once and nothing more is sent.
    //
    // Step 3 (Group2_Step3_Prescription_State_Design.md): a channel is announced while its product is ISOBUS, enabled
    // and its unit can be sent, whatever VR is. VR on: setpoints as above. VR off: 0xF2 [DDI 158][0][channel] every
    // second instead, so the implement uses its own rate. "Ask again": [DDI 158][1][channel] once. 0xF5 of 16 bytes
    // adds 158 / has 158 / no reply (flags bits 2-5) and DDI 288 (byte 15); clsProduct.RxCase() reads them.
    public class clsTaskControllerRate : IDisposable
    {
        private const int AddressRefreshMs = 5000;
        private const int CheckMs = 200;
        private const int InfoResendMs = 2000;
        private const int MaxNameBytes = 24;
        private const int ResendMs = 1000;
        private const int TCport = 8888;
        private List<IPAddress> LocalAddresses = new List<IPAddress>();
        private DateTime LocalAddressesTime = DateTime.MinValue;
        private readonly int RateProducts = Props.MaxProducts - 2;     // the last two are fans
        private readonly ushort[] LastDDI;
        private readonly ushort[] LastInfoDDI;
        private readonly string[] LastInfoName;
        private readonly DateTime[] LastInfoSent;
        private readonly DateTime[] LastSent;
        private readonly int[] LastValue;
        private readonly DateTime[] LastRxOffSent;      // VR off: DDI 158 = 0 per channel every ResendMs
        private readonly bool[] Sending;                // the channel is announced: ISOBUS, enabled, its unit can be sent
        private bool LastVR;                            // VR as on the last tick
        private readonly DateTime StartTime = DateTime.Now;
        private const ushort DDIrxState = 158;          // Prescription Control State (Group2_Step3_Prescription_State_Design.md)
        private const int StartupHoldSeconds = 5;       // no "no TC" case right after RC starts
        private const int AskResultSeconds = 6;         // Ask again: the result is read after AOG-TC's confirm (about 4 s)
        private readonly DateTime?[] AskedTime;         // per product: when Ask again was sent, until its result is read
        private readonly System.Windows.Forms.Timer SendTimer;
        private bool AnySending;
        private bool cDisposed;
        private UdpClient Udp;
        private bool Receiving;
        private const int FailNoteSeconds = 60;
        private DateTime LastFailNote = DateTime.MinValue;    // send failures go to the PGN log at most once a minute

        // PGN 0xF5 from AOG-TaskController, back on the socket the setpoints go out from: per channel, what the
        // implement reports (Group2_Step2_Recording_Design.md). [channel][actual DDI lo, hi][rate int32]
        // [total DDI lo, hi][total int32][elements][flags: bit 0 rate valid, bit 1 total valid]
        private const int ActualsFreshMs = 3000;
        private readonly object ActualsLock = new object();
        private readonly TCActuals[] Actuals;

        private class TCActuals
        {
            public ushort ActualDDI;
            public byte Elements;
            public bool Extended;       // 16-byte frame (step 3): the prescription fields below are valid
            public byte Flags;
            public bool Has158;         // flags bit 4: the channel's elements have DDI 158
            public bool NoReply;        // flags bit 5: no reply to 158 after AOG-TC's resends
            public int Rate;
            public DateTime Received = DateTime.MinValue;
            public byte RxMode;         // byte 15: DDI 288 Actual Prescription Mode, 0 = unknown / not present
            public byte RxState;        // flags bits 2-3: DDI 158 as reported (0 off, 1 on, 2 error, 3 not installed)
            public int Total;
            public ushort TotalDDI;
        }

        public clsTaskControllerRate()
        {
            LastDDI = new ushort[RateProducts];
            LastInfoDDI = new ushort[RateProducts];
            LastInfoName = new string[RateProducts];
            LastInfoSent = new DateTime[RateProducts];
            LastSent = new DateTime[RateProducts];
            LastValue = new int[RateProducts];
            LastRxOffSent = new DateTime[RateProducts];
            AskedTime = new DateTime?[RateProducts];
            Sending = new bool[RateProducts];
            LastVR = Props.VariableRateEnabled;
            Actuals = new TCActuals[RateProducts];
            for (int i = 0; i < RateProducts; i++) Actuals[i] = new TCActuals();

            // UI thread timer, like Core's MainTimer: products are read on the thread that edits them
            SendTimer = new System.Windows.Forms.Timer();
            SendTimer.Interval = CheckMs;
            SendTimer.Tick += SendTimer_Tick;
        }

        public void Dispose()
        {
            if (!cDisposed)
            {
                Stop();
                SendTimer.Dispose();
                cDisposed = true;
            }
        }

        public void Start()
        {
            SendTimer.Enabled = true;
        }

        public void Stop()
        {
            SendTimer.Enabled = false;
            CloseSocket();
        }

        /// <summary>What the implement applied for a product (PGN 0xF5), when it is fresh (under 3 s) and the
        /// channel has elements. Rate: ISO raw in ActualDDI's unit (2, 7 or 12). Total: mL (351), g (352) or count (82).</summary>
        public bool ImplementActuals(int ProductID, out ushort ActualDDI, out int Rate, out bool RateValid, out ushort TotalDDI, out int Total, out bool TotalValid)
        {
            bool Result = false;
            ActualDDI = 0;
            Rate = 0;
            RateValid = false;
            TotalDDI = 0;
            Total = 0;
            TotalValid = false;
            if (ProductID >= 0 && ProductID < RateProducts)
            {
                lock (ActualsLock)
                {
                    TCActuals A = Actuals[ProductID];
                    if (A.Elements > 0 && (DateTime.Now - A.Received).TotalMilliseconds < ActualsFreshMs)
                    {
                        ActualDDI = A.ActualDDI;
                        Rate = A.Rate;
                        RateValid = (A.Flags & 1) != 0;
                        TotalDDI = A.TotalDDI;
                        Total = A.Total;
                        TotalValid = (A.Flags & 2) != 0;
                        Result = true;
                    }
                }
            }
            return Result;
        }

        /// <summary>True while the product's channel is announced (0xF4): ISOBUS, enabled, its unit can be sent.</summary>
        public bool Announced(int ProductID)
        {
            return ProductID >= 0 && ProductID < RateProducts && Sending[ProductID];
        }

        /// <summary>True while VR is on and at least one ISOBUS product is in case 1-6: the Rx button shows red.</summary>
        public bool AnyRxCase()
        {
            bool Result = false;
            if (Props.VariableRateEnabled)
            {
                for (int i = 0; i < RateProducts && !Result; i++)
                {
                    Result = Sending[i] && Core.Products.Item(i).RxCase() != RxCases.None;
                }
            }
            return Result;
        }

        /// <summary>The red Rx dialog's entries, one per product in case 1-6: [0] "1. Seed: the implement refused.",
        /// [1] the advice ("It is using its own rate setting, not RC's map."). CanAskAgain: any of them in cases 1-4.</summary>
        public List<string[]> RxLines(out bool CanAskAgain)
        {
            List<string[]> Result = new List<string[]>();
            CanAskAgain = false;
            for (int i = 0; i < RateProducts; i++)
            {
                if (Sending[i])
                {
                    clsProduct Prd = Core.Products.Item(i);
                    RxCases Case = Prd.RxCase();
                    if (Case != RxCases.None)
                    {
                        Result.Add(new string[] { (i + 1).ToString() + ". " + Prd.ProductName + ": " + clsProduct.RxCaseText(Case),
                            clsProduct.RxCaseAdvice(Case) });
                        if (Case <= RxCases.NoReply) CanAskAgain = true;
                    }
                }
            }
            return Result;
        }

        /// <summary>"Ask again" for every product in case 1-4 (red Rx dialog).</summary>
        public void AskAgainAll()
        {
            for (int i = 0; i < RateProducts; i++)
            {
                RxCases Case = Sending[i] ? Core.Products.Item(i).RxCase() : RxCases.None;
                if (Case >= RxCases.TurnedOff && Case <= RxCases.NoReply) AskAgain(i);
            }
        }

        /// <summary>"Ask again" (red Rx dialog): DDI 158 = 1 once for the product's channel, and a new request
        /// for telling "turned off" from "refused". Nothing for a channel that isn't announced. About 6 s later
        /// the result is checked: refused, error or no reply gets one popup.</summary>
        public void AskAgain(int ProductID)
        {
            try
            {
                if (Announced(ProductID))
                {
                    Send(DDIrxState, 1, (byte)(ProductID + 1));
                    Core.Products.Item(ProductID).RxRequest();
                    AskedTime[ProductID] = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                Core.PgnLog.Note(PgnLink.TC, "ask again failed: " + ex.Message);
                CloseSocket();
            }
        }

        /// <summary>The prescription state AOG-TaskController reports for a product (0xF5, step 3). Fresh: a frame
        /// within 3 s. Matched: it has elements. Extended: a 16-byte frame, so RxState, Has158, NoReply and RxMode
        /// are valid. Returns Fresh.</summary>
        public bool ImplementStatus(int ProductID, out bool Matched, out bool Extended, out byte RxState, out bool Has158, out bool NoReply, out byte RxMode)
        {
            bool Fresh = false;
            Matched = false;
            Extended = false;
            RxState = 0;
            Has158 = false;
            NoReply = false;
            RxMode = 0;
            if (ProductID >= 0 && ProductID < RateProducts)
            {
                lock (ActualsLock)
                {
                    TCActuals A = Actuals[ProductID];
                    Fresh = (DateTime.Now - A.Received).TotalMilliseconds < ActualsFreshMs;
                    if (Fresh)
                    {
                        Matched = A.Elements > 0;
                        Extended = A.Extended;
                        RxState = A.RxState;
                        Has158 = A.Has158;
                        NoReply = A.NoReply;
                        RxMode = A.RxMode;
                    }
                }
            }
            return Fresh;
        }

        /// <summary>True in the first seconds after RC starts: no 0xF5 yet is normal, not "no TC".</summary>
        public bool StartupHold
        {
            get { return (DateTime.Now - StartTime).TotalSeconds < StartupHoldSeconds; }
        }

        private void CloseSocket()
        {
            try { Udp?.Close(); } catch { }
            Udp = null;
            Receiving = false;
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            UdpClient Client = (UdpClient)ar.AsyncState;
            bool Again = true;
            try
            {
                IPEndPoint From = new IPEndPoint(IPAddress.Any, 0);
                byte[] Data = Client.EndReceive(ar, ref From);
                Core.PgnLog.Add(PgnLink.TC, false, Data);
                ParseActuals(Data);
            }
            catch (ObjectDisposedException)
            {
                Again = false;      // socket closed (Stop, or reopened after an error)
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTaskControllerRate/ReceiveCallback: " + ex.Message);
            }

            if (Again)
            {
                try
                {
                    Client.BeginReceive(ReceiveCallback, Client);
                }
                catch (Exception ex)
                {
                    if (!(ex is ObjectDisposedException)) Props.WriteErrorLog("clsTaskControllerRate/BeginReceive: " + ex.Message);
                }
            }
        }

        private void ParseActuals(byte[] Data)
        {
            // 80 81 80 F5 0F [15 bytes] [checksum]; step 3: 80 81 80 F5 10 [16 bytes] [checksum], byte 15 = DDI 288,
            // flags bits 2-3 = DDI 158, bit 4 = has 158, bit 5 = no reply. A frame with elements = 0 is stored too:
            // the TC is there but nothing is matched to the channel (case 6); ImplementActuals still needs elements.
            if (Data != null && Data.Length >= 21 && Data[0] == 0x80 && Data[1] == 0x81 && Data[3] == 0xF5 && Data[4] >= 15)
            {
                int Channel = Data[5];
                if (Channel >= 1 && Channel <= RateProducts)
                {
                    bool Extended = Data[4] >= 16 && Data.Length >= 22;
                    lock (ActualsLock)
                    {
                        TCActuals A = Actuals[Channel - 1];
                        A.ActualDDI = (ushort)(Data[6] | Data[7] << 8);
                        A.Rate = BitConverter.ToInt32(Data, 8);
                        A.TotalDDI = (ushort)(Data[12] | Data[13] << 8);
                        A.Total = BitConverter.ToInt32(Data, 14);
                        A.Elements = Data[18];
                        A.Flags = Data[19];
                        A.Extended = Extended;
                        A.RxState = (byte)((Data[19] >> 2) & 3);
                        A.Has158 = Extended && (Data[19] & 0x10) != 0;
                        A.NoReply = Extended && (Data[19] & 0x20) != 0;
                        A.RxMode = Extended ? Data[20] : (byte)0;
                        A.Received = DateTime.Now;
                    }
                }
            }
        }

        private static byte[] BuildFrame(ushort DDI, int Value, byte Channel)
        {
            byte[] Data = new byte[13];
            Data[0] = 0x80;
            Data[1] = 0x81;
            Data[2] = 0x7F;     // source: AgIO, the only one AOG-TaskController accepts
            Data[3] = 0xF2;     // process data
            Data[4] = 7;        // DDI, value, channel
            Data[5] = (byte)DDI;
            Data[6] = (byte)(DDI >> 8);
            Data[7] = (byte)Value;
            Data[8] = (byte)(Value >> 8);
            Data[9] = (byte)(Value >> 16);
            Data[10] = (byte)(Value >> 24);
            Data[11] = Channel;

            int Sum = 0;
            for (int i = 2; i < 12; i++) Sum += Data[i];
            Data[12] = (byte)Sum;
            return Data;
        }

        // 127.0.0.1 plus this PC's own IPv4 addresses on adapters that are up, refreshed every few seconds
        private List<IPAddress> Destinations()
        {
            if ((DateTime.Now - LocalAddressesTime).TotalMilliseconds >= AddressRefreshMs)
            {
                List<IPAddress> Found = new List<IPAddress> { IPAddress.Loopback };
                try
                {
                    foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (Nic.OperationalStatus == OperationalStatus.Up && Nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                        {
                            foreach (UnicastIPAddressInformation Info in Nic.GetIPProperties().UnicastAddresses)
                            {
                                if (Info.Address.AddressFamily == AddressFamily.InterNetwork && !Found.Contains(Info.Address)) Found.Add(Info.Address);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsTaskControllerRate/Destinations: " + ex.Message);
                }
                LocalAddresses = Found;
                LocalAddressesTime = DateTime.Now;
            }
            return LocalAddresses;
        }

        // channel info frame (PGN 0xF4): channel, DDI (0 = not in use), name cut to MaxNameBytes on a UTF-8 character boundary
        private static byte[] BuildInfoFrame(byte Channel, ushort DDI, string Name)
        {
            byte[] NameBytes = System.Text.Encoding.UTF8.GetBytes(Name ?? "");
            int NameLength = Math.Min(NameBytes.Length, MaxNameBytes);
            while (NameLength > 0 && NameLength < NameBytes.Length && (NameBytes[NameLength] & 0xC0) == 0x80) NameLength--;

            byte[] Data = new byte[9 + NameLength];
            Data[0] = 0x80;
            Data[1] = 0x81;
            Data[2] = 0x7F;     // source: AgIO, as the setpoints
            Data[3] = 0xF4;     // channel info
            Data[4] = (byte)(3 + NameLength);
            Data[5] = Channel;
            Data[6] = (byte)DDI;
            Data[7] = (byte)(DDI >> 8);
            Array.Copy(NameBytes, 0, Data, 8, NameLength);

            int Sum = 0;
            for (int i = 2; i < Data.Length - 1; i++) Sum += Data[i];
            Data[Data.Length - 1] = (byte)Sum;
            return Data;
        }

        private void Send(ushort DDI, int Value, byte Channel)
        {
            SendFrame(BuildFrame(DDI, Value, Channel));
        }

        // every channel's type and name while any product sends: on a change and every InfoResendMs;
        // when the last product stops, DDI 0 for every channel once
        private void SendChannelInfo(DateTime Now)
        {
            bool Any = false;
            for (int i = 0; i < RateProducts; i++) Any |= Sending[i];

            if (Any || AnySending)
            {
                for (int i = 0; i < RateProducts; i++)
                {
                    clsProduct Prd = Core.Products.Item(i);
                    ushort DDI = Sending[i] ? Prd.TCddi() : (ushort)0;
                    string Name = DDI != 0 ? Prd.ProductName : "";
                    if (!Any || DDI != LastInfoDDI[i] || Name != LastInfoName[i] || (Now - LastInfoSent[i]).TotalMilliseconds >= InfoResendMs)
                    {
                        SendFrame(BuildInfoFrame((byte)(i + 1), DDI, Name));
                        LastInfoDDI[i] = DDI;
                        LastInfoName[i] = Name;
                        LastInfoSent[i] = Now;
                    }
                }
            }
            AnySending = Any;
        }

        // Ask again's one popup: refused, error or no reply once AOG-TC has had time to confirm. Nothing when the
        // implement took it (158 = 1); lasting conditions are shown by the red Rx button and the alarm.
        private void CheckAskResults(DateTime Now)
        {
            for (int i = 0; i < RateProducts; i++)
            {
                if (AskedTime[i].HasValue && (Now - AskedTime[i].Value).TotalSeconds >= AskResultSeconds)
                {
                    AskedTime[i] = null;
                    clsProduct Prd = Core.Products.Item(i);
                    RxCases Case = Prd.RxCase();
                    if (Case == RxCases.Refused || Case == RxCases.Error || Case == RxCases.NoReply)
                    {
                        Props.ShowMessage((i + 1).ToString() + ". " + Prd.ProductName + ": " + clsProduct.RxCaseText(Case)
                            + " " + clsProduct.RxCaseAdvice(Case), Language.Lang.lgRxPopupTitle, 10000);
                    }
                }
            }
        }

        private void SendFrame(byte[] Frame)
        {
            if (Udp == null)
            {
                Udp = new UdpClient();
                // without this an ICMP "port unreachable" (a destination address with nothing on 8888) makes the
                // next receive fail with WSAECONNRESET
                const int SIO_UDP_CONNRESET = -1744830452;
                Udp.Client.IOControl(SIO_UDP_CONNRESET, new byte[] { 0 }, null);
            }
            Core.PgnLog.Add(PgnLink.TC, true, Frame);     // once per frame, not per destination
            foreach (IPAddress Address in Destinations())
            {
                Udp.Send(Frame, Frame.Length, new IPEndPoint(Address, TCport));
            }

            // the socket has a port once it has sent: AOG-TaskController's actuals (0xF5) come back to it
            if (!Receiving)
            {
                Receiving = true;
                Udp.BeginReceive(ReceiveCallback, Udp);
            }
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Step 3 (Group2_Step3_Prescription_State_Design.md): a channel is announced (0xF4) while its product
                // is ISOBUS, enabled and its unit can be sent, whatever VR is. VR on: RC's rate as setpoints. VR off:
                // DDI 158 = 0 every second, so the implement uses its own rate. A channel that stops gets no 0 setpoint:
                // its 0xF4 with DDI 0 hands control back.
                DateTime Now = DateTime.Now;
                bool VR = Props.VariableRateEnabled;
                bool VRchanged = VR != LastVR;
                LastVR = VR;
                if (VRchanged && AnySending)
                {
                    Props.WriteActivityLog("Task controller rate: " + (VR ? "VR on, RC's rate" : "VR off, implement's own rate (DDI 158 = 0)"));
                }

                for (int i = 0; i < RateProducts; i++)
                {
                    clsProduct Prd = Core.Products.Item(i);
                    ushort DDI = Prd.TCddi();
                    bool On = Prd.Enabled && DDI != 0;
                    byte Channel = (byte)(i + 1);

                    if (On != Sending[i])
                    {
                        Sending[i] = On;
                        Props.WriteActivityLog("Task controller rate, product " + Channel.ToString()
                            + (On ? ": on channel " + Channel.ToString() + ", DDI " + DDI.ToString() : ": stopped"));
                        LastDDI[i] = 0;
                        LastRxOffSent[i] = DateTime.MinValue;
                        if (On && VR) Prd.RxRequest();      // a new request: case 1 vs 2 counts from here
                    }
                    else if (On && VRchanged)
                    {
                        LastDDI[i] = 0;                     // the first message of the new mode goes at once
                        LastRxOffSent[i] = DateTime.MinValue;
                        if (VR) Prd.RxRequest();
                    }

                    if (On)
                    {
                        if (VR)
                        {
                            int Value = Prd.TCvalue();
                            if (Value != LastValue[i] || DDI != LastDDI[i] || (Now - LastSent[i]).TotalMilliseconds >= ResendMs)
                            {
                                Send(DDI, Value, Channel);
                                LastValue[i] = Value;
                                LastDDI[i] = DDI;
                                LastSent[i] = Now;
                            }
                        }
                        else if ((Now - LastRxOffSent[i]).TotalMilliseconds >= ResendMs)
                        {
                            Send(DDIrxState, 0, Channel);
                            LastRxOffSent[i] = Now;
                        }
                    }
                }

                SendChannelInfo(Now);
                CheckAskResults(Now);
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    // e.g. an adapter unplugged: every 200 ms tick fails, so a note once a minute, not an error each time
                    if ((DateTime.Now - LastFailNote).TotalSeconds >= FailNoteSeconds)
                    {
                        Core.PgnLog.Note(PgnLink.TC, "send failed: " + ex.Message);
                        LastFailNote = DateTime.Now;
                    }
                }
                else
                {
                    Props.WriteErrorLog("clsTaskControllerRate/SendTimer_Tick: " + ex.Message);
                }
                CloseSocket();     // opened again on the next send
            }
        }
    }
}
