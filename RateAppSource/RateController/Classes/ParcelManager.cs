using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RateController.Classes
{
    public static class ParcelManager
    {
        private static readonly string FilePath = Props.FieldNamesPath;

        public static void AddParcel(Parcel NewParcel)
        {
            var mappings = GetParcels();
            NewParcel.ID = mappings.Any() ? mappings.Max(m => m.ID) + 1 : 0;
            mappings.Add(NewParcel);
            SaveParcels(mappings);
            EnsureFieldFolders(NewParcel.ID);
        }

        public static bool DeleteParcel(int FieldID, out bool InUse)
        {
            bool Result = false;
            InUse = false;
            try
            {
                if (JobManager.IsFieldIDUsed(FieldID))
                {
                    InUse = true;
                }
                else
                {
                    var mappings = GetParcels();
                    var mappingToRemove = mappings.FirstOrDefault(m => m.ID == FieldID);
                    if (mappingToRemove != null)
                    {
                        mappings.Remove(mappingToRemove);
                        SaveParcels(mappings);
                        Result = true;
                        GetDefaultParcel();
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("ParcelManager/DeleteParcel: " + ex.Message);
            }
            return Result;
        }

        public static bool EditParcel(Parcel UpdatedParcel)
        {
            var mappings = GetParcels();
            var mapping = mappings.FirstOrDefault(m => m.ID == UpdatedParcel.ID);
            if (mapping != null)
            {
                mapping.Name = UpdatedParcel.Name;
                SaveParcels(mappings);
                return true;
            }
            return false;
        }

        public static string ElevationFolder(int id) => Path.Combine(FieldFolder(id), "Elevation");

        public static string ElevationPath(int id)
        {
            string path = Path.Combine(ElevationFolder(id), "Elevation.csv");
            return File.Exists(path) ? path : null;
        }

        public static void EnsureFieldFolders(int id)
        {
            foreach (string dir in new[]
            {
                FieldFolder(id), MapsFolder(id), YieldFolder(id),
                ElevationFolder(id), KmlFolder(id)
            })
            {
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
        }

        public static string FieldFolder(int id) => Path.Combine(Props.DefaultDir, "Fields", $"Field_{id}");

        public static List<Parcel> GetParcels()
        {
            if (File.Exists(FilePath))
            {
                try
                {
                    string json = File.ReadAllText(FilePath);
                    var mappings = JsonConvert.DeserializeObject<List<Parcel>>(json) ?? new List<Parcel>();
                    return mappings.Where(p => !string.IsNullOrWhiteSpace(p.Name)).OrderBy(p => p.Name).ToList();
                }
                catch (Exception ex)
                {
                    Props.WriteErrorLog("Error reading file: " + ex.Message);
                    return new List<Parcel>();
                }
            }
            else
            {
                return new List<Parcel>();
            }
        }

        public static List<string> GetPrescriptionFiles(int id) => GetFilenames(MapsFolder(id), "*.shp");

        public static List<string> GetYieldFiles(int id) => GetFilenames(YieldFolder(id), "*.csv");

        public static void Initialize()
        {
            GetDefaultParcel();
        }

        public static string KmlFolder(int id) => Path.Combine(FieldFolder(id), "Kml");

        public static string MapsFolder(int id) =>
                                                    Path.Combine(FieldFolder(id), "Maps");

        public static void SaveParcels(List<Parcel> mappings)
        {
            try
            {
                var cleaned = mappings.Where(p => !string.IsNullOrWhiteSpace(p.Name)).ToList();
                string json = JsonConvert.SerializeObject(cleaned, Formatting.Indented);
                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("Error saving file: " + ex.Message);
            }
        }

        public static Parcel SearchParcel(int ID)
        {
            return GetParcels().FirstOrDefault(p => p.ID == ID);
        }

        public static string YieldFolder(int id) => Path.Combine(FieldFolder(id), "Yield");

        private static bool GetDefaultParcel()
        {
            bool Result = false;
            var Flds = GetParcels();
            if (Flds.FirstOrDefault(m => m.ID == 0) == null)
            {
                Parcel DefaultParcel = new Parcel();
                DefaultParcel.ID = 0;
                DefaultParcel.Name = "Default";
                Flds.Add(DefaultParcel);
                SaveParcels(Flds);
                EnsureFieldFolders(DefaultParcel.ID);
                Result = true;
            }

            return Result;
        }

        private static List<string> GetFilenames(string folder, string pattern)
        {
            if (!Directory.Exists(folder)) return new List<string>();
            return Directory.GetFiles(folder, pattern)
                .Select(Path.GetFileName)
                .OrderByDescending(f => f)
                .ToList();
        }
    }

    public class Parcel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}