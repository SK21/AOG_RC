using System.Diagnostics;
using System;

namespace RateController
{
    public class PGN32614
    {
        // to Arduino from Rate Controller
        //0	    HeaderLo		    102
        //1	    HeaderHi		    127
        //2     Mod/Sen ID          0-15/0-15
        //3	    relay Lo		    0-7
        //4 	relay Hi		    8-15
        //5	    rate set Lo		    10 X actual
        //6     rate set Mid
        //7	    rate set Hi
        //8	    Flow Cal Lo	        1000 X actual 
        //9     Flow Cal Mid
        //10    Flow Cal Hi
        //11	Command
        //	        - bit 0		    reset acc.Quantity
        //	        - bit 1,2		valve type 0-3
        //	        - bit 3		    MasterOn
        //          - bit 4         0 - average time for multiple pulses, 1 - time for one pulse
        //          - bit 5         AutoOn
        //          - bit 6         Debug pgn on
        //          - bit 7         Calibration On
        //12    power relay Lo      list of power type relays 0-7
        //13    power relay Hi      list of power type relays 8-15
        //14    Cal PWM             PWM setting for weight calibration
        //15    CRC

        private const byte cByteCount = 16;
        private readonly clsProduct Prod;
        private byte[] cData = new byte[cByteCount];

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

            int Relays = Prod.mf.RelayObjects.Status();
            cData[3] = (byte)Relays;
            cData[4] = (byte)(Relays >> 8);

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
                Tmp = (Prod.ManualAdjust * 10.0);
            }

            cData[5] = (byte)Tmp;
            cData[6] = (byte)((int)Tmp >> 8);
            cData[7] = (byte)((int)Tmp >> 16);

            // flow cal
            Tmp = Prod.MeterCal * 1000.0;
            cData[8] = (byte)Tmp;
            cData[9] = (byte)((int)Tmp >> 8);
            cData[10] = (byte)((int)Tmp >> 16);

            // command byte
            cData[11] = 0;
            if (Prod.EraseAccumulatedUnits) cData[11] |= 0b00000001;
            Prod.EraseAccumulatedUnits = false;

            switch (Prod.ControlType)
            {
                case ControlTypeEnum.FastClose:
                    cData[11] &= 0b11111011; // clear bit 2
                    cData[11] |= 0b00000010; // set bit 1
                    break;

                case ControlTypeEnum.Motor:
                    cData[11] |= 0b00000100; // set bit 2
                    cData[11] &= 0b11111101; // clear bit 1
                    break;

                case ControlTypeEnum.MotorWeights:
                    cData[11] |= 0b00000110; // set bit 2 and 1
                    break;

                default:
                    // standard
                    cData[11] &= 0b11111001; // clear bit 2 and 1
                    break;
            }

            if (Prod.mf.SectionControl.MasterOn()) cData[11] |= 0b00001000;
            if (Prod.UseMultiPulse) cData[11] |= 0b00010000;
            if (Prod.mf.SwitchBox.SwitchOn(SwIDs.Auto)) cData[11] |= 0b00100000;
            if (Prod.DebugArduino) cData[11] |= 0b01000000;
            if (Prod.DoCal && Prod.ControlType == ControlTypeEnum.MotorWeights) cData[11] |= 0b10000000;

            // power relays
            for (int i = 0; i < 16; i++)
            {
                int Power = Prod.mf.RelayObjects.PowerRelays();
                cData[12] = (byte)Power;
                cData[13] = (byte)(Power >> 8);
            }

            // pwm cal
            if (Prod.mf.SectionControl.MasterOn())
            {
                cData[14] = (byte)Prod.CalPWM;
            }
            else
            {
                cData[14] = 0;
            }

            // CRC
            cData[cByteCount - 1] = Prod.mf.Tls.CRC(cData, cByteCount - 1);

            // send
            if (Prod.mf.SimMode == SimType.VirtualNano)
            {
                Prod.VirtualNano.ReceiveSerial(cData);
            }
            else
            {
                Prod.mf.SendSerial(cData);
                Prod.mf.UDPmodules.SendUDPMessage(cData);
            }
        }
    }
}