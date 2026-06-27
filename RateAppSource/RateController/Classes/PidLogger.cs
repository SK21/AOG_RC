using System;
using System.Globalization;
using System.IO;

namespace RateController.Classes
{
    public struct PidLogSample
    {
        public DateTime PCTime;
        public uint ModuleMillis;
        public int ModuleID;
        public int SensorID;
        public double Target;
        public double Applied;
        public double Error;
        public double Integral;
        public double Change;
        public double PWM;
        public int Samples;
        public int InoID;
    }

    // Records PID diagnostics (PGN 32402) to a CSV file for offline analysis.
    // One global instance writes a single file; UDP receive runs on a background
    // thread, so all file access is locked.
    //
    // Example file contents:
    //   PCTime,ModuleMillis,ModuleID,SensorID,Target,Applied,Error,Integral,Change,PWM,Samples,InoID
    //   14:32:07.812,1048576,0,0,120,114.3,5.7,12.4,38.2,162,12,25066
    //   14:32:07.912,1048676,0,0,120,118.9,1.1,12.4,7.3,170,12,25066
    //   14:32:08.013,1048776,0,0,120,121.4,-1.4,0,-9.6,160,9,25066
    //
    //   PCTime       - tablet clock when the sample was received (HH:mm:ss.fff)
    //   ModuleMillis - Teensy millis() at the PID loop; use this to see true sub-200 ms cadence
    //   Target       - target rate (UPM)
    //   Applied      - measured rate (UPM)
    //   Error        - Target - Applied, raw before deadband/constrain
    //   Integral     - accumulated integral term (cleared on error sign-flip)
    //   Change       - PID change amount this loop
    //   PWM          - resulting valve PWM (signed)
    //   Samples      - pulse samples used in the median this loop (time-window filter; low = low flow / fewer samples)
    //   InoID        - firmware build id that produced the row (e.g. 25066); identifies the exact firmware
    public class PidLogger
    {
        private readonly object cLock = new object();
        private string cFilePath;
        private bool cRunning;
        private StreamWriter cWriter;

        public string FilePath
        { get { return cFilePath; } }

        public bool IsRunning
        { get { return cRunning; } }

        public void Log(PidLogSample s)
        {
            lock (cLock)
            {
                if (cRunning && cWriter != null)
                {
                    try
                    {
                        cWriter.WriteLine(string.Join(",",
                            s.PCTime.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture),
                            s.ModuleMillis.ToString(CultureInfo.InvariantCulture),
                            s.ModuleID.ToString(CultureInfo.InvariantCulture),
                            s.SensorID.ToString(CultureInfo.InvariantCulture),
                            s.Target.ToString("0.###", CultureInfo.InvariantCulture),
                            s.Applied.ToString("0.###", CultureInfo.InvariantCulture),
                            s.Error.ToString("0.###", CultureInfo.InvariantCulture),
                            s.Integral.ToString("0.#", CultureInfo.InvariantCulture),
                            s.Change.ToString("0.###", CultureInfo.InvariantCulture),
                            s.PWM.ToString("0.###", CultureInfo.InvariantCulture),
                            s.Samples.ToString(CultureInfo.InvariantCulture),
                            s.InoID.ToString(CultureInfo.InvariantCulture)));
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("PidLogger/Log " + ex.Message);
                    }
                }
            }
        }

        public void Start()
        {
            lock (cLock)
            {
                if (!cRunning)
                {
                    try
                    {
                        string folder = Path.Combine(Props.DataFolder, "PIDLogs");
                        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                        cFilePath = Path.Combine(folder, "PIDlog_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
                        cWriter = new StreamWriter(cFilePath, false) { AutoFlush = true };
                        cWriter.WriteLine("PCTime,ModuleMillis,ModuleID,SensorID,Target,Applied,Error,Integral,Change,PWM,Samples,InoID");
                        cRunning = true;
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("PidLogger/Start " + ex.Message);
                    }
                }
            }
        }

        public void Stop()
        {
            lock (cLock)
            {
                if (cRunning)
                {
                    try
                    {
                        cWriter?.Flush();
                        cWriter?.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Props.WriteErrorLog("PidLogger/Stop " + ex.Message);
                    }
                    cWriter = null;
                    cRunning = false;
                }
            }
        }
    }
}
