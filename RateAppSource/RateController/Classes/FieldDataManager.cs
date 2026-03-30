using RateController.Classes;
using System;
using System.Collections.Generic;
using System.IO;

namespace RateController.Classes
{
    public static class FieldDataManager
    {
        private static string cSelectedYieldPath;
        private static string cSelectedElevationPath;

        public static event EventHandler SelectionChanged;

        public static string SelectedYieldPath
        {
            get { return cSelectedYieldPath; }
        }

        public static string SelectedElevationPath
        {
            get { return cSelectedElevationPath; }
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
                cSelectedElevationPath = null;
            }
            else
            {
                // Default yield: first file in field's Yield folder
                List<string> yieldFiles = ParcelManager.GetYieldFiles(job.FieldID);
                if (yieldFiles.Count > 0)
                    cSelectedYieldPath = Path.Combine(ParcelManager.YieldFolder(job.FieldID), yieldFiles[0]);
                else
                    cSelectedYieldPath = null;

                // Default elevation: active elevation for the field
                cSelectedElevationPath = ParcelManager.ActiveElevationPath(job.FieldID);
            }

            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetYieldPath(string path)
        {
            cSelectedYieldPath = path;
            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        public static void SetElevationPath(string path)
        {
            cSelectedElevationPath = path;
            SelectionChanged?.Invoke(null, EventArgs.Empty);
        }

        private static void JobManager_JobChanged(object sender, EventArgs e)
        {
            LoadForCurrentJob();
        }
    }
}
