using System;
using System.Windows.Forms;

namespace RateController.Controls
{
    public partial class Hbar : UserControl
    {
        private bool cHelpMode = false;
        private int cHoverTime;
        private bool Hovering = false;
        private Timer HoverTimer;
        private HScrollBar HSB;
        private int lc = 1;
        private int sc = 1;

        public Hbar()
        {
            InitializeComponent();
            cHoverTime = 500;
            HSB = hScrollBar1;
            HSB.Dock = DockStyle.Fill;
            HSB.Minimum = 0;
            HSB.Maximum = 100;
            HSB.SmallChange = 1;
            HSB.LargeChange = 1;
            HoverTimer = new Timer();
            HoverTimer.Tick += HoverTimer_Tick;
            HSB.MouseEnter += Hbar_MouseEnter;
            HSB.MouseLeave += Hbar_MouseLeave;
            HSB.ValueChanged += HSB_ValueChanged;
        }

        public event EventHandler Clicked;

        public event EventHandler ValueChanged;

        public bool HelpMode
        {
            get { return cHelpMode; }
            set
            {
                if (value)
                {
                    sc = HSB.SmallChange;
                    lc = HSB.LargeChange;
                    HSB.SmallChange = 0;
                    HSB.LargeChange = 0;
                }
                else
                {
                    if (sc > 0)
                    {
                        HSB.SmallChange = sc;
                    }
                    else
                    {
                        HSB.SmallChange = 1;
                    }

                    if (lc > 0)
                    {
                        HSB.LargeChange = lc;
                    }
                    else
                    {
                        HSB.LargeChange = 1;
                    }
                }
                cHelpMode = value;
            }
        }

        public int HoverTime
        {
            get { return cHoverTime; }
            set
            {
                if (value > 0 && value < 2000)
                {
                    cHoverTime = value;
                    HoverTimer.Interval = value;
                }
            }
        }

        public int LargeChange
        {
            get { return HSB.LargeChange; }
            set { HSB.LargeChange = value; }
        }

        public int SmallChange
        {
            get { return HSB.SmallChange; }
            set { HSB.SmallChange = value; }
        }

        public int Value
        {
            get { return HSB.Value; }
            set { HSB.Value = value; }
        }

        private void Hbar_MouseEnter(object sender, EventArgs e)
        {
            Hovering = true;
            HoverTimer.Start();
        }

        private void Hbar_MouseLeave(object sender, EventArgs e)
        {
            Hovering = false;
            HoverTimer.Stop();
        }

        private void HoverTimer_Tick(object sender, EventArgs e)
        {
            HoverTimer.Stop();
            if (Hovering)
            {
                Clicked?.Invoke(sender, EventArgs.Empty);
            }
        }

        private void HSB_ValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(sender, e);
        }
    }
}