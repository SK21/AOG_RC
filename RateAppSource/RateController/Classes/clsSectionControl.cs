using RateController.PGNs;
using System;

namespace RateController.Classes
{
    public class clsSectionControl
    {
        private readonly System.Windows.Forms.Timer PrimeTimer;
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
        private bool[] RCzoneOn;
        private bool[] SectionOnBySwitchBox;
        private int TimerCount;
        private bool WorkSWOnLast;

        public clsSectionControl()
        {
            SectionOnBySwitchBox = new bool[Props.MaxSections];
            RCzoneOn = new bool[8];

            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.AOGsections.SectionsChanged += AOGsections_SectionsChanged;

            MasterIsOn = false;
            ForceOff = true;
            MasterIsOnLast = true;   // force initial change detection

            PrimeTimer = new System.Windows.Forms.Timer();
            PrimeTimer.Interval = 1000;
            PrimeTimer.Tick += PrimingTimerTick;
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
            var inputs = ReadSwitchInputs();
            HandleMasterAndPrime(ref inputs);
            BuildSectionSwitchIntent();
            ApplySectionsNoZones(inputs);
            SyncAOGNoZones(inputs);
        }

        public void UpdateSectionStatusWithZones()
        {
            var inputs = ReadSwitchInputs();
            HandleMasterAndPrime(ref inputs);
            BuildZoneSwitchIntent();
            ApplySectionsWithZones(inputs);
            SyncAOGWithZones(inputs);
        }

        private void AOGsections_SectionsChanged(object sender, EventArgs e)
        {
            if (Props.UseZones)
            {
                UpdateSectionStatusWithZones();
            }
            else
            {
                UpdateSectionStatusNoZones();
            }

            Core.SendRelays();
        }

        private void ApplySectionsNoZones(SwitchInputs inputs)
        {
            foreach (clsSection sec in Core.Sections.Items)
            {
                bool IsOn = false;
                if (sec.Enabled)
                {
                    if (inputs.AOGConnected && inputs.AutoSectionOn && !cPrimeOn)
                    {
                        IsOn = MasterIsOn && Core.AOGsections.SectionIsOn(sec.ID) && SectionOnBySwitchBox[sec.ID];
                    }
                    else
                    {
                        IsOn = MasterIsOn && SectionOnBySwitchBox[sec.ID];
                    }
                }
                sec.IsON = IsOn;
            }
        }

        private void ApplySectionsWithZones(SwitchInputs inputs)
        {
            for (int i = 0; i < Props.MaxSections; i++)
            {
                bool zoneSwitchOn = ZoneSwitchOnForSection(i);
                bool isOn = false;

                if (inputs.AOGConnected && inputs.AutoSectionOn && !cPrimeOn)
                {
                    isOn = MasterIsOn && zoneSwitchOn && Core.AOGsections.SectionIsOn(i);
                }
                else
                {
                    isOn = MasterIsOn && zoneSwitchOn;
                }

                Core.Sections.Item(i).IsON = isOn;
            }
        }

        private void BuildSectionSwitchIntent()
        {
            Array.Clear(SectionOnBySwitchBox, 0, SectionOnBySwitchBox.Length);

            foreach (clsSection sec in Core.Sections.Items)
            {
                SectionOnBySwitchBox[sec.ID] = sec.Enabled && Core.SwitchBox.SectionSwitchOn(sec.SwitchID);
            }
        }

        private void BuildZoneSwitchIntent()
        {
            Array.Clear(RCzoneOn, 0, RCzoneOn.Length);

            foreach (clsZone zone in Core.Zones.Items)
            {
                RCzoneOn[zone.ID] = zone.Enabled && Core.SwitchBox.SectionSwitchOn(zone.SwitchID);
            }
        }

        private void HandleMasterAndPrime(ref SwitchInputs inputs)
        {
            if (inputs.MasterSWOn)
            {
                SetPriming();
            }
            else
            {
                PrimeInitialized = false;
            }

            if (cPrimeOn)
            {
                inputs.MasterSWOn = true;
            }
            else
            {
                if (inputs.MasterSWOff || ForceOff) MasterSWOnPending = false;
                if (inputs.MasterSWOn) MasterSWOnPending = true;

                inputs.MasterSWOff = inputs.MasterSWOff || !inputs.WorkSWOn || ForceOff;
                inputs.MasterSWOn = (inputs.MasterSWOn || MasterIsOn) && inputs.WorkSWOn;

                if (WorkSWOnLast != inputs.WorkSWOn)
                {
                    WorkSWOnLast = inputs.WorkSWOn;
                    if (inputs.WorkSWOn && MasterSWOnPending)
                    {
                        inputs.MasterSWOn = true;
                    }
                }
            }

            if (inputs.MasterSWOff)
            {
                MasterIsOn = false;
                cPrimeOn = false;
                PrimeTimer.Enabled = false;
                ForceOff = false;
            }
            else if (inputs.MasterSWOn)
            {
                MasterIsOn = true;
            }
        }

