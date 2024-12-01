using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsRelays
    {
        private readonly List<clsRelay> cRelays = new List<clsRelay>();
        private readonly FormStart mf;
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

        public clsRelays(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cRelays.AsReadOnly();
            cPowerRelays = new int[mf.MaxModules];
            cInvertedRelays = new int[mf.MaxModules];
        }

        public IList<clsRelay> Items { get => cItems; set => cItems = value; }

        public int Count()
        {
            return cRelays.Count;
        }

        public int InvertedRelays(int ModuleID)
        {
            int Result = 0;
            if (ModuleID >= 0 && ModuleID < mf.MaxModules) Result = cInvertedRelays[ModuleID];
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
            for (int m = 0; m < mf.MaxModules; m++)
            {
                for (int r = 0; r < mf.MaxRelays; r++)
                {
                    clsRelay Rly = new clsRelay(mf, r, m);
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
            if (ModuleID >= 0 && ModuleID < mf.MaxModules) Result = cPowerRelays[ModuleID];
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
                            if (CurrentSection < mf.MaxSections)
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
                mf.Tls.WriteErrorLog("clsRelays/Renumber: " + ex.Message);
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
                bool FlowEnabled = (mf.Products.Item(mf.CurrentProduct()).Speed() > 0.1);

                if (mf.SwitchBox.Connected())
                {
                    if (mf.SwitchBox.AutoSectionOn)
                    {
                        // auto on when master switch is on and flow enabled
                        MasterRelayOn = mf.SwitchBox.MasterOn && FlowEnabled;
                    }
                    else
                    {
                        // manual on when master switch is on
                        MasterRelayOn = mf.SwitchBox.MasterOn;
                    }
                }
                else
                {
                    // no switchbox, set from aog
                    MasterRelayOn = FlowEnabled;
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
                for (int i = 0; i < mf.MaxSections; i++)
                {
                    if (mf.Sections.Item(i).IsON)
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
                        if (Rly.Type == RelayTypes.TramLeft) Rly.IsON = mf.MachineData.TramLeft;
                        if (Rly.Type == RelayTypes.TramRight) Rly.IsON = mf.MachineData.TramRight;
                        if (Rly.Type == RelayTypes.GeoStop) Rly.IsON = mf.MachineData.GeoStop;
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
                                if (MasterFound && !MasterRelayOn)
                                {
                                    // master is switched, don't adjust section relays
                                }
                                else
                                {
                                    // set relay by section
                                    if (Rly.SectionID == -1)
                                    {
                                        // no section
                                        Rly.IsON = false;
                                    }
                                    else
                                    {
                                        Rly.IsON = mf.Sections.Items[Rly.SectionID].IsON;
                                    }
                                }
                                break;

                            case RelayTypes.Slave:
                                if (MasterFound && !MasterRelayOn)
                                {
                                    // master is switched, don't adjust slave relays
                                }
                                else
                                {
                                    // set relay if at lease one section on
                                    Rly.IsON = SectionsOn;
                                }
                                break;

                            case RelayTypes.Power:
                                cRelays[i].IsON = true;
                                break;

                            case RelayTypes.Invert_Section:
                                if (MasterFound && !MasterRelayOn)
                                {
                                    // master is switched, don't adjust invert relays
                                }
                                else
                                {
                                    // set relay by section
                                    if (Rly.SectionID == -1)
                                    {
                                        // no section
                                        Rly.IsON = false;
                                    }
                                    else
                                    {
                                        Rly.IsON = !mf.Sections.Items[Rly.SectionID].IsON;
                                    }
                                }
                                break;

                            case RelayTypes.Switch:
                                bool Found = false;
                                foreach (clsSwitch SW in mf.SwitchObjects.Items)
                                {
                                    if (SW.RelayID - 1 == Rly.ID && SW.ModuleID == ModuleID)
                                    {
                                        Rly.IsON = mf.SwitchBox.SwitchIsOn((SwIDs)(SW.ID + 5));
                                        Found = true;
                                        break;
                                    }
                                }
                                if (!Found) Rly.IsON = false;
                                break;
                        }

                        // build return int
                        if (Rly.IsON) Result |= (int)Math.Pow(2, Rly.ID);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("clsRelays/SetRelays: " + ex.Message);
            }

            return Result;
        }

        private void BuildInvertedRelays()
        {
            for (int i = 0; i < mf.MaxModules; i++)
            {
                cInvertedRelays[i] = 0;
                for (int j = 0; j < cRelays.Count; j++)
                {
                    clsRelay Rly = cRelays[j];
                    if (Rly.Type == RelayTypes.Invert_Section && Rly.ModuleID == i) cInvertedRelays[i] |= (int)Math.Pow(2, Rly.ID);
                }
            }
        }

        private void BuildPowerRelays()
        {
            // 16 bit list indicating which relays are power type
            // needed for example when powering off a combo close valve in case of comm failure
            for (int i = 0; i < mf.MaxModules; i++)
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
            byte HydLift = mf.MachineData.HydLift;
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
                RaiseTimer = (byte)(mf.MachineConfig.RaiseTime - (byte)(DateTime.Now - RaiseStart).TotalSeconds);
                LowerTimer = 0;
            }

            if (LowerTimer > 0)
            {
                LowerTimer = (byte)(mf.MachineConfig.LowerTime - (byte)(DateTime.Now - LowerStart).TotalSeconds);
            }

            if ((HydLift == 1 || HydLift == 2) && mf.MachineData.Connected() && (LowerTimer <= 0 || RaiseTimer <= 0))
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
            IsRaise = IsRaise && mf.MachineConfig.HydEnable;
            if (!mf.MachineConfig.ActiveHigh) IsRaise = !IsRaise;

            IsLower = IsLower && mf.MachineConfig.HydEnable;
            if (!mf.MachineConfig.ActiveHigh) IsLower = !IsLower;

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