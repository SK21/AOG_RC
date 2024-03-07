using System;
using System.Diagnostics;
using System.Security.Policy;

namespace RateController
{
    public class clsSectionControl
    {
        private const int AdjustDelay = 500;
        private const byte MaxSteps = 5;
        private const int StepDelay = 2000;
        private const double StepMultiplier = 0.05;   // rate change amount for each step
        private DateTime AdjustTime;
        private bool AutoLast;
        private bool AutoSectionLast;
        private bool Changed;
        private bool cPrimeOn;
        private bool ForceOff;
        private bool LastState;
        private bool MasterChanged;
        private bool MasterIsOn;
        private bool MasterLast;
        private bool MasterSWOnPending;
        private FormStart mf;
        private DateTime OnFirstPressed;
        private bool Pressed;
        private bool PrimeInitialized;
        private double RateDir;
        private int RateStep;
        private bool[] SectionOnAOG;
        private bool[] SectionOnSB;
        private DateTime StepTime;
        private System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer();
        private int TimerCount = 0;
        private bool WorkSWOnLast;
        private bool[] RCzoneOn = new bool[8];

        public clsSectionControl(FormStart CallingForm)
        {
            mf = CallingForm;
            SectionOnSB = new bool[mf.MaxSections];
            SectionOnAOG = new bool[mf.MaxSections];
            mf.AutoSteerPGN.RelaysChanged += AutoSteerPGN_RelaysChanged;
            mf.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            mf.AOGsections.SectionsChanged += AOGsections_SectionsChanged;
            MasterIsOn = false;
            ForceOff = true;
            MasterLast = true;  // to cause a change flag to be set
            Timer1.Tick += new EventHandler(Timer1_Tick);
            Timer1.Interval = 1000;
            Timer1.Enabled = false;
        }

        public bool MasterOn
        { get { return MasterIsOn; } }

        public bool PrimeOn
        { get { return cPrimeOn; } }

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

                        if (mf.SwitchBox.SwitchIsOn(SwIDs.Auto) | mf.SwitchBox.SwitchIsOn(SwIDs.AutoRate))
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

                        if (!mf.SwitchBox.SwitchIsOn(SwIDs.Auto) && !mf.SwitchBox.SwitchIsOn(SwIDs.AutoRate) && (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose))
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
            if(mf.UseZones)
            {
                UpdateSectionStatusWithZones();
            }
            else
            {
                UpdateSectionStatusNoZones();
            }
        }

