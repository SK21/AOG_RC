﻿using RateController.Classes;
using System;

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
                if (value >= 0 && value < Props.MaxSwitches)
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
                    throw new ArgumentException("Must be between 0 and " + (Props.MaxSwitches - 1).ToString());
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
            bool.TryParse(Props.GetProp(Name + "_enabled"), out cEnabled);
            float.TryParse(Props.GetProp(Name + "_width"), out cWidth);
            int.TryParse(Props.GetProp(Name + "_SwitchID"), out cSwitchID);
        }

        public void Save()
        {
            if (cEdited)
            {
                Props.SetProp(Name + "_enabled", cEnabled.ToString());
                Props.SetProp(Name + "_width", cWidth.ToString());
                Props.SetProp(Name + "_SwitchID", cSwitchID.ToString());
                cEdited = false;
            }
        }
    }
}