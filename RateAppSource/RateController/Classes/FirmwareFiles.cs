using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateController.Classes
{
    // Module firmware files from the latest GitHub release, fetched the way PCBsetup does:
    // ModulesHex.zip unpacked to <Application folder>\Firmware\ModulesHex. Only on the update
    // page's button, never by itself. The Teensy file's date (Versions.json key 1 when it was
    // downloaded) is kept next to it.
    public static class FirmwareFiles
    {
        private const string HexFilesURL = "https://github.com/SK21/AOG_RC/releases/latest/download/ModulesHex.zip";
        private static readonly HttpClient cHttp = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        public static string Folder => Path.Combine(Props.ApplicationFolder, "Firmware");

        public static string TeensyHex => Path.Combine(Folder, "ModulesHex", "RCteensy.ino.hex");

        private static string VersionFile => Path.Combine(Folder, "RCteensyVersion.txt");

        // "v2026.11.14" as in Versions.json, "" when unknown
        public static string TeensyVersion()
        {
            string Result = "";
            try
            {
                if (File.Exists(VersionFile) && File.Exists(TeensyHex)) Result = File.ReadAllText(VersionFile).Trim();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareFiles/TeensyVersion: " + ex.Message);
            }
            return Result;
        }

        // "v2026.11.14" -> 20261114, 0 when it isn't a date
        public static int VersionDate(string Version)
        {
            int Result = 0;
            string[] P = (Version ?? "").TrimStart('v', 'V').Split('.');
            if (P.Length == 3 && int.TryParse(P[0], out int Y) && int.TryParse(P[1], out int M) && int.TryParse(P[2], out int D))
            {
                Result = Y * 10000 + M * 100 + D;
            }
            return Result;
        }

        // downloads and unpacks ModulesHex.zip; Version is saved as the Teensy file's date
        public static async Task<bool> Download(string Version)
        {
            bool Result = false;
            string Zip = Path.Combine(Folder, "ModulesHex.zip");
            try
            {
                Directory.CreateDirectory(Folder);
                using (HttpResponseMessage Response = await cHttp.GetAsync(HexFilesURL, HttpCompletionOption.ResponseHeadersRead))
                {
                    Response.EnsureSuccessStatusCode();
                    using (Stream From = await Response.Content.ReadAsStreamAsync())
                    using (FileStream To = new FileStream(Zip, FileMode.Create, FileAccess.Write))
                    {
                        await From.CopyToAsync(To);
                    }
                }

                string HexFolder = Path.Combine(Folder, "ModulesHex");
                if (Directory.Exists(HexFolder)) Directory.Delete(HexFolder, true);
                Unzip(Zip, Folder);
                File.WriteAllText(VersionFile, Version ?? "");
                Result = File.Exists(TeensyHex);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareFiles/Download: " + ex.Message);
            }

            try
            {
                if (File.Exists(Zip)) File.Delete(Zip);
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareFiles/Download delete: " + ex.Message);
            }
            return Result;
        }

        // every entry must land inside the target folder
        private static void Unzip(string Zip, string Target)
        {
            string Root = Path.GetFullPath(Target + Path.DirectorySeparatorChar);
            using (FileStream Stream = File.OpenRead(Zip))
            using (ZipArchive Archive = new ZipArchive(Stream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry Entry in Archive.Entries)
                {
                    string Dest = Path.GetFullPath(Path.Combine(Root, Entry.FullName));
                    if (Dest.StartsWith(Root, StringComparison.OrdinalIgnoreCase))
                    {
                        if (Entry.FullName.EndsWith("/") || Entry.FullName.EndsWith("\\"))
                        {
                            Directory.CreateDirectory(Dest);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Dest));
                            using (Stream From = Entry.Open())
                            using (FileStream To = new FileStream(Dest, FileMode.Create, FileAccess.Write))
                            {
                                From.CopyTo(To);
                            }
                        }
                    }
                }
            }
        }
    }
}
