namespace RateController.Classes
{
    public class clsPressure
    {
        private readonly FormStart mf;
        private double cIntercept;
        private int cMinimumRateData;
        private int cModuleID;         // 0-7
        private string cName;
        private double cSlope;

        public clsPressure(FormStart main, int ModuleID)
        {
            mf = main;
            cModuleID = ModuleID;
            cName = "_M" + cModuleID.ToString();
        }

        public double Intercept
        { get { return cIntercept; } set { cIntercept = value; } }

        public int MinimumRawData
        { get { return cMinimumRateData; }set { cMinimumRateData = value; } }

        public int ModuleID
        { get { return cModuleID; } }

        public double Slope
        { get { return cSlope; } set { cSlope = value; } }

        public void Load()
        {
            if (double.TryParse(mf.Tls.LoadProperty("PressureSlope" + cName), out double sl)) cSlope = sl;
            if (double.TryParse(mf.Tls.LoadProperty("PressureIntercept" + cName), out double cpt)) cIntercept = cpt;
            if (int.TryParse(mf.Tls.LoadProperty("PressureMin" + cName), out int mn)) cMinimumRateData = mn;
        }

        public void Save()
        {
            mf.Tls.SaveProperty("PressureSlope" + cName, cSlope.ToString());
            mf.Tls.SaveProperty("PressureIntercept" + cName, cIntercept.ToString());
            mf.Tls.SaveProperty("PressureMin" + cName, cMinimumRateData.ToString());
        }
    }
}