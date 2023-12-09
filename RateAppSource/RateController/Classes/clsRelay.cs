using System;

namespace RateController
{
    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section,HydUp,HydDown,TramRight,TramLeft,GeoStop,None };

    public class clsRelay
    {
        private readonly int cID;
        private readonly FormStart mf;
        private byte cModuleID;
        private string cName;
        private bool cRelayOn = false;
        private int cSectionID;
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
            get { return cSectionID; }
            set
            {
                if (value >= -1 && value < mf.MaxSections)
                {
                    cSectionID = value;
                }
                else
                {
                    cSectionID = -1;    // no section value
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
        }

        public void Save()
        {
            // Should only be called from clsRelays. Need to run sub
            // BuildPowerRelays on change.
            mf.Tls.SaveProperty("RelayType" + cName, cType.ToString());
            mf.Tls.SaveProperty("RelaySection" + cName, cSectionID.ToString());
        }
    }
}