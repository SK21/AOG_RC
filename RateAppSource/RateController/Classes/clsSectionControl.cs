using RateController.PGNs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsSectionControl
    {
        private const int AdjustDelay = 250;
        private const byte MaxSteps = 5;
        private const int StepDelay = 2000;
        private const double StepMultiplier = 0.025;   // rate change amount for each step
        private DateTime AdjustTime;
        private bool AutoSectionLast;
        private bool AutoSectionsChanged;
        private bool Changed;
        private bool cPrimeOn;
        private bool ForceOff;
        private bool LastState;
        private bool MasterIsOn;
        private bool MasterIsOnChanged;
        private bool MasterIsOnLast;
        private bool MasterSWOnPending;
        private DateTime OnFirstPressed;
        private bool Pressed;
        private bool PrimeInitialized;
        private System.Windows.Forms.Timer PrimeTimer = new System.Windows.Forms.Timer();
        private double RateDir;
        private int RateStep;
        private bool[] RCsectionOn;
        private bool[] RCzoneOn = new bool[8];
        private DateTime StepTime;
        private int TimerCount = 0;
        private bool WorkSWOnLast;

        public clsSectionControl()
        {
            RCsectionOn = new bool[Props.MaxSections];
            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.AOGsections.SectionsChanged += AOGsections_SectionsChanged;
            MasterIsOn = false;
            ForceOff = true;
            MasterIsOnLast = true;  // to cause a change flag to be set
            PrimeTimer.Tick += new EventHandler(PrimingTimerTick);
            PrimeTimer.Interval = 1000;
            PrimeTimer.Enabled = false;
        }

        public bool MasterOn
        { get { return MasterIsOn; } }

        public bool PrimeOn
        { get { return cPrimeOn; } }

        public void ReadRateSwitches()
        {
            try
            {
                if (Core.SwitchBox.RateUp)
                {
                    Pressed = true;
                    RateDir = 1;
                }
                else if (Core.SwitchBox.RateDown)
                {
                    Pressed = true;
                    RateDir = -1;
                }
                else
                {
                    Pressed = false;
                    RateStep = 1;
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

                        int ID = Props.CurrentProduct;
                        if (ID < 0) ID = 0;
                        clsProduct Prd = Core.Products.Item(ID);

                        if (Core.SwitchBox.AutoRateOn)
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
                            if (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose
                                || Prd.ControlType == ControlTypeEnum.ComboCloseTimed)
                            {
                                // adjust flow valve
                                byte ADJ = (byte)(255.0 * Prd.MinPWMadjust / 100.0);
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
                        int ID = Props.CurrentProduct;
                        if (ID < 0) ID = 0;
                        clsProduct Prd = Core.Products.Item(ID);

                        if (!Core.SwitchBox.AutoRateOn &&
                            (Prd.ControlType == ControlTypeEnum.Valve || Prd.ControlType == ControlTypeEnum.ComboClose || Prd.ControlType == ControlTypeEnum.ComboCloseTimed))
                        {
                            Prd.ManualPWM = 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsSectionControl/ " + ex.Message);
            }
        }

        public void UpdateSectionStatusNoZones()
        {
            // only runs when switchbox is connected

            bool WorkSWOn = Core.SwitchBox.WorkOn;
            bool MasterSWOff = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOff);
            bool MasterSWOn = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOn);

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
                // handle work logic
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

            // match switchbox and AOG
            Array.Clear(RCsectionOn, 0, RCsectionOn.Length);

            if (MasterSWOff)
            {
                MasterIsOn = false;
                cPrimeOn = false;
                PrimeTimer.Enabled = false;
                ForceOff = false;
            }
            else if (MasterSWOn)
            {
                MasterIsOn = true;

                //set RC sections by switchbox switch positions
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    RCsectionOn[Sec.ID] = (Core.SwitchBox.SectionSwitchOn(Sec.SwitchID) && Sec.Enabled);
                }
            }

            // set sections on
            if (Core.AutoSteerPGN.Connected() && !cPrimeOn && Core.SwitchBox.AutoSectionOn)
            {
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    if (Sec.Enabled) Sec.IsON = Core.AOGsections.SectionIsOn(Sec.ID);
                }
            }
            else
            {
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    if (Sec.Enabled)
                    {
                        if (Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly)
                        {
                            Sec.IsON = Core.SwitchBox.SectionSwitchOn(Sec.SwitchID);
                        }
                        else
                        {
                            Sec.IsON = RCsectionOn[Sec.ID];
                        }
                    }
                }
            }

            // update AOG
            if (Core.AutoSteerPGN.Connected())
            {
                PGN234 ToAOG = new PGN234();
                int Max = 16;

                if (MasterIsOnLast != MasterIsOn)
                {
                    MasterIsOnLast = MasterIsOn;
                    MasterIsOnChanged = true;
                }

                if (MasterIsOn)
                {
                    // master on
                    bool SectionsChanged = false;
                    for (int i = 0; i < Max; i++)
                    {
                        if (RCsectionOn[i] != Core.AOGsections.SectionIsOn(i))
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }

                    if (AutoSectionLast != Core.SwitchBox.AutoSectionOn)
                    {
                        AutoSectionsChanged = true;
                        AutoSectionLast = Core.SwitchBox.AutoSectionOn;

                        if (AutoSectionLast && MasterIsOn)
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

                    if (SectionsChanged || MasterIsOnChanged || AutoSectionsChanged)
                    {
                        MasterIsOnChanged = false;
                        AutoSectionsChanged = false;

                        if (!Core.SwitchBox.AutoSectionOn)
                        {
                            // auto off, send on bytes to match switchbox
                            for (int i = 0; i < Max; i++)
                            {
                                if (RCsectionOn[i])
                                {
                                    if (i < 8)
                                    {
                                        ToAOG.OnLo = Core.Tls.BitSet(ToAOG.OnLo, i);
                                    }
                                    else
                                    {
                                        ToAOG.OnHi = Core.Tls.BitSet(ToAOG.OnHi, i - 8);
                                    }
                                }
                            }
                        }
                    }

                    // send off bytes to match switchbox
                    if (Props.MaxSections < Max) Max = Props.MaxSections;
                    for (int i = 0; i < Max; i++)
                    {
                        if (!RCsectionOn[i])
                        {
                            if (i < 8)
                            {
                                ToAOG.OffLo = Core.Tls.BitSet(ToAOG.OffLo, i);
                            }
                            else
                            {
                                ToAOG.OffHi = Core.Tls.BitSet(ToAOG.OffHi, i - 8);
                            }
                        }
                    }
                }
                else
                {
                    // master off
                    if (MasterIsOnChanged)
                    {
                        MasterIsOnChanged = false;
                        ToAOG.Command = 2;  // auto off
                        AutoSectionLast = false;
                        ToAOG.OffLo = 255;
                        ToAOG.OffHi = 255;
                    }
                }

                ToAOG.Send();
            }
        }

        public void UpdateSectionStatusWithZones()
        {
            // only runs when switchbox is connected

            bool WorkSWOn = Core.SwitchBox.WorkOn;
            bool MasterSWOff = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOff);
            bool MasterSWOn = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOn);

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
                // handle work switch logic
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

            // match switchbox and AOG
            Array.Clear(RCzoneOn, 0, RCzoneOn.Length);

            if (MasterSWOff)
            {
                MasterIsOn = false;
                cPrimeOn = false;
                PrimeTimer.Enabled = false;
                ForceOff = false;
            }
            else if (MasterSWOn)
            {
                MasterIsOn = true;

                // set RC zones by switchbox switch positions
                foreach (clsZone Zone in Core.Zones.Items)
                {
                    if (Zone.Enabled) RCzoneOn[Zone.ID] = Core.SwitchBox.SectionSwitchOn(Zone.SwitchID);
                }
            }

            // set sections on
            foreach (clsZone Zn in Core.Zones.Items)
            {
                if (Zn.Enabled)
                {
                    if (Core.AutoSteerPGN.Connected() && !cPrimeOn && Core.SwitchBox.AutoSectionOn)
                    {
                        for (int i = Zn.Start - 1; i < Zn.End; i++)
                        {
                            Core.Sections.Item(i).IsON = Core.AOGsections.SectionIsOn(i);
                        }
                    }
                    else
                    {
                        for (int i = Zn.Start - 1; i < Zn.End; i++)
                        {
                            if (Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly)
                            {
                                Core.Sections.Item(i).IsON = Core.SwitchBox.SectionSwitchOn(Zn.SwitchID);
                            }
                            else
                            {
                                Core.Sections.Item(i).IsON = RCzoneOn[Zn.ID];
                            }
                        }
                    }
                }
            }

            // update AOG
            if (Core.AutoSteerPGN.Connected())
            {
                PGN234 ToAOG = new PGN234();

                if (MasterIsOnLast != MasterIsOn)
                {
                    MasterIsOnLast = MasterIsOn;
                    MasterIsOnChanged = true;
                }

                if (MasterIsOn)
                {
                    bool SectionsChanged = false;
                    for (int i = 0; i < Core.AOGsections.SectionCount; i++)
                    {
                        if (Core.SwitchBox.SectionSwitchOn(Core.Sections.Item(i).SwitchID) != Core.AOGsections.SectionIsOn(i))
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }

                    if (AutoSectionLast != Core.SwitchBox.AutoSectionOn)
                    {
                        AutoSectionsChanged = true;
                        AutoSectionLast = Core.SwitchBox.AutoSectionOn;

                        if (AutoSectionLast && MasterIsOn)
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

                    if (SectionsChanged || MasterIsOnChanged || AutoSectionsChanged)
                    {
                        MasterIsOnChanged = false;
                        AutoSectionsChanged = false;

                        if (!Core.SwitchBox.AutoSectionOn)
                        {
                            // auto off, send on bytes to match RC zones
                            foreach (clsZone Zn in Core.Zones.Items)
                            {
                                if (RCzoneOn[Zn.ID] && Zn.ID < 8) ToAOG.OnLo = Core.Tls.BitSet(ToAOG.OnLo, Zn.ID);
                            }
                        }
                    }

                    // send off bytes to match RC zones
                    foreach (clsZone Zn in Core.Zones.Items)
                    {
                        if (!RCzoneOn[Zn.ID] && Zn.ID < 8) ToAOG.OffLo = Core.Tls.BitSet(ToAOG.OffLo, Zn.ID);
                    }
                }
                else
                {
                    // master off
                    if (MasterIsOnChanged)
                    {
                        MasterIsOnChanged = false;
                        ToAOG.Command = 2;  // auto off
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
            if (Core.SwitchBox.Connected())
            {
                if (Props.UseZones)
                {
                    UpdateSectionStatusWithZones();
                }
                else
                {
                    UpdateSectionStatusNoZones();
                }
            }
            else
            {
                // no switchbox, match AOG sections
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    if (Sec.ID < Core.AOGsections.SectionCount)
                    {
                        Sec.IsON = Core.AOGsections.SectionIsOn(Sec.ID);
                    }
                    else
                    {
                        Sec.IsON = false;
                    }
                }
            }
        }

        private void PrimingTimerTick(Object myObject, EventArgs myEventArgs)
        {
            TimerCount++;
            if (TimerCount > Props.PrimeTime)
            {
                TimerCount = 0;
                PrimeTimer.Enabled = false;
                cPrimeOn = false;
                PrimeInitialized = false;

                ForceOff = !Props.ResumeAfterPrime;
                if (!Props.ResumeAfterPrime) Core.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            }
        }

        private void SetPriming()
        {
            // turn sections on if master held in on position for a defined time
            if (PrimeInitialized)
            {
                if (((DateTime.Now - OnFirstPressed).TotalSeconds > Props.PrimeDelay) && Core.SwitchBox.SwitchIsOn(SwIDs.MasterOn))
                {
                    // priming mode
                    cPrimeOn = true;
                    PrimeTimer.Enabled = true;
                }
            }
            else
            {
                if (Props.Speed_KMH < 0.1)
                {
                    PrimeInitialized = true;
                    OnFirstPressed = DateTime.Now;
                    cPrimeOn = false;
                    PrimeTimer.Enabled = false;
                }
            }
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            ReadRateSwitches();
            if (Props.UseZones)
            {
                UpdateSectionStatusWithZones();
            }
            else
            {
                UpdateSectionStatusNoZones();
            }
            Core.SendRelays();    // for quicker response than waiting for TimerMain
        }
    }
}
