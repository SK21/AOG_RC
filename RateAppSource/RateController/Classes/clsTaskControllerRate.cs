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
    public class clsTaskControllerRate : IDisposable
    {
        private const int AddressRefreshMs = 5000;
        private const int CheckMs = 200;
        private const int ResendMs = 1000;
        private const int TCport = 8888;
        private List<IPAddress> LocalAddresses = new List<IPAddress>();
        private DateTime LocalAddressesTime = DateTime.MinValue;
        private readonly int RateProducts = Props.MaxProducts - 2;     // the last two are fans
        private readonly ushort[] LastDDI;
        private readonly DateTime[] LastSent;
        private readonly int[] LastValue;
        private readonly bool[] Sending;
        private readonly System.Windows.Forms.Timer SendTimer;
        private bool cDisposed;
        private UdpClient Udp;

        public clsTaskControllerRate()
        {
            LastDDI = new ushort[RateProducts];
            LastSent = new DateTime[RateProducts];
            LastValue = new int[RateProducts];
            Sending = new bool[RateProducts];

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
            try { Udp?.Close(); } catch { }
            Udp = null;
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

        private void Send(ushort DDI, int Value, byte Channel)
        {
            if (Udp == null) Udp = new UdpClient();
            byte[] Frame = BuildFrame(DDI, Value, Channel);
            foreach (IPAddress Address in Destinations())
            {
                Udp.Send(Frame, Frame.Length, new IPEndPoint(Address, TCport));
            }
        }

        private void SendTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                DateTime Now = DateTime.Now;
                for (int i = 0; i < RateProducts; i++)
                {
                    clsProduct Prd = Core.Products.Item(i);
                    ushort DDI = Prd.TCddi();
                    bool On = DDI != 0;

                    if (On != Sending[i])
                    {
                        Sending[i] = On;
                        Props.WriteActivityLog("Task controller rate, product " + (i + 1).ToString()
                            + (On ? ": sending on channel " + (i + 1).ToString() + ", DDI " + DDI.ToString() : ": stopped"));

                        // switched off: 0 at once rather than after AOG-TaskController's 4 s timeout
                        if (!On && LastDDI[i] != 0) Send(LastDDI[i], 0, (byte)(i + 1));
                        LastDDI[i] = 0;
                    }

                    if (On)
                    {
                        int Value = Prd.TCvalue();
                        if (Value != LastValue[i] || DDI != LastDDI[i] || (Now - LastSent[i]).TotalMilliseconds >= ResendMs)
                        {
                            Send(DDI, Value, (byte)(i + 1));
                            LastValue[i] = Value;
                            LastDDI[i] = DDI;
                            LastSent[i] = Now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsTaskControllerRate/SendTimer_Tick: " + ex.Message);
                try { Udp?.Close(); } catch { }
                Udp = null;     // opened again on the next send
            }
        }
    }
}
