using AgOpenGPS;
using RateController.Language;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRelayPins : Form
    {
        private System.Windows.Forms.TextBox[] Boxes;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuRelayPins(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;

            Boxes = new System.Windows.Forms.TextBox[] { tbRelay1,tbRelay2,tbRelay3,tbRelay4,tbRelay5,tbRelay6,tbRelay7,tbRelay8,
            tbRelay9,tbRelay10,tbRelay11,tbRelay12,tbRelay13,tbRelay14,tbRelay15,tbRelay16};

            for (int i = 0; i < 16; i++)
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
                byte[] Pins = new byte[16];

                for (int i=0;i<16;i++)
                {
                    if (byte.TryParse(Boxes[i].Text,out val))
                    {
                        Pins[i] = val;
                    }
                    else
                    {
                        Pins[i] = 255;
                    }
                }
                mf.ModuleConfig.RelayPins(Pins);
                mf.ModuleConfig.Save();

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }


        private void frmMenuRelayPins_FormClosed(object sender, FormClosedEventArgs e)
        {
            mf.Tls.SaveFormData(this);
        }

        private void frmMenuRelayPins_Load(object sender, EventArgs e)
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
        }

        private void UpdateForm()
        {
            Initializing = true;
            byte[] data = mf.ModuleConfig.GetData();
            string[] display = new string[data.Length];
            for (int i = 13; i < 29; i++)
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

            tbRelay1.Text = display[13].ToString();
            tbRelay2.Text = display[14].ToString();
            tbRelay3.Text = display[15].ToString();
            tbRelay4.Text = display[16].ToString();

            tbRelay5.Text = display[17].ToString();
            tbRelay6.Text = display[18].ToString();
            tbRelay7.Text = display[19].ToString();
            tbRelay8.Text = display[20].ToString();

            tbRelay9.Text = display[21].ToString();
            tbRelay10.Text = display[22].ToString();
            tbRelay11.Text = display[23].ToString();
            tbRelay12.Text = display[24].ToString();

            tbRelay13.Text = display[25].ToString();
            tbRelay14.Text = display[26].ToString();
            tbRelay15.Text = display[27].ToString();
            tbRelay16.Text = display[28].ToString();

            Initializing = false;
        }
    }
}