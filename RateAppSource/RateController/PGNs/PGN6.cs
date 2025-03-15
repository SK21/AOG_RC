using System;

namespace RateController.PGNs
{
    public class PGN6
    {
        // Rate settings
        // 0	rate set lo		1000 X actual
        // 1	rate set mid
        // 2	rate set hi
        // 3	flow cal lo		100 X actual
        // 4	flow cal mid
        // 5	flow cal hi
        // 6	manual PWM
        // 7	commands
        //			- bit 0		reset accumulated quantity
        //			- bit 1,2,3	control type 0-4
        //			- bit 4		master is on
        //			- bit 5		-
        //			- bit 6		auto is on

        private byte[] cData;
        private clsProduct Prod;

        public PGN6(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
            cData = new byte[8];
        }

        public void Send()
        {
            Array.Clear(cData, 0, cData.Length);

            // rate set
            if (Prod.Enabled)
            {
                double RateSet;
                if ((Prod.ControlType == ControlTypeEnum.Fan && !Prod.FanOn) || Prod.AppMode == ApplicationMode.DocumentTarget)
                {
                    RateSet = 0;
                }
                else
                {
                    RateSet = Prod.TargetUPM() * 1000.0;
                    if (RateSet < (Prod.MinUPM * 1000.0)) RateSet = Prod.MinUPMinUse() * 1000.0;
                }

                cData[0] = (byte)RateSet;
                cData[1] = (byte)((int)RateSet >> 8);
                cData[2] = (byte)((int)RateSet >> 16);
            }

            // flow cal
            double Tmp = Prod.MeterCal * 100.0;
            cData[3] = (byte)Tmp;
            cData[4] = (byte)((int)Tmp >> 8);
            cData[5] = (byte)((int)Tmp >> 16);

            // command byte
            if (Prod.EraseAccumulatedUnits) cData[7] |= 0b00000001;
            Prod.EraseAccumulatedUnits = false;

            switch (Prod.ControlType)
            {
                case ControlTypeEnum.ComboClose:
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    cData[7] |= 0b00000010; // set bit 1
                    break;

                case ControlTypeEnum.Motor:
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    cData[7] |= 0b00000100; // set bit 2
                    break;

                case ControlTypeEnum.MotorWeights:
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    cData[7] |= 0b00000110; // set bit 1, 2
                    break;

                case ControlTypeEnum.Fan:
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    cData[7] |= 0b00001000; // set bit 3
                    break;

                case ControlTypeEnum.ComboCloseTimed:
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    cData[7] |= 0b00001010; // set bit 1, 3
                    break;

                default:
                    // standard valve
                    cData[7] &= 0b11110001; // clear bit 1, 2, 3
                    break;
            }

            // master on
            if (Prod.mf.SwitchBox.Connected())
            {
                if (Prod.mf.SectionControl.MasterOn || Prod.CalRun || Prod.CalSetMeter
                    || Prod.mf.Tls.MasterSwitchMode == MasterSwitchMode.Override
                    || Prod.mf.Tls.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly) cData[7] |= 0b00010000;

                if (Prod.mf.SwitchBox.MasterOn) cData[7] |= 0b00100000;
            }
            else
            {
                cData[7] |= 0b00010000;
            }

            if ((Prod.mf.SwitchBox.AutoRateOn || Prod.CalSetMeter) && !Prod.CalRun)
            {
                // auto on
                cData[7] |= 0b01000000;

                if (Prod.ControlType != ControlTypeEnum.Valve && Prod.ControlType != ControlTypeEnum.ComboClose)
                {
                    // keep manual motor setting the same as auto when not in use
                    // for smooth transition from auto to manual control
                    Prod.ManualPWM = (int)Prod.PWM();
                }
            }

            // manual cal
            if ((Prod.mf.SectionControl.MasterOn
                || Prod.mf.Tls.MasterSwitchMode == MasterSwitchMode.Override
                || Prod.mf.Tls.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly) && Prod.Enabled)
            {
                cData[6] = (byte)Prod.ManualPWM;
            }

            if (Prod.mf.CanBus1.IsOpen) Prod.mf.CanBus1.SendCanMessage(6, (byte)Prod.ModuleID, Prod.SensorID, cData);
        }
    }
}