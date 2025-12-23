using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsVersionChecker
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private string FileLocation;
        private Dictionary<int, ModuleInfo> Modules = new Dictionary<int, ModuleInfo>();
        private RCappInfo RCapp = null;
        private string VersionsURL = "https://github.com/SK21/AOG_RC/releases/latest/download/Versions.json";

        public clsVersionChecker()
        {
            FileLocation = Props.ApplicationFolder + "//ModuleVersions.json";
            LoadFromFile(FileLocation);
        }

        public DateTime RCappDate => RCapp?.Date ?? DateTime.MinValue;

        public string RCappLatest => RCapp?.Version ?? "N/A";

        public async Task<bool> HasVersionChanged()
        {
            bool Result = true;
            try
            {
                string NewJson = await _httpClient.GetStringAsync(VersionsURL);

                if (File.Exists(FileLocation))
                {
                    string ExistingJson = File.ReadAllText(FileLocation);
                    Result = (ExistingJson != NewJson);
                }

                if (Result)
                {
                    File.WriteAllText(FileLocation, NewJson);
                    LoadFromFile(FileLocation);
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage("Version update failed: " + ex.Message);
            }
            return Result;
        }

        public string ModuleDescription(int ModuleID) =>
                    Modules.TryGetValue(ModuleID, out var info) ? info.Description : "Unknown";

        public string ModuleVersion(int ModuleID)
        {
            if (!Modules.TryGetValue(ModuleID, out var info) || string.IsNullOrWhiteSpace(info.Version))
                return "N/A";

            var version = info.Version.TrimStart('v');
            var parts = version.Split('.');
            if (parts.Length != 3) return info.Version;

            if (int.TryParse(parts[0], out int year) &&
                int.TryParse(parts[1], out int month) &&
                int.TryParse(parts[2], out int day))
            {
                return $"v{year:D4}.{month:D2}.{day:D2}";
            }

            return info.Version;
        }

        private void LoadFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var jObject = JObject.Parse(json);

                    RCapp = null;
                    Modules.Clear();

                    foreach (var property in jObject.Properties())
                    {
                        if (property.Name == "RCapp")
                        {
                            RCapp = property.Value.ToObject<RCappInfo>();
                        }
                        else if (int.TryParse(property.Name, out int key))
                        {
                            var info = property.Value.ToObject<ModuleInfo>();
                            if (info != null) Modules[key] = info;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Props.ShowMessage("Could not load version information: " + ex.Message, "Help", 5000);
            }
        }

        private class ModuleInfo
        {
            public string Description { get; set; }
            public string Version { get; set; }
        }

        private class RCappInfo
        {
            public DateTime Date { get; set; }
            public string Version { get; set; }
        }
    }
}