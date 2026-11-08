using RateController.Classes;
using System.Net;

namespace RateController.PGNs
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

        public PGN32503()
        {
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
            cData[5] = Core.Tls.CRC(cData, 5);

            // every board on every network hears it (not addressed by MAC: every board moves).
            // Not the tablet hotspot: an ESP32 restarts on this PGN (UDPComm.SendAllNetworks)
            if (IPAddress.TryParse(EP, out _))
            {
                UDPComm.SendAllNetworks(cData, 28888);
                Result = true;
            }
            return Result;
        }
    }
}
