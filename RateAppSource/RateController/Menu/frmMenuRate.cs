using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuRate : Form
    {
        private bool cEdited;
        private frmMenu MyMenu;

        public frmMenuRate(frmMenu menu)
        {
            InitializeComponent();
            MyMenu = menu;
            MyMenu.MenuMoved += MyMenu_MenuMoved;
        }

        public bool Edited
        { get { return cEdited; } }

        private void frmMenuRate_Load(object sender, EventArgs e)
        {
            // menu 800,600
            // sub menu 540,630
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MyMenu.Width - 260;
            this.Height = MyMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            btnRight.Left = btnCancel.Left - 78;
            btnRight.Top = btnOK.Top;
            btnLeft.Left = btnRight.Left - 78;
            btnLeft.Top = btnOK.Top;
            btnResetTank.Left = btnLeft.Left - 78;
            btnResetTank.Top = btnOK.Top;
            btnPIDloadDefaults.Left = btnResetTank.Left - 78;
            btnPIDloadDefaults.Top = btnOK.Top;
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
            }
        }

        private void MyMenu_MenuMoved(object sender, EventArgs e)
        {
            PositionForm();
        }

        private void PositionForm()
        {
            this.Top = MyMenu.Top + 36;
            this.Left = MyMenu.Left + 246;
        }
    }
}