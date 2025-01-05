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
    public partial class frmMenuSettings : Form
    {
        private bool cEdited;
        private bool HelpMode = false;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        public frmMenuSettings(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
        }
        public bool Edited
        { get { return cEdited; } }
        private void PositionForm()
        {
            this.Top = MainMenu.Top + 30;
            this.Left = MainMenu.Left + 246;
        }

        private void frmMenuSettings_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            //SetLanguage();
            //MainMenu.MenuMoved += MyMenu_MenuMoved;
            mf.Tls.LoadFormData(this, "", false);
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MainMenu.Width - 260;
            this.Height = MainMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            //btnResetTank.Left = btnLeft.Left - 78;
            //btnResetTank.Top = btnOK.Top;
            //btnHelp.Left = btnResetTank.Left - 78;
            //btnHelp.Top = btnOK.Top;
            PositionForm();

            foreach (Control con in this.Controls)
            {
                if (con is Label ctl)
                {
                    ctl.ForeColor = Properties.Settings.Default.ForeColour;
                    ctl.BackColor = Properties.Settings.Default.BackColour;
                    ctl.Font = Properties.Settings.Default.MenuFontSmall;
                }
                if (con is Button but)
                {
                    but.ForeColor = Properties.Settings.Default.ForeColour;
                    but.BackColor = Properties.Settings.Default.BackColour;
                    but.FlatAppearance.MouseDownBackColor = Properties.Settings.Default.MouseDown;
                }
                if (con is Panel pnl)
                {
                    pnl.BackColor = Properties.Settings.Default.BackColour;
                    foreach (Control pcon in pnl.Controls)
                    {
                        if (pcon is Label pctl)
                        {
                            pctl.ForeColor = Properties.Settings.Default.ForeColour;
                            pctl.BackColor = Properties.Settings.Default.BackColour;
                            pctl.Font = Properties.Settings.Default.MenuFontSmall;
                        }

                        if (pcon is TextBox tb)
                        {
                            tb.ForeColor = Properties.Settings.Default.ForeColour;
                            tb.BackColor = Properties.Settings.Default.BackColour;
                            tb.Font = Properties.Settings.Default.MenuFontSmall;
                            tb.BorderStyle = BorderStyle.FixedSingle;
                        }

                        if (pcon is CheckBox cb)
                        {
                            cb.BackColor = Properties.Settings.Default.ForeColour;
                            cb.FlatAppearance.CheckedBackColor = Color.LightGreen;
                        }

                        //if (pcon is ComboBox cbox)
                        //{
                        //    cbox.ForeColor = Properties.Settings.Default.ForeColour;
                        //    cbox.BackColor = Properties.Settings.Default.BackColour;
                        //    cbox.Font = Properties.Settings.Default.MenuFontSmall;
                        //    cbox.DrawMode = DrawMode.OwnerDrawFixed;
                        //    cbox.DrawItem += ComboBox_DrawItem;
                        //}
                    }
                }
            }
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            //UpdateForm();

        }
    }

}
