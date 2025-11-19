using RateController.Classes;

namespace RateController
{
    public class PGN238
    {
        // Machine config
        // 0    header Hi       128
        // 1    header Lo       129
        // 2    source          126
        // 3    AGIO PGN        238
        // 4    length          8
        // 5    raiseTime
        // 6    lowerTime
        // 7    hydEnable   - not used
        // 8    set0
        //      - bit 0     0 active high, 1 active low
        //      - bit 1     0 hyd disabled, 1 hyd enabled
        // 9    User1
        // 10   User2
        // 11   User3
        // 12   User4
        // 13   CRC

        private readonly FormStart mf;
        private bool cHydEnable;
        private byte cLowerTime;
        private byte cRaiseTime;
        private byte cSet0;
        private int totalHeaderByteCount = 5;

        public PGN238(FormStart CalledFrom)
        {
            mf = CalledFrom;
            Load();
        }

        public bool ActiveHigh
        { get { return !mf.Tls.BitRead(cSet0, 0); } }

        public bool HydEnable
        { get { return mf.Tls.BitRead(cSet0, 1); } }

        public byte LowerTime
        { get { return cLowerTime; } }

        public byte RaiseTime
        { get { return cRaiseTime; } }

        public void ParseByteData(byte[] Data)
        {
            if (Data.Length > totalHeaderByteCount)
            {
                if (Data.Length == Data[4] + totalHeaderByteCount + 1)
                {
                    if (mf.Tls.GoodCRC(Data, 2))
                    {
                        cRaiseTime = Data[5];
                        cLowerTime = Data[6];
                        cHydEnable = (Data[7] > 0);
                        cSet0 = Data[8];
                        Save();
                    }
                }
            }
        }

        private void Load()
        {
            byte.TryParse(Props.GetProp("RaiseTime"), out cRaiseTime);
            byte.TryParse(Props.GetProp("LowerTime"), out cLowerTime);
            bool.TryParse(Props.GetProp("HydEnable"), out cHydEnable);
            byte.TryParse(Props.GetProp("HydSettings"), out cSet0);
        }

        private void Save()
        {
            Props.SetProp("RaiseTime", cRaiseTime.ToString());
            Props.SetProp("LowerTime", cLowerTime.ToString());
            Props.SetProp("HydEnable", cHydEnable.ToString());
            Props.SetProp("HydSettings", cSet0.ToString());
        }
    }
}