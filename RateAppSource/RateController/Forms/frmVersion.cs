using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmVersion : Form
    {
        private int cModuleID;
        private clsVersionChecker VC;

        public frmVersion()
        {
            InitializeComponent();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            if (cModuleID > 0)
            {
                cModuleID--;
                UpdateForm();
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            if (cModuleID < Props.MaxModules)
            {
                cModuleID++;
                UpdateForm();
            }
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            await VC.HasVersionChanged();
            Props.ShowMessage("Updating.", "Help", 5000);
        }

        private void frmVersion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            VC = null;
            timer1.Enabled = false;
        }

        private void frmVersion_Load(object sender, EventArgs e)
        {
            VC = new clsVersionChecker();
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            cModuleID = Core.Products.Item(Props.CurrentProduct).ModuleID;
            SetLanguage();
            UpdateForm();
            timer1.Enabled = true;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            Props.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void SetLanguage()
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            lbAppCurrent.Text = Props.AppVersion();
            lbAppLatest.Text = VC.RCappLatest;
            lbMod.Text = cModuleID.ToString("N0");

            if (Core.ModulesStatus.Connected(cModuleID) && Core.ModulesStatus.ModuleType(cModuleID) > 0)
            {
                lbModule.Text = Enum.GetName(typeof(ModuleTypes), Core.ModulesStatus.ModuleType(cModuleID));
                lbModuleCurrent.Text = Core.ModulesStatus.ModuleVersion(cModuleID);
                lbModuleLatest.Text = VC.ModuleVersion(Core.ModulesStatus.ModuleType(cModuleID));
            }
            else
            {
                lbModule.Text = "--";
                lbModuleCurrent.Text = "--";
                lbModuleLatest.Text = "--";
            }

            if (Core.SwitchBox.Connected())
            {
                lbSwitchBoxCurrent.Text = Core.SwitchBox.ModuleVersion();
                lbSwitchBoxLatest.Text = VC.ModuleVersion((int)ModuleTypes.Nano_SwitchBox);
            }
            else
            {
                lbSwitchBoxCurrent.Text = "--";
                lbSwitchBoxLatest.Text = "--";
            }
        }
    }
}