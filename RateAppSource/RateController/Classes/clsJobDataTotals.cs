using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace RateController.Classes
{
    public class clsJobDataTotals
    {
        private const string DataFileName = "JobStats.CSV";

        public void GetTotals(Job jb, int ProductID, out TimeSpan WorkedTime, out double QuantityTotal, out double HectaresTotal)
        {
            WorkedTime = TimeSpan.Zero;
            QuantityTotal = 0;
            HectaresTotal = 0;
            var timestamps = new List<DateTime>();

            if (jb != null)
            {
                string FilePath = Path.Combine(jb.JobFolder, DataFileName);

                try
                {
                    using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fs))
                    {
                        string line = reader.ReadLine();
                        if (line != null)
                        {
                            bool headerDetected = line.StartsWith("TimeStamp", StringComparison.OrdinalIgnoreCase);
                            if (!headerDetected)
                            {
                                Accumulate(line, ProductID, ref QuantityTotal, ref HectaresTotal, timestamps);
                            }
                        }
                        while ((line = reader.ReadLine()) != null)
                        {
                            Accumulate(line, ProductID, ref QuantityTotal, ref HectaresTotal, timestamps);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("clsJobDataTotals/GetTotals: " + ex.Message);
                }

                GetTime(timestamps, out WorkedTime);
            }
        }

        private void Accumulate(string line, int ProductID, ref double quantity, ref double hectares, List<DateTime> timestamps)
        {
            // CSV format: TimeStamp,ProductID,Quantity,Hectares
            // Example: 2025-12-06T12:34:56,3,0.2500,1.2345

            if (!string.IsNullOrWhiteSpace(line))
            {
                var parts = line.Split(',');
                if (parts.Length > 3)
                {
                    if (int.TryParse(parts[1], out int ID))
                    {
                        if (ID == ProductID)
                        {
                            // Parse Quantity and Hectares using invariant culture
                            if (!double.TryParse(parts[2], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double qt))
                            {
                                qt = 0;
                            }
                            if (!double.TryParse(parts[3], NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out double hc))
                            {
                                hc = 0;
                            }

                            quantity += qt;
                            hectares += hc;

                            DateTime ts;
                            if (DateTime.TryParseExact(parts[0], "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out ts))
                            {
                                timestamps.Add(ts);
                            }
                        }
                    }
                }
            }
        }

        private void GetTime(List<DateTime> timestamps, out TimeSpan span)
        {
            span = TimeSpan.Zero;
            try
            {
                if (timestamps.Count > 1)
                {
                    timestamps.Sort();

                    // Compute all consecutive intervals.
                    var intervals = new List<TimeSpan>(timestamps.Count - 1);
                    for (int i = 1; i < timestamps.Count; i++)
                    {
                        var delta = timestamps[i] - timestamps[i - 1];
                        if (delta > TimeSpan.Zero)
                        {
                            intervals.Add(delta);
                        }
                    }

                    if (intervals.Count > 1)
                    {
                        // Average interval across all observed intervals.
                        double avgTicks = 0;
                        foreach (var ts in intervals)
                        {
                            avgTicks += ts.Ticks;
                        }
                        avgTicks /= intervals.Count;
                        var avgInterval = new TimeSpan(Convert.ToInt64(avgTicks));

                        // Threshold: 120% of average.
                        var threshold = TimeSpan.FromTicks((long)(avgInterval.Ticks * 1.2));

                        // Sum only intervals that are <= threshold.
                        long acceptedTicks = 0;
                        foreach (var ts in intervals)
                        {
                            if (ts <= threshold)
                            {
                                acceptedTicks += ts.Ticks;
                            }
                        }
                        span = new TimeSpan(acceptedTicks);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobDataTotals/GetTime: " + ex.Message);
            }
        }
    }
}