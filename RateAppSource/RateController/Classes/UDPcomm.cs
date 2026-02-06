using RateController.Classes;
using RateController.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class UDPComm
    {
        private readonly frmMain mf;
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

        public UDPComm(frmMain CallingForm, int ReceivePort, int SendToPort, int SendFromPort, string ConnectionName, string DestinationEndPoint = "")
        {
            mf = CallingForm;
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cConnectionName = ConnectionName;
            SetEP(DestinationEndPoint);
            Props.WriteLog("Ethernet Log.txt", "", true, true);
        }

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg);

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
                    cNetworkEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    Props.SetProp("EndPoint_" + cConnectionName, value);
                    cSubNet = data[0].ToString() + "." + data[1].ToString() + "." + data[2].ToString();
                }
            }
        }

        public string SubNet
        { get { return cSubNet; } }

        public string Log()
        {
            return cLog;
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
                recvSocket.Bind(new IPEndPoint(IPAddress.Any, cReceivePort));

                // initialize the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

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

        private void HandleData(int Port, byte[] Data)
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
                            foreach (clsProduct Prod in Core.Products.Items)
                            {
                                Prod.UDPcommFromArduino(Data, PGN);
                            }
                            break;

                        case 32401:
                            Core.ModulesStatus.ParseByteData(Data);
                            break;

                        case 32618:
                            Core.SwitchBox.ParseByteData(Data);
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
                            mf.BeginInvoke(HandleDataDelegate, new object[] { port, localMsg });
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