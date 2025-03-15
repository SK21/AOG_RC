using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RateController.PGNs
{
    public class PGN7
    {
        private byte[] cData;

        private int cModuleID;

        // Relay settings
        // 0	relay lo		0-7
        // 1	relay hi		8-15
        // 2	power relay lo	power type relay 0-7
        // 3	power relay hi  power type relay 8-15
        // 4	inverted lo		inverted type relay 0-7
        // 5	inverted hi		inverted type relay 8-15

        private FormStart mf;

        public PGN7(FormStart CalledFrom, int ModuleID)
        {
            mf = CalledFrom;
            cData = new byte[6];
            cModuleID = ModuleID;
        }

        public void Send()
        {
            Array.Clear(cData, 0, cData.Length);

            int Relays = mf.RelayObjects.SetRelays(cModuleID);

            cData[0] = (byte)Relays;
            cData[1] = (byte)(Relays >> 8);

            // power relays
            int Power = mf.RelayObjects.PowerRelays(cModuleID);
            cData[2] = (byte)Power;
            cData[3] = (byte)(Power >> 8);

            // inverted relays
            int Inverted = mf.RelayObjects.InvertedRelays(cModuleID);
            cData[4] = (byte)Inverted;
            cData[5] = (byte)(Inverted >> 8);

            if (mf.CanBus1.IsOpen) mf.CanBus1.SendCanMessage(7, (byte)cModuleID, 0, cData);
            //to do, mf.UDPmodules.SendUDPMessage(cData);
        }
    }
}