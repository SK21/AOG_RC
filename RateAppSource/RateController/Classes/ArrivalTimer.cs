using System.Collections.Generic;
using System.Diagnostics;

namespace RateController.Classes
{
    // The average gap between a module's reports over the last WindowMs (the PGN Time on the Help
    // page). One gap alone jumps with the main window's load, since reports are handled on its
    // thread; the average doesn't. Copies of a report arriving by a second PC adapter are dropped
    // in UDPComm. A board on two links (an ESP32 on Ethernet and WiFi) sends each report from two
    // addresses within a few ms: an arrival under MinGapMs after the last counted one is that
    // second copy and isn't counted (boards send every 200 ms).
    public class ArrivalTimer
    {
        private const double MinGapMs = 50;
        private const double WindowMs = 2000;
        private readonly object cLock = new object();
        private readonly Queue<long> cTimes = new Queue<long>();
        private long cLast;

        public double AverageMs
        {
            get
            {
                double Result = 0;
                lock (cLock)
                {
                    if (cTimes.Count > 1)
                    {
                        Result = (cLast - cTimes.Peek()) * 1000.0 / Stopwatch.Frequency / (cTimes.Count - 1);
                    }
                }
                return Result;
            }
        }

        public void Mark()
        {
            lock (cLock)
            {
                long Now = Stopwatch.GetTimestamp();
                if (cTimes.Count == 0 || (Now - cLast) * 1000.0 / Stopwatch.Frequency >= MinGapMs)
                {
                    cLast = Now;
                    cTimes.Enqueue(cLast);
                    long Oldest = cLast - (long)(WindowMs * Stopwatch.Frequency / 1000);
                    while (cTimes.Count > 2 && cTimes.Peek() < Oldest) cTimes.Dequeue();
                }
            }
        }
    }
}
