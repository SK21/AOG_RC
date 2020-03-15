using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController
{
    class CAveraging
    {
        private int MaxDataPoints = 0;
        private int ID = 0;
        private int Count = 0;
        private double Total = 0;
        private double LastAve = 0;
        private bool NewData = false;
        private double[] Data;
        public CAveraging(int MaxPoints = 10)
        {
            if (MaxPoints < 0 | MaxPoints > 100) MaxPoints = 10;
            Data = new double[MaxPoints];
            MaxDataPoints = MaxPoints;
        }
        public void AddDataPoint(double NewPoint)
        {
            Count++;
            Data[ID] = NewPoint;
            ID++;
            if (ID > MaxDataPoints - 1) ID = 0;
            if (Count > MaxDataPoints) Count = MaxDataPoints;
            NewData = true;
        }

        public double Average()
        {
            if (NewData)
            {
                NewData = false;
                Total = 0;
                for (int i = 0; i < Count; i++)
                {
                    Total += Data[i];
                }
                if (Count == 0)
                {
                    LastAve = 0;
                    return 0;
                }
                else
                {
                    LastAve = Total / Count;
                    return LastAve;
                }
            }
            else
            {
                return LastAve;
            }
        }
    }
}
