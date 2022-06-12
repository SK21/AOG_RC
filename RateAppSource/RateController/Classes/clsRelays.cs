using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsRelays
    {
        public IList<clsRelay> Items;
        private int cPowerRelays;
        private List<clsRelay> cRelays = new List<clsRelay>();
        private FormStart mf;

        public clsRelays(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cRelays.AsReadOnly();
        }

        public int Count()
        {
            return cRelays.Count;
        }

        public clsRelay Item(int RelayID)
        {
            int IDX = ListID(RelayID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cRelays[IDX];
        }

        public void Load()
        {
            cRelays.Clear();
            for (int i = 0; i < mf.MaxRelays; i++)
            {
                clsRelay Rly = new clsRelay(mf, i);
                cRelays.Add(Rly);
                Rly.Load();
            }
            BuildPowerRelays();
        }

        public int PowerRelays()
        { return cPowerRelays; }

        public void Save(int RelayID = 0)
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
                cRelays[ListID(RelayID)].Save();
            }
            BuildPowerRelays();
        }

        public int Status()
        {
            // based on sections status and relay type set relays
            // return int value for relayLo, relayHi

            bool SectionsOn = false;    // whether at least on section is on
            bool MasterOn = false;      // whether at least one master relay is on
            bool MasterFound = false;

            // check if at least one section on
            for (int i = 0; i < mf.MaxSections; i++)
            {
                if(mf.Sections.Item(i).IsON)
                //if (mf.Sections.IsSectionOn(i))
                {
                    SectionsOn = true;
                    break;
                }
            }

            // set master relay
            for (int i = 0; i < cRelays.Count; i++)
            {
                clsRelay Rly = cRelays[i];

                if (Rly.Type == RelayTypes.Master)
                {
                    Rly.IsON = SectionsOn;
                    MasterOn = SectionsOn;
                    MasterFound = true;
                }
            }

            // set section, slave, power relays
            int Result = 0;
            for (int i = 0; i < cRelays.Count; i++)
            {
                clsRelay Rly = cRelays[i];

                switch (Rly.Type)
                {
                    case RelayTypes.Section:
                        if (MasterFound && !MasterOn)
                        {
                            // leave relay to previous value, master relay is off
                            // do nothing
                        }
                        else
                        {
                            // set relay by section
                            Rly.IsON = mf.Sections.Items[Rly.SectionID].IsON;
                        }
                        break;

                    case RelayTypes.Slave:
                        if (MasterFound && !MasterOn)
                        {
                            // leave relay to previous value, master relay is off
                            // do nothing
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
                        if (MasterFound && !MasterOn)
                        {
                            // leave relay to previous value, master relay is off
                            // do nothing
                        }
                        else
                        {
                            // set relay by section
                            Rly.IsON = !mf.Sections.Items[Rly.SectionID].IsON;
                        }
                        break;
                }

                // build return int
                if (Rly.IsON) Result |= (int)Math.Pow(2, i);
            }

            return Result;
        }

        private  void BuildPowerRelays()
        {
            // 16 bit list indicating which relays are power type
            // needed for example when powering off a combo close valve in case of comm failure
            cPowerRelays = 0;
            for (int i = 0; i < cRelays.Count; i++)
            {
                if (cRelays[i].Type == RelayTypes.Power) cPowerRelays |= (int)Math.Pow(2, i);
            }
        }

        private int ListID(int RelayID)
        {
            for (int i = 0; i < cRelays.Count; i++)
            {
                if (cRelays[i].ID == RelayID) return i;
            }
            return -1;
        }
    }
}