using AgOpenGPS;
using RateController.Language;
using System;
using System.Linq;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuPins : Form
    {
        private System.Windows.Forms.TextBox[] Boxes;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuPins(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            Boxes = new System.Windows.Forms.TextBox[] { tbFlow1, tbFlow2, tbDir1, tbDir2, tbPWM1, tbPWM2, tbWrk, tbPressure };

            for (int i = 0; i < 8; i++)
            {
                Boxes[i].Enter += Boxes_Enter;
                Boxes[i].TextChanged += Boxes_TextChanged;
            }
        }

        private void Boxes_Enter(object sender, EventArgs e)
        {
            System.Windows.Forms.TextBox bx = (System.Windows.Forms.TextBox)sender;
            double temp = 0;
            if (double.TryParse(bx.Text.Trim(), out double vl)) temp = vl;
            using (var form = new FormNumeric(0, 50, temp))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    bx.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void Boxes_TextChanged(object sender, EventArgs e)
        {
            SetButtons(true);
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
                byte val;
                MainMenu.ModuleConfig1.Momentary = ckMomentary.Checked;

                // flow
                if (byte.TryParse(tbFlow1.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor0Flow = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor0Flow = 255;
                }
                if (byte.TryParse(tbFlow2.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor1Flow = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor1Flow = 255;
                }

                // motor
                if (byte.TryParse(tbDir1.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor0Dir = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor0Dir = 255;
                }
                if (byte.TryParse(tbDir2.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor1Dir = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor1Dir = 255;
                }
                if (byte.TryParse(tbPWM1.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor0PWM = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor0PWM = 255;
                }
                if (byte.TryParse(tbPWM2.Text, out val))
                {
                    MainMenu.ModuleConfig2.Sensor1PWM = val;
                }
                else
                {
                    MainMenu.ModuleConfig2.Sensor1PWM = 255;
                }

                // Work Pin
                if (byte.TryParse(tbWrk.Text, out val))
                {
                    MainMenu.ModuleConfig1.WorkPin = val;
                }
                else
                {
                    MainMenu.ModuleConfig1.WorkPin = 255;
                }

                // Pressure
                if (byte.TryParse(tbPressure.Text, out val))
                {
                    MainMenu.ModuleConfig1.PressurePin = val;
                }
                else
                {
                    MainMenu.ModuleConfig1.PressurePin = 255;
                }
                MainMenu.ModuleConfig1.Save();
                MainMenu.ModuleConfig2.Save();

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuPins/btnOk_Click: " + ex.Message);
            }
        }

        private void frmMenuPins_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuPins_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
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

        private void MainMenu_ModuleDefaultsSet(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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
            lbWorkPin.Text = Lang.lgWorkPin;
            ckMomentary.Text = Lang.lgMomentary;
            lbPressure.Text = Lang.lgPressurePin;
        }

        private void UpdateForm()
        {
            Initializing = true;
            string[] Display = Enumerable.Repeat("-", 6).ToArray();

            byte[] data = MainMenu.ModuleConfig2.GetData();
            for (int i = 0; i < 6; i++)
            {
                if (data[i] < 60) Display[i] = data[i].ToString();
            }
            tbFlow1.Text = Display[0];
            tbDir1.Text = Display[1];
            tbPWM1.Text = Display[2];
            tbFlow2.Text = Display[3];
            tbDir2.Text = Display[4];
            tbPWM2.Text = Display[5];

            // work pin
            if (MainMenu.ModuleConfig1.WorkPin > 60)
            {
                tbWrk.Text = "-";
            }
            else
            {
                tbWrk.Text = MainMenu.ModuleConfig1.WorkPin.ToString();
            }

            // pressure pin
            if (MainMenu.ModuleConfig1.PressurePin > 60)
            {
                tbPressure.Text = "-";
            }
            else
            {
                tbPressure.Text = MainMenu.ModuleConfig1.PressurePin.ToString();
            }

            ckMomentary.Checked = MainMenu.ModuleConfig1.Momentary;

            Initializing = false;
        }
    }
}