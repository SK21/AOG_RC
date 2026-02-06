using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
    }

    public class Parcel
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}