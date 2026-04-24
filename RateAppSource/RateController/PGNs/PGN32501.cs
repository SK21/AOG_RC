using RateController.Classes;
using System;

namespace RateController.PGNs
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
        //9     FlowMasterValveIndex    0-15, 255 disabled
        //10    CRC

        private const byte cByteCount = 11;
        private byte[] cData = new byte[cByteCount];
        private int cModuleID;
        private DateTime cSendTime;

        public PGN32501(int ModuleID)
        {
            cModuleID = ModuleID;
        }

        public DateTime SendTime
        { get { return cSendTime; } }

        public void Send()
        {
            Array.Clear(cData, 0, cByteCount);
            cData[0] = 245;
            cData[1] = 126;
            cData[2] = Core.Tls.BuildModSenID((byte)cModuleID, 0);

            int Relays = Core.RelayObjects.SetRelays(cModuleID);

            cData[3] = (byte)Relays;
            cData[4] = (byte)(Relays >> 8);

            // power relays
            int Power = Core.RelayObjects.PowerRelays(cModuleID);
            cData[5] = (byte)Power;
            cData[6] = (byte)(Power >> 8);

            // inverted relays
            int Inverted = Core.RelayObjects.InvertedRelays(cModuleID);
            cData[7] = (byte)Inverted;
            cData[8] = (byte)(Inverted >> 8);

            cData[9] = FlowMasterValveIndex();

            // CRC
            cData[cByteCount - 1] = Core.Tls.CRC(cData, cByteCount - 1);

            // send - route through CAN bridge or Ethernet
            if (Props.CanEnabled && Core.CanBridgeComm != null)
            {
                Core.CanBridgeComm.SendModuleCommand(cData);
            }
            else
            {
                Core.UDPmodules.Send(cData);
            }

            cSendTime = DateTime.Now;
        }

        private byte FlowMasterValveIndex()
        {
            FlowMasterValveMode mode;
            if (!Enum.TryParse(Props.GetProp(Props.FlowMasterValveModePropName(cModuleID)), out mode))
            {
                mode = (bool.TryParse(Props.GetProp(Props.FlowMaster2WirePropName(cModuleID)), out bool legacyEnabled) && legacyEnabled)
                    ? FlowMasterValveMode.TwoWire
                    : FlowMasterValveMode.ThreeWire;
            }

            if (mode != FlowMasterValveMode.TwoWire)
            {
                return 255;
            }

            int result = -1;
            int count = 0;
            foreach (clsRelay relay in Core.RelayObjects.Items)
            {
                if (relay.ModuleID == cModuleID && relay.Type == RelayTypes.FlowMaster)
                {
                    result = relay.ID;
                    count++;
                    if (count > 1) return 255;
                }
            }

            return (count == 1) ? (byte)result : (byte)255;
        }
    }
}
