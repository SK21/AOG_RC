using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace AOG_Companion_App
{
    class UDPcomm
    {
        private readonly Form1 mf;
        public bool Connected;

        private int cReceivePort;
        private int cSendPort;

        private IPAddress epIP;
        private delegate void HandleDataDelegateObj(byte[] msg);
        private HandleDataDelegateObj HandleDataDelegate = null;
        private byte[] buffer = new byte[1024];

        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        public UDPcomm(Form1 CallingForm)
        {
            mf = CallingForm;
        }

        public void Start(string ReceivePort, string SendPort)
        {
            try
            {
                recvSocket.Close();
            }
            catch (Exception)
            {

            }

            try
            {
                sendSocket.Close();
            }
            catch (Exception)
            {

            }

            try
            {
                cReceivePort = int.Parse(ReceivePort);
                cSendPort = int.Parse(SendPort);

                HandleDataDelegate = HandleData;
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, cReceivePort);
                recvSocket.Bind(recv);

                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint server = new IPEndPoint(IPAddress.Any, cSendPort);
                sendSocket.Bind(server);

                EndPoint client = new IPEndPoint(IPAddress.Any, 0);
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);

                Connected = true;
            }
            catch (Exception ex)
            {
                Connected = false;
                MessageBox.Show("Not started, check send or receive port & restart." + Environment.NewLine + Environment.NewLine + ex.Message.ToString(), "", MessageBoxButtons.OK);
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

                    if (PGN >= 33000 & PGN <= 33999)
                    {
                        // for companion apps
                        mf.Notify("Received PGN" + PGN.ToString());
                        //SendMessage(Data);
                    }

                }
            }
            catch (Exception)
            {

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

        public void SendMessage(byte[] Data)
        {
            try
            {
                if (Connected)
                {
                    epIP = IPAddress.Parse("127.0.0.1");
                    IPEndPoint EndPt = new IPEndPoint(epIP, 9999);

                    if (Data.Length != 0)
                    {
                        sendSocket.BeginSendTo(Data, 0, Data.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
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
