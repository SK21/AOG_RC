﻿namespace RateController
{
    public class PGN32614
    {
        // to Arduino from Rate Controller
        //0	    HeaderLo		102
        //1	    HeaderHi		127
        //2     Mod/Sen ID      0-15/0-15
        //3	    relay Lo		0-7
        //4 	relay Hi		8-15
        //5	    rate set Lo		10 X actual
        //6     rate set Mid
        //7	    rate set Hi
        //8	    Flow Cal Lo		100 X actual
        //9	    Flow Cal Hi
        //10	Command
        //	        - bit 0		    reset acc.Quantity
        //	        - bit 1,2		valve type 0-3
        //	        - bit 3		    simulate flow
        //          - bit 4         0 - average time for multiple pulses, 1 - time for one pulse
        //          - bit 5         AutoOn

        private readonly clsProduct Prod;
        private byte[] cData = new byte[11];

        public PGN32614(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
            cData[0] = 102;
            cData[1] = 127;
        }

        public void Send()
        {
            double Tmp = 0;

            cData[2] = Prod.mf.Tls.BuildModSenID(Prod.ModuleID, (byte)Prod.SensorID);
            cData[3] = Prod.mf.Sections.SectionLo();
            cData[4] = Prod.mf.Sections.SectionHi();

            // rate set
            if (Prod.mf.SwitchBox.SwitchOn(SwIDs.Auto))
            {
                // auto rate
                Tmp = Prod.TargetUPM() * 10.0;
                if (Tmp < (Prod.MinUPM * 10.0)) Tmp = Prod.MinUPM * 10.0;
            }
            else
            {
                // manual rate
                Tmp = (Prod.ManualRateFactor * 10.0);
            }

            cData[5] = (byte)Tmp;
            cData[6] = (byte)((int)Tmp >> 8);
            cData[7] = (byte)((int)Tmp >> 16);

            // flow cal
            Tmp = Prod.FlowCal * 100.0;
            cData[8] = (byte)Tmp;
            cData[9] = (byte)((int)Tmp >> 8);

            // command byte
            cData[10] = 0;
            if (Prod.EraseApplied) cData[10] |= 0b00000001;
            Prod.EraseApplied = false;

            switch (Prod.ValveType)
            {
                case 1:
                    cData[10] &= 0b11111011; // clear bit 2
                    cData[10] |= 0b00000010; // set bit 1
                    break;

                case 2:
                    cData[10] |= 0b00000100; // set bit 2
                    cData[10] &= 0b11111101; // clear bit 1
                    break;

                case 3:
                    cData[10] |= 0b00000110; // set bit 2 and 1
                    break;

                default:
                    cData[10] &= 0b11111001; // clear bit 2 and 1
                    break;
            }

            if (Prod.SimulationType != SimType.None) cData[10] |= 0b00001000; else cData[10] &= 0b11110111;
            if (Prod.UseMultiPulse) cData[10] |= 0b00010000; else cData[10] &= 0b11101111;
            if (Prod.mf.SwitchBox.SwitchOn(SwIDs.Auto)) cData[10] |= 0b00100000;
            //Debug.Print(cData[10].ToString("N3"));

            // send
            if (Prod.SimulationType == SimType.VirtualNano)
            {
                Prod.VirtualNano.ReceiveSerial(cData);
            }
            else
            {
                Prod.mf.SendSerial(cData);
                Prod.mf.UDPnetwork.SendUDPMessage(cData);
            }
        }
    }
}