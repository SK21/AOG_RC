using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;

namespace RateController.Classes
{
    public class DataCollector
    {
        private const int RecordIntervalMS = 1000;
        private readonly object _lock = new object();

        private readonly List<RateReading> Readings = new List<RateReading>();
        private readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(30);
        private readonly Stopwatch SaveStopWatch = new Stopwatch();
        private bool cEnabled = true;
        private int LastCount = 0;
        private double LastLatitude = 0;
        private double LastLongitude = 0;
        private int LastProductID = -1;
        private int lastSavedIndex = 0;
        private string LastSavePath;
        private bool ReadyForNewData = false;
        private Timer RecordTimer;

        public DataCollector()
        {
            LoadData();

            cEnabled = bool.TryParse(Props.GetProp("RecordRates"), out bool rec) ? rec : true;

            RecordTimer = new Timer();
            RecordTimer.Elapsed += RecordTimer_Elapsed;
            RecordTimer.Interval = RecordIntervalMS;
            RecordTimer.Enabled = cEnabled;
            JobManager.JobChanged += Props_JobChanged;
            Props.ProfileChanged += Props_ProfileChanged;
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                cEnabled = value;
                RecordTimer.Enabled = cEnabled;
                Props.SetProp("RecordRates", cEnabled.ToString());

                if (cEnabled)
                {
                    SaveStopWatch.Start();
                }
                else
                {
                    SaveStopWatch.Stop();
                }
            }
        }

        public void ClearReadings()
        {
            lock (_lock)
            {
                Readings.Clear();
                lastSavedIndex = 0;
                LastProductID = -1;
                LastCount = 0;
                if (Props.IsPathSafe(JobManager.CurrentRateDataPath)) File.Delete(JobManager.CurrentRateDataPath);
            }
        }

