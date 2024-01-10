using System;
using System.Diagnostics;

namespace RateController
{
    public class clsSectionControl
    {
        private const double StepMultiplier = 0.05;   // rate change amount for each step
        private const byte MaxSteps = 5;
        private const int StepDelay = 2000;     // ms between step adjustments
        private DateTime StepTime;
        private const int AdjustDelay = 500;    // ms between rate adjustments
        private DateTime AdjustTime;
        private bool AutoLast;
        private bool Changed;
        private bool LastState;
        private bool MasterChanged;
        private bool MasterLast;
        private bool MasterOnSB;
        private FormStart mf;
        private bool Pressed;
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
                    Pressed = true;
                    RateDir = 1;
                }
                else if (mf.SwitchBox.RateDown)
                {
                    Pressed = true;
                    RateDir = -1;
                }
                else
                {
                    Pressed = false;
                    RateStep = 0;
                }
                Changed = (LastState != Pressed);
                LastState = Pressed;

                if (Pressed)
                {
                    // set step
                    if ((DateTime.Now - StepTime).TotalMilliseconds > StepDelay)
                    {
                        StepTime = DateTime.Now;
                        RateStep++;
                        if (RateStep > MaxSteps) RateStep = MaxSteps;
                    }

                    // adjust rate
                    if ((DateTime.Now - AdjustTime).TotalMilliseconds > AdjustDelay)
                    {
                        AdjustTime = DateTime.Now;

                        int ID = mf.CurrentProduct();
                        if (ID < 0) ID = 0;
                        clsProduct Prd = mf.Products.Item(ID);

                        if (mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
                        {
                            // auto rate
                            double CurrentRate = Prd.RateSet;
                            if (CurrentRate == 0) CurrentRate = 1;

                            if (RateDir == 1)
                            {
                                CurrentRate = CurrentRate * (1 + (StepMultiplier * RateStep));
                            }
                            else
                            {
                                CurrentRate = CurrentRate / (1 + (StepMultiplier * RateStep));
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
                                Prd.ManualPWM = (int)((ADJ + ADJ * StepMultiplier * RateStep) * RateDir);
                            }
                            else
                            {
                                // adjust motor
                                Prd.ManualPWM += (int)(5 * RateDir);
                            }
                        }
                    }
                }
                else
                {
                    if (Changed)
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

            if (MasterLast != MasterOnSB)
            {
                MasterLast = MasterOnSB;
                MasterChanged = true;
            }

            // update AOG
            if (mf.AutoSteerPGN.Connected())
            {
                PGN234 ToAOG = new PGN234(mf);
                int Max = 16;
                if (MasterOnSB)
                {
                    // master on
                    bool SectionsChanged = false;
                    for (int i = 0; i < 8; i++)
                    {
                        if (SectionOnSB[i] != SectionOnAOG[i] || SectionOnSB[i + 8] != SectionOnAOG[i + 8])
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }

                    if (SectionsChanged || MasterChanged)
                    {
                        MasterChanged = false;

                        // send off bytes to match switchbox
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

                        if (!mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
                        {
                            // auto off, send on bytes to match switchbox
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
                    }

                    // check for switches off
                    foreach (clsSection Sec in mf.Sections.Items)
                    {
                        if (!mf.SwitchBox.SectionSwitchOn(Sec.SwitchID) || !Sec.Enabled)
                        {
                            if (Sec.ID < 8)
                            {
                                ToAOG.OffLo = mf.Tls.BitSet(ToAOG.OffLo, Sec.ID);
                            }
                            else if (Sec.ID < 16)
                            {
                                ToAOG.OffHi = mf.Tls.BitSet(ToAOG.OffHi, Sec.ID - 8);
                            }
                        }
                    }

                    if (AutoLast != mf.SwitchBox.SwitchIsOn(SwIDs.Auto))
                    {
                        AutoLast = mf.SwitchBox.SwitchIsOn(SwIDs.Auto);
                        if (AutoLast && MasterOnSB)
                        {
                            // auto on
                            ToAOG.Command = 1;
                        }
                        else
                        {
                            // auto off
                            ToAOG.Command = 2;
                        }
                    }
                }
                else
                {
                    // master off
                    if (MasterChanged)
                    {
                        MasterChanged = false;
                        ToAOG.Command = 2;  // auto off
                        AutoLast = false;
                        ToAOG.OffLo = 255;
                        ToAOG.OffHi = 255;
                    }
                }

                ToAOG.Send();
            }
        }

        private void AutoSteerPGN_RelaysChanged(object sender, PGN254.RelaysChangedArgs e)
        {
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
            UpdateSectionStatus();
        }
    }
}