        private void UpdateSectionStatusWithZones()
        {
            bool WorkSWOn;
            bool MasterSWOn;
            bool MasterSWOff;

            // match switchbox and AOG
            Array.Clear(RCzoneOn, 0, RCzoneOn.Length);

            WorkSWOn = mf.SwitchBox.WorkOn;
            MasterSWOff = mf.SwitchBox.SwitchIsOn(SwIDs.MasterOff);
            MasterSWOn = mf.SwitchBox.SwitchIsOn(SwIDs.MasterOn);

            if (MasterSWOn)
            {
                SetPriming();
            }
            else
            {
                PrimeInitialized = false;
            }

            if (cPrimeOn)
            {
                MasterSWOn = true;
            }
            else
            {
                if (MasterSWOff || ForceOff) MasterSWOnPending = false;
                if (MasterSWOn) MasterSWOnPending = true;

                MasterSWOff = MasterSWOff || !WorkSWOn || ForceOff;
                MasterSWOn = (MasterSWOn || MasterIsOn) && WorkSWOn;

                if (WorkSWOnLast != WorkSWOn)
                {
                    WorkSWOnLast = WorkSWOn;
                    if (WorkSWOn && MasterSWOnPending) MasterSWOn = true;
                }
            }

            if (MasterSWOff)
            {
                MasterIsOn = false;
                cPrimeOn = false;
                Timer1.Enabled = false;
                ForceOff = false;
            }
            else if (MasterSWOn)
            {
                MasterIsOn = true;

                // set zones by switchbox switch positions
                foreach (clsZone Zone in mf.Zones.Items)
                {
                    RCzoneOn[Zone.ID] = mf.SwitchBox.SectionSwitchOn(Zone.SwitchID);
                }

                if ((mf.SwitchBox.SwitchIsOn(SwIDs.Auto) || mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection)) && mf.AutoSteerPGN.Connected() && !cPrimeOn)
                {
                    // match AOG zone status only on RC zones that are on
                    // any AOG section on in a zone makes whole RC zone on
                    foreach (clsZone Zn in mf.Zones.Items)
                    {
                        if (RCzoneOn[Zn.ID])
                        {
                            for (int i = Zn.Start; i < Zn.End + 1; i++)
                            {
                                if(mf.AOGsections.SectionIsOn(i))
                                {
                                    RCzoneOn[Zn.ID] = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            // set sections on
            foreach (clsZone Zn in mf.Zones.Items)
            {
                for (int i = Zn.Start; i < Zn.End + 1; i++)
                {
                    mf.Sections.Item(i).IsON = RCzoneOn[Zn.ID];
                }
            }

            if (MasterLast != MasterIsOn)
            {
                MasterLast = MasterIsOn;
                MasterChanged = true;
            }

            // update AOG
            if (mf.AutoSteerPGN.Connected())
            {
                PGN234 ToAOG = new PGN234(mf);
                if (MasterIsOn)
                {
                    // master on
                    bool SectionsChanged = false;
                    for (int i = 0; i < mf.AOGsections.SectionCount; i++)
                    {
                        if (mf.Sections.Item(i).IsON != mf.AOGsections.SectionIsOn(i))
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }
                    Debug.Print("SectionsChanged: " + SectionsChanged.ToString());

                    if (SectionsChanged || MasterChanged)
                    {
                        MasterChanged = false;

                        // send off bytes to match RC zones
                        ToAOG.OffLo = 0;
                        ToAOG.OffHi = 0;
                        foreach (clsZone Zn in mf.Zones.Items)
                        {
                            if (!RCzoneOn[Zn.ID] && Zn.ID < 8) ToAOG.OffLo = mf.Tls.BitSet(ToAOG.OffLo, Zn.ID);
                        }

                        if (!mf.SwitchBox.SwitchIsOn(SwIDs.Auto) && !mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection))
                        {
                            // auto off, send on bytes to match RC zones
                            ToAOG.OnLo = 0;
                            ToAOG.OnHi = 0;
                            foreach (clsZone Zn in mf.Zones.Items)
                            {
                                if (RCzoneOn[Zn.ID] && Zn.ID < 8) ToAOG.OnLo = mf.Tls.BitSet(ToAOG.OnLo, Zn.ID);
                            }
                        }
                    }

                    //// check for switches off
                    //ToAOG.OffLo = 0;
                    //ToAOG.OffHi = 0;
                    //foreach (clsZone Zn in mf.Zones.Items)
                    //{
                    //    if (!RCzoneOn[Zn.ID] && Zn.ID < 8) ToAOG.OffLo = mf.Tls.BitSet(ToAOG.OffLo, Zn.ID);
                    //}

                    if (AutoLast != mf.SwitchBox.SwitchIsOn(SwIDs.Auto) || AutoSectionLast != mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection))
                    {
                        AutoLast = mf.SwitchBox.SwitchIsOn(SwIDs.Auto);
                        AutoSectionLast = mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection);

                        if (AutoLast && MasterIsOn || AutoSectionLast & MasterIsOn)
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
                        AutoSectionLast = false;
                        ToAOG.OffLo = 255;
                        ToAOG.OffHi = 255;
                    }
                }

                Debug.Print(ToAOG.OffLo.ToString() + ", " + ToAOG.OnLo.ToString());
                ToAOG.Send();
            }
        }

        private void UpdateSectionStatusNoZones()
        {
            bool WorkSWOn;
            bool MasterSWOn;
            bool MasterSWOff;

            // match switchbox and AOG
            Array.Clear(SectionOnSB, 0, SectionOnSB.Length);

            WorkSWOn = mf.SwitchBox.WorkOn;
            MasterSWOff = mf.SwitchBox.SwitchIsOn(SwIDs.MasterOff);
            MasterSWOn = mf.SwitchBox.SwitchIsOn(SwIDs.MasterOn);

            if (MasterSWOn)
            {
                SetPriming();
            }
            else
            {
                PrimeInitialized = false;
            }

            if (cPrimeOn)
            {
                MasterSWOn = true;
            }
            else
            {
                if (MasterSWOff || ForceOff) MasterSWOnPending = false;
                if (MasterSWOn) MasterSWOnPending = true;

                MasterSWOff = MasterSWOff || !WorkSWOn || ForceOff;
                MasterSWOn = (MasterSWOn || MasterIsOn) && WorkSWOn;

                if (WorkSWOnLast != WorkSWOn)
                {
                    WorkSWOnLast = WorkSWOn;
                    if (WorkSWOn && MasterSWOnPending) MasterSWOn = true;
                }
            }

            if (MasterSWOff)
            {
                MasterIsOn = false;
                cPrimeOn = false;
                Timer1.Enabled = false;
                ForceOff = false;
            }
            else if (MasterSWOn)
            {
                MasterIsOn = true;

                // set sections by switchbox switch positions
                foreach (clsSection Sec in mf.Sections.Items)
                {
                    SectionOnSB[Sec.ID] = (mf.SwitchBox.SectionSwitchOn(Sec.SwitchID) && Sec.Enabled);
                }

                if ((mf.SwitchBox.SwitchIsOn(SwIDs.Auto) || mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection)) && mf.AutoSteerPGN.Connected() && !cPrimeOn)
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

            if (MasterLast != MasterIsOn)
            {
                MasterLast = MasterIsOn;
                MasterChanged = true;
            }

            // update AOG
            if (mf.AutoSteerPGN.Connected())
            {
                PGN234 ToAOG = new PGN234(mf);
                int Max = 16;
                if (MasterIsOn)
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

                        if (!mf.SwitchBox.SwitchIsOn(SwIDs.Auto) && !mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection))
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

                    if (AutoLast != mf.SwitchBox.SwitchIsOn(SwIDs.Auto) || AutoSectionLast != mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection))
                    {
                        AutoLast = mf.SwitchBox.SwitchIsOn(SwIDs.Auto);
                        AutoSectionLast = mf.SwitchBox.SwitchIsOn(SwIDs.AutoSection);

                        if (AutoLast && MasterIsOn || AutoSectionLast & MasterIsOn)
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
                        AutoSectionLast = false;
                        ToAOG.OffLo = 255;
                        ToAOG.OffHi = 255;
                    }
                }

                ToAOG.Send();
            }
        }
        private void AOGsections_SectionsChanged(object sender, EventArgs e)
        {
            Debug.Print("AOGsections_SectionsChanged");
            if(mf.SwitchBox.Connected())
            {
                UpdateSectionStatus();
            }
            else
            {
                // no switchbox, match AOG zones
                foreach(clsSection Sec in mf.Sections.Items)
                {
                    if (Sec.ID < mf.AOGsections.SectionCount   )
                    {
                        Sec.IsON=mf.AOGsections.SectionIsOn(Sec.ID);
                    }
                    else
                    {
                        Sec.IsON = false;
                    }
                }
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

        private void SetPriming()
        {
            if (PrimeInitialized)
            {
                if (((DateTime.Now - OnFirstPressed).TotalSeconds > mf.PrimeDelay) && mf.SwitchBox.SwitchIsOn(SwIDs.MasterOn))
                {
                    // priming mode
                    cPrimeOn = true;
                    Timer1.Enabled = true;
                }
            }
            else
            {
                if (mf.Products.Item(mf.CurrentProduct()).Speed() < 0.1)
                {
                    PrimeInitialized = true;
                    OnFirstPressed = DateTime.Now;
                    cPrimeOn = false;
                    Timer1.Enabled = false;
                }
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, PGN32618.SwitchPGNargs e)
        {
            ReadRateSwitches();
            UpdateSectionStatus();
        }

        private void Timer1_Tick(Object myObject, EventArgs myEventArgs)
        {
            TimerCount++;
            if (TimerCount > mf.PrimeTime)
            {
                TimerCount = 0;
                Timer1.Enabled = false;
                cPrimeOn = false;
                PrimeInitialized = false;
                ForceOff = true;
            }
        }
    }
}