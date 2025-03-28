using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuColor : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;

        public frmMenuColor(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
            this.DoubleBuffered = true;
            this.Visible = false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UpdateForm();
            btnOK.Focus();
            SetButtons(false);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                // set colors
                if (rbColour1.Checked)
                {
                    Properties.Settings.Default.DisplayForeColour = tbColourDefault1.ForeColor;
                    Properties.Settings.Default.DisplayBackColour = tbColourDefault1.BackColor;
                }
                else if (rbColour2.Checked)
                {
                    Properties.Settings.Default.DisplayForeColour = tbColourDefault2.ForeColor;
                    Properties.Settings.Default.DisplayBackColour = tbColourDefault2.BackColor;
                }
                else
                {
                    Properties.Settings.Default.DisplayForeColour = tbColourUser1.ForeColor;
                    Properties.Settings.Default.DisplayBackColour = tbColourUser1.BackColor;
                }
                Properties.Settings.Default.DisplayForeColourUser = tbColourUser1.ForeColor;
                Properties.Settings.Default.DisplayBackColourUser = tbColourUser1.BackColor;
                Properties.Settings.Default.Save();
                mf.RaiseColorChanged();

                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuConfig/btnOk_Click: " + ex.Message);
            }
        }

        private void colorPanel_Click(object sender, EventArgs e)
        {
            SetButtons(true);
            rbColourUser.Checked = true;
        }

        private void colorPanel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(mf.Tls.ScreenBitmap, 0, 0, colorPanel.Width, colorPanel.Height);
        }

        private void ColorPanel_Touch(object sender, MouseEventArgs e)
        {
            try
            {
                int saturation = 1;

                if (e.Button == MouseButtons.Left)
                {
                    // Get color from touch position
                    float hue = (float)e.X / colorPanel.Width;
                    float brightness = 1 - (float)e.Y / colorPanel.Height;

                    if (e.Y < 10)
                    {
                        saturation = 0;
                    }
                    else
                    {
                        saturation = 1;
                    }

                    Color NewColor = mf.Tls.ColorFromHSV(hue * 360, saturation, brightness);
                    if (NewColor.A == 255 && NewColor.R == 255 && NewColor.G == 255 && NewColor.B == 255)
                    {
                        NewColor = Color.FromArgb(255, 255, 255, 254);
                    }

                    if (rbForeColor.Checked)
                    {
                        tbColourUser1.ForeColor = NewColor;
                    }
                    else
                    {
                        tbColourUser1.BackColor = NewColor;
                    }
                }
            }
            catch (Exception ex)
            {
                Props.WriteErrorLog("frmMenuColor/ColorPanel_Touch: " + ex.Message);
            }
        }

        private void frmMenuColor_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuColor_Load(object sender, EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            PositionForm();
            UpdateForm();
            this.Visible = true;
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

        private void rbColour1_Click(object sender, EventArgs e)
        {
            SetButtons(true);
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
            tbColourDefault1.Text = Lang.lgColor + " 1";
            tbColourDefault2.Text = Lang.lgColor + " 2";
            rbForeColor.Text = Lang.lgTextColor;
            rbBackColor.Text = Lang.lgBackColor;
        }

        private void tbColourDefault1_Click(object sender, EventArgs e)
        {
            rbColour1.Checked = true;
            SetButtons(true);
        }

        private void tbColourDefault2_Click(object sender, EventArgs e)
        {
            rbColour2.Checked = true;
            SetButtons(true);
        }

        private void tbColourUser1_Click(object sender, EventArgs e)
        {
            rbColourUser.Checked = true;
            SetButtons(true);
        }

        private void UpdateForm()
        {
            Initializing = true;

            // set colours
            tbColourUser1.ForeColor = Properties.Settings.Default.DisplayForeColourUser;
            tbColourUser1.BackColor = Properties.Settings.Default.DisplayBackColourUser;
            if (Properties.Settings.Default.DisplayForeColour == tbColourDefault1.ForeColor && Properties.Settings.Default.DisplayBackColour == tbColourDefault1.BackColor)
            {
                rbColour1.Checked = true;
            }
            else if (Properties.Settings.Default.DisplayForeColour == tbColourDefault2.ForeColor && Properties.Settings.Default.DisplayBackColour == tbColourDefault2.BackColor)
            {
                rbColour2.Checked = true;
            }
            else
            {
                rbColourUser.Checked = true;
            }

            Initializing = false;
        }
    }
}