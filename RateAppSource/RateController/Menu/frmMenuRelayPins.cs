using AgOpenGPS;
using System;
using System.Linq;
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

                for (int i = 0; i < 16; i++)
                {
                    if (byte.TryParse(Boxes[i].Text, out val))
                    {
                        Pins[i] = val;
                    }
                    else
                    {
                        Pins[i] = 255;
                    }
                }
                MainMenu.ModuleConfig3.SetRelayPins(Pins);
                MainMenu.ModuleConfig3.Save();
                MainMenu.ModuleConfig4.SetRelayPins(Pins);
                MainMenu.ModuleConfig4.Save();

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
        }

        private void UpdateForm()
        {
            Initializing = true;
            string[] Display = Enumerable.Repeat("-", 8).ToArray();

            byte[] data = MainMenu.ModuleConfig3.GetData();
            for (int i = 0; i < 8; i++)
            {
                if (data[i] < 60) Display[i] = data[i].ToString();
            }

            tbRelay1.Text = Display[0];
            tbRelay2.Text = Display[1];
            tbRelay3.Text = Display[2];
            tbRelay4.Text = Display[3];

            tbRelay5.Text = Display[4];
            tbRelay6.Text = Display[5];
            tbRelay7.Text = Display[6];
            tbRelay8.Text = Display[7];

            Display = Enumerable.Repeat("-", 8).ToArray();

            data = MainMenu.ModuleConfig4.GetData();
            for (int i = 0; i < 8; i++)
            {
                if (data[i] < 60) Display[i] = data[i].ToString();
            }

            tbRelay9.Text = Display[0];
            tbRelay10.Text = Display[1];
            tbRelay11.Text = Display[2];
            tbRelay12.Text = Display[3];

            tbRelay13.Text = Display[4];
            tbRelay14.Text = Display[5];
            tbRelay15.Text = Display[6];
            tbRelay16.Text = Display[7];

            Initializing = false;
        }
    }
}