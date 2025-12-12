using Microsoft.Win32;
using RateController.Properties;
using System;
using System.Threading;
using System.Windows.Forms;

namespace RateController
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // check AOG language setting
            using (var regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AgOpenGPS"))
            {
                var lang = regKey?.GetValue("Language") as string;
                if (!string.IsNullOrWhiteSpace(lang))
                {
                    Settings.Default.AOG_language = lang;
                    Settings.Default.Save();
                }
            }

            if (Settings.Default.setF_culture == "")
            {
                Settings.Default.setF_culture = "en";
                Settings.Default.Save();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Properties.Settings.Default.setF_culture);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.setF_culture);

            using (FormStart frmStart = new FormStart())
            {
                Application.Run(frmStart);
            }
        }
    }
}
