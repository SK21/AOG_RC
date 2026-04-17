using Microsoft.Win32;
using RateController.Classes;
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
        private static bool _errorShown = false;

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

        private static void HandleException(Exception ex, string source)
        {
            try
            {
                Log(ex);
                Props.WriteErrorLog("Unhandled (" + source + "): " + ex?.Message);

                if (!_errorShown)
                {
                    _errorShown = true;
                    MessageBox.Show(
                        "An unexpected error occurred:\n\n" + ex?.Message +
                        "\n\nDetails have been logged. Check the error log on the Help page.",
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                // prevent recursive crash if handler itself fails
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
                HandleException(e.Exception, "UI Thread");
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                HandleException(e.ExceptionObject as Exception, "AppDomain");
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