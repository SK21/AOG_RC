using RateController.Menu;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmMenu : Form
    {
        public const int StartLeft = 100;
        public const int StartTop = 100;
        private const int FormHeight = 680;
        private const int FormWidth = 800;
        private const int SubFirstSpacing = 75;
        private const int SubOffset = 10;
        private const int SubSpacing = 55;
        private bool Expanded = false;
        private FormStart mf;

        public frmMenu(FormStart cf)
        {
            InitializeComponent();
            this.mf = cf;
        }

        public event EventHandler MenuMoved;

        private void butDiag_Click(object sender, EventArgs e)
        {
            CloseOwned();
            butTuning.Visible = !Expanded;
            butEthernet.Visible = !Expanded;
            butActivity.Visible = !Expanded;
            butError.Visible = !Expanded;
            butHelp.Visible = !Expanded;

            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butDiag.Top = (int)butDiag.Tag;
            }
            else
            {
                Expanded = true;
                butFile.Visible = false;
                butProducts.Visible = false;
                butMachine.Visible = false;
                butModules.Visible = false;
                butOptions.Visible = false;
                butDiag.Visible = true;
                butDiag.Tag = butDiag.Top;
                butDiag.Top = butFile.Top;

                int Pos = butFile.Top;
                butTuning.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butTuning.Top = Pos;

                butEthernet.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butEthernet.Top = Pos;

                butActivity.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butActivity.Top = Pos;

                butError.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butError.Top = Pos;

                butHelp.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butHelp.Top = Pos;
            }
        }

        private void butFile_Click(object sender, EventArgs e)
        {
            CloseOwned();
            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butNew.Visible = false;
                butOpen.Visible = false;
                butSaveAs.Visible = false;
            }
            else
            {
                Expanded = true;
                butFile.Visible = true;
                butProducts.Visible = false;
                butMachine.Visible = false;
                butModules.Visible = false;
                butOptions.Visible = false;
                butDiag.Visible = false;

                int Pos = butFile.Top;
                butNew.Visible = true;
                butNew.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butNew.Top = Pos;

                butOpen.Visible = true;
                butOpen.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butOpen.Top = Pos;

                butSaveAs.Visible = true;
                butSaveAs.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butSaveAs.Top = Pos;
            }
        }

        private void butMachine_Click(object sender, EventArgs e)
        {
            CloseOwned();
            butSections.Visible = !Expanded;
            butRelays.Visible = !Expanded;
            butCalibrate.Visible = !Expanded;

            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butMachine.Top = (int)butMachine.Tag;
            }
            else
            {
                Expanded = true;
                butFile.Visible = false;
                butProducts.Visible = false;
                butMachine.Visible = true;
                butModules.Visible = false;
                butOptions.Visible = false;
                butDiag.Visible = false;
                butMachine.Tag = butMachine.Top;
                butMachine.Top = butFile.Top;

                int Pos = butFile.Top;
                butSections.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butSections.Top = Pos;

                butRelays.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butRelays.Top = Pos;

                butCalibrate.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butCalibrate.Top = Pos;
            }
        }

        private void butModules_Click(object sender, EventArgs e)
        {
            CloseOwned();
            butNetwork.Visible = !Expanded;
            butBoards.Visible = !Expanded;
            butConfig.Visible = !Expanded;
            butPins.Visible = !Expanded;
            butRelayPins.Visible = !Expanded;
            butWifi.Visible = !Expanded;
            butValves.Visible = !Expanded;

            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butModules.Top = (int)butModules.Tag;
            }
            else
            {
                Expanded = true;
                butFile.Visible = false;
                butProducts.Visible = false;
                butMachine.Visible = false;
                butModules.Visible = true;
                butOptions.Visible = false;
                butDiag.Visible = false;
                butModules.Tag = butModules.Top;
                butModules.Top = butFile.Top;

                int Pos = butFile.Top;
                butNetwork.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butNetwork.Top = Pos;

                butBoards.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butBoards.Top = Pos;

                butConfig.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butConfig.Top = Pos;

                butPins.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butPins.Top = Pos;

                butRelayPins.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butRelayPins.Top = Pos;

                butWifi.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butWifi.Top = Pos;

                butValves.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butValves.Top = Pos;
            }
        }

        private void butOptions_Click(object sender, EventArgs e)
        {
            CloseOwned();
            butDisplay.Visible = !Expanded;
            butPrimed.Visible = !Expanded;
            butSwitches.Visible = !Expanded;
            butLanguage.Visible = !Expanded;
            butOther.Visible = !Expanded;

            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butOptions.Top = (int)butOptions.Tag;
            }
            else
            {
                Expanded = true;
                butFile.Visible = false;
                butProducts.Visible = false;
                butMachine.Visible = false;
                butModules.Visible = false;
                butOptions.Visible = true;
                butDiag.Visible = false;
                butOptions.Tag = butOptions.Top;
                butOptions.Top = butFile.Top;

                int Pos = butFile.Top;
                butDisplay.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butDisplay.Top = Pos;

                butPrimed.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butPrimed.Top = Pos;

                butSwitches.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butSwitches.Top = Pos;

                butLanguage.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butLanguage.Top = Pos;

                butOther.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butOther.Top = Pos;
            }
        }

        private void butProducts_Click(object sender, EventArgs e)
        {
            CloseOwned();
            butRate.Visible = !Expanded;
            butControl.Visible = !Expanded;
            butSettings.Visible = !Expanded;
            butMode.Visible = !Expanded;
            butMonitor.Visible = !Expanded;
            butData.Visible = !Expanded;

            if (Expanded)
            {
                Expanded = false;
                butFile.Visible = true;
                butProducts.Visible = true;
                butMachine.Visible = true;
                butModules.Visible = true;
                butOptions.Visible = true;
                butDiag.Visible = true;
                butProducts.Top = (int)butProducts.Tag;
            }
            else
            {
                Expanded = true;
                butFile.Visible = false;
                butProducts.Visible = true;
                butMachine.Visible = false;
                butModules.Visible = false;
                butOptions.Visible = false;
                butDiag.Visible = false;
                butProducts.Tag = butProducts.Top;
                butProducts.Top = butFile.Top;

                int Pos = butFile.Top;
                butRate.Left = butFile.Left + SubOffset;
                Pos += SubFirstSpacing;
                butRate.Top = Pos;

                butControl.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butControl.Top = Pos;

                butSettings.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butSettings.Top = Pos;

                butMode.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butMode.Top = Pos;

                butMonitor.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butMonitor.Top = Pos;

                butData.Left = butFile.Left + SubOffset;
                Pos += SubSpacing;
                butData.Top = Pos;
            }
        }

        private void butRate_Click(object sender, EventArgs e)
        {
            Form fs = mf.Tls.IsFormOpen("frmMenuRate");

            if (fs == null)
            {
                Form frm = new frmMenuRate(this);
                frm.Owner = this;
                frm.Show();
            }
            else
            {
                fs.Focus();
            }
        }

        private void CloseOwned()
        {
            foreach (Form ownedForm in this.OwnedForms)
            {
                ownedForm.Close();
            }
        }

        private void frmMenu_Load(object sender, EventArgs e)
        {
            this.BackColor = Properties.Settings.Default.BackColour;
            this.Width = FormWidth;
            this.Height = FormHeight;
            butPowerOff.Top = this.Height - 117;

            foreach (Control con in this.Controls)
            {
                if (con is Button but)
                {
                    but.ForeColor = Properties.Settings.Default.ForeColour;
                    but.BackColor = Properties.Settings.Default.BackColour;
                    but.FlatAppearance.MouseDownBackColor = Properties.Settings.Default.MouseDown;
                }
            }
        }

        private void frmMenu_LocationChanged(object sender, EventArgs e)
        {
            MenuMoved?.Invoke(this, EventArgs.Empty);
        }

        private void butPowerOff_Click(object sender, EventArgs e)
        {
            mf.Close();
        }
    }
}