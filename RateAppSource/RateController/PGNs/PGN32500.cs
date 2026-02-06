using RateController.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    enum CommandPGN32500 : byte
    {
        ResetQuantity = 1,              // 0000 0001, bit 0
        ControlStandard = 0,            // 0000 0000, bits 1,2,3 cleared
        ControlComboClose = 2,          // 0000 0010, bit 1
        ControlMotor = 4,               // 0000 0100, bit 2
        ControlMotorWeights = 6,        // 0000 0110, bit 1, bit 2
        ControlFan = 8,                 // 0000 1000, bit 3
        ControlComboCloseTimed = 10,    // 0000 1010, bit 1, bit 3
        MasterOnMode = 16,              // 0001 0000, bit 4
        MasterOnPosition = 32,          // 0010 0000, bit 5
        AutoOn = 64,                    // 0100 0000, bit 6
        CalibrationOn = 128             // 1000 0000, bit 7
    }
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
        //	        - bit 4		    MasterOn mode
        //          - bit 5         MasterOn switch position
        //          - bit 6         AutoOn
        //          - bit 7         Calibration On
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
            if (Prod.ModuleID >= 0 && Prod.SensorID >= 0)
            {
                double Tmp = 0;
                double RateSet = 0;
                Array.Clear(cData, 0, cByteCount);
                cData[0] = 244;
                cData[1] = 126;

                cData[2] = Core.Tls.BuildModSenID((byte)Prod.ModuleID, Prod.SensorID);

                // flow cal
                Tmp = Prod.MeterCal * 1000.0;
                cData[6] = (byte)Tmp;
                cData[7] = (byte)((int)Tmp >> 8);
                cData[8] = (byte)((int)Tmp >> 16);

                // command byte
                cData[9] = 0;

                switch (Prod.ControlType)
                {
                    case ControlTypeEnum.ComboClose:
                        cData[9] = (byte)CommandPGN32500.ControlComboClose;
                        break;

                    case ControlTypeEnum.Motor:
                        cData[9] = (byte)CommandPGN32500.ControlMotor;
                        break;

                    case ControlTypeEnum.MotorWeights:
                        cData[9] = (byte)CommandPGN32500.ControlMotorWeights;
                        break;

                    case ControlTypeEnum.Fan:
                        cData[9] = (byte)CommandPGN32500.ControlFan;
                        break;

                    case ControlTypeEnum.ComboCloseTimed:
                        cData[9] = (byte)CommandPGN32500.ControlComboCloseTimed;
                        break;

                    default:
                        // standard valve, leave bits 1, 2 and 3 unset
                        break;
                }

                if (Prod.EraseAccumulatedUnits) cData[9] |= (byte)CommandPGN32500.ResetQuantity;
                Prod.EraseAccumulatedUnits = false;

                if (Props.RateCalibrationOn)
                {
                    // calibrate
                    RateSet = Prod.TargetUPM() * 1000.0;

                    cData[9] |= (byte)(CommandPGN32500.CalibrationOn | CommandPGN32500.MasterOnMode);

                    if (Prod.CalIsLocked)
                    {
                        // Testing Rate, run in manual at CalPWM
                        cData[10] = (byte)Prod.ManualPWM;
                        cData[11] = (byte)(Prod.ManualPWM >> 8);
                    }
                    else
                    {
                        // Setting PWM, auto on, find CalPWM
                        cData[9] |= (byte)CommandPGN32500.AutoOn;
                    }
                }
                else
                {
                    // normal run
                    // rate
                    if ((Prod.ControlType == ControlTypeEnum.Fan && !Prod.FanOn) || Prod.AppMode == ApplicationMode.DocumentTarget
                            || Prod.AppMode == ApplicationMode.DocumentApplied)
                    {
                        RateSet = 0;
                    }
                    else
                    {
                        RateSet = Prod.TargetUPM() * 1000.0;
                        if (RateSet < (Prod.MinUPM * 1000.0)) RateSet = Prod.MinUPMinUse() * 1000.0;
                    }

                    // master on
                    if (Core.SwitchBox.Connected())
                    {
                        if (Core.SectionControl.MasterOn
                            || Props.MasterSwitchMode == MasterSwitchMode.Override
                            || Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly) cData[9] |= (byte)CommandPGN32500.MasterOnMode;

                        if (Core.SwitchBox.MasterOn) cData[9] |= (byte)CommandPGN32500.MasterOnPosition;
                    }
                    else
                    {
                        // switchbox missing, always master on
                        cData[9] |= (byte)CommandPGN32500.MasterOnMode;
                    }

                    if (Core.SwitchBox.AutoRateOn)
                    {
                        // auto on
                        cData[9] |= (byte)CommandPGN32500.AutoOn;

                        if (Prod.ControlType != ControlTypeEnum.Valve && Prod.ControlType != ControlTypeEnum.ComboClose)
                        {
                            // keep manual motor setting the same as auto when not in use
                            // for smooth transition from auto to manual control
                            Prod.ManualPWM = (int)Prod.PWM();
                        }
                    }

                    // manual
                    if ((Core.SectionControl.MasterOn
                        || Props.MasterSwitchMode == MasterSwitchMode.Override
                        || Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly) && Prod.Enabled)
                    {
                        cData[10] = (byte)Prod.ManualPWM;
                        cData[11] = (byte)(Prod.ManualPWM >> 8);
                    }
                }

                if (Prod.Enabled)
                {
                    cData[3] = (byte)RateSet;
                    cData[4] = (byte)((int)RateSet >> 8);
                    cData[5] = (byte)((int)RateSet >> 16);
                }

                // CRC
                cData[cByteCount - 1] = Core.Tls.CRC(cData, cByteCount - 1);

                // send - route through gateway if ISOBUS enabled
                if (Props.IsobusEnabled && Core.IsobusComm != null)
                {
                    Core.IsobusComm.SendModuleCommand(cData);
                }
                else
                {
                    Core.UDPmodules.Send(cData);
                }

                cSendTime = DateTime.Now;
            }
        }
    }
}
