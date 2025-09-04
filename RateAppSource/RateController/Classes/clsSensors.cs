using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace RateController.Classes
{
    public class clsSensors
    {
        public IList<clsSensor> Items;
        private List<clsSensor> cSensors = new List<clsSensor>();
        private FormStart mf;

        public clsSensors(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSensors.AsReadOnly();
        }

        public int Count
        {
            get
            {
                int tmp = 0;
                for (int i = 0; i < Props.MaxSensorsPerModule; i++)
                {
                    if (cSensors[i].Enabled) tmp++;
                }
                return tmp;
            }
            set
            {
                if (value < 0 || value > Props.MaxSensorsPerModule)
                {
                    throw new ArgumentException("Invalid sensor number. (clsSensors)");
                }
                else
                {
                    for (int i = 0; i < Props.MaxSensorsPerModule; i++)
                    {
                        int Tmp = ListID(i);
                        if (i < value)
                        {
                            cSensors[Tmp].Enabled = true;
                        }
                        else
                        {
                            cSensors[Tmp].Enabled = false;
                        }
                        cSensors[Tmp].Save();
                    }
                }
            }
        }

        public clsSensor Item(int SensorID)
        {
            int ID = ListID(SensorID);
            if (ID == -1) throw new ArgumentException("Invalid sensor number. (clsSensors)");
            return cSensors[ID];
        }

        public void Load()
        {
            cSensors.Clear();
            for (int j = 0; j < Props.MaxModules; j++)
            {
                for (int i = 0; i < Props.MaxSensorsPerModule; i++)
                {
                    clsSensor Sen = new clsSensor(mf, i, j);
                    Sen.Load();
                    cSensors.Add(Sen);
                }
            }
        }

        public void Save()
        {
            for (int i = 0; i < cSensors.Count; i++)
            {
                cSensors[i].Save();
            }
        }

        private int ListID(int SectionID)
        {
            for (int i = 0; i < cSensors.Count; i++)
            {
                if (cSensors[i].ID == SectionID) return i;
            }
            return -1;
        }
    }
}