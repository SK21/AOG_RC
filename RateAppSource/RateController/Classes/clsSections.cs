using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsSections
    {
        // to use ForEach
        public IList<clsSection> Items;

        private double cHectaresPerMinute;
        private List<clsSection> cSections = new List<clsSection>();
        private int Tmp;

        public clsSections()
        {
            Items = cSections.AsReadOnly();
            Core.SectionsPGN.SectionsChanged += AOG_SectionsChanged;
            Core.UpdateStatus += Core_UpdateStatus;
            Core.AppExit += Core_AppExit;
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

        public double HectaresPerMinute
        {
            get { return cHectaresPerMinute; }
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
                    Core.SectionControl.UpdateSectionStatusWithZones();
                }
                else
                {
                    Core.SectionControl.UpdateSectionStatusNoZones();
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
                clsSection Sec = new clsSection(i);
                Sec.Load();
                cSections.Add(Sec);
            }
        }

        public void Save()
        {
            foreach (var section in cSections.ToList()) // ToList for safe shutdown
            {
                section.Save();
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
            if (Core.SwitchBox.MasterOn)
            {
                for (int i = 0; i < Props.MaxSections; i++)
                {
                    if (Core.Sections.Item(i).IsON)
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

        public double WorkRatePerHour()
        {
            double Result = 0;
            if (WorkingWidth(true) > 0)
            {
                if (!Props.UseMetric)
                {
                    Result = cHectaresPerMinute * 2.47105 * 60.0;
                }
                else
                {
                    Result = cHectaresPerMinute * 60.0;
                }
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

            string Message = "Section width changes from AOG, " + Core.SectionsPGN.SectionCount().ToString() + " section(s).";
            Message += "\nUnits are in " + Units + ".";

            for (int i = 0; i < Core.SectionsPGN.SectionCount(); i++)
            {
                if (!Props.UseMetric)
                {
                    Wdth = Math.Round((double)(Core.SectionsPGN.Width_cm(i)) * 0.393701);
                }
                else
                {
                    Wdth = Core.SectionsPGN.Width_cm(i);
                }
                Message += "\n" + "Section " + (i + 1).ToString() + "    " + Wdth.ToString();
            }
            Message += "\n\n Accept the changes?";

            var Hlp = new frmMsgBox(Message, "Section Changes");
            Hlp.ShowDialog();
            bool Result = Hlp.Result;
            Hlp.Close();
            if (Result)
            {
                // change sections
                Count = Core.SectionsPGN.SectionCount();
                for (int i = 0; i < Core.SectionsPGN.SectionCount(); i++)
                {
                    Items[i].Width_cm = (float)Core.SectionsPGN.Width_cm(i);
                }
                Props.ShowMessage("Sections changed. Check switch definitions.", "Sections", 5000);
            }
        }

        private void Core_AppExit(object sender, EventArgs e)
        {
            Core.SectionsPGN.SectionsChanged -= AOG_SectionsChanged;
            Core.UpdateStatus -= Core_UpdateStatus;
            Core.AppExit -= Core_AppExit;
        }

        private void Core_UpdateStatus(object sender, EventArgs e)
        {
            cHectaresPerMinute = WorkingWidth() * Props.Speed_KMH / 600.0;
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
