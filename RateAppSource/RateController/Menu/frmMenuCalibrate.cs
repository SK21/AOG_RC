using AgOpenGPS;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RateController.Menu
{
    public partial class frmMenuCalibrate : Form
    {
        private clsCalibrates Cals;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        private bool Running;

        public frmMenuCalibrate(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            Cals = new clsCalibrates(mf);
            Cals.Edited += Cals_Edited;
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            mf.SimMode = SimType.Sim_Speed;
            Running = true;
            SetButtons();
            Cals.Running(true);
            btnCalStop.Focus();
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            mf.SimMode = SimType.Sim_None;
            Running = false;
            SetButtons();
            Cals.Running(false);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            Cals.Reset();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (double.TryParse(tbSpeed.Text, out double Tmp)) mf.SimSpeed = Tmp;
                Cals.Save();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuCalibrate/btnOk_Click: " + ex.Message);
            }
        }

        private void Cals_Edited(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
            mf.SimMode = SimType.Sim_None;
            btnCalStop.PerformClick();
            Cals.Close();
        }

        private void frmMenuCalibrate_Load(object sender, EventArgs e)
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

            lbDescription.Font = new Font("Tahoma", 12);
            lbDescription.ForeColor = SystemColors.Highlight;
            lbPulses.Font = new Font("Tahoma", 12);
            lbPulses.ForeColor = SystemColors.Highlight;
            lbBaseRate.Font= new Font("Tahoma", 12);
            lbBaseRate.ForeColor = SystemColors.Highlight;
            lbCalFactor.Font= new Font("Tahoma", 12);
            lbCalFactor.ForeColor = SystemColors.Highlight;
            lbExpected.Font= new Font("Tahoma", 12);
            lbExpected.ForeColor = SystemColors.Highlight;
            lbMeasured.Font= new Font("Tahoma", 12);
            lbMeasured.ForeColor = SystemColors.Highlight;
            lbMeterSet.Font= new Font("Tahoma", 12);
            lbMeterSet.ForeColor = SystemColors.Highlight;

            PositionForm();

            LoadCals();

            if (mf.UseInches)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
            }

            foreach (Control c in this.Controls)
            {
                c.Tag = c.ForeColor;
            }

            Cals.Update();
            UpdateForm();
            SetButtons();
        }

        private void LoadCals()
        {
            Cals.Load();

            clsCalibrate Cal = Cals.Item(0);
            Cal.Description = lbName0;
            Cal.Power = btnPwr0;
            Cal.Pulses = lbPulses0;
            Cal.RateBox = tbRate0;
            Cal.CalFactorBox = tbFactor0;
            Cal.Expected = lbExpected0;
            Cal.Measured = tbMeasured0;
            Cal.Progress = pb0;
            Cal.Locked = btnSet0;

            Cal = Cals.Item(1);
            Cal.Description = lbName1;
            Cal.Power = btnPwr1;
            Cal.Pulses = lbPulses1;
            Cal.RateBox = tbRate1;
            Cal.CalFactorBox = tbFactor1;
            Cal.Expected = lbExpected1;
            Cal.Measured = tbMeasured1;
            Cal.Progress = pb1;
            Cal.Locked = btnSet1;

            Cal = Cals.Item(2);
            Cal.Description = lbName2;
            Cal.Power = btnPwr2;
            Cal.Pulses = lbPulses2;
            Cal.RateBox = tbRate2;
            Cal.CalFactorBox = tbFactor2;
            Cal.Expected = lbExpected2;
            Cal.Measured = tbMeasured2;
            Cal.Progress = pb2;
            Cal.Locked = btnSet2;

            Cal = Cals.Item(3);
            Cal.Description = lbName3;
            Cal.Power = btnPwr3;
            Cal.Pulses = lbPulses3;
            Cal.RateBox = tbRate3;
            Cal.CalFactorBox = tbFactor3;
            Cal.Expected = lbExpected3;
            Cal.Measured = tbMeasured3;
            Cal.Progress = pb3;
            Cal.Locked = btnSet3;
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

        private void SetButtons(bool Edited=false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    btnCalStart.Enabled = false;
                    btnCalStop.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnCalStart.Enabled = !Running;
                    btnCalStop.Enabled = Running;
                    //btnOK.Enabled = !Running;
                }

                tbSpeed.Enabled = !Running;
                cEdited = Edited;
                this.Tag = cEdited;
            }
        }

        private void SetLanguage()
        {
            this.Text = Lang.lgCalibrate;
            lbDescription.Text = Lang.lgCalProduct;
            lbPulses.Text = Lang.lgPulses;
            lbBaseRate.Text = Lang.lgBaseRate;
            lbCalFactor.Text = Lang.lgCalFactor;
            lbExpected.Text = Lang.lgExpectedAmount;
            lbMeasured.Text = Lang.lgMeasuredAmount;
            lbMeterSet.Text = Lang.lgMeterSet;
            lbCalSpeed.Text = Lang.lgCalSpeed;
        }

        private void tbSpeed_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            using (var form = new FormNumeric(0, 40, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbSpeed.Text = form.ReturnValue.ToString("N1");
                }
            }
        }

        private void tbSpeed_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void tbSpeed_Validating(object sender, CancelEventArgs e)
        {
            double tempD;
            double.TryParse(tbSpeed.Text, out tempD);
            if (tempD < 0 || tempD > 40)
            {
                System.Media.SystemSounds.Exclamation.Play();
                e.Cancel = true;
            }
        }

        private void UpdateForm()
        {
            Initializing = true;
            tbSpeed.Text = mf.SimSpeed.ToString("N1");
            Initializing = false;
        }
    }
}