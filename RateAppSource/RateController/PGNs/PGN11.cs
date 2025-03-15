using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public class PGN11
    {
        // PGN 11, sensor info 2
        // 0    scale reading X 10, byte lo
        // 1    scale Mid
        // 2    scale Hi
        // 3    Status

        private readonly clsProduct Prod;
        private bool cSensorReceiving;
        private double cWeight;

        public PGN11(clsProduct CalledFrom)
        {
            Prod = CalledFrom;
        }

        public bool SensorReceiving
        { get { return cSensorReceiving; } }
        public double Weight
        { get { return cWeight; } }

        public void ParseData(byte[] Data)
        {
            cWeight = (Data[2] << 16 | Data[1] << 8 | Data[0]) / 10.0;
            cSensorReceiving = ((Data[3] & 0b00000001) == 0b00000001);
        }
    }
}