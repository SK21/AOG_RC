using System;

namespace RateController
{
    public class clsSectionControl
    {
        public struct CTLbytes
        {
            public byte AOGRlys0;
            public byte AOGRlys1;
            public bool AutoOn;
            public bool MasterOn;
            public bool RateDown;
            public bool RateUp;

            public byte RC0;
            public byte RC1;
            public byte Rlys0_Off;
            public byte Rlys0_On;      // section relays 0-7
            public byte Rlys1_Off;
            public byte Rlys1_On;      // section relays 8-15
            public byte SBRlys0;    // relays 0-7 based on switchbox switch positions
            public byte SBRlys1;    // relays 8-15 based on switchbox switch positions
            public byte SwHi;       // switches 0-7
            public byte SwLo;       // switches 8-15
        }

        public struct State
        {
            public bool Changed;
            public bool Pressed;
            public bool Released;
        }

        private DateTime AdjustTime;
        private CTLbytes CTL = new CTLbytes();
        private State CurrentState;
        private FormStart mf;

        private State PreviousState;
        private double RateDir;
        private int RateStep;

        private int StepDelay = 1000;   // ms between rate adjustments
        private double StepMultiplier = 0.05;   // rate change amount for each step
        private PGN234 ToAOG;

        public clsSectionControl(FormStart CallingForm)
        {
            mf = CallingForm;

            CTL.AutoOn = true;
            CTL.MasterOn = false;
            CTL.RateUp = false;
            CTL.RateDown = false;
            CTL.SwLo = 0;
            CTL.SwHi = 0;
            CTL.Rlys0_On = 0;
            CTL.Rlys1_On = 0;

            mf.AutoSteerPGN.RelaysChanged += AutoSteerPGN_RelaysChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;

            ToAOG = new PGN234(mf);
        }

        public bool AutoOn()
        {
            return CTL.AutoOn;
        }

        public bool MasterOn()
        {
            return CTL.MasterOn;
        }

        public void ReadRateSwitches()
        {
            if (mf.SwitchBox.RateUp)
            {
                CurrentState.Pressed = true;
                CurrentState.Released = false;
                CurrentState.Changed = PreviousState.Released;
                RateDir = 1;
            }
            else if (mf.SwitchBox.RateDown)
            {
                CurrentState.Pressed = true;
                CurrentState.Released = false;
                CurrentState.Changed = PreviousState.Released;
                RateDir = -1;
            }
            else
            {
                CurrentState.Released = true;
                CurrentState.Pressed = false;
                CurrentState.Changed = PreviousState.Pressed;
                RateStep = 0;
            }
            PreviousState = CurrentState;

            if (CurrentState.Pressed)
            {
                // adjust rate
                if ((DateTime.Now - AdjustTime).TotalMilliseconds > StepDelay)
                {
                    AdjustTime = DateTime.Now;

                    int ID = mf.CurrentProduct();
                    if (ID < 0) ID = 0;
                    clsProduct Prd = mf.Products.Item(ID);

                    RateStep++;
                    if (RateStep > 4) RateStep = 4;

                    if (mf.SwitchBox.SwitchOn(SwIDs.Auto))
                    {
                        // auto rate
                        double CurrentRate = Prd.RateSet;
                        if (CurrentRate == 0) CurrentRate = 1;
                        CurrentRate += CurrentRate * StepMultiplier * RateStep * RateDir;
                        Prd.RateSet = CurrentRate;
                    }
                    else
                    {
                        // manual rate
                        if (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose)
                        {
                            // adjust flow valve
                            Prd.ManualPWM = (int)(Prd.PIDmin * RateStep * RateDir);
                        }
                        else
                        {
                            // adjust motor
                            Prd.ManualPWM += (int)(5 * RateDir);
                        }
                    }
                }
            }

            if (CurrentState.Released && CurrentState.Changed)
            {
                // stop manual adjusting flow valve when rate adjust buttons are not pushed
                int ID = mf.CurrentProduct();
                if (ID < 0) ID = 0;
                clsProduct Prd = mf.Products.Item(ID);

                if (!mf.SwitchBox.SwitchOn(SwIDs.Auto) && (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose))
                {
                    Prd.ManualPWM = 0;
                }
            }
        }

        public void UpdateSectionStatus(bool UpdateAOG = false)
        {
            if (UpdateAOG)
            {
                // update AOG
                ToAOG.OnLo = CTL.Rlys0_On;
                ToAOG.OnHi = CTL.Rlys1_On;
                ToAOG.OffLo = CTL.Rlys0_Off;
                ToAOG.OffHi = CTL.Rlys1_Off;

                if (CTL.AutoOn)
                {
                    ToAOG.Command = 1;
                }
                else
                {
                    ToAOG.Command = 2;
                }

                ToAOG.Send();
            }
            // update sections
            mf.Sections.UpdateSectionsOn(CTL.RC0, CTL.RC1, false);
        }

