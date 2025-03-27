using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Timers;

namespace RateController.Classes
{
    public class DataCollector
    {
        private readonly object _lock = new object();

        private readonly List<RateReading> Readings = new List<RateReading>();
        private string cFilePath;
        private double LastLatitude = 0;
        private double LastLongitude = 0;
        private bool NewData = false;
        private bool ReadyForNewData = false;
        private Timer RecordTimer;
        private Timer SaveTimer;

        public DataCollector()
        {
            SaveTimer = new Timer(30000);
            SaveTimer.Elapsed += SaveTimer_Elapsed;
            SaveTimer.Enabled = Props.RateRecordEnabled;

            RecordTimer = new Timer();
            RecordTimer.Elapsed += RecordTimer_Elapsed;
            RecordTimer.Enabled = Props.RateRecordEnabled;
            RecordIntervalSeconds = Props.RateRecordInterval;

            Props.JobChanged += Props_JobChanged;
            Props.RecordSettingsChanged += Props_RecordSettingsChanged;
        }

        private void Props_RecordSettingsChanged(object sender, EventArgs e)
        {
            RecordTimer.Enabled = Props.RateRecordEnabled;
            RecordIntervalSeconds = Props.RateRecordInterval;
        }

        private void Props_JobChanged(object sender, EventArgs e)
        {
            cFilePath = Props.CurrentRateDataPath;
            LoadDataFromCsv();
        }

        public int RecordIntervalSeconds
        {
            set
            {
                if (value < 1 || value > 600) value = 30;
                SaveTimer.Interval = value * 1000;
            }
        }


        public IReadOnlyList<RateReading> GetReadings()
        {
            lock (_lock)
            {
                return Readings.ToList();
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
                        NewData = true;
                    }
                }
            }
        }

        public void SaveDataToCsv()
        {
            if (NewData)
            {
                List<RateReading> snapshot;
                lock (_lock)
                {
                    // Take a snapshot for thread safety.
                    snapshot = Readings.ToList();
                }

                using (var writer = new StreamWriter(cFilePath))
                {
                    // Prepare the header (includes columns for 5 applied and 5 target rates).
                    string header = "Timestamp,Latitude,Longitude," +
                                    "AppliedRate1,AppliedRate2,AppliedRate3,AppliedRate4,AppliedRate5," +
                                    "TargetRate1,TargetRate2,TargetRate3,TargetRate4,TargetRate5";
                    writer.WriteLine(header);

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
                        NewData = false;
                    }
                }
            }
        }

        private bool HasMoved(double latitude, double longitude)
        {
            // check for a movement of about 3.6 ft, 0.00001 degrees (5 decimal places)
            bool Result = false;
            if (LastLatitude > 0 && LastLongitude > 0)
            {
                double NewLat = Math.Round(latitude, 5);
                double NewLng = Math.Round(longitude, 5);
                if (NewLat != LastLatitude || NewLng != LastLongitude)
                {
                    Result = true;
                    LastLatitude = NewLat;
                    LastLongitude = NewLng;
                }
            }
            return Result;
        }

        private void LoadDataFromCsv()
        {
            if (!File.Exists(cFilePath))
                throw new FileNotFoundException("The specified data file was not found.", cFilePath);

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
                        for (int i = 8; i < 14; i++)
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
        }

        private void RecordTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ReadyForNewData = true;
            RecordTimer.Enabled = false;
        }

        private void SaveTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SaveDataToCsv();
        }
    }
}