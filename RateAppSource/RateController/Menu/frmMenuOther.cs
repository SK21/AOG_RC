using AgOpenGPS;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuOther : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuOther(FormStart main, frmMenu menu)
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
                if (double.TryParse(tbSimSpeed.Text, out double Speed)) mf.SimSpeed = Speed;
                mf.UseInches = !ckMetric.Checked;
                mf.SwitchBox.UseWorkSwitch = ckWorkSwitch.Checked;
                mf.MasterOverride = ckNoMaster.Checked;
                if (ckSimSpeed.Checked)
                {
                    mf.SimMode = SimType.Sim_Speed;
                }
                else
                {
                    mf.SimMode = SimType.Sim_None;
                }
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuOther/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuOther_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuOther_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            PositionForm();
            UpdateForm();
        }

        private void MainMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
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
            ckWorkSwitch.Text = Lang.lgWorkSwitch;
        }

        private void UpdateForm()
        {
            Initializing = true;

            ckMetric.Checked = !mf.UseInches;
            ckWorkSwitch.Checked = mf.SwitchBox.UseWorkSwitch;
            ckSimSpeed.Checked = (mf.SimMode == SimType.Sim_Speed);
            ckNoMaster.Checked = mf.MasterOverride;
            tbSimSpeed.Text = mf.SimSpeed.ToString("N1");
            if (mf.UseInches)
            {
                lbSimUnits.Text = "MPH";
            }
            else
            {
                lbSimUnits.Text = "KMH";
            }

            Initializing = false;
        }

        private void ckMetric_CheckedChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
    }
}