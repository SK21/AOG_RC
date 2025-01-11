using System;

namespace RateController
{
    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section, HydUp, HydDown, TramRight, TramLeft, GeoStop, Switch, None };

    public class clsRelay
    {
        private readonly int cID;       // 0-15
        private readonly FormStart mf;
        private byte cModuleID;         // 0-7
        private string cName;
        private bool cRelayOn = false;
        private int cSectionID;         // 0-127
        private int cSwitchID;          // 0-15
        private RelayTypes cType = RelayTypes.Section;

        public clsRelay(FormStart main, int RelayID, int ModuleID)
        {
            mf = main;
            cID = RelayID;
            cModuleID = (byte)ModuleID;
            cName = "_R" + cID.ToString() + "_M" + cModuleID.ToString();
            cSectionID = cModuleID * 16 + cID;
        }

        public int ID
        { get { return cID; } }

        public bool IsON
        {
            get { return cRelayOn; }
            set { cRelayOn = value; }
        }

        public int ModuleID
        {
            get { return cModuleID; }
        }

        public int SectionID
        {
            // for 'Section' type relays
            get { return cSectionID; }
            set
            {
                if (value >= -1 && value < mf.MaxSections)
                {
                    cSectionID = value;
                }
                else
                {
                    cSectionID = -1;    // no section
                }
            }
        }

        public int SwitchID
        {
            // for 'Switch' type relays
            get { return cSwitchID; }
            set
            {
                if (value >= -1 && value < mf.MaxSwitches)
                {
                    cSwitchID = value;
                }
                else
                {
                    cSwitchID = -1;    // no switch
                }
            }
        }

        public RelayTypes Type
        {
            get { return cType; }
            set { cType = value; }
        }

        public string TypeDescription
        {
            get { return mf.TypeDescriptions[(int)cType]; }
            set
            {
                var index = Array.IndexOf(mf.TypeDescriptions, value);
                if (index != -1) cType = (RelayTypes)index;
            }
        }

        public void Load()
        {
            if (Enum.TryParse(mf.Tls.LoadProperty("RelayType" + cName), true, out RelayTypes tmp)) cType = tmp;
            if (int.TryParse(mf.Tls.LoadProperty("RelaySection" + cName), out int T)) cSectionID = T;
            if (int.TryParse(mf.Tls.LoadProperty("RelaySwitch" + cName), out int sw)) cSwitchID = sw;
        }

        public void Save()
        {
            // Should only be called from clsRelays. Need to run sub
            // BuildPowerRelays on change.
            mf.Tls.SaveProperty("RelayType" + cName, cType.ToString());
            mf.Tls.SaveProperty("RelaySection" + cName, cSectionID.ToString());
            mf.Tls.SaveProperty("RelaySwitch" + cName, cSwitchID.ToString());
        }
    }
}