using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsSections
    {
        // to use ForEach
        public IList<clsSection> Items;

        private List<clsSection> cSections = new List<clsSection>();
        private FormStart mf;
        private byte OnHiLast;
        private byte OnLoLast;
        private byte Rlys0;
        private byte Rlys1;
        private int Tmp;

        public clsSections(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSections.AsReadOnly();
            mf.SectionsPGN.SectionsChanged += AOG_SectionsChanged;
        }

        public int Count
        {
            get
            {
                int tmp = 0;
                for (int i = 0; i < mf.MaxSections; i++)
                {
                    if (cSections[i].Enabled) tmp++;
                }
                return tmp;
            }
            set
            {
                if (value < 0 | value > mf.MaxSections)
                {
                    throw new ArgumentException("Invalid section number. (clsSections)");
                }
                else
                {
                    for (int i = 0; i < mf.MaxSections; i++)
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
            for (int i = 0; i < mf.MaxSections; i++)
            {
                if (cSections[i].SwitchChanged) Changed = true;
                cSections[i].SwitchChanged = false;
            }
            if (Changed)
            {
                mf.SectionControl.UpdateSectionStatus();
            }
        }

        public clsSection Item(int SectionID)
        {
            int IDX = ListID(SectionID);
            if (IDX == -1) throw new ArgumentException("Invalid section number. (clsSections)");
            return cSections[IDX];
        }

        public void Load()
        {
            cSections.Clear();
            for (int i = 0; i < mf.MaxSections; i++)
            {
                clsSection Sec = new clsSection(mf, i);
                Sec.Load();
                cSections.Add(Sec);
            }
            UpdateSectionsOn();
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

        public float TotalWidth(bool UseInches)
        {
            float Result = 0;
            float cWorkingWidth_cm = 0;

            for (int i = 0; i < mf.MaxSections; i++)
            {
                if (cSections[i].Enabled)
                {
                    cWorkingWidth_cm += Item(i).Width_cm;
                }
            }

            if (UseInches)
            {
                Result = (float)((cWorkingWidth_cm / 100.0) * 3.28);   // feet
            }
            else
            {
                Result = (float)(cWorkingWidth_cm / 100.0);    // meters
            }
            return Result;
        }

        public bool UpdateSectionsOn(byte OnLo = 0, byte OnHi = 0, bool UseCurrent = true)
        {
            // returns true if section on status has changed for any section
            bool Result = false;
            bool tmp = false;

            if (UseCurrent)
            {
                OnLo = Rlys0;
                OnHi = Rlys1;
            }

            if (OnLoLast != OnLo || OnHiLast != OnHi || !mf.SectionControl.MasterOn())
            {
                OnLoLast = OnLo;
                OnHiLast = OnHi;

                Rlys0 = OnLo;
                Rlys1 = OnHi;

                try
                {
                    for (int i = 0; i < mf.MaxSections; i++)
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

        public float WorkingWidth(bool UseInches)
        {
            float Result = 0;
            float cWorkingWidth_cm = 0;

            for (int i = 0; i < mf.MaxSections; i++)
            {
                if (mf.Sections.Item(i).IsON)
                {
                    cWorkingWidth_cm += Item(i).Width_cm;
                }
            }

            if (UseInches)
            {
                Result = (float)((cWorkingWidth_cm / 100.0) * 3.28);   // feet
            }
            else
            {
                Result = (float)(cWorkingWidth_cm / 100.0);    // meters
            }
            return Result;
        }

        private void AOG_SectionsChanged(object sender, EventArgs e)
        {
            double Wdth;
            string Units;
            if (mf.UseInches)
            {
                Units = "Inches";
            }
            else
            {
                Units = "cm";
            }

            string Message = "Section width changes from AOG, " + mf.SectionsPGN.SectionCount().ToString() + " section(s).";
            Message += "\nUnits are in " + Units + ".";

            for (int i = 0; i < mf.SectionsPGN.SectionCount(); i++)
            {
                if (mf.UseInches)
                {
                    Wdth = Math.Round((double)(mf.SectionsPGN.Width_cm(i)) * 0.393701);
                }
                else
                {
                    Wdth = mf.SectionsPGN.Width_cm(i);
                }
                Message += "\n" + "Section " + (i + 1).ToString() + "    " + Wdth.ToString();
            }
            Message += "\n\n Accept the changes?";

            var Hlp = new frmMsgBox(mf, Message, "Section Changes");
            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                // change sections
                Count = mf.SectionsPGN.SectionCount();
                for (int i = 0; i < mf.SectionsPGN.SectionCount(); i++)
                {
                    Items[i].Width_cm = (float)mf.SectionsPGN.Width_cm(i);
                }
                mf.Tls.ShowHelp("Sections changed. Check switch definitions.", "Sections", 5000);
            }
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