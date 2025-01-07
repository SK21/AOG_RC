using RateController.Menu;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
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
        private bool Expanded = false;
        private string LastScreen = "";
        private bool LoadLast = false;
        private FormStart mf;
        private MouseButtons MouseButtonClicked;
        private Point MouseDownLocation;
        private int cBoard;

        public frmMenu(FormStart cf, int ProductID, bool LoadLst = false)
        {
            InitializeComponent();
            this.mf = cf;
            ChangeProduct(ProductID);
            LoadLast = LoadLst;
        }
        public int Board {
            get { return cBoard; } 
            set
            {
                cBoard = value;
                mf.Tls.SaveProperty("DefaultBoard", cBoard.ToString());
            } 
        }

        public event EventHandler MenuMoved;

        public clsProduct CurrentProduct
        {
            get { return cCurrentProduct; }
        }

        public void ChangeProduct(int NewID, bool NoFans = false)
        {
            if (NewID < 0) NewID = 0;
            if (NewID > mf.MaxProducts - 1) NewID = mf.MaxProducts - 1;
            if (NoFans && NewID > mf.MaxProducts - 3) NewID = mf.MaxProducts - 3;
            cCurrentProduct = mf.Products.Item(NewID);
        }

        public void StyleControls(Control Parent)
        {
            if (Parent is Form frm)
            {
                frm.BackColor = Properties.Settings.Default.BackColour;
            }

            foreach (Control con in Parent.Controls)
            {
                if (con is Label ctl)
                {
                    ctl.ForeColor = Properties.Settings.Default.ForeColour;
                    ctl.BackColor = Properties.Settings.Default.BackColour;
                    ctl.Font = Properties.Settings.Default.MenuFontSmall;
                }

                if (con is Button but)
                {
                    but.ForeColor = Properties.Settings.Default.ForeColour;
                    but.BackColor = Properties.Settings.Default.BackColour;
                    but.FlatAppearance.MouseDownBackColor = Properties.Settings.Default.MouseDown;
                }

                if (con is ComboBox cbox)
                {
                    cbox.ForeColor = Properties.Settings.Default.ForeColour;
                    cbox.BackColor = Properties.Settings.Default.BackColour;
                    cbox.Font = Properties.Settings.Default.MenuFontSmall;
                }

                if (con is CheckBox cb)
                {
                    cb.ForeColor = Properties.Settings.Default.ForeColour;
                    cb.BackColor = Properties.Settings.Default.BackColour;
                    cb.FlatAppearance.CheckedBackColor = Color.LightGreen;
                }

                if (con.HasChildren) StyleControls(con);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Define the border color and thickness
            Color borderColor = Properties.Settings.Default.ForeColour;
            int borderWidth = 1;

            // Draw the border
            using (Pen pen = new Pen(borderColor, borderWidth))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
            }
        }

        private void butClose_Click(object sender, EventArgs e)
        {
            if (ClosedOwned()) this.Close();
        }

        private void butDiag_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butTuning.Visible = !Expanded;
                butEthernet.Visible = !Expanded;
                butActivity.Visible = !Expanded;
                butError.Visible = !Expanded;
                butHelp.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
                    butDiag.Top = (int)butDiag.Tag;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = false;
                    butProducts.Visible = false;
                    butMachine.Visible = false;
                    butModules.Visible = false;
                    butOptions.Visible = false;
                    butDiag.Visible = true;
                    butDiag.Tag = butDiag.Top;
                    butDiag.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butTuning.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butTuning.Top = Pos;

                    butEthernet.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butEthernet.Top = Pos;

                    butActivity.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butActivity.Top = Pos;

                    butError.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butError.Top = Pos;

                    butHelp.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butHelp.Top = Pos;
                }
            }
        }

        private void butFile_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
                    butNew.Visible = false;
                    butOpen.Visible = false;
                    butSaveAs.Visible = false;
                }
                else
                {
                    Expanded = true;
                    butFile.Visible = true;
                    butProducts.Visible = false;
                    butMachine.Visible = false;
                    butModules.Visible = false;
                    butOptions.Visible = false;
                    butDiag.Visible = false;

                    int Pos = butFile.Top;
                    butNew.Visible = true;
                    butNew.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butNew.Top = Pos;

                    butOpen.Visible = true;
                    butOpen.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butOpen.Top = Pos;

                    butSaveAs.Visible = true;
                    butSaveAs.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butSaveAs.Top = Pos;
                }
            }
        }

        private void butMachine_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butSections.Visible = !Expanded;
                butRelays.Visible = !Expanded;
                butCalibrate.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
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
                    butDiag.Visible = false;
                    butMachine.Tag = butMachine.Top;
                    butMachine.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butSections.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butSections.Top = Pos;

                    butRelays.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butRelays.Top = Pos;

                    butCalibrate.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butCalibrate.Top = Pos;
                }
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

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
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
                    butDiag.Visible = false;
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
                }
            }
        }

        private void butNew_Click(object sender, EventArgs e)
        {
            mf.NewFile();
        }

        private void butOptions_Click(object sender, EventArgs e)
        {
            if (ClosedOwned())
            {
                butDisplay.Visible = !Expanded;
                butPrimed.Visible = !Expanded;
                butSwitches.Visible = !Expanded;
                butLanguage.Visible = !Expanded;
                butOther.Visible = !Expanded;

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
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
                    butDiag.Visible = false;
                    butOptions.Tag = butOptions.Top;
                    butOptions.Top = butFile.Top;

                    int Pos = butFile.Top;
                    butDisplay.Left = butFile.Left + SubOffset;
                    Pos += SubFirstSpacing;
                    butDisplay.Top = Pos;

                    butPrimed.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butPrimed.Top = Pos;

                    butSwitches.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butSwitches.Top = Pos;

                    butLanguage.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butLanguage.Top = Pos;

                    butOther.Left = butFile.Left + SubOffset;
                    Pos += SubSpacing;
                    butOther.Top = Pos;
                }
            }
        }

        private void butPowerOff_Click(object sender, EventArgs e)
        {
            if (ClosedOwned()) mf.Close();
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

                if (Expanded)
                {
                    Expanded = false;
                    butFile.Visible = true;
                    butProducts.Visible = true;
                    butMachine.Visible = true;
                    butModules.Visible = true;
                    butOptions.Visible = true;
                    butDiag.Visible = true;
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
                    butDiag.Visible = false;
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

                    butRate.PerformClick();
                }
            }
        }

        private void butRate_Click(object sender, EventArgs e)
        {
            if (CheckEdited())
            {
                LastScreen = "frmMenuRate";
                Form fs = mf.Tls.IsFormOpen(LastScreen);

                if (fs == null)
                {
                    Form frm = new frmMenuRate(mf, this);
                    frm.Owner = this;
                    frm.Text = "Opened";
                    frm.Show();
                }
                else
                {
                    fs.Text = "Focused";
                    fs.Focus();
                }
            }
        }

        private bool ClosedOwned()
        {
            foreach (Form ownedForm in this.OwnedForms)
            {
                ownedForm.Close();
            }
            return !Convert.ToBoolean(OwnedForms.Length);    // check if all closed, could be unsaved data
        }
        private bool CheckEdited()
        {
            bool Result = true;
            foreach(Form OwnedForm in this.OwnedForms)
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

        private void frmMenu_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            if (LastScreen != "" && LastScreen != null) mf.Tls.SaveProperty("LastScreen", LastScreen);
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            this.Width = FormWidth;
            this.Height = FormHeight;
            StyleControls(this);
            butPowerOff.Left = 12;
            butPowerOff.Top = this.Height - 100;
            btnHelp.Left = 160;
            btnHelp.Top = butPowerOff.Top + 3;
            butClose.Left = btnHelp.Left;
            if (LoadLast) LoadLastScreen();
            if (int.TryParse(mf.Tls.LoadProperty("DefaultBoard"), out int value)) cBoard = value;
        }

        private void frmMenu_LocationChanged(object sender, EventArgs e)
        {
            MenuMoved?.Invoke(this, EventArgs.Empty);
        }

        private void frmMenu_MouseDown(object sender, MouseEventArgs e)
        {
            MouseButtonClicked = e.Button;
            if (e.Button == MouseButtons.Right) MouseDownLocation = e.Location;
        }

        private void frmMenu_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Location = new Point(this.Left + e.X - MouseDownLocation.X, this.Top + e.Y - MouseDownLocation.Y);
        }

        private void LoadLastScreen()
        {
            try
            {
                string Last = mf.Tls.LoadProperty("LastScreen");
                Debug.Print("Last screen: " + Last);
                if (mf.Tls.IsFormNameValid(Last))
                {
                    Form fs = mf.Tls.IsFormOpen(Last);

                    if (fs == null)
                    {
                        switch (Last)
                        {
                            case "frmMenuControl":
                                fs = new frmMenuControl(mf, this);
                                butProducts.PerformClick();
                                break;

                            case "frmMenuSettings":
                                fs=new frmMenuSettings(mf, this);
                                butProducts.PerformClick(); 
                                break;

                            case "frmMenuMode":
                                fs = new frmMenuMode(mf, this);
                                butProducts.PerformClick();
                                break;

                            case "frmMenuMonitoring":
                                fs = new frmMenuMonitoring(mf, this);
                                butProducts.PerformClick();
                                break;

                            case "frmMenuData":
                                fs = new frmMenuData(mf, this);
                                butProducts.PerformClick();
                                break;

                            case "frmMenuSections":
                                fs = new frmMenuSections(mf, this);
                                butMachine.PerformClick();
                                break;

                            case "frmMenuRelays":
                                fs = new frmMenuRelays(mf, this);
                                butMachine.PerformClick();
                                break;

                            case "frmMenuCalibrate":
                                fs = new frmMenuCalibrate(mf, this);
                                butMachine.PerformClick();
                                break;

                            case "frmMenuNetwork":
                                fs = new frmMenuNetwork(mf, this);
                                butModules.PerformClick();
                                break;

                            case "frmMenuConfig":
                                fs = new frmMenuConfig(mf, this);
                                butModules.PerformClick();
                                break;

                            default:
                                fs = new frmMenuRate(mf, this);
                                butProducts.PerformClick();
                                break;
                        }
                        fs.Owner = this;
                        LastScreen = Last;
                        fs.Show();
                    }
                    else
                    {
                        fs.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenu/LoadLastScree: " + ex.Message);
            }
        }

        private void butControl_Click(object sender, EventArgs e)
        {
            if (CheckEdited())
            {
                LastScreen = "frmMenuControl";
                Form fs = mf.Tls.IsFormOpen(LastScreen);

                if (fs == null)
                {
                    Form frm = new frmMenuControl(mf, this);
                    frm.Owner = this;
                    frm.Text = "Opened";
                    frm.Show();
                }
                else
                {
                    fs.Text = "Focused";
                    fs.Focus();
                }
            }
        }

        private void butSettings_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuSettings";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuSettings(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string FileName = exeDirectory + "Help\\" + LastScreen + ".pdf";
            try
            {
                Process.Start(new ProcessStartInfo { FileName = FileName, UseShellExecute = true });
            }
            catch (Exception ex)
            {

                mf.Tls.WriteErrorLog("frmMenu/bthHelp_Click: " + ex.Message);
            }
        }

        private void butMode_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuMode";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuMode(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butMonitor_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuMonitoring";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuMonitoring(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butData_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuData";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuData(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butOpen_Click(object sender, EventArgs e)
        {
            mf.OpenFile();
        }

        private void butSaveAs_Click(object sender, EventArgs e)
        {
            mf.SaveFileAs();
        }

        private void butSections_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuSections";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuSections(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butRelays_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuRelays";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuRelays(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butCalibrate_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuCalibrate";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuCalibrate(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butNetwork_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuNetwork";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuNetwork(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }

        private void butConfig_Click(object sender, EventArgs e)
        {
            LastScreen = "frmMenuConfig";
            Form fs = mf.Tls.IsFormOpen(LastScreen);

            if (fs == null)
            {
                Form frm = new frmMenuConfig(mf, this);
                frm.Owner = this;
                frm.Text = "Opened";
                frm.Show();
            }
            else
            {
                fs.Text = "Focused";
                fs.Focus();
            }
        }
    }
}