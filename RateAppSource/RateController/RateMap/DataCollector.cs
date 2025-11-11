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
        private const int RecordIntervalMS = 500;
        private readonly object _lock = new object();

        private readonly List<RateReading> Readings = new List<RateReading>();
        private readonly TimeSpan SaveInterval = TimeSpan.FromSeconds(30);
        private readonly Stopwatch SaveStopWatch = new Stopwatch();
        private string cFilePath;
        private double LastLatitude = 0;
        private double LastLongitude = 0;
        private int lastSavedIndex = 0;
        private bool ReadyForNewData = false;
        private Timer RecordTimer;

        public DataCollector()
        {
            RecordTimer = new Timer();
            RecordTimer.Elapsed += RecordTimer_Elapsed;
            RecordTimer.Enabled = Props.RateRecordEnabled;
            RecordTimer.Interval = RecordIntervalMS;

            cFilePath = Props.CurrentRateDataPath;
            LoadDataFromCsv();

            if (Props.RateRecordEnabled) SaveStopWatch.Start();

            Props.JobChanged += Props_JobChanged;
            Props.RateDataSettingsChanged += Props_RateDataSettingsChanged;
        }

        public int DataPoints
        { get { return Readings.Count; } }

        public void ClearReadings()
        {
            lock (_lock)
            {
                Readings.Clear();
                lastSavedIndex = 0;
                if (Props.IsPathSafeToDelete(cFilePath)) File.Delete(cFilePath);
            }
        }

        public IReadOnlyList<RateReading> GetReadings()
        {
            lock (_lock)
            {
                return Readings.ToList();
            }
        }

        public int GetReadingsCount()
        {
            lock (_lock)
            {
                return Readings.Count;
            }
        }

        public void RecordReading(double latitude, double longitude, double[] appliedRates, double[] targetRates)
        {
            if (ReadyForNewData)
            {
                ReadyForNewData = false;
                RecordTimer.Enabled = Props.RateRecordEnabled;

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
                            SaveDataToCsv();
                            SaveStopWatch.Start();
                        }
                    }
                }
            }
        }

        public void SaveDataToCsv()
        {
            try
            {
                List<RateReading> snapshot;
                lock (_lock)
                {
                    // Skip the readings that have already been saved.
                    snapshot = Readings.Skip(lastSavedIndex).ToList();
                    // Update the index so that next time only the new items will be written.
                    lastSavedIndex = Readings.Count;
                }

                bool fileExists = File.Exists(cFilePath);
                using (var writer = new StreamWriter(cFilePath, append: true))
                {
                    // If the file does not yet exist, write the header.
                    if (!fileExists)
                    {
                        string header = "Timestamp,Latitude,Longitude," +
                                        "AppliedRate1,AppliedRate2,AppliedRate3,AppliedRate4,AppliedRate5," +
                                        "TargetRate1,TargetRate2,TargetRate3,TargetRate4,TargetRate5";
                        writer.WriteLine(header);
                    }

                    // Write each reading as a CSV line.
                    foreach (var reading in snapshot)
                    {
                        string timestamp = reading.Timestamp.ToString("O", CultureInfo.InvariantCulture);
                        string lat = reading.Latitude.ToString(CultureInfo.InvariantCulture);
                        string lon = reading.Longitude.ToString(CultureInfo.InvariantCulture);
                        string basicValues = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", timestamp, lat, lon);

                        // Prepare the applied rates columns (filling unused columns with empty strings).
                        string[] appliedColumns = new string[5];
                        for (int i = 0; i < 5; i++)
                        {
                            appliedColumns[i] = i < reading.AppliedRates.Length
                                ? reading.AppliedRates[i].ToString(CultureInfo.InvariantCulture)
                                : "";
                        }
                        string appliedData = string.Join(",", appliedColumns);

                        // Prepare the target rates columns.
                        string[] targetColumns = new string[5];
                        for (int i = 0; i < 5; i++)
                        {
                            targetColumns[i] = i < reading.TargetRates.Length
                                ? reading.TargetRates[i].ToString(CultureInfo.InvariantCulture)
                                : "";
                        }
                        string targetData = string.Join(",", targetColumns);

                        // Construct the full CSV line.
                        string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2}", basicValues, appliedData, targetData);
                        writer.WriteLine(csvLine);
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

        private void LoadDataFromCsv()
        {
            try
            {
                if (!File.Exists(cFilePath))
                    throw new FileNotFoundException("The specified data file was not found.", cFilePath);

                // Clear the existing readings before loading.
                lock (_lock)
                {
                    Readings.Clear();
                }

                // Using a using block ensures the file is closed immediately after reading.
                using (var reader = new StreamReader(cFilePath))
                {
                    if (!reader.EndOfStream)
                    {
                        // Read and discard header line.
                        string headerLine = reader.ReadLine();

                        while (!reader.EndOfStream)
                        {
                            string line = reader.ReadLine();
                            if (string.IsNullOrWhiteSpace(line))
                                continue;

                            // Assuming the CSV format matches our SaveDataToCsv header.
                            // Split on comma. This simple approach assumes that the data values do not contain commas.
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
                            for (int i = 3; i < 8; i++)
                            {
                                double rate;
                                if (double.TryParse(parts[i], NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
                                {
                                    appliedRates.Add(rate);
                                }
                            }

                            // Parse target rates.
                            List<double> targetRates = new List<double>();
                            for (int i = 8; i < 13; i++)
                            {
                                double rate;
                                if (double.TryParse(parts[i], NumberStyles.Number, CultureInfo.InvariantCulture, out rate))
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
                    }
                }
                // After fully loading the CSV, set lastSavedIndex to the current count of readings.
                lock (_lock)
                {
                    lastSavedIndex = Readings.Count;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("DataCollector/LoadDataFromCSV: " + ex.Message);
            }
        }

        private void Props_JobChanged(object sender, EventArgs e)
        {
            cFilePath = Props.CurrentRateDataPath;
            LoadDataFromCsv();
        }

        private void Props_RateDataSettingsChanged(object sender, EventArgs e)
        {
            RecordTimer.Enabled = Props.RateRecordEnabled;
            RecordTimer.Interval = RecordIntervalMS;
            if (Props.RateRecordEnabled)
            {
                SaveStopWatch.Start();
            }
            else
            {
                SaveStopWatch.Stop();
            }
        }

        private void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadyForNewData = true;
            RecordTimer.Enabled = false;
        }
    }
}