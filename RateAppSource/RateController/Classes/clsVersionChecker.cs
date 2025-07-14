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
                    var doc = JsonDocument.Parse(json);
                    Dictionary<int, ModuleInfo> ModulesUpdate = new Dictionary<int, ModuleInfo>();
                    RCappInfo RCappUpdate = null;

                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        if (property.Name == "RCapp")
                        {
                            RCappUpdate = JsonSerializer.Deserialize<RCappInfo>(property.Value.GetRawText());
                        }
                        else
                        {
                            if (int.TryParse(property.Name, out int key))
                            {
                                var info = JsonSerializer.Deserialize<ModuleInfo>(property.Value.GetRawText());
                                ModulesUpdate.Add(key, info);
                            }
                        }
                    }

                    RCapp = RCappUpdate;
                    Modules = ModulesUpdate;

                    var saveObj = new
                    {
                        RCapp,
                        Modules
                    };

                    string savedJson = JsonSerializer.Serialize(saveObj, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(FileLocation, savedJson);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
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
                        RCapp = JsonSerializer.Deserialize<RCappInfo>(property.Value.GetRawText());
                    }
                    else if (property.Name == "Modules")
                    {
                        foreach (var moduleProp in property.Value.EnumerateObject())
                        {
                            if (int.TryParse(moduleProp.Name, out int key))
                            {
                                var info = JsonSerializer.Deserialize<ModuleInfo>(moduleProp.Value.GetRawText());
                                Modules[key] = info;
                            }
                        }
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