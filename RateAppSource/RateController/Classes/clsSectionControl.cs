using System;
using System.Diagnostics;

namespace RateController
{
    public class clsSectionControl
    {
        private const double AutoMultiplier = 0.05;
        private const byte ManualMin = 10;
        private const double ManualMultiplier = 0.25;   // rate change amount for each step
        private const byte MaxSteps = 5;
        private const int StepDelay = 1000; // ms between rate adjustments
        private DateTime AdjustTime;
        private State CurrentState;
        private bool MasterOnSB;
        private FormStart mf;
        private State PreviousState;
        private double RateDir;
        private int RateStep;
        private bool[] SectionOnAOG;
        private bool[] SectionOnSB;

        public clsSectionControl(FormStart CallingForm)
        {
            mf = CallingForm;
            SectionOnSB = new bool[mf.MaxSections];
            SectionOnAOG = new bool[mf.MaxSections];
            mf.AutoSteerPGN.RelaysChanged += AutoSteerPGN_RelaysChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            MasterOnSB = true;
        }

        public bool MasterOn()
        {
            return MasterOnSB;
        }

        public void ReadRateSwitches()
        {
            try
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
                        if (RateStep > MaxSteps) RateStep = MaxSteps;

                        if (mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
                        {
                            // auto rate
                            double CurrentRate = Prd.RateSet;
                            if (CurrentRate == 0) CurrentRate = 1;

                            if (RateDir == 1)
                            {
                                CurrentRate = CurrentRate * (1 + (AutoMultiplier * RateStep));
                            }
                            else
                            {
                                CurrentRate = CurrentRate / (1 + (AutoMultiplier * RateStep));
                            }

                            Prd.RateSet = CurrentRate;
                        }
                        else
                        {
                            // manual rate
                            if (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose)
                            {
                                // adjust flow valve
                                byte ADJ = Prd.PIDmin;
                                if (ADJ < ManualMin) ADJ = ManualMin;
                                Prd.ManualPWM = (int)((ADJ + ADJ * ManualMultiplier * RateStep) * RateDir);
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

                    if (!mf.SwitchBox.SwitchIsOn(SwIDs.Auto) && (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose))
                    {
                        Prd.ManualPWM = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsSectionControl/ " + ex.Message);
            }
        }

        public void UpdateSectionStatus()
        {
            // match switchbox and AOG
            Array.Clear(SectionOnSB, 0, SectionOnSB.Length);

            if (mf.SwitchBox.SwitchIsOn(SwIDs.MasterOff))
            {
                MasterOnSB = false;
            }
            else if (mf.SwitchBox.SwitchIsOn(SwIDs.MasterOn) || MasterOnSB)
            {
                MasterOnSB = true;

                // set sections by switchbox switch positions
                foreach (clsSection Sec in mf.Sections.Items)
                {
                    SectionOnSB[Sec.ID] = (mf.SwitchBox.SectionSwitchOn(Sec.SwitchID) && Sec.Enabled);
                }

                if (mf.SwitchBox.SwitchIsOn(SwIDs.Auto) && mf.AutoSteerPGN.Connected())
                {
                    // match AOG section status, only on sections 0-15
                    for (int i = 0; i < 16; i++)
                    {
                        if (SectionOnSB[i])
                        {
                            // check if AOG has switched it off
                            SectionOnSB[i] = SectionOnAOG[i];
                        }
                    }
                }
            }
            foreach (clsSection Sec in mf.Sections.Items)
            {
                Sec.IsON = SectionOnSB[Sec.ID];
            }

            UpdateAOG();
        }

        private void AutoSteerPGN_RelaysChanged(object sender, PGN254.RelaysChangedArgs e)
        {
            Debug.Print("AutoSteerPGN_RelaysChanged");
            Array.Clear(SectionOnAOG, 0, SectionOnAOG.Length);

            // only sections 0-15 are set in this pgn
            for (int i = 0; i < 8; i++)
            {
                SectionOnAOG[i] = mf.Tls.BitRead(e.RelayLo, i);
                SectionOnAOG[i + 8] = mf.Tls.BitRead(e.RelayHi, i);
            }

            if (mf.SwitchBox.Connected())
            {
                UpdateSectionStatus();
            }
            else
            {
                // no switchbox, match AOG
                foreach (clsSection Sec in mf.Sections.Items)
                {
                    if (Sec.ID < 16)
                    {
                        Sec.IsON = SectionOnAOG[Sec.ID];
                    }
                    else
                    {
                        Sec.IsON = false;
                    }
                }
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            ReadRateSwitches();

            Array.Clear(SectionOnSB, 0, SectionOnSB.Length);

            if (mf.SwitchBox.SwitchIsOn(SwIDs.MasterOff))
            {
                MasterOnSB = false;
            }
            else if (mf.SwitchBox.SwitchIsOn(SwIDs.MasterOn) || MasterOnSB)
            {
                MasterOnSB = true;

                // set sections by switchbox switch positions
                foreach (clsSection Sec in mf.Sections.Items)
                {
                    SectionOnSB[Sec.ID] = (mf.SwitchBox.SectionSwitchOn(Sec.SwitchID) && Sec.Enabled);
                }
            }

            foreach (clsSection Sec in mf.Sections.Items)
            {
                Sec.IsON = SectionOnSB[Sec.ID];
            }
            if (mf.AutoSteerPGN.Connected()) UpdateAOG();
        }

        private void UpdateAOG()
        {
            PGN234 ToAOG = new PGN234(mf);
            int Max = 16;

            if (mf.MaxSections < Max) Max = mf.MaxSections;
            for (int i = 0; i < Max; i++)
            {
                if (!mf.Sections.Items[i].IsON)
                {
                    if (i < 8)
                    {
                        ToAOG.OffLo = mf.Tls.BitSet(ToAOG.OffLo, i);
                    }
                    else
                    {
                        ToAOG.OffHi = mf.Tls.BitSet(ToAOG.OffHi, i);
                    }
                }
            }

            if (mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
            {
                // auto on, only send off bytes
                ToAOG.Command = 1;
            }
            else
            {
                // auto off, send off and on bytes to match switchbox
                ToAOG.Command = 2;
                for (int i = 0; i < Max; i++)
                {
                    if (mf.Sections.Items[i].IsON)
                    {
                        if (i < 8)
                        {
                            ToAOG.OnLo = mf.Tls.BitSet(ToAOG.OnLo, i);
                        }
                        else
                        {
                            ToAOG.OnHi = mf.Tls.BitSet(ToAOG.OnHi, i);
                        }
                    }
                }
            }
            ToAOG.Send();
        }

        private struct State
        {
            public bool Changed;
            public bool Pressed;
            public bool Released;
        }
    }
}