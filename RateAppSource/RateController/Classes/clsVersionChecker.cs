using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace RateController.Classes
{
    public class clsVersionChecker
    {
        private string FileLocation = Props.ApplicationFolder + "//ModuleVersions.json";
        private Dictionary<int, ModuleInfo> Modules = new Dictionary<int, ModuleInfo>();
        private RCappInfo RCapp = null;
        private string VersionsURL = "https://github.com/SK21/AOG_RC/releases/latest/download/Versions.json";
        private FormStart mf;
        public clsVersionChecker(FormStart CalledFrom)
        {
            mf= CalledFrom;
            LoadFromFile(FileLocation);
        }

        public DateTime RCappDate => RCapp?.Date ?? DateTime.MinValue;

        public string RCappLatest => RCapp?.Version ?? "N/A";

        public string ModuleVersion(int ModuleID) =>
            Modules.TryGetValue(ModuleID, out var info) ? info.Version : "N/A";

        public string ModuleDescription(int ModuleID) =>
            Modules.TryGetValue(ModuleID, out var info) ? info.Description : "Unknown";
        public async Task Update()
        {
            using (var client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(VersionsURL);
                    File.WriteAllText(FileLocation, json);
                    LoadFromFile(FileLocation);
                }
                catch (Exception ex)
                {
                    mf.Tls.ShowMessage("Version update failed: " + ex.Message,"Help",5000);
                }
            }
        }


        private void LoadFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var doc = JsonDocument.Parse(json);

                RCapp = null;
                Modules.Clear();

                foreach (var property in doc.RootElement.EnumerateObject())
                {
                    if (property.Name == "RCapp")
                    {
                        RCapp = JsonConvert.DeserializeObject<RCappInfo>(property.Value.GetRawText());
                    }
                    else if (int.TryParse(property.Name, out int key))
                    {
                        var info = JsonConvert.DeserializeObject<ModuleInfo>(property.Value.GetRawText());
                        Modules[key] = info;
                    }
                }
            }
        }

        private class ModuleInfo
        {
            public string Version { get; set; }
            public string Description { get; set; }
        }

        private class RCappInfo
        {
            public DateTime Date { get; set; }
            public string Version { get; set; }
        }
    }
}