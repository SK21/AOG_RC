using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsSections
    {
        public IList<clsSection> Items;
        private List<clsSection> cSections = new List<clsSection>();
        private float cWorkedArea_ha;

        private float cWorkingWidth_cm;
        private float HectaresPerSecond;
        private DateTime LastTime;

        private FormStart mf;
        private float SecondsWorked;
        private PGN32617 SwitchPGN;

        public clsSections(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSections.AsReadOnly(); // to use ForEach

            mf.AOG.SectionsChanged += AOG_SectionsChanged;
            SwitchPGN = new PGN32617(mf);
        }

        public int Count()
        {
            return cSections.Count;
        }

        public clsSection Item(int SectionID)
        {
            int IDX = ListID(SectionID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cSections[IDX];
        }

        public void Load()
        {
            cSections.Clear();
            for (int i = 0; i < 16; i++)
            {
                clsSection Sec = new clsSection(mf, i);
                Sec.Load();
                cSections.Add(Sec);
            }
        }

        public void ResetWorkedArea()
        {
            cWorkedArea_ha = 0;
        }

        public void Save()
        {
            for (int i = 0; i < 16; i++)
            {
                cSections[i].Save();
            }
        }

        public float TotalWidth(bool UseInches = true)
        {
            float Result = 0;
            for (int i = 0; i < 16; i++)
            {
                if (cSections[i].Enabled)
                {
                    if (UseInches)
                    {
                        Result += cSections[i].Width_inches;
                    }
                    else
                    {
                        Result += cSections[i].Width_cm;
                    }
                }
            }
            return Result;
        }

        public void UpdateSectionsOn(byte SectionsHi, byte SectionsLo)
        {
            cWorkingWidth_cm = 0;
            for (int i = 0; i < 16; i++)
            {
                if (i < 8)
                {
                    cSections[i].SectionOn = IsBitSet(SectionsLo, i);
                }
                else
                {
                    cSections[i].SectionOn = IsBitSet(SectionsHi, i - 8);
                }

                if (cSections[i].SectionOn) cWorkingWidth_cm += cSections[i].Width_cm;
            }
        }

        public void UpdateWorkedArea()
        {
            SecondsWorked = (float)(DateTime.Now - LastTime).TotalSeconds;
            LastTime = DateTime.Now;
            HectaresPerSecond = (float)(WorkingWidth_M() * mf.AOG.Speed() * 0.1 / 3600.0);
            cWorkedArea_ha += (HectaresPerSecond * SecondsWorked);
        }

        public float WorkedArea_ha()
        {
            return cWorkedArea_ha;
        }

        public float WorkingWidth_M()
        {
            return (float)(cWorkingWidth_cm / 100.0);
        }

        private void AOG_SectionsChanged(object sender, PGN32612.SectionsChangedEventArgs e)
        {
            UpdateSectionsOn(e.SectionHi, e.SectionLo);
        }

        private bool IsBitSet(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        private int ListID(int SectionID)
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                if (cSections[i].ID == SectionID) return i;
            }
            return -1;
        }

        public void SendSwitchesPGN(bool Always = false)
        {
            bool Changed = false;
            for (int i = 0; i < 16; i++)
            {
                if (cSections[i].SwitchChanged) Changed = true;
                cSections[i].SwitchChanged = false;
            }
            if (Changed || Always) SwitchPGN.Send();
        }
    }
}