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
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\AgOpenGPS");
            if (regKey != null)
            {
                Settings.Default.AOG_language = regKey.GetValue("Language").ToString();
                Settings.Default.Save();
            }
            regKey.Close();

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

            Environment.Exit(0);
        }
    }
}
