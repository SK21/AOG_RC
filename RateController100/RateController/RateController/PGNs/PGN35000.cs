using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    public class PGN35000
    {
        //PGN35000 to Arduino from Rate Controller				
        //0	HeaderHi		136	
        //1	HeaderLo		184	
        //2	relay Hi		8-15	
        //3	relay Lo		0-7	
        //4	rate set Hi		100 X actual	
        //5	rate set Lo		100 X actual	
        //6	Flow Cal Hi		100 X actual	
        //7	Flow Cal Lo		100 X actual	
        //8	Command			
        //	- bit 0		    reset acc.Quantity
        //	- bit 1,2		valve type 0-3	
        //	- bit 3		    simulate flow

        private const byte cByteCount = 9;
        private const byte HeaderHi = 136;
        private const byte HeaderLo = 184;

        public byte RelayHi;
        public byte RelayLo;
        public byte RateSetHi;
        public byte RateSetLo;
        public byte FlowCalHi;
        public byte FlowCalLo;
        public byte Command;

        private readonly CRateCals RC;

        public PGN35000(CRateCals CalledFrom)
        {
            RC = CalledFrom;
        }

        public byte ByteCount()
        {
            return cByteCount;
        }

        public byte[] Data()
        {
            byte[] Vals = new byte[cByteCount];
            Vals[0] = HeaderHi;
            Vals[1] = HeaderLo;
            Vals[2] = RelayHi;
            Vals[3] = RelayLo;
            Vals[4] = RateSetHi;
            Vals[5] = RateSetLo;
            Vals[6] = FlowCalHi;
            Vals[7] = FlowCalLo;
            Vals[8] = Command;
            return Vals;
        }

        public void Send()
        {
            RelayHi = RC.AogRec35400.SectionControlByteHi;
            RelayLo = RC.AogRec35400.SectionControlByteLo;

            int Temp = (int)(RC.UPMsetting() * 100.0);
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

            if (RC.SimulationType!=SimType.None) Command |= 0b00001000; else Command &= 0b11110111;

            if (RC.SimulationType==SimType.VirtualNano)
            {
                RC.Nano.ReceiveSerial(Data());
            }
            else
            {
                RC.mf.SER.SendtoRC(Data());
                RC.mf.UDPnetwork.SendUDPMessage(Data());
            }
        }
    }
}
