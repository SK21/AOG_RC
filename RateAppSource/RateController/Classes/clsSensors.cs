using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;

namespace RateController.Classes
{
    public class clsSensors
    {
        public IList<clsSensor> Items;
        private int cMaxRecID = 0;
        private List<clsSensor> cSensors = new List<clsSensor>();
        private FormStart mf;

        public clsSensors(FormStart CallingForm)
        {
            mf = CallingForm;
            Items = cSensors.AsReadOnly();
            LoadMaxRec();
            Load();
        }

        public clsSensor AddSensor(int ModuleID, int SensorID)
        {
            clsSensor Result = null;
            if (!Exists(ModuleID, SensorID)
                && ModuleID >= 0 && ModuleID < Props.MaxModules
                && SensorID >= 0 && SensorID < Props.MaxSensorsPerModule)
            {
                cMaxRecID++;
                clsSensor sensor = new clsSensor(cMaxRecID);
                sensor.SetModuleSensor(ModuleID, SensorID, this);
                sensor.Save();
                cSensors.Add(sensor);
                Result = sensor;
                SaveMaxRec();
            }
            return Result;
        }

        public bool DeleteSensor(int ModuleID, int SensorID)
        {
            bool Result = false;
            clsSensor Sen = cSensors.FirstOrDefault(s => s.ModuleID == ModuleID && s.SensorID == SensorID);
            if (Sen != null)
            {
                cSensors.Remove(Sen);

                // set module ID and sensor ID to -1 to show it has been deleted
                Sen.SetModuleSensor(-1, -1, this);
                Sen.Save();
                Result = true;
            }
            return Result;
        }

        public bool EditSensorIDs(clsSensor Sensor, int ModuleID, int SensorID)
        {
            bool Result = false;
            if (!Exists(ModuleID, SensorID)
                && ModuleID >= 0 && ModuleID < Props.MaxModules
                && SensorID >= 0 && SensorID < Props.MaxSensorsPerModule)
            {
                Sensor.SetModuleSensor(ModuleID, SensorID, this);
                Sensor.Save();
                Result = true;
            }
            return Result;
        }

        public clsSensor Item(int ModuleID, int SensorID)
        {
            var sensor = cSensors.FirstOrDefault(s => s.ModuleID == ModuleID && s.SensorID == SensorID);
            return sensor;
        }

        public void Load()
        {
            for (int i = 0; i <= cMaxRecID; i++)
            {
                clsSensor Sen = new clsSensor( i);
                if (Sen.Load()) cSensors.Add(Sen);
            }
        }

        public void Save()
        {
            for (int i = 0; i < cSensors.Count; i++)
            {
                cSensors[i].Save();
            }
        }

        private bool Exists(int moduleId, int sensorId, clsSensor except = null)
        {
            return cSensors.Any(s => s != except
                                    && s.ModuleID == moduleId
                                    && s.SensorID == sensorId);
        }

        private void LoadMaxRec()
        {
            if (int.TryParse(Props.GetProp("RateSensorMaxID"), out int mx)) cMaxRecID = mx;
        }

        private void SaveMaxRec()
        {
            Props.SetProp("RateSensorMaxID", cMaxRecID.ToString());
        }
    }
}