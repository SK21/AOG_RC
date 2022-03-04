using System;
using System.Collections.Generic;

namespace RateController
{
    public class clsPressures
    {
        public IList<clsPressure> Items;
        private byte cOffAverageSetting;
        private List<clsPressure> cPressures = new List<clsPressure>();
        private bool cUseOffAverageAlarm;
        private FormStart mf;

        public clsPressures(FormStart CallingFrom)
        {
            mf = CallingFrom;
            Items = cPressures.AsReadOnly();
            LoadData();
        }
        public byte OffPressureSetting
        {
            get { return cOffAverageSetting; }
            set
            {
                if (value >= 0 && value <= 40)
                {
                    cOffAverageSetting = value;
                    SaveData();
                }
                else
                {
                    throw new ArgumentException("Invalid Off-Average setting.");
                }
            }
        }

        public bool UseAlarm 
        {
            get { return cUseOffAverageAlarm; }
            set
            {
                cUseOffAverageAlarm = value;
                SaveData();
            } 
        }

        public bool AlarmOn()
        {
            bool Result = false;
            int Count = 0;
            float Total = 0;
            float Ave = 0;

            if (UseAlarm)
            {
                // get total
                for (int i = 0; i < cPressures.Count; i++)
                {
                    if (cPressures[i].UnitsVolts > 0)
                    {
                        if (mf.Sections.Item(cPressures[i].SectionID).SectionOn)
                        {
                            Count++;
                            Total += cPressures[i].Pressure();
                        }
                    }
                }
                if (Count > 0) { Ave = Total / Count; }

                // check average
                for (int i = 0; i < cPressures.Count; i++)
                {
                    if (cPressures[i].UnitsVolts > 0)
                    {
                        if (mf.Sections.Item(cPressures[i].SectionID).SectionOn)
                        {
                            // too low?
                            if (cPressures[i].Pressure() < (Ave * cOffAverageSetting / 100))
                            {
                                Result = true;
                                break;
                            }

                            // too high?
                            if (cPressures[i].Pressure() > (Ave * (1 + cOffAverageSetting / 100)))
                            {
                                Result = true;
                                break;
                            }
                        }
                    }
                }
            }
            return Result;
        }

        public clsPressure Item(int PressureID)
        {
            int IDX = ListId(PressureID);
            if (IDX == -1) throw new IndexOutOfRangeException();
            return cPressures[IDX];
        }

        public void Load()
        {
            cPressures.Clear();
            for (int i = 0; i < 16; i++)
            {
                clsPressure Pres = new clsPressure(mf, i);
                Pres.Load();
                cPressures.Add(Pres);
            }
            LoadData();
        }

        public void Save()
        {
            for (int i = 0; i < 16; i++)
            {
                cPressures[i].Save();
            }
        }

        private int ListId(int PressureID)
        {
            for (int i = 0; i < 16; i++)
            {
                if (cPressures[i].ID == PressureID) return i;
            }
            return -1;
        }

        private void LoadData()
        {
            bool.TryParse(mf.Tls.LoadProperty("OffPressureAlarm"), out cUseOffAverageAlarm);
            byte.TryParse(mf.Tls.LoadProperty("OffPressureSetting"), out cOffAverageSetting);
        }

        private void SaveData()
        {
            mf.Tls.SaveProperty("OffPressureAlarm", cUseOffAverageAlarm.ToString());
            mf.Tls.SaveProperty("OffPressureSetting", cOffAverageSetting.ToString());
        }
    }
}