using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsSectionsNew
    {
        public IList<clsSection> Items; // to use ForEach
        private List<clsSection> cSections = new List<clsSection>();
        private float cWorkingWidth_cm;
        private FormStart mf;
        private byte Rlys0;
        private byte Rlys1;
        private int Tmp;

        byte OnLoLast;
        byte OnHiLast;

        public clsSectionsNew(FormStart CallingForm)
        {
            mf = CallingForm;
        }

        public int Count
        {
            get
            {
                int tmp = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (cSections[i].Enabled) tmp++;
                }
                return tmp;
            }

            set
            {
                if (value < 0 | value > 16)
                {
                    throw new IndexOutOfRangeException();
                }
                else
                {
                    for (int i = 0; i < 16; i++)
                    {
                        Tmp = ListID(i);
                        if (i < value)
                        {
                            cSections[Tmp].Enabled = true;
                        }
                        else
                        {
                            cSections[Tmp].Enabled = false;
                            cSections[Tmp].Width_inches = 0;
                        }
                        cSections[Tmp].Save();
                    }
                }
            }
        }

        public void CheckSwitchDefinitions()
        {
            bool Changed = false;
            for (int i = 0; i < 16; i++)
            {
                if (cSections[i].SwitchChanged) Changed = true;
                cSections[i].SwitchChanged = false;
            }
            if (Changed)
            {
                //SendStatusUpdate();
                mf.SwitchIDs.Send();
            }
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
            //UpdateSectionsOn();
        }

        public void Save()
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                cSections[i].Save();
            }
        }

        public byte SectionHi()
        {
            return Rlys1;
        }

        public byte SectionLo()
        {
            return Rlys0;
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

        public bool UpdateSectionsOn(byte OnLo, byte OnHi)
        {
            // returns true if section on status has changed for any section
            bool Result = false;
            bool tmp = false;

            if (OnLoLast != OnLo || OnHiLast != OnHi)
            {
                OnLoLast = OnLo;
                OnHiLast = OnHi;

                Rlys0 = OnLo;
                Rlys1 = OnHi;

                try
                {
                    cWorkingWidth_cm = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        tmp = cSections[i].IsON;
                        if (i < 8)
                        {
                            cSections[i].IsON = mf.Tls.BitRead(Rlys0, i);
                        }
                        else
                        {
                            cSections[i].IsON = mf.Tls.BitRead(Rlys1, i - 8);
                        }
                        if (cSections[i].IsON != tmp) Result = true;
                        if (cSections[i].IsON) cWorkingWidth_cm += cSections
                        [i].Width_cm;
                    }
                }
                catch (Exception ex)
                {
                    mf.Tls.WriteErrorLog("clsSections/UpdateSectionsOn: " +
                    ex.Message);
                    mf.Tls.ShowHelp(ex.Message, "Sections", 3000, true);
                }
            }
            return Result;
        }

        private int ListID(int SectionID)
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                if (cSections[i].ID == SectionID) return i;
            }
            return -1;
        }
    }
}