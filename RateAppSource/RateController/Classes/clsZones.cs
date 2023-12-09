using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsZones
    {
        public IList<clsZone> Items;
        private List<clsZone> cZones = new List<clsZone>();
        private FormStart mf;

        public clsZones(FormStart CallingFrom)
        {
            mf = CallingFrom;
            Items = cZones.AsReadOnly();
        }

        public int Count
        { get { return Items.Count; } }

        public void Build(int SectionsPerZone)
        {
            int ZoneCount = 0;
            int CurrentZone = 0;
            for (int i = 0; i < 8; i++)
            {
                cZones[i].Start = 0;
                cZones[i].End = 0;
            }

            foreach (clsSection Sec in mf.Sections.Items)
            {
                if (Sec.Enabled)
                {
                    if (cZones[CurrentZone].Start == 0) cZones[CurrentZone].Start = Sec.ID + 1;
                    ZoneCount++;
                    if (ZoneCount >= SectionsPerZone)
                    {
                        ZoneCount = 0;
                        CurrentZone++;
                    }
                    if (CurrentZone > 7) CurrentZone = 7;
                }
            }

            for (int i = 0; i < 7; i++)
            {
                if (cZones[i].Start > 0)
                {
                    if (cZones[i + 1].Start == 0)
                    {
                        cZones[i].End = mf.Sections.Count;
                    }
                    else
                    {
                        cZones[i].End = cZones[i + 1].Start - 1;
                    }
                }
                else
                {
                    cZones[i].End = 0;
                }
            }
            if (cZones[7].Start > 0)
            {
                cZones[7].End = mf.Sections.Count;
            }
            else
            {
                cZones[7].End = 0;
            }
        }

        public clsZone Item(int ID)
        {
            int IDX = ListID(ID);
            if (IDX == -1) throw new ArgumentException("clsZones/Invalid section number.");
            return cZones[IDX];
        }

        public void Load()
        {
            cZones.Clear();
            for (int i = 0; i < 8; i++)
            {
                clsZone Zone = new clsZone(mf, i);
                Zone.Load();
                cZones.Add(Zone);
            }
        }

        public void Save()
        {
            for (int i = 0; i < cZones.Count; i++)
            {
                cZones[i].Save();

                if (cZones[i].Start > 0)
                {
                    for (int j = cZones[i].Start; j <= cZones[i].End; j++)
                    {
                        clsSection Sec = mf.Sections.Item(j - 1);
                        if (Sec.Enabled)
                        {
                            Sec.SwitchID = cZones[i].SwitchID;
                            Sec.Width_cm = cZones[i].Width_cm;
                            Sec.Save();
                        }
                    }
                }
            }
        }

        public void Update(int Zone, int End, int SectionsPerZone)
        {
            bool Done = false;
            if (End <= mf.Sections.Count && End > -1 && Zone > -1 && Zone < 8)
            {
                if (cZones[Zone].Start > 0)
                {
                    cZones[Zone].End = End;
                    if (cZones[Zone].End < cZones[Zone].Start) cZones[Zone].End = cZones[Zone].Start;
                    for (int i = Zone + 1; i < 8; i++)
                    {
                        if (cZones[i - 1].End < mf.Sections.Count && !Done)
                        {
                            cZones[i].Start = cZones[i - 1].End + 1;
                            if (cZones[i].End < cZones[i].Start) cZones[i].End = cZones[i].Start + SectionsPerZone - 1;
                            if (cZones[i].End > mf.Sections.Count) cZones[i].End = mf.Sections.Count;
                        }
                        else
                        {
                            Done = true;
                            cZones[i].Start = 0;
                            cZones[i].End = 0;
                        }
                    }
                }
            }
        }

        private int ListID(int ID)
        {
            for (int i = 0; i < cZones.Count; i++)
            {
                if (cZones[i].ID == ID) return i;
            }
            return -1;
        }
    }
}