using System;
using System.Net;
using System.Net.Sockets;

namespace IMUapp
{
    public class UDPComm
    {
        private readonly FormMain mf;

        public bool isUDPSendConnected;

        private byte[] buffer = new byte[1024];

        private IPAddress epIP;
        private int cReceivePort;
        private int cSendPort;

        private HandleDataDelegateObj HandleDataDelegate = null;

        private int PGN;
        private Socket recvSocket;
        private Socket sendSocket;

        public UDPComm(FormMain CallingForm, string DestinationIP, int ReceivePort, int SendPort)
        {
            mf = CallingForm;
            epIP = IPAddress.Parse(DestinationIP);
            cReceivePort = ReceivePort;
            cSendPort = SendPort;
        }

        // new data event
        public delegate void NewDataDelegate(byte[] Data);

        // Status delegate
        private delegate void HandleDataDelegateObj(int port, byte[] msg);

        //sends byte array
        public void SendUDPMessage(byte[] byteData)
        {
            if (isUDPSendConnected)
            {
                try
                {
                    IPEndPoint EndPt = new IPEndPoint(epIP, 9999);

                    // Send packet to the zero
                    if (byteData.Length != 0)
                        sendSocket.BeginSendTo(byteData, 0, byteData.Length, SocketFlags.None, EndPt, new AsyncCallback(SendData), null);
                }
                catch (Exception)
                {

                }
            }
        }

        public string StartUDPServer()
        {
            try
            {
                // initialize the delegate which updates the message received
                HandleDataDelegate = HandleData;

                // initialize the receive socket
                recvSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server and port
                IPEndPoint recv = new IPEndPoint(IPAddress.Any, cReceivePort);
                // Associate the socket with this IP address and port
                recvSocket.Bind(recv);

                // initialize the send socket
                sendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                // Initialise the IPEndPoint for the server to send on port 
                IPEndPoint server = new IPEndPoint(IPAddress.Any, cSendPort);
                sendSocket.Bind(server);

                // Initialise the IPEndPoint for the client - async listner client only!
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                // Start listening for incoming data
                recvSocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref client, new AsyncCallback(ReceiveData), recvSocket);
                isUDPSendConnected = true;
                return GetLocalIP();
            }
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog("UDP Server" + e);
                mf.Tls.TimedMessageBox("UDP start error: ", e.Message);
                return "";
            }
        }

        public string GetLocalIP()
        {
            try
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.ToString();
                }
            }
            catch (Exception Ex)
            {
                return "";
            }
        }

        private void HandleData(int Port, byte[] Data)
        {
            if (Data.Length == 10)
            {
                PGN = Data[0] << 8 | Data[1];
                switch (PGN)
                {
                    case 32750:
                        mf.IMUdata.ParseByteData(Data);
                        break;
                }
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
            catch (Exception e)
            {
                mf.Tls.WriteErrorLog("UDP Recv data " + e.ToString());
                mf.Tls.TimedMessageBox("ReceiveData Error", e.Message);
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
