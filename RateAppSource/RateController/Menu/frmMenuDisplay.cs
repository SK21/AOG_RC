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
    public partial class frmMenuDisplay : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;
        private FormStart mf;
        public frmMenuDisplay(FormStart main, frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            mf = main;
            this.Tag = false;
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
                mf.ModuleConfig.Is3Wire = rb3Wire.Checked;
                SetButtons(false);
                UpdateForm();
            }
            catch (Exception ex)
            {
                mf.Tls.WriteErrorLog("frmMenuDisplay/btnOk_Click: " + ex.Message);
            }
        }
    }
}
