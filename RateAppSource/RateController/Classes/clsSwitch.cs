namespace RateController
{
    public class clsSwitch
    {
        private readonly int cID;
        private readonly FormStart mf;
        private string cDescription;
        private byte cModuleID;     // 0-7
        private string cName;
        private byte cRelayID;      // 1-16

        public clsSwitch(FormStart main, int ID)
        {
            cID = ID;
            mf = main;
            cName = "_SW_" + cID.ToString();
            cModuleID = 0;
            cRelayID = 0;
            cDescription = (ID + 1).ToString();
        }

        public string Description
        {
            get { return cDescription; }
            set
            {
                if (value.Length > 0 && value.Length < 6) cDescription = value;
                if (value.Length > 0)
                {
                    if (value.Length > 5)
                    {
                        cDescription = value.Substring(0, 5);
                    }
                    else
                    {
                        cDescription = value;
                    }
                }
            }
        }

        public int ID
        { get { return cID; } }

        public byte ModuleID
        {
            get { return cModuleID; }
            set
            {
                if (value < mf.MaxModules) cModuleID = value;
            }
        }

        public byte RelayID
        {
            get { return cRelayID; }
            set
            {
                if (value < mf.MaxRelays + 1) cRelayID = value;
            }
        }

        public void Load()
        {
            if (byte.TryParse(mf.Tls.LoadProperty("Module" + cName), out byte md)) cModuleID = md;
            if (byte.TryParse(mf.Tls.LoadProperty("Relay" + cName), out byte rly)) cRelayID = rly;
            string Des = mf.Tls.LoadProperty("Description" + cName);
            if (Des != null) cDescription = Des;
        }

        public void Save()
        {
            mf.Tls.SaveProperty("Module" + cName, cModuleID.ToString());
            mf.Tls.SaveProperty("Relay" + cName, cRelayID.ToString());
            mf.Tls.SaveProperty("Description" + cName, cDescription);
        }
    }
}