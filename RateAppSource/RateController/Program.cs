using Microsoft.Win32;
using RateController.Forms;
using RateController.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController
{
    internal static class Program
    {
        private static void Log(Exception ex)
        {
            if (ex == null) return;

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "RC_Crash.log");

                File.AppendAllText(path,
                    $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}  {ex}\r\n\r\n");
            }
            catch
            {
                // Avoid recursive crash loops
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (s, e) =>
            {
                Log(e.Exception);
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Log(e.ExceptionObject as Exception);
            };

            try
            {
                if (Settings.Default.CurrentLanguage == "none")
                {
                    string Lang = null;
                    bool Found = false;
                    // check AOG language setting
                    using (var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AgOpenGPS"))
                    {
                        Lang = regKey?.GetValue("Language") as string;
                    }

                    if (!string.IsNullOrWhiteSpace(Lang))
                    {
                        string[] LanguageIDs = new string[] { "en", "de", "hu", "nl", "pl", "ru", "fr", "lt" };
                        foreach (string LanguageID in LanguageIDs)
                        {
                            if (Lang == LanguageID)
                            {
                                Settings.Default.CurrentLanguage = Lang;
                                Found = true;
                                break;
                            }
                        }
                    }

                    if (!Found) Settings.Default.CurrentLanguage = "en";
                    Settings.Default.Save();
                }
            }
            catch
            {
                Settings.Default.CurrentLanguage = "en";
                Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Properties.Settings.Default.CurrentLanguage);
                Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.CurrentLanguage);
            }
            catch
            {
                var fallback = new CultureInfo("en");
                Thread.CurrentThread.CurrentCulture = fallback;
                Thread.CurrentThread.CurrentUICulture = fallback;
            }

            using (frmMain StartUp = new frmMain())
            {
                Application.Run(StartUp);
            }
        }
    }
}