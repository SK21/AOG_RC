using RateController.Classes;
using RateController.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class UDPComm
    {
        // learned subnets: a subnet modules were heard on drops out this long after the last packet
        private const int LearnedExpirySec = 30;

        private readonly frmMain mf;
        private readonly Dictionary<string, DateTime> cLearned = new Dictionary<string, DateTime>();   // subnet broadcast -> last heard (UTC), socket thread writes
        private readonly Dictionary<string, string> cBroadcastOf = new Dictionary<string, string>();  // sender address -> its subnet broadcast (BroadcastFor), under LearnedLock
        private readonly bool cLearnSubnets;
        private readonly HashSet<string> cLearnedLogged = new HashSet<string>();    // what the log last reported, send side only
        private readonly object LearnedLock = new object();
        private readonly Dictionary<int, string> cModuleIP = new Dictionary<int, string>();      // module ID -> last sender address of its PGN 32401
        private byte[] buffer = new byte[1024];
        private string cConnectionName;
        private string cLog;
        private IPAddress cNetworkEP;
        private int cReceivePort;   // local ports must be unique for each app on same pc and each class instance
        private int cSendFromPort;
        private int cSendToPort;
        private string cSubNet;
        private HandleDataDelegateObj HandleDataDelegate = null;
        private Socket recvSocket;
        private volatile bool Running = false;
        private Socket sendSocket;

        public UDPComm(frmMain CallingForm, int ReceivePort, int SendToPort, int SendFromPort, string ConnectionName, string DestinationEndPoint = "", bool LearnSubnets = false)
        {
            // LearnSubnets: also send to every subnet data has arrived from recently, not just the
            // selected one. Modules on the tablet hotspot (192.168.137.x) or the PC's WiFi are heard
            // through the any-address receive socket but were never sent to
            mf = CallingForm;
            cLearnSubnets = LearnSubnets;
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cConnectionName = ConnectionName;
            SetEP(DestinationEndPoint);
            Props.WriteLog("Ethernet Log.txt", "", true, true);
        }

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg, IPAddress from);

        // While true, Send also goes to 255.255.255.255 on every adapter (SendAllNetworks):
        // set only for the one Send to a board heard on another network (New_Board_Any_Network.md)
        public bool AllNetworks { get; set; } = false;

        public bool IsRunning
        { get { return Running; } }

        public string NetworkEP
        {
            get { return cNetworkEP.ToString(); }
            set
            {
                string[] data;
                if (IPAddress.TryParse(value, out _))
                {
                    data = value.Split('.');
                    cNetworkEP = BroadcastFor(IPAddress.Parse(value));
                    Props.SetProp("EndPoint_" + cConnectionName, value);
                    cSubNet = data[0].ToString() + "." + data[1].ToString() + "." + data[2].ToString();
                }
            }
        }

        public string SubNet
        { get { return cSubNet; } }

        public static string AdapterName(string Subnet)
        {
            // the PC adapter with an IPv4 address on Subnet (a.b.c), "Tablet hotspot" for the
            // Windows mobile hotspot, "" when no adapter that is up is on it
            string Result = "";
            try
            {
                foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (Result == "" && Nic.OperationalStatus == OperationalStatus.Up && Nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (UnicastIPAddressInformation Info in Nic.GetIPProperties().UnicastAddresses)
                        {
                            if (Result == "" && Info.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                byte[] B = Info.Address.GetAddressBytes();
                                if (B[0] + "." + B[1] + "." + B[2] == Subnet)
                                {
                                    Result = IsTabletHotspot(Nic, Info.Address) ? "Tablet hotspot" : Nic.Name;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/AdapterName: " + ex.Message);
            }
            return Result;
        }

        public static IPAddress BroadcastFor(IPAddress Address)
        {
            // subnet broadcast for Address, from the mask of the PC adapter on its a.b.c (same
            // match as AdapterName). An iPhone hotspot is 172.20.10.0/28, broadcast .15, where
            // a.b.c.255 is off-subnet and is dropped by the phone. No adapter: a.b.c.255 as before
            byte[] A = Address.GetAddressBytes();
            IPAddress Result = IPAddress.Parse(A[0] + "." + A[1] + "." + A[2] + ".255");
            try
            {
                bool Found = false;
                foreach (NetworkInterface Nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (!Found && Nic.OperationalStatus == OperationalStatus.Up && Nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (UnicastIPAddressInformation Info in Nic.GetIPProperties().UnicastAddresses)
                        {
                            if (!Found && Info.Address.AddressFamily == AddressFamily.InterNetwork && Info.IPv4Mask != null)
                            {
                                byte[] L = Info.Address.GetAddressBytes();
                                byte[] M = Info.IPv4Mask.GetAddressBytes();
                                if (L[0] == A[0] && L[1] == A[1] && L[2] == A[2] && M.Length == 4)
                                {
                                    byte[] B = new byte[4];
                                    for (int i = 0; i < 4; i++) B[i] = (byte)(L[i] | ~M[i]);
                                    Result = new IPAddress(B);
                                    Found = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPComm/BroadcastFor: " + ex.Message);
            }
            return Result;
        }

        public static bool IsTabletHotspot(NetworkInterface Nic, IPAddress Address)
        {
            // the Windows mobile hotspot runs on a Wi-Fi Direct virtual adapter at 192.168.137.1
            return Nic.Description.IndexOf("Wi-Fi Direct Virtual", StringComparison.OrdinalIgnoreCase) >= 0
                || Address.ToString() == "192.168.137.1";
        }

        public string ModuleIP(int ID)
        {
            // where module ID's status packets last came from, "" if never heard (LearnSubnets only).
            // Not aged: callers check Core.ModulesStatus.Connected first
            string Result = "";
            lock (LearnedLock)
            {
                if (cModuleIP.TryGetValue(ID, out string IP)) Result = IP;
            }
            return Result;
        }

        public string Log()
        {
            return cLog;
        }

        // 255.255.255.255 on every IPv4 adapter that is up, so a board on any subnet of the
        // LAN hears it. Not the tablet hotspot: only WiFi modules are on it, whose address
        // comes from the hotspot. Used by 32503 (subnet change) and by AllNetworks.
        public static void SendAllNetworks(byte[] Data, int Port)
        {
            IPEndPoint epAll = new IPEndPoint(IPAddress.Broadcast, Port);
            foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                try
                {
                    if (nic.Supports(NetworkInterfaceComponent.IPv4) && nic.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var info in nic.GetIPProperties().UnicastAddresses)
                        {
                            if (info.Address.AddressFamily == AddressFamily.InterNetwork &&
                                !IPAddress.IsLoopback(info.Address) &&
                                info.IPv4Mask != null &&
                                !IsTabletHotspot(nic, info.Address))
                            {
                                using (Socket s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                                {
                                    try
                                    {
                                        s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                                        s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                        s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);
                                        s.Bind(new IPEndPoint(info.Address, 9578));
                                        s.SendTo(Data, 0, Data.Length, SocketFlags.None, epAll);
                                    }
                                    catch (Exception ex)
                                    {
                                        Props.WriteErrorLog("UDPComm/SendAllNetworks/Bind error " + ex.Message);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("UDPComm/SendAllNetworks/nic loop error " + ex.Message);
                }
            }
        }

        public void Send(byte[] byteData)
        {
            if (Running && sendSocket != null)
            {
                try
                {
                    int PGN = byteData[0] | byteData[1] << 8;
                    AddToLog("               > " + PGN.ToString());

                    if (byteData.Length != 0)
                    {
                        // network
                        IPEndPoint EndPt = new IPEndPoint(cNetworkEP, cSendToPort);
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(HandleSend), null);

                        // other subnets modules are heard on (tablet hotspot, PC WiFi, second adapter)
                        if (cLearnSubnets)
                        {
                            foreach (IPAddress Learned in LearnedTargets())
                            {
                                IPEndPoint Ep = new IPEndPoint(Learned, cSendToPort);
                                sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, Ep, new AsyncCallback(HandleSend), null);
                            }
                        }

                        // a board on another network (New_Board_Any_Network.md)
                        if (AllNetworks) SendAllNetworks(byteData, cSendToPort);
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("UDPcomm/Send " + ex.Message);
                }
            }
        }

        public void Start()
        {
            try
            {
                // initialize the delegate which updates the message received
                HandleDataDelegate = HandleData;

                // initialize the receive socket
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                recvSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                recvSocket.Bind(new IPEndPoint(IPAddress.Any, cReceivePort));

                // initialize the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                sendSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // Initialise the IPEndPoint for the server to send on port
                IPEndPoint server = new IPEndPoint(IPAddress.Any, cSendFromPort);
                sendSocket.Bind(server);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(Receive), recvSocket);
                Running = true;
            }
            catch (Exception e)
            {
                Props.WriteErrorLog("UDPcomm/Start: \n" + e.Message);
            }
        }

        public void Stop()
        {
            if (Running)
            {
                Running = false;

                try { recvSocket?.Close(); } catch { }
                try { sendSocket?.Close(); } catch { }

                recvSocket = null;
                sendSocket = null;

                Props.WriteLog("Ethernet Log.txt", cLog);
            }
        }

        public void UpdateLog()
        {
            Props.WriteLog("Ethernet Log.txt", cLog);
            cLog = "";
        }

        private void AddToLog(string NewData)
        {
            cLog += DateTime.Now.Second.ToString() + "  " + NewData + Environment.NewLine;
            if (cLog.Length > 100000)
            {
                cLog = cLog.Substring(cLog.Length - 98000, 98000);
            }
            cLog = cLog.Replace("\0", string.Empty);
        }

        private void HandleData(int Port, byte[] Data, IPAddress From)
        {
            try
            {
                if (Data.Length > 1 && !Core.IsShuttingDown)
                {
                    int PGN = Data[1] << 8 | Data[0];   // rc modules little endian
                    AddToLog("< " + PGN.ToString());

                    switch (PGN)
                    {
                        case 32400:
                            if (!Props.CanEnabled)
                            {
                                foreach (clsProduct Prod in Core.Products.Items)
                                {
                                    Prod.UDPcommFromArduino(Data, PGN);
                                }
                            }
                            break;

                        case 32401:
                            if (!Props.CanEnabled) Core.ModulesStatus.ParseByteData(Data);
                            break;

                        case 32403:
                            if (!Props.CanEnabled) Core.ModulesBoardID.ParseByteData(Data, From);
                            break;

                        case 32402:
                            Core.PgnPidLog.ParseByteData(Data);
                            break;

                        case 32618:
                            Core.SwitchBox.ParseByteData(Data);
                            break;

                        case 32296:
                            Core.ScaleIndicator.ParseByteData(Data);
                            break;

                        case 33152: // AOG, 0x81, 0x80
                            switch (Data[3])
                            {
                                case 100:
                                    // AOG roll corrected lat,lon
                                    Core.GPS.GPS0.ParseByteData(Data);
                                    break;

                                case 208:
                                    // Twol gps
                                    Core.GPS.ParseByteData(Data);
                                    break;

                                case 229:
                                    // aog sections
                                    Core.AOGsections.ParseByteData(Data);
                                    break;

                                case 235:
                                    // section widths
                                    Core.SectionsPGN.ParseByteData(Data);
                                    break;

                                case 238:
                                    // machine config
                                    Core.MachineConfig.ParseByteData(Data);
                                    break;

                                case 239:
                                    // machine data
                                    Core.MachineData.ParseByteData(Data);
                                    break;

                                case 254:
                                    // AutoSteer AGIO PGN
                                    Core.AutoSteerPGN.ParseByteData(Data);
                                    break;
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPcomm/HandleData " + ex.Message);
            }
        }

        private void HandleSend(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog(" UDP Send Data" + ex.ToString());
            }
        }

        private void Learn(IPAddress Sender)
        {
            // socket thread. The sender's subnet broadcast (BroadcastFor, same as NetworkEP),
            // cached per sender so adapters are not enumerated on every packet.
            // Loopback and link-local (no DHCP) are skipped
            byte[] B = Sender.GetAddressBytes();
            if (B.Length == 4 && !IPAddress.IsLoopback(Sender) && !(B[0] == 169 && B[1] == 254))
            {
                string Key = Sender.ToString();
                string Bc;
                lock (LearnedLock)
                {
                    cBroadcastOf.TryGetValue(Key, out Bc);
                }
                if (Bc == null)
                {
                    Bc = BroadcastFor(Sender).ToString();
                    lock (LearnedLock)
                    {
                        cBroadcastOf[Key] = Bc;
                    }
                }
                lock (LearnedLock)
                {
                    cLearned[Bc] = DateTime.UtcNow;
                }
            }
        }

        private List<IPAddress> LearnedTargets()
        {
            // learned subnets heard from within LearnedExpirySec, the selected one excluded (it is
            // always sent to). Expired ones are dropped. Changes are written to the Ethernet log
            List<IPAddress> Result = new List<IPAddress>();
            List<string> Live = new List<string>();
            string Selected = cNetworkEP.ToString();
            lock (LearnedLock)
            {
                List<string> Expired = new List<string>();
                foreach (KeyValuePair<string, DateTime> Kv in cLearned)
                {
                    if ((DateTime.UtcNow - Kv.Value).TotalSeconds > LearnedExpirySec)
                    {
                        Expired.Add(Kv.Key);
                    }
                    else if (Kv.Key != Selected)
                    {
                        Live.Add(Kv.Key);
                    }
                }
                foreach (string K in Expired) cLearned.Remove(K);
            }

            foreach (string K in Live)
            {
                Result.Add(IPAddress.Parse(K));
                if (cLearnedLogged.Add(K)) AddToLog("+ subnet " + K);
            }
            List<string> Gone = new List<string>();
            foreach (string K in cLearnedLogged)
            {
                if (!Live.Contains(K)) Gone.Add(K);
            }
            foreach (string K in Gone)
            {
                cLearnedLogged.Remove(K);
                AddToLog("- subnet " + K + " (no module " + LearnedExpirySec + " s)");
            }
            return Result;
        }

        private void Receive(IAsyncResult asyncResult)
        {
            if (Running)
            {
                try
                {
                    // Initialise the IPEndPoint for the client
                    EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                    // Receive all data
                    int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                    byte[] localMsg = null;
                    int port = 0;

                    if (msgLen > 0)
                    {
                        localMsg = new byte[msgLen];
                        Array.Copy(buffer, localMsg, msgLen);
                        port = ((IPEndPoint)epSender).Port;
                        if (cLearnSubnets)
                        {
                            Learn(((IPEndPoint)epSender).Address);

                            // PGN 32401 module status, byte 2 = raw module ID: remember where it lives
                            if (msgLen > 2 && localMsg[0] == 145 && localMsg[1] == 126)
                            {
                                lock (LearnedLock)
                                {
                                    cModuleIP[localMsg[2]] = ((IPEndPoint)epSender).Address.ToString();
                                }
                            }
                        }
                    }

                    // Listen for more connections again...
                    try
                    {
                        if (Running && recvSocket != null)
                        {
                            EndPoint nextSender = new IPEndPoint(IPAddress.Any, 0);
                            recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref nextSender, new AsyncCallback(Receive), recvSocket);
                        }
                    }
                    catch (System.ObjectDisposedException)
                    {
                        // socket closed during shutdown
                    }

                    // Update status through a delegate (avoid blocking the socket thread)
                    if (Running && !Core.IsShuttingDown && msgLen > 0 && HandleDataDelegate != null && mf != null && mf.IsHandleCreated && !mf.IsDisposed)
                    {
                        try
                        {
                            mf.BeginInvoke(HandleDataDelegate, new object[] { port, localMsg, ((IPEndPoint)epSender).Address });
                        }
                        catch (InvalidOperationException)
                        {
                            // form might be closing/disposed
                        }
                    }
                }
                catch (System.ObjectDisposedException)
                {
                    // do nothing
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("UDPcomm/Receive " + ex.Message);
                }
            }
        }

        private void SetEP(string DestinationEndPoint)
        {
            try
            {
                if (IPAddress.TryParse(DestinationEndPoint, out _))
                {
                    NetworkEP = DestinationEndPoint;
                }
                else
                {
                    string EP = Props.GetProp("EndPoint_" + cConnectionName);
                    if (IPAddress.TryParse(EP, out _))
                    {
                        NetworkEP = EP;
                    }
                    else
                    {
                        NetworkEP = "192.168.1.255";
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("UDPcomm/SetEP " + ex.Message);
            }
        }
    }
}