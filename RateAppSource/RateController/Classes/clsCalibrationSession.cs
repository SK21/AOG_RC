using RateController.Language;
using System;
using System.Windows.Forms;

namespace RateController.Classes
{
    public enum CalStopReasons
    { None, PhoneLost }

    public class clsCalibrationSession
    {
        // calibration state shared by frmMenuCalibrate and the phone page.
        // products are prepared while either one uses it and restored when both are done.
        // a run started from the phone stops if the page goes quiet (dead-man).

        public const string SpeedRestoreKey = "CalSpeedRestore";     // app prop, original speed source while a run is active
        private const int WebIdleSeconds = 120;
        private const int WebRunSeconds = 5;
        private clsCalibrates cCals;
        private bool cFormOpen = false;
        private CalStopReasons cStopReason = CalStopReasons.None;
        private bool cWebOwned = false;
        private bool cWebUsing = false;
        private SpeedType StartSim;
        private DateTime WebLastSeen;
        private Timer WebTimer = new Timer();

        public clsCalibrationSession()
        {
            WebTimer.Interval = 1000;
            WebTimer.Tick += WebTimer_Tick;
        }

        public event EventHandler<int> StepOneComplete;     // product ID

        public clsCalibrates Cals
        { get { return cCals; } }

        public CalStopReasons StopReason
        { get { return cStopReason; } }

        public bool WebOwned
        { get { return cWebOwned; } }

        public void Close()
        {
            if (cCals != null)
            {
                if (IsRunning()) Stop();
                Props.SpeedMode = StartSim;
                foreach (clsCalibrate cl in cCals.Items) cl.CalFinished -= Cal_CalFinished;
                cCals.Close();
                cCals = null;
            }
            cFormOpen = false;
            cWebUsing = false;
            WebTimer.Stop();
        }

        public void FormClosed()
        {
            cFormOpen = false;
            if (IsRunning() && !cWebOwned) Stop();
            CloseIfUnused();
        }

        public void FormOpened()
        {
            cFormOpen = true;
            Open();
        }

        public bool IsRunning()
        {
            bool Result = false;
            if (cCals != null)
            {
                foreach (clsCalibrate cl in cCals.Items)
                {
                    if (cl.Running) Result = true;
                }
            }
            return Result;
        }

        public string Start(int ProductID, bool FromWeb)
        {
            // returns the message to show
            string Result = "";
            if (cCals != null && !IsRunning() && cCals.ReadyToCalibrate())
            {
                // a locked motor/fan with no PWM would run with the motor stopped
                bool PWMmissing = false;
                foreach (clsCalibrate cl in cCals.Items)
                {
                    if (cl.PowerOn && cl.Locked && cl.CalPWM == 0)
                    {
                        switch (cl.Product.ControlType)
                        {
                            case ControlTypeEnum.Motor:
                            case ControlTypeEnum.MotorWeights:
                            case ControlTypeEnum.Fan:
                                PWMmissing = true;
                                break;
                        }
                    }
                }

                if (PWMmissing)
                {
                    Result = Lang.lgCalEnterPWM;
                }
                else
                {
                    Props.SetAppProp(SpeedRestoreKey, StartSim.ToString());   // interrupted run restores this at the next start
                    Props.SpeedMode = SpeedType.Simulated;
                    cCals.Running(true);
                    Props.RateCalibrationOn = true;
                    cWebOwned = FromWeb;
                    cStopReason = CalStopReasons.None;
                    if (FromWeb) WebLastSeen = DateTime.Now;
                    Result = cCals.Item(ProductID).Locked ? Lang.lgCalSettingMeterCal : Lang.lgCalSettingSpeed;
                }
            }
            return Result;
        }

        public void Stop()
        {
            if (cCals != null)
            {
                Props.SpeedMode = StartSim;
                Props.SetAppProp(SpeedRestoreKey, "");
                cCals.Running(false);
                Props.RateCalibrationOn = false;
                cWebOwned = false;
            }
        }

        public void WebSeen()
        {
            // phone calibration page status request
            WebLastSeen = DateTime.Now;
            cWebUsing = true;
            Open();
            WebTimer.Start();
        }

        private void Cal_CalFinished(object sender, EventArgs e)
        {
            int count = 0;
            foreach (clsCalibrate cl in cCals.Items)
            {
                if (cl.Running) count++;
            }
            if (count == 1) Stop();
            StepOneComplete?.Invoke(this, ((clsCalibrate)sender).ID);
        }

        private void CloseIfUnused()
        {
            if (!cFormOpen && !cWebUsing) Close();
        }

        private void Open()
        {
            if (cCals == null)
            {
                StartSim = Props.SpeedMode;
                cCals = new clsCalibrates();
                cCals.Load();
                foreach (clsCalibrate cl in cCals.Items) cl.CalFinished += Cal_CalFinished;
                cStopReason = CalStopReasons.None;
            }
        }

        private void WebTimer_Tick(object sender, EventArgs e)
        {
            double Quiet = (DateTime.Now - WebLastSeen).TotalSeconds;
            if (cWebOwned && IsRunning() && Quiet > WebRunSeconds)
            {
                // dead-man, the phone that started this run is gone
                Stop();
                cStopReason = CalStopReasons.PhoneLost;
            }
            if (Quiet > WebIdleSeconds)
            {
                cWebUsing = false;
                WebTimer.Stop();
                CloseIfUnused();
            }
        }
    }
}
