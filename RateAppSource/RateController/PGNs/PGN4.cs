using GMap.NET.MapProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN4
    {
        // subnet change
        // 0	IP 0
        // 1	IP 1
        // 2	IP 2

        private FormStart mf;
        private byte[] Part = new byte[3];

        public PGN4(FormStart Main)
        {
            mf = Main;
        }

        public bool Send(string EP)
        {
            bool Result = false;
            string[] data = EP.Split('.');
            if (data.Length > 2)
            {
                Result = true;
                for (int i = 0; i < 3; i++)
                {
                    if (!byte.TryParse(data[i], out Part[i]))
                    {
                        Result = false;
                        break;
                    }
                }
                if (Result)
                {
                    mf.CanBus1.SendCanMessage(4, 0, 0, Part);
                    SendUDP(EP);
                }
            }
            return Result;
        }

        private bool SendUDP(string EP)
        {
            //PGN32503, Subnet change
            //0     HeaderLo    247
            //1     HeaderHI    126
            //2     IP 0
            //3     IP 1
            //4     IP 2
            //5     CRC
            byte[] cData = new byte[6];
            cData[0] = 247;
            cData[1] = 126;
            cData[2] = Part[0];
            cData[3] = Part[1];
            cData[4] = Part[2];
            cData[5] = mf.Tls.CRC(cData, 5);

            bool Result = false;

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
                                            mf.Tls.WriteErrorLog("frmNework/btnSend_Click/Bind error " + ex.Message);
                                        }

                                        scanSocket.Dispose();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    mf.Tls.WriteErrorLog("frmNework/btnSend_Click/nic loop error " + ex.Message);
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