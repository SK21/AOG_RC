using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN8
    {
        // Control settings
        // 0	High adjust
        // 1	Low adjust
        // 2	Threshold
        // 3	Minimum
        // 4	Maximum
        // 5	Scale Factor

        private readonly clsProduct Prod;

        public PGN8(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public void Send()
        {
            byte[] Data = new byte[6];
            Data[0] = (byte)Prod.HighAdjust;
            Data[1] = (byte)Prod.LowAdjust;
            Data[2] = (byte)Prod.Threshold;
            Data[3] = (byte)Prod.MinAdjust;
            Data[4] = (byte)Prod.MaxAdjust;
            Data[5] = (byte)Prod.ScalingFactor;

            Prod.mf.CanBus1.SendCanMessage(8, (byte)Prod.ModuleID, Prod.SensorID, Data);
        }
    }
}