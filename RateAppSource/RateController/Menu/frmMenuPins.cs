using AgOpenGPS;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
                mf.ModuleConfig.Momentary = ckMomentary.Checked;

                // flow
                if (byte.TryParse(tbFlow1.Text, out val))
                {
                    mf.ModuleConfig.Sensor0Flow = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor0Flow = 255;
                }
                if (byte.TryParse(tbFlow2.Text, out val))
                {
                    mf.ModuleConfig.Sensor1Flow = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor1Flow = 255;
                }

                // motor
                if (byte.TryParse(tbDir1.Text, out val))
                {
                    mf.ModuleConfig.Sensor0Dir = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor0Dir = 255;
                }
                if (byte.TryParse(tbDir2.Text, out val))
                {
                    mf.ModuleConfig.Sensor1Dir = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor1Dir = 255;
                }
                if (byte.TryParse(tbPWM1.Text, out val))
                {
                    mf.ModuleConfig.Sensor0PWM = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor0PWM = 255;
                }
                if (byte.TryParse(tbPWM2.Text, out val))
                {
                    mf.ModuleConfig.Sensor1PWM = val;
                }
                else
                {
                    mf.ModuleConfig.Sensor1PWM = 255;
                }

                // Work Pin
                if (byte.TryParse(tbWrk.Text, out val))
                {
                    mf.ModuleConfig.WorkPin = val;
                }
                else
                {
                    mf.ModuleConfig.WorkPin = 255;
                }

                // Pressure
                if (byte.TryParse(tbPressure.Text, out val))
                {
                    mf.ModuleConfig.PressurePin = val;
                }
                else
                {
                    mf.ModuleConfig.PressurePin = 255;
                }
                mf.ModuleConfig.Save();

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

        private void MainMenu_ModuleDefaultsSet(object sender, EventArgs e)
        {
            UpdateForm();
            SetButtons(false);
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
            lbWorkPin.Text = Lang.lgWorkPin;
            lbFlow1.Text = Lang.lgFlow + " 1";
            lbFlow2.Text = Lang.lgFlow + " 2";
            //ckMomentary.Text = Lang.

        }

        private void UpdateForm()
        {
            Initializing = true;
            byte[] data = mf.ModuleConfig.GetData();
            string[] display = new string[data.Length];
            for (int i = 7; i < 13; i++)
            {
                if (data[i] > 60)
                {
                    display[i] = "-";
                }
                else
                {
                    display[i] = data[i].ToString();
                }
            }

            ckMomentary.Checked = mf.ModuleConfig.Momentary;
            tbFlow1.Text = display[7].ToString();
            tbDir1.Text = display[8].ToString();
            tbPWM1.Text = display[9].ToString();
            tbFlow2.Text = display[10].ToString();
            tbDir2.Text = display[11].ToString();
            tbPWM2.Text = display[12].ToString();

            // work pin
            if (data[29] > 60)
            {
                tbWrk.Text = "-";
            }
            else
            {
                tbWrk.Text = data[29].ToString();
            }

            // pressure pin
            if (data[30] > 60)
            {
                tbPressure.Text = "-";
            }
            else
            {
                tbPressure.Text = data[30].ToString();
            }

            Initializing = false;
        }
    }
}