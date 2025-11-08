using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuDisplay : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuDisplay(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (ckSimSpeed.Checked)
                {
                    Props.SimMode = SimType.Sim_Speed;
                }
                else
                {
                    Props.SimMode = SimType.Sim_None;
                }

                if (double.TryParse(tbSimSpeed.Text, out double Speed)) Props.SimSpeed = Speed;
                Props.UseMetric = ckMetric.Checked;
                Props.UseTransparent = ckTransparent.Checked;
                Props.UseLargeScreen = ckLargeScreen.Checked;
                Props.UseRateDisplay= ckRateDisplay.Checked;
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuDisplay/btnOk_Click: " + ex.Message);
            }
        }

        private void ckLargeScreen_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }



        private void frmMenuDisplay_FormClosed(object sender, FormClosedEventArgs e)
        {
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
            PositionForm();
            UpdateForm();
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
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
            ckLargeScreen.Text = Lang.lgLargeScreen;
            ckTransparent.Text = Lang.lgTransparent;
            ckMetric.Text = Lang.lgMetric;
            ckSimSpeed.Text = Lang.lgSimulateSpeed;
            ckRateDisplay.Text = Lang.lgCurrentRate;
        }

        private void tbSimSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSimSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSimSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
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

        private void UpdateForm()
        {
            Initializing = true;

            if (Props.UseMetric)
            {
                lbSimUnits.Text = "KMH";
            }
            else
            {
                lbSimUnits.Text = "MPH";
            }

            tbSimSpeed.Text = Props.SimSpeed.ToString("N1");
            ckSimSpeed.Checked = (Props.SimMode == SimType.Sim_Speed);
            ckMetric.Checked = Props.UseMetric;
            ckTransparent.Checked = Props.UseTransparent;
            ckLargeScreen.Checked = Props.UseLargeScreen;
            ckRateDisplay.Checked = Props.UseRateDisplay;

            Initializing = false;
        }
    }
}