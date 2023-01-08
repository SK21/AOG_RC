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
        //	        - bit 1,2,3		control type 0-4
        //	        - bit 4		    MasterOn
        //          - bit 5         0 - average time for multiple pulses, 1 - time for one pulse
        //          - bit 6         AutoOn
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
            double RateSet;

            cData[2] = Prod.mf.Tls.BuildModSenID(Prod.ModuleID, Prod.SensorID);
            //cData[3] = Prod.mf.Sections.SectionLo();
            //cData[4] = Prod.mf.Sections.SectionHi();

            int Relays = Prod.mf.RelayObjects.Status();
            cData[3] = (byte)Relays;
            cData[4] = (byte)(Relays >> 8);

            // rate set
            if (Prod.ControlType == ControlTypeEnum.Fan && !Prod.FanOn)
            {
                RateSet = 0;
            }
            else
            {
                if (Prod.mf.SwitchBox.SwitchOn(SwIDs.Auto))
                {
                    // auto rate
                    RateSet = Prod.TargetUPM() * 10.0;
                    if (RateSet < (Prod.MinUPM * 10.0)) RateSet = Prod.MinUPM * 10.0;
                }
                else
                {
                    // manual rate
                    RateSet = (Prod.ManualAdjust * 10.0);
                }
            }

            if (Prod.EnableProdDensity && Prod.ProdDensity > 0) RateSet = (RateSet / Prod.ProdDensity) * 100;

            cData[5] = (byte)RateSet;
            cData[6] = (byte)((int)RateSet >> 8);
            cData[7] = (byte)((int)RateSet >> 16);

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
                    cData[11] &= 0b11110001; // clear bit 1, 2, 3
                    cData[11] |= 0b00000010; // set bit 1
                    break;

                case ControlTypeEnum.Motor:
                    cData[11] &= 0b11110001; // clear bit 1, 2, 3
                    cData[11] |= 0b00000100; // set bit 2
                    break;

                case ControlTypeEnum.MotorWeights:
                    cData[11] &= 0b11110001; // clear bit 1, 2, 3
                    cData[11] |= 0b00000110; // set bit 1, 2
                    break;

                case ControlTypeEnum.Fan:
                    cData[11] &= 0b11110001; // clear bit 1, 2, 3
                    cData[11] |= 0b00001000; // set bit 3
                    break;

                default:
                    // standard
                    cData[11] &= 0b11110001; // clear bit 1, 2, 3
                    break;
            }

            if (Prod.mf.Sections.IsMasterOn()) cData[11] |= 0b00010000;
            if (Prod.UseMultiPulse) cData[11] |= 0b00100000;
            if (Prod.mf.SwitchBox.SwitchOn(SwIDs.Auto)) cData[11] |= 0b01000000;
            if (Prod.DoCal && Prod.ControlType == ControlTypeEnum.MotorWeights) cData[11] |= 0b10000000;

            // power relays
            for (int i = 0; i < 16; i++)
            {
                int Power = Prod.mf.RelayObjects.PowerRelays();
                cData[12] = (byte)Power;
                cData[13] = (byte)(Power >> 8);
            }

            // pwm cal
            cData[14] = Prod.CalPWM;

            // CRC
            cData[cByteCount - 1] = Prod.mf.Tls.CRC(cData, cByteCount - 1);

            // send
            if (Prod.SimulationType == SimType.VirtualNano)
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