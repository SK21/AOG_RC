using RateController.Classes;
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
                for (int i = 0; i < Props.MaxSections; i++)
                {
                    if (cSections[i].Enabled) tmp++;
                }
                return tmp;
            }
            set
            {
                if (value < 0 || value > Props.MaxSections)
                {
                    throw new ArgumentException("Invalid section number. (clsSections)");
                }
                else
                {
                    for (int i = 0; i < Props.MaxSections; i++)
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
            for (int i = 0; i < Props.MaxSections; i++)
            {
                if (cSections[i].SwitchChanged) Changed = true;
                cSections[i].SwitchChanged = false;
            }
            if (Changed)
            {
                if (Props.UseZones)
                {
                    mf.SectionControl.UpdateSectionStatusWithZones();
                }
                else
                {
                    mf.SectionControl.UpdateSectionStatusNoZones();
                }
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
            for (int i = 0; i < Props.MaxSections; i++)
            {
                clsSection Sec = new clsSection(mf, i);
                Sec.Load();
                cSections.Add(Sec);
            }
        }


        public void Save()
        {
            for (int i = 0; i < cSections.Count; i++)
            {
                cSections[i].Save();
            }
        }

        public float TotalWidth(bool ReturnFeet = false)
        {
            float Result = 0;

            if (cSections.Count >= Props.MaxSections)
            {
                float cWorkingWidth_cm = 0;

                for (int i = 0; i < Props.MaxSections; i++)
                {
                    if (cSections[i].Enabled)
                    {
                        cWorkingWidth_cm += Item(i).Width_cm;
                    }
                }

                if (ReturnFeet)
                {
                    Result = (float)(cWorkingWidth_cm * 0.032808);   // feet
                }
                else
                {
                    Result = (float)(cWorkingWidth_cm * 0.01);    // meters
                }
            }

            return Result;
        }

        public float WorkingWidth(bool ReturnFeet = false)
        {
            float Result = 0;
            float cWorkingWidth_cm = 0;

            for (int i = 0; i < Props.MaxSections; i++)
            {
                if (mf.Sections.Item(i).IsON)
                {
                    cWorkingWidth_cm += Item(i).Width_cm;
                }
            }

            if (ReturnFeet)
            {
                Result = (float)(cWorkingWidth_cm * 0.032808);   // feet
            }
            else
            {
                Result = (float)(cWorkingWidth_cm * 0.01);    // meters
            }
            return Result;
        }

        private void AOG_SectionsChanged(object sender, EventArgs e)
        {
            double Wdth;
            string Units;
            if (!Props.UseMetric)
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
                if (!Props.UseMetric)
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

            var Hlp = new frmMsgBox( Message, "Section Changes");
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
                Props.ShowMessage("Sections changed. Check switch definitions.", "Sections", 5000);
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