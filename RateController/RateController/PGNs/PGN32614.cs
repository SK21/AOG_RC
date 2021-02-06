using System.Diagnostics;

namespace RateController
{
    public class PGN32614
    {
        // to Arduino from Rate Controller
        //0	HeaderHi		127
        //1	HeaderLo		230
        //2 Controller ID
        //3	relay Hi		8-15
        //4	relay Lo		0-7
        //5	rate set Hi		100 X actual
        //6	rate set Lo		100 X actual
        //7	Flow Cal Hi		100 X actual
        //8	Flow Cal Lo		100 X actual
        //9	Command
        //	- bit 0		    reset acc.Quantity
        //	- bit 1,2		valve type 0-3
        //	- bit 3		    simulate flow
        //  - bit 4         0 UsePDI, 1 UseVCN 

        public byte Command;
        public byte FlowCalHi;
        public byte FlowCalLo;
        public byte RateSetHi;
        public byte RateSetLo;
        public byte RelayHi;
        public byte RelayLo;

        private byte cProdID;
        private byte cConID;

        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 102;
        private readonly CRateCals RC;

        public PGN32614(CRateCals CalledFrom, byte ProductID)
        {
            RC = CalledFrom;
            cProdID = ProductID;
            cConID = ControllerID;
        }

        public byte ControllerID { get { return cConID; } set { cConID = value; } }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = cConID;
            Vals[3] = RelayHi;
            Vals[4] = RelayLo;
            Vals[5] = RateSetHi;
            Vals[6] = RateSetLo;
            Vals[7] = FlowCalHi;
            Vals[8] = FlowCalLo;
            Vals[9] = Command;
            return Vals;
        }

        public void Send()
        {
            RelayHi = RC.mf.AOG.SectionControlByteHi;
            RelayLo = RC.mf.AOG.SectionControlByteLo;

            int Temp = (int)(RC.TargetUPM() * 100.0);
            RateSetHi = (byte)(Temp >> 8);
            RateSetLo = (byte)Temp;

            Temp = (int)(RC.FlowCal * 100.0);
            FlowCalHi = (byte)(Temp >> 8);
            FlowCalLo = (byte)Temp;

            // command byte
            Command = 0;
            if (RC.EraseApplied) Command |= 0b00000001;
            RC.EraseApplied = false;

            switch (RC.ValveType)
            {
                case 1:
                    Command &= 0b11111011; // clear bit 2
                    Command |= 0b00000010; // set bit 1
                    break;

                case 2:
                    Command |= 0b00000100; // set bit 2
                    Command &= 0b11111101; // clear bit 1
                    break;

                case 3:
                    Command |= 0b00000110; // set bit 2 and 1
                    break;

                default:
                    Command &= 0b11111001; // clear bit 2 and 1
                    break;
            }

            if (RC.SimulationType != SimType.None) Command |= 0b00001000; else Command &= 0b11110111;

            if (RC.UseVCN) Command |= 0b00010000; else Command &= 0b11101111;

            if (RC.SimulationType == SimType.VirtualNano)
            {
                RC.Nano.ReceiveSerial(Data());
            }
            else
            {
                RC.mf.SER[cProdID].SendtoRC(Data());
                RC.mf.UDPnetwork.SendUDPMessage(Data());
            }
        }
    }
}