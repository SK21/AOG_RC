using System;

namespace RateController
{
    public enum RelayTypes
    { Section, Slave, Master, Power, Invert_Section };

    public class clsRelay
    {
        private string cDescription;
        private readonly int cID;
        private bool cRelayOn = false;
        private int cSectionID;
        private RelayTypes cType = RelayTypes.Section;
        private readonly FormStart mf;

        public clsRelay(FormStart CallingFrom, int ID)
        {
            mf = CallingFrom;
            // to do, check for duplicate ID
            cID = ID;
            cDescription = "Relay " + ID.ToString();
            cSectionID = ID;    // default to a matching section ID
        }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if (value.Length > 25)
                {
                    cDescription = value.Substring(0, 25);
                }
                else
                {
                    cDescription = value;
                }
            }
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

        public int ID
        { get { return cID; } }

        public bool IsON
        {
            get { return cRelayOn; }
            set { cRelayOn = value; }
        }

        public int SectionID
        {
            get { return cSectionID; }
            set
            {
                if (value >= 0 && value < mf.MaxSections)
                {
                    cSectionID = value;
                }
            }
        }

        public RelayTypes Type
        {
            get { return cType; }
            set { cType = value; }
        }

        public void Load()
        {
            RelayTypes tmp;
            int T;
            if (Enum.TryParse(mf.Tls.LoadProperty("RelayType" + ID.ToString()), true, out tmp)) cType = tmp;
            if (int.TryParse(mf.Tls.LoadProperty("RelaySection" + ID.ToString()), out T)) cSectionID = T;
            cDescription = mf.Tls.LoadProperty("RelayDescription" + ID.ToString());
        }

        public void Save()
        {
            // Should only be called from clsRelays. Need to run sub
            // BuildPowerRelays on change.
            mf.Tls.SaveProperty("RelayType" + ID.ToString(), cType.ToString());
            mf.Tls.SaveProperty("RelaySection" + ID.ToString(), cSectionID.ToString());
            mf.Tls.SaveProperty("RelayDescription" + ID.ToString(), cDescription);
        }
    }
}