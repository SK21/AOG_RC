using System;
using System.Collections.Generic;

namespace RateController.Classes
{
    public class clsSensors
    {
        public IList<clsSensor> Items;
        private List<clsSensor> cSensors = new List<clsSensor>();
        private FormStart mf;
        private byte[] SensorCounts = new byte[Props.MaxModules];

        public clsSensors(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSensors.AsReadOnly();
            LoadCounts();
            if (SensorCounts[0] == 0) SensorCounts[0] = 1;  // default to at least one sensor
            Load();
        }

        public byte Count(byte ModuleID)
        {
            byte Result = 0;
            if (ModuleID < Props.MaxModules) Result = SensorCounts[ModuleID];
            return Result;
        }

        public clsSensor Item(byte ModuleID, byte SensorID)
        {
            int ID = ListID(ModuleID, SensorID);
            if (ID == -1) throw new ArgumentException("Invalid sensor number. (clsSensors)");
            return cSensors[ID];
        }

        public void Load()
        {
            cSensors.Clear();
            for (int j = 0; j < Props.MaxModules; j++)
            {
                for (int i = 0; i < SensorCounts[j]; i++)
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

        public void SetModuleSensorCount(byte ModuleID, byte Count)
        {
            if (Count > Props.MaxSensorsPerModule || ModuleID>Props.MaxModules)
            {
                throw new ArgumentException("Invalid sensor count.");
            }
            else
            {
                SensorCounts[ModuleID] = Count;
                SaveCounts();
                Load();
            }
        }

        private int ListID(byte ModuleID, byte SensorID)
        {
            for (int i = 0; i < cSensors.Count; i++)
            {
                if (cSensors[i].ID == SensorID && cSensors[i].ModuleID == ModuleID) return i;
            }
            return -1;
        }

        private void LoadCounts()
        {
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (byte.TryParse(Props.GetProp("ModuleSensorCount" + i.ToString()), out byte c))
                {
                    SensorCounts[i] = c;
                }
                else
                {
                    SensorCounts[i] = 0;
                }
            }
        }

        private void SaveCounts()
        {
            for (int i = 0; i < Props.MaxModules; i++)
            {
                Props.SetProp("ModuleSensorCount" + i.ToString(), SensorCounts[i].ToString());
            }
        }
    }
}