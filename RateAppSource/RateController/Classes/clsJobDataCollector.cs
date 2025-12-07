using System;
using System.Globalization;
using System.IO;

namespace RateController.Classes
{
    public class clsJobDataCollector
    {
        // JobStats.CSV in the job folder
        // each entry is updated every minute

        private string CSVheader = "ProductID,StartTime,EndTime,Quantity,Hectares";
        private string DataFileName = "JobStats.CSV";
        private string DateFormat = "yyyy-MM-ddTHH:mm:ss";
        private double[] LastQuantity;
        private JobProductData[] ProductData;
        private System.Windows.Forms.Timer RecordTimer;

        public clsJobDataCollector()
        {
            LastQuantity = new double[Props.MaxProducts];
            ProductData = new JobProductData[Props.MaxProducts];
            RecordTimer = new System.Windows.Forms.Timer();
            RecordTimer.Interval = 1000;   // milliseconds
            RecordTimer.Tick += RecordTimer_Tick;
            RecordTimer.Enabled = false;
            LoadData();
            JobManager.JobChanged += JobManager_JobChanged;
        }

        private void JobManager_JobChanged(object sender, EventArgs e)
        {
            // todo: save old data first
            LoadData();
        }

        public bool Record
        {
            get { return RecordTimer.Enabled; }
            set { RecordTimer.Enabled = value; }
        }


        private void LoadData()
        {
            try
            {
                Job CurrentJob = JobManager.CurrentJob;
                if (CurrentJob != null)
                {
                    string FileLocation = Path.Combine(CurrentJob.JobFolder, DataFileName);
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
                                    if (DateTime.TryParseExact(values[1], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime st))
                                    {
                                        record.StartTime = st;
                                    }
                                    else
                                    {
                                        record.StartTime = DateTime.MinValue;
                                    }

                                    if (DateTime.TryParseExact(values[2], DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime et))
                                    {
                                        record.EndTime = et;
                                    }
                                    else
                                    {
                                        record.EndTime = DateTime.MinValue;
                                    }

                                    record.Quantity = double.TryParse(values[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double qt) ? qt : 0;
                                    LastQuantity[record.ProductID] = record.Quantity;

                                    record.Hectares = double.TryParse(values[4], NumberStyles.Float, CultureInfo.InvariantCulture, out double hc) ? hc : 0;

                                    ProductData[record.ProductID] = record;
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
        }

        private void RecordData()
        {
            try
            {
                if (Props.MainForm.Products.ProductsAreOn())
                {
                    Job CurrentJob = JobManager.CurrentJob;
                    if (CurrentJob != null)
                    {
                        DateTime SaveTime = DateTime.Now;
                        string FileLocation = Path.Combine(CurrentJob.JobFolder, DataFileName);

                        using (var writer = new StreamWriter(FileLocation))
                        {
                            writer.WriteLine(CSVheader);

                            foreach (clsProduct Prd in Props.MainForm.Products.Items)
                            {
                                if (Prd.ControlType != ControlTypeEnum.Fan && Prd.ProductOn(false))
                                {
                                    JobProductData PD = ProductData[Prd.ID];

                                    if (PD.StartTime == DateTime.MinValue || SaveTime < PD.StartTime) PD.StartTime = SaveTime;
                                    if (PD.EndTime == DateTime.MinValue || SaveTime > PD.EndTime) PD.EndTime = SaveTime;

                                    // quantity
                                    double Applied = Prd.UnitsApplied() - LastQuantity[Prd.ID];
                                    LastQuantity[Prd.ID] = Prd.UnitsApplied();
                                    if (Applied > 0) PD.Quantity += Applied;

                                    // hectares
                                    PD.Hectares += Prd.HectaresWorked();

                                    string ID = Prd.ID.ToString();
                                    string Start = PD.StartTime.ToString(DateFormat, CultureInfo.InvariantCulture);
                                    string End = PD.EndTime.ToString(DateFormat, CultureInfo.InvariantCulture);
                                    string Quantity = PD.Quantity.ToString("F4", CultureInfo.InvariantCulture);
                                    string Hectares = PD.Hectares.ToString("F4", CultureInfo.InvariantCulture);

                                    string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3},{4}", ID, Start, End, Quantity, Hectares);
                                    writer.WriteLine(csvLine);
                                }
                            }
                        }
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
            RecordData();
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