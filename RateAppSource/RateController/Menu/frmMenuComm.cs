using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Menu
{
    public partial class frmMenuComm : Form
    {
        private bool cEdited;
        private bool Initializing = false;
        private frmMenu MainMenu;

        public frmMenuComm(frmMenu menu)
        {
            InitializeComponent();
            MainMenu = menu;
            this.Tag = false;
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            SetButtons(false);
            UpdateForm();
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            SetButtons(false);
            UpdateForm();
        }

        private void frmMenuComm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Props.SaveFormLocation(this);
        }

        private void frmMenuComm_Load(object sender, System.EventArgs e)
        {
            SubMenuLayout.SetFormLayout(this, MainMenu, btnOK);

            btnCancel.Left = btnOK.Left - SubMenuLayout.ButtonSpacing;
            btnCancel.Top = btnOK.Top;
            MainMenu.StyleControls(this);
            SetLanguage();
            MainMenu.MenuMoved += MainMenu_MenuMoved;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            PositionForm();
            UpdateForm();
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

            //ckIsoBus.Checked = Props.CanEnabled;

            //switch (Props.CurrentCanDriver)
            //{
            //    case CanDriver.InnoMaker:
            //        rbAdapter2.Checked = true;
            //        break;

            //    case CanDriver.PCAN:
            //        rbAdapter3.Checked = true;
            //        break;

            //    default:
            //        rbAdapter1.Checked = true;  // SLCAN
            //        break;
            //}

            //ckDiagnostics.Checked = Props.ShowCanDiagnostics;

            //RefreshComPorts();
            //UpdatePortVisibility();

            //SetBoxes();

            //gbxDrivers.Enabled = !ckIsoBus.Checked;
            //ckDiagnostics.Enabled = !ckIsoBus.Checked;

            Initializing = false;
        }

        private void gbEthernet_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);

        }
    }
}