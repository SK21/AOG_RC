using RateController.Classes;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace RateController
{
    public class PGN32503
    {
        //PGN32503, Subnet change
        //0     HeaderLo    247
        //1     HeaderHI    126
        //2     IP 0
        //3     IP 1
        //4     IP 2
        //5     CRC

        private byte[] cData = new byte[6];
        private FormStart mf;

        public PGN32503(FormStart Main)
        {
            mf = Main;
            cData[0] = 247;
            cData[1] = 126;
        }

        public bool Send(string EP)
        {
            bool Result = false;
            string[] data = EP.Split('.');
            cData[2] = byte.Parse(data[0]);
            cData[3] = byte.Parse(data[1]);
            cData[4] = byte.Parse(data[2]);

            // CRC
            cData[5] = mf.Tls.CRC(cData, 5);

            // based on AGIO/FormUDP
            IPEndPoint epModuleSet = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 28888);
            IPAddress IP;
            if (IPAddress.TryParse(EP, out IP))
            {
                //loop thru all interfaces
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.Supports(NetworkInterfaceComponent.IPv4) && nic.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (var info in nic.GetIPProperties().UnicastAddresses)
                        {
                            // Only InterNetwork and not loopback which have a subnetmask
                            if (info.Address.AddressFamily == AddressFamily.InterNetwork &&
                                !IPAddress.IsLoopback(info.Address) &&
                                info.IPv4Mask != null)
                            {
                                Socket scanSocket;
                                try
                                {
                                    if (nic.OperationalStatus == OperationalStatus.Up
                                        && info.IPv4Mask != null)
                                    {
                                        scanSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                                        scanSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);
                                        scanSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                                        scanSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontRoute, true);

                                        try
                                        {
                                            scanSocket.Bind(new IPEndPoint(info.Address, 9578));
                                            scanSocket.SendTo(cData, 0, cData.Length, SocketFlags.None, epModuleSet);
                                        }
                                        catch (Exception ex)
                                        {
                                            Props.WriteErrorLog("frmNework/btnSend_Click/Bind error " + ex.Message);
                                        }

                                        scanSocket.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Props.WriteErrorLog("frmNework/btnSend_Click/nic loop error " + ex.Message);
                                }
                            }
                        }
                    }
                }
                Result = true;
            }
            return Result;
        }
    }
}