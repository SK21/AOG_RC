using System;

namespace RateController
{
    public class clsPressure
    {
        private bool cEdited;
        private int cID = 0;

        private string cDescription;
        private int cModuleID;
        private int cSensorID;
        private int cSectionID;
        private float cUnitsVolts;

        private FormStart mf;
        private string Name;

        public clsPressure(FormStart CallingFrom, int ID)
        {
            mf = CallingFrom;
            cID = ID;
            Name = "Pressure" + ID.ToString();
        }

        public bool Edited { get { return cEdited; } }

        public int ID { get { return cID; } }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if (value.Length > 15)
                {
                    if (cDescription != value.Substring(0, 15))
                    {
                        cDescription = value.Substring(0, 15);
                        cEdited = true;
                    }
                }
                else
                {
                    if (cDescription != value)
                    {
                        cDescription = value;
                        cEdited = true;
                    }
                }
            }
        }

        public int ModuleID
        {
            get { return cModuleID; }
            set
            {
                if (value >= 0 && value < 256)
                {
                    if (cModuleID != value)
                    {
                        cModuleID = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Module must be between 0 and 255.");
                }
            }
        }

        public int SensorID
        {
            get { return cSensorID; }
            set
            {
                if (value >= 0 && value < 16)
                {
                    if (cSensorID != value)
                    {
                        cSensorID = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Sensor must be between 0 and 15.");
                }
            }
        }
        
        public int SectionID
        {
            get { return cSectionID; }
            set
            {
                if (value >= 0 && value < 16)
                {
                    if (cSectionID != value)
                    {
                        cSectionID = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Section must be between 1 and 16.");
                }
            }
        }

        public float UnitsVolts
        {
            get { return cUnitsVolts; }
            set
            {
                if (value >= 0 && value <= 1000)
                {
                    if (cUnitsVolts != value)
                    {
                        cUnitsVolts = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and 1000.");
                }
            }
        }

        public void Load()
        {
            cDescription = mf.Tls.LoadProperty(Name + "_Description");
            int.TryParse(mf.Tls.LoadProperty(Name + "_ModuleID"), out cModuleID);
            int.TryParse(mf.Tls.LoadProperty(Name + "_SensorID"), out cSensorID);
            int.TryParse(mf.Tls.LoadProperty(Name + "_SectionID"), out cSectionID);
            float.TryParse(mf.Tls.LoadProperty(Name + "_UnitsVolts"), out cUnitsVolts);
        }

        public void Save()
        {
            if (cEdited)
            {
                mf.Tls.SaveProperty(Name + "_Description", cDescription);
                mf.Tls.SaveProperty(Name + "_ModuleID", cModuleID.ToString());
                mf.Tls.SaveProperty(Name + "_SensorID", cSensorID.ToString());
                mf.Tls.SaveProperty(Name + "_SectionID", cSectionID.ToString());
                mf.Tls.SaveProperty(Name + "_UnitsVolts", cUnitsVolts.ToString());
                cEdited = false;
            }
        }

        public float Pressure()
        {
            return mf.PressureData.Pressure((byte)cModuleID, (byte)ID);
        }
    }
}