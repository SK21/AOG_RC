using RateController.Classes;
using System;

namespace RateController
{
    public class clsZone
    {
        private bool cEdited;
        private int cID;
        private int cSwitchID = 0;
        private float cWidth = 0;   // cm
        private FormStart mf;
        private string Name;
        private int SecEnd = 0; // one based
        private int SecStart = 0;

        public clsZone(FormStart CallingFrom, int ID)
        {
            mf = CallingFrom;
            cID = ID;
            Name = "Zone" + ID.ToString();
        }

        public bool Enabled
        {
            get { return (SecEnd > 0 && SecStart > 0 && cWidth > 0); }
        }

        public int End
        {
            get { return SecEnd; }
            set
            {
                if (SecEnd != value)
                {
                    SecEnd = value;
                    cEdited = true;
                }
            }
        }

        public int ID
        { get { return cID; } }

        public int Start
        {
            get { return SecStart; }
            set
            {
                if (SecStart != value)
                {
                    SecStart = value;
                    cEdited = true;
                }
            }
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

        public bool IsValid()
        {
            bool Result = false;
            if (Start > -1 && Start <= mf.Sections.Count
                && End >= Start && End <= mf.Sections.Count)
            {
                Result = true;
            }
            return Result;
        }

        public void Load()
        {
            float.TryParse(Props.GetProp(Name + "_width"), out cWidth);
            int.TryParse(Props.GetProp(Name + "_SwitchID"), out cSwitchID);
            int.TryParse(Props.GetProp(Name + "_Start"), out SecStart);
            int.TryParse(Props.GetProp(Name + "_End"), out SecEnd);
        }

        public void Save()
        {
            if (cEdited)
            {
                if (IsValid())
                {
                    Props.SetProp(Name + "_width", cWidth.ToString());
                    Props.SetProp(Name + "_SwitchID", cSwitchID.ToString());
                    Props.SetProp(Name + "_Start", SecStart.ToString());
                    Props.SetProp(Name + "_End", SecEnd.ToString());
                    cEdited = false;
                }
                else
                {
                    throw new ArgumentException("clsZone/Invalid sections.");
                }
            }
        }
    }
}