using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace RateController.Classes
{
    // Module firmware files from the latest GitHub release, fetched the way PCBsetup does:
    // ModulesHex.zip unpacked to <Application folder>\Firmware\ModulesHex. Only from the update
    // page: its button, or when it opens with no Teensy file at all. The Teensy file's date
    // (Versions.json key 1 when it was downloaded) is kept next to it.
    //
    // The app also ships the Teensy file it was built with (BundledHex, linked in the project
    // from Modules\Teensy Rate\RCteensy.ino.hex), so an update works offline. The page uses
    // whichever of the two has the newer firmware tag (BestTeensyHex).
    public static class FirmwareFiles
    {
        private const string HexFilesURL = "https://github.com/SK21/AOG_RC/releases/latest/download/ModulesHex.zip";
        private static readonly HttpClient cHttp = new HttpClient { Timeout = TimeSpan.FromMinutes(3) };

        public static string Folder => Path.Combine(Props.ApplicationFolder, "Firmware");

        public static string TeensyHex => Path.Combine(Folder, "ModulesHex", "RCteensy.ino.hex");

        public static string BundledHex => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Firmware", "RCteensy.ino.hex");

        // the downloaded or the bundled Teensy file, whichever has the newer firmware tag
        // (a file without a Teensy rate tag counts as oldest), "" when neither exists.
        // Version: the downloaded file's release version, "" for the bundled one.
        public static string BestTeensyHex(out string Version)
        {
            string Result = "";
            Version = "";
            int Downloaded = TagDate(TeensyHex);
            int Bundled = TagDate(BundledHex);
            if (File.Exists(TeensyHex) && (!File.Exists(BundledHex) || Downloaded >= Bundled))
            {
                Result = TeensyHex;
                Version = TeensyVersion();
            }
            else if (File.Exists(BundledHex))
            {
                Result = BundledHex;
            }
            return Result;
        }

        // the file's Teensy rate firmware tag date (yyyymmdd), 0 when it has none or can't be read
        private static int TagDate(string FileName)
        {
            int Result = 0;
            try
            {
                if (File.Exists(FileName))
                {
                    var Records = FirmwareUpdater.ReadHex(FileName);
                    if (Records != null && FirmwareUpdater.FindTag(Records, out byte Type, out ushort ID)
                        && Type == (byte)ModuleTypes.Teensy_Rate) Result = PGNs.PGN32403.InoDate(ID);
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("FirmwareFiles/TagDate: " + ex.Message);
            }
            return Result;
        }

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
