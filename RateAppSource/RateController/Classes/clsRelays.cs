using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsRelays
    {
        private readonly List<clsRelay> cRelays = new List<clsRelay>();
        private int[] cInvertedRelays;
        private IList<clsRelay> cItems;
        private int[] cPowerRelays;
        private bool IsLower;
        private bool IsRaise;
        private byte LastTrigger;
        private DateTime LowerStart;
        private byte LowerTimer;
        private DateTime RaiseStart;
        private byte RaiseTimer;

        public clsRelays()
        {
            Items = cRelays.AsReadOnly();
            cPowerRelays = new int[Props.MaxModules];
            cInvertedRelays = new int[Props.MaxModules];
        }

        public IList<clsRelay> Items { get => cItems; set => cItems = value; }

        public int Count()
        {
            return cRelays.Count;
        }

        public int InvertedRelays(int ModuleID)
        {
            int Result = 0;
            if (ModuleID >= 0 && ModuleID < Props.MaxModules) Result = cInvertedRelays[ModuleID];
            return Result;
        }

        public clsRelay Item(int RelayID, int ModuleID)
        {
            int IDX = ListID(RelayID, ModuleID);
            if (IDX == -1) throw new ArgumentOutOfRangeException();
            return cRelays[IDX];
        }

        public void Load(bool LoadfromFile = true)
        {
            cRelays.Clear();
            for (int m = 0; m < Props.MaxModules; m++)
            {
                for (int r = 0; r < Props.MaxRelays; r++)
                {
                    clsRelay Rly = new clsRelay(r, m);
                    cRelays.Add(Rly);
                    if (LoadfromFile) Rly.Load();
                }
            }
            BuildPowerRelays();
            BuildInvertedRelays();
        }

        public int PowerRelays(int ModuleID)
        {
            int Result = 0;
            if (ModuleID >= 0 && ModuleID < Props.MaxModules) Result = cPowerRelays[ModuleID];
            return Result;
        }

        public RelayTypes RelayTypeID(string Description)
        {
            RelayTypes Result = RelayTypes.None;
            var index = Array.IndexOf(Props.TypeDescriptions, Description);
            if (index != -1) Result = (RelayTypes)index;
            return Result;
        }

        public void Renumber(int StartRelay, int StartModule, int StartSection)
        {
            try
            {
                int CurrentSection = StartSection;
                int FirstRelay;
                for (int j = StartModule; j < 8; j++)
                {
                    if (j == StartModule)
                    {
                        FirstRelay = StartRelay;
                    }
                    else
                    {
                        FirstRelay = 0;
                    }
                    for (int i = FirstRelay; i < 16; i++)
                    {
                        clsRelay Rly = Item(i, j);
                        int tmp = Rly.SectionID;
                        //if (Rly.Type == RelayTypes.None && CurrentSection < mf.MaxSections) Rly.Type = RelayTypes.Section;
                        if (Rly.Type == RelayTypes.Section || Rly.Type == RelayTypes.Invert_Section
                            || ((Rly.Type == RelayTypes.TramRight || Rly.Type == RelayTypes.TramLeft) && tmp > 0))
                        {
                            if (CurrentSection < Props.MaxSections)
                            {
                                Rly.SectionID = CurrentSection;
                                CurrentSection++;
                            }
                            else
                            {
                                Rly.Type = RelayTypes.None;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsRelays/Renumber: " + ex.Message);
            }
        }

        public void Reset()
        {
            Load(false);
        }

        public void Save(int RelayID = 0, int ModuleID = 0)
        {
            if (IsValid())
            {
                if (RelayID == 0)
                {
                    // save all
                    for (int i = 0; i < cRelays.Count; i++)
                    {
                        cRelays[i].Save();
                    }
                }
                else
                {
                    // save selected
                    cRelays[ListID(RelayID, ModuleID)].Save();
                }
                BuildPowerRelays();
                BuildInvertedRelays();
            }
            else
            {
                throw new ArgumentException("Relay definitions are not valid.");
            }
        }

        public int SetRelays(int ModuleID)
        {
            int Result = 0;
            try
            {
                // based on sections status and relay type set relays
                // return int value for relayLo, relayHi

                bool SectionsOn = false;        // whether at least on section is on
                bool MasterRelayOn;
                bool MasterFound = false;
                bool FlowEnabled = (Props.Speed_KMH > 0.1);

                if (Props.RateCalibrationOn)
                {
                    MasterRelayOn = true;
                }
                else
                {
                    if (Core.SwitchBox.Connected())
                    {
                        if (Core.SwitchBox.AutoSectionOn)
                        {
                            // auto on when master switch is on and flow enabled
                            MasterRelayOn = Core.SwitchBox.MasterOn && FlowEnabled;
                        }
                        else
                        {
                            // manual on when master switch is on
                            MasterRelayOn = Core.SwitchBox.MasterOn;
                        }
                    }
                    else
                    {
                        // no switchbox, set from aog
                        MasterRelayOn = FlowEnabled;
                    }
                }

                // set master relays
                for (int i = 0; i < cRelays.Count; i++)
                {
                    clsRelay Rly = cRelays[i];

                    if (Rly.Type == RelayTypes.Master)
                    {
                        Rly.IsON = MasterRelayOn;
                        MasterFound = true;
                    }
                }

                // check if at least one section on
                for (int i = 0; i < Props.MaxSections; i++)
                {
                    if (Core.Sections.Item(i).IsON)
                    {
                        SectionsOn = true;
                        break;
                    }
                }

                // set hydraulic relays
                HydUPDown(ModuleID);

                // set tram lines, geo stop
                for (int i = 0; i < cRelays.Count; i++)
                {
                    clsRelay Rly = cRelays[i];
                    if (Rly.ModuleID == ModuleID)
                    {
                        if (Rly.Type == RelayTypes.TramLeft) Rly.IsON = Core.MachineData.TramLeft;
                        if (Rly.Type == RelayTypes.TramRight) Rly.IsON = Core.MachineData.TramRight;
                        if (Rly.Type == RelayTypes.GeoStop) Rly.IsON = Core.MachineData.GeoStop;
                    }
                }

                // set section, slave, power relays
                for (int i = 0; i < cRelays.Count; i++)
                {
                    clsRelay Rly = cRelays[i];
                    if (Rly.ModuleID == ModuleID)
                    {
                        switch (Rly.Type)
                        {
                            case RelayTypes.Section:
                                if ((MasterFound && MasterRelayOn) || !MasterFound
                                    || (MasterFound && Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly))
                                {
                                    // set relay by section
                                    if (Rly.SectionID == -1)
                                    {
                                        // no section
                                        Rly.IsON = false;
                                    }
                                    else
                                    {
                                        if (Rly.Type == RelayTypes.Section && Props.RateCalibrationOn)
                                        {
                                            Rly.IsON = true;
                                        }
                                        else
                                        {
                                            Rly.IsON = Core.Sections.Items[Rly.SectionID].IsON;
                                        }
                                    }
                                }
                                break;

                            case RelayTypes.Slave:
                                if ((MasterFound && MasterRelayOn) || !MasterFound
                                    || (MasterFound && Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly))
                                {
                                    // set relay if at lease one section on
                                    Rly.IsON = SectionsOn;
                                }
                                break;

                            case RelayTypes.Power:
                                cRelays[i].IsON = true;
                                break;

                            case RelayTypes.Invert_Section:
                                if ((MasterFound && MasterRelayOn) || !MasterFound
                                    || (MasterFound && Props.MasterSwitchMode == MasterSwitchMode.ControlMasterRelayOnly))
                                {
                                    // set relay by section
                                    if (Rly.SectionID == -1)
                                    {
                                        // no section
                                        Rly.IsON = false;
                                    }
                                    else
                                    {
                                        Rly.IsON = !Core.Sections.Items[Rly.SectionID].IsON;
                                    }
                                }
                                break;

                            case RelayTypes.Switch:
                                Rly.IsON = Core.SwitchBox.SwitchIsOn((SwIDs)(Rly.SwitchID + 5));
                                break;

                            case RelayTypes.Invert_Master:
                                Rly.IsON = (MasterFound && !MasterRelayOn) || !SectionsOn;
                                break;
                        }

                        // build return int
                        if (Rly.IsON)
                        {
                            Result |= (int)Math.Pow(2, Rly.ID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsRelays/SetRelays: " + ex.Message);
            }

            return Result;
        }

        private void BuildInvertedRelays()
        {
            // a list of relays that should be powered on by the module in case of comm failure with the app
            for (int i = 0; i < Props.MaxModules; i++)
            {
                cInvertedRelays[i] = 0;
                for (int j = 0; j < cRelays.Count; j++)
                {
                    clsRelay Rly = cRelays[j];
                    if ((Rly.Type == RelayTypes.Invert_Section || Rly.Type == RelayTypes.Invert_Master) && Rly.ModuleID == i) cInvertedRelays[i] |= (int)Math.Pow(2, Rly.ID);
                }
            }
        }

        private void BuildPowerRelays()
        {
            // 16 bit list indicating which relays are power type
            // needed for example when powering off a combo close valve in case of comm failure with the app
            for (int i = 0; i < Props.MaxModules; i++)
            {
                cPowerRelays[i] = 0;
                for (int j = 0; j < cRelays.Count; j++)
                {
                    clsRelay Rly = cRelays[j];
                    if (Rly.Type == RelayTypes.Power && Rly.ModuleID == i) cPowerRelays[i] |= (int)Math.Pow(2, Rly.ID);
                }
            }
        }

        private void HydUPDown(int ModuleID)
        {
            byte HydLift = Core.MachineData.HydLift;
            if (HydLift != LastTrigger && (HydLift == 1 || HydLift == 2))
            {
                LastTrigger = HydLift;
                LowerTimer = 0;
                RaiseTimer = 0;
            }

            switch (HydLift)
            {
                case 1:
                    RaiseTimer = 1;
                    LowerStart = DateTime.Now;
                    break;

                case 2:
                    LowerTimer = 1;
                    RaiseStart = DateTime.Now;
                    break;
            }

            if (RaiseTimer > 0)
            {
                RaiseTimer = (byte)(Core.MachineConfig.RaiseTime - (byte)(DateTime.Now - RaiseStart).TotalSeconds);
                LowerTimer = 0;
            }

            if (LowerTimer > 0)
            {
                LowerTimer = (byte)(Core.MachineConfig.LowerTime - (byte)(DateTime.Now - LowerStart).TotalSeconds);
            }

            if ((HydLift == 1 || HydLift == 2) && Core.MachineData.Connected() && (LowerTimer <= 0 || RaiseTimer <= 0))
            {
                // adjust
                IsLower = (LowerTimer > 0);
                IsRaise = (RaiseTimer > 0);
            }
            else
            {
                // turn off
                LowerTimer = 0;
                RaiseTimer = 0;
                LastTrigger = 0;
                IsLower = false;
                IsRaise = false;
            }

            // adjust on/off for HydEnable, ActiveHigh
            IsRaise = IsRaise && Core.MachineConfig.HydEnable;
            if (!Core.MachineConfig.ActiveHigh) IsRaise = !IsRaise;

            IsLower = IsLower && Core.MachineConfig.HydEnable;
            if (!Core.MachineConfig.ActiveHigh) IsLower = !IsLower;

            for (int i = 0; i < cRelays.Count; i++)
            {
                clsRelay Rly = cRelays[i];
                if (Rly.Type == RelayTypes.HydUp && Rly.ModuleID == ModuleID) Rly.IsON = IsRaise;
                if (Rly.Type == RelayTypes.HydDown && Rly.ModuleID == ModuleID) Rly.IsON = IsLower;
            }
        }

        private bool IsValid()
        {
            bool Result = false;
            // need to check validity before save
            //throw new NotImplementedException();
            Result = true;
            return Result;
        }

        private int ListID(int RelayID, int ModuleID)
        {
            for (int i = 0; i < cRelays.Count; i++)
            {
                if (cRelays[i].ID == RelayID && cRelays[i].ModuleID == ModuleID) return i;
            }
            return -1;
        }
    }
}
