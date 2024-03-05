using System;
using System.Diagnostics;

namespace RateController
{
    public class clsSection
    {
        private bool cEdited;
        private bool cEnabled = false;
        private int cID = 0;
        private bool cSectionOn = false;
        private bool cSwitchChanged = false;
        private int cSwitchID = 0;
        private float cWidth = 0;   // cm

        private FormStart mf;
        private string Name;

        public clsSection(FormStart CallingFrom, int ID)
        {
            mf = CallingFrom;
            cID = ID;
            Name = "Sec" + ID.ToString();
        }

        public bool Edited
        { get { return cEdited; } }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                if (cEnabled != value)
                {
                    cEnabled = value;
                    cEdited = true;
                }
            }
        }

        public int ID
        { get { return cID; } }

        public bool IsON
        {
            get
            {
                bool Result = false;
                if (cEnabled) Result = cSectionOn;
                return Result;
            }
            set
            {
                if (cSectionOn != value)
                {
                    cSectionOn = value;
                    cEdited = true;
                }
            }
        }

        public bool SwitchChanged
        {
            get { return cSwitchChanged; }
            set { cSwitchChanged = value; }
        }

        public int SwitchID
        {
            get { return cSwitchID; }
            set
            {
                if (value >= 0 && value < mf.MaxSwitches)
                {
                    if (cSwitchID != value)
                    {
                        cSwitchID = value;
                        cEdited = true;
                        cSwitchChanged = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and " + (mf.MaxSwitches - 1).ToString());
                }
            }
        }

        public float Width_cm
        {
            get { return cWidth; }
            set
            {
                if (value >= 0 && value < 10000)
                {
                    if (cWidth != value)
                    {
                        cWidth = value;
                        cEdited = true;
                    }
                }
                else
                {
                    throw new ArgumentException("Must be between 0 and 10000");
                }
            }
        }

        public float Width_inches
        {
            get { return (float)(cWidth / 2.54); }
            set
            {
                Width_cm = (float)(value * 2.54);
            }
        }

        public void Load()
        {
            bool.TryParse(mf.Tls.LoadProperty(Name + "_enabled"), out cEnabled);
            float.TryParse(mf.Tls.LoadProperty(Name + "_width"), out cWidth);
            int.TryParse(mf.Tls.LoadProperty(Name + "_SwitchID"), out cSwitchID);
        }

        public void Save()
        {
            if (cEdited)
            {
                mf.Tls.SaveProperty(Name + "_enabled", cEnabled.ToString());
                mf.Tls.SaveProperty(Name + "_width", cWidth.ToString());
                mf.Tls.SaveProperty(Name + "_SwitchID", cSwitchID.ToString());
                cEdited = false;
            }
        }
    }
}