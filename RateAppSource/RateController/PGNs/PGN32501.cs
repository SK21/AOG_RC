using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32501
    {
        //PGN32501, Relay settings from RC to module
        //0	    HeaderLo		    245
        //1	    HeaderHi		    126
        //2     Module ID
        //3	    relay Lo		    0-7
        //4 	relay Hi		    8-15
        //5     power relay Lo      list of power type relays 0-7
        //6     power relay Hi      list of power type relays 8-15
        //7     InvertedLo
        //8     InvertedHi
        //9     CRC

        private const byte cByteCount = 10;
        private byte[] cData = new byte[cByteCount];
        private int cModuleID;
        private DateTime cSendTime;
        private FormStart mf;

        public PGN32501(FormStart Main, int ModuleID)
        {
            mf = Main;
            cModuleID = ModuleID;
        }

        public DateTime SendTime
        { get { return cSendTime; } }

        public void Send()
        {
            Array.Clear(cData, 0, cByteCount);
            cData[0] = 245;
            cData[1] = 126;
            cData[2] = mf.Tls.BuildModSenID((byte)cModuleID, 0);

            int Relays = mf.RelayObjects.SetRelays(cModuleID);

            cData[3] = (byte)Relays;
            cData[4] = (byte)(Relays >> 8);

            // power relays
            int Power = mf.RelayObjects.PowerRelays(cModuleID);
            cData[5] = (byte)Power;
            cData[6] = (byte)(Power >> 8);

            // inverted relays
            int Inverted = mf.RelayObjects.InvertedRelays(cModuleID);
            cData[7] = (byte)Inverted;
            cData[8] = (byte)(Inverted >> 8);

            // CRC
            cData[cByteCount - 1] = mf.Tls.CRC(cData, cByteCount - 1);

            // send
            //Debug.Print("Relays: " + cData[3].ToString() + ", " + cData[4].ToString()); 
            mf.UDPmodules.SendUDPMessage(cData);

            cSendTime = DateTime.Now;
        }
    }
}