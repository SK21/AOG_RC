using AgOpenGPS;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmCalibrate : Form
    {
        public FormStart mf;
        private clsCalibrates Cals;
        private bool FormEdited;
        private bool Initializing;
        private bool Running;

        public frmCalibrate(FormStart CalledFrom)
        {
            Initializing = true;
            InitializeComponent();

            #region // language

            this.Text = Lang.lgCalibrate;
            lbDescription.Text = Lang.lgCalProduct;
            lbPulses.Text = Lang.lgPulses;
            lbBaseRate.Text = Lang.lgBaseRate;
            lbCalFactor.Text = Lang.lgCalFactor;
            lbExpected.Text = Lang.lgExpectedAmount;
            lbMeasured.Text = Lang.lgMeasuredAmount;
            lbMeterSet.Text = Lang.lgMeterSet;
            lbCalSpeed.Text = Lang.lgCalSpeed;

            #endregion // language

            mf = CalledFrom;
            Cals = new clsCalibrates(mf);
            Cals.Edited += Cals_Edited;

            //mf.SimSpeed = 0;
        }

        private void btnCalStart_Click(object sender, EventArgs e)
        {
            mf.SimMode = SimType.Sim_Speed;
            Running = true;
            SetButtons();
            Cals.Running(true);
            //mf.SimSpeed = Speed;
            btnCalStop.Focus();
        }

        private void btnCalStop_Click(object sender, EventArgs e)
        {
            mf.SimMode = SimType.Sim_None;
            Running = false;
            SetButtons();
            Cals.Running(false);
            //mf.SimSpeed = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
            Cals.Reset();
        }

        private void btnHelp_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmCalHelp");

            if (fs == null)
            {
                Form frm = new frmCalHelp(mf);
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (!FormEdited)
                {
                    this.Close();
                }
                else
                {
                    if (mf.Tls.ReadOnly)
                    {
                        mf.Tls.ShowHelp("File is read only.", "Help", 5000, false, false, true);
                    }
                    else
                    {
                        // save data
                        if (double.TryParse(tbSpeed.Text, out double Tmp)) mf.SimSpeed = Tmp;

                        Cals.Save();
                        SetButtons(false);
                    }
                }
            }
            catch (Exception ex)
            {
                mf.Tls.ShowHelp(ex.Message, this.Text, 3000, true);
            }
        }

        private void Cals_Edited(object sender, EventArgs e)
        {
            SetButtons(true);
        }

        private void frmCalibrate_FormClosed(object sender, FormClosedEventArgs e)
        {
                mf.Tls.SaveFormData(this);
            mf.SimMode = SimType.Sim_None;

            Cals.Close();
        }

        private void frmCalibrate_Load(object sender, EventArgs e)
        {
            mf.Tls.LoadFormData(this);
            LoadCals();

            if (mf.UseInches)
            {
                lbSpeed.Text = "MPH";
            }
            else
            {
                lbSpeed.Text = "KMH";
            }

            //String Spd = mf.Tls.LoadProperty("CalSpeed");
            //double.TryParse(Spd, out double Tmp);
            //Speed = Tmp;

            foreach (Control c in this.Controls)
            {
                c.Tag = c.ForeColor;
            }

            SetDayMode();
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

        private void SetButtons(bool Edited = false)
        {
            if (!Initializing)
            {
                if (Edited)
                {
                    btnCancel.Enabled = true;
                    btnOK.Image = Properties.Resources.Save;
                    btnCalStart.Enabled = false;
                    btnCalStop.Enabled = false;
                }
                else
                {
                    btnCancel.Enabled = false;
                    btnOK.Image = Properties.Resources.OK;
                    btnCalStart.Enabled = !Running;
                    btnCalStop.Enabled = Running;
                    btnOK.Enabled = !Running;
                }
                tbSpeed.Enabled = !Running;
                FormEdited = Edited;
            }
        }

        private void SetDayMode()
        {
            if (Properties.Settings.Default.IsDay)
            {
                this.BackColor = Properties.Settings.Default.DayColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = (Color)c.Tag;
                }
            }
            else
            {
                this.BackColor = Properties.Settings.Default.NightColour;

                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                }
            }
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

        private void tbSpeed_Validating(object sender, System.ComponentModel.CancelEventArgs e)
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
            //tbSpeed.Text = Speed.ToString("N1");
            tbSpeed.Text = mf.SimSpeed.ToString("N1");
            Initializing = false;
        }
    }
}