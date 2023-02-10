using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RateController
{
    public class UDPComm
    {
        private bool cIsUDPSendConnected;
        private readonly FormStart mf;
        private byte[] buffer = new byte[1024];

        private readonly string cDestinationIP;

        private IPAddress cEthernetEP;

        // local ports must be unique for each app on same pc and each class instance
        private int cReceivePort;

        private int cSendFromPort;
        private int cSendToPort;
        private bool cUpdateDestinationIP;
        private readonly bool cUseLoopback;

        // wifi endpoint address
        private IPAddress cWiFiEP;

        private string cWiFiIP;

        // local wifi ip address
        private HandleDataDelegateObj HandleDataDelegate = null;

        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        public UDPComm(FormStart CallingForm, int ReceivePort, int SendToPort
            , int SendFromPort, string DestinationIP = "", bool UseLoopBack = false
            , bool UpdateDestinationIP = false)
        {
            mf = CallingForm;
            cReceivePort = ReceivePort;
            cSendToPort = SendToPort;
            cSendFromPort = SendFromPort;
            cUseLoopback = UseLoopBack;
            cUpdateDestinationIP = UpdateDestinationIP;
            cDestinationIP = DestinationIP;

            SetEndPoints();

            NetworkChange.NetworkAddressChanged += new NetworkAddressChangedEventHandler(AddressChanged);
        }

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg);

        public string EthernetEP
        {
            get { return cEthernetEP.ToString(); }
            set
            {
                IPAddress IP;
                string[] data;

                if (IPAddress.TryParse(value, out IP))
                {
                    data = value.Split('.');
                    cEthernetEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    mf.Tls.SaveProperty("EthernetEP", value);
                }
            }
        }

        public void Close()
        {
            recvSocket.Close();
            sendSocket.Close();
        }

        public string WifiEP
        {
            get { return cWiFiEP.ToString(); }
            set
            {
                IPAddress IP;
                string[] data;

                if (IPAddress.TryParse(value, out IP))
                {
                    data = value.Split('.');
                    cWiFiEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    cWiFiIP = value;
                    mf.Tls.SaveProperty("WifiIP", value);
                }
            }
        }

        public bool IsUDPSendConnected { get => cIsUDPSendConnected; set => cIsUDPSendConnected = value; }

        public string EthernetIP()
        {
            string Adr;
            IPAddress IP;
            string Result;

            Adr = GetLocalIPv4(NetworkInterfaceType.Ethernet);
            if (IPAddress.TryParse(Adr, out IP))
            {
                Result = IP.ToString();
            }
            else
            {
                Result = "127.0.0.1";
            }
            return Result;
        }

        //sends byte array
        public void SendUDPMessage(byte[] byteData)
        {
            if (IsUDPSendConnected)
            {
                try
                {
                    if (byteData.Length != 0)
                    {
                        // ethernet
                        IPEndPoint EndPt = new IPEndPoint(cEthernetEP, cSendToPort);
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);

                        if (!cUseLoopback)
                        {
                            // wifi
                            EndPt = new IPEndPoint(cWiFiEP, cSendToPort);
                            sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("UDPcomm/SendUDPMessage " + ex.Message);
                }
            }
        }

        public void StartUDPServer()
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
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);
                IsUDPSendConnected = true;
            }
            catch (Exception e)
            {
                //mf.Tls.ShowHelp("UDP start error: \n" + e.Message, "Comm", 3000, true);
                mf.Tls.WriteErrorLog("StartUDPServer: \n" + e.Message);
            }
        }

        public string WifiIP()
        {
            return cWiFiIP;
        }

        private void AddressChanged(object sender, EventArgs e)
        {
            if (cUpdateDestinationIP) SetEndPoints();
            mf.Tls.WriteActivityLog("UDPcomm: Network Address Changed");
        }

        private string GetLocalIPv4(NetworkInterfaceType _type)
        {
            // https://stackoverflow.com/questions/6803073/get-local-ip-address

            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }

        private void HandleData(int Port, byte[] Data)
        {
            try
            {
                if (Data.Length > 1)
                {
                    PGN = Data[0] << 8 | Data[1];   // AGIO big endian
                    if (PGN == 32897)
                    {
                        if (Data.Length > 2)
                        {
                            // AGIO
                            switch (Data[3])
                            {
                                case 254:
                                    // AutoSteer AGIO PGN
                                    mf.AutoSteerPGN.ParseByteData(Data);
                                    break;

                                case 230:
                                    // vr data
                                    mf.VRdata.ParseByteData(Data);
                                    break;

                                case 235:
                                    // section widths
                                    mf.SectionsPGN.ParseByteData(Data);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        PGN = Data[1] << 8 | Data[0];   // rc modules little endian
                        switch (PGN)
                        {
                            case 32503:
                                mf.WifiStatus.ParseByteData(Data);
                                break;

                            case 32501:
                            case 32613:
                                foreach (clsProduct Prod in mf.Products.Items)
                                {
                                    Prod.UDPcommFromArduino(Data, PGN);
                                }
                                break;

                            case 32618:
                                mf.SwitchBox.ParseByteData(Data);
                                break;

                            case 32621:
                                mf.PressureData.ParseByteData(Data);
                                break;

                            case 0xABC:
                                // debug info from module
                                Debug.Print("");
                                for (int i = 0; i < Data.Length; i++)
                                {
                                    Debug.Print(DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + "  " + i.ToString() + " " + Data[i].ToString());
                                }
                                Debug.Print("");
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("UDPcomm/HandleData " + ex.Message);
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                // Initialise the IPEndPoint for the client
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                // Receive all data
                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                // Listen for more connections again...
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                int port = ((IPEndPoint)epSender).Port;
                // Update status through a delegate
                mf.Invoke(HandleDataDelegate, new object[] { port, localMsg });
            }
            catch (System.ObjectDisposedException)
            {
                // do nothing
            }
            catch (Exception ex)
            {
                //mf.Tls.ShowHelp("ReceiveData Error \n" + e.Message, "Comm", 3000, true);
                mf.Tls.WriteErrorLog("UDPcomm/ReceiveData " + ex.Message);
            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog(" UDP Send Data" + ex.ToString());
            }
        }

        private void SetEndPoints()
        {
            string Adr;
            IPAddress IP;
            string[] data;

            try
            {
                // ethernet
                cEthernetEP = IPAddress.Parse("192.168.1.255");
                if (IPAddress.TryParse(cDestinationIP, out IP))
                {
                    // keep pre-defined address
                    cEthernetEP = IP;
                }

                // wifi
                cWiFiIP = "127.0.0.1";
                cWiFiEP = IPAddress.Parse(cWiFiIP);
                Adr = GetLocalIPv4(NetworkInterfaceType.Wireless80211);
                if (IPAddress.TryParse(Adr, out IP))
                {
                    data = Adr.Split('.');
                    cWiFiEP = IPAddress.Parse(data[0] + "." + data[1] + "." + data[2] + ".255");
                    cWiFiIP = Adr;
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("UDPcomm/SetEndPoints " + ex.Message);
            }
        }
    }
}