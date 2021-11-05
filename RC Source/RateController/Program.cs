using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using RateController.Properties;
using System.Threading;

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
            if (Properties.Settings.Default.setF_culture == "")
            {
                Properties.Settings.Default.setF_culture = "en";
                Properties.Settings.Default.Save();
            }

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Properties.Settings.Default.setF_culture);
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(Properties.Settings.Default.setF_culture);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormStart());
        }
    }
}
