using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace RateController.Classes
{
    public class Job
    {
        private static readonly object _syncLock = new object();

        private static readonly TimeSpan CacheValidity = TimeSpan.FromMinutes(5);
        private static List<Job> _cachedJobs;
        private static DateTime _lastCacheUpdate;

        private static string FilePath = Props.JobsDataPath;

        public DateTime Date { get; set; }
        public int FieldID { get; set; }
        public int ID { get; private set; }

        public string JobFolder
        { get { return Path.GetDirectoryName(FilePath) + "\\Job_" + ID.ToString(); } }

        public string Name { get; set; }
        public string Notes { get; set; }

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

                // Create a new job instance copying properties from the source.
                Job newJob = new Job
                {
                    Date = DateTime.Now,
                    FieldID = sourceJob.FieldID,
                    Name = sourceJob.Name + "_Copy",  // Optionally append "Copy" to the name.
                    Notes = sourceJob.Notes
                };

                // Add the new job (this assigns a new ID and creates a new folder structure).
                AddJob(newJob);

                // Copy folder structure data from the source job folder to the new job folder.
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
                var jobs = GetJobs();
                Job jobToRemove = jobs.FirstOrDefault(j => j.ID == id);
                if (jobToRemove == null)
                    return false;

                jobs.Remove(jobToRemove);
                SaveJobsToFile(jobs);
                return true;
            }
        }

        public static List<Job> FilterByDate(DateTime startDate, DateTime endDate)
        {
            lock (_syncLock)
            {
                return GetJobs().Where(job => job.Date >= startDate && job.Date <= endDate).ToList();
            }
        }

        public static List<Job> FilterByFieldID(int fieldID)
        {
            lock (_syncLock)
            {
                return GetJobs().Where(job => job.FieldID == fieldID).ToList();
            }
        }

        public static List<Job> GetJobs()
        {
            lock (_syncLock)
            {
                // If no cache or cache is expired, reload from file
                if (_cachedJobs == null || DateTime.Now - _lastCacheUpdate > CacheValidity)
                {
                    _cachedJobs = LoadJobsFromFile();
                    _lastCacheUpdate = DateTime.Now;
                }
                return _cachedJobs;
            }
        }

        public static Job SearchJob(int id)
        {
            lock (_syncLock)
            {
                return GetJobs().FirstOrDefault(job => job.ID == id);
            }
        }

        public static List<Job> SortByDate(bool ascending = true)
        {
            lock (_syncLock)
            {
                var jobs = GetJobs();
                return ascending ? jobs.OrderBy(job => job.Date).ToList() : jobs.OrderByDescending(job => job.Date).ToList();
            }
        }

        public static List<Job> SortByName(bool ascending = true)
        {
            lock (_syncLock)
            {
                var jobs = GetJobs();
                return ascending ? jobs.OrderBy(job => job.Name).ToList() : jobs.OrderByDescending(job => job.Name).ToList();
            }
        }

        public static bool UpdateJob(Job updatedJob)
        {
            lock (_syncLock)
            {
                var jobs = GetJobs();
                int index = jobs.FindIndex(j => j.ID == updatedJob.ID);
                if (index == -1)
                    return false;

                jobs[index] = updatedJob; // Update in-memory
                SaveJobsToFile(jobs);     // Persist and re-sync cache
                return true;
            }
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            // Create destination directory if it does not exist.
            if (!Directory.Exists(destinationDir))
            {
                Directory.CreateDirectory(destinationDir);
            }

            // Copy each file.
            foreach (string filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFile = Path.Combine(destinationDir, fileName);
                File.Copy(filePath, destFile, true);
            }

            // Recursively copy subdirectories.
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
                string baseDir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(baseDir))
                {
                    string jobFolderName = $"Job_{job.ID}";
                    string jobFolderPath = Path.Combine(baseDir, jobFolderName);

                    if (!Directory.Exists(jobFolderPath))
                    {
                        Directory.CreateDirectory(jobFolderPath);
                    }

                    string rateDataFilePath = Path.Combine(jobFolderPath, "RateData.csv");
                    if (!File.Exists(rateDataFilePath))
                    {
                        File.WriteAllText(rateDataFilePath, string.Empty);
                    }

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
                    Props.WriteErrorLog("clsJobs/CreateJobFolderStructure: No base directory.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobs/CreateJobFolderStructure: " + ex.Message);
            }
        }

        private static List<Job> LoadJobsFromFile()
        {
            if (!File.Exists(FilePath))
                return new List<Job>();

            try
            {
                string json = File.ReadAllText(FilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<Job>();

                return JsonSerializer.Deserialize<List<Job>>(json) ?? new List<Job>();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobs/LoadJobsFromFile: " + ex.Message);
                return new List<Job>();
            }
        }

        private static void SaveJobsToFile(List<Job> jobs)
        {
            try
            {
                string json = JsonSerializer.Serialize(jobs, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FilePath, json);

                // Update the cache and cache timestamp in a write-through manner
                _cachedJobs = jobs;
                _lastCacheUpdate = DateTime.Now;
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("clsJobs/SaveJobsToFile: " + ex.Message);
            }
        }
    }
}