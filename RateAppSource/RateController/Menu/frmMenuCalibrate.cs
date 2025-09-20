using AgOpenGPS;
using RateController.Classes;
using RateController.Language;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
        private SimType StartSim;

        public frmMenuCalibrate(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            StartSim=Props.SimMode;
            Cals = new clsCalibrates(mf);
            Cals.Edited += Cals_Edited;
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            // btnCalStop needs to be next in tab order
            // after btnCalStart to receive the focus
            Props.SimMode = SimType.Sim_Speed;
            Running = true;
            SetButtons();
            Props.RateCalibrationOn=true;
            Cals.Running(true);
        }
        private void btnCalStop_Click(object sender, EventArgs e)
        {
            Props.SimMode = StartSim;
            Running = false;
            SetButtons();
            Props.RateCalibrationOn = false;
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
                if (double.TryParse(tbSpeed.Text, out double Tmp)) Props.SimSpeed = Tmp;
                Cals.Save();
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuCalibrate/btnOk_Click: " + ex.Message);
            }
        }

        private void Cals_Edited(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmMenuCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            Props.SimMode = StartSim;
            btnCalStop.PerformClick();
            Cals.Close();
        }

        private void frmMenuCalibrate_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            SetLanguage();

            lbDescription.Font = new Font("Tahoma", 12);
            lbDescription.ForeColor = SystemColors.Highlight;
            lbPulses.Font = new Font("Tahoma", 12);
            lbPulses.ForeColor = SystemColors.Highlight;
            lbBaseRate.Font = new Font("Tahoma", 12);
            lbBaseRate.ForeColor = SystemColors.Highlight;
            lbCalFactor.Font = new Font("Tahoma", 12);
            lbCalFactor.ForeColor = SystemColors.Highlight;
            lbExpected.Font = new Font("Tahoma", 12);
            lbExpected.ForeColor = SystemColors.Highlight;
            lbMeasured.Font = new Font("Tahoma", 12);
            lbMeasured.ForeColor = SystemColors.Highlight;
            lbMeterSet.Font = new Font("Tahoma", 12);
            lbMeterSet.ForeColor = SystemColors.Highlight;

            PositionForm();

            if (!Props.UseMetric)
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

            LoadCals();
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
            Cal.PWMDisplay = lbPWM0;

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
            Cal.PWMDisplay = lbPWM1;

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
            Cal.PWMDisplay = lbPWM2;

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
            Cal.PWMDisplay = lbPWM3;
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

        private void SetButtons(bool Edited = false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Enabled = true;
                    btnCalStop.Enabled = false;
                    btnCalStart.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Enabled = false;
                    btnCalStop.Enabled = Running;
                    btnCalStart.Enabled = !Running;
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
            tbSpeed.Text = Props.SimSpeed.ToString("N1");
            Initializing = false;
        }
    }
}