        private void PrimingTimerTick(object sender, EventArgs e)
        {
            TimerCount++;

            if (TimerCount > Props.PrimeTime)
            {
                TimerCount = 0;
                PrimeTimer.Enabled = false;
                cPrimeOn = false;
                PrimeInitialized = false;

                ForceOff = !Props.ResumeAfterPrime;
                if (!Props.ResumeAfterPrime)
                {
                    Core.vSwitchBox.PressSwitch(SwIDs.MasterOff);
                }
            }
        }

        private SwitchInputs ReadSwitchInputs()
        {
            return new SwitchInputs
            {
                WorkSWOn = Core.SwitchBox.WorkOn,
                MasterSWOff = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOff),
                MasterSWOn = Core.SwitchBox.SwitchIsOn(SwIDs.MasterOn),
                AutoSectionOn = Core.SwitchBox.AutoSectionOn,
                MachineIsMoving = Props.Speed_KMH > 0.1,
                AOGConnected = Core.AutoSteerPGN.Connected()
            };
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
                    if (!cPrimeOn && (DateTime.Now - OnFirstPressed).TotalSeconds > Props.PrimeDelay && Core.SwitchBox.SwitchIsOn(SwIDs.MasterOn))
                    {
                        cPrimeOn = true;
                        TimerCount = 0;
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
            if (Props.UseZones)
            {
                UpdateSectionStatusWithZones();
            }
            else
            {
                UpdateSectionStatusNoZones();
            }

            Core.SendRelays();
        }

        private void SyncAOGNoZones(SwitchInputs inputs)
        {
            if (inputs.AOGConnected)
            {
                PGN234 ToAOG = new PGN234();
                int max = 16;
                if (Props.MaxSections < max) max = Props.MaxSections;

                if (MasterIsOnLast != MasterIsOn)
                {
                    MasterIsOnLast = MasterIsOn;
                    MasterIsOnChanged = true;
                }

                if (MasterIsOn)
                {
                    bool SectionsChanged = false;

                    for (int i = 0; i < max; i++)
                    {
                        if (SectionOnBySwitchBox[i] != Core.AOGsections.SectionIsOn(i))
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }

                    if (AutoSectionLast != inputs.AutoSectionOn)
                    {
                        AutoSectionsChanged = true;
                        AutoSectionLast = inputs.AutoSectionOn;

                        if (AutoSectionLast)
                        {
                            ToAOG.Command = 1;   // auto on
                        }
                        else
                        {
                            ToAOG.Command = 2;   // auto off
                        }
                    }

                    if (SectionsChanged || MasterIsOnChanged || AutoSectionsChanged)
                    {
                        MasterIsOnChanged = false;
                        AutoSectionsChanged = false;

                        if (!inputs.AutoSectionOn || cPrimeOn)
                        {
                            for (int i = 0; i < max; i++)
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

                    for (int i = 0; i < max; i++)
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

        private void SyncAOGWithZones(SwitchInputs inputs)
        {
            if (inputs.AOGConnected)
            {
                PGN234 ToAOG = new PGN234();
                int max = 8;

                if (MasterIsOnLast != MasterIsOn)
                {
                    MasterIsOnLast = MasterIsOn;
                    MasterIsOnChanged = true;
                }

                if (MasterIsOn)
                {
                    bool SectionsChanged = false;

                    for (int i = 0; i < max; i++)
                    {
                        if (RCzoneOn[i] != Core.AOGsections.SectionIsOn(i))
                        {
                            SectionsChanged = true;
                            break;
                        }
                    }

                    if (AutoSectionLast != inputs.AutoSectionOn)
                    {
                        AutoSectionsChanged = true;
                        AutoSectionLast = inputs.AutoSectionOn;

                        ToAOG.Command = (byte)(AutoSectionLast ? 1 : 2);
                    }

                    if (SectionsChanged || MasterIsOnChanged || AutoSectionsChanged)
                    {
                        MasterIsOnChanged = false;
                        AutoSectionsChanged = false;

                        if (!inputs.AutoSectionOn || cPrimeOn)
                        {
                            for (int i = 0; i < max; i++)
                            {
                                if (RCzoneOn[i])
                                {
                                    ToAOG.OnLo = Core.Tls.BitSet(ToAOG.OnLo, i);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < max; i++)
                    {
                        if (!RCzoneOn[i])
                        {
                            ToAOG.OffLo = Core.Tls.BitSet(ToAOG.OffLo, i);
                        }
                    }
                }
                else
                {
                    if (MasterIsOnChanged)
                    {
                        MasterIsOnChanged = false;
                        ToAOG.Command = 2;
                        AutoSectionLast = false;
                        ToAOG.OffLo = 255;
                        ToAOG.OffHi = 255;
                    }
                }

                ToAOG.Send();
            }
        }

        private bool ZoneSwitchOnForSection(int sectionIndex)
        {
            bool isOn = false;

            foreach (clsZone zone in Core.Zones.Items)
            {
                if (zone.Enabled && sectionIndex >= zone.Start - 1 && sectionIndex < zone.End)
                {
                    isOn = RCzoneOn[zone.ID];
                    break;
                }
            }

            return isOn;
        }

        private struct SwitchInputs
        {
            public bool AOGConnected;
            public bool AutoSectionOn;
            public bool MachineIsMoving;
            public bool MasterSWOff;
            public bool MasterSWOn;
            public bool WorkSWOn;
        }
    }
}