        private void AutoSteerPGN_RelaysChanged(object sender, PGN254.RelaysChangedArgs e)
        {
            CTL.AOGRlys0 = e.RelayLo;
            CTL.AOGRlys1 = e.RelayHi;

            if (mf.SwitchBox.Connected() || mf.SimMode == SimType.Speed)
            {
                if (CTL.AutoOn && mf.SimMode != SimType.Speed)
                {
                    // auto on, only send off bytes
                    CTL.Rlys0_On = 0;
                    CTL.Rlys1_On = 0;
                    CTL.Rlys0_Off = (byte)~CTL.SBRlys0;
                    CTL.Rlys1_Off = (byte)~CTL.SBRlys1;
                    CTL.RC0 = CTL.AOGRlys0;
                    CTL.RC1 = CTL.AOGRlys1;
                }
                else
                {
                    // switchbox auto off, match switchbox
                    CTL.Rlys0_On = CTL.SBRlys0;
                    CTL.Rlys1_On = CTL.SBRlys1;
                    CTL.Rlys0_Off = (byte)~CTL.Rlys0_On;
                    CTL.Rlys1_Off = (byte)~CTL.Rlys1_On;
                    CTL.RC0 = CTL.SBRlys0;
                    CTL.RC1 = CTL.SBRlys1;
                }
            }
            else
            {
                // no switchbox, match AOG
                CTL.Rlys0_On = CTL.AOGRlys0;
                CTL.Rlys1_On = CTL.AOGRlys1;
                CTL.Rlys0_Off = (byte)~CTL.Rlys0_On;
                CTL.Rlys1_Off = (byte)~CTL.Rlys1_On;
                CTL.RC0 = CTL.AOGRlys0;
                CTL.RC1 = CTL.AOGRlys1;
            }

            UpdateSectionStatus(CTL.Rlys0_On != CTL.AOGRlys0 || CTL.Rlys1_On != CTL.AOGRlys1);
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            CTL.AutoOn = mf.SwitchBox.SwitchOn(SwIDs.Auto);

            // set relay bytes based on switchbox
            if (mf.SwitchBox.SwitchOn(SwIDs.MasterOff))
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = false;
            }
            else if (mf.SwitchBox.SwitchOn(SwIDs.MasterOn) || CTL.MasterOn)
            {
                CTL.SBRlys0 = 0;
                CTL.SBRlys1 = 0;
                CTL.MasterOn = true;

                foreach (clsSection Sec in mf.Sections.Items)
                {
                    if (mf.SwitchBox.SwitchOn((SwIDs)(Sec.SwitchID + 5)) && Sec.Enabled)
                    {
                        if (Sec.ID < 8)
                        {
                            CTL.SBRlys0 = mf.Tls.BitSet(CTL.SBRlys0, Sec.ID);
                        }
                        else
                        {
                            CTL.SBRlys1 = mf.Tls.BitSet(CTL.SBRlys1, Sec.ID - 8);
                        }
                    }
                }
            }

            // set section relay bytes
            if (CTL.MasterOn)
            {
                if (CTL.AutoOn)
                {
                    if (mf.AutoSteerPGN.Connected() && mf.SimMode != SimType.Speed)
                    {
                        // aog connected, only send which switches that are off
                        CTL.Rlys0_On = 0;
                        CTL.Rlys1_On = 0;
                        CTL.Rlys0_Off = (byte)~CTL.SBRlys0;
                        CTL.Rlys1_Off = (byte)~CTL.SBRlys1;
                        CTL.RC0 = CTL.AOGRlys0;
                        CTL.RC1 = CTL.AOGRlys1;
                    }
                    else
                    {
                        // no aog, match switchbox
                        CTL.Rlys0_On = CTL.SBRlys0;
                        CTL.Rlys1_On = CTL.SBRlys1;
                        CTL.Rlys0_Off = (byte)~CTL.Rlys0_On;
                        CTL.Rlys1_Off = (byte)~CTL.Rlys1_On;
                        CTL.RC0 = CTL.SBRlys0;
                        CTL.RC1 = CTL.SBRlys1;
                    }
                }
                else
                {
                    // auto off, match switchbox
                    CTL.Rlys0_On = CTL.SBRlys0;
                    CTL.Rlys1_On = CTL.SBRlys1;
                    CTL.Rlys0_Off = (byte)~CTL.Rlys0_On;
                    CTL.Rlys1_Off = (byte)~CTL.Rlys1_On;
                    CTL.RC0 = CTL.SBRlys0;
                    CTL.RC1 = CTL.SBRlys1;
                }
            }
            else
            {
                // master off, relays off
                CTL.Rlys0_On = 0;
                CTL.Rlys1_On = 0;
                CTL.Rlys0_Off = (byte)~CTL.Rlys0_On;
                CTL.Rlys1_Off = (byte)~CTL.Rlys1_On;
                CTL.RC0 = 0;
                CTL.RC1 = 0;
            }

            UpdateSectionStatus(true);
            ReadRateSwitches();
        }
    }
}