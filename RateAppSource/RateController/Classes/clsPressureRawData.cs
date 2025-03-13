namespace RateController.Classes
{
    public class clsPressureRawData
    {
        public int ID { get; set; }
        public int ModuleID { get; set; }
        public double Pressure { get; set; }
        public int RawData { get; set; }

        public bool IsValid()
        {
            return RawData > 0 && RawData < 5000 &&
                   ModuleID >= 0 && ModuleID < 8 &&
                   Pressure >= 0 && Pressure < 1000;
        }
    }
}