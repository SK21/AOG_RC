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
        private FormStart mf;
        private Dictionary<int, ModuleInfo> Modules = new Dictionary<int, ModuleInfo>();
        private RCappInfo RCapp = null;
        private string VersionsURL = "https://github.com/SK21/AOG_RC/releases/latest/download/Versions.json";

        public clsVersionChecker(FormStart CalledFrom)
        {
            mf = CalledFrom;
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
                mf.Tls.ShowMessage("Version update failed: " + ex.Message);
            }
            return Result;
        }

        public string ModuleDescription(int ModuleID) =>
                    Modules.TryGetValue(ModuleID, out var info) ? info.Description : "Unknown";

        public string ModuleVersion(int ModuleID) =>
                            Modules.TryGetValue(ModuleID, out var info) ? info.Version : "N/A";

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
                mf.Tls.ShowMessage("Could not load version information: " + ex.Message, "Help", 5000);
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