using System;
using System.Diagnostics;

namespace RateController
{
    public class PGN32500
    {
        //PGN32500, Rate settings from RC to module
        //0	    HeaderLo		    244
        //1	    HeaderHi		    126
        //2     Mod/Sen ID          0-15/0-15
        //3	    rate set Lo		    1000 X actual
        //4     rate set Mid
        //5	    rate set Hi
        //6	    Flow Cal Lo	        1000 X actual
        //7     Flow Cal Mid
        //8     Flow Cal Hi
        //9	    Command
        //	        - bit 0		    reset acc.Quantity
        //	        - bit 1,2,3		control type 0-4
        //	        - bit 4		    MasterOn
        //          - bit 5         0 - time for one pulse, 1 - average time for multiple pulses
        //          - bit 6         AutoOn
        //          - bit 7         -
        //10    manual pwm Lo
        //11    manual pwm Hi
        //12    -
        //13    CRC

        private const byte cByteCount = 14;
        private byte[] cData = new byte[cByteCount];
        private DateTime cSendTime;
        private clsProduct Prod;

        public PGN32500(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public DateTime SendTime
        { get { return cSendTime; } }

        public void Send()
        {
            double Tmp = 0;
            double RateSet;
            Array.Clear(cData, 0, cByteCount);
            cData[0] = 244;
            cData[1] = 126;

            cData[2] = Prod.mf.Tls.BuildModSenID((byte)Prod.ModuleID, Prod.SensorID);

            // rate set
            if (Prod.ControlType == ControlTypeEnum.Fan && !Prod.FanOn)
            {
                RateSet = 0;
            }
            else
            {
                RateSet = Prod.TargetUPM() * 1000.0;
                if (RateSet < (Prod.MinUPM * 1000.0)) RateSet = Prod.MinUPM * 1000.0;
            }

            if (Prod.Enabled)
            {
                cData[3] = (byte)RateSet;
                cData[4] = (byte)((int)RateSet >> 8);
                cData[5] = (byte)((int)RateSet >> 16);
            }

            // flow cal
            Tmp = Prod.MeterCal * 1000.0;
            cData[6] = (byte)Tmp;
            cData[7] = (byte)((int)Tmp >> 8);
            cData[8] = (byte)((int)Tmp >> 16);

            // command byte
            if (Prod.EraseAccumulatedUnits) cData[9] |= 0b00000001;
            Prod.EraseAccumulatedUnits = false;

            switch (Prod.ControlType)
            {
                case ControlTypeEnum.ComboClose:
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    cData[9] |= 0b00000010; // set bit 1
                    break;

                case ControlTypeEnum.Motor:
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    cData[9] |= 0b00000100; // set bit 2
                    break;

                case ControlTypeEnum.MotorWeights:
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    cData[9] |= 0b00000110; // set bit 1, 2
                    break;

                case ControlTypeEnum.Fan:
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    cData[9] |= 0b00001000; // set bit 3
                    break;

                case ControlTypeEnum.ComboCloseTimed:
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    cData[9] |= 0b00001010; // set bit 1, 3
                    break;

                default:
                    // standard valve
                    cData[9] &= 0b11110001; // clear bit 1, 2, 3
                    break;
            }

            if (Prod.mf.SwitchBox.Connected())
            {
                if (Prod.mf.SectionControl.MasterOn() || Prod.CalRun || Prod.CalSetMeter) cData[9] |= 0b00010000;
            }
            else
            {
                cData[9] |= 0b00010000;
            }

            if (Prod.UseMultiPulse) cData[9] |= 0b00100000;

            if ((Prod.mf.SwitchBox.SwitchIsOn(SwIDs.Auto) || Prod.CalSetMeter) && !Prod.CalRun)
            {
                // auto on
                cData[9] |= 0b01000000;

                if (Prod.ControlType != ControlTypeEnum.Valve && Prod.ControlType != ControlTypeEnum.ComboClose)
                {
                    // keep manual motor setting the same as auto when not in use
                    // for smooth transition from auto to manual control
                    //Prod.ManualPWM = (int)Prod.PWM();
                }
            }

            // manual cal
            if (Prod.mf.SectionControl.MasterOn() && Prod.Enabled)
            {
                cData[10] = (byte)Prod.ManualPWM;
                cData[11] = (byte)(Prod.ManualPWM >> 8);
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

                cSendTime = DateTime.Now;
            }
        }
    }
}