using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public class DataCollector : IDisposable
    {
        public const string CSVheader = "Timestamp,Latitude,Longitude," + "AppliedRate1,AppliedRate2,AppliedRate3,AppliedRate4,AppliedRate5," + "WidthMeters";
        private readonly object _lock = new object();
        private readonly List<RateReading> Readings = new List<RateReading>();
        private bool cEnabled = true;
        private double LastLatitude = 0;
        private double LastLongitude = 0;
        private int lastSavedIndex = 0;
        private string LastSavePath;
        private DateTime LastSaveTime = DateTime.Now;
        private int SaveIntervalSeconds = 30;

        public DataCollector(int SaveToDiscSeconds = 30)
        {
            SaveIntervalSeconds = SaveToDiscSeconds;
            LoadData();

            cEnabled = bool.TryParse(Props.GetProp("RecordRates"), out bool rec) ? rec : true;

            JobManager.JobChanged += Props_JobChanged;
            Props.ProfileChanged += Props_ProfileChanged;
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
                Props.SetProp("RecordRates", cEnabled.ToString());
            }
        }

        public void ClearReadings()
        {
            lock (_lock)
            {
                Readings.Clear();
                lastSavedIndex = 0;
                if (Props.IsPathSafe(JobManager.CurrentRateDataPath)) File.Delete(JobManager.CurrentRateDataPath);
            }
        }

        public bool CSVheaderValid(string DataFilePath = null)
        {
            bool Result = false;
            try
            {
                if (DataFilePath == null) DataFilePath = LastSavePath;

                if (DataFilePath != null)
                {
                    if (!File.Exists(DataFilePath))
                    {
                        using (var writer = new StreamWriter(DataFilePath, false))
                        {
                            writer.WriteLine(CSVheader);
                        }
                    }

                    string firstLine = null;
                    foreach (string line in File.ReadLines(DataFilePath))
                    {
                        firstLine = line;
                        break;
                    }

                    bool headerIsMissing = false;
                    if (string.IsNullOrWhiteSpace(firstLine))
                    {
                        headerIsMissing = true;
                    }
                    else if (!firstLine.StartsWith("Timestamp", StringComparison.OrdinalIgnoreCase))
                    {
                        headerIsMissing = true;
                    }

                    // prepend header to any existing data
                    if (headerIsMissing)
                    {
                        string[] existingLines = File.ReadAllLines(DataFilePath);
                        using (var headerWriter = new StreamWriter(DataFilePath, false))
                        {
                            headerWriter.WriteLine(CSVheader);
                            for (int i = 0; i < existingLines.Length; i++)
                            {
                                headerWriter.WriteLine(existingLines[i]);
                            }
                        }
                    }
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/CheckCSVheader: " + ex.Message);
            }
            return Result;
        }

        public int DataPoints(int ProductID)
        {
            lock (_lock)
            {
                return Readings.Count(r => r.AppliedRates.Length > ProductID && r.AppliedRates[ProductID] > 0);
            }
        }

        public void Dispose()
        {
            JobManager.JobChanged -= Props_JobChanged;
            Props.ProfileChanged -= Props_ProfileChanged;
        }

        public IReadOnlyList<RateReading> GetReadings()
        {
            lock (_lock)
            {
                return Readings.ToList();
            }
        }

        public void LoadData()
        {
            try
            {
                if (File.Exists(JobManager.CurrentRateDataPath))
                {
                    SaveData(); // save any unrecorded data

                    // Clear the existing readings before loading.
                    lock (_lock)
                    {
                        Readings.Clear();
                    }

                    if (CSVheaderValid(JobManager.CurrentRateDataPath))
                    {
                        string[] allLines = File.ReadAllLines(JobManager.CurrentRateDataPath);

                        // parse the lines (skip header)
                        for (int i = 1; i < allLines.Length; i++)
                        {
                            string line = allLines[i];
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            string[] parts = line.Split(',');

                            // There should be 9 parts: Timestamp, Latitude, Longitude + 5 applied + width.
                            if (parts.Length < 9)
                                continue;

                            // Parse basic values.
                            DateTime timestamp;
                            double latitude, longitude;

                            if (!DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out timestamp) ||
                            !double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out latitude) ||
                            !double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                            {
                                continue;
                            }

                            // Parse applied rates.
                            List<double> appliedRates = new List<double>();
                            for (int j = 3; j < 8; j++)
                            {
                                double rate;
                                if (double.TryParse(parts[j], NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
                                {
                                    appliedRates.Add(rate);
                                }
                            }

                            // Parse implement width
                            double width = double.TryParse(parts[8], NumberStyles.Number, CultureInfo.InvariantCulture, out double wd) ? wd : 0;

                            if (appliedRates.Count > 0)
                            {
                                var reading = new RateReading(timestamp, latitude, longitude, appliedRates.ToArray(), width);
                                lock (_lock)
                                {
                                    Readings.Add(reading);
                                }
                            }
                        }
                    }

                    // After fully loading the CSV, set lastSavedIndex to the current count of readings.
                    lock (_lock)
                    {
                        lastSavedIndex = Readings.Count;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/LoadData: " + ex.Message);
            }
        }

        public void RecordReading(double latitude, double longitude, double[] appliedRates)
        {
            if (cEnabled)
            {
                double ImplementWidthMeters = Props.MainForm.Sections.TotalWidth(false);

                bool IsValid = true;
                if (appliedRates == null || appliedRates.Length == 0 || appliedRates.Length > 5)
                {
                    IsValid = false;
                }
                else if (ImplementWidthMeters < 0.01)
                {
                    IsValid = false;
                }

                if (IsValid && HasMoved(latitude, longitude))
                {
                    var reading = new RateReading(DateTime.Now, latitude, longitude, appliedRates, ImplementWidthMeters);
                    lock (_lock)
                    {
                        Readings.Add(reading);
                    }

                    if ((DateTime.Now - LastSaveTime).TotalSeconds > SaveIntervalSeconds)
                    {
                        SaveData(JobManager.CurrentRateDataPath);
                        LastSaveTime = DateTime.Now;
                    }
                }
            }
        }

        public void SaveData(string DataFilePath = null)
        {
            try
            {
                if (DataFilePath == null) DataFilePath = LastSavePath;

                if (DataFilePath != null)
                {
                    LastSavePath = DataFilePath;
                    List<RateReading> snapshot;
                    lock (_lock)
                    {
                        snapshot = Readings.Skip(lastSavedIndex).ToList();
                        lastSavedIndex = Readings.Count;
                    }

                    if (snapshot.Count > 0 && CSVheaderValid())
                    {
                        using (var writer = new StreamWriter(DataFilePath, true))
                        {
                            foreach (var reading in snapshot)
                            {
                                string timestamp = reading.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                                string lat = reading.Latitude.ToString(CultureInfo.InvariantCulture);
                                string lon = reading.Longitude.ToString(CultureInfo.InvariantCulture);
                                string basicValues = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", timestamp, lat, lon);

                                string[] appliedColumns = new string[5];
                                for (int i = 0; i < 5; i++)
                                {
                                    if (i < reading.AppliedRates.Length)
                                    {
                                        appliedColumns[i] = reading.AppliedRates[i].ToString(CultureInfo.InvariantCulture);
                                    }
                                    else
                                    {
                                        appliedColumns[i] = string.Empty;
                                    }
                                }
                                string appliedData = string.Join(",", appliedColumns);

                                string ImplementWidthMeters = reading.ImplementWidthMeters.ToString(CultureInfo.InvariantCulture);

                                string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", basicValues, appliedData, ImplementWidthMeters);
                                writer.WriteLine(csvLine);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/SaveData: " + ex.Message);
            }
        }

        private bool HasMoved(double latitude, double longitude)
        {
            // check for a movement of about 3.6 ft, 0.00001 degrees (5 decimal places)
            bool Result = false;
            double NewLat = Math.Round(latitude, 5);
            double NewLng = Math.Round(longitude, 5);
            if (NewLat != LastLatitude || NewLng != LastLongitude)
            {
                Result = true;
                LastLatitude = NewLat;
                LastLongitude = NewLng;
            }
            return Result;
        }

        private void Props_JobChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        private void Props_ProfileChanged(object sender, EventArgs e)
        {
            LoadData();
        }
    }
}