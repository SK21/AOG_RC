using System;
using System.Collections.Generic;
using System.IO;

namespace RateController.Classes
{
    public static class FieldDataManager
    {
        private static string cSelectedYieldPath;

        public static event EventHandler SelectionChanged;

        public static string ElevationPath
        {
            get
            {
                Job job = JobManager.CurrentJob;
                if (job == null || job.FieldID < 0) return null;
                return ParcelManager.ElevationPath(job.FieldID);
            }
        }

        public static string SelectedYieldPath
        {
            get { return cSelectedYieldPath; }
        }

        public static void Initialize()
        {
            JobManager.JobChanged += JobManager_JobChanged;
            LoadForCurrentJob();
        }

        public static void LoadForCurrentJob()
        {
            Job job = JobManager.CurrentJob;
            if (job == null || job.FieldID < 0)
            {
                cSelectedYieldPath = null;
            }
            else
            {
                // Default yield: first file in field's Yield folder
                List<string> yieldFiles = ParcelManager.GetYieldFiles(job.FieldID);
                if (yieldFiles.Count > 0)
                {
                    cSelectedYieldPath = Path.Combine(ParcelManager.YieldFolder(job.FieldID), yieldFiles[0]);
                }
                else
                {
                    cSelectedYieldPath = null;
                }
            }

            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetYieldPath(string path)
        {
            cSelectedYieldPath = path;
            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadForCurrentJob();
        }
    }
}