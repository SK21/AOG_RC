using RateController.PGNs;
using System;

namespace RateController.Classes
{
    public class clsSectionControl
    {
        private bool AutoSectionLast;
        private bool AutoSectionsChanged;
        private bool cPrimeOn;
        private bool ForceOff;
        private bool MasterIsOn;
        private bool MasterIsOnChanged;
        private bool MasterIsOnLast;
        private bool MasterSWOnPending;
        private DateTime OnFirstPressed;
        private bool PrimeInitialized;
        private System.Windows.Forms.Timer PrimeTimer = new System.Windows.Forms.Timer();
        private bool[] RCzoneOn = new bool[8];
        private bool[] SectionOnBySwitchBox;
        private int TimerCount = 0;

        private bool WorkSWOnLast;

        public clsSectionControl()
        {
            SectionOnBySwitchBox = new bool[Props.MaxSections];
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

        public void StartPrime()
        {
            if (!Props.MasterMaintained && Props.Speed_KMH < 0.1)
            {
                cPrimeOn = true;
                TimerCount = 0;
                PrimeTimer.Enabled = true;
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
            Array.Clear(SectionOnBySwitchBox, 0, SectionOnBySwitchBox.Length);

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

                //set sections on by switchbox switch positions
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    SectionOnBySwitchBox[Sec.ID] = (Core.SwitchBox.SectionSwitchOn(Sec.SwitchID) && Sec.Enabled);
                }
            }

            // set sections on
            bool MachineIsMoving = (Props.Speed_KMH > 0.1);
            if (Core.AutoSteerPGN.Connected() && !cPrimeOn && Core.SwitchBox.AutoSectionOn)
            {
                // AOG auto section control
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    if (Sec.Enabled) Sec.IsON = MasterIsOn && Core.AOGsections.SectionIsOn(Sec.ID) && MachineIsMoving;
                }
            }
            else
            {
                // manual control or priming or no AOG
                bool IsOn;
                foreach (clsSection Sec in Core.Sections.Items)
                {
                    if (Sec.Enabled)
                    {
                        IsOn = false;
                        if (!Core.SwitchBox.AutoSectionOn || MachineIsMoving)
                        {
                            IsOn = SectionOnBySwitchBox[Sec.ID];
                        }
                        Sec.IsON = IsOn;
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
                        if (SectionOnBySwitchBox[i] != Core.AOGsections.SectionIsOn(i))
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
                                if (SectionOnBySwitchBox[i])
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
                        if (!SectionOnBySwitchBox[i])
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
            bool MachineIsMoving = (Props.Speed_KMH > 0.1);
            foreach (clsZone Zn in Core.Zones.Items)
            {
                if (Zn.Enabled)
                {
                    if (Core.AutoSteerPGN.Connected() && !cPrimeOn && Core.SwitchBox.AutoSectionOn)
                    {
                        for (int i = Zn.Start - 1; i < Zn.End; i++)
                        {
                            Core.Sections.Item(i).IsON = MasterIsOn && MachineIsMoving && Core.AOGsections.SectionIsOn(i);
                        }
                    }
                    else
                    {
                        for (int i = Zn.Start - 1; i < Zn.End; i++)
                        {
                            Core.Sections.Item(i).IsON = RCzoneOn[Zn.ID];
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
            if (Props.MasterMaintained)
            {
                // priming disabled with a maintained master switch
                cPrimeOn = false;
            }
            else
            {
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
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            if (Core.SwitchBox.RateUp || Core.SwitchBox.RateDown) Core.SendRateSettings();
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