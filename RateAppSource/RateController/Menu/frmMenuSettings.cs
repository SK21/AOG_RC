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
            btnHelp.Left = btnLeft.Left - 78;
            btnHelp.Top = btnOK.Top;
            PositionForm();
            MainMenu.StyleControls(this);
            lbProduct.Font = new Font(lbProduct.Font.FontFamily, 18, FontStyle.Underline);
            //UpdateForm();

        }
    }

}
