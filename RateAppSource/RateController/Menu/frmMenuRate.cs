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
    public partial class frmMenuRate : Form
    {
        private frmMenu MyMenu;

        public frmMenuRate(frmMenu menu)
        {
            InitializeComponent();
            MyMenu = menu;
            MyMenu.MenuMoved += MyMenu_MenuMoved;
        }

        private void frmMenuRate_Load(object sender, EventArgs e)
        {
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = MyMenu.Width - 260;
            this.Height = MyMenu.Height - 50;
            btnOK.Left = this.Width - 84;
            btnOK.Top = this.Height - 84;
            btnCancel.Left = btnOK.Left - 78;
            btnCancel.Top = btnOK.Top;
            PositionForm();
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