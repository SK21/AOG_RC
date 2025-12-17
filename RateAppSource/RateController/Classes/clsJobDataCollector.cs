using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace RateController.Classes
{
    public class clsJobDataCollector
    {
        // JobStats.CSV in the job folder
        // each entry is updated every minute

        private string CSVheader = "ProductID,StartTime,EndTime,Quantity,Hectares";
        private Job CurrentJob;
        private string DataFileName = "JobStats.CSV";
        private string DateFormat = "yyyy-MM-ddTHH:mm:ss";
        private double[] LastHectares;
        private double[] LastQuantity;
        private JobProductData[] ProductData;
        private System.Windows.Forms.Timer RecordTimer;
        private int SaveCounter = 0;

        public clsJobDataCollector()
        {
            Debug.Print("Data Collector");
            LastQuantity = new double[Props.MaxProducts];
            LastHectares = new double[Props.MaxProducts];
            ProductData = new JobProductData[Props.MaxProducts];
            RecordTimer = new System.Windows.Forms.Timer();
            RecordTimer.Interval = 1000;   // milliseconds
            RecordTimer.Tick += RecordTimer_Tick;
            RecordTimer.Enabled = false;

            CurrentJob = JobManager.CurrentJob;
            InitializeProductData();
            ProductData = LoadData(CurrentJob);

            JobManager.JobChanged += JobManager_JobChanged;
            Props.AppExit += Props_AppExit;
        }

        public bool Enabled
        {
            get { return RecordTimer.Enabled; }
            set { RecordTimer.Enabled = value; }
        }

        public JobProductData[] JobData(Job JB)
        {
            JobProductData[] Data = LoadData(JB);
            return Data;
        }

        private void InitializeProductData()
        {
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                // Max value causes LastQuantity and LastHectares to be
                // re-baselined on first run.
                LastQuantity[i] = double.MaxValue;
                LastHectares[i] = double.MaxValue;
            }
        }

        private void JobManager_JobChanged(object sender, EventArgs e)
        {
            SaveData(); // save old job data

            CurrentJob = JobManager.CurrentJob;
            InitializeProductData();
            ProductData = LoadData(CurrentJob);
        }

        private JobProductData[] LoadData(Job JB)
        {
            JobProductData[] Data = new JobProductData[Props.MaxProducts];
            for (int i = 0; i < Props.MaxProducts; i++)
            {
                Data[i] = new JobProductData
                {
                    ProductID = i,
                    StartTime = DateTime.MinValue,
                    EndTime = DateTime.MinValue,
                    Quantity = 0,
                    Hectares = 0
                };
            }

            try
            {
                if (JB != null)
                {
                    string FileLocation = Path.Combine(JB.JobFolder, DataFileName);
                    if (File.Exists(FileLocation))
                    {
                        using (var reader = new StreamReader(FileLocation))
                        {
                            string headerline = reader.ReadLine();  // skip header
                            while (!reader.EndOfStream)
                            {
                                string line = reader.ReadLine();
                                var values = line.Split(',');
                                if (values.Length == 5)
                                {
                                    var record = new JobProductData();

                                    record.ProductID = int.TryParse(values[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int ID) ? ID : -1;
                                    if (record.ProductID >= 0 && record.ProductID < Props.MaxProducts)
                                    {
                                        record.StartTime = DateTime.TryParseExact(values[1], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime st) ? st : DateTime.MinValue;
                                        record.EndTime = DateTime.TryParseExact(values[2], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime et) ? et : DateTime.MinValue;
                                        record.Quantity = double.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double qt) ? qt : 0;
                                        record.Hectares = double.TryParse(values[4], NumberStyles.Float, CultureInfo.InvariantCulture, out double hc) ? hc : 0;

                                        Data[record.ProductID] = record;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobDataCollector/LoadData: " + ex.Message);
            }
            return Data;
        }

        private void Props_AppExit(object sender, EventArgs e)
        {
            SaveData();
            JobManager.JobChanged-= JobManager_JobChanged;
            Props.AppExit-= Props_AppExit;
        }

        private void RecordData()
        {
            try
            {
                if (CurrentJob != null)
                {
                    DateTime SaveTime = DateTime.Now;
                    string FileLocation = Path.Combine(CurrentJob.JobFolder, DataFileName);

                    foreach (clsProduct Prd in Props.MainForm.Products.Items)
                    {
                        JobProductData PD = ProductData[Prd.ID];
                        if (Prd.ControlType != ControlTypeEnum.Fan)
                        {
                            if (Prd.ProductOn(false))
                            {
                                if (PD.StartTime == DateTime.MinValue || SaveTime < PD.StartTime) PD.StartTime = SaveTime;
                                if (PD.EndTime == DateTime.MinValue || SaveTime > PD.EndTime) PD.EndTime = SaveTime;
                            }

                            // quantity
                            double Applied = Prd.UnitsApplied() - LastQuantity[Prd.ID];
                            LastQuantity[Prd.ID] = Prd.UnitsApplied();
                            if (Applied > 0) PD.Quantity += Applied;

                            // hectares
                            double worked = Prd.SessionTotalHectares() - LastHectares[Prd.ID];
                            LastHectares[Prd.ID] = Prd.SessionTotalHectares();
                            if (worked > 0) PD.Hectares += worked;
                        }
                        ProductData[Prd.ID] = PD;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobDataCollector/RecordData: " + ex.Message);
            }
        }

        private void RecordTimer_Tick(object sender, EventArgs e)
        {
            if (Props.MainForm.Products.ProductsAreOn())
            {
                RecordData();

                SaveCounter++;
                if (SaveCounter > 60)
                {
                    SaveCounter = 0;
                    SaveData();
                }
            }
        }

        private void SaveData()
        {
            try
            {
                string FileLocation = Path.Combine(CurrentJob.JobFolder, DataFileName);
                using (var writer = new StreamWriter(FileLocation, false))
                {
                    writer.WriteLine(CSVheader);

                    for (int i = 0; i < Props.MaxProducts; i++)
                    {
                        var PD = ProductData[i];

                        string ID = PD.ProductID.ToString(CultureInfo.InvariantCulture);
                        string Start = (PD.StartTime == DateTime.MinValue ? "" : PD.StartTime.ToString(DateFormat, CultureInfo.InvariantCulture));
                        string End = (PD.EndTime == DateTime.MinValue ? "" : PD.EndTime.ToString(DateFormat, CultureInfo.InvariantCulture));
                        string Quantity = PD.Quantity.ToString("F4", CultureInfo.InvariantCulture);
                        string Hectares = PD.Hectares.ToString("F4", CultureInfo.InvariantCulture);

                        string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", ID, Start, End, Quantity, Hectares);
                        writer.WriteLine(csvLine);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobDataCollector/SaveData: " + ex.Message);
            }
        }
    }

    public class JobProductData
    {
        public DateTime EndTime { get; set; }
        public double Hectares { get; set; }
        public int ProductID { get; set; }
        public double Quantity { get; set; }
        public DateTime StartTime { get; set; }
    }
}