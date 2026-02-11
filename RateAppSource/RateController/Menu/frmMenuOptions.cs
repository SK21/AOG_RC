using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuOptions : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private bool WheelSpeedChanged = false;

        public frmMenuOptions(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCal_Click(object sender, EventArgs e)
        {
            double calNumber = 0;
            bool wasCanceled = true;

            using (var dlg = new RateController.Forms.frmSpeedCal())
            {
                var result = dlg.ShowDialog(this);
                wasCanceled = (result != DialogResult.OK) || dlg.Canceled;
                calNumber = dlg.CalNumber;
            }

            if (!wasCanceled) tbWheelCal.Text = calNumber.ToString("N0");
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            WheelSpeedChanged = false;
            butUpdateModules.Enabled = rbWheel.Checked;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (rbAOG.Checked)
                {
                    Props.SpeedMode = SpeedType.GPS;
                }
                else if (rbIsoBusSpeed.Checked)
                {
                    Props.SpeedMode = SpeedType.ISOBUS;
                }
                else if (rbWheel.Checked)
                {
                    Props.SpeedMode = SpeedType.Wheel;
                }
                else
                {
                    Props.SpeedMode = SpeedType.Simulated;
                }

                if (int.TryParse(tbWheelModule.Text, out int wm)) Core.WheelSpeed.WheelModule = wm;

                if (int.TryParse(tbWheelPin.Text, out int wp) && (ValidPin(wp)))
                {
                    Core.WheelSpeed.WheelPin = wp;
                }
                else
                {
                    Core.WheelSpeed.WheelPin = 255;
                }

                if (double.TryParse(tbWheelCal.Text, out double wc)) Core.WheelSpeed.WheelCal = wc;

                if (double.TryParse(tbSimSpeed.Text, out double Speed))
                {
                    if (Props.UseMetric)
                    {
                        Props.SimSpeed_KMH = Speed;
                    }
                    else
                    {
                        Props.SimSpeed_KMH = Speed * Props.MPHtoKPH;
                    }
                }

                Props.UseMetric = ckMetric.Checked;
                Props.UseRateDisplay = ckRateDisplay.Checked;

                if (WheelSpeedChanged)
                {
                    Core.WheelSpeed.Send();
                    WheelSpeedChanged = false;
                    butUpdateModules.Enabled = rbWheel.Checked;
                }

                // ISOBUS settings
                // Save CAN driver and COM port BEFORE starting gateway (so UpdateGatewayConfig uses new values)
                if (rbAdapter2.Checked)
                {
                    Props.CurrentCanDriver = CanDriver.InnoMaker;
                }
                else if (rbAdapter3.Checked)
                {
                    Props.CurrentCanDriver = CanDriver.PCAN;
                }
                else
                {
                    Props.CurrentCanDriver = CanDriver.SLCAN;
                }

                if (cbComPort.SelectedItem != null)
                {
                    Props.CanPort = cbComPort.SelectedItem.ToString();
                }

                // Check if diagnostics setting changed
                bool diagnosticsChanged = Props.ShowCanDiagnostics != ckDiagnostics.Checked;
                Props.ShowCanDiagnostics = ckDiagnostics.Checked;

                if (ckIsoBus.Checked && !Props.IsobusEnabled)
                {
                    // Start ISOBUS gateway - first ensure clean state
                    Core.IsobusComm?.StopGateway();
                    Core.IsobusComm?.StopUDP();
                    System.Threading.Thread.Sleep(300);

                    bool udpStarted = Core.IsobusComm?.StartUDP() ?? false;
                    bool gatewayStarted = Core.IsobusComm?.StartGateway() ?? false;

                    if (gatewayStarted && udpStarted)
                    {
                        Props.IsobusEnabled = true;
                        Props.ShowMessage("ISOBUS Gateway started.");
                    }
                    else if (!gatewayStarted)
                    {
                        Props.ShowMessage("Failed to start ISOBUS Gateway. Check that IsobusGateway.exe exists.");
                        Core.IsobusComm?.StopUDP();
                    }
                    else
                    {
                        Props.IsobusEnabled = true;
                        Props.ShowMessage("ISOBUS Gateway started but UDP failed.");
                    }
                }
                else if (!ckIsoBus.Checked && Props.IsobusEnabled)
                {
                    // Stop ISOBUS gateway
                    Core.IsobusComm?.StopGateway();
                    Core.IsobusComm?.StopUDP();
                    Props.IsobusEnabled = false;
                    Props.ShowMessage("ISOBUS Gateway stopped.");

                    // If ISOBUS speed was selected, switch to GPS
                    if (rbIsoBusSpeed.Checked)
                    {
                        rbAOG.Checked = true;
                        Props.SpeedMode = SpeedType.GPS;
                    }
                }

                // Restart gateway if diagnostics changed and ISOBUS is enabled (to apply console visibility)
                if (diagnosticsChanged && Props.IsobusEnabled && Core.IsobusComm != null)
                {
                    Core.IsobusComm.StopGateway();
                    System.Threading.Thread.Sleep(500);
                    Core.IsobusComm.StartGateway();
                    Props.ShowMessage(ckDiagnostics.Checked ? "Gateway diagnostics enabled." : "Gateway diagnostics disabled.");
                }

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuDisplay/btnOk_Click: " + ex.Message);
            }
        }

        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            RefreshComPorts();
        }

        private void butUpdateModules_Click(object sender, EventArgs e)
        {
            Core.WheelSpeed.Send();
        }

        private void cbComPort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void ckDiagnostics_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing) SetButtons(true);
        }

        private void ckIsobusEnabled_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void ckLargeScreen_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;
            Props.SaveFormLocation(this);
        }

        private void frmMenuDisplay_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;
            lbPulses.Font = new Font(lbPulses.Font.FontFamily, 12f, lbPulses.Font.Style,
                                lbPulses.Font.Unit, lbPulses.Font.GdiCharSet, lbPulses.Font.GdiVerticalFont);

            tabControl1.ItemSize = new Size((tabControl1.Width - 14) / tabControl1.TabCount, tabControl1.ItemSize.Height);

            foreach (TabPage tb in tabControl1.TabPages)
            {
                tb.BackColor = Properties.Settings.Default.MainBackColour;
            }

            timer1.Enabled = true;

            PositionForm();
            SetBoxes();
            UpdateForm();
        }

        private void gbNetwork_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }


        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + SubMenuLayout.TopOffset;
            this.Left = MainMenu.Left + SubMenuLayout.LeftOffset;
        }

        private void rbAdapter1_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                UpdatePortVisibility();
            }
        }

        private void rbAOG_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                SetButtons(true);
                SetBoxes();
            }
        }


        private void rbWheel_CheckedChanged(object sender, EventArgs e)
        {
            if (!Initializing)
            {
                WheelSpeedChanged = true;
                SetButtons(true);
                SetBoxes();
            }
        }

        private void RefreshComPorts()
        {
            cbComPort.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports);
            foreach (string port in ports)
            {
                cbComPort.Items.Add(port);
            }

            // Select current port if it exists
            int index = cbComPort.FindStringExact(Props.CanPort);
            if (index >= 0)
            {
                cbComPort.SelectedIndex = index;
            }
            else if (cbComPort.Items.Count > 0)
            {
                cbComPort.SelectedIndex = 0;
            }
        }

        private void SetBoxes()
        {
            tbWheelModule.Enabled = rbWheel.Checked;
            tbWheelPin.Enabled = rbWheel.Checked;
            tbWheelCal.Enabled = rbWheel.Checked;
            tbSimSpeed.Enabled = rbSimulated.Checked;
            lbCal.Enabled = rbWheel.Checked;
            lbModule.Enabled = rbWheel.Checked;
            lbPin.Enabled = rbWheel.Checked;
            butUpdateModules.Enabled = rbWheel.Checked;
            lbPulses.Enabled = rbWheel.Checked;
            btnCal.Enabled = rbWheel.Checked;

            // ISOBUS speed option only available when ISOBUS is enabled
            rbIsoBusSpeed.Enabled = ckIsoBus.Checked;
        }

        private void SetButtons(bool Edited)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                }

                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            ckMetric.Text = Lang.lgMetric;
            rbSimulated.Text = Lang.lgSimulateSpeed;
        }

        private void tbSimSpeed_Enter(object sender, EventArgs e)
        {

        }

        private void tbSimSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelCal_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            using (var form = new FormNumeric(0, 0xffffff, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelCal.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void tbWheelCal_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbWheelCal.Text, out tempD);
            if (tempD < 0 || tempD > 0xffffff)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelModule_Enter(object sender, EventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            using (var form = new FormNumeric(0, 7, tempInt))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbWheelModule.Text = form.ReturnValue.ToString();
                }
            }
        }

        private void tbWheelModule_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelModule.Text, out tempInt);
            if (tempInt < 0 || tempInt > 7)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void tbWheelPin_Enter(object sender, EventArgs e)
        {
            var bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;

            using (var form = new FormNumeric(0, 50, temp))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    if (form.IsBlank)
                    {
                        bx.Text = "-";
                    }
                    else
                    {
                        bx.Text = form.ReturnValue.ToString("N0");
                    }
                }
            }
        }

        private void tbWheelPin_Validating(object sender, CancelEventArgs e)
        {
            int tempInt;
            int.TryParse(tbWheelPin.Text, out tempInt);
            if (tempInt < 0 || tempInt > 50)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Check if ISOBUS is enabled and comm object exists
            if (Core.IsobusComm != null && Props.IsobusEnabled)
            {
                // lbConnected = Actual ISOBUS module data being received (PGN 32400/32401)
                if (Core.IsobusComm.ModuleDataReceiving)
                {
                    lbConnected.Image = Properties.Resources.On;
                }
                else
                {
                    lbConnected.Image = Properties.Resources.Off;
                }

                // lbDriverFound = Gateway process responding via UDP
                if (Core.IsobusComm.GatewayConnected)
                {
                    lbDriverFound.Image = Properties.Resources.On;
                }
                else
                {
                    lbDriverFound.Image = Properties.Resources.Off;
                }
            }
            else
            {
                // ISOBUS not enabled - show off for both
                lbConnected.Image = Properties.Resources.Off;
                lbDriverFound.Image = Properties.Resources.Off;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;

            switch (Props.SpeedMode)
            {
                case SpeedType.Wheel:
                    rbWheel.Checked = true;
                    break;

                case SpeedType.Simulated:
                    rbSimulated.Checked = true;
                    break;

                case SpeedType.ISOBUS:
                    rbIsoBusSpeed.Checked = true;
                    break;

                default:
                    rbAOG.Checked = true;
                    break;
            }

            tbWheelModule.Text = Core.WheelSpeed.WheelModule.ToString("N0");

            if (Core.WheelSpeed.WheelPin == 255)
            {
                tbWheelPin.Text = "-";
            }
            else
            {
                tbWheelPin.Text = Core.WheelSpeed.WheelPin.ToString("N0");
            }

            tbWheelCal.Text = Core.WheelSpeed.WheelCal.ToString("N0");

            if (Props.UseMetric)
            {
                lbSimUnits.Text = "KMH";
                lbPulses.Text = "(pulses/km)";
                tbSimSpeed.Text = Props.SimSpeed_KMH.ToString("N1");
            }
            else
            {
                lbSimUnits.Text = "MPH";
                lbPulses.Text = "(pulses/mile)";
                tbSimSpeed.Text = (Props.SimSpeed_KMH / Props.MPHtoKPH).ToString("N1");
            }

            ckMetric.Checked = Props.UseMetric;
            ckRateDisplay.Checked = Props.UseRateDisplay;
            ckIsoBus.Checked = Props.IsobusEnabled;

            switch (Props.CurrentCanDriver)
            {
                case CanDriver.InnoMaker:
                    rbAdapter2.Checked = true;
                    break;

                case CanDriver.PCAN:
                    rbAdapter3.Checked = true;
                    break;

                default:
                    rbAdapter1.Checked = true;  // SLCAN
                    break;
            }

            ckDiagnostics.Checked = Props.ShowCanDiagnostics;

            RefreshComPorts();
            UpdatePortVisibility();

            SetBoxes();

            gbxDrivers.Enabled = !ckIsoBus.Checked;
            ckDiagnostics.Enabled = !ckIsoBus.Checked;

            Initializing = false;
        }

        private void UpdatePortVisibility()
        {
            // Only SLCAN uses COM port - other drivers are native USB
            bool showPort = rbAdapter1.Checked;  // rbAdapter1 = SLCAN
            gbxPort.Visible = showPort;
            gbxPort.Enabled = !ckIsoBus.Checked;
        }

        private bool ValidPin(int pin)
        {
            bool Result = true;
            if (Core.ModuleConfig.Sensor0Flow == pin || Core.ModuleConfig.Sensor1Flow == pin)
            {
                Result = false;
                Props.ShowMessage("Invalid pin, duplicate of flow pin.");
            }
            return Result;
        }
    }
}