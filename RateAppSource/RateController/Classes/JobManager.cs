using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RateController.Classes
{
    public static class JobManager
    {
        private static readonly object SyncLock = new object();
        private static bool cJobFilter = true;
        private static string cJobsFolder;
        private static bool cShowJobs;
        private static string JobDataName = "JobData.txt";
        private static List<Job> JobsList;

        public static event EventHandler JobChanged;

        public static string CurrentJobDescription
        {
            get
            {
                Job current = SearchJob(CurrentJobID);
                string fld = "";
                Parcel currentParcel = ParcelManager.SearchParcel(current.FieldID);
                if (currentParcel != null && currentParcel.Name.Trim() != "") fld = " - " + currentParcel.Name;
                return current.Name + fld;
            }
        }

        public static int CurrentJobID
        {
            get
            {
                Job current = SearchJob(Properties.Settings.Default.CurrentJob);
                if (current == null)
                {
                    Properties.Settings.Default.CurrentJob = 0;
                    Properties.Settings.Default.Save();
                    current = SearchJob(0);
                }
                return Properties.Settings.Default.CurrentJob;
            }
            set
            {
                Properties.Settings.Default.CurrentJob = value;
                Properties.Settings.Default.Save();
                JobChanged?.Invoke(null, EventArgs.Empty);
            }
        }

        public static string CurrentMapPath
        {
            get
            {
                return MapPath(Properties.Settings.Default.CurrentJob);
            }
        }

        public static string CurrentRateDataPath
        {
            get
            {
                return RateDataPath(Properties.Settings.Default.CurrentJob);
            }
        }

        public static bool JobFilter
        {
            get { return cJobFilter; }
            set
            {
                cJobFilter = value;
                Props.SetAppProp("StickyJobFilter", cJobFilter.ToString());
            }
        }

        public static string JobsFolder
        { get { return cJobsFolder; } }

        public static bool ShowJobs
        {
            get { return cShowJobs; }
            set
            {
                cShowJobs = value;
                Props.SetAppProp("ShowJobs", cShowJobs.ToString());
            }
        }

        public static void AddJob(Job newJob)
        {
            lock (SyncLock)
            {
                List<Job> jobs = GetJobsList();
                newJob.ID = jobs.Any() ? jobs.Max(j => j.ID) + 1 : 0;
                JobsList.Add(newJob);
                SaveJob(newJob);
            }
        }

        public static void CheckDefaultJob()
        {
            lock (SyncLock)
            {
                Job defaultJob = SearchJob(0);
                if (defaultJob == null)
                {
                    defaultJob = new Job
                    {
                        ID = 0,
                        Date = DateTime.Now,
                        FieldID = -1,
                        Name = "Default Job",
                        Notes = ""
                    };
                    List<Job> jobs = GetJobsList();
                    JobsList.Add(defaultJob);
                    SaveJob(defaultJob);
                }
            }
        }

        public static bool CopyJobData(int FromID, int ToID, bool EraseRateData = true)
        {
            lock (SyncLock)
            {
                bool Result = false;
                try
                {
                    Job fromJob = SearchJob(FromID);
                    Job toJob = SearchJob(ToID);
                    if (fromJob != null && toJob != null)
                    {
                        string fromFolder = fromJob.JobFolder;
                        string toFolder = toJob.JobFolder;
                        if (Directory.Exists(fromFolder))
                        {
                            if (CopyJob(fromFolder, toFolder))
                            {
                                if (EraseRateData)
                                {
                                    // erase RateData.csv by overwriting it with an empty string
                                    string rateDataFilePath = Path.Combine(toFolder, "RateData.csv");
                                    File.WriteAllText(rateDataFilePath, string.Empty);
                                }

                                Result = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("JobManager/CopyJobData: " + ex.Message);
                }
                return Result;
            }
        }

        public static bool DeleteJob(int id)
        {
            lock (SyncLock)
            {
                bool result = false;
                try
                {
                    List<Job> jobs = GetJobsList();
                    Job jobToRemove = jobs.FirstOrDefault(j => j.ID == id);
                    if (jobToRemove != null)
                    {
                        string jobFolderPath = jobToRemove.JobFolder;
                        if (Props.IsPathSafe(jobFolderPath))
                        {
                            if (Directory.Exists(jobFolderPath))
                            {
                                Directory.Delete(jobFolderPath, true);
                            }
                            JobsList = null;
                            result = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("JobManager/DeleteJob: " + ex.Message);
                }
                return result;
            }
        }

        public static List<Job> FilterJobs(DateTime? startDate = null, DateTime? endDate = null, int? fieldID = null)
        {
            lock (SyncLock)
            {
                IEnumerable<Job> filteredJobs = GetJobsList().ToList();

                if (startDate.HasValue && endDate.HasValue)
                {
                    filteredJobs = filteredJobs.Where(job => job.Date >= startDate.Value && job.Date <= endDate.Value).ToList();
                }

                if (fieldID.HasValue)
                {
                    filteredJobs = filteredJobs.Where(job => job.FieldID == fieldID.Value).ToList();
                }

                return filteredJobs.ToList();
            }
        }

        public static List<Job> GetJobsList()
        {
            lock (SyncLock)
            {
                try
                {
                    if (JobsList == null)
                    {
                        JobsList = new List<Job>();
                        if (!Directory.Exists(JobManager.JobsFolder)) Props.CheckFolders();

                        foreach (string dir in Directory.GetDirectories(cJobsFolder, "Job_*", SearchOption.TopDirectoryOnly))
                        {
                            Job NewJob = LoadJob(dir);

                            if (NewJob != null) JobsList.Add(NewJob);
                        }

                        JobsList = JobsList.OrderBy(j => j.ID).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("JobManager/GetJobs: " + ex.Message);
                }
            }
            return JobsList;
        }

        public static void Initialize()
        {
            // jobs folder
            string name = Props.DefaultDir + "\\Jobs";
            if (!Directory.Exists(name)) Directory.CreateDirectory(name);
            cJobsFolder = name;

            // check for default job
            CheckDefaultJob();

            // check job folder structure
            List<Job> jobs = GetJobsList();
            foreach (Job job in jobs)
            {
                CheckFolderStructure(job);
            }

            // check user files, current job
            int CurrentJob = CurrentJobID;
            if (JobManager.SearchJob(CurrentJob) == null)
            {
                CurrentJobID = 0;
            }
            else
            {
                CurrentJobID = Properties.Settings.Default.CurrentJob;
            }

            // settings
            cShowJobs = bool.TryParse(Props.GetAppProp("ShowJobs"), out bool ja) ? ja : false;
            cJobFilter = bool.TryParse(Props.GetAppProp("StickyJobFilter"), out bool jf) ? jf : true;
        }

        public static bool IsFieldIDUsed(int fieldID)
        {
            lock (SyncLock)
            {
                return GetJobsList().Any(job => job.FieldID == fieldID);
            }
        }

        public static Job LoadJob(string JobFolderPath)
        {
            Job Result = null;
            String FolderName = Path.GetFileName(JobFolderPath.Trim(Path.DirectorySeparatorChar));
            try
            {
                if (FolderName.StartsWith("Job_", StringComparison.OrdinalIgnoreCase))
                {
                    string FileName = Path.Combine(JobFolderPath, JobDataName);
                    if (File.Exists(FileName))
                    {
                        string Json = File.ReadAllText(FileName);
                        if (!string.IsNullOrWhiteSpace(Json)) Result = JsonSerializer.Deserialize<Job>(Json);
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/GetJob: " + ex.Message);
            }

            if (Result == null)
            {
                try
                {
                    // recover missing job data
                    string idPart = FolderName.Substring(4);
                    if (int.TryParse(idPart, out int id))
                    {
                        Job RecoveredJob = new Job
                        {
                            Date = Directory.GetCreationTime(JobFolderPath),
                            FieldID = -1,
                            ID = id,
                            Name = "Recovered Job " + id,
                            Notes = "Recovered from existing job folder."
                        };

                        // save job data
                        if (SaveJob(RecoveredJob)) Result = RecoveredJob;
                    }
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("JobManager/GetJob: " + ex.Message);
                    throw;
                }
            }
            return Result;
        }

        public static string MapPath(int JobID)
        {
            string Result = null;
            Job JB = SearchJob(JobID);
            if (JB != null) Result = Path.Combine(JB.JobFolder, "Map\\Job.shp");
            return Result;
        }

        public static string RateDataPath(int JobID)
        {
            string Result = null;
            Job JB = SearchJob(JobID);
            if (JB != null) Result = Path.Combine(JB.JobFolder, "RateData.csv");
            return Result;
        }

        public static bool SaveJob(Job NewJob)
        {
            bool Result = false;
            try
            {
                CheckFolderStructure(NewJob);
                string JobPath = Path.Combine(cJobsFolder, "Job_" + NewJob.ID.ToString());
                JobPath = Path.Combine(JobPath, JobDataName);

                string json = JsonSerializer.Serialize(NewJob, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(JobPath, json);
                Result = true;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/SaveJob: " + ex.Message);
            }
            return Result;
        }

        public static Job SearchJob(int JobID)
        {
            lock (SyncLock)
            {
                return GetJobsList().FirstOrDefault(j => j.ID == JobID);
            }
        }

        private static void CheckFolderStructure(Job job)
        {
            try
            {
                string baseDir = cJobsFolder;
                if (!string.IsNullOrEmpty(baseDir))
                {
                    string jobFolderName = $"Job_{job.ID}";
                    string jobFolderPath = Path.Combine(baseDir, jobFolderName);
                    if (!Directory.Exists(jobFolderPath))
                    {
                        Directory.CreateDirectory(jobFolderPath);
                    }
                    // Create a default rate data file.
                    string rateDataFilePath = Path.Combine(jobFolderPath, "RateData.csv");
                    if (!File.Exists(rateDataFilePath))
                    {
                        File.WriteAllText(rateDataFilePath, string.Empty);
                    }
                    // Create Map folder and placeholder files.
                    string mapFolderPath = Path.Combine(jobFolderPath, "Map");
                    if (!Directory.Exists(mapFolderPath))
                    {
                        Directory.CreateDirectory(mapFolderPath);
                    }
                    string[] mapFiles = new string[]
                    {
                        $"Job.cpg",
                        $"Job.dbf",
                        $"Job.shp"
                    };
                    foreach (string fileName in mapFiles)
                    {
                        string filePath = Path.Combine(mapFolderPath, fileName);
                        if (!File.Exists(filePath))
                        {
                            File.WriteAllText(filePath, string.Empty);
                        }
                    }

                    string JobData = Path.Combine(jobFolderPath, JobDataName);
                    if (!File.Exists(JobData)) File.WriteAllText(JobData, string.Empty);
                }
                else
                {
                    Props.WriteErrorLog("JobManager/CheckFolderStructure: No base directory.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/CheckFolderStructure: " + ex.Message);
            }
        }

        private static bool CopyJob(string sourceDir, string destinationDir)
        {
            bool Result = false;
            try
            {
                if (Props.IsPathSafe(destinationDir))
                {
                    foreach (string filePath in Directory.GetFiles(sourceDir))
                    {
                        string fileName = Path.GetFileName(filePath);
                        if (!fileName.Equals(JobDataName, StringComparison.OrdinalIgnoreCase))
                        {
                            string destFilePath = Path.Combine(destinationDir, fileName);
                            File.Copy(filePath, destFilePath, overwrite: true);
                        }
                    }

                    // Recursively copy subdirectories
                    foreach (string subDir in Directory.GetDirectories(sourceDir))
                    {
                        string subDirName = Path.GetFileName(subDir);
                        string destSubDir = Path.Combine(destinationDir, subDirName);
                        CopyJob(subDir, destSubDir);
                    }
                    Result = true;
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/CopyJob: " + ex.Message);
            }
            return Result;
        }
    }

    public class Job
    {
        public DateTime Date { get; set; }
        public string DisplayName => $"{Name.PadRight(15)} {Date:dd-MMM}";
        public int FieldID { get; set; }
        public int ID { get; set; }
        public string JobFolder => Path.Combine(JobManager.JobsFolder, $"Job_{ID}");
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}