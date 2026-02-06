using AgOpenGPS;
using RateController.Classes;
using System;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRelayPins : Form
    {
        private System.Windows.Forms.TextBox[] Boxes;
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuRelayPins( frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
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
                Core.ModuleConfig.RelayPins(Pins);
                Core.ModuleConfig.Save();

                SetButtons(false);
                UpdateForm();
                MainMenu.HighlightUpdateButton();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }


        private void frmMenuRelayPins_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuRelayPins_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            MainMenu.ModuleDefaultsSet += MainMenu_ModuleDefaultsSet;
            this.BackColor = Properties.Settings.Default.MainBackColour;
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
        }

        private void UpdateForm()
        {
            Initializing = true;
            byte[] data = Core.ModuleConfig.GetData();
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

        private void btnRescan_Click(object sender, EventArgs e)
        {
            tbRelay1.Text = "-";
            tbRelay2.Text = "-";
            tbRelay3.Text = "-";
            tbRelay4.Text = "-";
            tbRelay5.Text = "-";
            tbRelay6.Text = "-";
            tbRelay7.Text = "-";
            tbRelay8.Text = "-";
            tbRelay9.Text = "-";
            tbRelay10.Text = "-";
            tbRelay11.Text = "-";
            tbRelay12.Text = "-";
            tbRelay13.Text = "-";
            tbRelay14.Text = "-";
            tbRelay15.Text = "-";
            tbRelay16.Text = "-";
        }
    }
}