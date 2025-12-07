using System;
using System.Globalization;
using System.IO;

namespace RateController.Classes
{
    public class clsJobDataCollector
    {
        // JobStats.CSV in the job folder
        // date/time,  product ID, Quantity applied, hectares worked
        // each entry is updated every minute

        private string DataFileName = "JobStats.CSV";
        private double[] LastQuantity;
        private System.Windows.Forms.Timer RecordTimer;

        public clsJobDataCollector()
        {
            LastQuantity = new double[Props.MaxProducts];
            RecordTimer = new System.Windows.Forms.Timer();
            RecordTimer.Interval = 1000;   // milliseconds
            RecordTimer.Tick += RecordTimer_Tick;
            RecordTimer.Enabled = false;
        }

        public bool Record
        {
            get { return RecordTimer.Enabled; }
            set { RecordTimer.Enabled = value; }
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
                        bool FileFound = File.Exists(FileLocation);

                        using (var fs = new FileStream(FileLocation, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                        using (var writer = new StreamWriter(fs))
                        {
                            writer.AutoFlush = true; 
                            
                            if (!FileFound)
                            {
                                string header = "TimeStamp,ProductID,Quantity,Hectares";
                                writer.WriteLine(header);
                            }

                            foreach (clsProduct Prd in Props.MainForm.Products.Items)
                            {
                                if (Prd.ControlType != ControlTypeEnum.Fan && Prd.ProductOn(false))
                                {
                                    string timestamp = SaveTime.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);
                                    string ID = Prd.ID.ToString();

                                    // quantity
                                    double Applied = Prd.UnitsApplied() - LastQuantity[Prd.ID];
                                    LastQuantity[Prd.ID] = Prd.UnitsApplied();
                                    string Quantity = "0.00";
                                    if (Applied > 0)
                                    {
                                        Quantity = Applied.ToString("F4", CultureInfo.InvariantCulture);
                                    }

                                    // hectares
                                    string Worked = Prd.HectaresWorked().ToString("F4", CultureInfo.InvariantCulture);

                                    string csvLine = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", timestamp, ID, Quantity, Worked);
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
}