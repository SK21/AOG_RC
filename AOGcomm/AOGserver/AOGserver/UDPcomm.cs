using System;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace AOGserver
{
    class UDPcomm
    {
        private readonly Form1 mf;
        public bool Connected;
        private string cEndIP;

        private IPAddress epIP;
        private delegate void HandleDataDelegateObj(byte[] msg);
        private HandleDataDelegateObj HandleDataDelegate = null;
        private byte[] buffer = new byte[1024];

        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        public UDPcomm(Form1 CallingForm, string EndIP)
        {
            mf = CallingForm;
            cEndIP = EndIP;
        }

        public void Start()
        {
            try
            {
                HandleDataDelegate = HandleData;
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, 9999);
                recvSocket.Bind(recv);

                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint server = new IPEndPoint(IPAddress.Any, 2188);
                sendSocket.Bind(server);

                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);

                Connected = true;
            }
            catch (Exception)
            {
                Connected = false;
            }
        }

        private void HandleData(byte[] Data)
        {
            try
            {
                if (Data.Length == 10)
                {
                    PGN = Data[0] << 8 | Data[1];

                    // AOG - PGNs 31000 to 31999
                    // arduino modules - PGNs 32000 to 32999
                    // companion apps - PGNs 33000 to 33999

                    if (PGN >= 31000 & PGN <= 31999)
                    {
                        // for AOG
                        UpdateForm("Received PGN" + PGN.ToString());
                    }
                    else if (PGN >= 32000 & PGN <= 32999)
                    {
                        // for modules
                        SendMessage(Data);
                    }
                    else if (PGN >= 33000 & PGN <= 33999)
                    {
                        // for companion apps
                        SendMessage(Data, "127.0.0.1");
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void UpdateForm(string Message)
        {
            foreach (Control c in mf.Controls)
            {
                if (c.Name == "tbReceive")
                {
                    c.Text += Message + Environment.NewLine;
                    break;
                }
            }
        }

        private void ReceiveData(IAsyncResult asyncResult)
        {
            try
            {
                EndPoint epSender = new IPEndPoint(IPAddress.Any, 0);

                int msgLen = recvSocket.EndReceiveFrom(asyncResult, ref epSender);

                byte[] localMsg = new byte[msgLen];
                Array.Copy(buffer, localMsg, msgLen);

                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref epSender, new AsyncCallback(ReceiveData), epSender);

                mf.Invoke(HandleDataDelegate, new object[] { localMsg });
            }
            catch (Exception)
            {

            }
        }

        public void SendMessage(byte[] Data, string DestinationIP = "")
        {
            try
            {
                if (Connected)
                {
                    if (DestinationIP == "") DestinationIP = cEndIP;
                    epIP = IPAddress.Parse(DestinationIP);
                    IPEndPoint EndPt;
                    for (int i = 0; i < 5; i++)
                    {
                        EndPt = new IPEndPoint(epIP, 8888 + i);

                        if (Data.Length != 0)
                        {
                            sendSocket.BeginSendTo(Data, 0, Data.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void SendData(IAsyncResult asyncResult)
        {
            try
            {
                sendSocket.EndSend(asyncResult);
            }
            catch (Exception)
            {

            }
        }

    }
}
