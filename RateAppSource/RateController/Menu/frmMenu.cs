using RateController.Classes;
using RateController.Forms;
using RateController.Language;
using RateController.Menu;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmMenu : Form
    {
        public const int StartLeft = 100;
        public const int StartTop = 100;
        private const int FormHeight = 680;
        private const int FormWidth = 800;
        private const int SubFirstSpacing = 75;
        private const int SubOffset = 10;
        private const int SubSpacing = 55;
        private clsProduct cCurrentProduct;
        private string cLastScreen = "";
        private bool cMenuNetworkHasRan = false;
        private bool Expanded = false;
        private Button[] Items;
        private bool LoadLast = false;
        private FormStart mf;
        private Point MouseDownLocation;

        public frmMenu(FormStart cf, int ProductID, bool LoadLst = false)
        {
            InitializeComponent();
            this.mf = cf;

            Items = new Button[] { butProfiles, butJobs,  butRate, butControl, butSettings, butMode, butMonitor,
                butData, butSections, butRelays, butCalibrate, butNetwork, butConfig, butPins, butRelayPins, butWifi,
                butValves, butDisplay, butPrimed, butSwitches, butLanguage, butColor,butRateData };

            ChangeProduct(ProductID);
            LoadLast = LoadLst;
        }

        public event EventHandler MenuMoved;

        public event EventHandler ModuleDefaultsSet;

        public event EventHandler ProductChanged;

        public clsProduct CurrentProduct
        {
            get { return cCurrentProduct; }
        }

        public string LastScreen
        { get { return cLastScreen; } }

        public bool MenuNetworkHasRan
        { get { return cMenuNetworkHasRan; } set { cMenuNetworkHasRan = value; } }

        public void ChangeProduct(int NewID, bool NoFans = false)
        {
            if (NewID < 0) NewID = 0;
            if (NewID > Props.MaxProducts - 1) NewID = Props.MaxProducts - 1;
            if (NoFans && NewID > Props.MaxProducts - 3) NewID = Props.MaxProducts - 3;
            cCurrentProduct = mf.Products.Item(NewID);
            ProductChanged?.Invoke(this, EventArgs.Empty);
        }

        public void DefaultsSet()
        {
            ModuleDefaultsSet?.Invoke(this, EventArgs.Empty);
        }

        public void HighlightUpdateButton(bool Highlight = true)
        {
            if (Highlight)
            {
                butUpdateModules.FlatAppearance.BorderSize = 4;
                butUpdateModules.FlatAppearance.BorderColor = Color.DarkGreen;
            }
            else
            {
                butUpdateModules.FlatAppearance.BorderSize = 0;
            }
        }

        public void ShowProfile()
        {
            string Nm = Props.CurrentFileName().Length <= 20 ? Props.CurrentFileName() : Props.CurrentFileName().Substring(0, 20) + "...";
            lbFileName.Text = "[" + Nm + "]";

            string job = Path.GetFileNameWithoutExtension(Props.CurrentJobName);
            Nm = job.Length <= 20 ? job : job.Substring(0, 20) + "...";
            lbJob.Text = "[" + Nm + "]";
        }

        public void StyleControls(Control Parent)
        {
            if (Parent is Form frm)
            {
                frm.BackColor = Properties.Settings.Default.MainBackColour;
            }

            foreach (Control con in Parent.Controls)
            {
                if (con is Label ctl)
                {
                    ctl.ForeColor = Properties.Settings.Default.MainForeColour;
                    ctl.BackColor = Properties.Settings.Default.MainBackColour;
                    ctl.Font = Properties.Settings.Default.MenuFontSmall;
                }

                if (con is Button but)
                {
                    but.ForeColor = Properties.Settings.Default.MainForeColour;
                    but.BackColor = Properties.Settings.Default.MainBackColour;
                    but.FlatAppearance.MouseDownBackColor = Properties.Settings.Default.MouseDown;
                }

                if (con is ComboBox cbox)
                {
                    cbox.ForeColor = Properties.Settings.Default.MainForeColour;
                    cbox.BackColor = Properties.Settings.Default.MainBackColour;
                    cbox.Font = Properties.Settings.Default.MenuFontSmall;
                }

                if (con is CheckBox cb)
                {
                    cb.ForeColor = Properties.Settings.Default.MainForeColour;
                    cb.BackColor = Properties.Settings.Default.MainBackColour;
                    cb.FlatAppearance.CheckedBackColor = Color.LightGreen;
                }

                if (con.HasChildren) StyleControls(con);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.MainForeColour;
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = exeDirectory + "Help\\" + cLastScreen + ".pdf";
            try
            {
                if (File.Exists(FileName))
                {
                    Process.Start(new ProcessStartInfo { FileName = FileName, UseShellExecute = true });
                }
                else
                {
                    mf.Tls.ShowMessage("No help available.");
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenu/bthHelp_Click: " + ex.Message);
            }
        }

        private void btnPressure_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuPressure";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuPressure(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butCalibrate_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuCalibrate";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuCalibrate(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            if (ClosedOwned()) this.Close();
        }

        private void butColor_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuColor";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuColor(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butConfig_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuConfig";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuConfig(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butControl_Click(object sender, EventArgs e)
        {
            if (CheckEdited())
            {
                cLastScreen = "frmMenuControl";
                if (sender is Button button) HighlightButton(button);
                Form fs = Props.IsFormOpen(cLastScreen);

                if (fs == null)
                {
                    Form frm = new frmMenuControl(mf, this);
                    frm.Owner = this;
                    frm.Show();
                }
            }
        }

        private void butData_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuData";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuData(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butDisplay_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuDisplay";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuDisplay(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butFile_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butProfiles.Visible = !Expanded;
                butJobs.Visible = !Expanded;
                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = true;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = true;
                    butProducts.Visible = false;
                    butMachine.Visible = false;
                    butModules.Visible = false;
                    butOptions.Visible = false;
                    butHelpScreen.Visible = false;

                    int Pos = butFile.Top;
                    butProfiles.Visible = true;
                    butProfiles.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butProfiles.Top = Pos;

                    butJobs.Visible = true;
                    butJobs.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butJobs.Top = Pos;

                    butProfiles.PerformClick();
                }
            }
        }

        private void butHelpScreen_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuHelp";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuHelp(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butJobs_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuJobs";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuJobs(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butLanguage_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuLanguage";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuLanguage(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butMachine_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butSections.Visible = !Expanded;
                butRelays.Visible = !Expanded;
                butCalibrate.Visible = !Expanded;
                butSwitches.Visible = !Expanded;
                butPrimed.Visible = !Expanded;
                btnPressure.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = true;
                    butMachine.Top = (int)butMachine.Tag;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = false;
                    butProducts.Visible = false;
                    butMachine.Visible = true;
                    butModules.Visible = false;
                    butOptions.Visible = false;
                    butHelpScreen.Visible = false;
                    butMachine.Tag = butMachine.Top;
                    butMachine.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butSections.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butSections.Top = Pos;

                    butRelays.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butRelays.Top = Pos;

                    butSwitches.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butSwitches.Top = Pos;

                    btnPressure.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    btnPressure.Top = Pos;

                    butPrimed.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butPrimed.Top = Pos;

                    butCalibrate.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butCalibrate.Top = Pos;

                    butSections.PerformClick();
                }
            }
        }

        private void butMap_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuRateMap";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuRateMap(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butMode_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuMode";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuMode(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butModules_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butNetwork.Visible = !Expanded;
                butConfig.Visible = !Expanded;
                butPins.Visible = !Expanded;
                butRelayPins.Visible = !Expanded;
                butWifi.Visible = !Expanded;
                butValves.Visible = !Expanded;
                butUpdateModules.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = true;
                    butModules.Top = (int)butModules.Tag;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = false;
                    butProducts.Visible = false;
                    butMachine.Visible = false;
                    butModules.Visible = true;
                    butOptions.Visible = false;
                    butHelpScreen.Visible = false;
                    butModules.Tag = butModules.Top;
                    butModules.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butNetwork.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butNetwork.Top = Pos;

                    butConfig.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butConfig.Top = Pos;

                    butPins.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butPins.Top = Pos;

                    butRelayPins.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butRelayPins.Top = Pos;

                    butWifi.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butWifi.Top = Pos;

                    butValves.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butValves.Top = Pos;

                    butUpdateModules.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butUpdateModules.Top = Pos;

                    butNetwork.PerformClick();
                }
            }
        }

        private void butMonitor_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuMonitoring";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuMonitoring(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                ProductChanged?.Invoke(this, EventArgs.Empty);  // to get form to update
                fs.Focus();
            }
        }

        private void butNetwork_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuNetwork";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuNetwork(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butOptions_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butDisplay.Visible = !Expanded;
                butLanguage.Visible = !Expanded;
                butColor.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = true;
                    butOptions.Top = (int)butOptions.Tag;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = false;
                    butProducts.Visible = false;
                    butMachine.Visible = false;
                    butModules.Visible = false;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = false;
                    butOptions.Tag = butOptions.Top;
                    butOptions.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butDisplay.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butDisplay.Top = Pos;

                    butLanguage.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butLanguage.Top = Pos;

                    butColor.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butColor.Top = Pos;

                    butDisplay.PerformClick();
                }
            }
        }

        private void butPins_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuPins";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuPins(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butPowerOff_Click(object sender, EventArgs e)
        {
            if (ClosedOwned()) mf.Close();
        }

        private void butPrimed_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuPrimed";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuPrimed(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butProducts_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butRate.Visible = !Expanded;
                butControl.Visible = !Expanded;
                butSettings.Visible = !Expanded;
                butMode.Visible = !Expanded;
                butMonitor.Visible = !Expanded;
                butData.Visible = !Expanded;
                butMap.Visible = !Expanded;
                butRateData.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butHelpScreen.Visible = true;
                    butProducts.Top = (int)butProducts.Tag;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = false;
                    butProducts.Visible = true;
                    butMachine.Visible = false;
                    butModules.Visible = false;
                    butOptions.Visible = false;
                    butHelpScreen.Visible = false;
                    butProducts.Tag = butProducts.Top;
                    butProducts.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butRate.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butRate.Top = Pos;

                    butControl.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butControl.Top = Pos;

                    butSettings.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butSettings.Top = Pos;

                    butMode.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butMode.Top = Pos;

                    butMonitor.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butMonitor.Top = Pos;

                    butData.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butData.Top = Pos;

                    butMap.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butMap.Top = Pos;

                    butRateData.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butRateData.Top = Pos;

                    butRate.PerformClick();
                }
            }
        }

        private void butProfiles_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuProfiles";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuProfiles(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butRate_Click(object sender, EventArgs e)
        {
            if (CheckEdited())
            {
                cLastScreen = "frmMenuRate";
                if (sender is Button button) HighlightButton(button);
                Form fs = Props.IsFormOpen(cLastScreen);

                if (fs == null)
                {
                    Form frm = new frmMenuRate(mf, this);
                    frm.Owner = this;
                    frm.Show();
                }
                else
                {
                    fs.Focus();
                }
            }
        }

        private void butRateData_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuRateData";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuRateData(mf, this);
                frm.Owner = this;
                frm.Show();
            }
        }

        private void butRelayPins_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuRelayPins";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuRelayPins(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butRelays_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuRelays";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuRelays(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butSections_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuSections";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuSections(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butSettings_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuSettings";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuSettings(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butSwitches_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuSwitches";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuSwitches(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butUpdateModules_Click(object sender, EventArgs e)
        {
            try
            {
                mf.ModuleConfig.Send();
                mf.NetworkConfig.Send();
                mf.Tls.ShowMessage("Settings sent to module", "Config", 10000);

                HighlightUpdateButton(false);
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage("frmModuleConfig/btnSendToModule  " + ex.Message, "Help", 10000, true, true);
            }
        }

        private void butValves_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuValves";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuValves(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void butWifi_Click(object sender, EventArgs e)
        {
            cLastScreen = "frmMenuWifi";
            if (sender is Button button) HighlightButton(button);
            Form fs = Props.IsFormOpen(cLastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuWifi(mf, this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private bool CheckEdited()
        {
            bool Result = true;
            foreach (Form OwnedForm in this.OwnedForms)
            {
                if ((bool)OwnedForm.Tag)
                {
                    var Hlp = new frmMsgBox(mf, "Unsaved Changes, Confirm Exit?", "Exit", true);
                    Hlp.TopMost = true;

                    Hlp.ShowDialog();
                    bool Res = Hlp.Result;
                    Hlp.Close();
                    if (Res)
                    {
                        // move from form
                        OwnedForm.Close();
                    }
                    else
                    {
                        // don't move from form
                        Result = false;
                        break;
                    }
                }
            }
            return Result;
        }

        private bool ClosedOwned()
        {
            foreach (Form ownedForm in this.OwnedForms)
            {
                ownedForm.Close();
            }
            return !Convert.ToBoolean(OwnedForms.Length);    // check if all closed, could be unsaved data
        }

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            if (cLastScreen != "" && cLastScreen != null) Props.SetProp("LastScreen", cLastScreen);
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.Width = FormWidth;
            this.Height = FormHeight;
            StyleControls(this);
            butPowerOff.Left = 12;
            butPowerOff.Top = this.Height - 75;
            btnHelp.Left = 86;
            btnHelp.Top = this.Height - 75;
            butClose.Left = 160;
            butClose.Top = this.Height - 75;
            if (LoadLast) LoadLastScreen();
            SetLanguage();
            ShowProfile();

            Font ValFont = new Font(lbFileName.Font.FontFamily, 12, FontStyle.Regular);
            lbFileName.Font = ValFont;
            lbJob.Font = ValFont;
        }

        private void frmMenu_LocationChanged(object sender, EventArgs e)
        {
            MenuMoved?.Invoke(this, EventArgs.Empty);
        }

        private void frmMenu_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void frmMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void HighlightButton(Button btn)
        {
            // clear hightlights
            foreach (Control cntrl in this.Controls)
            {
                if (cntrl is Button but)
                {
                    but.FlatAppearance.BorderSize = 0;
                }
            }

            // highlight btn
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.Blue;
        }

        private void LoadLastScreen()
        {
            try
            {
                string Last = Props.GetProp("LastScreen");
                if (Props.IsFormNameValid(Last))
                {
                    Form fs;
                    switch (Last)
                    {
                        case "frmMenuProfiles":
                            butFile.PerformClick(); // frmMenuProfiles opened by default
                            HighlightButton(butProfiles);
                            break;

                        case "frmMenuJobs":
                            butFile.PerformClick();
                            fs = new frmMenuJobs(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butJobs);
                            fs.Show();
                            break;

                        case "frmMenuControl":
                            butProducts.PerformClick();
                            fs = new frmMenuControl(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butControl);
                            fs.Show();
                            break;

                        case "frmMenuSettings":
                            butProducts.PerformClick();
                            fs = new frmMenuSettings(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butSettings);
                            fs.Show();
                            break;

                        case "frmMenuMode":
                            butProducts.PerformClick();
                            fs = new frmMenuMode(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butMode);
                            fs.Show();
                            break;

                        case "frmMenuMonitoring":
                            butProducts.PerformClick();
                            fs = new frmMenuMonitoring(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butMonitor);
                            fs.Show();
                            break;

                        case "frmMenuData":
                            butProducts.PerformClick();
                            fs = new frmMenuData(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butData);
                            fs.Show();
                            break;

                        case "frmMenuRateMap":
                            butProducts.PerformClick();
                            fs = new frmMenuRateMap(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butMap);
                            fs.Show();
                            break;

                        case "frmMenuRateData":
                            butProducts.PerformClick();
                            fs = new frmMenuRateData(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butRateData);
                            fs.Show();
                            break;

                        case "frmMenuSections":
                            butMachine.PerformClick();  // frmMenuSections opened by default
                            HighlightButton(butSections);
                            break;

                        case "frmMenuRelays":
                            butMachine.PerformClick();
                            fs = new frmMenuRelays(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butRelays);
                            fs.Show();
                            break;

                        case "frmMenuPrimed":
                            butMachine.PerformClick();
                            fs = new frmMenuPrimed(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butPrimed);
                            fs.Show();
                            break;

                        case "frmMenuCalibrate":
                            butMachine.PerformClick();
                            fs = new frmMenuCalibrate(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butCalibrate);
                            fs.Show();
                            break;

                        case "frmMenuSwitches":
                            butMachine.PerformClick();
                            fs = new frmMenuSwitches(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butSwitches);
                            fs.Show();
                            break;

                        case "frmMenuNetwork":
                            butModules.PerformClick();  // frmMenuNetwork opened by default
                            HighlightButton(butNetwork);
                            break;

                        case "frmMenuConfig":
                            butModules.PerformClick();
                            fs = new frmMenuConfig(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butNetwork);
                            fs.Show();
                            break;

                        case "frmMenuPins":
                            butModules.PerformClick();
                            fs = new frmMenuPins(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butPins);
                            fs.Show();
                            break;

                        case "frmMenuRelayPins":
                            butModules.PerformClick();
                            fs = new frmMenuRelayPins(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butRelayPins);
                            fs.Show();
                            break;

                        case "frmMenuWifi":
                            butModules.PerformClick();
                            fs = new frmMenuWifi(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            fs.Show();
                            break;

                        case "frmMenuValves":
                            butModules.PerformClick();
                            fs = new frmMenuValves(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butValves);
                            fs.Show();
                            break;

                        case "frmMenuDisplay":
                            butOptions.PerformClick();  // frmMenuDisplay opened by default
                            HighlightButton(butDisplay);
                            break;

                        case "frmMenuPressure":
                            butMachine.PerformClick();
                            fs = new frmMenuPressure(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(btnPressure);
                            fs.Show();
                            break;

                        case "frmMenuLanguage":
                            butOptions.PerformClick();
                            fs = new frmMenuLanguage(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butLanguage);
                            fs.Show();
                            break;

                        case "frmMenuColor":
                            butOptions.PerformClick();
                            fs = new frmMenuColor(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            HighlightButton(butColor);
                            fs.Show();
                            break;

                        case "frmMenuHelp":
                            fs = new frmMenuHelp(mf, this);
                            fs.Owner = this;
                            cLastScreen = Last;
                            fs.Show();
                            break;

                        default:
                            // frmMenuRate
                            butProducts.PerformClick(); // frmMenuRate opened by default
                            HighlightButton(butRate);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenu/LoadLastScreen: " + ex.Message);
            }
        }

        private void SetLanguage()
        {
            butFile.Text = Lang.lgFile;
            butProducts.Text = Lang.lgProductsMenu;
            butMachine.Text = Lang.lgMachine;
            butModules.Text = Lang.lgModules;
            butOptions.Text = Lang.lgOptions;
            butHelpScreen.Text = Lang.lgHelp;

            butJobs.Text = Lang.lgJobs;
            butProfiles.Text = Lang.lgProfiles;
            butRate.Text = Lang.lgRate;
            butControl.Text = Lang.lgControl;
            butSettings.Text = Lang.lgSettings;
            butMode.Text = Lang.lgMode;
            butMonitor.Text = Lang.lgMonitoring;
            butData.Text = Lang.lgData;
            butMap.Text = Lang.lgRateMap;
            butRateData.Text = Lang.lgRateData;

            butSections.Text = Lang.lgSections;
            butRelays.Text = Lang.lgRelays;
            butSwitches.Text = Lang.lgSwitches;
            butPrimed.Text = Lang.lgPrimedStart;
            butCalibrate.Text = Lang.lgCalibrate;
            butBoards.Text = Lang.lgBoards;
            butNetwork.Text = Lang.lgNetwork;
            butConfig.Text = Lang.lgConfig;
            butPins.Text = Lang.lgPins;
            butRelayPins.Text = Lang.lgRelayPins;
            butWifi.Text = Lang.lgWifi;
            butValves.Text = Lang.lgValves;
            butUpdateModules.Text = Lang.lgSend;
            butDisplay.Text = Lang.lgDisplay;
            butLanguage.Text = Lang.lgLanguage;
            butColor.Text = Lang.lgColor;
            btnPressure.Text = Lang.lgPressure;
        }
    }
}