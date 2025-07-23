using AgOpenGPS;
using RateController.Classes;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController.Forms
{
    public partial class frmVersion : Form
    {
        private FormStart mf;
        private clsVersionChecker VC;

        public frmVersion(FormStart CallingForm)
        {
            InitializeComponent();
            mf = CallingForm;
            VC = new clsVersionChecker(mf);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnUpdate_Click(object sender, EventArgs e)
        {
            await VC.HasVersionChanged();
            mf.Tls.ShowMessage("Update request sent.", "Help", 5000);
        }

        private void frmVersion_FormClosed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
        }

        private void frmVersion_Load(object sender, EventArgs e)
        {
            Props.LoadFormLocation(this);
            this.BackColor = Properties.Settings.Default.MainBackColour;
            SetLanguage();
            UpdateForm();
            timer1.Enabled = true;
        }

        private void groupBox1_Paint(object sender, PaintEventArgs e)
        {
            mf.Tls.DrawGroupBox((GroupBox)sender, e.Graphics, this.BackColor, Color.Black, Color.Blue);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/AOG_RC/releases");
                linkLabel1.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
            }
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                VisitLink("https://github.com/SK21/PCBsetup");
                linkLabel4.LinkVisited = true;
            }
            catch (Exception ex)
            {
                mf.Tls.ShowMessage(ex.Message, "Help", 15000, true);
            }
        }

        private void SetLanguage()
        {
        }

        private void tbModuleID_Enter(object sender, EventArgs e)
        {
            double tempD;
            double.TryParse(tbModuleID.Text, out tempD);
            using (var form = new FormNumeric(0, Props.MaxModules, tempD))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    tbModuleID.Text = form.ReturnValue.ToString("N0");
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            lbAppCurrent.Text = Props.AppVersion();
            lbAppLatest.Text = VC.RCappLatest;

            int.TryParse(tbModuleID.Text, out int ModuleID);
            clsProduct Prod = mf.Products.Item(ModuleID);
            if (Prod.ModuleSending && mf.ModulesStatus.ModuleType((byte)ModuleID) > 0)
            {
                lbModule.Text = Enum.GetName(typeof(ModuleTypes), mf.ModulesStatus.ModuleType((byte)ModuleID));
                lbModuleCurrent.Text = mf.ModulesStatus.ModuleVersion((byte)ModuleID);
                lbModuleLatest.Text = VC.ModuleVersion(mf.ModulesStatus.ModuleType((byte)ModuleID));
            }
            else
            {
                lbModule.Text = "--";
                lbModuleCurrent.Text = "--";
                lbModuleLatest.Text = "--";
            }

            if (mf.SwitchBox.Connected())
            {
                lbSwitchBoxCurrent.Text = mf.SwitchBox.ModuleVersion();
                lbSwitchBoxLatest.Text = VC.ModuleVersion((int)ModuleTypes.Nano_SwitchBox);
            }
            else
            {
                lbSwitchBoxCurrent.Text = "--";
                lbSwitchBoxLatest.Text = "--";
            }
        }

        private void VisitLink(string Link)
        {
            System.Diagnostics.Process.Start(Link);
        }
    }
}