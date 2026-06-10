using RateController.Classes;
using RateController.Language;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RateController
{
    public partial class frmSwitches : Form
    {
        private static readonly SwIDs[] SwIdMap = {
            SwIDs.sw0, SwIDs.sw1, SwIDs.sw2,  SwIDs.sw3,
            SwIDs.sw4, SwIDs.sw5, SwIDs.sw6,  SwIDs.sw7,
            SwIDs.sw8, SwIDs.sw9, SwIDs.sw10, SwIDs.sw11,
            SwIDs.sw12,SwIDs.sw13,SwIDs.sw14, SwIDs.sw15
        };
        private Color ColorOff1 = Color.Silver;
        private Color ColorOff2 = Color.Silver;
        private Button[] _switchButtons = new Button[0];
        private int mouseX = 0;
        private int mouseY = 0;
        private int windowLeft = 0;
        private int windowTop = 0;

        public frmSwitches()
        {
            InitializeComponent();
        }

        public void SetDescriptions()
        {
            for (int i = 0; i < _switchButtons.Length; i++)
                _switchButtons[i].Text = (i + 1).ToString();
        }

        public void CreateSwitchButtons()
        {
            foreach (Button b in _switchButtons) Controls.Remove(b);

            int count = Props.SwitchCount;
            _switchButtons = new Button[count];

            const int btnW = 64, btnH = 46, startX = 12, startY = 68, spacingX = 78, spacingY = 57;

            for (int i = 0; i < count; i++)
            {
                Button btn = new Button();
                btn.Size = new Size(btnW, btnH);
                btn.Location = new Point(startX + (i % 4) * spacingX, startY + (i / 4) * spacingY);
                btn.Text = (i + 1).ToString();
                btn.Tag = i;
                btn.Click += SwitchButton_Click;
                _switchButtons[i] = btn;
                Controls.Add(btn);
            }

            int rows = count / 4;
            int autoY = startY + rows * spacingY + 11;
            btnAutoRate.Location = new Point(startX, autoY);
            btnAutoSection.Location = new Point(startX + 2 * spacingX, autoY);
            this.ClientSize = new Size(this.ClientSize.Width, autoY + btnH + 10);
        }

        private void SwitchButton_Click(object sender, EventArgs e)
        {
            int idx = (int)((Button)sender).Tag;
            if (idx < SwIdMap.Length) Core.vSwitchBox.PressSwitch(SwIdMap[idx]);
        }

        private void btnAutoRate_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoRate);
        }

        private void btnAutoSection_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PressSwitch(SwIDs.AutoSection);
        }

        private void btnDown_MouseDown(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateDownPressed();
        }

        private void btnDown_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateDownReleased();
        }

        private void btnMaster_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.MasterPressed();
        }

        private void btnMaster_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.MasterReleased();
        }

        private void btnPrime_Click(object sender, EventArgs e)
        {
            Core.vSwitchBox.PrimePressed();
        }

        private void btnUp_MouseDown(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateUpPressed();
        }

        private void btnUp_MouseUp(object sender, MouseEventArgs e)
        {
            Core.vSwitchBox.RateUpReleased();
        }

        private void Core_ProfileChanged(object sender, EventArgs e)
        {
            LoadSettings();
        }

        private void frmSwitches_Closed(object sender, FormClosedEventArgs e)
        {
            Props.SaveFormLocation(this);
            timer1.Enabled = false;
            Core.vSwitchBox.UsefrmSwitches = false;
            Core.SwitchBox.SwitchPGNreceived -= SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged -= Core_ProfileChanged;
        }

        private void frmSwitches_Load(object sender, EventArgs e)
        {
            Core.SwitchBox.SwitchPGNreceived += SwitchBox_SwitchPGNreceived;
            Core.ProfileChanged += Core_ProfileChanged;
            this.BackColor = Properties.Settings.Default.MainBackColour;

            Props.LoadFormLocation(this);
            CreateSwitchButtons();
            timer1.Enabled = true;
            Core.vSwitchBox.UsefrmSwitches = true;
            Core.vSwitchBox.PressSwitch(SwIDs.MasterOff);
            SetDescriptions();
            LoadSettings();
        }

        private void LoadSettings()
        {
            UpdateForm();
            SetLanguage();
        }

        private void mouseMove_MouseDown(object sender, MouseEventArgs e)
        {
            // Log the current window location and the mouse location.
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;
                mouseX = e.X;
                mouseY = e.Y;
            }
        }

        private void mouseMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                windowTop = this.Top;
                windowLeft = this.Left;

                Point pos = new Point(0, 0);

                pos.X = windowLeft + e.X - mouseX;
                pos.Y = windowTop + e.Y - mouseY;
                this.Location = pos;
            }
        }

        private void SetLanguage()
        {
            btnAutoRate.Text = Lang.lgAutoRate;
            btnAutoSection.Text = Lang.lgAutoSection;
        }

        private void SwitchBox_SwitchPGNreceived(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateForm();
        }

        private void UpdateForm()
        {
            btnPrime.Enabled = !Props.MasterMaintained && Props.Speed_KMH < 0.1;

            btnPrime.BackColor = Core.SectionControl.PrimeOn ? Color.LightGreen : ColorOff2;
            btnMaster.BackColor = Core.SwitchBox.MasterOn ? Color.LightGreen : ColorOff2;

            btnUp.BackColor = Core.SwitchBox.SwitchIsOn(SwIDs.RateUp) ? Color.LightGreen : this.TransparencyKey;
            btnDown.BackColor = Core.SwitchBox.SwitchIsOn(SwIDs.RateDown) ? Color.LightGreen : this.TransparencyKey;

            for (int i = 0; i < _switchButtons.Length; i++)
                _switchButtons[i].BackColor = Core.SwitchBox.SwitchIsOn(SwIdMap[i]) ? Color.LightGreen : ColorOff1;

            btnAutoSection.BackColor = Core.SwitchBox.AutoSectionOn ? Color.LightGreen : ColorOff1;
            btnAutoRate.BackColor = Core.SwitchBox.AutoRateOn ? Color.LightGreen : ColorOff1;
        }
    }
}