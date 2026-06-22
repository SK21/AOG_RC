using RateController.Classes.Can;
using RateController.Forms;
using RateController.PGNs;
using RateController.RateMap;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace RateController.Classes
{
    public static class Core
    {
        public static PGN229 AOGsections;
        public static PGN254 AutoSteerPGN;
        public static CanBridgeComm CanBridgeComm;
        public static PGN208 GPS;
        public static PGN238 MachineConfig;
        public static PGN239 MachineData;
        public static frmMain MainForm;
        public static PGN32700 ModuleConfig;
        public static PGN32401 ModulesStatus;
        public static PGN32402 PgnPidLog;
        public static PidLogger PidLogger;
        public static clsProducts Products;
        public static clsAlarm RCalarm;
        public static clsRelays RelayObjects;
        public static PGN32501[] RelaySettings;
        public static PGN32296 ScaleIndicator;
        public static clsSectionControl SectionControl;
        public static clsRateAdjustController RateAdjustController;
        public static clsSections Sections;
        public static PGN235 SectionsPGN;
        public static PGN32618 SwitchBox;
        public static clsTools Tls = new clsTools();
        public static UDPComm UDPaog;
        public static UDPComm UDPmodules;
        public static clsVirtualSwitchBox vSwitchBox;
        public static PGN32504 WheelSpeed;
        public static clsZones Zones;
        public static bool IsRestarting { get; private set; }
        public static bool IsShuttingDown { get; private set; }
        public static bool IsUserExitRequested { get; private set; }

        #region private variables

        private static DateTime cStartTime;
        private static System.Timers.Timer MainTimer;
        private static bool[] cPressureGateLastConnected;   // tracks connect edge for on-connect pressure-gate resend

        #endregion private variables

        #region events

        public static event EventHandler AppExit;

        public static event EventHandler ColorChanged;

        public static event EventHandler ProfileChanged;

        public static event EventHandler RestoreMain;

        public static event EventHandler UpdateStatus;

        public static event EventHandler ModuleDataReceived;

        #endregion events

        public static bool AppShutDown(FormClosingEventArgs e)
        {
            bool Result = false;

            if (IsUserExitRequested || IsRestarting || e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.TaskManagerClosing)
            {
                IsShuttingDown = true;

                SafeTry(() => MainTimer.Enabled = false);
                SafeTry(() => UDPaog.Stop());
                SafeTry(() => UDPmodules.Stop());

                SafeTry(() => CanBridgeComm?.Stop());

                SafeTry(() => SafeEvent.Raise(AppExit));
                SafeTry(() => Sections.Save());
                SafeTry(() => LogRunTime());
                Result = true;
            }

            return Result;
        }

        public static void ChangeProfile(string NewProfile)
        {
            bool FileOpened = false;
            FileOpened = OpenFile(NewProfile);
            if (!FileOpened) FileOpened = OpenFile(Props.ProfilesFolder + "\\Default\\Default.rcs");

            if (FileOpened)
            {
                SafeEvent.Raise(ProfileChanged);
            }
            else
            {
                MessageBox.Show("The application must shut down due to an unexpected error.",
                    "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public static void Initialize(frmMain frm)
        {
            try
            {
                MainForm = frm;

                if (Tls.PrevInstance()) Application.Exit();

                Props.CheckFolders();
                Props.OpenFile(Props.CurrentFile);

                AutoSteerPGN = new PGN254();
                SectionsPGN = new PGN235();
                MachineConfig = new PGN238();
                MachineData = new PGN239();

                SwitchBox = new PGN32618();
                ModulesStatus = new PGN32401();
                PgnPidLog = new PGN32402();
                PidLogger = new PidLogger();
                // LogPID persists across restarts and already sets PGN 32500 bit 5 (module sends
                // PGN 32402), so start the CSV writer here too - otherwise logging only began when
                // the help page opened and fired ckPidLog.CheckedChanged.
                if (Props.LogPID) PidLogger.Start();

                Sections = new clsSections();
                RCalarm = new clsAlarm();

                Products = new clsProducts();
                Products.Load();

                RelayObjects = new clsRelays();

                RelaySettings = new PGN32501[Props.MaxModules];
                for (int i = 0; i < Props.MaxModules; i++)
                {
                    RelaySettings[i] = new PGN32501(i);
                }

                cPressureGateLastConnected = new bool[Props.MaxModules];

                vSwitchBox = new clsVirtualSwitchBox();
                Zones = new clsZones();
                ModuleConfig = new PGN32700();
                AOGsections = new PGN229();
                SectionControl = new clsSectionControl();
                RateAdjustController = new clsRateAdjustController();
                ScaleIndicator = new PGN32296();
                GPS = new PGN208();
                WheelSpeed = new PGN32504();

                ChangeProfile(Props.CurrentFile);

                ParcelManager.Initialize();
                JobManager.Initialize();
                MapController.Initialize();
                Props.JobCollector.Enabled = true;

                UDPaog = new UDPComm(MainForm, 17777, 15555, 1460, "UDPaog", "127.255.255.255");        // AOG
                UDPmodules = new UDPComm(MainForm, 29999, 28888, 1480, "UDPmodules");                   // arduino
                CanBridgeComm = new CanBridgeComm();

                UDPmodules.Start();
                if (!UDPmodules.IsRunning)
                {
                    Props.ShowMessage("UDPnetwork failed to start.", "", 3000, true, true);
                }

                UDPaog.Start();
                if (!UDPaog.IsRunning)
                {
                    Props.ShowMessage("UDPagio failed to start.", "", 3000, true, true);
                }

                if (Props.CanEnabled)
                {
                    CanBridgeComm.Start(Props.CurrentCanDriver, Props.CanPort);
                }

                Props.DisplayPressure();
                Props.DisplayRate();
                //Props.DisplayMapPreview();

                MainTimer = new System.Timers.Timer(1000);
                MainTimer.Elapsed += MainTimer_Elapsed;
                MainTimer.AutoReset = true;
                MainTimer.Enabled = true;

                Props.WriteActivityLog("Started", true);
                cStartTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not start: " + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
        }

        public static void RaiseModuleDataReceived()
        {
            SafeEvent.Raise(ModuleDataReceived);
        }

        public static void RaiseColorChanged()
        {
            SafeEvent.Raise(ColorChanged);
        }

        public static void SetMainDisplay(bool ShowNormal)
        {
            if (ShowNormal)
            {
                Props.SaveFormLocation(MainForm.MainMini);
                MainForm.MainMini.Hide();
                MainForm.WindowState = FormWindowState.Normal;
                SafeEvent.Raise(RestoreMain);
            }
            else
            {
                MainForm.MainMini.Show();
                MainForm.WindowState = FormWindowState.Minimized;
            }
        }

        public static void RequestRestart()
        {
            IsRestarting = true;
            Application.Restart();
        }

        public static void RequestUserExit()
        {
            bool Result = false;

            if (!Products.Connected())
            {
                Result = true;
            }
            else
            {
                using (var Hlp = new frmMsgBox("Confirm Exit?", "Exit", true))
                {
                    Hlp.TopMost = true;
                    Hlp.ShowDialog();
                    Result = Hlp.Result;
                }
            }

            if (Result)
            {
                IsUserExitRequested = true;
                Application.Exit();
            }
        }
        public static void SendRateSettings()
        {
            foreach (clsProduct Prd in Products.Items)
            {
                if (Prd.Enabled) Prd.ModuleRateSettings.Send();
            }
        }
        public static void SendRelays()
        {
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (ModulesStatus.Connected(i)) RelaySettings[i].Send();
            }
        }

        public static void SendPressureGate()
        {
            PGN32505 gate = new PGN32505();
            for (int i = 0; i < Props.MaxModules; i++)
            {
                if (ModulesStatus.Connected(i)) gate.Send(i);
            }
        }

        public static void ResendPressureGateOnConnect()
        {
            // Resend the gate threshold to any module on its disconnected->connected edge,
            // so a freshly powered/never-configured module is armed without a profile reload.
            PGN32505 gate = new PGN32505();
            for (int i = 0; i < Props.MaxModules; i++)
            {
                bool connected = ModulesStatus.Connected(i);
                if (connected && !cPressureGateLastConnected[i]) gate.Send(i);
                cPressureGateLastConnected[i] = connected;
            }
        }

        public static int UseCanComm(bool enable)
        {
            int Result = -1;
            if (enable)
            {
                // use CanBus
                // Start CAN bridge — ensure clean state first
                Core.CanBridgeComm?.Stop();

                bool started = Core.CanBridgeComm?.Start(Props.CurrentCanDriver, Props.CanPort) ?? false;
                if (started)
                {
                    ModuleConfig.Send(1);
                    Props.CanEnabled = true;
                    Props.ShowMessage("CAN Bridge started.", "Help", 10000);
                    Result = 1;
                }
                else
                {
                    Result = 2;
                }
            }

            if (Result != 1)
            {
                // use Ethernet
                ModuleConfig.Send(0);
                if (Props.CanEnabled)
                {
                    CanBridgeComm?.Stop();
                    Props.ShowMessage("CAN Bridge stopped.", "Help", 10000);
                    Props.CanEnabled = false;
                }
                Result += 1;
            }
            return Result;
        }

        private static void LogRunTime()
        {
            Props.WriteActivityLog("Stopped");
            string mes = "Run time (hours): " + ((DateTime.Now - cStartTime).TotalSeconds / 3600.0).ToString("N1");
            Props.WriteActivityLog(mes);
        }

        private static void MainTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            RateAdjustController.Update();
            SafeEvent.Raise(UpdateStatus);
            SendRelays();
            ResendPressureGateOnConnect();
            RCalarm.CheckAlarms();
        }

        private static Boolean OpenFile(string NewProfile)
        {
            bool Result = false;
            if (Props.OpenFile(NewProfile))
            {
                Sections.Load();
                Sections.CheckSwitchDefinitions();
                RelayObjects.Load();
                Zones.Load();
                Props.DisplaySwitches();
                Products.Load();
                Products.UpdateSensorSettings();
                SendPressureGate();
                Props.ShowScales();
                Result = true;
            }

            return Result;
        }

        private static void SafeTry(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                try
                {
                    Props.WriteActivityLog("Shutdown error: " + ex.Message);
                }
                catch
                {
                    // Last line of defense: swallow everything
                    // because shutdown must continue no matter what
                }
            }
        }
    }

    public static class FormManager
    {
        private static readonly Dictionary<string, Form> forms = new Dictionary<string, Form>();

        public static void CloseForm<T>() where T : Form
        {
            CloseForm<T>("default");
        }

        public static void CloseForm<T>(string instance) where T : Form
        {
            string key = MakeKey(typeof(T), instance);

            Form frm;
            if (forms.TryGetValue(key, out frm) && !frm.IsDisposed)
            {
                frm.Close();
            }
        }

        // ---------------------------
        // SINGLE‑INSTANCE API
        // ---------------------------
        public static void ShowForm(Form frm)
        {
            ShowForm(frm, "default");
        }

        // ---------------------------
        // MULTI‑INSTANCE API
        // ---------------------------
        public static void ShowForm(Form frm, string instance)
        {
            string key = MakeKey(frm.GetType(), instance);

            var main = Core.MainForm;
            if (main == null || main.IsDisposed || !main.IsHandleCreated)
                return;

            if (main.InvokeRequired)
            {
                main.BeginInvoke((Action)(() => ShowForm(frm, instance)));
                return;
            }

            Form existing;
            if (forms.TryGetValue(key, out existing) && !existing.IsDisposed)
            {
                existing.BringToFront();
                return;
            }

            forms[key] = frm;

            frm.FormClosed += (s, e) => forms.Remove(key);
            frm.Show();
        }

        private static string MakeKey(Type t, string instance)
        {
            return t.FullName + "_" + instance;
        }
    }

    public static class SafeEvent
    {
        public static void Raise(EventHandler evt, EventArgs args = null, object sender = null)
        {
            if (evt == null)
                return;

            if (args == null)
                args = EventArgs.Empty;

            if (sender == null)
                sender = typeof(Core);

            foreach (EventHandler handler in evt.GetInvocationList())
            {
                InvokeHandler(handler, sender, args);
            }
        }

        private static void InvokeHandler(EventHandler handler, object sender, EventArgs args)
        {
            // Try to get the target as a Control
            if (handler.Target is Control ctrl)
            {
                if (ctrl.IsDisposed || ctrl.Disposing || !ctrl.IsHandleCreated)
                    return;

                if (ctrl.InvokeRequired)
                {
                    ctrl.BeginInvoke(new Action(() => handler(sender, args)));
                }
                else
                {
                    handler(sender, args);
                }
            }
            else
            {
                // Non‑UI subscriber
                handler(sender, args);
            }
        }
    }
}