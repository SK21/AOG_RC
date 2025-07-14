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
        private string FileLocation = Props.ApplicationFolder + "//versionCache.json";
        private Dictionary<int, ModuleInfo> Modules = new Dictionary<int, ModuleInfo>();
        private RCappInfo RCapp = null;

        public clsVersionChecker()
        {
            LoadFromFile(FileLocation);
        }

        public DateTime RCappDate => RCapp?.Date ?? DateTime.MinValue;

        public string RCappLatest => RCapp?.Version ?? "N/A";

        public DateTime ModuleDate(int ModuleID) =>
            Modules.TryGetValue(ModuleID, out var info) ? info.Date : DateTime.MinValue;

        public string ModuleDescription(int ModuleID) =>
            Modules.TryGetValue(ModuleID, out var info) ? info.Description : "Unknown";

        public async Task Update()
        {
            string url = "https://raw.githubusercontent.com/SK21/AOG_RC/Versions/Versions.json";

            using (var client = new HttpClient())
            {
                try
                {
                    string json = await client.GetStringAsync(url);

                    var jsonObj = JsonConvert.DeserializeObject<Dictionary<string, JObject>>(json);
                    if (jsonObj == null)
                        return;

                    var modulesUpdate = new Dictionary<int, ModuleInfo>();
                    RCappInfo rcappUpdate = null;

                    foreach (var kvp in jsonObj)
                    {
                        if (kvp.Key == "RCapp")
                        {
                            rcappUpdate = kvp.Value.ToObject<RCappInfo>();
                        }
                        else if (int.TryParse(kvp.Key, out int moduleId))
                        {
                            var module = kvp.Value.ToObject<ModuleInfo>();
                            modulesUpdate[moduleId] = module;
                        }
                    }

                    // Apply updates
                    RCapp = rcappUpdate;
                    Modules = modulesUpdate;

                    // Build flat structure for saving
                    var saveObj = new JObject();

                    saveObj["RCapp"] = JObject.FromObject(RCapp);

                    foreach (var kvp in Modules)
                    {
                        saveObj[kvp.Key.ToString()] = JObject.FromObject(kvp.Value);
                    }

                    // Write to file
                    File.WriteAllText(FileLocation, saveObj.ToString(Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Update failed: " + ex.Message);
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
            public DateTime Date { get; set; }
            public string Description { get; set; }
        }

        private class RCappInfo
        {
            public DateTime Date { get; set; }
            public string Version { get; set; }
        }
    }
}