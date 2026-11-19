using Microsoft.Win32;
using RateController.Classes;
using RateController.Forms;
using RateController.Properties;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        private const string InstanceMutexName = "Global\\RateController_SingleInstance";
        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xffff);
        private const int ASFW_ANY = -1;

        private static Mutex InstanceMutex;
        private static bool OwnsInstanceMutex = false;

        /// <summary>
        /// Message a second copy posts to ask the running copy to show itself (handled in frmMain.WndProc).
        /// </summary>
        public static readonly int WM_SHOWINSTANCE = RegisterWindowMessage("RateController_ShowInstance");

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool AllowSetForegroundWindow(int dwProcessId);

        /// <summary>
        /// Releases the single-instance lock so a restarted copy can take it (call on the UI thread).
        /// </summary>
        public static void ReleaseInstanceLock()
        {
            if (InstanceMutex != null)
            {
                try
                {
                    if (OwnsInstanceMutex) InstanceMutex.ReleaseMutex();
                }
                catch
                {
                    // already released
                }
                InstanceMutex.Dispose();
                InstanceMutex = null;
                OwnsInstanceMutex = false;
            }
        }

        /// <summary>
        /// Asks the already-running copy of RateController to show itself.
        /// </summary>
        private static void SignalExistingInstance()
        {
            try
            {
                if (WM_SHOWINSTANCE != 0)
                {
                    AllowSetForegroundWindow(ASFW_ANY);
                    PostMessage(HWND_BROADCAST, WM_SHOWINSTANCE, IntPtr.Zero, IntPtr.Zero);
                }
            }
            catch
            {
                // nothing to signal; this copy exits regardless
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            bool createdNew;
            try
            {
                InstanceMutex = new Mutex(true, InstanceMutexName, out createdNew);
            }
            catch (UnauthorizedAccessException)
            {
                // the mutex exists but belongs to another user's session
                InstanceMutex = null;
                createdNew = false;
            }

            if (createdNew)
            {
                OwnsInstanceMutex = true;
                RunApp();
                ReleaseInstanceLock();
            }
            else
            {
                InstanceMutex?.Dispose();
                InstanceMutex = null;
                SignalExistingInstance();
            }
        }

        private static void RunApp()
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