        public int DataPoints(int ProductID)
        {
            int Result = 0;
            if ((lastSavedIndex == Readings.Count) && (ProductID == LastProductID))
            {
                Result = LastCount;
            }
            else
            {
                LastCount = Readings.Count(r => r.AppliedRates.Length > ProductID && r.AppliedRates[ProductID] > 0);
                LastProductID = ProductID;
                Result = LastCount;
            }
            return Result;
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
                if (!File.Exists(JobManager.CurrentRateDataPath))
                    throw new FileNotFoundException("The specified data file was not found.", JobManager.CurrentRateDataPath);

                SaveStopWatch.Reset();
                SaveData(LastSavePath); // save any unrecorded data

                // Clear the existing readings before loading.
                lock (_lock)
                {
                    Readings.Clear();
                    LastProductID = -1;
                    LastCount = 0;
                }

                string expectedHeader = "Timestamp,Latitude,Longitude,AppliedRate1,AppliedRate2,AppliedRate3,AppliedRate4,AppliedRate5,TargetRate1,TargetRate2,TargetRate3,TargetRate4,TargetRate5";

                // Check and fix header if missing
                string[] allLines = File.ReadAllLines(JobManager.CurrentRateDataPath);
                if (allLines.Length > 0)
                {
                    string firstLine = allLines[0].Trim();
                    if (string.IsNullOrWhiteSpace(firstLine) || !firstLine.Equals(expectedHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        // Header is missing or invalid: prepend it
                        List<string> linesWithHeader = new List<string> { expectedHeader };
                        linesWithHeader.AddRange(allLines);
                        File.WriteAllLines(JobManager.CurrentRateDataPath, linesWithHeader);
                        Props.WriteErrorLog($"DataCollector/LoadData: Missing or invalid header in {JobManager.CurrentRateDataPath}. Header added automatically.");
                        allLines = linesWithHeader.ToArray();
                    }
                }
                else
                {
                    // Empty file: add header
                    File.WriteAllText(JobManager.CurrentRateDataPath, expectedHeader + Environment.NewLine);
                    Props.WriteErrorLog($"DataCollector/LoadData: Empty file {JobManager.CurrentRateDataPath}. Header added.");
                    allLines = new string[] { expectedHeader };
                }

                // Now parse the lines (skip header)
                for (int i = 1; i < allLines.Length; i++)
                {
                    string line = allLines[i];
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] parts = line.Split(',');

                    // There should be 13 parts: Timestamp, Latitude, Longitude + 5 applied + 5 target.
                    if (parts.Length < 13)
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

                    // Parse target rates.
                    List<double> targetRates = new List<double>();
                    for (int j = 8; j < 13; j++)
                    {
                        double rate;
                        if (double.TryParse(parts[j], NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
                        {
                            targetRates.Add(rate);
                        }
                    }

                    // Only add the reading if the number of applied and target rates match.
                    if (appliedRates.Count == targetRates.Count && appliedRates.Count > 0)
                    {
                        var reading = new RateReading(timestamp, latitude, longitude, appliedRates.ToArray(), targetRates.ToArray());
                        lock (_lock)
                        {
                            Readings.Add(reading);
                        }
                    }
                }

                // After fully loading the CSV, set lastSavedIndex to the current count of readings.
                lock (_lock)
                {
                    lastSavedIndex = Readings.Count;
                }

                if (cEnabled) SaveStopWatch.Start();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/LoadDataFromCSV: " + ex.Message);
            }
        }

        public void RecordReading(double latitude, double longitude, double[] appliedRates, double[] targetRates)
        {
            if (ReadyForNewData && Props.ProductsAreOn)
            {
                ReadyForNewData = false;
                RecordTimer.Enabled = cEnabled;

                bool IsValid = true;

                if (appliedRates == null || appliedRates.Length == 0 || appliedRates.Length > 5)
                {
                    IsValid = false;
                }
                else if (targetRates == null || targetRates.Length == 0 || targetRates.Length > 5)
                {
                    IsValid = false;
                }
                else if (appliedRates.Length != targetRates.Length) IsValid = false;

                if (IsValid && HasMoved(latitude, longitude))
                {
                    var reading = new RateReading(DateTime.UtcNow, latitude, longitude, appliedRates, targetRates);
                    lock (_lock)
                    {
                        Readings.Add(reading);

                        if (SaveStopWatch.Elapsed >= SaveInterval)
                        {
                            SaveStopWatch.Reset();
                            SaveData(JobManager.CurrentRateDataPath);
                            SaveStopWatch.Start();
                        }
                    }
                }
            }
        }

        public void SaveData(string DataFilePath)
        {
            try
            {
                if (DataFilePath != null)
                {
                    LastSavePath = DataFilePath;
                    List<RateReading> snapshot;
                    lock (_lock)
                    {
                        snapshot = Readings.Skip(lastSavedIndex).ToList();
                        lastSavedIndex = Readings.Count;
                    }

                    bool fileExists = File.Exists(DataFilePath);
                    string header = "Timestamp,Latitude,Longitude," +
                                    "AppliedRate1,AppliedRate2,AppliedRate3,AppliedRate4,AppliedRate5," +
                                    "TargetRate1,TargetRate2,TargetRate3,TargetRate4,TargetRate5";

                    using (var writer = new StreamWriter(DataFilePath, append: true))
                    {
                        // Check if header is missing (file exists but no header)
                        if (fileExists)
                        {
                            string firstLine = File.ReadLines(DataFilePath).FirstOrDefault();
                            if (string.IsNullOrWhiteSpace(firstLine) || !firstLine.Contains("Timestamp"))
                            {
                                // Prepend header by rewriting the file
                                var existingLines = File.ReadAllLines(DataFilePath);
                                File.WriteAllText(DataFilePath, header + Environment.NewLine);
                                File.AppendAllLines(DataFilePath, existingLines);
                            }
                        }
                        else
                        {
                            // New file: write header
                            writer.WriteLine(header);
                        }

                        // Append new data
                        foreach (var reading in snapshot)
                        {
                            string timestamp = reading.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                            string lat = reading.Latitude.ToString(CultureInfo.InvariantCulture);
                            string lon = reading.Longitude.ToString(CultureInfo.InvariantCulture);
                            string basicValues = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", timestamp, lat, lon);

                            string[] appliedColumns = new string[5];
                            for (int i = 0; i < 5; i++)
                            {
                                appliedColumns[i] = i < reading.AppliedRates.Length
                                    ? reading.AppliedRates[i].ToString(CultureInfo.InvariantCulture)
                                    : "";
                            }
                            string appliedData = string.Join(",", appliedColumns);

                            string[] targetColumns = new string[5];
                            for (int i = 0; i < 5; i++)
                            {
                                targetColumns[i] = i < reading.TargetRates.Length
                                    ? reading.TargetRates[i].ToString(CultureInfo.InvariantCulture)
                                    : "";
                            }
                            string targetData = string.Join(",", targetColumns);

                            string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", basicValues, appliedData, targetData);
                            writer.WriteLine(csvLine);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/SaveDataToCSV: " + ex.Message);
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

        private void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadyForNewData = true;
            RecordTimer.Enabled = false;
        }
    }
}