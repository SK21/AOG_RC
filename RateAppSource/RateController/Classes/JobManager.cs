using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RateController.Classes
{
    public static class JobManager
    {
        private static readonly object _syncLock = new object();
        private static readonly TimeSpan CacheValidity = TimeSpan.FromMinutes(5);
        private static List<Job> _cachedJobs;
        private static DateTime _lastCacheUpdate;

        public static void AddJob(Job newJob)
        {
            lock (_syncLock)
            {
                List<Job> jobs = GetJobs();
                newJob.ID = jobs.Any() ? jobs.Max(j => j.ID) + 1 : 0;
                jobs.Add(newJob);
                SaveJobsToFile(jobs);
                CreateJobFolderStructure(newJob);
            }
        }

        public static Job CopyJob(int sourceJobId)
        {
            lock (_syncLock)
            {
                // Locate the source job.
                Job sourceJob = SearchJob(sourceJobId);
                if (sourceJob == null)
                    return null;

                // Create a new instance copying properties from the source.
                Job newJob = new Job
                {
                    Date = DateTime.Now,
                    FieldID = sourceJob.FieldID,
                    Name = sourceJob.Name + "_Copy",
                    Notes = sourceJob.Notes
                };

                // Add the new job (this assigns a new ID and creates the folder structure).
                AddJob(newJob);

                // Copy folder data from the source job folder to the new job folder.
                string sourceFolder = sourceJob.JobFolder;
                string destinationFolder = newJob.JobFolder;
                if (Directory.Exists(sourceFolder))
                {
                    CopyDirectory(sourceFolder, destinationFolder);
                }
                return newJob;
            }
        }

        public static bool DeleteJob(int id)
        {
            lock (_syncLock)
            {
                bool result = false;
                try
                {
                    List<Job> jobs = GetJobs();
                    Job jobToRemove = jobs.FirstOrDefault(j => j.ID == id);
                    if (jobToRemove != null)
                    {
                        string jobFolderPath = jobToRemove.JobFolder;
                        if (Props.IsPathSafeToDelete(jobFolderPath))
                        {
                            if (Directory.Exists(jobFolderPath))
                            {
                                Directory.Delete(jobFolderPath, true);
                            }
                            jobs.Remove(jobToRemove);
                            SaveJobsToFile(jobs);
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

        public static bool EditJob(Job updatedJob)
        {
            lock (_syncLock)
            {
                List<Job> jobs = GetJobs();
                int index = jobs.FindIndex(j => j.ID == updatedJob.ID);
                if (index == -1)
                    return false;
                jobs[index] = updatedJob;
                SaveJobsToFile(jobs);
                return true;
            }
        }

        public static List<Job> FilterJobs(DateTime? startDate = null, DateTime? endDate = null, int? fieldID = null)
        {
            lock (_syncLock)
            {
                IEnumerable<Job> filteredJobs = GetJobs();

                if (startDate.HasValue && endDate.HasValue)
                {
                    filteredJobs = filteredJobs.Where(job => job.Date >= startDate.Value && job.Date <= endDate.Value);
                }

                if (fieldID.HasValue)
                {
                    filteredJobs = filteredJobs.Where(job => job.FieldID == fieldID.Value);
                }

                return filteredJobs.ToList();
            }
        }

        public static List<Job> GetJobs()
        {
            lock (_syncLock)
            {
                // If no cache or the cache is expired, reload from file.
                if (_cachedJobs == null || DateTime.Now - _lastCacheUpdate > CacheValidity)
                {
                    _cachedJobs = LoadJobsFromFile();
                    _lastCacheUpdate = DateTime.Now;
                }
                return _cachedJobs;
            }
        }

        public static bool IsFieldIDUsed(int fieldID)
        {
            lock (_syncLock)
            {
                return GetJobs().Any(job => job.FieldID == fieldID);
            }
        }

        public static Job SearchJob(int id)
        {
            lock (_syncLock)
            {
                return GetJobs().FirstOrDefault(j => j.ID == id);
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFile = Path.Combine(destinationDir, fileName);
                File.Copy(filePath, destFile, true);
            }
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destinationDir, dirName);
                CopyDirectory(subDir, destSubDir);
            }
        }

        private static void CreateJobFolderStructure(Job job)
        {
            try
            {
                string baseDir = Path.GetDirectoryName(Props.JobsDataPath);
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
                        $"Job_{job.ID}.cpg",
                        $"Job_{job.ID}.dbf",
                        $"Job_{job.ID}.shp"
                    };
                    foreach (string fileName in mapFiles)
                    {
                        string filePath = Path.Combine(mapFolderPath, fileName);
                        if (!File.Exists(filePath))
                        {
                            File.WriteAllText(filePath, string.Empty);
                        }
                    }
                }
                else
                {
                    Props.WriteErrorLog("JobManager/CreateJobFolderStructure: No base directory.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/CreateJobFolderStructure: " + ex.Message);
            }
        }

        private static List<Job> LoadJobsFromFile()
        {
            if (!File.Exists(Props.JobsDataPath))
                return new List<Job>();

            try
            {
                string json = File.ReadAllText(Props.JobsDataPath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Job>();

                return JsonSerializer.Deserialize<List<Job>>(json) ?? new List<Job>();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/LoadJobsFromFile: " + ex.Message);
                return new List<Job>();
            }
        }

        private static void SaveJobsToFile(List<Job> jobs)
        {
            try
            {
                string json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(Props.JobsDataPath, json);
                _cachedJobs = jobs;
                _lastCacheUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("JobManager/SaveJobsToFile: " + ex.Message);
            }
        }
    }

    public class Job
    {
        public DateTime Date { get; set; }
        public int FieldID { get; set; }
        public int ID { get; set; }
        public string JobFolder => Path.Combine(Path.GetDirectoryName(Props.JobsDataPath), $"Job_{ID}");
        public string Name { get; set; }
        public string Notes { get; set; }
    }
}